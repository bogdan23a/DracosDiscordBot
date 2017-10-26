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
using Apache.Http.Impl.Conn;
using Sharpen;

namespace Apache.Http.Impl.Conn
{
	/// <summary>Internal class.</summary>
	/// <remarks>Internal class.</remarks>
	/// <since>4.3</since>
	internal class LoggingInputStream : InputStream
	{
		private readonly InputStream @in;

		private readonly Wire wire;

		public LoggingInputStream(InputStream @in, Wire wire) : base()
		{
			this.@in = @in;
			this.wire = wire;
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override int Read()
		{
			int b = @in.Read();
			if (b != -1)
			{
				wire.Input(b);
			}
			return b;
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override int Read(byte[] b)
		{
			int bytesRead = @in.Read(b);
			if (bytesRead != -1)
			{
				wire.Input(b, 0, bytesRead);
			}
			return bytesRead;
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override int Read(byte[] b, int off, int len)
		{
			int bytesRead = @in.Read(b, off, len);
			if (bytesRead != -1)
			{
				wire.Input(b, off, bytesRead);
			}
			return bytesRead;
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override long Skip(long n)
		{
			return base.Skip(n);
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override int Available()
		{
			return @in.Available();
		}

		public override void Mark(int readlimit)
		{
			lock (this)
			{
				base.Mark(readlimit);
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override void Reset()
		{
			lock (this)
			{
				base.Reset();
			}
		}

		public override bool MarkSupported()
		{
			return false;
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override void Close()
		{
			@in.Close();
		}
	}
}
