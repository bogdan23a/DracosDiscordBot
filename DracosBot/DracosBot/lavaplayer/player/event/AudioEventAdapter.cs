using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sedmelluq.discord.lavaplayer.player.@event
{
    using FriendlyException = com.sedmelluq.discord.lavaplayer.tools.FriendlyException;
    using AudioTrack = com.sedmelluq.discord.lavaplayer.track.AudioTrack;
    using AudioTrackEndReason = com.sedmelluq.discord.lavaplayer.track.AudioTrackEndReason;

    /// <summary>
    /// Adapter for different event handlers as method overrides
    /// </summary>
    public abstract class AudioEventAdapter : AudioEventListener
    {
        /// <param name="player"> Audio player </param>
        public virtual void onPlayerPause(AudioPlayer player)
        {
            // Adapter dummy method
        }

        /// <param name="player"> Audio player </param>
        public virtual void onPlayerResume(AudioPlayer player)
        {
            // Adapter dummy method
        }

        /// <param name="player"> Audio player </param>
        /// <param name="track"> Audio track that started </param>
        public virtual void onTrackStart(AudioPlayer player, AudioTrack track)
        {
            // Adapter dummy method
        }

        /// <param name="player"> Audio player </param>
        /// <param name="track"> Audio track that ended </param>
        /// <param name="endReason"> The reason why the track stopped playing </param>
        public virtual void onTrackEnd(AudioPlayer player, AudioTrack track, AudioTrackEndReason endReason)
        {
            // Adapter dummy method
        }

        /// <param name="player"> Audio player </param>
        /// <param name="track"> Audio track where the exception occurred </param>
        /// <param name="exception"> The exception that occurred </param>
        public virtual void onTrackException(AudioPlayer player, AudioTrack track, FriendlyException exception)
        {
            // Adapter dummy method
        }

        /// <param name="player"> Audio player </param>
        /// <param name="track"> Audio track where the exception occurred </param>
        /// <param name="thresholdMs"> The wait threshold that was exceeded for this event to trigger </param>
        public virtual void onTrackStuck(AudioPlayer player, AudioTrack track, long thresholdMs)
        {
            // Adapter dummy method
        }

        public void onEvent(AudioEvent @event)
        {
            if (@event is PlayerPauseEvent)
            {
                onPlayerPause(@event.player);
            }
            else if (@event is PlayerResumeEvent)
            {
                onPlayerResume(@event.player);
            }
            else if (@event is TrackStartEvent)
            {
                onTrackStart(@event.player, ((TrackStartEvent)@event).track);
            }
            else if (@event is TrackEndEvent)
            {
                onTrackEnd(@event.player, ((TrackEndEvent)@event).track, ((TrackEndEvent)@event).endReason);
            }
            else if (@event is TrackExceptionEvent)
            {
                onTrackException(@event.player, ((TrackExceptionEvent)@event).track, ((TrackExceptionEvent)@event).exception);
            }
            else if (@event is TrackStuckEvent)
            {
                onTrackStuck(@event.player, ((TrackStuckEvent)@event).track, ((TrackStuckEvent)@event).thresholdMs);
            }
        }
    }
}

