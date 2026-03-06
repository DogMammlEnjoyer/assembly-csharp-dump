using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Oculus.Interaction
{
	public class SnapInteractable : Interactable<SnapInteractor, SnapInteractable>, IRigidbodyRef
	{
		public Rigidbody Rigidbody
		{
			get
			{
				return this._rigidbody;
			}
		}

		private ISnapPoseDelegate SnapPoseDelegate { get; set; }

		private IMovementProvider MovementProvider { get; set; }

		private void Reset()
		{
			this._rigidbody = base.GetComponentInParent<Rigidbody>();
		}

		protected override void Awake()
		{
			base.Awake();
			this.MovementProvider = (this._movementProvider as IMovementProvider);
			this.SnapPoseDelegate = (this._snapPoseDelegate as ISnapPoseDelegate);
		}

		protected override void Start()
		{
			this.BeginStart(ref this._started, delegate
			{
				base.Start();
			});
			if (SnapInteractable._registry == null)
			{
				SnapInteractable._registry = new CollisionInteractionRegistry<SnapInteractor, SnapInteractable>();
				this.SetRegistry(SnapInteractable._registry);
			}
			if (this.MovementProvider == null)
			{
				this.MovementProvider = base.gameObject.AddComponent<MoveTowardsTargetProvider>();
				this._movementProvider = (this.MovementProvider as MonoBehaviour);
			}
			this.EndStart(ref this._started);
		}

		protected override void InteractorAdded(SnapInteractor interactor)
		{
			base.InteractorAdded(interactor);
			if (this.SnapPoseDelegate != null)
			{
				this.SnapPoseDelegate.TrackElement(interactor.Identifier, interactor.SnapPose);
			}
		}

		protected override void InteractorRemoved(SnapInteractor interactor)
		{
			base.InteractorRemoved(interactor);
			if (this.SnapPoseDelegate != null)
			{
				this.SnapPoseDelegate.UntrackElement(interactor.Identifier);
			}
		}

		protected override void SelectingInteractorAdded(SnapInteractor interactor)
		{
			base.SelectingInteractorAdded(interactor);
			if (this.SnapPoseDelegate != null)
			{
				this.SnapPoseDelegate.SnapElement(interactor.Identifier, interactor.SnapPose);
			}
		}

		protected override void SelectingInteractorRemoved(SnapInteractor interactor)
		{
			base.SelectingInteractorRemoved(interactor);
			if (this.SnapPoseDelegate != null)
			{
				this.SnapPoseDelegate.UnsnapElement(interactor.Identifier);
			}
		}

		public void InteractorHoverUpdated(SnapInteractor interactor)
		{
			if (this.SnapPoseDelegate != null)
			{
				this.SnapPoseDelegate.MoveTrackedElement(interactor.Identifier, interactor.SnapPose);
			}
		}

		public bool PoseForInteractor(SnapInteractor interactor, out Pose result)
		{
			if (this.SnapPoseDelegate != null)
			{
				return this.SnapPoseDelegate.SnapPoseForElement(interactor.Identifier, interactor.SnapPose, out result);
			}
			result = base.transform.GetPose(Space.World);
			return true;
		}

		public IMovement GenerateMovement(in Pose from, SnapInteractor interactor)
		{
			Pose target;
			if (this.PoseForInteractor(interactor, out target))
			{
				IMovement movement = this.MovementProvider.CreateMovement();
				movement.StopAndSetPose(from);
				movement.MoveTo(target);
				return movement;
			}
			return null;
		}

		public void InjectAllSnapInteractable(Rigidbody rigidbody)
		{
			this.InjectRigidbody(rigidbody);
		}

		public void InjectRigidbody(Rigidbody rigidbody)
		{
			this._rigidbody = rigidbody;
		}

		public void InjectOptionalMovementProvider(IMovementProvider provider)
		{
			this._movementProvider = (provider as Object);
			this.MovementProvider = provider;
		}

		public void InjectOptionalSnapPoseDelegate(ISnapPoseDelegate snapPoseDelegate)
		{
			this._snapPoseDelegate = (snapPoseDelegate as Object);
			this.SnapPoseDelegate = snapPoseDelegate;
		}

		[SerializeField]
		private Rigidbody _rigidbody;

		[FormerlySerializedAs("_snapPosesProvider")]
		[FormerlySerializedAs("_posesProvider")]
		[SerializeField]
		[Optional]
		[Interface(typeof(ISnapPoseDelegate), new Type[]
		{

		})]
		private Object _snapPoseDelegate;

		[SerializeField]
		[Optional]
		[Interface(typeof(IMovementProvider), new Type[]
		{

		})]
		private Object _movementProvider;

		private static CollisionInteractionRegistry<SnapInteractor, SnapInteractable> _registry;
	}
}
