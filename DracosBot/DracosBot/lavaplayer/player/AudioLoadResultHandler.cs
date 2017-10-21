using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sedmelluq.discord.lavaplayer.player
{
    using FriendlyException = com.sedmelluq.discord.lavaplayer.tools.FriendlyException;
    using AudioPlaylist = com.sedmelluq.discord.lavaplayer.track.AudioPlaylist;
    using AudioTrack = com.sedmelluq.discord.lavaplayer.track.AudioTrack;

    /// <summary>
    /// Handles the result of loading an item from an audio player manager.
    /// </summary>
    public interface AudioLoadResultHandler
    {
        /// <summary>
        /// Called when the requested item is a track and it was successfully loaded. </summary>
        /// <param name="track"> The loaded track </param>
        void trackLoaded(AudioTrack track);

        /// <summary>
        /// Called when the requested item is a playlist and it was successfully loaded. </summary>
        /// <param name="playlist"> The loaded playlist </param>
        void playlistLoaded(AudioPlaylist playlist);

        /// <summary>
        /// Called when there were no items found by the specified identifier.
        /// </summary>
        void noMatches();

        /// <summary>
        /// Called when loading an item failed with an exception. </summary>
        /// <param name="exception"> The exception that was thrown </param>
        void loadFailed(FriendlyException exception);
    }
}
