using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Fusion
{
	[Serializable]
	public struct Mask256 : IEquatable<Mask256>
	{
		public unsafe long this[int i]
		{
			get
			{
				return *(ref this.values.FixedElementField + (IntPtr)i * 8);
			}
			set
			{
				*(ref this.values.FixedElementField + (IntPtr)i * 8) = value;
			}
		}

		public unsafe void Clear()
		{
			this.values.FixedElementField = 0L;
			*(ref this.values.FixedElementField + 8) = 0L;
			*(ref this.values.FixedElementField + (IntPtr)2 * 8) = 0L;
			*(ref this.values.FixedElementField + (IntPtr)3 * 8) = 0L;
		}

		public unsafe void SetBit(int bitIndex, bool set)
		{
			if (set)
			{
				bool flag = bitIndex < 64;
				if (flag)
				{
					this.values.FixedElementField = (this.values.FixedElementField | 1L << bitIndex);
				}
				else
				{
					bool flag2 = bitIndex < 128;
					if (flag2)
					{
						*(ref this.values.FixedElementField + 8) |= 1L << bitIndex - 64;
					}
					else
					{
						bool flag3 = bitIndex < 192;
						if (flag3)
						{
							*(ref this.values.FixedElementField + (IntPtr)2 * 8) |= 1L << bitIndex - 128;
						}
						else
						{
							bool flag4 = bitIndex < 256;
							if (flag4)
							{
								*(ref this.values.FixedElementField + (IntPtr)3 * 8) |= 1L << bitIndex - 192;
							}
						}
					}
				}
			}
			else
			{
				bool flag5 = bitIndex < 64;
				if (flag5)
				{
					this.values.FixedElementField = (this.values.FixedElementField & ~(1L << bitIndex));
				}
				else
				{
					bool flag6 = bitIndex < 128;
					if (flag6)
					{
						*(ref this.values.FixedElementField + 8) &= ~(1L << bitIndex - 64);
					}
					else
					{
						bool flag7 = bitIndex < 192;
						if (flag7)
						{
							*(ref this.values.FixedElementField + (IntPtr)2 * 8) &= ~(1L << bitIndex - 128);
						}
						else
						{
							bool flag8 = bitIndex < 256;
							if (flag8)
							{
								*(ref this.values.FixedElementField + (IntPtr)3 * 8) &= ~(1L << bitIndex - 192);
							}
						}
					}
				}
			}
		}

		public unsafe bool GetBit(int bitIndex)
		{
			bool flag = bitIndex < 64;
			bool result;
			if (flag)
			{
				result = ((this.values.FixedElementField & 1L << bitIndex) != 0L);
			}
			else
			{
				bool flag2 = bitIndex < 128;
				if (flag2)
				{
					result = ((*(ref this.values.FixedElementField + 8) & 1L << bitIndex - 64) != 0L);
				}
				else
				{
					bool flag3 = bitIndex < 192;
					if (flag3)
					{
						result = ((*(ref this.values.FixedElementField + (IntPtr)2 * 8) & 1L << bitIndex - 128) != 0L);
					}
					else
					{
						bool flag4 = bitIndex < 256;
						result = (flag4 && (*(ref this.values.FixedElementField + (IntPtr)3 * 8) & 1L << bitIndex - 192) != 0L);
					}
				}
			}
			return result;
		}

		public unsafe Mask256(long a, long b = 0L, long c = 0L, long d = 0L)
		{
			this = default(Mask256);
			this.values.FixedElementField = a;
			*(ref this.values.FixedElementField + 8) = b;
			*(ref this.values.FixedElementField + (IntPtr)2 * 8) = c;
			*(ref this.values.FixedElementField + (IntPtr)3 * 8) = d;
		}

		public static implicit operator long(Mask256 mask)
		{
			return mask.values.FixedElementField;
		}

		public static implicit operator Mask256(long value)
		{
			return new Mask256(value, 0L, 0L, 0L);
		}

		public unsafe static Mask256 operator &(Mask256 a, Mask256 b)
		{
			return new Mask256(a.values.FixedElementField & b.values.FixedElementField, *(ref a.values.FixedElementField + 8) & *(ref b.values.FixedElementField + 8), *(ref a.values.FixedElementField + (IntPtr)2 * 8) & *(ref b.values.FixedElementField + (IntPtr)2 * 8), *(ref a.values.FixedElementField + (IntPtr)3 * 8) & *(ref b.values.FixedElementField + (IntPtr)3 * 8));
		}

		public unsafe static Mask256 operator |(Mask256 a, Mask256 b)
		{
			return new Mask256(a.values.FixedElementField | b.values.FixedElementField, *(ref a.values.FixedElementField + 8) | *(ref b.values.FixedElementField + 8), *(ref a.values.FixedElementField + (IntPtr)2 * 8) | *(ref b.values.FixedElementField + (IntPtr)2 * 8), *(ref a.values.FixedElementField + (IntPtr)3 * 8) | *(ref b.values.FixedElementField + (IntPtr)3 * 8));
		}

		public unsafe static Mask256 operator ~(Mask256 a)
		{
			return new Mask256(~a.values.FixedElementField, ~(*(ref a.values.FixedElementField + 8)), ~(*(ref a.values.FixedElementField + (IntPtr)2 * 8)), ~(*(ref a.values.FixedElementField + (IntPtr)3 * 8)));
		}

		public override bool Equals(object obj)
		{
			bool result;
			if (obj is Mask256)
			{
				Mask256 other = (Mask256)obj;
				result = this.Equals(other);
			}
			else
			{
				result = false;
			}
			return result;
		}

		public override int GetHashCode()
		{
			return this.values.FixedElementField.GetHashCode() ^ (ref this.values.FixedElementField + 8).GetHashCode() ^ (ref this.values.FixedElementField + (IntPtr)2 * 8).GetHashCode() ^ (ref this.values.FixedElementField + (IntPtr)3 * 8).GetHashCode();
		}

		public unsafe bool Equals(Mask256 other)
		{
			return this.values.FixedElementField == other.values.FixedElementField && *(ref this.values.FixedElementField + 8) == *(ref other.values.FixedElementField + 8) && *(ref this.values.FixedElementField + (IntPtr)2 * 8) == *(ref other.values.FixedElementField + (IntPtr)2 * 8) && *(ref this.values.FixedElementField + (IntPtr)3 * 8) == *(ref other.values.FixedElementField + (IntPtr)3 * 8);
		}

		public unsafe bool IsNothing()
		{
			return this.values.FixedElementField == 0L && *(ref this.values.FixedElementField + 8) == 0L && *(ref this.values.FixedElementField + (IntPtr)2 * 8) == 0L && *(ref this.values.FixedElementField + (IntPtr)3 * 8) == 0L;
		}

		public unsafe override string ToString()
		{
			return string.Format("{0}:{1}:{2}:{3}", new object[]
			{
				this.values.FixedElementField,
				*(ref this.values.FixedElementField + 8),
				*(ref this.values.FixedElementField + (IntPtr)2 * 8),
				*(ref this.values.FixedElementField + (IntPtr)3 * 8)
			});
		}

		[FixedBuffer(typeof(long), 4)]
		[SerializeField]
		private Mask256.<values>e__FixedBuffer values;

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 32)]
		public struct <values>e__FixedBuffer
		{
			public long FixedElementField;
		}
	}
}
