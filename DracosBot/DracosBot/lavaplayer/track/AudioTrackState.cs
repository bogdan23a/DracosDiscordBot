using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sedmelluq.discord.lavaplayer.track
{
    /// <summary>
    /// The execution state of an audio track
    /// </summary>
    public enum AudioTrackState
    {
        INACTIVE,
        LOADING,
        PLAYING,
        SEEKING,
        FINISHED
    }
}
