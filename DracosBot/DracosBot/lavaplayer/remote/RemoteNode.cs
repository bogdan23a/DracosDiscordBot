using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Generic;

namespace com.sedmelluq.discord.lavaplayer.remote
{
    using NodeStatisticsMessage = com.sedmelluq.discord.lavaplayer.remote.message.NodeStatisticsMessage;
    using AudioTrack = com.sedmelluq.discord.lavaplayer.track.AudioTrack;


    /// <summary>
    /// A remote node interface which provides information about a specific node.
    /// </summary>
    public interface RemoteNode
    {
        /// <returns> The address of this remote node. </returns>
        string Address { get; }

        /// <returns> Gets the current state of connection between the node processor and the node. </returns>
        ConnectionState ConnectionState { get; }

        /// <returns> The last statistics received from this node. May be null. </returns>
        NodeStatisticsMessage LastStatistics { get; }

        /// <returns> The minimum amount of time in milliseconds between the start time of two ticks. </returns>
        int TickMinimumInterval { get; }

        /// <summary>
        /// To record all ticks, it is possible to calculate the minimum amount of time which guarantees that none are
        /// discarded in the internal history (minimumInterval * tickHistoryCapacity).
        /// </summary>
        /// <returns> The number of ticks that are kept in history. </returns>
        int TickHistoryCapacity { get; }

        /// <param name="reset"> Whether to reset the history so next calls will only contain new ones. </param>
        /// <returns> All the ticks in the history, up to the history capacity. In case of an overflow, newer ones will replace
        ///         older ones. </returns>
        IList<RemoteNode_Tick> getLastTicks(bool reset);

        /// <returns> The number of tracks being played by this player manager through this node. </returns>
        int PlayingTrackCount { get; }

        /// <returns> List of tracks being played by this node for the current player manager. </returns>
        IList<AudioTrack> PlayingTracks { get; }

        /// <returns> Map containing the balancer penalty factors, with "Total" being the sum of all others. </returns>
        IDictionary<string, int?> BalancerPenaltyDetails { get; }

        /// <summary>
        /// Checks if a audio track is being played by this node.
        /// </summary>
        /// <param name="track"> The audio track. </param>
        /// <returns> True if this node is playing that track. </returns>
        bool isPlayingTrack(AudioTrack track);

    }
    internal class Tick
        {
            /// <summary>
            /// The time when the node processor started building the request to send to the node.
            /// </summary>
            public readonly long startTime;
            /// <summary>
            /// The time when the processing the response data from the node was finished.
            /// </summary>
            public readonly long endTime;
            /// <summary>
            /// Response code from the node. -1 in case of connection failure.
            /// </summary>
            public readonly int responseCode;
            /// <summary>
            /// The size of the request in bytes.
            /// </summary>
            public readonly int requestSize;
            /// <summary>
            /// The size of the uncompressed response in bytes.
            /// </summary>
            public readonly int responseSize;

            /// <param name="startTime"> The time when the node processor started building the request to send to the node. </param>
            /// <param name="endTime"> The time when the processing the response data from the node was finished. </param>
            /// <param name="responseCode"> Response code from the node. -1 in case of connection failure. </param>
            /// <param name="requestSize"> The size of the request in bytes. </param>
            /// <param name="responseSize"> The size of the uncompressed response in bytes. </param>
            public Tick(long startTime, long endTime, int responseCode, int requestSize, int responseSize)
            {
                this.startTime = startTime;
                this.endTime = endTime;
                this.responseCode = responseCode;
                this.requestSize = requestSize;
                this.responseSize = responseSize;
            }
        }


        /// <summary>
        /// State of the connection to this node.
        /// </summary>
        internal sealed class ConnectionState
    {
        /// <summary>
        /// The node processor is current in the middle of attempting a connection to this node. Happens on every
        /// reconnection attempt, even if the node has already been offline for a period of time.
        /// </summary>
        public static readonly ConnectionState PENDING = new ConnectionState("PENDING", InnerEnum.PENDING);
        /// <summary>
        /// The node is currently online, new tracks can be sent to this node.
        /// </summary>
        public static readonly ConnectionState ONLINE = new ConnectionState("ONLINE", InnerEnum.ONLINE);
        /// <summary>
        /// The node is offline, this is the state the node has before attempting the first request and after any failed
        /// request to a node until a new request is successful.
        /// </summary>
        public static readonly ConnectionState OFFLINE = new ConnectionState("OFFLINE", InnerEnum.OFFLINE);
        /// <summary>
        /// This node has been removed from the list of nodes to use. In case a node with the same address is added, this
        /// instance will not be reactivated, but a new one should be retrieved.
        /// </summary>
        public static readonly ConnectionState REMOVED = new ConnectionState("REMOVED", InnerEnum.REMOVED);

        private static readonly IList<ConnectionState> valueList = new List<ConnectionState>();

        static ConnectionState()
        {
            valueList.Add(PENDING);
            valueList.Add(ONLINE);
            valueList.Add(OFFLINE);
            valueList.Add(REMOVED);
        }

        public enum InnerEnum
        {
            PENDING,
            ONLINE,
            OFFLINE,
            REMOVED
        }

        public readonly InnerEnum innerEnumValue;
        private readonly string nameValue;
        private readonly int ordinalValue;
        private static int nextOrdinal = 0;

        private ConnectionState(string name, InnerEnum innerEnum)
        {
            nameValue = name;
            ordinalValue = nextOrdinal++;
            innerEnumValue = innerEnum;
        }

        /// <returns> Shortcut for ordinal. </returns>
        public int id()
        {
            return ordinal();
        }

        public static IList<ConnectionState> values()
        {
            return valueList;
        }

        public int ordinal()
        {
            return ordinalValue;
        }

        public override string ToString()
        {
            return nameValue;
        }

        public static ConnectionState valueOf(string name)
        {
            foreach (ConnectionState enumInstance in ConnectionState.valueList)
            {
                if (enumInstance.nameValue == name)
                {
                    return enumInstance;
                }
            }
            throw new System.ArgumentException(name);
        }
    }
}