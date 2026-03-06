using System;
using System.Collections.Generic;
using Unity.Mathematics;

namespace UnityEngine.Rendering.UnifiedRayTracing
{
	internal sealed class AccelStructInstances : IDisposable
	{
		internal AccelStructInstances(GeometryPool geometryPool)
		{
			this.m_GeometryPool = geometryPool;
		}

		public void Dispose()
		{
			foreach (AccelStructInstances.InstanceEntry instanceEntry in this.m_Instances.Values)
			{
				GeometryPoolHandle geometryPoolHandle = instanceEntry.geometryPoolHandle;
				this.m_GeometryPool.Unregister(geometryPoolHandle);
			}
			this.m_GeometryPool.SendGpuCommands();
			PersistentGpuArray<AccelStructInstances.RTInstance> instanceBuffer = this.m_InstanceBuffer;
			if (instanceBuffer != null)
			{
				instanceBuffer.Dispose();
			}
			this.m_GeometryPool.Dispose();
		}

		public PersistentGpuArray<AccelStructInstances.RTInstance> instanceBuffer
		{
			get
			{
				return this.m_InstanceBuffer;
			}
		}

		public IReadOnlyCollection<AccelStructInstances.InstanceEntry> instances
		{
			get
			{
				return this.m_Instances.Values;
			}
		}

		public GeometryPool geometryPool
		{
			get
			{
				return this.m_GeometryPool;
			}
		}

		public int AddInstance(MeshInstanceDesc meshInstance, uint materialID, uint renderingLayerMask)
		{
			BlockAllocator.Allocation allocation = this.m_InstanceBuffer.Add(1)[0];
			this.AddInstance(allocation, meshInstance, materialID, renderingLayerMask);
			return allocation.block.offset;
		}

		public unsafe int AddInstances(Span<MeshInstanceDesc> meshInstances, Span<uint> materialIDs, Span<uint> renderingLayerMask)
		{
			BlockAllocator.Allocation[] array = this.m_InstanceBuffer.Add(meshInstances.Length);
			for (int i = 0; i < meshInstances.Length; i++)
			{
				this.AddInstance(array[i], meshInstances[i], *materialIDs[i], *renderingLayerMask[i]);
			}
			return array[0].block.offset;
		}

		private void AddInstance(BlockAllocator.Allocation slotAllocation, in MeshInstanceDesc meshInstance, uint materialID, uint renderingLayerMask)
		{
			GeometryPoolHandle geometryPoolHandle;
			if (!this.m_GeometryPool.Register(meshInstance.mesh, out geometryPoolHandle))
			{
				throw new InvalidOperationException("Failed to allocate geometry data for instance");
			}
			this.m_GeometryPool.SendGpuCommands();
			this.m_InstanceBuffer.Set(slotAllocation, new AccelStructInstances.RTInstance
			{
				localToWorld = meshInstance.localToWorldMatrix,
				localToWorldNormals = AccelStructInstances.NormalMatrix(meshInstance.localToWorldMatrix),
				previousLocalToWorld = meshInstance.localToWorldMatrix,
				userMaterialID = materialID,
				instanceMask = meshInstance.mask,
				renderingLayerMask = renderingLayerMask,
				geometryIndex = (uint)(this.m_GeometryPool.GetEntryGeomAllocation(geometryPoolHandle).meshChunkTableAlloc.block.offset + meshInstance.subMeshIndex)
			});
			GeometryPool.MeshChunk meshChunk = this.m_GeometryPool.GetEntryGeomAllocation(geometryPoolHandle).meshChunks[meshInstance.subMeshIndex];
			AccelStructInstances.InstanceEntry value = new AccelStructInstances.InstanceEntry
			{
				geometryPoolHandle = geometryPoolHandle,
				indexInInstanceBuffer = slotAllocation,
				instanceMask = meshInstance.mask,
				vertexOffset = (uint)(meshChunk.vertexAlloc.block.offset * (GeometryPool.GetVertexByteSize() / 4)),
				indexOffset = (uint)meshChunk.indexAlloc.block.offset
			};
			this.m_Instances.Add(slotAllocation.block.offset, value);
		}

		public GeometryPool.MeshChunk GetEntryGeomAllocation(GeometryPoolHandle handle, int submeshIndex)
		{
			return this.m_GeometryPool.GetEntryGeomAllocation(handle).meshChunks[submeshIndex];
		}

		public GraphicsBuffer indexBuffer
		{
			get
			{
				return this.m_GeometryPool.globalIndexBuffer;
			}
		}

		public GraphicsBuffer vertexBuffer
		{
			get
			{
				return this.m_GeometryPool.globalVertexBuffer;
			}
		}

		public void RemoveInstance(int instanceHandle)
		{
			AccelStructInstances.InstanceEntry instanceEntry;
			this.m_Instances.TryGetValue(instanceHandle, out instanceEntry);
			this.m_Instances.Remove(instanceHandle);
			this.m_InstanceBuffer.Remove(instanceEntry.indexInInstanceBuffer);
			GeometryPoolHandle geometryPoolHandle = instanceEntry.geometryPoolHandle;
			this.m_GeometryPool.Unregister(geometryPoolHandle);
			this.m_GeometryPool.SendGpuCommands();
		}

