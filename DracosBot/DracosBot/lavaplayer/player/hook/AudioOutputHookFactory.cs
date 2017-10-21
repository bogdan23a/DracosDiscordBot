using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sedmelluq.discord.lavaplayer.player.hook
{
    /// <summary>
    /// Factory for audio output hook instances.
    /// </summary>
    public interface AudioOutputHookFactory
    {
        /// <returns> New instance of an audio output hook </returns>
        AudioOutputHook createOutputHook();
    }
}
