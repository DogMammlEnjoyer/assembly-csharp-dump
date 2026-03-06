using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(350)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct EquippedProfileItemsChanged_t
	{
		public const int k_iCallback = 350;

		public CSteamID m_steamID;
	}
}
