using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sedmelluq.discord.lavaplayer.track
{
    using AudioPlayerManager = com.sedmelluq.discord.lavaplayer.player.AudioPlayerManager;
    using AudioFrameProvider = com.sedmelluq.discord.lavaplayer.track.playback.AudioFrameProvider;
    using AudioTrackExecutor = com.sedmelluq.discord.lavaplayer.track.playback.AudioTrackExecutor;
    using LocalAudioTrackExecutor = com.sedmelluq.discord.lavaplayer.track.playback.LocalAudioTrackExecutor;

    /// <summary>
    /// Methods of an audio track that should not be visible outside of the library
    /// </summary>
    public interface InternalAudioTrack : AudioTrack, AudioFrameProvider
    {
        /// <param name="executor"> Executor to assign to the track </param>
        /// <param name="applyPrimordialState"> True if the state previously applied to this track should be copied to new executor. </param>
        void assignExecutor(AudioTrackExecutor executor, bool applyPrimordialState);

        /// <returns> Get the active track executor </returns>
        AudioTrackExecutor ActiveExecutor { get; }

        /// <summary>
        /// Perform any necessary loading and then enter the read/seek loop </summary>
        /// <param name="executor"> The local executor which processes this track </param>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: void process(com.sedmelluq.discord.lavaplayer.track.playback.LocalAudioTrackExecutor executor) throws Exception;
        void process(LocalAudioTrackExecutor executor);

        /// <param name="playerManager"> The player manager which is executing this track </param>
        /// <returns> A custom local executor for this track. Unless this track requires a special executor, this should return
        ///         null as the default one will be used in that case. </returns>
        AudioTrackExecutor createLocalExecutor(AudioPlayerManager playerManager);
    }
}
