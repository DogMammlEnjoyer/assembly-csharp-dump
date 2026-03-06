using System;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	public class BalloonHapticBump : MonoBehaviour
	{
		private void OnCollisionEnter(Collision other)
		{
			if (other.collider.GetComponentInParent<Balloon>() != null)
			{
				Hand componentInParent = this.physParent.GetComponentInParent<Hand>();
				if (componentInParent != null)
				{
					componentInParent.TriggerHapticPulse(500);
				}
			}
		}

		public GameObject physParent;
	}
}
