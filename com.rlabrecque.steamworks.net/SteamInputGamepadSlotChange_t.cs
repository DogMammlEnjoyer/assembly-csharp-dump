using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(2804)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct SteamInputGamepadSlotChange_t
	{
		public const int k_iCallback = 2804;

		public AppId_t m_unAppID;

		public InputHandle_t m_ulDeviceHandle;

		public ESteamInputType m_eDeviceType;

		public int m_nOldGamepadSlot;

		public int m_nNewGamepadSlot;
	}
}
