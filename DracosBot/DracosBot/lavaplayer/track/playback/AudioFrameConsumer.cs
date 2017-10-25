using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sedmelluq.discord.lavaplayer.track.playback
{
    /// <summary>
    /// A consumer for audio frames
    /// </summary>
    public interface AudioFrameConsumer
    {
        /// <summary>
        /// Consumes the frame, may block </summary>
        /// <param name="frame"> The frame to consume </param>
        /// <exception cref="InterruptedException"> When interrupted </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: void consume(AudioFrame frame) throws InterruptedException;
        void consume(AudioFrame frame);

        /// <summary>
        /// Rebuild all caches frames </summary>
        /// <param name="rebuilder"> The rebuilder to use </param>
        void rebuild(AudioFrameRebuilder rebuilder);
    }
}
