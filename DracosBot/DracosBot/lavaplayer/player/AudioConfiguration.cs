using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace com.sedmelluq.discord.lavaplayer.player
{
    using AudioDataFormat = com.sedmelluq.discord.lavaplayer.format.AudioDataFormat;
    using StandardAudioDataFormats = com.sedmelluq.discord.lavaplayer.format.StandardAudioDataFormats;

    /// <summary>
    /// Configuration for audio processing.
    /// </summary>
    public class AudioConfiguration
    {
        public const int OPUS_QUALITY_MAX = 10;

        private volatile ResamplingQuality resamplingQuality;
        private volatile int opusEncodingQuality;
        private volatile AudioDataFormat outputFormat;

        /// <summary>
        /// Create a new configuration with default values.
        /// </summary>
        public AudioConfiguration()
        {
            resamplingQuality = ResamplingQuality.LOW;
            opusEncodingQuality = OPUS_QUALITY_MAX;
            outputFormat = StandardAudioDataFormats.DISCORD_OPUS;
        }

        public virtual ResamplingQuality getResamplingQuality()
        {
            return resamplingQuality;
        }

        public virtual void setResamplingQuality(ResamplingQuality resamplingQuality)
        {
            this.resamplingQuality = resamplingQuality;
        }

        public virtual int OpusEncodingQuality
        {
            get
            {
                return opusEncodingQuality;
            }
            set
            {
                this.opusEncodingQuality = Math.Max(0, Math.Min(value, OPUS_QUALITY_MAX));
            }
        }


        public virtual AudioDataFormat OutputFormat
        {
            get
            {
                return outputFormat;
            }
            set
            {
                this.outputFormat = value;
            }
        }


        /// <returns> A copy of this configuration. </returns>
        public virtual AudioConfiguration copy()
        {
            AudioConfiguration copy = new AudioConfiguration();
            copy.setResamplingQuality(resamplingQuality);
            copy.OpusEncodingQuality = opusEncodingQuality;
            copy.OutputFormat = outputFormat;
            return copy;
        }

        /// <summary>
        /// Resampling quality levels
        /// </summary>
        public enum ResamplingQuality
        {
            HIGH,
            MEDIUM,
            LOW
        }
    }
}
