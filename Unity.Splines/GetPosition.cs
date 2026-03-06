using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace UnityEngine.Splines
{
	[BurstCompile]
	public struct GetPosition : IJobParallelFor
	{
		public void Execute(int index)
		{
			this.Positions[index] = this.Spline.EvaluatePosition((float)index / ((float)this.Positions.Length - 1f));
		}

		[ReadOnly]
		public NativeSpline Spline;

		[WriteOnly]
		public NativeArray<float3> Positions;
	}
}
