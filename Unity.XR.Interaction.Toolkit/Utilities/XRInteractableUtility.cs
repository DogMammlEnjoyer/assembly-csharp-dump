using System;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace UnityEngine.XR.Interaction.Toolkit.Utilities
{
	public static class XRInteractableUtility
	{
		private static bool allowTriggerColliders { get; set; }

		public static bool TryGetClosestCollider(IXRInteractable interactable, Vector3 position, out DistanceInfo distanceInfo)
		{
			Vector3 point = default(Vector3);
			float num = float.MaxValue;
			Collider collider = null;
			bool result = false;
			foreach (Collider collider2 in interactable.colliders)
			{
				if (!(collider2 == null) && collider2.gameObject.activeInHierarchy && collider2.enabled && (!collider2.isTrigger || XRInteractableUtility.allowTriggerColliders))
				{
					Vector3 position2 = collider2.transform.position;
					float sqrMagnitude = (position - position2).sqrMagnitude;
					if (sqrMagnitude < num)
					{
						result = true;
						num = sqrMagnitude;
						point = position2;
						collider = collider2;
					}
				}
			}
			distanceInfo = new DistanceInfo
			{
				point = point,
				distanceSqr = num,
				collider = collider
			};
			return result;
		}

		public static bool TryGetClosestPointOnCollider(IXRInteractable interactable, Vector3 position, out DistanceInfo distanceInfo)
		{
			Vector3 point = default(Vector3);
			Collider collider = null;
			float num = float.MaxValue;
			bool result = false;
			foreach (Collider collider2 in interactable.colliders)
			{
				if (!(collider2 == null) && collider2.gameObject.activeInHierarchy && collider2.enabled && (!collider2.isTrigger || XRInteractableUtility.allowTriggerColliders))
				{
					Vector3 vector = collider2.ClosestPoint(position);
					float sqrMagnitude = (position - vector).sqrMagnitude;
					if (sqrMagnitude < num)
					{
						result = true;
						num = sqrMagnitude;
						point = vector;
						collider = collider2;
					}
				}
			}
			distanceInfo = new DistanceInfo
			{
				point = point,
				distanceSqr = num,
				collider = collider
			};
			return result;
		}

		internal struct AllowTriggerCollidersScope : IDisposable
		{
			public AllowTriggerCollidersScope(bool newAllowTriggerColliders)
			{
				this.m_Disposed = false;
				this.m_OldValue = XRInteractableUtility.allowTriggerColliders;
				XRInteractableUtility.allowTriggerColliders = newAllowTriggerColliders;
			}

			public void Dispose()
			{
				if (this.m_Disposed)
				{
					return;
				}
				this.m_Disposed = true;
				XRInteractableUtility.allowTriggerColliders = this.m_OldValue;
			}

			private bool m_Disposed;

			private readonly bool m_OldValue;
		}
	}
}
