using System;
using UnityEngine;

namespace Unity.Cinemachine
{
	[Obsolete("CinemachinePathBase has been deprecated. Use SplineContainer instead")]
	[AddComponentMenu("")]
	[SaveDuringPlay]
	[DisallowMultipleComponent]
	public class CinemachineSmoothPath : CinemachinePathBase
	{
		public override float MinPos
		{
			get
			{
				return 0f;
			}
		}

		public override float MaxPos
		{
			get
			{
				int num = this.m_Waypoints.Length - 1;
				if (num < 1)
				{
					return 0f;
				}
				return (float)(this.m_Looped ? (num + 1) : num);
			}
		}

		public override bool Looped
		{
			get
			{
				return this.m_Looped;
			}
		}

		public override int DistanceCacheSampleStepsPerSegment
		{
			get
			{
				return this.m_Resolution;
			}
		}

		private void OnValidate()
		{
			this.InvalidateDistanceCache();
		}

		private void Reset()
		{
			this.m_Looped = false;
			this.m_Waypoints = new CinemachineSmoothPath.Waypoint[]
			{
				new CinemachineSmoothPath.Waypoint
				{
					position = new Vector3(0f, 0f, -5f)
				},
				new CinemachineSmoothPath.Waypoint
				{
					position = new Vector3(0f, 0f, 5f)
				}
			};
			this.m_Appearance = new CinemachinePathBase.Appearance();
			this.InvalidateDistanceCache();
		}

		public override void InvalidateDistanceCache()
		{
			base.InvalidateDistanceCache();
			this.m_ControlPoints1 = null;
			this.m_ControlPoints2 = null;
		}

		internal void UpdateControlPoints()
		{
			int num = (this.m_Waypoints == null) ? 0 : this.m_Waypoints.Length;
			if (num > 1 && (this.Looped != this.m_IsLoopedCache || this.m_ControlPoints1 == null || this.m_ControlPoints1.Length != num || this.m_ControlPoints2 == null || this.m_ControlPoints2.Length != num))
			{
				Vector4[] array = new Vector4[num];
				Vector4[] array2 = new Vector4[num];
				Vector4[] array3 = new Vector4[num];
				for (int i = 0; i < num; i++)
				{
					array3[i] = this.m_Waypoints[i].AsVector4;
				}
				if (this.Looped)
				{
					SplineHelpers.ComputeSmoothControlPointsLooped(ref array3, ref array, ref array2);
				}
				else
				{
					SplineHelpers.ComputeSmoothControlPoints(ref array3, ref array, ref array2);
				}
				this.m_ControlPoints1 = new CinemachineSmoothPath.Waypoint[num];
				this.m_ControlPoints2 = new CinemachineSmoothPath.Waypoint[num];
				for (int j = 0; j < num; j++)
				{
					this.m_ControlPoints1[j] = CinemachineSmoothPath.Waypoint.FromVector4(array[j]);
					this.m_ControlPoints2[j] = CinemachineSmoothPath.Waypoint.FromVector4(array2[j]);
				}
				this.m_IsLoopedCache = this.Looped;
			}
		}

		private float GetBoundingIndices(float pos, out int indexA, out int indexB)
		{
			pos = this.StandardizePos(pos);
			int num = this.m_Waypoints.Length;
			if (num < 2)
			{
				indexA = (indexB = 0);
			}
			else
			{
				indexA = Mathf.FloorToInt(pos);
				if (indexA >= num)
				{
					pos -= this.MaxPos;
					indexA = 0;
				}
				indexB = indexA + 1;
				if (indexB == num)
				{
					if (this.Looped)
					{
						indexB = 0;
					}
					else
					{
						indexB--;
						indexA--;
					}
				}
			}
			return pos;
		}

		public override Vector3 EvaluateLocalPosition(float pos)
		{
			Vector3 result = Vector3.zero;
			if (this.m_Waypoints.Length != 0)
			{
				this.UpdateControlPoints();
				int num;
				int num2;
				pos = this.GetBoundingIndices(pos, out num, out num2);
				if (num == num2)
				{
					result = this.m_Waypoints[num].position;
				}
				else
				{
					result = SplineHelpers.Bezier3(pos - (float)num, this.m_Waypoints[num].position, this.m_ControlPoints1[num].position, this.m_ControlPoints2[num].position, this.m_Waypoints[num2].position);
				}
			}
			return result;
		}

		public override Vector3 EvaluateLocalTangent(float pos)
		{
			Vector3 result = Vector3.forward;
			if (this.m_Waypoints.Length > 1)
			{
				this.UpdateControlPoints();
				int num;
				int num2;
				pos = this.GetBoundingIndices(pos, out num, out num2);
				if (!this.Looped && num == this.m_Waypoints.Length - 1)
				{
					num--;
				}
				result = SplineHelpers.BezierTangent3(pos - (float)num, this.m_Waypoints[num].position, this.m_ControlPoints1[num].position, this.m_ControlPoints2[num].position, this.m_Waypoints[num2].position);
			}
			return result;
		}

		public override Quaternion EvaluateLocalOrientation(float pos)
		{
			Quaternion result = Quaternion.identity;
			if (this.m_Waypoints.Length != 0)
			{
				int num;
				int num2;
				pos = this.GetBoundingIndices(pos, out num, out num2);
				float angle;
				if (num == num2)
				{
					angle = this.m_Waypoints[num].roll;
				}
				else
				{
					this.UpdateControlPoints();
					angle = SplineHelpers.Bezier1(pos - (float)num, this.m_Waypoints[num].roll, this.m_ControlPoints1[num].roll, this.m_ControlPoints2[num].roll, this.m_Waypoints[num2].roll);
				}
				Vector3 vector = this.EvaluateLocalTangent(pos);
				if (!vector.AlmostZero())
				{
					result = Quaternion.LookRotation(vector) * CinemachineSmoothPath.RollAroundForward(angle);
				}
			}
			return result;
		}

		private static Quaternion RollAroundForward(float angle)
		{
			float f = angle * 0.5f * 0.017453292f;
			return new Quaternion(0f, 0f, Mathf.Sin(f), Mathf.Cos(f));
		}

		[Tooltip("If checked, then the path ends are joined to form a continuous loop.")]
		public bool m_Looped;

		[Tooltip("The waypoints that define the path.  They will be interpolated using a bezier curve.")]
		public CinemachineSmoothPath.Waypoint[] m_Waypoints = new CinemachineSmoothPath.Waypoint[0];

		internal CinemachineSmoothPath.Waypoint[] m_ControlPoints1;

		internal CinemachineSmoothPath.Waypoint[] m_ControlPoints2;

		private bool m_IsLoopedCache;

		[Serializable]
		public struct Waypoint
		{
			internal Vector4 AsVector4
			{
				get
				{
					return new Vector4(this.position.x, this.position.y, this.position.z, this.roll);
				}
			}

			internal static CinemachineSmoothPath.Waypoint FromVector4(Vector4 v)
			{
				return new CinemachineSmoothPath.Waypoint
				{
					position = new Vector3(v[0], v[1], v[2]),
					roll = v[3]
				};
			}

			[Tooltip("Position in path-local space")]
			public Vector3 position;

			[Tooltip("Defines the roll of the path at this waypoint.  The other orientation axes are inferred from the tangent and world up.")]
			public float roll;
		}
	}
}
