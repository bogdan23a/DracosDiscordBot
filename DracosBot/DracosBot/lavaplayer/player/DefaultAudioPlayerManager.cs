using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Generic;
using java.util.concurrent;
using java.util.concurrent.atomic;

namespace com.sedmelluq.discord.lavaplayer.player
{
    using AudioOutputHook = com.sedmelluq.discord.lavaplayer.player.hook.AudioOutputHook;
    using AudioOutputHookFactory = com.sedmelluq.discord.lavaplayer.player.hook.AudioOutputHookFactory;
    using RemoteAudioTrackExecutor = com.sedmelluq.discord.lavaplayer.remote.RemoteAudioTrackExecutor;
    using RemoteNodeManager = com.sedmelluq.discord.lavaplayer.remote.RemoteNodeManager;
    using RemoteNodeRegistry = com.sedmelluq.discord.lavaplayer.remote.RemoteNodeRegistry;
    using AudioSourceManager = com.sedmelluq.discord.lavaplayer.source.AudioSourceManager;
    using DaemonThreadFactory = com.sedmelluq.discord.lavaplayer.tools.DaemonThreadFactory;
    using DataFormatTools = com.sedmelluq.discord.lavaplayer.tools.DataFormatTools;
    using ExceptionTools = com.sedmelluq.discord.lavaplayer.tools.ExceptionTools;
    using ExecutorTools = com.sedmelluq.discord.lavaplayer.tools.ExecutorTools;
    using FriendlyException = com.sedmelluq.discord.lavaplayer.tools.FriendlyException;
    using OrderedExecutor = com.sedmelluq.discord.lavaplayer.tools.OrderedExecutor;
    using GarbageCollectionMonitor = com.sedmelluq.discord.lavaplayer.tools.GarbageCollectionMonitor;
    using HttpConfigurable = com.sedmelluq.discord.lavaplayer.tools.io.HttpConfigurable;
    using MessageInput = com.sedmelluq.discord.lavaplayer.tools.io.MessageInput;
    using MessageOutput = com.sedmelluq.discord.lavaplayer.tools.io.MessageOutput;
    using AudioItem = com.sedmelluq.discord.lavaplayer.track.AudioItem;
    using AudioPlaylist = com.sedmelluq.discord.lavaplayer.track.AudioPlaylist;
    using AudioReference = com.sedmelluq.discord.lavaplayer.track.AudioReference;
    using AudioTrack = com.sedmelluq.discord.lavaplayer.track.AudioTrack;
    using AudioTrackInfo = com.sedmelluq.discord.lavaplayer.track.AudioTrackInfo;
    using DecodedTrackHolder = com.sedmelluq.discord.lavaplayer.track.DecodedTrackHolder;
    using InternalAudioTrack = com.sedmelluq.discord.lavaplayer.track.InternalAudioTrack;
    using TrackStateListener = com.sedmelluq.discord.lavaplayer.track.TrackStateListener;
    using AudioTrackExecutor = com.sedmelluq.discord.lavaplayer.track.playback.AudioTrackExecutor;
    using LocalAudioTrackExecutor = com.sedmelluq.discord.lavaplayer.track.playback.LocalAudioTrackExecutor;
    using RequestConfig = org.apache.http.client.config.RequestConfig;
    using Logger = org.slf4j.Logger;
    using LoggerFactory = org.slf4j.LoggerFactory;


    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static com.sedmelluq.discord.lavaplayer.tools.FriendlyException.Severity.FAULT;
    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static com.sedmelluq.discord.lavaplayer.tools.FriendlyException.Severity.SUSPICIOUS;

    /// <summary>
    /// The default implementation of audio player manager.
    /// </summary>
    public class DefaultAudioPlayerManager : lavaplayer.player.AudioPlayerManager
    {
        private const int TRACK_INFO_VERSIONED = 1;
        private const int TRACK_INFO_VERSION = 2;

        private static readonly int DEFAULT_FRAME_BUFFER_DURATION = (int)TimeUnit.SECONDS.toMillis(5);
        private static readonly int DEFAULT_CLEANUP_THRESHOLD = (int)TimeUnit.MINUTES.toMillis(1);

