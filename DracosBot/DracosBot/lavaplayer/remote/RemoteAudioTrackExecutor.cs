using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AudioConfiguration = com.sedmelluq.discord.lavaplayer.player.AudioConfiguration;
using ExceptionTools = com.sedmelluq.discord.lavaplayer.tools.ExceptionTools;
using FriendlyException = com.sedmelluq.discord.lavaplayer.tools.FriendlyException;
using AudioTrack = com.sedmelluq.discord.lavaplayer.track.AudioTrack;
using AudioTrackEndReason = com.sedmelluq.discord.lavaplayer.track.AudioTrackEndReason;
using AudioTrackState = com.sedmelluq.discord.lavaplayer.track.AudioTrackState;
using TrackMarker = com.sedmelluq.discord.lavaplayer.track.TrackMarker;
using TrackMarkerTracker = com.sedmelluq.discord.lavaplayer.track.TrackMarkerTracker;
using TrackStateListener = com.sedmelluq.discord.lavaplayer.track.TrackStateListener;
using AudioFrame = com.sedmelluq.discord.lavaplayer.track.playback.AudioFrame;
using AudioFrameBuffer = com.sedmelluq.discord.lavaplayer.track.playback.AudioFrameBuffer;
using AudioFrameProviderTools = com.sedmelluq.discord.lavaplayer.track.playback.AudioFrameProviderTools;
using AudioTrackExecutor = com.sedmelluq.discord.lavaplayer.track.playback.AudioTrackExecutor;
using Microsoft.Extensions.Logging;
using java.util.concurrent.atomic;
using MarkerState = com.sedmelluq.discord.lavaplayer.track.TrackMarkerHandler_MarkerState;
using java.util.concurrent;
using System.Diagnostics;

namespace com.sedmelluq.discord.lavaplayer.remote
{
    using Logger = ILogger;
    using LoggerFactory = ILoggerFactory;

    /// <summary>
    /// This executor delegates the actual audio processing to a remote node.
    /// </summary>
    public class RemoteAudioTrackExecutor : AudioTrackExecutor
    {
        private static readonly LoggerFactory factory;

        private static readonly Logger log = factory.CreateLogger<RemoteAudioTrackExecutor>();

        private const long NO_SEEK = -1;
        private const int BUFFER_DURATION_MS = 3000;

        private readonly AudioTrack track;
        private readonly AudioConfiguration configuration;
        private readonly RemoteNodeManager remoteNodeManager;
        private readonly AtomicInteger volumeLevel;
        private readonly long executorId;
        private readonly AudioFrameBuffer frameBuffer;
        private readonly AtomicLong lastFrameTimecode = new AtomicLong();
        private readonly AtomicLong pendingSeek = new AtomicLong(NO_SEEK);
        private readonly TrackMarkerTracker markerTracker = new TrackMarkerTracker();
        private volatile TrackStateListener activeListener;
        private volatile bool hasReceivedData;
        private volatile bool hasStarted;
        private volatile Exception trackException;

        /// <param name="track"> Audio track to play </param>
        /// <param name="configuration"> Configuration for audio processing </param>
        /// <param name="remoteNodeManager"> Manager of remote nodes </param>
        /// <param name="volumeLevel"> Mutable volume level </param>
        public RemoteAudioTrackExecutor(AudioTrack track, AudioConfiguration configuration, RemoteNodeManager remoteNodeManager, AtomicInteger volumeLevel)
        {

            this.track = track;
            this.configuration = configuration.copy();
            this.remoteNodeManager = remoteNodeManager;
            this.volumeLevel = volumeLevel;
            this.executorId = nanoTime();
            this.frameBuffer = new AudioFrameBuffer(BUFFER_DURATION_MS, configuration.OutputFormat, null);
        }
        private static long nanoTime()
        {
            long nano = 10000L * Stopwatch.GetTimestamp();
            nano /= TimeSpan.TicksPerMillisecond;
            nano *= 100L;
            return nano;
        }

        /// <returns> The unique ID for this executor </returns>
        public virtual long ExecutorId
        {
            get
            {
                return executorId;
            }
        }

        /// 
        /// <returns> The configuration to use for processing audio </returns>
        public virtual AudioConfiguration Configuration
        {
            get
            {
                return configuration;
            }
        }

        /// <returns> The current volume of the track </returns>
        public virtual int Volume
        {
            get
            {
                return volumeLevel.get();
            }
        }

        /// <returns> The track that this executor is playing </returns>
        public virtual AudioTrack Track
        {
            get
            {
                return track;
            }
        }


