using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sedmelluq.discord.lavaplayer.remote.message
{
    /// <summary>
    /// Message to notify the node that the track has been stopped in the master. No more requests for that track will occur
    /// so the track may be deleted from the node.
    /// </summary>
    public class TrackStoppedMessage : RemoteMessage
    {
        /// <summary>
        /// The ID for the track executor
        /// </summary>
        public readonly long executorId;

        /// <param name="executorId"> The ID for the track executor </param>
        public TrackStoppedMessage(long executorId)
        {
            this.executorId = executorId;
        }
    }
}
