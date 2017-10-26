using java.io;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace com.sedmelluq.discord.lavaplayer.remote.message
{

    /// <summary>
    /// Codec for encoding and decoding remote messages.
    /// </summary>
    /// @param <T> The message class </param>
    public interface RemoteMessageCodec<T> where T : RemoteMessage
    {
        /// <returns> The class of the message this codec works with </returns>
        Type MessageClass { get; }

        /// <param name="message"> If set, returns version to use for this specific message. </param>
        /// <returns> Latest version of this codec, or version to use if the message is specified. </returns>
        int version(RemoteMessage message);

        /// <summary>
        /// Encode the message to the specified output.
        /// </summary>
        /// <param name="out"> The output stream </param>
        /// <param name="message"> The message to encode </param>
        /// <exception cref="IOException"> When an IO error occurs </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: void encode(java.io.DataOutput out, T message) throws java.io.IOException;
        void encode(DataOutput @out, T message);

        /// <summary>
        /// Decode a message from the specified input.
        /// </summary>
        /// <param name="in"> The input stream </param>
        /// <param name="version"> Version of the message </param>
        /// <returns> The decoded message </returns>
        /// <exception cref="IOException"> When an IO error occurs </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: T decode(java.io.DataInput in, int version) throws java.io.IOException;
        T decode(DataInput @in, int version);
    }
}
