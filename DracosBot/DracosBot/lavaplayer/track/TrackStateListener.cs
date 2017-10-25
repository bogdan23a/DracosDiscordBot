using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sedmelluq.discord.lavaplayer.track
{
    using FriendlyException = com.sedmelluq.discord.lavaplayer.tools.FriendlyException;

    /// <summary>
    /// Listener of track execution events.
    /// </summary>
    public interface TrackStateListener
    {
        /// <summary>
        /// Called when an exception occurs while a track is playing or loading. This is always fatal, but it may have left
        /// some data in the audio buffer which can still play until the buffer clears out.
        /// </summary>
        /// <param name="track"> The audio track for which the exception occurred </param>
        /// <param name="exception"> The exception that occurred </param>
        void onTrackException(AudioTrack track, FriendlyException exception);

        /// <summary>
        /// Called when an exception occurs while a track is playing or loading. This is always fatal, but it may have left
        /// some data in the audio buffer which can still play until the buffer clears out.
        /// </summary>
        /// <param name="track"> The audio track for which the exception occurred </param>
        /// <param name="thresholdMs"> The wait threshold that was exceeded for this event to trigger </param>
        void onTrackStuck(AudioTrack track, long thresholdMs);
    }
}

