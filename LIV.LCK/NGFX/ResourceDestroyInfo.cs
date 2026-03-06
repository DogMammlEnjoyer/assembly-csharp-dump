using System;
using System.Runtime.InteropServices;

namespace Liv.NGFX
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct ResourceDestroyInfo
	{
		public ResourceDestroyInfo(IntPtr ctx, uint id)
		{
			this.m_context = ctx;
			this.m_id = id;
		}

		public static EventType eventType = EventType.ResourceDestroy;

		private IntPtr m_context;

		private uint m_id;
	}
}
