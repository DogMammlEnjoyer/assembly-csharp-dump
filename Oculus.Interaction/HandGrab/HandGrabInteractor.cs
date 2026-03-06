using System;
using Oculus.Interaction.Grab;
using Oculus.Interaction.GrabAPI;
using Oculus.Interaction.Input;
using Oculus.Interaction.Throw;
using UnityEngine;

namespace Oculus.Interaction.HandGrab
{
	public class HandGrabInteractor : PointerInteractor<HandGrabInteractor, HandGrabInteractable>, IHandGrabInteractor, IHandGrabState, IRigidbodyRef
	{
		public IHand Hand { get; private set; }

		public bool HoverOnZeroStrength
		{
			get
			{
				return this._hoverOnZeroStrength;
			}
			set
			{
				this._hoverOnZeroStrength = value;
			}
		}

		[Obsolete("Use Grabbable instead")]
		public IThrowVelocityCalculator VelocityCalculator { get; set; }

		public IMovement Movement { get; set; }

		public bool MovementFinished { get; set; }

		public HandGrabTarget HandGrabTarget { get; } = new HandGrabTarget();

		public Transform WristPoint
		{
			get
			{
				return this._grabOrigin;
			}
		}

		public Transform PinchPoint
		{
			get
			{
				return this._pinchPoint;
			}
		}

		public Transform PalmPoint
		{
			get
			{
				return this._gripPoint;
			}
		}

		public HandGrabAPI HandGrabApi
		{
			get
			{
				return this._handGrabApi;
			}
		}

		public GrabTypeFlags SupportedGrabTypes
		{
			get
			{
				return this._supportedGrabTypes;
			}
		}

		public IHandGrabInteractable TargetInteractable
		{
			get
			{
				return base.Interactable;
			}
		}

		public virtual bool IsGrabbing
		{
			get
			{
				return base.HasSelectedInteractable && this.Movement != null && this.Movement.Stopped;
			}
		}

		public float FingersStrength { get; private set; }

		public float WristStrength { get; private set; }

		public Pose WristToGrabPoseOffset { get; private set; }

		public HandFingerFlags GrabbingFingers()
		{
			return this.GrabbingFingers(base.SelectedInteractable);
		}

		public Rigidbody Rigidbody
		{
			get
			{
				return this._rigidbody;
			}
		}

		protected virtual void Reset()
		{
			this._hand = (base.GetComponentInParent<IHand>() as MonoBehaviour);
			this._handGrabApi = base.GetComponentInParent<HandGrabAPI>();
		}

		protected override void Awake()
		{
			base.Awake();
			this.Hand = (this._hand as IHand);
			this.VelocityCalculator = (this._velocityCalculator as IThrowVelocityCalculator);
			this._nativeId = 5208257256663643250UL;
		}

		protected override void Start()
		{
			this.BeginStart(ref this._started, delegate
			{
				base.Start();
			});
			foreach (Collider collider in this.Rigidbody.GetComponentsInChildren<Collider>())
			{
			}
			this._velocityCalculator != null;
			this.EndStart(ref this._started);
		}

		protected override void DoHoverUpdate()
		{
			base.DoHoverUpdate();
			this._handGrabShouldSelect = false;
			if (base.Interactable == null)
			{
				return;
			}
			this.UpdateTarget(base.Interactable);
			this._currentGrabType = this.ComputeShouldSelect(base.Interactable);
			if (this._currentGrabType != GrabTypeFlags.None)
			{
				this._handGrabShouldSelect = true;
			}
		}

		protected override void InteractableSet(HandGrabInteractable interactable)
		{
			base.InteractableSet(interactable);
			this.UpdateTarget(base.Interactable);
		}

		protected override void InteractableUnset(HandGrabInteractable interactable)
		{
			base.InteractableUnset(interactable);
			this.SetGrabStrength(0f);
		}

