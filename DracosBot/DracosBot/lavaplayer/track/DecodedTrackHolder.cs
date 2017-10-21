using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sedmelluq.discord.lavaplayer.track
{
    /// <summary>
    /// The result of decoding a track.
    /// </summary>
    public class DecodedTrackHolder
    {
        /// <summary>
        /// The decoded track. This may be null if there was a track to decode, but the decoding could not be performed because
        /// of an older serialization version or because the track source it used is not loaded.
        /// </summary>
        public readonly AudioTrack decodedTrack;

        /// <param name="decodedTrack"> The decoded track </param>
        public DecodedTrackHolder(AudioTrack decodedTrack)
        {
            this.decodedTrack = decodedTrack;
        }
    }
}
