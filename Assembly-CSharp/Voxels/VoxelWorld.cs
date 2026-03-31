using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace Voxels
{
	[DefaultExecutionOrder(5)]
	public class VoxelWorld : MonoBehaviour
	{
		public bool Initialized { get; private set; }

		public bool IsInfinite
		{
			get
			{
				return this.worldType == VoxelWorld.WorldType.Infinite;
			}
		}

		public bool UpdateWorld
		{
			get
			{
				return this._updateWorld;
			}
			set
			{
				this._updateWorld = value;
			}
		}

		public int3 ChunkSize { get; private set; }

		public int VoxelDimension { get; private set; }

		public int VoxelCount { get; private set; }

		public MeshGenerationMode MeshGenerationMode
		{
			get
			{
				return this.generationParameters.MeshGenerationMode;
			}
		}

		public bool WorldGenerationComplete
		{
			get
			{
				return this.chunks.Count > 0 && this.chunksToGenerate.Count == 0;
			}
		}

		public static bool ExistsFor(Scene scene)
		{
			return VoxelWorld.WorldLookup.ContainsKey(scene.GetHashCode());
		}

		public static bool ExistsFor(GameObject gameObject)
		{
			return VoxelWorld.ExistsFor(gameObject.scene);
		}

		public static bool ExistsFor(Component component)
		{
			return VoxelWorld.ExistsFor(component.gameObject.scene);
		}

		public static void SetFor(Scene scene, VoxelWorld voxelWorld)
		{
			if (!VoxelWorld.WorldLookup.TryAdd(scene.GetHashCode(), voxelWorld))
			{
				throw new InvalidOperationException(string.Format("Scene {0} already has a VoxelWorld set.", scene));
			}
		}

		public static void SetFor(GameObject gameObject, VoxelWorld voxelWorld)
		{
			VoxelWorld.SetFor(gameObject.scene, voxelWorld);
		}

		public static void SetFor(Component component, VoxelWorld voxelWorld)
		{
			VoxelWorld.SetFor(component.gameObject.scene, voxelWorld);
		}

		public static VoxelWorld GetFor(Scene scene)
		{
			VoxelWorld result;
			if (!VoxelWorld.WorldLookup.TryGetValue(scene.GetHashCode(), out result))
			{
				Debug.LogError(string.Format("No VoxelWorld found for scene {0}", scene));
			}
			return result;
		}

		public static VoxelWorld GetFor(GameObject gameObject)
		{
			return VoxelWorld.GetFor(gameObject.scene);
		}

		public static VoxelWorld GetFor(Component component)
		{
			return VoxelWorld.GetFor(component.gameObject.scene);
		}

		private void Awake()
		{
			if (this.registerAsSceneWorld && !VoxelWorld.ExistsFor(this))
			{
				VoxelWorld.SetFor(base.gameObject, this);
			}
			MeshGenerationMode meshGenerationMode = this.generationParameters.MeshGenerationMode;
			if (meshGenerationMode != MeshGenerationMode.MarchingCubes)
			{
				if (meshGenerationMode != MeshGenerationMode.SurfaceNets)
				{
					throw new ArgumentOutOfRangeException();
				}
				Chunk.Pad = 2;
			}
			else
			{
				Chunk.Pad = 1;
			}
			if (!this.target)
			{
				this.target = base.transform;
			}
		}

		private void Start()
		{
			Chunk.DefaultSize = this.chunkSize;
			this.ChunkSize = Chunk.DefaultSize;
			this.VoxelDimension = this.chunkSize + Chunk.Pad;
			this.VoxelCount = this.VoxelDimension * this.VoxelDimension * this.VoxelDimension;
			int num = this.viewDistance * 2 + 1;
			this.chunksToGenerate = new NativeHashSet<int3>(num * num * num, Allocator.Persistent);
			this.ConfigurePools();
			this.Initialized = true;
		}

		private void OnDestroy()
		{
			if (this.persistChanges)
			{
				this.SaveChunks();
			}
		}

		private void Update()
		{
			if (!this._updateWorld)
			{
				return;
			}
			if (this.generationQueueChanged)
			{
				this.sortJobHandle.Complete();
				this.sortedChunkCount = this.sortedChunks.Length;
				this.chunkSortIndex = 0;
				this.generationQueueChanged = false;
			}
			foreach (ChunkTaskSet chunkTaskSet in this.chunkJobs.Values)
			{
				if (chunkTaskSet.CompleteIfReady())
				{
					this.HandleJobCompletion(chunkTaskSet);
				}
			}
			foreach (int3 key in this.completedJobs)
			{
				this.chunkJobs.Remove(key);
			}
			this.completedJobs.Clear();
			using (Dictionary<int3, Chunk>.ValueCollection.Enumerator enumerator3 = this.chunks.Values.GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					Chunk chunk = enumerator3.Current;
					if (chunk.IsDirty)
					{
						this.chunksToGenerate.Add(chunk.Id);
						this.generationQueueChanged = true;
					}
				}
				goto IL_155;
			}
			IL_11E:
			int num = this.chunkSortIndex;
			this.chunkSortIndex = num + 1;
			int3 @int = this.sortedChunks[num];
			this.chunksToGenerate.Remove(@int);
			this.ProcessChunk(@int);
			IL_155:
			if (this.chunkSortIndex >= this.sortedChunks.Length || this.chunkJobs.Count >= this.maxJobs)
			{
				this.UpdateVisibleChunks(false);
				return;
			}
			goto IL_11E;
		}

		private void SaveChunks()
		{
			Debug.Log("Saving chunks...");
			foreach (Chunk chunk in this.chunks.Values)
			{
				if (chunk.IsDataChanged)
				{
					ChunkIO.SaveChunk(new ChunkDTO(chunk));
				}
			}
		}

		private void ConfigurePools()
		{
			this._chunkPool = new UnityEngine.Pool.ObjectPool<Chunk>(() => new Chunk(int3.zero, this.ChunkSize, -1), delegate(Chunk chunk)
			{
			}, delegate(Chunk chunk)
			{
				if (chunk.Component)
				{
					this._chunkComponentPool.Release(chunk.Component);
					chunk.SetComponent(null);
				}
				chunk.Clear();
			}, delegate(Chunk chunk)
			{
				chunk.Dispose();
			}, true, 100, 100);
			this._chunkComponentPool = new UnityEngine.Pool.ObjectPool<ChunkComponent>(() => Object.Instantiate<ChunkComponent>(this.chunkPrefab), delegate(ChunkComponent chunkComponent)
			{
				chunkComponent.gameObject.SetActive(false);
				chunkComponent.transform.SetParent(base.transform, false);
			}, delegate(ChunkComponent chunkComponent)
			{
				if (chunkComponent.meshFilter.sharedMesh)
				{
					Mesh sharedMesh = chunkComponent.meshFilter.sharedMesh;
					chunkComponent.meshFilter.sharedMesh = null;
					chunkComponent.meshCollider.sharedMesh = null;
					this._meshPool.Release(sharedMesh);
				}
				chunkComponent.gameObject.SetActive(false);
			}, delegate(ChunkComponent chunkComponent)
			{
				if (chunkComponent)
				{
					Object.Destroy(chunkComponent.gameObject);
				}
			}, true, 100, 100);
			this._meshPool = new UnityEngine.Pool.ObjectPool<Mesh>(() => new Mesh(), null, delegate(Mesh mesh)
			{
				mesh.Clear(false);
			}, null, true, 100, 100);
		}

		private Chunk GetPooledChunk(int3 chunkId)
		{
			Chunk chunk = this._chunkPool.Get();
			chunk.World = this;
			chunk.Id = chunkId;
			return chunk;
		}

		private Chunk CreateOrLoadChunk(int3 chunkId)
		{
			Chunk pooledChunk = this.GetPooledChunk(chunkId);
			ChunkDTO from;
			if (this.persistChanges && ChunkIO.TryLoadChunk(chunkId, out from))
			{
				pooledChunk.SetFrom(from);
			}
			else
			{
				pooledChunk.Id = chunkId;
			}
			return pooledChunk;
		}

		private void Save(Chunk chunk)
		{
			if (chunk.IsDataChanged)
			{
				ChunkIO.SaveChunk(new ChunkDTO(chunk));
			}
		}

		private void Unload(Chunk chunk)
		{
			if (this.persistChanges)
			{
				this.Save(chunk);
			}
			this._chunkPool.Release(chunk);
		}

		private void UpdateVisibleChunks(bool isFirstTime = false)
		{
			int3 chunkIdForWorldPosition = this.GetChunkIdForWorldPosition(this.target.position);
			if (chunkIdForWorldPosition.Equals(this.playerChunk) && !this.generationQueueChanged)
			{
				return;
			}
			this.playerChunk = chunkIdForWorldPosition;
			this.generationQueueChanged = true;
			VoxelWorld.WorldType worldType = this.worldType;
			if (worldType != VoxelWorld.WorldType.Infinite)
			{
				if (worldType != VoxelWorld.WorldType.Bounded)
				{
					throw new ArgumentOutOfRangeException();
				}
				ValueTuple<int3, int3> chunkBoundsForLocalBounds = this.GetChunkBoundsForLocalBounds(this.worldBounds);
				int3 item = chunkBoundsForLocalBounds.Item1;
				int3 item2 = chunkBoundsForLocalBounds.Item2;
				for (int i = item.x; i <= item2.x; i++)
				{
					for (int j = item.y; j <= item2.y; j++)
					{
						for (int k = item.z; k <= item2.z; k++)
						{
							int3 @int = new int3(i, j, k);
							if (!this.chunks.ContainsKey(@int) && !this.chunksToGenerate.Contains(@int))
							{
								this.chunksToGenerate.Add(@int);
							}
						}
					}
				}
			}
			else
			{
				for (int l = -this.viewDistance; l <= this.viewDistance; l++)
				{
					for (int m = -this.viewDistance; m <= this.viewDistance; m++)
					{
						for (int n = -this.viewDistance; n <= this.viewDistance; n++)
						{
							int3 int2 = this.playerChunk + new int3(l, m, n);
							if (!this.chunks.ContainsKey(int2) && !this.chunksToGenerate.Contains(int2))
							{
								this.chunksToGenerate.Add(int2);
							}
						}
					}
				}
			}
			SortChunksJob jobData = new SortChunksJob
			{
				ChunkSet = this.chunksToGenerate,
				SortedChunks = this.sortedChunks
			};
			this.sortJobHandle = jobData.Schedule(default(JobHandle));
			if (this.worldType == VoxelWorld.WorldType.Infinite)
			{
				int num = this.viewDistance + 2;
				this.chunksToRemove.Clear();
				foreach (int3 int3 in this.chunks.Keys)
				{
					if (Mathf.Abs(int3.x - this.playerChunk.x) > num || Mathf.Abs(int3.y - this.playerChunk.y) > num || Mathf.Abs(int3.z - this.playerChunk.z) > num)
					{
						this.chunksToRemove.Add(int3);
					}
				}
				foreach (int3 key in this.chunksToRemove)
				{
					Chunk chunk;
					if (this.chunks.TryGetValue(key, out chunk))
					{
						ChunkTaskSet chunkTaskSet;
						if (this.chunkJobs.TryGetValue(key, out chunkTaskSet))
						{
							chunkTaskSet.Complete();
							this.chunkJobs.Remove(key);
						}
						this.Unload(chunk);
						this.chunks.Remove(key);
					}
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int3 GetChunkIdForWorldPosition(Vector3 worldPosition)
		{
			return this.GetLocalPosition(worldPosition).LocalPositionToChunkId(this.ChunkSize);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int3 GetChunkIdForLocalPosition(Vector3 voxelWorldPosition)
		{
			return voxelWorldPosition.LocalPositionToChunkId(this.ChunkSize);
		}

		public void SetWorldType(VoxelWorld.WorldType newWorldType, bool force = false)
		{
			if (!force && this.worldType == newWorldType)
			{
				return;
			}
			this.worldType = newWorldType;
			this.RegenerateAllChunks();
		}

		public void SetWorldBounds(UnityEngine.BoundsInt bounds)
		{
			this.worldBounds = bounds;
			this.SetWorldType(VoxelWorld.WorldType.Bounded, true);
		}

		public static void SaveWorld(Scene scene)
		{
			VoxelWorld @for = VoxelWorld.GetFor(scene);
			foreach (Chunk chunk in @for.chunks.Values)
			{
				@for.Save(chunk);
			}
		}

		public static void ResetWorld(Scene scene)
		{
			ChunkIO.DeleteWorld();
			VoxelWorld @for = VoxelWorld.GetFor(scene);
			if (@for)
			{
				@for.RegenerateAllChunks();
			}
		}

		private void RegenerateAllChunks()
		{
			foreach (ChunkTaskSet chunkTaskSet in this.chunkJobs.Values)
			{
				chunkTaskSet.Complete();
			}
			this.chunkJobs.Clear();
			this.completedJobs.Clear();
			this.chunksToGenerate.Clear();
			this.sortedChunks.Clear();
			foreach (Chunk chunk in this.chunks.Values)
			{
				chunk.Clear();
			}
			this.generationQueueChanged = true;
		}

		private void ProcessChunk(int3 chunkId)
		{
			Chunk chunk;
			if (!this.chunks.TryGetValue(chunkId, out chunk))
			{
				chunk = this.CreateOrLoadChunk(chunkId);
				this.chunks[chunkId] = chunk;
			}
			if (!chunk.IsDirty)
			{
				Debug.LogWarning(string.Format("{0} is not dirty, skipping processing", chunk));
				return;
			}
			chunk.IsDirty = false;
			ChunkTaskSet chunkTaskSet = new ChunkTaskSet(chunk, this.generationParameters, Array.Empty<ValueTuple<ChunkTaskSet.ChunkTaskDelegate, Action<Chunk>>>());
			ChunkState state = chunk.State;
			if (state < ChunkState.VoxelDataGenerated)
			{
				chunkTaskSet.AddTask(new ChunkTaskSet.ChunkTaskDelegate(ChunkTask.CreateVoxelDataJob), null);
			}
			if (state < ChunkState.MeshDataGenerated)
			{
				if (this.generationParameters.MeshGenerationMode == MeshGenerationMode.MarchingCubes)
				{
					chunkTaskSet.AddTask(new ChunkTaskSet.ChunkTaskDelegate(ChunkTask.CreateMeshDataJob), new Action<Chunk>(this.CreateChunkMesh));
				}
				else if (this.generationParameters.MeshGenerationMode == MeshGenerationMode.SurfaceNets)
				{
					chunkTaskSet.AddTask(new ChunkTaskSet.ChunkTaskDelegate(ChunkTask.CreateMeshDataJob), null);
					chunkTaskSet.AddTask(new ChunkTaskSet.ChunkTaskDelegate(ChunkTask.CreateSurfaceNetsPostProcessingJob), new Action<Chunk>(this.CreateChunkMesh));
				}
				else
				{
					Debug.LogError(string.Format("Unknown mesh generation mode: {0}", this.generationParameters.MeshGenerationMode));
				}
			}
			else if (state < ChunkState.MeshCreated)
			{
				chunkTaskSet.AddTask(null, new Action<Chunk>(this.CreateChunkMesh));
			}
			if (state < ChunkState.CollisionBaked)
			{
				chunkTaskSet.AddTask(new ChunkTaskSet.ChunkTaskDelegate(ChunkTask.CreateCollisionJob), new Action<Chunk>(this.AssignMesh));
			}
			else if (state < ChunkState.MeshAssigned)
			{
				chunkTaskSet.AddTask(null, new Action<Chunk>(this.AssignMesh));
			}
			if (!chunkTaskSet.IsEmpty)
			{
				this.chunkJobs.Add(chunk.Id, chunkTaskSet);
				chunkTaskSet.Start();
				return;
			}
			Debug.LogWarning(string.Format("{0} was dirty but nothing to do?", chunk));
		}

		private void MeshChunkImmediately(Chunk chunk)
		{
			ChunkTask.CreateMeshDataJob(chunk, this.generationParameters).Complete();
			if (this.generationParameters.MeshGenerationMode == MeshGenerationMode.SurfaceNets)
			{
				ChunkTask.CreateSurfaceNetsPostProcessingJob(chunk, this.generationParameters).Complete();
			}
			chunk.Mesh = this.CreateMesh(chunk);
			if (chunk.Mesh)
			{
				ChunkTask.CreateCollisionJob(chunk, default(GenerationParameters)).Complete();
			}
			this.AssignMesh(chunk);
			this.chunkJobs.Remove(chunk.Id);
		}

		private void CreateChunkMesh(Chunk chunk)
		{
			chunk.Mesh = this.CreateMesh(chunk);
		}

		private Mesh CreateMesh(Chunk chunk)
		{
			if (chunk.VertexCount == 0)
			{
				chunk.DisposeMeshData();
				chunk.IsMeshCreated = true;
				return null;
			}
			int vertexCount = chunk.VertexCount;
			Mesh mesh = this._meshPool.Get();
			if (vertexCount > chunk.VertexData.Length)
			{
				Debug.LogError(string.Format("Vertex count {0} exceeds allocated vertex data length {1} for chunk {2}. This is likely a bug in the meshing job.", vertexCount, chunk.VertexData.Length, chunk.Id));
				return null;
			}
			mesh.SetVertexBufferParams(vertexCount, MeshVertexData.VertexBufferMemoryLayout);
			mesh.SetIndexBufferParams(vertexCount, IndexFormat.UInt16);
			mesh.SetVertexBufferData<MeshVertexData>(chunk.VertexData, 0, 0, vertexCount, 0, MeshUpdateFlags.DontValidateIndices);
			mesh.SetIndexBufferData<ushort>(chunk.TriangleData, 0, 0, vertexCount, MeshUpdateFlags.DontValidateIndices);
			mesh.subMeshCount = 1;
			mesh.SetSubMesh(0, new SubMeshDescriptor(0, vertexCount, MeshTopology.Triangles), MeshUpdateFlags.Default);
			mesh.RecalculateBounds();
			chunk.DisposeMeshData();
			chunk.IsMeshCreated = true;
			return mesh;
		}

		private void AssignMesh(Chunk chunk)
		{
			Mesh mesh = chunk.Mesh;
			if (mesh)
			{
				if (!chunk.Component)
				{
					ChunkComponent chunkComponent = this._chunkComponentPool.Get();
					chunkComponent.transform.SetParent(base.transform, false);
					chunkComponent.transform.localScale = Vector3.one * this.worldScale;
					chunkComponent.transform.localPosition = (chunk.Id * this.ChunkSize).ToVector3() * this.worldScale;
					chunk.SetComponent(chunkComponent);
				}
				chunk.MeshFilter.sharedMesh = mesh;
				chunk.MeshCollider.sharedMesh = mesh;
				chunk.GameObject.SetActive(true);
			}
			else if (chunk.Component)
			{
				this._chunkComponentPool.Release(chunk.Component);
				chunk.SetComponent(null);
			}
			chunk.IsMeshAssigned = true;
			chunk.IsDirty = false;
		}

		public void SetVoxelDensityCustom(UnityEngine.BoundsInt worldBounds, Func<int3, byte, byte> setDensityFunction)
		{
			VoxelWorld.<>c__DisplayClass85_0 CS$<>8__locals1 = new VoxelWorld.<>c__DisplayClass85_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.worldBounds = worldBounds;
			CS$<>8__locals1.setDensityFunction = setDensityFunction;
			this.ForEachChunkInBounds(CS$<>8__locals1.worldBounds, new Action<Chunk>(CS$<>8__locals1.<SetVoxelDensityCustom>g__SetVoxelDensityInChunk|0));
		}

		public void SetVoxelDataCustom(UnityEngine.BoundsInt worldBounds, [TupleElementNames(new string[]
		{
			"density",
			"material",
			"density",
			"material"
		})] Func<int3, ValueTuple<byte, byte>, ValueTuple<byte, byte>> setDataFunction)
		{
			VoxelWorld.<>c__DisplayClass86_0 CS$<>8__locals1 = new VoxelWorld.<>c__DisplayClass86_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.worldBounds = worldBounds;
			CS$<>8__locals1.setDataFunction = setDataFunction;
			this.ForEachChunkInBounds(CS$<>8__locals1.worldBounds, new Action<Chunk>(CS$<>8__locals1.<SetVoxelDataCustom>g__SetVoxelDataInChunk|0));
		}

		public void SetVoxelDataCustom(int3[] voxels, [TupleElementNames(new string[]
		{
			"density",
			"material",
			"density",
			"material"
		})] Func<int3, ValueTuple<byte, byte>, ValueTuple<byte, byte>> setDataFunction)
		{
			VoxelWorld.<>c__DisplayClass87_0 CS$<>8__locals1 = new VoxelWorld.<>c__DisplayClass87_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.voxels = voxels;
			CS$<>8__locals1.setDataFunction = setDataFunction;
			UnityEngine.BoundsInt boundsFor = VoxelWorld.GetBoundsFor(CS$<>8__locals1.voxels);
			this.ForEachChunkInBounds(boundsFor, new Action<Chunk>(CS$<>8__locals1.<SetVoxelDataCustom>g__SetVoxelDataInChunk|0));
		}

		public static UnityEngine.BoundsInt GetBoundsFor(int3[] voxels)
		{
			int3 @int = new int3(int.MaxValue, int.MaxValue, int.MaxValue);
			int3 int2 = new int3(int.MinValue, int.MinValue, int.MinValue);
			foreach (int3 x in voxels)
			{
				@int = math.min(x, @int);
				int2 = math.max(x, int2);
			}
			return new UnityEngine.BoundsInt
			{
				min = @int.ToVectorInt(),
				max = int2.ToVectorInt()
			};
		}

		[return: TupleElementNames(new string[]
		{
			"min",
			"max"
		})]
		public ValueTuple<int3, int3> GetChunkBoundsForLocalBounds(UnityEngine.BoundsInt worldBounds)
		{
			return new ValueTuple<int3, int3>((worldBounds.min - Vector3Int.one * Chunk.Pad).LocalPositionToChunkId(this.ChunkSize), worldBounds.max.LocalPositionToChunkId(this.ChunkSize));
		}

		public bool BoundsChunksLoaded(UnityEngine.BoundsInt localWorldBounds)
		{
			if (!this.Initialized)
			{
				return false;
			}
			ValueTuple<int3, int3> chunkBoundsForLocalBounds = this.GetChunkBoundsForLocalBounds(localWorldBounds);
			int3 item = chunkBoundsForLocalBounds.Item1;
			int3 item2 = chunkBoundsForLocalBounds.Item2;
			for (int i = item.x; i <= item2.x; i++)
			{
				for (int j = item.y; j <= item2.y; j++)
				{
					for (int k = item.z; k <= item2.z; k++)
					{
						int3 key = new int3(i, j, k);
						Chunk chunk;
						if (!this.chunks.TryGetValue(key, out chunk))
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		private void ForEachChunkInBounds(UnityEngine.BoundsInt worldBounds, Action<Chunk> action)
		{
			int3 @int = (worldBounds.min - Vector3Int.one * Chunk.Pad).LocalPositionToChunkId(this.ChunkSize);
			int3 int2 = worldBounds.max.LocalPositionToChunkId(this.ChunkSize);
			for (int i = @int.x; i <= int2.x; i++)
			{
				for (int j = @int.y; j <= int2.y; j++)
				{
					for (int k = @int.z; k <= int2.z; k++)
					{
						int3 key = new int3(i, j, k);
						Chunk obj;
						if (this.chunks.TryGetValue(key, out obj))
						{
							action(obj);
						}
					}
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Chunk GetChunkForLocalPosition(int3 worldPosition)
		{
			return this.chunks.GetValueOrDefault(worldPosition.LocalPositionToChunkId(this.ChunkSize));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Chunk GetChunkForLocalPosition(Vector3 worldPosition)
		{
			return this.chunks.GetValueOrDefault(worldPosition.LocalPositionToChunkId(this.ChunkSize));
		}

		private void ForEachVoxelInChunkInBounds(UnityEngine.BoundsInt worldBounds, Chunk chunk, Action<int3, int3, int, byte> action)
		{
			int3 @int = new int3(worldBounds.min.x, worldBounds.min.y, worldBounds.min.z);
			int3 int2 = new int3(worldBounds.max.x, worldBounds.max.y, worldBounds.max.z);
			int3 int3 = chunk.Id * chunk.Size;
			int3 int4 = int3 + chunk.Dimensions - 1;
			@int.x = math.max(@int.x, int3.x);
			@int.y = math.max(@int.y, int3.y);
			@int.z = math.max(@int.z, int3.z);
			int2.x = math.min(int2.x, int4.x);
			int2.y = math.min(int2.y, int4.y);
			int2.z = math.min(int2.z, int4.z);
			if (@int.x > int2.x || @int.y > int2.y || @int.z > int2.z)
			{
				Debug.LogWarning(string.Format("No overlap between chunk {0} and bounds {1}", chunk.Id, worldBounds));
				return;
			}
			for (int i = @int.x; i <= int2.x; i++)
			{
				for (int j = @int.y; j <= int2.y; j++)
				{
					for (int k = @int.z; k <= int2.z; k++)
					{
						int3 int5 = new int3(i, j, k);
						int3 int6 = int5 - chunk.Id * chunk.Size;
						int num = int6.x + this.VoxelDimension * (int6.y + this.VoxelDimension * int6.z);
						byte arg = chunk.Density[num];
						action(int5, int6, num, arg);
					}
				}
			}
		}

		private void ForEachVoxelInChunkInBounds(UnityEngine.BoundsInt worldBounds, Chunk chunk, Action<int3, int3, int, byte, byte> action)
		{
			int3 @int = new int3(worldBounds.min.x, worldBounds.min.y, worldBounds.min.z);
			int3 int2 = new int3(worldBounds.max.x, worldBounds.max.y, worldBounds.max.z);
			int3 int3 = chunk.Id * chunk.Size;
			int3 int4 = int3 + chunk.Dimensions - 1;
			@int.x = math.max(@int.x, int3.x);
			@int.y = math.max(@int.y, int3.y);
			@int.z = math.max(@int.z, int3.z);
			int2.x = math.min(int2.x, int4.x);
			int2.y = math.min(int2.y, int4.y);
			int2.z = math.min(int2.z, int4.z);
			if (@int.x > int2.x || @int.y > int2.y || @int.z > int2.z)
			{
				Debug.LogWarning(string.Format("No overlap between chunk {0} and bounds {1}", chunk.Id, worldBounds));
				return;
			}
			for (int i = @int.x; i <= int2.x; i++)
			{
				for (int j = @int.y; j <= int2.y; j++)
				{
					for (int k = @int.z; k <= int2.z; k++)
					{
						int3 int5 = new int3(i, j, k);
						int3 int6 = int5 - chunk.Id * chunk.Size;
						int num = int6.x + this.VoxelDimension * (int6.y + this.VoxelDimension * int6.z);
						byte arg = chunk.Density[num];
						byte arg2 = chunk.Material[num];
						action(int5, int6, num, arg, arg2);
					}
				}
			}
		}

		private void ForEachSpecifiedVoxelInChunk(int3[] voxels, Chunk chunk, Action<int3, int3, int, byte, byte> action)
		{
			int3 @int = chunk.Id * chunk.Size;
			int3 max = @int + chunk.Dimensions - 1;
			foreach (int3 int2 in voxels)
			{
				if (int2.IsInBounds(@int, max))
				{
					int3 int3 = int2 - chunk.Id * chunk.Size;
					int num = int3.x + this.VoxelDimension * (int3.y + this.VoxelDimension * int3.z);
					byte arg = chunk.Density[num];
					byte arg2 = chunk.Material[num];
					action(int2, int3, num, arg, arg2);
				}
			}
		}

		private void HandleJobCompletion(ChunkTaskSet chunkTask)
		{
			if (!chunkTask.HasChunks)
			{
				Debug.LogError("Chunk is null in HandleJobCompletion");
			}
			Chunk chunk = chunkTask.Chunk;
			if (chunk.State < ChunkState.MeshAssigned || chunk.IsDirty)
			{
				Debug.LogWarning(string.Format("{0} job completed with state {1} and dirty {2}", chunk, chunk.State, chunk.IsDirty));
			}
			this.completedJobs.Add(chunkTask.Chunk.Id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public byte GetDensityAt(Vector3 voxelWorldPosition)
		{
			return this.GetDensityAt(voxelWorldPosition.ToInt3(), 0);
		}

		public byte GetDensityAt(int3 voxelWorldPosition, byte defaultDensity = 0)
		{
			Chunk chunkForLocalPosition = this.GetChunkForLocalPosition(voxelWorldPosition);
			if (chunkForLocalPosition != null && chunkForLocalPosition.IsDataGenerated)
			{
				int3 localPosition = chunkForLocalPosition.GetLocalPosition(voxelWorldPosition);
				return chunkForLocalPosition.Density[localPosition.x + this.VoxelDimension * (localPosition.y + this.VoxelDimension * localPosition.z)];
			}
			return defaultDensity;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetDensityAt(Vector3 voxelWorldPosition, byte density)
		{
			this.SetDensityAt(voxelWorldPosition.ToInt3(), density);
		}

		public void SetDensityAt(int3 voxelWorldPosition, byte density)
		{
			Chunk chunkForLocalPosition = this.GetChunkForLocalPosition(voxelWorldPosition);
			if (chunkForLocalPosition != null && chunkForLocalPosition.IsDataGenerated)
			{
				int3 localPosition = chunkForLocalPosition.GetLocalPosition(voxelWorldPosition);
				int index = localPosition.x + this.VoxelDimension * (localPosition.y + this.VoxelDimension * localPosition.z);
				if (chunkForLocalPosition.Density[index] != density)
				{
					chunkForLocalPosition.Density[index] = density;
					chunkForLocalPosition.IsDataChanged = true;
					chunkForLocalPosition.IsMeshCreated = false;
					chunkForLocalPosition.IsDirty = true;
					return;
				}
			}
			else
			{
				Debug.LogWarning(string.Format("No chunk found for world position {0}, cannot set density.", voxelWorldPosition));
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3 GetLocalPosition(Vector3 worldPosition)
		{
			return Matrix4x4.TRS(base.transform.position, base.transform.rotation, Vector3.one * this.worldScale).inverse.MultiplyPoint(worldPosition);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3 GetWorldPosition(Vector3 localPosition)
		{
			return Matrix4x4.TRS(base.transform.position, base.transform.rotation, Vector3.one * this.worldScale).MultiplyPoint(localPosition);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3 GetWorldPosition(int3 localPosition)
		{
			return this.GetWorldPosition(localPosition.ToVector3());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int3 GetVoxelForWorldPosition(Vector3 worldPosition)
		{
			return this.GetLocalPosition(worldPosition).RoundToInt();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int3 GetVoxelForLocalPosition(Vector3 localPosition)
		{
			return localPosition.RoundToInt();
		}

		public float Scale
		{
			get
			{
				return this.worldScale;
			}
		}

		private static readonly Dictionary<int, VoxelWorld> WorldLookup = new Dictionary<int, VoxelWorld>();

		[Header("World Settings")]
		public GenerationParameters generationParameters = new GenerationParameters
		{
			NoiseScale = 0.01f,
			GroundLevel = 0f,
			HeightScale = 0.01f,
			Octaves = 4,
			Persistence = 0.5f,
			IsoLevel = 0f,
			Seed = 12345,
			NormalThreshold = 60f,
			AreaWeightedNormals = true
		};

		[SerializeField]
		private VoxelWorld.WorldType worldType;

		[SerializeField]
		private UnityEngine.BoundsInt worldBounds;

		[SerializeField]
		private float worldScale = 1f;

		[SerializeField]
		private int chunkSize = 32;

		[SerializeField]
		private int viewDistance = 5;

		[SerializeField]
		private int maxJobs = 10;

		[SerializeField]
		private bool registerAsSceneWorld = true;

		[SerializeField]
		private bool persistChanges = true;

		[Header("References")]
		public ChunkComponent chunkPrefab;

		public Transform target;

		protected Dictionary<int3, Chunk> chunks = new Dictionary<int3, Chunk>();

		private Dictionary<int3, ChunkTaskSet> chunkJobs = new Dictionary<int3, ChunkTaskSet>();

		private List<int3> completedJobs = new List<int3>();

		protected NativeHashSet<int3> chunksToGenerate;

		protected NativeList<int3> sortedChunks = new NativeList<int3>(Allocator.Persistent);

		protected int chunkSortIndex;

		protected JobHandle sortJobHandle;

		protected int sortedChunkCount;

		protected int3 playerChunk = new int3(int.MaxValue, int.MaxValue, int.MaxValue);

		protected bool generationQueueChanged;

		private List<int3> chunksToRemove = new List<int3>();

		private UnityEngine.Pool.ObjectPool<Chunk> _chunkPool;

		private UnityEngine.Pool.ObjectPool<ChunkComponent> _chunkComponentPool;

		private UnityEngine.Pool.ObjectPool<Mesh> _meshPool;

		private bool _updateWorld = true;

		public enum WorldType
		{
			Infinite,
			Bounded
		}
	}
}
