using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Cinemachine
{
	[Obsolete("CinemachineConfiner has been deprecated. Use CinemachineConfiner2D or CinemachineConfiner3D instead")]
	[AddComponentMenu("")]
	[SaveDuringPlay]
	[ExecuteAlways]
	[DisallowMultipleComponent]
	public class CinemachineConfiner : CinemachineExtension
	{
		public bool CameraWasDisplaced(CinemachineVirtualCameraBase vcam)
		{
			return this.GetCameraDisplacementDistance(vcam) > 0f;
		}

		public float GetCameraDisplacementDistance(CinemachineVirtualCameraBase vcam)
		{
			return base.GetExtraState<CinemachineConfiner.VcamExtraState>(vcam).ConfinerDisplacement;
		}

		private void Reset()
		{
			this.m_ConfineMode = CinemachineConfiner.Mode.Confine3D;
			this.m_BoundingVolume = null;
			this.m_BoundingShape2D = null;
			this.m_ConfineScreenEdges = true;
			this.m_Damping = 0f;
		}

		private void OnValidate()
		{
			this.m_Damping = Mathf.Max(0f, this.m_Damping);
		}

		protected override void ConnectToVcam(bool connect)
		{
			base.ConnectToVcam(connect);
		}

		public bool IsValid
		{
			get
			{
				return (this.m_ConfineMode == CinemachineConfiner.Mode.Confine3D && this.m_BoundingVolume != null && this.m_BoundingVolume.enabled && this.m_BoundingVolume.gameObject.activeInHierarchy) || (this.m_ConfineMode == CinemachineConfiner.Mode.Confine2D && this.m_BoundingShape2D != null && this.m_BoundingShape2D.enabled && this.m_BoundingShape2D.gameObject.activeInHierarchy);
			}
		}

		public override float GetMaxDampTime()
		{
			return this.m_Damping;
		}

		protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
		{
			if (this.IsValid && stage == CinemachineCore.Stage.Body)
			{
				CinemachineConfiner.VcamExtraState extraState = base.GetExtraState<CinemachineConfiner.VcamExtraState>(vcam);
				Vector3 vector;
				if (this.m_ConfineScreenEdges && state.Lens.Orthographic)
				{
					vector = this.ConfineOrthoCameraToScreenEdges(ref state);
				}
				else
				{
					vector = this.ConfinePoint(state.GetCorrectedPosition());
				}
				if (this.m_Damping > 0f && deltaTime >= 0f && vcam.PreviousStateIsValid)
				{
					Vector3 vector2 = vector - extraState.PreviousDisplacement;
					vector2 = Damper.Damp(vector2, this.m_Damping, deltaTime);
					vector = extraState.PreviousDisplacement + vector2;
				}
				extraState.PreviousDisplacement = vector;
				state.PositionCorrection += vector;
				extraState.ConfinerDisplacement = vector.magnitude;
			}
		}

		[Obsolete("Please use InvalidateCache() instead")]
		public void InvalidatePathCache()
		{
			this.InvalidatePathCache();
		}

		public void InvalidateCache()
		{
			this.m_PathCache = null;
			this.m_BoundingShape2DCache = null;
		}

		private bool ValidatePathCache()
		{
			if (this.m_BoundingShape2DCache != this.m_BoundingShape2D)
			{
				this.InvalidateCache();
				this.m_BoundingShape2DCache = this.m_BoundingShape2D;
			}
			Type left = (this.m_BoundingShape2D == null) ? null : this.m_BoundingShape2D.GetType();
			if (left == typeof(PolygonCollider2D))
			{
				PolygonCollider2D polygonCollider2D = this.m_BoundingShape2D as PolygonCollider2D;
				if (this.m_PathCache == null || this.m_PathCache.Count != polygonCollider2D.pathCount || this.m_PathTotalPointCount != polygonCollider2D.GetTotalPointCount())
				{
					this.m_PathCache = new List<List<Vector2>>();
					for (int i = 0; i < polygonCollider2D.pathCount; i++)
					{
						Vector2[] path = polygonCollider2D.GetPath(i);
						List<Vector2> list = new List<Vector2>();
						for (int j = 0; j < path.Length; j++)
						{
							list.Add(path[j]);
						}
						this.m_PathCache.Add(list);
					}
					this.m_PathTotalPointCount = polygonCollider2D.GetTotalPointCount();
				}
				return true;
			}
			if (left == typeof(CompositeCollider2D))
			{
				CompositeCollider2D compositeCollider2D = this.m_BoundingShape2D as CompositeCollider2D;
				if (this.m_PathCache == null || this.m_PathCache.Count != compositeCollider2D.pathCount || this.m_PathTotalPointCount != compositeCollider2D.pointCount)
				{
					this.m_PathCache = new List<List<Vector2>>();
					Vector2[] array = new Vector2[compositeCollider2D.pointCount];
					Vector3 lossyScale = this.m_BoundingShape2D.transform.lossyScale;
					Vector2 b = new Vector2(1f / lossyScale.x, 1f / lossyScale.y);
					for (int k = 0; k < compositeCollider2D.pathCount; k++)
					{
						int path2 = compositeCollider2D.GetPath(k, array);
						List<Vector2> list2 = new List<Vector2>();
						for (int l = 0; l < path2; l++)
						{
							list2.Add(array[l] * b);
						}
						this.m_PathCache.Add(list2);
					}
					this.m_PathTotalPointCount = compositeCollider2D.pointCount;
				}
				return true;
			}
			this.InvalidateCache();
			return false;
		}

		private Vector3 ConfinePoint(Vector3 camPos)
		{
			if (this.m_ConfineMode == CinemachineConfiner.Mode.Confine3D)
			{
				return this.m_BoundingVolume.ClosestPoint(camPos) - camPos;
			}
			Vector2 vector = camPos;
			Vector2 a = vector;
			if (this.m_BoundingShape2D.OverlapPoint(camPos))
			{
				return Vector3.zero;
			}
			if (!this.ValidatePathCache())
			{
				return Vector3.zero;
			}
			float num = float.MaxValue;
			for (int i = 0; i < this.m_PathCache.Count; i++)
			{
				int count = this.m_PathCache[i].Count;
				if (count > 0)
				{
					Vector3 v = this.m_BoundingShape2D.transform.TransformPoint(this.m_PathCache[i][count - 1] + this.m_BoundingShape2D.offset);
					for (int j = 0; j < count; j++)
					{
						Vector2 vector2 = this.m_BoundingShape2D.transform.TransformPoint(this.m_PathCache[i][j] + this.m_BoundingShape2D.offset);
						Vector2 vector3 = Vector2.Lerp(v, vector2, vector.ClosestPointOnSegment(v, vector2));
						float num2 = Vector2.SqrMagnitude(vector - vector3);
						if (num2 < num)
						{
							num = num2;
							a = vector3;
						}
						v = vector2;
					}
				}
			}
			return a - vector;
		}

		private Vector3 ConfineOrthoCameraToScreenEdges(ref CameraState state)
		{
			Quaternion correctedOrientation = state.GetCorrectedOrientation();
			float orthographicSize = state.Lens.OrthographicSize;
			float d = orthographicSize * state.Lens.Aspect;
			Vector3 b = correctedOrientation * Vector3.right * d;
			Vector3 b2 = correctedOrientation * Vector3.up * orthographicSize;
			Vector3 vector = Vector3.zero;
			Vector3 a = state.GetCorrectedPosition();
			Vector3 b3 = Vector3.zero;
			for (int i = 0; i < 12; i++)
			{
				Vector3 vector2 = this.ConfinePoint(a - b2 - b);
				if (vector2.AlmostZero())
				{
					vector2 = this.ConfinePoint(a + b2 + b);
				}
				if (vector2.AlmostZero())
				{
					vector2 = this.ConfinePoint(a - b2 + b);
				}
				if (vector2.AlmostZero())
				{
					vector2 = this.ConfinePoint(a + b2 - b);
				}
				if (vector2.AlmostZero())
				{
					break;
				}
				if ((vector2 + b3).AlmostZero())
				{
					vector += vector2 * 0.5f;
					break;
				}
				vector += vector2;
				a += vector2;
				b3 = vector2;
			}
			return vector;
		}

		internal Type UpgradeToCm3_GetTargetType()
		{
			if (this.m_ConfineMode != CinemachineConfiner.Mode.Confine3D)
			{
				return typeof(CinemachineConfiner2D);
			}
			return typeof(CinemachineConfiner3D);
		}

		internal void UpgradeToCm3(CinemachineConfiner3D c)
		{
			c.BoundingVolume = this.m_BoundingVolume;
		}

		internal void UpgradeToCm3(CinemachineConfiner2D c)
		{
			c.BoundingShape2D = this.m_BoundingShape2D;
			c.Damping = this.m_Damping;
		}

		[Tooltip("The confiner can operate using a 2D bounding shape or a 3D bounding volume")]
		public CinemachineConfiner.Mode m_ConfineMode;

		[Tooltip("The volume within which the camera is to be contained")]
		public Collider m_BoundingVolume;

		[Tooltip("The 2D shape within which the camera is to be contained")]
		public Collider2D m_BoundingShape2D;

		private Collider2D m_BoundingShape2DCache;

		[Tooltip("If camera is orthographic, screen edges will be confined to the volume.  If not checked, then only the camera center will be confined")]
		public bool m_ConfineScreenEdges = true;

		[Tooltip("How gradually to return the camera to the bounding volume if it goes beyond the borders.  Higher numbers are more gradual.")]
		[Range(0f, 10f)]
		public float m_Damping;

		private List<List<Vector2>> m_PathCache;

		private int m_PathTotalPointCount;

		public enum Mode
		{
			Confine2D,
			Confine3D
		}

		private class VcamExtraState : CinemachineExtension.VcamExtraStateBase
		{
			public Vector3 PreviousDisplacement;

			public float ConfinerDisplacement;
		}
	}
}
