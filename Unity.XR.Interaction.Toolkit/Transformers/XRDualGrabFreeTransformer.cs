using System;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace UnityEngine.XR.Interaction.Toolkit.Transformers
{
	[AddComponentMenu("XR/Transformers/XR Dual Grab Free Transformer", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Transformers.XRDualGrabFreeTransformer.html")]
	public class XRDualGrabFreeTransformer : XRBaseGrabTransformer
	{
		public XRDualGrabFreeTransformer.PoseContributor multiSelectPosition
		{
			get
			{
				return this.m_MultiSelectPosition;
			}
			set
			{
				this.m_MultiSelectPosition = value;
			}
		}

		public XRDualGrabFreeTransformer.PoseContributor multiSelectRotation
		{
			get
			{
				return this.m_MultiSelectRotation;
			}
			set
			{
				this.m_MultiSelectRotation = value;
			}
		}

		protected override XRBaseGrabTransformer.RegistrationMode registrationMode
		{
			get
			{
				return XRBaseGrabTransformer.RegistrationMode.Multiple;
			}
		}

		internal Pose lastInteractorAttachPose { get; private set; }

		protected virtual void OnDrawGizmosSelected()
		{
		}

		public override void OnGrabCountChanged(XRGrabInteractable grabInteractable, Pose targetPose, Vector3 localScale)
		{
			base.OnGrabCountChanged(grabInteractable, targetPose, localScale);
			if (grabInteractable.interactorsSelecting.Count == 2)
			{
				this.m_LastUp = grabInteractable.transform.up;
			}
		}

		public override void Process(XRGrabInteractable grabInteractable, XRInteractionUpdateOrder.UpdatePhase updatePhase, ref Pose targetPose, ref Vector3 localScale)
		{
			if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic || updatePhase == XRInteractionUpdateOrder.UpdatePhase.OnBeforeRender)
			{
				this.UpdateTarget(grabInteractable, ref targetPose);
			}
		}

		private void UpdateTarget(XRGrabInteractable grabInteractable, ref Pose targetPose)
		{
			if (grabInteractable.interactorsSelecting.Count == 1)
			{
				XRSingleGrabFreeTransformer.UpdateTarget(grabInteractable, ref targetPose);
				return;
			}
			this.UpdateTargetMulti(grabInteractable, ref targetPose);
		}

		private void UpdateTargetMulti(XRGrabInteractable grabInteractable, ref Pose targetPose)
		{
			Pose worldPose = grabInteractable.interactorsSelecting[0].GetAttachTransform(grabInteractable).GetWorldPose();
			Pose worldPose2 = grabInteractable.interactorsSelecting[1].GetAttachTransform(grabInteractable).GetWorldPose();
			Pose pose = worldPose;
			switch (this.m_MultiSelectPosition)
			{
			default:
				pose.position = worldPose.position;
				break;
			case XRDualGrabFreeTransformer.PoseContributor.Second:
				pose.position = worldPose2.position;
				break;
			case XRDualGrabFreeTransformer.PoseContributor.Average:
				pose.position = (worldPose.position + worldPose2.position) * 0.5f;
				break;
			}
			Vector3 vector = (worldPose2.position - worldPose.position).normalized;
			Vector3 vector2;
			Vector3 rhs;
			switch (this.m_MultiSelectRotation)
			{
			default:
				vector2 = worldPose.up;
				rhs = worldPose.right;
				if (vector == Vector3.zero)
				{
					vector = worldPose.forward;
				}
				break;
			case XRDualGrabFreeTransformer.PoseContributor.Second:
				vector2 = worldPose2.up;
				rhs = worldPose2.right;
				if (vector == Vector3.zero)
				{
					vector = worldPose2.forward;
				}
				break;
			case XRDualGrabFreeTransformer.PoseContributor.Average:
				vector2 = Vector3.Slerp(worldPose.up, worldPose2.up, 0.5f);
				rhs = Vector3.Slerp(worldPose.right, worldPose2.right, 0.5f);
				if (vector == Vector3.zero)
				{
					vector = worldPose.forward;
				}
				break;
			}
			Vector3 a = Vector3.Cross(vector, rhs);
			float num = Mathf.PingPong(Vector3.Angle(vector2, vector), 90f);
			vector2 = Vector3.Slerp(a, vector2, num / 90f);
			Vector3 rhs2 = Vector3.Cross(vector2, vector);
			vector2 = Vector3.Cross(vector, rhs2);
			if (Vector3.Dot(vector2, this.m_LastUp) <= 0f)
			{
				vector2 = -vector2;
			}
			this.m_LastUp = vector2;
			pose.rotation = Quaternion.LookRotation(vector, vector2);
			this.lastInteractorAttachPose = pose;
			if (!grabInteractable.trackRotation)
			{
				targetPose.position = pose.position;
				return;
			}
			if (this.m_MultiSelectRotation == XRDualGrabFreeTransformer.PoseContributor.First || this.m_MultiSelectRotation == XRDualGrabFreeTransformer.PoseContributor.Second)
			{
				int index = (this.m_MultiSelectRotation == XRDualGrabFreeTransformer.PoseContributor.First) ? 0 : 1;
				Transform attachTransform = grabInteractable.GetAttachTransform(grabInteractable.interactorsSelecting[index]);
				Vector3 direction = grabInteractable.transform.GetWorldPose().position - attachTransform.position;
				Vector3 point = attachTransform.InverseTransformDirection(direction);
				targetPose.position = pose.rotation * point + pose.position;
			}
			else if (this.m_MultiSelectRotation == XRDualGrabFreeTransformer.PoseContributor.Average)
			{
				targetPose.position = pose.position;
			}
			targetPose.rotation = pose.rotation;
		}

		[SerializeField]
		private XRDualGrabFreeTransformer.PoseContributor m_MultiSelectPosition;

		[SerializeField]
		private XRDualGrabFreeTransformer.PoseContributor m_MultiSelectRotation = XRDualGrabFreeTransformer.PoseContributor.Average;

		private Vector3 m_LastUp;

		public enum PoseContributor
		{
			First,
			Second,
			Average
		}
	}
}
