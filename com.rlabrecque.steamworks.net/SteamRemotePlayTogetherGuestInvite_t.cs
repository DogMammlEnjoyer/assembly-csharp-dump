using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(5703)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct SteamRemotePlayTogetherGuestInvite_t
	{
		public string m_szConnectURL
		{
			get
			{
				return InteropHelp.ByteArrayToStringUTF8(this.m_szConnectURL_);
			}
			set
			{
				InteropHelp.StringToByteArrayUTF8(value, this.m_szConnectURL_, 1024);
			}
		}

		public const int k_iCallback = 5703;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
		private byte[] m_szConnectURL_;
	}
}
