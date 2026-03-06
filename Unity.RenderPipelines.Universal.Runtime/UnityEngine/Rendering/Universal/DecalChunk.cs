using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.Jobs;

namespace UnityEngine.Rendering.Universal
{
	internal abstract class DecalChunk : IDisposable
	{
		public int count { get; protected set; }

		public int capacity { get; protected set; }

		public JobHandle currentJobHandle { get; set; }

		public virtual void Push()
		{
			int count = this.count;
			this.count = count + 1;
		}

		public abstract void RemoveAtSwapBack(int index);

		public abstract void SetCapacity(int capacity);

		public virtual void Dispose()
		{
		}

		protected void ResizeNativeArray(ref TransformAccessArray array, DecalProjector[] decalProjectors, int capacity)
		{
			TransformAccessArray transformAccessArray = new TransformAccessArray(capacity, -1);
			if (array.isCreated)
			{
				for (int i = 0; i < array.length; i++)
				{
					transformAccessArray.Add(decalProjectors[i].transform);
				}
				array.Dispose();
			}
			array = transformAccessArray;
		}

		protected void RemoveAtSwapBack<T>(ref NativeArray<T> array, int index, int count) where T : struct
		{
			array[index] = array[count - 1];
		}

		protected void RemoveAtSwapBack<T>(ref T[] array, int index, int count)
		{
			array[index] = array[count - 1];
		}
	}
}
