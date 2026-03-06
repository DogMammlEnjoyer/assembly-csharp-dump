using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Mono.Btls
{
	internal class MonoBtlsBio : MonoBtlsObject
	{
		internal MonoBtlsBio(MonoBtlsBio.BoringBioHandle handle) : base(handle)
		{
		}

		protected internal new MonoBtlsBio.BoringBioHandle Handle
		{
			get
			{
				return (MonoBtlsBio.BoringBioHandle)base.Handle;
			}
		}

		public static MonoBtlsBio CreateMonoStream(Stream stream)
		{
			return MonoBtlsBioMono.CreateStream(stream, false);
		}

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_bio_read(IntPtr bio, IntPtr data, int len);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_bio_write(IntPtr bio, IntPtr data, int len);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_bio_flush(IntPtr bio);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_bio_indent(IntPtr bio, uint indent, uint max_indent);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_bio_hexdump(IntPtr bio, IntPtr data, int len, uint indent);

		[DllImport("libmono-btls-shared")]
		private static extern void mono_btls_bio_print_errors(IntPtr bio);

		[DllImport("libmono-btls-shared")]
		private static extern void mono_btls_bio_free(IntPtr handle);

		public int Read(byte[] buffer, int offset, int size)
		{
			base.CheckThrow();
			IntPtr intPtr = Marshal.AllocHGlobal(size);
			if (intPtr == IntPtr.Zero)
			{
				throw new OutOfMemoryException();
			}
			bool flag = false;
			int result;
			try
			{
				this.Handle.DangerousAddRef(ref flag);
				int num = MonoBtlsBio.mono_btls_bio_read(this.Handle.DangerousGetHandle(), intPtr, size);
				if (num > 0)
				{
					Marshal.Copy(intPtr, buffer, offset, num);
				}
				result = num;
			}
			finally
			{
				if (flag)
				{
					this.Handle.DangerousRelease();
				}
				Marshal.FreeHGlobal(intPtr);
			}
			return result;
		}

		public int Write(byte[] buffer, int offset, int size)
		{
			base.CheckThrow();
			IntPtr intPtr = Marshal.AllocHGlobal(size);
			if (intPtr == IntPtr.Zero)
			{
				throw new OutOfMemoryException();
			}
			bool flag = false;
			int result;
			try
			{
				this.Handle.DangerousAddRef(ref flag);
				Marshal.Copy(buffer, offset, intPtr, size);
				result = MonoBtlsBio.mono_btls_bio_write(this.Handle.DangerousGetHandle(), intPtr, size);
			}
			finally
			{
				if (flag)
				{
					this.Handle.DangerousRelease();
				}
				Marshal.FreeHGlobal(intPtr);
			}
			return result;
		}

		public int Flush()
		{
			base.CheckThrow();
			bool flag = false;
			int result;
			try
			{
				this.Handle.DangerousAddRef(ref flag);
				result = MonoBtlsBio.mono_btls_bio_flush(this.Handle.DangerousGetHandle());
			}
			finally
			{
				if (flag)
				{
					this.Handle.DangerousRelease();
				}
			}
			return result;
		}

		public int Indent(uint indent, uint max_indent)
		{
			base.CheckThrow();
			bool flag = false;
			int result;
			try
			{
				this.Handle.DangerousAddRef(ref flag);
				result = MonoBtlsBio.mono_btls_bio_indent(this.Handle.DangerousGetHandle(), indent, max_indent);
			}
			finally
			{
				if (flag)
				{
					this.Handle.DangerousRelease();
				}
			}
			return result;
		}

		public int HexDump(byte[] buffer, uint indent)
		{
			base.CheckThrow();
			IntPtr intPtr = Marshal.AllocHGlobal(buffer.Length);
			if (intPtr == IntPtr.Zero)
			{
				throw new OutOfMemoryException();
			}
			bool flag = false;
			int result;
			try
			{
				this.Handle.DangerousAddRef(ref flag);
				Marshal.Copy(buffer, 0, intPtr, buffer.Length);
				result = MonoBtlsBio.mono_btls_bio_hexdump(this.Handle.DangerousGetHandle(), intPtr, buffer.Length, indent);
			}
			finally
			{
				if (flag)
				{
					this.Handle.DangerousRelease();
				}
				Marshal.FreeHGlobal(intPtr);
			}
			return result;
		}

		public void PrintErrors()
		{
			base.CheckThrow();
			bool flag = false;
			try
			{
				this.Handle.DangerousAddRef(ref flag);
				MonoBtlsBio.mono_btls_bio_print_errors(this.Handle.DangerousGetHandle());
			}
			finally
			{
				if (flag)
				{
					this.Handle.DangerousRelease();
				}
			}
		}

		protected internal class BoringBioHandle : MonoBtlsObject.MonoBtlsHandle
		{
			public BoringBioHandle(IntPtr handle) : base(handle, true)
			{
			}

			protected override bool ReleaseHandle()
			{
				if (this.handle != IntPtr.Zero)
				{
					MonoBtlsBio.mono_btls_bio_free(this.handle);
					this.handle = IntPtr.Zero;
				}
				return true;
			}
		}
	}
}
