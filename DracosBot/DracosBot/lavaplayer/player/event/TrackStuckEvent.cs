using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sedmelluq.discord.lavaplayer.player.@event
{
    using AudioTrack = com.sedmelluq.discord.lavaplayer.track.AudioTrack;

    /// <summary>
    /// Event that is fired when a track was started, but no audio frames from it have arrived in a long time, specified
    /// by the threshold set via AudioPlayerManager.setTrackStuckThreshold().
    /// </summary>
    public class TrackStuckEvent : AudioEvent
    {
        /// <summary>
        /// Audio track where the exception occurred
        /// </summary>
        public readonly AudioTrack track;
        /// <summary>
        /// The wait threshold that was exceeded for this event to trigger
        /// </summary>
        public readonly long thresholdMs;

        /// <param name="player"> Audio player </param>
        /// <param name="track"> Audio track where the exception occurred </param>
        /// <param name="thresholdMs"> The wait threshold that was exceeded for this event to trigger </param>
        public TrackStuckEvent(AudioPlayer player, AudioTrack track, long thresholdMs) : base(player)
        {
            this.track = track;
            this.thresholdMs = thresholdMs;
        }
    }
}
