using System;
using System.Collections.Generic;
using Oculus.Interaction.GrabAPI;
using UnityEngine;

namespace Oculus.Interaction.HandGrab
{
	public class HandGrabUseInteractable : Interactable<HandGrabUseInteractor, HandGrabUseInteractable>
	{
		private IHandGrabUseDelegate HandUseDelegate { get; set; }

		public GrabbingRule UseFingers
		{
			get
			{
				return this._useFingers;
			}
			set
			{
				this._useFingers = value;
			}
		}

		public float StrengthDeadzone
		{
			get
			{
				return this._strengthDeadzone;
			}
			set
			{
				this._strengthDeadzone = value;
			}
		}

		public float UseProgress { get; private set; }

		public List<HandGrabPose> RelaxGrabPoints
		{
			get
			{
				return this._relaxedHandGrabPoses;
			}
		}

		public List<HandGrabPose> TightGrabPoints
		{
			get
			{
				return this._tightHandGrabPoses;
			}
		}

		public float UseStrengthDeadZone
		{
			get
			{
				return this._strengthDeadzone;
			}
		}

		protected virtual void Reset()
		{
			HandGrabInteractable componentInParent = base.GetComponentInParent<HandGrabInteractable>();
			if (componentInParent != null)
			{
				this._relaxedHandGrabPoses = new List<HandGrabPose>(componentInParent.HandGrabPoses);
			}
		}

		protected override void Awake()
		{
			base.Awake();
			this.HandUseDelegate = (this._handUseDelegate as IHandGrabUseDelegate);
		}

		protected override void SelectingInteractorAdded(HandGrabUseInteractor interactor)
		{
			base.SelectingInteractorAdded(interactor);
			IHandGrabUseDelegate handUseDelegate = this.HandUseDelegate;
			if (handUseDelegate == null)
			{
				return;
			}
			handUseDelegate.BeginUse();
		}

		protected override void SelectingInteractorRemoved(HandGrabUseInteractor interactor)
		{
			base.SelectingInteractorRemoved(interactor);
			IHandGrabUseDelegate handUseDelegate = this.HandUseDelegate;
			if (handUseDelegate == null)
			{
				return;
			}
			handUseDelegate.EndUse();
		}

		public float ComputeUseStrength(float strength)
		{
			this.UseProgress = ((this.HandUseDelegate != null) ? this.HandUseDelegate.ComputeUseStrength(strength) : strength);
			return this.UseProgress;
		}

		public bool FindBestHandPoses(float handScale, ref HandPose relaxedHandPose, ref HandPose tightHandPose, out float score)
		{
			if (this.FindScaledHandPose(this._relaxedHandGrabPoses, handScale, ref relaxedHandPose) && this.FindScaledHandPose(this._tightHandGrabPoses, handScale, ref tightHandPose))
			{
				score = 1f;
				return true;
			}
			score = 0f;
			return false;
		}

		private bool FindScaledHandPose(List<HandGrabPose> _handGrabPoses, float handScale, ref HandPose handPose)
		{
			if (_handGrabPoses.Count == 1 && _handGrabPoses[0].HandPose != null)
			{
				handPose.CopyFrom(_handGrabPoses[0].HandPose, false);
				return true;
			}
			if (_handGrabPoses.Count <= 1)
			{
				return false;
			}
			HandGrabPose handGrabPose;
			HandGrabPose handGrabPose2;
			float t;
			GrabPoseFinder.FindInterpolationRange(handScale / base.transform.lossyScale.x, _handGrabPoses, out handGrabPose, out handGrabPose2, out t);
			if (handGrabPose.HandPose != null && handGrabPose2.HandPose != null)
			{
				HandPose handPose2 = handGrabPose.HandPose;
				HandPose handPose3 = handGrabPose2.HandPose;
				HandPose.Lerp(handPose2, handPose3, t, ref handPose);
				return true;
			}
			if (handGrabPose.HandPose != null)
			{
				handPose.CopyFrom(handGrabPose.HandPose, false);
				return true;
			}
			if (handGrabPose2.HandPose != null)
			{
				handPose.CopyFrom(handGrabPose2.HandPose, false);
				return true;
			}
			return false;
		}

		public void InjectOptionalForwardUseDelegate(IHandGrabUseDelegate useDelegate)
		{
			this._handUseDelegate = (useDelegate as Object);
			this.HandUseDelegate = useDelegate;
		}

		public void InjectOptionalRelaxedHandGrabPoints(List<HandGrabPose> relaxedHandGrabPoints)
		{
			this._relaxedHandGrabPoses = relaxedHandGrabPoints;
		}

		public void InjectOptionalTightHandGrabPoints(List<HandGrabPose> tightHandGrabPoints)
		{
			this._tightHandGrabPoses = tightHandGrabPoints;
		}

		[SerializeField]
		[Interface(typeof(IHandGrabUseDelegate), new Type[]
		{

		})]
		[Optional(OptionalAttribute.Flag.DontHide)]
		private Object _handUseDelegate;

		[SerializeField]
		private GrabbingRule _useFingers;

		[SerializeField]
		[Range(0f, 1f)]
		private float _strengthDeadzone = 0.2f;

		[SerializeField]
		[Optional(OptionalAttribute.Flag.DontHide)]
		private List<HandGrabPose> _relaxedHandGrabPoses = new List<HandGrabPose>();

		[SerializeField]
		[Optional(OptionalAttribute.Flag.DontHide)]
		private List<HandGrabPose> _tightHandGrabPoses = new List<HandGrabPose>();
	}
}
