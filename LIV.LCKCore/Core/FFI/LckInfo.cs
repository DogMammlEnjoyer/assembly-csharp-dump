using System;

namespace Liv.Lck.Core.FFI
{
	internal struct LckInfo
	{
		public static LckInfo AllocateFromLckInfo(LckInfo lckInfo)
		{
			return new LckInfo(lckInfo);
		}

		public void Free()
		{
			InteropUtilities.Free(this.Version);
		}

		private LckInfo(LckInfo lckInfo)
		{
			this.Version = InteropUtilities.StringToUTF8Pointer(lckInfo.Version);
			this.BuildNumber = lckInfo.BuildNumber;
		}

		public IntPtr Version;

		public int BuildNumber;
	}
}
