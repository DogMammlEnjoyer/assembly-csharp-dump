using System;
using Oculus.Interaction.Grab;
using Oculus.Interaction.Throw;
using UnityEngine;

namespace Oculus.Interaction
{
	public class GrabInteractor : PointerInteractor<GrabInteractor, GrabInteractable>, IRigidbodyRef
	{
		public Rigidbody Rigidbody
		{
			get
			{
				return this._rigidbody;
			}
		}

		[Obsolete("Use Grabbable instead")]
		public IThrowVelocityCalculator VelocityCalculator { get; set; }

		protected override void Awake()
		{
			base.Awake();
			base.Selector = (this._selector as ISelector);
			this.VelocityCalculator = (this._velocityCalculator as IThrowVelocityCalculator);
			this._nativeId = 5148284398804954994UL;
		}

		protected override void Start()
		{
			this.BeginStart(ref this._started, delegate
			{
				base.Start();
			});
			this._colliders = this.Rigidbody.GetComponentsInChildren<Collider>();
			foreach (Collider collider in this._colliders)
			{
			}
			if (this._grabCenter == null)
			{
				this._grabCenter = base.transform;
			}
			if (this._grabTarget == null)
			{
				this._grabTarget = this._grabCenter;
			}
			this._velocityCalculator != null;
			this._tween = new Tween(Pose.identity, 0.5f, 0.25f, null);
			this.EndStart(ref this._started);
		}

		protected override void DoPreprocess()
		{
			base.transform.position = this._grabCenter.position;
			base.transform.rotation = this._grabCenter.rotation;
		}

		protected override GrabInteractable ComputeCandidate()
		{
			Vector3 position = this.Rigidbody.transform.position;
			GrabInteractable result = null;
			GrabPoseScore referenceScore = GrabPoseScore.Max;
			foreach (GrabInteractable grabInteractable in Interactable<GrabInteractor, GrabInteractable>.Registry.List(this))
			{
				Collider[] colliders = grabInteractable.Colliders;
				Vector3 vector;
				GrabPoseScore grabPoseScore = GrabPoseHelper.CollidersScore(position, grabInteractable.Colliders, out vector);
				if (grabPoseScore.IsBetterThan(referenceScore))
				{
					referenceScore = grabPoseScore;
					result = grabInteractable;
				}
			}
			return result;
		}

		public void ForceSelect(GrabInteractable interactable)
		{
			this._isSelectionOverriden = true;
			this._selectedInteractableOverride = interactable;
			this.SetComputeCandidateOverride(() => interactable, true);
			this.SetComputeShouldSelectOverride(() => interactable == this.Interactable, true);
			this.SetComputeShouldUnselectOverride(() => interactable != this.SelectedInteractable, false);
		}

