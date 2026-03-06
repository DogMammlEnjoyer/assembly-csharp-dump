using System;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation
{
	[AddComponentMenu("XR/Locomotion/Teleportation Provider", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.TeleportationProvider.html")]
	[MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
	public class TeleportationProvider : LocomotionProvider
	{
		protected TeleportRequest currentRequest { get; set; }

		protected bool validRequest { get; set; }

		public float delayTime
		{
			get
			{
				return this.m_DelayTime;
			}
			set
			{
				this.m_DelayTime = value;
			}
		}

		public override bool canStartMoving
		{
			get
			{
				return this.m_DelayTime <= 0f || Time.time - this.m_DelayStartTime >= this.m_DelayTime;
			}
		}

		public virtual bool QueueTeleportRequest(TeleportRequest teleportRequest)
		{
			this.currentRequest = teleportRequest;
			this.validRequest = true;
			return true;
		}

		public XROriginUpAlignment upTransformation { get; set; } = new XROriginUpAlignment();

		public XRCameraForwardXZAlignment forwardTransformation { get; set; } = new XRCameraForwardXZAlignment();

		public XRBodyGroundPosition positionTransformation { get; set; } = new XRBodyGroundPosition();

		protected virtual void Update()
		{
			if (!this.validRequest)
			{
				return;
			}
			if (base.locomotionState == LocomotionState.Idle)
			{
				if (this.m_DelayTime > 0f)
				{
					if (base.TryPrepareLocomotion())
					{
						this.m_DelayStartTime = Time.time;
					}
				}
				else
				{
					base.TryStartLocomotionImmediately();
				}
			}
			if (base.locomotionState == LocomotionState.Moving)
			{
				switch (this.currentRequest.matchOrientation)
				{
				case MatchOrientation.WorldSpaceUp:
					this.upTransformation.targetUp = Vector3.up;
					base.TryQueueTransformation(this.upTransformation);
					break;
				case MatchOrientation.TargetUp:
					this.upTransformation.targetUp = this.currentRequest.destinationRotation * Vector3.up;
					base.TryQueueTransformation(this.upTransformation);
					break;
				case MatchOrientation.TargetUpAndForward:
					this.upTransformation.targetUp = this.currentRequest.destinationRotation * Vector3.up;
					base.TryQueueTransformation(this.upTransformation);
					this.forwardTransformation.targetDirection = this.currentRequest.destinationRotation * Vector3.forward;
					base.TryQueueTransformation(this.forwardTransformation);
					break;
				}
				this.positionTransformation.targetPosition = this.currentRequest.destinationPosition;
				base.TryQueueTransformation(this.positionTransformation);
				base.TryEndLocomotion();
				this.validRequest = false;
			}
		}

		[SerializeField]
		[Tooltip("The time (in seconds) to delay the teleportation once it is activated.")]
		private float m_DelayTime;

		private float m_DelayStartTime;
	}
}
