using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sedmelluq.discord.lavaplayer.player.@event
{
    /// <summary>
    /// Listener of audio events.
    /// </summary>
    public interface AudioEventListener
    {
        /// <param name="event"> The event </param>
         void onEvent(AudioEvent @event);
    }
}