		public void ForceRelease()
		{
			this._isSelectionOverriden = false;
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

		public override void Unselect()
		{
			if (base.State == InteractorState.Select && this._isSelectionOverriden && (base.SelectedInteractable == this._selectedInteractableOverride || base.SelectedInteractable == null))
			{
				this._isSelectionOverriden = false;
				this._selectedInteractableOverride = null;
				this.ClearComputeShouldUnselectOverride();
			}
			base.Unselect();
		}

		protected override void InteractableSelected(GrabInteractable interactable)
		{
			Pose pose = this._grabTarget.GetPose(Space.World);
			Pose grabSourceForTarget = this._interactable.GetGrabSourceForTarget(pose);
			this._tween.StopAndSetPose(grabSourceForTarget);
			base.InteractableSelected(interactable);
			this._tween.MoveTo(pose);
		}

		protected override void InteractableUnselected(GrabInteractable interactable)
		{
			base.InteractableUnselected(interactable);
			ReleaseVelocityInformation releaseVelocityInformation = (this.VelocityCalculator != null) ? this.VelocityCalculator.CalculateThrowVelocity(interactable.transform) : new ReleaseVelocityInformation(Vector3.zero, Vector3.zero, Vector3.zero, false);
			interactable.ApplyVelocities(releaseVelocityInformation.LinearVelocity, releaseVelocityInformation.AngularVelocity);
		}

		protected override void HandlePointerEventRaised(PointerEvent evt)
		{
			base.HandlePointerEventRaised(evt);
			if (base.SelectedInteractable == null)
			{
				return;
			}
			if (evt.Type == PointerEventType.Select || evt.Type == PointerEventType.Unselect || evt.Type == PointerEventType.Cancel)
			{
				Pose pose = this._grabTarget.GetPose(Space.World);
				if (base.SelectedInteractable.ResetGrabOnGrabsUpdated)
				{
					Pose grabSourceForTarget = this._interactable.GetGrabSourceForTarget(pose);
					this._tween.StopAndSetPose(grabSourceForTarget);
					base.SelectedInteractable.PointableElement.ProcessPointerEvent(new PointerEvent(base.Identifier, PointerEventType.Move, this._tween.Pose, base.Data));
					this._tween.MoveTo(pose);
					return;
				}
				this._tween.StopAndSetPose(pose);
				base.SelectedInteractable.PointableElement.ProcessPointerEvent(new PointerEvent(base.Identifier, PointerEventType.Move, pose, base.Data));
				this._tween.MoveTo(pose);
			}
		}

		protected override Pose ComputePointerPose()
		{
			if (base.SelectedInteractable != null)
			{
				return this._tween.Pose;
			}
			return this._grabTarget.GetPose(Space.World);
		}

		protected override void DoSelectUpdate()
		{
			GrabInteractable selectedInteractable = this._selectedInteractable;
			if (selectedInteractable == null)
			{
				return;
			}
			this._tween.UpdateTarget(this._grabTarget.GetPose(Space.World));
			this._tween.Tick();
			this._outsideReleaseDist = false;
			if (selectedInteractable.ReleaseDistance > 0f)
			{
				float num = float.MaxValue;
				Collider[] colliders = selectedInteractable.Colliders;
				for (int i = 0; i < colliders.Length; i++)
				{
					float sqrMagnitude = (colliders[i].bounds.center - this.Rigidbody.transform.position).sqrMagnitude;
					num = Mathf.Min(num, sqrMagnitude);
				}
				float num2 = selectedInteractable.ReleaseDistance * selectedInteractable.ReleaseDistance;
				if (num > num2)
				{
					this._outsideReleaseDist = true;
				}
			}
		}

		protected override bool ComputeShouldUnselect()
		{
			return this._outsideReleaseDist || base.ComputeShouldUnselect();
		}

		public void InjectAllGrabInteractor(ISelector selector, Rigidbody rigidbody)
		{
			this.InjectSelector(selector);
			this.InjectRigidbody(rigidbody);
		}

		public void InjectSelector(ISelector selector)
		{
			this._selector = (selector as Object);
			base.Selector = selector;
		}

		public void InjectRigidbody(Rigidbody rigidbody)
		{
			this._rigidbody = rigidbody;
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

		[Tooltip("The selection mechanism that broadcasts select and release events. For example, a ControllerSelector.")]
		[SerializeField]
		[Interface(typeof(ISelector), new Type[]
		{

		})]
		private Object _selector;

		[Tooltip("The hand or controller's Rigidbody, which detects interactables.")]
		[SerializeField]
		private Rigidbody _rigidbody;

		[Tooltip("The center of the grab.")]
		[SerializeField]
		[Optional]
		private Transform _grabCenter;

		[Tooltip("The location where the interactable will move when selected.")]
		[SerializeField]
		[Optional]
		private Transform _grabTarget;

		private Collider[] _colliders;

		private Tween _tween;

		private bool _outsideReleaseDist;

		[Tooltip("Determines how the object will move when thrown.")]
		[SerializeField]
		[Interface(typeof(IThrowVelocityCalculator), new Type[]
		{

		})]
		[Optional(OptionalAttribute.Flag.Obsolete)]
		[Obsolete("Use Grabbable instead")]
		private Object _velocityCalculator;

		private GrabInteractable _selectedInteractableOverride;

		private bool _isSelectionOverriden;
	}
}
