using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sedmelluq.discord.lavaplayer.track
{
    /// <summary>
    /// A track marker handler.
    /// </summary>
    public interface TrackMarkerHandler
    {
        /// <param name="state"> The state of the marker when it is triggered. </param>
        void handle(TrackMarkerHandler_MarkerState state);
        /// <summary>
        /// The state of the marker at the moment the handle method is called.
        /// </summary>
    }

    public enum TrackMarkerHandler_MarkerState
    {
        /// <summary>
        /// The specified position has been reached with normal playback.
        /// </summary>
        REACHED,
        /// <summary>
        /// The marker has been removed by setting the marker of the track to null.
        /// </summary>
        REMOVED,
        /// <summary>
        /// The marker has been overwritten by setting the marker of the track to another non-null marker.
        /// </summary>
        OVERWRITTEN,
        /// <summary>
        /// A seek was performed which jumped over the marked position.
        /// </summary>
        BYPASSED,
        /// <summary>
        /// The track was stopped before it ended, before the marked position was reached.
        /// </summary>
        STOPPED,
        /// <summary>
        /// The playback position was already beyond the marked position when the marker was placed.
        /// </summary>
        LATE,
        /// <summary>
        /// The track ended without the marker being triggered (either due to an exception or because the track duration was
        /// smaller than the marked position).
        /// </summary>
        ENDED
    }
}
