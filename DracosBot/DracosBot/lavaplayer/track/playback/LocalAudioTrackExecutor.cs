using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
namespace com.sedmelluq.discord.lavaplayer.track.playback
{
    using AudioDataFormat = com.sedmelluq.discord.lavaplayer.format.AudioDataFormat;
    using AudioConfiguration = com.sedmelluq.discord.lavaplayer.player.AudioConfiguration;
    using ExceptionTools = com.sedmelluq.discord.lavaplayer.tools.ExceptionTools;
    using FriendlyException = com.sedmelluq.discord.lavaplayer.tools.FriendlyException;
    //using Logger = org.slf4j.Logger;
    //using LoggerFactory = org.slf4j.LoggerFactory;


    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    using findDeepExceptuon = tools.ExceptionTools.findDeepException;
    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    using FAULT = com.sedmelluq.discord.lavaplayer.tools.FriendlyException.Severity.FAULT;
    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    using SUSPICIOUS = com.sedmelluq.discord.lavaplayer.tools.FriendlyException.Severity.SUSPICIOUS;
    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    using ENDED = com.sedmelluq.discord.lavaplayer.track.TrackMarkerHandler_MarkerState.ENDED;
    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    using STOPPED = com.sedmelluq.discord.lavaplayer.track.TrackMarkerHandler_MarkerState.STOPPED;

    /// <summary>
    /// Handles the execution and output buffering of an audio track.
    /// </summary>
    public class LocalAudioTrackExecutor : AudioTrackExecutor
    {
        private static readonly ILoggerFactory loggerFactory = new ILoggerFactory()
        private static readonly ILogger log = ILoggerFactory.(typeof(LocalAudioTrackExecutor));

        private readonly InternalAudioTrack audioTrack;
        private readonly AudioProcessingContext processingContext;
        private readonly bool useSeekGhosting;
        private readonly AudioFrameBuffer frameBuffer;
        private readonly AtomicReference<Thread> playingThread = new AtomicReference<Thread>();
        private readonly AtomicBoolean isStopping = new AtomicBoolean(false);
        private readonly AtomicLong pendingSeek = new AtomicLong(-1);
        private readonly AtomicLong lastFrameTimecode = new AtomicLong(0);
        private readonly AtomicReference<AudioTrackState> state = new AtomicReference<AudioTrackState>(AudioTrackState.INACTIVE);
        private readonly object actionSynchronizer = new object();
        private readonly TrackMarkerTracker markerTracker = new TrackMarkerTracker();
        private bool interruptibleForSeek = false;
        private volatile Exception trackException;

        /// <param name="audioTrack"> The audio track that this executor executes </param>
        /// <param name="configuration"> Configuration to use for audio processing </param>
        /// <param name="volumeLevel"> Mutable volume level to use when executing the track </param>
        /// <param name="useSeekGhosting"> Whether to keep providing old frames continuing from the previous position during a seek
        ///                        until frames from the new position arrive. </param>
        /// <param name="bufferDuration"> The size of the frame buffer in milliseconds </param>
        public LocalAudioTrackExecutor(InternalAudioTrack audioTrack, AudioConfiguration configuration, AtomicInteger volumeLevel, bool useSeekGhosting, int bufferDuration)
        {

            this.audioTrack = audioTrack;
            AudioDataFormat currentFormat = configuration.OutputFormat;
            this.frameBuffer = new AudioFrameBuffer(bufferDuration, currentFormat, isStopping);
            this.processingContext = new AudioProcessingContext(configuration, frameBuffer, volumeLevel, currentFormat);
            this.useSeekGhosting = useSeekGhosting;
        }

        public virtual AudioProcessingContext ProcessingContext
        {
            get
            {
                return processingContext;
            }
        }

        public override AudioFrameBuffer AudioBuffer
        {
            get
            {
                return frameBuffer;
            }
        }


