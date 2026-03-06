using System;
using System.Collections.Generic;
using Pathfinding.Util;
using UnityEngine;
using UnityEngine.Serialization;

namespace Pathfinding
{
	[AddComponentMenu("Pathfinding/AI/AIPath (2D,3D)")]
	public class AIPath : AIBase, IAstarAI
	{
		public override void Teleport(Vector3 newPosition, bool clearPath = true)
		{
			this.reachedEndOfPath = false;
			base.Teleport(newPosition, clearPath);
		}

		public float remainingDistance
		{
			get
			{
				if (!this.interpolator.valid)
				{
					return float.PositiveInfinity;
				}
				return this.interpolator.remainingDistance + this.movementPlane.ToPlane(this.interpolator.position - base.position).magnitude;
			}
		}

		public bool reachedDestination
		{
			get
			{
				if (!this.reachedEndOfPath)
				{
					return false;
				}
				if (!this.interpolator.valid || this.remainingDistance + this.movementPlane.ToPlane(base.destination - this.interpolator.endPoint).magnitude > this.endReachedDistance)
				{
					return false;
				}
				if (this.orientation != OrientationMode.YAxisForward)
				{
					float num;
					this.movementPlane.ToPlane(base.destination - base.position, out num);
					float num2 = this.tr.localScale.y * this.height;
					if (num > num2 || (double)num < (double)(-(double)num2) * 0.5)
					{
						return false;
					}
				}
				return true;
			}
		}

