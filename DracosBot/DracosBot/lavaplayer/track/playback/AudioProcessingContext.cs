using java.util.concurrent.atomic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sedmelluq.discord.lavaplayer.track.playback
{
    using AudioDataFormat = com.sedmelluq.discord.lavaplayer.format.AudioDataFormat;
    using AudioConfiguration = com.sedmelluq.discord.lavaplayer.player.AudioConfiguration;

    /// <summary>
    /// Context for processing audio. Contains configuration for encoding and the output where the frames go to.
    /// </summary>
    public class AudioProcessingContext
    {
        /// <summary>
        /// Audio encoding or filtering related configuration
        /// </summary>
        public readonly AudioConfiguration configuration;
        /// <summary>
        /// Consumer for the produced audio frames
        /// </summary>
        public readonly AudioFrameConsumer frameConsumer;
        /// <summary>
        /// Mutable volume level for the audio
        /// </summary>
        public readonly AtomicInteger volumeLevel;
        /// <summary>
        /// Output format to use throughout this processing cycle
        /// </summary>
        public readonly AudioDataFormat outputFormat;

        /// <param name="configuration"> Audio encoding or filtering related configuration </param>
        /// <param name="frameConsumer"> Consumer for the produced audio frames </param>
        /// <param name="volumeLevel"> Mutable volume level for the audio </param>
        /// <param name="outputFormat"> Output format to use throughout this processing cycle </param>
        public AudioProcessingContext(AudioConfiguration configuration, AudioFrameConsumer frameConsumer, AtomicInteger volumeLevel, AudioDataFormat outputFormat)
        {

            this.configuration = configuration;
            this.frameConsumer = frameConsumer;
            this.volumeLevel = volumeLevel;
            this.outputFormat = outputFormat;
        }
    }
}

