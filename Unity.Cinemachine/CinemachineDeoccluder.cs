using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Cinemachine
{
	[AddComponentMenu("Cinemachine/Procedural/Extensions/Cinemachine Deoccluder")]
	[SaveDuringPlay]
	[ExecuteAlways]
	[DisallowMultipleComponent]
	[RequiredTarget(RequiredTargetAttribute.RequiredTargets.Tracking)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineDeoccluder.html")]
	public class CinemachineDeoccluder : CinemachineExtension, IShotQualityEvaluator
	{
		public bool IsTargetObscured(CinemachineVirtualCameraBase vcam)
		{
			return base.GetExtraState<CinemachineDeoccluder.VcamExtraState>(vcam).TargetObscured;
		}

		public bool CameraWasDisplaced(CinemachineVirtualCameraBase vcam)
		{
			return this.GetCameraDisplacementDistance(vcam) > 0f;
		}

		public float GetCameraDisplacementDistance(CinemachineVirtualCameraBase vcam)
		{
			return base.GetExtraState<CinemachineDeoccluder.VcamExtraState>(vcam).PreviousDisplacement.magnitude;
		}

		private void OnValidate()
		{
			this.AvoidObstacles.DistanceLimit = Mathf.Max(0f, this.AvoidObstacles.DistanceLimit);
			this.AvoidObstacles.MinimumOcclusionTime = Mathf.Max(0f, this.AvoidObstacles.MinimumOcclusionTime);
			this.AvoidObstacles.CameraRadius = Mathf.Max(0f, this.AvoidObstacles.CameraRadius);
			this.MinimumDistanceFromTarget = Mathf.Max(0.01f, this.MinimumDistanceFromTarget);
			this.ShotQualityEvaluation.NearLimit = Mathf.Max(0.1f, this.ShotQualityEvaluation.NearLimit);
			this.ShotQualityEvaluation.FarLimit = Mathf.Max(this.ShotQualityEvaluation.NearLimit, this.ShotQualityEvaluation.FarLimit);
			this.ShotQualityEvaluation.OptimalDistance = Mathf.Clamp(this.ShotQualityEvaluation.OptimalDistance, this.ShotQualityEvaluation.NearLimit, this.ShotQualityEvaluation.FarLimit);
		}

		private void Reset()
		{
			this.CollideAgainst = 1;
			this.IgnoreTag = string.Empty;
			this.TransparentLayers = 0;
			this.MinimumDistanceFromTarget = 0.3f;
			this.AvoidObstacles = CinemachineDeoccluder.ObstacleAvoidance.Default;
			this.ShotQualityEvaluation = CinemachineDeoccluder.QualityEvaluation.Default;
		}

		protected override void OnDestroy()
		{
			RuntimeUtility.DestroyScratchCollider();
			base.OnDestroy();
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			List<CinemachineDeoccluder.VcamExtraState> list = new List<CinemachineDeoccluder.VcamExtraState>();
			base.GetAllExtraStates<CinemachineDeoccluder.VcamExtraState>(list);
			for (int i = 0; i < list.Count; i++)
			{
				list[i].StateIsValid = false;
			}
		}

		public void DebugCollisionPaths(List<List<Vector3>> paths, List<List<Collider>> obstacles)
		{
			if (paths != null)
			{
				paths.Clear();
			}
			if (obstacles != null)
			{
				obstacles.Clear();
			}
			if (this.m_extraStateCache == null)
			{
				this.m_extraStateCache = new List<CinemachineDeoccluder.VcamExtraState>();
			}
			base.GetAllExtraStates<CinemachineDeoccluder.VcamExtraState>(this.m_extraStateCache);
			for (int i = 0; i < this.m_extraStateCache.Count; i++)
			{
				CinemachineDeoccluder.VcamExtraState vcamExtraState = this.m_extraStateCache[i];
				if (vcamExtraState.DebugResolutionPath != null && vcamExtraState.DebugResolutionPath.Count > 0)
				{
					if (paths != null)
					{
						paths.Add(vcamExtraState.DebugResolutionPath);
					}
					if (obstacles != null)
					{
						obstacles.Add(vcamExtraState.OccludingObjects);
					}
				}
			}
		}

		public override float GetMaxDampTime()
		{
			if (!this.AvoidObstacles.Enabled)
			{
				return 0f;
			}
			return Mathf.Max(this.AvoidObstacles.Damping, Mathf.Max(this.AvoidObstacles.DampingWhenOccluded, this.AvoidObstacles.SmoothingTime));
		}

		public override void OnTargetObjectWarped(CinemachineVirtualCameraBase vcam, Transform target, Vector3 positionDelta)
		{
			base.GetExtraState<CinemachineDeoccluder.VcamExtraState>(vcam).PreviousCameraPosition += positionDelta;
		}

		public override void ForceCameraPosition(CinemachineVirtualCameraBase vcam, Vector3 pos, Quaternion rot)
		{
			base.GetExtraState<CinemachineDeoccluder.VcamExtraState>(vcam).PreviousCameraPosition = pos;
		}

		protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
		{
			if (stage == CinemachineCore.Stage.Body)
			{
				CinemachineDeoccluder.VcamExtraState extraState = base.GetExtraState<CinemachineDeoccluder.VcamExtraState>(vcam);
				extraState.TargetObscured = false;
				List<Vector3> debugResolutionPath = extraState.DebugResolutionPath;
				if (debugResolutionPath != null)
				{
					debugResolutionPath.Clear();
				}
				List<Collider> occludingObjects = extraState.OccludingObjects;
				if (occludingObjects != null)
				{
					occludingObjects.Clear();
				}
				if (!vcam.PreviousStateIsValid || deltaTime < 0f)
				{
					extraState.StateIsValid = false;
				}
				if (!this.AvoidObstacles.Enabled)
				{
					extraState.StateIsValid = false;
				}
				else
				{
					Vector3 correctedPosition = state.GetCorrectedPosition();
					Vector3 referenceUp = state.ReferenceUp;
					bool flag = state.HasLookAt();
					Vector3 vector = flag ? state.ReferenceLookAt : state.GetCorrectedPosition();
					Vector3 vector2;
					bool avoidanceResolutionTargetPoint = this.GetAvoidanceResolutionTargetPoint(vcam, ref state, out vector2);
					Vector2 a = flag ? state.RawOrientation.GetCameraRotationToTarget(vector - correctedPosition, referenceUp) : Vector2.zero;
					Quaternion rotationDampingBypass = state.RotationDampingBypass;
					if (extraState.StateIsValid)
					{
						extraState.PreviousDisplacement = rotationDampingBypass * extraState.PreviousDisplacement;
					}
					Vector3 vector3 = avoidanceResolutionTargetPoint ? this.PreserveLineOfSight(ref state, ref extraState, vector2) : Vector3.zero;
					if (this.AvoidObstacles.MinimumOcclusionTime > 0.0001f)
					{
						float currentTime = CinemachineCore.CurrentTime;
						if (vector3.AlmostZero())
						{
							extraState.OcclusionStartTime = 0f;
						}
						else
						{
							if (extraState.OcclusionStartTime <= 0f)
							{
								extraState.OcclusionStartTime = currentTime;
							}
							if (extraState.StateIsValid && currentTime - extraState.OcclusionStartTime < this.AvoidObstacles.MinimumOcclusionTime)
							{
								vector3 = extraState.PreviousDisplacement;
							}
						}
					}
					if (avoidanceResolutionTargetPoint && this.AvoidObstacles.SmoothingTime > 0.0001f)
					{
						if (!extraState.StateIsValid)
						{
							extraState.ResetDistanceSmoothing(0f);
						}
						Vector3 vector4 = correctedPosition + vector3;
						Vector3 a2 = vector4 - vector2;
						float num = a2.magnitude;
						if (num > 0.0001f)
						{
							a2 /= num;
							if (!vector3.AlmostZero())
							{
								extraState.UpdateDistanceSmoothing(num);
							}
							num = extraState.ApplyDistanceSmoothing(num, this.AvoidObstacles.SmoothingTime);
							vector3 += vector2 + a2 * num - vector4;
						}
					}
					if (vector3.AlmostZero())
					{
						extraState.ResetDistanceSmoothing(this.AvoidObstacles.SmoothingTime);
					}
					Vector3 vector5 = correctedPosition + vector3;
					if (this.AvoidObstacles.Strategy != CinemachineDeoccluder.ObstacleAvoidance.ResolutionStrategy.PullCameraForward)
					{
						vector3 += this.RespectCameraRadius(vector5, vector2);
					}
					float num2 = this.AvoidObstacles.DampingWhenOccluded;
					if (avoidanceResolutionTargetPoint && extraState.StateIsValid && this.AvoidObstacles.DampingWhenOccluded + this.AvoidObstacles.Damping > 0.0001f)
					{
						float sqrMagnitude = vector3.sqrMagnitude;
						float sqrMagnitude2 = extraState.PreviousDisplacement.sqrMagnitude;
						if (Mathf.Abs(sqrMagnitude - sqrMagnitude2) > 9.999999E-09f)
						{
							num2 = ((sqrMagnitude > sqrMagnitude2) ? this.AvoidObstacles.DampingWhenOccluded : this.AvoidObstacles.Damping);
							if (sqrMagnitude < 0.0001f && num2 < extraState.PreviousDampTime)
							{
								num2 = extraState.PreviousDampTime + Damper.Damp(num2 - extraState.PreviousDampTime, num2, deltaTime);
							}
							if (this.AvoidObstacles.Strategy == CinemachineDeoccluder.ObstacleAvoidance.ResolutionStrategy.PullCameraForward)
							{
								Vector3 a3 = correctedPosition + vector3 - vector2;
								float num3 = a3.magnitude;
								Vector3 a4 = a3 / num3;
								float num4 = extraState.PreviousCameraOffset.magnitude;
								float num5 = (correctedPosition - vector2).magnitude - Mathf.Sqrt(sqrMagnitude2);
								if (Mathf.Abs(num3 - num5) < Mathf.Abs(num3 - num4))
								{
									num4 = num5;
								}
								num3 = num4 + Damper.Damp(num3 - num4, num2, deltaTime);
								vector5 = vector2 + a4 * num3;
								vector3 = vector5 - correctedPosition;
							}
							else
							{
								Vector3 vector6 = vector2 + rotationDampingBypass * extraState.PreviousCameraOffset - correctedPosition;
								if (vector6.sqrMagnitude > sqrMagnitude2)
								{
									vector6 = extraState.PreviousDisplacement;
								}
								vector3 = vector6 + Damper.Damp(vector3 - vector6, num2, deltaTime);
							}
						}
					}
					state.PositionCorrection += vector3;
					vector5 = state.GetCorrectedPosition();
					if (flag && vector3.sqrMagnitude > 0.0001f)
					{
						Quaternion orient = Quaternion.LookRotation(vector - vector5, referenceUp);
						state.RawOrientation = orient.ApplyCameraRotation(-a, referenceUp);
						if (extraState.StateIsValid)
						{
							Vector3 v = extraState.PreviousCameraPosition - vector;
							Vector3 v2 = vector5 - vector;
							if (v.sqrMagnitude > 0.0001f && v2.sqrMagnitude > 0.0001f)
							{
								state.RotationDampingBypass = UnityVectorExtensions.SafeFromToRotation(v, v2, referenceUp);
							}
						}
					}
					extraState.PreviousDisplacement = vector3;
					extraState.PreviousCameraOffset = vector5 - vector2;
					extraState.PreviousCameraPosition = vector5;
					extraState.PreviousDampTime = num2;
					extraState.StateIsValid = true;
				}
			}
			if (stage == CinemachineCore.Stage.Finalize && this.ShotQualityEvaluation.Enabled && state.HasLookAt())
			{
				CinemachineDeoccluder.VcamExtraState extraState2 = base.GetExtraState<CinemachineDeoccluder.VcamExtraState>(vcam);
				extraState2.TargetObscured = (state.IsTargetOffscreen() || this.IsTargetObscured(state));
				if (extraState2.TargetObscured)
				{
					state.ShotQuality *= 0.2f;
				}
				if (extraState2.StateIsValid && !extraState2.PreviousDisplacement.AlmostZero())
				{
					state.ShotQuality *= 0.8f;
				}
				float num6 = 0f;
				if (this.ShotQualityEvaluation.OptimalDistance > 0f)
				{
					float num7 = Vector3.Magnitude(state.ReferenceLookAt - state.GetFinalPosition());
					if (num7 <= this.ShotQualityEvaluation.OptimalDistance)
					{
						if (num7 >= this.ShotQualityEvaluation.NearLimit)
						{
							num6 = this.ShotQualityEvaluation.MaxQualityBoost * (num7 - this.ShotQualityEvaluation.NearLimit) / (this.ShotQualityEvaluation.OptimalDistance - this.ShotQualityEvaluation.NearLimit);
						}
					}
					else
					{
						num7 -= this.ShotQualityEvaluation.OptimalDistance;
						if (num7 < this.ShotQualityEvaluation.FarLimit)
						{
							num6 = this.ShotQualityEvaluation.MaxQualityBoost * (1f - num7 / this.ShotQualityEvaluation.FarLimit);
						}
					}
					state.ShotQuality *= 1f + num6;
				}
			}
		}

		private bool GetAvoidanceResolutionTargetPoint(CinemachineVirtualCameraBase vcam, ref CameraState state, out Vector3 resolutuionTargetPoint)
		{
			bool flag = state.HasLookAt();
			resolutuionTargetPoint = (flag ? state.ReferenceLookAt : state.GetCorrectedPosition());
			if (this.AvoidObstacles.UseFollowTarget.Enabled)
			{
				Transform follow = vcam.Follow;
				if (follow != null)
				{
					flag = true;
					resolutuionTargetPoint = TargetPositionCache.GetTargetPosition(follow) + TargetPositionCache.GetTargetRotation(follow) * Vector3.up * this.AvoidObstacles.UseFollowTarget.YOffset;
				}
			}
			return flag;
		}

		private Vector3 PreserveLineOfSight(ref CameraState state, ref CinemachineDeoccluder.VcamExtraState extra, Vector3 lookAtPoint)
		{
			if (this.CollideAgainst != 0 && this.CollideAgainst != this.TransparentLayers)
			{
				Vector3 correctedPosition = state.GetCorrectedPosition();
				RaycastHit obstacle = default(RaycastHit);
				Vector3 vector = this.PullCameraInFrontOfNearestObstacle(correctedPosition, lookAtPoint, this.CollideAgainst & ~this.TransparentLayers, ref obstacle);
				if (obstacle.collider != null)
				{
					extra.AddPointToDebugPath(vector, obstacle.collider);
					if (this.AvoidObstacles.Strategy != CinemachineDeoccluder.ObstacleAvoidance.ResolutionStrategy.PullCameraForward)
					{
						Vector3 pushDir = correctedPosition - lookAtPoint;
						vector = this.PushCameraBack(vector, pushDir, obstacle, lookAtPoint, new Plane(state.ReferenceUp, correctedPosition), pushDir.magnitude, this.AvoidObstacles.MaximumEffort, ref extra);
					}
				}
				return vector - correctedPosition;
			}
			return Vector3.zero;
		}

		private Vector3 PullCameraInFrontOfNearestObstacle(Vector3 cameraPos, Vector3 lookAtPos, int layerMask, ref RaycastHit hitInfo)
		{
			Vector3 vector = cameraPos;
			Vector3 vector2 = cameraPos - lookAtPos;
			float magnitude = vector2.magnitude;
			if (magnitude > 0.0001f)
			{
				vector2 /= magnitude;
				float num = this.MinimumDistanceFromTarget + this.AvoidObstacles.CameraRadius + 0.001f;
				if (magnitude > num)
				{
					float num2 = Mathf.Max(magnitude - num - this.AvoidObstacles.CameraRadius, 0.001f);
					if (this.AvoidObstacles.DistanceLimit > 0.0001f)
					{
						num2 = Mathf.Min(this.AvoidObstacles.DistanceLimit, num2);
					}
					if (RuntimeUtility.SphereCastIgnoreTag(new Ray(lookAtPos + vector2 * num, vector2), this.AvoidObstacles.CameraRadius, out hitInfo, num2, layerMask, this.IgnoreTag))
					{
						vector = hitInfo.point + hitInfo.normal * (this.AvoidObstacles.CameraRadius + 0.001f);
					}
					if ((lookAtPos - vector).sqrMagnitude < num * num)
					{
						vector = lookAtPos + vector2 * num;
					}
				}
			}
			return vector;
		}

		private Vector3 PushCameraBack(Vector3 currentPos, Vector3 pushDir, RaycastHit obstacle, Vector3 lookAtPos, Plane startPlane, float targetDistance, int iterations, ref CinemachineDeoccluder.VcamExtraState extra)
		{
			Vector3 vector = Vector3.zero;
			if (obstacle.collider == null || !this.GetWalkingDirection(currentPos, pushDir, obstacle, ref vector))
			{
				return currentPos;
			}
			Ray ray = new Ray(currentPos, vector);
			float num = this.GetPushBackDistance(ray, startPlane, targetDistance, lookAtPos);
			if (num <= 0.0001f)
			{
				return currentPos;
			}
			float num2 = CinemachineDeoccluder.ClampRayToBounds(ray, num, obstacle.collider.bounds);
			num = Mathf.Min(num, num2 + 0.001f);
			RaycastHit obstacle2;
			Vector3 vector2;
			if (RuntimeUtility.SphereCastIgnoreTag(ray, this.AvoidObstacles.CameraRadius, out obstacle2, num, this.CollideAgainst & ~this.TransparentLayers, this.IgnoreTag))
			{
				float distance = obstacle2.distance - 0.001f;
				vector2 = ray.GetPoint(distance);
				extra.AddPointToDebugPath(vector2, obstacle2.collider);
				if (iterations > 1)
				{
					vector2 = this.PushCameraBack(vector2, vector, obstacle2, lookAtPos, startPlane, targetDistance, iterations - 1, ref extra);
				}
				return vector2;
			}
			vector2 = ray.GetPoint(num);
			vector = vector2 - lookAtPos;
			float magnitude = vector.magnitude;
			RaycastHit raycastHit;
			if (magnitude < 0.0001f || RuntimeUtility.SphereCastIgnoreTag(new Ray(lookAtPos, vector), this.AvoidObstacles.CameraRadius, out raycastHit, magnitude - 0.001f, this.CollideAgainst & ~this.TransparentLayers, this.IgnoreTag))
			{
				return currentPos;
			}
			ray = new Ray(vector2, vector);
			extra.AddPointToDebugPath(vector2, null);
			num = this.GetPushBackDistance(ray, startPlane, targetDistance, lookAtPos);
			if (num > 0.0001f)
			{
				if (!RuntimeUtility.SphereCastIgnoreTag(ray, this.AvoidObstacles.CameraRadius, out obstacle2, num, this.CollideAgainst & ~this.TransparentLayers, this.IgnoreTag))
				{
					vector2 = ray.GetPoint(num);
					extra.AddPointToDebugPath(vector2, null);
				}
				else
				{
					float distance2 = obstacle2.distance - 0.001f;
					vector2 = ray.GetPoint(distance2);
					extra.AddPointToDebugPath(vector2, obstacle2.collider);
					if (iterations > 1)
					{
						vector2 = this.PushCameraBack(vector2, vector, obstacle2, lookAtPos, startPlane, targetDistance, iterations - 1, ref extra);
					}
				}
			}
			return vector2;
		}

		private bool GetWalkingDirection(Vector3 pos, Vector3 pushDir, RaycastHit obstacle, ref Vector3 outDir)
		{
			Vector3 normal = obstacle.normal;
			float num = 0.0050000004f;
			int num2 = Physics.SphereCastNonAlloc(pos, num, pushDir.normalized, this.m_CornerBuffer, 0f, this.CollideAgainst & ~this.TransparentLayers, QueryTriggerInteraction.Ignore);
			if (num2 > 1)
			{
				for (int i = 0; i < num2; i++)
				{
					if (!(this.m_CornerBuffer[i].collider == null) && (this.IgnoreTag.Length <= 0 || !this.m_CornerBuffer[i].collider.CompareTag(this.IgnoreTag)))
					{
						Type type = this.m_CornerBuffer[i].collider.GetType();
						if (type == typeof(BoxCollider) || type == typeof(SphereCollider) || type == typeof(CapsuleCollider))
						{
							Vector3 direction = this.m_CornerBuffer[i].collider.ClosestPoint(pos) - pos;
							if (direction.magnitude > 1E-05f && this.m_CornerBuffer[i].collider.Raycast(new Ray(pos, direction), out this.m_CornerBuffer[i], num))
							{
								if (!(this.m_CornerBuffer[i].normal - obstacle.normal).AlmostZero())
								{
									normal = this.m_CornerBuffer[i].normal;
									break;
								}
								break;
							}
						}
					}
				}
			}
			Vector3 vector = Vector3.Cross(obstacle.normal, normal);
			if (vector.AlmostZero())
			{
				vector = Vector3.ProjectOnPlane(pushDir, obstacle.normal);
			}
			else
			{
				float num3 = Vector3.Dot(vector, pushDir);
				if (Mathf.Abs(num3) < 0.0001f)
				{
					return false;
				}
				if (num3 < 0f)
				{
					vector = -vector;
				}
			}
			if (vector.AlmostZero())
			{
				return false;
			}
			outDir = vector.normalized;
			return true;
		}

		private float GetPushBackDistance(Ray ray, Plane startPlane, float targetDistance, Vector3 lookAtPos)
		{
			float num = targetDistance - (ray.origin - lookAtPos).magnitude;
			if (num < 0.0001f)
			{
				return 0f;
			}
			if (this.AvoidObstacles.Strategy == CinemachineDeoccluder.ObstacleAvoidance.ResolutionStrategy.PreserveCameraDistance)
			{
				return num;
			}
			float num2;
			if (!startPlane.Raycast(ray, out num2))
			{
				num2 = 0f;
			}
			num2 = Mathf.Min(num, num2);
			if (num2 < 0.0001f)
			{
				return 0f;
			}
			float num3 = Mathf.Abs(UnityVectorExtensions.Angle(startPlane.normal, ray.direction) - 90f);
			num2 = Mathf.Lerp(0f, num2, num3 / 0.1f);
			return num2;
		}

		private static float ClampRayToBounds(Ray ray, float distance, Bounds bounds)
		{
			float num;
			if (Vector3.Dot(ray.direction, Vector3.up) > 0f)
			{
				if (new Plane(Vector3.down, bounds.max).Raycast(ray, out num) && num > 0.0001f)
				{
					distance = Mathf.Min(distance, num);
				}
			}
			else if (Vector3.Dot(ray.direction, Vector3.down) > 0f && new Plane(Vector3.up, bounds.min).Raycast(ray, out num) && num > 0.0001f)
			{
				distance = Mathf.Min(distance, num);
			}
			if (Vector3.Dot(ray.direction, Vector3.right) > 0f)
			{
				if (new Plane(Vector3.left, bounds.max).Raycast(ray, out num) && num > 0.0001f)
				{
					distance = Mathf.Min(distance, num);
				}
			}
			else if (Vector3.Dot(ray.direction, Vector3.left) > 0f && new Plane(Vector3.right, bounds.min).Raycast(ray, out num) && num > 0.0001f)
			{
				distance = Mathf.Min(distance, num);
			}
			if (Vector3.Dot(ray.direction, Vector3.forward) > 0f)
			{
				if (new Plane(Vector3.back, bounds.max).Raycast(ray, out num) && num > 0.0001f)
				{
					distance = Mathf.Min(distance, num);
				}
			}
			else if (Vector3.Dot(ray.direction, Vector3.back) > 0f && new Plane(Vector3.forward, bounds.min).Raycast(ray, out num) && num > 0.0001f)
			{
				distance = Mathf.Min(distance, num);
			}
			return distance;
		}

		private Vector3 RespectCameraRadius(Vector3 cameraPos, Vector3 lookAtPos)
		{
			Vector3 vector = Vector3.zero;
			if (this.AvoidObstacles.CameraRadius < 0.0001f || this.CollideAgainst == 0)
			{
				return vector;
			}
			Vector3 vector2 = cameraPos - lookAtPos;
			float magnitude = vector2.magnitude;
			if (magnitude > 0.0001f)
			{
				vector2 /= magnitude;
			}
			int num = Physics.OverlapSphereNonAlloc(cameraPos, this.AvoidObstacles.CameraRadius, CinemachineDeoccluder.s_ColliderBuffer, this.CollideAgainst, QueryTriggerInteraction.Ignore);
			if (num == 0 && this.TransparentLayers != 0 && magnitude > this.MinimumDistanceFromTarget + 0.0001f)
			{
				float num2 = magnitude - this.MinimumDistanceFromTarget;
				RaycastHit raycastHit;
				if (RuntimeUtility.SphereCastIgnoreTag(new Ray(lookAtPos + vector2 * this.MinimumDistanceFromTarget, vector2), this.AvoidObstacles.CameraRadius, out raycastHit, num2, this.CollideAgainst, this.IgnoreTag))
				{
					Collider collider = raycastHit.collider;
					if (!collider.Raycast(new Ray(cameraPos, -vector2), out raycastHit, num2))
					{
						CinemachineDeoccluder.s_ColliderBuffer[num++] = collider;
					}
				}
			}
			if ((num > 0 && magnitude == 0f) || magnitude > this.MinimumDistanceFromTarget)
			{
				SphereCollider scratchCollider = RuntimeUtility.GetScratchCollider();
				scratchCollider.radius = this.AvoidObstacles.CameraRadius;
				Vector3 vector3 = cameraPos;
				for (int i = 0; i < num; i++)
				{
					Collider collider2 = CinemachineDeoccluder.s_ColliderBuffer[i];
					if (this.IgnoreTag.Length <= 0 || !collider2.CompareTag(this.IgnoreTag))
					{
						if (magnitude > this.MinimumDistanceFromTarget)
						{
							vector2 = vector3 - lookAtPos;
							float magnitude2 = vector2.magnitude;
							if (magnitude2 > 0.0001f)
							{
								vector2 /= magnitude2;
								Ray ray = new Ray(lookAtPos, vector2);
								RaycastHit raycastHit;
								if (collider2.Raycast(ray, out raycastHit, magnitude2 + this.AvoidObstacles.CameraRadius))
								{
									vector3 = ray.GetPoint(raycastHit.distance) - vector2 * 0.001f;
								}
							}
						}
						Vector3 a;
						float d;
						if (Physics.ComputePenetration(scratchCollider, vector3, Quaternion.identity, collider2, collider2.transform.position, collider2.transform.rotation, out a, out d))
						{
							vector3 += a * d;
						}
					}
				}
				vector = vector3 - cameraPos;
			}
			if (magnitude > 0.0001f && this.MinimumDistanceFromTarget > 0.0001f)
			{
				float num3 = Mathf.Max(this.MinimumDistanceFromTarget, this.AvoidObstacles.CameraRadius) + 0.001f;
				if ((cameraPos + vector - lookAtPos).magnitude < num3)
				{
					vector = lookAtPos - cameraPos + vector2 * num3;
				}
			}
			return vector;
		}

		private bool IsTargetObscured(CameraState state)
		{
			if (state.HasLookAt())
			{
				Vector3 referenceLookAt = state.ReferenceLookAt;
				Vector3 correctedPosition = state.GetCorrectedPosition();
				Vector3 vector = referenceLookAt - correctedPosition;
				float magnitude = vector.magnitude;
				if (magnitude < Mathf.Max(this.MinimumDistanceFromTarget, 0.0001f))
				{
					return true;
				}
				RaycastHit raycastHit;
				if (RuntimeUtility.SphereCastIgnoreTag(new Ray(correctedPosition, vector.normalized), this.AvoidObstacles.CameraRadius, out raycastHit, magnitude - this.MinimumDistanceFromTarget, this.CollideAgainst & ~this.TransparentLayers, this.IgnoreTag))
				{
					return true;
				}
			}
			return false;
		}

		[Tooltip("Objects on these layers will be detected")]
		public LayerMask CollideAgainst = 1;

		[TagField]
		[Tooltip("Obstacles with this tag will be ignored.  It is a good idea to set this field to the target's tag")]
		public string IgnoreTag = string.Empty;

		[Tooltip("Objects on these layers will never obstruct view of the target")]
		public LayerMask TransparentLayers = 0;

		[Tooltip("Obstacles closer to the target than this will be ignored")]
		public float MinimumDistanceFromTarget = 0.3f;

		[FoldoutWithEnabledButton("Enabled")]
		public CinemachineDeoccluder.ObstacleAvoidance AvoidObstacles;

		[FoldoutWithEnabledButton("Enabled")]
		public CinemachineDeoccluder.QualityEvaluation ShotQualityEvaluation = CinemachineDeoccluder.QualityEvaluation.Default;

		private List<CinemachineDeoccluder.VcamExtraState> m_extraStateCache;

		private const float k_PrecisionSlush = 0.001f;

		private RaycastHit[] m_CornerBuffer = new RaycastHit[4];

		private const float k_AngleThreshold = 0.1f;

		private static Collider[] s_ColliderBuffer = new Collider[5];

		[Serializable]
		public struct ObstacleAvoidance
		{
			internal static CinemachineDeoccluder.ObstacleAvoidance Default
			{
				get
				{
					return new CinemachineDeoccluder.ObstacleAvoidance
					{
						Enabled = true,
						DistanceLimit = 0f,
						MinimumOcclusionTime = 0f,
						CameraRadius = 0.4f,
						Strategy = CinemachineDeoccluder.ObstacleAvoidance.ResolutionStrategy.PullCameraForward,
						MaximumEffort = 4,
						SmoothingTime = 0f,
						Damping = 0.4f,
						DampingWhenOccluded = 0.2f
					};
				}
			}

			[Tooltip("When enabled, will attempt to resolve situations where the line of sight to the target is blocked by an obstacle")]
			public bool Enabled;

			[Tooltip("The maximum raycast distance when checking if the line of sight to this camera's target is clear.  If the setting is 0 or less, the current actual distance to target will be used.")]
			public float DistanceLimit;

			[Tooltip("Don't take action unless occlusion has lasted at least this long.")]
			public float MinimumOcclusionTime;

			[Tooltip("Camera will try to maintain this distance from any obstacle.  Try to keep this value small.  Increase it if you are seeing inside obstacles due to a large FOV on the camera.")]
			public float CameraRadius;

			[EnabledProperty("Enabled", "")]
			public CinemachineDeoccluder.ObstacleAvoidance.FollowTargetSettings UseFollowTarget;

			[Tooltip("The way in which the Deoccluder will attempt to preserve sight of the target.")]
			public CinemachineDeoccluder.ObstacleAvoidance.ResolutionStrategy Strategy;

			[Range(1f, 10f)]
			[Tooltip("Upper limit on how many obstacle hits to process.  Higher numbers may impact performance.  In most environments, 4 is enough.")]
			public int MaximumEffort;

			[Range(0f, 2f)]
			[Tooltip("Smoothing to apply to obstruction resolution.  Nearest camera point is held for at least this long")]
			public float SmoothingTime;

			[Range(0f, 10f)]
			[Tooltip("How gradually the camera returns to its normal position after having been corrected.  Higher numbers will move the camera more gradually back to normal.")]
			public float Damping;

			[Range(0f, 10f)]
			[Tooltip("How gradually the camera moves to resolve an occlusion.  Higher numbers will move the camera more gradually.")]
			public float DampingWhenOccluded;

			[Serializable]
			public struct FollowTargetSettings
			{
				[Tooltip("Use the Follow target when resolving occlusions, instead of the LookAt target.")]
				public bool Enabled;

				[Tooltip("Vertical offset from the Follow target's root, in target local space")]
				public float YOffset;
			}

			public enum ResolutionStrategy
			{
				PullCameraForward,
				PreserveCameraHeight,
				PreserveCameraDistance
			}
		}

		[Serializable]
		public struct QualityEvaluation
		{
			internal static CinemachineDeoccluder.QualityEvaluation Default
			{
				get
				{
					return new CinemachineDeoccluder.QualityEvaluation
					{
						NearLimit = 5f,
						FarLimit = 30f,
						OptimalDistance = 10f,
						MaxQualityBoost = 0.2f
					};
				}
			}

			[Tooltip("If enabled, will evaluate shot quality based on target distance and occlusion")]
			public bool Enabled;

			[Tooltip("If greater than zero, maximum quality boost will occur when target is this far from the camera")]
			public float OptimalDistance;

			[Tooltip("Shots with targets closer to the camera than this will not get a quality boost")]
			public float NearLimit;

			[Tooltip("Shots with targets farther from the camera than this will not get a quality boost")]
			public float FarLimit;

			[Tooltip("High quality shots will be boosted by this fraction of their normal quality")]
			public float MaxQualityBoost;
		}

		private class VcamExtraState : CinemachineExtension.VcamExtraStateBase
		{
			public void AddPointToDebugPath(Vector3 p, Collider c)
			{
			}

			public float ApplyDistanceSmoothing(float distance, float smoothingTime)
			{
				if (this.m_SmoothedTime != 0f && smoothingTime > 0.0001f && CinemachineCore.CurrentTime - this.m_SmoothedTime < smoothingTime)
				{
					return Mathf.Min(distance, this.m_SmoothedDistance);
				}
				return distance;
			}

			public void UpdateDistanceSmoothing(float distance)
			{
				if (!this.StateIsValid || this.m_SmoothedDistance == 0f || distance < this.m_SmoothedDistance)
				{
					this.m_SmoothedDistance = distance;
					this.m_SmoothedTime = CinemachineCore.CurrentTime;
				}
			}

			public void ResetDistanceSmoothing(float smoothingTime)
			{
				if (CinemachineCore.CurrentTime - this.m_SmoothedTime >= smoothingTime)
				{
					this.m_SmoothedDistance = (this.m_SmoothedTime = 0f);
				}
			}

			public Vector3 PreviousDisplacement;

			public bool TargetObscured;

			public float OcclusionStartTime;

			public List<Vector3> DebugResolutionPath;

			public List<Collider> OccludingObjects;

			public Vector3 PreviousCameraOffset;

			public Vector3 PreviousCameraPosition;

			public float PreviousDampTime;

			public bool StateIsValid;

			private float m_SmoothedDistance;

			private float m_SmoothedTime;
		}
	}
}
