using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine.Rendering.RadeonRays;

namespace UnityEngine.Rendering.UnifiedRayTracing
{
	internal class ComputeRayTracingAccelStruct : IRayTracingAccelStruct, IDisposable
	{
		internal ComputeRayTracingAccelStruct(AccelerationStructureOptions options, RayTracingResources resources, ReferenceCounter counter, int blasBufferInitialSizeBytes = 67108864)
		{
			this.m_CopyShader = resources.copyBuffer;
			this.m_RadeonRaysAPI = new RadeonRaysAPI(new RadeonRaysShaders
			{
				bitHistogram = resources.bitHistogram,
				blockReducePart = resources.blockReducePart,
				blockScan = resources.blockScan,
				buildHlbvh = resources.buildHlbvh,
				restructureBvh = resources.restructureBvh,
				scatter = resources.scatter
			});
			this.m_BuildFlags = options.buildFlags;
			this.m_Blases = new Dictionary<ValueTuple<int, int>, ComputeRayTracingAccelStruct.MeshBlas>();
			int num = blasBufferInitialSizeBytes / RadeonRaysAPI.BvhInternalNodeSizeInBytes();
			this.m_BlasBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, num, RadeonRaysAPI.BvhInternalNodeSizeInBytes());
			this.m_BlasLeavesBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, num, RadeonRaysAPI.BvhLeafNodeSizeInBytes());
			this.m_BlasPositions = new BLASPositionsPool(resources.copyPositions, resources.copyBuffer);
			this.m_BlasAllocator = default(BlockAllocator);
			this.m_BlasAllocator.Initialize(num);
			this.m_BlasLeavesAllocator = default(BlockAllocator);
			this.m_BlasLeavesAllocator.Initialize(num);
			this.m_Counter = counter;
			this.m_Counter.Inc();
		}

		internal GraphicsBuffer topLevelBvhBuffer
		{
			get
			{
				if (this.m_TopLevelAccelStruct == null)
				{
					return null;
				}
				return this.m_TopLevelAccelStruct.GetValueOrDefault().topLevelBvh;
			}
		}

		internal GraphicsBuffer bottomLevelBvhBuffer
		{
			get
			{
				if (this.m_TopLevelAccelStruct == null)
				{
					return null;
				}
				return this.m_TopLevelAccelStruct.GetValueOrDefault().bottomLevelBvhs;
			}
		}

		internal GraphicsBuffer instanceInfoBuffer
		{
			get
			{
				if (this.m_TopLevelAccelStruct == null)
				{
					return null;
				}
				return this.m_TopLevelAccelStruct.GetValueOrDefault().instanceInfos;
			}
		}

		public void Dispose()
		{
			foreach (ComputeRayTracingAccelStruct.MeshBlas meshBlas in this.m_Blases.Values)
			{
				if (meshBlas.buildInfo.triangleIndices != null)
				{
					meshBlas.buildInfo.triangleIndices.Dispose();
				}
			}
			this.m_Counter.Dec();
			this.m_RadeonRaysAPI.Dispose();
			this.m_BlasBuffer.Dispose();
			this.m_BlasLeavesBuffer.Dispose();
			this.m_BlasPositions.Dispose();
			this.m_BlasAllocator.Dispose();
			this.m_BlasLeavesAllocator.Dispose();
			if (this.m_TopLevelAccelStruct == null)
			{
				return;
			}
			this.m_TopLevelAccelStruct.GetValueOrDefault().Dispose();
		}

		public int AddInstance(MeshInstanceDesc meshInstance)
		{
			ComputeRayTracingAccelStruct.MeshBlas orAllocateMeshBlas = this.GetOrAllocateMeshBlas(meshInstance.mesh, meshInstance.subMeshIndex);
			orAllocateMeshBlas.IncRef();
			this.FreeTopLevelAccelStruct();
			int num = this.NewHandle();
			this.m_RadeonInstances.Add(num, new ComputeRayTracingAccelStruct.RadeonRaysInstance
			{
				geomKey = new ValueTuple<int, int>(meshInstance.mesh.GetHashCode(), meshInstance.subMeshIndex),
				blas = orAllocateMeshBlas,
				instanceMask = meshInstance.mask,
				triangleCullingEnabled = meshInstance.enableTriangleCulling,
				invertTriangleCulling = meshInstance.frontTriangleCounterClockwise,
				userInstanceID = (uint)((meshInstance.instanceID == uint.MaxValue) ? num : ((int)meshInstance.instanceID)),
				localToWorldTransform = ComputeRayTracingAccelStruct.ConvertTranform(meshInstance.localToWorldMatrix)
			});
			return num;
		}

		public void RemoveInstance(int instanceHandle)
		{
			this.ReleaseHandle(instanceHandle);
			ComputeRayTracingAccelStruct.RadeonRaysInstance radeonRaysInstance;
			this.m_RadeonInstances.Remove(instanceHandle, out radeonRaysInstance);
			ComputeRayTracingAccelStruct.MeshBlas blas = radeonRaysInstance.blas;
			blas.DecRef();
			if (blas.IsUnreferenced())
			{
				this.DeleteMeshBlas(radeonRaysInstance.geomKey, blas);
			}
			this.FreeTopLevelAccelStruct();
		}

		public void ClearInstances()
		{
			this.m_FreeHandles.Clear();
			this.m_RadeonInstances.Clear();
			foreach (ComputeRayTracingAccelStruct.MeshBlas meshBlas in this.m_Blases.Values)
			{
				if (meshBlas.buildInfo.triangleIndices != null)
				{
					meshBlas.buildInfo.triangleIndices.Dispose();
				}
			}
			this.m_Blases.Clear();
			this.m_BlasPositions.Clear();
			int capacity = this.m_BlasAllocator.capacity;
			this.m_BlasAllocator.Dispose();
			this.m_BlasAllocator = default(BlockAllocator);
			this.m_BlasAllocator.Initialize(capacity);
			capacity = this.m_BlasLeavesAllocator.capacity;
			this.m_BlasLeavesAllocator.Dispose();
			this.m_BlasLeavesAllocator = default(BlockAllocator);
			this.m_BlasLeavesAllocator.Initialize(capacity);
			this.FreeTopLevelAccelStruct();
		}

		public void UpdateInstanceTransform(int instanceHandle, Matrix4x4 localToWorldMatrix)
		{
			this.m_RadeonInstances[instanceHandle].localToWorldTransform = ComputeRayTracingAccelStruct.ConvertTranform(localToWorldMatrix);
			this.FreeTopLevelAccelStruct();
		}

		public void UpdateInstanceID(int instanceHandle, uint instanceID)
		{
			this.m_RadeonInstances[instanceHandle].userInstanceID = instanceID;
			this.FreeTopLevelAccelStruct();
		}

		public void UpdateInstanceMask(int instanceHandle, uint mask)
		{
			this.m_RadeonInstances[instanceHandle].instanceMask = mask;
			this.FreeTopLevelAccelStruct();
		}

		public void Build(CommandBuffer cmd, GraphicsBuffer scratchBuffer)
		{
			ulong buildScratchBufferRequiredSizeInBytes = this.GetBuildScratchBufferRequiredSizeInBytes();
			if (buildScratchBufferRequiredSizeInBytes > 0UL && (scratchBuffer == null || (long)(scratchBuffer.count * scratchBuffer.stride) < (long)buildScratchBufferRequiredSizeInBytes))
			{
				throw new ArgumentException("scratchBuffer size is too small");
			}
			if (buildScratchBufferRequiredSizeInBytes > 0UL && scratchBuffer.stride != 4)
			{
				throw new ArgumentException("scratchBuffer stride must be 4");
			}
			if (this.m_TopLevelAccelStruct != null)
			{
				return;
			}
			this.CreateBvh(cmd, scratchBuffer);
		}

		public ulong GetBuildScratchBufferRequiredSizeInBytes()
		{
			return this.GetBvhBuildScratchBufferSizeInDwords() * 4UL;
		}

		private void FreeTopLevelAccelStruct()
		{
			if (this.m_TopLevelAccelStruct != null)
			{
				this.m_TopLevelAccelStruct.GetValueOrDefault().Dispose();
			}
			this.m_TopLevelAccelStruct = null;
		}

		private ComputeRayTracingAccelStruct.MeshBlas GetOrAllocateMeshBlas(Mesh mesh, int subMeshIndex)
		{
			ComputeRayTracingAccelStruct.MeshBlas meshBlas;
			if (this.m_Blases.TryGetValue(new ValueTuple<int, int>(mesh.GetHashCode(), subMeshIndex), out meshBlas))
			{
				return meshBlas;
			}
			meshBlas = new ComputeRayTracingAccelStruct.MeshBlas();
			this.AllocateBlas(mesh, subMeshIndex, meshBlas);
			this.m_Blases[new ValueTuple<int, int>(mesh.GetHashCode(), subMeshIndex)] = meshBlas;
			return meshBlas;
		}

		private void AllocateBlas(Mesh mesh, int submeshIndex, ComputeRayTracingAccelStruct.MeshBlas blas)
		{
			int num = RadeonRaysAPI.BvhInternalNodeSizeInDwords();
			mesh.indexBufferTarget |= GraphicsBuffer.Target.Raw;
			mesh.vertexBufferTarget |= GraphicsBuffer.Target.Raw;
			SubMeshDescriptor subMesh = mesh.GetSubMesh(submeshIndex);
			int vertexStride;
			int verticesStartOffset;
			using (GraphicsBuffer graphicsBuffer = this.LoadPositionBuffer(mesh, out vertexStride, out verticesStartOffset))
			{
				GraphicsBuffer triangleIndices = this.LoadIndexBuffer(mesh);
				VertexBufferChunk info = default(VertexBufferChunk);
				info.vertices = graphicsBuffer;
				info.verticesStartOffset = verticesStartOffset;
				info.baseVertex = subMesh.baseVertex + subMesh.firstVertex;
				info.vertexCount = (uint)subMesh.vertexCount;
				info.vertexStride = (uint)vertexStride;
				if (!this.m_BlasPositions.Add(info, out blas.blasVertices))
				{
					throw new UnifiedRayTracingException("Can't allocate a GraphicsBuffer bigger than 2GB", UnifiedRayTracingError.OutOfGraphicsBufferMemory);
				}
				MeshBuildInfo meshBuildInfo = new MeshBuildInfo
				{
					vertices = this.m_BlasPositions.VertexBuffer,
					verticesStartOffset = blas.blasVertices.block.offset * 3,
					baseVertex = 0,
					triangleIndices = triangleIndices,
					vertexCount = (uint)blas.blasVertices.block.count,
					triangleCount = (uint)(subMesh.indexCount / 3),
					indicesStartOffset = subMesh.indexStart,
					baseIndex = -subMesh.firstVertex,
					indexFormat = ((mesh.indexFormat == IndexFormat.UInt32) ? IndexFormat.Int32 : IndexFormat.Int16),
					vertexStride = 3U
				};
				blas.buildInfo = meshBuildInfo;
				ulong num2 = this.m_RadeonRaysAPI.GetMeshBuildMemoryRequirements(meshBuildInfo, this.ConvertFlagsToGpuBuild(this.m_BuildFlags)).bvhSizeInDwords / (ulong)((long)num);
				if (num2 > 2147483647UL)
				{
					throw new UnifiedRayTracingException("Can't allocate a GraphicsBuffer bigger than 2GB", UnifiedRayTracingError.OutOfGraphicsBufferMemory);
				}
				blas.bvhAlloc = this.AllocateBlasInternalNodes((int)num2);
				if (!blas.bvhAlloc.valid)
				{
					this.m_BlasPositions.Remove(ref blas.blasVertices);
					throw new UnifiedRayTracingException("Can't allocate a GraphicsBuffer bigger than 2GB", UnifiedRayTracingError.OutOfGraphicsBufferMemory);
				}
				blas.bvhLeavesAlloc = this.AllocateBlasLeafNodes((int)meshBuildInfo.triangleCount);
				if (!blas.bvhLeavesAlloc.valid)
				{
					this.m_BlasPositions.Remove(ref blas.blasVertices);
					this.m_BlasAllocator.FreeAllocation(blas.bvhAlloc);
					throw new UnifiedRayTracingException("Can't allocate a GraphicsBuffer bigger than 2GB", UnifiedRayTracingError.OutOfGraphicsBufferMemory);
				}
			}
		}

		private GraphicsBuffer LoadIndexBuffer(Mesh mesh)
		{
			return mesh.GetIndexBuffer();
		}

		private GraphicsBuffer LoadPositionBuffer(Mesh mesh, out int stride, out int offset)
		{
			VertexAttribute attr = VertexAttribute.Position;
			int vertexAttributeStream = mesh.GetVertexAttributeStream(attr);
			stride = mesh.GetVertexBufferStride(vertexAttributeStream) / 4;
			offset = mesh.GetVertexAttributeOffset(attr) / 4;
			return mesh.GetVertexBuffer(vertexAttributeStream);
		}

		private void DeleteMeshBlas([TupleElementNames(new string[]
		{
			"mesh",
			"subMeshIndex"
		})] ValueTuple<int, int> geomKey, ComputeRayTracingAccelStruct.MeshBlas blas)
		{
			this.m_BlasAllocator.FreeAllocation(blas.bvhAlloc);
			blas.bvhAlloc = BlockAllocator.Allocation.Invalid;
			this.m_BlasLeavesAllocator.FreeAllocation(blas.bvhLeavesAlloc);
			blas.bvhLeavesAlloc = BlockAllocator.Allocation.Invalid;
			this.m_BlasPositions.Remove(ref blas.blasVertices);
			if (blas.buildInfo.triangleIndices != null)
			{
				blas.buildInfo.triangleIndices.Dispose();
			}
			this.m_Blases.Remove(geomKey);
		}

		private ulong GetBvhBuildScratchBufferSizeInDwords()
		{
			RadeonRaysAPI.BvhInternalNodeSizeInDwords();
			ulong num = 0UL;
			foreach (KeyValuePair<ValueTuple<int, int>, ComputeRayTracingAccelStruct.MeshBlas> keyValuePair in this.m_Blases)
			{
				if (!keyValuePair.Value.bvhBuilt)
				{
					MeshBuildMemoryRequirements meshBuildMemoryRequirements = this.m_RadeonRaysAPI.GetMeshBuildMemoryRequirements(keyValuePair.Value.buildInfo, this.ConvertFlagsToGpuBuild(this.m_BuildFlags));
					num = math.max(num, meshBuildMemoryRequirements.buildScratchSizeInDwords);
				}
			}
			ulong buildScratchSizeInDwords = this.m_RadeonRaysAPI.GetSceneBuildMemoryRequirements((uint)this.m_RadeonInstances.Count).buildScratchSizeInDwords;
			num = math.max(num, buildScratchSizeInDwords);
			num = math.max(4UL, num);
			return num;
		}

		private void CreateBvh(CommandBuffer cmd, GraphicsBuffer scratchBuffer)
		{
			this.BuildMissingBottomLevelAccelStructs(cmd, scratchBuffer);
			this.BuildTopLevelAccelStruct(cmd, scratchBuffer);
		}

		private void BuildMissingBottomLevelAccelStructs(CommandBuffer cmd, GraphicsBuffer scratchBuffer)
		{
			foreach (ComputeRayTracingAccelStruct.MeshBlas meshBlas in this.m_Blases.Values)
			{
				if (!meshBlas.bvhBuilt)
				{
					meshBlas.buildInfo.vertices = this.m_BlasPositions.VertexBuffer;
					BottomLevelLevelAccelStruct bottomLevelLevelAccelStruct = new BottomLevelLevelAccelStruct
					{
						bvh = this.m_BlasBuffer,
						bvhOffset = (uint)meshBlas.bvhAlloc.block.offset,
						bvhLeaves = this.m_BlasLeavesBuffer,
						bvhLeavesOffset = (uint)meshBlas.bvhLeavesAlloc.block.offset
					};
					this.m_RadeonRaysAPI.BuildMeshAccelStruct(cmd, meshBlas.buildInfo, this.ConvertFlagsToGpuBuild(this.m_BuildFlags), scratchBuffer, bottomLevelLevelAccelStruct);
					meshBlas.buildInfo.triangleIndices.Dispose();
					meshBlas.buildInfo.triangleIndices = null;
					meshBlas.bvhBuilt = true;
				}
			}
		}

		private void BuildTopLevelAccelStruct(CommandBuffer cmd, GraphicsBuffer scratchBuffer)
		{
			Instance[] array = new Instance[this.m_RadeonInstances.Count];
			int num = 0;
			foreach (ComputeRayTracingAccelStruct.RadeonRaysInstance radeonRaysInstance in this.m_RadeonInstances.Values)
			{
				array[num].meshAccelStructOffset = (uint)radeonRaysInstance.blas.bvhAlloc.block.offset;
				array[num].localToWorldTransform = radeonRaysInstance.localToWorldTransform;
				array[num].instanceMask = radeonRaysInstance.instanceMask;
				array[num].vertexOffset = (uint)(radeonRaysInstance.blas.blasVertices.block.offset * 3);
				array[num].meshAccelStructLeavesOffset = (uint)radeonRaysInstance.blas.bvhLeavesAlloc.block.offset;
				array[num].triangleCullingEnabled = radeonRaysInstance.triangleCullingEnabled;
				array[num].invertTriangleCulling = radeonRaysInstance.invertTriangleCulling;
				array[num].userInstanceID = radeonRaysInstance.userInstanceID;
				num++;
			}
			if (this.m_TopLevelAccelStruct != null)
			{
				this.m_TopLevelAccelStruct.GetValueOrDefault().Dispose();
			}
			this.m_TopLevelAccelStruct = new TopLevelAccelStruct?(this.m_RadeonRaysAPI.BuildSceneAccelStruct(cmd, this.m_BlasBuffer, array, scratchBuffer));
		}

		private BuildFlags ConvertFlagsToGpuBuild(BuildFlags flags)
		{
			if ((flags & BuildFlags.PreferFastBuild) != BuildFlags.None && (flags & BuildFlags.PreferFastTrace) == BuildFlags.None)
			{
				return BuildFlags.PreferFastBuild;
			}
			return BuildFlags.None;
		}

		public void Bind(CommandBuffer cmd, string name, IRayTracingShader shader)
		{
			shader.SetBufferParam(cmd, Shader.PropertyToID(name + "bvh"), this.topLevelBvhBuffer);
			shader.SetBufferParam(cmd, Shader.PropertyToID(name + "bottomBvhs"), this.bottomLevelBvhBuffer);
			shader.SetBufferParam(cmd, Shader.PropertyToID(name + "bottomBvhLeaves"), this.m_BlasLeavesBuffer);
			shader.SetBufferParam(cmd, Shader.PropertyToID(name + "instanceInfos"), this.instanceInfoBuffer);
			shader.SetBufferParam(cmd, Shader.PropertyToID(name + "vertexBuffer"), this.m_BlasPositions.VertexBuffer);
			shader.SetIntParam(cmd, Shader.PropertyToID(name + "vertexStride"), 3);
		}

		private static Transform ConvertTranform(Matrix4x4 input)
		{
			return new Transform
			{
				row0 = input.GetRow(0),
				row1 = input.GetRow(1),
				row2 = input.GetRow(2)
			};
		}

		private static Matrix4x4 ConvertTranform(Transform input)
		{
			Matrix4x4 result = default(Matrix4x4);
			result.SetRow(0, input.row0);
			result.SetRow(1, input.row1);
			result.SetRow(2, input.row2);
			result.SetRow(3, new Vector4(0f, 0f, 0f, 1f));
			return result;
		}

		private static int3 GetFaceIndices(List<int> indices, int triangleIdx)
		{
			return new int3(indices[3 * triangleIdx], indices[3 * triangleIdx + 1], indices[3 * triangleIdx + 2]);
		}

		private static ComputeRayTracingAccelStruct.Triangle GetTriangle(List<Vector3> vertices, int3 idx)
		{
			ComputeRayTracingAccelStruct.Triangle result;
			result.v0 = vertices[idx.x];
			result.v1 = vertices[idx.y];
			result.v2 = vertices[idx.z];
			return result;
		}

		private BlockAllocator.Allocation AllocateBlasInternalNodes(int allocationNodeCount)
		{
			BlockAllocator.Allocation result = this.m_BlasAllocator.Allocate(allocationNodeCount);
			if (!result.valid)
			{
				int oldCapacity;
				int newCapacity;
				result = this.m_BlasAllocator.GrowAndAllocate(allocationNodeCount, int.MaxValue / RadeonRaysAPI.BvhInternalNodeSizeInBytes(), out oldCapacity, out newCapacity);
				if (!result.valid)
				{
					return result;
				}
				GraphicsHelpers.ReallocateBuffer(this.m_CopyShader, oldCapacity, newCapacity, RadeonRaysAPI.BvhInternalNodeSizeInBytes(), ref this.m_BlasBuffer);
			}
			return result;
		}

		private BlockAllocator.Allocation AllocateBlasLeafNodes(int allocationNodeCount)
		{
			BlockAllocator.Allocation result = this.m_BlasLeavesAllocator.Allocate(allocationNodeCount);
			if (!result.valid)
			{
				int oldCapacity;
				int newCapacity;
				result = this.m_BlasLeavesAllocator.GrowAndAllocate(allocationNodeCount, int.MaxValue / RadeonRaysAPI.BvhLeafNodeSizeInBytes(), out oldCapacity, out newCapacity);
				if (!result.valid)
				{
					return result;
				}
				GraphicsHelpers.ReallocateBuffer(this.m_CopyShader, oldCapacity, newCapacity, RadeonRaysAPI.BvhLeafNodeSizeInBytes(), ref this.m_BlasLeavesBuffer);
			}
			return result;
		}

		private int NewHandle()
		{
			if (this.m_FreeHandles.Count != 0)
			{
				return (int)(this.m_FreeHandles.Dequeue() ^ this.m_HandleObfuscation);
			}
			return this.m_RadeonInstances.Count ^ (int)this.m_HandleObfuscation;
		}

		private void ReleaseHandle(int handle)
		{
			this.m_FreeHandles.Enqueue((uint)(handle ^ (int)this.m_HandleObfuscation));
		}

		private readonly uint m_HandleObfuscation = (uint)Random.Range(int.MinValue, int.MaxValue);

		private readonly RadeonRaysAPI m_RadeonRaysAPI;

		private readonly BuildFlags m_BuildFlags;

		private readonly ReferenceCounter m_Counter;

		[TupleElementNames(new string[]
		{
			"mesh",
			"subMeshIndex"
		})]
		private readonly Dictionary<ValueTuple<int, int>, ComputeRayTracingAccelStruct.MeshBlas> m_Blases;

		internal BlockAllocator m_BlasAllocator;

		private GraphicsBuffer m_BlasBuffer;

		internal BlockAllocator m_BlasLeavesAllocator;

		private GraphicsBuffer m_BlasLeavesBuffer;

		private readonly BLASPositionsPool m_BlasPositions;

		private TopLevelAccelStruct? m_TopLevelAccelStruct;

		private readonly ComputeShader m_CopyShader;

		private readonly Dictionary<int, ComputeRayTracingAccelStruct.RadeonRaysInstance> m_RadeonInstances = new Dictionary<int, ComputeRayTracingAccelStruct.RadeonRaysInstance>();

		private readonly Queue<uint> m_FreeHandles = new Queue<uint>();

		private struct Triangle
		{
			public float3 v0;

			public float3 v1;

			public float3 v2;
		}

		private sealed class RadeonRaysInstance
		{
			[TupleElementNames(new string[]
			{
				"mesh",
				"subMeshIndex"
			})]
			public ValueTuple<int, int> geomKey;

			public ComputeRayTracingAccelStruct.MeshBlas blas;

			public uint instanceMask;

			public bool triangleCullingEnabled;

			public bool invertTriangleCulling;

			public uint userInstanceID;

			public Transform localToWorldTransform;
		}

		private sealed class MeshBlas
		{
			public void IncRef()
			{
				this.refCount += 1U;
			}

			public void DecRef()
			{
				this.refCount -= 1U;
			}

			public bool IsUnreferenced()
			{
				return this.refCount == 0U;
			}

			public MeshBuildInfo buildInfo;

			public BlockAllocator.Allocation bvhAlloc;

			public BlockAllocator.Allocation bvhLeavesAlloc;

			public BlockAllocator.Allocation blasVertices;

			public bool bvhBuilt;

			private uint refCount;
		}
	}
}
