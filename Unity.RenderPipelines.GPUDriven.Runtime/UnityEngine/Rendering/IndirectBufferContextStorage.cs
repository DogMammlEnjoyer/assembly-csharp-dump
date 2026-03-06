using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering
{
	internal struct IndirectBufferContextStorage : IDisposable
	{
		public GraphicsBuffer instanceBuffer
		{
			get
			{
				return this.m_InstanceBuffer;
			}
		}

		public GraphicsBuffer instanceInfoBuffer
		{
			get
			{
				return this.m_InstanceInfoBuffer;
			}
		}

		public GraphicsBuffer argsBuffer
		{
			get
			{
				return this.m_ArgsBuffer;
			}
		}

		public GraphicsBuffer drawInfoBuffer
		{
			get
			{
				return this.m_DrawInfoBuffer;
			}
		}

		public GraphicsBufferHandle visibleInstanceBufferHandle
		{
			get
			{
				return this.m_InstanceBuffer.bufferHandle;
			}
		}

		public GraphicsBufferHandle indirectArgsBufferHandle
		{
			get
			{
				return this.m_ArgsBuffer.bufferHandle;
			}
		}

		public IndirectBufferContextHandles ImportBuffers(RenderGraph renderGraph)
		{
			return new IndirectBufferContextHandles
			{
				instanceBuffer = renderGraph.ImportBuffer(this.m_InstanceBuffer, false),
				instanceInfoBuffer = renderGraph.ImportBuffer(this.m_InstanceInfoBuffer, false),
				argsBuffer = renderGraph.ImportBuffer(this.m_ArgsBuffer, false),
				drawInfoBuffer = renderGraph.ImportBuffer(this.m_DrawInfoBuffer, false)
			};
		}

		public NativeArray<IndirectInstanceInfo> instanceInfoGlobalArray
		{
			get
			{
				return this.m_InstanceInfoStaging;
			}
		}

		public NativeArray<IndirectDrawInfo> drawInfoGlobalArray
		{
			get
			{
				return this.m_DrawInfoStaging;
			}
		}

		public NativeArray<int> allocationCounters
		{
			get
			{
				return this.m_AllocationCounters;
			}
		}

		public void Init()
		{
			int num = 256;
			int maxInstanceCount = 64 * num;
			int num2 = 8;
			this.AllocateInstanceBuffers(maxInstanceCount);
			this.AllocateDrawBuffers(num);
			this.m_ContextIndexFromViewID = new NativeHashMap<int, int>(num2, Allocator.Persistent);
			this.m_Contexts = new NativeList<IndirectBufferContext>(num2, Allocator.Persistent);
			this.m_ContextAllocInfo = new NativeArray<IndirectBufferAllocInfo>(num2, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			this.m_AllocationCounters = new NativeArray<int>(2, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			this.ResetAllocators();
		}

		private void AllocateInstanceBuffers(int maxInstanceCount)
		{
			this.m_InstanceBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Raw, maxInstanceCount, 4);
			this.m_InstanceInfoBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 2 * maxInstanceCount, Marshal.SizeOf<IndirectInstanceInfo>());
			this.m_InstanceInfoStaging = new NativeArray<IndirectInstanceInfo>(maxInstanceCount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			this.m_BufferLimits.maxInstanceCount = maxInstanceCount;
		}

		private void FreeInstanceBuffers()
		{
			this.m_InstanceBuffer.Release();
			this.m_InstanceInfoBuffer.Release();
			this.m_InstanceInfoStaging.Dispose();
			this.m_BufferLimits.maxInstanceCount = 0;
		}

		private void AllocateDrawBuffers(int maxDrawCount)
		{
			this.m_ArgsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured | GraphicsBuffer.Target.IndirectArguments, (maxDrawCount + 1) * 5, 4);
			this.m_DrawInfoBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, maxDrawCount, Marshal.SizeOf<IndirectDrawInfo>());
			this.m_DrawInfoStaging = new NativeArray<IndirectDrawInfo>(maxDrawCount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			this.m_BufferLimits.maxDrawCount = maxDrawCount;
		}

		private void FreeDrawBuffers()
		{
			this.m_ArgsBuffer.Release();
			this.m_DrawInfoBuffer.Release();
			this.m_DrawInfoStaging.Dispose();
			this.m_BufferLimits.maxDrawCount = 0;
		}

		public void Dispose()
		{
			this.SyncContexts();
			this.FreeInstanceBuffers();
			this.FreeDrawBuffers();
			this.m_ContextIndexFromViewID.Dispose();
			this.m_Contexts.Dispose();
			this.m_ContextAllocInfo.Dispose();
			this.m_AllocationCounters.Dispose();
		}

		private void SyncContexts()
		{
			for (int i = 0; i < this.m_Contexts.Length; i++)
			{
				this.m_Contexts[i].cullingJobHandle.Complete();
			}
		}

		private void ResetAllocators()
		{
			this.m_ContextAllocCounter = 0;
			this.m_ContextIndexFromViewID.Clear();
			this.m_Contexts.Clear();
			int num = 0;
			ref this.m_AllocationCounters.FillArray(num, 0, -1);
		}

		private void GrowBuffers()
		{
			if (this.m_ContextAllocCounter > this.m_ContextAllocInfo.Length)
			{
				int num = this.m_ContextAllocCounter * 6 / 5;
				this.m_Contexts.Clear();
				this.m_Contexts.SetCapacity(num);
				this.m_ContextAllocInfo.Dispose();
				this.m_ContextAllocInfo = new NativeArray<IndirectBufferAllocInfo>(num, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			}
			int num2 = this.m_AllocationCounters[0];
			if (num2 > this.m_BufferLimits.maxInstanceCount)
			{
				int maxInstanceCount = num2 * 6 / 5;
				this.FreeInstanceBuffers();
				this.AllocateInstanceBuffers(maxInstanceCount);
			}
			int num3 = this.m_AllocationCounters[1];
			if (num3 > this.m_BufferLimits.maxDrawCount)
			{
				int maxDrawCount = num3 * 6 / 5;
				this.FreeDrawBuffers();
				this.AllocateDrawBuffers(maxDrawCount);
			}
		}

		public void ClearContextsAndGrowBuffers()
		{
			this.SyncContexts();
			this.GrowBuffers();
			this.ResetAllocators();
		}

		public int TryAllocateContext(int viewID)
		{
			if (this.m_ContextIndexFromViewID.ContainsKey(viewID))
			{
				return -1;
			}
			int num = -1;
			this.m_ContextAllocCounter++;
			if (this.m_Contexts.Length < this.m_ContextAllocInfo.Length)
			{
				num = this.m_Contexts.Length;
				IndirectBufferContext indirectBufferContext = default(IndirectBufferContext);
				this.m_Contexts.Add(indirectBufferContext);
				this.m_ContextIndexFromViewID.Add(viewID, num);
			}
			return num;
		}

		public int TryGetContextIndex(int viewID)
		{
			int result;
			if (!this.m_ContextIndexFromViewID.TryGetValue(viewID, out result))
			{
				result = -1;
			}
			return result;
		}

		public NativeArray<IndirectBufferAllocInfo> GetAllocInfoSubArray(int contextIndex)
		{
			int start = Mathf.Max(contextIndex, 0);
			return this.m_ContextAllocInfo.GetSubArray(start, 1);
		}

		public IndirectBufferAllocInfo GetAllocInfo(int contextIndex)
		{
			IndirectBufferAllocInfo result = default(IndirectBufferAllocInfo);
			if (0 <= contextIndex && contextIndex < this.m_Contexts.Length)
			{
				result = this.m_ContextAllocInfo[contextIndex];
			}
			return result;
		}

		public void CopyFromStaging(CommandBuffer cmd, in IndirectBufferAllocInfo allocInfo)
		{
			IndirectBufferAllocInfo indirectBufferAllocInfo = allocInfo;
			if (!indirectBufferAllocInfo.IsEmpty())
			{
				cmd.SetBufferData<IndirectDrawInfo>(this.m_DrawInfoBuffer, this.m_DrawInfoStaging, allocInfo.drawAllocIndex, allocInfo.drawAllocIndex, allocInfo.drawCount);
				cmd.SetBufferData<IndirectInstanceInfo>(this.m_InstanceInfoBuffer, this.m_InstanceInfoStaging, allocInfo.instanceAllocIndex, 2 * allocInfo.instanceAllocIndex, allocInfo.instanceCount);
			}
		}

		public IndirectBufferLimits GetLimits(int contextIndex)
		{
			IndirectBufferLimits result = default(IndirectBufferLimits);
			if (contextIndex >= 0)
			{
				result = this.m_BufferLimits;
			}
			return result;
		}

		public IndirectBufferContext GetBufferContext(int contextIndex)
		{
			IndirectBufferContext result = default(IndirectBufferContext);
			if (0 <= contextIndex && contextIndex < this.m_Contexts.Length)
			{
				result = this.m_Contexts[contextIndex];
			}
			return result;
		}

		public void SetBufferContext(int contextIndex, IndirectBufferContext ctx)
		{
			if (0 <= contextIndex && contextIndex < this.m_Contexts.Length)
			{
				this.m_Contexts[contextIndex] = ctx;
			}
		}

		private const int kAllocatorCount = 2;

		internal const int kExtraDrawAllocationCount = 1;

		internal const int kInstanceInfoGpuOffsetMultiplier = 2;

		private IndirectBufferLimits m_BufferLimits;

		private GraphicsBuffer m_InstanceBuffer;

		private GraphicsBuffer m_InstanceInfoBuffer;

		private NativeArray<IndirectInstanceInfo> m_InstanceInfoStaging;

		private GraphicsBuffer m_ArgsBuffer;

		private GraphicsBuffer m_DrawInfoBuffer;

		private NativeArray<IndirectDrawInfo> m_DrawInfoStaging;

		private int m_ContextAllocCounter;

		private NativeHashMap<int, int> m_ContextIndexFromViewID;

		private NativeList<IndirectBufferContext> m_Contexts;

		private NativeArray<IndirectBufferAllocInfo> m_ContextAllocInfo;

		private NativeArray<int> m_AllocationCounters;
	}
}
