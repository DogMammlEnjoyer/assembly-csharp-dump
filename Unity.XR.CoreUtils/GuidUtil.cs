using System;

namespace Unity.XR.CoreUtils
{
	public static class GuidUtil
	{
		public static Guid Compose(ulong low, ulong high)
		{
			return new Guid((uint)(low & (ulong)-1), (ushort)((low & 281470681743360UL) >> 32), (ushort)((low & 18446462598732840960UL) >> 48), (byte)(high & 255UL), (byte)((high & 65280UL) >> 8), (byte)((high & 16711680UL) >> 16), (byte)((high & (ulong)-16777216) >> 24), (byte)((high & 1095216660480UL) >> 32), (byte)((high & 280375465082880UL) >> 40), (byte)((high & 71776119061217280UL) >> 48), (byte)((high & 18374686479671623680UL) >> 56));
		}
	}
}
