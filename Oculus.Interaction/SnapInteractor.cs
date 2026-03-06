using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Oculus.Interaction
{
	public class SnapInteractor : Interactor<SnapInteractor, SnapInteractable>, IRigidbodyRef
	{
		public IPointableElement PointableElement
		{
			get
			{
				return this._pointableElement;
			}
		}

		public Rigidbody Rigidbody
		{
			get
			{
				return this._rigidbody;
			}
		}

		public Pose SnapPose
		{
			get
			{
				return this._snapPoseTransform.GetPose(Space.World);
			}
		}

		private void Reset()
		{
			this._rigidbody = base.GetComponentInParent<Rigidbody>();
			this._pointableElement = base.GetComponentInParent<PointableElement>();
		}

		public float DistanceThreshold
		{
			get
			{
				return this._distanceThreshold;
			}
			set
			{
				this._distanceThreshold = value;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			this._nativeId = 6011849687482789746UL;
		}

		protected override void Start()
		{
			this.BeginStart(ref this._started, delegate
			{
				base.Start();
			});
			if (this._snapPoseTransform == null)
			{
				this._snapPoseTransform = base.transform;
			}
			this.EndStart(ref this._started);
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if (this._started)
			{
				this._pointableElement.WhenPointerEventRaised += this.HandlePointerEventRaised;
				if (this._defaultInteractable != null)
				{
					this.SetComputeCandidateOverride(() => this._defaultInteractable, true);
					this.SetComputeShouldSelectOverride(() => true, true);
				}
			}
		}

		protected override void OnDisable()
		{
			if (this._started)
			{
				this._pointableElement.WhenPointerEventRaised -= this.HandlePointerEventRaised;
			}
			base.OnDisable();
		}

		protected override bool ComputeShouldSelect()
		{
			return this._shouldSelect;
		}

		protected override bool ComputeShouldUnselect()
		{
			return this._shouldUnselect;
		}

		protected override void DoHoverUpdate()
		{
			base.DoHoverUpdate();
			this._shouldUnselect = false;
			if (base.Interactable == null)
			{
				return;
			}
			this.GeneratePointerEvent(PointerEventType.Move);
			base.Interactable.InteractorHoverUpdated(this);
		}

		protected override void DoSelectUpdate()
		{
			base.DoSelectUpdate();
			if (this._movement == null || base.Interactable == null)
			{
				this._shouldUnselect = true;
				return;
			}
			Pose target;
			if (base.Interactable.PoseForInteractor(this, out target))
			{
				this._movement.UpdateTarget(target);
				this._movement.Tick();
				this.GeneratePointerEvent(PointerEventType.Move);
				return;
			}
			this._shouldUnselect = true;
		}

		protected override void InteractableSet(SnapInteractable interactable)
		{
			base.InteractableSet(interactable);
			if (interactable != null)
			{
				this.GeneratePointerEvent(PointerEventType.Hover);
			}
		}

		protected override void InteractableUnset(SnapInteractable interactable)
		{
			if (interactable != null)
			{
				this.GeneratePointerEvent(PointerEventType.Unhover);
			}
			base.InteractableUnset(interactable);
		}

		protected override void InteractableSelected(SnapInteractable interactable)
		{
			base.InteractableSelected(interactable);
			this._shouldSelect = false;
			if (interactable != null)
			{
				Pose pose = this._snapPoseTransform.GetPose(Space.World);
				this._movement = interactable.GenerateMovement(pose, this);
				if (this._movement != null)
				{
					this.GeneratePointerEvent(PointerEventType.Select);
				}
			}
		}

		protected override void InteractableUnselected(SnapInteractable interactable)
		{
			IMovement movement = this._movement;
			if (movement != null)
			{
				movement.StopAndSetPose(this._movement.Pose);
			}
			if (interactable != null)
			{
				this.GeneratePointerEvent(PointerEventType.Unselect);
			}
			base.InteractableUnselected(interactable);
			this._movement = null;
		}

		protected virtual void HandlePointerEventRaised(PointerEvent evt)
		{
			if (this._pointableElement.SelectingPointsCount == 0 && evt.Identifier != base.Identifier && evt.Type == PointerEventType.Unselect && base.Interactable != null)
			{
				this._shouldSelect = true;
			}
			if (evt.Identifier == base.Identifier && evt.Type == PointerEventType.Cancel && base.Interactable != null)
			{
				base.Interactable.RemoveInteractorByIdentifier(base.Identifier);
			}
		}

		private void GeneratePointerEvent(PointerEventType pointerEventType)
		{
			Pose pose = this.ComputePointerPose();
			this._pointableElement.ProcessPointerEvent(new PointerEvent(base.Identifier, pointerEventType, pose, base.Data));
		}

		protected override void DoPreprocess()
		{
			if (this._pointableElement.Points.Count == 0)
			{
				if (this._idleStarted < 0f)
				{
					this._idleStarted = Time.time;
					return;
				}
			}
			else
			{
				this._idleStarted = -1f;
			}
		}

		protected Pose ComputePointerPose()
		{
			if (this._movement != null)
			{
				return this._movement.Pose;
			}
			return this.SnapPose;
		}

		private bool TimedOut()
		{
			return this._timeOutInteractable != null && this._timeOut >= 0f && this._idleStarted >= 0f && Time.time - this._idleStarted > this._timeOut;
		}

		protected override SnapInteractable ComputeCandidate()
		{
			if (this.TimedOut())
			{
				this._shouldSelect = true;
				return this._timeOutInteractable;
			}
			if (this._pointableElement.SelectingPointsCount != 0)
			{
				float num = this._distanceThreshold * this._distanceThreshold;
				SnapInteractable result = null;
				float num2 = float.MaxValue;
				float num3 = float.MaxValue;
				foreach (SnapInteractable snapInteractable in Interactable<SnapInteractor, SnapInteractable>.Registry.List(this))
				{
					Pose pose;
					if (snapInteractable.PoseForInteractor(this, out pose))
					{
						float sqrMagnitude = (pose.position - this._snapPoseTransform.position).sqrMagnitude;
						if (sqrMagnitude <= num2)
						{
							float num4 = Quaternion.Angle(pose.rotation, this._snapPoseTransform.rotation);
							if (Mathf.Abs(sqrMagnitude - num2) >= num || num4 < num3)
							{
								num2 = sqrMagnitude;
								num3 = num4;
								result = snapInteractable;
							}
						}
					}
				}
				return result;
			}
			if (!this._shouldSelect)
			{
				return null;
			}
			return base.Interactable;
		}

		public void InjectAllSnapInteractor(PointableElement pointableElement, Rigidbody rigidbody)
		{
			this.InjectPointableElement(pointableElement);
			this.InjectRigidbody(rigidbody);
		}

		public void InjectPointableElement(PointableElement pointableElement)
		{
			this._pointableElement = pointableElement;
		}

		public void InjectRigidbody(Rigidbody rigidbody)
		{
			this._rigidbody = rigidbody;
		}

		public void InjectOptionalSnapPoseTransform(Transform snapPoint)
		{
			this._snapPoseTransform = snapPoint;
		}

		public void InjectOptionalTimeOutInteractable(SnapInteractable interactable)
		{
			this._timeOutInteractable = interactable;
		}

		public void InjectOptionaTimeOut(float timeOut)
		{
			this._timeOut = timeOut;
		}

		[Tooltip("The object's Grabbable component.")]
		[SerializeField]
		private PointableElement _pointableElement;

		[Tooltip("The object's RigidBody component.")]
		[SerializeField]
		private Rigidbody _rigidbody;

		[Tooltip("Used to determine which object should snap to your hand when there are multiple to choose from. Objects with a lower threshold have a higher priority.")]
		[SerializeField]
		private float _distanceThreshold = 0.01f;

		[SerializeField]
		[Optional]
		[FormerlySerializedAs("_snapPoint")]
		[FormerlySerializedAs("_dropPoint")]
		private Transform _snapPoseTransform;

		[Tooltip("The default Interactable to snap to until you interact with the object.")]
		[SerializeField]
		[Optional]
		private SnapInteractable _defaultInteractable;

		[SerializeField]
		[Optional]
		[Tooltip("Interactable to automatically snap to when the associated Pointable is not being pointed at for Time-Out seconds")]
		private SnapInteractable _timeOutInteractable;

		[SerializeField]
		[Optional]
		[Tooltip("When the associated Pointable is not being pointed at for Time-Out seconds the SnapInteractor will snap to the TimeOutInteractable, unless it is null.")]
		private float _timeOut;

		private float _idleStarted = -1f;

		private IMovement _movement;

		private bool _shouldSelect;

		private bool _shouldUnselect;
	}
}
