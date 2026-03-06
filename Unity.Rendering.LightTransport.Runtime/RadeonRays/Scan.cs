using System;

namespace UnityEngine.Rendering.RadeonRays
{
	internal class Scan
	{
		public Scan(RadeonRaysShaders shaders)
		{
			this.shaderScan = shaders.blockScan;
			this.kernelScan = this.shaderScan.FindKernel("BlockScanAdd");
			this.shaderReduce = shaders.blockReducePart;
			this.kernelReduce = this.shaderReduce.FindKernel("BlockReducePart");
		}

		public void Execute(CommandBuffer cmd, GraphicsBuffer buffer, uint inputKeysOffset, uint outputKeysOffset, uint scratchDataOffset, uint size)
		{
			if (size > 1024U)
			{
				uint num = Common.CeilDivide(size, 1024U);
				this.SetState(cmd, this.shaderReduce, this.kernelReduce, size, buffer, inputKeysOffset, scratchDataOffset, outputKeysOffset);
				cmd.DispatchCompute(this.shaderReduce, this.kernelReduce, (int)num, 1, 1);
				if (num > 1024U)
				{
					uint num2 = Common.CeilDivide(num, 1024U);
					this.SetState(cmd, this.shaderReduce, this.kernelReduce, num, buffer, scratchDataOffset, scratchDataOffset + num, scratchDataOffset);
					cmd.DispatchCompute(this.shaderReduce, this.kernelReduce, (int)num2, 1, 1);
					Common.EnableKeyword(cmd, this.shaderScan, "ADD_PART_SUM", false);
					this.SetState(cmd, this.shaderScan, this.kernelScan, num2, buffer, scratchDataOffset + num, scratchDataOffset, scratchDataOffset + num);
					cmd.DispatchCompute(this.shaderScan, this.kernelScan, 1, 1, 1);
				}
				Common.EnableKeyword(cmd, this.shaderScan, "ADD_PART_SUM", num > 1024U);
				this.SetState(cmd, this.shaderScan, this.kernelScan, num, buffer, scratchDataOffset, scratchDataOffset + num, scratchDataOffset);
				uint threadGroupsX = Common.CeilDivide(num, 1024U);
				cmd.DispatchCompute(this.shaderScan, this.kernelScan, (int)threadGroupsX, 1, 1);
			}
			Common.EnableKeyword(cmd, this.shaderScan, "ADD_PART_SUM", size > 1024U);
			this.SetState(cmd, this.shaderScan, this.kernelScan, size, buffer, inputKeysOffset, scratchDataOffset, outputKeysOffset);
			uint threadGroupsX2 = Common.CeilDivide(size, 1024U);
			cmd.DispatchCompute(this.shaderScan, this.kernelScan, (int)threadGroupsX2, 1, 1);
		}

		private void SetState(CommandBuffer cmd, ComputeShader shader, int kernelIndex, uint size, GraphicsBuffer buffer, uint inputKeysOffset, uint scratchDataOffset, uint outputKeysOffset)
		{
			cmd.SetComputeIntParam(shader, SID.g_constants_num_keys, (int)size);
			cmd.SetComputeIntParam(shader, SID.g_constants_input_keys_offset, (int)inputKeysOffset);
			cmd.SetComputeIntParam(shader, SID.g_constants_part_sums_offset, (int)scratchDataOffset);
			cmd.SetComputeIntParam(shader, SID.g_constants_output_keys_offset, (int)outputKeysOffset);
			cmd.SetComputeBufferParam(shader, kernelIndex, SID.g_buffer, buffer);
		}

		public static ulong GetScratchDataSizeInDwords(uint size)
		{
			if (size <= 1024U)
			{
				return 0UL;
			}
			uint num = Common.CeilDivide(size, 1024U);
			if (num <= 1024U)
			{
				return (ulong)num;
			}
			uint num2 = Common.CeilDivide(num, 1024U);
			return (ulong)(num + num2);
		}

		private readonly ComputeShader shaderScan;

		private readonly int kernelScan;

		private readonly ComputeShader shaderReduce;

		private readonly int kernelReduce;

		private const uint kKeysPerThread = 4U;

		private const uint kGroupSize = 256U;

		private const uint kKeysPerGroup = 1024U;
	}
}
