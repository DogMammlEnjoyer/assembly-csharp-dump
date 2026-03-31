using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Voxels
{
	public static class MeshUtilities
	{
		public static void SplitByAngle(this Mesh mesh, float angleDeg, bool areaWeight = true, Allocator allocator = Allocator.TempJob)
		{
			NativeArray<float3> srcVerts = new NativeArray<float3>(mesh.vertexCount, allocator, NativeArrayOptions.ClearMemory);
			List<Vector3> list = new List<Vector3>(mesh.vertexCount);
			mesh.GetVertices(list);
			for (int i = 0; i < list.Count; i++)
			{
				srcVerts[i] = list[i];
			}
			new NativeArray<byte>(mesh.vertexCount, allocator, NativeArrayOptions.ClearMemory);
			NativeArray<int> srcTris = new NativeArray<int>(mesh.triangles, allocator);
			MeshUtilities.MeshData meshData = MeshUtilities.SplitByAngle(srcVerts, srcTris, angleDeg, areaWeight, allocator);
			mesh.Clear();
			mesh.SetVertices<float3>(meshData.Vertices.AsArray());
			mesh.SetTriangles(meshData.Triangles.AsArray().ToArray(), 0, false);
			mesh.SetNormals<float3>(meshData.Normals.AsArray());
			mesh.RecalculateBounds();
			srcVerts.Dispose();
			srcTris.Dispose();
			meshData.Dispose();
		}

		public static MeshUtilities.MeshData SplitByAngle(NativeArray<float3> srcVerts, NativeArray<int> srcTris, float angleDeg, bool areaWeight = true, Allocator allocator = Allocator.TempJob)
		{
			NativeArray<float3> faceN = new NativeArray<float3>(srcTris.Length / 3, allocator, NativeArrayOptions.ClearMemory);
			new MeshUtilities.FaceNormalJob
			{
				Verts = srcVerts,
				Tris = srcTris,
				FaceN = faceN
			}.Schedule(faceN.Length, 64, default(JobHandle)).Complete();
			NativeList<float3> nativeList = new NativeList<float3>(srcVerts.Length, allocator);
			NativeList<int> nativeList2 = new NativeList<int>(srcTris.Length, allocator);
			new MeshUtilities.SplitJob
			{
				CosThresh = math.cos(math.radians(angleDeg)),
				SrcVerts = srcVerts,
				SrcTris = srcTris,
				FaceN = faceN,
				DstVerts = nativeList,
				DstTris = nativeList2
			}.Run<MeshUtilities.SplitJob>();
			NativeList<float3> normals = new NativeList<float3>(nativeList.Length, allocator);
			normals.ResizeUninitialized(nativeList.Length);
			MeshUtilities.RecalcNormalsJobified(nativeList, nativeList2, areaWeight, allocator, ref normals);
			faceN.Dispose();
			return new MeshUtilities.MeshData
			{
				Vertices = nativeList,
				Triangles = nativeList2,
				Normals = normals
			};
		}

		public static MeshUtilities.VoxelMeshData SplitByAngle(NativeArray<float3> srcVerts, NativeArray<byte> srcMats, NativeArray<int> srcTris, float angleDeg, bool areaWeight = true, Allocator allocator = Allocator.TempJob)
		{
			NativeArray<float3> faceN = new NativeArray<float3>(srcTris.Length / 3, allocator, NativeArrayOptions.ClearMemory);
			new MeshUtilities.FaceNormalJob
			{
				Verts = srcVerts,
				Tris = srcTris,
				FaceN = faceN
			}.Schedule(faceN.Length, 64, default(JobHandle)).Complete();
			NativeList<float3> nativeList = new NativeList<float3>(srcVerts.Length, allocator);
			NativeList<byte> nativeList2 = new NativeList<byte>(srcVerts.Length, allocator);
			NativeList<int> nativeList3 = new NativeList<int>(srcTris.Length, allocator);
			new MeshUtilities.SplitVoxelMeshJob
			{
				CosThresh = math.cos(math.radians(angleDeg)),
				SrcVerts = srcVerts,
				SrcMats = srcMats,
				SrcTris = srcTris,
				FaceN = faceN,
				DstVerts = nativeList,
				DstMats = nativeList2,
				DstTris = nativeList3
			}.Run<MeshUtilities.SplitVoxelMeshJob>();
			NativeList<float3> normals = new NativeList<float3>(nativeList.Length, allocator);
			normals.ResizeUninitialized(nativeList.Length);
			MeshUtilities.RecalcNormalsJobified(nativeList, nativeList3, areaWeight, allocator, ref normals);
			faceN.Dispose();
			return new MeshUtilities.VoxelMeshData
			{
				Vertices = nativeList,
				Materials = nativeList2,
				Triangles = nativeList3,
				Normals = normals
			};
		}

		private static void RecalcNormalsJobified(NativeList<float3> verts, NativeList<int> tris, bool areaWeight, Allocator alloc, ref NativeList<float3> outNormals)
		{
			int length = verts.Length;
			int num = tris.Length / 3;
			NativeArray<float3> nativeArray = new NativeArray<float3>(num, alloc, NativeArrayOptions.ClearMemory);
			new MeshUtilities.TriNormalJob
			{
				V = verts.AsArray(),
				T = tris.AsArray(),
				Out = nativeArray,
				AreaWeight = (areaWeight ? 1 : 0)
			}.Schedule(num, 64, default(JobHandle)).Complete();
			NativeParallelMultiHashMap<int, int> v2T = new NativeParallelMultiHashMap<int, int>(tris.Length, alloc);
			new MeshUtilities.BuildAdjJob
			{
				T = tris.AsArray(),
				MapW = v2T.AsParallelWriter()
			}.Schedule(num, 64, default(JobHandle)).Complete();
			new MeshUtilities.VertexNormalJob
			{
				AreaWeight = (areaWeight ? 1 : 0),
				TriN = nativeArray,
				V2T = v2T,
				Out = outNormals.AsArray()
			}.Schedule(length, 64, default(JobHandle)).Complete();
			nativeArray.Dispose();
			v2T.Dispose();
		}

		public struct MeshData : IDisposable
		{
			public void Dispose()
			{
				this.Vertices.Dispose();
				this.Triangles.Dispose();
				this.Normals.Dispose();
			}

			public NativeList<float3> Vertices;

			public NativeList<int> Triangles;

			public NativeList<float3> Normals;
		}

		public struct VoxelMeshData : IDisposable
		{
			public void Dispose()
			{
				this.Vertices.Dispose();
				this.Materials.Dispose();
				this.Triangles.Dispose();
				this.Normals.Dispose();
			}

			public NativeList<float3> Vertices;

			public NativeList<byte> Materials;

			public NativeList<int> Triangles;

			public NativeList<float3> Normals;
		}

		[BurstCompile]
		public struct FaceNormalJob : IJobParallelFor
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
		public struct SplitJob : IJob
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
				NativeList<MeshUtilities.SplitJob.Bucket> nativeList = new NativeList<MeshUtilities.SplitJob.Bucket>(length, Allocator.Temp);
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
							MeshUtilities.SplitJob.Bucket bucket = nativeList[num3];
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
							MeshUtilities.SplitJob.Bucket bucket2 = new MeshUtilities.SplitJob.Bucket
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
		public struct SplitVoxelMeshJob : IJob
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
				NativeList<MeshUtilities.SplitVoxelMeshJob.Bucket> nativeList = new NativeList<MeshUtilities.SplitVoxelMeshJob.Bucket>(length, Allocator.Temp);
				this.DstVerts.Clear();
				this.DstMats.Clear();
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
							MeshUtilities.SplitVoxelMeshJob.Bucket bucket = nativeList[num3];
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
							byte b = this.SrcMats[index];
							this.DstMats.Add(b);
							MeshUtilities.SplitVoxelMeshJob.Bucket bucket2 = new MeshUtilities.SplitVoxelMeshJob.Bucket
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
			public NativeArray<byte> SrcMats;

			[ReadOnly]
			public NativeArray<int> SrcTris;

			[ReadOnly]
			public NativeArray<float3> FaceN;

			public NativeList<float3> DstVerts;

			public NativeList<byte> DstMats;

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
}
