using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sedmelluq.discord.lavaplayer.remote.message
{
    using AudioFrame = com.sedmelluq.discord.lavaplayer.track.playback.AudioFrame;

    /// <summary>
    /// Message from an audio node to the master as a response to TrackFrameRequestMessage. Bouncing back the seeked position
    /// is necessary because then the master can clear up the pending seek only if it matches this number. Otherwise the seek
    /// position has been changed while the data for the previous seek was requested. The master also cannot clear the seek
    /// state when sending the request, because in case the request fails, the seek will be discarded.
    /// </summary>
    public class TrackFrameDataMessage : RemoteMessage
    {
        /// <summary>
        /// The ID for the track executor
        /// </summary>
        public readonly long executorId;
        /// <summary>
        /// Frames provided by the node. These are missing the audio format, which must be attached locally. It can be assumed
        /// that the node provides data in the format that it was initially requested in.
        /// </summary>
        public readonly IList<AudioFrame> frames;
        /// <summary>
        /// If these are the last frames for the track. After receiving a message with this set to true, no more requests about
        /// this track should be made to the node as it has already deleted the track from its registry.
        /// </summary>
        public readonly bool finished;
        /// <summary>
        /// In case the data request included a seek, then this will report that the seek was completed by having the requested
        /// seek position as a value. When no seek was performed, this is -1. The frames returned in this message start from
        /// this position.
        /// </summary>
        public readonly long seekedPosition;

        /// <param name="executorId"> The ID for the track executor </param>
        /// <param name="frames"> Frames provided by the node </param>
        /// <param name="finished"> If these are the last frames for the track </param>
        /// <param name="seekedPosition"> The position of the seek that was performed </param>
        public TrackFrameDataMessage(long executorId, IList<AudioFrame> frames, bool finished, long seekedPosition)
        {
            this.executorId = executorId;
            this.frames = frames;
            this.finished = finished;
            this.seekedPosition = seekedPosition;
        }
    }
}
