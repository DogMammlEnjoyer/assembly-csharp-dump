using System;
using UnityEngine.Events;

namespace GorillaTag.Shared.Scripts.Cosmetics.ActionRestrictions
{
	[Serializable]
	public class ExclusionZoneStateEvent<T> : ZoneStateEventBase
	{
		public void Invoke(VRRig vrRig, T arg)
		{
			if (base.IsRestricted(vrRig))
			{
				ExclusionZoneStateEvent<T>.TypedEvent typedEvent = this.onRestricted;
				if (typedEvent == null)
				{
					return;
				}
				typedEvent.Invoke(arg);
				return;
			}
			else
			{
				ExclusionZoneStateEvent<T>.TypedEvent typedEvent2 = this.onNormal;
				if (typedEvent2 == null)
				{
					return;
				}
				typedEvent2.Invoke(arg);
				return;
			}
		}

		public ExclusionZoneStateEvent<T>.TypedEvent onNormal;

		public ExclusionZoneStateEvent<T>.TypedEvent onRestricted;

		[Serializable]
		public class TypedEvent : UnityEvent<T>
		{
		}
	}
}
