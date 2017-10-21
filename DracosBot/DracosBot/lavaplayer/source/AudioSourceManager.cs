using java.io;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sedmelluq.discord.lavaplayer.source
{
    using DefaultAudioPlayerManager = com.sedmelluq.discord.lavaplayer.player.DefaultAudioPlayerManager;
    using AudioItem = com.sedmelluq.discord.lavaplayer.track.AudioItem;
    using AudioReference = com.sedmelluq.discord.lavaplayer.track.AudioReference;
    using AudioTrack = com.sedmelluq.discord.lavaplayer.track.AudioTrack;
    using AudioTrackInfo = com.sedmelluq.discord.lavaplayer.track.AudioTrackInfo;


    /// <summary>
    /// Manager for a source of audio items.
    /// </summary>
    public interface AudioSourceManager
    {
        /// <summary>
        /// Every source manager implementation should have its unique name as it is used to determine which source manager
        /// should be able to decode a serialized audio track.
        /// </summary>
        /// <returns> The name of this source manager </returns>
        string SourceName { get; }

        /// <summary>
        /// Returns an audio track for the input string. It should return null if it can immediately detect that there is no
        /// track for this identifier for this source. If checking that requires more expensive operations, then it should
        /// return a track instance and check that in InternalAudioTrack#loadTrackInfo.
        /// </summary>
        /// <param name="manager"> The audio manager to attach to the loaded tracks </param>
        /// <param name="reference"> The reference with the identifier which the source manager should find the track with </param>
        /// <returns> The loaded item or null on unrecognized identifier </returns>
        AudioItem loadItem(DefaultAudioPlayerManager manager, AudioReference reference);

        /// <summary>
        /// Returns whether the specified track can be encoded. The argument is always a track created by this manager. Being
        /// encodable also means that it must be possible to play this track on a different node, so it should not depend on
        /// any resources that are only available on the current system.
        /// </summary>
        /// <param name="track"> The track to check </param>
        /// <returns> True if it is encodable </returns>
        bool isTrackEncodable(AudioTrack track);

        /// <summary>
        /// Encodes an audio track into the specified output. The contents of AudioTrackInfo do not have to be included since
        /// they are written to the output already before this call. This will only be called for tracks which were loaded by
        /// this source manager and for which isEncodable() returns true.
        /// </summary>
        /// <param name="track"> The track to encode </param>
        /// <param name="output"> Output where to write the decoded format to </param>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: void encodeTrack(com.sedmelluq.discord.lavaplayer.track.AudioTrack track, java.io.DataOutput output) throws java.io.IOException;
        void encodeTrack(AudioTrack track, DataOutput output);

        /// <summary>
        /// Decodes an audio track from the encoded format encoded with encodeTrack().
        /// </summary>
        /// <param name="trackInfo"> The track info </param>
        /// <param name="input"> The input where to read the bytes of the encoded format </param>
        /// <returns> The decoded track </returns>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: com.sedmelluq.discord.lavaplayer.track.AudioTrack decodeTrack(com.sedmelluq.discord.lavaplayer.track.AudioTrackInfo trackInfo, java.io.DataInput input) throws java.io.IOException;
        AudioTrack decodeTrack(AudioTrackInfo trackInfo, DataInput input);

        /// <summary>
        /// Shut down the source manager, freeing all associated resources and threads. A source manager is not responsible for
        /// terminating the tracks that it has created.
        /// </summary>
        void shutdown();
    }
}
