using System;

namespace UnityEngine.VFX
{
	[Serializable]
	internal class EventAttributeInt : EventAttributeValue<int>
	{
		public EventAttributeInt() : base((VFXEventAttribute e, int id) => e.HasInt(id), delegate(VFXEventAttribute e, int id, int value)
		{
			e.SetInt(id, value);
		})
		{
		}
	}
}
