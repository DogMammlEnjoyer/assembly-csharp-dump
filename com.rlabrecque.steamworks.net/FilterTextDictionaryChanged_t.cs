using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(739)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct FilterTextDictionaryChanged_t
	{
		public const int k_iCallback = 739;

		public int m_eLanguage;
	}
}
