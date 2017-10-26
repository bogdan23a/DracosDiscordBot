using java.io;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 
namespace com.sedmelluq.discord.lavaplayer.tools.io
{

    /// <summary>
    /// A thread-safe manager for HTTP interfaces.
    /// </summary>
    public interface HttpInterfaceManager : HttpConfigurable, Closeable
    {
        /// <returns> An HTTP interface for use by the current thread. </returns>
        HttpInterface Interface { get; }
    }
}
