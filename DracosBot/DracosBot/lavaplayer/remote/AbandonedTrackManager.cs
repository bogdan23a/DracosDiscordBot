using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using java.util.concurrent;
using NodeStatisticsMessage = com.sedmelluq.discord.lavaplayer.remote.message.NodeStatisticsMessage;
using FriendlyException = com.sedmelluq.discord.lavaplayer.tools.FriendlyException;
using AudioTrackState = com.sedmelluq.discord.lavaplayer.track.AudioTrackState;
using Severity = com.sedmelluq.discord.lavaplayer.tools.FriendlyException.Severity;
namespace com.sedmelluq.discord.lavaplayer.remote
{

    using Logger = ILogger;
    using LoggerFactory = ILoggerFactory;



    /// <summary>
    /// Takes over tracks of offline remote nodes.
    /// </summary>
    public class AbandonedTrackManager
    {
        private static readonly LoggerFactory factory;

        private static readonly Logger log = factory.CreateLogger<AbandonedTrackManager>();

        private static readonly long EXPIRE_THRESHOLD = TimeUnit.SECONDS.toMillis(10);
        private const long CRITICAL_PENALTY = 750;

        private readonly BlockingQueue<AbandonedExecutor> abandonedExecutors;

        /// <summary>
        /// Create an instance.
        /// </summary>
        public AbandonedTrackManager()
        {
            this.abandonedExecutors = new ArrayBlockingQueue<>(2000);
        }

        /// <summary>
        /// Adds a track executor to abandoned tracks. The abandoned track manager will take over managing its lifecycle and
        /// the caller should not use it any further.
        /// </summary>
        /// <param name="executor"> The executor to register as an abandoned track. </param>
        public virtual void add(RemoteAudioTrackExecutor executor)
        {
            if (abandonedExecutors.offer(new AbandonedExecutor(DateTimeHelperClass.CurrentUnixTimeMillis(), executor)))
            {
                log.debug("{} has been put up for adoption.", executor);
            }
            else
            {
                log.debug("{} has been discarded, adoption queue is full.", executor);

                executor.dispatchException(new FriendlyException("Cannot find a node to play the track on.",Severity.COMMON, null));
                executor.stop();
            }
        }

        /// <summary>
        /// Distributes any abandoned tracks between the specified nodes. Only online nodes which are not under too heavy load
        /// are used. The number of tracks that can be assigned to a node depends on the number of tracks it is already
        /// processing (track count can increase only by 1/15th on each call, or by 5).
        /// </summary>
        /// <param name="nodes"> Remote nodes to give abandoned tracks to. </param>
        public virtual void distribute(IList<RemoteNodeProcessor> nodes)
        {
            if (abandonedExecutors.Empty)
            {
                return;
            }

            IList<Adopter> adopters = findAdopters(nodes);
            AbandonedExecutor executor;
            long currentTime = DateTimeHelperClass.CurrentUnixTimeMillis();
            int maximum = getMaximumAdoptions(adopters);
            int assigned = 0;

            while (assigned < maximum && (executor = abandonedExecutors.poll()) != null)
            {
                if (checkValidity(executor, currentTime))
                {
                    Adopter adopter = selectNextAdopter(adopters);
                    log.debug("Node {} is adopting {}.", adopter.node.Address, executor.executor);

                    adopter.node.startPlaying(executor.executor);
                    assigned++;
                }
            }
        }

        /// <summary>
        /// Shut down the abandoned track manager, stopping any tracks.
        /// </summary>
        public virtual void shutdown()
        {
            AbandonedExecutor executor;

            while ((executor = abandonedExecutors.poll()) != null)
            {
                executor.executor.dispatchException(new FriendlyException("Node system was shut down.", Severity.COMMON, null));
                executor.executor.stop();
            }
        }
    }

    //---------------------------------------------------------------------------------------------------------
    //	Copyright © 2007 - 2017 Tangible Software Solutions Inc.
    //	This class can be used by anyone provided that the copyright notice remains intact.
    //
    //	This class is used to replace calls to Java's System.currentTimeMillis with the C# equivalent.
    //	Unix time is defined as the number of seconds that have elapsed since midnight UTC, 1 January 1970.
    //---------------------------------------------------------------------------------------------------------
    internal static class DateTimeHelperClass
    {
        private static readonly System.DateTime Jan1st1970 = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        internal static long CurrentUnixTimeMillis()
        {
            return (long)(System.DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
        }
    }

