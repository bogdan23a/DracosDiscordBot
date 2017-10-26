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
using System.Globalization;
using System.IO;
using Apache.Http.Impl.Auth;
using Apache.Http.Util;
using Org.Apache.Commons.Codec.Binary;
using Sharpen;

namespace Apache.Http.Impl.Auth
{
	/// <summary>
	/// Provides an implementation for NTLMv1, NTLMv2, and NTLM2 Session forms of the NTLM
	/// authentication protocol.
	/// </summary>
	/// <remarks>
	/// Provides an implementation for NTLMv1, NTLMv2, and NTLM2 Session forms of the NTLM
	/// authentication protocol.
	/// </remarks>
	/// <since>4.1</since>
	internal sealed class NTLMEngineImpl : NTLMEngine
	{
		protected internal const int FlagRequestUnicodeEncoding = unchecked((int)(0x00000001
			));

		protected internal const int FlagRequestTarget = unchecked((int)(0x00000004));

		protected internal const int FlagRequestSign = unchecked((int)(0x00000010));

		protected internal const int FlagRequestSeal = unchecked((int)(0x00000020));

		protected internal const int FlagRequestLanManagerKey = unchecked((int)(0x00000080
			));

		protected internal const int FLAG_REQUEST_NTLMv1 = unchecked((int)(0x00000200));

		protected internal const int FlagDomainPresent = unchecked((int)(0x00001000));

		protected internal const int FlagWorkstationPresent = unchecked((int)(0x00002000)
			);

		protected internal const int FlagRequestAlwaysSign = unchecked((int)(0x00008000));

		protected internal const int FlagRequestNtlm2Session = unchecked((int)(0x00080000
			));

		protected internal const int FlagRequestVersion = unchecked((int)(0x02000000));

		protected internal const int FlagTargetinfoPresent = unchecked((int)(0x00800000));

		protected internal const int FlagRequest128bitKeyExch = unchecked((int)(0x20000000
			));

		protected internal const int FlagRequestExplicitKeyExch = unchecked((int)(0x40000000
			));

		protected internal const int FlagRequest56bitEncryption = unchecked((int)(0x80000000
			));

		/// <summary>Secure random generator</summary>
		private static readonly SecureRandom RndGen;

		static NTLMEngineImpl()
		{
			// Flags we use; descriptions according to:
			// http://davenport.sourceforge.net/ntlm.html
			// and
			// http://msdn.microsoft.com/en-us/library/cc236650%28v=prot.20%29.aspx
			// Unicode string encoding requested
			// Requests target field
			// Requests all messages have a signature attached, in NEGOTIATE message.
			// Request key exchange for message confidentiality in NEGOTIATE message.  MUST be used in conjunction with 56BIT.
			// Request Lan Manager key instead of user session key
			// Request NTLMv1 security.  MUST be set in NEGOTIATE and CHALLENGE both
			// Domain is present in message
			// Workstation is present in message
			// Requests a signature block on all messages.  Overridden by REQUEST_SIGN and REQUEST_SEAL.
			// From server in challenge, requesting NTLM2 session security
			// Request protocol version
			// From server in challenge message, indicating targetinfo is present
			// Request explicit 128-bit key exchange
			// Request explicit key exchange
			// Must be used in conjunction with SEAL
			SecureRandom rnd = null;
			try
			{
				rnd = SecureRandom.GetInstance("SHA1PRNG");
			}
			catch (Exception)
			{
			}
			RndGen = rnd;
		}

		/// <summary>Character encoding</summary>
		internal const string DefaultCharset = "ASCII";

		/// <summary>The character set to use for encoding the credentials</summary>
		private string credentialCharset = DefaultCharset;

		/// <summary>The signature string as bytes in the default encoding</summary>
		private static readonly byte[] Signature;

		static NTLMEngineImpl()
		{
			byte[] bytesWithoutNull = EncodingUtils.GetBytes("NTLMSSP", "ASCII");
			Signature = new byte[bytesWithoutNull.Length + 1];
			System.Array.Copy(bytesWithoutNull, 0, Signature, 0, bytesWithoutNull.Length);
			Signature[bytesWithoutNull.Length] = unchecked((byte)unchecked((int)(0x00)));
		}

		/// <summary>Returns the response for the given message.</summary>
		/// <remarks>Returns the response for the given message.</remarks>
		/// <param name="message">the message that was received from the server.</param>
		/// <param name="username">the username to authenticate with.</param>
		/// <param name="password">the password to authenticate with.</param>
		/// <param name="host">The host.</param>
		/// <param name="domain">the NT domain to authenticate in.</param>
		/// <returns>The response.</returns>
		/// <exception cref="Apache.Http.HttpException">If the messages cannot be retrieved.</exception>
		/// <exception cref="Apache.Http.Impl.Auth.NTLMEngineException"></exception>
		internal string GetResponseFor(string message, string username, string password, 
			string host, string domain)
		{
			string response;
			if (message == null || message.Trim().Equals(string.Empty))
			{
				response = GetType1Message(host, domain);
			}
			else
			{
				NTLMEngineImpl.Type2Message t2m = new NTLMEngineImpl.Type2Message(message);
				response = GetType3Message(username, password, host, domain, t2m.GetChallenge(), 
					t2m.GetFlags(), t2m.GetTarget(), t2m.GetTargetInfo());
			}
			return response;
		}

		/// <summary>
		/// Creates the first message (type 1 message) in the NTLM authentication
		/// sequence.
		/// </summary>
		/// <remarks>
		/// Creates the first message (type 1 message) in the NTLM authentication
		/// sequence. This message includes the user name, domain and host for the
		/// authentication session.
		/// </remarks>
		/// <param name="host">the computer name of the host requesting authentication.</param>
		/// <param name="domain">The domain to authenticate with.</param>
		/// <returns>String the message to add to the HTTP request header.</returns>
		/// <exception cref="Apache.Http.Impl.Auth.NTLMEngineException"></exception>
		internal string GetType1Message(string host, string domain)
		{
			return new NTLMEngineImpl.Type1Message(domain, host).GetResponse();
		}

		/// <summary>Creates the type 3 message using the given server nonce.</summary>
		/// <remarks>
		/// Creates the type 3 message using the given server nonce. The type 3
		/// message includes all the information for authentication, host, domain,
		/// username and the result of encrypting the nonce sent by the server using
		/// the user's password as the key.
		/// </remarks>
		/// <param name="user">The user name. This should not include the domain name.</param>
		/// <param name="password">The password.</param>
		/// <param name="host">The host that is originating the authentication request.</param>
		/// <param name="domain">The domain to authenticate within.</param>
		/// <param name="nonce">the 8 byte array the server sent.</param>
		/// <returns>The type 3 message.</returns>
		/// <exception cref="NTLMEngineException">
		/// If
		/// fails.
		/// </exception>
		/// <exception cref="Apache.Http.Impl.Auth.NTLMEngineException"></exception>
		internal string GetType3Message(string user, string password, string host, string
			 domain, byte[] nonce, int type2Flags, string target, byte[] targetInformation)
		{
			return new NTLMEngineImpl.Type3Message(domain, host, user, password, nonce, type2Flags
				, target, targetInformation).GetResponse();
		}

		/// <returns>Returns the credentialCharset.</returns>
		internal string GetCredentialCharset()
		{
			return credentialCharset;
		}

		/// <param name="credentialCharset">The credentialCharset to set.</param>
		internal void SetCredentialCharset(string credentialCharset)
		{
			this.credentialCharset = credentialCharset;
		}

		/// <summary>Strip dot suffix from a name</summary>
		private static string StripDotSuffix(string value)
		{
			if (value == null)
			{
				return null;
			}
			int index = value.IndexOf(".");
			if (index != -1)
			{
				return Sharpen.Runtime.Substring(value, 0, index);
			}
			return value;
		}

		/// <summary>Convert host to standard form</summary>
		private static string ConvertHost(string host)
		{
			return StripDotSuffix(host);
		}

		/// <summary>Convert domain to standard form</summary>
		private static string ConvertDomain(string domain)
		{
			return StripDotSuffix(domain);
		}

		/// <exception cref="Apache.Http.Impl.Auth.NTLMEngineException"></exception>
		private static int ReadULong(byte[] src, int index)
		{
			if (src.Length < index + 4)
			{
				throw new NTLMEngineException("NTLM authentication - buffer too small for DWORD");
			}
			return (src[index] & unchecked((int)(0xff))) | ((src[index + 1] & unchecked((int)
				(0xff))) << 8) | ((src[index + 2] & unchecked((int)(0xff))) << 16) | ((src[index
				 + 3] & unchecked((int)(0xff))) << 24);
		}

		/// <exception cref="Apache.Http.Impl.Auth.NTLMEngineException"></exception>
		private static int ReadUShort(byte[] src, int index)
		{
			if (src.Length < index + 2)
			{
				throw new NTLMEngineException("NTLM authentication - buffer too small for WORD");
			}
			return (src[index] & unchecked((int)(0xff))) | ((src[index + 1] & unchecked((int)
				(0xff))) << 8);
		}

