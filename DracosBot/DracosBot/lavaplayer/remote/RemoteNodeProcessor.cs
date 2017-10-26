using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AudioDataFormat = com.sedmelluq.discord.lavaplayer.format.AudioDataFormat;
using DefaultAudioPlayerManager = com.sedmelluq.discord.lavaplayer.player.DefaultAudioPlayerManager;
using NodeStatisticsMessage = com.sedmelluq.discord.lavaplayer.remote.message.NodeStatisticsMessage;
using RemoteMessage = com.sedmelluq.discord.lavaplayer.remote.message.RemoteMessage;
using RemoteMessageMapper = com.sedmelluq.discord.lavaplayer.remote.message.RemoteMessageMapper;
using TrackExceptionMessage = com.sedmelluq.discord.lavaplayer.remote.message.TrackExceptionMessage;
using TrackFrameDataMessage = com.sedmelluq.discord.lavaplayer.remote.message.TrackFrameDataMessage;
using TrackFrameRequestMessage = com.sedmelluq.discord.lavaplayer.remote.message.TrackFrameRequestMessage;
using TrackStartRequestMessage = com.sedmelluq.discord.lavaplayer.remote.message.TrackStartRequestMessage;
using TrackStartResponseMessage = com.sedmelluq.discord.lavaplayer.remote.message.TrackStartResponseMessage;
using TrackStoppedMessage = com.sedmelluq.discord.lavaplayer.remote.message.TrackStoppedMessage;
using ExceptionTools = com.sedmelluq.discord.lavaplayer.tools.ExceptionTools;
using FriendlyException = com.sedmelluq.discord.lavaplayer.tools.FriendlyException;
using RingBufferMath = com.sedmelluq.discord.lavaplayer.tools.RingBufferMath;
using HttpClientTools = com.sedmelluq.discord.lavaplayer.tools.io.HttpClientTools;
using HttpInterface = com.sedmelluq.discord.lavaplayer.tools.io.HttpInterface;
using HttpInterfaceManager = com.sedmelluq.discord.lavaplayer.tools.io.HttpInterfaceManager;
using SimpleHttpInterfaceManager = com.sedmelluq.discord.lavaplayer.tools.io.SimpleHttpInterfaceManager;
using AudioTrack = com.sedmelluq.discord.lavaplayer.track.AudioTrack;
using InternalAudioTrack = com.sedmelluq.discord.lavaplayer.track.InternalAudioTrack;
using AudioFrame = com.sedmelluq.discord.lavaplayer.track.playback.AudioFrame;
using AudioFrameBuffer = com.sedmelluq.discord.lavaplayer.track.playback.AudioFrameBuffer;
using AudioTrackExecutor = com.sedmelluq.discord.lavaplayer.track.playback.AudioTrackExecutor;
using IOUtils = org.apache.commons.io.IOUtils;
using CountingInputStream = org.apache.commons.io.input.CountingInputStream;
using RequestConfig = Apache.Http.Client.Config.RequestConfig;
using CloseableHttpResponse = Apache.Http.Client.Methods.CloseableHttpResponse;
using HttpPost = Apache.Http.Client.Methods.HttpPost;
using ByteArrayEntity = Apache.Http.Entity.ByteArrayEntity;
using HttpClientBuilder = Apache.Http.Impl.Client.HttpClientBuilder;
using Microsoft.Extensions.Logging;
using java.util.concurrent;
using System.Threading;
using Sharpen;


namespace com.sedmelluq.discord.lavaplayer.remote
{
    using Logger = ILogger;
    using LoggerFactory = ILoggerFactory;


    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static com.sedmelluq.discord.lavaplayer.tools.FriendlyException.Severity.SUSPICIOUS;

    /// <summary>
    /// Processes one remote node.
    /// </summary>
    public class RemoteNodeProcessor : RemoteNode, ThreadStart
    {
        private static readonly LoggerFactory factory;

        private static readonly Logger log = factory.CreateLogger<RemoteNodeProcessor>();

        private const int CONNECT_TIMEOUT = 1000;
        private const int SOCKET_TIMEOUT = 2000;
        private const int TRACK_KILL_THRESHOLD = 10000;
        private const int TICK_MINIMUM_INTERVAL = 500;
        private const int NODE_REQUEST_HISTORY = 200;

