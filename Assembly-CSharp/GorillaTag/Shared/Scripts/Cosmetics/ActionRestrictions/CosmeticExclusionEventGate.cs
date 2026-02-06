using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Shared.Scripts.Cosmetics.ActionRestrictions
{
	public class CosmeticExclusionEventGate : MonoBehaviour
	{
		private void Awake()
		{
			this.ownerRig = base.GetComponentInParent<VRRig>();
		}

		public void InvokeEvent()
		{
			if (CosmeticExclusionQuery.IsRestricted(this.ownerRig, this.effectSource))
			{
				UnityEvent unityEvent = this.onRestricted;
				if (unityEvent == null)
				{
					return;
				}
				unityEvent.Invoke();
				return;
			}
			else
			{
				UnityEvent unityEvent2 = this.onNormal;
				if (unityEvent2 == null)
				{
					return;
				}
				unityEvent2.Invoke();
				return;
			}
		}

		[Header("Context")]
		[Tooltip("Optional effect source.\nIf set and has CosmeticExclusionSource, world position will be checked.")]
		[SerializeField]
		private GameObject effectSource;

		[Header("Forwarded Events")]
		[SerializeField]
		private UnityEvent onNormal;

		[SerializeField]
		private UnityEvent onRestricted;

		private VRRig ownerRig;
	}
}
