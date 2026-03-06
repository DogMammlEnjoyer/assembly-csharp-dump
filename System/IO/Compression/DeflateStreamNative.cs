using System;
using System.Runtime.InteropServices;
using System.Threading;
using Mono.Util;

namespace System.IO.Compression
{
	internal class DeflateStreamNative
	{
		private DeflateStreamNative()
		{
		}

		public static DeflateStreamNative Create(Stream compressedStream, CompressionMode mode, bool gzip)
		{
			DeflateStreamNative deflateStreamNative = new DeflateStreamNative();
			deflateStreamNative.data = GCHandle.Alloc(deflateStreamNative);
			deflateStreamNative.feeder = ((mode == CompressionMode.Compress) ? new DeflateStreamNative.UnmanagedReadOrWrite(DeflateStreamNative.UnmanagedWrite) : new DeflateStreamNative.UnmanagedReadOrWrite(DeflateStreamNative.UnmanagedRead));
			deflateStreamNative.z_stream = DeflateStreamNative.CreateZStream(mode, gzip, deflateStreamNative.feeder, GCHandle.ToIntPtr(deflateStreamNative.data));
			if (deflateStreamNative.z_stream.IsInvalid)
			{
				deflateStreamNative.Dispose(true);
				return null;
			}
			deflateStreamNative.base_stream = compressedStream;
			return deflateStreamNative;
		}

		~DeflateStreamNative()
		{
			this.Dispose(false);
		}

		public void Dispose(bool disposing)
		{
			if (disposing && !this.disposed)
			{
				this.disposed = true;
				GC.SuppressFinalize(this);
			}
			else
			{
				this.base_stream = Stream.Null;
			}
			this.io_buffer = null;
			if (this.z_stream != null && !this.z_stream.IsInvalid)
			{
				this.z_stream.Dispose();
			}
			GCHandle gchandle = this.data;
			if (this.data.IsAllocated)
			{
				this.data.Free();
			}
		}

		public void Flush()
		{
			int result = DeflateStreamNative.Flush(this.z_stream);
			this.CheckResult(result, "Flush");
		}

		public int ReadZStream(IntPtr buffer, int length)
		{
			int result = DeflateStreamNative.ReadZStream(this.z_stream, buffer, length);
			this.CheckResult(result, "ReadInternal");
			return result;
		}

		public void WriteZStream(IntPtr buffer, int length)
		{
			int result = DeflateStreamNative.WriteZStream(this.z_stream, buffer, length);
			this.CheckResult(result, "WriteInternal");
		}

		[MonoPInvokeCallback(typeof(DeflateStreamNative.UnmanagedReadOrWrite))]
		private static int UnmanagedRead(IntPtr buffer, int length, IntPtr data)
		{
			DeflateStreamNative deflateStreamNative = GCHandle.FromIntPtr(data).Target as DeflateStreamNative;
			if (deflateStreamNative == null)
			{
				return -1;
			}
			return deflateStreamNative.UnmanagedRead(buffer, length);
		}

		private int UnmanagedRead(IntPtr buffer, int length)
		{
			if (this.io_buffer == null)
			{
				this.io_buffer = new byte[4096];
			}
			int count = Math.Min(length, this.io_buffer.Length);
			int num;
			try
			{
				num = this.base_stream.Read(this.io_buffer, 0, count);
			}
			catch (Exception ex)
			{
				this.last_error = ex;
				return -12;
			}
			if (num > 0)
			{
				Marshal.Copy(this.io_buffer, 0, buffer, num);
			}
			return num;
		}

		[MonoPInvokeCallback(typeof(DeflateStreamNative.UnmanagedReadOrWrite))]
		private static int UnmanagedWrite(IntPtr buffer, int length, IntPtr data)
		{
			DeflateStreamNative deflateStreamNative = GCHandle.FromIntPtr(data).Target as DeflateStreamNative;
			if (deflateStreamNative == null)
			{
				return -1;
			}
			return deflateStreamNative.UnmanagedWrite(buffer, length);
		}

		private unsafe int UnmanagedWrite(IntPtr buffer, int length)
		{
			int num = 0;
			while (length > 0)
			{
				if (this.io_buffer == null)
				{
					this.io_buffer = new byte[4096];
				}
				int num2 = Math.Min(length, this.io_buffer.Length);
				Marshal.Copy(buffer, this.io_buffer, 0, num2);
				try
				{
					this.base_stream.Write(this.io_buffer, 0, num2);
				}
				catch (Exception ex)
				{
					this.last_error = ex;
					return -12;
				}
				buffer = new IntPtr((void*)((byte*)buffer.ToPointer() + num2));
				length -= num2;
				num += num2;
			}
			return num;
		}

		private void CheckResult(int result, string where)
		{
			if (result >= 0)
			{
				return;
			}
			Exception ex = Interlocked.Exchange<Exception>(ref this.last_error, null);
			if (ex != null)
			{
				throw ex;
			}
			string str;
			switch (result)
			{
			case -11:
				str = "IO error";
				goto IL_94;
			case -10:
				str = "Invalid argument(s)";
				goto IL_94;
			case -6:
				str = "Invalid version";
				goto IL_94;
			case -5:
				str = "Internal error (no progress possible)";
				goto IL_94;
			case -4:
				str = "Not enough memory";
				goto IL_94;
			case -3:
				str = "Corrupted data";
				goto IL_94;
			case -2:
				str = "Internal error";
				goto IL_94;
			case -1:
				str = "Unknown error";
				goto IL_94;
			}
			str = "Unknown error";
			IL_94:
			throw new IOException(str + " " + where);
		}

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl)]
		private static extern DeflateStreamNative.SafeDeflateStreamHandle CreateZStream(CompressionMode compress, bool gzip, DeflateStreamNative.UnmanagedReadOrWrite feeder, IntPtr data);

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl)]
		private static extern int CloseZStream(IntPtr stream);

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl)]
		private static extern int Flush(DeflateStreamNative.SafeDeflateStreamHandle stream);

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl)]
		private static extern int ReadZStream(DeflateStreamNative.SafeDeflateStreamHandle stream, IntPtr buffer, int length);

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl)]
		private static extern int WriteZStream(DeflateStreamNative.SafeDeflateStreamHandle stream, IntPtr buffer, int length);

		private const int BufferSize = 4096;

		private DeflateStreamNative.UnmanagedReadOrWrite feeder;

		private Stream base_stream;

		private DeflateStreamNative.SafeDeflateStreamHandle z_stream;

		private GCHandle data;

		private bool disposed;

		private byte[] io_buffer;

		private Exception last_error;

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int UnmanagedReadOrWrite(IntPtr buffer, int length, IntPtr data);

		private sealed class SafeDeflateStreamHandle : SafeHandle
		{
			public override bool IsInvalid
			{
				get
				{
					return this.handle == IntPtr.Zero;
				}
			}

			private SafeDeflateStreamHandle() : base(IntPtr.Zero, true)
			{
			}

			internal SafeDeflateStreamHandle(IntPtr handle) : base(handle, true)
			{
			}

			protected override bool ReleaseHandle()
			{
				try
				{
					DeflateStreamNative.CloseZStream(this.handle);
				}
				catch
				{
				}
				return true;
			}
		}
	}
}
