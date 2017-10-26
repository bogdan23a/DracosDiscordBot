/**
 * Couchbase Lite for .NET
 *
 * Original iOS version by Jens Alfke
 * Android Port by Marty Schoch, Traun Leyden
 * C# Port by Zack Gramana
 *
 * Copyright (c) 2012, 2013 Couchbase, Inc. All rights reserved.
 * Portions (c) 2013 Xamarin, Inc. All rights reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
 * except in compliance with the License. You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software distributed under the
 * License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
 * either express or implied. See the License for the specific language governing permissions
 * and limitations under the License.
 */

using System;
using System.IO;
using System.Net;
using Apache.Http;
using Apache.Http.Conn.Socket;
using Apache.Http.Conn.Ssl;
using Apache.Http.Protocol;
using Apache.Http.Util;
using Javax.Net;
using Sharpen;

namespace Apache.Http.Conn.Ssl
{
	/// <summary>Layered socket factory for TLS/SSL connections.</summary>
	/// <remarks>
	/// Layered socket factory for TLS/SSL connections.
	/// <p>
	/// SSLSocketFactory can be used to validate the identity of the HTTPS server against a list of
	/// trusted certificates and to authenticate to the HTTPS server using a private key.
	/// <p>
	/// SSLSocketFactory will enable server authentication when supplied with
	/// a
	/// <see cref="Sharpen.KeyStore">trust-store</see>
	/// file containing one or several trusted certificates. The client
	/// secure socket will reject the connection during the SSL session handshake if the target HTTPS
	/// server attempts to authenticate itself with a non-trusted certificate.
	/// <p>
	/// Use JDK keytool utility to import a trusted certificate and generate a trust-store file:
	/// <pre>
	/// keytool -import -alias "my server cert" -file server.crt -keystore my.truststore
	/// </pre>
	/// <p>
	/// In special cases the standard trust verification process can be bypassed by using a custom
	/// <see cref="TrustStrategy">TrustStrategy</see>
	/// . This interface is primarily intended for allowing self-signed
	/// certificates to be accepted as trusted without having to add them to the trust-store file.
	/// <p>
	/// SSLSocketFactory will enable client authentication when supplied with
	/// a
	/// <see cref="Sharpen.KeyStore">key-store</see>
	/// file containing a private key/public certificate
	/// pair. The client secure socket will use the private key to authenticate
	/// itself to the target HTTPS server during the SSL session handshake if
	/// requested to do so by the server.
	/// The target HTTPS server will in its turn verify the certificate presented
	/// by the client in order to establish client's authenticity.
	/// <p>
	/// Use the following sequence of actions to generate a key-store file
	/// </p>
	/// <ul>
	/// <li>
	/// <p>
	/// Use JDK keytool utility to generate a new key
	/// <pre>keytool -genkey -v -alias "my client key" -validity 365 -keystore my.keystore</pre>
	/// For simplicity use the same password for the key as that of the key-store
	/// </p>
	/// </li>
	/// <li>
	/// <p>
	/// Issue a certificate signing request (CSR)
	/// <pre>keytool -certreq -alias "my client key" -file mycertreq.csr -keystore my.keystore</pre>
	/// </p>
	/// </li>
	/// <li>
	/// <p>
	/// Send the certificate request to the trusted Certificate Authority for signature.
	/// One may choose to act as her own CA and sign the certificate request using a PKI
	/// tool, such as OpenSSL.
	/// </p>
	/// </li>
	/// <li>
	/// <p>
	/// Import the trusted CA root certificate
	/// <pre>keytool -import -alias "my trusted ca" -file caroot.crt -keystore my.keystore</pre>
	/// </p>
	/// </li>
	/// <li>
	/// <p>
	/// Import the PKCS#7 file containg the complete certificate chain
	/// <pre>keytool -import -alias "my client key" -file mycert.p7 -keystore my.keystore</pre>
	/// </p>
	/// </li>
	/// <li>
	/// <p>
	/// Verify the content the resultant keystore file
	/// <pre>keytool -list -v -keystore my.keystore</pre>
	/// </p>
	/// </li>
	/// </ul>
	/// </remarks>
	/// <since>4.0</since>
	public class SSLConnectionSocketFactory : LayeredConnectionSocketFactory
	{
		public const string Tls = "TLS";