        public override void execute(TrackStateListener listener)
        {
            InterruptedException interrupt = null;

            if (Thread.interrupted())
            {
                log.debug("Cleared a stray interrupt.");
            }

            if (playingThread.compareAndSet(null, Thread.CurrentThread))
            {
                log.debug("Starting to play track {} locally with listener {}", audioTrack.Info.identifier, listener);

                state.set(AudioTrackState.LOADING);

                try
                {
                    audioTrack.process(this);

                    log.debug("Playing track {} finished or was stopped.", audioTrack.Identifier);
                }
                catch (Exception e)
                {
                    // Temporarily clear the interrupted status so it would not disrupt listener methods.
                    interrupt = findInterrupt(e);

                    if (interrupt != null && checkStopped())
                    {
                        log.debug("Track {} was interrupted outside of execution loop.", audioTrack.Identifier);
                    }
                    else
                    {
                        frameBuffer.setTerminateOnEmpty();

                        FriendlyException exception = ExceptionTools.wrapUnfriendlyExceptions("Something broke when playing the track.", FAULT, e);
                        ExceptionTools.log(log, exception, "playback of " + audioTrack.Identifier);

                        trackException = exception;
                        listener.onTrackException(audioTrack, exception);

                        ExceptionTools.rethrowErrors(e);
                    }
                }
                finally
                {
                    lock (actionSynchronizer)
                    {
                        interrupt = interrupt != null ? interrupt : findInterrupt(null);

                        playingThread.compareAndSet(Thread.CurrentThread, null);

                        markerTracker.trigger(ENDED);
                        state.set(AudioTrackState.FINISHED);
                    }

                    if (interrupt != null)
                    {
                        Thread.CurrentThread.Interrupt();
                    }
                }
            }
            else
            {
                log.warn("Tried to start an already playing track {}", audioTrack.Identifier);
            }
        }

        public override void stop()
        {
            lock (actionSynchronizer)
            {
                Thread thread = playingThread.get();

                if (thread != null)
                {
                    log.debug("Requesting stop for track {}", audioTrack.Identifier);

                    isStopping.compareAndSet(false, true);
                    thread.Interrupt();
                }
                else
                {
                    log.debug("Tried to stop track {} which is not playing.", audioTrack.Identifier);
                }
            }
        }

        /// <returns> True if the track has been scheduled to stop and then clears the scheduled stop bit. </returns>
        public virtual bool checkStopped()
        {
            return isStopping.compareAndSet(true, false);
        }

        /// <summary>
        /// Wait until all the frames from the frame buffer have been consumed. Keeps the buffering thread alive to keep it
        /// interruptible for seeking until buffer is empty.
        /// </summary>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public void waitOnEnd() throws InterruptedException
        public virtual void waitOnEnd()
        {
            frameBuffer.setTerminateOnEmpty();
            frameBuffer.waitForTermination();
        }


        /// <summary>
        /// Interrupt the buffering thread, either stop or seek should have been set beforehand. </summary>
        /// <returns> True if there was a thread to interrupt. </returns>
        public virtual bool interrupt()
        {
            lock (actionSynchronizer)
            {
                Thread thread = playingThread.get();

                if (thread != null)
                {
                    thread.Interrupt();
                    return true;
                }

                return false;
            }
        }

        public override long Position
        {
            get
            {
                long seek = pendingSeek.get();
                return seek != -1 ? seek : lastFrameTimecode.get();
            }
            set
            {
                if (!audioTrack.Seekable)
                {
                    return;
                }

                lock (actionSynchronizer)
                {
                    if (value < 0)
                    {
                        value = 0;
                    }

                    pendingSeek.set(value);

                    if (!useSeekGhosting)
                    {
                        frameBuffer.clear();
                    }

                    interruptForSeek();
                }
            }
        }


        public override AudioTrackState State
        {
            get
            {
                return state.get();
            }
        }

    /// <returns> True if this track is currently in the middle of a seek. </returns>
        private bool PerformingSeek
        {
            get
            {
                return pendingSeek.get() != -1 || (useSeekGhosting && frameBuffer.hasClearOnInsert());
            }
        }

        public override TrackMarker Marker
        {
            set
            {
                markerTracker.set(value, Position);
            }
        }

        public override bool failedBeforeLoad()
        {
            return trackException != null && !frameBuffer.hasReceivedFrames();
        }

        /// <summary>
        /// Execute the read and seek loop for the track. </summary>
        /// <param name="readExecutor"> Callback for reading the track </param>
        /// <param name="seekExecutor"> Callback for performing a seek on the track, may be null on a non-seekable track </param>
        public virtual void executeProcessingLoop(ReadExecutor readExecutor, SeekExecutor seekExecutor)
        {
            bool proceed = true;

            checkPendingSeek(seekExecutor);

            while (proceed)
            {
                state.set(AudioTrackState.PLAYING);
                proceed = false;

                try
                {
                    // An interrupt may have been placed while we were handling the previous one.
                    if (Thread.interrupted() && !handlePlaybackInterrupt(null, seekExecutor))
                    {
                        break;
                    }

                    InterruptibleForSeek = true;
                    readExecutor.performRead();
                    InterruptibleForSeek = false;

                    // Must not finish before terminator frame has been consumed the user may still want to perform seeks until then
                    waitOnEnd();
                }
                catch (Exception e)
                {
                    InterruptibleForSeek = false;
                    InterruptedException interruption = findInterrupt(e);

                    if (interruption != null)
                    {
                        proceed = handlePlaybackInterrupt(interruption, seekExecutor);
                    }
                    else
                    {
                        throw ExceptionTools.wrapUnfriendlyExceptions("Something went wrong when decoding the track.", FAULT, e);
                    }
                }
            }
        }
    

