using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using java.util.concurrent;
using java.lang;
using java.util;
using java.util.concurrent.atomic;
using System.Collections.Concurrent;
//using Microsoft.Extensions.Logging;
namespace com.sedmelluq.discord.lavaplayer.track.playback
{
    using AudioDataFormat = com.sedmelluq.discord.lavaplayer.format.AudioDataFormat;


    /// <summary>
    /// A frame buffer. Stores the specified duration worth of frames in the internal buffer.
    /// Consumes frames in a blocking manner and provides frames in a non-blocking manner.
    /// </summary>
    public class AudioFrameBuffer : AudioFrameConsumer, AudioFrameProvider
    {
        //private static readonly ILoggerFactory loggerFactory = new LoggerFactory()
                                                                      // .AddConsole()
                                                                      // .AddDebug();
        //private static readonly ILogger logger = loggerFactory.CreateLogger(typeof(AudioFrameBuffer));

        private readonly object synchronizer;
        private readonly int fullCapacity;
        //private readonly ArrayBlockingQueue/*<AudioFrame>*/ audioFrames;
        private readonly BlockingCollection<AudioFrame> _audioFrames;
        private readonly AudioDataFormat format;
        private readonly AtomicBoolean stopping;
        private volatile bool locked;
        private volatile bool receivedFrames;
        private bool terminated;
        private bool terminateOnEmpty;
        private bool clearOnInsert;

        /// <param name="bufferDuration"> The length of the internal buffer in milliseconds </param>
        /// <param name="format"> The format of the frames held in this buffer </param>
        public AudioFrameBuffer(int bufferDuration, AudioDataFormat format, AtomicBoolean stopping)
        {
            synchronizer = new object();
            fullCapacity = bufferDuration / 20 + 1;
            _audioFrames = new BlockingCollection<AudioFrame>/*<AudioFrame>*/(fullCapacity);
            this.format = format;
            this.stopping = stopping;
            terminated = false;
            terminateOnEmpty = false;
            clearOnInsert = false;
            receivedFrames = false;
        }

        public void Clear<T>(BlockingCollection<T> blockingCollection)
        {
            if (blockingCollection == null)
            {
                throw new ArgumentNullException("blockingCollection");
            }

            while (blockingCollection.Count > 0)
            {
                T item;
                blockingCollection.TryTake(out item);
            }
        }



        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: @Override public void consume(AudioFrame frame) throws InterruptedException
        public void consume(AudioFrame frame)
        {
            // If an interrupt sent along with setting the stopping status was silently consumed elsewhere, this check should
            // still trigger. Guarantees that stopped tracks cannot get stuck in this method. Possible performance improvement:
            // offer with timeout, check stopping if timed out, then put?
            if (stopping != null && stopping.get())
            {
                throw new InterruptedException();
            }

            if (!locked)
            {
                receivedFrames = true;

                if (clearOnInsert)
                {
                    Clear<AudioFrame>(_audioFrames);
                    clearOnInsert = false;
                }

                _audioFrames.Add(frame);
            }
        }

        /// <returns> Number of frames that can be added to the buffer without blocking. </returns>
        public virtual int RemainingCapacity
        {
            get
            {
                return _audioFrames.Count();
            }
        }

        /// <returns> Total number of frames that the buffer can hold. </returns>
        public virtual int FullCapacity
        {
            get
            {
                return fullCapacity;
            }
        }

        /// <summary>
        /// Wait until another thread has consumed a terminator frame from this buffer </summary>
        /// <exception cref="InterruptedException"> When interrupted, expected on seek or stop </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public void waitForTermination() throws InterruptedException
        public virtual void waitForTermination()
        {
            lock (synchronizer)
            {
                while (!terminated)
                {
                    Monitor.Wait(synchronizer);
                }
            }
        }
        public AudioFrame Poll(BlockingCollection<AudioFrame> audioFrame)
        {
            AudioFrame returnVal = audioFrame.FirstOrDefault<AudioFrame>();
            audioFrame.TryTake(out returnVal);
            return returnVal;
        }
        public AudioFrame Poll(BlockingCollection<AudioFrame> audioFrame, long timeout, TimeUnit unit)
        {
            AudioFrame returnVal = audioFrame.FirstOrDefault<AudioFrame>();
            
            audioFrame.TryTake(out returnVal);
            return returnVal;

            unit.wait(timeout);
            return null;
        }

