using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using java.io;

namespace com.sedmelluq.discord.lavaplayer.remote.message
{
    using ExceptionTools = com.sedmelluq.discord.lavaplayer.tools.ExceptionTools;


    /// <summary>
    /// Codec for track exception message.
    /// </summary>
    public class TrackExceptionCodec : RemoteMessageCodec<TrackExceptionMessage>
    {
        public Type MessageClass
        {
            get
            {
                return typeof(TrackExceptionMessage);
            }
        }

        public int version(RemoteMessage message)
        {
            return 1;
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: @Override public void encode(java.io.DataOutput out, TrackExceptionMessage message) throws java.io.IOException
        public void encode(DataOutput @out, TrackExceptionMessage message)
        {
            @out.writeLong(message.executorId);
            ExceptionTools.encodeException(@out, message.exception);
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: @Override public TrackExceptionMessage decode(java.io.DataInput in, int version) throws java.io.IOException
        public TrackExceptionMessage decode(DataInput @in, int version)
        {
            return new TrackExceptionMessage(@in.readLong(), ExceptionTools.decodeException(@in));
        }
    }
}
