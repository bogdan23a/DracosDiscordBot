using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sedmelluq.discord.lavaplayer.remote.message
{
    /// <summary>
    /// A message sent to the node to request frames from a track. This is sent even when no frames are required as it is
    /// used to indicate that the track is still alive in the master.
    /// </summary>
    public class TrackFrameRequestMessage : RemoteMessage
    {
        /// <summary>
        /// The ID for the track executor
        /// </summary>
        public readonly long executorId;
        /// <summary>
        /// Maximum number of frames that can be included in the response
        /// </summary>
        public readonly int maximumFrames;
        /// <summary>
        /// Current volume of the track
        /// </summary>
        public readonly int volume;
        /// <summary>
        /// The position to seek to. Value is -1 if no seeking is required at the moment.
        /// </summary>
        public readonly long seekPosition;

        /// <param name="executorId"> The ID for the track executor </param>
        /// <param name="maximumFrames"> Maximum number of frames that can be included in the response </param>
        /// <param name="volume"> Current volume of the track </param>
        /// <param name="seekPosition"> The position to seek to </param>
        public TrackFrameRequestMessage(long executorId, int maximumFrames, int volume, long seekPosition)
        {
            this.executorId = executorId;
            this.maximumFrames = maximumFrames;
            this.volume = volume;
            this.seekPosition = seekPosition;
        }
    }
}