        private readonly DefaultAudioPlayerManager playerManager;
        private readonly string nodeAddress;
        private readonly java.util.concurrent.ScheduledThreadPoolExecutor scheduledExecutor;
        private readonly HttpInterfaceManager httpInterfaceManager;
        private readonly AbandonedTrackManager abandonedTrackManager;
        private readonly BlockingQueue<RemoteMessage> queuedMessages;
        private readonly ConcurrentMap<long?, RemoteAudioTrackExecutor> playingTracks;
        private readonly RemoteMessageMapper mapper;
        private readonly AtomicBoolean threadRunning;
        private readonly AtomicInteger connectionState;
        private readonly LinkedList<Tick> tickHistory;
        private volatile int aliveTickCounter;
        private volatile int requestTimingPenalty;
        private long lastAliveTime;
        private volatile NodeStatisticsMessage lastStatistics;
        private volatile bool closed;

        /// <param name="playerManager"> Audio player manager </param>
        /// <param name="nodeAddress"> Address of this node </param>
        /// <param name="scheduledExecutor"> Scheduler to use to schedule reconnects </param>
        /// <param name="httpInterfaceManager"> HTTP interface manager to use for communicating with node </param>
        /// <param name="abandonedTrackManager"> Abandoned track manager, where the playing tracks are sent if node goes offline </param>
        public RemoteNodeProcessor(DefaultAudioPlayerManager playerManager, string nodeAddress, java.util.concurrent.ScheduledThreadPoolExecutor scheduledExecutor, HttpInterfaceManager httpInterfaceManager, AbandonedTrackManager abandonedTrackManager)
        {

            this.playerManager = playerManager;
            this.nodeAddress = nodeAddress;
            this.scheduledExecutor = scheduledExecutor;
            this.httpInterfaceManager = httpInterfaceManager;
            this.abandonedTrackManager = abandonedTrackManager;
            queuedMessages = new LinkedBlockingQueue<>();
            playingTracks = new ConcurrentHashMap<>();
            mapper = new RemoteMessageMapper();
            threadRunning = new AtomicBoolean();
            connectionState = new AtomicInteger(ConnectionState.OFFLINE.id());
            tickHistory = new LinkedList<>(NODE_REQUEST_HISTORY);
            closed = false;
        }

        /// <returns> The address of this node. </returns>
        public virtual string NodeAddress
        {
            get
            {
                return nodeAddress;
            }
        }

        /// <summary>
        /// Start playing a track through this remote node. </summary>
        /// <param name="executor"> The executor of the track </param>
        public virtual void startPlaying(RemoteAudioTrackExecutor executor)
        {
            AudioTrack track = executor.Track;

            if (playingTracks.PutIfAbsent(executor.ExecutorId, executor) == null)
            {
                long position = executor.NextInputTimecode;
                log.LogInformation("Sending request to play {} {} from position {}", track.Identifier, executor.ExecutorId, position);

                queuedMessages.add(new TrackStartRequestMessage(executor.ExecutorId, track.Info, playerManager.encodeTrackDetails(track), executor.Volume, executor.Configuration, position));
            }
        }


        /// <summary>
        /// Clear the track from this node. </summary>
        /// <param name="executor"> Executor of the track </param>
        /// <param name="notifyNode"> Whether it is necessary to notify the node </param>
        public virtual void trackEnded(RemoteAudioTrackExecutor executor, bool notifyNode)
        {
            if (playingTracks.remove(executor.ExecutorId) != null)
            {
                log.LogInformation("Track {} removed from node {} (context {})", executor.Track.Identifier, nodeAddress, executor.ExecutorId);

                if (notifyNode)
                {
                    log.LogInformation("Notifying node {} of track stop for {} (context {})", nodeAddress, executor.Track.Identifier, executor.ExecutorId);

                    queuedMessages.add(new TrackStoppedMessage(executor.ExecutorId));
                }

                executor.detach();
            }
        }

        /// <summary>
        /// Mark this processor as shut down. No further tasks for it will be scheduled.
        /// </summary>
        public virtual void shutdown()
        {
            processHealthCheck(true);
            closed = true;
            scheduledExecutor.remove(this);
        }