        public AudioFrame provide()
        {
            AudioFrame frame = Poll(_audioFrames);

            if (frame == null)
            {
                return fetchPendingTerminator();
            }

            return filterFrame(frame);
        }

       

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: @Override public AudioFrame provide(long timeout, TimeUnit unit) throws TimeoutException, InterruptedException
        public AudioFrame provide(long timeout, TimeUnit unit)
        {
            AudioFrame frame = Poll(_audioFrames);

            if (frame == null)
            {
                AudioFrame terminator = fetchPendingTerminator();
                if (terminator != null)
                {
                    return terminator;
                }

                if (timeout > 0)
                {
                    frame = Poll(_audioFrames, timeout, unit);
                    terminator = fetchPendingTerminator();

                    if (terminator != null)
                    {
                        return terminator;
                    }
                }
            }

            return filterFrame(frame);
        }

        private AudioFrame fetchPendingTerminator()
        {
            lock (synchronizer)
            {
                if (terminateOnEmpty)
                {
                    terminateOnEmpty = false;
                    terminated = true;
                    Monitor.PulseAll(synchronizer);
                    return AudioFrame.TERMINATOR;
                }
            }

            return null;
        }

        private AudioFrame filterFrame(AudioFrame frame)
        {
            if (frame != null && frame.volume == 0)
            {
                return new AudioFrame(frame.timecode, format.silence, 0, format);
            }

            return frame;
        }

        /// <summary>
        /// Signal that no more input is expected and if the content frames have been consumed, emit a terminator frame.
        /// </summary>
        public virtual void setTerminateOnEmpty()
        {
            lock (synchronizer)
            {
                // Count this also as inserting the terminator frame, hence trigger clearOnInsert
                if (clearOnInsert)
                {
                    Clear<AudioFrame>(_audioFrames);
                    clearOnInsert = false;
                }

                if (!terminated)
                {
                    terminateOnEmpty = true;
                }
            }
        }

        /// <summary>
        /// Signal that the next frame provided to the buffer will clear the frames before it. This is useful when the next
        /// data is not contiguous with the current frame buffer, but the remaining frames in the buffer should be used until
        /// the next data arrives to prevent a situation where the buffer cannot provide any frames for a while.
        /// </summary>
        public virtual void setClearOnInsert()
        {
            lock (synchronizer)
            {
                clearOnInsert = true;
                terminateOnEmpty = false;
            }
        }

        /// <returns> Whether the next frame is set to clear the buffer. </returns>
        public virtual bool hasClearOnInsert()
        {
            return clearOnInsert;
        }

        /// <summary>
        /// Clear the buffer.
        /// </summary>
        public virtual void clear()
        {
            Clear<AudioFrame>(_audioFrames);
        }
        /// <summary>
        /// Lock the buffer so no more incoming frames are accepted.
        /// </summary>
        public virtual void lockBuffer()
        {
            locked = true;
        }

        /// <returns> True if this buffer has received any input frames. </returns>
        public virtual bool hasReceivedFrames()
        {
            return receivedFrames;
        }


        public int drainTo<T>(BlockingCollection<T> blockingCollection, IList<T> toDrain)
        {
            while (blockingCollection.Count > 0)
            {
                T item;
                blockingCollection.TryTake(out item);
            }

            int countItems = 0;
            while(blockingCollection.Count > 0)
            {
                T item;
                toDrain.Add(blockingCollection.First<T>());
                blockingCollection.TryTake(out item);
                countItems++;
            }
            return countItems;
        }
        public void rebuild(AudioFrameRebuilder rebuilder)
        {
            IList<AudioFrame> frames = new List<AudioFrame>();
            //int frameCount = _audioFrames.drainTo(frames);
            int frameCount = drainTo(_audioFrames, frames);

            //log.debug("Running rebuilder {} on {} buffered frames.", rebuilder.GetType().Name, frameCount);

            foreach (AudioFrame frame in frames)
            {
                _audioFrames.Add(rebuilder.rebuild(frame));
            }
        }

        /// <returns> The timecode of the last frame in the buffer, null if the buffer is empty or is marked to be cleared upon
        ///         receiving the next frame. </returns>
        public virtual long? LastInputTimecode
        {
            get
            {
                long? lastTimecode = null;

                lock (synchronizer)
                {
                    if (!clearOnInsert)
                    {
                        foreach (AudioFrame frame in _audioFrames)
                        {
                            lastTimecode = frame.timecode;
                        }
                    }
                }

                return lastTimecode;
            }
        }
    }
}

