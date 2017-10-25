using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sedmelluq.discord.lavaplayer.tools
{
    /// <summary>
    /// Decoded serialized exception. The original exception class is not restored, instead all exceptions will be instances
    /// of this class and contain the original class name and message as fields and as the message.
    /// </summary>
    public class DecodedException : Exception
    {
        /// <summary>
        /// Original exception class name
        /// </summary>
        public readonly string className;
        /// <summary>
        /// Original exception message
        /// </summary>
        public readonly string originalMessage;

        /// <param name="className"> Original exception class name </param>
        /// <param name="originalMessage"> Original exception message </param>
        /// <param name="cause"> Cause of this exception </param>
        public DecodedException(string className, string originalMessage, DecodedException cause) : base(className + ": " + originalMessage, cause)
        {

            this.className = className;
            this.originalMessage = originalMessage;
        }
    }
}