        public void run()
        {
            if (closed || !threadRunning.compareAndSet(false, true))
            {
                log.LogDebug("Not running node processor for {}, thread already active.", nodeAddress);
                return;
            }

            log.debug("Trying to connect to node {}.", nodeAddress);

            connectionState.set(ConnectionState.PENDING.id());

            try
            {
                using (HttpInterface httpInterface = httpInterfaceManager.Interface)
                {
                    RingBufferMath timingAverage = new RingBufferMath(10, @in => Math.Pow(@in, 5.0), @out => Math.Pow(@out, 0.2));

                    while (processOneTick(httpInterface, timingAverage))
                    {
                        aliveTickCounter = Math.Max(1, aliveTickCounter + 1);
                        lastAliveTime = DateTimeHelperClass.CurrentUnixTimeMillis();
                    }
                }
            }
            catch (InterruptedException)
            {
                log.info("Node {} processing was stopped.", nodeAddress);
                Thread.CurrentThread.Interrupt();
            }
            catch (IOException e)
            {
                if (aliveTickCounter > 0)
                {
                    log.error("Node {} went offline with exception.", nodeAddress, e);
                }
                else
                {
                    log.debug("Retry, node {} is still offline.", nodeAddress);
                }
            }
            catch (Exception e)
            {
                log.error("Node {} appears offline due to unexpected exception.", nodeAddress, e);

                ExceptionTools.rethrowErrors(e);
            }
            finally
            {
                processHealthCheck(true);
                connectionState.set(ConnectionState.OFFLINE.id());

                aliveTickCounter = Math.Min(-1, aliveTickCounter - 1);
                threadRunning.set(false);

                if (!closed)
                {
                    long delay = ScheduleDelay;

                    if (aliveTickCounter == -1)
                    {
                        log.info("Node {} loop ended, retry scheduled in {}.", nodeAddress, delay);
                    }

                    scheduledExecutor.schedule(this, delay, TimeUnit.MILLISECONDS);
                }
                else
                {
                    log.info("Node {} loop ended, node was removed.", nodeAddress);
                }
            }
        }

        private long ScheduleDelay
        {
            get
            {
                if (aliveTickCounter >= -5)
                {
                    return 1000;
                }
                else if (aliveTickCounter >= -20)
                {
                    return 3000;
                }
                else
                {
                    return 10000;
                }
            }
        }

        //---------------------------------------------------------------------------------------------------------
        //	Copyright © 2007 - 2017 Tangible Software Solutions Inc.
        //	This class can be used by anyone provided that the copyright notice remains intact.
        //
        //	This class is used to replace calls to Java's System.currentTimeMillis with the C# equivalent.
        //	Unix time is defined as the number of seconds that have elapsed since midnight UTC, 1 January 1970.
        //---------------------------------------------------------------------------------------------------------
        internal static class DateTimeHelperClass
        {
            private static readonly System.DateTime Jan1st1970 = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
            internal static long CurrentUnixTimeMillis()
            {
                return (long)(System.DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
            }
        }
    using System;
using System.Collections.Generic;
using System.Threading;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private boolean processOneTick(HttpInterface httpInterface, RingBufferMath timingAverage) throws Exception
 private bool processOneTick(HttpInterface httpInterface, RingBufferMath timingAverage)
    {
        TickBuilder tickBuilder = new TickBuilder(DateTimeHelperClass.CurrentUnixTimeMillis());

        try
        {
            if (!dispatchOneTick(httpInterface, tickBuilder))
            {
                return false;
            }
        }
        finally
        {
            tickBuilder.endTime = DateTimeHelperClass.CurrentUnixTimeMillis();
            recordTick(tickBuilder.build(), timingAverage);
        }

        long sleepDuration = Math.Max((tickBuilder.startTime + 500) - tickBuilder.endTime, 10);

        Thread.Sleep(sleepDuration);
        return true;
    }

    //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
    //ORIGINAL LINE: private boolean dispatchOneTick(HttpInterface httpInterface, TickBuilder tickBuilder) throws Exception
    private bool dispatchOneTick(HttpInterface httpInterface, TickBuilder tickBuilder)
    {
        bool success = false;
        HttpPost post = new HttpPost("http://" + nodeAddress + "/tick");

        abandonedTrackManager.distribute(Collections.singletonList(this));

        ByteArrayEntity entity = new ByteArrayEntity(buildRequestBody());
        post.Entity = entity;

        tickBuilder.requestSize = (int)entity.ContentLength;

        CloseableHttpResponse response = httpInterface.execute(post);

        try
        {
            tickBuilder.responseCode = response.StatusLine.StatusCode;
            if (tickBuilder.responseCode != 200)
            {
                throw new IOException("Returned an unexpected response code " + tickBuilder.responseCode);
            }

            if (connectionState.compareAndSet(ConnectionState.PENDING.id(), ConnectionState.ONLINE.id()))
            {
                log.info("Node {} came online.", nodeAddress);
            }
            else if (connectionState.get() != ConnectionState.ONLINE.id())
            {
                log.warn("Node {} received successful response, but had already lost control of its tracks.", nodeAddress);
                return false;
            }

            lastAliveTime = DateTimeHelperClass.CurrentUnixTimeMillis();

            if (!handleResponseBody(response.Entity.Content, tickBuilder))
            {
                return false;
            }

            success = true;
        }
        finally
        {
            if (!success)
            {
                IOUtils.closeQuietly(response);
            }
            else
            {
                IOUtils.closeQuietly(response.Entity.Content);
            }
        }

        return true;
    }

