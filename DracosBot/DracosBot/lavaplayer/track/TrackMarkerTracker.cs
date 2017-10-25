using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using java.util.concurrent.atomic;
namespace com.sedmelluq.discord.lavaplayer.track
{

    /// <summary>
    /// Tracks the state of a track position marker.
    /// </summary>
    public class TrackMarkerTracker
    {
        TrackMarkerHandler_MarkerState BYPASSED = TrackMarkerHandler_MarkerState.BYPASSED;
        TrackMarkerHandler_MarkerState LATE = TrackMarkerHandler_MarkerState.LATE;
        TrackMarkerHandler_MarkerState OVERWRITTEN = TrackMarkerHandler_MarkerState.OVERWRITTEN;
        TrackMarkerHandler_MarkerState REACHED = TrackMarkerHandler_MarkerState.REACHED;
        TrackMarkerHandler_MarkerState REMOVED = TrackMarkerHandler_MarkerState.REMOVED;

        private readonly AtomicReference<TrackMarker> current = new AtomicReference<TrackMarker>();

        /// <summary>
        /// Set a new track position marker. </summary>
        /// <param name="marker"> Marker </param>
        /// <param name="currentTimecode"> Current timecode of the track when this marker is set </param>
        public virtual void set(TrackMarker marker, long currentTimecode)
        {
            TrackMarker previous = current.getAndSet(marker);

            if (previous != null)
            {
                previous.handler.handle(marker != null ? OVERWRITTEN : REMOVED);
            }

            if (marker != null && currentTimecode >= marker.timecode)
            {
                trigger(marker, LATE);
            }
        }

        /// <summary>
        /// Remove the current marker. </summary>
        /// <returns> The removed marker. </returns>
        public virtual TrackMarker remove()
        {
            return current.getAndSet(null);
        }

        /// <summary>
        /// Trigger and remove the marker with the specified state. </summary>
        /// <param name="state"> The state of the marker to pass to the handler. </param>
        public virtual void trigger(TrackMarkerHandler_MarkerState state)
        {
            TrackMarker marker = current.getAndSet(null);

            if (marker != null)
            {
                marker.handler.handle(state);
            }
        }

        /// <summary>
        /// Check a timecode which was reached by normal playback, trigger REACHED if necessary. </summary>
        /// <param name="timecode"> Timecode which was reached by normal playback. </param>
        public virtual void checkPlaybackTimecode(long timecode)
        {
            TrackMarker marker = current.get();

            if (marker != null && timecode >= marker.timecode)
            {
                trigger(marker, REACHED);
            }
        }

        /// <summary>
        /// Check a timecode which was reached by seeking, trigger BYPASSED if necessary. </summary>
        /// <param name="timecode"> Timecode which was reached by seeking. </param>
        public virtual void checkSeekTimecode(long timecode)
        {
            TrackMarker marker = current.get();

            if (marker != null && timecode >= marker.timecode)
            {
                trigger(marker, BYPASSED);
            }
        }

        private void trigger(TrackMarker marker, TrackMarkerHandler_MarkerState state)
        {
            if (current.compareAndSet(marker, null))
            {
                marker.handler.handle(state);
            }
        }
    }
}
