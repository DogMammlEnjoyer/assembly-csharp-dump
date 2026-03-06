using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding.Examples;
using Pathfinding.Util;
using UnityEngine;
using UnityEngine.Serialization;

namespace Pathfinding
{
	[AddComponentMenu("Pathfinding/AI/RichAI (3D, for navmesh)")]
	public class RichAI : AIBase, IAstarAI
	{
		public bool traversingOffMeshLink { get; protected set; }

		public float remainingDistance
		{
			get
			{
				return this.distanceToSteeringTarget + Vector3.Distance(this.steeringTarget, this.richPath.Endpoint);
			}
		}

		public bool reachedEndOfPath
		{
			get
			{
				return this.approachingPathEndpoint && this.distanceToSteeringTarget < this.endReachedDistance;
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
				if (this.approachingPathEndpoint && this.distanceToSteeringTarget + this.movementPlane.ToPlane(base.destination - this.richPath.Endpoint).magnitude > this.endReachedDistance)
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

		public bool hasPath
		{
			get
			{
				return this.richPath.GetCurrentPart() != null;
			}
		}

		public bool pathPending
		{
			get
			{
				return this.waitingForPathCalculation || this.delayUpdatePath;
			}
		}

		public Vector3 steeringTarget { get; protected set; }

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

		public bool approachingPartEndpoint
		{
			get
			{
				return this.lastCorner && this.nextCorners.Count == 1;
			}
		}

		public bool approachingPathEndpoint
		{
			get
			{
				return this.approachingPartEndpoint && this.richPath.IsLastPart;
			}
		}

		public override void Teleport(Vector3 newPosition, bool clearPath = true)
		{
			NNInfo nninfo = (AstarPath.active != null) ? AstarPath.active.GetNearest(newPosition) : default(NNInfo);
			float elevation;
			this.movementPlane.ToPlane(newPosition, out elevation);
			newPosition = this.movementPlane.ToWorld(this.movementPlane.ToPlane((nninfo.node != null) ? nninfo.position : newPosition), elevation);
			base.Teleport(newPosition, clearPath);
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			this.traversingOffMeshLink = false;
			base.StopAllCoroutines();
		}

		protected override bool shouldRecalculatePath
		{
			get
			{
				return base.shouldRecalculatePath && !this.traversingOffMeshLink;
			}
		}

		public override void SearchPath()
		{
			if (this.traversingOffMeshLink)
			{
				this.delayUpdatePath = true;
				return;
			}
			base.SearchPath();
		}

		protected override void OnPathComplete(Path p)
		{
			this.waitingForPathCalculation = false;
			p.Claim(this);
			if (p.error)
			{
				p.Release(this, false);
				return;
			}
			if (this.traversingOffMeshLink)
			{
				this.delayUpdatePath = true;
			}
			else
			{
				RandomPath randomPath = p as RandomPath;
				if (randomPath != null)
				{
					base.destination = randomPath.originalEndPoint;
				}
				else
				{
					MultiTargetPath multiTargetPath = p as MultiTargetPath;
					if (multiTargetPath != null)
					{
						base.destination = multiTargetPath.originalEndPoint;
					}
				}
				this.richPath.Initialize(this.seeker, p, true, this.funnelSimplification);
				RichFunnel richFunnel = this.richPath.GetCurrentPart() as RichFunnel;
				if (richFunnel != null)
				{
					if (this.updatePosition)
					{
						this.simulatedPosition = this.tr.position;
					}
					Vector2 b = this.movementPlane.ToPlane(this.UpdateTarget(richFunnel));
					this.steeringTarget = this.nextCorners[0];
					Vector2 a = this.movementPlane.ToPlane(this.steeringTarget);
					this.distanceToSteeringTarget = (a - b).magnitude;
					if (this.lastCorner && this.nextCorners.Count == 1 && this.distanceToSteeringTarget <= this.endReachedDistance)
					{
						this.NextPart();
					}
				}
			}
			p.Release(this, false);
		}

		protected override void ClearPath()
		{
			base.CancelCurrentPathRequest();
			this.richPath.Clear();
			this.lastCorner = false;
			this.delayUpdatePath = false;
			this.distanceToSteeringTarget = float.PositiveInfinity;
		}

		protected void NextPart()
		{
			if (!this.richPath.CompletedAllParts)
			{
				if (!this.richPath.IsLastPart)
				{
					this.lastCorner = false;
				}
				this.richPath.NextPart();
				if (this.richPath.CompletedAllParts)
				{
					this.OnTargetReached();
				}
			}
		}

		public void GetRemainingPath(List<Vector3> buffer, out bool stale)
		{
			this.richPath.GetRemainingPath(buffer, this.simulatedPosition, out stale);
		}

		protected virtual void OnTargetReached()
		{
		}

		protected virtual Vector3 UpdateTarget(RichFunnel fn)
		{
			this.nextCorners.Clear();
			bool flag;
			Vector3 result = fn.Update(this.simulatedPosition, this.nextCorners, 2, out this.lastCorner, out flag);
			if (flag && !this.waitingForPathCalculation && base.canSearch)
			{
				this.SearchPath();
			}
			return result;
		}

		protected override void MovementUpdateInternal(float deltaTime, out Vector3 nextPosition, out Quaternion nextRotation)
		{
			if (this.updatePosition)
			{
				this.simulatedPosition = this.tr.position;
			}
			if (this.updateRotation)
			{
				this.simulatedRotation = this.tr.rotation;
			}
			RichPathPart currentPart = this.richPath.GetCurrentPart();
			if (currentPart is RichSpecial)
			{
				if (!this.traversingOffMeshLink && !this.richPath.CompletedAllParts)
				{
					base.StartCoroutine(this.TraverseSpecial(currentPart as RichSpecial));
				}
				nextPosition = (this.steeringTarget = this.simulatedPosition);
				nextRotation = base.rotation;
				return;
			}
			RichFunnel richFunnel = currentPart as RichFunnel;
			if (richFunnel != null && !base.isStopped)
			{
				this.TraverseFunnel(richFunnel, deltaTime, out nextPosition, out nextRotation);
				return;
			}
			this.velocity2D -= Vector2.ClampMagnitude(this.velocity2D, this.acceleration * deltaTime);
			this.FinalMovement(this.simulatedPosition, deltaTime, float.PositiveInfinity, 1f, out nextPosition, out nextRotation);
			this.steeringTarget = this.simulatedPosition;
		}

		private void TraverseFunnel(RichFunnel fn, float deltaTime, out Vector3 nextPosition, out Quaternion nextRotation)
		{
			Vector3 vector = this.UpdateTarget(fn);
			float elevation;
			Vector2 vector2 = this.movementPlane.ToPlane(vector, out elevation);
			if (Time.frameCount % 5 == 0 && this.wallForce > 0f && this.wallDist > 0f)
			{
				this.wallBuffer.Clear();
				fn.FindWalls(this.wallBuffer, this.wallDist);
			}
			this.steeringTarget = this.nextCorners[0];
			Vector2 vector3 = this.movementPlane.ToPlane(this.steeringTarget);
			Vector2 vector4 = vector3 - vector2;
			Vector2 vector5 = VectorMath.Normalize(vector4, out this.distanceToSteeringTarget);
			Vector2 a = this.CalculateWallForce(vector2, elevation, vector5);
			Vector2 targetVelocity;
			if (this.approachingPartEndpoint)
			{
				targetVelocity = ((this.slowdownTime > 0f) ? Vector2.zero : (vector5 * this.maxSpeed));
				a *= Math.Min(this.distanceToSteeringTarget / 0.5f, 1f);
				if (this.distanceToSteeringTarget <= this.endReachedDistance)
				{
					this.NextPart();
				}
			}
			else
			{
				targetVelocity = (((this.nextCorners.Count > 1) ? this.movementPlane.ToPlane(this.nextCorners[1]) : (vector2 + 2f * vector4)) - vector3).normalized * this.maxSpeed;
			}
			Vector2 forwardsVector = this.movementPlane.ToPlane(this.simulatedRotation * ((this.orientation == OrientationMode.YAxisForward) ? Vector3.up : Vector3.forward));
			Vector2 a2 = MovementUtilities.CalculateAccelerationToReachPoint(vector3 - vector2, targetVelocity, this.velocity2D, this.acceleration, this.rotationSpeed, this.maxSpeed, forwardsVector);
			this.velocity2D += (a2 + a * this.wallForce) * deltaTime;
			float num = this.distanceToSteeringTarget + Vector3.Distance(this.steeringTarget, fn.exactEnd);
			float slowdownFactor = (num < this.maxSpeed * this.slowdownTime) ? Mathf.Sqrt(num / (this.maxSpeed * this.slowdownTime)) : 1f;
			this.FinalMovement(vector, deltaTime, num, slowdownFactor, out nextPosition, out nextRotation);
		}

		private void FinalMovement(Vector3 position3D, float deltaTime, float distanceToEndOfPath, float slowdownFactor, out Vector3 nextPosition, out Quaternion nextRotation)
		{
			Vector2 forward = this.movementPlane.ToPlane(this.simulatedRotation * ((this.orientation == OrientationMode.YAxisForward) ? Vector3.up : Vector3.forward));
			this.velocity2D = MovementUtilities.ClampVelocity(this.velocity2D, this.maxSpeed, slowdownFactor, this.slowWhenNotFacingTarget && this.enableRotation, forward);
			base.ApplyGravity(deltaTime);
			if (this.rvoController != null && this.rvoController.enabled)
			{
				Vector3 pos = position3D + this.movementPlane.ToWorld(Vector2.ClampMagnitude(this.velocity2D, distanceToEndOfPath), 0f);
				this.rvoController.SetTarget(pos, this.velocity2D.magnitude, this.maxSpeed);
			}
			Vector2 vector = this.lastDeltaPosition = base.CalculateDeltaToMoveThisFrame(this.movementPlane.ToPlane(position3D), distanceToEndOfPath, deltaTime);
			float num = this.approachingPartEndpoint ? Mathf.Clamp01(1.1f * slowdownFactor - 0.1f) : 1f;
			nextRotation = (this.enableRotation ? base.SimulateRotationTowards(vector, this.rotationSpeed * num * deltaTime) : this.simulatedRotation);
			nextPosition = position3D + this.movementPlane.ToWorld(vector, this.verticalVelocity * deltaTime);
		}

		protected override Vector3 ClampToNavmesh(Vector3 position, out bool positionChanged)
		{
			if (this.richPath != null)
			{
				RichFunnel richFunnel = this.richPath.GetCurrentPart() as RichFunnel;
				if (richFunnel != null)
				{
					Vector3 a = richFunnel.ClampToNavmesh(position);
					Vector2 vector = this.movementPlane.ToPlane(a - position);
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
			}
			positionChanged = false;
			return position;
		}

		private Vector2 CalculateWallForce(Vector2 position, float elevation, Vector2 directionToTarget)
		{
			if (this.wallForce <= 0f || this.wallDist <= 0f)
			{
				return Vector2.zero;
			}
			float num = 0f;
			float num2 = 0f;
			Vector3 vector = this.movementPlane.ToWorld(position, elevation);
			for (int i = 0; i < this.wallBuffer.Count; i += 2)
			{
				float sqrMagnitude = (VectorMath.ClosestPointOnSegment(this.wallBuffer[i], this.wallBuffer[i + 1], vector) - vector).sqrMagnitude;
				if (sqrMagnitude <= this.wallDist * this.wallDist)
				{
					Vector2 normalized = this.movementPlane.ToPlane(this.wallBuffer[i + 1] - this.wallBuffer[i]).normalized;
					float num3 = Vector2.Dot(directionToTarget, normalized);
					float num4 = 1f - Math.Max(0f, 2f * (sqrMagnitude / (this.wallDist * this.wallDist)) - 1f);
					if (num3 > 0f)
					{
						num2 = Math.Max(num2, num3 * num4);
					}
					else
					{
						num = Math.Max(num, -num3 * num4);
					}
				}
			}
			return new Vector2(directionToTarget.y, -directionToTarget.x) * (num2 - num);
		}

		protected virtual IEnumerator TraverseSpecial(RichSpecial link)
		{
			this.traversingOffMeshLink = true;
			this.velocity2D = Vector3.zero;
			IEnumerator routine = (this.onTraverseOffMeshLink != null) ? this.onTraverseOffMeshLink(link) : this.TraverseOffMeshLinkFallback(link);
			yield return base.StartCoroutine(routine);
			this.traversingOffMeshLink = false;
			this.NextPart();
			if (this.delayUpdatePath)
			{
				this.delayUpdatePath = false;
				if (base.canSearch)
				{
					this.SearchPath();
				}
			}
			yield break;
		}

		protected IEnumerator TraverseOffMeshLinkFallback(RichSpecial link)
		{
			float duration = (this.maxSpeed > 0f) ? (Vector3.Distance(link.second.position, link.first.position) / this.maxSpeed) : 1f;
			float startTime = Time.time;
			for (;;)
			{
				Vector3 vector = Vector3.Lerp(link.first.position, link.second.position, Mathf.InverseLerp(startTime, startTime + duration, Time.time));
				if (this.updatePosition)
				{
					this.tr.position = vector;
				}
				else
				{
					this.simulatedPosition = vector;
				}
				if (Time.time >= startTime + duration)
				{
					break;
				}
				yield return null;
			}
			yield break;
		}

		protected override void OnDrawGizmos()
		{
			base.OnDrawGizmos();
			if (this.tr != null)
			{
				Gizmos.color = RichAI.GizmoColorPath;
				Vector3 from = base.position;
				for (int i = 0; i < this.nextCorners.Count; i++)
				{
					Gizmos.DrawLine(from, this.nextCorners[i]);
					from = this.nextCorners[i];
				}
			}
		}

		protected override int OnUpgradeSerializedData(int version, bool unityThread)
		{
			if (unityThread && this.animCompatibility != null)
			{
				this.anim = this.animCompatibility;
			}
			return base.OnUpgradeSerializedData(version, unityThread);
		}

		[Obsolete("Use SearchPath instead. [AstarUpgradable: 'UpdatePath' -> 'SearchPath']")]
		public void UpdatePath()
		{
			this.SearchPath();
		}

		[Obsolete("Use velocity instead (lowercase 'v'). [AstarUpgradable: 'Velocity' -> 'velocity']")]
		public Vector3 Velocity
		{
			get
			{
				return base.velocity;
			}
		}

		[Obsolete("Use steeringTarget instead. [AstarUpgradable: 'NextWaypoint' -> 'steeringTarget']")]
		public Vector3 NextWaypoint
		{
			get
			{
				return this.steeringTarget;
			}
		}

		[Obsolete("Use Vector3.Distance(transform.position, ai.steeringTarget) instead.")]
		public float DistanceToNextWaypoint
		{
			get
			{
				return this.distanceToSteeringTarget;
			}
		}

		[Obsolete("Use canSearch instead. [AstarUpgradable: 'repeatedlySearchPaths' -> 'canSearch']")]
		public bool repeatedlySearchPaths
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

		[Obsolete("When unifying the interfaces for different movement scripts, this property has been renamed to reachedEndOfPath (lowercase t).  [AstarUpgradable: 'TargetReached' -> 'reachedEndOfPath']")]
		public bool TargetReached
		{
			get
			{
				return this.reachedEndOfPath;
			}
		}

		[Obsolete("Use pathPending instead (lowercase 'p'). [AstarUpgradable: 'PathPending' -> 'pathPending']")]
		public bool PathPending
		{
			get
			{
				return this.pathPending;
			}
		}

		[Obsolete("Use approachingPartEndpoint (lowercase 'a') instead")]
		public bool ApproachingPartEndpoint
		{
			get
			{
				return this.approachingPartEndpoint;
			}
		}

		[Obsolete("Use approachingPathEndpoint (lowercase 'a') instead")]
		public bool ApproachingPathEndpoint
		{
			get
			{
				return this.approachingPathEndpoint;
			}
		}

		[Obsolete("This property has been renamed to 'traversingOffMeshLink'. [AstarUpgradable: 'TraversingSpecial' -> 'traversingOffMeshLink']")]
		public bool TraversingSpecial
		{
			get
			{
				return this.traversingOffMeshLink;
			}
		}

		[Obsolete("This property has been renamed to steeringTarget")]
		public Vector3 TargetPoint
		{
			get
			{
				return this.steeringTarget;
			}
		}

		[Obsolete("Use the onTraverseOffMeshLink event or the ... component instead. Setting this value will add a ... component")]
		public Animation anim
		{
			get
			{
				AnimationLinkTraverser component = base.GetComponent<AnimationLinkTraverser>();
				if (!(component != null))
				{
					return null;
				}
				return component.anim;
			}
			set
			{
				this.animCompatibility = null;
				AnimationLinkTraverser animationLinkTraverser = base.GetComponent<AnimationLinkTraverser>();
				if (animationLinkTraverser == null)
				{
					animationLinkTraverser = base.gameObject.AddComponent<AnimationLinkTraverser>();
				}
				animationLinkTraverser.anim = value;
			}
		}

		public float acceleration = 5f;

		public float rotationSpeed = 360f;

		public float slowdownTime = 0.5f;

		public float endReachedDistance = 0.01f;

		public float wallForce = 3f;

		public float wallDist = 1f;

		public bool funnelSimplification;

		public bool slowWhenNotFacingTarget = true;

		public Func<RichSpecial, IEnumerator> onTraverseOffMeshLink;

		protected readonly RichPath richPath = new RichPath();

		protected bool delayUpdatePath;

		protected bool lastCorner;

		protected float distanceToSteeringTarget = float.PositiveInfinity;

		protected readonly List<Vector3> nextCorners = new List<Vector3>();

		protected readonly List<Vector3> wallBuffer = new List<Vector3>();

		protected static readonly Color GizmoColorPath = new Color(0.03137255f, 0.30588236f, 0.7607843f);

		[FormerlySerializedAs("anim")]
		[SerializeField]
		[HideInInspector]
		private Animation animCompatibility;
	}
}
