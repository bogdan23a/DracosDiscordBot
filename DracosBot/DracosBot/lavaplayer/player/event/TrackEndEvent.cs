using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sedmelluq.discord.lavaplayer.player.@event
{
    using AudioTrack = com.sedmelluq.discord.lavaplayer.track.AudioTrack;
    using AudioTrackEndReason = com.sedmelluq.discord.lavaplayer.track.AudioTrackEndReason;

    /// <summary>
    /// Event that is fired when an audio track ends in an audio player, either by interruption, exception or reaching the end.
    /// </summary>
    public class TrackEndEvent : AudioEvent
    {
        /// <summary>
        /// Audio track that ended
        /// </summary>
        public readonly AudioTrack track;
        /// <summary>
        /// The reason why the track stopped playing
        /// </summary>
        public readonly AudioTrackEndReason endReason;

        /// <param name="player"> Audio player </param>
        /// <param name="track"> Audio track that ended </param>
        /// <param name="endReason"> The reason why the track stopped playing </param>
        public TrackEndEvent(AudioPlayer player, AudioTrack track, AudioTrackEndReason endReason) : base(player)
        {
            this.track = track;
            this.endReason = endReason;
        }
    }
}
