using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.Cinemachine
{
	[AddComponentMenu("Cinemachine/Procedural/Extensions/Cinemachine Confiner 2D")]
	[SaveDuringPlay]
	[ExecuteAlways]
	[DisallowMultipleComponent]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineConfiner2D.html")]
	public class CinemachineConfiner2D : CinemachineExtension
	{
		private void OnValidate()
		{
			this.Damping = Mathf.Max(0f, this.Damping);
			this.SlowingDistance = Mathf.Max(0f, this.SlowingDistance);
			this.m_ShapeCache.MaxComputationTimePerFrameInSeconds = 0.008333334f;
			this.OversizeWindow.MaxWindowSize = Mathf.Max(0f, this.OversizeWindow.MaxWindowSize);
			if (this.m_LegacyMaxWindowSize != -2f)
			{
				this.OversizeWindow = new CinemachineConfiner2D.OversizeWindowSettings
				{
					Enabled = (this.m_LegacyMaxWindowSize >= 0f),
					MaxWindowSize = Mathf.Max(0f, this.m_LegacyMaxWindowSize)
				};
				this.m_LegacyMaxWindowSize = -2f;
			}
		}

		private void Reset()
		{
			this.Damping = 0.5f;
			this.SlowingDistance = 5f;
			this.OversizeWindow = default(CinemachineConfiner2D.OversizeWindowSettings);
		}

		public override float GetMaxDampTime()
		{
			return Mathf.Max(this.Damping, this.SlowingDistance * 0.2f);
		}

		public override void OnTargetObjectWarped(CinemachineVirtualCameraBase vcam, Transform target, Vector3 positionDelta)
		{
			CinemachineConfiner2D.VcamExtraState extraState = base.GetExtraState<CinemachineConfiner2D.VcamExtraState>(vcam);
			if (extraState.Vcam.Follow == target)
			{
				extraState.PreviousCameraPosition += positionDelta;
			}
		}

		public void InvalidateLensCache()
		{
			if (this.m_ExtraStateCache == null)
			{
				this.m_ExtraStateCache = new List<CinemachineConfiner2D.VcamExtraState>();
			}
			base.GetAllExtraStates<CinemachineConfiner2D.VcamExtraState>(this.m_ExtraStateCache);
			for (int i = 0; i < this.m_ExtraStateCache.Count; i++)
			{
				CinemachineConfiner2D.VcamExtraState vcamExtraState = this.m_ExtraStateCache[i];
				if (vcamExtraState.Vcam != null)
				{
					vcamExtraState.BakedSolution = null;
					vcamExtraState.FrustumHeight = 0f;
				}
			}
		}

		public void InvalidateBoundingShapeCache()
		{
			this.m_ShapeCache.Invalidate();
			this.InvalidateLensCache();
		}

		[Obsolete("Call InvalidateBoundingShapeCache() instead.", false)]
		public void InvalidateCache()
		{
			this.InvalidateBoundingShapeCache();
		}

		public bool BoundingShapeIsBaked
		{
			get
			{
				ConfinerOven confinerOven = this.m_ShapeCache.ConfinerOven;
				return confinerOven != null && confinerOven.State == ConfinerOven.BakingState.BAKED;
			}
		}

		public bool BakeBoundingShape(CinemachineVirtualCameraBase vcam, float maxTimeInSeconds)
		{
			bool flag;
			if (!this.m_ShapeCache.ValidateCache(this.BoundingShape2D, this.OversizeWindow, vcam.State.Lens.Aspect, out flag))
			{
				return false;
			}
			if (this.m_ShapeCache.ConfinerOven == null)
			{
				return false;
			}
			if (this.m_ShapeCache.ConfinerOven.State == ConfinerOven.BakingState.BAKING)
			{
				this.m_ShapeCache.ConfinerOven.BakeConfiner(maxTimeInSeconds);
			}
			return this.m_ShapeCache.ConfinerOven.State == ConfinerOven.BakingState.BAKED;
		}

		protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
		{
			if (stage == CinemachineCore.Stage.Body)
			{
				float aspect = state.Lens.Aspect;
				bool flag;
				if (!this.m_ShapeCache.ValidateCache(this.BoundingShape2D, this.OversizeWindow, aspect, out flag))
				{
					return;
				}
				CinemachineConfiner2D.VcamExtraState extraState = base.GetExtraState<CinemachineConfiner2D.VcamExtraState>(vcam);
				Vector3 correctedPosition = state.GetCorrectedPosition();
				if (flag || extraState.BakedSolution == null || !extraState.BakedSolution.IsValid())
				{
					Matrix4x4 deltaWorldToBaked = this.m_ShapeCache.DeltaWorldToBaked;
					this.m_ShapeCache.AspectRatio = aspect;
					extraState.FrustumHeight = CinemachineConfiner2D.CalculateHalfFrustumHeight(state.Lens, deltaWorldToBaked.MultiplyPoint3x4(correctedPosition).z) * deltaWorldToBaked.lossyScale.x;
					extraState.BakedSolution = this.m_ShapeCache.ConfinerOven.GetBakedSolution(extraState.FrustumHeight);
				}
				Vector3 fwd = state.GetCorrectedOrientation() * Vector3.forward;
				Vector3 vector = this.ConfinePoint(correctedPosition, extraState, fwd);
				if (this.SlowingDistance > 0.0001f && deltaTime >= 0f && vcam.PreviousStateIsValid)
				{
					Vector3 previousCameraPosition = extraState.PreviousCameraPosition;
					Vector3 a = vector - previousCameraPosition;
					float magnitude = a.magnitude;
					if (magnitude > 0.0001f)
					{
						float num = this.GetDistanceFromEdge(previousCameraPosition, a / magnitude, this.SlowingDistance, extraState, fwd) / this.SlowingDistance;
						vector = Vector3.Lerp(previousCameraPosition, vector, num * num * num + 0.05f);
					}
				}
				Vector3 previousDisplacement = extraState.PreviousDisplacement;
				Vector3 vector2 = vector - correctedPosition;
				extraState.PreviousDisplacement = vector2;
				if (!vcam.PreviousStateIsValid || deltaTime < 0f || this.Damping <= 0f)
				{
					extraState.DampedDisplacement = Vector3.zero;
				}
				else
				{
					if (previousDisplacement.sqrMagnitude > 0.01f && Vector2.Angle(previousDisplacement, vector2) > 10f)
					{
						extraState.DampedDisplacement += vector2 - previousDisplacement;
					}
					extraState.DampedDisplacement -= Damper.Damp(extraState.DampedDisplacement, this.Damping, deltaTime);
					vector2 -= extraState.DampedDisplacement;
				}
				state.PositionCorrection += vector2;
				extraState.PreviousCameraPosition = state.GetCorrectedPosition();
			}
		}

		private Vector3 ConfinePoint(Vector3 pos, CinemachineConfiner2D.VcamExtraState extra, Vector3 fwd)
		{
			Vector3 v = this.m_ShapeCache.DeltaWorldToBaked.MultiplyPoint3x4(pos);
			ConfinerOven.BakedSolution bakedSolution = extra.BakedSolution;
			Vector2 vector = v;
			Vector3 a = this.m_ShapeCache.DeltaBakedToWorld.MultiplyPoint3x4(bakedSolution.ConfinePoint(vector));
			return a - fwd * Vector3.Dot(fwd, a - pos);
		}

		private float GetDistanceFromEdge(Vector3 p, Vector3 dirUnit, float max, CinemachineConfiner2D.VcamExtraState extra, Vector3 fwd)
		{
			p += dirUnit * max;
			return Mathf.Max(0f, max - (this.ConfinePoint(p, extra, fwd) - p).magnitude);
		}

		public static float CalculateHalfFrustumHeight(in LensSettings lens, in float cameraPosLocalZ)
		{
			LensSettings lensSettings = lens;
			float f;
			if (lensSettings.Orthographic)
			{
				f = lens.OrthographicSize;
			}
			else
			{
				f = cameraPosLocalZ * Mathf.Tan(lens.FieldOfView * 0.5f * 0.017453292f);
			}
			return Mathf.Abs(f);
		}

		[Tooltip("The 2D shape within which the camera is to be contained.  Can be polygon-, box-, or composite collider 2D.\n\nRemark: When assigning a GameObject here in the editor, this will be set to the first Collider2D found on the assigned GameObject!")]
		[FormerlySerializedAs("m_BoundingShape2D")]
		public Collider2D BoundingShape2D;

		[Tooltip("Damping applied around corners to avoid jumps.  Higher numbers are more gradual.")]
		[Range(0f, 5f)]
		[FormerlySerializedAs("m_Damping")]
		public float Damping;

		[Tooltip("Size of the slow-down zone at the edge of the bounding shape.")]
		public float SlowingDistance;

		[FoldoutWithEnabledButton("Enabled")]
		public CinemachineConfiner2D.OversizeWindowSettings OversizeWindow;

		private List<CinemachineConfiner2D.VcamExtraState> m_ExtraStateCache;

		private CinemachineConfiner2D.ShapeCache m_ShapeCache;

		[SerializeField]
		[HideInInspector]
		[NoSaveDuringPlay]
		[FormerlySerializedAs("m_MaxWindowSize")]
		private float m_LegacyMaxWindowSize = -2f;

		private const float k_CornerAngleThreshold = 10f;

		[Serializable]
		public struct OversizeWindowSettings
		{
			[Tooltip("Enable optimizing of computation and memory costs in the event that the window size is expected to be larger than will fit inside the confining shape.\nEnable only if needed, because it's costly")]
			public bool Enabled;

			[Tooltip("To optimize computation and memory costs, set this to the largest view size that the camera is expected to have.  The confiner will not compute a polygon cache for frustum sizes larger than this.  This refers to the size in world units of the frustum at the confiner plane (for orthographic cameras, this is just the orthographic size).  If set to 0, then this parameter is ignored and a polygon cache will be calculated for all potential window sizes.")]
			public float MaxWindowSize;

			[Tooltip("For large window sizes, the confiner will potentially generate polygons with zero area.  The padding may be used to add a small amount of area to these polygons, to prevent them from being a series of disconnected dots.")]
			[Range(0f, 100f)]
			public float Padding;
		}

		private class VcamExtraState : CinemachineExtension.VcamExtraStateBase
		{
			public ConfinerOven.BakedSolution BakedSolution;

			public Vector3 PreviousDisplacement;

			public Vector3 DampedDisplacement;

			public Vector3 PreviousCameraPosition;

			public float FrustumHeight;
		}

		private struct ShapeCache
		{
			public void Invalidate()
			{
				this.m_OversizeWindowSettings = default(CinemachineConfiner2D.OversizeWindowSettings);
				this.DeltaBakedToWorld = (this.DeltaWorldToBaked = Matrix4x4.identity);
				this.m_BoundingShape2D = null;
				this.OriginalPath = null;
				this.ConfinerOven = null;
			}

			public bool ValidateCache(Collider2D boundingShape2D, CinemachineConfiner2D.OversizeWindowSettings oversize, float aspectRatio, out bool confinerStateChanged)
			{
				confinerStateChanged = false;
				if (this.IsValid(boundingShape2D, oversize, aspectRatio))
				{
					if (this.ConfinerOven.State == ConfinerOven.BakingState.BAKING)
					{
						this.ConfinerOven.BakeConfiner(this.MaxComputationTimePerFrameInSeconds);
						confinerStateChanged = (this.ConfinerOven.State > ConfinerOven.BakingState.BAKING);
					}
					this.CalculateDeltaTransformationMatrix();
					if (this.DeltaWorldToBaked.lossyScale.IsUniform())
					{
						return true;
					}
				}
				this.Invalidate();
				if (boundingShape2D == null)
				{
					return false;
				}
				confinerStateChanged = true;
				PolygonCollider2D polygonCollider2D = boundingShape2D as PolygonCollider2D;
				if (polygonCollider2D == null)
				{
					BoxCollider2D boxCollider2D = boundingShape2D as BoxCollider2D;
					if (boxCollider2D == null)
					{
						CompositeCollider2D compositeCollider2D = boundingShape2D as CompositeCollider2D;
						if (compositeCollider2D == null)
						{
							return false;
						}
						this.OriginalPath = new List<List<Vector2>>();
						this.m_BakedToWorld = boundingShape2D.transform.localToWorldMatrix;
						Vector2[] array = new Vector2[compositeCollider2D.pointCount];
						for (int i = 0; i < compositeCollider2D.pathCount; i++)
						{
							int path = compositeCollider2D.GetPath(i, array);
							List<Vector2> list = new List<Vector2>();
							for (int j = 0; j < path; j++)
							{
								list.Add(this.m_BakedToWorld.MultiplyPoint3x4(array[j]));
							}
							this.OriginalPath.Add(list);
						}
					}
					else
					{
						this.m_BakedToWorld = boundingShape2D.transform.localToWorldMatrix;
						Vector2 size = boxCollider2D.size;
						float num = size.y / 2f;
						float num2 = size.x / 2f;
						Vector3 v = this.m_BakedToWorld.MultiplyPoint3x4(new Vector3(-num2, num));
						Vector3 v2 = this.m_BakedToWorld.MultiplyPoint3x4(new Vector3(num2, num));
						Vector3 v3 = this.m_BakedToWorld.MultiplyPoint3x4(new Vector3(num2, -num));
						Vector3 v4 = this.m_BakedToWorld.MultiplyPoint3x4(new Vector3(-num2, -num));
						this.OriginalPath = new List<List<Vector2>>
						{
							new List<Vector2>
							{
								v,
								v2,
								v3,
								v4
							}
						};
					}
				}
				else
				{
					this.OriginalPath = new List<List<Vector2>>();
					this.m_BakedToWorld = boundingShape2D.transform.localToWorldMatrix;
					for (int k = 0; k < polygonCollider2D.pathCount; k++)
					{
						Vector2[] path2 = polygonCollider2D.GetPath(k);
						List<Vector2> list2 = new List<Vector2>();
						for (int l = 0; l < path2.Length; l++)
						{
							list2.Add(this.m_BakedToWorld.MultiplyPoint3x4(path2[l]));
						}
						this.OriginalPath.Add(list2);
					}
				}
				if (!CinemachineConfiner2D.ShapeCache.<ValidateCache>g__HasAnyPoints|10_0(this.OriginalPath))
				{
					return false;
				}
				this.ConfinerOven = new ConfinerOven(ref this.OriginalPath, ref aspectRatio, oversize.Enabled ? oversize.MaxWindowSize : -1f, oversize.Padding);
				this.m_BoundingShape2D = boundingShape2D;
				this.m_OversizeWindowSettings = oversize;
				this.AspectRatio = aspectRatio;
				this.CalculateDeltaTransformationMatrix();
				return true;
			}

			private bool IsValid(in Collider2D boundingShape2D, in CinemachineConfiner2D.OversizeWindowSettings oversize, float aspectRatio)
			{
				return boundingShape2D != null && this.m_BoundingShape2D != null && this.m_BoundingShape2D == boundingShape2D && this.OriginalPath != null && this.ConfinerOven != null && Math.Abs(this.AspectRatio - aspectRatio) < 0.0001f && this.m_OversizeWindowSettings.Enabled == oversize.Enabled && this.m_OversizeWindowSettings.Padding == oversize.Padding && Mathf.Abs(this.m_OversizeWindowSettings.MaxWindowSize - oversize.MaxWindowSize) < 0.0001f;
			}

			private void CalculateDeltaTransformationMatrix()
			{
				Matrix4x4 rhs = Matrix4x4.Translate(-this.m_BoundingShape2D.offset) * this.m_BoundingShape2D.transform.worldToLocalMatrix;
				this.DeltaWorldToBaked = this.m_BakedToWorld * rhs;
				this.DeltaBakedToWorld = this.DeltaWorldToBaked.inverse;
			}

			[CompilerGenerated]
			internal static bool <ValidateCache>g__HasAnyPoints|10_0(List<List<Vector2>> originalPath)
			{
				for (int i = 0; i < originalPath.Count; i++)
				{
					if (originalPath[i].Count != 0)
					{
						return true;
					}
				}
				return false;
			}

			public ConfinerOven ConfinerOven;

			public List<List<Vector2>> OriginalPath;

			public Matrix4x4 DeltaWorldToBaked;

			public Matrix4x4 DeltaBakedToWorld;

			public float AspectRatio;

			private CinemachineConfiner2D.OversizeWindowSettings m_OversizeWindowSettings;

			internal float MaxComputationTimePerFrameInSeconds;

			private Matrix4x4 m_BakedToWorld;

			private Collider2D m_BoundingShape2D;
		}
	}
}
