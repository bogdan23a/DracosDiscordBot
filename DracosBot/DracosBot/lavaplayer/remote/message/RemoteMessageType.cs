using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Generic;

namespace com.sedmelluq.discord.lavaplayer.remote.message
{
    /// <summary>
    /// All remote message types.
    /// </summary>
    public sealed class RemoteMessageType
    {
        public static readonly RemoteMessageType TRACK_START_REQUEST = new RemoteMessageType("TRACK_START_REQUEST", InnerEnum.TRACK_START_REQUEST, new RemoteMessageCodec<TrackStartRequestCodec>());
        public static readonly RemoteMessageType TRACK_START_RESPONSE = new RemoteMessageType("TRACK_START_RESPONSE", InnerEnum.TRACK_START_RESPONSE, new TrackStartResponseCodec());
        public static readonly RemoteMessageType TRACK_FRAME_REQUEST = new RemoteMessageType("TRACK_FRAME_REQUEST", InnerEnum.TRACK_FRAME_REQUEST, new TrackFrameRequestCodec());
        public static readonly RemoteMessageType TRACK_FRAME_DATA = new RemoteMessageType("TRACK_FRAME_DATA", InnerEnum.TRACK_FRAME_DATA, new TrackFrameDataCodec());
        public static readonly RemoteMessageType TRACK_STOPPED = new RemoteMessageType("TRACK_STOPPED", InnerEnum.TRACK_STOPPED, new TrackStoppedCodec());
        public static readonly RemoteMessageType TRACK_EXCEPTION = new RemoteMessageType("TRACK_EXCEPTION", InnerEnum.TRACK_EXCEPTION, new TrackExceptionCodec());
        public static readonly RemoteMessageType NODE_STATISTICS = new RemoteMessageType("NODE_STATISTICS", InnerEnum.NODE_STATISTICS, new NodeStatisticsCodec());

        private static readonly IList<RemoteMessageType> valueList = new List<RemoteMessageType>();

        static RemoteMessageType()
        {
            valueList.Add(TRACK_START_REQUEST);
            valueList.Add(TRACK_START_RESPONSE);
            valueList.Add(TRACK_FRAME_REQUEST);
            valueList.Add(TRACK_FRAME_DATA);
            valueList.Add(TRACK_STOPPED);
            valueList.Add(TRACK_EXCEPTION);
            valueList.Add(NODE_STATISTICS);
        }

        public enum InnerEnum
        {
            TRACK_START_REQUEST,
            TRACK_START_RESPONSE,
            TRACK_FRAME_REQUEST,
            TRACK_FRAME_DATA,
            TRACK_STOPPED,
            TRACK_EXCEPTION,
            NODE_STATISTICS
        }

        public readonly InnerEnum innerEnumValue;
        private readonly string nameValue;
        private readonly int ordinalValue;
        private static int nextOrdinal = 0;

        /// <summary>
        /// The codec used for encoding and decoding this type of message.
        /// </summary>
        public readonly RemoteMessageCodec<RemoteMessage> codec;

        internal RemoteMessageType(string name, InnerEnum innerEnum, RemoteMessageCodec<RemoteMessage> codec)
        {
            this.codec = codec;

            nameValue = name;
            ordinalValue = nextOrdinal++;
            innerEnumValue = innerEnum;
        }

    public static IList<RemoteMessageType> values()
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

    public static RemoteMessageType valueOf(string name)
    {
        foreach (RemoteMessageType enumInstance in RemoteMessageType.valueList)
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
