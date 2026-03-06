using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public static class RigidbodyKinematicLockerExtension
	{
		public static bool IsLocked(this Rigidbody rigidbody)
		{
			RigidbodyKinematicLocker rigidbodyKinematicLocker;
			return rigidbody.TryGetComponent<RigidbodyKinematicLocker>(out rigidbodyKinematicLocker) && rigidbodyKinematicLocker.IsLocked;
		}

		public static void LockKinematic(this Rigidbody rigidbody)
		{
			RigidbodyKinematicLocker rigidbodyKinematicLocker;
			if (!rigidbody.TryGetComponent<RigidbodyKinematicLocker>(out rigidbodyKinematicLocker))
			{
				rigidbodyKinematicLocker = rigidbody.gameObject.AddComponent<RigidbodyKinematicLocker>();
			}
			rigidbodyKinematicLocker.LockKinematic();
		}

		public static void UnlockKinematic(this Rigidbody rigidbody)
		{
			RigidbodyKinematicLocker rigidbodyKinematicLocker;
			if (!rigidbody.TryGetComponent<RigidbodyKinematicLocker>(out rigidbodyKinematicLocker))
			{
				Debug.LogError("Too many calls to UnlockKinematic.Expected calls to LockKinematic to balance the kinematic state.", rigidbody);
				return;
			}
			rigidbodyKinematicLocker.UnlockKinematic();
		}
	}
}
