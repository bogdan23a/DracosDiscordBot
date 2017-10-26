using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using java.util.concurrent;
using java.lang;

namespace com.sedmelluq.discord.lavaplayer.track.playback
{
    using ExceptionTools = com.sedmelluq.discord.lavaplayer.tools.ExceptionTools;


    /// <summary>
    /// Encapsulates common behavior shared by different audio frame providers.
    /// </summary>
    public class AudioFrameProviderTools
    {
        /// <param name="provider"> Delegates a call to frame provide without timeout to the timed version of it. </param>
        /// <returns> The audio frame from provide method. </returns>
        public static AudioFrame delegateToTimedProvide(AudioFrameProvider provider)
        {
            try
            {
                return provider.provide(0, TimeUnit.MILLISECONDS);
            }
            catch (System.Exception e) when (e is System.TimeoutException || e is InterruptedException)
            {
                ExceptionTools.keepInterrupted(e);
                throw new System.Exception(e);
            }
        }
    }
}

