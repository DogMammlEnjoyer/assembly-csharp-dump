using System;
using Unity.Collections;
using Unity.Mathematics;

namespace UnityEngine.Rendering.Universal
{
	internal class DecalDrawCallChunk : DecalChunk
	{
		public int subCallCount
		{
			get
			{
				return this.subCallCounts[0];
			}
			set
			{
				this.subCallCounts[0] = value;
			}
		}

		public override void RemoveAtSwapBack(int entityIndex)
		{
			base.RemoveAtSwapBack<float4x4>(ref this.decalToWorlds, entityIndex, base.count);
			base.RemoveAtSwapBack<float4x4>(ref this.normalToDecals, entityIndex, base.count);
			base.RemoveAtSwapBack<float>(ref this.renderingLayerMasks, entityIndex, base.count);
			base.RemoveAtSwapBack<DecalSubDrawCall>(ref this.subCalls, entityIndex, base.count);
			int count = base.count;
			base.count = count - 1;
		}

		public override void SetCapacity(int newCapacity)
		{
			ref this.decalToWorlds.ResizeArray(newCapacity);
			ref this.normalToDecals.ResizeArray(newCapacity);
			ref this.renderingLayerMasks.ResizeArray(newCapacity);
			ref this.subCalls.ResizeArray(newCapacity);
			base.capacity = newCapacity;
		}

		public override void Dispose()
		{
			this.subCallCounts.Dispose();
			if (base.capacity == 0)
			{
				return;
			}
			this.decalToWorlds.Dispose();
			this.normalToDecals.Dispose();
			this.renderingLayerMasks.Dispose();
			this.subCalls.Dispose();
			base.count = 0;
			base.capacity = 0;
		}

		public NativeArray<float4x4> decalToWorlds;

		public NativeArray<float4x4> normalToDecals;

		public NativeArray<float> renderingLayerMasks;

		public NativeArray<DecalSubDrawCall> subCalls;

		public NativeArray<int> subCallCounts;
	}
}