		public bool reachedEndOfPath { get; protected set; }

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
				return this.waitingForPathCalculation;
			}
		}

		public Vector3 steeringTarget
		{
			get
			{
				if (!this.interpolator.valid)
				{
					return base.position;
				}
				return this.interpolator.position;
			}
		}

		float IAstarAI.radius
		{
			get
			{
				return this.radius;
			}
			set
			{
				this.radius = value;
			}
		}

		float IAstarAI.height
		{
			get
			{
				return this.height;
			}
			set
			{
				this.height = value;
			}
		}

		float IAstarAI.maxSpeed
		{
			get
			{
				return this.maxSpeed;
			}
			set
			{
				this.maxSpeed = value;
			}
		}

		bool IAstarAI.canSearch
		{
			get
			{
				return base.canSearch;
			}
			set
			{
				base.canSearch = value;
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

		public void GetRemainingPath(List<Vector3> buffer, out bool stale)
		{
			buffer.Clear();
			buffer.Add(base.position);
			if (!this.interpolator.valid)
			{
				stale = true;
				return;
			}
			stale = false;
			this.interpolator.GetRemainingPath(buffer);
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			if (this.path != null)
			{
				this.path.Release(this, false);
			}
			this.path = null;
			this.interpolator.SetPath(null);
			this.reachedEndOfPath = false;
		}

		public virtual void OnTargetReached()
		{
		}

		protected override void OnPathComplete(Path newPath)
		{
			ABPath abpath = newPath as ABPath;
			if (abpath == null)
			{
				throw new Exception("This function only handles ABPaths, do not use special path types");
			}
			this.waitingForPathCalculation = false;
			abpath.Claim(this);
			if (abpath.error)
			{
				abpath.Release(this, false);
				base.SetPath(null, true);
				return;
			}
			if (this.path != null)
			{
				this.path.Release(this, false);
			}
			this.path = abpath;
			RandomPath randomPath = this.path as RandomPath;
			if (randomPath != null)
			{
				base.destination = randomPath.originalEndPoint;
			}
			else
			{
				MultiTargetPath multiTargetPath = this.path as MultiTargetPath;
				if (multiTargetPath != null)
				{
					base.destination = multiTargetPath.originalEndPoint;
				}
			}
			if (this.path.vectorPath.Count == 1)
			{
				this.path.vectorPath.Add(this.path.vectorPath[0]);
			}
			this.interpolator.SetPath(this.path.vectorPath);
			ITransformedGraph transformedGraph = (this.path.path.Count > 0) ? (AstarData.GetGraph(this.path.path[0]) as ITransformedGraph) : null;
			this.movementPlane = ((transformedGraph != null) ? transformedGraph.transform : ((this.orientation == OrientationMode.YAxisForward) ? new GraphTransform(Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(-90f, 270f, 90f), Vector3.one)) : GraphTransform.identityTransform));
			this.reachedEndOfPath = false;
			this.interpolator.MoveToLocallyClosestPoint((this.GetFeetPosition() + abpath.originalStartPoint) * 0.5f, true, true);
			this.interpolator.MoveToLocallyClosestPoint(this.GetFeetPosition(), true, true);
			this.interpolator.MoveToCircleIntersection2D(base.position, this.pickNextWaypointDist, this.movementPlane);
			if (this.remainingDistance <= this.endReachedDistance)
			{
				this.reachedEndOfPath = true;
				this.OnTargetReached();
			}
		}

		protected override void ClearPath()
		{
			base.CancelCurrentPathRequest();
			if (this.path != null)
			{
				this.path.Release(this, false);
			}
			this.path = null;
			this.interpolator.SetPath(null);
			this.reachedEndOfPath = false;
		}

		protected override void MovementUpdateInternal(float deltaTime, out Vector3 nextPosition, out Quaternion nextRotation)
		{
			float num = this.maxAcceleration;
			if (num < 0f)
			{
				num *= -this.maxSpeed;
			}
			if (this.updatePosition)
			{
				this.simulatedPosition = this.tr.position;
			}
			if (this.updateRotation)
			{
				this.simulatedRotation = this.tr.rotation;
			}
			Vector3 simulatedPosition = this.simulatedPosition;
			this.interpolator.MoveToCircleIntersection2D(simulatedPosition, this.pickNextWaypointDist, this.movementPlane);
			Vector2 deltaPosition = this.movementPlane.ToPlane(this.steeringTarget - simulatedPosition);
			float num2 = deltaPosition.magnitude + Mathf.Max(0f, this.interpolator.remainingDistance);
			bool reachedEndOfPath = this.reachedEndOfPath;
			this.reachedEndOfPath = (num2 <= this.endReachedDistance && this.interpolator.valid);
			if (!reachedEndOfPath && this.reachedEndOfPath)
			{
				this.OnTargetReached();
			}
			Vector2 vector = this.movementPlane.ToPlane(this.simulatedRotation * ((this.orientation == OrientationMode.YAxisForward) ? Vector3.up : Vector3.forward));
			bool flag = base.isStopped || (this.reachedDestination && this.whenCloseToDestination == CloseToDestinationMode.Stop);
			float num3;
			if (this.interpolator.valid && !flag)
			{
				num3 = ((num2 < this.slowdownDistance) ? Mathf.Sqrt(num2 / this.slowdownDistance) : 1f);
				if (this.reachedEndOfPath && this.whenCloseToDestination == CloseToDestinationMode.Stop)
				{
					this.velocity2D -= Vector2.ClampMagnitude(this.velocity2D, num * deltaTime);
				}
				else
				{
					this.velocity2D += MovementUtilities.CalculateAccelerationToReachPoint(deltaPosition, deltaPosition.normalized * this.maxSpeed, this.velocity2D, num, this.rotationSpeed, this.maxSpeed, vector) * deltaTime;
				}
			}
			else
			{
				num3 = 1f;
				this.velocity2D -= Vector2.ClampMagnitude(this.velocity2D, num * deltaTime);
			}
			this.velocity2D = MovementUtilities.ClampVelocity(this.velocity2D, this.maxSpeed, num3, this.slowWhenNotFacingTarget && this.enableRotation, vector);
			base.ApplyGravity(deltaTime);
			if (this.rvoController != null && this.rvoController.enabled)
			{
				Vector3 pos = simulatedPosition + this.movementPlane.ToWorld(Vector2.ClampMagnitude(this.velocity2D, num2), 0f);
				this.rvoController.SetTarget(pos, this.velocity2D.magnitude, this.maxSpeed);
			}
			Vector2 p = this.lastDeltaPosition = base.CalculateDeltaToMoveThisFrame(this.movementPlane.ToPlane(simulatedPosition), num2, deltaTime);
			nextPosition = simulatedPosition + this.movementPlane.ToWorld(p, this.verticalVelocity * this.lastDeltaTime);
			this.CalculateNextRotation(num3, out nextRotation);
		}

		protected virtual void CalculateNextRotation(float slowdown, out Quaternion nextRotation)
		{
			if (this.lastDeltaTime > 1E-05f && this.enableRotation)
			{
				Vector2 direction;
				if (this.rvoController != null && this.rvoController.enabled)
				{
					Vector2 b = this.lastDeltaPosition / this.lastDeltaTime;
					direction = Vector2.Lerp(this.velocity2D, b, 4f * b.magnitude / (this.maxSpeed + 0.0001f));
				}
				else
				{
					direction = this.velocity2D;
				}
				float num = this.rotationSpeed * Mathf.Max(0f, (slowdown - 0.3f) / 0.7f);
				nextRotation = base.SimulateRotationTowards(direction, num * this.lastDeltaTime);
				return;
			}
			nextRotation = base.rotation;
		}

		protected override Vector3 ClampToNavmesh(Vector3 position, out bool positionChanged)
		{
			if (this.constrainInsideGraph)
			{
				AIPath.cachedNNConstraint.tags = this.seeker.traversableTags;
				AIPath.cachedNNConstraint.graphMask = this.seeker.graphMask;
				AIPath.cachedNNConstraint.distanceXZ = true;
				Vector3 position2 = AstarPath.active.GetNearest(position, AIPath.cachedNNConstraint).position;
				Vector2 vector = this.movementPlane.ToPlane(position2 - position);
				float sqrMagnitude = vector.sqrMagnitude;
				if (sqrMagnitude > 1.0000001E-06f)
				{
					this.velocity2D -= vector * Vector2.Dot(vector, this.velocity2D) / sqrMagnitude;
					if (this.rvoController != null && this.rvoController.enabled)
					{
						this.rvoController.SetCollisionNormal(vector);
					}
					positionChanged = true;
					return position + this.movementPlane.ToWorld(vector, 0f);
				}
			}
			positionChanged = false;
			return position;
		}

		protected override int OnUpgradeSerializedData(int version, bool unityThread)
		{
			if (version < 1)
			{
				this.rotationSpeed *= 90f;
			}
			return base.OnUpgradeSerializedData(version, unityThread);
		}

		[Obsolete("When unifying the interfaces for different movement scripts, this property has been renamed to reachedEndOfPath.  [AstarUpgradable: 'TargetReached' -> 'reachedEndOfPath']")]
		public bool TargetReached
		{
			get
			{
				return this.reachedEndOfPath;
			}
		}

		[Obsolete("This field has been renamed to #rotationSpeed and is now in degrees per second instead of a damping factor")]
		public float turningSpeed
		{
			get
			{
				return this.rotationSpeed / 90f;
			}
			set
			{
				this.rotationSpeed = value * 90f;
			}
		}

		[Obsolete("This member has been deprecated. Use 'maxSpeed' instead. [AstarUpgradable: 'speed' -> 'maxSpeed']")]
		public float speed
		{
			get
			{
				return this.maxSpeed;
			}
			set
			{
				this.maxSpeed = value;
			}
		}

		[Obsolete("Only exists for compatibility reasons. Use desiredVelocity or steeringTarget instead.")]
		public Vector3 targetDirection
		{
			get
			{
				return (this.steeringTarget - this.tr.position).normalized;
			}
		}

		[Obsolete("This method no longer calculates the velocity. Use the desiredVelocity property instead")]
		public Vector3 CalculateVelocity(Vector3 position)
		{
			return base.desiredVelocity;
		}

		public float maxAcceleration = -2.5f;

		[FormerlySerializedAs("turningSpeed")]
		public float rotationSpeed = 360f;

		public float slowdownDistance = 0.6f;

		public float pickNextWaypointDist = 2f;

		public float endReachedDistance = 0.2f;

		public bool alwaysDrawGizmos;

		public bool slowWhenNotFacingTarget = true;

		public CloseToDestinationMode whenCloseToDestination;

		public bool constrainInsideGraph;

		protected Path path;

		protected PathInterpolator interpolator = new PathInterpolator();

		private static NNConstraint cachedNNConstraint = NNConstraint.Default;
	}
}
