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
using Apache.Http.Client.Entity;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Zip.Compression;
using Sharpen;

namespace Apache.Http.Client.Entity
{
	/// <summary>Deflate input stream.</summary>
	/// <remarks>
	/// Deflate input stream.    This class includes logic needed for various Rfc's in order
	/// to reasonably implement the "deflate" compression style.
	/// </remarks>
	public class DeflateInputStream : InputStream
	{
		private InputStream sourceStream;

		/// <exception cref="System.IO.IOException"></exception>
		public DeflateInputStream(InputStream wrapped)
		{
			byte[] peeked = new byte[6];
			PushbackInputStream pushback = new PushbackInputStream(wrapped, peeked.Length);
			int headerLength = pushback.Read(peeked);
			if (headerLength == -1)
			{
				throw new IOException("Unable to read the response");
			}
			byte[] dummy = new byte[1];
			Inflater inf = new Inflater();
			try
			{
				int n;
				while ((n = inf.Inflate(dummy)) == 0)
				{
					if (inf.IsFinished)
					{
						throw new IOException("Unable to read the response");
					}
					if (inf.NeedsDictionary())
					{
						break;
					}
					if (inf.IsNeedingInput)
					{
						inf.SetInput(peeked);
					}
				}
				if (n == -1)
				{
					throw new IOException("Unable to read the response");
				}
				pushback.Unread(peeked, 0, headerLength);
				sourceStream = new DeflateInputStream.DeflateStream(pushback, new Inflater());
			}
			catch (SharpZipBaseException)
			{
				pushback.Unread(peeked, 0, headerLength);
				sourceStream = new DeflateInputStream.DeflateStream(pushback, new Inflater(true));
			}
			finally
			{
				inf.Finish();
			}
		}

		/// <summary>Read a byte.</summary>
		/// <remarks>Read a byte.</remarks>
		/// <exception cref="System.IO.IOException"></exception>
		public override int Read()
		{
			return sourceStream.Read();
		}

		/// <summary>Read lots of bytes.</summary>
		/// <remarks>Read lots of bytes.</remarks>
		/// <exception cref="System.IO.IOException"></exception>
		public override int Read(byte[] b)
		{
			return sourceStream.Read(b);
		}

		/// <summary>Read lots of specific bytes.</summary>
		/// <remarks>Read lots of specific bytes.</remarks>
		/// <exception cref="System.IO.IOException"></exception>
		public override int Read(byte[] b, int off, int len)
		{
			return sourceStream.Read(b, off, len);
		}

		/// <summary>Skip</summary>
		/// <exception cref="System.IO.IOException"></exception>
		public override long Skip(long n)
		{
			return sourceStream.Skip(n);
		}

		/// <summary>Get available.</summary>
		/// <remarks>Get available.</remarks>
		/// <exception cref="System.IO.IOException"></exception>
		public override int Available()
		{
			return sourceStream.Available();
		}

		/// <summary>Mark.</summary>
		/// <remarks>Mark.</remarks>
		public override void Mark(int readLimit)
		{
			sourceStream.Mark(readLimit);
		}

		/// <summary>Reset.</summary>
		/// <remarks>Reset.</remarks>
		/// <exception cref="System.IO.IOException"></exception>
		public override void Reset()
		{
			sourceStream.Reset();
		}

		/// <summary>Check if mark is supported.</summary>
		/// <remarks>Check if mark is supported.</remarks>
		public override bool MarkSupported()
		{
			return sourceStream.MarkSupported();
		}

		/// <summary>Close.</summary>
		/// <remarks>Close.</remarks>
		/// <exception cref="System.IO.IOException"></exception>
		public override void Close()
		{
			sourceStream.Close();
		}

		internal class DeflateStream : InflaterInputStream
		{
			private bool closed = false;

			public DeflateStream(InputStream @in, Inflater inflater) : base(@in, inflater)
			{
			}

			/// <exception cref="System.IO.IOException"></exception>
			public override void Close()
			{
				if (closed)
				{
					return;
				}
				closed = true;
				inf.Finish();
				base.Close();
			}
		}
	}
}
