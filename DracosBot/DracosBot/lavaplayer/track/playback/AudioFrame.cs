using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sedmelluq.discord.lavaplayer.track.playback
{
    using AudioDataFormat = com.sedmelluq.discord.lavaplayer.format.AudioDataFormat;

    /// <summary>
    /// A single audio frame.
    /// </summary>
    public class AudioFrame
    {
        /// <summary>
        /// An AudioFrame instance which marks the end of an audio track, the time code or buffer from it should not be tried
        /// to access.
        /// </summary>
        public static readonly AudioFrame TERMINATOR = new AudioFrame(0, null, 0, null);

        /// <summary>
        /// Timecode of this frame in milliseconds.
        /// </summary>
        public readonly long timecode;

        /// <summary>
        /// Buffer for this frame, in the format specified in the format field.
        /// </summary>
        public readonly sbyte[] data;

        /// <summary>
        /// Volume level of the audio in this frame. Internally when this value is 0, the data may actually contain a
        /// non-silent frame. This is to allow frames with 0 volume to be modified later. These frames should still be
        /// handled as silent frames.
        /// </summary>
        public readonly int volume;

        /// <summary>
        /// Specifies the format of audio in the data buffer.
        /// </summary>
        public readonly AudioDataFormat format;

        /// <param name="timecode"> Timecode of this frame in milliseconds. </param>
        /// <param name="data"> Buffer for this frame, in the format specified in the format field. </param>
        /// <param name="volume"> Volume level of the audio in this frame. </param>
        /// <param name="format"> Specifies the format of audio in the data buffer. </param>
        public AudioFrame(long timecode, sbyte[] data, int volume, AudioDataFormat format)
        {
            this.timecode = timecode;
            this.data = data;
            this.volume = volume;
            this.format = format;
        }

        /// <returns> True if this is a terminator instance. </returns>
        public virtual bool Terminator
        {
            get
            {
                return this == TERMINATOR;
            }
        }
    }
}
