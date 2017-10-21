using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace com.sedmelluq.discord.lavaplayer.track
{
    /// <summary>
    /// Meta info for an audio track
    /// </summary>
    public class AudioTrackInfo
    {
        /// <summary>
        /// Track title
        /// </summary>
        public readonly string title;
        /// <summary>
        /// Track author, if known
        /// </summary>
        public readonly string author;
        /// <summary>
        /// Length of the track in milliseconds, Long.MAX_VALUE for streams
        /// </summary>
        public readonly long length;
        /// <summary>
        /// Audio source specific track identifier
        /// </summary>
        public readonly string identifier;
        /// <summary>
        /// True if this track is a stream
        /// </summary>
        public readonly bool isStream;
        /// <summary>
        /// URL of the track, or local path to the file.
        /// </summary>
        public readonly string uri;

        /// <param name="title"> Track title </param>
        /// <param name="author"> Track author, if known </param>
        /// <param name="length"> Length of the track in milliseconds </param>
        /// <param name="identifier"> Audio source specific track identifier </param>
        /// <param name="isStream"> True if this track is a stream </param>
        /// <param name="uri"> URL of the track or path to its file. </param>
        public AudioTrackInfo(string title, string author, long length, string identifier, bool isStream, string uri)
        {
            this.title = title;
            this.author = author;
            this.length = length;
            this.identifier = identifier;
            this.isStream = isStream;
            this.uri = uri;
        }
    }
}
