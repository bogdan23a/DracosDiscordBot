using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sedmelluq.discord.lavaplayer.remote.message
{
    /// <summary>
    /// Used for cases where the message could not be decoded.
    /// </summary>
    public class UnknownMessage : RemoteMessage
    {
        /// <summary>
        /// Keep a singleton instance as there can be no difference between instances.
        /// </summary>
        public static readonly UnknownMessage INSTANCE = new UnknownMessage();
    }
}
