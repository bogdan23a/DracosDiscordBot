using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sedmelluq.discord.lavaplayer.track.playback
{
    /// <summary>
    /// Interface for classes which can rebuild audio frames.
    /// </summary>
    public interface AudioFrameRebuilder
    {
        /// <summary>
        /// Rebuilds a frame (for example by reencoding) </summary>
        /// <param name="frame"> The audio frame </param>
        /// <returns> The new frame (may be the same as input) </returns>
        AudioFrame rebuild(AudioFrame frame);
    }
}
