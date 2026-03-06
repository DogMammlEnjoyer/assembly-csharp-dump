using System;
using System.Runtime.InteropServices;

namespace Mono.Btls
{
	internal class MonoBtlsBioMemory : MonoBtlsBio
	{
		[DllImport("libmono-btls-shared")]
		private static extern IntPtr mono_btls_bio_mem_new();

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_bio_mem_get_data(IntPtr handle, out IntPtr data);

		public MonoBtlsBioMemory() : base(new MonoBtlsBio.BoringBioHandle(MonoBtlsBioMemory.mono_btls_bio_mem_new()))
		{
		}

		public byte[] GetData()
		{
			bool flag = false;
			byte[] result;
			try
			{
				base.Handle.DangerousAddRef(ref flag);
				IntPtr source;
				int num = MonoBtlsBioMemory.mono_btls_bio_mem_get_data(base.Handle.DangerousGetHandle(), out source);
				base.CheckError(num > 0, "GetData");
				byte[] array = new byte[num];
				Marshal.Copy(source, array, 0, num);
				result = array;
			}
			finally
			{
				if (flag)
				{
					base.Handle.DangerousRelease();
				}
			}
			return result;
		}
	}
}
