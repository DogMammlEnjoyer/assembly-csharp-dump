using System;

namespace UnityEngine.Rendering
{
	internal struct EditorInstanceDataArrays : IDataArrays
	{
		public void Initialize(int initCapacity)
		{
		}

		public void Dispose()
		{
		}

		public void Grow(int newCapacity)
		{
		}

		public void Remove(int index, int lastIndex)
		{
		}

		public void SetDefault(int index)
		{
		}

		internal readonly struct ReadOnly
		{
			public ReadOnly(in CPUInstanceData instanceData)
			{
			}
		}
	}
}
