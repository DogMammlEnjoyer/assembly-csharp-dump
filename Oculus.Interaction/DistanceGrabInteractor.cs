using System;
using Oculus.Interaction.Throw;
using UnityEngine;

namespace Oculus.Interaction
{
	public class DistanceGrabInteractor : PointerInteractor<DistanceGrabInteractor, DistanceGrabInteractable>, IDistanceInteractor, IInteractorView
	{
		[Obsolete("Use Grabbable instead")]
		public IThrowVelocityCalculator VelocityCalculator { get; set; }

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

		protected override void Awake()
		{
			base.Awake();
			base.Selector = (this._selector as ISelector);
			this.VelocityCalculator = (this._velocityCalculator as IThrowVelocityCalculator);
			this._nativeId = 4929598210385797474UL;
		}

		protected override void Start()
		{
			this.BeginStart(ref this._started, delegate
			{
				base.Start();
			});
			if (this._grabCenter == null)
			{
				this._grabCenter = base.transform;
			}
			if (this._grabTarget == null)
			{
				this._grabTarget = this._grabCenter;
			}
			this._velocityCalculator != null;
			this.EndStart(ref this._started);
		}

		protected override void DoPreprocess()
		{
			base.transform.position = this._grabCenter.position;
			base.transform.rotation = this._grabCenter.rotation;
		}

		protected override DistanceGrabInteractable ComputeCandidate()
		{
			Vector3 hitPoint;
			DistanceGrabInteractable result = this._distantCandidateComputer.ComputeCandidate(Interactable<DistanceGrabInteractor, DistanceGrabInteractable>.Registry, this, out hitPoint);
			this.HitPoint = hitPoint;
			return result;
		}

		protected override void InteractableSelected(DistanceGrabInteractable interactable)
		{
			Pose pose = this._grabTarget.GetPose(Space.World);
			this._movement = interactable.GenerateMovement(pose);
			base.InteractableSelected(interactable);
			interactable.WhenPointerEventRaised += this.HandleOtherPointerEventRaised;
		}

		protected override void InteractableUnselected(DistanceGrabInteractable interactable)
		{
			interactable.WhenPointerEventRaised -= this.HandleOtherPointerEventRaised;
			base.InteractableUnselected(interactable);
			this._movement = null;
			ReleaseVelocityInformation releaseVelocityInformation = (this.VelocityCalculator != null) ? this.VelocityCalculator.CalculateThrowVelocity(interactable.transform) : new ReleaseVelocityInformation(Vector3.zero, Vector3.zero, Vector3.zero, false);
			interactable.ApplyVelocities(releaseVelocityInformation.LinearVelocity, releaseVelocityInformation.AngularVelocity);
		}

		private void HandleOtherPointerEventRaised(PointerEvent evt)
		{
			if (base.SelectedInteractable == null)
			{
				return;
			}
			if (evt.Type == PointerEventType.Select || evt.Type == PointerEventType.Unselect)
			{
				Pose pose = this._grabTarget.GetPose(Space.World);
				if (base.SelectedInteractable.ResetGrabOnGrabsUpdated)
				{
					this._movement = base.SelectedInteractable.GenerateMovement(pose);
					base.SelectedInteractable.PointableElement.ProcessPointerEvent(new PointerEvent(base.Identifier, PointerEventType.Move, this._movement.Pose, base.Data));
				}
			}
			if (evt.Identifier == base.Identifier && evt.Type == PointerEventType.Cancel)
			{
				base.SelectedInteractable.WhenPointerEventRaised -= this.HandleOtherPointerEventRaised;
			}
		}

		protected override Pose ComputePointerPose()
		{
			if (this._movement != null)
			{
				return this._movement.Pose;
			}
			return this._grabTarget.GetPose(Space.World);
		}

		protected override void DoSelectUpdate()
		{
			if (this._selectedInteractable == null)
			{
				return;
			}
			this._movement.UpdateTarget(this._grabTarget.GetPose(Space.World));
			this._movement.Tick();
		}

		public void InjectAllDistanceGrabInteractor(ISelector selector, DistantCandidateComputer<DistanceGrabInteractor, DistanceGrabInteractable> distantCandidateComputer)
		{
			this.InjectSelector(selector);
			this.InjectDistantCandidateComputer(distantCandidateComputer);
		}

		public void InjectSelector(ISelector selector)
		{
			this._selector = (selector as Object);
			base.Selector = selector;
		}

		public void InjectDistantCandidateComputer(DistantCandidateComputer<DistanceGrabInteractor, DistanceGrabInteractable> distantCandidateComputer)
		{
			this._distantCandidateComputer = distantCandidateComputer;
		}

		public void InjectOptionalGrabCenter(Transform grabCenter)
		{
			this._grabCenter = grabCenter;
		}

		public void InjectOptionalGrabTarget(Transform grabTarget)
		{
			this._grabTarget = grabTarget;
		}

		[Obsolete("Use Grabbable instead")]
		public void InjectOptionalVelocityCalculator(IThrowVelocityCalculator velocityCalculator)
		{
			this._velocityCalculator = (velocityCalculator as Object);
			this.VelocityCalculator = velocityCalculator;
		}

		[Tooltip("The selection mechanism to trigger the grab.")]
		[SerializeField]
		[Interface(typeof(ISelector), new Type[]
		{

		})]
		private Object _selector;

		[Tooltip("The center of the grab.")]
		[SerializeField]
		[Optional]
		private Transform _grabCenter;

		[Tooltip("The location where the interactable will move when selected.")]
		[SerializeField]
		[Optional]
		private Transform _grabTarget;

		[Tooltip("Determines how the object will move when thrown.")]
		[SerializeField]
		[Interface(typeof(IThrowVelocityCalculator), new Type[]
		{

		})]
		[Optional(OptionalAttribute.Flag.Obsolete)]
		[Obsolete("Use Grabbable instead")]
		private Object _velocityCalculator;

		[SerializeField]
		private DistantCandidateComputer<DistanceGrabInteractor, DistanceGrabInteractable> _distantCandidateComputer = new DistantCandidateComputer<DistanceGrabInteractor, DistanceGrabInteractable>();

		private IMovement _movement;
	}
}