  /// <summary>
  /// Remove expired or stopped tracks from the abandoned track queue.
  /// </summary>
  public virtual void drainExpired()
    {
        AbandonedExecutor executor;
        long currentTime = DateTimeHelperClass.CurrentUnixTimeMillis();

        while ((executor = abandonedExecutors.peek()) != null)
        {
            if (!checkValidity(executor, currentTime) && abandonedExecutors.remove(executor))
            {
                log.debug("Abandoned executor {} removed from queue.", executor.executor);
            }
        }
    }

    private bool checkValidity(AbandonedExecutor executor, long currentTime)
    {
        long expirationTime = currentTime - EXPIRE_THRESHOLD;

        if (executor.executor.State == AudioTrackState.FINISHED)
        {
            log.debug("{} has been cleared from adoption queue because it was stopped.", executor.executor);
            return false;
        }
        else if (executor.orphanedTime < expirationTime)
        {
            log.debug("{} has been cleared from adoption queue because it expired.", executor.executor);

            executor.executor.dispatchException(new FriendlyException("Could not find next node to play on.", Severity.COMMON, null));
            executor.executor.stop();
            return false;
        }
        else
        {
            return true;
        }
    }

    private IList<Adopter> findAdopters(IList<RemoteNodeProcessor> nodes)
    {
        IList<Adopter> adopters = new List<Adopter>();

        foreach (RemoteNodeProcessor node in nodes)
        {
            int penalty = node.BalancerPenalty;
            NodeStatisticsMessage statistics = node.LastStatistics;

            if (penalty >= CRITICAL_PENALTY || statistics == null)
            {
                continue;
            }

            int maximumAdoptions = Math.Max(5, statistics.playingTrackCount / 15);
            adopters.Add(new Adopter(node, maximumAdoptions));
        }

        return adopters;
    }

    private Adopter selectNextAdopter(IList<Adopter> adopters)
    {
        Adopter selected = null;

        foreach (Adopter adopter in adopters)
        {
            if (adopter.adoptions < adopter.maximumAdoptions && (selected == null || adopter.fillRate() > selected.fillRate()))
            {
                selected = adopter;
            }
        }

        if (selected != null)
        {
            selected.adoptions++;
        }

        return selected;
    }

    private int getMaximumAdoptions(IList<Adopter> adopters)
    {
        int total = 0;

        foreach (Adopter adopter in adopters)
        {
            total += adopter.maximumAdoptions;
        }

        return total;
    }

    private class AbandonedExecutor
    {
        private readonly long orphanedTime;
        private readonly RemoteAudioTrackExecutor executor;

        private AbandonedExecutor(long orphanedTime, RemoteAudioTrackExecutor executor)
        {
            this.orphanedTime = orphanedTime;
            this.executor = executor;
        }
    }

    private class Adopter
    {
        private readonly RemoteNodeProcessor node;
        private readonly long maximumAdoptions;
        private int adoptions;

        private Adopter(RemoteNodeProcessor node, long maximumAdoptions)
        {
            this.node = node;
            this.maximumAdoptions = maximumAdoptions;
            this.adoptions = 0;
        }

        private float fillRate()
        {
            return (float)adoptions / maximumAdoptions;
        }
    }
}

//---------------------------------------------------------------------------------------------------------
//	Copyright © 2007 - 2017 Tangible Software Solutions Inc.
//	This class can be used by anyone provided that the copyright notice remains intact.
//
//	This class is used to replace calls to Java's System.currentTimeMillis with the C# equivalent.
//	Unix time is defined as the number of seconds that have elapsed since midnight UTC, 1 January 1970.
//---------------------------------------------------------------------------------------------------------
internal static class DateTimeHelperClass
{
    private static readonly System.DateTime Jan1st1970 = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
    internal static long CurrentUnixTimeMillis()
    {
        return (long)(System.DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
    }
}
