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
using Apache.Http.Conn;
using Apache.Http.Util;
using Sharpen;

namespace Apache.Http.Conn
{
	/// <summary>
	/// A stream wrapper that triggers actions on
	/// <see cref="Close()">close()</see>
	/// and EOF.
	/// Primarily used to auto-release an underlying managed connection when the response
	/// body is consumed or no longer needed.
	/// </summary>
	/// <seealso cref="EofSensorWatcher">EofSensorWatcher</seealso>
	/// <since>4.0</since>
	public class EofSensorInputStream : InputStream, ConnectionReleaseTrigger
	{
		/// <summary>The wrapped input stream, while accessible.</summary>
		/// <remarks>
		/// The wrapped input stream, while accessible.
		/// The value changes to <code>null</code> when the wrapped stream
		/// becomes inaccessible.
		/// </remarks>
		protected internal InputStream wrappedStream;

		/// <summary>Indicates whether this stream itself is closed.</summary>
		/// <remarks>
		/// Indicates whether this stream itself is closed.
		/// If it isn't, but
		/// <see cref="wrappedStream">wrappedStream</see>
		/// is <code>null</code>, we're running in EOF mode.
		/// All read operations will indicate EOF without accessing
		/// the underlying stream. After closing this stream, read
		/// operations will trigger an
		/// <see cref="System.IO.IOException">IOException</see>
		/// .
		/// </remarks>
		/// <seealso cref="IsReadAllowed()">isReadAllowed</seealso>
		private bool selfClosed;

		/// <summary>The watcher to be notified, if any.</summary>
		/// <remarks>The watcher to be notified, if any.</remarks>
		private readonly EofSensorWatcher eofWatcher;

		/// <summary>Creates a new EOF sensor.</summary>
		/// <remarks>
		/// Creates a new EOF sensor.
		/// If no watcher is passed, the underlying stream will simply be
		/// closed when EOF is detected or
		/// <see cref="Close()">close</see>
		/// is called.
		/// Otherwise, the watcher decides whether the underlying stream
		/// should be closed before detaching from it.
		/// </remarks>
		/// <param name="in">the wrapped stream</param>
		/// <param name="watcher">
		/// the watcher for events, or <code>null</code> for
		/// auto-close behavior without notification
		/// </param>
		public EofSensorInputStream(InputStream @in, EofSensorWatcher watcher)
		{
			// don't use FilterInputStream as the base class, we'd have to
			// override markSupported(), mark(), and reset() to disable them
			Args.NotNull(@in, "Wrapped stream");
			wrappedStream = @in;
			selfClosed = false;
			eofWatcher = watcher;
		}

		internal virtual bool IsSelfClosed()
		{
			return selfClosed;
		}

		internal virtual InputStream GetWrappedStream()
		{
			return wrappedStream;
		}

