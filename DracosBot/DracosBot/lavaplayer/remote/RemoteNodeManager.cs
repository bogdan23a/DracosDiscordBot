using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Generic;
using System.Threading;
using Spring.Util;
using DefaultAudioPlayerManager = com.sedmelluq.discord.lavaplayer.player.DefaultAudioPlayerManager;
using AudioEventAdapter = com.sedmelluq.discord.lavaplayer.player.@event.AudioEventAdapter;
using DaemonThreadFactory = com.sedmelluq.discord.lavaplayer.tools.DaemonThreadFactory;
using ExecutorTools = com.sedmelluq.discord.lavaplayer.tools.ExecutorTools;
using FriendlyException = com.sedmelluq.discord.lavaplayer.tools.FriendlyException;
using HttpInterfaceManager = com.sedmelluq.discord.lavaplayer.tools.io.HttpInterfaceManager;
using AudioTrack = com.sedmelluq.discord.lavaplayer.track.AudioTrack;
using AudioTrackEndReason = com.sedmelluq.discord.lavaplayer.track.AudioTrackEndReason;
using InternalAudioTrack = com.sedmelluq.discord.lavaplayer.track.InternalAudioTrack;
using AudioTrackExecutor = com.sedmelluq.discord.lavaplayer.track.playback.AudioTrackExecutor;
//using IOUtils = Spring.Util.io;
using com.sedmelluq.discord.lavaplayer.remote;
using java.util.concurrent.atomic;
using java.util.concurrent;
using static com.sedmelluq.discord.lavaplayer.tools.FriendlyException;
using com.sedmelluq.discord.lavaplayer.player;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//import static com.sedmelluq.discord.lavaplayer.tools.FriendlyException.Severity.SUSPICIOUS;

/// <summary>
/// Manager of remote nodes for audio processing.
/// </summary>
public class RemoteNodeManager : AudioEventAdapter, RemoteNodeRegistry, ThreadStart
{

    private readonly DefaultAudioPlayerManager playerManager;
    private readonly HttpInterfaceManager httpInterfaceManager;
    private readonly IList<RemoteNodeProcessor> processors;
    private readonly AbandonedTrackManager abandonedTrackManager;
    private readonly AtomicBoolean enabled;
    private readonly object @lock;
    private volatile ScheduledThreadPoolExecutor scheduler;
    private volatile IList<RemoteNodeProcessor> activeProcessors;

    /// <param name="playerManager"> Audio player manager </param>
    public RemoteNodeManager(DefaultAudioPlayerManager playerManager)
    {
        this.playerManager = playerManager;
        this.httpInterfaceManager = RemoteNodeProcessor.createHttpInterfaceManager();
        this.processors = new List<RemoteNodeProcessor>();
        this.abandonedTrackManager = new AbandonedTrackManager();
        this.enabled = new AtomicBoolean();
        this.@lock = new object();
        this.activeProcessors = new List<RemoteNodeProcessor>();
    }

    /// <summary>
    /// Enable and initialise the remote nodes. </summary>
    /// <param name="nodeAddresses"> Addresses of remote nodes </param>
    public virtual void initialise(IList<string> nodeAddresses)
    {
        lock (@lock)
        {
            if (enabled.compareAndSet(false, true))
            {
                startScheduler(nodeAddresses.Count + 1);
            }
            else
            {
                scheduler.CorePoolSize = nodeAddresses.Count + 1;
            }

            IList<string> newNodeAddresses = new List<string>(nodeAddresses);

            for (IEnumerator<RemoteNodeProcessor> iterator = processors.GetEnumerator(); iterator.MoveNext();)
            {
                RemoteNodeProcessor processor = iterator.Current;

                if (!newNodeAddresses.Remove(processor.NodeAddress))
                {
                    processor.shutdown();
                    //JAVA TO C# CONVERTER TODO TASK: .NET enumerators are read-only:
                    iterator.remove();
                }
            }

            foreach (string nodeAddress in newNodeAddresses)
            {
                RemoteNodeProcessor processor = new RemoteNodeProcessor(playerManager, nodeAddress, scheduler, httpInterfaceManager, abandonedTrackManager);

                scheduler.submit(processor);
                processors.Add(processor);
            }

            activeProcessors = new List<RemoteNodeProcessor>(processors);
        }
    }

    /// <summary>
    /// Shut down, freeing all threads and stopping all tracks executed on remote nodes. </summary>
    /// <param name="terminal"> True if initialise will never be called again. </param>
    public virtual void shutdown(bool terminal)
    {
        lock (@lock)
        {
            if (!enabled.compareAndSet(true, false))
            {
                return;
            }

            ExecutorTools.shutdownExecutor(scheduler, "node manager");

            foreach (RemoteNodeProcessor processor in processors)
            {
                processor.processHealthCheck(true);
            }

            abandonedTrackManager.shutdown();

            processors.Clear();
            activeProcessors = new List<RemoteNodeProcessor>(processors);
        }

        if (terminal)
        {
            IOUtils.closeQuietly(httpInterfaceManager);
        }
    }

    public bool Enabled
    {
        get => enabled;
    }


    /// <summary>
    /// Start playing an audio track remotely. </summary>
    /// <param name="remoteExecutor"> The executor of the track </param>
    public virtual void startPlaying(RemoteAudioTrackExecutor remoteExecutor)
    {
        RemoteNodeProcessor processor = NodeForNextTrack;

        processor.startPlaying(remoteExecutor);
    }

    private void startScheduler(int initialSize)
    {
        ScheduledThreadPoolExecutor scheduledExecutor = new ScheduledThreadPoolExecutor(initialSize, new DaemonThreadFactory("remote"));
        scheduledExecutor.scheduleAtFixedRate(this, 2000, 2000, TimeUnit.MILLISECONDS);
        scheduler = scheduledExecutor;
    }

    private RemoteNodeProcessor NodeForNextTrack
    {
        get
        {
            int lowestPenalty = int.MaxValue;
            RemoteNodeProcessor node = null;

            foreach (RemoteNodeProcessor processor in processors)
            {
                int penalty = processor.BalancerPenalty;

                if (penalty < lowestPenalty)
                {
                    lowestPenalty = penalty;
                    node = processor;
                }
            }

            if (node == null)
            {
                throw new FriendlyException("No available machines for playing track.", Severity.SUSPICIOUS, null);
            }

            return node;
        }
    }

    public void onTrackEnd(AudioPlayer player, AudioTrack track, AudioTrackEndReason endReason)
    {
        AudioTrackExecutor executor = ((InternalAudioTrack)track).ActiveExecutor;

        if (endReason != AudioTrackEndReason.FINISHED && executor is RemoteAudioTrackExecutor)
        {
            foreach (RemoteNodeProcessor processor in activeProcessors)
            {
                processor.trackEnded((RemoteAudioTrackExecutor)executor, true);
            }
        }
    }

    public void run()
    {
        foreach (RemoteNodeProcessor processor in activeProcessors)
        {
            processor.processHealthCheck(false);
        }

        abandonedTrackManager.drainExpired();
    }

    public RemoteNode getNodeUsedForTrack(AudioTrack track)
    {
        foreach (RemoteNodeProcessor processor in activeProcessors)
        {
            if (processor.isPlayingTrack(track))
            {
                return processor;
            }
        }

        return null;
    }

    public  List<RemoteNode> Nodes
    {
        get
        {
            return new List<RemoteNode>(activeProcessors);
        }
    }
}
