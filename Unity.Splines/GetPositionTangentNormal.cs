using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace UnityEngine.Splines
{
	public struct GetPositionTangentNormal : IJobParallelFor
	{
		public void Execute(int index)
		{
			float3 value;
			float3 value2;
			float3 value3;
			this.Spline.Evaluate((float)index / ((float)this.Positions.Length - 1f), out value, out value2, out value3);
			this.Positions[index] = value;
			this.Tangents[index] = value2;
			this.Normals[index] = value3;
		}

		[ReadOnly]
		public NativeSpline Spline;

		[WriteOnly]
		public NativeArray<float3> Positions;

		[WriteOnly]
		public NativeArray<float3> Tangents;

		[WriteOnly]
		public NativeArray<float3> Normals;
	}
}
