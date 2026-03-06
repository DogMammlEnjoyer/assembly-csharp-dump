using System;
using Unity.Collections;

namespace UnityEngine.Rendering.Universal
{
	internal class DecalCulledChunk : DecalChunk
	{
		public override void RemoveAtSwapBack(int entityIndex)
		{
			base.RemoveAtSwapBack<int>(ref this.visibleDecalIndexArray, entityIndex, base.count);
			base.RemoveAtSwapBack<int>(ref this.visibleDecalIndices, entityIndex, base.count);
			int count = base.count;
			base.count = count - 1;
		}

		public override void SetCapacity(int newCapacity)
		{
			ArrayExtensions.ResizeArray<int>(ref this.visibleDecalIndexArray, newCapacity);
			ref this.visibleDecalIndices.ResizeArray(newCapacity);
			if (this.cullingGroups == null)
			{
				this.cullingGroups = new CullingGroup();
			}
			base.capacity = newCapacity;
		}

		public override void Dispose()
		{
			if (base.capacity == 0)
			{
				return;
			}
			this.visibleDecalIndices.Dispose();
			this.visibleDecalIndexArray = null;
			base.count = 0;
			base.capacity = 0;
			this.cullingGroups.Dispose();
			this.cullingGroups = null;
		}

		public Vector3 cameraPosition;

		public ulong sceneCullingMask;

		public int cullingMask;

		public CullingGroup cullingGroups;

		public int[] visibleDecalIndexArray;

		public NativeArray<int> visibleDecalIndices;

		public int visibleDecalCount;
	}
}
