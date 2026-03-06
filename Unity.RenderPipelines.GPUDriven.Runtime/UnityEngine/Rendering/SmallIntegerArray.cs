using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.Rendering
{
	internal struct SmallIntegerArray : IDisposable
	{
		public bool Valid { readonly get; private set; }

		public SmallIntegerArray(int length, Allocator allocator)
		{
			this.m_FixedArray = default(FixedList32Bytes<int>);
			this.m_List = default(UnsafeList<int>);
			this.Length = length;
			this.Valid = true;
			if (this.Length <= this.m_FixedArray.Capacity)
			{
				this.m_FixedArray = default(FixedList32Bytes<int>);
				this.m_FixedArray.Length = this.Length;
				this.m_IsEmbedded = true;
				return;
			}
			this.m_List = new UnsafeList<int>(this.Length, allocator, NativeArrayOptions.UninitializedMemory);
			this.m_List.Resize(this.Length, NativeArrayOptions.UninitializedMemory);
			this.m_IsEmbedded = false;
		}

		public int this[int index]
		{
			get
			{
				if (this.m_IsEmbedded)
				{
					return this.m_FixedArray[index];
				}
				return this.m_List[index];
			}
			set
			{
				if (this.m_IsEmbedded)
				{
					this.m_FixedArray[index] = value;
					return;
				}
				this.m_List[index] = value;
			}
		}

		public void Dispose()
		{
			if (!this.Valid)
			{
				return;
			}
			this.m_List.Dispose();
			this.Valid = false;
		}

		private FixedList32Bytes<int> m_FixedArray;

		private UnsafeList<int> m_List;

		private readonly bool m_IsEmbedded;

		public readonly int Length;
	}
}
