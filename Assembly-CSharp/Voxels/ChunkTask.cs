using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Voxels
{
	public struct ChunkTask
	{
		public bool IsCreated
		{
			get
			{
				return !this.Handle.Equals(default(JobHandle));
			}
		}

		public bool IsCompleted
		{
			get
			{
				return this.Handle.IsCompleted;
			}
		}

		public bool CompleteIfReady()
		{
			if (this.Handle.IsCompleted)
			{
				this.Complete();
				return true;
			}
			return false;
		}

		public void Complete()
		{
			this.Handle.Complete();
			Action onJobComplete = this._onJobComplete;
			if (onJobComplete != null)
			{
				onJobComplete();
			}
			this._onJobComplete = null;
		}

		public ChunkTask(Chunk chunk, JobHandle handle, Action onComplete = null)
		{
			this.Chunk = chunk;
			this.Handle = handle;
			this._onJobComplete = onComplete;
		}

		public static ChunkTask CreateVoxelDataJob(Chunk chunk, GenerationParameters parameters)
		{
			if (!chunk.Density.IsCreated)
			{
				chunk.Density = new NativeArray<byte>(chunk.VoxelCount, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			}
			if (!chunk.Material.IsCreated)
			{
				chunk.Material = new NativeArray<byte>(chunk.VoxelCount, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			}
			JobHandle handle = new GenerateVoxelDataJob
			{
				chunkPosition = chunk.Id,
				chunkSize = chunk.Size.x,
				dimension = chunk.Dimensions.x,
				noiseScale = parameters.NoiseScale,
				groundLevel = parameters.GroundLevel,
				heightCompensation = parameters.HeightCompensation,
				octaves = parameters.Octaves,
				persistence = parameters.Persistence,
				heightScale = parameters.HeightScale,
				seed = parameters.Seed,
				voxels = chunk.Density,
				materials = chunk.Material
			}.Schedule(chunk.VoxelCount, 64, default(JobHandle));
			Action onComplete = delegate()
			{
				chunk.IsDataGenerated = true;
				chunk.IsMeshGenerated = false;
				chunk.IsDirty = true;
			};
			return new ChunkTask(chunk, handle, onComplete);
		}

		public static ChunkTask CreateMeshDataJob(Chunk chunk, GenerationParameters parameters)
		{
			ChunkTask.<>c__DisplayClass11_0 CS$<>8__locals1 = new ChunkTask.<>c__DisplayClass11_0();
			CS$<>8__locals1.chunk = chunk;
			CS$<>8__locals1.parameters = parameters;
			CS$<>8__locals1.triangleCounter = new NativeCounter(Allocator.TempJob);
			CS$<>8__locals1.onComplete = null;
			MeshGenerationMode meshGenerationMode = CS$<>8__locals1.parameters.MeshGenerationMode;
			JobHandle jobHandle;
			if (meshGenerationMode != MeshGenerationMode.MarchingCubes)
			{
				if (meshGenerationMode != MeshGenerationMode.SurfaceNets)
				{
					throw new ArgumentOutOfRangeException();
				}
				jobHandle = CS$<>8__locals1.<CreateMeshDataJob>g__CreateSurfaceNetsMeshJob|1();
			}
			else
			{
				jobHandle = CS$<>8__locals1.<CreateMeshDataJob>g__CreateMarchingCubesMeshJob|0();
			}
			JobHandle handle = jobHandle;
			return new ChunkTask(CS$<>8__locals1.chunk, handle, CS$<>8__locals1.onComplete);
		}

		public static ChunkTask CreateSurfaceNetsPostProcessingJob(Chunk chunk, GenerationParameters parameters)
		{
			object genericMeshData = chunk.GenericMeshData;
			if (!(genericMeshData is SurfaceNetsBuffer))
			{
				throw new InvalidOperationException(string.Format("{0} GenericMeshData is not a SurfaceNetsBuffer.", chunk));
			}
			SurfaceNetsBuffer surfaceNetsBuffer = (SurfaceNetsBuffer)genericMeshData;
			if (surfaceNetsBuffer.Triangles.Length < 3)
			{
				chunk.IsMeshGenerated = true;
				chunk.IsCollisionBaked = false;
				chunk.IsDirty = true;
				chunk.VertexCount = 0;
				return default(ChunkTask);
			}
			NativeCounter triangleCounter = new NativeCounter(Allocator.TempJob);
			int length = math.min(chunk.VoxelCount * 15, 65535);
			if (!chunk.VertexData.IsCreated)
			{
				chunk.VertexData = new NativeArray<MeshVertexData>(length, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			}
			if (!chunk.TriangleData.IsCreated)
			{
				chunk.TriangleData = new NativeArray<ushort>(length, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			}
			MeshUtilities.VoxelMeshData voxelMeshData = MeshUtilities.SplitByAngle(surfaceNetsBuffer.Vertices.AsArray(), surfaceNetsBuffer.Materials.AsArray(), surfaceNetsBuffer.Triangles.AsArray(), parameters.NormalThreshold, parameters.AreaWeightedNormals, Allocator.TempJob);
			ref NativeList<float3> ptr = ref surfaceNetsBuffer.Vertices;
			NativeList<float3> nativeList = voxelMeshData.Vertices;
			NativeList<float3> nativeList2 = surfaceNetsBuffer.Vertices;
			ptr = nativeList;
			voxelMeshData.Vertices = nativeList2;
			ref NativeList<byte> ptr2 = ref surfaceNetsBuffer.Materials;
			NativeList<byte> materials = voxelMeshData.Materials;
			NativeList<byte> materials2 = surfaceNetsBuffer.Materials;
			ptr2 = materials;
			voxelMeshData.Materials = materials2;
			ptr = ref surfaceNetsBuffer.Normals;
			nativeList2 = voxelMeshData.Normals;
			nativeList = surfaceNetsBuffer.Normals;
			ptr = nativeList2;
			voxelMeshData.Normals = nativeList;
			ref NativeList<int> ptr3 = ref surfaceNetsBuffer.Triangles;
			NativeList<int> triangles = voxelMeshData.Triangles;
			NativeList<int> triangles2 = surfaceNetsBuffer.Triangles;
			ptr3 = triangles;
			voxelMeshData.Triangles = triangles2;
			chunk.GenericMeshData = surfaceNetsBuffer;
			voxelMeshData.Dispose();
			JobHandle handle = new AssembleVertexDataJob
			{
				vertexData = chunk.VertexData,
				triangleData = chunk.TriangleData,
				triangleCounter = triangleCounter,
				srcVerts = surfaceNetsBuffer.Vertices.AsArray(),
				srcMats = surfaceNetsBuffer.Materials.AsArray(),
				srcNorm = surfaceNetsBuffer.Normals.AsArray(),
				srcTris = surfaceNetsBuffer.Triangles.AsArray()
			}.Schedule(default(JobHandle));
			Action onComplete = delegate()
			{
				chunk.IsMeshGenerated = true;
				chunk.IsCollisionBaked = false;
				chunk.IsDirty = true;
				chunk.VertexCount = triangleCounter.Count * 3;
				triangleCounter.Dispose();
			};
			return new ChunkTask(chunk, handle, onComplete);
		}

		public static ChunkTask CreateCollisionJob(Chunk chunk, GenerationParameters parameters = default(GenerationParameters))
		{
			if (chunk.Mesh == null)
			{
				chunk.IsCollisionBaked = true;
				chunk.IsDirty = true;
				return default(ChunkTask);
			}
			JobHandle handle = new CollisionJob
			{
				MeshId = chunk.Mesh.GetEntityId()
			}.Schedule(default(JobHandle));
			Action onJobComplete = delegate()
			{
				chunk.IsCollisionBaked = true;
				chunk.IsDirty = true;
			};
			return new ChunkTask
			{
				Handle = handle,
				_onJobComplete = onJobComplete,
				Chunk = chunk
			};
		}

		public Chunk Chunk;

		public JobHandle Handle;

		private Action _onJobComplete;
	}
}