        private const int MAXIMUM_LOAD_REDIRECTS = 5;
        private const int DEFAULT_LOADER_POOL_SIZE = 10;
        private const int LOADER_QUEUE_CAPACITY = 5000;

        private static readonly Logger log = LoggerFactory.getLogger(typeof(DefaultAudioPlayerManager));

        private readonly IList<AudioSourceManager> sourceManagers;
        private volatile System.Func<RequestConfig, RequestConfig> httpConfigurator;

        // Executors
        private readonly ExecutorService trackPlaybackExecutorService;
        private readonly ThreadPoolExecutor trackInfoExecutorService;
        private readonly ScheduledExecutorService scheduledExecutorService;
        private readonly OrderedExecutor orderedInfoExecutor;

        // Configuration
        private long trackStuckThreshold;
        private volatile AudioConfiguration configuration;
        private readonly AtomicLong cleanupThreshold;
        private volatile int frameBufferDuration;
        private volatile bool useSeekGhosting;
        private volatile AudioOutputHookFactory outputHookFactory;

        // Additional services
        private readonly RemoteNodeManager remoteNodeManager;
        private readonly GarbageCollectionMonitor garbageCollectionMonitor;
        private readonly AudioPlayerLifecycleManager lifecycleManager;

        /// <summary>
        /// Create a new instance
        /// </summary>
        //JAVA TO C# CONVERTER WARNING: The following constructor is declared outside of its associated class:
        //ORIGINAL LINE: public DefaultAudioPlayerManager()
        public DefaultAudioPlayerManager()
        {
            sourceManagers = new List<>();

            // Executors
            trackPlaybackExecutorService = new ThreadPoolExecutor(1, int.MaxValue, 10, TimeUnit.SECONDS, new SynchronousQueue(), new DaemonThreadFactory("playback"));
            trackInfoExecutorService = ExecutorTools.createEagerlyScalingExecutor(1, DEFAULT_LOADER_POOL_SIZE, TimeUnit.SECONDS.toMillis(30), LOADER_QUEUE_CAPACITY, new DaemonThreadFactory("info-loader"));
            scheduledExecutorService = Executors.newScheduledThreadPool(1, new DaemonThreadFactory("manager"));
            orderedInfoExecutor = new OrderedExecutor(trackInfoExecutorService);

            // Configuration
            trackStuckThreshold = TimeUnit.MILLISECONDS.toNanos(10000);
            configuration = new AudioConfiguration();
            cleanupThreshold = new AtomicLong(DEFAULT_CLEANUP_THRESHOLD);
            frameBufferDuration = DEFAULT_FRAME_BUFFER_DURATION;
            useSeekGhosting = true;
            outputHookFactory = null;

            // Additional services
            remoteNodeManager = new RemoteNodeManager(this);
            garbageCollectionMonitor = new GarbageCollectionMonitor(scheduledExecutorService);
            lifecycleManager = new AudioPlayerLifecycleManager(scheduledExecutorService, cleanupThreshold);
            lifecycleManager.initialise();
        }

        public override void shutdown()
        {
            remoteNodeManager.shutdown(true);
            garbageCollectionMonitor.disable();
            lifecycleManager.shutdown();

            foreach (AudioSourceManager sourceManager in sourceManagers)
            {
                sourceManager.shutdown();
            }

            ExecutorTools.shutdownExecutor(trackPlaybackExecutorService, "track playback");
            ExecutorTools.shutdownExecutor(trackInfoExecutorService, "track info");
            ExecutorTools.shutdownExecutor(scheduledExecutorService, "scheduled operations");
        }

        public override AudioOutputHookFactory OutputHookFactory
        {
            set
            {
                this.outputHookFactory = value;
            }
        }

        public override void useRemoteNodes(params string[] nodeAddresses)
        {
            if (nodeAddresses.Length > 0)
            {
                remoteNodeManager.initialise(nodeAddresses);
            }
            else
            {
                remoteNodeManager.shutdown(false);
            }
        }

        public override void enableGcMonitoring()
        {
            garbageCollectionMonitor.enable();
        }

