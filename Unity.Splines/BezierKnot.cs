using System;
using Unity.Mathematics;

namespace UnityEngine.Splines
{
	[Serializable]
	public struct BezierKnot : ISerializationCallbackReceiver, IEquatable<BezierKnot>
	{
		public BezierKnot(float3 position)
		{
			this = new BezierKnot(position, 0f, 0f, quaternion.identity);
		}

		public BezierKnot(float3 position, float3 tangentIn, float3 tangentOut)
		{
			this = new BezierKnot(position, tangentIn, tangentOut, quaternion.identity);
		}

		public BezierKnot(float3 position, float3 tangentIn, float3 tangentOut, quaternion rotation)
		{
			this.Position = position;
			this.TangentIn = tangentIn;
			this.TangentOut = tangentOut;
			this.Rotation = rotation;
		}

		public BezierKnot Transform(float4x4 matrix)
		{
			quaternion quaternion = math.mul(new quaternion(matrix), this.Rotation);
			quaternion q = math.inverse(quaternion);
			return new BezierKnot(math.transform(matrix, this.Position), math.rotate(q, math.rotate(matrix, math.rotate(this.Rotation, this.TangentIn))), math.rotate(q, math.rotate(matrix, math.rotate(this.Rotation, this.TangentOut))), quaternion);
		}

		public static BezierKnot operator +(BezierKnot knot, float3 rhs)
		{
			return new BezierKnot(knot.Position + rhs, knot.TangentIn, knot.TangentOut, knot.Rotation);
		}

		public static BezierKnot operator -(BezierKnot knot, float3 rhs)
		{
			return new BezierKnot(knot.Position - rhs, knot.TangentIn, knot.TangentOut, knot.Rotation);
		}

		internal BezierKnot BakeTangentDirectionToRotation(bool mirrored, BezierTangent main = BezierTangent.Out)
		{
			if (mirrored)
			{
				float num = math.length((main == BezierTangent.In) ? this.TangentIn : this.TangentOut);
				return new BezierKnot(this.Position, new float3(0f, 0f, -num), new float3(0f, 0f, num), SplineUtility.GetKnotRotation(math.mul(this.Rotation, (main == BezierTangent.In) ? (-this.TangentIn) : this.TangentOut), math.mul(this.Rotation, math.up())));
			}
			return new BezierKnot(this.Position, new float3(0f, 0f, -math.length(this.TangentIn)), new float3(0f, 0f, math.length(this.TangentOut)), this.Rotation = SplineUtility.GetKnotRotation(math.mul(this.Rotation, (main == BezierTangent.In) ? (-this.TangentIn) : this.TangentOut), math.mul(this.Rotation, math.up())));
		}

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			if (math.lengthsq(this.Rotation) == 0f)
			{
				this.Rotation = quaternion.identity;
			}
		}

		public override string ToString()
		{
			return string.Format("{{{0}, {1}, {2}, {3}}}", new object[]
			{
				this.Position,
				this.TangentIn,
				this.TangentOut,
				this.Rotation
			});
		}

		public bool Equals(BezierKnot other)
		{
			return this.Position.Equals(other.Position) && this.TangentIn.Equals(other.TangentIn) && this.TangentOut.Equals(other.TangentOut) && this.Rotation.Equals(other.Rotation);
		}

		public override bool Equals(object obj)
		{
			if (obj is BezierKnot)
			{
				BezierKnot other = (BezierKnot)obj;
				return this.Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine<float3, float3, float3, quaternion>(this.Position, this.TangentIn, this.TangentOut, this.Rotation);
		}

		public float3 Position;

		public float3 TangentIn;

		public float3 TangentOut;

		public quaternion Rotation;
	}
}
