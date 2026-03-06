using System;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering
{
	public struct GPUSort
	{
		public GPUSort(GPUSort.SystemResources resources)
		{
			this.resources = resources;
			this.m_Keywords = new LocalKeyword[]
			{
				new LocalKeyword(resources.computeAsset, "STAGE_BMS"),
				new LocalKeyword(resources.computeAsset, "STAGE_LOCAL_DISPERSE"),
				new LocalKeyword(resources.computeAsset, "STAGE_BIG_FLIP"),
				new LocalKeyword(resources.computeAsset, "STAGE_BIG_DISPERSE")
			};
		}

		private void DispatchStage(CommandBuffer cmd, GPUSort.Args args, uint h, GPUSort.Stage stage)
		{
			using (new ProfilingScope(cmd, ProfilingSampler.Get<GPUSort.Stage>(stage)))
			{
				foreach (LocalKeyword localKeyword in this.m_Keywords)
				{
					cmd.SetKeyword(this.resources.computeAsset, localKeyword, false);
				}
				cmd.SetKeyword(this.resources.computeAsset, this.m_Keywords[(int)stage], true);
				cmd.SetComputeIntParam(this.resources.computeAsset, "_H", (int)h);
				cmd.SetComputeIntParam(this.resources.computeAsset, "_Total", (int)args.count);
				cmd.SetComputeBufferParam(this.resources.computeAsset, 0, "_KeyBuffer", args.resources.sortBufferKeys);
				cmd.SetComputeBufferParam(this.resources.computeAsset, 0, "_ValueBuffer", args.resources.sortBufferValues);
				cmd.DispatchCompute(this.resources.computeAsset, 0, args.workGroupCount, 1, 1);
			}
		}

		private void CopyBuffer(CommandBuffer cmd, GraphicsBuffer src, GraphicsBuffer dst)
		{
			foreach (LocalKeyword localKeyword in this.m_Keywords)
			{
				cmd.SetKeyword(this.resources.computeAsset, localKeyword, false);
			}
			int num = src.count * src.stride / 4;
			cmd.SetComputeBufferParam(this.resources.computeAsset, 1, "_CopySrcBuffer", src);
			cmd.SetComputeBufferParam(this.resources.computeAsset, 1, "_CopyDstBuffer", dst);
			cmd.SetComputeIntParam(this.resources.computeAsset, "_CopyEntriesCount", num);
			cmd.DispatchCompute(this.resources.computeAsset, 1, (num + 63) / 64, 1, 1);
		}

		internal static int DivRoundUp(int x, int y)
		{
			return (x + y - 1) / y;
		}

		public void Dispatch(CommandBuffer cmd, GPUSort.Args args)
		{
			uint count = args.count;
			this.CopyBuffer(cmd, args.inputKeys, args.resources.sortBufferKeys);
			this.CopyBuffer(cmd, args.inputValues, args.resources.sortBufferValues);
			args.workGroupCount = Math.Max(1, GPUSort.DivRoundUp((int)count, 2048));
			uint num = Math.Min(2048U, args.maxDepth);
			this.DispatchStage(cmd, args, num, GPUSort.Stage.LocalBMS);
			for (num *= 2U; num <= Math.Min(count, args.maxDepth); num *= 2U)
			{
				this.DispatchStage(cmd, args, num, GPUSort.Stage.BigFlip);
				for (uint num2 = num / 2U; num2 > 1U; num2 /= 2U)
				{
					if (num2 <= 2048U)
					{
						this.DispatchStage(cmd, args, num2, GPUSort.Stage.LocalDisperse);
						break;
					}
					this.DispatchStage(cmd, args, num2, GPUSort.Stage.BigDisperse);
				}
			}
		}

		private const uint kWorkGroupSize = 1024U;

		private LocalKeyword[] m_Keywords;

		private GPUSort.SystemResources resources;

		private enum Stage
		{
			LocalBMS,
			LocalDisperse,
			BigFlip,
			BigDisperse
		}

		public struct Args
		{
			public uint count;

			public uint maxDepth;

			public GraphicsBuffer inputKeys;

			public GraphicsBuffer inputValues;

			public GPUSort.SupportResources resources;

			internal int workGroupCount;
		}

		public struct RenderGraphResources
		{
			public static GPUSort.RenderGraphResources Create(int count, RenderGraph renderGraph, RenderGraphBuilder builder)
			{
				GraphicsBuffer.Target target = GraphicsBuffer.Target.CopyDestination | GraphicsBuffer.Target.Raw;
				GPUSort.RenderGraphResources result = default(GPUSort.RenderGraphResources);
				BufferDesc bufferDesc = new BufferDesc(count, 4, target);
				bufferDesc.name = "Keys";
				result.sortBufferKeys = builder.CreateTransientBuffer(bufferDesc);
				BufferDesc bufferDesc2 = new BufferDesc(count, 4, target);
				bufferDesc2.name = "Values";
				result.sortBufferValues = builder.CreateTransientBuffer(bufferDesc2);
				return result;
			}

			public BufferHandle sortBufferKeys;

			public BufferHandle sortBufferValues;
		}

		public struct SupportResources
		{
			public static GPUSort.SupportResources Load(GPUSort.RenderGraphResources renderGraphResources)
			{
				return new GPUSort.SupportResources
				{
					sortBufferKeys = renderGraphResources.sortBufferKeys,
					sortBufferValues = renderGraphResources.sortBufferValues
				};
			}

			public void Dispose()
			{
				if (this.sortBufferKeys != null)
				{
					this.sortBufferKeys.Dispose();
					this.sortBufferKeys = null;
				}
				if (this.sortBufferValues != null)
				{
					this.sortBufferValues.Dispose();
					this.sortBufferValues = null;
				}
			}

			public GraphicsBuffer sortBufferKeys;

			public GraphicsBuffer sortBufferValues;
		}

		public struct SystemResources
		{
			public ComputeShader computeAsset;
		}
	}
}
