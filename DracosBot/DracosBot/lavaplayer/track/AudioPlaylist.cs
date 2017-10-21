using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Generic;

namespace com.sedmelluq.discord.lavaplayer.track
{

    /// <summary>
    /// Playlist of audio tracks
    /// </summary>
    public interface AudioPlaylist : AudioItem
    {
        /// <returns> Name of the playlist </returns>
        string Name { get; }

        /// <returns> List of tracks in the playlist </returns>
        IList<AudioTrack> Tracks { get; }

        /// <returns> Track that is explicitly selected, may be null. This same instance occurs in the track list. </returns>
        AudioTrack SelectedTrack { get; }

        /// <returns> True if the playlist was created from search results. </returns>
        bool SearchResult { get; }
    }
}

