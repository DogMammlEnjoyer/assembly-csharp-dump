using System;
using UnityEngine.Events;

namespace GorillaTag.Shared.Scripts.Cosmetics.ActionRestrictions
{
	[Serializable]
	public class ExclusionZoneStateEvent<T0, T1> : ZoneStateEventBase
	{
		public void Invoke(VRRig vrRig, T0 arg0, T1 arg1)
		{
			if (base.IsRestricted(vrRig))
			{
				ExclusionZoneStateEvent<T0, T1>.TypedEvent typedEvent = this.onRestricted;
				if (typedEvent == null)
				{
					return;
				}
				typedEvent.Invoke(arg0, arg1);
				return;
			}
			else
			{
				ExclusionZoneStateEvent<T0, T1>.TypedEvent typedEvent2 = this.onNormal;
				if (typedEvent2 == null)
				{
					return;
				}
				typedEvent2.Invoke(arg0, arg1);
				return;
			}
		}

		public ExclusionZoneStateEvent<T0, T1>.TypedEvent onNormal;

		public ExclusionZoneStateEvent<T0, T1>.TypedEvent onRestricted;

		[Serializable]
		public class TypedEvent : UnityEvent<T0, T1>
		{
		}
	}
}
