using System;

namespace UnityEngine.VFX
{
	[Serializable]
	internal class EventAttributeFloat : EventAttributeValue<float>
	{
		public EventAttributeFloat() : base((VFXEventAttribute e, int id) => e.HasFloat(id), delegate(VFXEventAttribute e, int id, float value)
		{
			e.SetFloat(id, value);
		})
		{
		}
	}
}
