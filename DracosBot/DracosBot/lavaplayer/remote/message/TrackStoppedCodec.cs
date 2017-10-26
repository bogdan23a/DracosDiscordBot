using java.io;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sedmelluq.discord.lavaplayer.remote.message
{

    /// <summary>
    /// Codec for stopped track notification message.
    /// </summary>
    public class TrackStoppedCodec : RemoteMessageCodec<TrackStoppedMessage>
    {
        public Type MessageClass
        {
            get
            {
                return typeof(TrackStoppedMessage);
            }
        }

        public int version(RemoteMessage message)
        {
            return 1;
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: @Override public void encode(java.io.DataOutput out, TrackStoppedMessage message) throws java.io.IOException
        public void encode(DataOutput @out, TrackStoppedMessage message)
        {
            @out.writeLong(message.executorId);
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: @Override public TrackStoppedMessage decode(java.io.DataInput in, int version) throws java.io.IOException
        public TrackStoppedMessage decode(DataInput @in, int version)
        {
            return new TrackStoppedMessage(@in.readLong());
        }
    }
}
