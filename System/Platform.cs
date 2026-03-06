using System;
using System.Runtime.InteropServices;

namespace System
{
	internal static class Platform
	{
		[DllImport("libc")]
		private static extern int uname(IntPtr buf);

		private static void CheckOS()
		{
			if (Environment.OSVersion.Platform != PlatformID.Unix)
			{
				Platform.checkedOS = true;
				return;
			}
			IntPtr intPtr = Marshal.AllocHGlobal(8192);
			if (Platform.uname(intPtr) == 0)
			{
				string a = Marshal.PtrToStringAnsi(intPtr);
				if (!(a == "Darwin"))
				{
					if (!(a == "FreeBSD"))
					{
						if (!(a == "AIX"))
						{
							if (!(a == "OS400"))
							{
								if (a == "OpenBSD")
								{
									Platform.isOpenBSD = true;
								}
							}
							else
							{
								Platform.isIBMi = true;
							}
						}
						else
						{
							Platform.isAix = true;
						}
					}
					else
					{
						Platform.isFreeBSD = true;
					}
				}
				else
				{
					Platform.isMacOS = true;
				}
			}
			Marshal.FreeHGlobal(intPtr);
			Platform.checkedOS = true;
		}

		public static bool IsMacOS
		{
			get
			{
				if (!Platform.checkedOS)
				{
					try
					{
						Platform.CheckOS();
					}
					catch (DllNotFoundException)
					{
						Platform.isMacOS = false;
					}
				}
				return Platform.isMacOS;
			}
		}

		public static bool IsFreeBSD
		{
			get
			{
				if (!Platform.checkedOS)
				{
					Platform.CheckOS();
				}
				return Platform.isFreeBSD;
			}
		}

		public static bool IsOpenBSD
		{
			get
			{
				if (!Platform.checkedOS)
				{
					Platform.CheckOS();
				}
				return Platform.isOpenBSD;
			}
		}

		public static bool IsIBMi
		{
			get
			{
				if (!Platform.checkedOS)
				{
					Platform.CheckOS();
				}
				return Platform.isIBMi;
			}
		}

		public static bool IsAix
		{
			get
			{
				if (!Platform.checkedOS)
				{
					Platform.CheckOS();
				}
				return Platform.isAix;
			}
		}

		private static bool checkedOS;

		private static bool isMacOS;

		private static bool isAix;

		private static bool isIBMi;

		private static bool isFreeBSD;

		private static bool isOpenBSD;
	}
}
