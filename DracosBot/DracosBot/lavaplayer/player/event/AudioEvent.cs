using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sedmelluq.discord.lavaplayer.player.@event
{

    /// <summary>
    /// An event related to an audio player.
    /// </summary>
    public abstract class AudioEvent
    {
        /// <summary>
        /// The related audio player.
        /// </summary>
        public readonly AudioPlayer player;

        /// <param name="player"> The related audio player. </param>
        public AudioEvent(AudioPlayer player)
        {
            this.player = player;
        }
    }
}
