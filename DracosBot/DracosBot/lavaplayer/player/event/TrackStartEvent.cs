using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sedmelluq.discord.lavaplayer.player.@event
{
    using AudioTrack = com.sedmelluq.discord.lavaplayer.track.AudioTrack;

    /// <summary>
    /// Event that is fired when a track starts playing.
    /// </summary>
    public class TrackStartEvent : AudioEvent
    {
        /// <summary>
        /// Audio track that started
        /// </summary>
        public readonly AudioTrack track;

        /// <param name="player"> Audio player </param>
        /// <param name="track"> Audio track that started </param>
        public TrackStartEvent(AudioPlayer player, AudioTrack track) : base(player)
        {
            this.track = track;
        }
    }
}
