using System;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine.Scripting;

namespace Fusion
{
	public static class ReadWriteUtilsForWeaver
	{
		[Preserve]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static bool ReadBoolean(int* data)
		{
			return *data != 0;
		}

		[Preserve]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void WriteBoolean(int* data, bool value)
		{
			*data = (value ? 1 : 0);
		}

		[Preserve]
		public unsafe static int GetByteArrayHashCode(byte* ptr, int length)
		{
			return HashCodeUtilities.GetArrayHashCode<byte>(ptr, length, 352654597);
		}

		[Preserve]
		public unsafe static int WriteStringUtf8NoHash(void* destination, string str)
		{
			return Native.WriteLengthPrefixedUTF8(destination, str);
		}

		[Preserve]
		public unsafe static int ReadStringUtf8NoHash(void* source, out string result)
		{
			return Native.ReadLengthPrefixedUTF8(source, out result);
		}

		[Preserve]
		public static int GetByteCountUtf8NoHash(string value)
		{
			return Native.GetLengthPrefixedUTF8ByteCount(value);
		}

		[Preserve]
		public static int GetStringHashCode(string value, int maxLength)
		{
			int len = Math.Min(value.Length, maxLength);
			return value.GetHashDeterministicInternal(len, 352654597);
		}

		[Preserve]
		public unsafe static int WriteStringUtf32NoHash(int* ptr, int maxLength, string value)
		{
			bool flag = string.IsNullOrEmpty(value);
			int result;
			if (flag)
			{
				*ptr = 0;
				result = 4;
			}
			else
			{
				UTF32Tools.ConversionResult conversionResult = UTF32Tools.Convert(value, (uint*)(ptr + 1), maxLength);
				*ptr = conversionResult.CodePointCount;
				result = (conversionResult.CodePointCount + 1) * 4;
			}
			return result;
		}

		[Preserve]
		public unsafe static int ReadStringUtf32NoHash(int* ptr, int maxLength, out string result)
		{
			int num = Math.Min(*ptr, maxLength);
			int* value = ptr + 1;
			bool flag = num == 0;
			if (flag)
			{
				result = "";
			}
			else
			{
				result = new string((sbyte*)value, 0, num * 4, Encoding.UTF32);
			}
			return (num + 1) * 4;
		}

		[Preserve]
		public unsafe static int WriteStringUtf32WithHash(int* ptr, int maxLength, string value, ref string cache)
		{
			bool flag = string.IsNullOrEmpty(value);
			int result;
			if (flag)
			{
				*ptr = 0;
				ptr[1] = 0;
				result = 8;
			}
			else
			{
				UTF32Tools.ConversionResult conversionResult = UTF32Tools.Convert(value, (uint*)(ptr + 2), maxLength);
				*ptr = conversionResult.CodePointCount;
				Assert.Check(conversionResult.CharacterCount <= value.Length);
				bool flag2 = conversionResult.CharacterCount < value.Length;
				if (flag2)
				{
					cache = value.Substring(0, conversionResult.CharacterCount);
				}
				else
				{
					cache = value;
				}
				ptr[1] = cache.GetHashDeterministic(352654597);
				result = (conversionResult.CodePointCount + 2) * 4;
			}
			return result;
		}

		[Preserve]
		public unsafe static int ReadStringUtf32WithHash(int* ptr, int maxLength, ref string cache)
		{
			int num = Math.Min(*ptr, maxLength);
			int num2 = ptr[1];
			int* ptr2 = ptr + 2;
			bool flag = num == 0;
			if (flag)
			{
				cache = "";
			}
			else
			{
				bool flag2 = cache != null && num >= cache.Length / 2 && num <= cache.Length && num2 == cache.GetHashCode();
				if (flag2)
				{
					bool flag3 = UTF32Tools.CompareOrdinal(cache, (uint*)ptr2, num, false) == 0;
					if (flag3)
					{
						return (2 + num) * 4;
					}
				}
				cache = new string((sbyte*)ptr2, 0, num * 4, Encoding.UTF32);
			}
			return (2 + num) * 4;
		}

		[Preserve]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetWordCountString(int capacity, bool withCaching)
		{
			int result;
			if (withCaching)
			{
				result = 2 + capacity;
			}
			else
			{
				result = 1 + capacity;
			}
			return result;
		}

		[Preserve]
		public static int VerifyRawNetworkUnwrap<T>(int actual, int maxBytes)
		{
			bool flag = actual > maxBytes;
			if (flag)
			{
				throw new InvalidOperationException(string.Format("Overflow when unwrapping {0}: expected max {1}, got {2}", typeof(T).FullName, maxBytes, actual));
			}
			return actual;
		}

		[Preserve]
		public static int VerifyRawNetworkWrap<T>(int actual, int maxBytes)
		{
			bool flag = actual > maxBytes;
			if (flag)
			{
				throw new InvalidOperationException(string.Format("Overflow when wrapping {0}: expected max {1}, got {2}", typeof(T).FullName, maxBytes, actual));
			}
			return actual;
		}

		private const float ACCURACY = 1024f;

		private const int STRING_LENGTH_INDEX = 0;

		private const int STRING_HASHCODE_INDEX = 1;

		private const int STRING_DATA_INDEX = 2;

		private const int STRING_NOHASHCODE_DATA_INDEX = 1;
	}
}
