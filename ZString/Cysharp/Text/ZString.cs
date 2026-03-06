using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Cysharp.Text
{
	[NullableContext(1)]
	[Nullable(0)]
	public static class ZString
	{
		public unsafe static string Concat<[Nullable(2)] T1>(T1 arg1)
		{
			if (!(typeof(T1) == typeof(string)))
			{
				string result;
				using (Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true))
				{
					if (typeof(T1) == typeof(string))
					{
						if (arg1 != null)
						{
							utf16ValueStringBuilder.Append(*Unsafe.As<T1, string>(ref arg1));
						}
					}
					else if (typeof(T1) == typeof(int))
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T1, int>(ref arg1));
					}
					else
					{
						utf16ValueStringBuilder.Append<T1>(arg1);
					}
					result = utf16ValueStringBuilder.ToString();
				}
				return result;
			}
			if (arg1 == null)
			{
				return string.Empty;
			}
			return Unsafe.As<string>(arg1);
		}

		public unsafe static string Concat<[Nullable(2)] T1, [Nullable(2)] T2>(T1 arg1, T2 arg2)
		{
			string result;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true))
			{
				if (typeof(T1) == typeof(string))
				{
					if (arg1 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T1, string>(ref arg1));
					}
				}
				else if (typeof(T1) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T1, int>(ref arg1));
				}
				else
				{
					utf16ValueStringBuilder.Append<T1>(arg1);
				}
				if (typeof(T2) == typeof(string))
				{
					if (arg2 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T2, string>(ref arg2));
					}
				}
				else if (typeof(T2) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T2, int>(ref arg2));
				}
				else
				{
					utf16ValueStringBuilder.Append<T2>(arg2);
				}
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		public unsafe static string Concat<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3>(T1 arg1, T2 arg2, T3 arg3)
		{
			string result;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true))
			{
				if (typeof(T1) == typeof(string))
				{
					if (arg1 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T1, string>(ref arg1));
					}
				}
				else if (typeof(T1) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T1, int>(ref arg1));
				}
				else
				{
					utf16ValueStringBuilder.Append<T1>(arg1);
				}
				if (typeof(T2) == typeof(string))
				{
					if (arg2 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T2, string>(ref arg2));
					}
				}
				else if (typeof(T2) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T2, int>(ref arg2));
				}
				else
				{
					utf16ValueStringBuilder.Append<T2>(arg2);
				}
				if (typeof(T3) == typeof(string))
				{
					if (arg3 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T3, string>(ref arg3));
					}
				}
				else if (typeof(T3) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T3, int>(ref arg3));
				}
				else
				{
					utf16ValueStringBuilder.Append<T3>(arg3);
				}
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		public unsafe static string Concat<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			string result;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true))
			{
				if (typeof(T1) == typeof(string))
				{
					if (arg1 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T1, string>(ref arg1));
					}
				}
				else if (typeof(T1) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T1, int>(ref arg1));
				}
				else
				{
					utf16ValueStringBuilder.Append<T1>(arg1);
				}
				if (typeof(T2) == typeof(string))
				{
					if (arg2 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T2, string>(ref arg2));
					}
				}
				else if (typeof(T2) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T2, int>(ref arg2));
				}
				else
				{
					utf16ValueStringBuilder.Append<T2>(arg2);
				}
				if (typeof(T3) == typeof(string))
				{
					if (arg3 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T3, string>(ref arg3));
					}
				}
				else if (typeof(T3) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T3, int>(ref arg3));
				}
				else
				{
					utf16ValueStringBuilder.Append<T3>(arg3);
				}
				if (typeof(T4) == typeof(string))
				{
					if (arg4 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T4, string>(ref arg4));
					}
				}
				else if (typeof(T4) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T4, int>(ref arg4));
				}
				else
				{
					utf16ValueStringBuilder.Append<T4>(arg4);
				}
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		public unsafe static string Concat<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
		{
			string result;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true))
			{
				if (typeof(T1) == typeof(string))
				{
					if (arg1 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T1, string>(ref arg1));
					}
				}
				else if (typeof(T1) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T1, int>(ref arg1));
				}
				else
				{
					utf16ValueStringBuilder.Append<T1>(arg1);
				}
				if (typeof(T2) == typeof(string))
				{
					if (arg2 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T2, string>(ref arg2));
					}
				}
				else if (typeof(T2) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T2, int>(ref arg2));
				}
				else
				{
					utf16ValueStringBuilder.Append<T2>(arg2);
				}
				if (typeof(T3) == typeof(string))
				{
					if (arg3 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T3, string>(ref arg3));
					}
				}
				else if (typeof(T3) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T3, int>(ref arg3));
				}
				else
				{
					utf16ValueStringBuilder.Append<T3>(arg3);
				}
				if (typeof(T4) == typeof(string))
				{
					if (arg4 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T4, string>(ref arg4));
					}
				}
				else if (typeof(T4) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T4, int>(ref arg4));
				}
				else
				{
					utf16ValueStringBuilder.Append<T4>(arg4);
				}
				if (typeof(T5) == typeof(string))
				{
					if (arg5 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T5, string>(ref arg5));
					}
				}
				else if (typeof(T5) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T5, int>(ref arg5));
				}
				else
				{
					utf16ValueStringBuilder.Append<T5>(arg5);
				}
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		public unsafe static string Concat<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
		{
			string result;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true))
			{
				if (typeof(T1) == typeof(string))
				{
					if (arg1 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T1, string>(ref arg1));
					}
				}
				else if (typeof(T1) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T1, int>(ref arg1));
				}
				else
				{
					utf16ValueStringBuilder.Append<T1>(arg1);
				}
				if (typeof(T2) == typeof(string))
				{
					if (arg2 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T2, string>(ref arg2));
					}
				}
				else if (typeof(T2) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T2, int>(ref arg2));
				}
				else
				{
					utf16ValueStringBuilder.Append<T2>(arg2);
				}
				if (typeof(T3) == typeof(string))
				{
					if (arg3 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T3, string>(ref arg3));
					}
				}
				else if (typeof(T3) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T3, int>(ref arg3));
				}
				else
				{
					utf16ValueStringBuilder.Append<T3>(arg3);
				}
				if (typeof(T4) == typeof(string))
				{
					if (arg4 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T4, string>(ref arg4));
					}
				}
				else if (typeof(T4) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T4, int>(ref arg4));
				}
				else
				{
					utf16ValueStringBuilder.Append<T4>(arg4);
				}
				if (typeof(T5) == typeof(string))
				{
					if (arg5 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T5, string>(ref arg5));
					}
				}
				else if (typeof(T5) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T5, int>(ref arg5));
				}
				else
				{
					utf16ValueStringBuilder.Append<T5>(arg5);
				}
				if (typeof(T6) == typeof(string))
				{
					if (arg6 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T6, string>(ref arg6));
					}
				}
				else if (typeof(T6) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T6, int>(ref arg6));
				}
				else
				{
					utf16ValueStringBuilder.Append<T6>(arg6);
				}
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		public unsafe static string Concat<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
		{
			string result;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true))
			{
				if (typeof(T1) == typeof(string))
				{
					if (arg1 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T1, string>(ref arg1));
					}
				}
				else if (typeof(T1) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T1, int>(ref arg1));
				}
				else
				{
					utf16ValueStringBuilder.Append<T1>(arg1);
				}
				if (typeof(T2) == typeof(string))
				{
					if (arg2 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T2, string>(ref arg2));
					}
				}
				else if (typeof(T2) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T2, int>(ref arg2));
				}
				else
				{
					utf16ValueStringBuilder.Append<T2>(arg2);
				}
				if (typeof(T3) == typeof(string))
				{
					if (arg3 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T3, string>(ref arg3));
					}
				}
				else if (typeof(T3) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T3, int>(ref arg3));
				}
				else
				{
					utf16ValueStringBuilder.Append<T3>(arg3);
				}
				if (typeof(T4) == typeof(string))
				{
					if (arg4 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T4, string>(ref arg4));
					}
				}
				else if (typeof(T4) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T4, int>(ref arg4));
				}
				else
				{
					utf16ValueStringBuilder.Append<T4>(arg4);
				}
				if (typeof(T5) == typeof(string))
				{
					if (arg5 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T5, string>(ref arg5));
					}
				}
				else if (typeof(T5) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T5, int>(ref arg5));
				}
				else
				{
					utf16ValueStringBuilder.Append<T5>(arg5);
				}
				if (typeof(T6) == typeof(string))
				{
					if (arg6 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T6, string>(ref arg6));
					}
				}
				else if (typeof(T6) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T6, int>(ref arg6));
				}
				else
				{
					utf16ValueStringBuilder.Append<T6>(arg6);
				}
				if (typeof(T7) == typeof(string))
				{
					if (arg7 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T7, string>(ref arg7));
					}
				}
				else if (typeof(T7) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T7, int>(ref arg7));
				}
				else
				{
					utf16ValueStringBuilder.Append<T7>(arg7);
				}
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		public unsafe static string Concat<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
		{
			string result;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true))
			{
				if (typeof(T1) == typeof(string))
				{
					if (arg1 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T1, string>(ref arg1));
					}
				}
				else if (typeof(T1) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T1, int>(ref arg1));
				}
				else
				{
					utf16ValueStringBuilder.Append<T1>(arg1);
				}
				if (typeof(T2) == typeof(string))
				{
					if (arg2 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T2, string>(ref arg2));
					}
				}
				else if (typeof(T2) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T2, int>(ref arg2));
				}
				else
				{
					utf16ValueStringBuilder.Append<T2>(arg2);
				}
				if (typeof(T3) == typeof(string))
				{
					if (arg3 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T3, string>(ref arg3));
					}
				}
				else if (typeof(T3) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T3, int>(ref arg3));
				}
				else
				{
					utf16ValueStringBuilder.Append<T3>(arg3);
				}
				if (typeof(T4) == typeof(string))
				{
					if (arg4 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T4, string>(ref arg4));
					}
				}
				else if (typeof(T4) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T4, int>(ref arg4));
				}
				else
				{
					utf16ValueStringBuilder.Append<T4>(arg4);
				}
				if (typeof(T5) == typeof(string))
				{
					if (arg5 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T5, string>(ref arg5));
					}
				}
				else if (typeof(T5) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T5, int>(ref arg5));
				}
				else
				{
					utf16ValueStringBuilder.Append<T5>(arg5);
				}
				if (typeof(T6) == typeof(string))
				{
					if (arg6 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T6, string>(ref arg6));
					}
				}
				else if (typeof(T6) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T6, int>(ref arg6));
				}
				else
				{
					utf16ValueStringBuilder.Append<T6>(arg6);
				}
				if (typeof(T7) == typeof(string))
				{
					if (arg7 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T7, string>(ref arg7));
					}
				}
				else if (typeof(T7) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T7, int>(ref arg7));
				}
				else
				{
					utf16ValueStringBuilder.Append<T7>(arg7);
				}
				if (typeof(T8) == typeof(string))
				{
					if (arg8 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T8, string>(ref arg8));
					}
				}
				else if (typeof(T8) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T8, int>(ref arg8));
				}
				else
				{
					utf16ValueStringBuilder.Append<T8>(arg8);
				}
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		public unsafe static string Concat<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8, [Nullable(2)] T9>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
		{
			string result;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true))
			{
				if (typeof(T1) == typeof(string))
				{
					if (arg1 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T1, string>(ref arg1));
					}
				}
				else if (typeof(T1) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T1, int>(ref arg1));
				}
				else
				{
					utf16ValueStringBuilder.Append<T1>(arg1);
				}
				if (typeof(T2) == typeof(string))
				{
					if (arg2 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T2, string>(ref arg2));
					}
				}
				else if (typeof(T2) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T2, int>(ref arg2));
				}
				else
				{
					utf16ValueStringBuilder.Append<T2>(arg2);
				}
				if (typeof(T3) == typeof(string))
				{
					if (arg3 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T3, string>(ref arg3));
					}
				}
				else if (typeof(T3) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T3, int>(ref arg3));
				}
				else
				{
					utf16ValueStringBuilder.Append<T3>(arg3);
				}
				if (typeof(T4) == typeof(string))
				{
					if (arg4 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T4, string>(ref arg4));
					}
				}
				else if (typeof(T4) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T4, int>(ref arg4));
				}
				else
				{
					utf16ValueStringBuilder.Append<T4>(arg4);
				}
				if (typeof(T5) == typeof(string))
				{
					if (arg5 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T5, string>(ref arg5));
					}
				}
				else if (typeof(T5) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T5, int>(ref arg5));
				}
				else
				{
					utf16ValueStringBuilder.Append<T5>(arg5);
				}
				if (typeof(T6) == typeof(string))
				{
					if (arg6 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T6, string>(ref arg6));
					}
				}
				else if (typeof(T6) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T6, int>(ref arg6));
				}
				else
				{
					utf16ValueStringBuilder.Append<T6>(arg6);
				}
				if (typeof(T7) == typeof(string))
				{
					if (arg7 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T7, string>(ref arg7));
					}
				}
				else if (typeof(T7) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T7, int>(ref arg7));
				}
				else
				{
					utf16ValueStringBuilder.Append<T7>(arg7);
				}
				if (typeof(T8) == typeof(string))
				{
					if (arg8 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T8, string>(ref arg8));
					}
				}
				else if (typeof(T8) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T8, int>(ref arg8));
				}
				else
				{
					utf16ValueStringBuilder.Append<T8>(arg8);
				}
				if (typeof(T9) == typeof(string))
				{
					if (arg9 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T9, string>(ref arg9));
					}
				}
				else if (typeof(T9) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T9, int>(ref arg9));
				}
				else
				{
					utf16ValueStringBuilder.Append<T9>(arg9);
				}
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		public unsafe static string Concat<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8, [Nullable(2)] T9, [Nullable(2)] T10>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
		{
			string result;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true))
			{
				if (typeof(T1) == typeof(string))
				{
					if (arg1 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T1, string>(ref arg1));
					}
				}
				else if (typeof(T1) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T1, int>(ref arg1));
				}
				else
				{
					utf16ValueStringBuilder.Append<T1>(arg1);
				}
				if (typeof(T2) == typeof(string))
				{
					if (arg2 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T2, string>(ref arg2));
					}
				}
				else if (typeof(T2) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T2, int>(ref arg2));
				}
				else
				{
					utf16ValueStringBuilder.Append<T2>(arg2);
				}
				if (typeof(T3) == typeof(string))
				{
					if (arg3 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T3, string>(ref arg3));
					}
				}
				else if (typeof(T3) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T3, int>(ref arg3));
				}
				else
				{
					utf16ValueStringBuilder.Append<T3>(arg3);
				}
				if (typeof(T4) == typeof(string))
				{
					if (arg4 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T4, string>(ref arg4));
					}
				}
				else if (typeof(T4) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T4, int>(ref arg4));
				}
				else
				{
					utf16ValueStringBuilder.Append<T4>(arg4);
				}
				if (typeof(T5) == typeof(string))
				{
					if (arg5 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T5, string>(ref arg5));
					}
				}
				else if (typeof(T5) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T5, int>(ref arg5));
				}
				else
				{
					utf16ValueStringBuilder.Append<T5>(arg5);
				}
				if (typeof(T6) == typeof(string))
				{
					if (arg6 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T6, string>(ref arg6));
					}
				}
				else if (typeof(T6) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T6, int>(ref arg6));
				}
				else
				{
					utf16ValueStringBuilder.Append<T6>(arg6);
				}
				if (typeof(T7) == typeof(string))
				{
					if (arg7 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T7, string>(ref arg7));
					}
				}
				else if (typeof(T7) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T7, int>(ref arg7));
				}
				else
				{
					utf16ValueStringBuilder.Append<T7>(arg7);
				}
				if (typeof(T8) == typeof(string))
				{
					if (arg8 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T8, string>(ref arg8));
					}
				}
				else if (typeof(T8) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T8, int>(ref arg8));
				}
				else
				{
					utf16ValueStringBuilder.Append<T8>(arg8);
				}
				if (typeof(T9) == typeof(string))
				{
					if (arg9 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T9, string>(ref arg9));
					}
				}
				else if (typeof(T9) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T9, int>(ref arg9));
				}
				else
				{
					utf16ValueStringBuilder.Append<T9>(arg9);
				}
				if (typeof(T10) == typeof(string))
				{
					if (arg10 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T10, string>(ref arg10));
					}
				}
				else if (typeof(T10) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T10, int>(ref arg10));
				}
				else
				{
					utf16ValueStringBuilder.Append<T10>(arg10);
				}
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		public unsafe static string Concat<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8, [Nullable(2)] T9, [Nullable(2)] T10, [Nullable(2)] T11>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
		{
			string result;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true))
			{
				if (typeof(T1) == typeof(string))
				{
					if (arg1 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T1, string>(ref arg1));
					}
				}
				else if (typeof(T1) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T1, int>(ref arg1));
				}
				else
				{
					utf16ValueStringBuilder.Append<T1>(arg1);
				}
				if (typeof(T2) == typeof(string))
				{
					if (arg2 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T2, string>(ref arg2));
					}
				}
				else if (typeof(T2) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T2, int>(ref arg2));
				}
				else
				{
					utf16ValueStringBuilder.Append<T2>(arg2);
				}
				if (typeof(T3) == typeof(string))
				{
					if (arg3 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T3, string>(ref arg3));
					}
				}
				else if (typeof(T3) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T3, int>(ref arg3));
				}
				else
				{
					utf16ValueStringBuilder.Append<T3>(arg3);
				}
				if (typeof(T4) == typeof(string))
				{
					if (arg4 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T4, string>(ref arg4));
					}
				}
				else if (typeof(T4) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T4, int>(ref arg4));
				}
				else
				{
					utf16ValueStringBuilder.Append<T4>(arg4);
				}
				if (typeof(T5) == typeof(string))
				{
					if (arg5 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T5, string>(ref arg5));
					}
				}
				else if (typeof(T5) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T5, int>(ref arg5));
				}
				else
				{
					utf16ValueStringBuilder.Append<T5>(arg5);
				}
				if (typeof(T6) == typeof(string))
				{
					if (arg6 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T6, string>(ref arg6));
					}
				}
				else if (typeof(T6) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T6, int>(ref arg6));
				}
				else
				{
					utf16ValueStringBuilder.Append<T6>(arg6);
				}
				if (typeof(T7) == typeof(string))
				{
					if (arg7 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T7, string>(ref arg7));
					}
				}
				else if (typeof(T7) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T7, int>(ref arg7));
				}
				else
				{
					utf16ValueStringBuilder.Append<T7>(arg7);
				}
				if (typeof(T8) == typeof(string))
				{
					if (arg8 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T8, string>(ref arg8));
					}
				}
				else if (typeof(T8) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T8, int>(ref arg8));
				}
				else
				{
					utf16ValueStringBuilder.Append<T8>(arg8);
				}
				if (typeof(T9) == typeof(string))
				{
					if (arg9 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T9, string>(ref arg9));
					}
				}
				else if (typeof(T9) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T9, int>(ref arg9));
				}
				else
				{
					utf16ValueStringBuilder.Append<T9>(arg9);
				}
				if (typeof(T10) == typeof(string))
				{
					if (arg10 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T10, string>(ref arg10));
					}
				}
				else if (typeof(T10) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T10, int>(ref arg10));
				}
				else
				{
					utf16ValueStringBuilder.Append<T10>(arg10);
				}
				if (typeof(T11) == typeof(string))
				{
					if (arg11 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T11, string>(ref arg11));
					}
				}
				else if (typeof(T11) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T11, int>(ref arg11));
				}
				else
				{
					utf16ValueStringBuilder.Append<T11>(arg11);
				}
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		public unsafe static string Concat<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8, [Nullable(2)] T9, [Nullable(2)] T10, [Nullable(2)] T11, [Nullable(2)] T12>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
		{
			string result;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true))
			{
				if (typeof(T1) == typeof(string))
				{
					if (arg1 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T1, string>(ref arg1));
					}
				}
				else if (typeof(T1) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T1, int>(ref arg1));
				}
				else
				{
					utf16ValueStringBuilder.Append<T1>(arg1);
				}
				if (typeof(T2) == typeof(string))
				{
					if (arg2 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T2, string>(ref arg2));
					}
				}
				else if (typeof(T2) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T2, int>(ref arg2));
				}
				else
				{
					utf16ValueStringBuilder.Append<T2>(arg2);
				}
				if (typeof(T3) == typeof(string))
				{
					if (arg3 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T3, string>(ref arg3));
					}
				}
				else if (typeof(T3) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T3, int>(ref arg3));
				}
				else
				{
					utf16ValueStringBuilder.Append<T3>(arg3);
				}
				if (typeof(T4) == typeof(string))
				{
					if (arg4 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T4, string>(ref arg4));
					}
				}
				else if (typeof(T4) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T4, int>(ref arg4));
				}
				else
				{
					utf16ValueStringBuilder.Append<T4>(arg4);
				}
				if (typeof(T5) == typeof(string))
				{
					if (arg5 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T5, string>(ref arg5));
					}
				}
				else if (typeof(T5) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T5, int>(ref arg5));
				}
				else
				{
					utf16ValueStringBuilder.Append<T5>(arg5);
				}
				if (typeof(T6) == typeof(string))
				{
					if (arg6 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T6, string>(ref arg6));
					}
				}
				else if (typeof(T6) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T6, int>(ref arg6));
				}
				else
				{
					utf16ValueStringBuilder.Append<T6>(arg6);
				}
				if (typeof(T7) == typeof(string))
				{
					if (arg7 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T7, string>(ref arg7));
					}
				}
				else if (typeof(T7) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T7, int>(ref arg7));
				}
				else
				{
					utf16ValueStringBuilder.Append<T7>(arg7);
				}
				if (typeof(T8) == typeof(string))
				{
					if (arg8 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T8, string>(ref arg8));
					}
				}
				else if (typeof(T8) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T8, int>(ref arg8));
				}
				else
				{
					utf16ValueStringBuilder.Append<T8>(arg8);
				}
				if (typeof(T9) == typeof(string))
				{
					if (arg9 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T9, string>(ref arg9));
					}
				}
				else if (typeof(T9) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T9, int>(ref arg9));
				}
				else
				{
					utf16ValueStringBuilder.Append<T9>(arg9);
				}
				if (typeof(T10) == typeof(string))
				{
					if (arg10 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T10, string>(ref arg10));
					}
				}
				else if (typeof(T10) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T10, int>(ref arg10));
				}
				else
				{
					utf16ValueStringBuilder.Append<T10>(arg10);
				}
				if (typeof(T11) == typeof(string))
				{
					if (arg11 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T11, string>(ref arg11));
					}
				}
				else if (typeof(T11) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T11, int>(ref arg11));
				}
				else
				{
					utf16ValueStringBuilder.Append<T11>(arg11);
				}
				if (typeof(T12) == typeof(string))
				{
					if (arg12 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T12, string>(ref arg12));
					}
				}
				else if (typeof(T12) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T12, int>(ref arg12));
				}
				else
				{
					utf16ValueStringBuilder.Append<T12>(arg12);
				}
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		public unsafe static string Concat<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8, [Nullable(2)] T9, [Nullable(2)] T10, [Nullable(2)] T11, [Nullable(2)] T12, [Nullable(2)] T13>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
		{
			string result;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true))
			{
				if (typeof(T1) == typeof(string))
				{
					if (arg1 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T1, string>(ref arg1));
					}
				}
				else if (typeof(T1) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T1, int>(ref arg1));
				}
				else
				{
					utf16ValueStringBuilder.Append<T1>(arg1);
				}
				if (typeof(T2) == typeof(string))
				{
					if (arg2 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T2, string>(ref arg2));
					}
				}
				else if (typeof(T2) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T2, int>(ref arg2));
				}
				else
				{
					utf16ValueStringBuilder.Append<T2>(arg2);
				}
				if (typeof(T3) == typeof(string))
				{
					if (arg3 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T3, string>(ref arg3));
					}
				}
				else if (typeof(T3) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T3, int>(ref arg3));
				}
				else
				{
					utf16ValueStringBuilder.Append<T3>(arg3);
				}
				if (typeof(T4) == typeof(string))
				{
					if (arg4 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T4, string>(ref arg4));
					}
				}
				else if (typeof(T4) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T4, int>(ref arg4));
				}
				else
				{
					utf16ValueStringBuilder.Append<T4>(arg4);
				}
				if (typeof(T5) == typeof(string))
				{
					if (arg5 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T5, string>(ref arg5));
					}
				}
				else if (typeof(T5) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T5, int>(ref arg5));
				}
				else
				{
					utf16ValueStringBuilder.Append<T5>(arg5);
				}
				if (typeof(T6) == typeof(string))
				{
					if (arg6 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T6, string>(ref arg6));
					}
				}
				else if (typeof(T6) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T6, int>(ref arg6));
				}
				else
				{
					utf16ValueStringBuilder.Append<T6>(arg6);
				}
				if (typeof(T7) == typeof(string))
				{
					if (arg7 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T7, string>(ref arg7));
					}
				}
				else if (typeof(T7) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T7, int>(ref arg7));
				}
				else
				{
					utf16ValueStringBuilder.Append<T7>(arg7);
				}
				if (typeof(T8) == typeof(string))
				{
					if (arg8 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T8, string>(ref arg8));
					}
				}
				else if (typeof(T8) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T8, int>(ref arg8));
				}
				else
				{
					utf16ValueStringBuilder.Append<T8>(arg8);
				}
				if (typeof(T9) == typeof(string))
				{
					if (arg9 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T9, string>(ref arg9));
					}
				}
				else if (typeof(T9) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T9, int>(ref arg9));
				}
				else
				{
					utf16ValueStringBuilder.Append<T9>(arg9);
				}
				if (typeof(T10) == typeof(string))
				{
					if (arg10 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T10, string>(ref arg10));
					}
				}
				else if (typeof(T10) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T10, int>(ref arg10));
				}
				else
				{
					utf16ValueStringBuilder.Append<T10>(arg10);
				}
				if (typeof(T11) == typeof(string))
				{
					if (arg11 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T11, string>(ref arg11));
					}
				}
				else if (typeof(T11) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T11, int>(ref arg11));
				}
				else
				{
					utf16ValueStringBuilder.Append<T11>(arg11);
				}
				if (typeof(T12) == typeof(string))
				{
					if (arg12 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T12, string>(ref arg12));
					}
				}
				else if (typeof(T12) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T12, int>(ref arg12));
				}
				else
				{
					utf16ValueStringBuilder.Append<T12>(arg12);
				}
				if (typeof(T13) == typeof(string))
				{
					if (arg13 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T13, string>(ref arg13));
					}
				}
				else if (typeof(T13) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T13, int>(ref arg13));
				}
				else
				{
					utf16ValueStringBuilder.Append<T13>(arg13);
				}
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		public unsafe static string Concat<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8, [Nullable(2)] T9, [Nullable(2)] T10, [Nullable(2)] T11, [Nullable(2)] T12, [Nullable(2)] T13, [Nullable(2)] T14>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
		{
			string result;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true))
			{
				if (typeof(T1) == typeof(string))
				{
					if (arg1 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T1, string>(ref arg1));
					}
				}
				else if (typeof(T1) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T1, int>(ref arg1));
				}
				else
				{
					utf16ValueStringBuilder.Append<T1>(arg1);
				}
				if (typeof(T2) == typeof(string))
				{
					if (arg2 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T2, string>(ref arg2));
					}
				}
				else if (typeof(T2) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T2, int>(ref arg2));
				}
				else
				{
					utf16ValueStringBuilder.Append<T2>(arg2);
				}
				if (typeof(T3) == typeof(string))
				{
					if (arg3 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T3, string>(ref arg3));
					}
				}
				else if (typeof(T3) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T3, int>(ref arg3));
				}
				else
				{
					utf16ValueStringBuilder.Append<T3>(arg3);
				}
				if (typeof(T4) == typeof(string))
				{
					if (arg4 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T4, string>(ref arg4));
					}
				}
				else if (typeof(T4) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T4, int>(ref arg4));
				}
				else
				{
					utf16ValueStringBuilder.Append<T4>(arg4);
				}
				if (typeof(T5) == typeof(string))
				{
					if (arg5 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T5, string>(ref arg5));
					}
				}
				else if (typeof(T5) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T5, int>(ref arg5));
				}
				else
				{
					utf16ValueStringBuilder.Append<T5>(arg5);
				}
				if (typeof(T6) == typeof(string))
				{
					if (arg6 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T6, string>(ref arg6));
					}
				}
				else if (typeof(T6) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T6, int>(ref arg6));
				}
				else
				{
					utf16ValueStringBuilder.Append<T6>(arg6);
				}
				if (typeof(T7) == typeof(string))
				{
					if (arg7 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T7, string>(ref arg7));
					}
				}
				else if (typeof(T7) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T7, int>(ref arg7));
				}
				else
				{
					utf16ValueStringBuilder.Append<T7>(arg7);
				}
				if (typeof(T8) == typeof(string))
				{
					if (arg8 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T8, string>(ref arg8));
					}
				}
				else if (typeof(T8) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T8, int>(ref arg8));
				}
				else
				{
					utf16ValueStringBuilder.Append<T8>(arg8);
				}
				if (typeof(T9) == typeof(string))
				{
					if (arg9 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T9, string>(ref arg9));
					}
				}
				else if (typeof(T9) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T9, int>(ref arg9));
				}
				else
				{
					utf16ValueStringBuilder.Append<T9>(arg9);
				}
				if (typeof(T10) == typeof(string))
				{
					if (arg10 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T10, string>(ref arg10));
					}
				}
				else if (typeof(T10) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T10, int>(ref arg10));
				}
				else
				{
					utf16ValueStringBuilder.Append<T10>(arg10);
				}
				if (typeof(T11) == typeof(string))
				{
					if (arg11 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T11, string>(ref arg11));
					}
				}
				else if (typeof(T11) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T11, int>(ref arg11));
				}
				else
				{
					utf16ValueStringBuilder.Append<T11>(arg11);
				}
				if (typeof(T12) == typeof(string))
				{
					if (arg12 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T12, string>(ref arg12));
					}
				}
				else if (typeof(T12) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T12, int>(ref arg12));
				}
				else
				{
					utf16ValueStringBuilder.Append<T12>(arg12);
				}
				if (typeof(T13) == typeof(string))
				{
					if (arg13 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T13, string>(ref arg13));
					}
				}
				else if (typeof(T13) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T13, int>(ref arg13));
				}
				else
				{
					utf16ValueStringBuilder.Append<T13>(arg13);
				}
				if (typeof(T14) == typeof(string))
				{
					if (arg14 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T14, string>(ref arg14));
					}
				}
				else if (typeof(T14) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T14, int>(ref arg14));
				}
				else
				{
					utf16ValueStringBuilder.Append<T14>(arg14);
				}
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		public unsafe static string Concat<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8, [Nullable(2)] T9, [Nullable(2)] T10, [Nullable(2)] T11, [Nullable(2)] T12, [Nullable(2)] T13, [Nullable(2)] T14, [Nullable(2)] T15>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
		{
			string result;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true))
			{
				if (typeof(T1) == typeof(string))
				{
					if (arg1 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T1, string>(ref arg1));
					}
				}
				else if (typeof(T1) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T1, int>(ref arg1));
				}
				else
				{
					utf16ValueStringBuilder.Append<T1>(arg1);
				}
				if (typeof(T2) == typeof(string))
				{
					if (arg2 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T2, string>(ref arg2));
					}
				}
				else if (typeof(T2) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T2, int>(ref arg2));
				}
				else
				{
					utf16ValueStringBuilder.Append<T2>(arg2);
				}
				if (typeof(T3) == typeof(string))
				{
					if (arg3 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T3, string>(ref arg3));
					}
				}
				else if (typeof(T3) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T3, int>(ref arg3));
				}
				else
				{
					utf16ValueStringBuilder.Append<T3>(arg3);
				}
				if (typeof(T4) == typeof(string))
				{
					if (arg4 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T4, string>(ref arg4));
					}
				}
				else if (typeof(T4) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T4, int>(ref arg4));
				}
				else
				{
					utf16ValueStringBuilder.Append<T4>(arg4);
				}
				if (typeof(T5) == typeof(string))
				{
					if (arg5 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T5, string>(ref arg5));
					}
				}
				else if (typeof(T5) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T5, int>(ref arg5));
				}
				else
				{
					utf16ValueStringBuilder.Append<T5>(arg5);
				}
				if (typeof(T6) == typeof(string))
				{
					if (arg6 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T6, string>(ref arg6));
					}
				}
				else if (typeof(T6) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T6, int>(ref arg6));
				}
				else
				{
					utf16ValueStringBuilder.Append<T6>(arg6);
				}
				if (typeof(T7) == typeof(string))
				{
					if (arg7 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T7, string>(ref arg7));
					}
				}
				else if (typeof(T7) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T7, int>(ref arg7));
				}
				else
				{
					utf16ValueStringBuilder.Append<T7>(arg7);
				}
				if (typeof(T8) == typeof(string))
				{
					if (arg8 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T8, string>(ref arg8));
					}
				}
				else if (typeof(T8) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T8, int>(ref arg8));
				}
				else
				{
					utf16ValueStringBuilder.Append<T8>(arg8);
				}
				if (typeof(T9) == typeof(string))
				{
					if (arg9 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T9, string>(ref arg9));
					}
				}
				else if (typeof(T9) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T9, int>(ref arg9));
				}
				else
				{
					utf16ValueStringBuilder.Append<T9>(arg9);
				}
				if (typeof(T10) == typeof(string))
				{
					if (arg10 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T10, string>(ref arg10));
					}
				}
				else if (typeof(T10) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T10, int>(ref arg10));
				}
				else
				{
					utf16ValueStringBuilder.Append<T10>(arg10);
				}
				if (typeof(T11) == typeof(string))
				{
					if (arg11 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T11, string>(ref arg11));
					}
				}
				else if (typeof(T11) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T11, int>(ref arg11));
				}
				else
				{
					utf16ValueStringBuilder.Append<T11>(arg11);
				}
				if (typeof(T12) == typeof(string))
				{
					if (arg12 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T12, string>(ref arg12));
					}
				}
				else if (typeof(T12) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T12, int>(ref arg12));
				}
				else
				{
					utf16ValueStringBuilder.Append<T12>(arg12);
				}
				if (typeof(T13) == typeof(string))
				{
					if (arg13 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T13, string>(ref arg13));
					}
				}
				else if (typeof(T13) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T13, int>(ref arg13));
				}
				else
				{
					utf16ValueStringBuilder.Append<T13>(arg13);
				}
				if (typeof(T14) == typeof(string))
				{
					if (arg14 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T14, string>(ref arg14));
					}
				}
				else if (typeof(T14) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T14, int>(ref arg14));
				}
				else
				{
					utf16ValueStringBuilder.Append<T14>(arg14);
				}
				if (typeof(T15) == typeof(string))
				{
					if (arg15 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T15, string>(ref arg15));
					}
				}
				else if (typeof(T15) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T15, int>(ref arg15));
				}
				else
				{
					utf16ValueStringBuilder.Append<T15>(arg15);
				}
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		public unsafe static string Concat<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8, [Nullable(2)] T9, [Nullable(2)] T10, [Nullable(2)] T11, [Nullable(2)] T12, [Nullable(2)] T13, [Nullable(2)] T14, [Nullable(2)] T15, [Nullable(2)] T16>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16)
		{
			string result;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true))
			{
				if (typeof(T1) == typeof(string))
				{
					if (arg1 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T1, string>(ref arg1));
					}
				}
				else if (typeof(T1) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T1, int>(ref arg1));
				}
				else
				{
					utf16ValueStringBuilder.Append<T1>(arg1);
				}
				if (typeof(T2) == typeof(string))
				{
					if (arg2 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T2, string>(ref arg2));
					}
				}
				else if (typeof(T2) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T2, int>(ref arg2));
				}
				else
				{
					utf16ValueStringBuilder.Append<T2>(arg2);
				}
				if (typeof(T3) == typeof(string))
				{
					if (arg3 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T3, string>(ref arg3));
					}
				}
				else if (typeof(T3) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T3, int>(ref arg3));
				}
				else
				{
					utf16ValueStringBuilder.Append<T3>(arg3);
				}
				if (typeof(T4) == typeof(string))
				{
					if (arg4 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T4, string>(ref arg4));
					}
				}
				else if (typeof(T4) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T4, int>(ref arg4));
				}
				else
				{
					utf16ValueStringBuilder.Append<T4>(arg4);
				}
				if (typeof(T5) == typeof(string))
				{
					if (arg5 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T5, string>(ref arg5));
					}
				}
				else if (typeof(T5) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T5, int>(ref arg5));
				}
				else
				{
					utf16ValueStringBuilder.Append<T5>(arg5);
				}
				if (typeof(T6) == typeof(string))
				{
					if (arg6 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T6, string>(ref arg6));
					}
				}
				else if (typeof(T6) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T6, int>(ref arg6));
				}
				else
				{
					utf16ValueStringBuilder.Append<T6>(arg6);
				}
				if (typeof(T7) == typeof(string))
				{
					if (arg7 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T7, string>(ref arg7));
					}
				}
				else if (typeof(T7) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T7, int>(ref arg7));
				}
				else
				{
					utf16ValueStringBuilder.Append<T7>(arg7);
				}
				if (typeof(T8) == typeof(string))
				{
					if (arg8 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T8, string>(ref arg8));
					}
				}
				else if (typeof(T8) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T8, int>(ref arg8));
				}
				else
				{
					utf16ValueStringBuilder.Append<T8>(arg8);
				}
				if (typeof(T9) == typeof(string))
				{
					if (arg9 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T9, string>(ref arg9));
					}
				}
				else if (typeof(T9) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T9, int>(ref arg9));
				}
				else
				{
					utf16ValueStringBuilder.Append<T9>(arg9);
				}
				if (typeof(T10) == typeof(string))
				{
					if (arg10 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T10, string>(ref arg10));
					}
				}
				else if (typeof(T10) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T10, int>(ref arg10));
				}
				else
				{
					utf16ValueStringBuilder.Append<T10>(arg10);
				}
				if (typeof(T11) == typeof(string))
				{
					if (arg11 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T11, string>(ref arg11));
					}
				}
				else if (typeof(T11) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T11, int>(ref arg11));
				}
				else
				{
					utf16ValueStringBuilder.Append<T11>(arg11);
				}
				if (typeof(T12) == typeof(string))
				{
					if (arg12 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T12, string>(ref arg12));
					}
				}
				else if (typeof(T12) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T12, int>(ref arg12));
				}
				else
				{
					utf16ValueStringBuilder.Append<T12>(arg12);
				}
				if (typeof(T13) == typeof(string))
				{
					if (arg13 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T13, string>(ref arg13));
					}
				}
				else if (typeof(T13) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T13, int>(ref arg13));
				}
				else
				{
					utf16ValueStringBuilder.Append<T13>(arg13);
				}
				if (typeof(T14) == typeof(string))
				{
					if (arg14 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T14, string>(ref arg14));
					}
				}
				else if (typeof(T14) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T14, int>(ref arg14));
				}
				else
				{
					utf16ValueStringBuilder.Append<T14>(arg14);
				}
				if (typeof(T15) == typeof(string))
				{
					if (arg15 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T15, string>(ref arg15));
					}
				}
				else if (typeof(T15) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T15, int>(ref arg15));
				}
				else
				{
					utf16ValueStringBuilder.Append<T15>(arg15);
				}
				if (typeof(T16) == typeof(string))
				{
					if (arg16 != null)
					{
						utf16ValueStringBuilder.Append(*Unsafe.As<T16, string>(ref arg16));
					}
				}
				else if (typeof(T16) == typeof(int))
				{
					utf16ValueStringBuilder.Append(*Unsafe.As<T16, int>(ref arg16));
				}
				else
				{
					utf16ValueStringBuilder.Append<T16>(arg16);
				}
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		[NullableContext(0)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void AppendChars<TBufferWriter>([Nullable(1)] ref TBufferWriter sb, ReadOnlySpan<char> chars) where TBufferWriter : IBufferWriter<byte>
		{
			Span<byte> span = sb.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(chars.Length));
			sb.Advance(ZString.UTF8NoBom.GetBytes(chars, span));
		}

		public static Utf16ValueStringBuilder CreateStringBuilder()
		{
			return new Utf16ValueStringBuilder(false);
		}

		public static Utf8ValueStringBuilder CreateUtf8StringBuilder()
		{
			return new Utf8ValueStringBuilder(false);
		}

		public static Utf16ValueStringBuilder CreateStringBuilder(bool notNested)
		{
			return new Utf16ValueStringBuilder(notNested);
		}

		public static Utf8ValueStringBuilder CreateUtf8StringBuilder(bool notNested)
		{
			return new Utf8ValueStringBuilder(notNested);
		}

		public unsafe static string Join<[Nullable(2)] T>(char separator, params T[] values)
		{
			IntPtr intPtr = stackalloc byte[(UIntPtr)2];
			*intPtr = (short)separator;
			return ZString.JoinInternal<T>(new Span<char>(intPtr, 1), values.AsSpan<T>());
		}

		public unsafe static string Join<[Nullable(2)] T>(char separator, List<T> values)
		{
			IntPtr intPtr = stackalloc byte[(UIntPtr)2];
			*intPtr = (short)separator;
			return ZString.JoinInternal<T>(new Span<char>(intPtr, 1), values);
		}

		public unsafe static string Join<[Nullable(2)] T>(char separator, [Nullable(new byte[]
		{
			0,
			1
		})] ReadOnlySpan<T> values)
		{
			IntPtr intPtr = stackalloc byte[(UIntPtr)2];
			*intPtr = (short)separator;
			return ZString.JoinInternal<T>(new Span<char>(intPtr, 1), values);
		}

		public unsafe static string Join<[Nullable(2)] T>(char separator, IEnumerable<T> values)
		{
			IntPtr intPtr = stackalloc byte[(UIntPtr)2];
			*intPtr = (short)separator;
			return ZString.JoinInternal<T>(new Span<char>(intPtr, 1), values);
		}

		public unsafe static string Join<[Nullable(2)] T>(char separator, ICollection<T> values)
		{
			IntPtr intPtr = stackalloc byte[(UIntPtr)2];
			*intPtr = (short)separator;
			return ZString.JoinInternal<T>(new Span<char>(intPtr, 1), values.AsEnumerable<T>());
		}

		public unsafe static string Join<[Nullable(2)] T>(char separator, IList<T> values)
		{
			IntPtr intPtr = stackalloc byte[(UIntPtr)2];
			*intPtr = (short)separator;
			return ZString.JoinInternal<T>(new Span<char>(intPtr, 1), values);
		}

		public unsafe static string Join<[Nullable(2)] T>(char separator, IReadOnlyList<T> values)
		{
			IntPtr intPtr = stackalloc byte[(UIntPtr)2];
			*intPtr = (short)separator;
			return ZString.JoinInternal<T>(new Span<char>(intPtr, 1), values);
		}

		public unsafe static string Join<[Nullable(2)] T>(char separator, IReadOnlyCollection<T> values)
		{
			IntPtr intPtr = stackalloc byte[(UIntPtr)2];
			*intPtr = (short)separator;
			return ZString.JoinInternal<T>(new Span<char>(intPtr, 1), values.AsEnumerable<T>());
		}

		public static string Join<[Nullable(2)] T>(string separator, params T[] values)
		{
			return ZString.JoinInternal<T>(separator.AsSpan(), values.AsSpan<T>());
		}

		public static string Join<[Nullable(2)] T>(string separator, List<T> values)
		{
			return ZString.JoinInternal<T>(separator.AsSpan(), values);
		}

		public static string Join<[Nullable(2)] T>(string separator, [Nullable(new byte[]
		{
			0,
			1
		})] ReadOnlySpan<T> values)
		{
			return ZString.JoinInternal<T>(separator.AsSpan(), values);
		}

		public static string Join<[Nullable(2)] T>(string separator, ICollection<T> values)
		{
			return ZString.JoinInternal<T>(separator.AsSpan(), values.AsEnumerable<T>());
		}

		public static string Join<[Nullable(2)] T>(string separator, IList<T> values)
		{
			return ZString.JoinInternal<T>(separator.AsSpan(), values);
		}

		public static string Join<[Nullable(2)] T>(string separator, IReadOnlyList<T> values)
		{
			return ZString.JoinInternal<T>(separator.AsSpan(), values);
		}

		public static string Join<[Nullable(2)] T>(string separator, IReadOnlyCollection<T> values)
		{
			return ZString.JoinInternal<T>(separator.AsSpan(), values.AsEnumerable<T>());
		}

		public static string Join<[Nullable(2)] T>(string separator, IEnumerable<T> values)
		{
			return ZString.JoinInternal<T>(separator.AsSpan(), values);
		}

		public unsafe static string Join(char separator, [Nullable(new byte[]
		{
			0,
			1
		})] ReadOnlySpan<string> values)
		{
			IntPtr intPtr = stackalloc byte[(UIntPtr)2];
			*intPtr = (short)separator;
			return ZString.JoinInternal(new Span<char>(intPtr, 1), values);
		}

		public unsafe static string Join(char separator, params string[] values)
		{
			IntPtr intPtr = stackalloc byte[(UIntPtr)2];
			*intPtr = (short)separator;
			return ZString.JoinInternal(new Span<char>(intPtr, 1), values.AsSpan<string>());
		}

		public static string Join(string separator, params string[] values)
		{
			return ZString.JoinInternal(separator.AsSpan(), values.AsSpan<string>());
		}

		public static string Concat<[Nullable(2)] T>(params T[] values)
		{
			return ZString.JoinInternal<T>(default(ReadOnlySpan<char>), values.AsSpan<T>());
		}

		public static string Concat<[Nullable(2)] T>(List<T> values)
		{
			return ZString.JoinInternal<T>(default(ReadOnlySpan<char>), values);
		}

		public static string Concat<[Nullable(2)] T>([Nullable(new byte[]
		{
			0,
			1
		})] ReadOnlySpan<T> values)
		{
			return ZString.JoinInternal<T>(default(ReadOnlySpan<char>), values);
		}

		public static string Concat<[Nullable(2)] T>(ICollection<T> values)
		{
			return ZString.JoinInternal<T>(default(ReadOnlySpan<char>), values.AsEnumerable<T>());
		}

		public static string Concat<[Nullable(2)] T>(IList<T> values)
		{
			return ZString.JoinInternal<T>(default(ReadOnlySpan<char>), values);
		}

		public static string Concat<[Nullable(2)] T>(IReadOnlyList<T> values)
		{
			return ZString.JoinInternal<T>(default(ReadOnlySpan<char>), values);
		}

		public static string Concat<[Nullable(2)] T>(IReadOnlyCollection<T> values)
		{
			return ZString.JoinInternal<T>(default(ReadOnlySpan<char>), values.AsEnumerable<T>());
		}

		public static string Concat<[Nullable(2)] T>(IEnumerable<T> values)
		{
			return ZString.JoinInternal<T>(default(ReadOnlySpan<char>), values);
		}

		private static string JoinInternal<[Nullable(2)] T>([Nullable(0)] ReadOnlySpan<char> separator, IList<T> values)
		{
			IReadOnlyList<T> readOnlyList = values as IReadOnlyList<T>;
			readOnlyList = (readOnlyList ?? new ReadOnlyListAdaptor<T>(values));
			return ZString.JoinInternal<T>(separator, readOnlyList);
		}

		private static string JoinInternal<[Nullable(2)] T>([Nullable(0)] ReadOnlySpan<char> separator, IReadOnlyList<T> values)
		{
			if (values.Count == 0)
			{
				return string.Empty;
			}
			string[] array = values as string[];
			if (array != null)
			{
				return ZString.JoinInternal(separator, array.AsSpan<string>());
			}
			string result;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true))
			{
				utf16ValueStringBuilder.AppendJoinInternal<T>(separator, values);
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		[NullableContext(0)]
		[return: Nullable(1)]
		private unsafe static string JoinInternal<[Nullable(2)] T>(ReadOnlySpan<char> separator, [Nullable(new byte[]
		{
			0,
			1
		})] ReadOnlySpan<T> values)
		{
			if (values.Length == 0)
			{
				return string.Empty;
			}
			if (typeof(T) == typeof(string) && values.Length == 1)
			{
				return Unsafe.As<string>(*values[0]);
			}
			string result;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true))
			{
				utf16ValueStringBuilder.AppendJoinInternal<T>(separator, values);
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		private static string JoinInternal<[Nullable(2)] T>([Nullable(0)] ReadOnlySpan<char> separator, IEnumerable<T> values)
		{
			string result;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true))
			{
				utf16ValueStringBuilder.AppendJoinInternal<T>(separator, values);
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		[NullableContext(0)]
		[return: Nullable(1)]
		private unsafe static string JoinInternal(ReadOnlySpan<char> separator, [Nullable(new byte[]
		{
			0,
			1
		})] ReadOnlySpan<string> values)
		{
			if (values.Length == 0)
			{
				return string.Empty;
			}
			if (values.Length == 1)
			{
				return *values[0];
			}
			int num = (values.Length - 1) * separator.Length;
			for (int i = 0; i < values.Length; i++)
			{
				string text = *values[i];
				if (text != null)
				{
					num += text.Length;
				}
			}
			if (num == 0)
			{
				return string.Empty;
			}
			string text2 = string.Create<int>(num, 0, delegate(Span<char> _, int _)
			{
			});
			Span<char> span = MemoryMarshal.CreateSpan<char>(MemoryMarshal.GetReference<char>(text2.AsSpan()), text2.Length);
			int num2 = 0;
			for (int j = 0; j < values.Length; j++)
			{
				string text3 = *values[j];
				if (text3 != null)
				{
					text3.AsSpan().CopyTo(span.Slice(num2));
					num2 += text3.Length;
				}
				if (j < values.Length - 1)
				{
					if (separator.Length == 1)
					{
						*span[num2++] = (char)(*separator[0]);
					}
					else
					{
						separator.CopyTo(span.Slice(num2));
						num2 += separator.Length;
					}
				}
			}
			return text2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Format<[Nullable(2)] T1>(string format, T1 arg1)
		{
			string result;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true))
			{
				utf16ValueStringBuilder.AppendFormat<T1>(format, arg1);
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Format<[Nullable(2)] T1>([Nullable(0)] ReadOnlySpan<char> format, T1 arg1)
		{
			string result;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true))
			{
				utf16ValueStringBuilder.AppendFormat<T1>(format, arg1);
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Format<[Nullable(2)] T1, [Nullable(2)] T2>(string format, T1 arg1, T2 arg2)
		{
			string result;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true))
			{
				utf16ValueStringBuilder.AppendFormat<T1, T2>(format, arg1, arg2);
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Format<[Nullable(2)] T1, [Nullable(2)] T2>([Nullable(0)] ReadOnlySpan<char> format, T1 arg1, T2 arg2)
		{
			string result;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true))
			{
				utf16ValueStringBuilder.AppendFormat<T1, T2>(format, arg1, arg2);
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Format<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3>(string format, T1 arg1, T2 arg2, T3 arg3)
		{
			string result;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true))
			{
				utf16ValueStringBuilder.AppendFormat<T1, T2, T3>(format, arg1, arg2, arg3);
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Format<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3>([Nullable(0)] ReadOnlySpan<char> format, T1 arg1, T2 arg2, T3 arg3)
		{
			string result;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true))
			{
				utf16ValueStringBuilder.AppendFormat<T1, T2, T3>(format, arg1, arg2, arg3);
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Format<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			string result;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true))
			{
				utf16ValueStringBuilder.AppendFormat<T1, T2, T3, T4>(format, arg1, arg2, arg3, arg4);
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Format<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4>([Nullable(0)] ReadOnlySpan<char> format, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			string result;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true))
			{
				utf16ValueStringBuilder.AppendFormat<T1, T2, T3, T4>(format, arg1, arg2, arg3, arg4);
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Format<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
		{
			string result;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true))
			{
				utf16ValueStringBuilder.AppendFormat<T1, T2, T3, T4, T5>(format, arg1, arg2, arg3, arg4, arg5);
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Format<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5>([Nullable(0)] ReadOnlySpan<char> format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
		{
			string result;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true))
			{
				utf16ValueStringBuilder.AppendFormat<T1, T2, T3, T4, T5>(format, arg1, arg2, arg3, arg4, arg5);
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Format<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
		{
			string result;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true))
			{
				utf16ValueStringBuilder.AppendFormat<T1, T2, T3, T4, T5, T6>(format, arg1, arg2, arg3, arg4, arg5, arg6);
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Format<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6>([Nullable(0)] ReadOnlySpan<char> format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
		{
			string result;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true))
			{
				utf16ValueStringBuilder.AppendFormat<T1, T2, T3, T4, T5, T6>(format, arg1, arg2, arg3, arg4, arg5, arg6);
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Format<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
		{
			string result;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true))
			{
				utf16ValueStringBuilder.AppendFormat<T1, T2, T3, T4, T5, T6, T7>(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Format<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7>([Nullable(0)] ReadOnlySpan<char> format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
		{
			string result;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true))
			{
				utf16ValueStringBuilder.AppendFormat<T1, T2, T3, T4, T5, T6, T7>(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Format<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
		{
			string result;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true))
			{
				utf16ValueStringBuilder.AppendFormat<T1, T2, T3, T4, T5, T6, T7, T8>(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Format<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8>([Nullable(0)] ReadOnlySpan<char> format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
		{
			string result;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true))
			{
				utf16ValueStringBuilder.AppendFormat<T1, T2, T3, T4, T5, T6, T7, T8>(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Format<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8, [Nullable(2)] T9>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
		{
			string result;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true))
			{
				utf16ValueStringBuilder.AppendFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9>(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Format<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8, [Nullable(2)] T9>([Nullable(0)] ReadOnlySpan<char> format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
		{
			string result;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true))
			{
				utf16ValueStringBuilder.AppendFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9>(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Format<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8, [Nullable(2)] T9, [Nullable(2)] T10>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
		{
			string result;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true))
			{
				utf16ValueStringBuilder.AppendFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Format<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8, [Nullable(2)] T9, [Nullable(2)] T10>([Nullable(0)] ReadOnlySpan<char> format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
		{
			string result;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true))
			{
				utf16ValueStringBuilder.AppendFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Format<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8, [Nullable(2)] T9, [Nullable(2)] T10, [Nullable(2)] T11>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
		{
			string result;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true))
			{
				utf16ValueStringBuilder.AppendFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Format<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8, [Nullable(2)] T9, [Nullable(2)] T10, [Nullable(2)] T11>([Nullable(0)] ReadOnlySpan<char> format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
		{
			string result;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true))
			{
				utf16ValueStringBuilder.AppendFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Format<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8, [Nullable(2)] T9, [Nullable(2)] T10, [Nullable(2)] T11, [Nullable(2)] T12>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
		{
			string result;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true))
			{
				utf16ValueStringBuilder.AppendFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Format<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8, [Nullable(2)] T9, [Nullable(2)] T10, [Nullable(2)] T11, [Nullable(2)] T12>([Nullable(0)] ReadOnlySpan<char> format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
		{
			string result;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true))
			{
				utf16ValueStringBuilder.AppendFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Format<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8, [Nullable(2)] T9, [Nullable(2)] T10, [Nullable(2)] T11, [Nullable(2)] T12, [Nullable(2)] T13>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
		{
			string result;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true))
			{
				utf16ValueStringBuilder.AppendFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Format<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8, [Nullable(2)] T9, [Nullable(2)] T10, [Nullable(2)] T11, [Nullable(2)] T12, [Nullable(2)] T13>([Nullable(0)] ReadOnlySpan<char> format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
		{
			string result;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true))
			{
				utf16ValueStringBuilder.AppendFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Format<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8, [Nullable(2)] T9, [Nullable(2)] T10, [Nullable(2)] T11, [Nullable(2)] T12, [Nullable(2)] T13, [Nullable(2)] T14>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
		{
			string result;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true))
			{
				utf16ValueStringBuilder.AppendFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Format<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8, [Nullable(2)] T9, [Nullable(2)] T10, [Nullable(2)] T11, [Nullable(2)] T12, [Nullable(2)] T13, [Nullable(2)] T14>([Nullable(0)] ReadOnlySpan<char> format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
		{
			string result;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true))
			{
				utf16ValueStringBuilder.AppendFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Format<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8, [Nullable(2)] T9, [Nullable(2)] T10, [Nullable(2)] T11, [Nullable(2)] T12, [Nullable(2)] T13, [Nullable(2)] T14, [Nullable(2)] T15>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
		{
			string result;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true))
			{
				utf16ValueStringBuilder.AppendFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Format<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8, [Nullable(2)] T9, [Nullable(2)] T10, [Nullable(2)] T11, [Nullable(2)] T12, [Nullable(2)] T13, [Nullable(2)] T14, [Nullable(2)] T15>([Nullable(0)] ReadOnlySpan<char> format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
		{
			string result;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true))
			{
				utf16ValueStringBuilder.AppendFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Format<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8, [Nullable(2)] T9, [Nullable(2)] T10, [Nullable(2)] T11, [Nullable(2)] T12, [Nullable(2)] T13, [Nullable(2)] T14, [Nullable(2)] T15, [Nullable(2)] T16>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16)
		{
			string result;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true))
			{
				utf16ValueStringBuilder.AppendFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Format<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8, [Nullable(2)] T9, [Nullable(2)] T10, [Nullable(2)] T11, [Nullable(2)] T12, [Nullable(2)] T13, [Nullable(2)] T14, [Nullable(2)] T15, [Nullable(2)] T16>([Nullable(0)] ReadOnlySpan<char> format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16)
		{
			string result;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true))
			{
				utf16ValueStringBuilder.AppendFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		public static Utf16PreparedFormat<T1> PrepareUtf16<[Nullable(2)] T1>(string format)
		{
			return new Utf16PreparedFormat<T1>(format);
		}

		public static Utf8PreparedFormat<T1> PrepareUtf8<[Nullable(2)] T1>(string format)
		{
			return new Utf8PreparedFormat<T1>(format);
		}

		public static Utf16PreparedFormat<T1, T2> PrepareUtf16<[Nullable(2)] T1, [Nullable(2)] T2>(string format)
		{
			return new Utf16PreparedFormat<T1, T2>(format);
		}

		public static Utf8PreparedFormat<T1, T2> PrepareUtf8<[Nullable(2)] T1, [Nullable(2)] T2>(string format)
		{
			return new Utf8PreparedFormat<T1, T2>(format);
		}

		[NullableContext(2)]
		[return: Nullable(1)]
		public static Utf16PreparedFormat<T1, T2, T3> PrepareUtf16<T1, T2, T3>([Nullable(1)] string format)
		{
			return new Utf16PreparedFormat<T1, T2, T3>(format);
		}

		[NullableContext(2)]
		[return: Nullable(1)]
		public static Utf8PreparedFormat<T1, T2, T3> PrepareUtf8<T1, T2, T3>([Nullable(1)] string format)
		{
			return new Utf8PreparedFormat<T1, T2, T3>(format);
		}

		[NullableContext(2)]
		[return: Nullable(1)]
		public static Utf16PreparedFormat<T1, T2, T3, T4> PrepareUtf16<T1, T2, T3, T4>([Nullable(1)] string format)
		{
			return new Utf16PreparedFormat<T1, T2, T3, T4>(format);
		}

		[NullableContext(2)]
		[return: Nullable(1)]
		public static Utf8PreparedFormat<T1, T2, T3, T4> PrepareUtf8<T1, T2, T3, T4>([Nullable(1)] string format)
		{
			return new Utf8PreparedFormat<T1, T2, T3, T4>(format);
		}

		[NullableContext(2)]
		[return: Nullable(1)]
		public static Utf16PreparedFormat<T1, T2, T3, T4, T5> PrepareUtf16<T1, T2, T3, T4, T5>([Nullable(1)] string format)
		{
			return new Utf16PreparedFormat<T1, T2, T3, T4, T5>(format);
		}

		[NullableContext(2)]
		[return: Nullable(1)]
		public static Utf8PreparedFormat<T1, T2, T3, T4, T5> PrepareUtf8<T1, T2, T3, T4, T5>([Nullable(1)] string format)
		{
			return new Utf8PreparedFormat<T1, T2, T3, T4, T5>(format);
		}

		[NullableContext(2)]
		[return: Nullable(1)]
		public static Utf16PreparedFormat<T1, T2, T3, T4, T5, T6> PrepareUtf16<T1, T2, T3, T4, T5, T6>([Nullable(1)] string format)
		{
			return new Utf16PreparedFormat<T1, T2, T3, T4, T5, T6>(format);
		}

		[NullableContext(2)]
		[return: Nullable(1)]
		public static Utf8PreparedFormat<T1, T2, T3, T4, T5, T6> PrepareUtf8<T1, T2, T3, T4, T5, T6>([Nullable(1)] string format)
		{
			return new Utf8PreparedFormat<T1, T2, T3, T4, T5, T6>(format);
		}

		[NullableContext(2)]
		[return: Nullable(1)]
		public static Utf16PreparedFormat<T1, T2, T3, T4, T5, T6, T7> PrepareUtf16<T1, T2, T3, T4, T5, T6, T7>([Nullable(1)] string format)
		{
			return new Utf16PreparedFormat<T1, T2, T3, T4, T5, T6, T7>(format);
		}

		[NullableContext(2)]
		[return: Nullable(1)]
		public static Utf8PreparedFormat<T1, T2, T3, T4, T5, T6, T7> PrepareUtf8<T1, T2, T3, T4, T5, T6, T7>([Nullable(1)] string format)
		{
			return new Utf8PreparedFormat<T1, T2, T3, T4, T5, T6, T7>(format);
		}

		[NullableContext(2)]
		[return: Nullable(1)]
		public static Utf16PreparedFormat<T1, T2, T3, T4, T5, T6, T7, T8> PrepareUtf16<T1, T2, T3, T4, T5, T6, T7, T8>([Nullable(1)] string format)
		{
			return new Utf16PreparedFormat<T1, T2, T3, T4, T5, T6, T7, T8>(format);
		}

		[NullableContext(2)]
		[return: Nullable(1)]
		public static Utf8PreparedFormat<T1, T2, T3, T4, T5, T6, T7, T8> PrepareUtf8<T1, T2, T3, T4, T5, T6, T7, T8>([Nullable(1)] string format)
		{
			return new Utf8PreparedFormat<T1, T2, T3, T4, T5, T6, T7, T8>(format);
		}

		[NullableContext(2)]
		[return: Nullable(1)]
		public static Utf16PreparedFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9> PrepareUtf16<T1, T2, T3, T4, T5, T6, T7, T8, T9>([Nullable(1)] string format)
		{
			return new Utf16PreparedFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9>(format);
		}

		[NullableContext(2)]
		[return: Nullable(1)]
		public static Utf8PreparedFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9> PrepareUtf8<T1, T2, T3, T4, T5, T6, T7, T8, T9>([Nullable(1)] string format)
		{
			return new Utf8PreparedFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9>(format);
		}

		[NullableContext(2)]
		[return: Nullable(1)]
		public static Utf16PreparedFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> PrepareUtf16<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>([Nullable(1)] string format)
		{
			return new Utf16PreparedFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(format);
		}

		[NullableContext(2)]
		[return: Nullable(1)]
		public static Utf8PreparedFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> PrepareUtf8<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>([Nullable(1)] string format)
		{
			return new Utf8PreparedFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(format);
		}

		[NullableContext(2)]
		[return: Nullable(1)]
		public static Utf16PreparedFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> PrepareUtf16<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>([Nullable(1)] string format)
		{
			return new Utf16PreparedFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(format);
		}

		[NullableContext(2)]
		[return: Nullable(1)]
		public static Utf8PreparedFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> PrepareUtf8<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>([Nullable(1)] string format)
		{
			return new Utf8PreparedFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(format);
		}

		[NullableContext(2)]
		[return: Nullable(1)]
		public static Utf16PreparedFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> PrepareUtf16<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>([Nullable(1)] string format)
		{
			return new Utf16PreparedFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(format);
		}

		[NullableContext(2)]
		[return: Nullable(1)]
		public static Utf8PreparedFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> PrepareUtf8<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>([Nullable(1)] string format)
		{
			return new Utf8PreparedFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(format);
		}

		[NullableContext(2)]
		[return: Nullable(1)]
		public static Utf16PreparedFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> PrepareUtf16<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>([Nullable(1)] string format)
		{
			return new Utf16PreparedFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(format);
		}

		[NullableContext(2)]
		[return: Nullable(1)]
		public static Utf8PreparedFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> PrepareUtf8<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>([Nullable(1)] string format)
		{
			return new Utf8PreparedFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(format);
		}

		[NullableContext(2)]
		[return: Nullable(1)]
		public static Utf16PreparedFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> PrepareUtf16<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>([Nullable(1)] string format)
		{
			return new Utf16PreparedFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(format);
		}

		[NullableContext(2)]
		[return: Nullable(1)]
		public static Utf8PreparedFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> PrepareUtf8<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>([Nullable(1)] string format)
		{
			return new Utf8PreparedFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(format);
		}

		[NullableContext(2)]
		[return: Nullable(1)]
		public static Utf16PreparedFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> PrepareUtf16<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>([Nullable(1)] string format)
		{
			return new Utf16PreparedFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(format);
		}

		[NullableContext(2)]
		[return: Nullable(1)]
		public static Utf8PreparedFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> PrepareUtf8<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>([Nullable(1)] string format)
		{
			return new Utf8PreparedFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(format);
		}

		[NullableContext(2)]
		[return: Nullable(1)]
		public static Utf16PreparedFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> PrepareUtf16<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>([Nullable(1)] string format)
		{
			return new Utf16PreparedFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(format);
		}

		[NullableContext(2)]
		[return: Nullable(1)]
		public static Utf8PreparedFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> PrepareUtf8<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>([Nullable(1)] string format)
		{
			return new Utf8PreparedFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(format);
		}

		public static void Utf8Format<[Nullable(2)] T1>(IBufferWriter<byte> bufferWriter, string format, T1 arg1)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			int num = 0;
			for (int i = 0; i < format.Length; i++)
			{
				char c = format[i];
				if (c == '{')
				{
					if (i == format.Length - 1)
					{
						throw new FormatException("invalid format");
					}
					if (i != format.Length && format[i + 1] == '{')
					{
						int num2 = i - num;
						Span<byte> span = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num2));
						int bytes = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num2), span);
						bufferWriter.Advance(bytes);
						i++;
						num = i;
					}
					else
					{
						int num3 = i - num;
						Span<byte> span2 = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num3));
						int bytes2 = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num3), span2);
						bufferWriter.Advance(bytes2);
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						StandardFormat format2 = StandardFormat.Parse(parseResult.FormatString);
						if (parseResult.Index == 0)
						{
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T1>(ref bufferWriter, arg1, parseResult.Alignment, format2, "arg1");
						}
						else
						{
							ExceptionUtil.ThrowFormatException();
							ExceptionUtil.ThrowFormatException();
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && format[i + 1] == '}')
					{
						int num4 = i - num;
						Span<byte> span3 = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num4));
						int bytes3 = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num4), span3);
						bufferWriter.Advance(bytes3);
						i++;
						num = i;
					}
					else
					{
						ExceptionUtil.ThrowFormatException();
					}
				}
			}
			int num5 = format.Length - num;
			if (num5 > 0)
			{
				Span<byte> span4 = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num5));
				int bytes4 = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num5), span4);
				bufferWriter.Advance(bytes4);
			}
		}

		public static void Utf8Format<[Nullable(2)] T1, [Nullable(2)] T2>(IBufferWriter<byte> bufferWriter, string format, T1 arg1, T2 arg2)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			int num = 0;
			for (int i = 0; i < format.Length; i++)
			{
				char c = format[i];
				if (c == '{')
				{
					if (i == format.Length - 1)
					{
						throw new FormatException("invalid format");
					}
					if (i != format.Length && format[i + 1] == '{')
					{
						int num2 = i - num;
						Span<byte> span = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num2));
						int bytes = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num2), span);
						bufferWriter.Advance(bytes);
						i++;
						num = i;
					}
					else
					{
						int num3 = i - num;
						Span<byte> span2 = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num3));
						int bytes2 = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num3), span2);
						bufferWriter.Advance(bytes2);
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						StandardFormat format2 = StandardFormat.Parse(parseResult.FormatString);
						int index = parseResult.Index;
						if (index != 0)
						{
							if (index != 1)
							{
								ExceptionUtil.ThrowFormatException();
								ExceptionUtil.ThrowFormatException();
							}
							else
							{
								Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T2>(ref bufferWriter, arg2, parseResult.Alignment, format2, "arg2");
							}
						}
						else
						{
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T1>(ref bufferWriter, arg1, parseResult.Alignment, format2, "arg1");
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && format[i + 1] == '}')
					{
						int num4 = i - num;
						Span<byte> span3 = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num4));
						int bytes3 = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num4), span3);
						bufferWriter.Advance(bytes3);
						i++;
						num = i;
					}
					else
					{
						ExceptionUtil.ThrowFormatException();
					}
				}
			}
			int num5 = format.Length - num;
			if (num5 > 0)
			{
				Span<byte> span4 = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num5));
				int bytes4 = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num5), span4);
				bufferWriter.Advance(bytes4);
			}
		}

		public static void Utf8Format<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3>(IBufferWriter<byte> bufferWriter, string format, T1 arg1, T2 arg2, T3 arg3)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			int num = 0;
			for (int i = 0; i < format.Length; i++)
			{
				char c = format[i];
				if (c == '{')
				{
					if (i == format.Length - 1)
					{
						throw new FormatException("invalid format");
					}
					if (i != format.Length && format[i + 1] == '{')
					{
						int num2 = i - num;
						Span<byte> span = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num2));
						int bytes = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num2), span);
						bufferWriter.Advance(bytes);
						i++;
						num = i;
					}
					else
					{
						int num3 = i - num;
						Span<byte> span2 = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num3));
						int bytes2 = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num3), span2);
						bufferWriter.Advance(bytes2);
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						StandardFormat format2 = StandardFormat.Parse(parseResult.FormatString);
						switch (parseResult.Index)
						{
						case 0:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T1>(ref bufferWriter, arg1, parseResult.Alignment, format2, "arg1");
							break;
						case 1:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T2>(ref bufferWriter, arg2, parseResult.Alignment, format2, "arg2");
							break;
						case 2:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T3>(ref bufferWriter, arg3, parseResult.Alignment, format2, "arg3");
							break;
						default:
							ExceptionUtil.ThrowFormatException();
							ExceptionUtil.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && format[i + 1] == '}')
					{
						int num4 = i - num;
						Span<byte> span3 = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num4));
						int bytes3 = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num4), span3);
						bufferWriter.Advance(bytes3);
						i++;
						num = i;
					}
					else
					{
						ExceptionUtil.ThrowFormatException();
					}
				}
			}
			int num5 = format.Length - num;
			if (num5 > 0)
			{
				Span<byte> span4 = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num5));
				int bytes4 = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num5), span4);
				bufferWriter.Advance(bytes4);
			}
		}

		public static void Utf8Format<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4>(IBufferWriter<byte> bufferWriter, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			int num = 0;
			for (int i = 0; i < format.Length; i++)
			{
				char c = format[i];
				if (c == '{')
				{
					if (i == format.Length - 1)
					{
						throw new FormatException("invalid format");
					}
					if (i != format.Length && format[i + 1] == '{')
					{
						int num2 = i - num;
						Span<byte> span = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num2));
						int bytes = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num2), span);
						bufferWriter.Advance(bytes);
						i++;
						num = i;
					}
					else
					{
						int num3 = i - num;
						Span<byte> span2 = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num3));
						int bytes2 = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num3), span2);
						bufferWriter.Advance(bytes2);
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						StandardFormat format2 = StandardFormat.Parse(parseResult.FormatString);
						switch (parseResult.Index)
						{
						case 0:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T1>(ref bufferWriter, arg1, parseResult.Alignment, format2, "arg1");
							break;
						case 1:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T2>(ref bufferWriter, arg2, parseResult.Alignment, format2, "arg2");
							break;
						case 2:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T3>(ref bufferWriter, arg3, parseResult.Alignment, format2, "arg3");
							break;
						case 3:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T4>(ref bufferWriter, arg4, parseResult.Alignment, format2, "arg4");
							break;
						default:
							ExceptionUtil.ThrowFormatException();
							ExceptionUtil.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && format[i + 1] == '}')
					{
						int num4 = i - num;
						Span<byte> span3 = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num4));
						int bytes3 = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num4), span3);
						bufferWriter.Advance(bytes3);
						i++;
						num = i;
					}
					else
					{
						ExceptionUtil.ThrowFormatException();
					}
				}
			}
			int num5 = format.Length - num;
			if (num5 > 0)
			{
				Span<byte> span4 = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num5));
				int bytes4 = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num5), span4);
				bufferWriter.Advance(bytes4);
			}
		}

		public static void Utf8Format<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5>(IBufferWriter<byte> bufferWriter, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			int num = 0;
			for (int i = 0; i < format.Length; i++)
			{
				char c = format[i];
				if (c == '{')
				{
					if (i == format.Length - 1)
					{
						throw new FormatException("invalid format");
					}
					if (i != format.Length && format[i + 1] == '{')
					{
						int num2 = i - num;
						Span<byte> span = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num2));
						int bytes = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num2), span);
						bufferWriter.Advance(bytes);
						i++;
						num = i;
					}
					else
					{
						int num3 = i - num;
						Span<byte> span2 = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num3));
						int bytes2 = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num3), span2);
						bufferWriter.Advance(bytes2);
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						StandardFormat format2 = StandardFormat.Parse(parseResult.FormatString);
						switch (parseResult.Index)
						{
						case 0:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T1>(ref bufferWriter, arg1, parseResult.Alignment, format2, "arg1");
							break;
						case 1:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T2>(ref bufferWriter, arg2, parseResult.Alignment, format2, "arg2");
							break;
						case 2:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T3>(ref bufferWriter, arg3, parseResult.Alignment, format2, "arg3");
							break;
						case 3:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T4>(ref bufferWriter, arg4, parseResult.Alignment, format2, "arg4");
							break;
						case 4:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T5>(ref bufferWriter, arg5, parseResult.Alignment, format2, "arg5");
							break;
						default:
							ExceptionUtil.ThrowFormatException();
							ExceptionUtil.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && format[i + 1] == '}')
					{
						int num4 = i - num;
						Span<byte> span3 = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num4));
						int bytes3 = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num4), span3);
						bufferWriter.Advance(bytes3);
						i++;
						num = i;
					}
					else
					{
						ExceptionUtil.ThrowFormatException();
					}
				}
			}
			int num5 = format.Length - num;
			if (num5 > 0)
			{
				Span<byte> span4 = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num5));
				int bytes4 = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num5), span4);
				bufferWriter.Advance(bytes4);
			}
		}

		public static void Utf8Format<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6>(IBufferWriter<byte> bufferWriter, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			int num = 0;
			for (int i = 0; i < format.Length; i++)
			{
				char c = format[i];
				if (c == '{')
				{
					if (i == format.Length - 1)
					{
						throw new FormatException("invalid format");
					}
					if (i != format.Length && format[i + 1] == '{')
					{
						int num2 = i - num;
						Span<byte> span = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num2));
						int bytes = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num2), span);
						bufferWriter.Advance(bytes);
						i++;
						num = i;
					}
					else
					{
						int num3 = i - num;
						Span<byte> span2 = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num3));
						int bytes2 = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num3), span2);
						bufferWriter.Advance(bytes2);
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						StandardFormat format2 = StandardFormat.Parse(parseResult.FormatString);
						switch (parseResult.Index)
						{
						case 0:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T1>(ref bufferWriter, arg1, parseResult.Alignment, format2, "arg1");
							break;
						case 1:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T2>(ref bufferWriter, arg2, parseResult.Alignment, format2, "arg2");
							break;
						case 2:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T3>(ref bufferWriter, arg3, parseResult.Alignment, format2, "arg3");
							break;
						case 3:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T4>(ref bufferWriter, arg4, parseResult.Alignment, format2, "arg4");
							break;
						case 4:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T5>(ref bufferWriter, arg5, parseResult.Alignment, format2, "arg5");
							break;
						case 5:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T6>(ref bufferWriter, arg6, parseResult.Alignment, format2, "arg6");
							break;
						default:
							ExceptionUtil.ThrowFormatException();
							ExceptionUtil.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && format[i + 1] == '}')
					{
						int num4 = i - num;
						Span<byte> span3 = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num4));
						int bytes3 = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num4), span3);
						bufferWriter.Advance(bytes3);
						i++;
						num = i;
					}
					else
					{
						ExceptionUtil.ThrowFormatException();
					}
				}
			}
			int num5 = format.Length - num;
			if (num5 > 0)
			{
				Span<byte> span4 = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num5));
				int bytes4 = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num5), span4);
				bufferWriter.Advance(bytes4);
			}
		}

		public static void Utf8Format<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7>(IBufferWriter<byte> bufferWriter, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			int num = 0;
			for (int i = 0; i < format.Length; i++)
			{
				char c = format[i];
				if (c == '{')
				{
					if (i == format.Length - 1)
					{
						throw new FormatException("invalid format");
					}
					if (i != format.Length && format[i + 1] == '{')
					{
						int num2 = i - num;
						Span<byte> span = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num2));
						int bytes = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num2), span);
						bufferWriter.Advance(bytes);
						i++;
						num = i;
					}
					else
					{
						int num3 = i - num;
						Span<byte> span2 = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num3));
						int bytes2 = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num3), span2);
						bufferWriter.Advance(bytes2);
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						StandardFormat format2 = StandardFormat.Parse(parseResult.FormatString);
						switch (parseResult.Index)
						{
						case 0:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T1>(ref bufferWriter, arg1, parseResult.Alignment, format2, "arg1");
							break;
						case 1:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T2>(ref bufferWriter, arg2, parseResult.Alignment, format2, "arg2");
							break;
						case 2:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T3>(ref bufferWriter, arg3, parseResult.Alignment, format2, "arg3");
							break;
						case 3:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T4>(ref bufferWriter, arg4, parseResult.Alignment, format2, "arg4");
							break;
						case 4:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T5>(ref bufferWriter, arg5, parseResult.Alignment, format2, "arg5");
							break;
						case 5:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T6>(ref bufferWriter, arg6, parseResult.Alignment, format2, "arg6");
							break;
						case 6:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T7>(ref bufferWriter, arg7, parseResult.Alignment, format2, "arg7");
							break;
						default:
							ExceptionUtil.ThrowFormatException();
							ExceptionUtil.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && format[i + 1] == '}')
					{
						int num4 = i - num;
						Span<byte> span3 = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num4));
						int bytes3 = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num4), span3);
						bufferWriter.Advance(bytes3);
						i++;
						num = i;
					}
					else
					{
						ExceptionUtil.ThrowFormatException();
					}
				}
			}
			int num5 = format.Length - num;
			if (num5 > 0)
			{
				Span<byte> span4 = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num5));
				int bytes4 = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num5), span4);
				bufferWriter.Advance(bytes4);
			}
		}

		public static void Utf8Format<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8>(IBufferWriter<byte> bufferWriter, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			int num = 0;
			for (int i = 0; i < format.Length; i++)
			{
				char c = format[i];
				if (c == '{')
				{
					if (i == format.Length - 1)
					{
						throw new FormatException("invalid format");
					}
					if (i != format.Length && format[i + 1] == '{')
					{
						int num2 = i - num;
						Span<byte> span = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num2));
						int bytes = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num2), span);
						bufferWriter.Advance(bytes);
						i++;
						num = i;
					}
					else
					{
						int num3 = i - num;
						Span<byte> span2 = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num3));
						int bytes2 = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num3), span2);
						bufferWriter.Advance(bytes2);
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						StandardFormat format2 = StandardFormat.Parse(parseResult.FormatString);
						switch (parseResult.Index)
						{
						case 0:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T1>(ref bufferWriter, arg1, parseResult.Alignment, format2, "arg1");
							break;
						case 1:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T2>(ref bufferWriter, arg2, parseResult.Alignment, format2, "arg2");
							break;
						case 2:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T3>(ref bufferWriter, arg3, parseResult.Alignment, format2, "arg3");
							break;
						case 3:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T4>(ref bufferWriter, arg4, parseResult.Alignment, format2, "arg4");
							break;
						case 4:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T5>(ref bufferWriter, arg5, parseResult.Alignment, format2, "arg5");
							break;
						case 5:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T6>(ref bufferWriter, arg6, parseResult.Alignment, format2, "arg6");
							break;
						case 6:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T7>(ref bufferWriter, arg7, parseResult.Alignment, format2, "arg7");
							break;
						case 7:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T8>(ref bufferWriter, arg8, parseResult.Alignment, format2, "arg8");
							break;
						default:
							ExceptionUtil.ThrowFormatException();
							ExceptionUtil.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && format[i + 1] == '}')
					{
						int num4 = i - num;
						Span<byte> span3 = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num4));
						int bytes3 = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num4), span3);
						bufferWriter.Advance(bytes3);
						i++;
						num = i;
					}
					else
					{
						ExceptionUtil.ThrowFormatException();
					}
				}
			}
			int num5 = format.Length - num;
			if (num5 > 0)
			{
				Span<byte> span4 = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num5));
				int bytes4 = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num5), span4);
				bufferWriter.Advance(bytes4);
			}
		}

		public static void Utf8Format<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8, [Nullable(2)] T9>(IBufferWriter<byte> bufferWriter, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			int num = 0;
			for (int i = 0; i < format.Length; i++)
			{
				char c = format[i];
				if (c == '{')
				{
					if (i == format.Length - 1)
					{
						throw new FormatException("invalid format");
					}
					if (i != format.Length && format[i + 1] == '{')
					{
						int num2 = i - num;
						Span<byte> span = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num2));
						int bytes = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num2), span);
						bufferWriter.Advance(bytes);
						i++;
						num = i;
					}
					else
					{
						int num3 = i - num;
						Span<byte> span2 = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num3));
						int bytes2 = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num3), span2);
						bufferWriter.Advance(bytes2);
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						StandardFormat format2 = StandardFormat.Parse(parseResult.FormatString);
						switch (parseResult.Index)
						{
						case 0:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T1>(ref bufferWriter, arg1, parseResult.Alignment, format2, "arg1");
							break;
						case 1:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T2>(ref bufferWriter, arg2, parseResult.Alignment, format2, "arg2");
							break;
						case 2:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T3>(ref bufferWriter, arg3, parseResult.Alignment, format2, "arg3");
							break;
						case 3:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T4>(ref bufferWriter, arg4, parseResult.Alignment, format2, "arg4");
							break;
						case 4:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T5>(ref bufferWriter, arg5, parseResult.Alignment, format2, "arg5");
							break;
						case 5:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T6>(ref bufferWriter, arg6, parseResult.Alignment, format2, "arg6");
							break;
						case 6:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T7>(ref bufferWriter, arg7, parseResult.Alignment, format2, "arg7");
							break;
						case 7:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T8>(ref bufferWriter, arg8, parseResult.Alignment, format2, "arg8");
							break;
						case 8:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T9>(ref bufferWriter, arg9, parseResult.Alignment, format2, "arg9");
							break;
						default:
							ExceptionUtil.ThrowFormatException();
							ExceptionUtil.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && format[i + 1] == '}')
					{
						int num4 = i - num;
						Span<byte> span3 = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num4));
						int bytes3 = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num4), span3);
						bufferWriter.Advance(bytes3);
						i++;
						num = i;
					}
					else
					{
						ExceptionUtil.ThrowFormatException();
					}
				}
			}
			int num5 = format.Length - num;
			if (num5 > 0)
			{
				Span<byte> span4 = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num5));
				int bytes4 = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num5), span4);
				bufferWriter.Advance(bytes4);
			}
		}

		public static void Utf8Format<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8, [Nullable(2)] T9, [Nullable(2)] T10>(IBufferWriter<byte> bufferWriter, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			int num = 0;
			for (int i = 0; i < format.Length; i++)
			{
				char c = format[i];
				if (c == '{')
				{
					if (i == format.Length - 1)
					{
						throw new FormatException("invalid format");
					}
					if (i != format.Length && format[i + 1] == '{')
					{
						int num2 = i - num;
						Span<byte> span = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num2));
						int bytes = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num2), span);
						bufferWriter.Advance(bytes);
						i++;
						num = i;
					}
					else
					{
						int num3 = i - num;
						Span<byte> span2 = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num3));
						int bytes2 = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num3), span2);
						bufferWriter.Advance(bytes2);
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						StandardFormat format2 = StandardFormat.Parse(parseResult.FormatString);
						switch (parseResult.Index)
						{
						case 0:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T1>(ref bufferWriter, arg1, parseResult.Alignment, format2, "arg1");
							break;
						case 1:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T2>(ref bufferWriter, arg2, parseResult.Alignment, format2, "arg2");
							break;
						case 2:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T3>(ref bufferWriter, arg3, parseResult.Alignment, format2, "arg3");
							break;
						case 3:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T4>(ref bufferWriter, arg4, parseResult.Alignment, format2, "arg4");
							break;
						case 4:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T5>(ref bufferWriter, arg5, parseResult.Alignment, format2, "arg5");
							break;
						case 5:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T6>(ref bufferWriter, arg6, parseResult.Alignment, format2, "arg6");
							break;
						case 6:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T7>(ref bufferWriter, arg7, parseResult.Alignment, format2, "arg7");
							break;
						case 7:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T8>(ref bufferWriter, arg8, parseResult.Alignment, format2, "arg8");
							break;
						case 8:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T9>(ref bufferWriter, arg9, parseResult.Alignment, format2, "arg9");
							break;
						case 9:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T10>(ref bufferWriter, arg10, parseResult.Alignment, format2, "arg10");
							break;
						default:
							ExceptionUtil.ThrowFormatException();
							ExceptionUtil.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && format[i + 1] == '}')
					{
						int num4 = i - num;
						Span<byte> span3 = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num4));
						int bytes3 = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num4), span3);
						bufferWriter.Advance(bytes3);
						i++;
						num = i;
					}
					else
					{
						ExceptionUtil.ThrowFormatException();
					}
				}
			}
			int num5 = format.Length - num;
			if (num5 > 0)
			{
				Span<byte> span4 = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num5));
				int bytes4 = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num5), span4);
				bufferWriter.Advance(bytes4);
			}
		}

		public static void Utf8Format<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8, [Nullable(2)] T9, [Nullable(2)] T10, [Nullable(2)] T11>(IBufferWriter<byte> bufferWriter, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			int num = 0;
			for (int i = 0; i < format.Length; i++)
			{
				char c = format[i];
				if (c == '{')
				{
					if (i == format.Length - 1)
					{
						throw new FormatException("invalid format");
					}
					if (i != format.Length && format[i + 1] == '{')
					{
						int num2 = i - num;
						Span<byte> span = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num2));
						int bytes = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num2), span);
						bufferWriter.Advance(bytes);
						i++;
						num = i;
					}
					else
					{
						int num3 = i - num;
						Span<byte> span2 = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num3));
						int bytes2 = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num3), span2);
						bufferWriter.Advance(bytes2);
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						StandardFormat format2 = StandardFormat.Parse(parseResult.FormatString);
						switch (parseResult.Index)
						{
						case 0:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T1>(ref bufferWriter, arg1, parseResult.Alignment, format2, "arg1");
							break;
						case 1:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T2>(ref bufferWriter, arg2, parseResult.Alignment, format2, "arg2");
							break;
						case 2:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T3>(ref bufferWriter, arg3, parseResult.Alignment, format2, "arg3");
							break;
						case 3:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T4>(ref bufferWriter, arg4, parseResult.Alignment, format2, "arg4");
							break;
						case 4:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T5>(ref bufferWriter, arg5, parseResult.Alignment, format2, "arg5");
							break;
						case 5:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T6>(ref bufferWriter, arg6, parseResult.Alignment, format2, "arg6");
							break;
						case 6:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T7>(ref bufferWriter, arg7, parseResult.Alignment, format2, "arg7");
							break;
						case 7:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T8>(ref bufferWriter, arg8, parseResult.Alignment, format2, "arg8");
							break;
						case 8:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T9>(ref bufferWriter, arg9, parseResult.Alignment, format2, "arg9");
							break;
						case 9:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T10>(ref bufferWriter, arg10, parseResult.Alignment, format2, "arg10");
							break;
						case 10:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T11>(ref bufferWriter, arg11, parseResult.Alignment, format2, "arg11");
							break;
						default:
							ExceptionUtil.ThrowFormatException();
							ExceptionUtil.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && format[i + 1] == '}')
					{
						int num4 = i - num;
						Span<byte> span3 = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num4));
						int bytes3 = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num4), span3);
						bufferWriter.Advance(bytes3);
						i++;
						num = i;
					}
					else
					{
						ExceptionUtil.ThrowFormatException();
					}
				}
			}
			int num5 = format.Length - num;
			if (num5 > 0)
			{
				Span<byte> span4 = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num5));
				int bytes4 = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num5), span4);
				bufferWriter.Advance(bytes4);
			}
		}

		public static void Utf8Format<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8, [Nullable(2)] T9, [Nullable(2)] T10, [Nullable(2)] T11, [Nullable(2)] T12>(IBufferWriter<byte> bufferWriter, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			int num = 0;
			for (int i = 0; i < format.Length; i++)
			{
				char c = format[i];
				if (c == '{')
				{
					if (i == format.Length - 1)
					{
						throw new FormatException("invalid format");
					}
					if (i != format.Length && format[i + 1] == '{')
					{
						int num2 = i - num;
						Span<byte> span = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num2));
						int bytes = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num2), span);
						bufferWriter.Advance(bytes);
						i++;
						num = i;
					}
					else
					{
						int num3 = i - num;
						Span<byte> span2 = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num3));
						int bytes2 = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num3), span2);
						bufferWriter.Advance(bytes2);
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						StandardFormat format2 = StandardFormat.Parse(parseResult.FormatString);
						switch (parseResult.Index)
						{
						case 0:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T1>(ref bufferWriter, arg1, parseResult.Alignment, format2, "arg1");
							break;
						case 1:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T2>(ref bufferWriter, arg2, parseResult.Alignment, format2, "arg2");
							break;
						case 2:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T3>(ref bufferWriter, arg3, parseResult.Alignment, format2, "arg3");
							break;
						case 3:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T4>(ref bufferWriter, arg4, parseResult.Alignment, format2, "arg4");
							break;
						case 4:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T5>(ref bufferWriter, arg5, parseResult.Alignment, format2, "arg5");
							break;
						case 5:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T6>(ref bufferWriter, arg6, parseResult.Alignment, format2, "arg6");
							break;
						case 6:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T7>(ref bufferWriter, arg7, parseResult.Alignment, format2, "arg7");
							break;
						case 7:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T8>(ref bufferWriter, arg8, parseResult.Alignment, format2, "arg8");
							break;
						case 8:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T9>(ref bufferWriter, arg9, parseResult.Alignment, format2, "arg9");
							break;
						case 9:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T10>(ref bufferWriter, arg10, parseResult.Alignment, format2, "arg10");
							break;
						case 10:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T11>(ref bufferWriter, arg11, parseResult.Alignment, format2, "arg11");
							break;
						case 11:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T12>(ref bufferWriter, arg12, parseResult.Alignment, format2, "arg12");
							break;
						default:
							ExceptionUtil.ThrowFormatException();
							ExceptionUtil.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && format[i + 1] == '}')
					{
						int num4 = i - num;
						Span<byte> span3 = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num4));
						int bytes3 = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num4), span3);
						bufferWriter.Advance(bytes3);
						i++;
						num = i;
					}
					else
					{
						ExceptionUtil.ThrowFormatException();
					}
				}
			}
			int num5 = format.Length - num;
			if (num5 > 0)
			{
				Span<byte> span4 = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num5));
				int bytes4 = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num5), span4);
				bufferWriter.Advance(bytes4);
			}
		}

		public static void Utf8Format<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8, [Nullable(2)] T9, [Nullable(2)] T10, [Nullable(2)] T11, [Nullable(2)] T12, [Nullable(2)] T13>(IBufferWriter<byte> bufferWriter, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			int num = 0;
			for (int i = 0; i < format.Length; i++)
			{
				char c = format[i];
				if (c == '{')
				{
					if (i == format.Length - 1)
					{
						throw new FormatException("invalid format");
					}
					if (i != format.Length && format[i + 1] == '{')
					{
						int num2 = i - num;
						Span<byte> span = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num2));
						int bytes = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num2), span);
						bufferWriter.Advance(bytes);
						i++;
						num = i;
					}
					else
					{
						int num3 = i - num;
						Span<byte> span2 = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num3));
						int bytes2 = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num3), span2);
						bufferWriter.Advance(bytes2);
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						StandardFormat format2 = StandardFormat.Parse(parseResult.FormatString);
						switch (parseResult.Index)
						{
						case 0:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T1>(ref bufferWriter, arg1, parseResult.Alignment, format2, "arg1");
							break;
						case 1:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T2>(ref bufferWriter, arg2, parseResult.Alignment, format2, "arg2");
							break;
						case 2:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T3>(ref bufferWriter, arg3, parseResult.Alignment, format2, "arg3");
							break;
						case 3:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T4>(ref bufferWriter, arg4, parseResult.Alignment, format2, "arg4");
							break;
						case 4:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T5>(ref bufferWriter, arg5, parseResult.Alignment, format2, "arg5");
							break;
						case 5:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T6>(ref bufferWriter, arg6, parseResult.Alignment, format2, "arg6");
							break;
						case 6:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T7>(ref bufferWriter, arg7, parseResult.Alignment, format2, "arg7");
							break;
						case 7:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T8>(ref bufferWriter, arg8, parseResult.Alignment, format2, "arg8");
							break;
						case 8:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T9>(ref bufferWriter, arg9, parseResult.Alignment, format2, "arg9");
							break;
						case 9:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T10>(ref bufferWriter, arg10, parseResult.Alignment, format2, "arg10");
							break;
						case 10:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T11>(ref bufferWriter, arg11, parseResult.Alignment, format2, "arg11");
							break;
						case 11:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T12>(ref bufferWriter, arg12, parseResult.Alignment, format2, "arg12");
							break;
						case 12:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T13>(ref bufferWriter, arg13, parseResult.Alignment, format2, "arg13");
							break;
						default:
							ExceptionUtil.ThrowFormatException();
							ExceptionUtil.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && format[i + 1] == '}')
					{
						int num4 = i - num;
						Span<byte> span3 = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num4));
						int bytes3 = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num4), span3);
						bufferWriter.Advance(bytes3);
						i++;
						num = i;
					}
					else
					{
						ExceptionUtil.ThrowFormatException();
					}
				}
			}
			int num5 = format.Length - num;
			if (num5 > 0)
			{
				Span<byte> span4 = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num5));
				int bytes4 = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num5), span4);
				bufferWriter.Advance(bytes4);
			}
		}

		public static void Utf8Format<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8, [Nullable(2)] T9, [Nullable(2)] T10, [Nullable(2)] T11, [Nullable(2)] T12, [Nullable(2)] T13, [Nullable(2)] T14>(IBufferWriter<byte> bufferWriter, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			int num = 0;
			for (int i = 0; i < format.Length; i++)
			{
				char c = format[i];
				if (c == '{')
				{
					if (i == format.Length - 1)
					{
						throw new FormatException("invalid format");
					}
					if (i != format.Length && format[i + 1] == '{')
					{
						int num2 = i - num;
						Span<byte> span = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num2));
						int bytes = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num2), span);
						bufferWriter.Advance(bytes);
						i++;
						num = i;
					}
					else
					{
						int num3 = i - num;
						Span<byte> span2 = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num3));
						int bytes2 = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num3), span2);
						bufferWriter.Advance(bytes2);
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						StandardFormat format2 = StandardFormat.Parse(parseResult.FormatString);
						switch (parseResult.Index)
						{
						case 0:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T1>(ref bufferWriter, arg1, parseResult.Alignment, format2, "arg1");
							break;
						case 1:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T2>(ref bufferWriter, arg2, parseResult.Alignment, format2, "arg2");
							break;
						case 2:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T3>(ref bufferWriter, arg3, parseResult.Alignment, format2, "arg3");
							break;
						case 3:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T4>(ref bufferWriter, arg4, parseResult.Alignment, format2, "arg4");
							break;
						case 4:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T5>(ref bufferWriter, arg5, parseResult.Alignment, format2, "arg5");
							break;
						case 5:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T6>(ref bufferWriter, arg6, parseResult.Alignment, format2, "arg6");
							break;
						case 6:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T7>(ref bufferWriter, arg7, parseResult.Alignment, format2, "arg7");
							break;
						case 7:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T8>(ref bufferWriter, arg8, parseResult.Alignment, format2, "arg8");
							break;
						case 8:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T9>(ref bufferWriter, arg9, parseResult.Alignment, format2, "arg9");
							break;
						case 9:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T10>(ref bufferWriter, arg10, parseResult.Alignment, format2, "arg10");
							break;
						case 10:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T11>(ref bufferWriter, arg11, parseResult.Alignment, format2, "arg11");
							break;
						case 11:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T12>(ref bufferWriter, arg12, parseResult.Alignment, format2, "arg12");
							break;
						case 12:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T13>(ref bufferWriter, arg13, parseResult.Alignment, format2, "arg13");
							break;
						case 13:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T14>(ref bufferWriter, arg14, parseResult.Alignment, format2, "arg14");
							break;
						default:
							ExceptionUtil.ThrowFormatException();
							ExceptionUtil.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && format[i + 1] == '}')
					{
						int num4 = i - num;
						Span<byte> span3 = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num4));
						int bytes3 = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num4), span3);
						bufferWriter.Advance(bytes3);
						i++;
						num = i;
					}
					else
					{
						ExceptionUtil.ThrowFormatException();
					}
				}
			}
			int num5 = format.Length - num;
			if (num5 > 0)
			{
				Span<byte> span4 = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num5));
				int bytes4 = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num5), span4);
				bufferWriter.Advance(bytes4);
			}
		}

		public static void Utf8Format<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8, [Nullable(2)] T9, [Nullable(2)] T10, [Nullable(2)] T11, [Nullable(2)] T12, [Nullable(2)] T13, [Nullable(2)] T14, [Nullable(2)] T15>(IBufferWriter<byte> bufferWriter, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			int num = 0;
			for (int i = 0; i < format.Length; i++)
			{
				char c = format[i];
				if (c == '{')
				{
					if (i == format.Length - 1)
					{
						throw new FormatException("invalid format");
					}
					if (i != format.Length && format[i + 1] == '{')
					{
						int num2 = i - num;
						Span<byte> span = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num2));
						int bytes = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num2), span);
						bufferWriter.Advance(bytes);
						i++;
						num = i;
					}
					else
					{
						int num3 = i - num;
						Span<byte> span2 = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num3));
						int bytes2 = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num3), span2);
						bufferWriter.Advance(bytes2);
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						StandardFormat format2 = StandardFormat.Parse(parseResult.FormatString);
						switch (parseResult.Index)
						{
						case 0:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T1>(ref bufferWriter, arg1, parseResult.Alignment, format2, "arg1");
							break;
						case 1:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T2>(ref bufferWriter, arg2, parseResult.Alignment, format2, "arg2");
							break;
						case 2:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T3>(ref bufferWriter, arg3, parseResult.Alignment, format2, "arg3");
							break;
						case 3:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T4>(ref bufferWriter, arg4, parseResult.Alignment, format2, "arg4");
							break;
						case 4:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T5>(ref bufferWriter, arg5, parseResult.Alignment, format2, "arg5");
							break;
						case 5:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T6>(ref bufferWriter, arg6, parseResult.Alignment, format2, "arg6");
							break;
						case 6:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T7>(ref bufferWriter, arg7, parseResult.Alignment, format2, "arg7");
							break;
						case 7:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T8>(ref bufferWriter, arg8, parseResult.Alignment, format2, "arg8");
							break;
						case 8:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T9>(ref bufferWriter, arg9, parseResult.Alignment, format2, "arg9");
							break;
						case 9:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T10>(ref bufferWriter, arg10, parseResult.Alignment, format2, "arg10");
							break;
						case 10:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T11>(ref bufferWriter, arg11, parseResult.Alignment, format2, "arg11");
							break;
						case 11:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T12>(ref bufferWriter, arg12, parseResult.Alignment, format2, "arg12");
							break;
						case 12:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T13>(ref bufferWriter, arg13, parseResult.Alignment, format2, "arg13");
							break;
						case 13:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T14>(ref bufferWriter, arg14, parseResult.Alignment, format2, "arg14");
							break;
						case 14:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T15>(ref bufferWriter, arg15, parseResult.Alignment, format2, "arg15");
							break;
						default:
							ExceptionUtil.ThrowFormatException();
							ExceptionUtil.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && format[i + 1] == '}')
					{
						int num4 = i - num;
						Span<byte> span3 = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num4));
						int bytes3 = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num4), span3);
						bufferWriter.Advance(bytes3);
						i++;
						num = i;
					}
					else
					{
						ExceptionUtil.ThrowFormatException();
					}
				}
			}
			int num5 = format.Length - num;
			if (num5 > 0)
			{
				Span<byte> span4 = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num5));
				int bytes4 = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num5), span4);
				bufferWriter.Advance(bytes4);
			}
		}

		public static void Utf8Format<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8, [Nullable(2)] T9, [Nullable(2)] T10, [Nullable(2)] T11, [Nullable(2)] T12, [Nullable(2)] T13, [Nullable(2)] T14, [Nullable(2)] T15, [Nullable(2)] T16>(IBufferWriter<byte> bufferWriter, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			int num = 0;
			for (int i = 0; i < format.Length; i++)
			{
				char c = format[i];
				if (c == '{')
				{
					if (i == format.Length - 1)
					{
						throw new FormatException("invalid format");
					}
					if (i != format.Length && format[i + 1] == '{')
					{
						int num2 = i - num;
						Span<byte> span = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num2));
						int bytes = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num2), span);
						bufferWriter.Advance(bytes);
						i++;
						num = i;
					}
					else
					{
						int num3 = i - num;
						Span<byte> span2 = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num3));
						int bytes2 = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num3), span2);
						bufferWriter.Advance(bytes2);
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						StandardFormat format2 = StandardFormat.Parse(parseResult.FormatString);
						switch (parseResult.Index)
						{
						case 0:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T1>(ref bufferWriter, arg1, parseResult.Alignment, format2, "arg1");
							break;
						case 1:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T2>(ref bufferWriter, arg2, parseResult.Alignment, format2, "arg2");
							break;
						case 2:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T3>(ref bufferWriter, arg3, parseResult.Alignment, format2, "arg3");
							break;
						case 3:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T4>(ref bufferWriter, arg4, parseResult.Alignment, format2, "arg4");
							break;
						case 4:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T5>(ref bufferWriter, arg5, parseResult.Alignment, format2, "arg5");
							break;
						case 5:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T6>(ref bufferWriter, arg6, parseResult.Alignment, format2, "arg6");
							break;
						case 6:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T7>(ref bufferWriter, arg7, parseResult.Alignment, format2, "arg7");
							break;
						case 7:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T8>(ref bufferWriter, arg8, parseResult.Alignment, format2, "arg8");
							break;
						case 8:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T9>(ref bufferWriter, arg9, parseResult.Alignment, format2, "arg9");
							break;
						case 9:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T10>(ref bufferWriter, arg10, parseResult.Alignment, format2, "arg10");
							break;
						case 10:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T11>(ref bufferWriter, arg11, parseResult.Alignment, format2, "arg11");
							break;
						case 11:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T12>(ref bufferWriter, arg12, parseResult.Alignment, format2, "arg12");
							break;
						case 12:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T13>(ref bufferWriter, arg13, parseResult.Alignment, format2, "arg13");
							break;
						case 13:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T14>(ref bufferWriter, arg14, parseResult.Alignment, format2, "arg14");
							break;
						case 14:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T15>(ref bufferWriter, arg15, parseResult.Alignment, format2, "arg15");
							break;
						case 15:
							Utf8FormatHelper.FormatTo<IBufferWriter<byte>, T16>(ref bufferWriter, arg16, parseResult.Alignment, format2, "arg16");
							break;
						default:
							ExceptionUtil.ThrowFormatException();
							ExceptionUtil.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && format[i + 1] == '}')
					{
						int num4 = i - num;
						Span<byte> span3 = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num4));
						int bytes3 = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num4), span3);
						bufferWriter.Advance(bytes3);
						i++;
						num = i;
					}
					else
					{
						ExceptionUtil.ThrowFormatException();
					}
				}
			}
			int num5 = format.Length - num;
			if (num5 > 0)
			{
				Span<byte> span4 = bufferWriter.GetSpan(ZString.UTF8NoBom.GetMaxByteCount(num5));
				int bytes4 = ZString.UTF8NoBom.GetBytes(format.AsSpan(num, num5), span4);
				bufferWriter.Advance(bytes4);
			}
		}

		private static Encoding UTF8NoBom = new UTF8Encoding(false);
	}
}
