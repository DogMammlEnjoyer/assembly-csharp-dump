using System;
using System.Collections.Generic;
using Pathfinding.Util;
using UnityEngine;
using UnityEngine.Serialization;

namespace Pathfinding
{
	[RequireComponent(typeof(Seeker))]
	[AddComponentMenu("Pathfinding/AI/AILerp (2D,3D)")]
	[HelpURL("http://arongranberg.com/astar/documentation/stable/class_pathfinding_1_1_a_i_lerp.php")]
	public class AILerp : VersionedMonoBehaviour, IAstarAI
	{
		public float repathRate
		{
			get
			{
				return this.autoRepath.period;
			}
			set
			{
				this.autoRepath.period = value;
			}
		}

		public bool canSearch
		{
			get
			{
				return this.autoRepath.mode > AutoRepathPolicy.Mode.Never;
			}
			set
			{
				this.autoRepath.mode = (value ? AutoRepathPolicy.Mode.EveryNSeconds : AutoRepathPolicy.Mode.Never);
			}
		}

		[Obsolete("Use orientation instead")]
		public bool rotationIn2D
		{
			get
			{
				return this.orientation == OrientationMode.YAxisForward;
			}
			set
			{
				this.orientation = (value ? OrientationMode.YAxisForward : OrientationMode.ZAxisForward);
			}
		}

		public bool reachedEndOfPath { get; private set; }

		public bool reachedDestination
		{
			get
			{
				if (!this.reachedEndOfPath || !this.interpolator.valid)
				{
					return false;
				}
				Vector3 vector = this.destination - this.interpolator.endPoint;
				if (this.orientation == OrientationMode.YAxisForward)
				{
					vector.z = 0f;
				}
				else
				{
					vector.y = 0f;
				}
				return this.remainingDistance + vector.magnitude < 0.05f;
			}
		}

		public Vector3 destination { get; set; }

		[Obsolete("Use the destination property or the AIDestinationSetter component instead")]
		public Transform target
		{
			get
			{
				AIDestinationSetter component = base.GetComponent<AIDestinationSetter>();
				if (!(component != null))
				{
					return null;
				}
				return component.target;
			}
			set
			{
				this.targetCompatibility = null;
				AIDestinationSetter aidestinationSetter = base.GetComponent<AIDestinationSetter>();
				if (aidestinationSetter == null)
				{
					aidestinationSetter = base.gameObject.AddComponent<AIDestinationSetter>();
				}
				aidestinationSetter.target = value;
				this.destination = ((value != null) ? value.position : new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity));
			}
		}

		public Vector3 position
		{
			get
			{
				if (!this.updatePosition)
				{
					return this.simulatedPosition;
				}
				return this.tr.position;
			}
		}

		public Quaternion rotation
		{
			get
			{
				if (!this.updateRotation)
				{
					return this.simulatedRotation;
				}
				return this.tr.rotation;
			}
			set
			{
				if (this.updateRotation)
				{
					this.tr.rotation = value;
					return;
				}
				this.simulatedRotation = value;
			}
		}

		void IAstarAI.Move(Vector3 deltaPosition)
		{
		}

		float IAstarAI.radius
		{
			get
			{
				return 0f;
			}
			set
			{
			}
		}

		float IAstarAI.height
		{
			get
			{
				return 0f;
			}
			set
			{
			}
		}

		float IAstarAI.maxSpeed
		{
			get
			{
				return this.speed;
			}
			set
			{
				this.speed = value;
			}
		}

		bool IAstarAI.canSearch
		{
			get
			{
				return this.canSearch;
			}
			set
			{
				this.canSearch = value;
			}
		}

		bool IAstarAI.canMove
		{
			get
			{
				return this.canMove;
			}
			set
			{
				this.canMove = value;
			}
		}

		public Vector3 velocity
		{
			get
			{
				if (Time.deltaTime <= 1E-05f)
				{
					return Vector3.zero;
				}
				return (this.previousPosition1 - this.previousPosition2) / Time.deltaTime;
			}
		}

		Vector3 IAstarAI.desiredVelocity
		{
			get
			{
				return ((IAstarAI)this).velocity;
			}
		}

