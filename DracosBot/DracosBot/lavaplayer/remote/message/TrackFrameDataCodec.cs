using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Collections.Generic;
using java.io;

namespace com.sedmelluq.discord.lavaplayer.remote.message
{
    using AudioFrame = com.sedmelluq.discord.lavaplayer.track.playback.AudioFrame;


    /// <summary>
    /// Codec for track frame data message.
    /// </summary>
    public class TrackFrameDataCodec : RemoteMessageCodec<TrackFrameDataMessage>
    {
        public Type MessageClass
        {
            get
            {
                return typeof(TrackFrameDataMessage);
            }
        }

        public int version(RemoteMessage message)
        {
            return 1;
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: @Override public void encode(java.io.DataOutput out, TrackFrameDataMessage message) throws java.io.IOException
        public void encode(DataOutput @out, TrackFrameDataMessage message)
        {
            @out.writeLong(message.executorId);
            @out.writeInt(message.frames.Count);

            foreach (AudioFrame frame in message.frames)
            {
                @out.writeLong(frame.timecode);
                @out.writeInt(frame.data.Length);
                @out.writeBytes(frame.data.ToString());
                @out.writeInt(frame.volume);
            }

            @out.writeBoolean(message.finished);
            @out.writeLong(message.seekedPosition);
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: @Override public TrackFrameDataMessage decode(java.io.DataInput in, int version) throws java.io.IOException
        public TrackFrameDataMessage decode(DataInput @in, int version)
        {
            long executorId = @in.readLong();
            int frameCount = @in.readInt();

            IList<AudioFrame> frames = new List<AudioFrame>(frameCount);

            for (int i = 0; i < frameCount; i++)
            {
                long timecode = @in.readLong();
                sbyte[] data = new sbyte[@in.readInt()];
                @in.readFully(data);

                frames.Add(new AudioFrame(timecode, data, @in.readInt(), null));
            }

            return new TrackFrameDataMessage(executorId, frames, @in.readBoolean(), @in.readLong());
        }
    }
}
