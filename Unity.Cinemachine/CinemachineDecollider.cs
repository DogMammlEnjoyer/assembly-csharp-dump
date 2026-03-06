using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Cinemachine
{
	[AddComponentMenu("Cinemachine/Procedural/Extensions/Cinemachine Decollider")]
	[SaveDuringPlay]
	[ExecuteAlways]
	[DisallowMultipleComponent]
	[RequiredTarget(RequiredTargetAttribute.RequiredTargets.Tracking)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineDecollider.html")]
	public class CinemachineDecollider : CinemachineExtension
	{
		private void OnValidate()
		{
			this.CameraRadius = Mathf.Max(0.01f, this.CameraRadius);
		}

		private void Reset()
		{
			this.CameraRadius = 0.4f;
			this.TerrainResolution = new CinemachineDecollider.TerrainSettings
			{
				Enabled = true,
				TerrainLayers = 1,
				MaximumRaycast = 10f,
				Damping = 0.5f
			};
			this.Decollision = new CinemachineDecollider.DecollisionSettings
			{
				Enabled = false,
				ObstacleLayers = 1,
				Damping = 0.5f
			};
		}

		protected override void OnDestroy()
		{
			RuntimeUtility.DestroyScratchCollider();
			base.OnDestroy();
		}

		public override float GetMaxDampTime()
		{
			return Mathf.Max(this.Decollision.Enabled ? this.Decollision.Damping : 0f, this.TerrainResolution.Enabled ? this.TerrainResolution.Damping : 0f);
		}

		public override void ForceCameraPosition(CinemachineVirtualCameraBase vcam, Vector3 pos, Quaternion rot)
		{
			base.GetExtraState<CinemachineDecollider.VcamExtraState>(vcam).PreviousCorrectedCameraPosition = pos;
		}

		protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
		{
			if (stage == CinemachineCore.Stage.Body)
			{
				CinemachineDecollider.VcamExtraState extraState = base.GetExtraState<CinemachineDecollider.VcamExtraState>(vcam);
				Vector3 referenceUp = state.ReferenceUp;
				Vector3 correctedPosition = state.GetCorrectedPosition();
				bool flag = state.HasLookAt();
				Vector3 vector = flag ? state.ReferenceLookAt : state.GetCorrectedPosition();
				Vector3 avoidanceResolutionTargetPoint = this.GetAvoidanceResolutionTargetPoint(vcam, ref state);
				Vector2 a = flag ? state.RawOrientation.GetCameraRotationToTarget(vector - correctedPosition, state.ReferenceUp) : Vector2.zero;
				if (!vcam.PreviousStateIsValid)
				{
					deltaTime = -1f;
				}
				extraState.PreviousTerrainDisplacement = (this.TerrainResolution.Enabled ? this.ResolveTerrain(extraState, state.GetCorrectedPosition(), referenceUp, deltaTime) : 0f);
				state.PositionCorrection += extraState.PreviousTerrainDisplacement * referenceUp;
				if (this.Decollision.Enabled)
				{
					Vector3 correctedPosition2 = state.GetCorrectedPosition();
					Vector3 vector2 = this.DecollideCamera(correctedPosition2, avoidanceResolutionTargetPoint);
					vector2 = this.ApplySmoothingAndDamping(vector2, avoidanceResolutionTargetPoint, correctedPosition2, extraState, deltaTime);
					if (!vector2.AlmostZero())
					{
						state.PositionCorrection += vector2;
						float num = this.TerrainResolution.Enabled ? this.ResolveTerrain(extraState, state.GetCorrectedPosition(), referenceUp, -1f) : 0f;
						if (Mathf.Abs(num) > 0.0001f)
						{
							state.PositionCorrection += num * referenceUp;
							extraState.PreviousTerrainDisplacement = 0f;
						}
					}
				}
				Vector3 correctedPosition3 = state.GetCorrectedPosition();
				if (flag && !(correctedPosition - correctedPosition3).AlmostZero())
				{
					Quaternion orient = Quaternion.LookRotation(vector - correctedPosition3, referenceUp);
					state.RawOrientation = orient.ApplyCameraRotation(-a, referenceUp);
					if (deltaTime >= 0f)
					{
						Vector3 v = extraState.PreviousCorrectedCameraPosition - vector;
						Vector3 v2 = correctedPosition3 - vector;
						if (v.sqrMagnitude > 0.0001f && v2.sqrMagnitude > 0.0001f)
						{
							state.RotationDampingBypass = UnityVectorExtensions.SafeFromToRotation(v, v2, referenceUp);
						}
					}
				}
				extraState.PreviousCorrectedCameraPosition = correctedPosition3;
			}
		}

		private Vector3 GetAvoidanceResolutionTargetPoint(CinemachineVirtualCameraBase vcam, ref CameraState state)
		{
			Vector3 result = state.HasLookAt() ? state.ReferenceLookAt : state.GetCorrectedPosition();
			if (this.Decollision.UseFollowTarget.Enabled)
			{
				Transform follow = vcam.Follow;
				if (follow != null)
				{
					result = TargetPositionCache.GetTargetPosition(follow) + TargetPositionCache.GetTargetRotation(follow) * Vector3.up * this.Decollision.UseFollowTarget.YOffset;
				}
			}
			return result;
		}

		private float ResolveTerrain(CinemachineDecollider.VcamExtraState extra, Vector3 camPos, Vector3 up, float deltaTime)
		{
			float num = 0f;
			RaycastHit raycastHit;
			if (RuntimeUtility.SphereCastIgnoreTag(new Ray(camPos + this.TerrainResolution.MaximumRaycast * up, -up), this.CameraRadius + 0.0001f, out raycastHit, this.TerrainResolution.MaximumRaycast, this.TerrainResolution.TerrainLayers, string.Empty))
			{
				num = this.TerrainResolution.MaximumRaycast - raycastHit.distance + 0.0001f;
			}
			if (deltaTime >= 0f && this.TerrainResolution.Damping > 0.0001f && num < extra.PreviousTerrainDisplacement)
			{
				num = extra.PreviousTerrainDisplacement + Damper.Damp(num - extra.PreviousTerrainDisplacement, this.TerrainResolution.Damping, deltaTime);
			}
			return num;
		}

		private Vector3 DecollideCamera(Vector3 cameraPos, Vector3 lookAtPoint)
		{
			LayerMask mask = this.Decollision.ObstacleLayers;
			if (this.TerrainResolution.Enabled)
			{
				mask &= ~this.TerrainResolution.TerrainLayers;
			}
			if (mask == 0)
			{
				return Vector3.zero;
			}
			Vector3 vector = cameraPos - lookAtPoint;
			float magnitude = vector.magnitude;
			if (magnitude < 0.0001f)
			{
				return Vector3.zero;
			}
			int num = Physics.OverlapCapsuleNonAlloc(lookAtPoint, cameraPos, this.CameraRadius - 0.0001f, CinemachineDecollider.s_ColliderBuffer, mask, QueryTriggerInteraction.Ignore);
			if (num == 0)
			{
				return Vector3.zero;
			}
			vector /= magnitude;
			for (int i = 0; i < num; i++)
			{
				Collider collider = CinemachineDecollider.s_ColliderBuffer[i];
				CinemachineDecollider.s_ColliderOrderBuffer[i] = i;
				CinemachineDecollider.s_ColliderDistanceBuffer[i] = 0f;
				RaycastHit raycastHit;
				if (collider.Raycast(new Ray(lookAtPoint, vector), out raycastHit, magnitude + this.CameraRadius))
				{
					float num2 = raycastHit.distance - this.CameraRadius;
					if (num2 < this.CameraRadius)
					{
						num2 = Mathf.Max(0.01f, num2 + (this.CameraRadius - num2) * 0.5f);
					}
					CinemachineDecollider.s_ColliderDistanceBuffer[i] = num2;
				}
			}
			Array.Sort<int>(CinemachineDecollider.s_ColliderOrderBuffer, 0, num, CinemachineDecollider.s_ColliderBufferSorter);
			Vector3 vector2 = cameraPos;
			SphereCollider scratchCollider = RuntimeUtility.GetScratchCollider();
			scratchCollider.radius = this.CameraRadius - 0.0001f;
			for (int j = 0; j < num; j++)
			{
				int num3 = CinemachineDecollider.s_ColliderOrderBuffer[j];
				if (CinemachineDecollider.s_ColliderDistanceBuffer[num3] != 0f)
				{
					Collider collider2 = CinemachineDecollider.s_ColliderBuffer[num3];
					Vector3 vector3;
					float num4;
					if (Physics.ComputePenetration(scratchCollider, vector2, Quaternion.identity, collider2, collider2.transform.position, collider2.transform.rotation, out vector3, out num4))
					{
						vector2 = lookAtPoint + vector * CinemachineDecollider.s_ColliderDistanceBuffer[num3];
					}
				}
			}
			return vector2 - cameraPos;
		}

		private Vector3 ApplySmoothingAndDamping(Vector3 displacement, Vector3 lookAtPoint, Vector3 oldCamPos, CinemachineDecollider.VcamExtraState extra, float deltaTime)
		{
			Vector3 a = oldCamPos + displacement - lookAtPoint;
			float num = float.MaxValue;
			if (deltaTime >= 0f)
			{
				num = a.magnitude;
				if (num > this.CameraRadius)
				{
					Vector3 a2 = a / num;
					if (this.Decollision.SmoothingTime > 0.0001f)
					{
						num = extra.UpdateDistanceSmoothing(num, this.Decollision.SmoothingTime, !displacement.AlmostZero());
						displacement = lookAtPoint + a2 * num - oldCamPos;
					}
					if (this.Decollision.Damping > 0.0001f && num > extra.PreviousDistanceFromTarget)
					{
						float num2 = extra.PreviousDistanceFromTarget;
						float num3 = (oldCamPos - lookAtPoint).magnitude - extra.PreviouDecollisionDisplacement.magnitude;
						if (Mathf.Abs(num - num3) < Mathf.Abs(num - num2))
						{
							num2 = num3;
						}
						num = num2 + Damper.Damp(num - num2, this.Decollision.Damping, deltaTime);
						displacement = lookAtPoint + a2 * num - oldCamPos;
					}
				}
			}
			extra.PreviousDistanceFromTarget = num;
			extra.PreviouDecollisionDisplacement = displacement;
			return displacement;
		}

		[Tooltip("Camera will try to maintain this distance from any obstacle or terrain.  Increase it if necessary to keep the camera from clipping the near edge of obsacles.")]
		public float CameraRadius = 0.4f;

		[FoldoutWithEnabledButton("Enabled")]
		public CinemachineDecollider.DecollisionSettings Decollision;

		[FoldoutWithEnabledButton("Enabled")]
		public CinemachineDecollider.TerrainSettings TerrainResolution;

		private const int kColliderBufferSize = 10;

		private static Collider[] s_ColliderBuffer = new Collider[10];

		private static float[] s_ColliderDistanceBuffer = new float[10];

		private static int[] s_ColliderOrderBuffer = new int[10];

		private static readonly IComparer<int> s_ColliderBufferSorter = Comparer<int>.Create(delegate(int a, int b)
		{
			if (CinemachineDecollider.s_ColliderDistanceBuffer[a] == CinemachineDecollider.s_ColliderDistanceBuffer[b])
			{
				return 0;
			}
			if (CinemachineDecollider.s_ColliderDistanceBuffer[a] <= CinemachineDecollider.s_ColliderDistanceBuffer[b])
			{
				return 1;
			}
			return -1;
		});

		[Serializable]
		public struct DecollisionSettings
		{
			[Tooltip("When enabled, will attempt to push the camera out of intersecting objects")]
			public bool Enabled;

			[Tooltip("Objects on these layers will be detected")]
			public LayerMask ObstacleLayers;

			[EnabledProperty("Enabled", "")]
			public CinemachineDecollider.DecollisionSettings.FollowTargetSettings UseFollowTarget;

			[Range(0f, 10f)]
			[Tooltip("How gradually the camera returns to its normal position after having been corrected.  Higher numbers will move the camera more gradually back to normal.")]
			public float Damping;

			[Range(0f, 2f)]
			[Tooltip("Smoothing to apply to obstruction resolution.  Nearest camera point is held for at least this long")]
			public float SmoothingTime;

			[Serializable]
			public struct FollowTargetSettings
			{
				[Tooltip("Use the Follow target when resolving occlusions, instead of the LookAt target.")]
				public bool Enabled;

				[Tooltip("Vertical offset from the Follow target's root, in target local space")]
				public float YOffset;
			}
		}

		[Serializable]
		public struct TerrainSettings
		{
			[Tooltip("When enabled, will attempt to place the camera on top of terrain layers")]
			public bool Enabled;

			[Tooltip("Colliders on these layers will be detected")]
			public LayerMask TerrainLayers;

			[Tooltip("Specifies the maximum length of a raycast used to find terrain colliders")]
			public float MaximumRaycast;

			[Range(0f, 10f)]
			[Tooltip("How gradually the camera returns to its normal position after having been corrected.  Higher numbers will move the camera more gradually back to normal.")]
			public float Damping;
		}

		private class VcamExtraState : CinemachineExtension.VcamExtraStateBase
		{
			public float UpdateDistanceSmoothing(float distance, float smoothingTime, bool haveDisplacement)
			{
				if (haveDisplacement && (this.m_SmoothedDistance == 0f || distance <= this.m_SmoothedDistance))
				{
					this.m_SmoothedDistance = distance;
					this.m_SmoothingStartTime = CinemachineCore.CurrentTime;
				}
				if (this.m_SmoothingStartTime != 0f && CinemachineCore.CurrentTime - this.m_SmoothingStartTime < smoothingTime)
				{
					distance = Mathf.Min(distance, this.m_SmoothedDistance);
				}
				if (!haveDisplacement && CinemachineCore.CurrentTime - this.m_SmoothingStartTime >= smoothingTime)
				{
					this.m_SmoothedDistance = (this.m_SmoothingStartTime = 0f);
				}
				return distance;
			}

			public float PreviousTerrainDisplacement;

			public float PreviousDistanceFromTarget;

			public Vector3 PreviouDecollisionDisplacement;

			public Vector3 PreviousCorrectedCameraPosition;

			private float m_SmoothedDistance;

			private float m_SmoothingStartTime;
		}
	}
}
