using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fusion
{
	[NetworkStructWeaved(16)]
	[Serializable]
	[StructLayout(LayoutKind.Explicit)]
	public struct BitSet512 : INetworkStruct, IEquatable<BitSet512>, IEnumerable<int>, IEnumerable
	{
		public BitSet512.Iterator GetIterator()
		{
			return new BitSet512.Iterator(this);
		}

		public int Length
		{
			get
			{
				return 512;
			}
		}

		public unsafe static BitSet512 FromArray(ulong[] values)
		{
			bool flag = values == null;
			if (flag)
			{
				throw new ArgumentNullException("values");
			}
			bool flag2 = 8 != values.Length;
			if (flag2)
			{
				throw new ArgumentException("Array needs to be of length 8", "values");
			}
			BitSet512 result = default(BitSet512);
			for (int i = 0; i < 8; i++)
			{
				*(ref result.Bits.FixedElementField + (IntPtr)i * 8) = values[i];
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void Set(int bit)
		{
			Assert.Check(bit >= 0 && bit < 512);
			*(ref this.Bits.FixedElementField + (IntPtr)(bit / 64) * 8) |= 1UL << bit % 64;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void Clear(int bit)
		{
			Assert.Check(bit >= 0 && bit < 512);
			*(ref this.Bits.FixedElementField + (IntPtr)(bit / 64) * 8) &= ~(1UL << bit % 64);
		}

		public bool this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.IsSet(index);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				if (value)
				{
					this.Set(index);
				}
				else
				{
					this.Clear(index);
				}
			}
		}

		public unsafe void And(BitSet512 other)
		{
			this.Bits.FixedElementField = (this.Bits.FixedElementField & other.Bits.FixedElementField);
			*(ref this.Bits.FixedElementField + 8) &= *(ref other.Bits.FixedElementField + 8);
			*(ref this.Bits.FixedElementField + (IntPtr)2 * 8) &= *(ref other.Bits.FixedElementField + (IntPtr)2 * 8);
			*(ref this.Bits.FixedElementField + (IntPtr)3 * 8) &= *(ref other.Bits.FixedElementField + (IntPtr)3 * 8);
			*(ref this.Bits.FixedElementField + (IntPtr)4 * 8) &= *(ref other.Bits.FixedElementField + (IntPtr)4 * 8);
			*(ref this.Bits.FixedElementField + (IntPtr)5 * 8) &= *(ref other.Bits.FixedElementField + (IntPtr)5 * 8);
			*(ref this.Bits.FixedElementField + (IntPtr)6 * 8) &= *(ref other.Bits.FixedElementField + (IntPtr)6 * 8);
			*(ref this.Bits.FixedElementField + (IntPtr)7 * 8) &= *(ref other.Bits.FixedElementField + (IntPtr)7 * 8);
		}

		public unsafe void Or(BitSet512 other)
		{
			this.Bits.FixedElementField = (this.Bits.FixedElementField | other.Bits.FixedElementField);
			*(ref this.Bits.FixedElementField + 8) |= *(ref other.Bits.FixedElementField + 8);
			*(ref this.Bits.FixedElementField + (IntPtr)2 * 8) |= *(ref other.Bits.FixedElementField + (IntPtr)2 * 8);
			*(ref this.Bits.FixedElementField + (IntPtr)3 * 8) |= *(ref other.Bits.FixedElementField + (IntPtr)3 * 8);
			*(ref this.Bits.FixedElementField + (IntPtr)4 * 8) |= *(ref other.Bits.FixedElementField + (IntPtr)4 * 8);
			*(ref this.Bits.FixedElementField + (IntPtr)5 * 8) |= *(ref other.Bits.FixedElementField + (IntPtr)5 * 8);
			*(ref this.Bits.FixedElementField + (IntPtr)6 * 8) |= *(ref other.Bits.FixedElementField + (IntPtr)6 * 8);
			*(ref this.Bits.FixedElementField + (IntPtr)7 * 8) |= *(ref other.Bits.FixedElementField + (IntPtr)7 * 8);
		}

		public unsafe void Xor(BitSet512 other)
		{
			this.Bits.FixedElementField = (this.Bits.FixedElementField ^ other.Bits.FixedElementField);
			*(ref this.Bits.FixedElementField + 8) ^= *(ref other.Bits.FixedElementField + 8);
			*(ref this.Bits.FixedElementField + (IntPtr)2 * 8) ^= *(ref other.Bits.FixedElementField + (IntPtr)2 * 8);
			*(ref this.Bits.FixedElementField + (IntPtr)3 * 8) ^= *(ref other.Bits.FixedElementField + (IntPtr)3 * 8);
			*(ref this.Bits.FixedElementField + (IntPtr)4 * 8) ^= *(ref other.Bits.FixedElementField + (IntPtr)4 * 8);
			*(ref this.Bits.FixedElementField + (IntPtr)5 * 8) ^= *(ref other.Bits.FixedElementField + (IntPtr)5 * 8);
			*(ref this.Bits.FixedElementField + (IntPtr)6 * 8) ^= *(ref other.Bits.FixedElementField + (IntPtr)6 * 8);
			*(ref this.Bits.FixedElementField + (IntPtr)7 * 8) ^= *(ref other.Bits.FixedElementField + (IntPtr)7 * 8);
		}

		public unsafe void AndNot(BitSet512 other)
		{
			this.Bits.FixedElementField = (this.Bits.FixedElementField & ~other.Bits.FixedElementField);
			*(ref this.Bits.FixedElementField + 8) &= ~(*(ref other.Bits.FixedElementField + 8));
			*(ref this.Bits.FixedElementField + (IntPtr)2 * 8) &= ~(*(ref other.Bits.FixedElementField + (IntPtr)2 * 8));
			*(ref this.Bits.FixedElementField + (IntPtr)3 * 8) &= ~(*(ref other.Bits.FixedElementField + (IntPtr)3 * 8));
			*(ref this.Bits.FixedElementField + (IntPtr)4 * 8) &= ~(*(ref other.Bits.FixedElementField + (IntPtr)4 * 8));
			*(ref this.Bits.FixedElementField + (IntPtr)5 * 8) &= ~(*(ref other.Bits.FixedElementField + (IntPtr)5 * 8));
			*(ref this.Bits.FixedElementField + (IntPtr)6 * 8) &= ~(*(ref other.Bits.FixedElementField + (IntPtr)6 * 8));
			*(ref this.Bits.FixedElementField + (IntPtr)7 * 8) &= ~(*(ref other.Bits.FixedElementField + (IntPtr)7 * 8));
		}

		public unsafe void Not()
		{
			this.Bits.FixedElementField = ~this.Bits.FixedElementField;
			*(ref this.Bits.FixedElementField + 8) = ~(*(ref this.Bits.FixedElementField + 8));
			*(ref this.Bits.FixedElementField + (IntPtr)2 * 8) = ~(*(ref this.Bits.FixedElementField + (IntPtr)2 * 8));
			*(ref this.Bits.FixedElementField + (IntPtr)3 * 8) = ~(*(ref this.Bits.FixedElementField + (IntPtr)3 * 8));
			*(ref this.Bits.FixedElementField + (IntPtr)4 * 8) = ~(*(ref this.Bits.FixedElementField + (IntPtr)4 * 8));
			*(ref this.Bits.FixedElementField + (IntPtr)5 * 8) = ~(*(ref this.Bits.FixedElementField + (IntPtr)5 * 8));
			*(ref this.Bits.FixedElementField + (IntPtr)6 * 8) = ~(*(ref this.Bits.FixedElementField + (IntPtr)6 * 8));
			*(ref this.Bits.FixedElementField + (IntPtr)7 * 8) = ~(*(ref this.Bits.FixedElementField + (IntPtr)7 * 8));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void ClearAll()
		{
			this.Bits.FixedElementField = 0UL;
			*(ref this.Bits.FixedElementField + 8) = 0UL;
			*(ref this.Bits.FixedElementField + (IntPtr)2 * 8) = 0UL;
			*(ref this.Bits.FixedElementField + (IntPtr)3 * 8) = 0UL;
			*(ref this.Bits.FixedElementField + (IntPtr)4 * 8) = 0UL;
			*(ref this.Bits.FixedElementField + (IntPtr)5 * 8) = 0UL;
			*(ref this.Bits.FixedElementField + (IntPtr)6 * 8) = 0UL;
			*(ref this.Bits.FixedElementField + (IntPtr)7 * 8) = 0UL;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe bool IsSet(int bit)
		{
			return (*(ref this.Bits.FixedElementField + (IntPtr)(bit / 64) * 8) & 1UL << bit % 64) > 0UL;
		}

		public unsafe int GetSetCount()
		{
			return Maths.CountSetBits(this.Bits.FixedElementField) + Maths.CountSetBits(*(ref this.Bits.FixedElementField + 8)) + Maths.CountSetBits(*(ref this.Bits.FixedElementField + (IntPtr)2 * 8)) + Maths.CountSetBits(*(ref this.Bits.FixedElementField + (IntPtr)3 * 8)) + Maths.CountSetBits(*(ref this.Bits.FixedElementField + (IntPtr)4 * 8)) + Maths.CountSetBits(*(ref this.Bits.FixedElementField + (IntPtr)5 * 8)) + Maths.CountSetBits(*(ref this.Bits.FixedElementField + (IntPtr)6 * 8)) + Maths.CountSetBits(*(ref this.Bits.FixedElementField + (IntPtr)7 * 8));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe bool Any()
		{
			return this.Bits.FixedElementField != 0UL || *(ref this.Bits.FixedElementField + 8) != 0UL || *(ref this.Bits.FixedElementField + (IntPtr)2 * 8) != 0UL || *(ref this.Bits.FixedElementField + (IntPtr)3 * 8) != 0UL || *(ref this.Bits.FixedElementField + (IntPtr)4 * 8) != 0UL || *(ref this.Bits.FixedElementField + (IntPtr)5 * 8) != 0UL || *(ref this.Bits.FixedElementField + (IntPtr)6 * 8) != 0UL || *(ref this.Bits.FixedElementField + (IntPtr)7 * 8) > 0UL;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe bool Empty()
		{
			return this.Bits.FixedElementField == 0UL && *(ref this.Bits.FixedElementField + 8) == 0UL && *(ref this.Bits.FixedElementField + (IntPtr)2 * 8) == 0UL && *(ref this.Bits.FixedElementField + (IntPtr)3 * 8) == 0UL && *(ref this.Bits.FixedElementField + (IntPtr)4 * 8) == 0UL && *(ref this.Bits.FixedElementField + (IntPtr)5 * 8) == 0UL && *(ref this.Bits.FixedElementField + (IntPtr)6 * 8) == 0UL && *(ref this.Bits.FixedElementField + (IntPtr)7 * 8) == 0UL;
		}

		public unsafe override int GetHashCode()
		{
			fixed (ulong* ptr = &this.Bits.FixedElementField)
			{
				ulong* ptr2 = ptr;
				return HashCodeUtilities.GetArrayHashCode<ulong>(ptr2, 8, 43);
			}
		}

		public override bool Equals(object obj)
		{
			bool result;
			if (obj is BitSet512)
			{
				BitSet512 other = (BitSet512)obj;
				result = this.Equals(other);
			}
			else
			{
				result = false;
			}
			return result;
		}

		public unsafe bool Equals(BitSet512 other)
		{
			return this.Bits.FixedElementField == other.Bits.FixedElementField && *(ref this.Bits.FixedElementField + 8) == *(ref other.Bits.FixedElementField + 8) && *(ref this.Bits.FixedElementField + (IntPtr)2 * 8) == *(ref other.Bits.FixedElementField + (IntPtr)2 * 8) && *(ref this.Bits.FixedElementField + (IntPtr)3 * 8) == *(ref other.Bits.FixedElementField + (IntPtr)3 * 8) && *(ref this.Bits.FixedElementField + (IntPtr)4 * 8) == *(ref other.Bits.FixedElementField + (IntPtr)4 * 8) && *(ref this.Bits.FixedElementField + (IntPtr)5 * 8) == *(ref other.Bits.FixedElementField + (IntPtr)5 * 8) && *(ref this.Bits.FixedElementField + (IntPtr)6 * 8) == *(ref other.Bits.FixedElementField + (IntPtr)6 * 8) && *(ref this.Bits.FixedElementField + (IntPtr)7 * 8) == *(ref other.Bits.FixedElementField + (IntPtr)7 * 8);
		}

		public unsafe BitSet512.Enumerator GetEnumerator()
		{
			fixed (ulong* ptr = &this.Bits.FixedElementField)
			{
				ulong* bits = ptr;
				return new BitSet512.Enumerator(bits);
			}
		}

		IEnumerator<int> IEnumerable<int>.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public static bool operator ==(BitSet512 a, BitSet512 b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(BitSet512 a, BitSet512 b)
		{
			return !a.Equals(b);
		}

		public const int SIZE = 64;

		public const int CAPACITY = 512;

		[FixedBuffer(typeof(ulong), 8)]
		[FieldOffset(0)]
		public BitSet512.<Bits>e__FixedBuffer Bits;

		public struct Iterator
		{
			public Iterator(BitSet512 set)
			{
				this._set = set;
				this._bit = -1;
			}

			public unsafe bool Next(out int index)
			{
				this._bit++;
				for (;;)
				{
					bool flag = this._bit >= 512;
					if (flag)
					{
						break;
					}
					ulong num = *(ref this._set.Bits.FixedElementField + (IntPtr)(this._bit / 64) * 8);
					int num2 = this._bit % 64;
					bool flag2 = (num & 1UL << num2) > 0UL;
					if (flag2)
					{
						goto Block_2;
					}
					bool flag3 = num == 0UL;
					if (flag3)
					{
						this._bit += 64;
					}
					else
					{
						bool flag4 = ((uint*)(&num))[num2 / 32] == 0U;
						if (flag4)
						{
							this._bit += 32;
						}
						else
						{
							bool flag5 = ((ushort*)(&num))[num2 / 16] == 0;
							if (flag5)
							{
								this._bit += 16;
							}
							else
							{
								int num3 = this._bit / 64;
								for (;;)
								{
									int num4 = this._bit + 1;
									this._bit = num4;
									if (num4 / 64 != num3)
									{
										break;
									}
									bool flag6 = (num & 1UL << this._bit % 64) > 0UL;
									if (flag6)
									{
										goto Block_6;
									}
								}
							}
						}
					}
				}
				index = -1;
				return false;
				Block_2:
				index = this._bit;
				return true;
				Block_6:
				index = this._bit;
				return true;
			}

			private int _bit;

			public BitSet512 _set;
		}

		public struct Enumerator : IEnumerator<int>, IEnumerator, IDisposable
		{
			public unsafe Enumerator(ulong* bits)
			{
				this._bits = bits;
				this._bit = -1;
			}

			public int Current
			{
				get
				{
					return this._bit;
				}
			}

			public void Reset()
			{
				this._bit = -1;
			}

			public unsafe bool MoveNext()
			{
				this._bit++;
				for (;;)
				{
					bool flag = this._bit >= 512;
					if (flag)
					{
						break;
					}
					ulong num = this._bits[this._bit / 64];
					int num2 = this._bit % 64;
					bool flag2 = (num & 1UL << num2) > 0UL;
					if (flag2)
					{
						goto Block_2;
					}
					bool flag3 = num == 0UL;
					if (flag3)
					{
						this._bit += 64;
					}
					else
					{
						bool flag4 = ((uint*)(&num))[num2 / 32] == 0U;
						if (flag4)
						{
							this._bit += 32;
						}
						else
						{
							bool flag5 = ((ushort*)(&num))[num2 / 16] == 0;
							if (flag5)
							{
								this._bit += 16;
							}
							else
							{
								int num3 = this._bit / 64;
								for (;;)
								{
									int num4 = this._bit + 1;
									this._bit = num4;
									if (num4 / 64 != num3)
									{
										break;
									}
									bool flag6 = (num & 1UL << this._bit % 64) > 0UL;
									if (flag6)
									{
										goto Block_6;
									}
								}
							}
						}
					}
				}
				return false;
				Block_2:
				return true;
				Block_6:
				return true;
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			public void Dispose()
			{
				this._bits = null;
				this._bit = -1;
			}

			private unsafe ulong* _bits;

			private int _bit;
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 64)]
		public struct <Bits>e__FixedBuffer
		{
			public ulong FixedElementField;
		}
	}
}
