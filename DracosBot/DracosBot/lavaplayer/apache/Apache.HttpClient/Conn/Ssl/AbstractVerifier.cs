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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using Apache.Http.Conn.Ssl;
using Apache.Http.Conn.Util;
using Org.Apache.Commons.Logging;
using Sharpen;

namespace Apache.Http.Conn.Ssl
{
	/// <summary>
	/// Abstract base class for all standard
	/// <see cref="X509HostnameVerifier">X509HostnameVerifier</see>
	/// implementations.
	/// </summary>
	/// <since>4.0</since>
	public abstract class AbstractVerifier : X509HostnameVerifier
	{
		/// <summary>
		/// This contains a list of 2nd-level domains that aren't allowed to
		/// have wildcards when combined with country-codes.
		/// </summary>
		/// <remarks>
		/// This contains a list of 2nd-level domains that aren't allowed to
		/// have wildcards when combined with country-codes.
		/// For example: [*.co.uk].
		/// <p/>
		/// The [*.co.uk] problem is an interesting one.  Should we just hope
		/// that CA's would never foolishly allow such a certificate to happen?
		/// Looks like we're the only implementation guarding against this.
		/// Firefox, Curl, Sun Java 1.4, 5, 6 don't bother with this check.
		/// </remarks>
		private static readonly string[] BadCountry2lds = new string[] { "ac", "co", "com"
			, "ed", "edu", "go", "gouv", "gov", "info", "lg", "ne", "net", "or", "org" };

		static AbstractVerifier()
		{
			// Just in case developer forgot to manually sort the array.  :-)
			Arrays.Sort(BadCountry2lds);
		}

		private readonly Log log = LogFactory.GetLog(GetType());

		public AbstractVerifier() : base()
		{
		}

		/// <exception cref="System.IO.IOException"></exception>
		public void Verify(string host, SSLSocket ssl)
		{
			if (host == null)
			{
				throw new ArgumentNullException("host to verify is null");
			}
			SSLSession session = ssl.GetSession();
			if (session == null)
			{
				// In our experience this only happens under IBM 1.4.x when
				// spurious (unrelated) certificates show up in the server'
				// chain.  Hopefully this will unearth the real problem:
				InputStream @in = ssl.GetInputStream();
				@in.Available();
				// If ssl.getInputStream().available() didn't cause an
				// exception, maybe at least now the session is available?
				session = ssl.GetSession();
				if (session == null)
				{
					// If it's still null, probably a startHandshake() will
					// unearth the real problem.
					ssl.StartHandshake();
					// Okay, if we still haven't managed to cause an exception,
					// might as well go for the NPE.  Or maybe we're okay now?
					session = ssl.GetSession();
				}
			}
			Certificate[] certs = session.GetPeerCertificates();
			X509Certificate x509 = (X509Certificate)certs[0];
			Verify(host, x509);
		}

		public bool Verify(string host, SSLSession session)
		{
			try
			{
				Certificate[] certs = session.GetPeerCertificates();
				X509Certificate x509 = (X509Certificate)certs[0];
				Verify(host, x509);
				return true;
			}
			catch (SSLException)
			{
				return false;
			}
		}

		/// <exception cref="Sharpen.SSLException"></exception>
		public void Verify(string host, X509Certificate cert)
		{
			string[] cns = GetCNs(cert);
			string[] subjectAlts = GetSubjectAlts(cert, host);
			Verify(host, cns, subjectAlts);
		}