		public const string Ssl = "SSL";

		public const string Sslv2 = "SSLv2";

		public static readonly X509HostnameVerifier AllowAllHostnameVerifier = new AllowAllHostnameVerifier
			();

		public static readonly X509HostnameVerifier BrowserCompatibleHostnameVerifier = new 
			BrowserCompatHostnameVerifier();

		public static readonly X509HostnameVerifier StrictHostnameVerifier = new StrictHostnameVerifier
			();

		/// <summary>
		/// Obtains default SSL socket factory with an SSL context based on the standard JSSE
		/// trust material (<code>cacerts</code> file in the security properties directory).
		/// </summary>
		/// <remarks>
		/// Obtains default SSL socket factory with an SSL context based on the standard JSSE
		/// trust material (<code>cacerts</code> file in the security properties directory).
		/// System properties are not taken into consideration.
		/// </remarks>
		/// <returns>default SSL socket factory</returns>
		/// <exception cref="Apache.Http.Conn.Ssl.SSLInitializationException"></exception>
		public static Apache.Http.Conn.Ssl.SSLConnectionSocketFactory GetSocketFactory()
		{
			return new Apache.Http.Conn.Ssl.SSLConnectionSocketFactory(SSLContexts.CreateDefault
				(), BrowserCompatibleHostnameVerifier);
		}

		private static string[] Split(string s)
		{
			if (TextUtils.IsBlank(s))
			{
				return null;
			}
			return s.Split(" *, *");
		}

		/// <summary>
		/// Obtains default SSL socket factory with an SSL context based on system properties
		/// as described in
		/// <a href="http://docs.oracle.com/javase/1.5.0/docs/guide/security/jsse/JSSERefGuide.html">
		/// "JavaTM Secure Socket Extension (JSSE) Reference Guide for the JavaTM 2 Platform
		/// Standard Edition 5</a>
		/// </summary>
		/// <returns>default system SSL socket factory</returns>
		/// <exception cref="Apache.Http.Conn.Ssl.SSLInitializationException"></exception>
		public static Apache.Http.Conn.Ssl.SSLConnectionSocketFactory GetSystemSocketFactory
			()
		{
			return new Apache.Http.Conn.Ssl.SSLConnectionSocketFactory((SSLSocketFactory)SSLSocketFactory
				.GetDefault(), Split(Runtime.GetProperty("https.protocols")), Split(Runtime.GetProperty
				("https.cipherSuites")), BrowserCompatibleHostnameVerifier);
		}

		private readonly SSLSocketFactory socketfactory;

		private readonly X509HostnameVerifier hostnameVerifier;

		private readonly string[] supportedProtocols;

		private readonly string[] supportedCipherSuites;

		public SSLConnectionSocketFactory(SSLContext sslContext) : this(sslContext, BrowserCompatibleHostnameVerifier
			)
		{
		}

		public SSLConnectionSocketFactory(SSLContext sslContext, X509HostnameVerifier hostnameVerifier
			) : this(Args.NotNull(sslContext, "SSL context").GetSocketFactory(), null, null, 
			hostnameVerifier)
		{
		}

		public SSLConnectionSocketFactory(SSLContext sslContext, string[] supportedProtocols
			, string[] supportedCipherSuites, X509HostnameVerifier hostnameVerifier) : this(
			Args.NotNull(sslContext, "SSL context").GetSocketFactory(), supportedProtocols, 
			supportedCipherSuites, hostnameVerifier)
		{
		}

