using System;

namespace UnityEngine.Rendering.RadeonRays
{
	internal sealed class RestructureBvh : IDisposable
	{
		public RestructureBvh(RadeonRaysShaders shaders)
		{
			this.shader = shaders.restructureBvh;
			this.kernelInitPrimitiveCounts = this.shader.FindKernel("InitPrimitiveCounts");
			this.kernelFindTreeletRoots = this.shader.FindKernel("FindTreeletRoots");
			this.kernelRestructure = this.shader.FindKernel("Restructure");
			this.kernelPrepareTreeletsDispatchSize = this.shader.FindKernel("PrepareTreeletsDispatchSize");
			this.treeletDispatchIndirectBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 6, 4);
		}

		public void Dispose()
		{
			this.treeletDispatchIndirectBuffer.Dispose();
		}

		public ulong GetScratchDataSizeInDwords(uint triangleCount)
		{
			return (ulong)RestructureBvh.ScratchBufferLayout.Create(triangleCount).TotalSize;
		}

		public static uint GetBvhNodeCount(uint leafCount)
		{
			return leafCount - 1U;
		}

		public void Execute(CommandBuffer cmd, GraphicsBuffer vertices, int verticesOffset, uint vertexStride, uint triangleCount, GraphicsBuffer scratch, in BottomLevelLevelAccelStruct result)
		{
			RestructureBvh.ScratchBufferLayout scratchBufferLayout = RestructureBvh.ScratchBufferLayout.Create(triangleCount);
			Common.EnableKeyword(cmd, this.shader, "TOP_LEVEL", false);
			cmd.SetComputeIntParam(this.shader, SID.g_vertices_offset, verticesOffset);
			cmd.SetComputeIntParam(this.shader, SID.g_constants_vertex_stride, (int)vertexStride);
			cmd.SetComputeIntParam(this.shader, SID.g_constants_triangle_count, (int)triangleCount);
			cmd.SetComputeIntParam(this.shader, SID.g_treelet_count_offset, (int)scratchBufferLayout.TreeletCount);
			cmd.SetComputeIntParam(this.shader, SID.g_treelet_roots_offset, (int)scratchBufferLayout.TreeletRoots);
			cmd.SetComputeIntParam(this.shader, SID.g_primitive_counts_offset, (int)scratchBufferLayout.PrimitiveCounts);
			cmd.SetComputeIntParam(this.shader, SID.g_leaf_parents_offset, (int)scratchBufferLayout.LeafParents);
			cmd.SetComputeIntParam(this.shader, SID.g_bvh_offset, (int)result.bvhOffset);
			cmd.SetComputeIntParam(this.shader, SID.g_bvh_leaves_offset, (int)result.bvhLeavesOffset);
			uint num = 64U;
			for (int i = 0; i < 3; i++)
			{
				cmd.SetComputeIntParam(this.shader, SID.g_constants_min_prims_per_treelet, (int)num);
				this.BindKernelArguments(cmd, this.kernelInitPrimitiveCounts, vertices, scratch, result);
				cmd.DispatchCompute(this.shader, this.kernelInitPrimitiveCounts, (int)Common.CeilDivide(2048U, 256U), 1, 1);
				this.BindKernelArguments(cmd, this.kernelFindTreeletRoots, vertices, scratch, result);
				cmd.DispatchCompute(this.shader, this.kernelFindTreeletRoots, (int)Common.CeilDivide(2048U, 256U), 1, 1);
				this.BindKernelArguments(cmd, this.kernelPrepareTreeletsDispatchSize, vertices, scratch, result);
				cmd.DispatchCompute(this.shader, this.kernelPrepareTreeletsDispatchSize, 1, 1, 1);
				this.BindKernelArguments(cmd, this.kernelRestructure, vertices, scratch, result);
				cmd.SetComputeIntParam(this.shader, SID.g_remainder_treelets, 0);
				cmd.DispatchCompute(this.shader, this.kernelRestructure, this.treeletDispatchIndirectBuffer, 0U);
				if (Common.CeilDivide(triangleCount, num) > 65535U)
				{
					cmd.SetComputeIntParam(this.shader, SID.g_remainder_treelets, 1);
					cmd.DispatchCompute(this.shader, this.kernelRestructure, this.treeletDispatchIndirectBuffer, 12U);
				}
				num *= 2U;
			}
		}

		private void BindKernelArguments(CommandBuffer cmd, int kernel, GraphicsBuffer vertices, GraphicsBuffer scratch, BottomLevelLevelAccelStruct result)
		{
			cmd.SetComputeBufferParam(this.shader, kernel, SID.g_vertices, vertices);
			cmd.SetComputeBufferParam(this.shader, kernel, SID.g_scratch_buffer, scratch);
			cmd.SetComputeBufferParam(this.shader, kernel, SID.g_bvh, result.bvh);
			cmd.SetComputeBufferParam(this.shader, kernel, SID.g_bvh_leaves, result.bvhLeaves);
			cmd.SetComputeBufferParam(this.shader, kernel, SID.g_treelet_dispatch_buffer, this.treeletDispatchIndirectBuffer);
		}

		private readonly ComputeShader shader;

		private readonly int kernelInitPrimitiveCounts;

		private readonly int kernelFindTreeletRoots;

		private readonly int kernelRestructure;

		private readonly int kernelPrepareTreeletsDispatchSize;

		private const int numIterations = 3;

		private readonly GraphicsBuffer treeletDispatchIndirectBuffer;

		private const uint kGroupSize = 256U;

		private const uint kTrianglesPerThread = 8U;

		private const uint kTrianglesPerGroup = 2048U;

		private const uint kMinPrimitivesPerTreelet = 64U;

		private const int kMaxThreadGroupsPerDispatch = 65535;

		private struct ScratchBufferLayout
		{
			public static RestructureBvh.ScratchBufferLayout Create(uint triangleCount)
			{
				RestructureBvh.ScratchBufferLayout result = default(RestructureBvh.ScratchBufferLayout);
				result.LeafParents = result.Reserve(triangleCount);
				result.TreeletCount = result.Reserve(1U);
				result.TreeletRoots = result.Reserve(triangleCount);
				result.PrimitiveCounts = result.Reserve(RestructureBvh.GetBvhNodeCount(triangleCount));
				return result;
			}

			private uint Reserve(uint size)
			{
				uint totalSize = this.TotalSize;
				this.TotalSize += size;
				return totalSize;
			}

			public uint LeafParents;

			public uint TreeletCount;

			public uint TreeletRoots;

			public uint PrimitiveCounts;

			public uint TotalSize;
		}
	}
}
