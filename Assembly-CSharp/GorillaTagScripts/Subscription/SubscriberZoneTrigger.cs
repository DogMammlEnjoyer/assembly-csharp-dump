using System;
using GorillaLocomotion;
using UnityEngine;

namespace GorillaTagScripts.Subscription
{
	public class SubscriberZoneTrigger : MonoBehaviour
	{
		private void OnTriggerEnter(Collider other)
		{
			if (GTPlayer.Instance != null && other == GTPlayer.Instance.bodyCollider && this.parentZone != null)
			{
				this.parentZone.OnZoneEnter(this.isRestrictedZone);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (GTPlayer.Instance != null && other == GTPlayer.Instance.bodyCollider && this.parentZone != null)
			{
				this.parentZone.OnZoneExit(this.isRestrictedZone);
			}
		}

		public SubscriberExclusiveZone parentZone;

		public bool isRestrictedZone;
	}
}
