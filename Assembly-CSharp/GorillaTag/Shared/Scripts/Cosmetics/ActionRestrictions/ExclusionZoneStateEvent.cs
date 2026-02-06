using System;
using UnityEngine.Events;

namespace GorillaTag.Shared.Scripts.Cosmetics.ActionRestrictions
{
	[Serializable]
	public class ExclusionZoneStateEvent
	{
		public void Invoke(VRRig vrRig)
		{
			if (CosmeticExclusionZoneRegistry.IsRestricted(vrRig))
			{
				UnityEvent onRestricted = this.OnRestricted;
				if (onRestricted == null)
				{
					return;
				}
				onRestricted.Invoke();
				return;
			}
			else
			{
				UnityEvent onNormal = this.OnNormal;
				if (onNormal == null)
				{
					return;
				}
				onNormal.Invoke();
				return;
			}
		}

		public UnityEvent OnNormal;

		public UnityEvent OnRestricted;
	}
}
