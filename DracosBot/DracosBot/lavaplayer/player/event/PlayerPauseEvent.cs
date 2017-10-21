using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sedmelluq.discord.lavaplayer.player.@event
{

    /// <summary>
    /// Event that is fired when a player is paused.
    /// </summary>
    public class PlayerPauseEvent : AudioEvent
    {
        /// <param name="player"> Audio player </param>
        public PlayerPauseEvent(AudioPlayer player) : base(player)
        {
        }
    }
}
