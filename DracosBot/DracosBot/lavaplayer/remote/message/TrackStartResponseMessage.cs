using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sedmelluq.discord.lavaplayer.remote.message
{
    /// <summary>
    /// This is the response to a TrackStartRequestMessage. It indicates whether the track was successfully started on the
    /// node. This does not guarantee that any frames from the track will arrive, only that its execution was submitted.
    /// </summary>
    public class TrackStartResponseMessage : RemoteMessage
    {
        /// <summary>
        /// The ID for the track executor
        /// </summary>
        public readonly long executorId;
        /// <summary>
        /// Whether the track was successfully started in the node
        /// </summary>
        public readonly bool success;
        /// <summary>
        /// The reason in case the track was not started
        /// </summary>
        public readonly string failureReason;

        /// <param name="executorId"> The ID for the track executor </param>
        /// <param name="success"> Whether the track was successfully started in the node </param>
        /// <param name="failureReason"> The reason in case the track was not started </param>
        public TrackStartResponseMessage(long executorId, bool success, string failureReason)
        {
            this.executorId = executorId;
            this.success = success;
            this.failureReason = failureReason;
        }
    }
}
