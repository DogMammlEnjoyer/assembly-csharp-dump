using System;
using System.Runtime.InteropServices;

namespace Liv.Lck.Core
{
	public struct LckInfo
	{
		[MarshalAs(UnmanagedType.LPUTF8Str)]
		public string Version;

		public int BuildNumber;

		[MarshalAs(UnmanagedType.U1)]
		public bool ForceStagingEnvironment;
	}
}
