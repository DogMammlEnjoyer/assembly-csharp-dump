using System;
using Unity.Mathematics;

namespace UnityEngine.Rendering.RadeonRays
{
	internal class HlbvhBuilder
	{
		public HlbvhBuilder(RadeonRaysShaders shaders)
		{
			this.shaderBuildHlbvh = shaders.buildHlbvh;
			this.kernelInit = this.shaderBuildHlbvh.FindKernel("Init");
			this.kernelCalculateAabb = this.shaderBuildHlbvh.FindKernel("CalculateAabb");
			this.kernelCalculateMortonCodes = this.shaderBuildHlbvh.FindKernel("CalculateMortonCodes");
			this.kernelBuildTreeBottomUp = this.shaderBuildHlbvh.FindKernel("BuildTreeBottomUp");
			this.radixSort = new RadixSort(shaders);
		}

		public uint GetScratchDataSizeInDwords(uint triangleCount)
		{
			return HlbvhBuilder.ScratchBufferLayout.Create(triangleCount).TotalSize;
		}

		public static uint GetBvhNodeCount(uint leafCount)
		{
			return leafCount - 1U;
		}

		public uint GetResultDataSizeInDwords(uint triangleCount)
		{
			uint num = HlbvhBuilder.GetBvhNodeCount(triangleCount) + 1U;
			uint num2 = 16U;
			return num * num2;
		}

		public void Execute(CommandBuffer cmd, GraphicsBuffer vertices, int verticesOffset, uint vertexStride, GraphicsBuffer indices, int indicesOffset, int baseIndex, IndexFormat indexFormat, uint triangleCount, GraphicsBuffer scratch, in BottomLevelLevelAccelStruct result)
		{
			Common.EnableKeyword(cmd, this.shaderBuildHlbvh, "TOP_LEVEL", false);
			Common.EnableKeyword(cmd, this.shaderBuildHlbvh, "UINT16_INDICES", indexFormat == IndexFormat.Int16);
			HlbvhBuilder.ScratchBufferLayout scratchBufferLayout = HlbvhBuilder.ScratchBufferLayout.Create(triangleCount);
			cmd.SetComputeIntParam(this.shaderBuildHlbvh, SID.g_indices_offset, indicesOffset);
			cmd.SetComputeIntParam(this.shaderBuildHlbvh, SID.g_base_index, baseIndex);
			cmd.SetComputeIntParam(this.shaderBuildHlbvh, SID.g_vertices_offset, verticesOffset);
			cmd.SetComputeIntParam(this.shaderBuildHlbvh, SID.g_constants_vertex_stride, (int)vertexStride);
			cmd.SetComputeIntParam(this.shaderBuildHlbvh, SID.g_constants_triangle_count, (int)triangleCount);
			cmd.SetComputeIntParam(this.shaderBuildHlbvh, SID.g_bvh_offset, (int)result.bvhOffset);
			cmd.SetComputeIntParam(this.shaderBuildHlbvh, SID.g_bvh_leaves_offset, (int)result.bvhLeavesOffset);
			cmd.SetComputeIntParam(this.shaderBuildHlbvh, SID.g_internal_node_range_offset, (int)scratchBufferLayout.InternalNodeRange);
			cmd.SetComputeIntParam(this.shaderBuildHlbvh, SID.g_leaf_parents_offset, (int)scratchBufferLayout.LeafParents);
			cmd.SetComputeIntParam(this.shaderBuildHlbvh, SID.g_aabb_offset, (int)scratchBufferLayout.Aabb);
			this.BindKernelArguments(cmd, this.kernelInit, vertices, indices, scratch, scratchBufferLayout, result, false);
			cmd.DispatchCompute(this.shaderBuildHlbvh, this.kernelInit, 1, 1, 1);
			this.BindKernelArguments(cmd, this.kernelCalculateAabb, vertices, indices, scratch, scratchBufferLayout, result, false);
			cmd.DispatchCompute(this.shaderBuildHlbvh, this.kernelCalculateAabb, (int)Common.CeilDivide(triangleCount, 2048U), 1, 1);
			this.BindKernelArguments(cmd, this.kernelCalculateMortonCodes, vertices, indices, scratch, scratchBufferLayout, result, false);
			cmd.DispatchCompute(this.shaderBuildHlbvh, this.kernelCalculateMortonCodes, (int)Common.CeilDivide(triangleCount, 2048U), 1, 1);
			this.radixSort.Execute(cmd, scratch, scratchBufferLayout.MortonCodes, scratchBufferLayout.SortedMortonCodes, scratchBufferLayout.PrimitiveRefs, scratchBufferLayout.SortedPrimitiveRefs, scratchBufferLayout.SortMemory, triangleCount);
			this.BindKernelArguments(cmd, this.kernelBuildTreeBottomUp, vertices, indices, scratch, scratchBufferLayout, result, true);
			cmd.DispatchCompute(this.shaderBuildHlbvh, this.kernelBuildTreeBottomUp, (int)Common.CeilDivide(triangleCount, 2048U), 1, 1);
		}

