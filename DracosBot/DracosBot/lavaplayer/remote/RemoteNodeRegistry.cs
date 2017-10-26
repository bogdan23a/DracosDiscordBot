using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sedmelluq.discord.lavaplayer.remote
{
    using AudioTrack = com.sedmelluq.discord.lavaplayer.track.AudioTrack;

    /// <summary>
    /// Registry of currently used remote nodes by a player manager.
    /// </summary>
    public interface RemoteNodeRegistry
    {
        /// <returns> True if using remote nodes for audio processing is enabled. </returns>
        bool Enabled { get; }

        /// <summary>
        /// Finds the node which is playing the specified track.
        /// </summary>
        /// <param name="track"> The track to check. </param>
        /// <returns> The node which is playing this track, or null if no node is playing it. </returns>
        RemoteNode getNodeUsedForTrack(AudioTrack track);

        /// <returns> List of all nodes currently in use (including ones which are offline). </returns>
        IList<RemoteNode> Nodes { get; }
    }
}
