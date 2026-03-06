using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(168)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct GetTicketForWebApiResponse_t
	{
		public const int k_iCallback = 168;

		public HAuthTicket m_hAuthTicket;

		public EResult m_eResult;

		public int m_cubTicket;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2560)]
		public byte[] m_rgubTicket;
	}
}
