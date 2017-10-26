using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sedmelluq.discord.lavaplayer.remote.message
{
    using FriendlyException = com.sedmelluq.discord.lavaplayer.tools.FriendlyException;

    /// <summary>
    /// Track exception message which is sent by the node when processing the track in the node fails.
    /// </summary>
    public class TrackExceptionMessage : RemoteMessage
    {
        /// <summary>
        /// The ID for the track executor
        /// </summary>
        public readonly long executorId;
        /// <summary>
        /// Exception that was thrown by the local executor
        /// </summary>
        public readonly FriendlyException exception;

        /// <param name="executorId"> The ID for the track executor </param>
        /// <param name="exception"> Exception that was thrown by the local executor </param>
        public TrackExceptionMessage(long executorId, FriendlyException exception)
        {
            this.executorId = executorId;
            this.exception = exception;
        }
    }
}

