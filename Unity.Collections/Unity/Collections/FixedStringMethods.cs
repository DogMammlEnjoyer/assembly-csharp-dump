using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace Unity.Collections
{
	[GenerateTestsForBurstCompatibility]
	[GenerateTestsForBurstCompatibility]
	[GenerateTestsForBurstCompatibility]
	[GenerateTestsForBurstCompatibility]
	public static class FixedStringMethods
	{
		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes)
		})]
		public static FormatError Append<[IsUnmanaged] T>(this T fs, Unicode.Rune rune) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			int length = fs.Length;
			int num = rune.LengthInUtf8Bytes();
			if (!fs.TryResize(length + num, NativeArrayOptions.UninitializedMemory))
			{
				return FormatError.Overflow;
			}
			return ref fs.Write(ref length, rune);
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes)
		})]
		public static FormatError Append<[IsUnmanaged] T>(this T fs, char ch) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			return ref fs.Append(ch);
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes)
		})]
		public unsafe static FormatError AppendRawByte<[IsUnmanaged] T>(this T fs, byte a) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			int length = fs.Length;
			if (!fs.TryResize(length + 1, NativeArrayOptions.UninitializedMemory))
			{
				return FormatError.Overflow;
			}
			fs.GetUnsafePtr()[length] = a;
			return FormatError.None;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes)
		})]
		public unsafe static FormatError Append<[IsUnmanaged] T>(this T fs, Unicode.Rune rune, int count) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			int length = fs.Length;
			if (!fs.TryResize(length + rune.LengthInUtf8Bytes() * count, NativeArrayOptions.UninitializedMemory))
			{
				return FormatError.Overflow;
			}
			int capacity = fs.Capacity;
			byte* unsafePtr = fs.GetUnsafePtr();
			int num = length;
			for (int i = 0; i < count; i++)
			{
				if (Unicode.UcsToUtf8(unsafePtr, ref num, capacity, rune) != ConversionError.None)
				{
					return FormatError.Overflow;
				}
			}
			return FormatError.None;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes)
		})]
		public unsafe static FormatError Append<[IsUnmanaged] T>(this T fs, long input) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			byte* ptr = stackalloc byte[(UIntPtr)20];
			int num = 20;
			if (input >= 0L)
			{
				do
				{
					byte b = (byte)(input % 10L);
					ptr[--num] = 48 + b;
					input /= 10L;
				}
				while (input != 0L);
			}
			else
			{
				do
				{
					byte b2 = (byte)(input % 10L);
					ptr[--num] = 48 - b2;
					input /= 10L;
				}
				while (input != 0L);
				ptr[--num] = 45;
			}
			return ref fs.Append(ptr + num, 20 - num);
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes)
		})]
		public static FormatError Append<[IsUnmanaged] T>(this T fs, int input) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			return ref fs.Append((long)input);
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes)
		})]
		public unsafe static FormatError Append<[IsUnmanaged] T>(this T fs, ulong input) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			byte* ptr = stackalloc byte[(UIntPtr)20];
			int num = 20;
			do
			{
				byte b = (byte)(input % 10UL);
				ptr[--num] = 48 + b;
				input /= 10UL;
			}
			while (input != 0UL);
			return ref fs.Append(ptr + num, 20 - num);
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes)
		})]
		public static FormatError Append<[IsUnmanaged] T>(this T fs, uint input) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			return ref fs.Append((ulong)input);
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes)
		})]
		public unsafe static FormatError Append<[IsUnmanaged] T>(this T fs, float input, char decimalSeparator = '.') where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedStringUtils.UintFloatUnion uintFloatUnion = new FixedStringUtils.UintFloatUnion
			{
				floatValue = input
			};
			uint num = uintFloatUnion.uintValue >> 31;
			uintFloatUnion.uintValue &= 2147483647U;
			if ((uintFloatUnion.uintValue & 2139095040U) == 2139095040U)
			{
				if (uintFloatUnion.uintValue != 2139095040U)
				{
					return ref fs.Append('N', 'a', 'N');
				}
				FormatError result;
				if (num != 0U && (result = ref fs.Append('-')) != FormatError.None)
				{
					return result;
				}
				return ref fs.Append('I', 'n', 'f', 'i', 'n', 'i', 't', 'y');
			}
			else
			{
				FormatError result;
				if (num != 0U && uintFloatUnion.uintValue != 0U && (result = ref fs.Append('-')) != FormatError.None)
				{
					return result;
				}
				ulong num2 = 0UL;
				int num3 = 0;
				FixedStringUtils.Base2ToBase10(ref num2, ref num3, uintFloatUnion.floatValue);
				char* ptr = stackalloc char[(UIntPtr)18];
				int i = 0;
				while (i < 9)
				{
					ulong num4 = num2 % 10UL;
					ptr[(IntPtr)(8 - i++) * 2] = (char)(48UL + num4);
					num2 /= 10UL;
					if (num2 <= 0UL)
					{
						char* ptr2 = ptr + 9 - i;
						int j = -num3 - i + 1;
						if (j > 0)
						{
							if (j > 4)
							{
								return ref fs.AppendScientific(ptr2, i, num3, decimalSeparator);
							}
							if ((result = ref fs.Append('0', decimalSeparator)) != FormatError.None)
							{
								return result;
							}
							for (j--; j > 0; j--)
							{
								if ((result = ref fs.Append('0')) != FormatError.None)
								{
									return result;
								}
							}
							for (int k = 0; k < i; k++)
							{
								if ((result = ref fs.Append(ptr2[k])) != FormatError.None)
								{
									return result;
								}
							}
							return FormatError.None;
						}
						else
						{
							int l = num3;
							if (l <= 0)
							{
								int num5 = i + num3;
								for (int m = 0; m < i; m++)
								{
									if (m == num5 && (result = ref fs.Append(decimalSeparator)) != FormatError.None)
									{
										return result;
									}
									if ((result = ref fs.Append(ptr2[m])) != FormatError.None)
									{
										return result;
									}
								}
								return FormatError.None;
							}
							if (l > 4)
							{
								return ref fs.AppendScientific(ptr2, i, num3, decimalSeparator);
							}
							for (int n = 0; n < i; n++)
							{
								if ((result = ref fs.Append(ptr2[n])) != FormatError.None)
								{
									return result;
								}
							}
							while (l > 0)
							{
								if ((result = ref fs.Append('0')) != FormatError.None)
								{
									return result;
								}
								l--;
							}
							return FormatError.None;
						}
					}
				}
				return FormatError.Overflow;
			}
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public static FormatError Append<[IsUnmanaged] T, [IsUnmanaged] T2>(this T fs, in T2 input) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			ref T2 ptr = ref UnsafeUtilityExtensions.AsRef<T2>(input);
			return ref fs.Append(ptr.GetUnsafePtr(), ptr.Length);
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public static CopyError CopyFrom<[IsUnmanaged] T, [IsUnmanaged] T2>(this T fs, in T2 input) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			fs.Length = 0;
			if (ref fs.Append(input) != FormatError.None)
			{
				return CopyError.Truncation;
			}
			return CopyError.None;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes)
		})]
		public unsafe static FormatError Append<[IsUnmanaged] T>(this T fs, byte* utf8Bytes, int utf8BytesLength) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			int length = fs.Length;
			if (!fs.TryResize(length + utf8BytesLength, NativeArrayOptions.UninitializedMemory))
			{
				return FormatError.Overflow;
			}
			UnsafeUtility.MemCpy((void*)(fs.GetUnsafePtr() + length), (void*)utf8Bytes, (long)utf8BytesLength);
			return FormatError.None;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public unsafe static FormatError Append<[IsUnmanaged] T>(this T fs, string s) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			int num = s.Length * 4;
			byte* ptr = stackalloc byte[(UIntPtr)num];
			int utf8BytesLength;
			fixed (string text = s)
			{
				char* ptr2 = text;
				if (ptr2 != null)
				{
					ptr2 += RuntimeHelpers.OffsetToStringData / 2;
				}
				if (UTF8ArrayUnsafeUtility.Copy(ptr, out utf8BytesLength, num, ptr2, s.Length) != CopyError.None)
				{
					return FormatError.Overflow;
				}
			}
			return ref fs.Append(ptr, utf8BytesLength);
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static CopyError CopyFrom<[IsUnmanaged] T>(this T fs, string s) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			fs.Length = 0;
			if (ref fs.Append(s) != FormatError.None)
			{
				return CopyError.Truncation;
			}
			return CopyError.None;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public unsafe static CopyError CopyFromTruncated<[IsUnmanaged] T>(this T fs, string s) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			char* ptr = s;
			if (ptr != null)
			{
				ptr += RuntimeHelpers.OffsetToStringData / 2;
			}
			int length;
			CopyError result = UTF8ArrayUnsafeUtility.Copy(fs.GetUnsafePtr(), out length, fs.Capacity, ptr, s.Length);
			fs.Length = length;
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public unsafe static CopyError CopyFromTruncated<[IsUnmanaged] T, [IsUnmanaged] T2>(this T fs, in T2 input) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			byte* unsafePtr = fs.GetUnsafePtr();
			int capacity = fs.Capacity;
			T2 t = input;
			byte* unsafePtr2 = t.GetUnsafePtr();
			t = input;
			int length;
			CopyError result = UTF8ArrayUnsafeUtility.Copy(unsafePtr, out length, capacity, unsafePtr2, t.Length);
			fs.Length = length;
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public unsafe static FormatError AppendFormat<[IsUnmanaged] T, [IsUnmanaged] U, [IsUnmanaged] T0>(this T dest, in U format, in T0 arg0) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes where U : struct, ValueType, INativeList<byte>, IUTF8Bytes where T0 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			ref U ptr = ref UnsafeUtilityExtensions.AsRef<U>(format);
			int length = ptr.Length;
			byte* unsafePtr = ptr.GetUnsafePtr();
			int i = 0;
			while (i < length)
			{
				byte b = unsafePtr[i++];
				FormatError formatError;
				if (b == 123)
				{
					if (i >= length)
					{
						return FormatError.BadFormatSpecifier;
					}
					b = unsafePtr[i++];
					if (b >= 48 && b <= 57 && i < length && unsafePtr[i++] == 125)
					{
						if (b - 48 == 0)
						{
							formatError = ref dest.Append(arg0);
						}
						else
						{
							formatError = FormatError.BadFormatSpecifier;
						}
					}
					else if (b == 123)
					{
						formatError = ref dest.AppendRawByte(b);
					}
					else
					{
						formatError = FormatError.BadFormatSpecifier;
					}
				}
				else if (b == 125)
				{
					if (i < length)
					{
						b = unsafePtr[i++];
					}
					if (b == 125)
					{
						formatError = ref dest.AppendRawByte(b);
					}
					else
					{
						formatError = FormatError.BadFormatSpecifier;
					}
				}
				else
				{
					formatError = ref dest.AppendRawByte(b);
				}
				if (formatError != FormatError.None)
				{
					return formatError;
				}
			}
			return FormatError.None;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public unsafe static FormatError AppendFormat<[IsUnmanaged] T, [IsUnmanaged] U, [IsUnmanaged] T0, [IsUnmanaged] T1>(this T dest, in U format, in T0 arg0, in T1 arg1) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes where U : struct, ValueType, INativeList<byte>, IUTF8Bytes where T0 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			ref U ptr = ref UnsafeUtilityExtensions.AsRef<U>(format);
			int length = ptr.Length;
			byte* unsafePtr = ptr.GetUnsafePtr();
			int i = 0;
			while (i < length)
			{
				byte b = unsafePtr[i++];
				FormatError formatError;
				if (b == 123)
				{
					if (i >= length)
					{
						return FormatError.BadFormatSpecifier;
					}
					b = unsafePtr[i++];
					if (b >= 48 && b <= 57 && i < length && unsafePtr[i++] == 125)
					{
						int num = (int)(b - 48);
						if (num != 0)
						{
							if (num != 1)
							{
								formatError = FormatError.BadFormatSpecifier;
							}
							else
							{
								formatError = ref dest.Append(arg1);
							}
						}
						else
						{
							formatError = ref dest.Append(arg0);
						}
					}
					else if (b == 123)
					{
						formatError = ref dest.AppendRawByte(b);
					}
					else
					{
						formatError = FormatError.BadFormatSpecifier;
					}
				}
				else if (b == 125)
				{
					if (i < length)
					{
						b = unsafePtr[i++];
					}
					if (b == 125)
					{
						formatError = ref dest.AppendRawByte(b);
					}
					else
					{
						formatError = FormatError.BadFormatSpecifier;
					}
				}
				else
				{
					formatError = ref dest.AppendRawByte(b);
				}
				if (formatError != FormatError.None)
				{
					return formatError;
				}
			}
			return FormatError.None;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public unsafe static FormatError AppendFormat<[IsUnmanaged] T, [IsUnmanaged] U, [IsUnmanaged] T0, [IsUnmanaged] T1, [IsUnmanaged] T2>(this T dest, in U format, in T0 arg0, in T1 arg1, in T2 arg2) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes where U : struct, ValueType, INativeList<byte>, IUTF8Bytes where T0 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			ref U ptr = ref UnsafeUtilityExtensions.AsRef<U>(format);
			int length = ptr.Length;
			byte* unsafePtr = ptr.GetUnsafePtr();
			int i = 0;
			while (i < length)
			{
				byte b = unsafePtr[i++];
				FormatError formatError;
				if (b == 123)
				{
					if (i >= length)
					{
						return FormatError.BadFormatSpecifier;
					}
					b = unsafePtr[i++];
					if (b >= 48 && b <= 57 && i < length && unsafePtr[i++] == 125)
					{
						switch (b)
						{
						case 48:
							formatError = ref dest.Append(arg0);
							break;
						case 49:
							formatError = ref dest.Append(arg1);
							break;
						case 50:
							formatError = ref dest.Append(arg2);
							break;
						default:
							formatError = FormatError.BadFormatSpecifier;
							break;
						}
					}
					else if (b == 123)
					{
						formatError = ref dest.AppendRawByte(b);
					}
					else
					{
						formatError = FormatError.BadFormatSpecifier;
					}
				}
				else if (b == 125)
				{
					if (i < length)
					{
						b = unsafePtr[i++];
					}
					if (b == 125)
					{
						formatError = ref dest.AppendRawByte(b);
					}
					else
					{
						formatError = FormatError.BadFormatSpecifier;
					}
				}
				else
				{
					formatError = ref dest.AppendRawByte(b);
				}
				if (formatError != FormatError.None)
				{
					return formatError;
				}
			}
			return FormatError.None;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public unsafe static FormatError AppendFormat<[IsUnmanaged] T, [IsUnmanaged] U, [IsUnmanaged] T0, [IsUnmanaged] T1, [IsUnmanaged] T2, [IsUnmanaged] T3>(this T dest, in U format, in T0 arg0, in T1 arg1, in T2 arg2, in T3 arg3) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes where U : struct, ValueType, INativeList<byte>, IUTF8Bytes where T0 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T3 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			ref U ptr = ref UnsafeUtilityExtensions.AsRef<U>(format);
			int length = ptr.Length;
			byte* unsafePtr = ptr.GetUnsafePtr();
			int i = 0;
			while (i < length)
			{
				byte b = unsafePtr[i++];
				FormatError formatError;
				if (b == 123)
				{
					if (i >= length)
					{
						return FormatError.BadFormatSpecifier;
					}
					b = unsafePtr[i++];
					if (b >= 48 && b <= 57 && i < length && unsafePtr[i++] == 125)
					{
						switch (b)
						{
						case 48:
							formatError = ref dest.Append(arg0);
							break;
						case 49:
							formatError = ref dest.Append(arg1);
							break;
						case 50:
							formatError = ref dest.Append(arg2);
							break;
						case 51:
							formatError = ref dest.Append(arg3);
							break;
						default:
							formatError = FormatError.BadFormatSpecifier;
							break;
						}
					}
					else if (b == 123)
					{
						formatError = ref dest.AppendRawByte(b);
					}
					else
					{
						formatError = FormatError.BadFormatSpecifier;
					}
				}
				else if (b == 125)
				{
					if (i < length)
					{
						b = unsafePtr[i++];
					}
					if (b == 125)
					{
						formatError = ref dest.AppendRawByte(b);
					}
					else
					{
						formatError = FormatError.BadFormatSpecifier;
					}
				}
				else
				{
					formatError = ref dest.AppendRawByte(b);
				}
				if (formatError != FormatError.None)
				{
					return formatError;
				}
			}
			return FormatError.None;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public unsafe static FormatError AppendFormat<[IsUnmanaged] T, [IsUnmanaged] U, [IsUnmanaged] T0, [IsUnmanaged] T1, [IsUnmanaged] T2, [IsUnmanaged] T3, [IsUnmanaged] T4>(this T dest, in U format, in T0 arg0, in T1 arg1, in T2 arg2, in T3 arg3, in T4 arg4) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes where U : struct, ValueType, INativeList<byte>, IUTF8Bytes where T0 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T3 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T4 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			ref U ptr = ref UnsafeUtilityExtensions.AsRef<U>(format);
			int length = ptr.Length;
			byte* unsafePtr = ptr.GetUnsafePtr();
			int i = 0;
			while (i < length)
			{
				byte b = unsafePtr[i++];
				FormatError formatError;
				if (b == 123)
				{
					if (i >= length)
					{
						return FormatError.BadFormatSpecifier;
					}
					b = unsafePtr[i++];
					if (b >= 48 && b <= 57 && i < length && unsafePtr[i++] == 125)
					{
						switch (b)
						{
						case 48:
							formatError = ref dest.Append(arg0);
							break;
						case 49:
							formatError = ref dest.Append(arg1);
							break;
						case 50:
							formatError = ref dest.Append(arg2);
							break;
						case 51:
							formatError = ref dest.Append(arg3);
							break;
						case 52:
							formatError = ref dest.Append(arg4);
							break;
						default:
							formatError = FormatError.BadFormatSpecifier;
							break;
						}
					}
					else if (b == 123)
					{
						formatError = ref dest.AppendRawByte(b);
					}
					else
					{
						formatError = FormatError.BadFormatSpecifier;
					}
				}
				else if (b == 125)
				{
					if (i < length)
					{
						b = unsafePtr[i++];
					}
					if (b == 125)
					{
						formatError = ref dest.AppendRawByte(b);
					}
					else
					{
						formatError = FormatError.BadFormatSpecifier;
					}
				}
				else
				{
					formatError = ref dest.AppendRawByte(b);
				}
				if (formatError != FormatError.None)
				{
					return formatError;
				}
			}
			return FormatError.None;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public unsafe static FormatError AppendFormat<[IsUnmanaged] T, [IsUnmanaged] U, [IsUnmanaged] T0, [IsUnmanaged] T1, [IsUnmanaged] T2, [IsUnmanaged] T3, [IsUnmanaged] T4, [IsUnmanaged] T5>(this T dest, in U format, in T0 arg0, in T1 arg1, in T2 arg2, in T3 arg3, in T4 arg4, in T5 arg5) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes where U : struct, ValueType, INativeList<byte>, IUTF8Bytes where T0 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T3 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T4 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T5 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			ref U ptr = ref UnsafeUtilityExtensions.AsRef<U>(format);
			int length = ptr.Length;
			byte* unsafePtr = ptr.GetUnsafePtr();
			int i = 0;
			while (i < length)
			{
				byte b = unsafePtr[i++];
				FormatError formatError;
				if (b == 123)
				{
					if (i >= length)
					{
						return FormatError.BadFormatSpecifier;
					}
					b = unsafePtr[i++];
					if (b >= 48 && b <= 57 && i < length && unsafePtr[i++] == 125)
					{
						switch (b)
						{
						case 48:
							formatError = ref dest.Append(arg0);
							break;
						case 49:
							formatError = ref dest.Append(arg1);
							break;
						case 50:
							formatError = ref dest.Append(arg2);
							break;
						case 51:
							formatError = ref dest.Append(arg3);
							break;
						case 52:
							formatError = ref dest.Append(arg4);
							break;
						case 53:
							formatError = ref dest.Append(arg5);
							break;
						default:
							formatError = FormatError.BadFormatSpecifier;
							break;
						}
					}
					else if (b == 123)
					{
						formatError = ref dest.AppendRawByte(b);
					}
					else
					{
						formatError = FormatError.BadFormatSpecifier;
					}
				}
				else if (b == 125)
				{
					if (i < length)
					{
						b = unsafePtr[i++];
					}
					if (b == 125)
					{
						formatError = ref dest.AppendRawByte(b);
					}
					else
					{
						formatError = FormatError.BadFormatSpecifier;
					}
				}
				else
				{
					formatError = ref dest.AppendRawByte(b);
				}
				if (formatError != FormatError.None)
				{
					return formatError;
				}
			}
			return FormatError.None;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public unsafe static FormatError AppendFormat<[IsUnmanaged] T, [IsUnmanaged] U, [IsUnmanaged] T0, [IsUnmanaged] T1, [IsUnmanaged] T2, [IsUnmanaged] T3, [IsUnmanaged] T4, [IsUnmanaged] T5, [IsUnmanaged] T6>(this T dest, in U format, in T0 arg0, in T1 arg1, in T2 arg2, in T3 arg3, in T4 arg4, in T5 arg5, in T6 arg6) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes where U : struct, ValueType, INativeList<byte>, IUTF8Bytes where T0 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T3 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T4 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T5 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T6 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			ref U ptr = ref UnsafeUtilityExtensions.AsRef<U>(format);
			int length = ptr.Length;
			byte* unsafePtr = ptr.GetUnsafePtr();
			int i = 0;
			while (i < length)
			{
				byte b = unsafePtr[i++];
				FormatError formatError;
				if (b == 123)
				{
					if (i >= length)
					{
						return FormatError.BadFormatSpecifier;
					}
					b = unsafePtr[i++];
					if (b >= 48 && b <= 57 && i < length && unsafePtr[i++] == 125)
					{
						switch (b)
						{
						case 48:
							formatError = ref dest.Append(arg0);
							break;
						case 49:
							formatError = ref dest.Append(arg1);
							break;
						case 50:
							formatError = ref dest.Append(arg2);
							break;
						case 51:
							formatError = ref dest.Append(arg3);
							break;
						case 52:
							formatError = ref dest.Append(arg4);
							break;
						case 53:
							formatError = ref dest.Append(arg5);
							break;
						case 54:
							formatError = ref dest.Append(arg6);
							break;
						default:
							formatError = FormatError.BadFormatSpecifier;
							break;
						}
					}
					else if (b == 123)
					{
						formatError = ref dest.AppendRawByte(b);
					}
					else
					{
						formatError = FormatError.BadFormatSpecifier;
					}
				}
				else if (b == 125)
				{
					if (i < length)
					{
						b = unsafePtr[i++];
					}
					if (b == 125)
					{
						formatError = ref dest.AppendRawByte(b);
					}
					else
					{
						formatError = FormatError.BadFormatSpecifier;
					}
				}
				else
				{
					formatError = ref dest.AppendRawByte(b);
				}
				if (formatError != FormatError.None)
				{
					return formatError;
				}
			}
			return FormatError.None;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public unsafe static FormatError AppendFormat<[IsUnmanaged] T, [IsUnmanaged] U, [IsUnmanaged] T0, [IsUnmanaged] T1, [IsUnmanaged] T2, [IsUnmanaged] T3, [IsUnmanaged] T4, [IsUnmanaged] T5, [IsUnmanaged] T6, [IsUnmanaged] T7>(this T dest, in U format, in T0 arg0, in T1 arg1, in T2 arg2, in T3 arg3, in T4 arg4, in T5 arg5, in T6 arg6, in T7 arg7) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes where U : struct, ValueType, INativeList<byte>, IUTF8Bytes where T0 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T3 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T4 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T5 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T6 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T7 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			ref U ptr = ref UnsafeUtilityExtensions.AsRef<U>(format);
			int length = ptr.Length;
			byte* unsafePtr = ptr.GetUnsafePtr();
			int i = 0;
			while (i < length)
			{
				byte b = unsafePtr[i++];
				FormatError formatError;
				if (b == 123)
				{
					if (i >= length)
					{
						return FormatError.BadFormatSpecifier;
					}
					b = unsafePtr[i++];
					if (b >= 48 && b <= 57 && i < length && unsafePtr[i++] == 125)
					{
						switch (b)
						{
						case 48:
							formatError = ref dest.Append(arg0);
							break;
						case 49:
							formatError = ref dest.Append(arg1);
							break;
						case 50:
							formatError = ref dest.Append(arg2);
							break;
						case 51:
							formatError = ref dest.Append(arg3);
							break;
						case 52:
							formatError = ref dest.Append(arg4);
							break;
						case 53:
							formatError = ref dest.Append(arg5);
							break;
						case 54:
							formatError = ref dest.Append(arg6);
							break;
						case 55:
							formatError = ref dest.Append(arg7);
							break;
						default:
							formatError = FormatError.BadFormatSpecifier;
							break;
						}
					}
					else if (b == 123)
					{
						formatError = ref dest.AppendRawByte(b);
					}
					else
					{
						formatError = FormatError.BadFormatSpecifier;
					}
				}
				else if (b == 125)
				{
					if (i < length)
					{
						b = unsafePtr[i++];
					}
					if (b == 125)
					{
						formatError = ref dest.AppendRawByte(b);
					}
					else
					{
						formatError = FormatError.BadFormatSpecifier;
					}
				}
				else
				{
					formatError = ref dest.AppendRawByte(b);
				}
				if (formatError != FormatError.None)
				{
					return formatError;
				}
			}
			return FormatError.None;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public unsafe static FormatError AppendFormat<[IsUnmanaged] T, [IsUnmanaged] U, [IsUnmanaged] T0, [IsUnmanaged] T1, [IsUnmanaged] T2, [IsUnmanaged] T3, [IsUnmanaged] T4, [IsUnmanaged] T5, [IsUnmanaged] T6, [IsUnmanaged] T7, [IsUnmanaged] T8>(this T dest, in U format, in T0 arg0, in T1 arg1, in T2 arg2, in T3 arg3, in T4 arg4, in T5 arg5, in T6 arg6, in T7 arg7, in T8 arg8) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes where U : struct, ValueType, INativeList<byte>, IUTF8Bytes where T0 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T3 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T4 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T5 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T6 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T7 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T8 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			ref U ptr = ref UnsafeUtilityExtensions.AsRef<U>(format);
			int length = ptr.Length;
			byte* unsafePtr = ptr.GetUnsafePtr();
			int i = 0;
			while (i < length)
			{
				byte b = unsafePtr[i++];
				FormatError formatError;
				if (b == 123)
				{
					if (i >= length)
					{
						return FormatError.BadFormatSpecifier;
					}
					b = unsafePtr[i++];
					if (b >= 48 && b <= 57 && i < length && unsafePtr[i++] == 125)
					{
						switch (b)
						{
						case 48:
							formatError = ref dest.Append(arg0);
							break;
						case 49:
							formatError = ref dest.Append(arg1);
							break;
						case 50:
							formatError = ref dest.Append(arg2);
							break;
						case 51:
							formatError = ref dest.Append(arg3);
							break;
						case 52:
							formatError = ref dest.Append(arg4);
							break;
						case 53:
							formatError = ref dest.Append(arg5);
							break;
						case 54:
							formatError = ref dest.Append(arg6);
							break;
						case 55:
							formatError = ref dest.Append(arg7);
							break;
						case 56:
							formatError = ref dest.Append(arg8);
							break;
						default:
							formatError = FormatError.BadFormatSpecifier;
							break;
						}
					}
					else if (b == 123)
					{
						formatError = ref dest.AppendRawByte(b);
					}
					else
					{
						formatError = FormatError.BadFormatSpecifier;
					}
				}
				else if (b == 125)
				{
					if (i < length)
					{
						b = unsafePtr[i++];
					}
					if (b == 125)
					{
						formatError = ref dest.AppendRawByte(b);
					}
					else
					{
						formatError = FormatError.BadFormatSpecifier;
					}
				}
				else
				{
					formatError = ref dest.AppendRawByte(b);
				}
				if (formatError != FormatError.None)
				{
					return formatError;
				}
			}
			return FormatError.None;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public unsafe static FormatError AppendFormat<[IsUnmanaged] T, [IsUnmanaged] U, [IsUnmanaged] T0, [IsUnmanaged] T1, [IsUnmanaged] T2, [IsUnmanaged] T3, [IsUnmanaged] T4, [IsUnmanaged] T5, [IsUnmanaged] T6, [IsUnmanaged] T7, [IsUnmanaged] T8, [IsUnmanaged] T9>(this T dest, in U format, in T0 arg0, in T1 arg1, in T2 arg2, in T3 arg3, in T4 arg4, in T5 arg5, in T6 arg6, in T7 arg7, in T8 arg8, in T9 arg9) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes where U : struct, ValueType, INativeList<byte>, IUTF8Bytes where T0 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T3 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T4 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T5 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T6 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T7 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T8 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T9 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			ref U ptr = ref UnsafeUtilityExtensions.AsRef<U>(format);
			int length = ptr.Length;
			byte* unsafePtr = ptr.GetUnsafePtr();
			int i = 0;
			while (i < length)
			{
				byte b = unsafePtr[i++];
				FormatError formatError;
				if (b == 123)
				{
					if (i >= length)
					{
						return FormatError.BadFormatSpecifier;
					}
					b = unsafePtr[i++];
					if (b >= 48 && b <= 57 && i < length && unsafePtr[i++] == 125)
					{
						switch (b)
						{
						case 48:
							formatError = ref dest.Append(arg0);
							break;
						case 49:
							formatError = ref dest.Append(arg1);
							break;
						case 50:
							formatError = ref dest.Append(arg2);
							break;
						case 51:
							formatError = ref dest.Append(arg3);
							break;
						case 52:
							formatError = ref dest.Append(arg4);
							break;
						case 53:
							formatError = ref dest.Append(arg5);
							break;
						case 54:
							formatError = ref dest.Append(arg6);
							break;
						case 55:
							formatError = ref dest.Append(arg7);
							break;
						case 56:
							formatError = ref dest.Append(arg8);
							break;
						case 57:
							formatError = ref dest.Append(arg9);
							break;
						default:
							formatError = FormatError.BadFormatSpecifier;
							break;
						}
					}
					else if (b == 123)
					{
						formatError = ref dest.AppendRawByte(b);
					}
					else
					{
						formatError = FormatError.BadFormatSpecifier;
					}
				}
				else if (b == 125)
				{
					if (i < length)
					{
						b = unsafePtr[i++];
					}
					if (b == 125)
					{
						formatError = ref dest.AppendRawByte(b);
					}
					else
					{
						formatError = FormatError.BadFormatSpecifier;
					}
				}
				else
				{
					formatError = ref dest.AppendRawByte(b);
				}
				if (formatError != FormatError.None)
				{
					return formatError;
				}
			}
			return FormatError.None;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes)
		})]
		internal static FormatError Append<[IsUnmanaged] T>(this T fs, char a, char b) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			if ((FormatError.None | ref fs.Append(a) | ref fs.Append(b)) != FormatError.None)
			{
				return FormatError.Overflow;
			}
			return FormatError.None;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes)
		})]
		internal static FormatError Append<[IsUnmanaged] T>(this T fs, char a, char b, char c) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			if ((FormatError.None | ref fs.Append(a) | ref fs.Append(b) | ref fs.Append(c)) != FormatError.None)
			{
				return FormatError.Overflow;
			}
			return FormatError.None;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes)
		})]
		internal static FormatError Append<[IsUnmanaged] T>(this T fs, char a, char b, char c, char d, char e, char f, char g, char h) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			if ((FormatError.None | ref fs.Append(a) | ref fs.Append(b) | ref fs.Append(c) | ref fs.Append(d) | ref fs.Append(e) | ref fs.Append(f) | ref fs.Append(g) | ref fs.Append(h)) != FormatError.None)
			{
				return FormatError.Overflow;
			}
			return FormatError.None;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes)
		})]
		internal unsafe static FormatError AppendScientific<[IsUnmanaged] T>(this T fs, char* source, int sourceLength, int decimalExponent, char decimalSeparator = '.') where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FormatError result;
			if ((result = ref fs.Append(*source)) != FormatError.None)
			{
				return result;
			}
			if (sourceLength > 1)
			{
				if ((result = ref fs.Append(decimalSeparator)) != FormatError.None)
				{
					return result;
				}
				for (int i = 1; i < sourceLength; i++)
				{
					if ((result = ref fs.Append(source[i])) != FormatError.None)
					{
						return result;
					}
				}
			}
			if ((result = ref fs.Append('E')) != FormatError.None)
			{
				return result;
			}
			if (decimalExponent < 0)
			{
				if ((result = ref fs.Append('-')) != FormatError.None)
				{
					return result;
				}
				decimalExponent *= -1;
				decimalExponent -= sourceLength - 1;
			}
			else
			{
				if ((result = ref fs.Append('+')) != FormatError.None)
				{
					return result;
				}
				decimalExponent += sourceLength - 1;
			}
			char* ptr = stackalloc char[(UIntPtr)4];
			for (int j = 0; j < 2; j++)
			{
				int num = decimalExponent % 10;
				ptr[1 - j] = (char)(48 + num);
				decimalExponent /= 10;
			}
			for (int k = 0; k < 2; k++)
			{
				if ((result = ref fs.Append(ptr[k])) != FormatError.None)
				{
					return result;
				}
			}
			return FormatError.None;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes)
		})]
		internal static bool Found<[IsUnmanaged] T>(this T fs, ref int offset, char a, char b, char c) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			int num = offset;
			if ((ref fs.Read(ref offset).value | 32) == (int)a && (ref fs.Read(ref offset).value | 32) == (int)b && (ref fs.Read(ref offset).value | 32) == (int)c)
			{
				return true;
			}
			offset = num;
			return false;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes)
		})]
		internal static bool Found<[IsUnmanaged] T>(this T fs, ref int offset, char a, char b, char c, char d, char e, char f, char g, char h) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			int num = offset;
			if ((ref fs.Read(ref offset).value | 32) == (int)a && (ref fs.Read(ref offset).value | 32) == (int)b && (ref fs.Read(ref offset).value | 32) == (int)c && (ref fs.Read(ref offset).value | 32) == (int)d && (ref fs.Read(ref offset).value | 32) == (int)e && (ref fs.Read(ref offset).value | 32) == (int)f && (ref fs.Read(ref offset).value | 32) == (int)g && (ref fs.Read(ref offset).value | 32) == (int)h)
			{
				return true;
			}
			offset = num;
			return false;
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[Conditional("UNITY_DOTS_DEBUG")]
		internal static void CheckSubstringInRange(int strLength, int startIndex, int length)
		{
			if (startIndex < 0)
			{
				throw new ArgumentOutOfRangeException(string.Format("startIndex {0} must be positive.", startIndex));
			}
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException(string.Format("length {0} cannot be negative.", length));
			}
			if (startIndex > strLength)
			{
				throw new ArgumentOutOfRangeException(string.Format("startIndex {0} cannot be larger than string length {1}.", startIndex, strLength));
			}
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes)
		})]
		public static T Substring<[IsUnmanaged] T>(this T str, int startIndex, int length) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			length = math.min(length, str.Length - startIndex);
			T result = Activator.CreateInstance<T>();
			ref result.Append(str.GetUnsafePtr() + startIndex, length);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes)
		})]
		public static T Substring<[IsUnmanaged] T>(this T str, int startIndex) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			return ref str.Substring(startIndex, str.Length - startIndex);
		}

		public static NativeText Substring(this NativeText str, int startIndex, int length, AllocatorManager.AllocatorHandle allocator)
		{
			length = math.min(length, str.Length - startIndex);
			NativeText result = new NativeText(length, allocator);
			ref result.Append(str.GetUnsafePtr() + startIndex, length);
			return result;
		}

		public static NativeText Substring(this NativeText str, int startIndex, AllocatorManager.AllocatorHandle allocator)
		{
			return ref str.Substring(startIndex, str.Length - startIndex);
		}

		public unsafe static NativeText Substring(this NativeText str, int startIndex, int length)
		{
			return ref str.Substring(startIndex, length, str.m_Data->m_UntypedListData.Allocator);
		}

		public static NativeText Substring(this NativeText str, int startIndex)
		{
			return ref str.Substring(startIndex, str.Length - startIndex);
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes)
		})]
		public static int IndexOf<[IsUnmanaged] T>(this T fs, Unicode.Rune rune) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			int length = fs.Length;
			int num;
			for (int i = 0; i < length; i = num)
			{
				num = i;
				if (ref fs.Read(ref num).value == rune.value)
				{
					return i;
				}
			}
			return -1;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes)
		})]
		public unsafe static int IndexOf<[IsUnmanaged] T>(this T fs, byte* bytes, int bytesLen) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			byte* unsafePtr = fs.GetUnsafePtr();
			int length = fs.Length;
			int i = 0;
			IL_3C:
			while (i <= length - bytesLen)
			{
				for (int j = 0; j < bytesLen; j++)
				{
					if (unsafePtr[i + j] != bytes[j])
					{
						i++;
						goto IL_3C;
					}
				}
				return i;
			}
			return -1;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes)
		})]
		public unsafe static int IndexOf<[IsUnmanaged] T>(this T fs, byte* bytes, int bytesLen, int startIndex, int distance = 2147483647) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			byte* unsafePtr = fs.GetUnsafePtr();
			int length = fs.Length;
			int num = Math.Min(distance - 1, length - bytesLen);
			int i = startIndex;
			IL_4F:
			while (i <= num)
			{
				for (int j = 0; j < bytesLen; j++)
				{
					if (unsafePtr[i + j] != bytes[j])
					{
						i++;
						goto IL_4F;
					}
				}
				return i;
			}
			return -1;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public static int IndexOf<[IsUnmanaged] T, [IsUnmanaged] T2>(this T fs, in T2 other) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			ref T2 ptr = ref UnsafeUtilityExtensions.AsRef<T2>(other);
			return ref fs.IndexOf(ptr.GetUnsafePtr(), ptr.Length);
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public static int IndexOf<[IsUnmanaged] T, [IsUnmanaged] T2>(this T fs, in T2 other, int startIndex, int distance = 2147483647) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			ref T2 ptr = ref UnsafeUtilityExtensions.AsRef<T2>(other);
			return ref fs.IndexOf(ptr.GetUnsafePtr(), ptr.Length, startIndex, distance);
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public static bool Contains<[IsUnmanaged] T, [IsUnmanaged] T2>(this T fs, in T2 other) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			return ref fs.IndexOf(other) != -1;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes)
		})]
		public static int LastIndexOf<[IsUnmanaged] T>(this T fs, Unicode.Rune rune) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			if (Unicode.IsValidCodePoint(rune.value))
			{
				for (int i = fs.Length - 1; i >= 0; i--)
				{
					Unicode.Rune rune2 = ref fs.Peek(i);
					if (Unicode.IsValidCodePoint(rune2.value) && rune2.value == rune.value)
					{
						return i;
					}
				}
			}
			return -1;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes)
		})]
		public unsafe static int LastIndexOf<[IsUnmanaged] T>(this T fs, byte* bytes, int bytesLen) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			byte* unsafePtr = fs.GetUnsafePtr();
			int i = fs.Length - bytesLen;
			IL_3C:
			while (i >= 0)
			{
				for (int j = 0; j < bytesLen; j++)
				{
					if (unsafePtr[i + j] != bytes[j])
					{
						i--;
						goto IL_3C;
					}
				}
				return i;
			}
			return -1;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes)
		})]
		public unsafe static int LastIndexOf<[IsUnmanaged] T>(this T fs, byte* bytes, int bytesLen, int startIndex, int distance = 2147483647) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			byte* unsafePtr = fs.GetUnsafePtr();
			startIndex = Math.Min(fs.Length - bytesLen, startIndex);
			int num = Math.Max(0, startIndex - distance);
			int i = startIndex;
			IL_50:
			while (i >= num)
			{
				for (int j = 0; j < bytesLen; j++)
				{
					if (unsafePtr[i + j] != bytes[j])
					{
						i--;
						goto IL_50;
					}
				}
				return i;
			}
			return -1;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public static int LastIndexOf<[IsUnmanaged] T, [IsUnmanaged] T2>(this T fs, in T2 other) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			ref T2 ptr = ref UnsafeUtilityExtensions.AsRef<T2>(other);
			return ref fs.LastIndexOf(ptr.GetUnsafePtr(), ptr.Length);
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public static int LastIndexOf<[IsUnmanaged] T, [IsUnmanaged] T2>(this T fs, in T2 other, int startIndex, int distance = 2147483647) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			ref T2 ptr = ref UnsafeUtilityExtensions.AsRef<T2>(other);
			return ref fs.LastIndexOf(ptr.GetUnsafePtr(), ptr.Length, startIndex, distance);
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes)
		})]
		public unsafe static int CompareTo<[IsUnmanaged] T>(this T fs, byte* bytes, int bytesLen) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			byte* unsafePtr = fs.GetUnsafePtr();
			int length = fs.Length;
			int num = (length < bytesLen) ? length : bytesLen;
			for (int i = 0; i < num; i++)
			{
				if (unsafePtr[i] < bytes[i])
				{
					return -1;
				}
				if (unsafePtr[i] > bytes[i])
				{
					return 1;
				}
			}
			if (length < bytesLen)
			{
				return -1;
			}
			if (length > bytesLen)
			{
				return 1;
			}
			return 0;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public static int CompareTo<[IsUnmanaged] T, [IsUnmanaged] T2>(this T fs, in T2 other) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			ref T2 ptr = ref UnsafeUtilityExtensions.AsRef<T2>(other);
			return ref fs.CompareTo(ptr.GetUnsafePtr(), ptr.Length);
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes)
		})]
		public unsafe static bool Equals<[IsUnmanaged] T>(this T fs, byte* bytes, int bytesLen) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			byte* unsafePtr = fs.GetUnsafePtr();
			return fs.Length == bytesLen && (unsafePtr == bytes || ref fs.CompareTo(bytes, bytesLen) == 0);
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public static bool Equals<[IsUnmanaged] T, [IsUnmanaged] T2>(this T fs, in T2 other) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			ref T2 ptr = ref UnsafeUtilityExtensions.AsRef<T2>(other);
			return ref fs.Equals(ptr.GetUnsafePtr(), ptr.Length);
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes)
		})]
		public static Unicode.Rune Peek<[IsUnmanaged] T>(this T fs, int index) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			if (index >= fs.Length)
			{
				return Unicode.BadRune;
			}
			Unicode.Rune result;
			Unicode.Utf8ToUcs(out result, fs.GetUnsafePtr(), ref index, fs.Capacity);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes)
		})]
		public static Unicode.Rune Read<[IsUnmanaged] T>(this T fs, ref int index) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			if (index >= fs.Length)
			{
				return Unicode.BadRune;
			}
			Unicode.Rune result;
			Unicode.Utf8ToUcs(out result, fs.GetUnsafePtr(), ref index, fs.Capacity);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes)
		})]
		public static FormatError Write<[IsUnmanaged] T>(this T fs, ref int index, Unicode.Rune rune) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			if (Unicode.UcsToUtf8(fs.GetUnsafePtr(), ref index, fs.Capacity, rune) != ConversionError.None)
			{
				return FormatError.Overflow;
			}
			return FormatError.None;
		}

		[ExcludeFromBurstCompatTesting("Returns managed string")]
		public unsafe static string ConvertToString<[IsUnmanaged] T>(this T fs) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			char* ptr = stackalloc char[checked(unchecked((UIntPtr)(fs.Length * 2)) * 2)];
			int length = 0;
			Unicode.Utf8ToUtf16(fs.GetUnsafePtr(), fs.Length, ptr, out length, fs.Length * 2);
			return new string(ptr, 0, length);
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes)
		})]
		public unsafe static int ComputeHashCode<[IsUnmanaged] T>(this T fs) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			return (int)CollectionHelper.Hash((void*)fs.GetUnsafePtr(), fs.Length);
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes)
		})]
		public static int EffectiveSizeOf<[IsUnmanaged] T>(this T fs) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			return 2 + fs.Length + 1;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes)
		})]
		public unsafe static bool StartsWith<[IsUnmanaged] T>(this T fs, Unicode.Rune rune) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			int num = rune.LengthInUtf8Bytes();
			return fs.Length >= num && UTF8ArrayUnsafeUtility.StrCmp(fs.GetUnsafePtr(), num, &rune, 1) == 0;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public unsafe static bool StartsWith<[IsUnmanaged] T, [IsUnmanaged] U>(this T fs, in U other) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes where U : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			U u = other;
			int length = u.Length;
			if (fs.Length >= length)
			{
				byte* unsafePtr = fs.GetUnsafePtr();
				int utf8LengthInBytesA = length;
				u = other;
				return UTF8ArrayUnsafeUtility.StrCmp(unsafePtr, utf8LengthInBytesA, u.GetUnsafePtr(), length) == 0;
			}
			return false;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes)
		})]
		public unsafe static bool EndsWith<[IsUnmanaged] T>(this T fs, Unicode.Rune rune) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			int num = rune.LengthInUtf8Bytes();
			return fs.Length >= num && UTF8ArrayUnsafeUtility.StrCmp(fs.GetUnsafePtr() + fs.Length - num, num, &rune, 1) == 0;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public unsafe static bool EndsWith<[IsUnmanaged] T, [IsUnmanaged] U>(this T fs, in U other) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes where U : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			U u = other;
			int length = u.Length;
			if (fs.Length >= length)
			{
				byte* utf8BufferA = fs.GetUnsafePtr() + fs.Length - length;
				int utf8LengthInBytesA = length;
				u = other;
				return UTF8ArrayUnsafeUtility.StrCmp(utf8BufferA, utf8LengthInBytesA, u.GetUnsafePtr(), length) == 0;
			}
			return false;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes)
		})]
		internal unsafe static int TrimStartIndex<[IsUnmanaged] T>(this T fs) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			int length = fs.Length;
			byte* unsafePtr = fs.GetUnsafePtr();
			int num = 0;
			int num2;
			Unicode.Rune rune;
			do
			{
				num2 = num;
			}
			while (Unicode.Utf8ToUcs(out rune, unsafePtr, ref num, length) == ConversionError.None && rune.IsWhiteSpace());
			num -= num - num2;
			return num;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes)
		})]
		internal unsafe static int TrimStartIndex<[IsUnmanaged] T>(this T fs, ReadOnlySpan<Unicode.Rune> trimRunes) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			int length = fs.Length;
			byte* unsafePtr = fs.GetUnsafePtr();
			int num = 0;
			int num2;
			ConversionError conversionError;
			bool flag;
			do
			{
				num2 = num;
				Unicode.Rune rhs;
				conversionError = Unicode.Utf8ToUcs(out rhs, unsafePtr, ref num, length);
				flag = false;
				int num3 = 0;
				int length2 = trimRunes.Length;
				while (num3 < length2 && !flag)
				{
					flag |= (*trimRunes[num3] == rhs);
					num3++;
				}
			}
			while (conversionError == ConversionError.None && flag);
			num -= num - num2;
			return num;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes)
		})]
		internal unsafe static int TrimEndIndex<[IsUnmanaged] T>(this T fs) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			int length = fs.Length;
			byte* unsafePtr = fs.GetUnsafePtr();
			int num = length;
			int num2;
			Unicode.Rune rune;
			do
			{
				num2 = num;
			}
			while (Unicode.Utf8ToUcsReverse(out rune, unsafePtr, ref num, length) == ConversionError.None && rune.IsWhiteSpace());
			num += num2 - num;
			return num;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes)
		})]
		internal unsafe static int TrimEndIndex<[IsUnmanaged] T>(this T fs, ReadOnlySpan<Unicode.Rune> trimRunes) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			int length = fs.Length;
			byte* unsafePtr = fs.GetUnsafePtr();
			int num = length;
			int num2;
			ConversionError conversionError;
			bool flag;
			do
			{
				num2 = num;
				Unicode.Rune rhs;
				conversionError = Unicode.Utf8ToUcsReverse(out rhs, unsafePtr, ref num, length);
				flag = false;
				int num3 = 0;
				int length2 = trimRunes.Length;
				while (num3 < length2 && !flag)
				{
					flag |= (*trimRunes[num3] == rhs);
					num3++;
				}
			}
			while (conversionError == ConversionError.None && flag);
			num += num2 - num;
			return num;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes)
		})]
		public static T TrimStart<[IsUnmanaged] T>(this T fs) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			int num = ref fs.TrimStartIndex<T>();
			T result = Activator.CreateInstance<T>();
			ref result.Append(fs.GetUnsafePtr() + num, fs.Length - num);
			return result;
		}

		public static UnsafeText TrimStart(this UnsafeText fs, AllocatorManager.AllocatorHandle allocator)
		{
			int num = ref fs.TrimStartIndex<UnsafeText>();
			int num2 = fs.Length - num;
			UnsafeText result = new UnsafeText(num2, allocator);
			ref result.Append(fs.GetUnsafePtr() + num, num2);
			return result;
		}

		public static NativeText TrimStart(this NativeText fs, AllocatorManager.AllocatorHandle allocator)
		{
			int num = ref fs.TrimStartIndex<NativeText>();
			int num2 = fs.Length - num;
			NativeText result = new NativeText(num2, allocator);
			ref result.Append(fs.GetUnsafePtr() + num, num2);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes)
		})]
		public static T TrimStart<[IsUnmanaged] T>(this T fs, ReadOnlySpan<Unicode.Rune> trimRunes) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			int num = ref fs.TrimStartIndex(trimRunes);
			T result = Activator.CreateInstance<T>();
			ref result.Append(fs.GetUnsafePtr() + num, fs.Length - num);
			return result;
		}

		public static UnsafeText TrimStart(this UnsafeText fs, AllocatorManager.AllocatorHandle allocator, ReadOnlySpan<Unicode.Rune> trimRunes)
		{
			int num = ref fs.TrimStartIndex(trimRunes);
			int num2 = fs.Length - num;
			UnsafeText result = new UnsafeText(num2, allocator);
			ref result.Append(fs.GetUnsafePtr() + num, num2);
			return result;
		}

		public static NativeText TrimStart(this NativeText fs, AllocatorManager.AllocatorHandle allocator, ReadOnlySpan<Unicode.Rune> trimRunes)
		{
			int num = ref fs.TrimStartIndex(trimRunes);
			int num2 = fs.Length - num;
			NativeText result = new NativeText(num2, allocator);
			ref result.Append(fs.GetUnsafePtr() + num, num2);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes)
		})]
		public static T TrimEnd<[IsUnmanaged] T>(this T fs) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			int utf8BytesLength = ref fs.TrimEndIndex<T>();
			T result = Activator.CreateInstance<T>();
			ref result.Append(fs.GetUnsafePtr(), utf8BytesLength);
			return result;
		}

		public static UnsafeText TrimEnd(this UnsafeText fs, AllocatorManager.AllocatorHandle allocator)
		{
			int num = ref fs.TrimEndIndex<UnsafeText>();
			UnsafeText result = new UnsafeText(num, allocator);
			ref result.Append(fs.GetUnsafePtr(), num);
			return result;
		}

		public static NativeText TrimEnd(this NativeText fs, AllocatorManager.AllocatorHandle allocator)
		{
			int num = ref fs.TrimEndIndex<NativeText>();
			NativeText result = new NativeText(num, allocator);
			ref result.Append(fs.GetUnsafePtr(), num);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes)
		})]
		public static T TrimEnd<[IsUnmanaged] T>(this T fs, ReadOnlySpan<Unicode.Rune> trimRunes) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			int utf8BytesLength = ref fs.TrimEndIndex(trimRunes);
			T result = Activator.CreateInstance<T>();
			ref result.Append(fs.GetUnsafePtr(), utf8BytesLength);
			return result;
		}

		public static UnsafeText TrimEnd(this UnsafeText fs, AllocatorManager.AllocatorHandle allocator, ReadOnlySpan<Unicode.Rune> trimRunes)
		{
			int num = ref fs.TrimEndIndex(trimRunes);
			UnsafeText result = new UnsafeText(num, allocator);
			ref result.Append(fs.GetUnsafePtr(), num);
			return result;
		}

		public static NativeText TrimEnd(this NativeText fs, AllocatorManager.AllocatorHandle allocator, ReadOnlySpan<Unicode.Rune> trimRunes)
		{
			int num = ref fs.TrimEndIndex(trimRunes);
			NativeText result = new NativeText(num, allocator);
			ref result.Append(fs.GetUnsafePtr(), num);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes)
		})]
		public static T Trim<[IsUnmanaged] T>(this T fs) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			int num = ref fs.TrimStartIndex<T>();
			if (num == fs.Length)
			{
				return Activator.CreateInstance<T>();
			}
			int num2 = ref fs.TrimEndIndex<T>();
			T result = Activator.CreateInstance<T>();
			ref result.Append(fs.GetUnsafePtr() + num, num2 - num);
			return result;
		}

		public static UnsafeText Trim(this UnsafeText fs, AllocatorManager.AllocatorHandle allocator)
		{
			int num = ref fs.TrimStartIndex<UnsafeText>();
			if (num == fs.Length)
			{
				return new UnsafeText(0, allocator);
			}
			int num2 = ref fs.TrimEndIndex<UnsafeText>() - num;
			UnsafeText result = new UnsafeText(num2, allocator);
			ref result.Append(fs.GetUnsafePtr() + num, num2);
			return result;
		}

		public static NativeText Trim(this NativeText fs, AllocatorManager.AllocatorHandle allocator)
		{
			int num = ref fs.TrimStartIndex<NativeText>();
			if (num == fs.Length)
			{
				return new NativeText(0, allocator);
			}
			int num2 = ref fs.TrimEndIndex<NativeText>() - num;
			NativeText result = new NativeText(num2, allocator);
			ref result.Append(fs.GetUnsafePtr() + num, num2);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes)
		})]
		public static T Trim<[IsUnmanaged] T>(this T fs, ReadOnlySpan<Unicode.Rune> trimRunes) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			int num = ref fs.TrimStartIndex(trimRunes);
			if (num == fs.Length)
			{
				return Activator.CreateInstance<T>();
			}
			int num2 = ref fs.TrimEndIndex(trimRunes);
			T result = Activator.CreateInstance<T>();
			ref result.Append(fs.GetUnsafePtr() + num, num2 - num);
			return result;
		}

		public static UnsafeText Trim(this UnsafeText fs, AllocatorManager.AllocatorHandle allocator, ReadOnlySpan<Unicode.Rune> trimRunes)
		{
			int num = ref fs.TrimStartIndex(trimRunes);
			if (num == fs.Length)
			{
				return new UnsafeText(0, allocator);
			}
			int num2 = ref fs.TrimEndIndex<UnsafeText>() - num;
			UnsafeText result = new UnsafeText(num2, allocator);
			ref result.Append(fs.GetUnsafePtr() + num, num2);
			return result;
		}

		public static NativeText Trim(this NativeText fs, AllocatorManager.AllocatorHandle allocator, ReadOnlySpan<Unicode.Rune> trimRunes)
		{
			int num = ref fs.TrimStartIndex(trimRunes);
			if (num == fs.Length)
			{
				return new NativeText(0, allocator);
			}
			int num2 = ref fs.TrimEndIndex<NativeText>() - num;
			NativeText result = new NativeText(num2, allocator);
			ref result.Append(fs.GetUnsafePtr() + num, num2);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes)
		})]
		public unsafe static T ToLowerAscii<[IsUnmanaged] T>(this T fs) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			int length = fs.Length;
			byte* unsafePtr = fs.GetUnsafePtr();
			T result = Activator.CreateInstance<T>();
			ConversionError conversionError = ConversionError.None;
			int num = 0;
			while (num < length && conversionError == ConversionError.None)
			{
				Unicode.Rune rune;
				conversionError = Unicode.Utf8ToUcs(out rune, unsafePtr, ref num, length);
				ref result.Append(rune.ToLowerAscii());
			}
			return result;
		}

		public unsafe static UnsafeText ToLowerAscii(this UnsafeText fs, AllocatorManager.AllocatorHandle allocator)
		{
			int length = fs.Length;
			byte* unsafePtr = fs.GetUnsafePtr();
			UnsafeText result = new UnsafeText(length, allocator);
			ConversionError conversionError = ConversionError.None;
			int num = 0;
			while (num < length && conversionError == ConversionError.None)
			{
				Unicode.Rune rune;
				conversionError = Unicode.Utf8ToUcs(out rune, unsafePtr, ref num, length);
				ref result.Append(rune.ToLowerAscii());
			}
			return result;
		}

		public unsafe static NativeText ToLowerAscii(this NativeText fs, AllocatorManager.AllocatorHandle allocator)
		{
			int length = fs.Length;
			byte* unsafePtr = fs.GetUnsafePtr();
			NativeText result = new NativeText(length, allocator);
			ConversionError conversionError = ConversionError.None;
			int num = 0;
			while (num < length && conversionError == ConversionError.None)
			{
				Unicode.Rune rune;
				conversionError = Unicode.Utf8ToUcs(out rune, unsafePtr, ref num, length);
				ref result.Append(rune.ToLowerAscii());
			}
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes)
		})]
		public unsafe static T ToUpperAscii<[IsUnmanaged] T>(this T fs) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			int length = fs.Length;
			byte* unsafePtr = fs.GetUnsafePtr();
			T result = Activator.CreateInstance<T>();
			ConversionError conversionError = ConversionError.None;
			int num = 0;
			while (num < length && conversionError == ConversionError.None)
			{
				Unicode.Rune rune;
				conversionError = Unicode.Utf8ToUcs(out rune, unsafePtr, ref num, length);
				ref result.Append(rune.ToUpperAscii());
			}
			return result;
		}

		public unsafe static UnsafeText ToUpperAscii(this UnsafeText fs, AllocatorManager.AllocatorHandle allocator)
		{
			int length = fs.Length;
			byte* unsafePtr = fs.GetUnsafePtr();
			UnsafeText result = new UnsafeText(length, allocator);
			ConversionError conversionError = ConversionError.None;
			int num = 0;
			while (num < length && conversionError == ConversionError.None)
			{
				Unicode.Rune rune;
				conversionError = Unicode.Utf8ToUcs(out rune, unsafePtr, ref num, length);
				ref result.Append(rune.ToUpperAscii());
			}
			return result;
		}

		public unsafe static NativeText ToUpperAscii(this NativeText fs, AllocatorManager.AllocatorHandle allocator)
		{
			int length = fs.Length;
			byte* unsafePtr = fs.GetUnsafePtr();
			NativeText result = new NativeText(length, allocator);
			ConversionError conversionError = ConversionError.None;
			int num = 0;
			while (num < length && conversionError == ConversionError.None)
			{
				Unicode.Rune rune;
				conversionError = Unicode.Utf8ToUcs(out rune, unsafePtr, ref num, length);
				ref result.Append(rune.ToUpperAscii());
			}
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes)
		})]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool ParseLongInternal<[IsUnmanaged] T>(ref T fs, ref int offset, out long value) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			int num = offset;
			int num2 = 1;
			if (offset < fs.Length)
			{
				if (ref fs.Peek(offset).value == 43)
				{
					ref fs.Read(ref offset);
				}
				else if (ref fs.Peek(offset).value == 45)
				{
					num2 = -1;
					ref fs.Read(ref offset);
				}
			}
			int num3 = offset;
			value = 0L;
			while (offset < fs.Length && Unicode.Rune.IsDigit(ref fs.Peek(offset)))
			{
				value *= 10L;
				value += (long)(ref fs.Read(ref offset).value - 48);
			}
			value = (long)num2 * value;
			if (offset == num3)
			{
				offset = num;
				return false;
			}
			return true;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes)
		})]
		public static ParseError Parse<[IsUnmanaged] T>(this T fs, ref int offset, ref int output) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			long num;
			if (!FixedStringMethods.ParseLongInternal<T>(ref fs, ref offset, out num))
			{
				return ParseError.Syntax;
			}
			if (num > 2147483647L)
			{
				return ParseError.Overflow;
			}
			if (num < -2147483648L)
			{
				return ParseError.Overflow;
			}
			output = (int)num;
			return ParseError.None;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes)
		})]
		public static ParseError Parse<[IsUnmanaged] T>(this T fs, ref int offset, ref uint output) where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			long num;
			if (!FixedStringMethods.ParseLongInternal<T>(ref fs, ref offset, out num))
			{
				return ParseError.Syntax;
			}
			if (num > (long)((ulong)-1))
			{
				return ParseError.Overflow;
			}
			if (num < 0L)
			{
				return ParseError.Overflow;
			}
			output = (uint)num;
			return ParseError.None;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes)
		})]
		public static ParseError Parse<[IsUnmanaged] T>(this T fs, ref int offset, ref float output, char decimalSeparator = '.') where T : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			int num = offset;
			int num2 = 1;
			if (offset < fs.Length)
			{
				if (ref fs.Peek(offset).value == 43)
				{
					ref fs.Read(ref offset);
				}
				else if (ref fs.Peek(offset).value == 45)
				{
					num2 = -1;
					ref fs.Read(ref offset);
				}
			}
			if (ref fs.Found(ref offset, 'n', 'a', 'n'))
			{
				output = new FixedStringUtils.UintFloatUnion
				{
					uintValue = 4290772992U
				}.floatValue;
				return ParseError.None;
			}
			if (ref fs.Found(ref offset, 'i', 'n', 'f', 'i', 'n', 'i', 't', 'y'))
			{
				output = ((num2 == 1) ? float.PositiveInfinity : float.NegativeInfinity);
				return ParseError.None;
			}
			ulong num3 = 0UL;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			while (offset < fs.Length && Unicode.Rune.IsDigit(ref fs.Peek(offset)))
			{
				num6++;
				if (num4 < 9)
				{
					ulong num7 = num3 * 10UL + (ulong)((long)(ref fs.Peek(offset).value - 48));
					if (num7 > num3)
					{
						num4++;
					}
					num3 = num7;
				}
				else
				{
					num5--;
				}
				ref fs.Read(ref offset);
			}
			if (offset < fs.Length && ref fs.Peek(offset).value == (int)decimalSeparator)
			{
				ref fs.Read(ref offset);
				while (offset < fs.Length && Unicode.Rune.IsDigit(ref fs.Peek(offset)))
				{
					num6++;
					if (num4 < 9)
					{
						ulong num8 = num3 * 10UL + (ulong)((long)(ref fs.Peek(offset).value - 48));
						if (num8 > num3)
						{
							num4++;
						}
						num3 = num8;
						num5++;
					}
					ref fs.Read(ref offset);
				}
			}
			if (num6 == 0)
			{
				offset = num;
				return ParseError.Syntax;
			}
			int num9 = 0;
			int num10 = 1;
			if (offset < fs.Length && (ref fs.Peek(offset).value | 32) == 101)
			{
				ref fs.Read(ref offset);
				if (offset < fs.Length)
				{
					if (ref fs.Peek(offset).value == 43)
					{
						ref fs.Read(ref offset);
					}
					else if (ref fs.Peek(offset).value == 45)
					{
						num10 = -1;
						ref fs.Read(ref offset);
					}
				}
				int num11 = offset;
				while (offset < fs.Length && Unicode.Rune.IsDigit(ref fs.Peek(offset)))
				{
					num9 = num9 * 10 + (ref fs.Peek(offset).value - 48);
					ref fs.Read(ref offset);
				}
				if (offset == num11)
				{
					offset = num;
					return ParseError.Syntax;
				}
				if (num9 > 38)
				{
					if (num10 == 1)
					{
						return ParseError.Overflow;
					}
					return ParseError.Underflow;
				}
			}
			num9 = num9 * num10 - num5;
			ParseError parseError = FixedStringUtils.Base10ToBase2(ref output, num3, num9);
			if (parseError != ParseError.None)
			{
				return parseError;
			}
			output *= (float)num2;
			return ParseError.None;
		}
	}
}
