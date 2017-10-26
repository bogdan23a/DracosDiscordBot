using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sedmelluq.discord.lavaplayer.remote.message
{
    using AudioConfiguration = com.sedmelluq.discord.lavaplayer.player.AudioConfiguration;
    using AudioTrackInfo = com.sedmelluq.discord.lavaplayer.track.AudioTrackInfo;

    /// <summary>
    /// The message that is sent to the node when the master requests the node to start playing a track.
    /// </summary>
    public class TrackStartRequestMessage : RemoteMessage
    {
        /// <summary>
        /// The ID for the track executor
        /// </summary>
        public readonly long executorId;
        /// <summary>
        /// Generic track information
        /// </summary>
        public readonly AudioTrackInfo trackInfo;
        /// <summary>
        /// Track specific extra information that is required to initialise the track object
        /// </summary>
        public readonly sbyte[] encodedTrack;
        /// <summary>
        /// Initial volume of the track
        /// </summary>
        public readonly int volume;
        /// <summary>
        /// Configuration to use for audio processing
        /// </summary>
        public readonly AudioConfiguration configuration;
        /// <summary>
        /// Position to start playing at in milliseconds
        /// </summary>
        public readonly long position;

        /// <param name="executorId"> The ID for the track executor </param>
        /// <param name="trackInfo"> Generic track information </param>
        /// <param name="encodedTrack"> Track specific extra information that is required to initialise the track object </param>
        /// <param name="volume"> Initial volume of the track </param>
        /// <param name="configuration"> Configuration to use for audio processing </param>
        /// <param name="position"> Position to start playing at in milliseconds </param>
        public TrackStartRequestMessage(long executorId, AudioTrackInfo trackInfo, sbyte[] encodedTrack, int volume, AudioConfiguration configuration, long position)
        {

            this.executorId = executorId;
            this.encodedTrack = encodedTrack;
            this.trackInfo = trackInfo;
            this.volume = volume;
            this.configuration = configuration;
            this.position = position;
        }
    }
}
