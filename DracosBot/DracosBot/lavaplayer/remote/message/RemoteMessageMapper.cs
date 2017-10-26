using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using java.io;

namespace com.sedmelluq.discord.lavaplayer.remote.message
{
    using Logger = ILogger;
    using LoggerFactory = ILoggerFactory;


    /// <summary>
    /// Handles encoding and decoding of messages.
    /// </summary>
    public class RemoteMessageMapper
    {
        private static readonly LoggerFactory factory;
        
        private static readonly Logger log = factory.CreateLogger<RemoteMessageMapper>();

        private readonly IDictionary<Type, RemoteMessageType> encodingMap;

        /// <summary>
        /// Create a new instance.
        /// </summary>
        public RemoteMessageMapper()
        {
            encodingMap = new IdentityHashMap<>();

            initialiseEncodingMap();
        }

        private void initialiseEncodingMap()
        {
            foreach (RemoteMessageType type in typeof(RemoteMessageType).EnumConstants)
            {
                encodingMap[type.codec.MessageClass] = type;
            }
        }

        /// <summary>
        /// Decodes one message. If the input stream indicates the end of messages, null is returned.
        /// </summary>
        /// <param name="input"> The input stream containing the message </param>
        /// <returns> The decoded message or null in case no more messages are present </returns>
        /// <exception cref="IOException"> When an IO error occurs </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public RemoteMessage decode(java.io.DataInput input) throws java.io.IOException
        public virtual RemoteMessage decode(DataInput input)
        {
            int messageSize = input.readInt();
            if (messageSize == 0)
            {
                return null;
            }

            RemoteMessageType[] types = typeof(RemoteMessageType).EnumConstants;
            int typeIndex = input.readByte() & 0xFF;
            int version = input.readByte() & 0xFF;

            if (typeIndex >= types.Length)
            {
                log.LogWarning("Invalid message type {}.", typeIndex);
                input.readFully(new sbyte[messageSize - 1]);
                return UnknownMessage.INSTANCE;
            }

            RemoteMessageType type = types[typeIndex];

            if (version < 1 || version > type.codec.version(null))
            {
                log.LogWarning("Invalid version {} for message {}.", version, type);
                input.readFully(new sbyte[messageSize - 2]);
                return UnknownMessage.INSTANCE;
            }

            return type.codec.decode(input, version);
        }

        /// <summary>
        /// Encodes one message.
        /// </summary>
        /// <param name="output"> The output stream to encode to </param>
        /// <param name="message"> The message to encode </param>
        /// <exception cref="IOException"> When an IO error occurs </exception>
        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public void encode(java.io.DataOutputStream output, RemoteMessage message) throws java.io.IOException
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        public virtual void encode(DataOutputStream output, RemoteMessage message)
        {
            RemoteMessageType type = encodingMap[message.GetType()];

            System.IO.MemoryStream messageOutputBytes = new System.IO.MemoryStream();
            DataOutput messageOutput = new DataOutputStream(messageOutputBytes);

            RemoteMessageCodec<RemoteMessage> codec = type.codec;
            codec.encode(messageOutput, message);

            output.writeInt(messageOutputBytes.Capacity + 2);
            output.writeByte((sbyte)type.ordinal());
            output.writeByte((sbyte)type.codec.version(message));
            messageOutputBytes.WriteTo(output);
        }

        /// <summary>
        /// Write the marker to indicate no more messages are in the stream.
        /// </summary>
        /// <param name="output"> The output stream </param>
        /// <exception cref="IOException"> When an IO error occurs </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public void endOutput(java.io.DataOutputStream output) throws java.io.IOException
        public virtual void endOutput(DataOutputStream output)
        {
            output.writeInt(0);
        }
    }
}

