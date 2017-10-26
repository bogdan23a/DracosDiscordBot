using java.io;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace com.sedmelluq.discord.lavaplayer.tools.io
{
   /* using IOUtils = org.apache.commons.io.IOUtils;
    using BoundedInputStream = org.apache.commons.io.input.BoundedInputStream;
    using CountingInputStream = org.apache.commons.io.input.CountingInputStream;
    */

    /// <summary>
    /// An input for messages with their size known so unknown messages can be skipped.
    /// </summary>
    public class MessageInput
    {
        private readonly CountingInputStream countingInputStream;
        private readonly DataInputStream dataInputStream;
        private int messageSize;
        private int messageFlags;

        /// <param name="inputStream"> Input stream to read from. </param>
        public MessageInput(java.io.InputStream inputStream)
        {
            this.countingInputStream = new CountingInputStream(inputStream);
            this.dataInputStream = new DataInputStream(inputStream);
        }

        /// <returns> Data input for the next message. Note that it does not automatically skip over the last message if it was
        ///         not fully read, for that purpose, skipRemainingBytes() should be explicitly called after reading every
        ///         message. A null return value indicates the position where MessageOutput#finish() had written the end
        ///         marker. </returns>
        /// <exception cref="IOException"> On IO error </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public java.io.DataInput nextMessage() throws java.io.IOException
        public virtual DataInput nextMessage()
        {
            int value = dataInputStream.readInt();
            messageFlags = (int)((value & 0xC0000000L) >> 30);
            messageSize = value & 0x3FFFFFFF;

            if (messageSize == 0)
            {
                return null;
            }

            return new DataInputStream(new BoundedInputStream(countingInputStream, messageSize));
        }

        /// <returns> Flags (values 0-3) of the last message for which nextMessage() was called. </returns>
        public virtual int MessageFlags
        {
            get
            {
                return messageFlags;
            }
        }

        /// <summary>
        /// Skip the remaining bytes of the last message returned from nextMessage(). This must be called if it is not certain
        /// that all of the bytes of the message were consumed. </summary>
        /// <exception cref="IOException"> On IO error </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public void skipRemainingBytes() throws java.io.IOException
        public virtual void skipRemainingBytes()
        {
            long count = countingInputStream.resetByteCount();

            if (count < messageSize)
            {
                IOUtils.skipFully(dataInputStream, messageSize - count);
            }

            messageSize = 0;
        }
    }
}