		protected override void DoSelectUpdate()
		{
			this._handGrabShouldUnselect = false;
			if (base.SelectedInteractable == null)
			{
				this._handGrabShouldUnselect = true;
				return;
			}
			this.UpdateTargetSliding(base.SelectedInteractable);
			Pose handGrabPose = this.GetHandGrabPose();
			this.Movement.UpdateTarget(handGrabPose);
			this.Movement.Tick();
			GrabTypeFlags grabTypeFlags = this.ComputeShouldSelect(base.SelectedInteractable);
			GrabTypeFlags grabTypeFlags2 = this.ComputeShouldUnselect(base.SelectedInteractable);
			this._currentGrabType |= grabTypeFlags;
			this._currentGrabType &= ~grabTypeFlags2;
			if (grabTypeFlags2 != GrabTypeFlags.None && this._currentGrabType == GrabTypeFlags.None)
			{
				this._handGrabShouldUnselect = true;
			}
		}

		protected override void InteractableSelected(HandGrabInteractable interactable)
		{
			if (interactable != null)
			{
				this.WristToGrabPoseOffset = this.GetGrabOffset();
				this.Movement = this.GenerateMovement(interactable);
				this.SetGrabStrength(1f);
			}
			base.InteractableSelected(interactable);
		}

		protected override void InteractableUnselected(HandGrabInteractable interactable)
		{
			base.InteractableUnselected(interactable);
			this.Movement = null;
			this._currentGrabType = GrabTypeFlags.None;
			if (this.VelocityCalculator != null)
			{
				ReleaseVelocityInformation releaseVelocityInformation = this.VelocityCalculator.CalculateThrowVelocity(interactable.transform);
				interactable.ApplyVelocities(releaseVelocityInformation.LinearVelocity, releaseVelocityInformation.AngularVelocity);
			}
		}

		protected override void HandlePointerEventRaised(PointerEvent evt)
		{
			base.HandlePointerEventRaised(evt);
			if (base.SelectedInteractable == null || !base.SelectedInteractable.ResetGrabOnGrabsUpdated)
			{
				return;
			}
			if (evt.Identifier != base.Identifier && (evt.Type == PointerEventType.Select || evt.Type == PointerEventType.Unselect))
			{
				this.WristToGrabPoseOffset = this.GetGrabOffset();
				this.SetTarget(base.SelectedInteractable, this._currentGrabType);
				this.Movement = this.GenerateMovement(base.SelectedInteractable);
				Pose targetGrabPose = this.GetTargetGrabPose();
				PointerEvent evt2 = new PointerEvent(base.Identifier, PointerEventType.Move, targetGrabPose, base.Data);
				base.SelectedInteractable.PointableElement.ProcessPointerEvent(evt2);
			}
		}

		protected override Pose ComputePointerPose()
		{
			if (this.Movement != null)
			{
				return this.Movement.Pose;
			}
			return this.GetHandGrabPose();
		}

		protected override bool ComputeShouldSelect()
		{
			return this._handGrabShouldSelect;
		}

		protected override bool ComputeShouldUnselect()
		{
			return this._handGrabShouldUnselect || (this._selectedInteractableOverride != null && this._selectedInteractableOverride != base.SelectedInteractable);
		}

		public override bool CanSelect(HandGrabInteractable interactable)
		{
			return base.CanSelect(interactable) && this.CanInteractWith(interactable);
		}

		protected override HandGrabInteractable ComputeCandidate()
		{
			InteractableRegistry<HandGrabInteractor, HandGrabInteractable>.InteractableSet interactableSet = Interactable<HandGrabInteractor, HandGrabInteractable>.Registry.List(this);
			float num = float.NegativeInfinity;
			GrabPoseScore referenceScore = GrabPoseScore.Max;
			HandGrabInteractable result = null;
			foreach (HandGrabInteractable handGrabInteractable in interactableSet)
			{
				float num2;
				GrabTypeFlags grabTypeFlags = this.SelectingGrabTypes(handGrabInteractable, num, out num2);
				if (grabTypeFlags != GrabTypeFlags.None)
				{
					GrabPoseScore poseScore = this.GetPoseScore(handGrabInteractable, grabTypeFlags, ref this._cachedResult);
					if (num2 > num || poseScore.IsBetterThan(referenceScore))
					{
						num = num2;
						referenceScore = poseScore;
						result = handGrabInteractable;
					}
				}
			}
			return result;
		}

