using System;
using UnityEngine;

namespace Unity.Cinemachine
{
	[Obsolete("CinemachinePath has been deprecated. Use SplineContainer instead")]
	[AddComponentMenu("")]
	[SaveDuringPlay]
	[DisallowMultipleComponent]
	public class CinemachinePath : CinemachinePathBase
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

		private void Reset()
		{
			this.m_Looped = false;
			this.m_Waypoints = new CinemachinePath.Waypoint[]
			{
				new CinemachinePath.Waypoint
				{
					position = new Vector3(0f, 0f, -5f),
					tangent = new Vector3(1f, 0f, 0f)
				},
				new CinemachinePath.Waypoint
				{
					position = new Vector3(0f, 0f, 5f),
					tangent = new Vector3(1f, 0f, 0f)
				}
			};
			this.m_Appearance = new CinemachinePathBase.Appearance();
			this.InvalidateDistanceCache();
		}

		private void OnValidate()
		{
			this.InvalidateDistanceCache();
		}

		public override int DistanceCacheSampleStepsPerSegment
		{
			get
			{
				return this.m_Resolution;
			}
		}

		private float GetBoundingIndices(float pos, out int indexA, out int indexB)
		{
			pos = this.StandardizePos(pos);
			int num = Mathf.RoundToInt(pos);
			if (Mathf.Abs(pos - (float)num) < 0.0001f)
			{
				indexA = (indexB = ((num == this.m_Waypoints.Length) ? 0 : num));
			}
			else
			{
				indexA = Mathf.FloorToInt(pos);
				if (indexA >= this.m_Waypoints.Length)
				{
					pos -= this.MaxPos;
					indexA = 0;
				}
				indexB = Mathf.CeilToInt(pos);
				if (indexB >= this.m_Waypoints.Length)
				{
					indexB = 0;
				}
			}
			return pos;
		}

		public override Vector3 EvaluateLocalPosition(float pos)
		{
			Vector3 result = Vector3.zero;
			if (this.m_Waypoints.Length != 0)
			{
				int num;
				int num2;
				pos = this.GetBoundingIndices(pos, out num, out num2);
				if (num == num2)
				{
					result = this.m_Waypoints[num].position;
				}
				else
				{
					CinemachinePath.Waypoint waypoint = this.m_Waypoints[num];
					CinemachinePath.Waypoint waypoint2 = this.m_Waypoints[num2];
					result = SplineHelpers.Bezier3(pos - (float)num, this.m_Waypoints[num].position, waypoint.position + waypoint.tangent, waypoint2.position - waypoint2.tangent, waypoint2.position);
				}
			}
			return result;
		}

		public override Vector3 EvaluateLocalTangent(float pos)
		{
			Vector3 result = Vector3.forward;
			if (this.m_Waypoints.Length != 0)
			{
				int num;
				int num2;
				pos = this.GetBoundingIndices(pos, out num, out num2);
				if (num == num2)
				{
					result = this.m_Waypoints[num].tangent;
				}
				else
				{
					CinemachinePath.Waypoint waypoint = this.m_Waypoints[num];
					CinemachinePath.Waypoint waypoint2 = this.m_Waypoints[num2];
					result = SplineHelpers.BezierTangent3(pos - (float)num, this.m_Waypoints[num].position, waypoint.position + waypoint.tangent, waypoint2.position - waypoint2.tangent, waypoint2.position);
				}
			}
			return result;
		}

		public override Quaternion EvaluateLocalOrientation(float pos)
		{
			Quaternion result = Quaternion.identity;
			if (this.m_Waypoints.Length != 0)
			{
				int indexA;
				int indexB;
				pos = this.GetBoundingIndices(pos, out indexA, out indexB);
				Vector3 vector = this.EvaluateLocalTangent(pos);
				if (!vector.AlmostZero())
				{
					result = Quaternion.LookRotation(vector) * CinemachinePath.RollAroundForward(this.GetRoll(indexA, indexB, pos));
				}
			}
			return result;
		}

		internal float GetRoll(int indexA, int indexB, float standardizedPos)
		{
			if (indexA == indexB)
			{
				return this.m_Waypoints[indexA].roll;
			}
			float num = this.m_Waypoints[indexA].roll;
			float num2 = this.m_Waypoints[indexB].roll;
			if (indexB == 0)
			{
				num %= 360f;
				num2 %= 360f;
			}
			return Mathf.Lerp(num, num2, standardizedPos - (float)indexA);
		}

		private static Quaternion RollAroundForward(float angle)
		{
			float f = angle * 0.5f * 0.017453292f;
			return new Quaternion(0f, 0f, Mathf.Sin(f), Mathf.Cos(f));
		}

		[Tooltip("If checked, then the path ends are joined to form a continuous loop.")]
		public bool m_Looped;

		[Tooltip("The waypoints that define the path.  They will be interpolated using a bezier curve.")]
		public CinemachinePath.Waypoint[] m_Waypoints = new CinemachinePath.Waypoint[0];

		[Serializable]
		public struct Waypoint
		{
			[Tooltip("Position in path-local space")]
			public Vector3 position;

			[Tooltip("Offset from the position, which defines the tangent of the curve at the waypoint.  The length of the tangent encodes the strength of the bezier handle.  The same handle is used symmetrically on both sides of the waypoint, to ensure smoothness.")]
			public Vector3 tangent;

			[Tooltip("Defines the roll of the path at this waypoint.  The other orientation axes are inferred from the tangent and world up.")]
			public float roll;
		}
	}
}
