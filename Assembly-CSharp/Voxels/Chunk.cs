using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Voxels
{
	[Serializable]
	public class Chunk : IDisposable
	{
		public ChunkComponent Component { get; private set; }

		public GameObject GameObject { get; private set; }

		public MeshFilter MeshFilter { get; private set; }

		public MeshRenderer MeshRenderer { get; private set; }

		public MeshCollider MeshCollider { get; private set; }

		public static int3 DefaultSize { get; set; } = 32;

		public static int Pad { get; set; } = 1;

		public ChunkState State
		{
			get
			{
				if (!this.IsDataGenerated)
				{
					return ChunkState.Created;
				}
				if (!this.IsMeshGenerated)
				{
					return ChunkState.VoxelDataGenerated;
				}
				if (!this.IsMeshCreated)
				{
					return ChunkState.MeshDataGenerated;
				}
				if (!this.IsCollisionBaked)
				{
					return ChunkState.MeshCreated;
				}
				if (!this.IsMeshAssigned)
				{
					return ChunkState.CollisionBaked;
				}
				return ChunkState.MeshAssigned;
			}
		}

		public Chunk(ChunkDTO dto)
		{
			this.Id = dto.Id;
			this.Size = dto.Size;
			this.Dimensions = dto.Dimensions;
			this.VoxelCount = this.Dimensions.x * this.Dimensions.y * this.Dimensions.z;
			this.Density = dto.Density;
			this.Material = dto.Material;
			this.VertexData = default(NativeArray<MeshVertexData>);
			this.TriangleData = default(NativeArray<ushort>);
			this.GenericMeshData = null;
		}

		public Chunk(int3 id, int3 size, int padding = -1)
		{
			if (padding < 0)
			{
				padding = Chunk.Pad;
			}
			this.Id = id;
			this.Size = size;
			this.Dimensions = size + padding;
			this.VoxelCount = this.Dimensions.x * this.Dimensions.y * this.Dimensions.z;
			this.Density = default(NativeArray<byte>);
			this.Material = default(NativeArray<byte>);
			this.VertexData = default(NativeArray<MeshVertexData>);
			this.TriangleData = default(NativeArray<ushort>);
			this.GenericMeshData = null;
		}

		public void SetFrom(ChunkDTO dto)
		{
			this.Id = dto.Id;
			this.Size = dto.Size;
			this.Dimensions = dto.Dimensions;
			this.VoxelCount = this.Dimensions.x * this.Dimensions.y * this.Dimensions.z;
			this.Dispose();
			this.Density = dto.Density;
			this.Material = dto.Material;
			this.IsDataGenerated = true;
			this.IsDataChanged = true;
			this.IsMeshGenerated = false;
			this.IsMeshCreated = false;
			this.IsCollisionBaked = false;
			this.IsMeshAssigned = false;
			this.IsDirty = true;
			this.VertexCount = 0;
			this.Mesh = null;
		}

		public void Clear()
		{
			this.DisposeMeshData();
			this.IsDataGenerated = false;
			this.IsDataChanged = false;
			this.IsMeshGenerated = false;
			this.IsMeshCreated = false;
			this.IsCollisionBaked = false;
			this.IsMeshAssigned = false;
			this.IsDirty = true;
			this.VertexCount = 0;
			this.Mesh = null;
		}

		public void SetComponent(ChunkComponent chunkComponent)
		{
			this.Component = chunkComponent;
			if (chunkComponent)
			{
				this.GameObject = chunkComponent.gameObject;
				this.MeshFilter = chunkComponent.meshFilter;
				this.MeshRenderer = chunkComponent.meshRenderer;
				this.MeshCollider = chunkComponent.meshCollider;
				this.Component.name = Chunk.GetChunkName(this.Id);
				this.Component.World = this.World;
				return;
			}
			this.GameObject = null;
			this.MeshFilter = null;
			this.MeshRenderer = null;
			this.MeshCollider = null;
		}

		public void Dispose()
		{
			if (this.Density.IsCreated)
			{
				this.Density.Dispose();
				this.Density = default(NativeArray<byte>);
			}
			if (this.Material.IsCreated)
			{
				this.Material.Dispose();
				this.Material = default(NativeArray<byte>);
			}
			this.DisposeMeshData();
			if (this.Component)
			{
				Object.Destroy(this.Component.gameObject);
			}
		}

		public void DisposeMeshData()
		{
			if (this.VertexData.IsCreated)
			{
				this.VertexData.Dispose();
				this.VertexData = default(NativeArray<MeshVertexData>);
			}
			if (this.TriangleData.IsCreated)
			{
				this.TriangleData.Dispose();
				this.TriangleData = default(NativeArray<ushort>);
			}
			IDisposable disposable = this.GenericMeshData as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
				this.GenericMeshData = null;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int3 GetLocalPosition(int3 voxelPosition)
		{
			return voxelPosition - this.Id * this.Size;
		}

		public override string ToString()
		{
			return string.Format("Chunk ({0}, {1}, {2}) [{3}{4}{5}{6}{7}{8}]", new object[]
			{
				this.Id.x,
				this.Id.y,
				this.Id.z,
				this.IsDataGenerated ? "D" : "_",
				this.IsDataChanged ? "C" : "_",
				this.IsMeshGenerated ? "G" : "_",
				this.IsMeshCreated ? "M" : "_",
				this.IsCollisionBaked ? "B" : "_",
				this.IsMeshAssigned ? "A" : "_"
			});
		}

		public static string GetChunkName(int3 id)
		{
			return string.Format("Chunk_{0}_{1}_{2}", id.x, id.y, id.z);
		}

		public VoxelWorld World;

		public int3 Id;

		public int3 Size;

		public int3 Dimensions;

		public int VoxelCount;

		public NativeArray<byte> Density;

		public NativeArray<byte> Material;

		public NativeArray<MeshVertexData> VertexData;

		public NativeArray<ushort> TriangleData;

		public object GenericMeshData;

		public bool IsDataGenerated;

		public bool IsDataChanged;

		public bool IsMeshGenerated;

		public bool IsMeshCreated;

		public bool IsCollisionBaked;

		public bool IsMeshAssigned;

		public bool IsDirty = true;

		public int VertexCount;

		public Mesh Mesh;
	}
}
