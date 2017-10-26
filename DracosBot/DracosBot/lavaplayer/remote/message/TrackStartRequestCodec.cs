using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using java.io;

namespace com.sedmelluq.discord.lavaplayer.remote.message
{
    using AudioDataFormat = com.sedmelluq.discord.lavaplayer.format.AudioDataFormat;
    using StandardAudioDataFormats = com.sedmelluq.discord.lavaplayer.format.StandardAudioDataFormats;
    using AudioConfiguration = com.sedmelluq.discord.lavaplayer.player.AudioConfiguration;
    using AudioTrackInfo = com.sedmelluq.discord.lavaplayer.track.AudioTrackInfo;


    /// <summary>
    /// Codec for track start message.
    /// </summary>
    public class TrackStartRequestCodec : RemoteMessageCodec<TrackStartRequestMessage>
    {
        private const int VERSION_INITIAL = 1;
        private const int VERSION_WITH_FORMAT = 2;
        private const int VERSION_WITH_POSITION = 3;

        public  Type MessageClass
        {
            get
            {
                return typeof(TrackStartRequestMessage);
            }
        }

        public int version(RemoteMessage message)
        {
            // Backwards compatibility with older nodes.
            if (message is TrackStartRequestMessage)
            {
                if (((TrackStartRequestMessage)message).position != 0)
                {
                    return VERSION_WITH_POSITION;
                }

                AudioDataFormat format = ((TrackStartRequestMessage)message).configuration.OutputFormat;

                if (!format.Equals(StandardAudioDataFormats.DISCORD_OPUS))
                {
                    return VERSION_WITH_FORMAT;
                }

                return VERSION_INITIAL;
            }

            return VERSION_WITH_POSITION;
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: @Override public void encode(java.io.DataOutput out, TrackStartRequestMessage message) throws java.io.IOException
        public void encode(DataOutput @out, TrackStartRequestMessage message)
        {
            int version = version(message);

            @out.writeLong(message.executorId);
            @out.writeUTF(message.trackInfo.title);
            @out.writeUTF(message.trackInfo.author);
            @out.writeLong(message.trackInfo.length);
            @out.writeUTF(message.trackInfo.identifier);
            @out.writeBoolean(message.trackInfo.isStream);
            @out.writeInt(message.encodedTrack.Length);
            @out.writeBytes(message.encodedTrack.ToString());
            @out.writeInt(message.volume);
            @out.writeUTF(message.configuration.getResamplingQuality().ToString());
            @out.writeInt(message.configuration.OpusEncodingQuality);

            if (version >= VERSION_WITH_FORMAT)
            {
                AudioDataFormat format = message.configuration.OutputFormat;
                @out.writeInt(format.channelCount);
                @out.writeInt(format.sampleRate);
                @out.writeInt(format.chunkSampleCount);
                @out.writeUTF(format.codec.name());
            }

            if (version >= VERSION_WITH_POSITION)
            {
                @out.writeLong(message.position);
            }
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: @Override public TrackStartRequestMessage decode(java.io.DataInput in, int version) throws java.io.IOException
        public TrackStartRequestMessage decode(DataInput @in, int version)
        {
            long executorId = @in.readLong();
            AudioTrackInfo trackInfo = new AudioTrackInfo(@in.readUTF(), @in.readUTF(), @in.readLong(), @in.readUTF(), @in.readBoolean(), null);

            sbyte[] encodedTrack = new sbyte[@in.readInt()];
            @in.readFully(encodedTrack);

            int volume = @in.readInt();
            AudioConfiguration configuration = new AudioConfiguration();
            configuration.ResamplingQuality = AudioConfiguration.ResamplingQuality.valueOf(@in.readUTF());
            configuration.OpusEncodingQuality = @in.readInt();

            if (version >= VERSION_WITH_FORMAT)
            {
                AudioDataFormat format = new AudioDataFormat(@in.readInt(), @in.readInt(), @in.readInt(), AudioDataFormat.Codec.valueOf(@in.readUTF()));

                configuration.OutputFormat = format;
            }

            long position = 0;

            if (version >= VERSION_WITH_POSITION)
            {
                position = @in.readLong();
            }

            return new TrackStartRequestMessage(executorId, trackInfo, encodedTrack, volume, configuration, position);
        }
    }
}
