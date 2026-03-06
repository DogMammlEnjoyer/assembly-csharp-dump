using System;
using System.Runtime.InteropServices;
using Unity.Mathematics;

namespace UnityEngine.Rendering.RadeonRays
{
	internal class HlbvhTopLevelBuilder
	{
		public HlbvhTopLevelBuilder(RadeonRaysShaders shaders)
		{
			this.shaderBuildHlbvh = shaders.buildHlbvh;
			this.kernelInit = this.shaderBuildHlbvh.FindKernel("Init");
			this.kernelCalculateAabb = this.shaderBuildHlbvh.FindKernel("CalculateAabb");
			this.kernelCalculateMortonCodes = this.shaderBuildHlbvh.FindKernel("CalculateMortonCodes");
			this.kernelBuildTreeBottomUp = this.shaderBuildHlbvh.FindKernel("BuildTreeBottomUp");
			this.radixSort = new RadixSort(shaders);
		}

		public ulong GetScratchDataSizeInDwords(uint instanceCount)
		{
			return (ulong)HlbvhTopLevelBuilder.ScratchBufferLayout.Create(instanceCount).TotalSize;
		}

		public static uint GetBvhNodeCount(uint leafCount)
		{
			return leafCount - 1U;
		}

		public void AllocateResultBuffers(uint instanceCount, ref TopLevelAccelStruct accelStruct)
		{
			uint bvhNodeCount = HlbvhTopLevelBuilder.GetBvhNodeCount(instanceCount);
			accelStruct.Dispose();
			accelStruct.instanceInfos = new GraphicsBuffer(GraphicsBuffer.Target.Structured, (int)instanceCount, Marshal.SizeOf<InstanceInfo>());
			accelStruct.topLevelBvh = new GraphicsBuffer(GraphicsBuffer.Target.Structured, (int)(bvhNodeCount + 1U), Marshal.SizeOf<BvhNode>());
		}

		public void CreateEmpty(ref TopLevelAccelStruct accelStruct)
		{
			accelStruct.Dispose();
			accelStruct.topLevelBvh = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 2, Marshal.SizeOf<BvhNode>());
			accelStruct.instanceInfos = accelStruct.topLevelBvh;
			accelStruct.bottomLevelBvhs = accelStruct.topLevelBvh;
			accelStruct.instanceCount = 0U;
			BvhNode[] array = new BvhNode[2];
			array[0].child0 = 0U;
			array[0].child1 = 0U;
			array[0].parent = 0U;
			array[1].child0 = 0U;
			array[1].child1 = 0U;
			array[1].parent = uint.MaxValue;
			array[1].update = 0U;
			array[1].aabb0_min = new float3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
			array[1].aabb0_max = new float3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
			array[1].aabb1_min = new float3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
			array[1].aabb1_max = new float3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
			accelStruct.topLevelBvh.SetData(array);
		}

