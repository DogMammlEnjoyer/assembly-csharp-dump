using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering
{
	public struct GPUPrefixSum
	{
		public GPUPrefixSum(GPUPrefixSum.SystemResources resources)
		{
			this.resources = resources;
			this.resources.LoadKernels();
		}

		private unsafe Vector4 PackPrefixSumArgs(int a, int b, int c, int d)
		{
			return new Vector4(*(float*)(&a), *(float*)(&b), *(float*)(&c), *(float*)(&d));
		}

		internal void ExecuteCommonIndirect(CommandBuffer cmdBuffer, GraphicsBuffer inputBuffer, in GPUPrefixSum.SupportResources supportResources, bool isExclusive)
		{
			int kernelIndex = isExclusive ? this.resources.kernelPrefixSumOnGroupExclusive : this.resources.kernelPrefixSumOnGroup;
			int kernelIndex2 = isExclusive ? this.resources.kernelPrefixSumResolveParentExclusive : this.resources.kernelPrefixSumResolveParent;
			for (int i = 0; i < supportResources.maxLevelCount; i++)
			{
				Vector4 val = this.PackPrefixSumArgs(0, 0, 0, i);
				cmdBuffer.SetComputeVectorParam(this.resources.computeAsset, GPUPrefixSum.ShaderIDs._PrefixSumIntArgs, val);
				if (i == 0)
				{
					cmdBuffer.SetComputeBufferParam(this.resources.computeAsset, kernelIndex, GPUPrefixSum.ShaderIDs._InputBuffer, inputBuffer);
				}
				else
				{
					cmdBuffer.SetComputeBufferParam(this.resources.computeAsset, kernelIndex, GPUPrefixSum.ShaderIDs._InputBuffer, supportResources.prefixBuffer1);
				}
				cmdBuffer.SetComputeBufferParam(this.resources.computeAsset, kernelIndex, GPUPrefixSum.ShaderIDs._TotalLevelsBuffer, supportResources.totalLevelCountBuffer);
				cmdBuffer.SetComputeBufferParam(this.resources.computeAsset, kernelIndex, GPUPrefixSum.ShaderIDs._LevelsOffsetsBuffer, supportResources.levelOffsetBuffer);
				cmdBuffer.SetComputeBufferParam(this.resources.computeAsset, kernelIndex, GPUPrefixSum.ShaderIDs._OutputBuffer, supportResources.prefixBuffer0);
				cmdBuffer.DispatchCompute(this.resources.computeAsset, kernelIndex, supportResources.indirectDispatchArgsBuffer, (uint)(i * 16 * 4));
				if (i != supportResources.maxLevelCount - 1)
				{
					cmdBuffer.SetComputeBufferParam(this.resources.computeAsset, this.resources.kernelPrefixSumNextInput, GPUPrefixSum.ShaderIDs._InputBuffer, supportResources.prefixBuffer0);
					cmdBuffer.SetComputeBufferParam(this.resources.computeAsset, this.resources.kernelPrefixSumNextInput, GPUPrefixSum.ShaderIDs._LevelsOffsetsBuffer, supportResources.levelOffsetBuffer);
					cmdBuffer.SetComputeBufferParam(this.resources.computeAsset, this.resources.kernelPrefixSumNextInput, GPUPrefixSum.ShaderIDs._OutputBuffer, supportResources.prefixBuffer1);
					cmdBuffer.DispatchCompute(this.resources.computeAsset, this.resources.kernelPrefixSumNextInput, supportResources.indirectDispatchArgsBuffer, (uint)((i + 1) * 16 * 4));
				}
			}
			for (int j = supportResources.maxLevelCount - 1; j >= 1; j--)
			{
				Vector4 val2 = this.PackPrefixSumArgs(0, 0, 0, j);
				cmdBuffer.SetComputeVectorParam(this.resources.computeAsset, GPUPrefixSum.ShaderIDs._PrefixSumIntArgs, val2);
				cmdBuffer.SetComputeBufferParam(this.resources.computeAsset, kernelIndex2, GPUPrefixSum.ShaderIDs._InputBuffer, inputBuffer);
				cmdBuffer.SetComputeBufferParam(this.resources.computeAsset, kernelIndex2, GPUPrefixSum.ShaderIDs._OutputBuffer, supportResources.prefixBuffer0);
				cmdBuffer.SetComputeBufferParam(this.resources.computeAsset, kernelIndex2, GPUPrefixSum.ShaderIDs._LevelsOffsetsBuffer, supportResources.levelOffsetBuffer);
				cmdBuffer.DispatchCompute(this.resources.computeAsset, kernelIndex2, supportResources.indirectDispatchArgsBuffer, (uint)(((j - 1) * 16 + 8) * 4));
			}
		}

		public void DispatchDirect(CommandBuffer cmdBuffer, in GPUPrefixSum.DirectArgs arguments)
		{
			if (arguments.supportResources.prefixBuffer0 == null || arguments.supportResources.prefixBuffer1 == null)
			{
				throw new Exception("Support resources are not valid.");
			}
			if (arguments.input == null)
			{
				throw new Exception("Input source buffer cannot be null.");
			}
			if (arguments.inputCount > arguments.supportResources.alignedElementCount)
			{
				throw new Exception("Input count exceeds maximum count of support resources. Ensure to create support resources with enough space.");
			}
			Vector4 val = this.PackPrefixSumArgs(arguments.inputCount, arguments.supportResources.maxLevelCount, 0, 0);
			cmdBuffer.SetComputeVectorParam(this.resources.computeAsset, GPUPrefixSum.ShaderIDs._PrefixSumIntArgs, val);
			cmdBuffer.SetComputeBufferParam(this.resources.computeAsset, this.resources.kernelCalculateLevelDispatchArgsFromConst, GPUPrefixSum.ShaderIDs._OutputLevelsOffsetsBuffer, arguments.supportResources.levelOffsetBuffer);
			cmdBuffer.SetComputeBufferParam(this.resources.computeAsset, this.resources.kernelCalculateLevelDispatchArgsFromConst, GPUPrefixSum.ShaderIDs._OutputDispatchLevelArgsBuffer, arguments.supportResources.indirectDispatchArgsBuffer);
			cmdBuffer.SetComputeBufferParam(this.resources.computeAsset, this.resources.kernelCalculateLevelDispatchArgsFromConst, GPUPrefixSum.ShaderIDs._OutputTotalLevelsBuffer, arguments.supportResources.totalLevelCountBuffer);
			cmdBuffer.DispatchCompute(this.resources.computeAsset, this.resources.kernelCalculateLevelDispatchArgsFromConst, 1, 1, 1);
			this.ExecuteCommonIndirect(cmdBuffer, arguments.input, arguments.supportResources, arguments.exclusive);
		}

		public void DispatchIndirect(CommandBuffer cmdBuffer, in GPUPrefixSum.IndirectDirectArgs arguments)
		{
			if (arguments.supportResources.prefixBuffer0 == null || arguments.supportResources.prefixBuffer1 == null)
			{
				throw new Exception("Support resources are not valid.");
			}
			if (arguments.input == null || arguments.inputCountBuffer == null)
			{
				throw new Exception("Input source buffer and inputCountBuffer cannot be null.");
			}
			Vector4 val = this.PackPrefixSumArgs(0, arguments.supportResources.maxLevelCount, arguments.inputCountBufferByteOffset, 0);
			cmdBuffer.SetComputeVectorParam(this.resources.computeAsset, GPUPrefixSum.ShaderIDs._PrefixSumIntArgs, val);
			cmdBuffer.SetComputeBufferParam(this.resources.computeAsset, this.resources.kernelCalculateLevelDispatchArgsFromBuffer, GPUPrefixSum.ShaderIDs._InputCountBuffer, arguments.inputCountBuffer);
			cmdBuffer.SetComputeBufferParam(this.resources.computeAsset, this.resources.kernelCalculateLevelDispatchArgsFromBuffer, GPUPrefixSum.ShaderIDs._OutputLevelsOffsetsBuffer, arguments.supportResources.levelOffsetBuffer);
			cmdBuffer.SetComputeBufferParam(this.resources.computeAsset, this.resources.kernelCalculateLevelDispatchArgsFromBuffer, GPUPrefixSum.ShaderIDs._OutputDispatchLevelArgsBuffer, arguments.supportResources.indirectDispatchArgsBuffer);
			cmdBuffer.SetComputeBufferParam(this.resources.computeAsset, this.resources.kernelCalculateLevelDispatchArgsFromBuffer, GPUPrefixSum.ShaderIDs._OutputTotalLevelsBuffer, arguments.supportResources.totalLevelCountBuffer);
			cmdBuffer.DispatchCompute(this.resources.computeAsset, this.resources.kernelCalculateLevelDispatchArgsFromBuffer, 1, 1, 1);
			this.ExecuteCommonIndirect(cmdBuffer, arguments.input, arguments.supportResources, arguments.exclusive);
		}

		private GPUPrefixSum.SystemResources resources;

		[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.core@04755ad51d99\\Runtime\\Utilities\\GPUPrefixSum\\GPUPrefixSum.Data.cs")]
		internal static class ShaderDefs
		{
			public static int DivUpGroup(int value)
			{
				return (value + 128 - 1) / 128;
			}

			public static int AlignUpGroup(int value)
			{
				return GPUPrefixSum.ShaderDefs.DivUpGroup(value) * 128;
			}

			public static void CalculateTotalBufferSize(int maxElementCount, out int totalSize, out int levelCounts)
			{
				int i = GPUPrefixSum.ShaderDefs.AlignUpGroup(maxElementCount);
				totalSize = i;
				levelCounts = 1;
				while (i > 128)
				{
					i = GPUPrefixSum.ShaderDefs.AlignUpGroup(GPUPrefixSum.ShaderDefs.DivUpGroup(i));
					totalSize += i;
					levelCounts++;
				}
			}

			public const int GroupSize = 128;

			public const int ArgsBufferStride = 16;

			public const int ArgsBufferUpper = 0;

			public const int ArgsBufferLower = 8;
		}

		[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.core@04755ad51d99\\Runtime\\Utilities\\GPUPrefixSum\\GPUPrefixSum.Data.cs")]
		public struct LevelOffsets
		{
			public uint count;

			public uint offset;

			public uint parentOffset;
		}

		public struct RenderGraphResources
		{
			public BufferHandle output
			{
				get
				{
					return this.prefixBuffer0;
				}
			}

			public static GPUPrefixSum.RenderGraphResources Create(int newMaxElementCount, RenderGraph renderGraph, RenderGraphBuilder builder, bool outputIsTemp = false)
			{
				GPUPrefixSum.RenderGraphResources result = default(GPUPrefixSum.RenderGraphResources);
				result.Initialize(newMaxElementCount, renderGraph, builder, outputIsTemp);
				return result;
			}

			private void Initialize(int newMaxElementCount, RenderGraph renderGraph, RenderGraphBuilder builder, bool outputIsTemp = false)
			{
				newMaxElementCount = Math.Max(newMaxElementCount, 1);
				int count;
				int num;
				GPUPrefixSum.ShaderDefs.CalculateTotalBufferSize(newMaxElementCount, out count, out num);
				BufferDesc bufferDesc = new BufferDesc(count, 4, GraphicsBuffer.Target.Raw)
				{
					name = "prefixBuffer0"
				};
				BufferDesc bufferDesc2 = bufferDesc;
				BufferHandle bufferHandle2;
				if (!outputIsTemp)
				{
					BufferHandle bufferHandle = renderGraph.CreateBuffer(bufferDesc2);
					bufferHandle2 = builder.WriteBuffer(bufferHandle);
				}
				else
				{
					bufferHandle2 = builder.CreateTransientBuffer(bufferDesc2);
				}
				this.prefixBuffer0 = bufferHandle2;
				bufferDesc = new BufferDesc(newMaxElementCount, 4, GraphicsBuffer.Target.Raw);
				bufferDesc.name = "prefixBuffer1";
				this.prefixBuffer1 = builder.CreateTransientBuffer(bufferDesc);
				bufferDesc = new BufferDesc(1, 4, GraphicsBuffer.Target.Raw);
				bufferDesc.name = "totalLevelCountBuffer";
				this.totalLevelCountBuffer = builder.CreateTransientBuffer(bufferDesc);
				bufferDesc = new BufferDesc(num, Marshal.SizeOf<GPUPrefixSum.LevelOffsets>(), GraphicsBuffer.Target.Structured);
				bufferDesc.name = "levelOffsetBuffer";
				this.levelOffsetBuffer = builder.CreateTransientBuffer(bufferDesc);
				bufferDesc = new BufferDesc(16 * num, 4, GraphicsBuffer.Target.Structured | GraphicsBuffer.Target.IndirectArguments);
				bufferDesc.name = "indirectDispatchArgsBuffer";
				this.indirectDispatchArgsBuffer = builder.CreateTransientBuffer(bufferDesc);
				this.alignedElementCount = GPUPrefixSum.ShaderDefs.AlignUpGroup(newMaxElementCount);
				this.maxBufferCount = count;
				this.maxLevelCount = num;
			}

			internal int alignedElementCount;

			internal int maxBufferCount;

			internal int maxLevelCount;

			internal BufferHandle prefixBuffer0;

			internal BufferHandle prefixBuffer1;

			internal BufferHandle totalLevelCountBuffer;

			internal BufferHandle levelOffsetBuffer;

			internal BufferHandle indirectDispatchArgsBuffer;
		}

		public struct SupportResources
		{
			public GraphicsBuffer output
			{
				get
				{
					return this.prefixBuffer0;
				}
			}

			public static GPUPrefixSum.SupportResources Create(int maxElementCount)
			{
				GPUPrefixSum.SupportResources result = new GPUPrefixSum.SupportResources
				{
					alignedElementCount = 0,
					ownsResources = true
				};
				result.Resize(maxElementCount);
				return result;
			}

			public static GPUPrefixSum.SupportResources Load(GPUPrefixSum.RenderGraphResources shaderGraphResources)
			{
				GPUPrefixSum.SupportResources result = new GPUPrefixSum.SupportResources
				{
					alignedElementCount = 0,
					ownsResources = false
				};
				result.LoadFromShaderGraph(shaderGraphResources);
				return result;
			}

			internal void Resize(int newMaxElementCount)
			{
				if (!this.ownsResources)
				{
					throw new Exception("Cannot resize resources unless they are owned. Use GpuPrefixSumSupportResources.Create() for this.");
				}
				newMaxElementCount = Math.Max(newMaxElementCount, 1);
				if (this.alignedElementCount >= newMaxElementCount)
				{
					return;
				}
				this.Dispose();
				int count;
				int num;
				GPUPrefixSum.ShaderDefs.CalculateTotalBufferSize(newMaxElementCount, out count, out num);
				this.alignedElementCount = GPUPrefixSum.ShaderDefs.AlignUpGroup(newMaxElementCount);
				this.maxBufferCount = count;
				this.maxLevelCount = num;
				this.prefixBuffer0 = new GraphicsBuffer(GraphicsBuffer.Target.Raw, count, 4);
				this.prefixBuffer1 = new GraphicsBuffer(GraphicsBuffer.Target.Raw, newMaxElementCount, 4);
				this.totalLevelCountBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Raw, 1, 4);
				this.levelOffsetBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, num, Marshal.SizeOf<GPUPrefixSum.LevelOffsets>());
				this.indirectDispatchArgsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 16 * num, 4);
			}

			private void LoadFromShaderGraph(GPUPrefixSum.RenderGraphResources shaderGraphResources)
			{
				this.alignedElementCount = shaderGraphResources.alignedElementCount;
				this.maxBufferCount = shaderGraphResources.maxBufferCount;
				this.maxLevelCount = shaderGraphResources.maxLevelCount;
				this.prefixBuffer0 = shaderGraphResources.prefixBuffer0;
				this.prefixBuffer1 = shaderGraphResources.prefixBuffer1;
				this.totalLevelCountBuffer = shaderGraphResources.totalLevelCountBuffer;
				this.levelOffsetBuffer = shaderGraphResources.levelOffsetBuffer;
				this.indirectDispatchArgsBuffer = shaderGraphResources.indirectDispatchArgsBuffer;
			}

			public void Dispose()
			{
				if (this.alignedElementCount == 0 || !this.ownsResources)
				{
					return;
				}
				this.alignedElementCount = 0;
				GPUPrefixSum.SupportResources.<Dispose>g__TryFreeBuffer|15_0(this.prefixBuffer0);
				GPUPrefixSum.SupportResources.<Dispose>g__TryFreeBuffer|15_0(this.prefixBuffer1);
				GPUPrefixSum.SupportResources.<Dispose>g__TryFreeBuffer|15_0(this.levelOffsetBuffer);
				GPUPrefixSum.SupportResources.<Dispose>g__TryFreeBuffer|15_0(this.indirectDispatchArgsBuffer);
				GPUPrefixSum.SupportResources.<Dispose>g__TryFreeBuffer|15_0(this.totalLevelCountBuffer);
			}

			[CompilerGenerated]
			internal static void <Dispose>g__TryFreeBuffer|15_0(GraphicsBuffer resource)
			{
				if (resource != null)
				{
					resource.Dispose();
					resource = null;
				}
			}

			internal bool ownsResources;

			internal int alignedElementCount;

			internal int maxBufferCount;

			internal int maxLevelCount;

			internal GraphicsBuffer prefixBuffer0;

			internal GraphicsBuffer prefixBuffer1;

			internal GraphicsBuffer totalLevelCountBuffer;

			internal GraphicsBuffer levelOffsetBuffer;

			internal GraphicsBuffer indirectDispatchArgsBuffer;
		}

		public struct DirectArgs
		{
			public bool exclusive;

			public int inputCount;

			public GraphicsBuffer input;

			public GPUPrefixSum.SupportResources supportResources;
		}

		public struct IndirectDirectArgs
		{
			public bool exclusive;

			public int inputCountBufferByteOffset;

			public ComputeBuffer inputCountBuffer;

			public GraphicsBuffer input;

			public GPUPrefixSum.SupportResources supportResources;
		}

		public struct SystemResources
		{
			internal void LoadKernels()
			{
				if (this.computeAsset == null)
				{
					return;
				}
				this.kernelCalculateLevelDispatchArgsFromConst = this.computeAsset.FindKernel("MainCalculateLevelDispatchArgsFromConst");
				this.kernelCalculateLevelDispatchArgsFromBuffer = this.computeAsset.FindKernel("MainCalculateLevelDispatchArgsFromBuffer");
				this.kernelPrefixSumOnGroup = this.computeAsset.FindKernel("MainPrefixSumOnGroup");
				this.kernelPrefixSumOnGroupExclusive = this.computeAsset.FindKernel("MainPrefixSumOnGroupExclusive");
				this.kernelPrefixSumNextInput = this.computeAsset.FindKernel("MainPrefixSumNextInput");
				this.kernelPrefixSumResolveParent = this.computeAsset.FindKernel("MainPrefixSumResolveParent");
				this.kernelPrefixSumResolveParentExclusive = this.computeAsset.FindKernel("MainPrefixSumResolveParentExclusive");
			}

			public ComputeShader computeAsset;

			internal int kernelCalculateLevelDispatchArgsFromConst;

			internal int kernelCalculateLevelDispatchArgsFromBuffer;

			internal int kernelPrefixSumOnGroup;

			internal int kernelPrefixSumOnGroupExclusive;

			internal int kernelPrefixSumNextInput;

			internal int kernelPrefixSumResolveParent;

			internal int kernelPrefixSumResolveParentExclusive;
		}

		private static class ShaderIDs
		{
			public static readonly int _InputBuffer = Shader.PropertyToID("_InputBuffer");

			public static readonly int _OutputBuffer = Shader.PropertyToID("_OutputBuffer");

			public static readonly int _InputCountBuffer = Shader.PropertyToID("_InputCountBuffer");

			public static readonly int _TotalLevelsBuffer = Shader.PropertyToID("_TotalLevelsBuffer");

			public static readonly int _OutputTotalLevelsBuffer = Shader.PropertyToID("_OutputTotalLevelsBuffer");

			public static readonly int _OutputDispatchLevelArgsBuffer = Shader.PropertyToID("_OutputDispatchLevelArgsBuffer");

			public static readonly int _LevelsOffsetsBuffer = Shader.PropertyToID("_LevelsOffsetsBuffer");

			public static readonly int _OutputLevelsOffsetsBuffer = Shader.PropertyToID("_OutputLevelsOffsetsBuffer");

			public static readonly int _PrefixSumIntArgs = Shader.PropertyToID("_PrefixSumIntArgs");
		}
	}
}
