using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public class DistanceGrabInteractable : PointerInteractable<DistanceGrabInteractor, DistanceGrabInteractable>, IRigidbodyRef, IRelativeToRef, ICollidersRef
	{
		public Collider[] Colliders
		{
			get
			{
				return this._colliders;
			}
		}

		public Rigidbody Rigidbody
		{
			get
			{
				return this._rigidbody;
			}
		}

		private IMovementProvider MovementProvider { get; set; }

		public bool ResetGrabOnGrabsUpdated
		{
			get
			{
				return this._resetGrabOnGrabsUpdated;
			}
			set
			{
				this._resetGrabOnGrabsUpdated = value;
			}
		}

		public Transform RelativeTo
		{
			get
			{
				return this._grabSource;
			}
		}

		protected virtual void Reset()
		{
			this._rigidbody = base.GetComponentInParent<Rigidbody>();
		}

		protected override void Awake()
		{
			base.Awake();
			this.MovementProvider = (this._movementProvider as IMovementProvider);
		}

		protected override void Start()
		{
			this.BeginStart(ref this._started, delegate
			{
				base.Start();
			});
			this._colliders = this.Rigidbody.GetComponentsInChildren<Collider>();
			if (this.MovementProvider == null)
			{
				MoveTowardsTargetProvider provider = base.gameObject.AddComponent<MoveTowardsTargetProvider>();
				this.InjectOptionalMovementProvider(provider);
			}
			if (this._grabSource == null)
			{
				this._grabSource = this.Rigidbody.transform;
			}
			this.EndStart(ref this._started);
		}

		public IMovement GenerateMovement(in Pose to)
		{
			Pose pose = this._grabSource.GetPose(Space.World);
			IMovement movement = this.MovementProvider.CreateMovement();
			movement.StopAndSetPose(pose);
			movement.MoveTo(to);
			return movement;
		}

		[Obsolete("Use Grabbable instead")]
		public void ApplyVelocities(Vector3 linearVelocity, Vector3 angularVelocity)
		{
			if (this._physicsGrabbable == null)
			{
				return;
			}
			this._physicsGrabbable.ApplyVelocities(linearVelocity, angularVelocity);
		}

		public void InjectAllGrabInteractable(Rigidbody rigidbody)
		{
			this.InjectRigidbody(rigidbody);
		}

		public void InjectRigidbody(Rigidbody rigidbody)
		{
			this._rigidbody = rigidbody;
		}

		public void InjectOptionalGrabSource(Transform grabSource)
		{
			this._grabSource = grabSource;
		}

		[Obsolete("Use Grabbable instead")]
		public void InjectOptionalPhysicsGrabbable(PhysicsGrabbable physicsGrabbable)
		{
			this._physicsGrabbable = physicsGrabbable;
		}

		public void InjectOptionalMovementProvider(IMovementProvider provider)
		{
			this._movementProvider = (provider as Object);
			this.MovementProvider = provider;
		}

		private Collider[] _colliders;

		[Tooltip("The RigidBody of the interactable.")]
		[SerializeField]
		private Rigidbody _rigidbody;

		[Tooltip("An optional origin point for the grab.")]
		[SerializeField]
		[Optional]
		private Transform _grabSource;

		[Tooltip("Forces a release on all other grabbing interactors when grabbed by a new interactor.")]
		[SerializeField]
		private bool _resetGrabOnGrabsUpdated = true;

		[Tooltip("PhysicsGrabbable used when you grab the interactable.")]
		[SerializeField]
		[Optional(OptionalAttribute.Flag.Obsolete)]
		[Obsolete("Use Grabbable and/or RigidbodyKinematicLocker instead")]
		private PhysicsGrabbable _physicsGrabbable;

		[Tooltip("The IMovementProvider specifies how the interactable will align with the grabber when selected. If no IMovementProvider is set, the MoveTowardsTargetProvider is created and used as the provider.")]
		[Header("Snap")]
		[SerializeField]
		[Optional]
		[Interface(typeof(IMovementProvider), new Type[]
		{

		})]
		private Object _movementProvider;
	}
}