        public override void registerSourceManager(AudioSourceManager sourceManager)
        {
            sourceManagers.add(sourceManager);

            if (sourceManager is HttpConfigurable)
            {
                Function<RequestConfig, RequestConfig> configurator = httpConfigurator;

                if (httpConfigurator != null)
                {
                    ((HttpConfigurable)sourceManager).configureRequests(configurator);
                }
            }
        }

        public override T source<T>(Type<T> klass) where T : AudioSourceManager
        {
            foreach (AudioSourceManager sourceManager in sourceManagers)
            {
                if (klass.IsAssignableFrom(sourceManager.GetType()))
                {
                    return (T)sourceManager;
                }
            }

            return null;
        }

        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: @Override public Future<Void> loadItem(final String identifier, final AudioLoadResultHandler resultHandler)
        public override Future<Void> loadItem(string identifier, AudioLoadResultHandler resultHandler)
        {
            try
            {
                return trackInfoExecutorService.submit(createItemLoader(identifier, resultHandler));
            }
            catch (RejectedExecutionException e)
            {
                return handleLoadRejected(identifier, resultHandler, e);
            }
        }
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: @Override public Future<Void> loadItemOrdered(Object orderingKey, final String identifier, final AudioLoadResultHandler resultHandler)
        public override Future<Void> loadItemOrdered(object orderingKey, string identifier, AudioLoadResultHandler resultHandler)
        {
            try
            {
                return orderedInfoExecutor.submit(orderingKey, createItemLoader(identifier, resultHandler));
            }
            catch (RejectedExecutionException e)
            {
                return handleLoadRejected(identifier, resultHandler, e);
            }
        }

        private Future<Void> handleLoadRejected(string identifier, AudioLoadResultHandler resultHandler, RejectedExecutionException e)
        {
            FriendlyException exception = new FriendlyException("Cannot queue loading a track, queue is full.", SUSPICIOUS, e);
            ExceptionTools.log(log, exception, "queueing item " + identifier);

            resultHandler.loadFailed(exception);

            return ExecutorTools.COMPLETED_VOID;
        }

        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: private Callable<Void> createItemLoader(final String identifier, final AudioLoadResultHandler resultHandler)
        private Callable<Void> createItemLoader(string identifier, AudioLoadResultHandler resultHandler)
        {
            return () =>
            {
                bool[] reported = new bool[1];

                try
                {
                    if (!checkSourcesForItem(new AudioReference(identifier, null), resultHandler, reported))
                    {
                        log.debug("No matches for track with identifier {}.", identifier);
                        resultHandler.noMatches();
                    }
                }
                catch (Exception throwable)
                {
                    if (reported[0])
                    {
                        log.warn("Load result handler for {} threw an exception", identifier, throwable);
                    }
                    else
                    {
                        dispatchItemLoadFailure(identifier, resultHandler, throwable);
                    }

                    ExceptionTools.rethrowErrors(throwable);
                }

                return null;
            };
        }

