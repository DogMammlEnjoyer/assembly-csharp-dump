using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;

namespace UnityEngine.Rendering.UnifiedRayTracing
{
	internal sealed class GeometryPool : IDisposable
	{
		public static int GetVertexByteSize()
		{
			return 32;
		}

		public static int GetIndexByteSize()
		{
			return 4;
		}

		public static int GetMeshChunkTableEntryByteSize()
		{
			return Marshal.SizeOf<GeoPoolMeshChunk>();
		}

		private int GetFormatByteCount(VertexAttributeFormat format)
		{
			switch (format)
			{
			case VertexAttributeFormat.Float32:
				return 4;
			case VertexAttributeFormat.Float16:
				return 2;
			case VertexAttributeFormat.UNorm8:
				return 1;
			case VertexAttributeFormat.SNorm8:
				return 1;
			case VertexAttributeFormat.UNorm16:
				return 2;
			case VertexAttributeFormat.SNorm16:
				return 2;
			case VertexAttributeFormat.UInt8:
				return 1;
			case VertexAttributeFormat.SInt8:
				return 1;
			case VertexAttributeFormat.UInt16:
				return 2;
			case VertexAttributeFormat.SInt16:
				return 2;
			case VertexAttributeFormat.UInt32:
				return 4;
			case VertexAttributeFormat.SInt32:
				return 4;
			default:
				return 4;
			}
		}

		private static int DivUp(int x, int y)
		{
			return (x + y - 1) / y;
		}

		public GraphicsBuffer globalIndexBuffer
		{
			get
			{
				return this.m_GlobalIndexBuffer;
			}
		}

		public GraphicsBuffer globalVertexBuffer
		{
			get
			{
				return this.m_GlobalVertexBuffer;
			}
		}

		public int globalVertexBufferStrideBytes
		{
			get
			{
				return GeometryPool.GetVertexByteSize();
			}
		}

		public GraphicsBuffer globalMeshChunkTableEntryBuffer
		{
			get
			{
				return this.m_GlobalMeshChunkTableEntryBuffer;
			}
		}

		public int indicesCount
		{
			get
			{
				return this.m_MaxIndexCounts;
			}
		}

		public int verticesCount
		{
			get
			{
				return this.m_MaxVertCounts;
			}
		}

		public int meshChunkTablesEntryCount
		{
			get
			{
				return this.m_MaxMeshChunkTableEntriesCount;
			}
		}

