using System;
using Unity.XR.CoreUtils;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation
{
	[AddComponentMenu("XR/Teleportation Anchor", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.TeleportationAnchor.html")]
	[MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
	public class TeleportationAnchor : BaseTeleportationInteractable
	{
		public Transform teleportAnchorTransform
		{
			get
			{
				return this.m_TeleportAnchorTransform;
			}
			set
			{
				this.m_TeleportAnchorTransform = value;
			}
		}

		protected void OnValidate()
		{
			if (this.m_TeleportAnchorTransform == null)
			{
				this.m_TeleportAnchorTransform = base.transform;
			}
		}

		protected override void Reset()
		{
			this.m_TeleportAnchorTransform = base.transform;
		}

		protected void OnDrawGizmos()
		{
			if (this.m_TeleportAnchorTransform == null)
			{
				return;
			}
			Gizmos.color = Color.blue;
			GizmoHelpers.DrawWireCubeOriented(this.m_TeleportAnchorTransform.position, this.m_TeleportAnchorTransform.rotation, 1f);
			GizmoHelpers.DrawAxisArrows(this.m_TeleportAnchorTransform, 1f);
		}

		public override Transform GetAttachTransform(IXRInteractor interactor)
		{
			return this.m_TeleportAnchorTransform;
		}

		public void RequestTeleport()
		{
			base.SendTeleportRequest(null);
		}

		protected override bool GenerateTeleportRequest(IXRInteractor interactor, RaycastHit raycastHit, ref TeleportRequest teleportRequest)
		{
			if (this.m_TeleportAnchorTransform == null)
			{
				return false;
			}
			Pose worldPose = this.m_TeleportAnchorTransform.GetWorldPose();
			teleportRequest.destinationPosition = worldPose.position;
			teleportRequest.destinationRotation = worldPose.rotation;
			return true;
		}

		[ContextMenu("Teleport to anchor", false)]
		private void RequestTeleportFromEditor()
		{
			this.RequestTeleport();
		}

		[ContextMenu("Teleport to anchor", true)]
		private bool RequestTeleportFromEditorValidate()
		{
			return Application.isPlaying;
		}

		[SerializeField]
		[Tooltip("The Transform that represents the teleportation destination.")]
		private Transform m_TeleportAnchorTransform;
	}
}