    //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
    //ORIGINAL LINE: private byte[] buildRequestBody() throws IOException
    private sbyte[] buildRequestBody()
    {
        ByteArrayOutputStream outputBytes = new ByteArrayOutputStream();
        DataOutputStream output = new DataOutputStream(outputBytes);

        IList<RemoteMessage> messages = new List<RemoteMessage>();
        int queuedCount = queuedMessages.drainTo(messages);

        if (queuedCount > 0)
        {
            log.debug("Including {} queued messages in the request to {}.", queuedCount, nodeAddress);
        }

        foreach (RemoteAudioTrackExecutor executor in playingTracks.values())
        {
            long pendingSeek = executor.PendingSeek;

            AudioFrameBuffer buffer = executor.AudioBuffer;
            int neededFrames = pendingSeek == -1 ? buffer.RemainingCapacity : buffer.FullCapacity;

            messages.Add(new TrackFrameRequestMessage(executor.ExecutorId, neededFrames, executor.Volume, pendingSeek));
        }

        foreach (RemoteMessage message in messages)
        {
            mapper.encode(output, message);
        }

        mapper.endOutput(output);
        return outputBytes.toByteArray();
    }

    //---------------------------------------------------------------------------------------------------------
    //	Copyright © 2007 - 2017 Tangible Software Solutions Inc.
    //	This class can be used by anyone provided that the copyright notice remains intact.
    //
    //	This class is used to replace calls to Java's System.currentTimeMillis with the C# equivalent.
    //	Unix time is defined as the number of seconds that have elapsed since midnight UTC, 1 January 1970.
    //---------------------------------------------------------------------------------------------------------
    internal static class DateTimeHelperClass
    {
        private static readonly System.DateTime Jan1st1970 = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        internal static long CurrentUnixTimeMillis()
        {
            return (long)(System.DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
        }
    }

    private bool handleResponseBody(InputStream inputStream, TickBuilder tickBuilder)
    {
        CountingInputStream countingStream = new CountingInputStream(inputStream);
        DataInputStream input = new DataInputStream(countingStream);
        RemoteMessage message;

        try
        {
            while ((message = mapper.decode(input)) != null)
            {
                if (message is TrackStartResponseMessage)
                {
                    handleTrackStartResponse((TrackStartResponseMessage)message);
                }
                else if (message is TrackFrameDataMessage)
                {
                    handleTrackFrameData((TrackFrameDataMessage)message);
                }
                else if (message is TrackExceptionMessage)
                {
                    handleTrackException((TrackExceptionMessage)message);
                }
                else if (message is NodeStatisticsMessage)
                {
                    handleNodeStatistics((NodeStatisticsMessage)message);
                }
            }
        }
        catch (InterruptedException)
        {
            log.error("Node {} processing thread was interrupted.", nodeAddress);
            Thread.CurrentThread.Interrupt();
            return false;
        }
        catch (Exception e)
        {
            log.error("Error when processing response from node {}.", nodeAddress, e);
            ExceptionTools.rethrowErrors(e);
        }
        finally
        {
            tickBuilder.responseSize = countingStream.Count;
        }

        return true;
    }

    private void handleTrackStartResponse(TrackStartResponseMessage message)
    {
        if (message.success)
        {
            log.debug("Successful start confirmation from node {} for executor {}.", nodeAddress, message.executorId);
        }
        else
        {
            RemoteAudioTrackExecutor executor = playingTracks.get(message.executorId);

            if (executor != null)
            {
                executor.dispatchException(new FriendlyException("Remote machine failed to start track: " + message.failureReason, SUSPICIOUS, null));
                executor.stop();
            }
            else
            {
                log.debug("Received failed track start for an already stopped executor {} from node {}.", message.executorId, nodeAddress);
            }
        }
    }

