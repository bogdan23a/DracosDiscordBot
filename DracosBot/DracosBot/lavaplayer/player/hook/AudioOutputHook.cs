using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sedmelluq.discord.lavaplayer.player.hook
{
    using AudioFrame = com.sedmelluq.discord.lavaplayer.track.playback.AudioFrame;

    /// <summary>
    /// Hook for intercepting outgoing audio frames from AudioPlayer.
    /// </summary>
    public interface AudioOutputHook
    {
        /// <param name="player"> Audio player where the frame is coming from </param>
        /// <param name="frame"> Audio frame </param>
        /// <returns> The frame to pass onto the actual caller </returns>
        AudioFrame outgoingFrame(AudioPlayer player, AudioFrame frame);
    }
}