        /// <returns> The position of a seek that has not completed. Value is -1 in case no seeking is in progress. </returns>
        public virtual AtomicLong PendingSeek
        {
            get
            {
                return pendingSeek;
            }
        }

        /// <summary>
        /// Clear the current seeking if its position matches the specified position </summary>
        /// <param name="position"> The position to compare with </param>
        public virtual void clearSeek(long position)
        {
            if (position != NO_SEEK)
            {
                frameBuffer.setClearOnInsert();

                if (pendingSeek.compareAndSet(position, NO_SEEK))
                {
                    markerTracker.checkSeekTimecode(position);
                }
            }
        }

        /// <summary>
        /// Send the specified exception as an event to the active state listener. </summary>
        /// <param name="exception"> Exception to send </param>
        public virtual void dispatchException(FriendlyException exception)
        {
            TrackStateListener currentListener = activeListener;

            ExceptionTools.log(log, exception, track.Identifier);

            if (currentListener != null)
            {
                trackException = exception;
                currentListener.onTrackException(track, exception);
            }
        }

        /// <summary>
        /// Mark that this track has received data from the node.
        /// </summary>
        public virtual void receivedData()
        {
            hasReceivedData = true;
        }

        /// <summary>
        /// Detach the currently active listener, so no useless reference would be kept and no events would be sent there.
        /// </summary>
        public virtual void detach()
        {
            activeListener = null;

            markerTracker.trigger(MarkerState.ENDED);
        }

        public AudioFrameBuffer AudioBuffer
        {
            get
            {
                return frameBuffer;
            }
        }

        public void execute(TrackStateListener listener)
        {
            try
            {
                hasStarted = true;
                activeListener = listener;
                remoteNodeManager.startPlaying(this);
            }
            catch (Exception throwable)
            {
                listener.onTrackException(track, ExceptionTools.wrapUnfriendlyExceptions("An error occurred when trying to start track remotely.", FriendlyException.Severity.FAULT, throwable));

                ExceptionTools.rethrowErrors(throwable);
            }
        }

        public void stop()
        {
            frameBuffer.lockBuffer();
            frameBuffer.setTerminateOnEmpty();
            frameBuffer.clear();

            markerTracker.trigger(MarkerState.STOPPED);

            remoteNodeManager.onTrackEnd(null, track, AudioTrackEndReason.STOPPED);
        }

        public long Position
        {
            get
            {
                return lastFrameTimecode.get();
            }
            set
            {
                pendingSeek.set(value);
            }
        }
        public AudioTrackState State
        {
            get
            {
                if (hasStarted && activeListener == null)
                {
                    return AudioTrackState.FINISHED;
                }
                else if (!hasReceivedData)
                {
                    return AudioTrackState.LOADING;
                }
                else
                {
                    return AudioTrackState.PLAYING;
                }
            }
        }

        public TrackMarker Marker
        {
            set
            {
                markerTracker.set(value, Position);
            }
        }

        public AudioFrame provide()
        {
            AudioFrame frame = frameBuffer.provide();
            processProvidedFrame(frame);
            return frame;
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: @Override public AudioFrame provide(long timeout, TimeUnit unit) throws TimeoutException, InterruptedException
        public AudioFrame provide(long timeout, TimeUnit unit)
        {
            AudioFrame frame = frameBuffer.provide(timeout, unit);
            processProvidedFrame(frame);
            return frame;
        }

        private void processProvidedFrame(AudioFrame frame)
        {
            if (frame != null && !frame.Terminator)
            {
                lastFrameTimecode.set(frame.timecode);

                if (pendingSeek.get() == NO_SEEK && !frameBuffer.hasClearOnInsert())
                {
                    markerTracker.checkPlaybackTimecode(frame.timecode);
                }
            }
        }

        public bool failedBeforeLoad()
        {
            return trackException != null && !hasReceivedData;
        }

        /// <returns> The expected timecode of the next frame to receive from the remote node. </returns>
        public virtual long NextInputTimecode
        {
            get
            {
                bool dataReceived = hasReceivedData;
                long frameDuration = configuration.OutputFormat.frameDuration();

                if (dataReceived)
                {
                    long? lastBufferTimecode = frameBuffer.LastInputTimecode;
                    if (lastBufferTimecode != null)
                    {
                        return (long)lastBufferTimecode + frameDuration;
                    }
                }

                long seekPosition = pendingSeek.get();
                if (seekPosition != NO_SEEK)
                {
                    return seekPosition;
                }

                return dataReceived ? lastFrameTimecode.get() + frameDuration : lastFrameTimecode.get();
            }
        }

        public override string ToString()
        {
            return "RemoteExec " + executorId + ", " + track.Identifier;
        }
    }
}
