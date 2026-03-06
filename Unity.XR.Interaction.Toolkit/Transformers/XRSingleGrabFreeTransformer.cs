using System;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace UnityEngine.XR.Interaction.Toolkit.Transformers
{
	[AddComponentMenu("XR/Transformers/XR Single Grab Free Transformer", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Transformers.XRSingleGrabFreeTransformer.html")]
	public class XRSingleGrabFreeTransformer : XRBaseGrabTransformer
	{
		public override void Process(XRGrabInteractable grabInteractable, XRInteractionUpdateOrder.UpdatePhase updatePhase, ref Pose targetPose, ref Vector3 localScale)
		{
			if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic || updatePhase == XRInteractionUpdateOrder.UpdatePhase.OnBeforeRender)
			{
				XRSingleGrabFreeTransformer.UpdateTarget(grabInteractable, ref targetPose);
			}
		}

		internal static void UpdateTarget(XRGrabInteractable grabInteractable, ref Pose targetPose)
		{
			IXRSelectInteractor ixrselectInteractor = grabInteractable.interactorsSelecting[0];
			Pose worldPose = ixrselectInteractor.GetAttachTransform(grabInteractable).GetWorldPose();
			Pose worldPose2 = grabInteractable.transform.GetWorldPose();
			Transform attachTransform = grabInteractable.GetAttachTransform(ixrselectInteractor);
			Vector3 vector = worldPose2.position - attachTransform.position;
			if (grabInteractable.trackRotation)
			{
				Vector3 point = attachTransform.InverseTransformDirection(vector);
				Quaternion rhs = Quaternion.Inverse(Quaternion.Inverse(worldPose2.rotation) * attachTransform.rotation);
				targetPose.position = worldPose.rotation * point + worldPose.position;
				targetPose.rotation = worldPose.rotation * rhs;
				return;
			}
			targetPose.position = vector + worldPose.position;
		}
	}
}
