using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sedmelluq.discord.lavaplayer.track
{
    /// <summary>
    /// A track position marker. This makes the specified handler get called when the specified position is reached or
    /// reaching that position has become impossible. This guarantees that whenever a marker is set and the track is played,
    /// its handler will always be called.
    /// </summary>
    public class TrackMarker
    {
        /// <summary>
        /// The position of the track in milliseconds when this marker should trigger.
        /// </summary>
        public readonly long timecode;
        /// <summary>
        /// The handler for the marker. The handler is guaranteed to be never called more than once, and guaranteed to be
        /// called at least once if the track is started on a player.
        /// </summary>
        public readonly TrackMarkerHandler handler;

        /// <param name="timecode"> The position of the track in milliseconds when this marker should trigger. </param>
        /// <param name="handler"> The handler for the marker. </param>
        public TrackMarker(long timecode, TrackMarkerHandler handler)
        {
            this.timecode = timecode;
            this.handler = handler;
        }
    }
}

