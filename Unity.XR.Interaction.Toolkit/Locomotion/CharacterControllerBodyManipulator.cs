using System;
using Unity.XR.CoreUtils;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion
{
	[CreateAssetMenu(fileName = "CharacterControllerBodyManipulator", menuName = "XR/Locomotion/Character Controller Body Manipulator")]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Locomotion.CharacterControllerBodyManipulator.html")]
	public class CharacterControllerBodyManipulator : ScriptableConstrainedBodyManipulator
	{
		public override CollisionFlags lastCollisionFlags
		{
			get
			{
				if (!(this.characterController != null))
				{
					return CollisionFlags.None;
				}
				return this.characterController.collisionFlags;
			}
		}

		public override bool isGrounded
		{
			get
			{
				return this.characterController == null || this.characterController.isGrounded;
			}
		}

		public CharacterController characterController { get; private set; }

		public override void OnLinkedToBody(XRMovableBody body)
		{
			base.OnLinkedToBody(body);
			XROrigin xrOrigin = body.xrOrigin;
			GameObject origin = xrOrigin.Origin;
			CharacterController characterController;
			if (!origin.TryGetComponent<CharacterController>(out characterController) && origin != xrOrigin.gameObject)
			{
				xrOrigin.TryGetComponent<CharacterController>(out characterController);
			}
			if (characterController != null)
			{
				this.characterController = characterController;
				return;
			}
			Debug.LogWarning("No CharacterController found. Adding one to Origin GameObject '" + origin.name + "'.", this);
			this.characterController = origin.AddComponent<CharacterController>();
		}

		public override void OnUnlinkedFromBody()
		{
			base.OnUnlinkedFromBody();
			this.characterController = null;
		}

		public override CollisionFlags MoveBody(Vector3 motion)
		{
			if (base.linkedBody == null || this.characterController == null)
			{
				return CollisionFlags.None;
			}
			XROrigin xrOrigin = base.linkedBody.xrOrigin;
			Vector3 bodyGroundLocalPosition = base.linkedBody.GetBodyGroundLocalPosition();
			float num = xrOrigin.CameraInOriginSpaceHeight - bodyGroundLocalPosition.y;
			this.characterController.height = num;
			this.characterController.center = new Vector3(bodyGroundLocalPosition.x, bodyGroundLocalPosition.y + num * 0.5f + this.characterController.skinWidth, bodyGroundLocalPosition.z);
			return this.characterController.Move(motion);
		}
	}
}
