using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sedmelluq.discord.lavaplayer.player
{
    using AudioEvent = com.sedmelluq.discord.lavaplayer.player.@event.AudioEvent;
    using AudioEventListener = com.sedmelluq.discord.lavaplayer.player.@event.AudioEventListener;
    using PlayerPauseEvent = com.sedmelluq.discord.lavaplayer.player.@event.PlayerPauseEvent;
    using PlayerResumeEvent = com.sedmelluq.discord.lavaplayer.player.@event.PlayerResumeEvent;
    using TrackEndEvent = com.sedmelluq.discord.lavaplayer.player.@event.TrackEndEvent;
    using TrackExceptionEvent = com.sedmelluq.discord.lavaplayer.player.@event.TrackExceptionEvent;
    using TrackStartEvent = com.sedmelluq.discord.lavaplayer.player.@event.TrackStartEvent;
    using TrackStuckEvent = com.sedmelluq.discord.lavaplayer.player.@event.TrackStuckEvent;
    using AudioOutputHook = com.sedmelluq.discord.lavaplayer.player.hook.AudioOutputHook;
    using FriendlyException = com.sedmelluq.discord.lavaplayer.tools.FriendlyException;
    using AudioTrackEndReason = com.sedmelluq.discord.lavaplayer.track.AudioTrackEndReason;
    using InternalAudioTrack = com.sedmelluq.discord.lavaplayer.track.InternalAudioTrack;
    using TrackStateListener = com.sedmelluq.discord.lavaplayer.track.TrackStateListener;
    using AudioFrame = com.sedmelluq.discord.lavaplayer.track.playback.AudioFrame;
    using AudioFrameProvider = com.sedmelluq.discord.lavaplayer.track.playback.AudioFrameProvider;
    using AudioTrack = com.sedmelluq.discord.lavaplayer.track.AudioTrack;
    using AudioFrameProviderTools = com.sedmelluq.discord.lavaplayer.track.playback.AudioFrameProviderTools;
    using Logger = org.slf4j.Logger;
    using LoggerFactory = org.slf4j.LoggerFactory;


    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static com.sedmelluq.discord.lavaplayer.track.AudioTrackEndReason.CLEANUP;
    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static com.sedmelluq.discord.lavaplayer.track.AudioTrackEndReason.FINISHED;
    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static com.sedmelluq.discord.lavaplayer.track.AudioTrackEndReason.LOAD_FAILED;
    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static com.sedmelluq.discord.lavaplayer.track.AudioTrackEndReason.REPLACED;
    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static com.sedmelluq.discord.lavaplayer.track.AudioTrackEndReason.STOPPED;

    /// <summary>
    /// An audio player that is capable of playing audio tracks and provides audio frames from the currently playing track.
    /// </summary>
    public class AudioPlayer : AudioFrameProvider, TrackStateListener
    {
        private static readonly Logger log = LoggerFactory.getLogger(typeof(AudioPlayer));

        private volatile InternalAudioTrack activeTrack;
        private volatile long lastRequestTime;
        private volatile long lastReceiveTime;
        private volatile bool stuckEventSent;
        private volatile InternalAudioTrack shadowTrack;
        private readonly AtomicBoolean paused;
        private readonly DefaultAudioPlayerManager manager;
        private readonly IList<AudioEventListener> listeners;
        private readonly AtomicInteger volumeLevel;
        private readonly AudioOutputHook outputHook;
        private readonly object trackSwitchLock;

        /// <param name="manager"> Audio player manager which this player is attached to </param>
        /// <param name="outputHook"> Hook which can intercept outgoing audio frames </param>
        public AudioPlayer(DefaultAudioPlayerManager manager, AudioOutputHook outputHook)
        {
            this.manager = manager;
            this.outputHook = outputHook;
            activeTrack = null;
            paused = new AtomicBoolean();
            listeners = new List<>();
            volumeLevel = new AtomicInteger(100);
            trackSwitchLock = new object();
        }

        /// <returns> Currently playing track </returns>
        public virtual AudioTrack PlayingTrack
        {
            get
            {
                return activeTrack;
            }
        }

        /// <param name="track"> The track to start playing </param>
        public virtual void playTrack(AudioTrack track)
        {
            startTrack(track, false);
        }

        /// <param name="track"> The track to start playing, passing null will stop the current track and return false </param>
        /// <param name="noInterrupt"> Whether to only start if nothing else is playing </param>
        /// <returns> True if the track was started </returns>
        public virtual bool startTrack(AudioTrack track, bool noInterrupt)
        {
            InternalAudioTrack newTrack = (InternalAudioTrack)track;
            InternalAudioTrack previousTrack;

            lock (trackSwitchLock)
            {
                previousTrack = activeTrack;

                if (noInterrupt && previousTrack != null)
                {
                    return false;
                }
            }
            activeTrack = newTrack;
            lastRequestTime = DateTimeHelperClass.CurrentUnixTimeMillis();
            lastReceiveTime = System.nanoTime();
            stuckEventSent = false;

            if (previousTrack != null)
            {
                previousTrack.stop();
                dispatchEvent(new TrackEndEvent(this, previousTrack, newTrack == null ? STOPPED : REPLACED));

                shadowTrack = previousTrack;
            }


            if (newTrack == null)
            {
                shadowTrack = null;
                return false;
            }
            dispatchEvent(new TrackStartEvent(this, newTrack));

            manager.executeTrack(this, newTrack, manager.Configuration, volumeLevel);
            return true;


        }

        /// <summary>
        /// Stop currently playing track.
        /// </summary>
        public void stopTrack()
    {
        stopWithReason(STOPPED);
    }

    private void stopWithReason(AudioTrackEndReason reason)
    {
        shadowTrack = null;

        lock (trackSwitchLock)
        {
            InternalAudioTrack previousTrack = activeTrack;
            activeTrack = null;

            if (previousTrack != null)
            {
                previousTrack.stop();
                dispatchEvent(new TrackEndEvent(this, previousTrack, reason));
            }
        }
    }

    private AudioFrame provideShadowFrame()
    {
        InternalAudioTrack shadow = shadowTrack;
        AudioFrame frame = null;

        if (shadow != null)
        {
            frame = shadow.provide();

            if (frame != null && frame.Terminator)
            {
                shadowTrack = null;
                frame = null;
            }
        }

        return frame;
    }

    public AudioFrame provide()
    {
        return AudioFrameProviderTools.delegateToTimedProvide(this);
    }

    public AudioFrame provide(long timeout, TimeUnit unit) throws TimeoutException, InterruptedException
  {
	AudioFrame frame = provideDirectly(timeout, unit);
	if (outputHook != null)
	{
	  frame = outputHook.outgoingFrame(this, frame);
    }
	return frame;
  

/// <summary>
/// Provide an audio frame bypassing hooks. </summary>
/// <param name="timeout"> Specifies the maximum time to wait for data. Pass 0 for non-blocking mode. </param>
/// <param name="unit"> Specifies the time unit of the maximum wait time. </param>
/// <returns> An audio frame if available, otherwise null </returns>
public AudioFrame provideDirectly(long timeout, TimeUnit unit) throws TimeoutException, InterruptedException
  {
	InternalAudioTrack track;

lastRequestTime = DateTimeHelperClass.CurrentUnixTimeMillis();

	if (timeout == 0 && paused.get())
	{
	  return null;
	}

	while ((track = activeTrack) != null)
	{
	  AudioFrame frame = timeout > 0 ? track.provide(timeout, unit) : track.provide();

	  if (frame != null)
	  {
		lastReceiveTime = System.nanoTime();
		shadowTrack = null;

		if (frame.Terminator)
		{

          handleTerminator(track);
		  continue;
		}


	  }
	  else if (timeout == 0)
	  {

        checkStuck(track);

frame = provideShadowFrame();
	  }

	  return frame;
	}

	return null;
  }

  private void handleTerminator(InternalAudioTrack track)
{
    lock (trackSwitchLock)
    {
        if (activeTrack == track)
        {
            activeTrack = null;

            dispatchEvent(new TrackEndEvent(this, track, track.ActiveExecutor.failedBeforeLoad() ? LOAD_FAILED : FINISHED));
        }
    }
}

private void checkStuck(AudioTrack track)
{
    if (!stuckEventSent && System.nanoTime() - lastReceiveTime > manager.TrackStuckThresholdNanos)
    {
        stuckEventSent = true;
        dispatchEvent(new TrackStuckEvent(this, track, TimeUnit.NANOSECONDS.toMillis(manager.TrackStuckThresholdNanos)));
    }
}

public int Volume
{
	return volumeLevel.get();
    }
}
  public void setVolume(int volume)
{
    volumeLevel.set(Math.Min(150, Math.Max(0, volume)));
}

/// <returns> Whether the player is paused </returns>
public bool Paused
{
	return paused.get();
    }
}

  /// <param name="value"> True to pause, false to resume </param>
public void setPaused(bool value)
{
    if (paused.compareAndSet(!value, value))
    {
        if (value)
        {
            dispatchEvent(new PlayerPauseEvent(this));
        }
        else
        {
            dispatchEvent(new PlayerResumeEvent(this));
            lastReceiveTime = System.nanoTime();
        }
    }
}

/// <summary>
/// Destroy the player and stop playing track.
/// </summary>
public void destroy()
{
    stopTrack();
}

/// <summary>
/// Add a listener to events from this player. </summary>
/// <param name="listener"> New listener </param>
public void addListener(AudioEventListener listener)
{
    lock (trackSwitchLock)
    {
        listeners.add(listener);
    }
}

/// <summary>
/// Remove an attached listener using identity comparison. </summary>
/// <param name="listener"> The listener to remove </param>
public void removeListener(AudioEventListener listener)
{
    lock (trackSwitchLock)
    {
        for (IEnumerator<AudioEventListener> iterator = listeners.GetEnumerator(); iterator.MoveNext();)
        {
            if (iterator.Current == listener)
            {
                //JAVA TO C# CONVERTER TODO TASK: .NET enumerators are read-only:
                iterator.remove();
            }
        }
    }
}
private void dispatchEvent(AudioEvent @event)
{
    log.debug("Firing an event with class {}", @event.GetType().Name);

    lock (trackSwitchLock)
    {
        foreach (AudioEventListener listener in listeners)
        {
            try
            {
                listener.onEvent(@event);
            }
            catch (Exception e)
            {
                log.error("Handler of event {} threw an exception.", @event, e);
            }
        }
    }
}

public override void onTrackException(AudioTrack track, FriendlyException exception)
{
    dispatchEvent(new TrackExceptionEvent(this, track, exception));
}

public override void onTrackStuck(AudioTrack track, long thresholdMs)
{
    dispatchEvent(new TrackStuckEvent(this, track, thresholdMs));
}

/// <summary>
/// Check if the player should be "cleaned up" - stopped due to nothing using it, with the given threshold. </summary>
/// <param name="threshold"> Threshold in milliseconds to use </param>
public virtual void checkCleanup(long threshold)
{
    AudioTrack track = PlayingTrack;
    if (track != null && DateTimeHelperClass.CurrentUnixTimeMillis() - lastRequestTime >= threshold)
    {
        log.debug("Triggering cleanup on an audio player playing track {}", track);

        stopWithReason(CLEANUP);
    }
}

