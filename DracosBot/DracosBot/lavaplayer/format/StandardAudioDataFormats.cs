using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sedmelluq.discord.lavaplayer.format
{

    using  AudioDataFormatInstace = com.sedmelluq.discord.lavaplayer.format.AudioDataFormat.Codec;

    /// <summary>
    /// Standard output formats compatible with Discord.
    /// </summary>
    public class StandardAudioDataFormats
    {
        /// <summary>
        /// The Opus configuration used by both Discord and YouTube. Default.
        /// </summary>
        public static readonly AudioDataFormat DISCORD_OPUS = new AudioDataFormat(2, 48000, 960, AudioDataFormatInstace.OPUS);
        /// <summary>
        /// Signed 16-bit big-endian PCM format matching the parameters used by Discord.
        /// </summary>
        public static readonly AudioDataFormat DISCORD_PCM_S16_BE = new AudioDataFormat(2, 48000, 960, AudioDataFormatInstace.PCM_S16_BE);
        /// <summary>
        /// Signed 16-bit little-endian PCM format matching the parameters used by Discord.
        /// </summary>
        public static readonly AudioDataFormat DISCORD_PCM_S16_LE = new AudioDataFormat(2, 48000, 960, AudioDataFormatInstace.PCM_S16_LE);
    }
}
