using System;
using Pathfinding.RVO;
using Pathfinding.Util;
using UnityEngine;
using UnityEngine.Serialization;

namespace Pathfinding
{
	[RequireComponent(typeof(Seeker))]
	public abstract class AIBase : VersionedMonoBehaviour
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
				if (value)
				{
					if (this.autoRepath.mode == AutoRepathPolicy.Mode.Never)
					{
						this.autoRepath.mode = AutoRepathPolicy.Mode.EveryNSeconds;
						return;
					}
				}
				else
				{
					this.autoRepath.mode = AutoRepathPolicy.Mode.Never;
				}
			}
		}

		[Obsolete("Use the height property instead (2x this value)")]
		public float centerOffset
		{
			get
			{
				return this.height * 0.5f;
			}
			set
			{
				this.height = value * 2f;
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

		protected bool usingGravity { get; set; }

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

		public Vector3 destination { get; set; }

		public Vector3 velocity
		{
			get
			{
				if (this.lastDeltaTime <= 1E-06f)
				{
					return Vector3.zero;
				}
				return (this.prevPosition1 - this.prevPosition2) / this.lastDeltaTime;
			}
		}

		public Vector3 desiredVelocity
		{
			get
			{
				if (this.lastDeltaTime <= 1E-05f)
				{
					return Vector3.zero;
				}
				return this.movementPlane.ToWorld(this.lastDeltaPosition / this.lastDeltaTime, this.verticalVelocity);
			}
		}

		public bool isStopped { get; set; }

		public Action onSearchPath { get; set; }

		protected virtual bool shouldRecalculatePath
		{
			get
			{
				return !this.waitingForPathCalculation && this.autoRepath.ShouldRecalculatePath(this.position, this.radius, this.destination);
			}
		}

		protected AIBase()
		{
			this.destination = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
		}

		public virtual void FindComponents()
		{
			this.tr = base.transform;
			this.seeker = base.GetComponent<Seeker>();
			this.rvoController = base.GetComponent<RVOController>();
			this.controller = base.GetComponent<CharacterController>();
			this.rigid = base.GetComponent<Rigidbody>();
			this.rigid2D = base.GetComponent<Rigidbody2D>();
		}

		protected virtual void OnEnable()
		{
			this.FindComponents();
			Seeker seeker = this.seeker;
			seeker.pathCallback = (OnPathDelegate)Delegate.Combine(seeker.pathCallback, new OnPathDelegate(this.OnPathComplete));
			this.Init();
		}

		protected virtual void Start()
		{
			this.startHasRun = true;
			this.Init();
		}

		private void Init()
		{
			if (this.startHasRun)
			{
				if (this.canMove)
				{
					this.Teleport(this.position, false);
				}
				this.autoRepath.Reset();
				if (this.shouldRecalculatePath)
				{
					this.SearchPath();
				}
			}
		}

		public virtual void Teleport(Vector3 newPosition, bool clearPath = true)
		{
			if (clearPath)
			{
				this.ClearPath();
			}
			this.simulatedPosition = newPosition;
			this.prevPosition2 = newPosition;
			this.prevPosition1 = newPosition;
			if (this.updatePosition)
			{
				this.tr.position = newPosition;
			}
			if (this.rvoController != null)
			{
				this.rvoController.Move(Vector3.zero);
			}
			if (clearPath)
			{
				this.SearchPath();
			}
		}

		protected void CancelCurrentPathRequest()
		{
			this.waitingForPathCalculation = false;
			if (this.seeker != null)
			{
				this.seeker.CancelCurrentPathRequest(true);
			}
		}

		protected virtual void OnDisable()
		{
			this.ClearPath();
			Seeker seeker = this.seeker;
			seeker.pathCallback = (OnPathDelegate)Delegate.Remove(seeker.pathCallback, new OnPathDelegate(this.OnPathComplete));
			this.velocity2D = Vector3.zero;
			this.accumulatedMovementDelta = Vector3.zero;
			this.verticalVelocity = 0f;
			this.lastDeltaTime = 0f;
		}

		protected virtual void Update()
		{
			if (this.shouldRecalculatePath)
			{
				this.SearchPath();
			}
			this.usingGravity = (!(this.gravity == Vector3.zero) && (!this.updatePosition || ((this.rigid == null || this.rigid.isKinematic) && (this.rigid2D == null || this.rigid2D.isKinematic))));
			if (this.rigid == null && this.rigid2D == null && this.canMove)
			{
				Vector3 nextPosition;
				Quaternion nextRotation;
				this.MovementUpdate(Time.deltaTime, out nextPosition, out nextRotation);
				this.FinalizeMovement(nextPosition, nextRotation);
			}
		}

		protected virtual void FixedUpdate()
		{
			if ((!(this.rigid == null) || !(this.rigid2D == null)) && this.canMove)
			{
				Vector3 nextPosition;
				Quaternion nextRotation;
				this.MovementUpdate(Time.fixedDeltaTime, out nextPosition, out nextRotation);
				this.FinalizeMovement(nextPosition, nextRotation);
			}
		}

		public void MovementUpdate(float deltaTime, out Vector3 nextPosition, out Quaternion nextRotation)
		{
			this.lastDeltaTime = deltaTime;
			this.MovementUpdateInternal(deltaTime, out nextPosition, out nextRotation);
		}

		protected abstract void MovementUpdateInternal(float deltaTime, out Vector3 nextPosition, out Quaternion nextRotation);

		protected virtual void CalculatePathRequestEndpoints(out Vector3 start, out Vector3 end)
		{
			start = this.GetFeetPosition();
			end = this.destination;
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
			Vector3 start;
			Vector3 end;
			this.CalculatePathRequestEndpoints(out start, out end);
			ABPath path = ABPath.Construct(start, end, null);
			this.SetPath(path, false);
		}

		public virtual Vector3 GetFeetPosition()
		{
			return this.position;
		}

		protected abstract void OnPathComplete(Path newPath);

		protected abstract void ClearPath();

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
				this.CancelCurrentPathRequest();
				this.ClearPath();
				return;
			}
			if (path.PipelineState == PathState.Created)
			{
				this.waitingForPathCalculation = true;
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

		protected void ApplyGravity(float deltaTime)
		{
			if (this.usingGravity)
			{
				float num;
				this.velocity2D += this.movementPlane.ToPlane(deltaTime * (float.IsNaN(this.gravity.x) ? Physics.gravity : this.gravity), out num);
				this.verticalVelocity += num;
				return;
			}
			this.verticalVelocity = 0f;
		}

		protected Vector2 CalculateDeltaToMoveThisFrame(Vector2 position, float distanceToEndOfPath, float deltaTime)
		{
			if (this.rvoController != null && this.rvoController.enabled)
			{
				return this.movementPlane.ToPlane(this.rvoController.CalculateMovementDelta(this.movementPlane.ToWorld(position, 0f), deltaTime));
			}
			return Vector2.ClampMagnitude(this.velocity2D * deltaTime, distanceToEndOfPath);
		}

		public Quaternion SimulateRotationTowards(Vector3 direction, float maxDegrees)
		{
			return this.SimulateRotationTowards(this.movementPlane.ToPlane(direction), maxDegrees);
		}

		protected Quaternion SimulateRotationTowards(Vector2 direction, float maxDegrees)
		{
			if (direction != Vector2.zero)
			{
				Quaternion quaternion = Quaternion.LookRotation(this.movementPlane.ToWorld(direction, 0f), this.movementPlane.ToWorld(Vector2.zero, 1f));
				if (this.orientation == OrientationMode.YAxisForward)
				{
					quaternion *= Quaternion.Euler(90f, 0f, 0f);
				}
				return Quaternion.RotateTowards(this.simulatedRotation, quaternion, maxDegrees);
			}
			return this.simulatedRotation;
		}

		public virtual void Move(Vector3 deltaPosition)
		{
			this.accumulatedMovementDelta += deltaPosition;
		}

		public virtual void FinalizeMovement(Vector3 nextPosition, Quaternion nextRotation)
		{
			if (this.enableRotation)
			{
				this.FinalizeRotation(nextRotation);
			}
			this.FinalizePosition(nextPosition);
		}

		private void FinalizeRotation(Quaternion nextRotation)
		{
			this.simulatedRotation = nextRotation;
			if (this.updateRotation)
			{
				if (this.rigid != null)
				{
					this.rigid.MoveRotation(nextRotation);
					return;
				}
				if (this.rigid2D != null)
				{
					this.rigid2D.MoveRotation(nextRotation.eulerAngles.z);
					return;
				}
				this.tr.rotation = nextRotation;
			}
		}

		private void FinalizePosition(Vector3 nextPosition)
		{
			Vector3 vector = this.simulatedPosition;
			bool flag = false;
			if (this.controller != null && this.controller.enabled && this.updatePosition)
			{
				this.tr.position = vector;
				this.controller.Move(nextPosition - vector + this.accumulatedMovementDelta);
				vector = this.tr.position;
				if (this.controller.isGrounded)
				{
					this.verticalVelocity = 0f;
				}
			}
			else
			{
				float lastElevation;
				this.movementPlane.ToPlane(vector, out lastElevation);
				vector = nextPosition + this.accumulatedMovementDelta;
				if (this.usingGravity)
				{
					vector = this.RaycastPosition(vector, lastElevation);
				}
				flag = true;
			}
			bool flag2 = false;
			vector = this.ClampToNavmesh(vector, out flag2);
			if ((flag || flag2) && this.updatePosition)
			{
				if (this.rigid != null)
				{
					this.rigid.MovePosition(vector);
				}
				else if (this.rigid2D != null)
				{
					this.rigid2D.MovePosition(vector);
				}
				else
				{
					this.tr.position = vector;
				}
			}
			this.accumulatedMovementDelta = Vector3.zero;
			this.simulatedPosition = vector;
			this.UpdateVelocity();
		}

		protected void UpdateVelocity()
		{
			int frameCount = Time.frameCount;
			if (frameCount != this.prevFrame)
			{
				this.prevPosition2 = this.prevPosition1;
			}
			this.prevPosition1 = this.position;
			this.prevFrame = frameCount;
		}

		protected virtual Vector3 ClampToNavmesh(Vector3 position, out bool positionChanged)
		{
			positionChanged = false;
			return position;
		}

		protected Vector3 RaycastPosition(Vector3 position, float lastElevation)
		{
			float num;
			this.movementPlane.ToPlane(position, out num);
			float num2 = this.tr.localScale.y * this.height * 0.5f + Mathf.Max(0f, lastElevation - num);
			Vector3 vector = this.movementPlane.ToWorld(Vector2.zero, num2);
			RaycastHit raycastHit;
			if (Physics.Raycast(position + vector, -vector, out raycastHit, num2, this.groundMask, QueryTriggerInteraction.Ignore))
			{
				this.verticalVelocity *= Math.Max(0f, 1f - 5f * this.lastDeltaTime);
				return raycastHit.point;
			}
			return position;
		}

		protected virtual void OnDrawGizmosSelected()
		{
			if (Application.isPlaying)
			{
				this.FindComponents();
			}
		}

		protected virtual void OnDrawGizmos()
		{
			if (!Application.isPlaying || !base.enabled)
			{
				this.FindComponents();
			}
			Color color = AIBase.ShapeGizmoColor;
			if (this.rvoController != null && this.rvoController.locked)
			{
				color *= 0.5f;
			}
			if (this.orientation == OrientationMode.YAxisForward)
			{
				Draw.Gizmos.Cylinder(this.position, Vector3.forward, 0f, this.radius * this.tr.localScale.x, color);
			}
			else
			{
				Draw.Gizmos.Cylinder(this.position, this.rotation * Vector3.up, this.tr.localScale.y * this.height, this.radius * this.tr.localScale.x, color);
			}
			if (!float.IsPositiveInfinity(this.destination.x) && Application.isPlaying)
			{
				Draw.Gizmos.CircleXZ(this.destination, 0.2f, Color.blue, 0f, 6.2831855f);
			}
			this.autoRepath.DrawGizmos(this.position, this.radius);
		}

		protected override void Reset()
		{
			this.ResetShape();
			base.Reset();
		}

		private void ResetShape()
		{
			CharacterController component = base.GetComponent<CharacterController>();
			if (component != null)
			{
				this.radius = component.radius;
				this.height = Mathf.Max(this.radius * 2f, component.height);
			}
		}

		protected override int OnUpgradeSerializedData(int version, bool unityThread)
		{
			if (unityThread && !float.IsNaN(this.centerOffsetCompatibility))
			{
				this.height = this.centerOffsetCompatibility * 2f;
				this.ResetShape();
				RVOController component = base.GetComponent<RVOController>();
				if (component != null)
				{
					this.radius = component.radiusBackingField;
				}
				this.centerOffsetCompatibility = float.NaN;
			}
			if (unityThread && this.targetCompatibility != null)
			{
				this.target = this.targetCompatibility;
			}
			if (version <= 3)
			{
				this.repathRate = this.repathRateCompatibility;
				this.canSearch = this.canSearchCompability;
			}
			return 5;
		}

		public float radius = 0.5f;

		public float height = 2f;

		public bool canMove = true;

		[FormerlySerializedAs("speed")]
		public float maxSpeed = 1f;

		public Vector3 gravity = new Vector3(float.NaN, float.NaN, float.NaN);

		public LayerMask groundMask = -1;

		[SerializeField]
		[HideInInspector]
		[FormerlySerializedAs("centerOffset")]
		private float centerOffsetCompatibility = float.NaN;

		[SerializeField]
		[HideInInspector]
		[FormerlySerializedAs("repathRate")]
		private float repathRateCompatibility = float.NaN;

		[SerializeField]
		[HideInInspector]
		[FormerlySerializedAs("canSearch")]
		[FormerlySerializedAs("repeatedlySearchPaths")]
		private bool canSearchCompability;

		[FormerlySerializedAs("rotationIn2D")]
		public OrientationMode orientation;

		public bool enableRotation = true;

		protected Vector3 simulatedPosition;

		protected Quaternion simulatedRotation;

		private Vector3 accumulatedMovementDelta = Vector3.zero;

		protected Vector2 velocity2D;

		protected float verticalVelocity;

		protected Seeker seeker;

		protected Transform tr;

		protected Rigidbody rigid;

		protected Rigidbody2D rigid2D;

		protected CharacterController controller;

		protected RVOController rvoController;

		public IMovementPlane movementPlane = GraphTransform.identityTransform;

		[NonSerialized]
		public bool updatePosition = true;

		[NonSerialized]
		public bool updateRotation = true;

		public AutoRepathPolicy autoRepath = new AutoRepathPolicy();

		protected float lastDeltaTime;

		protected int prevFrame;

		protected Vector3 prevPosition1;

		protected Vector3 prevPosition2;

		protected Vector2 lastDeltaPosition;

		protected bool waitingForPathCalculation;

		[FormerlySerializedAs("target")]
		[SerializeField]
		[HideInInspector]
		private Transform targetCompatibility;

		private bool startHasRun;

		public static readonly Color ShapeGizmoColor = new Color(0.9411765f, 0.8352941f, 0.11764706f);
	}
}
