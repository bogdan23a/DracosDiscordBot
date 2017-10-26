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

using System.Collections.Generic;
using System.Net.Sockets;
using Apache.Http.Conn.Ssl;
using Sharpen;

namespace Apache.Http.Conn.Ssl
{
	/// <summary>
	/// Builder for
	/// <see cref="Sharpen.SSLContext">Sharpen.SSLContext</see>
	/// instances.
	/// </summary>
	/// <since>4.3</since>
	public class SSLContextBuilder
	{
		internal const string Tls = "TLS";

		internal const string Ssl = "SSL";

		private string protocol;

		private ICollection<KeyManager> keymanagers;

		private ICollection<TrustManager> trustmanagers;

		private SecureRandom secureRandom;

		public SSLContextBuilder() : base()
		{
			this.keymanagers = new HashSet<KeyManager>();
			this.trustmanagers = new HashSet<TrustManager>();
		}

		public virtual Apache.Http.Conn.Ssl.SSLContextBuilder UseTLS()
		{
			this.protocol = Tls;
			return this;
		}

		public virtual Apache.Http.Conn.Ssl.SSLContextBuilder UseSSL()
		{
			this.protocol = Ssl;
			return this;
		}

		public virtual Apache.Http.Conn.Ssl.SSLContextBuilder UseProtocol(string protocol
			)
		{
			this.protocol = protocol;
			return this;
		}

		public virtual Apache.Http.Conn.Ssl.SSLContextBuilder SetSecureRandom(SecureRandom
			 secureRandom)
		{
			this.secureRandom = secureRandom;
			return this;
		}

		/// <exception cref="Sharpen.NoSuchAlgorithmException"></exception>
		/// <exception cref="Sharpen.KeyStoreException"></exception>
		public virtual Apache.Http.Conn.Ssl.SSLContextBuilder LoadTrustMaterial(KeyStore 
			truststore, TrustStrategy trustStrategy)
		{
			TrustManagerFactory tmfactory = TrustManagerFactory.GetInstance(TrustManagerFactory
				.GetDefaultAlgorithm());
			tmfactory.Init(truststore);
			TrustManager[] tms = tmfactory.GetTrustManagers();
			if (tms != null)
			{
				if (trustStrategy != null)
				{
					for (int i = 0; i < tms.Length; i++)
					{
						TrustManager tm = tms[i];
						if (tm is X509TrustManager)
						{
							tms[i] = new SSLContextBuilder.TrustManagerDelegate((X509TrustManager)tm, trustStrategy
								);
						}
					}
				}
				for (int i_1 = 0; i_1 < tms.Length; i_1++)
				{
					this.trustmanagers.AddItem(tms[i_1]);
				}
			}
			return this;
		}

		/// <exception cref="Sharpen.NoSuchAlgorithmException"></exception>
		/// <exception cref="Sharpen.KeyStoreException"></exception>
		public virtual Apache.Http.Conn.Ssl.SSLContextBuilder LoadTrustMaterial(KeyStore 
			truststore)
		{
			return LoadTrustMaterial(truststore, null);
		}

		/// <exception cref="Sharpen.NoSuchAlgorithmException"></exception>
		/// <exception cref="Sharpen.KeyStoreException"></exception>
		/// <exception cref="Sharpen.UnrecoverableKeyException"></exception>
		public virtual Apache.Http.Conn.Ssl.SSLContextBuilder LoadKeyMaterial(KeyStore keystore
			, char[] keyPassword)
		{
			LoadKeyMaterial(keystore, keyPassword, null);
			return this;
		}

		/// <exception cref="Sharpen.NoSuchAlgorithmException"></exception>
		/// <exception cref="Sharpen.KeyStoreException"></exception>
		/// <exception cref="Sharpen.UnrecoverableKeyException"></exception>
		public virtual Apache.Http.Conn.Ssl.SSLContextBuilder LoadKeyMaterial(KeyStore keystore
			, char[] keyPassword, PrivateKeyStrategy aliasStrategy)
		{
			KeyManagerFactory kmfactory = KeyManagerFactory.GetInstance(KeyManagerFactory.GetDefaultAlgorithm
				());
			kmfactory.Init(keystore, keyPassword);
			KeyManager[] kms = kmfactory.GetKeyManagers();
			if (kms != null)
			{
				if (aliasStrategy != null)
				{
					for (int i = 0; i < kms.Length; i++)
					{
						KeyManager km = kms[i];
						if (km is X509KeyManager)
						{
							kms[i] = new SSLContextBuilder.KeyManagerDelegate((X509KeyManager)km, aliasStrategy
								);
						}
					}
				}
				for (int i_1 = 0; i_1 < kms.Length; i_1++)
				{
					keymanagers.AddItem(kms[i_1]);
				}
			}
			return this;
		}

