using System;
using System.Collections.Generic;
using Unity.Mathematics;

namespace FastSurfaceNets
{
	public class SurfaceNetsBuffer
	{
		internal void Reset(int arraySize)
		{
			this.Positions.Clear();
			this.Normals.Clear();
			this.Indices.Clear();
			this.SurfacePoints.Clear();
			this.SurfaceStrides.Clear();
			if (this.StrideToIndex.Length < arraySize)
			{
				Array.Resize<int>(ref this.StrideToIndex, arraySize);
			}
			for (int i = 0; i < arraySize; i++)
			{
				this.StrideToIndex[i] = int.MaxValue;
			}
		}

		public readonly List<float3> Positions = new List<float3>();

		public readonly List<float3> Normals = new List<float3>();

		public readonly List<int> Indices = new List<int>();

		internal readonly List<int3> SurfacePoints = new List<int3>();

		internal readonly List<int> SurfaceStrides = new List<int>();

		internal int[] StrideToIndex = Array.Empty<int>();

		public const int NullVertex = 2147483647;
	}
}
