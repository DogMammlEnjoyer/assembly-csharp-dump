using System;

namespace UnityEngine.VFX
{
	[Serializable]
	internal class EventAttributeUInt : EventAttributeValue<uint>
	{
		public EventAttributeUInt() : base((VFXEventAttribute e, int id) => e.HasUint(id), delegate(VFXEventAttribute e, int id, uint value)
		{
			e.SetUint(id, value);
		})
		{
		}
	}
}
