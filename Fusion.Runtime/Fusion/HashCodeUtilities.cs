using System;
using System.Runtime.CompilerServices;

namespace Fusion
{
	internal static class HashCodeUtilities
	{
		public static int GetHashDeterministic(this string str, int initialHash = 352654597)
		{
			return str.GetHashDeterministicInternal(str.Length, initialHash);
		}

		internal static int GetHashDeterministicInternal(this string str, int len, int initialHash)
		{
			int num = initialHash;
			int num2 = initialHash;
			for (int i = 0; i < len; i += 2)
			{
				num = ((num << 5) + num ^ (int)str[i]);
				bool flag = i == len - 1;
				if (flag)
				{
					break;
				}
				num2 = ((num2 << 5) + num2 ^ (int)str[i + 1]);
			}
			return num + num2 * 1566083941;
		}

		public static int CombineHashCodes(int a, int b)
		{
			return (a << 5) + a ^ b;
		}

		public static int CombineHashCodes(int a, int b, int c)
		{
			int num = (a << 5) + a ^ b;
			return (num << 5) + num ^ c;
		}

		public unsafe static int GetArrayHashCode<[IsUnmanaged] T>(T* ptr, int length, int initialHash = 352654597) where T : struct, ValueType
		{
			int num = initialHash;
			for (int i = 0; i < length; i++)
			{
				num = num * 31 + ptr[(IntPtr)i * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)].GetHashCode();
			}
			return num;
		}

		public static int GetHashCodeDeterministic(byte[] data, int initialHash = 0)
		{
			int num = initialHash;
			for (int i = 0; i < data.Length; i++)
			{
				num = num * 31 + (int)data[i];
			}
			return num;
		}

		public static int GetHashCodeDeterministic(string data, int initialHash = 0)
		{
			int num = initialHash;
			for (int i = 0; i < data.Length; i++)
			{
				num = num * 31 + (int)data[i];
			}
			return num;
		}

		public unsafe static int GetHashCodeDeterministic<[IsUnmanaged] T>(T data, int initialHash = 0) where T : struct, ValueType
		{
			return HashCodeUtilities.GetHashCodeDeterministic<T>(&data, initialHash);
		}

		public unsafe static int GetHashCodeDeterministic<[IsUnmanaged] T>(T* data, int initialHash = 0) where T : struct, ValueType
		{
			int num = initialHash;
			for (int i = 0; i < sizeof(T); i++)
			{
				num = num * 31 + (int)(*(byte*)(data + i / sizeof(T)));
			}
			return num;
		}

		public const int InitialHash = 352654597;
	}
}
