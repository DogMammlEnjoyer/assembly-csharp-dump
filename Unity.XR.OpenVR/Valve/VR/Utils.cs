using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Valve.VR
{
	public class Utils
	{
		public static IntPtr ToUtf8(string managedString)
		{
			if (managedString == null)
			{
				return IntPtr.Zero;
			}
			int num = Encoding.UTF8.GetByteCount(managedString) + 1;
			if (Utils.buffer.Length < num)
			{
				Utils.buffer = new byte[num];
			}
			int bytes = Encoding.UTF8.GetBytes(managedString, 0, managedString.Length, Utils.buffer, 0);
			Utils.buffer[bytes] = 0;
			IntPtr intPtr = Marshal.AllocHGlobal(bytes + 1);
			Marshal.Copy(Utils.buffer, 0, intPtr, bytes + 1);
			return intPtr;
		}

		private static byte[] buffer = new byte[1024];
	}
}
