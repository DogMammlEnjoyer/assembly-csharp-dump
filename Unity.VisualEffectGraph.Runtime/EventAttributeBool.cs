using System;

namespace UnityEngine.VFX
{
	[Serializable]
	internal class EventAttributeBool : EventAttributeValue<bool>
	{
		public EventAttributeBool() : base((VFXEventAttribute e, int id) => e.HasBool(id), delegate(VFXEventAttribute e, int id, bool value)
		{
			e.SetBool(id, value);
		})
		{
		}
	}
}