		Vector3 IAstarAI.steeringTarget
		{
			get
			{
				if (!this.interpolator.valid)
				{
					return this.simulatedPosition;
				}
				return this.interpolator.position + this.interpolator.tangent;
			}
		}

		public float remainingDistance
		{
			get
			{
				return Mathf.Max(this.interpolator.remainingDistance, 0f);
			}
			set
			{
				this.interpolator.remainingDistance = Mathf.Max(value, 0f);
			}
		}

		public bool hasPath
		{
			get
			{
				return this.interpolator.valid;
			}
		}

		public bool pathPending
		{
			get
			{
				return !this.canSearchAgain;
			}
		}

		public bool isStopped { get; set; }

		public Action onSearchPath { get; set; }

		protected AILerp()
		{
			this.destination = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
		}

		protected override void Awake()
		{
			base.Awake();
			this.tr = base.transform;
			this.seeker = base.GetComponent<Seeker>();
			this.seeker.startEndModifier.adjustStartPoint = (() => this.simulatedPosition);
		}

		protected virtual void Start()
		{
			this.startHasRun = true;
			this.Init();
		}

		protected virtual void OnEnable()
		{
			Seeker seeker = this.seeker;
			seeker.pathCallback = (OnPathDelegate)Delegate.Combine(seeker.pathCallback, new OnPathDelegate(this.OnPathComplete));
			this.Init();
		}

		private void Init()
		{
			if (this.startHasRun)
			{
				this.Teleport(this.position, false);
				this.autoRepath.Reset();
				if (this.shouldRecalculatePath)
				{
					this.SearchPath();
				}
			}
		}

		public void OnDisable()
		{
			this.ClearPath();
			Seeker seeker = this.seeker;
			seeker.pathCallback = (OnPathDelegate)Delegate.Remove(seeker.pathCallback, new OnPathDelegate(this.OnPathComplete));
		}

		public void GetRemainingPath(List<Vector3> buffer, out bool stale)
		{
			buffer.Clear();
			if (!this.interpolator.valid)
			{
				buffer.Add(this.position);
				stale = true;
				return;
			}
			stale = false;
			this.interpolator.GetRemainingPath(buffer);
			buffer[0] = this.position;
		}

		public void Teleport(Vector3 position, bool clearPath = true)
		{
			if (clearPath)
			{
				this.ClearPath();
			}
			this.previousPosition2 = position;
			this.previousPosition1 = position;
			this.simulatedPosition = position;
			if (this.updatePosition)
			{
				this.tr.position = position;
			}
			this.reachedEndOfPath = false;
			if (clearPath)
			{
				this.SearchPath();
			}
		}

		protected virtual bool shouldRecalculatePath
		{
			get
			{
				return this.canSearchAgain && this.autoRepath.ShouldRecalculatePath(this.position, 0f, this.destination);
			}
		}

		[Obsolete("Use SearchPath instead")]
		public virtual void ForceSearchPath()
		{
			this.SearchPath();
		}

		public virtual void SearchPath()
		{
			if (float.IsPositiveInfinity(this.destination.x))
			{
				return;
			}
			if (this.onSearchPath != null)
			{
				this.onSearchPath();
			}
			Vector3 feetPosition = this.GetFeetPosition();
			this.canSearchAgain = false;
			this.SetPath(ABPath.Construct(feetPosition, this.destination, null), false);
		}

		public virtual void OnTargetReached()
		{
		}

		protected virtual void OnPathComplete(Path _p)
		{
			ABPath abpath = _p as ABPath;
			if (abpath == null)
			{
				throw new Exception("This function only handles ABPaths, do not use special path types");
			}
			this.canSearchAgain = true;
			abpath.Claim(this);
			if (abpath.error)
			{
				abpath.Release(this, false);
				return;
			}
			if (this.interpolatePathSwitches)
			{
				this.ConfigurePathSwitchInterpolation();
			}
			ABPath abpath2 = this.path;
			this.path = abpath;
			this.reachedEndOfPath = false;
			RandomPath randomPath = this.path as RandomPath;
			if (randomPath != null)
			{
				this.destination = randomPath.originalEndPoint;
			}
			else
			{
				MultiTargetPath multiTargetPath = this.path as MultiTargetPath;
				if (multiTargetPath != null)
				{
					this.destination = multiTargetPath.originalEndPoint;
				}
			}
			if (this.path.vectorPath != null && this.path.vectorPath.Count == 1)
			{
				this.path.vectorPath.Insert(0, this.GetFeetPosition());
			}
			this.ConfigureNewPath();
			if (abpath2 != null)
			{
				abpath2.Release(this, false);
			}
			if (this.interpolator.remainingDistance < 0.0001f && !this.reachedEndOfPath)
			{
				this.reachedEndOfPath = true;
				this.OnTargetReached();
			}
		}

