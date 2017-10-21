using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sedmelluq.discord.lavaplayer.format
{
    /// <summary>
    /// Describes the format for audio with fixed chunk size.
    /// </summary>
    public class AudioDataFormat
    {
        private static readonly sbyte[] SILENT_OPUS_FRAME = new sbyte[] { unchecked((sbyte)0xFC), unchecked((sbyte)0xFF), unchecked((sbyte)0xFE) };


        /// <summary>
        /// Number of channels.
        /// </summary>
        public readonly int channelCount;
        /// <summary>
        /// Sample rate (frequency).
        /// </summary>
        public readonly int sampleRate;
        /// <summary>
        /// Number of samples in one chunk.
        /// </summary>
        public readonly int chunkSampleCount;
        /// <summary>
        /// Codec used to produce the raw buffer.
        /// </summary>
        public readonly Codec codec;
        /// <summary>
        /// Bytes representing a silent chunk with this format.
        /// </summary>
        public readonly sbyte[] silence;

        /// <param name="channelCount"> Number of channels. </param>
        /// <param name="sampleRate"> Sample rate (frequency). </param>
        /// <param name="chunkSampleCount"> Number of samples in one chunk. </param>
        /// <param name="codec"> Codec used to produce the raw buffer. </param>
        public AudioDataFormat(int channelCount, int sampleRate, int chunkSampleCount, Codec codec)
        {
            this.channelCount = channelCount;
            this.sampleRate = sampleRate;
            this.chunkSampleCount = chunkSampleCount;
            this.codec = codec;
            this.silence = produceSilence();
        }

        /// <param name="sampleSize"> Size per sample. </param>
        /// <returns> Size of a buffer that can fit one chunk in this format, assuming fixed sample size. </returns>
        public virtual int bufferSize(int sampleSize)
        {
            return chunkSampleCount * channelCount * sampleSize;
        }

        /// <returns> The duration in milliseconds of one frame in this format. </returns>
        public virtual long frameDuration()
        {
            return chunkSampleCount * 1000L / sampleRate;
        }

        public override bool Equals(object o)
        {
            if (this == o)
            {
                return true;
            }
            if (o == null || this.GetType() != o.GetType())
            {
                return false;
            }

            AudioDataFormat that = (AudioDataFormat)o;

            if (channelCount != that.channelCount)
            {
                return false;
            }
            if (sampleRate != that.sampleRate)
            {
                return false;
            }
            if (chunkSampleCount != that.chunkSampleCount)
            {
                return false;
            }
            return codec == that.codec;
        }

        public override int GetHashCode()
        {
            int result = channelCount;
            result = 31 * result + sampleRate;
            result = 31 * result + chunkSampleCount;
            result = 31 * result + codec.GetHashCode();
            return result;
        }

        private sbyte[] produceSilence()
        {
            if (codec == Codec.OPUS)
            {
                return SILENT_OPUS_FRAME;
            }
            else
            {
                return new sbyte[bufferSize(2)];
            }
        }

        /// <summary>
        /// Codec of the audio.
        /// </summary>
        public enum Codec
        {
            /// <summary>
            /// Opus codec.
            /// </summary>
            OPUS,

            /// <summary>
            /// Signed 16-bit little-endian PCM
            /// </summary>
            PCM_S16_LE, PCM_S16_BE
            /// <summary>
            /// Signed 16-bit big-endian PCM
            /// </summary>
        }
    }
}
