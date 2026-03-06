using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.HandGrab
{
	public class HandGrabStateVisual : MonoBehaviour
	{
		protected virtual void Awake()
		{
			this.HandGrabState = (this._handGrabState as IHandGrabState);
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this.EndStart(ref this._started);
		}

		private void LateUpdate()
		{
			float fingersConstraint;
			float wristConstraint;
			this.ConstrainingForce(this.HandGrabState, out fingersConstraint, out wristConstraint);
			this.UpdateHandPose(this.HandGrabState, fingersConstraint, wristConstraint);
			bool flag = this._areFingersFree && this._isWristFree;
			if (!flag || (flag && !this._wasCompletelyFree))
			{
				this._syntheticHand.MarkInputDataRequiresUpdate();
			}
			this._wasCompletelyFree = flag;
		}

		private void ConstrainingForce(IHandGrabState grabSource, out float fingersConstraint, out float wristConstraint)
		{
			HandGrabTarget handGrabTarget = grabSource.HandGrabTarget;
			fingersConstraint = (wristConstraint = 0f);
			if (handGrabTarget == null)
			{
				return;
			}
			if (grabSource.IsGrabbing && handGrabTarget.HandAlignment != HandAlignType.None)
			{
				fingersConstraint = grabSource.FingersStrength;
				wristConstraint = grabSource.WristStrength;
				return;
			}
			if (handGrabTarget.HandAlignment == HandAlignType.AttractOnHover)
			{
				fingersConstraint = grabSource.FingersStrength;
				wristConstraint = grabSource.WristStrength;
				return;
			}
			if (handGrabTarget.HandAlignment == HandAlignType.AlignFingersOnHover)
			{
				fingersConstraint = grabSource.FingersStrength;
			}
		}

		private void UpdateHandPose(IHandGrabState grabSource, float fingersConstraint, float wristConstraint)
		{
			HandGrabTarget handGrabTarget = grabSource.HandGrabTarget;
			if (handGrabTarget == null)
			{
				this.FreeFingers();
				this.FreeWrist();
				return;
			}
			if (fingersConstraint > 0f && handGrabTarget.HandPose != null)
			{
				this.UpdateFingers(handGrabTarget.HandPose, grabSource.GrabbingFingers(), fingersConstraint);
				this._areFingersFree = false;
			}
			else
			{
				this.FreeFingers();
			}
			if (wristConstraint > 0f)
			{
				Pose visualWristPose = grabSource.GetVisualWristPose();
				this._syntheticHand.LockWristPose(visualWristPose, wristConstraint, SyntheticHand.WristLockMode.Full, true, false);
				this._isWristFree = false;
				return;
			}
			this.FreeWrist();
		}

		private void UpdateFingers(HandPose handPose, HandFingerFlags grabbingFingers, float strength)
		{
			Quaternion[] jointRotations = handPose.JointRotations;
			this._syntheticHand.OverrideAllJoints(jointRotations, strength);
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

		public void InjectAllHandGrabInteractorVisual(IHandGrabState handGrabState, SyntheticHand syntheticHand)
		{
			this.InjectHandGrabState(handGrabState);
			this.InjectSyntheticHand(syntheticHand);
		}

		public void InjectHandGrabState(IHandGrabState handGrabState)
		{
			this.HandGrabState = handGrabState;
			this._handGrabState = (handGrabState as Object);
		}

		public void InjectSyntheticHand(SyntheticHand syntheticHand)
		{
			this._syntheticHand = syntheticHand;
		}

		[SerializeField]
		[Interface(typeof(IHandGrabState), new Type[]
		{

		})]
		private Object _handGrabState;

		private IHandGrabState HandGrabState;

		[SerializeField]
		private SyntheticHand _syntheticHand;

		private bool _areFingersFree = true;

		private bool _isWristFree = true;

		private bool _wasCompletelyFree = true;

		protected bool _started;
	}
}
