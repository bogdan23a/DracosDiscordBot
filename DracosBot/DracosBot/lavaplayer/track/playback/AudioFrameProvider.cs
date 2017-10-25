using java.util.concurrent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sedmelluq.discord.lavaplayer.track.playback
{

    /// <summary>
    /// A provider for audio frames
    /// </summary>
    public interface AudioFrameProvider
    {
        /// <returns> Provided frame, or null if none available </returns>
        AudioFrame provide();

        /// <param name="timeout"> Specifies the maximum time to wait for data. Pass 0 for non-blocking mode. </param>
        /// <param name="unit"> Specifies the time unit of the maximum wait time. </param>
        /// <returns> Provided frame. In case wait time is above zero, null indicates that no data is not available at the
        ///         current moment, otherwise null means the end of the track. </returns>
        /// <exception cref="TimeoutException"> When wait time is above zero, but no track info is found in that time. </exception>
        /// <exception cref="InterruptedException"> When wait is interrupted. </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: AudioFrame provide(long timeout, java.util.concurrent.TimeUnit unit) throws java.util.concurrent.TimeoutException, InterruptedException;
        AudioFrame provide(long timeout, TimeUnit unit);
    }
}
