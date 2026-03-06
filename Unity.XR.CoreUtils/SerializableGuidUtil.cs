using System;

namespace Unity.XR.CoreUtils
{
	public static class SerializableGuidUtil
	{
		public static SerializableGuid Create(Guid guid)
		{
			ulong guidLow;
			ulong guidHigh;
			guid.Decompose(out guidLow, out guidHigh);
			return new SerializableGuid(guidLow, guidHigh);
		}
	}
}
