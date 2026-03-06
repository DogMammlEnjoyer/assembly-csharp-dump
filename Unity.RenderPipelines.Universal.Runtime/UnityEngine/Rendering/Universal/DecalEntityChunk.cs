using System;
using Unity.Collections;
using UnityEngine.Jobs;

namespace UnityEngine.Rendering.Universal
{
	internal class DecalEntityChunk : DecalChunk
	{
		public override void Push()
		{
			int count = base.count;
			base.count = count + 1;
		}

		public override void RemoveAtSwapBack(int entityIndex)
		{
			base.RemoveAtSwapBack<DecalEntity>(ref this.decalEntities, entityIndex, base.count);
			base.RemoveAtSwapBack<DecalProjector>(ref this.decalProjectors, entityIndex, base.count);
			this.transformAccessArray.RemoveAtSwapBack(entityIndex);
			int count = base.count;
			base.count = count - 1;
		}

		public override void SetCapacity(int newCapacity)
		{
			ref this.decalEntities.ResizeArray(newCapacity);
			base.ResizeNativeArray(ref this.transformAccessArray, this.decalProjectors, newCapacity);
			ArrayExtensions.ResizeArray<DecalProjector>(ref this.decalProjectors, newCapacity);
			base.capacity = newCapacity;
		}

		public override void Dispose()
		{
			if (base.capacity == 0)
			{
				return;
			}
			this.decalEntities.Dispose();
			this.transformAccessArray.Dispose();
			this.decalProjectors = null;
			base.count = 0;
			base.capacity = 0;
		}

		public Material material;

		public NativeArray<DecalEntity> decalEntities;

		public DecalProjector[] decalProjectors;

		public TransformAccessArray transformAccessArray;
	}
}
