using System;
using UnityEngine;
using UnityEngine.Events;

namespace Oculus.Interaction.Samples
{
	public class LocomotionTutorialAnimationUnityEventWrapper : MonoBehaviour
	{
		public void EnableTeleportRay()
		{
			this.WhenEnableTeleportRay.Invoke();
		}

		public void DisableTeleportRay()
		{
			this.WhenDisableTeleportRay.Invoke();
		}

		public void EnableTurningRing()
		{
			this.WhenEnableTurningRing.Invoke();
		}

		public void DisableTurningRing()
		{
			this.WhenDisableTurningRing.Invoke();
		}

		public UnityEvent WhenEnableTeleportRay;

		public UnityEvent WhenDisableTeleportRay;

		public UnityEvent WhenEnableTurningRing;

		public UnityEvent WhenDisableTurningRing;
	}
}
