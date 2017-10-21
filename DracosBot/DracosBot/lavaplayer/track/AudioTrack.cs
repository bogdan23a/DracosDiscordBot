using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sedmelluq.discord.lavaplayer.track
{
    using AudioSourceManager = com.sedmelluq.discord.lavaplayer.source.AudioSourceManager;

    /// <summary>
    /// A playable audio track
    /// </summary>
    public interface AudioTrack : AudioItem
    {
        /// <returns> Track meta information </returns>
        AudioTrackInfo Info { get; }

        /// <returns> The identifier of the track </returns>
        string Identifier { get; }

        /// <returns> The current execution state of the track </returns>
        AudioTrackState State { get; }

        /// <summary>
        /// Stop the track if it is currently playing
        /// </summary>
        void stop();

        /// <returns> True if the track is seekable. </returns>
        bool Seekable { get; }

        /// <returns> Get the current position of the track in milliseconds </returns>
        long Position { get; set; }


        /// <param name="marker"> Track position marker to place </param>
        TrackMarker Marker { set; }

        /// <returns> Duration of the track in milliseconds </returns>
        long Duration { get; }

        /// <returns> Clone of this track which does not share the execution state of this track </returns>
        AudioTrack makeClone();

        /// <returns> The source manager which created this track. Null if not created by a source manager directly. </returns>
        AudioSourceManager SourceManager { get; }

        /// <summary>
        /// Attach an object with this track which can later be retrieved with <seealso cref="#getUserData()"/>. Useful for retrieving
        /// application-specific object from the track in callbacks.
        /// </summary>
        /// <param name="userData"> Object to store. </param>
        object UserData { set; get; }


        /// <returns> Object previously stored with <seealso cref="#setUserData(Object)"/> if it is of the specified type. If it is set,
        /// but with a different type, null is returned. </returns>
        T getUserData<T>(Type klass);
    }
}
