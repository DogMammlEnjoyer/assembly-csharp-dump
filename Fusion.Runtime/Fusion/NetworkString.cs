using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace Fusion
{
	[DebuggerDisplay("{Value}")]
	[NetworkStructWeaved(1, true)]
	[Serializable]
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct NetworkString<[IsUnmanaged] TSize> : INetworkString, INetworkStruct, IEquatable<NetworkString<TSize>>, IEnumerable<char>, IEnumerable where TSize : struct, ValueType, IFixedStorage
	{
		public NetworkString(string value)
		{
			this = default(NetworkString<TSize>);
			this.Value = value;
		}

		public int Capacity
		{
			get
			{
				return sizeof(TSize) / 4;
			}
		}

		public string Value
		{
			get
			{
				string result = null;
				this.Get(ref result);
				return result;
			}
			set
			{
				this.Set(value);
			}
		}

		public int Length
		{
			get
			{
				return this._length;
			}
		}

		public unsafe uint this[int index]
		{
			get
			{
				fixed (TSize* ptr = &this._data)
				{
					TSize* ptr2 = ptr;
					return ref *(uint*)(ptr2 + (IntPtr)this.SafeIndex(index) * 4 / (IntPtr)sizeof(TSize));
				}
			}
		}

		public static implicit operator NetworkString<TSize>(string str)
		{
			NetworkString<TSize> result = default(NetworkString<TSize>);
			result.Set(str);
			return result;
		}

		public static explicit operator string(NetworkString<TSize> str)
		{
			return str.Value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(NetworkString<TSize> a, NetworkString<TSize> b)
		{
			return !a.Equals(ref b);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(string a, NetworkString<TSize> b)
		{
			return !b.Equals(a);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(NetworkString<TSize> a, string b)
		{
			return !a.Equals(b);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(NetworkString<TSize> a, NetworkString<TSize> b)
		{
			return a.Equals(ref b);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(string a, NetworkString<TSize> b)
		{
			return b.Equals(a);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(NetworkString<TSize> a, string b)
		{
			return a.Equals(b);
		}

		public unsafe bool Get(ref string cache)
		{
			bool flag = cache != null && this.Compare(cache) == 0;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				int safeLength = this.SafeLength;
				bool flag2 = safeLength == 0;
				if (flag2)
				{
					cache = string.Empty;
				}
				else
				{
					fixed (TSize* ptr = &this._data)
					{
						TSize* value = ptr;
						cache = new string((sbyte*)value, 0, safeLength * 4, Encoding.UTF32);
					}
				}
				result = true;
			}
			return result;
		}

		public unsafe bool Set(string value)
		{
			value = (value ?? string.Empty);
			fixed (string text = value)
			{
				char* ptr = text;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				fixed (TSize* ptr2 = &this._data)
				{
					TSize* dst = ptr2;
					UTF32Tools.ConversionResult conversionResult = UTF32Tools.Convert(value, (uint*)dst, this.Capacity);
					this._length = conversionResult.CodePointCount;
					return conversionResult.CharacterCount == value.Length;
				}
			}
		}

		public int IndexOf(char c, int startIndex = 0)
		{
			return this.IndexOf((uint)c, startIndex, this.Length - startIndex);
		}

		public int IndexOf(char c, int startIndex, int count)
		{
			return this.IndexOf((uint)c, startIndex, count);
		}

		public int IndexOf(uint codePoint, int startIndex = 0)
		{
			return this.IndexOf(codePoint, startIndex, this.Length - startIndex);
		}

		public unsafe int IndexOf(uint codePoint, int startIndex, int count)
		{
			int safeLength = this.SafeLength;
			bool flag = startIndex < 0 || startIndex > safeLength;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("startIndex");
			}
			bool flag2 = count < 0 || startIndex + count > safeLength;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			fixed (TSize* ptr = &this._data)
			{
				TSize* ptr2 = ptr;
				uint* ptr3 = (uint*)(ptr2 + (IntPtr)startIndex * 4 / (IntPtr)sizeof(TSize));
				for (int i = 0; i < count; i++)
				{
					bool flag3 = ptr3[i] == codePoint;
					if (flag3)
					{
						return startIndex + i;
					}
				}
				return -1;
			}
		}

		public int IndexOf(string str, int startIndex = 0)
		{
			return this.IndexOf(str, startIndex, this.SafeLength - startIndex);
		}

		public unsafe int IndexOf(string str, int startIndex, int count)
		{
			bool flag = str == null;
			if (flag)
			{
				throw new ArgumentNullException("str");
			}
			bool flag2 = startIndex < 0 || startIndex > this.SafeLength;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("startIndex");
			}
			bool flag3 = count < 0 || startIndex + count > this.SafeLength;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			bool flag4 = count < str.Length;
			if (flag4)
			{
			}
			fixed (TSize* ptr = &this._data)
			{
				TSize* ptr2 = ptr;
				int num = UTF32Tools.IndexOf((uint*)(ptr2 + (IntPtr)startIndex * 4 / (IntPtr)sizeof(TSize)), count, str);
				bool flag5 = num < 0;
				int result;
				if (flag5)
				{
					result = num;
				}
				else
				{
					result = num + startIndex;
				}
				return result;
			}
		}

		public int IndexOf<[IsUnmanaged] TOtherSize>(NetworkString<TOtherSize> str, int startIndex = 0) where TOtherSize : struct, ValueType, IFixedStorage
		{
			return this.IndexOf<TOtherSize>(ref str, startIndex, this.SafeLength - startIndex);
		}

		public int IndexOf<[IsUnmanaged] TOtherSize>(NetworkString<TOtherSize> str, int startIndex, int count) where TOtherSize : struct, ValueType, IFixedStorage
		{
			return this.IndexOf<TOtherSize>(ref str, startIndex, count);
		}

		public int IndexOf<[IsUnmanaged] TOtherSize>(ref NetworkString<TOtherSize> str, int startIndex = 0) where TOtherSize : struct, ValueType, IFixedStorage
		{
			return this.IndexOf<TOtherSize>(ref str, startIndex, this.SafeLength - startIndex);
		}

		public unsafe int IndexOf<[IsUnmanaged] TOtherSize>(ref NetworkString<TOtherSize> str, int startIndex, int count) where TOtherSize : struct, ValueType, IFixedStorage
		{
			bool flag = startIndex < 0 || startIndex > this.SafeLength;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("startIndex");
			}
			bool flag2 = count < 0 || startIndex + count > this.SafeLength;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			bool flag3 = count < str.SafeLength;
			int result;
			if (flag3)
			{
				result = -1;
			}
			else
			{
				fixed (TOtherSize* ptr = &str._data)
				{
					TOtherSize* pattern = ptr;
					fixed (TSize* ptr2 = &this._data)
					{
						TSize* ptr3 = ptr2;
						int num = UTF32Tools.IndexOf((uint*)(ptr3 + (IntPtr)startIndex * 4 / (IntPtr)sizeof(TSize)), count, (uint*)pattern, str.SafeLength);
						bool flag4 = num < 0;
						if (flag4)
						{
							result = num;
						}
						else
						{
							result = num + startIndex;
						}
					}
				}
			}
			return result;
		}

		public bool Contains(char c)
		{
			return this.IndexOf(c, 0) >= 0;
		}

		public bool Contains(uint codePoint)
		{
			return this.IndexOf(codePoint, 0) >= 0;
		}

		public bool Contains(string str)
		{
			return this.IndexOf(str, 0) >= 0;
		}

		public bool Contains<[IsUnmanaged] TOtherSize>(NetworkString<TOtherSize> str) where TOtherSize : struct, ValueType, IFixedStorage
		{
			return this.IndexOf<TOtherSize>(ref str, 0) >= 0;
		}

		public bool Contains<[IsUnmanaged] TOtherSize>(ref NetworkString<TOtherSize> str) where TOtherSize : struct, ValueType, IFixedStorage
		{
			return this.IndexOf<TOtherSize>(ref str, 0) >= 0;
		}

		public NetworkString<TSize> Substring(int startIndex)
		{
			return this.Substring(startIndex, this.SafeLength - startIndex);
		}

		public unsafe NetworkString<TSize> Substring(int startIndex, int length)
		{
			bool flag = startIndex < 0 || startIndex >= this.SafeLength;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("startIndex");
			}
			bool flag2 = length < 0 || startIndex + length > this.SafeLength;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("length");
			}
			NetworkString<TSize> result = default(NetworkString<TSize>);
			fixed (TSize* ptr = &this._data)
			{
				TSize* ptr2 = ptr;
				result._length = length;
				Native.MemCpy((void*)(&result._data), (void*)(ptr2 + (IntPtr)startIndex * 4 / (IntPtr)sizeof(TSize)), length * 4);
			}
			return result;
		}

		public unsafe NetworkString<TSize> ToLower()
		{
			NetworkString<TSize> result = default(NetworkString<TSize>);
			fixed (TSize* ptr = &this._data)
			{
				TSize* src = ptr;
				UTF32Tools.ToLowerInvariant((uint*)src, (uint*)(&result._data), this.SafeLength);
				result._length = this.SafeLength;
			}
			return result;
		}

		public unsafe NetworkString<TSize> ToUpper()
		{
			NetworkString<TSize> result = default(NetworkString<TSize>);
			fixed (TSize* ptr = &this._data)
			{
				TSize* src = ptr;
				UTF32Tools.ToUpperInvariant((uint*)src, (uint*)(&result._data), this.SafeLength);
				result._length = this.SafeLength;
			}
			return result;
		}

		public unsafe int GetCharCount()
		{
			fixed (TSize* ptr = &this._data)
			{
				TSize* bytes = ptr;
				return Encoding.UTF32.GetCharCount((byte*)bytes, this.Length * 4);
			}
		}

		public unsafe int Compare(string s)
		{
			bool flag = s == null;
			if (flag)
			{
				throw new ArgumentNullException("s");
			}
			fixed (TSize* ptr = &this._data)
			{
				TSize* strB = ptr;
				return UTF32Tools.CompareOrdinal(s, (uint*)strB, this.SafeLength, false);
			}
		}

		public unsafe int Compare(NetworkString<TSize> s)
		{
			fixed (TSize* ptr = &this._data)
			{
				TSize* strA = ptr;
				return UTF32Tools.CompareOrdinal((uint*)strA, this.SafeLength, (uint*)(&s._data), s.SafeLength, false);
			}
		}

		public unsafe int Compare(ref NetworkString<TSize> s)
		{
			fixed (TSize* ptr = &this._data)
			{
				TSize* strA = ptr;
				fixed (TSize* ptr2 = &s._data)
				{
					TSize* strB = ptr2;
					return UTF32Tools.CompareOrdinal((uint*)strA, this.SafeLength, (uint*)strB, s.SafeLength, false);
				}
			}
		}

		public int Compare<[IsUnmanaged] TOtherSize>(NetworkString<TOtherSize> other) where TOtherSize : struct, ValueType, IFixedStorage
		{
			return this.Compare<TOtherSize>(ref other);
		}

		public unsafe int Compare<[IsUnmanaged] TOtherSize>(ref NetworkString<TOtherSize> other) where TOtherSize : struct, ValueType, IFixedStorage
		{
			fixed (TOtherSize* ptr = &other._data)
			{
				TOtherSize* strB = ptr;
				fixed (TSize* ptr2 = &this._data)
				{
					TSize* strA = ptr2;
					return UTF32Tools.CompareOrdinal((uint*)strA, this.SafeLength, (uint*)strB, other.SafeLength, false);
				}
			}
		}

		public bool Equals(string s)
		{
			return this.Compare(s) == 0;
		}

		public override bool Equals(object obj)
		{
			INetworkString networkString = obj as INetworkString;
			bool flag = networkString != null;
			return flag && networkString.Equals<TSize>(ref this);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(NetworkString<TSize> other)
		{
			return this.Compare(ref other) == 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(ref NetworkString<TSize> other)
		{
			return this.Compare(ref other) == 0;
		}

		public bool Equals<[IsUnmanaged] TOtherSize>(NetworkString<TOtherSize> other) where TOtherSize : struct, ValueType, IFixedStorage
		{
			return this.Compare<TOtherSize>(ref other) == 0;
		}

		public bool Equals<[IsUnmanaged] TOtherSize>(ref NetworkString<TOtherSize> other) where TOtherSize : struct, ValueType, IFixedStorage
		{
			return this.Compare<TOtherSize>(ref other) == 0;
		}

		public void Assign(string value)
		{
			this.Value = value;
		}

		public unsafe bool StartsWith(string s)
		{
			bool flag = s == null;
			if (flag)
			{
				throw new ArgumentNullException("s");
			}
			fixed (TSize* ptr = &this._data)
			{
				TSize* strA = ptr;
				return UTF32Tools.StartsWithOrdinal((uint*)strA, this.SafeLength, s, false);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe bool StartsWith<[IsUnmanaged] TOtherSize>(ref NetworkString<TOtherSize> other) where TOtherSize : struct, ValueType, IFixedStorage
		{
			fixed (TOtherSize* ptr = &other._data)
			{
				TOtherSize* strB = ptr;
				fixed (TSize* ptr2 = &this._data)
				{
					TSize* strA = ptr2;
					return UTF32Tools.StartsWithOrdinal((uint*)strA, this.SafeLength, (uint*)strB, other.SafeLength, false);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe bool EndsWith<[IsUnmanaged] TOtherSize>(ref NetworkString<TOtherSize> other) where TOtherSize : struct, ValueType, IFixedStorage
		{
			fixed (TOtherSize* ptr = &other._data)
			{
				TOtherSize* bStr = ptr;
				fixed (TSize* ptr2 = &this._data)
				{
					TSize* strA = ptr2;
					return UTF32Tools.EndsWithOrdinal((uint*)strA, this.SafeLength, (uint*)bStr, other.SafeLength, false);
				}
			}
		}

		public unsafe bool EndsWith(string s)
		{
			bool flag = s == null;
			if (flag)
			{
				throw new ArgumentNullException("s");
			}
			fixed (TSize* ptr = &this._data)
			{
				TSize* strA = ptr;
				return UTF32Tools.EndsWithOrdinal((uint*)strA, this.SafeLength, s, false);
			}
		}

		public unsafe override int GetHashCode()
		{
			fixed (TSize* ptr = &this._data)
			{
				TSize* str = ptr;
				return UTF32Tools.GetHashDeterministic((uint*)str, this.SafeLength);
			}
		}

		public override string ToString()
		{
			return this.Value;
		}

		public unsafe UTF32Tools.CharEnumerator GetEnumerator()
		{
			fixed (TSize* ptr = &this._data)
			{
				TSize* utf = ptr;
				return new UTF32Tools.CharEnumerator((uint*)utf, this.Length);
			}
		}

		IEnumerator<char> IEnumerable<char>.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private int SafeIndex(int index)
		{
			int safeLength = this.SafeLength;
			bool flag = index < 0 || index >= safeLength;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			return index;
		}

		private int SafeLength
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				bool flag = this._length < 0 || this._length > this.Capacity;
				if (flag)
				{
					throw new InvalidOperationException(string.Format("Invalid Length: {0}", this._length));
				}
				return this._length;
			}
		}

		[SerializeField]
		internal int _length;

		[SerializeField]
		internal TSize _data;
	}
}