		protected virtual void ClearPath()
		{
			if (this.seeker != null)
			{
				this.seeker.CancelCurrentPathRequest(true);
			}
			this.canSearchAgain = true;
			this.reachedEndOfPath = false;
			if (this.path != null)
			{
				this.path.Release(this, false);
			}
			this.path = null;
			this.interpolator.SetPath(null);
		}

		public void SetPath(Path path, bool updateDestinationFromPath = true)
		{
			if (updateDestinationFromPath)
			{
				ABPath abpath = path as ABPath;
				if (abpath != null && !(path is RandomPath))
				{
					this.destination = abpath.originalEndPoint;
				}
			}
			if (path == null)
			{
				this.ClearPath();
				return;
			}
			if (path.PipelineState == PathState.Created)
			{
				this.canSearchAgain = false;
				this.seeker.CancelCurrentPathRequest(true);
				this.seeker.StartPath(path, null);
				this.autoRepath.DidRecalculatePath(this.destination);
				return;
			}
			if (path.PipelineState != PathState.Returned)
			{
				throw new ArgumentException("You must call the SetPath method with a path that either has been completely calculated or one whose path calculation has not been started at all. It looks like the path calculation for the path you tried to use has been started, but is not yet finished.");
			}
			if (this.seeker.GetCurrentPath() != path)
			{
				this.seeker.CancelCurrentPathRequest(true);
				this.OnPathComplete(path);
				return;
			}
			throw new ArgumentException("If you calculate the path using seeker.StartPath then this script will pick up the calculated path anyway as it listens for all paths the Seeker finishes calculating. You should not call SetPath in that case.");
		}

		protected virtual void ConfigurePathSwitchInterpolation()
		{
			bool flag = this.interpolator.valid && this.interpolator.remainingDistance < 0.0001f;
			if (this.interpolator.valid && !flag)
			{
				this.previousMovementOrigin = this.interpolator.position;
				this.previousMovementDirection = this.interpolator.tangent.normalized * this.interpolator.remainingDistance;
				this.pathSwitchInterpolationTime = 0f;
				return;
			}
			this.previousMovementOrigin = Vector3.zero;
			this.previousMovementDirection = Vector3.zero;
			this.pathSwitchInterpolationTime = float.PositiveInfinity;
		}

		public virtual Vector3 GetFeetPosition()
		{
			return this.position;
		}

		protected virtual void ConfigureNewPath()
		{
			bool valid = this.interpolator.valid;
			Vector3 vector = valid ? this.interpolator.tangent : Vector3.zero;
			this.interpolator.SetPath(this.path.vectorPath);
			this.interpolator.MoveToClosestPoint(this.GetFeetPosition());
			if (this.interpolatePathSwitches && this.switchPathInterpolationSpeed > 0.01f && valid)
			{
				float num = Mathf.Max(-Vector3.Dot(vector.normalized, this.interpolator.tangent.normalized), 0f);
				this.interpolator.distance -= this.speed * num * (1f / this.switchPathInterpolationSpeed);
			}
		}

		protected virtual void Update()
		{
			if (this.shouldRecalculatePath)
			{
				this.SearchPath();
			}
			if (this.canMove)
			{
				Vector3 nextPosition;
				Quaternion nextRotation;
				this.MovementUpdate(Time.deltaTime, out nextPosition, out nextRotation);
				this.FinalizeMovement(nextPosition, nextRotation);
			}
		}

		public void MovementUpdate(float deltaTime, out Vector3 nextPosition, out Quaternion nextRotation)
		{
			if (this.updatePosition)
			{
				this.simulatedPosition = this.tr.position;
			}
			if (this.updateRotation)
			{
				this.simulatedRotation = this.tr.rotation;
			}
			Vector3 direction;
			nextPosition = this.CalculateNextPosition(out direction, this.isStopped ? 0f : deltaTime);
			if (this.enableRotation)
			{
				nextRotation = this.SimulateRotationTowards(direction, deltaTime);
				return;
			}
			nextRotation = this.simulatedRotation;
		}

