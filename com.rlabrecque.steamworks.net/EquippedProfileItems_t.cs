using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(351)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct EquippedProfileItems_t
	{
		public const int k_iCallback = 351;

		public EResult m_eResult;

		public CSteamID m_steamID;

		[MarshalAs(UnmanagedType.I1)]
		public bool m_bHasAnimatedAvatar;

		[MarshalAs(UnmanagedType.I1)]
		public bool m_bHasAvatarFrame;

		[MarshalAs(UnmanagedType.I1)]
		public bool m_bHasProfileModifier;

		[MarshalAs(UnmanagedType.I1)]
		public bool m_bHasProfileBackground;

		[MarshalAs(UnmanagedType.I1)]
		public bool m_bHasMiniProfileBackground;
	}
}