        private bool InterruptibleForSeek
        {
            set
            {
                lock (actionSynchronizer)
                {
                    interruptibleForSeek = value;
                }
            }
        }

        private void interruptForSeek()
        {
            bool interrupted = false;

            lock (actionSynchronizer)
            {
                if (interruptibleForSeek)
                {
                    interruptibleForSeek = false;
                    Thread thread = playingThread.get();

                    if (thread != null)
                    {
                        thread.Interrupt();
                        interrupted = true;
                    }
                }
            }

            if (interrupted)
            {
                log.debug("Interrupting playing thread to perform a seek {}", audioTrack.Identifier);
            }
            else
            {
                log.debug("Seeking on track {} while not in playback loop.", audioTrack.Identifier);
            }
        }

        private bool handlePlaybackInterrupt(InterruptedException interruption, SeekExecutor seekExecutor)
        {
            Thread.interrupted();

            if (checkStopped())
            {
                markerTracker.trigger(STOPPED);
                return false;
            }
            else if (checkPendingSeek(seekExecutor))
            {
                // Double-check, might have received a stop request while seeking
                if (checkStopped())
                {
                    markerTracker.trigger(STOPPED);
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else if (interruption != null)
            {
                Thread.CurrentThread.Interrupt();
                throw new FriendlyException("The track was unexpectedly terminated.", SUSPICIOUS, interruption);
            }
            else
            {
                return true;
            }
        }

    private InterruptedException findInterrupt(Exception throwable)
    {
        InterruptedException exception = findDeepException(throwable, typeof(InterruptedException));

        if (exception == null)
        {
            InterruptedIOException ioException = findDeepException(throwable, typeof(InterruptedIOException));

            if (ioException != null && (ioException.Message == null || !ioException.Message.contains("timed out")))
            {
                exception = new InterruptedException(ioException.Message);
            }
        }

        if (exception == null && Thread.interrupted())
        {
            return new InterruptedException();
        }

        return exception;
    }

    /// <summary>
    /// Performs a seek if it scheduled. </summary>
    /// <param name="seekExecutor"> Callback for performing a seek on the track </param>
    /// <returns> True if a seek was performed </returns>
    private bool checkPendingSeek(SeekExecutor seekExecutor)
    {
        if (!audioTrack.Seekable)
        {
            return false;
        }

        long seekPosition;

        lock (actionSynchronizer)
        {
            seekPosition = pendingSeek.get();

            if (seekPosition == -1)
            {
                return false;
            }

            log.debug("Track {} interrupted for seeking to {}.", audioTrack.Identifier, seekPosition);
            applySeekState(seekPosition);
        }

        try
        {
            seekExecutor.performSeek(seekPosition);
        }
        catch (Exception e)
        {
            throw ExceptionTools.wrapUnfriendlyExceptions("Something went wrong when seeking to a position.", FAULT, e);
        }

        return true;
    }
        private void applySeekState(long seekPosition)
        {
            state.set(AudioTrackState.SEEKING);

            if (useSeekGhosting)
            {
                frameBuffer.setClearOnInsert();
            }
            else
            {
                frameBuffer.clear();
            }

            pendingSeek.set(-1);
            markerTracker.checkSeekTimecode(seekPosition);
        }

        @Override
  public AudioFrame provide()
        {
            AudioFrame frame = frameBuffer.provide();
            processProvidedFrame(frame);
            return frame;
        }

        @Override
  public AudioFrame provide(long timeout, TimeUnit unit) throws TimeoutException, InterruptedException {
    AudioFrame frame = frameBuffer.provide(timeout, unit);
    processProvidedFrame(frame);
    return frame;
  }

    private void processProvidedFrame(AudioFrame frame)
    {
        if (frame != null && !frame.isTerminator())
        {
            if (!isPerformingSeek())
            {
                markerTracker.checkPlaybackTimecode(frame.timecode);
            }

            lastFrameTimecode.set(frame.timecode);
        }
    }

    /**
     * Read executor, see method description
     */
    public interface ReadExecutor
    {
        /**
         * Reads until interrupted or EOF
         * @throws InterruptedException
         */
        void performRead() throws InterruptedException;
    }

    /**
     * Seek executor, see method description
     */
    public interface SeekExecutor
    {
        /**
         * Perform a seek to the specified position
         * @param position Position in milliseconds
         */
        void performSeek(long position);
    }
}
