using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using java.io;

namespace com.sedmelluq.discord.lavaplayer.tools.io
{

    /// <summary>
    /// An output for  a series of messages which each have sizes specified before the start of the message. Even when the
    /// decoder does not recognize some of the messages, it can skip over the message since it knows its size in advance.
    /// </summary>
    public class MessageOutput
    {
        private readonly OutputStream outputStream;
        private readonly DataOutputStream dataOutputStream;
        private readonly MemoryStream messageByteOutput;
        private readonly DataOutputStream messageDataOutput;

        /// <param name="outputStream"> Output stream to write the messages to </param>
        public MessageOutput(OutputStream outputStream)
        {
            this.outputStream = outputStream;
            this.dataOutputStream = new DataOutputStream(outputStream);
            this.messageByteOutput = new MemoryStream();
            //this.messageDataOutput = new DataOutputStream(messageByteOutput);
        }

        /// <returns> Data output for a new message </returns>
        public virtual DataOutput startMessage()
        {
            messageByteOutput.reset();
            return messageDataOutput;
        }

        /// <summary>
        /// Commit previously started message to the underlying output stream. </summary>
        /// <exception cref="IOException"> On IO error </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public void commitMessage() throws java.io.IOException
        public virtual void commitMessage()
        {
            dataOutputStream.writeInt((int)messageByteOutput.Length);
            messageByteOutput.WriteTo(outputStream);
        }

        /// <summary>
        /// Commit previously started message to the underlying output stream. </summary>
        /// <param name="flags"> Flags to use when committing the message (0-3). </param>
        /// <exception cref="IOException"> On IO error </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public void commitMessage(int flags) throws java.io.IOException
        public virtual void commitMessage(int flags)
        {
            dataOutputStream.writeInt((int)messageByteOutput.Length | flags << 3);
            messageByteOutput.WriteTo(outputStream);
        }

        /// <summary>
        /// Write an end marker to the stream so that decoder knows to return null at this position. </summary>
        /// <exception cref="IOException"> On IO error </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public void finish() throws java.io.IOException
        public virtual void finish()
        {
            dataOutputStream.writeInt(0);
        }
    }
}
