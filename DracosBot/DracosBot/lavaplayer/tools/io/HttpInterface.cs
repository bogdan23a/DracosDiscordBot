using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using java.io;

namespace com.sedmelluq.discord.lavaplayer.tools.io
{
    using CloseableHttpResponse = Apache.Http.Client.Methods.CloseableHttpResponse;
    using HttpUriRequest = Apache.Http.Client.Methods.IHttpUriRequest;
    using HttpClientContext = Apache.Http.Client.Protocol.HttpClientContext;
    using CloseableHttpClient = Apache.Http.Impl.Client.CloseableHttpClient;


    /// <summary>
    /// An HTTP interface for performing HTTP requests in one specific thread. This also means it is not thread safe and should
    /// not be used in a thread it was not obtained in. For multi-thread use <seealso cref="HttpInterfaceManager#getInterface()"/>,
    /// should be called in each thread separately.
    /// </summary>
    public class HttpInterface : Closeable
    {
        private readonly CloseableHttpClient client;
        private readonly HttpClientContext context;
        private readonly bool ownedClient;
        private HttpUriRequest lastRequest;
        private bool available;

        /// <param name="client"> The http client instance used. </param>
        /// <param name="context"> The http context instance used. </param>
        /// <param name="ownedClient"> True if the client should be closed when this instance is closed. </param>
        public HttpInterface(CloseableHttpClient client, HttpClientContext context, bool ownedClient)
        {
            this.client = client;
            this.context = context;
            this.ownedClient = ownedClient;
            this.available = true;
        }

        /// <summary>
        /// Acquire exclusive use of this instance. This is released by calling close.
        /// </summary>
        /// <returns> True if this instance was not exclusively used when this method was called. </returns>
        public virtual bool acquire()
        {
            if (!available)
            {
                return false;
            }

            available = false;
            return true;
        }

        /// <summary>
        /// Executes the given query using the client and context stored in this instance.
        /// </summary>
        /// <param name="request"> The request to execute. </param>
        /// <returns> Closeable response from the server. </returns>
        /// <exception cref="IOException"> On network error. </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public org.apache.http.client.methods.CloseableHttpResponse execute(org.apache.http.client.methods.HttpUriRequest request) throws java.io.IOException
        public virtual CloseableHttpResponse execute(HttpUriRequest request)
        {
            lastRequest = request;
            return client.Execute(request, context);
        }

        /// <returns> The final URL after redirects for the last processed request. Original URL if no redirects were performed.
        ///         Null if no requests have been executed. Undefined state if last request threw an exception. </returns>
        public virtual Uri FinalLocation
        {
            get
            {
                IList<Uri> redirectLocations = context.RedirectLocations;

                if (redirectLocations != null && redirectLocations.Count > 0)
                {
                    return redirectLocations[redirectLocations.Count - 1];
                }
                else
                {
                    return lastRequest != null ? lastRequest.GetURI() : null;
                }
            }
        }

        /// <returns> Http client context used by this interface. </returns>
        public virtual HttpClientContext Context
        {
            get
            {
                return context;
            }
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: @Override public void close() throws java.io.IOException
        public virtual void Dispose()
        {
            available = true;

            if (ownedClient)
            {
                client.close();
            }
        }
    }
}
