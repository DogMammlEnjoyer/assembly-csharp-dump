using System;

namespace UnityEngine.Rendering.RadeonRays
{
	internal class RadixSort
	{
		public RadixSort(RadeonRaysShaders shaders)
		{
			this.shaderBitHistogram = shaders.bitHistogram;
			this.kernelBitHistogram = this.shaderBitHistogram.FindKernel("BitHistogram");
			this.shaderScatter = shaders.scatter;
			this.kernelScatter = this.shaderScatter.FindKernel("Scatter");
			this.scan = new Scan(shaders);
		}

		public void Execute(CommandBuffer cmd, GraphicsBuffer buffer, uint inputKeysOffset, uint outputKeysOffset, uint inputValuesOffset, uint outputValuesOffset, uint scratchDataOffset, uint size)
		{
			uint num = 16U * Common.CeilDivide(size, 1024U);
			uint threadGroupsX = Common.CeilDivide(size, 1024U);
			uint num2 = scratchDataOffset + size;
			uint num3 = num2 + size;
			uint scratchDataOffset2 = num3 + num;
			uint num4 = outputKeysOffset;
			uint num5 = outputValuesOffset;
			uint num6 = scratchDataOffset;
			uint num7 = num2;
			for (uint num8 = 0U; num8 < 32U; num8 += 4U)
			{
				cmd.SetComputeIntParam(this.shaderBitHistogram, SID.g_constants_num_keys, (int)size);
				cmd.SetComputeIntParam(this.shaderBitHistogram, SID.g_constants_num_blocks, (int)Common.CeilDivide(size, 1024U));
				cmd.SetComputeIntParam(this.shaderBitHistogram, SID.g_constants_bit_shift, (int)num8);
				cmd.SetComputeBufferParam(this.shaderBitHistogram, this.kernelBitHistogram, SID.g_buffer, buffer);
				cmd.SetComputeIntParam(this.shaderBitHistogram, SID.g_input_keys_offset, (int)((num8 == 0U) ? inputKeysOffset : num4));
				cmd.SetComputeIntParam(this.shaderBitHistogram, SID.g_group_histograms_offset, (int)num3);
				cmd.DispatchCompute(this.shaderBitHistogram, this.kernelBitHistogram, (int)threadGroupsX, 1, 1);
				this.scan.Execute(cmd, buffer, num3, num3, scratchDataOffset2, num);
				cmd.SetComputeIntParam(this.shaderScatter, SID.g_constants_num_keys, (int)size);
				cmd.SetComputeIntParam(this.shaderScatter, SID.g_constants_num_blocks, (int)Common.CeilDivide(size, 1024U));
				cmd.SetComputeIntParam(this.shaderScatter, SID.g_constants_bit_shift, (int)num8);
				cmd.SetComputeBufferParam(this.shaderScatter, this.kernelScatter, SID.g_buffer, buffer);
				cmd.SetComputeIntParam(this.shaderScatter, SID.g_input_keys_offset, (int)((num8 == 0U) ? inputKeysOffset : num4));
				cmd.SetComputeIntParam(this.shaderScatter, SID.g_group_histograms_offset, (int)num3);
				cmd.SetComputeIntParam(this.shaderScatter, SID.g_output_keys_offset, (int)num6);
				cmd.SetComputeIntParam(this.shaderScatter, SID.g_input_values_offset, (int)((num8 == 0U) ? inputValuesOffset : num5));
				cmd.SetComputeIntParam(this.shaderScatter, SID.g_output_values_offset, (int)num7);
				cmd.DispatchCompute(this.shaderScatter, this.kernelScatter, (int)threadGroupsX, 1, 1);
				uint num9 = num4;
				uint num10 = num6;
				num6 = num9;
				num4 = num10;
				uint num11 = num5;
				num10 = num7;
				num7 = num11;
				num5 = num10;
			}
		}

		public static ulong GetScratchDataSizeInDwords(uint size)
		{
			uint num = 16U * Common.CeilDivide(size, 1024U);
			return 0UL + (ulong)num + (ulong)(2U * size + 1024U) + Scan.GetScratchDataSizeInDwords(num);
		}

		private readonly ComputeShader shaderBitHistogram;

		private readonly int kernelBitHistogram;

		private readonly ComputeShader shaderScatter;

		private readonly int kernelScatter;

		private readonly Scan scan;

		private const uint kKeysPerThread = 4U;

		private const uint kGroupSize = 256U;

		private const uint kKeysPerGroup = 1024U;

		private const int kNumBitsPerPass = 4;
	}
}
