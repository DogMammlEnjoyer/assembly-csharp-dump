using System;

namespace UnityEngine.VFX
{
	[Serializable]
	internal class EventAttributeVector2 : EventAttributeValue<Vector2>
	{
		public EventAttributeVector2() : base((VFXEventAttribute e, int id) => e.HasVector2(id), delegate(VFXEventAttribute e, int id, Vector2 value)
		{
			e.SetVector2(id, value);
		})
		{
		}
	}
}