		/// <exception cref="Apache.Http.Impl.Auth.NTLMEngineException"></exception>
		private static byte[] ReadSecurityBuffer(byte[] src, int index)
		{
			int length = ReadUShort(src, index);
			int offset = ReadULong(src, index + 4);
			if (src.Length < offset + length)
			{
				throw new NTLMEngineException("NTLM authentication - buffer too small for data item"
					);
			}
			byte[] buffer = new byte[length];
			System.Array.Copy(src, offset, buffer, 0, length);
			return buffer;
		}

		/// <summary>Calculate a challenge block</summary>
		/// <exception cref="Apache.Http.Impl.Auth.NTLMEngineException"></exception>
		private static byte[] MakeRandomChallenge()
		{
			if (RndGen == null)
			{
				throw new NTLMEngineException("Random generator not available");
			}
			byte[] rval = new byte[8];
			lock (RndGen)
			{
				RndGen.NextBytes(rval);
			}
			return rval;
		}

		/// <summary>Calculate a 16-byte secondary key</summary>
		/// <exception cref="Apache.Http.Impl.Auth.NTLMEngineException"></exception>
		private static byte[] MakeSecondaryKey()
		{
			if (RndGen == null)
			{
				throw new NTLMEngineException("Random generator not available");
			}
			byte[] rval = new byte[16];
			lock (RndGen)
			{
				RndGen.NextBytes(rval);
			}
			return rval;
		}

		protected internal class CipherGen
		{
			protected internal readonly string domain;

			protected internal readonly string user;

			protected internal readonly string password;

			protected internal readonly byte[] challenge;

			protected internal readonly string target;

			protected internal readonly byte[] targetInformation;

			protected internal byte[] clientChallenge;

			protected internal byte[] clientChallenge2;

			protected internal byte[] secondaryKey;

			protected internal byte[] timestamp;

			protected internal byte[] lmHash = null;

			protected internal byte[] lmResponse = null;

			protected internal byte[] ntlmHash = null;

			protected internal byte[] ntlmResponse = null;

			protected internal byte[] ntlmv2Hash = null;

			protected internal byte[] lmv2Hash = null;

			protected internal byte[] lmv2Response = null;

			protected internal byte[] ntlmv2Blob = null;

			protected internal byte[] ntlmv2Response = null;

			protected internal byte[] ntlm2SessionResponse = null;

			protected internal byte[] lm2SessionResponse = null;

			protected internal byte[] lmUserSessionKey = null;

			protected internal byte[] ntlmUserSessionKey = null;

			protected internal byte[] ntlmv2UserSessionKey = null;

			protected internal byte[] ntlm2SessionResponseUserSessionKey = null;

			protected internal byte[] lanManagerSessionKey = null;

			public CipherGen(string domain, string user, string password, byte[] challenge, string
				 target, byte[] targetInformation, byte[] clientChallenge, byte[] clientChallenge2
				, byte[] secondaryKey, byte[] timestamp)
			{
				// Information we can generate but may be passed in (for testing)
				// Stuff we always generate
				this.domain = domain;
				this.target = target;
				this.user = user;
				this.password = password;
				this.challenge = challenge;
				this.targetInformation = targetInformation;
				this.clientChallenge = clientChallenge;
				this.clientChallenge2 = clientChallenge2;
				this.secondaryKey = secondaryKey;
				this.timestamp = timestamp;
			}

			public CipherGen(string domain, string user, string password, byte[] challenge, string
				 target, byte[] targetInformation) : this(domain, user, password, challenge, target
				, targetInformation, null, null, null, null)
			{
			}

			/// <summary>Calculate and return client challenge</summary>
			/// <exception cref="Apache.Http.Impl.Auth.NTLMEngineException"></exception>
			public virtual byte[] GetClientChallenge()
			{
				if (clientChallenge == null)
				{
					clientChallenge = MakeRandomChallenge();
				}
				return clientChallenge;
			}

			/// <summary>Calculate and return second client challenge</summary>
			/// <exception cref="Apache.Http.Impl.Auth.NTLMEngineException"></exception>
			public virtual byte[] GetClientChallenge2()
			{
				if (clientChallenge2 == null)
				{
					clientChallenge2 = MakeRandomChallenge();
				}
				return clientChallenge2;
			}

			/// <summary>Calculate and return random secondary key</summary>
			/// <exception cref="Apache.Http.Impl.Auth.NTLMEngineException"></exception>
			public virtual byte[] GetSecondaryKey()
			{
				if (secondaryKey == null)
				{
					secondaryKey = MakeSecondaryKey();
				}
				return secondaryKey;
			}

			/// <summary>Calculate and return the LMHash</summary>
			/// <exception cref="Apache.Http.Impl.Auth.NTLMEngineException"></exception>
			public virtual byte[] GetLMHash()
			{
				if (lmHash == null)
				{
					lmHash = LmHash(password);
				}
				return lmHash;
			}

			/// <summary>Calculate and return the LMResponse</summary>
			/// <exception cref="Apache.Http.Impl.Auth.NTLMEngineException"></exception>
			public virtual byte[] GetLMResponse()
			{
				if (lmResponse == null)
				{
					lmResponse = LmResponse(GetLMHash(), challenge);
				}
				return lmResponse;
			}

			/// <summary>Calculate and return the NTLMHash</summary>
			/// <exception cref="Apache.Http.Impl.Auth.NTLMEngineException"></exception>
			public virtual byte[] GetNTLMHash()
			{
				if (ntlmHash == null)
				{
					ntlmHash = NtlmHash(password);
				}
				return ntlmHash;
			}

			/// <summary>Calculate and return the NTLMResponse</summary>
			/// <exception cref="Apache.Http.Impl.Auth.NTLMEngineException"></exception>
			public virtual byte[] GetNTLMResponse()
			{
				if (ntlmResponse == null)
				{
					ntlmResponse = LmResponse(GetNTLMHash(), challenge);
				}
				return ntlmResponse;
			}

			/// <summary>Calculate the LMv2 hash</summary>
			/// <exception cref="Apache.Http.Impl.Auth.NTLMEngineException"></exception>
			public virtual byte[] GetLMv2Hash()
			{
				if (lmv2Hash == null)
				{
					lmv2Hash = Lmv2Hash(domain, user, GetNTLMHash());
				}
				return lmv2Hash;
			}

			/// <summary>Calculate the NTLMv2 hash</summary>
			/// <exception cref="Apache.Http.Impl.Auth.NTLMEngineException"></exception>
			public virtual byte[] GetNTLMv2Hash()
			{
				if (ntlmv2Hash == null)
				{
					ntlmv2Hash = Ntlmv2Hash(domain, user, GetNTLMHash());
				}
				return ntlmv2Hash;
			}

			/// <summary>Calculate a timestamp</summary>
			public virtual byte[] GetTimestamp()
			{
				if (timestamp == null)
				{
					long time = Runtime.CurrentTimeMillis();
					time += 11644473600000l;
					// milliseconds from January 1, 1601 -> epoch.
					time *= 10000;
					// tenths of a microsecond.
					// convert to little-endian byte array.
					timestamp = new byte[8];
					for (int i = 0; i < 8; i++)
					{
						timestamp[i] = unchecked((byte)time);
						time = (long)(((ulong)time) >> 8);
					}
				}
				return timestamp;
			}

			/// <summary>Calculate the NTLMv2Blob</summary>
			/// <exception cref="Apache.Http.Impl.Auth.NTLMEngineException"></exception>
			public virtual byte[] GetNTLMv2Blob()
			{
				if (ntlmv2Blob == null)
				{
					ntlmv2Blob = CreateBlob(GetClientChallenge2(), targetInformation, GetTimestamp());
				}
				return ntlmv2Blob;
			}

			/// <summary>Calculate the NTLMv2Response</summary>
			/// <exception cref="Apache.Http.Impl.Auth.NTLMEngineException"></exception>
			public virtual byte[] GetNTLMv2Response()
			{
				if (ntlmv2Response == null)
				{
					ntlmv2Response = Lmv2Response(GetNTLMv2Hash(), challenge, GetNTLMv2Blob());
				}
				return ntlmv2Response;
			}

			/// <summary>Calculate the LMv2Response</summary>
			/// <exception cref="Apache.Http.Impl.Auth.NTLMEngineException"></exception>
			public virtual byte[] GetLMv2Response()
			{
				if (lmv2Response == null)
				{
					lmv2Response = Lmv2Response(GetLMv2Hash(), challenge, GetClientChallenge());
				}
				return lmv2Response;
			}

			/// <summary>Get NTLM2SessionResponse</summary>
			/// <exception cref="Apache.Http.Impl.Auth.NTLMEngineException"></exception>
			public virtual byte[] GetNTLM2SessionResponse()
			{
				if (ntlm2SessionResponse == null)
				{
					ntlm2SessionResponse = Ntlm2SessionResponse(GetNTLMHash(), challenge, GetClientChallenge
						());
				}
				return ntlm2SessionResponse;
			}

