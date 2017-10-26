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

using System.IO;
using Sharpen;

namespace Apache.Http.Impl.Auth
{
	internal class HttpEntityDigester : OutputStream
	{
		private readonly MessageDigest digester;

		private bool closed;

		private byte[] digest;

		internal HttpEntityDigester(MessageDigest digester) : base()
		{
			this.digester = digester;
			this.digester.Reset();
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override void Write(int b)
		{
			if (this.closed)
			{
				throw new IOException("Stream has been already closed");
			}
			this.digester.Update(unchecked((byte)b));
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override void Write(byte[] b, int off, int len)
		{
			if (this.closed)
			{
				throw new IOException("Stream has been already closed");
			}
			this.digester.Update(b, off, len);
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override void Close()
		{
			if (this.closed)
			{
				return;
			}
			this.closed = true;
			this.digest = this.digester.Digest();
			base.Close();
		}

		public virtual byte[] GetDigest()
		{
			return this.digest;
		}
	}
}
