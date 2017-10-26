using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using java.io;

namespace com.sedmelluq.discord.lavaplayer.remote.message
{

    /// <summary>
    /// Codec for track start request response message.
    /// </summary>
    public class TrackStartResponseCodec : RemoteMessageCodec<TrackStartResponseMessage>
    {
        public Type MessageClass
        {
            get
            {
                return typeof(TrackStartResponseMessage);
            }
        }

        public int version(RemoteMessage message)
        {
            return 1;
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: @Override public void encode(java.io.DataOutput out, TrackStartResponseMessage message) throws java.io.IOException
        public void encode(DataOutput @out, TrackStartResponseMessage message)
        {
            @out.writeLong(message.executorId);
            @out.writeBoolean(message.success);

            if (!message.success)
            {
                @out.writeUTF(message.failureReason);
            }
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: @Override public TrackStartResponseMessage decode(java.io.DataInput in, int version) throws java.io.IOException
        public TrackStartResponseMessage decode(DataInput @in, int version)
        {
            long executorId = @in.readLong();
            bool success = @in.readBoolean();

            return new TrackStartResponseMessage(executorId, success, success ? null : @in.readUTF());
        }
    }
}