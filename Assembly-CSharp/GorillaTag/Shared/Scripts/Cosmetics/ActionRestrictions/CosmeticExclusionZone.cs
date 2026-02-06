using System;
using UnityEngine;

namespace GorillaTag.Shared.Scripts.Cosmetics.ActionRestrictions
{
	[RequireComponent(typeof(Collider))]
	public class CosmeticExclusionZone : MonoBehaviour
	{
		private void Awake()
		{
			this.zoneCollider = base.GetComponent<Collider>();
			this.zoneCollider.isTrigger = true;
			CosmeticExclusionZoneRegistryUtility.RegisterZone(this.zoneCollider);
		}

		private void OnDestroy()
		{
			CosmeticExclusionZoneRegistryUtility.UnregisterZone(this.zoneCollider);
		}

		private void OnTriggerEnter(Collider other)
		{
			VRRig componentInParent = other.GetComponentInParent<VRRig>();
			if (componentInParent != null)
			{
				CosmeticExclusionZoneRegistry.Enter(componentInParent);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			VRRig componentInParent = other.GetComponentInParent<VRRig>();
			if (componentInParent != null)
			{
				CosmeticExclusionZoneRegistry.Exit(componentInParent);
			}
		}

		private Collider zoneCollider;
	}
}