		public SSLConnectionSocketFactory(SSLSocketFactory socketfactory, X509HostnameVerifier
			 hostnameVerifier) : this(socketfactory, null, null, hostnameVerifier)
		{
		}

		public SSLConnectionSocketFactory(SSLSocketFactory socketfactory, string[] supportedProtocols
			, string[] supportedCipherSuites, X509HostnameVerifier hostnameVerifier)
		{
			this.socketfactory = Args.NotNull(socketfactory, "SSL socket factory");
			this.supportedProtocols = supportedProtocols;
			this.supportedCipherSuites = supportedCipherSuites;
			this.hostnameVerifier = hostnameVerifier != null ? hostnameVerifier : BrowserCompatibleHostnameVerifier;
		}

		/// <summary>
		/// Performs any custom initialization for a newly created SSLSocket
		/// (before the SSL handshake happens).
		/// </summary>
		/// <remarks>
		/// Performs any custom initialization for a newly created SSLSocket
		/// (before the SSL handshake happens).
		/// The default implementation is a no-op, but could be overriden to, e.g.,
		/// call
		/// <see cref="Sharpen.SSLSocket.SetEnabledCipherSuites(string[])">Sharpen.SSLSocket.SetEnabledCipherSuites(string[])
		/// 	</see>
		/// .
		/// </remarks>
		/// <exception cref="System.IO.IOException"></exception>
		protected internal virtual void PrepareSocket(SSLSocket socket)
		{
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual System.Net.Sockets.Socket CreateSocket(HttpContext context)
		{
			return SocketFactory.GetDefault().CreateSocket();
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual System.Net.Sockets.Socket ConnectSocket(int connectTimeout, System.Net.Sockets.Socket
			 socket, HttpHost host, IPEndPoint remoteAddress, IPEndPoint localAddress, HttpContext
			 context)
		{
			Args.NotNull(host, "HTTP host");
			Args.NotNull(remoteAddress, "Remote address");
			System.Net.Sockets.Socket sock = socket != null ? socket : CreateSocket(context);
			if (localAddress != null)
			{
				sock.Bind2(localAddress);
			}
			try
			{
				sock.Connect(remoteAddress, connectTimeout);
			}
			catch (IOException ex)
			{
				try
				{
					sock.Close();
				}
				catch (IOException)
				{
				}
				throw;
			}
			// Setup SSL layering if necessary
			if (sock is SSLSocket)
			{
				SSLSocket sslsock = (SSLSocket)sock;
				sslsock.StartHandshake();
				VerifyHostname(sslsock, host.GetHostName());
				return sock;
			}
			else
			{
				return CreateLayeredSocket(sock, host.GetHostName(), remoteAddress.Port, context);
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual System.Net.Sockets.Socket CreateLayeredSocket(System.Net.Sockets.Socket
			 socket, string target, int port, HttpContext context)
		{
			SSLSocket sslsock = (SSLSocket)this.socketfactory.CreateSocket(socket, target, port
				, true);
			if (supportedProtocols != null)
			{
				sslsock.SetEnabledProtocols(supportedProtocols);
			}
			if (supportedCipherSuites != null)
			{
				sslsock.SetEnabledCipherSuites(supportedCipherSuites);
			}
			PrepareSocket(sslsock);
			sslsock.StartHandshake();
			VerifyHostname(sslsock, target);
			return sslsock;
		}

		internal virtual X509HostnameVerifier GetHostnameVerifier()
		{
			return this.hostnameVerifier;
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void VerifyHostname(SSLSocket sslsock, string hostname)
		{
			try
			{
				this.hostnameVerifier.Verify(hostname, sslsock);
			}
			catch (IOException iox)
			{
				// verifyHostName() didn't blowup - good!
				// close the socket before re-throwing the exception
				try
				{
					sslsock.Close();
				}
				catch (Exception)
				{
				}
				throw;
			}
		}
	}
}
