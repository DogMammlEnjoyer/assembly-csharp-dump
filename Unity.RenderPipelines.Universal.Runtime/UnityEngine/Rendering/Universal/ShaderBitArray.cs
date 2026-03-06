using System;
using System.Text;

namespace UnityEngine.Rendering.Universal
{
	internal struct ShaderBitArray
	{
		public int elemLength
		{
			get
			{
				if (this.m_Data != null)
				{
					return this.m_Data.Length;
				}
				return 0;
			}
		}

		public int bitCapacity
		{
			get
			{
				return this.elemLength * 32;
			}
		}

		public float[] data
		{
			get
			{
				return this.m_Data;
			}
		}

		public void Resize(int bitCount)
		{
			if (this.bitCapacity > bitCount)
			{
				return;
			}
			int num = (bitCount + 31) / 32;
			int num2 = num;
			float[] data = this.m_Data;
			int? num3 = (data != null) ? new int?(data.Length) : null;
			if (num2 == num3.GetValueOrDefault() & num3 != null)
			{
				return;
			}
			float[] array = new float[num];
			if (this.m_Data != null)
			{
				for (int i = 0; i < this.m_Data.Length; i++)
				{
					array[i] = this.m_Data[i];
				}
			}
			this.m_Data = array;
		}

		public void Clear()
		{
			for (int i = 0; i < this.m_Data.Length; i++)
			{
				this.m_Data[i] = 0f;
			}
		}

		private void GetElementIndexAndBitOffset(int index, out int elemIndex, out int bitOffset)
		{
			elemIndex = index >> 5;
			bitOffset = (index & 31);
		}

		public unsafe bool this[int index]
		{
			get
			{
				int num;
				int num2;
				this.GetElementIndexAndBitOffset(index, out num, out num2);
				float[] data;
				float* ptr;
				if ((data = this.m_Data) == null || data.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &data[0];
				}
				uint* ptr2 = (uint*)(ptr + num);
				return (*ptr2 & 1U << num2) > 0U;
			}
			set
			{
				int num;
				int num2;
				this.GetElementIndexAndBitOffset(index, out num, out num2);
				float[] array;
				float* ptr;
				if ((array = this.m_Data) == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				uint* ptr2 = (uint*)(ptr + num);
				if (value)
				{
					*ptr2 |= 1U << num2;
				}
				else
				{
					*ptr2 &= ~(1U << num2);
				}
				array = null;
			}
		}

		public unsafe override string ToString()
		{
			int num = Math.Min(this.bitCapacity, 4096);
			byte* ptr = stackalloc byte[(UIntPtr)num];
			for (int i = 0; i < num; i++)
			{
				ptr[i] = (this[i] ? 49 : 48);
			}
			return new string((sbyte*)ptr, 0, num, Encoding.UTF8);
		}

		private const int k_BitsPerElement = 32;

		private const int k_ElementShift = 5;

		private const int k_ElementMask = 31;

		private float[] m_Data;
	}
}
