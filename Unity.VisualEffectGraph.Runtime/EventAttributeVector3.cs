using System;

namespace UnityEngine.VFX
{
	[Serializable]
	internal class EventAttributeVector3 : EventAttributeValue<Vector3>
	{
		public EventAttributeVector3() : base((VFXEventAttribute e, int id) => e.HasVector3(id), delegate(VFXEventAttribute e, int id, Vector3 value)
		{
			e.SetVector3(id, value);
		})
		{
		}
	}
}