		public void ClearInstances()
		{
			foreach (AccelStructInstances.InstanceEntry instanceEntry in this.m_Instances.Values)
			{
				GeometryPoolHandle geometryPoolHandle = instanceEntry.geometryPoolHandle;
				this.m_GeometryPool.Unregister(geometryPoolHandle);
			}
			this.m_GeometryPool.SendGpuCommands();
			this.m_Instances.Clear();
			this.m_InstanceBuffer.Clear();
		}

		public void UpdateInstanceTransform(int instanceHandle, Matrix4x4 localToWorldMatrix)
		{
			AccelStructInstances.InstanceEntry instanceEntry;
			this.m_Instances.TryGetValue(instanceHandle, out instanceEntry);
			AccelStructInstances.RTInstance element = this.m_InstanceBuffer.Get(instanceEntry.indexInInstanceBuffer);
			element.localToWorld = localToWorldMatrix;
			element.localToWorldNormals = AccelStructInstances.NormalMatrix(localToWorldMatrix);
			this.m_InstanceBuffer.Set(instanceEntry.indexInInstanceBuffer, element);
			this.m_TransformTouchedLastTimestamp = this.m_FrameTimestamp;
		}

		public void UpdateInstanceMaterialID(int instanceHandle, uint materialID)
		{
			AccelStructInstances.InstanceEntry instanceEntry;
			this.m_Instances.TryGetValue(instanceHandle, out instanceEntry);
			AccelStructInstances.RTInstance element = this.m_InstanceBuffer.Get(instanceEntry.indexInInstanceBuffer);
			element.userMaterialID = materialID;
			this.m_InstanceBuffer.Set(instanceEntry.indexInInstanceBuffer, element);
		}

		public void UpdateRenderingLayerMask(int instanceHandle, uint renderingLayerMask)
		{
			AccelStructInstances.InstanceEntry instanceEntry;
			this.m_Instances.TryGetValue(instanceHandle, out instanceEntry);
			AccelStructInstances.RTInstance element = this.m_InstanceBuffer.Get(instanceEntry.indexInInstanceBuffer);
			element.renderingLayerMask = renderingLayerMask;
			this.m_InstanceBuffer.Set(instanceEntry.indexInInstanceBuffer, element);
		}

		public void UpdateInstanceMask(int instanceHandle, uint mask)
		{
			AccelStructInstances.InstanceEntry instanceEntry;
			this.m_Instances.TryGetValue(instanceHandle, out instanceEntry);
			instanceEntry.instanceMask = mask;
			AccelStructInstances.RTInstance element = this.m_InstanceBuffer.Get(instanceEntry.indexInInstanceBuffer);
			element.instanceMask = mask;
			this.m_InstanceBuffer.Set(instanceEntry.indexInInstanceBuffer, element);
		}

		public void NextFrame()
		{
			if (this.m_FrameTimestamp - this.m_TransformTouchedLastTimestamp <= 1U)
			{
				this.m_InstanceBuffer.ModifyForEach(delegate(AccelStructInstances.RTInstance instance)
				{
					instance.previousLocalToWorld = instance.localToWorld;
					return instance;
				});
			}
			this.m_FrameTimestamp += 1U;
		}

		public bool instanceListValid
		{
			get
			{
				return this.m_InstanceBuffer != null;
			}
		}

		public void Bind(CommandBuffer cmd, IRayTracingShader shader)
		{
			ComputeBuffer gpuBuffer = this.m_InstanceBuffer.GetGpuBuffer(cmd);
			shader.SetBufferParam(cmd, Shader.PropertyToID("g_AccelStructInstanceList"), gpuBuffer);
			shader.SetBufferParam(cmd, Shader.PropertyToID("g_globalIndexBuffer"), this.m_GeometryPool.globalIndexBuffer);
			shader.SetBufferParam(cmd, Shader.PropertyToID("g_globalVertexBuffer"), this.m_GeometryPool.globalVertexBuffer);
			shader.SetIntParam(cmd, Shader.PropertyToID("g_globalVertexBufferStride"), this.m_GeometryPool.globalVertexBufferStrideBytes / 4);
			shader.SetBufferParam(cmd, Shader.PropertyToID("g_MeshList"), this.m_GeometryPool.globalMeshChunkTableEntryBuffer);
		}

		public int GetInstanceCount()
		{
			return this.m_Instances.Count;
		}

		private static float4x4 NormalMatrix(float4x4 m)
		{
			return new float4x4(math.inverse(math.transpose(new float3x3(m))), new float3(0.0));
		}

		private readonly GeometryPool m_GeometryPool;

		private readonly PersistentGpuArray<AccelStructInstances.RTInstance> m_InstanceBuffer = new PersistentGpuArray<AccelStructInstances.RTInstance>(100);

		private readonly Dictionary<int, AccelStructInstances.InstanceEntry> m_Instances = new Dictionary<int, AccelStructInstances.InstanceEntry>();

		private uint m_FrameTimestamp;

		private uint m_TransformTouchedLastTimestamp;

		public struct RTInstance
		{
			public float4x4 localToWorld;

			public float4x4 previousLocalToWorld;

			public float4x4 localToWorldNormals;

			public uint renderingLayerMask;

			public uint instanceMask;

			public uint userMaterialID;

			public uint geometryIndex;
		}

		public class InstanceEntry
		{
			public GeometryPoolHandle geometryPoolHandle;

			public BlockAllocator.Allocation indexInInstanceBuffer;

			public uint instanceMask;

			public uint vertexOffset;

			public uint indexOffset;
		}
	}
}