		private GrabTypeFlags SelectingGrabTypes(HandGrabInteractable interactable, float minFingerScoreRequired, out float fingerScore)
		{
			fingerScore = 1f;
			GrabTypeFlags grabTypeFlags;
			if (base.State == InteractorState.Select || (grabTypeFlags = this.ComputeShouldSelect(interactable)) == GrabTypeFlags.None)
			{
				fingerScore = HandGrabInteraction.ComputeHandGrabScore(this, interactable, out grabTypeFlags, false);
			}
			if (fingerScore < minFingerScoreRequired)
			{
				return GrabTypeFlags.None;
			}
			if (grabTypeFlags == GrabTypeFlags.None)
			{
				if (!this._hoverOnZeroStrength)
				{
					return GrabTypeFlags.None;
				}
				grabTypeFlags = (interactable.SupportedGrabTypes & this.SupportedGrabTypes);
			}
			if (this._gripCollider != null && (grabTypeFlags & GrabTypeFlags.Palm) != GrabTypeFlags.None && !this.OverlapsVolume(interactable, this._gripCollider))
			{
				grabTypeFlags &= ~GrabTypeFlags.Palm;
			}
			if (this._pinchCollider != null && (grabTypeFlags & GrabTypeFlags.Pinch) != GrabTypeFlags.None && !this.OverlapsVolume(interactable, this._pinchCollider))
			{
				grabTypeFlags &= ~GrabTypeFlags.Pinch;
			}
			return grabTypeFlags;
		}

		public void ForceSelect(HandGrabInteractable interactable, bool allowManualRelease = false)
		{
			this._selectedInteractableOverride = interactable;
			this.SetComputeCandidateOverride(() => interactable, true);
			this.SetComputeShouldSelectOverride(() => interactable == this.Interactable, true);
			if (!allowManualRelease)
			{
				this.SetComputeShouldUnselectOverride(() => interactable != this.SelectedInteractable, false);
			}
		}

		public void ForceRelease()
		{
			this._selectedInteractableOverride = null;
			this.ClearComputeCandidateOverride();
			this.ClearComputeShouldSelectOverride();
			if (base.State == InteractorState.Select)
			{
				this.SetComputeShouldUnselectOverride(() => true, true);
				return;
			}
			this.ClearComputeShouldUnselectOverride();
		}

		public override void SetComputeCandidateOverride(Func<HandGrabInteractable> computeCandidate, bool shouldClearOverrideOnSelect = true)
		{
			base.SetComputeCandidateOverride(() => computeCandidate(), shouldClearOverrideOnSelect);
		}

		public override void Unselect()
		{
			if (base.State == InteractorState.Select && this._selectedInteractableOverride != null && (base.SelectedInteractable == this._selectedInteractableOverride || base.SelectedInteractable == null))
			{
				this._selectedInteractableOverride = null;
				this.ClearComputeShouldUnselectOverride();
			}
			base.Unselect();
		}

		private bool OverlapsVolume(HandGrabInteractable interactable, Collider volume)
		{
			foreach (Collider collider in interactable.Colliders)
			{
				Vector3 vector;
				float num;
				if (collider.enabled && Physics.ComputePenetration(volume, volume.transform.position, volume.transform.rotation, collider, collider.transform.position, collider.transform.rotation, out vector, out num))
				{
					return true;
				}
			}
			return false;
		}

		private void UpdateTarget(HandGrabInteractable interactable)
		{
			this.WristToGrabPoseOffset = this.GetGrabOffset();
			float grabStrength;
			GrabTypeFlags selectingGrabTypes = this.SelectingGrabTypes(interactable, float.NegativeInfinity, out grabStrength);
			this.SetTarget(interactable, selectingGrabTypes);
			this.SetGrabStrength(grabStrength);
		}

		private void UpdateTargetSliding(HandGrabInteractable interactable)
		{
			if (interactable.Slippiness <= 0f)
			{
				return;
			}
			GrabTypeFlags selectingGrabTypes;
			if (HandGrabInteraction.ComputeHandGrabScore(this, interactable, out selectingGrabTypes, true) <= interactable.Slippiness)
			{
				this.SetTarget(interactable, selectingGrabTypes);
			}
		}

		private void SetTarget(IHandGrabInteractable interactable, GrabTypeFlags selectingGrabTypes)
		{
			GrabTypeFlags anchor;
			this.CalculateBestGrab(interactable, selectingGrabTypes, out anchor, ref this._cachedResult);
			this.HandGrabTarget.Set(interactable.RelativeTo, interactable.HandAlignment, anchor, this._cachedResult);
		}