		/// <exception cref="Sharpen.SSLException"></exception>
		public void Verify(string host, string[] cns, string[] subjectAlts, bool strictWithSubDomains
			)
		{
			// Build the list of names we're going to check.  Our DEFAULT and
			// STRICT implementations of the HostnameVerifier only use the
			// first CN provided.  All other CNs are ignored.
			// (Firefox, wget, curl, Sun Java 1.4, 5, 6 all work this way).
			List<string> names = new List<string>();
			if (cns != null && cns.Length > 0 && cns[0] != null)
			{
				names.AddItem(cns[0]);
			}
			if (subjectAlts != null)
			{
				foreach (string subjectAlt in subjectAlts)
				{
					if (subjectAlt != null)
					{
						names.AddItem(subjectAlt);
					}
				}
			}
			if (names.IsEmpty())
			{
				string msg = "Certificate for <" + host + "> doesn't contain CN or DNS subjectAlt";
				throw new SSLException(msg);
			}
			// StringBuilder for building the error message.
			StringBuilder buf = new StringBuilder();
			// We're can be case-insensitive when comparing the host we used to
			// establish the socket to the hostname in the certificate.
			string hostName = NormaliseIPv6Address(host.Trim().ToLower(CultureInfo.InvariantCulture
				));
			bool match = false;
			for (IEnumerator<string> it = names.GetEnumerator(); it.HasNext(); )
			{
				// Don't trim the CN, though!
				string cn = it.Next();
				cn = cn.ToLower(CultureInfo.InvariantCulture);
				// Store CN in StringBuilder in case we need to report an error.
				buf.Append(" <");
				buf.Append(cn);
				buf.Append('>');
				if (it.HasNext())
				{
					buf.Append(" OR");
				}
				// The CN better have at least two dots if it wants wildcard
				// action.  It also can't be [*.co.uk] or [*.co.jp] or
				// [*.org.uk], etc...
				string[] parts = cn.Split("\\.");
				bool doWildcard = parts.Length >= 3 && parts[0].EndsWith("*") && ValidCountryWildcard
					(cn) && !IsIPAddress(host);
				if (doWildcard)
				{
					string firstpart = parts[0];
					if (firstpart.Length > 1)
					{
						// e.g. server*
						string prefix = Sharpen.Runtime.Substring(firstpart, 0, firstpart.Length - 1);
						// e.g. server
						string suffix = Sharpen.Runtime.Substring(cn, firstpart.Length);
						// skip wildcard part from cn
						string hostSuffix = Sharpen.Runtime.Substring(hostName, prefix.Length);
						// skip wildcard part from host
						match = hostName.StartsWith(prefix) && hostSuffix.EndsWith(suffix);
					}
					else
					{
						match = hostName.EndsWith(Sharpen.Runtime.Substring(cn, 1));
					}
					if (match && strictWithSubDomains)
					{
						// If we're in strict mode, then [*.foo.com] is not
						// allowed to match [a.b.foo.com]
						match = CountDots(hostName) == CountDots(cn);
					}
				}
				else
				{
					match = hostName.Equals(NormaliseIPv6Address(cn));
				}
				if (match)
				{
					break;
				}
			}
			if (!match)
			{
				throw new SSLException("hostname in certificate didn't match: <" + host + "> !=" 
					+ buf);
			}
		}

		[Obsolete]
		[System.ObsoleteAttribute(@"(4.3.1) should not be a part of public APIs.")]
		public static bool AcceptableCountryWildcard(string cn)
		{
			string[] parts = cn.Split("\\.");
			if (parts.Length != 3 || parts[2].Length != 2)
			{
				return true;
			}
			// it's not an attempt to wildcard a 2TLD within a country code
			return System.Array.BinarySearch(BadCountry2lds, parts[1]) < 0;
		}

		internal virtual bool ValidCountryWildcard(string cn)
		{
			string[] parts = cn.Split("\\.");
			if (parts.Length != 3 || parts[2].Length != 2)
			{
				return true;
			}
			// it's not an attempt to wildcard a 2TLD within a country code
			return System.Array.BinarySearch(BadCountry2lds, parts[1]) < 0;
		}