		public void Execute(CommandBuffer cmd, GraphicsBuffer scratch, ref TopLevelAccelStruct accelStruct)
		{
			Common.EnableKeyword(cmd, this.shaderBuildHlbvh, "TOP_LEVEL", true);
			Common.EnableKeyword(cmd, this.shaderBuildHlbvh, "UINT16_INDICES", false);
			uint instanceCount = accelStruct.instanceCount;
			HlbvhTopLevelBuilder.ScratchBufferLayout scratchBufferLayout = HlbvhTopLevelBuilder.ScratchBufferLayout.Create(instanceCount);
			cmd.SetComputeIntParam(this.shaderBuildHlbvh, SID.g_constants_vertex_stride, 0);
			cmd.SetComputeIntParam(this.shaderBuildHlbvh, SID.g_constants_triangle_count, (int)instanceCount);
			cmd.SetComputeIntParam(this.shaderBuildHlbvh, SID.g_bvh_offset, 0);
			cmd.SetComputeIntParam(this.shaderBuildHlbvh, SID.g_internal_node_range_offset, (int)scratchBufferLayout.InternalNodeRange);
			cmd.SetComputeIntParam(this.shaderBuildHlbvh, SID.g_aabb_offset, (int)scratchBufferLayout.Aabb);
			this.BindKernelArguments(cmd, this.kernelInit, scratch, scratchBufferLayout, accelStruct, false);
			cmd.DispatchCompute(this.shaderBuildHlbvh, this.kernelInit, 1, 1, 1);
			this.BindKernelArguments(cmd, this.kernelCalculateAabb, scratch, scratchBufferLayout, accelStruct, false);
			cmd.DispatchCompute(this.shaderBuildHlbvh, this.kernelCalculateAabb, (int)Common.CeilDivide(instanceCount, 2048U), 1, 1);
			this.BindKernelArguments(cmd, this.kernelCalculateMortonCodes, scratch, scratchBufferLayout, accelStruct, false);
			cmd.DispatchCompute(this.shaderBuildHlbvh, this.kernelCalculateMortonCodes, (int)Common.CeilDivide(instanceCount, 2048U), 1, 1);
			this.radixSort.Execute(cmd, scratch, scratchBufferLayout.MortonCodes, scratchBufferLayout.SortedMortonCodes, scratchBufferLayout.PrimitiveRefs, scratchBufferLayout.SortedPrimitiveRefs, scratchBufferLayout.SortMemory, instanceCount);
			this.BindKernelArguments(cmd, this.kernelBuildTreeBottomUp, scratch, scratchBufferLayout, accelStruct, true);
			cmd.DispatchCompute(this.shaderBuildHlbvh, this.kernelBuildTreeBottomUp, (int)Common.CeilDivide(instanceCount, 2048U), 1, 1);
		}

		private void BindKernelArguments(CommandBuffer cmd, int kernel, GraphicsBuffer scratch, HlbvhTopLevelBuilder.ScratchBufferLayout scratchLayout, TopLevelAccelStruct accelStruct, bool setSortedCodes)
		{
			cmd.SetComputeBufferParam(this.shaderBuildHlbvh, kernel, SID.g_scratch_buffer, scratch);
			cmd.SetComputeBufferParam(this.shaderBuildHlbvh, kernel, SID.g_bvh, accelStruct.topLevelBvh);
			cmd.SetComputeBufferParam(this.shaderBuildHlbvh, kernel, SID.g_bottom_bvhs, accelStruct.bottomLevelBvhs);
			cmd.SetComputeBufferParam(this.shaderBuildHlbvh, kernel, SID.g_instance_infos, accelStruct.instanceInfos);
			cmd.SetComputeIntParam(this.shaderBuildHlbvh, SID.g_aabb_offset, (int)scratchLayout.Aabb);
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
			public static HlbvhTopLevelBuilder.ScratchBufferLayout Create(uint instanceCount)
			{
				HlbvhTopLevelBuilder.ScratchBufferLayout scratchBufferLayout = default(HlbvhTopLevelBuilder.ScratchBufferLayout);
				scratchBufferLayout.Aabb = scratchBufferLayout.Reserve(6U);
				scratchBufferLayout.MortonCodes = scratchBufferLayout.Reserve(instanceCount);
				scratchBufferLayout.PrimitiveRefs = scratchBufferLayout.Reserve(instanceCount);
				scratchBufferLayout.SortedMortonCodes = scratchBufferLayout.Reserve(instanceCount);
				scratchBufferLayout.SortedPrimitiveRefs = scratchBufferLayout.Reserve(instanceCount);
				scratchBufferLayout.SortMemory = scratchBufferLayout.Reserve((uint)RadixSort.GetScratchDataSizeInDwords(instanceCount));
				scratchBufferLayout.InternalNodeRange = scratchBufferLayout.MortonCodes;
				return scratchBufferLayout;
			}

			private uint Reserve(uint size)
			{
				uint totalSize = this.TotalSize;
				this.TotalSize += size;
				return totalSize;
			}

			public uint Aabb;

			public uint MortonCodes;

			public uint PrimitiveRefs;

			public uint SortedMortonCodes;

			public uint SortedPrimitiveRefs;

			public uint SortMemory;

			public uint InternalNodeRange;

			public uint TotalSize;
		}
	}
}
