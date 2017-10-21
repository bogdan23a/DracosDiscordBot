using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sedmelluq.discord.lavaplayer.remote.message
{
    /// <summary>
    /// A message detailing track and performance statistics that a node includes in every response.
    /// </summary>
    public class NodeStatisticsMessage : RemoteMessage
    {
        /// <summary>
        /// The number of tracks that are not paused
        /// </summary>
        public readonly int playingTrackCount;
        /// <summary>
        /// Total number of tracks being processed by the node
        /// </summary>
        public readonly int totalTrackCount;
        /// <summary>
        /// Total CPU usage of the system
        /// </summary>
        public readonly float systemCpuUsage;
        /// <summary>
        /// CPU usage of the node process
        /// </summary>
        public readonly float processCpuUsage;

        /// <param name="playingTrackCount"> The number of tracks that are not paused </param>
        /// <param name="totalTrackCount"> Total number of tracks being processed by the node </param>
        /// <param name="systemCpuUsage"> Total CPU usage of the machine </param>
        /// <param name="processCpuUsage"> CPU usage of the node process </param>
        public NodeStatisticsMessage(int playingTrackCount, int totalTrackCount, float systemCpuUsage, float processCpuUsage)
        {
            this.playingTrackCount = playingTrackCount;
            this.totalTrackCount = totalTrackCount;
            this.systemCpuUsage = systemCpuUsage;
            this.processCpuUsage = processCpuUsage;
        }
    }
}