			/// <summary>Calculate and return LM2 session response</summary>
			/// <exception cref="Apache.Http.Impl.Auth.NTLMEngineException"></exception>
			public virtual byte[] GetLM2SessionResponse()
			{
				if (lm2SessionResponse == null)
				{
					byte[] clientChallenge = GetClientChallenge();
					lm2SessionResponse = new byte[24];
					System.Array.Copy(clientChallenge, 0, lm2SessionResponse, 0, clientChallenge.Length
						);
					Arrays.Fill(lm2SessionResponse, clientChallenge.Length, lm2SessionResponse.Length
						, unchecked((byte)unchecked((int)(0x00))));
				}
				return lm2SessionResponse;
			}

			/// <summary>Get LMUserSessionKey</summary>
			/// <exception cref="Apache.Http.Impl.Auth.NTLMEngineException"></exception>
			public virtual byte[] GetLMUserSessionKey()
			{
				if (lmUserSessionKey == null)
				{
					byte[] lmHash = GetLMHash();
					lmUserSessionKey = new byte[16];
					System.Array.Copy(lmHash, 0, lmUserSessionKey, 0, 8);
					Arrays.Fill(lmUserSessionKey, 8, 16, unchecked((byte)unchecked((int)(0x00))));
				}
				return lmUserSessionKey;
			}

			/// <summary>Get NTLMUserSessionKey</summary>
			/// <exception cref="Apache.Http.Impl.Auth.NTLMEngineException"></exception>
			public virtual byte[] GetNTLMUserSessionKey()
			{
				if (ntlmUserSessionKey == null)
				{
					byte[] ntlmHash = GetNTLMHash();
					NTLMEngineImpl.MD4 md4 = new NTLMEngineImpl.MD4();
					md4.Update(ntlmHash);
					ntlmUserSessionKey = md4.GetOutput();
				}
				return ntlmUserSessionKey;
			}

			/// <summary>GetNTLMv2UserSessionKey</summary>
			/// <exception cref="Apache.Http.Impl.Auth.NTLMEngineException"></exception>
			public virtual byte[] GetNTLMv2UserSessionKey()
			{
				if (ntlmv2UserSessionKey == null)
				{
					byte[] ntlmv2hash = GetNTLMv2Hash();
					byte[] truncatedResponse = new byte[16];
					System.Array.Copy(GetNTLMv2Response(), 0, truncatedResponse, 0, 16);
					ntlmv2UserSessionKey = HmacMD5(truncatedResponse, ntlmv2hash);
				}
				return ntlmv2UserSessionKey;
			}

			/// <summary>Get NTLM2SessionResponseUserSessionKey</summary>
			/// <exception cref="Apache.Http.Impl.Auth.NTLMEngineException"></exception>
			public virtual byte[] GetNTLM2SessionResponseUserSessionKey()
			{
				if (ntlm2SessionResponseUserSessionKey == null)
				{
					byte[] ntlmUserSessionKey = GetNTLMUserSessionKey();
					byte[] ntlm2SessionResponseNonce = GetLM2SessionResponse();
					byte[] sessionNonce = new byte[challenge.Length + ntlm2SessionResponseNonce.Length
						];
					System.Array.Copy(challenge, 0, sessionNonce, 0, challenge.Length);
					System.Array.Copy(ntlm2SessionResponseNonce, 0, sessionNonce, challenge.Length, ntlm2SessionResponseNonce
						.Length);
					ntlm2SessionResponseUserSessionKey = HmacMD5(sessionNonce, ntlmUserSessionKey);
				}
				return ntlm2SessionResponseUserSessionKey;
			}

			/// <summary>Get LAN Manager session key</summary>
			/// <exception cref="Apache.Http.Impl.Auth.NTLMEngineException"></exception>
			public virtual byte[] GetLanManagerSessionKey()
			{
				if (lanManagerSessionKey == null)
				{
					byte[] lmHash = GetLMHash();
					byte[] lmResponse = GetLMResponse();
					try
					{
						byte[] keyBytes = new byte[14];
						System.Array.Copy(lmHash, 0, keyBytes, 0, 8);
						Arrays.Fill(keyBytes, 8, keyBytes.Length, unchecked((byte)unchecked((int)(0xbd)))
							);
						Key lowKey = CreateDESKey(keyBytes, 0);
						Key highKey = CreateDESKey(keyBytes, 7);
						byte[] truncatedResponse = new byte[8];
						System.Array.Copy(lmResponse, 0, truncatedResponse, 0, truncatedResponse.Length);
						Sharpen.Cipher des = Sharpen.Cipher.GetInstance("DES/ECB/NoPadding");
						des.Init(Sharpen.Cipher.EncryptMode, lowKey);
						byte[] lowPart = des.DoFinal(truncatedResponse);
						des = Sharpen.Cipher.GetInstance("DES/ECB/NoPadding");
						des.Init(Sharpen.Cipher.EncryptMode, highKey);
						byte[] highPart = des.DoFinal(truncatedResponse);
						lanManagerSessionKey = new byte[16];
						System.Array.Copy(lowPart, 0, lanManagerSessionKey, 0, lowPart.Length);
						System.Array.Copy(highPart, 0, lanManagerSessionKey, lowPart.Length, highPart.Length
							);
					}
					catch (Exception e)
					{
						throw new NTLMEngineException(e.Message, e);
					}
				}
				return lanManagerSessionKey;
			}
		}

		/// <summary>Calculates HMAC-MD5</summary>
		/// <exception cref="Apache.Http.Impl.Auth.NTLMEngineException"></exception>
		internal static byte[] HmacMD5(byte[] value, byte[] key)
		{
			NTLMEngineImpl.HMACMD5 hmacMD5 = new NTLMEngineImpl.HMACMD5(key);
			hmacMD5.Update(value);
			return hmacMD5.GetOutput();
		}

		/// <summary>Calculates RC4</summary>
		/// <exception cref="Apache.Http.Impl.Auth.NTLMEngineException"></exception>
		internal static byte[] Rc4(byte[] value, byte[] key)
		{
			try
			{
				Sharpen.Cipher rc4 = Sharpen.Cipher.GetInstance("RC4");
				rc4.Init(Sharpen.Cipher.EncryptMode, new SecretKeySpec(key, "RC4"));
				return rc4.DoFinal(value);
			}
			catch (Exception e)
			{
				throw new NTLMEngineException(e.Message, e);
			}
		}

		/// <summary>
		/// Calculates the NTLM2 Session Response for the given challenge, using the
		/// specified password and client challenge.
		/// </summary>
		/// <remarks>
		/// Calculates the NTLM2 Session Response for the given challenge, using the
		/// specified password and client challenge.
		/// </remarks>
		/// <returns>
		/// The NTLM2 Session Response. This is placed in the NTLM response
		/// field of the Type 3 message; the LM response field contains the
		/// client challenge, null-padded to 24 bytes.
		/// </returns>
		/// <exception cref="Apache.Http.Impl.Auth.NTLMEngineException"></exception>
		internal static byte[] Ntlm2SessionResponse(byte[] ntlmHash, byte[] challenge, byte
			[] clientChallenge)
		{
			try
			{
				// Look up MD5 algorithm (was necessary on jdk 1.4.2)
				// This used to be needed, but java 1.5.0_07 includes the MD5
				// algorithm (finally)
				// Class x = Class.forName("gnu.crypto.hash.MD5");
				// Method updateMethod = x.getMethod("update",new
				// Class[]{byte[].class});
				// Method digestMethod = x.getMethod("digest",new Class[0]);
				// Object mdInstance = x.newInstance();
				// updateMethod.invoke(mdInstance,new Object[]{challenge});
				// updateMethod.invoke(mdInstance,new Object[]{clientChallenge});
				// byte[] digest = (byte[])digestMethod.invoke(mdInstance,new
				// Object[0]);
				MessageDigest md5 = MessageDigest.GetInstance("MD5");
				md5.Update(challenge);
				md5.Update(clientChallenge);
				byte[] digest = md5.Digest();
				byte[] sessionHash = new byte[8];
				System.Array.Copy(digest, 0, sessionHash, 0, 8);
				return LmResponse(ntlmHash, sessionHash);
			}
			catch (Exception e)
			{
				if (e is NTLMEngineException)
				{
					throw (NTLMEngineException)e;
				}
				throw new NTLMEngineException(e.Message, e);
			}
		}

		/// <summary>Creates the LM Hash of the user's password.</summary>
		/// <remarks>Creates the LM Hash of the user's password.</remarks>
		/// <param name="password">The password.</param>
		/// <returns>
		/// The LM Hash of the given password, used in the calculation of the
		/// LM Response.
		/// </returns>
		/// <exception cref="Apache.Http.Impl.Auth.NTLMEngineException"></exception>
		private static byte[] LmHash(string password)
		{
			try
			{
				byte[] oemPassword = Sharpen.Runtime.GetBytesForString(password.ToUpper(CultureInfo
					.InvariantCulture), "US-ASCII");
				int length = Math.Min(oemPassword.Length, 14);
				byte[] keyBytes = new byte[14];
				System.Array.Copy(oemPassword, 0, keyBytes, 0, length);
				Key lowKey = CreateDESKey(keyBytes, 0);
				Key highKey = CreateDESKey(keyBytes, 7);
				byte[] magicConstant = Sharpen.Runtime.GetBytesForString("KGS!@#$%", "US-ASCII");
				Sharpen.Cipher des = Sharpen.Cipher.GetInstance("DES/ECB/NoPadding");
				des.Init(Sharpen.Cipher.EncryptMode, lowKey);
				byte[] lowHash = des.DoFinal(magicConstant);
				des.Init(Sharpen.Cipher.EncryptMode, highKey);
				byte[] highHash = des.DoFinal(magicConstant);
				byte[] lmHash = new byte[16];
				System.Array.Copy(lowHash, 0, lmHash, 0, 8);
				System.Array.Copy(highHash, 0, lmHash, 8, 8);
				return lmHash;
			}
			catch (Exception e)
			{
				throw new NTLMEngineException(e.Message, e);
			}
		}

