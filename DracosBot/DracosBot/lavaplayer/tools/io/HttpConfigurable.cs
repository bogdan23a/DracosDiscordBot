using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
namespace com.sedmelluq.discord.lavaplayer.tools.io
{
    using RequestConfig = HttpClient.;

    /// <summary>
    /// Represents a class where HTTP request configuration can be changed.
    /// </summary>
    public interface HttpConfigurable
    {
        /// <param name="configurator"> Function to reconfigure request config. </param>
        void configureRequests(System.Func<RequestConfig, RequestConfig> configurator);
    }
}
