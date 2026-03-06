using System;
using Oculus.Interaction.Grab;
using Oculus.Interaction.GrabAPI;
using Oculus.Interaction.Input;
using Oculus.Interaction.Throw;
using UnityEngine;

namespace Oculus.Interaction.HandGrab
{
	public class DistanceHandGrabInteractor : PointerInteractor<DistanceHandGrabInteractor, DistanceHandGrabInteractable>, IHandGrabInteractor, IHandGrabState, IDistanceInteractor, IInteractorView
	{
		public IHand Hand { get; private set; }

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

		public Pose Origin
		{
			get
			{
				return this._distantCandidateComputer.Origin;
			}
		}

		public Vector3 HitPoint { get; private set; }

		public IRelativeToRef DistanceInteractable
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
			this._nativeId = 4929598210385797474UL;
		}

		protected override void Start()
		{
			this.BeginStart(ref this._started, delegate
			{
				base.Start();
			});
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

		protected override void InteractableSet(DistanceHandGrabInteractable interactable)
		{
			base.InteractableSet(interactable);
			this.UpdateTarget(base.Interactable);
		}

		protected override void InteractableUnset(DistanceHandGrabInteractable interactable)
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

		protected override void InteractableSelected(DistanceHandGrabInteractable interactable)
		{
			if (interactable != null)
			{
				this.WristToGrabPoseOffset = this.GetGrabOffset();
				this.Movement = this.GenerateMovement(interactable);
				this.SetGrabStrength(1f);
			}
			base.InteractableSelected(interactable);
		}

		protected override void InteractableUnselected(DistanceHandGrabInteractable interactable)
		{
			base.InteractableUnselected(interactable);
			this.Movement = null;
			this._currentGrabType = GrabTypeFlags.None;
			ReleaseVelocityInformation releaseVelocityInformation = (this.VelocityCalculator != null) ? this.VelocityCalculator.CalculateThrowVelocity(interactable.transform) : new ReleaseVelocityInformation(Vector3.zero, Vector3.zero, Vector3.zero, false);
			interactable.ApplyVelocities(releaseVelocityInformation.LinearVelocity, releaseVelocityInformation.AngularVelocity);
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
			return this._handGrabShouldUnselect;
		}

		public override bool CanSelect(DistanceHandGrabInteractable interactable)
		{
			return base.CanSelect(interactable) && this.CanInteractWith(interactable);
		}

		protected override DistanceHandGrabInteractable ComputeCandidate()
		{
			Vector3 hitPoint;
			DistanceHandGrabInteractable distanceHandGrabInteractable = this._distantCandidateComputer.ComputeCandidate(Interactable<DistanceHandGrabInteractor, DistanceHandGrabInteractable>.Registry, this, out hitPoint);
			this.HitPoint = hitPoint;
			if (distanceHandGrabInteractable == null)
			{
				return null;
			}
			GrabTypeFlags grabTypes = this.SelectingGrabTypes(distanceHandGrabInteractable);
			if (this.GetPoseScore(distanceHandGrabInteractable, grabTypes, ref this._cachedResult).IsValid())
			{
				return distanceHandGrabInteractable;
			}
			return null;
		}

		private GrabTypeFlags SelectingGrabTypes(IHandGrabInteractable interactable)
		{
			GrabTypeFlags grabTypeFlags;
			if (base.State == InteractorState.Select || (grabTypeFlags = this.ComputeShouldSelect(interactable)) == GrabTypeFlags.None)
			{
				HandGrabInteraction.ComputeHandGrabScore(this, interactable, out grabTypeFlags, false);
			}
			if (grabTypeFlags == GrabTypeFlags.None)
			{
				grabTypeFlags = (interactable.SupportedGrabTypes & this.SupportedGrabTypes);
			}
			return grabTypeFlags;
		}

		private void UpdateTarget(IHandGrabInteractable interactable)
		{
			this.WristToGrabPoseOffset = this.GetGrabOffset();
			GrabTypeFlags selectingGrabTypes = this.SelectingGrabTypes(interactable);
			this.SetTarget(interactable, selectingGrabTypes);
			GrabTypeFlags grabTypeFlags;
			float grabStrength = HandGrabInteraction.ComputeHandGrabScore(this, interactable, out grabTypeFlags, false);
			this.SetGrabStrength(grabStrength);
		}

		private void UpdateTargetSliding(IHandGrabInteractable interactable)
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

		public void InjectAllDistanceHandGrabInteractor(HandGrabAPI handGrabApi, DistantCandidateComputer<DistanceHandGrabInteractor, DistanceHandGrabInteractable> distantCandidateComputer, Transform grabOrigin, IHand hand, GrabTypeFlags supportedGrabTypes)
		{
			this.InjectHandGrabApi(handGrabApi);
			this.InjectDistantCandidateComputer(distantCandidateComputer);
			this.InjectGrabOrigin(grabOrigin);
			this.InjectHand(hand);
			this.InjectSupportedGrabTypes(supportedGrabTypes);
		}

		public void InjectHandGrabApi(HandGrabAPI handGrabApi)
		{
			this._handGrabApi = handGrabApi;
		}

		public void InjectDistantCandidateComputer(DistantCandidateComputer<DistanceHandGrabInteractor, DistanceHandGrabInteractable> distantCandidateComputer)
		{
			this._distantCandidateComputer = distantCandidateComputer;
		}

		public void InjectHand(IHand hand)
		{
			this._hand = (hand as Object);
			this.Hand = hand;
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

		public void InjectOptionalPinchPoint(Transform pinchPoint)
		{
			this._pinchPoint = pinchPoint;
		}

		[Obsolete("Use Grabbable instead")]
		public void InjectOptionalVelocityCalculator(IThrowVelocityCalculator velocityCalculator)
		{
			this._velocityCalculator = (velocityCalculator as Object);
			this.VelocityCalculator = velocityCalculator;
		}

		[Tooltip("The hand to use.")]
		[SerializeField]
		[Interface(typeof(IHand), new Type[]
		{

		})]
		private Object _hand;

		[Tooltip("Detects when the hand grab selects or unselects.")]
		[SerializeField]
		private HandGrabAPI _handGrabApi;

		[Header("Grabbing")]
		[Tooltip("The grab types to support.")]
		[SerializeField]
		private GrabTypeFlags _supportedGrabTypes = GrabTypeFlags.Pinch;

		[Tooltip("The point on the hand used as the origin of the grab.")]
		[SerializeField]
		private Transform _grabOrigin;

		[Tooltip("Specifies an offset from the wrist that can be used to search for the best HandGrabInteractable available, act as a palm grab without a HandPose, and also act as an anchor for attaching the object.")]
		[SerializeField]
		[Optional]
		private Transform _gripPoint;

		[Tooltip("Specifies a moving point at the center of the tips of the currently pinching fingers. It's used to align interactables that don’t have a HandPose to the center of the pinch.")]
		[SerializeField]
		[Optional]
		private Transform _pinchPoint;

		[Tooltip("Determines how the object will move when thrown.")]
		[SerializeField]
		[Interface(typeof(IThrowVelocityCalculator), new Type[]
		{

		})]
		[Optional(OptionalAttribute.Flag.Obsolete)]
		[Obsolete("Use Grabbable instead")]
		private Object _velocityCalculator;

		[SerializeField]
		private DistantCandidateComputer<DistanceHandGrabInteractor, DistanceHandGrabInteractable> _distantCandidateComputer = new DistantCandidateComputer<DistanceHandGrabInteractor, DistanceHandGrabInteractable>();

		private bool _handGrabShouldSelect;

		private bool _handGrabShouldUnselect;

		private HandGrabResult _cachedResult = new HandGrabResult();

		private GrabTypeFlags _currentGrabType;
	}
}
