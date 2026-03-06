using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation
{
	[AddComponentMenu("XR/Teleportation Multi-Anchor Volume", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.TeleportationMultiAnchorVolume.html")]
	[MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
	public class TeleportationMultiAnchorVolume : BaseTeleportationInteractable
	{
		public List<Transform> anchorTransforms
		{
			get
			{
				return this.m_AnchorTransforms;
			}
		}

		public TeleportVolumeDestinationSettingsDatumProperty destinationEvaluationSettings
		{
			get
			{
				return this.m_DestinationEvaluationSettings;
			}
			set
			{
				this.m_DestinationEvaluationSettings = value;
			}
		}

		public ITeleportationVolumeAnchorFilter destinationEvaluationFilter
		{
			get
			{
				ITeleportationVolumeAnchorFilter destinationEvaluationFilter = this.m_DestinationEvaluationSettings.Value.destinationEvaluationFilter;
				if (destinationEvaluationFilter != null)
				{
					return destinationEvaluationFilter;
				}
				return this.m_DefaultAnchorFilterCache;
			}
		}

		public float destinationEvaluationProgress { get; private set; }

		public Transform destinationAnchor { get; private set; }

		public event Action<TeleportationMultiAnchorVolume> destinationAnchorChanged;

		private bool shouldDelayDestinationEvaluation
		{
			get
			{
				TeleportVolumeDestinationSettings value = this.m_DestinationEvaluationSettings.Value;
				return value.enableDestinationEvaluationDelay && value.destinationEvaluationDelayTime > 0f;
			}
		}

		protected void OnDrawGizmosSelected()
		{
			foreach (Transform transform in this.m_AnchorTransforms)
			{
				Gizmos.color = Color.blue;
				GizmoHelpers.DrawWireCubeOriented(transform.position, transform.rotation, 1f);
				GizmoHelpers.DrawAxisArrows(transform, 1f);
			}
		}

		protected override void Awake()
		{
			base.Awake();
			this.m_DefaultAnchorFilterCache = TeleportationMultiAnchorVolume.DefaultDestinationFilterCache.SubscribeAndGetInstance(this);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			TeleportationMultiAnchorVolume.DefaultDestinationFilterCache.Unsubscribe(this);
		}

		protected override void OnHoverEntered(HoverEnterEventArgs args)
		{
			base.OnHoverEntered(args);
			if (base.interactorsHovering.Count != 1)
			{
				return;
			}
			this.ClearDestinationAnchor();
			if (this.shouldDelayDestinationEvaluation)
			{
				this.m_WaitingToEvaluateDestination = true;
				this.m_WaitStartTime = Time.time;
				return;
			}
			this.EvaluateDestinationAnchor();
		}

		protected override void OnHoverExited(HoverExitEventArgs args)
		{
			base.OnHoverExited(args);
			if (!base.isHovered)
			{
				this.m_WaitingToEvaluateDestination = false;
				this.ClearDestinationAnchor();
			}
		}

		public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
		{
			base.ProcessInteractable(updatePhase);
			if (updatePhase != XRInteractionUpdateOrder.UpdatePhase.Dynamic || !base.isHovered)
			{
				return;
			}
			TeleportVolumeDestinationSettings value = this.m_DestinationEvaluationSettings.Value;
			if (this.m_WaitingToEvaluateDestination)
			{
				this.destinationEvaluationProgress = (Time.time - this.m_WaitStartTime) / value.destinationEvaluationDelayTime;
				if (this.destinationEvaluationProgress >= 1f)
				{
					this.m_WaitingToEvaluateDestination = false;
					this.EvaluateDestinationAnchor();
				}
				return;
			}
			if (value.pollForDestinationChange && Time.time - this.m_LastDestinationQueryTime > value.destinationPollFrequency)
			{
				this.m_LastDestinationQueryTime = Time.time;
				int destinationAnchorIndex = this.destinationEvaluationFilter.GetDestinationAnchorIndex(this);
				if (destinationAnchorIndex >= 0 && destinationAnchorIndex < this.m_AnchorTransforms.Count && this.m_AnchorTransforms[destinationAnchorIndex] == this.destinationAnchor)
				{
					return;
				}
				this.ClearDestinationAnchor();
				if (this.shouldDelayDestinationEvaluation)
				{
					this.m_WaitingToEvaluateDestination = true;
					this.m_WaitStartTime = Time.time;
					return;
				}
				this.destinationEvaluationProgress = 1f;
				if (destinationAnchorIndex >= 0 && destinationAnchorIndex < this.m_AnchorTransforms.Count)
				{
					this.SetDestinationAtValidIndex(destinationAnchorIndex);
				}
			}
		}

		private void EvaluateDestinationAnchor()
		{
			this.destinationEvaluationProgress = 1f;
			this.m_LastDestinationQueryTime = Time.time;
			int destinationAnchorIndex = this.destinationEvaluationFilter.GetDestinationAnchorIndex(this);
			if (destinationAnchorIndex >= 0 && destinationAnchorIndex < this.m_AnchorTransforms.Count)
			{
				this.SetDestinationAtValidIndex(destinationAnchorIndex);
			}
		}

		private void SetDestinationAtValidIndex(int anchorIndex)
		{
			this.destinationAnchor = this.m_AnchorTransforms[anchorIndex];
			Action<TeleportationMultiAnchorVolume> action = this.destinationAnchorChanged;
			if (action == null)
			{
				return;
			}
			action(this);
		}

		private void ClearDestinationAnchor()
		{
			this.destinationAnchor = null;
			this.destinationEvaluationProgress = 0f;
			Action<TeleportationMultiAnchorVolume> action = this.destinationAnchorChanged;
			if (action == null)
			{
				return;
			}
			action(this);
		}

		public override Transform GetAttachTransform(IXRInteractor interactor)
		{
			if (!(this.destinationAnchor != null))
			{
				return base.GetAttachTransform(interactor);
			}
			return this.destinationAnchor;
		}

		protected override bool GenerateTeleportRequest(IXRInteractor interactor, RaycastHit raycastHit, ref TeleportRequest teleportRequest)
		{
			if (this.destinationAnchor == null)
			{
				return false;
			}
			Pose worldPose = this.destinationAnchor.GetWorldPose();
			teleportRequest.destinationPosition = worldPose.position;
			teleportRequest.destinationRotation = worldPose.rotation;
			this.ClearDestinationAnchor();
			return true;
		}

		[SerializeField]
		[Tooltip("The transforms that represent the possible teleportation destinations.")]
		private List<Transform> m_AnchorTransforms = new List<Transform>();

		[SerializeField]
		[Tooltip("Settings for how this volume evaluates a destination anchor.")]
		private TeleportVolumeDestinationSettingsDatumProperty m_DestinationEvaluationSettings = new TeleportVolumeDestinationSettingsDatumProperty(new TeleportVolumeDestinationSettings());

		private ITeleportationVolumeAnchorFilter m_DefaultAnchorFilterCache;

		private bool m_WaitingToEvaluateDestination;

		private float m_WaitStartTime;

		private float m_LastDestinationQueryTime;

		private static class DefaultDestinationFilterCache
		{
			public static ITeleportationVolumeAnchorFilter SubscribeAndGetInstance(TeleportationMultiAnchorVolume user)
			{
				TeleportationMultiAnchorVolume.DefaultDestinationFilterCache.s_Users.Add(user);
				if (TeleportationMultiAnchorVolume.DefaultDestinationFilterCache.s_FilterInstance == null)
				{
					TeleportationMultiAnchorVolume.DefaultDestinationFilterCache.s_FilterInstance = ScriptableObject.CreateInstance<FurthestTeleportationAnchorFilter>();
				}
				return TeleportationMultiAnchorVolume.DefaultDestinationFilterCache.s_FilterInstance;
			}

			public static void Unsubscribe(TeleportationMultiAnchorVolume user)
			{
				TeleportationMultiAnchorVolume.DefaultDestinationFilterCache.s_Users.Remove(user);
				if (TeleportationMultiAnchorVolume.DefaultDestinationFilterCache.s_Users.Count == 0)
				{
					Object.Destroy(TeleportationMultiAnchorVolume.DefaultDestinationFilterCache.s_FilterInstance);
				}
			}

			private static FurthestTeleportationAnchorFilter s_FilterInstance;

			private static readonly HashSet<TeleportationMultiAnchorVolume> s_Users = new HashSet<TeleportationMultiAnchorVolume>();
		}
	}
}