    //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
    //ORIGINAL LINE: private void handleTrackFrameData(TrackFrameDataMessage message) throws Exception
    private void handleTrackFrameData(TrackFrameDataMessage message)
    {
        RemoteAudioTrackExecutor executor = playingTracks.get(message.executorId);

        if (executor != null)
        {
            if (message.seekedPosition >= 0)
            {
                executor.clearSeek(message.seekedPosition);
            }

            AudioFrameBuffer buffer = executor.AudioBuffer;
            executor.receivedData();

            AudioDataFormat format = executor.Configuration.OutputFormat;

            foreach (AudioFrame frame in message.frames)
            {
                buffer.consume(new AudioFrame(frame.timecode, frame.data, frame.volume, format));
            }

            if (message.finished)
            {
                buffer.setTerminateOnEmpty();
                trackEnded(executor, false);
            }
        }
    }

    private void handleTrackException(TrackExceptionMessage message)
    {
        RemoteAudioTrackExecutor executor = playingTracks.get(message.executorId);

        if (executor != null)
        {
            executor.dispatchException(message.exception);
        }
    }

    private void handleNodeStatistics(NodeStatisticsMessage message)
    {
        log.trace("Received stats from node: {} {} {} {}", message.playingTrackCount, message.totalTrackCount, message.processCpuUsage, message.systemCpuUsage);

        lastStatistics = message;
    }

    /// <returns> An HTTP interface manager with appropriate timeouts for node requests. </returns>
    public static HttpInterfaceManager createHttpInterfaceManager()
    {
        RequestConfig requestConfig = RequestConfig.custom().setConnectTimeout(CONNECT_TIMEOUT).setConnectionRequestTimeout(CONNECT_TIMEOUT).setSocketTimeout(SOCKET_TIMEOUT).build();

        HttpClientBuilder builder = HttpClientTools.createSharedCookiesHttpBuilder();
        builder.DefaultRequestConfig = requestConfig;
        return new SimpleHttpInterfaceManager(builder, requestConfig);
    }

    /// <summary>
    /// Check if there are any playing tracks on a node that has not shown signs of life in too long. In that case its
    /// playing tracks will also be marked dead.
    /// </summary>
    /// <param name="terminate"> Whether to terminate without checking the threshold </param>
    public virtual void processHealthCheck(bool terminate)
    {
        lock (this)
        {
            if (playingTracks.Empty || (!terminate && lastAliveTime >= DateTimeHelperClass.CurrentUnixTimeMillis() - TRACK_KILL_THRESHOLD))
            {
                return;
            }

            connectionState.set(ConnectionState.OFFLINE.id());

            if (!terminate)
            {
                log.warn("Bringing node {} offline since last response from it was {}ms ago.", nodeAddress, DateTimeHelperClass.CurrentUnixTimeMillis() - lastAliveTime);
            }

            // There may be some racing that manages to add a track after this, it will be dealt with on the next iteration
            foreach (long? executorId in new List<>(playingTracks.Keys))
            {
                RemoteAudioTrackExecutor executor = playingTracks.remove(executorId);

                if (executor != null)
                {
                    abandonedTrackManager.add(executor);
                }
            }
        }
    }

    private void recordTick(RemoteNode.Tick tick, RingBufferMath timingAverage)
    {
        timingAverage.add(tick.endTime - tick.startTime);
        requestTimingPenalty = (int)((1450.0f / ((1450.0f - Math.Min(timingAverage.mean(), 1440)) / 30.0f)) - 30.0f);

        lock (tickHistory)
        {
            if (tickHistory.size() == NODE_REQUEST_HISTORY)
            {
                tickHistory.removeFirst();
            }

            tickHistory.addLast(tick);
        }
    }

    public override string Address
    {
        get
        {
            return nodeAddress;
        }
    }

    public override ConnectionState ConnectionState
    {
        get
        {
            if (closed)
            {
                return ConnectionState.REMOVED;
            }
            else
            {
                return typeof(ConnectionState).EnumConstants[connectionState.get()];
            }
        }
    }

    public override NodeStatisticsMessage LastStatistics
    {
        get
        {
            return lastStatistics;
        }
    }

    public override int TickMinimumInterval
    {
        get
        {
            return TICK_MINIMUM_INTERVAL;
        }
    }

