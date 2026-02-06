using System;
using System.Runtime.InteropServices;

namespace POpusCodec
{
	public static class OpusLib
	{
		public static string Version
		{
			get
			{
				return Marshal.PtrToStringAnsi(Wrapper.opus_get_version_string());
			}
		}
	}
}