		public GeometryPool(in GeometryPoolDesc desc, ComputeShader geometryPoolShader, ComputeShader copyShader)
		{
			this.m_CopyShader = copyShader;
			this.LoadKernels(geometryPoolShader);
			this.m_CmdBuffer = new CommandBuffer();
			this.m_InputBufferReferences = new List<GraphicsBuffer>();
			this.m_MustClearCmdBuffer = false;
			this.m_PendingCmds = 0;
			this.m_MaxVertCounts = this.CalcVertexCount(desc.vertexPoolByteSize);
			this.m_MaxIndexCounts = this.CalcIndexCount(desc.indexPoolByteSize);
			this.m_MaxMeshChunkTableEntriesCount = this.CalcMeshChunkTablesCount(desc.meshChunkTablesByteSize);
			this.m_GlobalVertexBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, GeometryPool.DivUp(this.m_MaxVertCounts * GeometryPool.GetVertexByteSize(), 4), 4);
			this.m_GlobalIndexBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, this.m_MaxIndexCounts, 4);
			this.m_GlobalMeshChunkTableEntryBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, this.m_MaxMeshChunkTableEntriesCount, GeometryPool.GetMeshChunkTableEntryByteSize());
			this.m_DummyBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 16, 4);
			int capacity = 4096;
			this.m_MeshHashToGeoSlot = new NativeParallelHashMap<uint, int>(capacity, Allocator.Persistent);
			this.m_GeoSlots = new List<GeometryPool.GeometrySlot>();
			this.m_FreeGeoSlots = new NativeList<int>(Allocator.Persistent);
			this.m_GeoPoolEntryHashToSlot = new NativeParallelHashMap<uint, GeometryPoolHandle>(capacity, Allocator.Persistent);
			this.m_GeoPoolEntrySlots = new NativeList<GeometryPool.GeoPoolEntrySlot>(Allocator.Persistent);
			this.m_FreeGeoPoolEntrySlots = new NativeList<GeometryPoolHandle>(Allocator.Persistent);
			this.m_VertexAllocator = default(BlockAllocator);
			this.m_VertexAllocator.Initialize(this.m_MaxVertCounts);
			this.m_IndexAllocator = default(BlockAllocator);
			this.m_IndexAllocator.Initialize(this.m_MaxIndexCounts);
			this.m_MeshChunkTableAllocator = default(BlockAllocator);
			this.m_MeshChunkTableAllocator.Initialize(this.m_MaxMeshChunkTableEntriesCount);
		}

		private void DisposeInputBuffers()
		{
			if (this.m_InputBufferReferences.Count == 0)
			{
				return;
			}
			foreach (GraphicsBuffer graphicsBuffer in this.m_InputBufferReferences)
			{
				graphicsBuffer.Dispose();
			}
			this.m_InputBufferReferences.Clear();
		}

		public void Dispose()
		{
			this.m_IndexAllocator.Dispose();
			this.m_VertexAllocator.Dispose();
			this.m_MeshChunkTableAllocator.Dispose();
			this.m_DummyBuffer.Dispose();
			this.m_MeshHashToGeoSlot.Dispose();
			foreach (GeometryPool.GeometrySlot geometrySlot in this.m_GeoSlots)
			{
				if (geometrySlot.valid)
				{
					NativeArray<GeometryPool.MeshChunk> meshChunks = geometrySlot.meshChunks;
					meshChunks.Dispose();
				}
			}
			this.m_GeoSlots = null;
			this.m_FreeGeoSlots.Dispose();
			this.m_GeoPoolEntryHashToSlot.Dispose();
			this.m_GeoPoolEntrySlots.Dispose();
			this.m_FreeGeoPoolEntrySlots.Dispose();
			this.m_GlobalIndexBuffer.Dispose();
			this.m_GlobalVertexBuffer.Release();
			this.m_GlobalMeshChunkTableEntryBuffer.Dispose();
			this.m_CmdBuffer.Release();
			this.DisposeInputBuffers();
		}

		private void LoadKernels(ComputeShader geometryPoolShader)
		{
			this.m_GeometryPoolKernelsCS = geometryPoolShader;
			this.m_KernelMainUpdateIndexBuffer16 = this.m_GeometryPoolKernelsCS.FindKernel("MainUpdateIndexBuffer16");
			this.m_KernelMainUpdateIndexBuffer32 = this.m_GeometryPoolKernelsCS.FindKernel("MainUpdateIndexBuffer32");
			this.m_KernelMainUpdateVertexBuffer = this.m_GeometryPoolKernelsCS.FindKernel("MainUpdateVertexBuffer");
		}

		private int CalcVertexCount(int bufferByteSize)
		{
			return GeometryPool.DivUp(bufferByteSize, GeometryPool.GetVertexByteSize());
		}

		private int CalcIndexCount(int bufferByteSize)
		{
			return GeometryPool.DivUp(bufferByteSize, GeometryPool.GetIndexByteSize());
		}

		private int CalcMeshChunkTablesCount(int bufferByteSize)
		{
			return GeometryPool.DivUp(bufferByteSize, GeometryPool.GetMeshChunkTableEntryByteSize());
		}

		private void DeallocateGeometrySlot(ref GeometryPool.GeometrySlot slot)
		{
			if (slot.meshChunkTableAlloc.valid)
			{
				this.m_MeshChunkTableAllocator.FreeAllocation(slot.meshChunkTableAlloc);
				if (slot.meshChunks.IsCreated)
				{
					for (int i = 0; i < slot.meshChunks.Length; i++)
					{
						GeometryPool.MeshChunk meshChunk = slot.meshChunks[i];
						if (meshChunk.vertexAlloc.valid)
						{
							this.m_VertexAllocator.FreeAllocation(meshChunk.vertexAlloc);
						}
						if (meshChunk.indexAlloc.valid)
						{
							this.m_IndexAllocator.FreeAllocation(meshChunk.indexAlloc);
						}
					}
					slot.meshChunks.Dispose();
				}
			}
			slot = GeometryPool.GeometrySlot.Invalid;
		}

		private void DeallocateGeometrySlot(int geoSlotHandle)
		{
			GeometryPool.GeometrySlot geometrySlot = this.m_GeoSlots[geoSlotHandle];
			geometrySlot.refCount -= 1U;
			if (geometrySlot.refCount == 0U)
			{
				this.m_MeshHashToGeoSlot.Remove(geometrySlot.hash);
				this.DeallocateGeometrySlot(ref geometrySlot);
				this.m_FreeGeoSlots.Add(geoSlotHandle);
			}
			this.m_GeoSlots[geoSlotHandle] = geometrySlot;
		}

		private bool AllocateGeo(Mesh mesh, out int allocationHandle)
		{
			uint hashCode = (uint)mesh.GetHashCode();
			int num = 0;
			for (int i = 0; i < mesh.subMeshCount; i++)
			{
				num += (int)mesh.GetIndexCount(i);
			}
			if (this.m_MeshHashToGeoSlot.TryGetValue(hashCode, out allocationHandle))
			{
				GeometryPool.GeometrySlot value = this.m_GeoSlots[allocationHandle];
				value.refCount += 1U;
				this.m_GeoSlots[allocationHandle] = value;
				return true;
			}
			allocationHandle = -1;
			GeometryPool.GeometrySlot invalid = GeometryPool.GeometrySlot.Invalid;
			invalid.refCount = 1U;
			invalid.hash = hashCode;
			bool flag = true;
			if (mesh.subMeshCount > 0)
			{
				invalid.meshChunkTableAlloc = this.m_MeshChunkTableAllocator.Allocate(mesh.subMeshCount);
				if (!invalid.meshChunkTableAlloc.valid)
				{
					int oldCapacity;
					int num2;
					invalid.meshChunkTableAlloc = this.m_MeshChunkTableAllocator.GrowAndAllocate(mesh.subMeshCount, int.MaxValue / GeometryPool.GetMeshChunkTableEntryByteSize(), out oldCapacity, out num2);
					if (!invalid.meshChunkTableAlloc.valid)
					{
						throw new UnifiedRayTracingException("Can't allocate a GraphicsBuffer bigger than 2GB", UnifiedRayTracingError.OutOfGraphicsBufferMemory);
					}
					GraphicsHelpers.ReallocateBuffer(this.m_CopyShader, oldCapacity, num2, GeometryPool.GetMeshChunkTableEntryByteSize(), ref this.m_GlobalMeshChunkTableEntryBuffer);
					this.m_MaxMeshChunkTableEntriesCount = num2;
				}
				invalid.meshChunks = new NativeArray<GeometryPool.MeshChunk>(mesh.subMeshCount, Allocator.Persistent, NativeArrayOptions.ClearMemory);
				for (int j = 0; j < mesh.subMeshCount; j++)
				{
					SubMeshDescriptor subMesh = mesh.GetSubMesh(j);
					GeometryPool.MeshChunk invalid2 = GeometryPool.MeshChunk.Invalid;
					invalid2.vertexAlloc = this.m_VertexAllocator.Allocate(subMesh.vertexCount);
					if (!invalid2.vertexAlloc.valid)
					{
						int oldCapacity2;
						int num3;
						invalid2.vertexAlloc = this.m_VertexAllocator.GrowAndAllocate(subMesh.vertexCount, int.MaxValue / GeometryPool.GetVertexByteSize(), out oldCapacity2, out num3);
						if (!invalid2.vertexAlloc.valid)
						{
							throw new UnifiedRayTracingException("Can't allocate a GraphicsBuffer bigger than 2GB", UnifiedRayTracingError.OutOfGraphicsBufferMemory);
						}
						GraphicsHelpers.ReallocateBuffer(this.m_CopyShader, oldCapacity2, num3, GeometryPool.GetVertexByteSize(), ref this.m_GlobalVertexBuffer);
						this.m_MaxVertCounts = num3;
					}
					invalid2.indexAlloc = this.m_IndexAllocator.Allocate(subMesh.indexCount);
					if (!invalid2.indexAlloc.valid)
					{
						int oldCapacity3;
						int num4;
						invalid2.indexAlloc = this.m_IndexAllocator.GrowAndAllocate(subMesh.indexCount, 536870911, out oldCapacity3, out num4);
						if (!invalid2.indexAlloc.valid)
						{
							throw new UnifiedRayTracingException("Can't allocate a GraphicsBuffer bigger than 2GB", UnifiedRayTracingError.OutOfGraphicsBufferMemory);
						}
						GraphicsHelpers.ReallocateBuffer(this.m_CopyShader, oldCapacity3, num4, 4, ref this.m_GlobalIndexBuffer);
						this.m_MaxIndexCounts = num4;
					}
					invalid.meshChunks[j] = invalid2;
				}
			}
			if (!flag)
			{
				this.DeallocateGeometrySlot(ref invalid);
				return false;
			}
			if (this.m_FreeGeoSlots.IsEmpty)
			{
				allocationHandle = this.m_GeoSlots.Count;
				this.m_GeoSlots.Add(invalid);
			}
			else
			{
				allocationHandle = this.m_FreeGeoSlots[this.m_FreeGeoSlots.Length - 1];
				this.m_FreeGeoSlots.RemoveAtSwapBack(this.m_FreeGeoSlots.Length - 1);
				this.m_GeoSlots[allocationHandle] = invalid;
			}
			this.m_MeshHashToGeoSlot.Add(invalid.hash, allocationHandle);
			return true;
		}

		private void DeallocateGeoPoolEntrySlot(GeometryPoolHandle handle)
		{
			GeometryPool.GeoPoolEntrySlot geoPoolEntrySlot = this.m_GeoPoolEntrySlots[handle.index];
			geoPoolEntrySlot.refCount -= 1U;
			if (geoPoolEntrySlot.refCount == 0U)
			{
				this.m_GeoPoolEntryHashToSlot.Remove(geoPoolEntrySlot.hash);
				this.DeallocateGeoPoolEntrySlot(ref geoPoolEntrySlot);
				this.m_FreeGeoPoolEntrySlots.Add(handle);
			}
			this.m_GeoPoolEntrySlots[handle.index] = geoPoolEntrySlot;
		}

		private void DeallocateGeoPoolEntrySlot(ref GeometryPool.GeoPoolEntrySlot geoPoolEntrySlot)
		{
			if (geoPoolEntrySlot.geoSlotHandle != -1)
			{
				this.DeallocateGeometrySlot(geoPoolEntrySlot.geoSlotHandle);
			}
			geoPoolEntrySlot = GeometryPool.GeoPoolEntrySlot.Invalid;
		}

		public GeometryPoolEntryInfo GetEntryInfo(GeometryPoolHandle handle)
		{
			if (!handle.valid)
			{
				return GeometryPoolEntryInfo.NewDefault();
			}
			GeometryPool.GeoPoolEntrySlot geoPoolEntrySlot = this.m_GeoPoolEntrySlots[handle.index];
			if (!geoPoolEntrySlot.valid)
			{
				return GeometryPoolEntryInfo.NewDefault();
			}
			if (geoPoolEntrySlot.geoSlotHandle == -1)
			{
				Debug.LogErrorFormat("Found invalid geometry slot handle with handle id {0}.", new object[]
				{
					handle.index
				});
			}
			return new GeometryPoolEntryInfo
			{
				valid = geoPoolEntrySlot.valid,
				refCount = geoPoolEntrySlot.refCount
			};
		}

		public GeometryPool.GeometrySlot GetEntryGeomAllocation(GeometryPoolHandle handle)
		{
			GeometryPool.GeoPoolEntrySlot geoPoolEntrySlot = this.m_GeoPoolEntrySlots[handle.index];
			return this.m_GeoSlots[geoPoolEntrySlot.geoSlotHandle];
		}

		public int GetInstanceGeometryIndex(Mesh mesh)
		{
			return this.GetEntryGeomAllocation(this.GetHandle(mesh)).meshChunkTableAlloc.block.offset;
		}

		private void UpdateGeoGpuState(Mesh mesh, GeometryPoolHandle handle)
		{
			GeometryPool.GeoPoolEntrySlot geoPoolEntrySlot = this.m_GeoPoolEntrySlots[handle.index];
			GeometryPool.GeometrySlot geometrySlot = this.m_GeoSlots[geoPoolEntrySlot.geoSlotHandle];
			CommandBuffer commandBuffer = this.AllocateCommandBuffer();
			if (!geometrySlot.hasGPUData)
			{
				GraphicsBuffer graphicsBuffer = this.LoadIndexBuffer(mesh);
				GeometryPool.VertexBufferAttribInfo vertexBufferAttribInfo;
				this.LoadVertexAttribInfo(mesh, VertexAttribute.Position, out vertexBufferAttribInfo);
				GeometryPool.VertexBufferAttribInfo vertexBufferAttribInfo2;
				this.LoadVertexAttribInfo(mesh, VertexAttribute.TexCoord0, out vertexBufferAttribInfo2);
				GeometryPool.VertexBufferAttribInfo vertexBufferAttribInfo3;
				this.LoadVertexAttribInfo(mesh, VertexAttribute.TexCoord1, out vertexBufferAttribInfo3);
				GeometryPool.VertexBufferAttribInfo vertexBufferAttribInfo4;
				this.LoadVertexAttribInfo(mesh, VertexAttribute.Normal, out vertexBufferAttribInfo4);
				NativeArray<GeoPoolMeshChunk> data = new NativeArray<GeoPoolMeshChunk>(geometrySlot.meshChunks.Length, Allocator.Temp, NativeArrayOptions.ClearMemory);
				for (int i = 0; i < mesh.subMeshCount; i++)
				{
					SubMeshDescriptor subMesh = mesh.GetSubMesh(i);
					GeometryPool.MeshChunk meshChunk = geometrySlot.meshChunks[i];
					this.AddVertexUpdateCommand(commandBuffer, subMesh.baseVertex + subMesh.firstVertex, vertexBufferAttribInfo, vertexBufferAttribInfo2, vertexBufferAttribInfo3, vertexBufferAttribInfo4, meshChunk.vertexAlloc, this.m_GlobalVertexBuffer);
					this.AddIndexUpdateCommand(commandBuffer, mesh.indexFormat, graphicsBuffer, meshChunk.indexAlloc, subMesh.firstVertex, subMesh.indexStart, subMesh.indexCount, 0, this.m_GlobalIndexBuffer);
					data[i] = meshChunk.EncodeGPUEntry();
				}
				commandBuffer.SetBufferData<GeoPoolMeshChunk>(this.m_GlobalMeshChunkTableEntryBuffer, data, 0, geometrySlot.meshChunkTableAlloc.block.offset, data.Length);
				data.Dispose();
				geometrySlot.hasGPUData = true;
				this.m_GeoSlots[geoPoolEntrySlot.geoSlotHandle] = geometrySlot;
			}
		}

		private uint FNVHash(uint prevHash, uint dword)
		{
			for (int i = 0; i < 4; i++)
			{
				prevHash ^= (dword >> i * 8 & 255U);
				prevHash *= 2166136261U;
			}
			return prevHash;
		}

		private uint CalculateClusterHash(Mesh mesh, GeometryPoolSubmeshData[] submeshData)
		{
			uint num = (uint)mesh.GetHashCode();
			if (submeshData != null)
			{
				foreach (GeometryPoolSubmeshData geometryPoolSubmeshData in submeshData)
				{
					num = this.FNVHash(num, (uint)geometryPoolSubmeshData.submeshIndex);
					num = this.FNVHash(num, (uint)((geometryPoolSubmeshData.material == null) ? 0 : geometryPoolSubmeshData.material.GetHashCode()));
				}
			}
			return num;
		}

		public GeometryPoolHandle GetHandle(Mesh mesh)
		{
			uint key = this.CalculateClusterHash(mesh, null);
			GeometryPoolHandle result;
			if (this.m_GeoPoolEntryHashToSlot.TryGetValue(key, out result))
			{
				return result;
			}
			return GeometryPoolHandle.Invalid;
		}

		private static int FindSubmeshEntryInDesc(int submeshIndex, in GeometryPoolSubmeshData[] submeshData)
		{
			if (submeshData == null)
			{
				return -1;
			}
			for (int i = 0; i < submeshData.Length; i++)
			{
				if (submeshData[i].submeshIndex == submeshIndex)
				{
					return i;
				}
			}
			return -1;
		}

		public bool Register(Mesh mesh, out GeometryPoolHandle outHandle)
		{
			GeometryPoolEntryDesc geometryPoolEntryDesc = default(GeometryPoolEntryDesc);
			geometryPoolEntryDesc.mesh = mesh;
			geometryPoolEntryDesc.submeshData = null;
			return this.Register(geometryPoolEntryDesc, out outHandle);
		}

		public bool Register(in GeometryPoolEntryDesc entryDesc, out GeometryPoolHandle outHandle)
		{
			outHandle = GeometryPoolHandle.Invalid;
			if (entryDesc.mesh == null)
			{
				return false;
			}
			Mesh mesh = entryDesc.mesh;
			uint num = this.CalculateClusterHash(entryDesc.mesh, entryDesc.submeshData);
			if (this.m_GeoPoolEntryHashToSlot.TryGetValue(num, out outHandle))
			{
				GeometryPool.GeoPoolEntrySlot geoPoolEntrySlot = this.m_GeoPoolEntrySlots[outHandle.index];
				GeometryPool.GeometrySlot geometrySlot = this.m_GeoSlots[geoPoolEntrySlot.geoSlotHandle];
				geoPoolEntrySlot.refCount += 1U;
				this.m_GeoPoolEntrySlots[outHandle.index] = geoPoolEntrySlot;
				return true;
			}
			GeometryPool.GeoPoolEntrySlot invalid = GeometryPool.GeoPoolEntrySlot.Invalid;
			invalid.refCount = 1U;
			invalid.hash = num;
			List<GeometryPoolSubmeshData> list = new List<GeometryPoolSubmeshData>(mesh.subMeshCount);
			if (mesh.subMeshCount > 0 && entryDesc.submeshData != null)
			{
				for (int i = 0; i < mesh.subMeshCount; i++)
				{
					int num2 = GeometryPool.FindSubmeshEntryInDesc(i, entryDesc.submeshData);
					if (num2 == -1)
					{
						Debug.LogErrorFormat("Could not find submesh index {0} for mesh entry descriptor of mesh {1}.", new object[]
						{
							i,
							mesh.name
						});
					}
					else
					{
						list.Add(entryDesc.submeshData[num2]);
					}
				}
			}
			if (!this.AllocateGeo(mesh, out invalid.geoSlotHandle))
			{
				this.DeallocateGeoPoolEntrySlot(ref invalid);
				return false;
			}
			if (this.m_FreeGeoPoolEntrySlots.IsEmpty)
			{
				outHandle = new GeometryPoolHandle
				{
					index = this.m_GeoPoolEntrySlots.Length
				};
				this.m_GeoPoolEntrySlots.Add(invalid);
			}
			else
			{
				outHandle = this.m_FreeGeoPoolEntrySlots[this.m_FreeGeoPoolEntrySlots.Length - 1];
				this.m_FreeGeoPoolEntrySlots.RemoveAtSwapBack(this.m_FreeGeoPoolEntrySlots.Length - 1);
				this.m_GeoPoolEntrySlots[outHandle.index] = invalid;
			}
			this.m_GeoPoolEntryHashToSlot.Add(invalid.hash, outHandle);
			this.UpdateGeoGpuState(mesh, outHandle);
			return true;
		}

		public void Unregister(GeometryPoolHandle handle)
		{
			GeometryPool.GeoPoolEntrySlot geoPoolEntrySlot = this.m_GeoPoolEntrySlots[handle.index];
			this.DeallocateGeoPoolEntrySlot(handle);
		}

		public void SendGpuCommands()
		{
			if (this.m_PendingCmds != 0)
			{
				Graphics.ExecuteCommandBuffer(this.m_CmdBuffer);
				this.m_MustClearCmdBuffer = true;
				this.m_PendingCmds = 0;
			}
			this.DisposeInputBuffers();
		}

		private GraphicsBuffer LoadIndexBuffer(Mesh mesh)
		{
			mesh.indexBufferTarget |= GraphicsBuffer.Target.Raw;
			mesh.vertexBufferTarget |= GraphicsBuffer.Target.Raw;
			GraphicsBuffer indexBuffer = mesh.GetIndexBuffer();
			this.m_InputBufferReferences.Add(indexBuffer);
			return indexBuffer;
		}

		private void LoadVertexAttribInfo(Mesh mesh, VertexAttribute attribute, out GeometryPool.VertexBufferAttribInfo output)
		{
			if (!mesh.HasVertexAttribute(attribute))
			{
				output.buffer = null;
				output.stride = (output.offset = (output.byteCount = 0));
				return;
			}
			int vertexAttributeStream = mesh.GetVertexAttributeStream(attribute);
			output.stride = mesh.GetVertexBufferStride(vertexAttributeStream);
			output.offset = mesh.GetVertexAttributeOffset(attribute);
			output.byteCount = this.GetFormatByteCount(mesh.GetVertexAttributeFormat(attribute)) * mesh.GetVertexAttributeDimension(attribute);
			output.buffer = mesh.GetVertexBuffer(vertexAttributeStream);
			this.m_InputBufferReferences.Add(output.buffer);
		}

		private CommandBuffer AllocateCommandBuffer()
		{
			if (this.m_MustClearCmdBuffer)
			{
				this.m_CmdBuffer.Clear();
				this.m_MustClearCmdBuffer = false;
			}
			this.m_PendingCmds++;
			return this.m_CmdBuffer;
		}

		private void AddIndexUpdateCommand(CommandBuffer cmdBuffer, IndexFormat inputFormat, in GraphicsBuffer inputBuffer, in BlockAllocator.Allocation location, int firstVertex, int inputOffset, int indexCount, int outputOffset, GraphicsBuffer outputIdxBuffer)
		{
			if (location.block.count == 0)
			{
				return;
			}
			cmdBuffer.SetComputeIntParam(this.m_GeometryPoolKernelsCS, GeometryPool.GeoPoolShaderIDs._InputIBBaseOffset, inputOffset);
			cmdBuffer.SetComputeIntParam(this.m_GeometryPoolKernelsCS, GeometryPool.GeoPoolShaderIDs._InputIBCount, indexCount);
			cmdBuffer.SetComputeIntParam(this.m_GeometryPoolKernelsCS, GeometryPool.GeoPoolShaderIDs._InputFirstVertex, firstVertex);
			cmdBuffer.SetComputeIntParam(this.m_GeometryPoolKernelsCS, GeometryPool.GeoPoolShaderIDs._OutputIBOffset, location.block.offset + outputOffset);
			int kernelIndex = (inputFormat == IndexFormat.UInt16) ? this.m_KernelMainUpdateIndexBuffer16 : this.m_KernelMainUpdateIndexBuffer32;
			cmdBuffer.SetComputeBufferParam(this.m_GeometryPoolKernelsCS, kernelIndex, GeometryPool.GeoPoolShaderIDs._InputIndexBuffer, inputBuffer);
			cmdBuffer.SetComputeBufferParam(this.m_GeometryPoolKernelsCS, kernelIndex, GeometryPool.GeoPoolShaderIDs._OutputIndexBuffer, outputIdxBuffer);
			int num = GeometryPool.DivUp(location.block.count, 256);
			int num2 = GeometryPool.DivUp(num, 65535);
			for (int i = 0; i < num2; i++)
			{
				int val = i * 65535 * 256;
				int threadGroupsX = Math.Min(65535, num - i * 65535);
				cmdBuffer.SetComputeIntParam(this.m_GeometryPoolKernelsCS, GeometryPool.GeoPoolShaderIDs._DispatchIndexOffset, val);
				cmdBuffer.DispatchCompute(this.m_GeometryPoolKernelsCS, kernelIndex, threadGroupsX, 1, 1);
			}
		}

		private void AddVertexUpdateCommand(CommandBuffer cmdBuffer, int baseVertexOffset, in GeometryPool.VertexBufferAttribInfo pos, in GeometryPool.VertexBufferAttribInfo uv0, in GeometryPool.VertexBufferAttribInfo uv1, in GeometryPool.VertexBufferAttribInfo n, in BlockAllocator.Allocation location, GraphicsBuffer outputVertexBuffer)
		{
			if (location.block.count == 0)
			{
				return;
			}
			GeoPoolVertexAttribs geoPoolVertexAttribs = (GeoPoolVertexAttribs)0;
			GeometryPool.VertexBufferAttribInfo vertexBufferAttribInfo = pos;
			if (vertexBufferAttribInfo.valid)
			{
				geoPoolVertexAttribs |= GeoPoolVertexAttribs.Position;
			}
			vertexBufferAttribInfo = uv0;
			if (vertexBufferAttribInfo.valid)
			{
				geoPoolVertexAttribs |= GeoPoolVertexAttribs.Uv0;
			}
			vertexBufferAttribInfo = uv1;
			if (vertexBufferAttribInfo.valid)
			{
				geoPoolVertexAttribs |= GeoPoolVertexAttribs.Uv1;
			}
			vertexBufferAttribInfo = n;
			if (vertexBufferAttribInfo.valid)
			{
				geoPoolVertexAttribs |= GeoPoolVertexAttribs.Normal;
			}
			int count = location.block.count;
			cmdBuffer.SetComputeIntParam(this.m_GeometryPoolKernelsCS, GeometryPool.GeoPoolShaderIDs._InputVBCount, count);
			cmdBuffer.SetComputeIntParam(this.m_GeometryPoolKernelsCS, GeometryPool.GeoPoolShaderIDs._InputBaseVertexOffset, baseVertexOffset);
			cmdBuffer.SetComputeIntParam(this.m_GeometryPoolKernelsCS, GeometryPool.GeoPoolShaderIDs._OutputVBSize, this.m_MaxVertCounts);
			cmdBuffer.SetComputeIntParam(this.m_GeometryPoolKernelsCS, GeometryPool.GeoPoolShaderIDs._OutputVBOffset, location.block.offset);
			cmdBuffer.SetComputeIntParam(this.m_GeometryPoolKernelsCS, GeometryPool.GeoPoolShaderIDs._InputPosBufferStride, pos.stride);
			cmdBuffer.SetComputeIntParam(this.m_GeometryPoolKernelsCS, GeometryPool.GeoPoolShaderIDs._InputPosBufferOffset, pos.offset);
			cmdBuffer.SetComputeIntParam(this.m_GeometryPoolKernelsCS, GeometryPool.GeoPoolShaderIDs._InputUv0BufferStride, uv0.stride);
			cmdBuffer.SetComputeIntParam(this.m_GeometryPoolKernelsCS, GeometryPool.GeoPoolShaderIDs._InputUv0BufferOffset, uv0.offset);
			cmdBuffer.SetComputeIntParam(this.m_GeometryPoolKernelsCS, GeometryPool.GeoPoolShaderIDs._InputUv1BufferStride, uv1.stride);
			cmdBuffer.SetComputeIntParam(this.m_GeometryPoolKernelsCS, GeometryPool.GeoPoolShaderIDs._InputUv1BufferOffset, uv1.offset);
			cmdBuffer.SetComputeIntParam(this.m_GeometryPoolKernelsCS, GeometryPool.GeoPoolShaderIDs._InputNormalBufferStride, n.stride);
			cmdBuffer.SetComputeIntParam(this.m_GeometryPoolKernelsCS, GeometryPool.GeoPoolShaderIDs._InputNormalBufferOffset, n.offset);
			cmdBuffer.SetComputeIntParam(this.m_GeometryPoolKernelsCS, GeometryPool.GeoPoolShaderIDs._AttributesMask, (int)geoPoolVertexAttribs);
			int kernelMainUpdateVertexBuffer = this.m_KernelMainUpdateVertexBuffer;
			ComputeShader geometryPoolKernelsCS = this.m_GeometryPoolKernelsCS;
			int kernelIndex = kernelMainUpdateVertexBuffer;
			int posBuffer = GeometryPool.GeoPoolShaderIDs._PosBuffer;
			vertexBufferAttribInfo = pos;
			cmdBuffer.SetComputeBufferParam(geometryPoolKernelsCS, kernelIndex, posBuffer, vertexBufferAttribInfo.valid ? pos.buffer : this.m_DummyBuffer);
			ComputeShader geometryPoolKernelsCS2 = this.m_GeometryPoolKernelsCS;
			int kernelIndex2 = kernelMainUpdateVertexBuffer;
			int uv0Buffer = GeometryPool.GeoPoolShaderIDs._Uv0Buffer;
			vertexBufferAttribInfo = uv0;
			cmdBuffer.SetComputeBufferParam(geometryPoolKernelsCS2, kernelIndex2, uv0Buffer, vertexBufferAttribInfo.valid ? uv0.buffer : this.m_DummyBuffer);
			ComputeShader geometryPoolKernelsCS3 = this.m_GeometryPoolKernelsCS;
			int kernelIndex3 = kernelMainUpdateVertexBuffer;
			int uv1Buffer = GeometryPool.GeoPoolShaderIDs._Uv1Buffer;
			vertexBufferAttribInfo = uv1;
			cmdBuffer.SetComputeBufferParam(geometryPoolKernelsCS3, kernelIndex3, uv1Buffer, vertexBufferAttribInfo.valid ? uv1.buffer : this.m_DummyBuffer);
			ComputeShader geometryPoolKernelsCS4 = this.m_GeometryPoolKernelsCS;
			int kernelIndex4 = kernelMainUpdateVertexBuffer;
			int normalBuffer = GeometryPool.GeoPoolShaderIDs._NormalBuffer;
			vertexBufferAttribInfo = n;
			cmdBuffer.SetComputeBufferParam(geometryPoolKernelsCS4, kernelIndex4, normalBuffer, vertexBufferAttribInfo.valid ? n.buffer : this.m_DummyBuffer);
			cmdBuffer.SetComputeBufferParam(this.m_GeometryPoolKernelsCS, kernelMainUpdateVertexBuffer, GeometryPool.GeoPoolShaderIDs._OutputVB, outputVertexBuffer);
			int num = GeometryPool.DivUp(count, 256);
			int num2 = GeometryPool.DivUp(num, 65535);
			for (int i = 0; i < num2; i++)
			{
				int val = i * 65535 * 256;
				int threadGroupsX = Math.Min(65535, num - i * 65535);
				cmdBuffer.SetComputeIntParam(this.m_GeometryPoolKernelsCS, GeometryPool.GeoPoolShaderIDs._DispatchVertexOffset, val);
				cmdBuffer.DispatchCompute(this.m_GeometryPoolKernelsCS, kernelMainUpdateVertexBuffer, threadGroupsX, 1, 1);
			}
		}

		private const int kMaxThreadGroupsPerDispatch = 65535;

		private const int kThreadGroupSize = 256;

		private const int InvalidHandle = -1;

		private const GraphicsBuffer.Target VertexBufferTarget = GraphicsBuffer.Target.Structured;

		private const GraphicsBuffer.Target IndexBufferTarget = GraphicsBuffer.Target.Structured;

		private GraphicsBuffer m_GlobalIndexBuffer;

		private GraphicsBuffer m_GlobalVertexBuffer;

		private GraphicsBuffer m_GlobalMeshChunkTableEntryBuffer;

		private readonly GraphicsBuffer m_DummyBuffer;

		private int m_MaxVertCounts;

		private int m_MaxIndexCounts;

		private int m_MaxMeshChunkTableEntriesCount;

		private BlockAllocator m_VertexAllocator;

		private BlockAllocator m_IndexAllocator;

		private BlockAllocator m_MeshChunkTableAllocator;

		private NativeParallelHashMap<uint, int> m_MeshHashToGeoSlot;

		private List<GeometryPool.GeometrySlot> m_GeoSlots;

		private NativeList<int> m_FreeGeoSlots;

		private NativeParallelHashMap<uint, GeometryPoolHandle> m_GeoPoolEntryHashToSlot;

		private NativeList<GeometryPool.GeoPoolEntrySlot> m_GeoPoolEntrySlots;

		private NativeList<GeometryPoolHandle> m_FreeGeoPoolEntrySlots;

		private readonly List<GraphicsBuffer> m_InputBufferReferences;

		private readonly ComputeShader m_CopyShader;

		private ComputeShader m_GeometryPoolKernelsCS;

		private int m_KernelMainUpdateIndexBuffer16;

		private int m_KernelMainUpdateIndexBuffer32;

		private int m_KernelMainUpdateVertexBuffer;

		private readonly CommandBuffer m_CmdBuffer;

		private bool m_MustClearCmdBuffer;

		private int m_PendingCmds;

		private static class GeoPoolShaderIDs
		{
			public static readonly int _InputIBBaseOffset = Shader.PropertyToID("_InputIBBaseOffset");

			public static readonly int _DispatchIndexOffset = Shader.PropertyToID("_DispatchIndexOffset");

			public static readonly int _InputIBCount = Shader.PropertyToID("_InputIBCount");

			public static readonly int _OutputIBOffset = Shader.PropertyToID("_OutputIBOffset");

			public static readonly int _InputFirstVertex = Shader.PropertyToID("_InputFirstVertex");

			public static readonly int _InputIndexBuffer = Shader.PropertyToID("_InputIndexBuffer");

			public static readonly int _OutputIndexBuffer = Shader.PropertyToID("_OutputIndexBuffer");

			public static readonly int _InputVBCount = Shader.PropertyToID("_InputVBCount");

			public static readonly int _InputBaseVertexOffset = Shader.PropertyToID("_InputBaseVertexOffset");

			public static readonly int _DispatchVertexOffset = Shader.PropertyToID("_DispatchVertexOffset");

			public static readonly int _OutputVBSize = Shader.PropertyToID("_OutputVBSize");

			public static readonly int _OutputVBOffset = Shader.PropertyToID("_OutputVBOffset");

			public static readonly int _InputPosBufferStride = Shader.PropertyToID("_InputPosBufferStride");

			public static readonly int _InputPosBufferOffset = Shader.PropertyToID("_InputPosBufferOffset");

			public static readonly int _InputUv0BufferStride = Shader.PropertyToID("_InputUv0BufferStride");

			public static readonly int _InputUv0BufferOffset = Shader.PropertyToID("_InputUv0BufferOffset");

			public static readonly int _InputUv1BufferStride = Shader.PropertyToID("_InputUv1BufferStride");

			public static readonly int _InputUv1BufferOffset = Shader.PropertyToID("_InputUv1BufferOffset");

			public static readonly int _InputNormalBufferStride = Shader.PropertyToID("_InputNormalBufferStride");

			public static readonly int _InputNormalBufferOffset = Shader.PropertyToID("_InputNormalBufferOffset");

			public static readonly int _PosBuffer = Shader.PropertyToID("_PosBuffer");

			public static readonly int _Uv0Buffer = Shader.PropertyToID("_Uv0Buffer");

			public static readonly int _Uv1Buffer = Shader.PropertyToID("_Uv1Buffer");

			public static readonly int _NormalBuffer = Shader.PropertyToID("_NormalBuffer");

			public static readonly int _OutputVB = Shader.PropertyToID("_OutputVB");

			public static readonly int _AttributesMask = Shader.PropertyToID("_AttributesMask");
		}

		public struct MeshChunk
		{
			public GeoPoolMeshChunk EncodeGPUEntry()
			{
				return new GeoPoolMeshChunk
				{
					indexOffset = this.indexAlloc.block.offset,
					indexCount = this.indexAlloc.block.count,
					vertexOffset = this.vertexAlloc.block.offset,
					vertexCount = this.vertexAlloc.block.count
				};
			}

			public static GeometryPool.MeshChunk Invalid
			{
				get
				{
					return new GeometryPool.MeshChunk
					{
						vertexAlloc = BlockAllocator.Allocation.Invalid,
						indexAlloc = BlockAllocator.Allocation.Invalid
					};
				}
			}

			public BlockAllocator.Allocation vertexAlloc;

			public BlockAllocator.Allocation indexAlloc;
		}

		public struct GeometrySlot
		{
			public bool valid
			{
				get
				{
					return this.meshChunkTableAlloc.valid;
				}
			}

			public uint refCount;

			public uint hash;

			public BlockAllocator.Allocation meshChunkTableAlloc;

			public NativeArray<GeometryPool.MeshChunk> meshChunks;

			public bool hasGPUData;

			public static readonly GeometryPool.GeometrySlot Invalid = new GeometryPool.GeometrySlot
			{
				meshChunkTableAlloc = BlockAllocator.Allocation.Invalid,
				hasGPUData = false
			};
		}

		private struct GeoPoolEntrySlot
		{
			public bool valid
			{
				get
				{
					return this.geoSlotHandle != -1;
				}
			}

			public uint refCount;

			public uint hash;

			public int geoSlotHandle;

			public static readonly GeometryPool.GeoPoolEntrySlot Invalid = new GeometryPool.GeoPoolEntrySlot
			{
				refCount = 0U,
				hash = 0U,
				geoSlotHandle = -1
			};
		}

		private struct VertexBufferAttribInfo
		{
			public bool valid
			{
				get
				{
					return this.buffer != null;
				}
			}

			public GraphicsBuffer buffer;

			public int stride;

			public int offset;

			public int byteCount;
		}
	}
}
