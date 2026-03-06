using System;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine.Rendering.RadeonRays;

namespace UnityEngine.Rendering.UnifiedRayTracing
{
	internal sealed class BLASPositionsPool : IDisposable
	{
		public BLASPositionsPool(ComputeShader copyPositionsShader, ComputeShader copyShader)
		{
			this.m_VerticesBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 3000, 4);
			this.m_VerticesAllocator = default(BlockAllocator);
			this.m_VerticesAllocator.Initialize(1000);
			this.m_CopyPositionsShader = copyPositionsShader;
			this.m_CopyVerticesKernel = this.m_CopyPositionsShader.FindKernel("CopyVertexBuffer");
			this.m_CopyShader = copyShader;
		}

		public void Dispose()
		{
			this.m_VerticesBuffer.Dispose();
			this.m_VerticesAllocator.Dispose();
		}

		public GraphicsBuffer VertexBuffer
		{
			get
			{
				return this.m_VerticesBuffer;
			}
		}

		public void Clear()
		{
			this.m_VerticesBuffer.Dispose();
			this.m_VerticesBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 3000, 4);
			this.m_VerticesAllocator.Dispose();
			this.m_VerticesAllocator = default(BlockAllocator);
			this.m_VerticesAllocator.Initialize(1000);
		}

		public bool Add(VertexBufferChunk info, out BlockAllocator.Allocation verticesAllocation)
		{
			verticesAllocation = this.m_VerticesAllocator.Allocate((int)(info.vertexCount * 3U));
			if (!verticesAllocation.valid)
			{
				int oldCapacity;
				int newCapacity;
				verticesAllocation = this.m_VerticesAllocator.GrowAndAllocate((int)info.vertexCount, int.MaxValue / UnsafeUtility.SizeOf<float3>(), out oldCapacity, out newCapacity);
				if (!verticesAllocation.valid)
				{
					return false;
				}
				GraphicsHelpers.ReallocateBuffer(this.m_CopyShader, oldCapacity, newCapacity, UnsafeUtility.SizeOf<float3>(), ref this.m_VerticesBuffer);
			}
			CommandBuffer commandBuffer = new CommandBuffer();
			commandBuffer.SetComputeIntParam(this.m_CopyPositionsShader, "_InputPosBufferCount", (int)info.vertexCount);
			commandBuffer.SetComputeIntParam(this.m_CopyPositionsShader, "_InputPosBufferOffset", info.verticesStartOffset);
			commandBuffer.SetComputeIntParam(this.m_CopyPositionsShader, "_InputBaseVertex", info.baseVertex);
			commandBuffer.SetComputeIntParam(this.m_CopyPositionsShader, "_InputPosBufferStride", (int)info.vertexStride);
			commandBuffer.SetComputeIntParam(this.m_CopyPositionsShader, "_OutputPosBufferOffset", verticesAllocation.block.offset * 3);
			commandBuffer.SetComputeBufferParam(this.m_CopyPositionsShader, this.m_CopyVerticesKernel, "_InputPosBuffer", info.vertices);
			commandBuffer.SetComputeBufferParam(this.m_CopyPositionsShader, this.m_CopyVerticesKernel, "_OutputPosBuffer", this.m_VerticesBuffer);
			commandBuffer.DispatchCompute(this.m_CopyPositionsShader, this.m_CopyVerticesKernel, (int)Common.CeilDivide(info.vertexCount, 6144U), 1, 1);
			Graphics.ExecuteCommandBuffer(commandBuffer);
			return true;
		}

		public void Remove(ref BlockAllocator.Allocation verticesAllocation)
		{
			this.m_VerticesAllocator.FreeAllocation(verticesAllocation);
			verticesAllocation = BlockAllocator.Allocation.Invalid;
		}

		public const int VertexSizeInDwords = 3;

		private const int intialVertexCount = 1000;

		private GraphicsBuffer m_VerticesBuffer;

		private BlockAllocator m_VerticesAllocator;

		private readonly ComputeShader m_CopyPositionsShader;

		private readonly int m_CopyVerticesKernel;

		private readonly ComputeShader m_CopyShader;

		private const uint kItemsPerWorkgroup = 6144U;
	}
}
