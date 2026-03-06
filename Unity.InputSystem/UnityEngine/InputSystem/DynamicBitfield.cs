using System;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem
{
	internal struct DynamicBitfield
	{
		public void SetLength(int newLength)
		{
			int num = DynamicBitfield.BitCountToULongCount(newLength);
			if (this.array.length < num)
			{
				this.array.SetLength(num);
			}
			this.length = newLength;
		}

		public void SetBit(int bitIndex)
		{
			ref InlinedArray<ulong> ptr = ref this.array;
			int index = bitIndex / 64;
			ptr[index] |= 1UL << bitIndex % 64;
		}

		public bool TestBit(int bitIndex)
		{
			return (this.array[bitIndex / 64] & 1UL << bitIndex % 64) > 0UL;
		}

		public void ClearBit(int bitIndex)
		{
			ref InlinedArray<ulong> ptr = ref this.array;
			int index = bitIndex / 64;
			ptr[index] &= ~(1UL << bitIndex % 64);
		}

		public bool AnyBitIsSet()
		{
			for (int i = 0; i < this.array.length; i++)
			{
				if (this.array[i] != 0UL)
				{
					return true;
				}
			}
			return false;
		}

		private static int BitCountToULongCount(int bitCount)
		{
			return (bitCount + 63) / 64;
		}

		public InlinedArray<ulong> array;

		public int length;
	}
}
