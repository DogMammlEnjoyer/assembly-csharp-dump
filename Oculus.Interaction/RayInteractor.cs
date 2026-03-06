using System;
using Oculus.Interaction.Surfaces;
using UnityEngine;

namespace Oculus.Interaction
{
	public class RayInteractor : PointerInteractor<RayInteractor, RayInteractable>
	{
		public Vector3 Origin { get; protected set; }

		public Quaternion Rotation { get; protected set; }

		public Vector3 Forward { get; protected set; }

		public Vector3 End { get; set; }

		public float MaxRayLength
		{
			get
			{
				return this._maxRayLength;
			}
			set
			{
				this._maxRayLength = value;
			}
		}

		public SurfaceHit? CollisionInfo { get; protected set; }

		public Ray Ray { get; protected set; }

		protected override void Awake()
		{
			base.Awake();
			base.Selector = (this._selector as ISelector);
			this._nativeId = 5936159140244058656UL;
		}

		protected override void Start()
		{
			base.Start();
		}

		protected override void DoPreprocess()
		{
			Transform transform = this._rayOrigin.transform;
			this.Origin = transform.position;
			this.Rotation = transform.rotation;
			this.Forward = transform.forward;
			this.End = this.Origin + this.MaxRayLength * this.Forward;
			this.Ray = new Ray(this.Origin, this.Forward);
		}

		public override object CandidateProperties
		{
			get
			{
				return this._rayCandidateProperties;
			}
		}

		protected override RayInteractable ComputeCandidate()
		{
			this.CollisionInfo = null;
			RayInteractable rayInteractable = null;
			float num = float.MaxValue;
			Vector3 candidatePosition = Vector3.zero;
			foreach (RayInteractable rayInteractable2 in Interactable<RayInteractor, RayInteractable>.Registry.List(this))
			{
				RayInteractable rayInteractable3 = rayInteractable2;
				Ray ray = this.Ray;
				float maxRayLength = this.MaxRayLength;
				SurfaceHit value;
				if (rayInteractable3.Raycast(ray, out value, maxRayLength, false))
				{
					bool flag = Mathf.Abs(value.Distance - num) < this._equalDistanceThreshold;
					if ((!flag && value.Distance < num) || (flag && this.ComputeCandidateTiebreaker(rayInteractable2, rayInteractable) > 0))
					{
						num = value.Distance;
						rayInteractable = rayInteractable2;
						this.CollisionInfo = new SurfaceHit?(value);
						candidatePosition = value.Point;
					}
				}
			}
			float d = (rayInteractable != null) ? num : this.MaxRayLength;
			this.End = this.Origin + d * this.Forward;
			this._rayCandidateProperties = new RayInteractor.RayCandidateProperties(rayInteractable, candidatePosition);
			return rayInteractable;
		}

		protected override int ComputeCandidateTiebreaker(RayInteractable a, RayInteractable b)
		{
			int num = base.ComputeCandidateTiebreaker(a, b);
			if (num != 0)
			{
				return num;
			}
			return a.TiebreakerScore.CompareTo(b.TiebreakerScore);
		}

		protected override void InteractableSelected(RayInteractable interactable)
		{
			if (interactable != null)
			{
				this._movedHit = this.CollisionInfo.Value;
				Pose pose = new Pose(this._movedHit.Point, Quaternion.LookRotation(this._movedHit.Normal));
				Pose pose2 = new Pose(this._movedHit.Point, Quaternion.LookRotation(-this._movedHit.Normal));
				Pose pose3 = this._rayOrigin.GetPose(Space.World);
				this._movement = interactable.GenerateMovement(pose3, pose2);
				if (this._movement != null)
				{
					pose3 = this._movement.Pose;
					this._movementHitDelta = PoseUtils.Delta(pose3, pose);
				}
			}
			base.InteractableSelected(interactable);
		}

		protected override void InteractableUnselected(RayInteractable interactable)
		{
			if (this._movement != null)
			{
				this._movement.StopAndSetPose(this._movement.Pose);
			}
			base.InteractableUnselected(interactable);
			this._movement = null;
		}

		protected override void DoSelectUpdate()
		{
			RayInteractable selectedInteractable = this._selectedInteractable;
			if (this._movement != null)
			{
				this._movement.UpdateTarget(this._rayOrigin.GetPose(Space.World));
				this._movement.Tick();
				Pose pose = this._movement.Pose;
				Pose pose2 = PoseUtils.Multiply(pose, this._movementHitDelta);
				this._movedHit.Point = pose2.position;
				this._movedHit.Normal = pose2.forward;
				this.CollisionInfo = new SurfaceHit?(this._movedHit);
				this.End = this._movedHit.Point;
				return;
			}
			this.CollisionInfo = null;
			if (selectedInteractable != null)
			{
				RayInteractable rayInteractable = selectedInteractable;
				Ray ray = this.Ray;
				float maxRayLength = this.MaxRayLength;
				SurfaceHit value;
				if (rayInteractable.Raycast(ray, out value, maxRayLength, true))
				{
					this.End = value.Point;
					this.CollisionInfo = new SurfaceHit?(value);
					return;
				}
			}
			this.End = this.Origin + this.MaxRayLength * this.Forward;
		}

		protected override Pose ComputePointerPose()
		{
			if (this._movement != null)
			{
				return this._movement.Pose;
			}
			if (this.CollisionInfo != null)
			{
				Vector3 point = this.CollisionInfo.Value.Point;
				Quaternion rotation = Quaternion.LookRotation(this.CollisionInfo.Value.Normal);
				return new Pose(point, rotation);
			}
			return Pose.identity;
		}

		public void InjectAllRayInteractor(ISelector selector, Transform rayOrigin)
		{
			this.InjectSelector(selector);
			this.InjectRayOrigin(rayOrigin);
		}

		public void InjectSelector(ISelector selector)
		{
			this._selector = (selector as Object);
			base.Selector = selector;
		}

		public void InjectRayOrigin(Transform rayOrigin)
		{
			this._rayOrigin = rayOrigin;
		}

		public void InjectOptionalEqualDistanceThreshold(float equalDistanceThreshold)
		{
			this._equalDistanceThreshold = equalDistanceThreshold;
		}

		[Tooltip("A selector indicating when the Interactor should select or unselect the best available interactable.")]
		[SerializeField]
		[Interface(typeof(ISelector), new Type[]
		{

		})]
		private Object _selector;

		[Tooltip("The origin of the ray.")]
		[SerializeField]
		private Transform _rayOrigin;

		[Tooltip("The maximum length of the ray.")]
		[SerializeField]
		private float _maxRayLength = 5f;

		[SerializeField]
		[Tooltip("(Meters, World) The threshold below which distances to a surface are treated as equal for the purposes of ranking.")]
		private float _equalDistanceThreshold = 0.001f;

		private RayInteractor.RayCandidateProperties _rayCandidateProperties;

		private IMovement _movement;

		private SurfaceHit _movedHit;

		private Pose _movementHitDelta = Pose.identity;

		public class RayCandidateProperties : ICandidatePosition
		{
			public RayInteractable ClosestInteractable { get; }

			public Vector3 CandidatePosition { get; }

			public RayCandidateProperties(RayInteractable closestInteractable, Vector3 candidatePosition)
			{
				this.ClosestInteractable = closestInteractable;
				this.CandidatePosition = candidatePosition;
			}
		}
	}
}