        private void dispatchItemLoadFailure(string identifier, AudioLoadResultHandler resultHandler, Exception throwable)
        {
            FriendlyException exception = ExceptionTools.wrapUnfriendlyExceptions("Something went wrong when looking up the track", FAULT, throwable);
            ExceptionTools.log(log, exception, "loading item " + identifier);

            resultHandler.loadFailed(exception);
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: @Override public void encodeTrack(MessageOutput stream, AudioTrack track) throws IOException
        public override void encodeTrack(MessageOutput stream, AudioTrack track)
        {
            DataOutput output = stream.startMessage();
            output.write(TRACK_INFO_VERSION);

            AudioTrackInfo trackInfo = track.Info;
            output.writeUTF(trackInfo.title);
            output.writeUTF(trackInfo.author);
            output.writeLong(trackInfo.length);
            output.writeUTF(trackInfo.identifier);
            output.writeBoolean(trackInfo.isStream);
            DataFormatTools.writeNullableText(output, trackInfo.uri);

            encodeTrackDetails(track, output);
            output.writeLong(track.Position);

            stream.commitMessage(TRACK_INFO_VERSIONED);
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: @Override public DecodedTrackHolder decodeTrack(MessageInput stream) throws IOException
        public override DecodedTrackHolder decodeTrack(MessageInput stream)
        {
            DataInput input = stream.nextMessage();
            if (input == null)
            {
                return null;
            }

            int version = (stream.MessageFlags & TRACK_INFO_VERSIONED) != 0 ? (input.readByte() & 0xFF) : 1;

            AudioTrackInfo trackInfo = new AudioTrackInfo(input.readUTF(), input.readUTF(), input.readLong(), input.readUTF(), input.readBoolean(), version >= 2 ? DataFormatTools.readNullableText(input) : null);
            AudioTrack track = decodeTrackDetails(trackInfo, input);
            long position = input.readLong();

            if (track != null)
            {
                track.Position = position;
            }

            stream.skipRemainingBytes();

            return new DecodedTrackHolder(track);
        }

        /// <summary>
        /// Encodes an audio track to a byte array. Does not include AudioTrackInfo in the buffer. </summary>
        /// <param name="track"> The track to encode </param>
        /// <returns> The bytes of the encoded data </returns>
        public virtual sbyte[] encodeTrackDetails(AudioTrack track)
        {
            try
            {
                ByteArrayOutputStream byteOutput = new ByteArrayOutputStream();
                DataOutput output = new DataOutputStream(byteOutput);

                encodeTrackDetails(track, output);

                return byteOutput.toByteArray();
            }
            catch (IOException e)
            {
                throw new Exception(e);
            }
        }
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: private void encodeTrackDetails(AudioTrack track, DataOutput output) throws IOException
        private void encodeTrackDetails(AudioTrack track, DataOutput output)
        {
            AudioSourceManager sourceManager = track.SourceManager;
            output.writeUTF(sourceManager.SourceName);
            sourceManager.encodeTrack(track, output);
        }

        /// <summary>
        /// Decodes an audio track from a byte array. </summary>
        /// <param name="trackInfo"> Track info for the track to decode </param>
        /// <param name="buffer"> Byte array containing the encoded track </param>
        /// <returns> Decoded audio track </returns>
        public virtual AudioTrack decodeTrackDetails(AudioTrackInfo trackInfo, sbyte[] buffer)
        {
            try
            {
                DataInput input = new DataInputStream(new ByteArrayInputStream(buffer));
                return decodeTrackDetails(trackInfo, input);
            }
            catch (IOException e)
            {
                throw new Exception(e);
            }
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: private AudioTrack decodeTrackDetails(AudioTrackInfo trackInfo, DataInput input) throws IOException
        private AudioTrack decodeTrackDetails(AudioTrackInfo trackInfo, DataInput input)
        {
            string sourceName = input.readUTF();

            foreach (AudioSourceManager sourceManager in sourceManagers)
            {
                if (sourceName.Equals(sourceManager.SourceName))
                {
                    return sourceManager.decodeTrack(trackInfo, input);
                }
            }

            return null;
        }

        /// <summary>
        /// Executes an audio track with the given player and volume. </summary>
        /// <param name="listener"> A listener for track state events </param>
        /// <param name="track"> The audio track to execute </param>
        /// <param name="configuration"> The audio configuration to use for executing </param>
        /// <param name="volumeLevel"> The mutable volume level to use </param>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: public void executeTrack(final TrackStateListener listener, InternalAudioTrack track, AudioConfiguration configuration, AtomicInteger volumeLevel)
        public virtual void executeTrack(TrackStateListener listener, InternalAudioTrack track, AudioConfiguration configuration, AtomicInteger volumeLevel)
        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final AudioTrackExecutor executor = createExecutorForTrack(track, configuration, volumeLevel);
            AudioTrackExecutor executor = createExecutorForTrack(track, configuration, volumeLevel);
            track.assignExecutor(executor, true);

            trackPlaybackExecutorService.execute(() => executor.execute(listener));
        }

        private AudioTrackExecutor createExecutorForTrack(InternalAudioTrack track, AudioConfiguration configuration, AtomicInteger volumeLevel)
        {
            AudioSourceManager sourceManager = track.SourceManager;

            if (remoteNodeManager.Enabled && sourceManager != null && sourceManager.isTrackEncodable(track))
            {
                return new RemoteAudioTrackExecutor(track, configuration, remoteNodeManager, volumeLevel);
            }
            else
            {
                AudioTrackExecutor customExecutor = track.createLocalExecutor(this);

                if (customExecutor != null)
                {
                    return customExecutor;
                }
                else
                {
                    return new LocalAudioTrackExecutor(track, configuration, volumeLevel, useSeekGhosting, frameBufferDuration);
                }
            }
        }

        public override AudioConfiguration Configuration
        {
            get
            {
                return configuration;
            }
        }

        public override bool UsingSeekGhosting
        {
            get
            {
                return useSeekGhosting;
            }
        }

        public override bool UseSeekGhosting
        {
            set
            {
                this.useSeekGhosting = value;
            }
        }

        public override int FrameBufferDuration
        {
            get
            {
                return frameBufferDuration;
            }
            set
            {
                this.frameBufferDuration = Math.Max(200, value);
            }
        }


        public override long TrackStuckThreshold
        {
            set
            {
                this.trackStuckThreshold = TimeUnit.MILLISECONDS.toNanos(value);
            }
        }

        public virtual long TrackStuckThresholdNanos
        {
            get
            {
                return trackStuckThreshold;
            }
        }
        public override long PlayerCleanupThreshold
        {
            set
            {
                this.cleanupThreshold.set(value);
            }
        }

        public override int ItemLoaderThreadPoolSize
        {
            set
            {
                trackInfoExecutorService.MaximumPoolSize = value;
            }
        }

        private bool checkSourcesForItem(AudioReference reference, AudioLoadResultHandler resultHandler, bool[] reported)
        {
            AudioReference currentReference = reference;

            for (int redirects = 0; redirects < MAXIMUM_LOAD_REDIRECTS && currentReference.identifier != null; redirects++)
            {
                AudioItem item = checkSourcesForItemOnce(currentReference, resultHandler, reported);
                if (item == null)
                {
                    return false;
                }
                else if (!(item is AudioReference))
                {
                    return true;
                }
                currentReference = (AudioReference)item;
            }

            return false;
        }

        private AudioItem checkSourcesForItemOnce(AudioReference reference, AudioLoadResultHandler resultHandler, bool[] reported)
        {
            foreach (AudioSourceManager sourceManager in sourceManagers)
            {
                AudioItem item = sourceManager.loadItem(this, reference);

                if (item != null)
                {
                    if (item is AudioTrack)
                    {
                        log.debug("Loaded a track with identifier {} using {}.", reference.identifier, sourceManager.GetType().Name);
                        reported[0] = true;
                        resultHandler.trackLoaded((AudioTrack)item);
                    }
                    else if (item is AudioPlaylist)
                    {
                        log.debug("Loaded a playlist with identifier {} using {}.", reference.identifier, sourceManager.GetType().Name);
                        reported[0] = true;
                        resultHandler.playlistLoaded((AudioPlaylist)item);
                    }
                    return item;
                }
            }

            return null;
        }

        public virtual ExecutorService Executor
        {
            get
            {
                return trackPlaybackExecutorService;
            }
        }

        public override AudioPlayer createPlayer()
        {
            AudioOutputHook outputHook = outputHookFactory != null ? outputHookFactory.createOutputHook() : null;
            AudioPlayer player = new AudioPlayer(this, outputHook);
            player.addListener(lifecycleManager);

            if (remoteNodeManager.Enabled)
            {
                player.addListener(remoteNodeManager);
            }

            return player;
        }

        public override RemoteNodeRegistry RemoteNodeRegistry
        {
            get
            {
                return remoteNodeManager;
            }
        }

        public override Function<RequestConfig, RequestConfig> HttpRequestConfigurator
        {
            set
            {
                this.httpConfigurator = value;

                if (value != null)
                {
                    foreach (AudioSourceManager sourceManager in sourceManagers)
                    {
                        if (sourceManager is HttpConfigurable)
                        {
                            ((HttpConfigurable)sourceManager).configureRequests(value);
                        }
                    }
                }
            }
        }
    }
}

