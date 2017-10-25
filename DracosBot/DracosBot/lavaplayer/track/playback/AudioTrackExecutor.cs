using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sedmelluq.discord.lavaplayer.track.playback
{

    /// <summary>
    /// Executor which handles track execution and all operations on playing tracks.
    /// </summary>
    public interface AudioTrackExecutor : AudioFrameProvider
    {
        /// <returns> The audio buffer of this executor. </returns>
        AudioFrameBuffer AudioBuffer { get; }

        /// <summary>
        /// Execute the track, which means that this thread will fill the frame buffer until the track finishes or is stopped. </summary>
        /// <param name="listener"> Listener for track state events </param>
        void execute(TrackStateListener listener);

        /// <summary>
        /// Stop playing the track, terminating the thread that is filling the frame buffer.
        /// </summary>
        void stop();

        /// <returns> Timecode of the last played frame or in case a seek is in progress, the timecode of the frame being seeked to. </returns>
        long Position { get; set; }


        /// <returns> Current state of the executor </returns>
        AudioTrackState State { get; }

        /// <summary>
        /// Set track position marker. </summary>
        /// <param name="marker"> Track position marker to set. </param>
        TrackMarker Marker { set; }

        /// <returns> True if this track threw an exception before it provided any audio. </returns>
        bool failedBeforeLoad();
    }
}
