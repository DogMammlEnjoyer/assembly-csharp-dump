using System;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation
{
	[AddComponentMenu("XR/Teleportation Area", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.TeleportationArea.html")]
	[MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
	public class TeleportationArea : BaseTeleportationInteractable
	{
		protected override bool GenerateTeleportRequest(IXRInteractor interactor, RaycastHit raycastHit, ref TeleportRequest teleportRequest)
		{
			if (raycastHit.collider == null)
			{
				return false;
			}
			XRRayInteractor xrrayInteractor;
			if (TeleportationArea.IsSphereCastRay(interactor, out xrrayInteractor) && TeleportationArea.IsSphereCastOverlap(raycastHit))
			{
				return false;
			}
			teleportRequest.destinationPosition = raycastHit.point;
			teleportRequest.destinationRotation = base.transform.rotation;
			return true;
		}

		public override bool IsSelectableBy(IXRSelectInteractor interactor)
		{
			bool flag = base.IsSelectableBy(interactor);
			XRRayInteractor xrrayInteractor;
			RaycastHit raycastHit;
			return (!flag || !TeleportationArea.IsSphereCastRay(interactor, out xrrayInteractor) || !xrrayInteractor.TryGetCurrent3DRaycastHit(out raycastHit) || !TeleportationArea.IsSphereCastOverlap(raycastHit)) && flag;
		}

		private static bool IsSphereCastRay(IXRInteractor interactor, out XRRayInteractor rayInteractor)
		{
			rayInteractor = (interactor as XRRayInteractor);
			return rayInteractor != null && rayInteractor.hitDetectionType == XRRayInteractor.HitDetectionType.SphereCast;
		}

		private static bool IsSphereCastOverlap(RaycastHit raycastHit)
		{
			if (raycastHit.distance != 0f)
			{
				return false;
			}
			Vector3 point = raycastHit.point;
			return point.x == 0f && point.y == 0f && point.z == 0f;
		}
	}
}