		/// <exception cref="Sharpen.NoSuchAlgorithmException"></exception>
		/// <exception cref="Sharpen.KeyManagementException"></exception>
		public virtual SSLContext Build()
		{
			SSLContext sslcontext = SSLContext.GetInstance(this.protocol != null ? this.protocol
				 : Tls);
			sslcontext.Init(!keymanagers.IsEmpty() ? Sharpen.Collections.ToArray(keymanagers, 
				new KeyManager[keymanagers.Count]) : null, !trustmanagers.IsEmpty() ? Sharpen.Collections.ToArray
				(trustmanagers, new TrustManager[trustmanagers.Count]) : null, secureRandom);
			return sslcontext;
		}

		internal class TrustManagerDelegate : X509TrustManager
		{
			private readonly X509TrustManager trustManager;

			private readonly TrustStrategy trustStrategy;

			internal TrustManagerDelegate(X509TrustManager trustManager, TrustStrategy trustStrategy
				) : base()
			{
				this.trustManager = trustManager;
				this.trustStrategy = trustStrategy;
			}

			/// <exception cref="Sharpen.CertificateException"></exception>
			public virtual void CheckClientTrusted(X509Certificate[] chain, string authType)
			{
				this.trustManager.CheckClientTrusted(chain, authType);
			}

			/// <exception cref="Sharpen.CertificateException"></exception>
			public virtual void CheckServerTrusted(X509Certificate[] chain, string authType)
			{
				if (!this.trustStrategy.IsTrusted(chain, authType))
				{
					this.trustManager.CheckServerTrusted(chain, authType);
				}
			}

			public virtual X509Certificate[] GetAcceptedIssuers()
			{
				return this.trustManager.GetAcceptedIssuers();
			}
		}

		internal class KeyManagerDelegate : X509KeyManager
		{
			private readonly X509KeyManager keyManager;

			private readonly PrivateKeyStrategy aliasStrategy;

			internal KeyManagerDelegate(X509KeyManager keyManager, PrivateKeyStrategy aliasStrategy
				) : base()
			{
				this.keyManager = keyManager;
				this.aliasStrategy = aliasStrategy;
			}

			public virtual string[] GetClientAliases(string keyType, Principal[] issuers)
			{
				return this.keyManager.GetClientAliases(keyType, issuers);
			}

			public virtual string ChooseClientAlias(string[] keyTypes, Principal[] issuers, Socket
				 socket)
			{
				IDictionary<string, PrivateKeyDetails> validAliases = new Dictionary<string, PrivateKeyDetails
					>();
				foreach (string keyType in keyTypes)
				{
					string[] aliases = this.keyManager.GetClientAliases(keyType, issuers);
					if (aliases != null)
					{
						foreach (string alias in aliases)
						{
							validAliases.Put(alias, new PrivateKeyDetails(keyType, this.keyManager.GetCertificateChain
								(alias)));
						}
					}
				}
				return this.aliasStrategy.ChooseAlias(validAliases, socket);
			}

			public virtual string[] GetServerAliases(string keyType, Principal[] issuers)
			{
				return this.keyManager.GetServerAliases(keyType, issuers);
			}

			public virtual string ChooseServerAlias(string keyType, Principal[] issuers, Socket
				 socket)
			{
				IDictionary<string, PrivateKeyDetails> validAliases = new Dictionary<string, PrivateKeyDetails
					>();
				string[] aliases = this.keyManager.GetServerAliases(keyType, issuers);
				if (aliases != null)
				{
					foreach (string alias in aliases)
					{
						validAliases.Put(alias, new PrivateKeyDetails(keyType, this.keyManager.GetCertificateChain
							(alias)));
					}
				}
				return this.aliasStrategy.ChooseAlias(validAliases, socket);
			}

			public virtual X509Certificate[] GetCertificateChain(string alias)
			{
				return this.keyManager.GetCertificateChain(alias);
			}

			public virtual PrivateKey GetPrivateKey(string alias)
			{
				return this.keyManager.GetPrivateKey(alias);
			}
		}
	}
}