		/// <summary>Creates the NTLM Hash of the user's password.</summary>
		/// <remarks>Creates the NTLM Hash of the user's password.</remarks>
		/// <param name="password">The password.</param>
		/// <returns>
		/// The NTLM Hash of the given password, used in the calculation of
		/// the NTLM Response and the NTLMv2 and LMv2 Hashes.
		/// </returns>
		/// <exception cref="Apache.Http.Impl.Auth.NTLMEngineException"></exception>
		private static byte[] NtlmHash(string password)
		{
			try
			{
				byte[] unicodePassword = Sharpen.Runtime.GetBytesForString(password, "UnicodeLittleUnmarked"
					);
				NTLMEngineImpl.MD4 md4 = new NTLMEngineImpl.MD4();
				md4.Update(unicodePassword);
				return md4.GetOutput();
			}
			catch (UnsupportedEncodingException e)
			{
				throw new NTLMEngineException("Unicode not supported: " + e.Message, e);
			}
		}

		/// <summary>Creates the LMv2 Hash of the user's password.</summary>
		/// <remarks>Creates the LMv2 Hash of the user's password.</remarks>
		/// <returns>
		/// The LMv2 Hash, used in the calculation of the NTLMv2 and LMv2
		/// Responses.
		/// </returns>
		/// <exception cref="Apache.Http.Impl.Auth.NTLMEngineException"></exception>
		private static byte[] Lmv2Hash(string domain, string user, byte[] ntlmHash)
		{
			try
			{
				NTLMEngineImpl.HMACMD5 hmacMD5 = new NTLMEngineImpl.HMACMD5(ntlmHash);
				// Upper case username, upper case domain!
				hmacMD5.Update(Sharpen.Runtime.GetBytesForString(user.ToUpper(CultureInfo.InvariantCulture
					), "UnicodeLittleUnmarked"));
				if (domain != null)
				{
					hmacMD5.Update(Sharpen.Runtime.GetBytesForString(domain.ToUpper(CultureInfo.InvariantCulture
						), "UnicodeLittleUnmarked"));
				}
				return hmacMD5.GetOutput();
			}
			catch (UnsupportedEncodingException e)
			{
				throw new NTLMEngineException("Unicode not supported! " + e.Message, e);
			}
		}

		/// <summary>Creates the NTLMv2 Hash of the user's password.</summary>
		/// <remarks>Creates the NTLMv2 Hash of the user's password.</remarks>
		/// <returns>
		/// The NTLMv2 Hash, used in the calculation of the NTLMv2 and LMv2
		/// Responses.
		/// </returns>
		/// <exception cref="Apache.Http.Impl.Auth.NTLMEngineException"></exception>
		private static byte[] Ntlmv2Hash(string domain, string user, byte[] ntlmHash)
		{
			try
			{
				NTLMEngineImpl.HMACMD5 hmacMD5 = new NTLMEngineImpl.HMACMD5(ntlmHash);
				// Upper case username, mixed case target!!
				hmacMD5.Update(Sharpen.Runtime.GetBytesForString(user.ToUpper(CultureInfo.InvariantCulture
					), "UnicodeLittleUnmarked"));
				if (domain != null)
				{
					hmacMD5.Update(Sharpen.Runtime.GetBytesForString(domain, "UnicodeLittleUnmarked")
						);
				}
				return hmacMD5.GetOutput();
			}
			catch (UnsupportedEncodingException e)
			{
				throw new NTLMEngineException("Unicode not supported! " + e.Message, e);
			}
		}

		/// <summary>Creates the LM Response from the given hash and Type 2 challenge.</summary>
		/// <remarks>Creates the LM Response from the given hash and Type 2 challenge.</remarks>
		/// <param name="hash">The LM or NTLM Hash.</param>
		/// <param name="challenge">The server challenge from the Type 2 message.</param>
		/// <returns>The response (either LM or NTLM, depending on the provided hash).</returns>
		/// <exception cref="Apache.Http.Impl.Auth.NTLMEngineException"></exception>
		private static byte[] LmResponse(byte[] hash, byte[] challenge)
		{
			try
			{
				byte[] keyBytes = new byte[21];
				System.Array.Copy(hash, 0, keyBytes, 0, 16);
				Key lowKey = CreateDESKey(keyBytes, 0);
				Key middleKey = CreateDESKey(keyBytes, 7);
				Key highKey = CreateDESKey(keyBytes, 14);
				Sharpen.Cipher des = Sharpen.Cipher.GetInstance("DES/ECB/NoPadding");
				des.Init(Sharpen.Cipher.EncryptMode, lowKey);
				byte[] lowResponse = des.DoFinal(challenge);
				des.Init(Sharpen.Cipher.EncryptMode, middleKey);
				byte[] middleResponse = des.DoFinal(challenge);
				des.Init(Sharpen.Cipher.EncryptMode, highKey);
				byte[] highResponse = des.DoFinal(challenge);
				byte[] lmResponse = new byte[24];
				System.Array.Copy(lowResponse, 0, lmResponse, 0, 8);
				System.Array.Copy(middleResponse, 0, lmResponse, 8, 8);
				System.Array.Copy(highResponse, 0, lmResponse, 16, 8);
				return lmResponse;
			}
			catch (Exception e)
			{
				throw new NTLMEngineException(e.Message, e);
			}
		}

		/// <summary>
		/// Creates the LMv2 Response from the given hash, client data, and Type 2
		/// challenge.
		/// </summary>
		/// <remarks>
		/// Creates the LMv2 Response from the given hash, client data, and Type 2
		/// challenge.
		/// </remarks>
		/// <param name="hash">The NTLMv2 Hash.</param>
		/// <param name="clientData">The client data (blob or client challenge).</param>
		/// <param name="challenge">The server challenge from the Type 2 message.</param>
		/// <returns>
		/// The response (either NTLMv2 or LMv2, depending on the client
		/// data).
		/// </returns>
		/// <exception cref="Apache.Http.Impl.Auth.NTLMEngineException"></exception>
		private static byte[] Lmv2Response(byte[] hash, byte[] challenge, byte[] clientData
			)
		{
			NTLMEngineImpl.HMACMD5 hmacMD5 = new NTLMEngineImpl.HMACMD5(hash);
			hmacMD5.Update(challenge);
			hmacMD5.Update(clientData);
			byte[] mac = hmacMD5.GetOutput();
			byte[] lmv2Response = new byte[mac.Length + clientData.Length];
			System.Array.Copy(mac, 0, lmv2Response, 0, mac.Length);
			System.Array.Copy(clientData, 0, lmv2Response, mac.Length, clientData.Length);
			return lmv2Response;
		}

		/// <summary>
		/// Creates the NTLMv2 blob from the given target information block and
		/// client challenge.
		/// </summary>
		/// <remarks>
		/// Creates the NTLMv2 blob from the given target information block and
		/// client challenge.
		/// </remarks>
		/// <param name="targetInformation">The target information block from the Type 2 message.
		/// 	</param>
		/// <param name="clientChallenge">The random 8-byte client challenge.</param>
		/// <returns>The blob, used in the calculation of the NTLMv2 Response.</returns>
		private static byte[] CreateBlob(byte[] clientChallenge, byte[] targetInformation
			, byte[] timestamp)
		{
			byte[] blobSignature = new byte[] { unchecked((byte)unchecked((int)(0x01))), unchecked(
				(byte)unchecked((int)(0x01))), unchecked((byte)unchecked((int)(0x00))), unchecked(
				(byte)unchecked((int)(0x00))) };
			byte[] reserved = new byte[] { unchecked((byte)unchecked((int)(0x00))), unchecked(
				(byte)unchecked((int)(0x00))), unchecked((byte)unchecked((int)(0x00))), unchecked(
				(byte)unchecked((int)(0x00))) };
			byte[] unknown1 = new byte[] { unchecked((byte)unchecked((int)(0x00))), unchecked(
				(byte)unchecked((int)(0x00))), unchecked((byte)unchecked((int)(0x00))), unchecked(
				(byte)unchecked((int)(0x00))) };
			byte[] unknown2 = new byte[] { unchecked((byte)unchecked((int)(0x00))), unchecked(
				(byte)unchecked((int)(0x00))), unchecked((byte)unchecked((int)(0x00))), unchecked(
				(byte)unchecked((int)(0x00))) };
			byte[] blob = new byte[blobSignature.Length + reserved.Length + timestamp.Length 
				+ 8 + unknown1.Length + targetInformation.Length + unknown2.Length];
			int offset = 0;
			System.Array.Copy(blobSignature, 0, blob, offset, blobSignature.Length);
			offset += blobSignature.Length;
			System.Array.Copy(reserved, 0, blob, offset, reserved.Length);
			offset += reserved.Length;
			System.Array.Copy(timestamp, 0, blob, offset, timestamp.Length);
			offset += timestamp.Length;
			System.Array.Copy(clientChallenge, 0, blob, offset, 8);
			offset += 8;
			System.Array.Copy(unknown1, 0, blob, offset, unknown1.Length);
			offset += unknown1.Length;
			System.Array.Copy(targetInformation, 0, blob, offset, targetInformation.Length);
			offset += targetInformation.Length;
			System.Array.Copy(unknown2, 0, blob, offset, unknown2.Length);
			offset += unknown2.Length;
			return blob;
		}

