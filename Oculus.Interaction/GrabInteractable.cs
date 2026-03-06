using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public class GrabInteractable : PointerInteractable<GrabInteractor, GrabInteractable>, IRigidbodyRef, ICollidersRef
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

		public bool UseClosestPointAsGrabSource
		{
			get
			{
				return this._useClosestPointAsGrabSource;
			}
			set
			{
				this._useClosestPointAsGrabSource = value;
			}
		}

		public float ReleaseDistance
		{
			get
			{
				return this._releaseDistance;
			}
			set
			{
				this._releaseDistance = value;
			}
		}

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

		protected override void Awake()
		{
			base.Awake();
		}

		protected override void Start()
		{
			this.BeginStart(ref this._started, delegate
			{
				base.Start();
			});
			if (GrabInteractable._grabRegistry == null)
			{
				GrabInteractable._grabRegistry = new CollisionInteractionRegistry<GrabInteractor, GrabInteractable>();
				this.SetRegistry(GrabInteractable._grabRegistry);
			}
			this._colliders = this.Rigidbody.GetComponentsInChildren<Collider>();
			this.EndStart(ref this._started);
		}

		public Pose GetGrabSourceForTarget(Pose target)
		{
			if (this._grabSource == null && !this._useClosestPointAsGrabSource)
			{
				return target;
			}
			if (this._useClosestPointAsGrabSource)
			{
				return new Pose(Collisions.ClosestPointToColliders(target.position, this._colliders), target.rotation);
			}
			return this._grabSource.GetPose(Space.World);
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

		public void InjectOptionalReleaseDistance(float releaseDistance)
		{
			this._releaseDistance = releaseDistance;
		}

		[Obsolete("Use Grabbable instead")]
		public void InjectOptionalPhysicsGrabbable(PhysicsGrabbable physicsGrabbable)
		{
			this._physicsGrabbable = physicsGrabbable;
		}

		private Collider[] _colliders;

		[Tooltip("The Rigidbody of the object.")]
		[SerializeField]
		private Rigidbody _rigidbody;

		[Tooltip("An optional origin point for the grab.")]
		[SerializeField]
		[Optional]
		private Transform _grabSource;

		[Tooltip("If true, use the closest point to the interactor as the grab source.")]
		[SerializeField]
		private bool _useClosestPointAsGrabSource;

		[Tooltip(" ")]
		[SerializeField]
		private float _releaseDistance;

		[Tooltip("Forces a release on all other grabbing interactors when grabbed by a new interactor.")]
		[SerializeField]
		private bool _resetGrabOnGrabsUpdated = true;

		[Tooltip("The PhysicsGrabbable used when you grab the interactable.")]
		[SerializeField]
		[Optional(OptionalAttribute.Flag.Obsolete)]
		[Obsolete("Use Grabbable and/or RigidbodyKinematicLocker instead")]
		private PhysicsGrabbable _physicsGrabbable;

		private static CollisionInteractionRegistry<GrabInteractor, GrabInteractable> _grabRegistry;
	}
}
