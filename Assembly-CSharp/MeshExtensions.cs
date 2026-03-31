using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public static class MeshExtensions
{
	public static void SplitByAngle(this Mesh mesh, float angleDeg)
	{
		float num = Mathf.Cos(angleDeg * 0.017453292f);
		Vector3[] vertices = mesh.vertices;
		int[] triangles = mesh.triangles;
		int num2 = triangles.Length / 3;
		Vector3[] array = new Vector3[num2];
		for (int i = 0; i < num2; i++)
		{
			Vector3 b = vertices[triangles[i * 3]];
			Vector3 a = vertices[triangles[i * 3 + 1]];
			Vector3 a2 = vertices[triangles[i * 3 + 2]];
			array[i] = Vector3.Cross(a - b, a2 - b).normalized;
		}
		List<ValueTuple<int, Vector3>>[] array2 = new List<ValueTuple<int, Vector3>>[vertices.Length];
		for (int j = 0; j < array2.Length; j++)
		{
			array2[j] = new List<ValueTuple<int, Vector3>>();
		}
		List<Vector3> list = new List<Vector3>(vertices.Length);
		int[] array3 = new int[triangles.Length];
		for (int k = 0; k < num2; k++)
		{
			for (int l = 0; l < 3; l++)
			{
				int num3 = triangles[k * 3 + l];
				Vector3 vector = array[k];
				int num4 = -1;
				foreach (ValueTuple<int, Vector3> valueTuple in array2[num3])
				{
					int item = valueTuple.Item1;
					if (Vector3.Dot(valueTuple.Item2, vector) >= num)
					{
						num4 = item;
						break;
					}
				}
				if (num4 < 0)
				{
					num4 = list.Count;
					list.Add(vertices[num3]);
					array2[num3].Add(new ValueTuple<int, Vector3>(num4, vector));
				}
				array3[k * 3 + l] = num4;
			}
		}
		mesh.Clear();
		mesh.SetVertices(list);
		mesh.triangles = array3;
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
	}

	public static void SplitByAngleBurst(this Mesh mesh, float angleDeg, bool areaWeight = true, Allocator allocator = Allocator.TempJob)
	{
		NativeArray<float3> nativeArray = new NativeArray<float3>(mesh.vertexCount, allocator, NativeArrayOptions.ClearMemory);
		List<Vector3> list = new List<Vector3>(mesh.vertexCount);
		mesh.GetVertices(list);
		for (int i = 0; i < list.Count; i++)
		{
			nativeArray[i] = list[i];
		}
		NativeArray<int> nativeArray2 = new NativeArray<int>(mesh.triangles, allocator);
		NativeArray<float3> faceN = new NativeArray<float3>(nativeArray2.Length / 3, allocator, NativeArrayOptions.ClearMemory);
		new MeshExtensions.FaceNormalJob
		{
			Verts = nativeArray,
			Tris = nativeArray2,
			FaceN = faceN
		}.Schedule(faceN.Length, 64, default(JobHandle)).Complete();
		NativeList<float3> nativeList = new NativeList<float3>(nativeArray.Length, allocator);
		NativeList<int> nativeList2 = new NativeList<int>(nativeArray2.Length, allocator);
		new MeshExtensions.SplitJob
		{
			CosThresh = math.cos(math.radians(angleDeg)),
			SrcVerts = nativeArray,
			SrcTris = nativeArray2,
			FaceN = faceN,
			DstVerts = nativeList,
			DstTris = nativeList2
		}.Run<MeshExtensions.SplitJob>();
		NativeArray<float3> nativeArray3 = new NativeArray<float3>(nativeList.Length, allocator, NativeArrayOptions.ClearMemory);
		MeshExtensions.RecalcNormalsJobified(nativeList, nativeList2, areaWeight, allocator, ref nativeArray3);
		mesh.Clear();
		List<Vector3> list2 = new List<Vector3>(nativeList.Length);
		for (int j = 0; j < nativeList.Length; j++)
		{
			list2.Add(nativeList[j]);
		}
		mesh.SetVertices(list2);
		mesh.triangles = nativeList2.AsArray().ToArray();
		List<Vector3> list3 = new List<Vector3>(nativeArray3.Length);
		for (int k = 0; k < nativeArray3.Length; k++)
		{
			list3.Add(nativeArray3[k]);
		}
		mesh.SetNormals(list3);
		mesh.RecalculateBounds();
		nativeArray.Dispose();
		nativeArray2.Dispose();
		faceN.Dispose();
		nativeList.Dispose();
		nativeList2.Dispose();
		nativeArray3.Dispose();
	}

	private static void RecalcNormalsJobified(NativeList<float3> verts, NativeList<int> tris, bool areaWeight, Allocator alloc, ref NativeArray<float3> outNormals)
	{
		int length = verts.Length;
		int num = tris.Length / 3;
		NativeArray<float3> nativeArray = new NativeArray<float3>(num, alloc, NativeArrayOptions.ClearMemory);
		new MeshExtensions.TriNormalJob
		{
			V = verts.AsArray(),
			T = tris.AsArray(),
			Out = nativeArray,
			AreaWeight = (areaWeight ? 1 : 0)
		}.Schedule(num, 64, default(JobHandle)).Complete();
		NativeParallelMultiHashMap<int, int> v2T = new NativeParallelMultiHashMap<int, int>(tris.Length, alloc);
		new MeshExtensions.BuildAdjJob
		{
			T = tris.AsArray(),
			MapW = v2T.AsParallelWriter()
		}.Schedule(num, 64, default(JobHandle)).Complete();
		new MeshExtensions.VertexNormalJob
		{
			AreaWeight = (areaWeight ? 1 : 0),
			TriN = nativeArray,
			V2T = v2T,
			Out = outNormals
		}.Schedule(length, 64, default(JobHandle)).Complete();
		nativeArray.Dispose();
		v2T.Dispose();
	}

	[BurstCompile]
	private struct FaceNormalJob : IJobParallelFor
	{
		public void Execute(int index)
		{
			int num = index * 3;
			float3 rhs = this.Verts[this.Tris[num]];
			float3 lhs = this.Verts[this.Tris[num + 1]];
			float3 lhs2 = this.Verts[this.Tris[num + 2]];
			this.FaceN[index] = math.normalize(math.cross(lhs - rhs, lhs2 - rhs));
		}

		[ReadOnly]
		public NativeArray<float3> Verts;

		[ReadOnly]
		public NativeArray<int> Tris;

		[WriteOnly]
		public NativeArray<float3> FaceN;
	}

	[BurstCompile]
	private struct SplitJob : IJob
	{
		public void Execute()
		{
			int length = this.SrcVerts.Length;
			int num = this.SrcTris.Length / 3;
			NativeArray<int> nativeArray = new NativeArray<int>(length, Allocator.Temp, NativeArrayOptions.ClearMemory);
			for (int i = 0; i < length; i++)
			{
				nativeArray[i] = -1;
			}
			NativeList<MeshExtensions.SplitJob.Bucket> nativeList = new NativeList<MeshExtensions.SplitJob.Bucket>(length, Allocator.Temp);
			this.DstVerts.Clear();
			this.DstTris.ResizeUninitialized(this.SrcTris.Length);
			for (int j = 0; j < num; j++)
			{
				float3 @float = this.FaceN[j];
				for (int k = 0; k < 3; k++)
				{
					int index = this.SrcTris[j * 3 + k];
					int num2 = -1;
					for (int num3 = nativeArray[index]; num3 != -1; num3 = nativeList[num3].next)
					{
						MeshExtensions.SplitJob.Bucket bucket = nativeList[num3];
						if (math.dot(bucket.repN, @float) >= this.CosThresh)
						{
							num2 = bucket.newIdx;
							break;
						}
					}
					if (num2 == -1)
					{
						num2 = this.DstVerts.Length;
						float3 float2 = this.SrcVerts[index];
						this.DstVerts.Add(float2);
						MeshExtensions.SplitJob.Bucket bucket2 = new MeshExtensions.SplitJob.Bucket
						{
							next = nativeArray[index],
							newIdx = num2,
							repN = @float
						};
						nativeArray[index] = nativeList.Length;
						nativeList.Add(bucket2);
					}
					this.DstTris[j * 3 + k] = num2;
				}
			}
			nativeArray.Dispose();
			nativeList.Dispose();
		}

		public float CosThresh;

		[ReadOnly]
		public NativeArray<float3> SrcVerts;

		[ReadOnly]
		public NativeArray<int> SrcTris;

		[ReadOnly]
		public NativeArray<float3> FaceN;

		public NativeList<float3> DstVerts;

		public NativeList<int> DstTris;

		private struct Bucket
		{
			public int next;

			public int newIdx;

			public float3 repN;
		}
	}

	[BurstCompile]
	private struct TriNormalJob : IJobParallelFor
	{
		public void Execute(int i)
		{
			int num = i * 3;
			float3 rhs = this.V[this.T[num]];
			float3 lhs = this.V[this.T[num + 1]];
			float3 lhs2 = this.V[this.T[num + 2]];
			float3 @float = math.cross(lhs - rhs, lhs2 - rhs);
			this.Out[i] = ((this.AreaWeight == 0) ? math.normalize(@float) : @float);
		}

		[ReadOnly]
		public NativeArray<float3> V;

		[ReadOnly]
		public NativeArray<int> T;

		[WriteOnly]
		public NativeArray<float3> Out;

		public int AreaWeight;
	}

	[BurstCompile]
	private struct BuildAdjJob : IJobParallelFor
	{
		public void Execute(int triIdx)
		{
			int num = triIdx * 3;
			this.MapW.Add(this.T[num], triIdx);
			this.MapW.Add(this.T[num + 1], triIdx);
			this.MapW.Add(this.T[num + 2], triIdx);
		}

		[ReadOnly]
		public NativeArray<int> T;

		public NativeParallelMultiHashMap<int, int>.ParallelWriter MapW;
	}

	[BurstCompile]
	private struct VertexNormalJob : IJobParallelFor
	{
		public void Execute(int v)
		{
			NativeParallelMultiHashMap<int, int>.Enumerator valuesForKey = this.V2T.GetValuesForKey(v);
			if (!valuesForKey.MoveNext())
			{
				this.Out[v] = float3.zero;
				return;
			}
			int index = valuesForKey.Current;
			float3 @float = this.TriN[index];
			float3 float2 = (this.AreaWeight == 0) ? @float : math.normalize(@float);
			float3 float3 = float3.zero;
			NativeParallelMultiHashMap<int, int>.Enumerator valuesForKey2 = this.V2T.GetValuesForKey(v);
			while (valuesForKey2.MoveNext())
			{
				int index2 = valuesForKey2.Current;
				float3 float4 = this.TriN[index2];
				if (this.AreaWeight != 0)
				{
					math.normalize(float4);
				}
				float3 += float4;
			}
			this.Out[v] = ((math.lengthsq(float3) < 1E-09f) ? float2 : math.normalize(float3));
		}

		public int AreaWeight;

		[ReadOnly]
		public NativeArray<float3> TriN;

		[ReadOnly]
		public NativeParallelMultiHashMap<int, int> V2T;

		[WriteOnly]
		public NativeArray<float3> Out;
	}
}