		/// <summary>Creates a DES encryption key from the given key material.</summary>
		/// <remarks>Creates a DES encryption key from the given key material.</remarks>
		/// <param name="bytes">A byte array containing the DES key material.</param>
		/// <param name="offset">
		/// The offset in the given byte array at which the 7-byte key
		/// material starts.
		/// </param>
		/// <returns>
		/// A DES encryption key created from the key material starting at
		/// the specified offset in the given byte array.
		/// </returns>
		private static Key CreateDESKey(byte[] bytes, int offset)
		{
			byte[] keyBytes = new byte[7];
			System.Array.Copy(bytes, offset, keyBytes, 0, 7);
			byte[] material = new byte[8];
			material[0] = keyBytes[0];
			material[1] = unchecked((byte)(keyBytes[0] << 7 | (int)(((uint)(keyBytes[1] & unchecked(
				(int)(0xff)))) >> 1)));
			material[2] = unchecked((byte)(keyBytes[1] << 6 | (int)(((uint)(keyBytes[2] & unchecked(
				(int)(0xff)))) >> 2)));
			material[3] = unchecked((byte)(keyBytes[2] << 5 | (int)(((uint)(keyBytes[3] & unchecked(
				(int)(0xff)))) >> 3)));
			material[4] = unchecked((byte)(keyBytes[3] << 4 | (int)(((uint)(keyBytes[4] & unchecked(
				(int)(0xff)))) >> 4)));
			material[5] = unchecked((byte)(keyBytes[4] << 3 | (int)(((uint)(keyBytes[5] & unchecked(
				(int)(0xff)))) >> 5)));
			material[6] = unchecked((byte)(keyBytes[5] << 2 | (int)(((uint)(keyBytes[6] & unchecked(
				(int)(0xff)))) >> 6)));
			material[7] = unchecked((byte)(keyBytes[6] << 1));
			OddParity(material);
			return new SecretKeySpec(material, "DES");
		}

		/// <summary>Applies odd parity to the given byte array.</summary>
		/// <remarks>Applies odd parity to the given byte array.</remarks>
		/// <param name="bytes">The data whose parity bits are to be adjusted for odd parity.
		/// 	</param>
		private static void OddParity(byte[] bytes)
		{
			for (int i = 0; i < bytes.Length; i++)
			{
				byte b = bytes[i];
				bool needsParity = (((b >> 7) ^ (b >> 6) ^ (b >> 5) ^ (b >> 4) ^ (b >> 3) ^ (b >>
					 2) ^ (b >> 1)) & unchecked((int)(0x01))) == 0;
				if (needsParity)
				{
					bytes[i] |= unchecked((byte)unchecked((int)(0x01)));
				}
				else
				{
					bytes[i] &= unchecked((byte)unchecked((int)(0xfe)));
				}
			}
		}

		/// <summary>NTLM message generation, base class</summary>
		internal class NTLMMessage
		{
			/// <summary>The current response</summary>
			private byte[] messageContents = null;

			/// <summary>The current output position</summary>
			private int currentOutputPosition = 0;

			/// <summary>Constructor to use when message contents are not yet known</summary>
			internal NTLMMessage()
			{
			}

			/// <summary>Constructor to use when message contents are known</summary>
			/// <exception cref="Apache.Http.Impl.Auth.NTLMEngineException"></exception>
			internal NTLMMessage(string messageBody, int expectedType)
			{
				messageContents = Base64.DecodeBase64(EncodingUtils.GetBytes(messageBody, DefaultCharset
					));
				// Look for NTLM message
				if (messageContents.Length < Signature.Length)
				{
					throw new NTLMEngineException("NTLM message decoding error - packet too short");
				}
				int i = 0;
				while (i < Signature.Length)
				{
					if (messageContents[i] != Signature[i])
					{
						throw new NTLMEngineException("NTLM message expected - instead got unrecognized bytes"
							);
					}
					i++;
				}
				// Check to be sure there's a type 2 message indicator next
				int type = ReadULong(Signature.Length);
				if (type != expectedType)
				{
					throw new NTLMEngineException("NTLM type " + Sharpen.Extensions.ToString(expectedType
						) + " message expected - instead got type " + Sharpen.Extensions.ToString(type));
				}
				currentOutputPosition = messageContents.Length;
			}

			/// <summary>
			/// Get the length of the signature and flags, so calculations can adjust
			/// offsets accordingly.
			/// </summary>
			/// <remarks>
			/// Get the length of the signature and flags, so calculations can adjust
			/// offsets accordingly.
			/// </remarks>
			protected internal virtual int GetPreambleLength()
			{
				return Signature.Length + 4;
			}

			/// <summary>Get the message length</summary>
			protected internal virtual int GetMessageLength()
			{
				return currentOutputPosition;
			}

			/// <summary>Read a byte from a position within the message buffer</summary>
			/// <exception cref="Apache.Http.Impl.Auth.NTLMEngineException"></exception>
			protected internal virtual byte ReadByte(int position)
			{
				if (messageContents.Length < position + 1)
				{
					throw new NTLMEngineException("NTLM: Message too short");
				}
				return messageContents[position];
			}

			/// <summary>Read a bunch of bytes from a position in the message buffer</summary>
			/// <exception cref="Apache.Http.Impl.Auth.NTLMEngineException"></exception>
			protected internal virtual void ReadBytes(byte[] buffer, int position)
			{
				if (messageContents.Length < position + buffer.Length)
				{
					throw new NTLMEngineException("NTLM: Message too short");
				}
				System.Array.Copy(messageContents, position, buffer, 0, buffer.Length);
			}

			/// <summary>Read a ushort from a position within the message buffer</summary>
			/// <exception cref="Apache.Http.Impl.Auth.NTLMEngineException"></exception>
			protected internal virtual int ReadUShort(int position)
			{
				return NTLMEngineImpl.ReadUShort(messageContents, position);
			}

			/// <summary>Read a ulong from a position within the message buffer</summary>
			/// <exception cref="Apache.Http.Impl.Auth.NTLMEngineException"></exception>
			protected internal virtual int ReadULong(int position)
			{
				return NTLMEngineImpl.ReadULong(messageContents, position);
			}

			/// <summary>Read a security buffer from a position within the message buffer</summary>
			/// <exception cref="Apache.Http.Impl.Auth.NTLMEngineException"></exception>
			protected internal virtual byte[] ReadSecurityBuffer(int position)
			{
				return NTLMEngineImpl.ReadSecurityBuffer(messageContents, position);
			}

			/// <summary>Prepares the object to create a response of the given length.</summary>
			/// <remarks>Prepares the object to create a response of the given length.</remarks>
			/// <param name="maxlength">
			/// the maximum length of the response to prepare, not
			/// including the type and the signature (which this method
			/// adds).
			/// </param>
			protected internal virtual void PrepareResponse(int maxlength, int messageType)
			{
				messageContents = new byte[maxlength];
				currentOutputPosition = 0;
				AddBytes(Signature);
				AddULong(messageType);
			}

			/// <summary>Adds the given byte to the response.</summary>
			/// <remarks>Adds the given byte to the response.</remarks>
			/// <param name="b">the byte to add.</param>
			protected internal virtual void AddByte(byte b)
			{
				messageContents[currentOutputPosition] = b;
				currentOutputPosition++;
			}

			/// <summary>Adds the given bytes to the response.</summary>
			/// <remarks>Adds the given bytes to the response.</remarks>
			/// <param name="bytes">the bytes to add.</param>
			protected internal virtual void AddBytes(byte[] bytes)
			{
				if (bytes == null)
				{
					return;
				}
				foreach (byte b in bytes)
				{
					messageContents[currentOutputPosition] = b;
					currentOutputPosition++;
				}
			}

			/// <summary>Adds a USHORT to the response</summary>
			protected internal virtual void AddUShort(int value)
			{
				AddByte(unchecked((byte)(value & unchecked((int)(0xff)))));
				AddByte(unchecked((byte)(value >> 8 & unchecked((int)(0xff)))));
			}

			/// <summary>Adds a ULong to the response</summary>
			protected internal virtual void AddULong(int value)
			{
				AddByte(unchecked((byte)(value & unchecked((int)(0xff)))));
				AddByte(unchecked((byte)(value >> 8 & unchecked((int)(0xff)))));
				AddByte(unchecked((byte)(value >> 16 & unchecked((int)(0xff)))));
				AddByte(unchecked((byte)(value >> 24 & unchecked((int)(0xff)))));
			}

