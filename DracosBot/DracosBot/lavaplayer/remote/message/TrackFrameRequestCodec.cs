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
    /// Codec for track frame request message.
    /// </summary>
    public class TrackFrameRequestCodec : RemoteMessageCodec<TrackFrameRequestMessage>
    {
        public Type MessageClass
        {
            get
            {
                return typeof(TrackFrameRequestMessage);
            }
        }

        public int version(RemoteMessage message)
        {
            return 1;
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: @Override public void encode(java.io.DataOutput out, TrackFrameRequestMessage message) throws java.io.IOException
        public void encode(DataOutput @out, TrackFrameRequestMessage message)
        {
            @out.writeLong(message.executorId);
            @out.writeInt(message.maximumFrames);
            @out.writeInt(message.volume);
            @out.writeLong(message.seekPosition);
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: @Override public TrackFrameRequestMessage decode(java.io.DataInput in, int version) throws java.io.IOException
        public TrackFrameRequestMessage decode(DataInput @in, int version)
        {
            return new TrackFrameRequestMessage(@in.readLong(), @in.readInt(), @in.readInt(), @in.readLong());
        }
    }
}
