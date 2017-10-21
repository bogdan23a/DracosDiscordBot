using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace com.sedmelluq.discord.lavaplayer.tools
{
    /// <summary>
    /// An exception with a friendly message.
    /// </summary>
    public class FriendlyException : Exception
    {
        /// <summary>
        /// Severity of the exception
        /// </summary>
        public readonly Severity severity;

        /// <param name="friendlyMessage"> A message which is understandable to end-users </param>
        /// <param name="severity"> Severity of the exception </param>
        /// <param name="cause"> The cause of the exception with technical details </param>
        public FriendlyException(string friendlyMessage, Severity severity, Exception cause) : base(friendlyMessage, cause)
        {

            this.severity = severity;
        }

        /// <summary>
        /// Severity levels for FriendlyException
        /// </summary>
        public enum Severity
        {
            /// <summary>
            /// The cause is known and expected, indicates that there is nothing wrong with the library itself.
            /// </summary>
            COMMON,
            /// <summary>
            /// The cause might not be exactly known, but is possibly caused by outside factors. For example when an outside
            /// service responds in a format that we do not expect.
            /// </summary>
            SUSPICIOUS,
            /// <summary>
            /// If the probable cause is an issue with the library or when there is no way to tell what the cause might be.
            /// This is the default level and other levels are used in cases where the thrower has more in-depth knowledge
            /// about the error.
            /// </summary>
            FAULT
        }
    }
}