			/// <summary>
			/// Returns the response that has been generated after shrinking the
			/// array if required and base64 encodes the response.
			/// </summary>
			/// <remarks>
			/// Returns the response that has been generated after shrinking the
			/// array if required and base64 encodes the response.
			/// </remarks>
			/// <returns>The response as above.</returns>
			internal virtual string GetResponse()
			{
				byte[] resp;
				if (messageContents.Length > currentOutputPosition)
				{
					byte[] tmp = new byte[currentOutputPosition];
					System.Array.Copy(messageContents, 0, tmp, 0, currentOutputPosition);
					resp = tmp;
				}
				else
				{
					resp = messageContents;
				}
				return EncodingUtils.GetAsciiString(Base64.EncodeBase64(resp));
			}
		}

		/// <summary>Type 1 message assembly class</summary>
		internal class Type1Message : NTLMEngineImpl.NTLMMessage
		{
			protected internal byte[] hostBytes;

			protected internal byte[] domainBytes;

			/// <summary>Constructor.</summary>
			/// <remarks>Constructor. Include the arguments the message will need</remarks>
			/// <exception cref="Apache.Http.Impl.Auth.NTLMEngineException"></exception>
			internal Type1Message(string domain, string host) : base()
			{
				try
				{
					// Strip off domain name from the host!
					string unqualifiedHost = ConvertHost(host);
					// Use only the base domain name!
					string unqualifiedDomain = ConvertDomain(domain);
					hostBytes = unqualifiedHost != null ? Sharpen.Runtime.GetBytesForString(unqualifiedHost
						, "ASCII") : null;
					domainBytes = unqualifiedDomain != null ? Sharpen.Runtime.GetBytesForString(unqualifiedDomain
						.ToUpper(CultureInfo.InvariantCulture), "ASCII") : null;
				}
				catch (UnsupportedEncodingException e)
				{
					throw new NTLMEngineException("Unicode unsupported: " + e.Message, e);
				}
			}

			/// <summary>
			/// Getting the response involves building the message before returning
			/// it
			/// </summary>
			internal override string GetResponse()
			{
				// Now, build the message. Calculate its length first, including
				// signature or type.
				int finalLength = 32 + 8;
				// Set up the response. This will initialize the signature, message
				// type, and flags.
				PrepareResponse(finalLength, 1);
				// Flags. These are the complete set of flags we support.
				AddULong(FLAG_REQUEST_NTLMv1 | FlagRequestNtlm2Session | FlagRequestVersion | FlagRequestAlwaysSign
					 | FlagRequest128bitKeyExch | FlagRequest56bitEncryption | FlagRequestUnicodeEncoding
					);
				//FLAG_WORKSTATION_PRESENT |
				//FLAG_DOMAIN_PRESENT |
				// Required flags
				//FLAG_REQUEST_LAN_MANAGER_KEY |
				// Protocol version request
				// Recommended privacy settings
				//FLAG_REQUEST_SEAL |
				//FLAG_REQUEST_SIGN |
				// These must be set according to documentation, based on use of SEAL above
				//FLAG_REQUEST_EXPLICIT_KEY_EXCH |
				// Domain length (two times).
				AddUShort(0);
				AddUShort(0);
				// Domain offset.
				AddULong(32 + 8);
				// Host length (two times).
				AddUShort(0);
				AddUShort(0);
				// Host offset (always 32 + 8).
				AddULong(32 + 8);
				// Version
				AddUShort(unchecked((int)(0x0105)));
				// Build
				AddULong(2600);
				// NTLM revision
				AddUShort(unchecked((int)(0x0f00)));
				// Host (workstation) String.
				//addBytes(hostBytes);
				// Domain String.
				//addBytes(domainBytes);
				return base.GetResponse();
			}
		}

		/// <summary>Type 2 message class</summary>
		internal class Type2Message : NTLMEngineImpl.NTLMMessage
		{
			protected internal byte[] challenge;

			protected internal string target;

			protected internal byte[] targetInfo;

			protected internal int flags;

			/// <exception cref="Apache.Http.Impl.Auth.NTLMEngineException"></exception>
			internal Type2Message(string message) : base(message, 2)
			{
				// Type 2 message is laid out as follows:
				// First 8 bytes: NTLMSSP[0]
				// Next 4 bytes: Ulong, value 2
				// Next 8 bytes, starting at offset 12: target field (2 ushort lengths, 1 ulong offset)
				// Next 4 bytes, starting at offset 20: Flags, e.g. 0x22890235
				// Next 8 bytes, starting at offset 24: Challenge
				// Next 8 bytes, starting at offset 32: ??? (8 bytes of zeros)
				// Next 8 bytes, starting at offset 40: targetinfo field (2 ushort lengths, 1 ulong offset)
				// Next 2 bytes, major/minor version number (e.g. 0x05 0x02)
				// Next 8 bytes, build number
				// Next 2 bytes, protocol version number (e.g. 0x00 0x0f)
				// Next, various text fields, and a ushort of value 0 at the end
				// Parse out the rest of the info we need from the message
				// The nonce is the 8 bytes starting from the byte in position 24.
				challenge = new byte[8];
				ReadBytes(challenge, 24);
				flags = ReadULong(20);
				if ((flags & FlagRequestUnicodeEncoding) == 0)
				{
					throw new NTLMEngineException("NTLM type 2 message has flags that make no sense: "
						 + Sharpen.Extensions.ToString(flags));
				}
				// Do the target!
				target = null;
				// The TARGET_DESIRED flag is said to not have understood semantics
				// in Type2 messages, so use the length of the packet to decide
				// how to proceed instead
				if (GetMessageLength() >= 12 + 8)
				{
					byte[] bytes = ReadSecurityBuffer(12);
					if (bytes.Length != 0)
					{
						try
						{
							target = Sharpen.Runtime.GetStringForBytes(bytes, "UnicodeLittleUnmarked");
						}
						catch (UnsupportedEncodingException e)
						{
							throw new NTLMEngineException(e.Message, e);
						}
					}
				}
				// Do the target info!
				targetInfo = null;
				// TARGET_DESIRED flag cannot be relied on, so use packet length
				if (GetMessageLength() >= 40 + 8)
				{
					byte[] bytes = ReadSecurityBuffer(40);
					if (bytes.Length != 0)
					{
						targetInfo = bytes;
					}
				}
			}

			/// <summary>Retrieve the challenge</summary>
			internal virtual byte[] GetChallenge()
			{
				return challenge;
			}

			/// <summary>Retrieve the target</summary>
			internal virtual string GetTarget()
			{
				return target;
			}

			/// <summary>Retrieve the target info</summary>
			internal virtual byte[] GetTargetInfo()
			{
				return targetInfo;
			}

			/// <summary>Retrieve the response flags</summary>
			internal virtual int GetFlags()
			{
				return flags;
			}
		}

		/// <summary>Type 3 message assembly class</summary>
		internal class Type3Message : NTLMEngineImpl.NTLMMessage
		{
			protected internal int type2Flags;

			protected internal byte[] domainBytes;

			protected internal byte[] hostBytes;

			protected internal byte[] userBytes;

			protected internal byte[] lmResp;

			protected internal byte[] ntResp;

			protected internal byte[] sessionKey;

