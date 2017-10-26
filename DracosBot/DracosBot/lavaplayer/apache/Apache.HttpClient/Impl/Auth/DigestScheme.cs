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
using System.Text;
using Apache.Http;
using Apache.Http.Auth;
using Apache.Http.Impl.Auth;
using Apache.Http.Message;
using Apache.Http.Protocol;
using Apache.Http.Util;
using Org.Apache.Http;
using Sharpen;

namespace Apache.Http.Impl.Auth
{
	/// <summary>Digest authentication scheme as defined in RFC 2617.</summary>
	/// <remarks>
	/// Digest authentication scheme as defined in RFC 2617.
	/// Both MD5 (default) and MD5-sess are supported.
	/// Currently only qop=auth or no qop is supported. qop=auth-int
	/// is unsupported. If auth and auth-int are provided, auth is
	/// used.
	/// <p/>
	/// Since the digest username is included as clear text in the generated
	/// Authentication header, the charset of the username must be compatible
	/// with the HTTP element charset used by the connection.
	/// </remarks>
	/// <since>4.0</since>
	public class DigestScheme : RFC2617Scheme
	{
		/// <summary>
		/// Hexa values used when creating 32 character long digest in HTTP DigestScheme
		/// in case of authentication.
		/// </summary>
		/// <remarks>
		/// Hexa values used when creating 32 character long digest in HTTP DigestScheme
		/// in case of authentication.
		/// </remarks>
		/// <seealso cref="Encode(byte[])">Encode(byte[])</seealso>
		private static readonly char[] Hexadecimal = new char[] { '0', '1', '2', '3', '4'
			, '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };

		/// <summary>Whether the digest authentication process is complete</summary>
		private bool complete;

		private const int QopUnknown = -1;

		private const int QopMissing = 0;

		private const int QopAuthInt = 1;

		private const int QopAuth = 2;

		private string lastNonce;

		private long nounceCount;

		private string cnonce;

		private string a1;

		private string a2;

		/// <since>4.3</since>
		public DigestScheme(Encoding credentialsCharset) : base(credentialsCharset)
		{
			this.complete = false;
		}

		/// <summary>
		/// Creates an instance of <tt>DigestScheme</tt> with the given challenge
		/// state.
		/// </summary>
		/// <remarks>
		/// Creates an instance of <tt>DigestScheme</tt> with the given challenge
		/// state.
		/// </remarks>
		/// <since>4.2</since>
		[Obsolete]
		[System.ObsoleteAttribute(@"(4.3) do not use.")]
		public DigestScheme(ChallengeState challengeState) : base(challengeState)
		{
		}

		public DigestScheme() : this(Consts.Ascii)
		{
		}

		/// <summary>Processes the Digest challenge.</summary>
		/// <remarks>Processes the Digest challenge.</remarks>
		/// <param name="header">the challenge header</param>
		/// <exception cref="Apache.Http.Auth.MalformedChallengeException">
		/// is thrown if the authentication challenge
		/// is malformed
		/// </exception>
		public override void ProcessChallenge(Header header)
		{
			base.ProcessChallenge(header);
			this.complete = true;
		}

		/// <summary>Tests if the Digest authentication process has been completed.</summary>
		/// <remarks>Tests if the Digest authentication process has been completed.</remarks>
		/// <returns>
		/// <tt>true</tt> if Digest authorization has been processed,
		/// <tt>false</tt> otherwise.
		/// </returns>
		public override bool IsComplete()
		{
			string s = GetParameter("stale");
			if (Sharpen.Runtime.EqualsIgnoreCase("true", s))
			{
				return false;
			}
			else
			{
				return this.complete;
			}
		}

		/// <summary>Returns textual designation of the digest authentication scheme.</summary>
		/// <remarks>Returns textual designation of the digest authentication scheme.</remarks>
		/// <returns><code>digest</code></returns>
		public override string GetSchemeName()
		{
			return "digest";
		}

		/// <summary>Returns <tt>false</tt>.</summary>
		/// <remarks>Returns <tt>false</tt>. Digest authentication scheme is request based.</remarks>
		/// <returns><tt>false</tt>.</returns>
		public override bool IsConnectionBased()
		{
			return false;
		}

		public virtual void OverrideParamter(string name, string value)
		{
			GetParameters().Put(name, value);
		}

		/// <exception cref="Apache.Http.Auth.AuthenticationException"></exception>
		[Obsolete]
		[System.ObsoleteAttribute(@"(4.2) Use Apache.Http.Auth.ContextAwareAuthScheme.Authenticate(Apache.Http.Auth.Credentials, Org.Apache.Http.IHttpRequest, Apache.Http.Protocol.HttpContext)"
			)]
		public override Header Authenticate(Credentials credentials, IHttpRequest request
			)
		{
			return Authenticate(credentials, request, new BasicHttpContext());
		}

		/// <summary>
		/// Produces a digest authorization string for the given set of
		/// <see cref="Apache.Http.Auth.Credentials">Apache.Http.Auth.Credentials</see>
		/// , method name and URI.
		/// </summary>
		/// <param name="credentials">A set of credentials to be used for athentication</param>
		/// <param name="request">The request being authenticated</param>
		/// <exception cref="Apache.Http.Auth.InvalidCredentialsException">
		/// if authentication credentials
		/// are not valid or not applicable for this authentication scheme
		/// </exception>
		/// <exception cref="Apache.Http.Auth.AuthenticationException">
		/// if authorization string cannot
		/// be generated due to an authentication failure
		/// </exception>
		/// <returns>a digest authorization string</returns>
		public override Header Authenticate(Credentials credentials, IHttpRequest request
			, HttpContext context)
		{
			Args.NotNull(credentials, "Credentials");
			Args.NotNull(request, "HTTP request");
			if (GetParameter("realm") == null)
			{
				throw new AuthenticationException("missing realm in challenge");
			}
			if (GetParameter("nonce") == null)
			{
				throw new AuthenticationException("missing nonce in challenge");
			}
			// Add method name and request-URI to the parameter map
			GetParameters().Put("methodname", request.GetRequestLine().GetMethod());
			GetParameters().Put("uri", request.GetRequestLine().GetUri());
			string charset = GetParameter("charset");
			if (charset == null)
			{
				GetParameters().Put("charset", GetCredentialsCharset(request));
			}
			return CreateDigestHeader(credentials, request);
		}

		/// <exception cref="Apache.Http.Impl.Auth.UnsupportedDigestAlgorithmException"></exception>
		private static MessageDigest CreateMessageDigest(string digAlg)
		{
			try
			{
				return MessageDigest.GetInstance(digAlg);
			}
			catch (Exception)
			{
				throw new UnsupportedDigestAlgorithmException("Unsupported algorithm in HTTP Digest authentication: "
					 + digAlg);
			}
		}

		/// <summary>Creates digest-response header as defined in RFC2617.</summary>
		/// <remarks>Creates digest-response header as defined in RFC2617.</remarks>
		/// <param name="credentials">User credentials</param>
		/// <returns>The digest-response as String.</returns>
		/// <exception cref="Apache.Http.Auth.AuthenticationException"></exception>
		private Header CreateDigestHeader(Credentials credentials, IHttpRequest request)
		{
			string uri = GetParameter("uri");
			string realm = GetParameter("realm");
			string nonce = GetParameter("nonce");
			string opaque = GetParameter("opaque");
			string method = GetParameter("methodname");
			string algorithm = GetParameter("algorithm");
			// If an algorithm is not specified, default to MD5.
			if (algorithm == null)
			{
				algorithm = "MD5";
			}
			ICollection<string> qopset = new HashSet<string>(8);
			int qop = QopUnknown;
			string qoplist = GetParameter("qop");
			if (qoplist != null)
			{
				StringTokenizer tok = new StringTokenizer(qoplist, ",");
				while (tok.HasMoreTokens())
				{
					string variant = tok.NextToken().Trim();
					qopset.AddItem(variant.ToLower(CultureInfo.InvariantCulture));
				}
				if (request is HttpEntityEnclosingRequest && qopset.Contains("auth-int"))
				{
					qop = QopAuthInt;
				}
				else
				{
					if (qopset.Contains("auth"))
					{
						qop = QopAuth;
					}
				}
			}
			else
			{
				qop = QopMissing;
			}
			if (qop == QopUnknown)
			{
				throw new AuthenticationException("None of the qop methods is supported: " + qoplist
					);
			}
			string charset = GetParameter("charset");
			if (charset == null)
			{
				charset = "ISO-8859-1";
			}
			string digAlg = algorithm;
			if (Sharpen.Runtime.EqualsIgnoreCase(digAlg, "MD5-sess"))
			{
				digAlg = "MD5";
			}
			MessageDigest digester;
			try
			{
				digester = CreateMessageDigest(digAlg);
			}
			catch (UnsupportedDigestAlgorithmException)
			{
				throw new AuthenticationException("Unsuppported digest algorithm: " + digAlg);
			}
			string uname = credentials.GetUserPrincipal().GetName();
			string pwd = credentials.GetPassword();
			if (nonce.Equals(this.lastNonce))
			{
				nounceCount++;
			}
			else
			{
				nounceCount = 1;
				cnonce = null;
				lastNonce = nonce;
			}
			StringBuilder sb = new StringBuilder(256);
			Formatter formatter = new Formatter(sb, CultureInfo.InvariantCulture);
			formatter.Format("%08x", nounceCount);
			formatter.Close();
			string nc = sb.ToString();
			if (cnonce == null)
			{
				cnonce = CreateCnonce();
			}
			a1 = null;
			a2 = null;
			// 3.2.2.2: Calculating digest
			if (Sharpen.Runtime.EqualsIgnoreCase(algorithm, "MD5-sess"))
			{
				// H( unq(username-value) ":" unq(realm-value) ":" passwd )
				//      ":" unq(nonce-value)
				//      ":" unq(cnonce-value)
				// calculated one per session
				sb.Length = 0;
				sb.Append(uname).Append(':').Append(realm).Append(':').Append(pwd);
				string checksum = Encode(digester.Digest(EncodingUtils.GetBytes(sb.ToString(), charset
					)));
				sb.Length = 0;
				sb.Append(checksum).Append(':').Append(nonce).Append(':').Append(cnonce);
				a1 = sb.ToString();
			}
			else
			{
				// unq(username-value) ":" unq(realm-value) ":" passwd
				sb.Length = 0;
				sb.Append(uname).Append(':').Append(realm).Append(':').Append(pwd);
				a1 = sb.ToString();
			}
			string hasha1 = Encode(digester.Digest(EncodingUtils.GetBytes(a1, charset)));
			if (qop == QopAuth)
			{
				// Method ":" digest-uri-value
				a2 = method + ':' + uri;
			}
			else
			{
				if (qop == QopAuthInt)
				{
					// Method ":" digest-uri-value ":" H(entity-body)
					HttpEntity entity = null;
					if (request is HttpEntityEnclosingRequest)
					{
						entity = ((HttpEntityEnclosingRequest)request).GetEntity();
					}
					if (entity != null && !entity.IsRepeatable())
					{
						// If the entity is not repeatable, try falling back onto QOP_AUTH
						if (qopset.Contains("auth"))
						{
							qop = QopAuth;
							a2 = method + ':' + uri;
						}
						else
						{
							throw new AuthenticationException("Qop auth-int cannot be used with " + "a non-repeatable entity"
								);
						}
					}
					else
					{
						HttpEntityDigester entityDigester = new HttpEntityDigester(digester);
						try
						{
							if (entity != null)
							{
								entity.WriteTo(entityDigester);
							}
							entityDigester.Close();
						}
						catch (IOException ex)
						{
							throw new AuthenticationException("I/O error reading entity content", ex);
						}
						a2 = method + ':' + uri + ':' + Encode(entityDigester.GetDigest());
					}
				}
				else
				{
					a2 = method + ':' + uri;
				}
			}
			string hasha2 = Encode(digester.Digest(EncodingUtils.GetBytes(a2, charset)));
			// 3.2.2.1
			string digestValue;
			if (qop == QopMissing)
			{
				sb.Length = 0;
				sb.Append(hasha1).Append(':').Append(nonce).Append(':').Append(hasha2);
				digestValue = sb.ToString();
			}
			else
			{
				sb.Length = 0;
				sb.Append(hasha1).Append(':').Append(nonce).Append(':').Append(nc).Append(':').Append
					(cnonce).Append(':').Append(qop == QopAuthInt ? "auth-int" : "auth").Append(':')
					.Append(hasha2);
				digestValue = sb.ToString();
			}
			string digest = Encode(digester.Digest(EncodingUtils.GetAsciiBytes(digestValue)));
			CharArrayBuffer buffer = new CharArrayBuffer(128);
			if (IsProxy())
			{
				buffer.Append(AUTH.ProxyAuthResp);
			}
			else
			{
				buffer.Append(AUTH.WwwAuthResp);
			}
			buffer.Append(": Digest ");
			IList<BasicNameValuePair> @params = new AList<BasicNameValuePair>(20);
			@params.AddItem(new BasicNameValuePair("username", uname));
			@params.AddItem(new BasicNameValuePair("realm", realm));
			@params.AddItem(new BasicNameValuePair("nonce", nonce));
			@params.AddItem(new BasicNameValuePair("uri", uri));
			@params.AddItem(new BasicNameValuePair("response", digest));
			if (qop != QopMissing)
			{
				@params.AddItem(new BasicNameValuePair("qop", qop == QopAuthInt ? "auth-int" : "auth"
					));
				@params.AddItem(new BasicNameValuePair("nc", nc));
				@params.AddItem(new BasicNameValuePair("cnonce", cnonce));
			}
			// algorithm cannot be null here
			@params.AddItem(new BasicNameValuePair("algorithm", algorithm));
			if (opaque != null)
			{
				@params.AddItem(new BasicNameValuePair("opaque", opaque));
			}
			for (int i = 0; i < @params.Count; i++)
			{
				BasicNameValuePair param = @params[i];
				if (i > 0)
				{
					buffer.Append(", ");
				}
				string name = param.GetName();
				bool noQuotes = ("nc".Equals(name) || "qop".Equals(name) || "algorithm".Equals(name
					));
				BasicHeaderValueFormatter.Instance.FormatNameValuePair(buffer, param, !noQuotes);
			}
			return new BufferedHeader(buffer);
		}

		internal virtual string GetCnonce()
		{
			return cnonce;
		}

		internal virtual string GetA1()
		{
			return a1;
		}

		internal virtual string GetA2()
		{
			return a2;
		}

		/// <summary>
		/// Encodes the 128 bit (16 bytes) MD5 digest into a 32 characters long
		/// <CODE>String</CODE> according to RFC 2617.
		/// </summary>
		/// <remarks>
		/// Encodes the 128 bit (16 bytes) MD5 digest into a 32 characters long
		/// <CODE>String</CODE> according to RFC 2617.
		/// </remarks>
		/// <param name="binaryData">array containing the digest</param>
		/// <returns>encoded MD5, or <CODE>null</CODE> if encoding failed</returns>
		internal static string Encode(byte[] binaryData)
		{
			int n = binaryData.Length;
			char[] buffer = new char[n * 2];
			for (int i = 0; i < n; i++)
			{
				int low = (binaryData[i] & unchecked((int)(0x0f)));
				int high = ((binaryData[i] & unchecked((int)(0xf0))) >> 4);
				buffer[i * 2] = Hexadecimal[high];
				buffer[(i * 2) + 1] = Hexadecimal[low];
			}
			return new string(buffer);
		}

		/// <summary>Creates a random cnonce value based on the current time.</summary>
		/// <remarks>Creates a random cnonce value based on the current time.</remarks>
		/// <returns>The cnonce value as String.</returns>
		public static string CreateCnonce()
		{
			SecureRandom rnd = new SecureRandom();
			byte[] tmp = new byte[8];
			rnd.NextBytes(tmp);
			return Encode(tmp);
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			builder.Append("DIGEST [complete=").Append(complete).Append(", nonce=").Append(lastNonce
				).Append(", nc=").Append(nounceCount).Append("]");
			return builder.ToString();
		}
	}
}
