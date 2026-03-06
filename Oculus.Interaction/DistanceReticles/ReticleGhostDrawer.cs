using System;
using Oculus.Interaction.HandGrab;
using Oculus.Interaction.Input;
using UnityEngine;
using UnityEngine.Serialization;

namespace Oculus.Interaction.DistanceReticles
{
	public class ReticleGhostDrawer : InteractorReticle<ReticleDataGhost>
	{
		private IHandGrabInteractor HandGrabInteractor { get; set; }

		protected override IInteractorView Interactor { get; set; }

		protected override Component InteractableComponent
		{
			get
			{
				return this.HandGrabInteractor.TargetInteractable as Component;
			}
		}

		protected virtual void Awake()
		{
			this.HandVisual = (this._handVisual as IHandVisual);
			this.HandGrabInteractor = (this._handGrabInteractor as IHandGrabInteractor);
			this.Interactor = (this._handGrabInteractor as IInteractorView);
		}

		protected override void Start()
		{
			this.BeginStart(ref this._started, delegate
			{
				base.Start();
			});
			this.Transformer = this._syntheticHand.GetData().Config.TrackingToWorldTransformer;
			this.Hide();
			this.EndStart(ref this._started);
		}

		private void UpdateHandPose(IHandGrabState snapper)
		{
			HandGrabTarget handGrabTarget = snapper.HandGrabTarget;
			if (handGrabTarget == null)
			{
				this.FreeFingers();
				this.FreeWrist();
				return;
			}
			if (handGrabTarget.HandPose != null)
			{
				this.UpdateFingers(handGrabTarget.HandPose, snapper.GrabbingFingers());
				this._areFingersFree = false;
			}
			else
			{
				this.FreeFingers();
			}
			Pose visualWristPose = snapper.GetVisualWristPose();
			Pose wristPose = (this.Transformer != null) ? this.Transformer.ToTrackingPose(visualWristPose) : visualWristPose;
			this._syntheticHand.LockWristPose(wristPose, 1f, SyntheticHand.WristLockMode.Full, false, false);
			this._isWristFree = false;
		}

		private void UpdateFingers(HandPose handPose, HandFingerFlags grabbingFingers)
		{
			Quaternion[] jointRotations = handPose.JointRotations;
			this._syntheticHand.OverrideAllJoints(jointRotations, 1f);
			for (int i = 0; i < 5; i++)
			{
				int num = 1 << i;
				JointFreedom jointFreedom = handPose.FingersFreedom[i];
				if (jointFreedom == JointFreedom.Constrained && (grabbingFingers & (HandFingerFlags)num) != HandFingerFlags.None)
				{
					jointFreedom = JointFreedom.Locked;
				}
				SyntheticHand syntheticHand = this._syntheticHand;
				HandFinger handFinger = (HandFinger)i;
				syntheticHand.SetFingerFreedom(handFinger, jointFreedom, false);
			}
		}

		private bool FreeFingers()
		{
			if (!this._areFingersFree)
			{
				this._syntheticHand.FreeAllJoints();
				this._areFingersFree = true;
				return true;
			}
			return false;
		}

		private bool FreeWrist()
		{
			if (!this._isWristFree)
			{
				this._syntheticHand.FreeWrist(SyntheticHand.WristLockMode.Full);
				this._isWristFree = true;
				return true;
			}
			return false;
		}

		protected override void Align(ReticleDataGhost data)
		{
			this.UpdateHandPose(this.HandGrabInteractor);
			this._syntheticHand.MarkInputDataRequiresUpdate();
		}

		protected override void Draw(ReticleDataGhost data)
		{
			this.HandVisual.ForceOffVisibility = false;
		}

		protected override void Hide()
		{
			this.HandVisual.ForceOffVisibility = true;
			this._syntheticHand.MarkInputDataRequiresUpdate();
		}

		public void InjectAllReticleGhostDrawer(IHandGrabInteractor handGrabInteractor, SyntheticHand syntheticHand, IHandVisual visualHand)
		{
			this.InjectHandGrabInteractor(handGrabInteractor);
			this.InjectSyntheticHand(syntheticHand);
			this.InjectVisualHand(visualHand);
		}

		public void InjectHandGrabInteractor(IHandGrabInteractor handGrabInteractor)
		{
			this._handGrabInteractor = (handGrabInteractor as Object);
			this.HandGrabInteractor = handGrabInteractor;
			this.Interactor = (handGrabInteractor as IInteractorView);
		}

		public void InjectSyntheticHand(SyntheticHand syntheticHand)
		{
			this._syntheticHand = syntheticHand;
		}

		public void InjectVisualHand(IHandVisual visualHand)
		{
			this._handVisual = (visualHand as Object);
			this.HandVisual = visualHand;
		}

		[Tooltip("The hand grab interactor to use for pose data.")]
		[FormerlySerializedAs("_handGrabber")]
		[SerializeField]
		[Interface(typeof(IHandGrabInteractor), new Type[]
		{
			typeof(IInteractorView)
		})]
		private Object _handGrabInteractor;

		[Tooltip("Provides pose data for the ghost hand.")]
		[FormerlySerializedAs("_modifier")]
		[SerializeField]
		private SyntheticHand _syntheticHand;

		[Tooltip("Determines the visuals of the hand.")]
		[SerializeField]
		[Interface(typeof(IHandVisual), new Type[]
		{

		})]
		[FormerlySerializedAs("_visualHand")]
		private Object _handVisual;

		private IHandVisual HandVisual;

		private bool _areFingersFree = true;

		private bool _isWristFree = true;

		private ITrackingToWorldTransformer Transformer;
	}
}
