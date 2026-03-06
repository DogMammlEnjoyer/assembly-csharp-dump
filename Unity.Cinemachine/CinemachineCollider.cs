using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.Cinemachine
{
	[Obsolete("CinemachineCollider has been deprecated. Use CinemachineDeoccluder instead")]
	[AddComponentMenu("")]
	[SaveDuringPlay]
	[ExecuteAlways]
	[DisallowMultipleComponent]
	public class CinemachineCollider : CinemachineExtension, IShotQualityEvaluator
	{
		public bool IsTargetObscured(CinemachineVirtualCameraBase vcam)
		{
			return base.GetExtraState<CinemachineCollider.VcamExtraState>(vcam).targetObscured;
		}

		public bool CameraWasDisplaced(CinemachineVirtualCameraBase vcam)
		{
			return this.GetCameraDisplacementDistance(vcam) > 0f;
		}

		public float GetCameraDisplacementDistance(CinemachineVirtualCameraBase vcam)
		{
			return base.GetExtraState<CinemachineCollider.VcamExtraState>(vcam).previousDisplacement.magnitude;
		}

		private void OnValidate()
		{
			this.m_DistanceLimit = Mathf.Max(0f, this.m_DistanceLimit);
			this.m_MinimumOcclusionTime = Mathf.Max(0f, this.m_MinimumOcclusionTime);
			this.m_CameraRadius = Mathf.Max(0f, this.m_CameraRadius);
			this.m_MinimumDistanceFromTarget = Mathf.Max(0.01f, this.m_MinimumDistanceFromTarget);
			this.m_OptimalTargetDistance = Mathf.Max(0f, this.m_OptimalTargetDistance);
		}

		protected override void OnDestroy()
		{
			RuntimeUtility.DestroyScratchCollider();
			base.OnDestroy();
		}

		internal List<List<Vector3>> DebugPaths
		{
			get
			{
				List<List<Vector3>> list = new List<List<Vector3>>();
				if (this.m_extraStateCache == null)
				{
					this.m_extraStateCache = new List<CinemachineCollider.VcamExtraState>();
				}
				base.GetAllExtraStates<CinemachineCollider.VcamExtraState>(this.m_extraStateCache);
				foreach (CinemachineCollider.VcamExtraState vcamExtraState in this.m_extraStateCache)
				{
					if (vcamExtraState.debugResolutionPath != null && vcamExtraState.debugResolutionPath.Count > 0)
					{
						list.Add(vcamExtraState.debugResolutionPath);
					}
				}
				return list;
			}
		}

		public override float GetMaxDampTime()
		{
			return Mathf.Max(this.m_Damping, Mathf.Max(this.m_DampingWhenOccluded, this.m_SmoothingTime));
		}

		protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
		{
			if (stage == CinemachineCore.Stage.Body)
			{
				CinemachineCollider.VcamExtraState extraState = base.GetExtraState<CinemachineCollider.VcamExtraState>(vcam);
				extraState.targetObscured = false;
				List<Vector3> debugResolutionPath = extraState.debugResolutionPath;
				if (debugResolutionPath != null)
				{
					debugResolutionPath.RemoveRange(0, extraState.debugResolutionPath.Count);
				}
				if (this.m_AvoidObstacles)
				{
					Vector3 correctedPosition = state.GetCorrectedPosition();
					Quaternion rotationDampingBypass = state.RotationDampingBypass;
					extraState.previousDisplacement = rotationDampingBypass * extraState.previousDisplacement;
					Vector3 vector = this.PreserveLineOfSight(ref state, ref extraState);
					if (this.m_MinimumOcclusionTime > 0.0001f)
					{
						float currentTime = CinemachineCore.CurrentTime;
						if (vector.AlmostZero())
						{
							extraState.occlusionStartTime = 0f;
						}
						else
						{
							if (extraState.occlusionStartTime <= 0f)
							{
								extraState.occlusionStartTime = currentTime;
							}
							if (currentTime - extraState.occlusionStartTime < this.m_MinimumOcclusionTime)
							{
								vector = extraState.previousDisplacement;
							}
						}
					}
					if (this.m_SmoothingTime > 0.0001f && state.HasLookAt())
					{
						Vector3 vector2 = correctedPosition + vector;
						Vector3 a = vector2 - state.ReferenceLookAt;
						float num = a.magnitude;
						if (num > 0.0001f)
						{
							a /= num;
							if (!vector.AlmostZero())
							{
								extraState.UpdateDistanceSmoothing(num);
							}
							num = extraState.ApplyDistanceSmoothing(num, this.m_SmoothingTime);
							vector += state.ReferenceLookAt + a * num - vector2;
						}
					}
					if (vector.AlmostZero())
					{
						extraState.ResetDistanceSmoothing(this.m_SmoothingTime);
					}
					Vector3 vector3 = correctedPosition + vector;
					Vector3 vector4 = state.HasLookAt() ? state.ReferenceLookAt : vector3;
					vector += this.RespectCameraRadius(vector3, vector4);
					float num2 = this.m_DampingWhenOccluded;
					if (deltaTime >= 0f && vcam.PreviousStateIsValid && this.m_DampingWhenOccluded + this.m_Damping > 0.0001f)
					{
						float sqrMagnitude = vector.sqrMagnitude;
						num2 = ((sqrMagnitude > extraState.previousDisplacement.sqrMagnitude) ? this.m_DampingWhenOccluded : this.m_Damping);
						if (sqrMagnitude < 0.0001f)
						{
							num2 = extraState.previousDampTime - Damper.Damp(extraState.previousDampTime, num2, deltaTime);
						}
						if (num2 > 0f)
						{
							bool flag = false;
							if (vcam is CinemachineVirtualCamera)
							{
								CinemachineComponentBase cinemachineComponent = (vcam as CinemachineVirtualCamera).GetCinemachineComponent(CinemachineCore.Stage.Body);
								flag = (cinemachineComponent != null && cinemachineComponent.BodyAppliesAfterAim);
							}
							Vector3 vector5 = flag ? extraState.previousDisplacement : (vector4 + rotationDampingBypass * extraState.previousCameraOffset - correctedPosition);
							vector = vector5 + Damper.Damp(vector - vector5, num2, deltaTime);
						}
					}
					state.PositionCorrection += vector;
					vector3 = state.GetCorrectedPosition();
					if (state.HasLookAt() && vcam.PreviousStateIsValid)
					{
						Vector3 v = extraState.previousCameraPosition - state.ReferenceLookAt;
						Vector3 v2 = vector3 - state.ReferenceLookAt;
						if (v.sqrMagnitude > 0.0001f && v2.sqrMagnitude > 0.0001f)
						{
							state.RotationDampingBypass *= UnityVectorExtensions.SafeFromToRotation(v, v2, state.ReferenceUp);
						}
					}
					extraState.previousDisplacement = vector;
					extraState.previousCameraOffset = vector3 - vector4;
					extraState.previousCameraPosition = vector3;
					extraState.previousDampTime = num2;
				}
			}
			if (stage == CinemachineCore.Stage.Aim)
			{
				CinemachineCollider.VcamExtraState extraState2 = base.GetExtraState<CinemachineCollider.VcamExtraState>(vcam);
				extraState2.targetObscured = (CinemachineCollider.IsTargetOffscreen(state) || this.CheckForTargetObstructions(state));
				if (extraState2.targetObscured)
				{
					state.ShotQuality *= 0.2f;
				}
				if (!extraState2.previousDisplacement.AlmostZero())
				{
					state.ShotQuality *= 0.8f;
				}
				float num3 = 0f;
				if (this.m_OptimalTargetDistance > 0f && state.HasLookAt())
				{
					float num4 = Vector3.Magnitude(state.ReferenceLookAt - state.GetFinalPosition());
					if (num4 <= this.m_OptimalTargetDistance)
					{
						float num5 = this.m_OptimalTargetDistance / 2f;
						if (num4 >= num5)
						{
							num3 = 0.2f * (num4 - num5) / (this.m_OptimalTargetDistance - num5);
						}
					}
					else
					{
						num4 -= this.m_OptimalTargetDistance;
						float num6 = this.m_OptimalTargetDistance * 3f;
						if (num4 < num6)
						{
							num3 = 0.2f * (1f - num4 / num6);
						}
					}
					state.ShotQuality *= 1f + num3;
				}
			}
		}

		private Vector3 PreserveLineOfSight(ref CameraState state, ref CinemachineCollider.VcamExtraState extra)
		{
			Vector3 vector = Vector3.zero;
			if (state.HasLookAt() && this.m_CollideAgainst != 0 && this.m_CollideAgainst != this.m_TransparentLayers)
			{
				Vector3 correctedPosition = state.GetCorrectedPosition();
				Vector3 referenceLookAt = state.ReferenceLookAt;
				RaycastHit obstacle = default(RaycastHit);
				vector = this.PullCameraInFrontOfNearestObstacle(correctedPosition, referenceLookAt, this.m_CollideAgainst & ~this.m_TransparentLayers, ref obstacle);
				Vector3 vector2 = correctedPosition + vector;
				if (obstacle.collider != null)
				{
					extra.AddPointToDebugPath(vector2);
					if (this.m_Strategy != CinemachineCollider.ResolutionStrategy.PullCameraForward)
					{
						Vector3 pushDir = correctedPosition - referenceLookAt;
						vector2 = this.PushCameraBack(vector2, pushDir, obstacle, referenceLookAt, new Plane(state.ReferenceUp, correctedPosition), pushDir.magnitude, this.m_MaximumEffort, ref extra);
					}
				}
				vector = vector2 - correctedPosition;
			}
			return vector;
		}

		private Vector3 PullCameraInFrontOfNearestObstacle(Vector3 cameraPos, Vector3 lookAtPos, int layerMask, ref RaycastHit hitInfo)
		{
			Vector3 result = Vector3.zero;
			Vector3 vector = cameraPos - lookAtPos;
			float magnitude = vector.magnitude;
			if (magnitude > 0.0001f)
			{
				vector /= magnitude;
				float num = Mathf.Max(this.m_MinimumDistanceFromTarget, 0.0001f);
				if (magnitude < num + 0.0001f)
				{
					result = vector * (num - magnitude);
				}
				else
				{
					float num2 = magnitude - num;
					if (this.m_DistanceLimit > 0.0001f)
					{
						num2 = Mathf.Min(this.m_DistanceLimit, num2);
					}
					Ray ray = new Ray(cameraPos - num2 * vector, vector);
					num2 += 0.001f;
					if (num2 > 0.0001f && RuntimeUtility.RaycastIgnoreTag(ray, out hitInfo, num2, layerMask, this.m_IgnoreTag))
					{
						float distance = Mathf.Max(0f, hitInfo.distance - 0.001f);
						result = ray.GetPoint(distance) - cameraPos;
					}
				}
			}
			return result;
		}

		private Vector3 PushCameraBack(Vector3 currentPos, Vector3 pushDir, RaycastHit obstacle, Vector3 lookAtPos, Plane startPlane, float targetDistance, int iterations, ref CinemachineCollider.VcamExtraState extra)
		{
			Vector3 vector = Vector3.zero;
			if (!this.GetWalkingDirection(currentPos, pushDir, obstacle, ref vector))
			{
				return currentPos;
			}
			Ray ray = new Ray(currentPos, vector);
			float num = this.GetPushBackDistance(ray, startPlane, targetDistance, lookAtPos);
			if (num <= 0.0001f)
			{
				return currentPos;
			}
			float num2 = CinemachineCollider.ClampRayToBounds(ray, num, obstacle.collider.bounds);
			num = Mathf.Min(num, num2 + 0.001f);
			RaycastHit obstacle2;
			Vector3 vector2;
			if (RuntimeUtility.RaycastIgnoreTag(ray, out obstacle2, num, this.m_CollideAgainst & ~this.m_TransparentLayers, this.m_IgnoreTag))
			{
				float distance = obstacle2.distance - 0.001f;
				vector2 = ray.GetPoint(distance);
				extra.AddPointToDebugPath(vector2);
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
			if (magnitude < 0.0001f || RuntimeUtility.RaycastIgnoreTag(new Ray(lookAtPos, vector), out raycastHit, magnitude - 0.001f, this.m_CollideAgainst & ~this.m_TransparentLayers, this.m_IgnoreTag))
			{
				return currentPos;
			}
			ray = new Ray(vector2, vector);
			extra.AddPointToDebugPath(vector2);
			num = this.GetPushBackDistance(ray, startPlane, targetDistance, lookAtPos);
			if (num > 0.0001f)
			{
				if (!RuntimeUtility.RaycastIgnoreTag(ray, out obstacle2, num, this.m_CollideAgainst & ~this.m_TransparentLayers, this.m_IgnoreTag))
				{
					vector2 = ray.GetPoint(num);
					extra.AddPointToDebugPath(vector2);
				}
				else
				{
					float distance2 = obstacle2.distance - 0.001f;
					vector2 = ray.GetPoint(distance2);
					extra.AddPointToDebugPath(vector2);
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
			int num2 = Physics.SphereCastNonAlloc(pos, num, pushDir.normalized, this.m_CornerBuffer, 0f, this.m_CollideAgainst & ~this.m_TransparentLayers, QueryTriggerInteraction.Ignore);
			if (num2 > 1)
			{
				for (int i = 0; i < num2; i++)
				{
					if (!(this.m_CornerBuffer[i].collider == null) && (this.m_IgnoreTag.Length <= 0 || !this.m_CornerBuffer[i].collider.CompareTag(this.m_IgnoreTag)))
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
			if (this.m_Strategy == CinemachineCollider.ResolutionStrategy.PreserveCameraDistance)
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
			float num3 = (float)Mathf.Abs((int)(UnityVectorExtensions.Angle(startPlane.normal, ray.direction) - 90f));
			if (num3 < 0.1f)
			{
				num2 = Mathf.Lerp(0f, num2, num3 / 0.1f);
			}
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
			if (this.m_CameraRadius < 0.0001f || this.m_CollideAgainst == 0)
			{
				return vector;
			}
			Vector3 vector2 = cameraPos - lookAtPos;
			float magnitude = vector2.magnitude;
			if (magnitude > 0.0001f)
			{
				vector2 /= magnitude;
			}
			int num = Physics.OverlapSphereNonAlloc(cameraPos, this.m_CameraRadius, CinemachineCollider.s_ColliderBuffer, this.m_CollideAgainst, QueryTriggerInteraction.Ignore);
			if (num == 0 && this.m_TransparentLayers != 0 && magnitude > this.m_MinimumDistanceFromTarget + 0.0001f)
			{
				float num2 = magnitude - this.m_MinimumDistanceFromTarget;
				RaycastHit raycastHit;
				if (RuntimeUtility.RaycastIgnoreTag(new Ray(lookAtPos + vector2 * this.m_MinimumDistanceFromTarget, vector2), out raycastHit, num2, this.m_CollideAgainst, this.m_IgnoreTag))
				{
					Collider collider = raycastHit.collider;
					if (!collider.Raycast(new Ray(cameraPos, -vector2), out raycastHit, num2))
					{
						CinemachineCollider.s_ColliderBuffer[num++] = collider;
					}
				}
			}
			if ((num > 0 && magnitude == 0f) || magnitude > this.m_MinimumDistanceFromTarget)
			{
				SphereCollider scratchCollider = RuntimeUtility.GetScratchCollider();
				scratchCollider.radius = this.m_CameraRadius;
				Vector3 vector3 = cameraPos;
				for (int i = 0; i < num; i++)
				{
					Collider collider2 = CinemachineCollider.s_ColliderBuffer[i];
					if (this.m_IgnoreTag.Length <= 0 || !collider2.CompareTag(this.m_IgnoreTag))
					{
						if (magnitude > this.m_MinimumDistanceFromTarget)
						{
							vector2 = vector3 - lookAtPos;
							float magnitude2 = vector2.magnitude;
							if (magnitude2 > 0.0001f)
							{
								vector2 /= magnitude2;
								Ray ray = new Ray(lookAtPos, vector2);
								RaycastHit raycastHit;
								if (collider2.Raycast(ray, out raycastHit, magnitude2 + this.m_CameraRadius))
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
			if (magnitude > 0.0001f && this.m_MinimumDistanceFromTarget > 0.0001f)
			{
				float num3 = Mathf.Max(this.m_MinimumDistanceFromTarget, this.m_CameraRadius) + 0.001f;
				if ((cameraPos + vector - lookAtPos).magnitude < num3)
				{
					vector = lookAtPos - cameraPos + vector2 * num3;
				}
			}
			return vector;
		}

		private bool CheckForTargetObstructions(CameraState state)
		{
			if (state.HasLookAt())
			{
				Vector3 referenceLookAt = state.ReferenceLookAt;
				Vector3 correctedPosition = state.GetCorrectedPosition();
				Vector3 vector = referenceLookAt - correctedPosition;
				float magnitude = vector.magnitude;
				if (magnitude < Mathf.Max(this.m_MinimumDistanceFromTarget, 0.0001f))
				{
					return true;
				}
				RaycastHit raycastHit;
				if (RuntimeUtility.RaycastIgnoreTag(new Ray(correctedPosition, vector.normalized), out raycastHit, magnitude - this.m_MinimumDistanceFromTarget, this.m_CollideAgainst & ~this.m_TransparentLayers, this.m_IgnoreTag))
				{
					return true;
				}
			}
			return false;
		}

		private static bool IsTargetOffscreen(CameraState state)
		{
			if (state.HasLookAt())
			{
				Vector3 vector = state.ReferenceLookAt - state.GetCorrectedPosition();
				vector = Quaternion.Inverse(state.GetCorrectedOrientation()) * vector;
				if (state.Lens.Orthographic)
				{
					if (Mathf.Abs(vector.y) > state.Lens.OrthographicSize)
					{
						return true;
					}
					if (Mathf.Abs(vector.x) > state.Lens.OrthographicSize * state.Lens.Aspect)
					{
						return true;
					}
				}
				else
				{
					float num = state.Lens.FieldOfView / 2f;
					if (UnityVectorExtensions.Angle(vector.ProjectOntoPlane(Vector3.right), Vector3.forward) > num)
					{
						return true;
					}
					num = 57.29578f * Mathf.Atan(Mathf.Tan(num * 0.017453292f) * state.Lens.Aspect);
					if (UnityVectorExtensions.Angle(vector.ProjectOntoPlane(Vector3.up), Vector3.forward) > num)
					{
						return true;
					}
				}
			}
			return false;
		}

		internal void UpgradeToCm3(CinemachineDeoccluder c)
		{
			c.CollideAgainst = this.m_CollideAgainst;
			c.IgnoreTag = this.m_IgnoreTag;
			c.TransparentLayers = this.m_TransparentLayers;
			c.MinimumDistanceFromTarget = this.m_MinimumDistanceFromTarget;
			c.AvoidObstacles = new CinemachineDeoccluder.ObstacleAvoidance
			{
				Enabled = this.m_AvoidObstacles,
				DistanceLimit = this.m_DistanceLimit,
				MinimumOcclusionTime = this.m_MinimumOcclusionTime,
				CameraRadius = this.m_CameraRadius,
				Strategy = (CinemachineDeoccluder.ObstacleAvoidance.ResolutionStrategy)this.m_Strategy,
				MaximumEffort = this.m_MaximumEffort,
				SmoothingTime = this.m_SmoothingTime,
				Damping = this.m_Damping,
				DampingWhenOccluded = this.m_DampingWhenOccluded
			};
			if (this.m_OptimalTargetDistance > 0f)
			{
				c.ShotQualityEvaluation.OptimalDistance = this.m_OptimalTargetDistance;
			}
		}

		[Header("Obstacle Detection")]
		[Tooltip("Objects on these layers will be detected")]
		public LayerMask m_CollideAgainst = 1;

		[TagField]
		[Tooltip("Obstacles with this tag will be ignored.  It is a good idea to set this field to the target's tag")]
		public string m_IgnoreTag = string.Empty;

		[Tooltip("Objects on these layers will never obstruct view of the target")]
		public LayerMask m_TransparentLayers = 0;

		[Tooltip("Obstacles closer to the target than this will be ignored")]
		public float m_MinimumDistanceFromTarget = 0.1f;

		[Space]
		[Tooltip("When enabled, will attempt to resolve situations where the line of sight to the target is blocked by an obstacle")]
		[FormerlySerializedAs("m_PreserveLineOfSight")]
		public bool m_AvoidObstacles = true;

		[Tooltip("The maximum raycast distance when checking if the line of sight to this camera's target is clear.  If the setting is 0 or less, the current actual distance to target will be used.")]
		[FormerlySerializedAs("m_LineOfSightFeelerDistance")]
		public float m_DistanceLimit;

		[Tooltip("Don't take action unless occlusion has lasted at least this long.")]
		public float m_MinimumOcclusionTime;

		[Tooltip("Camera will try to maintain this distance from any obstacle.  Try to keep this value small.  Increase it if you are seeing inside obstacles due to a large FOV on the camera.")]
		public float m_CameraRadius = 0.1f;

		[Tooltip("The way in which the Collider will attempt to preserve sight of the target.")]
		public CinemachineCollider.ResolutionStrategy m_Strategy = CinemachineCollider.ResolutionStrategy.PreserveCameraHeight;

		[Range(1f, 10f)]
		[Tooltip("Upper limit on how many obstacle hits to process.  Higher numbers may impact performance.  In most environments, 4 is enough.")]
		public int m_MaximumEffort = 4;

		[Range(0f, 2f)]
		[Tooltip("Smoothing to apply to obstruction resolution.  Nearest camera point is held for at least this long")]
		public float m_SmoothingTime;

		[Range(0f, 10f)]
		[Tooltip("How gradually the camera returns to its normal position after having been corrected.  Higher numbers will move the camera more gradually back to normal.")]
		[FormerlySerializedAs("m_Smoothing")]
		public float m_Damping;

		[Range(0f, 10f)]
		[Tooltip("How gradually the camera moves to resolve an occlusion.  Higher numbers will move the camera more gradually.")]
		public float m_DampingWhenOccluded;

		[Header("Shot Evaluation")]
		[Tooltip("If greater than zero, a higher score will be given to shots when the target is closer to this distance.  Set this to zero to disable this feature.")]
		public float m_OptimalTargetDistance;

		private const float k_PrecisionSlush = 0.001f;

		private List<CinemachineCollider.VcamExtraState> m_extraStateCache;

		private RaycastHit[] m_CornerBuffer = new RaycastHit[4];

		private const float k_AngleThreshold = 0.1f;

		private static Collider[] s_ColliderBuffer = new Collider[5];

		public enum ResolutionStrategy
		{
			PullCameraForward,
			PreserveCameraHeight,
			PreserveCameraDistance
		}

		private class VcamExtraState : CinemachineExtension.VcamExtraStateBase
		{
			public void AddPointToDebugPath(Vector3 p)
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
				if (this.m_SmoothedDistance == 0f || distance < this.m_SmoothedDistance)
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

			public Vector3 previousDisplacement;

			public Vector3 previousCameraOffset;

			public Vector3 previousCameraPosition;

			public float previousDampTime;

			public bool targetObscured;

			public float occlusionStartTime;

			public List<Vector3> debugResolutionPath;

			private float m_SmoothedDistance;

			private float m_SmoothedTime;
		}
	}
}