			/// <summary>Constructor.</summary>
			/// <remarks>Constructor. Pass the arguments we will need</remarks>
			/// <exception cref="Apache.Http.Impl.Auth.NTLMEngineException"></exception>
			internal Type3Message(string domain, string host, string user, string password, byte
				[] nonce, int type2Flags, string target, byte[] targetInformation)
			{
				// Response flags from the type2 message
				// Save the flags
				this.type2Flags = type2Flags;
				// Strip off domain name from the host!
				string unqualifiedHost = ConvertHost(host);
				// Use only the base domain name!
				string unqualifiedDomain = ConvertDomain(domain);
				// Create a cipher generator class.  Use domain BEFORE it gets modified!
				NTLMEngineImpl.CipherGen gen = new NTLMEngineImpl.CipherGen(unqualifiedDomain, user
					, password, nonce, target, targetInformation);
				// Use the new code to calculate the responses, including v2 if that
				// seems warranted.
				byte[] userSessionKey;
				try
				{
					// This conditional may not work on Windows Server 2008 R2 and above, where it has not yet
					// been tested
					if (((type2Flags & FlagTargetinfoPresent) != 0) && targetInformation != null && target
						 != null)
					{
						// NTLMv2
						ntResp = gen.GetNTLMv2Response();
						lmResp = gen.GetLMv2Response();
						if ((type2Flags & FlagRequestLanManagerKey) != 0)
						{
							userSessionKey = gen.GetLanManagerSessionKey();
						}
						else
						{
							userSessionKey = gen.GetNTLMv2UserSessionKey();
						}
					}
					else
					{
						// NTLMv1
						if ((type2Flags & FlagRequestNtlm2Session) != 0)
						{
							// NTLM2 session stuff is requested
							ntResp = gen.GetNTLM2SessionResponse();
							lmResp = gen.GetLM2SessionResponse();
							if ((type2Flags & FlagRequestLanManagerKey) != 0)
							{
								userSessionKey = gen.GetLanManagerSessionKey();
							}
							else
							{
								userSessionKey = gen.GetNTLM2SessionResponseUserSessionKey();
							}
						}
						else
						{
							ntResp = gen.GetNTLMResponse();
							lmResp = gen.GetLMResponse();
							if ((type2Flags & FlagRequestLanManagerKey) != 0)
							{
								userSessionKey = gen.GetLanManagerSessionKey();
							}
							else
							{
								userSessionKey = gen.GetNTLMUserSessionKey();
							}
						}
					}
				}
				catch (NTLMEngineException)
				{
					// This likely means we couldn't find the MD4 hash algorithm -
					// fail back to just using LM
					ntResp = new byte[0];
					lmResp = gen.GetLMResponse();
					if ((type2Flags & FlagRequestLanManagerKey) != 0)
					{
						userSessionKey = gen.GetLanManagerSessionKey();
					}
					else
					{
						userSessionKey = gen.GetLMUserSessionKey();
					}
				}
				if ((type2Flags & FlagRequestSign) != 0)
				{
					if ((type2Flags & FlagRequestExplicitKeyExch) != 0)
					{
						sessionKey = Rc4(gen.GetSecondaryKey(), userSessionKey);
					}
					else
					{
						sessionKey = userSessionKey;
					}
				}
				else
				{
					sessionKey = null;
				}
				try
				{
					hostBytes = unqualifiedHost != null ? Sharpen.Runtime.GetBytesForString(unqualifiedHost
						, "UnicodeLittleUnmarked") : null;
					domainBytes = unqualifiedDomain != null ? Sharpen.Runtime.GetBytesForString(unqualifiedDomain
						.ToUpper(CultureInfo.InvariantCulture), "UnicodeLittleUnmarked") : null;
					userBytes = Sharpen.Runtime.GetBytesForString(user, "UnicodeLittleUnmarked");
				}
				catch (UnsupportedEncodingException e)
				{
					throw new NTLMEngineException("Unicode not supported: " + e.Message, e);
				}
			}

			/// <summary>Assemble the response</summary>
			internal override string GetResponse()
			{
				int ntRespLen = ntResp.Length;
				int lmRespLen = lmResp.Length;
				int domainLen = domainBytes != null ? domainBytes.Length : 0;
				int hostLen = hostBytes != null ? hostBytes.Length : 0;
				int userLen = userBytes.Length;
				int sessionKeyLen;
				if (sessionKey != null)
				{
					sessionKeyLen = sessionKey.Length;
				}
				else
				{
					sessionKeyLen = 0;
				}
				// Calculate the layout within the packet
				int lmRespOffset = 72;
				// allocate space for the version
				int ntRespOffset = lmRespOffset + lmRespLen;
				int domainOffset = ntRespOffset + ntRespLen;
				int userOffset = domainOffset + domainLen;
				int hostOffset = userOffset + userLen;
				int sessionKeyOffset = hostOffset + hostLen;
				int finalLength = sessionKeyOffset + sessionKeyLen;
				// Start the response. Length includes signature and type
				PrepareResponse(finalLength, 3);
				// LM Resp Length (twice)
				AddUShort(lmRespLen);
				AddUShort(lmRespLen);
				// LM Resp Offset
				AddULong(lmRespOffset);
				// NT Resp Length (twice)
				AddUShort(ntRespLen);
				AddUShort(ntRespLen);
				// NT Resp Offset
				AddULong(ntRespOffset);
				// Domain length (twice)
				AddUShort(domainLen);
				AddUShort(domainLen);
				// Domain offset.
				AddULong(domainOffset);
				// User Length (twice)
				AddUShort(userLen);
				AddUShort(userLen);
				// User offset
				AddULong(userOffset);
				// Host length (twice)
				AddUShort(hostLen);
				AddUShort(hostLen);
				// Host offset
				AddULong(hostOffset);
				// Session key length (twice)
				AddUShort(sessionKeyLen);
				AddUShort(sessionKeyLen);
				// Session key offset
				AddULong(sessionKeyOffset);
				// Flags.
				AddULong((type2Flags & FlagRequestLanManagerKey) | (type2Flags & FLAG_REQUEST_NTLMv1
					) | (type2Flags & FlagRequestNtlm2Session) | FlagRequestVersion | (type2Flags & 
					FlagRequestAlwaysSign) | (type2Flags & FlagRequestSeal) | (type2Flags & FlagRequestSign
					) | (type2Flags & FlagRequest128bitKeyExch) | (type2Flags & FlagRequest56bitEncryption
					) | (type2Flags & FlagRequestExplicitKeyExch) | (type2Flags & FlagTargetinfoPresent
					) | (type2Flags & FlagRequestUnicodeEncoding) | (type2Flags & FlagRequestTarget)
					);
				//FLAG_WORKSTATION_PRESENT |
				//FLAG_DOMAIN_PRESENT |
				// Required flags
				// Protocol version request
				// Recommended privacy settings
				// These must be set according to documentation, based on use of SEAL above
				// Version
				AddUShort(unchecked((int)(0x0105)));
				// Build
				AddULong(2600);
				// NTLM revision
				AddUShort(unchecked((int)(0x0f00)));
				// Add the actual data
				AddBytes(lmResp);
				AddBytes(ntResp);
				AddBytes(domainBytes);
				AddBytes(userBytes);
				AddBytes(hostBytes);
				if (sessionKey != null)
				{
					AddBytes(sessionKey);
				}
				return base.GetResponse();
			}
		}

		internal static void WriteULong(byte[] buffer, int value, int offset)
		{
			buffer[offset] = unchecked((byte)(value & unchecked((int)(0xff))));
			buffer[offset + 1] = unchecked((byte)(value >> 8 & unchecked((int)(0xff))));
			buffer[offset + 2] = unchecked((byte)(value >> 16 & unchecked((int)(0xff))));
			buffer[offset + 3] = unchecked((byte)(value >> 24 & unchecked((int)(0xff))));
		}

		internal static int F(int x, int y, int z)
		{
			return ((x & y) | (~x & z));
		}

		internal static int G(int x, int y, int z)
		{
			return ((x & y) | (x & z) | (y & z));
		}

		internal static int H(int x, int y, int z)
		{
			return (x ^ y ^ z);
		}

		internal static int Rotintlft(int val, int numbits)
		{
			return ((val << numbits) | ((int)(((uint)val) >> (32 - numbits))));
		}

		/// <summary>Cryptography support - MD4.</summary>
		/// <remarks>
		/// Cryptography support - MD4. The following class was based loosely on the
		/// RFC and on code found at http://www.cs.umd.edu/~harry/jotp/src/md.java.
		/// Code correctness was verified by looking at MD4.java from the jcifs
		/// library (http://jcifs.samba.org). It was massaged extensively to the
		/// final form found here by Karl Wright (kwright@metacarta.com).
		/// </remarks>
		internal class MD4
		{
			protected internal int A = unchecked((int)(0x67452301));

			protected internal int B = unchecked((int)(0xefcdab89));

			protected internal int C = unchecked((int)(0x98badcfe));

			protected internal int D = unchecked((int)(0x10325476));

			protected internal long count = 0L;

			protected internal byte[] dataBuffer = new byte[64];

			internal MD4()
			{
			}

			internal virtual void Update(byte[] input)
			{
				// We always deal with 512 bits at a time. Correspondingly, there is
				// a buffer 64 bytes long that we write data into until it gets
				// full.
				int curBufferPos = (int)(count & 63L);
				int inputIndex = 0;
				while (input.Length - inputIndex + curBufferPos >= dataBuffer.Length)
				{
					// We have enough data to do the next step. Do a partial copy
					// and a transform, updating inputIndex and curBufferPos
					// accordingly
					int transferAmt = dataBuffer.Length - curBufferPos;
					System.Array.Copy(input, inputIndex, dataBuffer, curBufferPos, transferAmt);
					count += transferAmt;
					curBufferPos = 0;
					inputIndex += transferAmt;
					ProcessBuffer();
				}
				// If there's anything left, copy it into the buffer and leave it.
				// We know there's not enough left to process.
				if (inputIndex < input.Length)
				{
					int transferAmt = input.Length - inputIndex;
					System.Array.Copy(input, inputIndex, dataBuffer, curBufferPos, transferAmt);
					count += transferAmt;
					curBufferPos += transferAmt;
				}
			}

			internal virtual byte[] GetOutput()
			{
				// Feed pad/length data into engine. This must round out the input
				// to a multiple of 512 bits.
				int bufferIndex = (int)(count & 63L);
				int padLen = (bufferIndex < 56) ? (56 - bufferIndex) : (120 - bufferIndex);
				byte[] postBytes = new byte[padLen + 8];
				// Leading 0x80, specified amount of zero padding, then length in
				// bits.
				postBytes[0] = unchecked((byte)unchecked((int)(0x80)));
				// Fill out the last 8 bytes with the length
				for (int i = 0; i < 8; i++)
				{
					postBytes[padLen + i] = unchecked((byte)((long)(((ulong)(count * 8)) >> (8 * i)))
						);
				}
				// Update the engine
				Update(postBytes);
				// Calculate final result
				byte[] result = new byte[16];
				WriteULong(result, A, 0);
				WriteULong(result, B, 4);
				WriteULong(result, C, 8);
				WriteULong(result, D, 12);
				return result;
			}