		private void SetGrabStrength(float strength)
		{
			this.FingersStrength = strength;
			this.WristStrength = strength;
		}

		public void InjectAllHandGrabInteractor(HandGrabAPI handGrabApi, Transform grabOrigin, IHand hand, Rigidbody rigidbody, GrabTypeFlags supportedGrabTypes)
		{
			this.InjectHandGrabApi(handGrabApi);
			this.InjectGrabOrigin(grabOrigin);
			this.InjectHand(hand);
			this.InjectRigidbody(rigidbody);
			this.InjectSupportedGrabTypes(supportedGrabTypes);
		}

		public void InjectHandGrabApi(HandGrabAPI handGrabAPI)
		{
			this._handGrabApi = handGrabAPI;
		}

		public void InjectHand(IHand hand)
		{
			this._hand = (hand as Object);
			this.Hand = hand;
		}

		public void InjectRigidbody(Rigidbody rigidbody)
		{
			this._rigidbody = rigidbody;
		}

		public void InjectSupportedGrabTypes(GrabTypeFlags supportedGrabTypes)
		{
			this._supportedGrabTypes = supportedGrabTypes;
		}

		public void InjectGrabOrigin(Transform grabOrigin)
		{
			this._grabOrigin = grabOrigin;
		}

		public void InjectOptionalGripPoint(Transform gripPoint)
		{
			this._gripPoint = gripPoint;
		}

		public void InjectOptionalGripCollider(Collider gripCollider)
		{
			this._gripCollider = gripCollider;
		}

		public void InjectOptionalPinchPoint(Transform pinchPoint)
		{
			this._pinchPoint = pinchPoint;
		}

		public void InjectOptionalPinchCollider(Collider pinchCollider)
		{
			this._pinchCollider = pinchCollider;
		}

		[Obsolete("Use Grabbable instead")]
		public void InjectOptionalVelocityCalculator(IThrowVelocityCalculator velocityCalculator)
		{
			this._velocityCalculator = (velocityCalculator as Object);
			this.VelocityCalculator = velocityCalculator;
		}

		[Tooltip("The IHand that should be able to grab.")]
		[SerializeField]
		[Interface(typeof(IHand), new Type[]
		{

		})]
		private Object _hand;

		[Tooltip("The hand's Rigidbody, which detects interactables.")]
		[SerializeField]
		private Rigidbody _rigidbody;

		[Tooltip("Detects when the hand grab selects or unselects.")]
		[SerializeField]
		private HandGrabAPI _handGrabApi;

		[Tooltip("The grab types that the hand supports.")]
		[SerializeField]
		private GrabTypeFlags _supportedGrabTypes = GrabTypeFlags.All;

		[SerializeField]
		[Tooltip("When enabled, nearby interactables can become candidates even if thefinger strength is 0")]
		private bool _hoverOnZeroStrength;

		[Tooltip("The origin of the grab.")]
		[SerializeField]
		private Transform _grabOrigin;

		[Tooltip("Specifies an offset from the wrist that can be used to search for the best HandGrabInteractable available, act as a palm grab without a HandPose, and also act as an anchor for attaching the object.")]
		[SerializeField]
		[Optional]
		private Transform _gripPoint;

		[Tooltip("Collider used to detect a palm grab.")]
		[SerializeField]
		[Optional]
		private Collider _gripCollider;

		[Tooltip("Specifies a moving point at the center of the tips of the currently pinching fingers. It's used to align interactables that don’t have a HandPose to the center of the pinch.")]
		[SerializeField]
		[Optional]
		private Transform _pinchPoint;

		[Tooltip("Collider used to detect a pinch grab.")]
		[SerializeField]
		[Optional]
		private Collider _pinchCollider;

		[Tooltip("Determines how the object will move when thrown.")]
		[SerializeField]
		[Interface(typeof(IThrowVelocityCalculator), new Type[]
		{

		})]
		[Optional(OptionalAttribute.Flag.Obsolete)]
		[Obsolete("Use Grabbable instead")]
		private Object _velocityCalculator;

		private bool _handGrabShouldSelect;

		private bool _handGrabShouldUnselect;

		private HandGrabResult _cachedResult = new HandGrabResult();

		private HandGrabInteractable _selectedInteractableOverride;

		private GrabTypeFlags _currentGrabType;
	}
}
