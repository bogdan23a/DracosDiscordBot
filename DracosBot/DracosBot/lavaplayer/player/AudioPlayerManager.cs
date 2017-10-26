using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sharpen;

namespace com.sedmelluq.discord.lavaplayer.player
{
    using AudioOutputHookFactory = com.sedmelluq.discord.lavaplayer.player.hook.AudioOutputHookFactory;
    using RemoteNodeRegistry = com.sedmelluq.discord.lavaplayer.remote.RemoteNodeRegistry;
    using AudioSourceManager = com.sedmelluq.discord.lavaplayer.source.AudioSourceManager;
    using MessageInput = com.sedmelluq.discord.lavaplayer.tools.io.MessageInput;
    using MessageOutput = com.sedmelluq.discord.lavaplayer.tools.io.MessageOutput;
    using AudioTrack = com.sedmelluq.discord.lavaplayer.track.AudioTrack;
    using DecodedTrackHolder = com.sedmelluq.discord.lavaplayer.track.DecodedTrackHolder;
    using RequestConfig = Apache.Http.Client.HttpRequestRetryHandler;


    /// <summary>
    /// Audio player manager which is used for creating audio players and loading tracks and playlists.
    /// </summary>
    public interface AudioPlayerManager
    {

        /// <summary>
        /// Shut down the manager. All threads will be stopped, the manager cannot be used any further. All players created
        /// with this manager will stop and all source managers registered to this manager will also be shut down.
        /// 
        /// Every thread created by the audio manager is a daemon thread, so calling this is not required for an application
        /// to be able to gracefully shut down, however it should be called if the application continues without requiring this
        /// manager any longer.
        /// </summary>
        void shutdown();

        /// <summary>
        /// Set the factory for audio output hooks for the players created by this manager. An audio output hook gets called
        /// for every audio frame leaving the audio player and may also change the return value to swap out or discard an audio
        /// frame.
        /// </summary>
        /// <param name="outputHookFactory"> Audio output hook factory </param>
        AudioOutputHookFactory OutputHookFactory { set; }

        /// <summary>
        /// Configure to use remote nodes for playback. On consecutive calls, the connections with previously used nodes will
        /// be severed and all remotely playing tracks will be stopped first.
        /// </summary>
        /// <param name="nodeAddresses"> The addresses of the remote nodes </param>
        void useRemoteNodes(params string[] nodeAddresses);

        /// <summary>
        /// Enable reporting GC pause length statistics to log (warn level with lengths bad for latency, debug level otherwise)
        /// </summary>
        void enableGcMonitoring();

        /// <param name="sourceManager"> The source manager to register, which will be used for subsequent loadItem calls </param>
        void registerSourceManager(AudioSourceManager sourceManager);

        /// <summary>
        /// Shortcut for accessing a source manager of a certain class. </summary>
        /// <param name="klass"> The class of the source manager to return. </param>
        /// @param <T> The class of the source manager. </param>
        /// <returns> The source manager of the specified class, or null if not registered. </returns>
        T source<T>(Type klass);

        /// <summary>
        /// Schedules loading a track or playlist with the specified identifier. </summary>
        /// <param name="identifier">    The identifier that a specific source manager should be able to find the track with. </param>
        /// <param name="resultHandler"> A handler to process the result of this operation. It can either end by finding a track,
        ///                      finding a playlist, finding nothing or terminating with an exception. </param>
        /// <returns> A future for this operation </returns>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: java.util.concurrent.Future<Void> loadItem(final String identifier, final AudioLoadResultHandler resultHandler);
        Future<System.Type> loadItem(string identifier, AudioLoadResultHandler resultHandler);

        /// <summary>
        /// Schedules loading a track or playlist with the specified identifier with an ordering key so that items with the
        /// same ordering key are handled sequentially in the order of calls to this method.
        /// </summary>
        /// <param name="orderingKey">   Object to use as the key for the ordering channel </param>
        /// <param name="identifier">    The identifier that a specific source manager should be able to find the track with. </param>
        /// <param name="resultHandler"> A handler to process the result of this operation. It can either end by finding a track,
        ///                      finding a playlist, finding nothing or terminating with an exception. </param>
        /// <returns> A future for this operation </returns>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: java.util.concurrent.Future<Void> loadItemOrdered(Object orderingKey, final String identifier, final AudioLoadResultHandler resultHandler);
        Future<System.Type> loadItemOrdered(object orderingKey, string identifier, AudioLoadResultHandler resultHandler);

        /// <summary>
        /// Encode a track into an output stream. If the decoder is not supposed to know the number of tracks in advance, then
        /// the encoder should call MessageOutput#finish() after all the tracks it wanted to write have been written. This will
        /// make decodeTrack() return null at that position
        /// </summary>
        /// <param name="stream"> The message stream to write it to. </param>
        /// <param name="track"> The track to encode. </param>
        /// <exception cref="IOException"> On IO error. </exception>
    }
}