		/// <summary>Checks whether the underlying stream can be read from.</summary>
		/// <remarks>Checks whether the underlying stream can be read from.</remarks>
		/// <returns>
		/// <code>true</code> if the underlying stream is accessible,
		/// <code>false</code> if this stream is in EOF mode and
		/// detached from the underlying stream
		/// </returns>
		/// <exception cref="System.IO.IOException">if this stream is already closed</exception>
		protected internal virtual bool IsReadAllowed()
		{
			if (selfClosed)
			{
				throw new IOException("Attempted read on closed stream.");
			}
			return (wrappedStream != null);
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override int Read()
		{
			int l = -1;
			if (IsReadAllowed())
			{
				try
				{
					l = wrappedStream.Read();
					CheckEOF(l);
				}
				catch (IOException ex)
				{
					CheckAbort();
					throw;
				}
			}
			return l;
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override int Read(byte[] b, int off, int len)
		{
			int l = -1;
			if (IsReadAllowed())
			{
				try
				{
					l = wrappedStream.Read(b, off, len);
					CheckEOF(l);
				}
				catch (IOException ex)
				{
					CheckAbort();
					throw;
				}
			}
			return l;
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override int Read(byte[] b)
		{
			return Read(b, 0, b.Length);
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override int Available()
		{
			int a = 0;
			// not -1
			if (IsReadAllowed())
			{
				try
				{
					a = wrappedStream.Available();
				}
				catch (IOException ex)
				{
					// no checkEOF() here, available() can't trigger EOF
					CheckAbort();
					throw;
				}
			}
			return a;
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override void Close()
		{
			// tolerate multiple calls to close()
			selfClosed = true;
			CheckClose();
		}

		/// <summary>Detects EOF and notifies the watcher.</summary>
		/// <remarks>
		/// Detects EOF and notifies the watcher.
		/// This method should only be called while the underlying stream is
		/// still accessible. Use
		/// <see cref="IsReadAllowed()">isReadAllowed</see>
		/// to
		/// check that condition.
		/// <br/>
		/// If EOF is detected, the watcher will be notified and this stream
		/// is detached from the underlying stream. This prevents multiple
		/// notifications from this stream.
		/// </remarks>
		/// <param name="eof">
		/// the result of the calling read operation.
		/// A negative value indicates that EOF is reached.
		/// </param>
		/// <exception cref="System.IO.IOException">in case of an IO problem on closing the underlying stream
		/// 	</exception>
		protected internal virtual void CheckEOF(int eof)
		{
			if ((wrappedStream != null) && (eof < 0))
			{
				try
				{
					bool scws = true;
					// should close wrapped stream?
					if (eofWatcher != null)
					{
						scws = eofWatcher.EofDetected(wrappedStream);
					}
					if (scws)
					{
						wrappedStream.Close();
					}
				}
				finally
				{
					wrappedStream = null;
				}
			}
		}

		/// <summary>Detects stream close and notifies the watcher.</summary>
		/// <remarks>
		/// Detects stream close and notifies the watcher.
		/// There's not much to detect since this is called by
		/// <see cref="Close()">close</see>
		/// .
		/// The watcher will only be notified if this stream is closed
		/// for the first time and before EOF has been detected.
		/// This stream will be detached from the underlying stream to prevent
		/// multiple notifications to the watcher.
		/// </remarks>
		/// <exception cref="System.IO.IOException">in case of an IO problem on closing the underlying stream
		/// 	</exception>
		protected internal virtual void CheckClose()
		{
			if (wrappedStream != null)
			{
				try
				{
					bool scws = true;
					// should close wrapped stream?
					if (eofWatcher != null)
					{
						scws = eofWatcher.StreamClosed(wrappedStream);
					}
					if (scws)
					{
						wrappedStream.Close();
					}
				}
				finally
				{
					wrappedStream = null;
				}
			}
		}

		/// <summary>Detects stream abort and notifies the watcher.</summary>
		/// <remarks>
		/// Detects stream abort and notifies the watcher.
		/// There's not much to detect since this is called by
		/// <see cref="AbortConnection()">abortConnection</see>
		/// .
		/// The watcher will only be notified if this stream is aborted
		/// for the first time and before EOF has been detected or the
		/// stream has been
		/// <see cref="Close()">closed</see>
		/// gracefully.
		/// This stream will be detached from the underlying stream to prevent
		/// multiple notifications to the watcher.
		/// </remarks>
		/// <exception cref="System.IO.IOException">in case of an IO problem on closing the underlying stream
		/// 	</exception>
		protected internal virtual void CheckAbort()
		{
			if (wrappedStream != null)
			{
				try
				{
					bool scws = true;
					// should close wrapped stream?
					if (eofWatcher != null)
					{
						scws = eofWatcher.StreamAbort(wrappedStream);
					}
					if (scws)
					{
						wrappedStream.Close();
					}
				}
				finally
				{
					wrappedStream = null;
				}
			}
		}

		/// <summary>
		/// Same as
		/// <see cref="Close()">close()</see>
		/// .
		/// </summary>
		/// <exception cref="System.IO.IOException"></exception>
		public virtual void ReleaseConnection()
		{
			Close();
		}

		/// <summary>Aborts this stream.</summary>
		/// <remarks>
		/// Aborts this stream.
		/// This is a special version of
		/// <see cref="Close()">close()</see>
		/// which prevents
		/// re-use of the underlying connection, if any. Calling this method
		/// indicates that there should be no attempt to read until the end of
		/// the stream.
		/// </remarks>
		/// <exception cref="System.IO.IOException"></exception>
		public virtual void AbortConnection()
		{
			// tolerate multiple calls
			selfClosed = true;
			CheckAbort();
		}
	}
}
