using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sedmelluq.discord.lavaplayer.player.@event
{
    using FriendlyException = com.sedmelluq.discord.lavaplayer.tools.FriendlyException;
    using AudioTrack = com.sedmelluq.discord.lavaplayer.track.AudioTrack;

    /// <summary>
    /// Event that is fired when an exception occurs in an audio track that causes it to halt or not start.
    /// </summary>
    public class TrackExceptionEvent : AudioEvent
    {
        /// <summary>
        /// Audio track where the exception occurred
        /// </summary>
        public readonly AudioTrack track;
        /// <summary>
        /// The exception that occurred
        /// </summary>
        public readonly FriendlyException exception;

        /// <param name="player"> Audio player </param>
        /// <param name="track"> Audio track where the exception occurred </param>
        /// <param name="exception"> The exception that occurred </param>
        public TrackExceptionEvent(AudioPlayer player, AudioTrack track, FriendlyException exception) : base(player)
        {
            this.track = track;
            this.exception = exception;
        }
    }
}