		public static string[] GetCNs(X509Certificate cert)
		{
			List<string> cnList = new List<string>();
			string subjectPrincipal = cert.GetSubjectX500Principal().ToString();
			StringTokenizer st = new StringTokenizer(subjectPrincipal, ",+");
			while (st.HasMoreTokens())
			{
				string tok = st.NextToken().Trim();
				if (tok.Length > 3)
				{
					if (Sharpen.Runtime.EqualsIgnoreCase(Sharpen.Runtime.Substring(tok, 0, 3), "CN="))
					{
						cnList.AddItem(Sharpen.Runtime.Substring(tok, 3));
					}
				}
			}
			if (!cnList.IsEmpty())
			{
				string[] cns = new string[cnList.Count];
				Sharpen.Collections.ToArray(cnList, cns);
				return cns;
			}
			else
			{
				return null;
			}
		}

		/// <summary>Extracts the array of SubjectAlt DNS or IP names from an X509Certificate.
		/// 	</summary>
		/// <remarks>
		/// Extracts the array of SubjectAlt DNS or IP names from an X509Certificate.
		/// Returns null if there aren't any.
		/// </remarks>
		/// <param name="cert">X509Certificate</param>
		/// <param name="hostname"></param>
		/// <returns>Array of SubjectALT DNS or IP names stored in the certificate.</returns>
		private static string[] GetSubjectAlts(X509Certificate cert, string hostname)
		{
			int subjectType;
			if (IsIPAddress(hostname))
			{
				subjectType = 7;
			}
			else
			{
				subjectType = 2;
			}
			List<string> subjectAltList = new List<string>();
			ICollection<IList<object>> c = null;
			try
			{
				c = cert.GetSubjectAlternativeNames();
			}
			catch (CertificateParsingException)
			{
			}
			if (c != null)
			{
				foreach (IList<object> aC in c)
				{
					IList<object> list = aC;
					int type = ((int)list[0]);
					if (type == subjectType)
					{
						string s = (string)list[1];
						subjectAltList.AddItem(s);
					}
				}
			}
			if (!subjectAltList.IsEmpty())
			{
				string[] subjectAlts = new string[subjectAltList.Count];
				Sharpen.Collections.ToArray(subjectAltList, subjectAlts);
				return subjectAlts;
			}
			else
			{
				return null;
			}
		}

		/// <summary>Extracts the array of SubjectAlt DNS names from an X509Certificate.</summary>
		/// <remarks>
		/// Extracts the array of SubjectAlt DNS names from an X509Certificate.
		/// Returns null if there aren't any.
		/// <p/>
		/// Note:  Java doesn't appear able to extract international characters
		/// from the SubjectAlts.  It can only extract international characters
		/// from the CN field.
		/// <p/>
		/// (Or maybe the version of OpenSSL I'm using to test isn't storing the
		/// international characters correctly in the SubjectAlts?).
		/// </remarks>
		/// <param name="cert">X509Certificate</param>
		/// <returns>Array of SubjectALT DNS names stored in the certificate.</returns>
		public static string[] GetDNSSubjectAlts(X509Certificate cert)
		{
			return GetSubjectAlts(cert, null);
		}

		/// <summary>Counts the number of dots "." in a string.</summary>
		/// <remarks>Counts the number of dots "." in a string.</remarks>
		/// <param name="s">string to count dots from</param>
		/// <returns>number of dots</returns>
		public static int CountDots(string s)
		{
			int count = 0;
			for (int i = 0; i < s.Length; i++)
			{
				if (s[i] == '.')
				{
					count++;
				}
			}
			return count;
		}

		private static bool IsIPAddress(string hostname)
		{
			return hostname != null && (InetAddressUtils.IsIPv4Address(hostname) || InetAddressUtils
				.IsIPv6Address(hostname));
		}

		private string NormaliseIPv6Address(string hostname)
		{
			if (hostname == null || !InetAddressUtils.IsIPv6Address(hostname))
			{
				return hostname;
			}
			try
			{
				IPAddress inetAddress = Sharpen.Extensions.GetAddressByName(hostname);
				return inetAddress.GetHostAddress();
			}
			catch (UnknownHostException uhe)
			{
				// Should not happen, because we check for IPv6 address above
				log.Error("Unexpected error converting " + hostname, uhe);
				return hostname;
			}
		}

		public abstract void Verify(string arg1, string[] arg2, string[] arg3);
	}
}