    public override int TickHistoryCapacity
    {
        get
        {
            return NODE_REQUEST_HISTORY;
        }
    }

    public override IList<Tick> getLastTicks(bool reset)
    {
        lock (tickHistory)
        {
            IList<Tick> result = new List<Tick>(tickHistory);

            if (reset)
            {
                tickHistory.clear();
            }

            return result;
        }
    }

    public override int PlayingTrackCount
    {
        get
        {
            return playingTracks.size();
        }
    }

    //---------------------------------------------------------------------------------------------------------
    //	Copyright © 2007 - 2017 Tangible Software Solutions Inc.
    //	This class can be used by anyone provided that the copyright notice remains intact.
    //
    //	This class is used to replace calls to Java's System.currentTimeMillis with the C# equivalent.
    //	Unix time is defined as the number of seconds that have elapsed since midnight UTC, 1 January 1970.
    //---------------------------------------------------------------------------------------------------------
    internal static class DateTimeHelperClass
    {
        private static readonly System.DateTime Jan1st1970 = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        internal static long CurrentUnixTimeMillis()
        {
            return (long)(System.DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
        }
    }

    public override IList<AudioTrack> PlayingTracks
    {
        get
        {
            IList<AudioTrack> tracks = new List<AudioTrack>();

            foreach (RemoteAudioTrackExecutor executor in playingTracks.values())
            {
                tracks.Add(executor.Track);
            }

            return tracks;
        }
    }

    private bool isUnavailableForTracks(NodeStatisticsMessage statistics)
    {
        return statistics == null || connectionState.get() != ConnectionState.ONLINE.id();
    }

    private int getPenaltyForPlayingTracks(NodeStatisticsMessage statistics)
    {
        int count = statistics.playingTrackCount;
        int penalty = Math.Min(count, 100);

        if (count > 100)
        {
            penalty += Math.Pow(count - 100.0, 0.7);
        }

        return penalty * 3 / 2;
    }

    private int getPenaltyForPausedTracks(NodeStatisticsMessage statistics)
    {
        return statistics.totalTrackCount - statistics.playingTrackCount;
    }

    private int getPenaltyForCpuUsage(NodeStatisticsMessage statistics)
    {
        return (int)((1.0f / ((1.0f - Math.Min(statistics.systemCpuUsage, 0.99f)) / 30.0f)) - 30.0f);
    }

    public override IDictionary<string, int?> BalancerPenaltyDetails
    {
        get
        {
            IDictionary<string, int?> details = new Dictionary<string, int?>();
            NodeStatisticsMessage statistics = lastStatistics;

            if (isUnavailableForTracks(statistics))
            {
                details["unavailable"] = int.MaxValue;
            }
            else
            {
                details["playing"] = getPenaltyForPlayingTracks(statistics);
                details["paused"] = getPenaltyForPausedTracks(statistics);
                details["cpu"] = getPenaltyForCpuUsage(statistics);
                details["timings"] = requestTimingPenalty;
            }

            int total = 0;
            foreach (int value in details.Values)
            {
                total += value;
            }
            details["total"] = total;

            return details;
        }
    }

    /// <returns> The penalty for load balancing. Node with the lowest value will receive the next track. </returns>
    public virtual int BalancerPenalty
    {
        get
        {
            NodeStatisticsMessage statistics = lastStatistics;

            if (isUnavailableForTracks(statistics))
            {
                return int.MaxValue;
            }

            return getPenaltyForPlayingTracks(statistics) + getPenaltyForPausedTracks(statistics) + getPenaltyForCpuUsage(statistics) + requestTimingPenalty;
        }
    }

    public bool isPlayingTrack(AudioTrack track)
    {
        AudioTrackExecutor executor = ((InternalAudioTrack)track).ActiveExecutor;

        if (executor is RemoteAudioTrackExecutor)
        {
            return playingTracks.containsKey(((RemoteAudioTrackExecutor)executor).ExecutorId);
        }

        return false;
    }

    class TickBuilder
    {
        private readonly long startTime;
        private long endTime;
        private int responseCode;
        private int requestSize;
        private int responseSize;

        private TickBuilder(long startTime)
        {
            this.startTime = startTime;
            this.responseCode = -1;
        }

        private Tick build()
        {
            return new Tick(startTime, endTime, responseCode, requestSize, responseSize);
        }
    }
}
