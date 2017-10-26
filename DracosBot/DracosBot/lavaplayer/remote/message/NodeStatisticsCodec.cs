using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using java.io;

namespace com.sedmelluq.discord.lavaplayer.remote.message
{

    /// <summary>
    /// Codec for node statistics message.
    /// </summary>
    public class NodeStatisticsCodec : RemoteMessageCodec<NodeStatisticsMessage>
    {
        public Type MessageClass
        {
            get
            {
                return typeof(NodeStatisticsMessage);
            }
        }

        public int version(RemoteMessage message)
        {
            return 1;
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: @Override public void encode(java.io.DataOutput out, NodeStatisticsMessage message) throws java.io.IOException
        public void encode(DataOutput @out, NodeStatisticsMessage message)
        {
            @out.writeInt(message.playingTrackCount);
            @out.writeInt(message.totalTrackCount);
            @out.writeFloat(message.systemCpuUsage);
            @out.writeFloat(message.processCpuUsage);
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: @Override public NodeStatisticsMessage decode(java.io.DataInput in, int version) throws java.io.IOException
        public NodeStatisticsMessage decode(DataInput @in, int version)
        {
            return new NodeStatisticsMessage(@in.readInt(), @in.readInt(), @in.readFloat(), @in.readFloat());
        }
    }
}