		public void FinalizeMovement(Vector3 nextPosition, Quaternion nextRotation)
		{
			this.previousPosition2 = this.previousPosition1;
			this.simulatedPosition = nextPosition;
			this.previousPosition1 = nextPosition;
			this.simulatedRotation = nextRotation;
			if (this.updatePosition)
			{
				this.tr.position = nextPosition;
			}
			if (this.updateRotation)
			{
				this.tr.rotation = nextRotation;
			}
		}

		private Quaternion SimulateRotationTowards(Vector3 direction, float deltaTime)
		{
			if (direction != Vector3.zero)
			{
				Quaternion quaternion = Quaternion.LookRotation(direction, (this.orientation == OrientationMode.YAxisForward) ? Vector3.back : Vector3.up);
				if (this.orientation == OrientationMode.YAxisForward)
				{
					quaternion *= Quaternion.Euler(90f, 0f, 0f);
				}
				return Quaternion.Slerp(this.simulatedRotation, quaternion, deltaTime * this.rotationSpeed);
			}
			return this.simulatedRotation;
		}

		protected virtual Vector3 CalculateNextPosition(out Vector3 direction, float deltaTime)
		{
			if (!this.interpolator.valid)
			{
				direction = Vector3.zero;
				return this.simulatedPosition;
			}
			this.interpolator.distance += deltaTime * this.speed;
			if (this.interpolator.remainingDistance < 0.0001f && !this.reachedEndOfPath)
			{
				this.reachedEndOfPath = true;
				this.OnTargetReached();
			}
			direction = this.interpolator.tangent;
			this.pathSwitchInterpolationTime += deltaTime;
			float num = this.switchPathInterpolationSpeed * this.pathSwitchInterpolationTime;
			if (this.interpolatePathSwitches && num < 1f)
			{
				return Vector3.Lerp(this.previousMovementOrigin + Vector3.ClampMagnitude(this.previousMovementDirection, this.speed * this.pathSwitchInterpolationTime), this.interpolator.position, num);
			}
			return this.interpolator.position;
		}

		protected override int OnUpgradeSerializedData(int version, bool unityThread)
		{
			if (unityThread && this.targetCompatibility != null)
			{
				this.target = this.targetCompatibility;
			}
			if (version <= 3)
			{
				this.repathRate = this.repathRateCompatibility;
				this.canSearch = this.canSearchCompability;
			}
			return 4;
		}

		public virtual void OnDrawGizmos()
		{
			this.tr = base.transform;
			this.autoRepath.DrawGizmos(this.position, 0f);
		}

		public AutoRepathPolicy autoRepath = new AutoRepathPolicy();

		public bool canMove = true;

		public float speed = 3f;

		[FormerlySerializedAs("rotationIn2D")]
		public OrientationMode orientation;

		public bool enableRotation = true;

		public float rotationSpeed = 10f;

		public bool interpolatePathSwitches = true;

		public float switchPathInterpolationSpeed = 5f;

		[NonSerialized]
		public bool updatePosition = true;

		[NonSerialized]
		public bool updateRotation = true;

		protected Seeker seeker;

		protected Transform tr;

		protected ABPath path;

		protected bool canSearchAgain = true;

		protected Vector3 previousMovementOrigin;

		protected Vector3 previousMovementDirection;

		protected float pathSwitchInterpolationTime;

		protected PathInterpolator interpolator = new PathInterpolator();

		private bool startHasRun;

		private Vector3 previousPosition1;

		private Vector3 previousPosition2;

		private Vector3 simulatedPosition;

		private Quaternion simulatedRotation;

		[FormerlySerializedAs("target")]
		[SerializeField]
		[HideInInspector]
		private Transform targetCompatibility;

		[SerializeField]
		[HideInInspector]
		[FormerlySerializedAs("repathRate")]
		private float repathRateCompatibility = float.NaN;

		[SerializeField]
		[HideInInspector]
		[FormerlySerializedAs("canSearch")]
		private bool canSearchCompability;
	}
}