		private void BindKernelArguments(CommandBuffer cmd, int kernel, GraphicsBuffer vertices, GraphicsBuffer indices, GraphicsBuffer scratch, HlbvhBuilder.ScratchBufferLayout scratchLayout, BottomLevelLevelAccelStruct result, bool setSortedCodes)
		{
			cmd.SetComputeBufferParam(this.shaderBuildHlbvh, kernel, SID.g_vertices, vertices);
			cmd.SetComputeBufferParam(this.shaderBuildHlbvh, kernel, SID.g_indices, indices);
			cmd.SetComputeBufferParam(this.shaderBuildHlbvh, kernel, SID.g_scratch_buffer, scratch);
			cmd.SetComputeBufferParam(this.shaderBuildHlbvh, kernel, SID.g_bvh, result.bvh);
			cmd.SetComputeBufferParam(this.shaderBuildHlbvh, kernel, SID.g_bvh_leaves, result.bvhLeaves);
			if (setSortedCodes)
			{
				cmd.SetComputeIntParam(this.shaderBuildHlbvh, SID.g_morton_codes_offset, (int)scratchLayout.SortedMortonCodes);
				cmd.SetComputeIntParam(this.shaderBuildHlbvh, SID.g_primitive_refs_offset, (int)scratchLayout.SortedPrimitiveRefs);
				return;
			}
			cmd.SetComputeIntParam(this.shaderBuildHlbvh, SID.g_morton_codes_offset, (int)scratchLayout.MortonCodes);
			cmd.SetComputeIntParam(this.shaderBuildHlbvh, SID.g_primitive_refs_offset, (int)scratchLayout.PrimitiveRefs);
		}

		private readonly ComputeShader shaderBuildHlbvh;

		private readonly int kernelInit;

		private readonly int kernelCalculateAabb;

		private readonly int kernelCalculateMortonCodes;

		private readonly int kernelBuildTreeBottomUp;

		private readonly RadixSort radixSort;

		private const uint kTrianglesPerThread = 8U;

		private const uint kGroupSize = 256U;

		private const uint kTrianglesPerGroup = 2048U;

		private struct ScratchBufferLayout
		{
			public static HlbvhBuilder.ScratchBufferLayout Create(uint triangleCount)
			{
				HlbvhBuilder.ScratchBufferLayout scratchBufferLayout = default(HlbvhBuilder.ScratchBufferLayout);
				scratchBufferLayout.SortMemory = scratchBufferLayout.Reserve(math.max((uint)RadixSort.GetScratchDataSizeInDwords(triangleCount), triangleCount));
				scratchBufferLayout.PrimitiveRefs = scratchBufferLayout.Reserve(triangleCount);
				scratchBufferLayout.MortonCodes = scratchBufferLayout.Reserve(triangleCount);
				scratchBufferLayout.SortedPrimitiveRefs = scratchBufferLayout.Reserve(triangleCount);
				scratchBufferLayout.SortedMortonCodes = scratchBufferLayout.Reserve(triangleCount);
				scratchBufferLayout.Aabb = scratchBufferLayout.Reserve(6U);
				scratchBufferLayout.InternalNodeRange = scratchBufferLayout.PrimitiveRefs;
				scratchBufferLayout.LeafParents = scratchBufferLayout.SortMemory;
				return scratchBufferLayout;
			}

			private uint Reserve(uint size)
			{
				uint totalSize = this.TotalSize;
				this.TotalSize += size;
				return totalSize;
			}

			public uint PrimitiveRefs;

			public uint MortonCodes;

			public uint SortedPrimitiveRefs;

			public uint SortedMortonCodes;

			public uint SortMemory;

			public uint Aabb;

			public uint LeafParents;

			public uint InternalNodeRange;

			public uint TotalSize;
		}
	}
}
