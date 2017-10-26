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
using System.Text;
using Apache.Http.Util;
using Org.Apache.Commons.Logging;
using Sharpen;

namespace Apache.Http.Impl.Conn
{
	/// <summary>Logs data to the wire LOG.</summary>
	/// <remarks>
	/// Logs data to the wire LOG.
	/// TODO: make package private. Should not be part of the public API.
	/// </remarks>
	/// <since>4.0</since>
	public class Wire
	{
		private readonly Log log;

		private readonly string id;

		/// <since>4.3</since>
		public Wire(Log log, string id)
		{
			this.log = log;
			this.id = id;
		}

		public Wire(Log log) : this(log, string.Empty)
		{
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void Wire(string header, InputStream instream)
		{
			StringBuilder buffer = new StringBuilder();
			int ch;
			while ((ch = instream.Read()) != -1)
			{
				if (ch == 13)
				{
					buffer.Append("[\\r]");
				}
				else
				{
					if (ch == 10)
					{
						buffer.Append("[\\n]\"");
						buffer.Insert(0, "\"");
						buffer.Insert(0, header);
						log.Debug(id + " " + buffer.ToString());
						buffer.Length = 0;
					}
					else
					{
						if ((ch < 32) || (ch > 127))
						{
							buffer.Append("[0x");
							buffer.Append(Sharpen.Extensions.ToHexString(ch));
							buffer.Append("]");
						}
						else
						{
							buffer.Append((char)ch);
						}
					}
				}
			}
			if (buffer.Length > 0)
			{
				buffer.Append('\"');
				buffer.Insert(0, '\"');
				buffer.Insert(0, header);
				log.Debug(id + " " + buffer.ToString());
			}
		}

		public virtual bool Enabled()
		{
			return log.IsDebugEnabled();
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void Output(InputStream outstream)
		{
			Args.NotNull(outstream, "Output");
			Wire(">> ", outstream);
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void Input(InputStream instream)
		{
			Args.NotNull(instream, "Input");
			Wire("<< ", instream);
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void Output(byte[] b, int off, int len)
		{
			Args.NotNull(b, "Output");
			Wire(">> ", new ByteArrayInputStream(b, off, len));
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void Input(byte[] b, int off, int len)
		{
			Args.NotNull(b, "Input");
			Wire("<< ", new ByteArrayInputStream(b, off, len));
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void Output(byte[] b)
		{
			Args.NotNull(b, "Output");
			Wire(">> ", new ByteArrayInputStream(b));
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void Input(byte[] b)
		{
			Args.NotNull(b, "Input");
			Wire("<< ", new ByteArrayInputStream(b));
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void Output(int b)
		{
			Output(new byte[] { unchecked((byte)b) });
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void Input(int b)
		{
			Input(new byte[] { unchecked((byte)b) });
		}

		/// <exception cref="System.IO.IOException"></exception>
		[Obsolete]
		[System.ObsoleteAttribute(@"(4.1)  do not use")]
		public virtual void Output(string s)
		{
			Args.NotNull(s, "Output");
			Output(Sharpen.Runtime.GetBytesForString(s));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[Obsolete]
		[System.ObsoleteAttribute(@"(4.1)  do not use")]
		public virtual void Input(string s)
		{
			Args.NotNull(s, "Input");
			Input(Sharpen.Runtime.GetBytesForString(s));
		}
	}
}
