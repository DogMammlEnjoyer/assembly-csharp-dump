using System;
using Unity.Mathematics;

namespace UnityEngine.Splines
{
	public struct BezierCurve : IEquatable<BezierCurve>
	{
		public float3 Tangent0
		{
			get
			{
				return this.P1 - this.P0;
			}
			set
			{
				this.P1 = this.P0 + value;
			}
		}

		public float3 Tangent1
		{
			get
			{
				return this.P2 - this.P3;
			}
			set
			{
				this.P2 = this.P3 + value;
			}
		}

		public BezierCurve(float3 p0, float3 p1)
		{
			this.P2 = p0;
			this.P0 = p0;
			this.P3 = p1;
			this.P1 = p1;
		}

		public BezierCurve(float3 p0, float3 p1, float3 p2)
		{
			float3 rhs = 0.6666667f * p1;
			this.P0 = p0;
			this.P1 = 0.33333334f * p0 + rhs;
			this.P2 = 0.33333334f * p2 + rhs;
			this.P3 = p2;
		}

		public BezierCurve(float3 p0, float3 p1, float3 p2, float3 p3)
		{
			this.P0 = p0;
			this.P1 = p1;
			this.P2 = p2;
			this.P3 = p3;
		}

		public BezierCurve(BezierKnot a, BezierKnot b)
		{
			this = new BezierCurve(a.Position, a.Position + math.rotate(a.Rotation, a.TangentOut), b.Position + math.rotate(b.Rotation, b.TangentIn), b.Position);
		}

		public BezierCurve Transform(float4x4 matrix)
		{
			return new BezierCurve(math.transform(matrix, this.P0), math.transform(matrix, this.P1), math.transform(matrix, this.P2), math.transform(matrix, this.P3));
		}

		public static BezierCurve FromTangent(float3 pointA, float3 tangentOutA, float3 pointB, float3 tangentInB)
		{
			return new BezierCurve(pointA, pointA + tangentOutA, pointB + tangentInB, pointB);
		}

		public BezierCurve GetInvertedCurve()
		{
			return new BezierCurve(this.P3, this.P2, this.P1, this.P0);
		}

		public bool Equals(BezierCurve other)
		{
			return this.P0.Equals(other.P0) && this.P1.Equals(other.P1) && this.P2.Equals(other.P2) && this.P3.Equals(other.P3);
		}

		public override bool Equals(object obj)
		{
			if (obj is BezierCurve)
			{
				BezierCurve other = (BezierCurve)obj;
				return this.Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return ((this.P0.GetHashCode() * 397 ^ this.P1.GetHashCode()) * 397 ^ this.P2.GetHashCode()) * 397 ^ this.P3.GetHashCode();
		}

		public static bool operator ==(BezierCurve left, BezierCurve right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(BezierCurve left, BezierCurve right)
		{
			return !left.Equals(right);
		}

		public float3 P0;

		public float3 P1;

		public float3 P2;

		public float3 P3;
	}
}