			protected internal virtual void ProcessBuffer()
			{
				// Convert current buffer to 16 ulongs
				int[] d = new int[16];
				for (int i = 0; i < 16; i++)
				{
					d[i] = (dataBuffer[i * 4] & unchecked((int)(0xff))) + ((dataBuffer[i * 4 + 1] & unchecked(
						(int)(0xff))) << 8) + ((dataBuffer[i * 4 + 2] & unchecked((int)(0xff))) << 16) +
						 ((dataBuffer[i * 4 + 3] & unchecked((int)(0xff))) << 24);
				}
				// Do a round of processing
				int Aa = A;
				int Bb = B;
				int Cc = C;
				int Dd = D;
				Round1(d);
				Round2(d);
				Round3(d);
				A += Aa;
				B += Bb;
				C += Cc;
				D += Dd;
			}

			protected internal virtual void Round1(int[] d)
			{
				A = Rotintlft((A + F(B, C, D) + d[0]), 3);
				D = Rotintlft((D + F(A, B, C) + d[1]), 7);
				C = Rotintlft((C + F(D, A, B) + d[2]), 11);
				B = Rotintlft((B + F(C, D, A) + d[3]), 19);
				A = Rotintlft((A + F(B, C, D) + d[4]), 3);
				D = Rotintlft((D + F(A, B, C) + d[5]), 7);
				C = Rotintlft((C + F(D, A, B) + d[6]), 11);
				B = Rotintlft((B + F(C, D, A) + d[7]), 19);
				A = Rotintlft((A + F(B, C, D) + d[8]), 3);
				D = Rotintlft((D + F(A, B, C) + d[9]), 7);
				C = Rotintlft((C + F(D, A, B) + d[10]), 11);
				B = Rotintlft((B + F(C, D, A) + d[11]), 19);
				A = Rotintlft((A + F(B, C, D) + d[12]), 3);
				D = Rotintlft((D + F(A, B, C) + d[13]), 7);
				C = Rotintlft((C + F(D, A, B) + d[14]), 11);
				B = Rotintlft((B + F(C, D, A) + d[15]), 19);
			}

			protected internal virtual void Round2(int[] d)
			{
				A = Rotintlft((A + G(B, C, D) + d[0] + unchecked((int)(0x5a827999))), 3);
				D = Rotintlft((D + G(A, B, C) + d[4] + unchecked((int)(0x5a827999))), 5);
				C = Rotintlft((C + G(D, A, B) + d[8] + unchecked((int)(0x5a827999))), 9);
				B = Rotintlft((B + G(C, D, A) + d[12] + unchecked((int)(0x5a827999))), 13);
				A = Rotintlft((A + G(B, C, D) + d[1] + unchecked((int)(0x5a827999))), 3);
				D = Rotintlft((D + G(A, B, C) + d[5] + unchecked((int)(0x5a827999))), 5);
				C = Rotintlft((C + G(D, A, B) + d[9] + unchecked((int)(0x5a827999))), 9);
				B = Rotintlft((B + G(C, D, A) + d[13] + unchecked((int)(0x5a827999))), 13);
				A = Rotintlft((A + G(B, C, D) + d[2] + unchecked((int)(0x5a827999))), 3);
				D = Rotintlft((D + G(A, B, C) + d[6] + unchecked((int)(0x5a827999))), 5);
				C = Rotintlft((C + G(D, A, B) + d[10] + unchecked((int)(0x5a827999))), 9);
				B = Rotintlft((B + G(C, D, A) + d[14] + unchecked((int)(0x5a827999))), 13);
				A = Rotintlft((A + G(B, C, D) + d[3] + unchecked((int)(0x5a827999))), 3);
				D = Rotintlft((D + G(A, B, C) + d[7] + unchecked((int)(0x5a827999))), 5);
				C = Rotintlft((C + G(D, A, B) + d[11] + unchecked((int)(0x5a827999))), 9);
				B = Rotintlft((B + G(C, D, A) + d[15] + unchecked((int)(0x5a827999))), 13);
			}

			protected internal virtual void Round3(int[] d)
			{
				A = Rotintlft((A + H(B, C, D) + d[0] + unchecked((int)(0x6ed9eba1))), 3);
				D = Rotintlft((D + H(A, B, C) + d[8] + unchecked((int)(0x6ed9eba1))), 9);
				C = Rotintlft((C + H(D, A, B) + d[4] + unchecked((int)(0x6ed9eba1))), 11);
				B = Rotintlft((B + H(C, D, A) + d[12] + unchecked((int)(0x6ed9eba1))), 15);
				A = Rotintlft((A + H(B, C, D) + d[2] + unchecked((int)(0x6ed9eba1))), 3);
				D = Rotintlft((D + H(A, B, C) + d[10] + unchecked((int)(0x6ed9eba1))), 9);
				C = Rotintlft((C + H(D, A, B) + d[6] + unchecked((int)(0x6ed9eba1))), 11);
				B = Rotintlft((B + H(C, D, A) + d[14] + unchecked((int)(0x6ed9eba1))), 15);
				A = Rotintlft((A + H(B, C, D) + d[1] + unchecked((int)(0x6ed9eba1))), 3);
				D = Rotintlft((D + H(A, B, C) + d[9] + unchecked((int)(0x6ed9eba1))), 9);
				C = Rotintlft((C + H(D, A, B) + d[5] + unchecked((int)(0x6ed9eba1))), 11);
				B = Rotintlft((B + H(C, D, A) + d[13] + unchecked((int)(0x6ed9eba1))), 15);
				A = Rotintlft((A + H(B, C, D) + d[3] + unchecked((int)(0x6ed9eba1))), 3);
				D = Rotintlft((D + H(A, B, C) + d[11] + unchecked((int)(0x6ed9eba1))), 9);
				C = Rotintlft((C + H(D, A, B) + d[7] + unchecked((int)(0x6ed9eba1))), 11);
				B = Rotintlft((B + H(C, D, A) + d[15] + unchecked((int)(0x6ed9eba1))), 15);
			}
		}

		/// <summary>
		/// Cryptography support - HMACMD5 - algorithmically based on various web
		/// resources by Karl Wright
		/// </summary>
		internal class HMACMD5
		{
			protected internal byte[] ipad;

			protected internal byte[] opad;

			protected internal MessageDigest md5;

			/// <exception cref="Apache.Http.Impl.Auth.NTLMEngineException"></exception>
			internal HMACMD5(byte[] input)
			{
				byte[] key = input;
				try
				{
					md5 = MessageDigest.GetInstance("MD5");
				}
				catch (Exception ex)
				{
					// Umm, the algorithm doesn't exist - throw an
					// NTLMEngineException!
					throw new NTLMEngineException("Error getting md5 message digest implementation: "
						 + ex.Message, ex);
				}
				// Initialize the pad buffers with the key
				ipad = new byte[64];
				opad = new byte[64];
				int keyLength = key.Length;
				if (keyLength > 64)
				{
					// Use MD5 of the key instead, as described in RFC 2104
					md5.Update(key);
					key = md5.Digest();
					keyLength = key.Length;
				}
				int i = 0;
				while (i < keyLength)
				{
					ipad[i] = unchecked((byte)(key[i] ^ unchecked((byte)unchecked((int)(0x36)))));
					opad[i] = unchecked((byte)(key[i] ^ unchecked((byte)unchecked((int)(0x5c)))));
					i++;
				}
				while (i < 64)
				{
					ipad[i] = unchecked((byte)unchecked((int)(0x36)));
					opad[i] = unchecked((byte)unchecked((int)(0x5c)));
					i++;
				}
				// Very important: update the digest with the ipad buffer
				md5.Reset();
				md5.Update(ipad);
			}

			/// <summary>Grab the current digest.</summary>
			/// <remarks>Grab the current digest. This is the "answer".</remarks>
			internal virtual byte[] GetOutput()
			{
				byte[] digest = md5.Digest();
				md5.Update(opad);
				return md5.Digest(digest);
			}

			/// <summary>Update by adding a complete array</summary>
			internal virtual void Update(byte[] input)
			{
				md5.Update(input);
			}

			/// <summary>Update the algorithm</summary>
			internal virtual void Update(byte[] input, int offset, int length)
			{
				md5.Update(input, offset, length);
			}
		}

		/// <exception cref="Apache.Http.Impl.Auth.NTLMEngineException"></exception>
		public string GenerateType1Msg(string domain, string workstation)
		{
			return GetType1Message(workstation, domain);
		}

		/// <exception cref="Apache.Http.Impl.Auth.NTLMEngineException"></exception>
		public string GenerateType3Msg(string username, string password, string domain, string
			 workstation, string challenge)
		{
			NTLMEngineImpl.Type2Message t2m = new NTLMEngineImpl.Type2Message(challenge);
			return GetType3Message(username, password, workstation, domain, t2m.GetChallenge(
				), t2m.GetFlags(), t2m.GetTarget(), t2m.GetTargetInfo());
		}
	}
}
