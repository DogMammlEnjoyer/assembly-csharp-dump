using System;
using Unity.Mathematics;

namespace UnityEngine.Rendering.RadeonRays
{
	internal class AABB
	{
		public AABB()
		{
			this.Min = new float3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
			this.Max = new float3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
		}

		public AABB(float3 min, float3 max)
		{
			this.Min = min;
			this.Max = max;
		}

		public void Encapsulate(AABB aabb)
		{
			this.Min = math.min(this.Min, aabb.Min);
			this.Max = math.max(this.Max, aabb.Max);
		}

		public void Encapsulate(float3 point)
		{
			this.Min = math.min(this.Min, point);
			this.Max = math.max(this.Max, point);
		}

		public bool Contains(AABB rhs)
		{
			return rhs.Min.x >= this.Min.x && rhs.Min.y >= this.Min.y && rhs.Min.z >= this.Min.z && rhs.Max.x <= this.Max.x && rhs.Max.y <= this.Max.y && rhs.Max.z <= this.Max.z;
		}

		public bool IsValid()
		{
			return this.Min.x <= this.Max.x && this.Min.y <= this.Max.y && this.Min.z <= this.Max.z;
		}

		public float3 Min;

		public float3 Max;
	}
}
