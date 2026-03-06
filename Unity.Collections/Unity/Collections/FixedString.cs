using System;
using System.Runtime.CompilerServices;

namespace Unity.Collections
{
	[GenerateTestsForBurstCompatibility]
	public static class FixedString
	{
		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, int arg1, int arg2, int arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, int arg1, int arg2, int arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, int arg1, int arg2, int arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, int arg1, int arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, float arg1, int arg2, int arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, float arg1, int arg2, int arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, float arg1, int arg2, int arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, float arg1, int arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, string arg1, int arg2, int arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, string arg1, int arg2, int arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, string arg1, int arg2, int arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, string arg1, int arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, T1 arg1, int arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, T1 arg1, int arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, T1 arg1, int arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, T2 arg1, int arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg2);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg3);
			ref result.AppendFormat(formatString, arg0, arg1, fixedString32Bytes, fixedString32Bytes2);
			return result;
		}

		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, int arg1, float arg2, int arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, int arg1, float arg2, int arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, int arg1, float arg2, int arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, int arg1, float arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, float arg1, float arg2, int arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, float arg1, float arg2, int arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, float arg1, float arg2, int arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, float arg1, float arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, string arg1, float arg2, int arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, string arg1, float arg2, int arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, string arg1, float arg2, int arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, string arg1, float arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, T1 arg1, float arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, T1 arg1, float arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, T1 arg1, float arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, T2 arg1, float arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg2, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg3);
			ref result.AppendFormat(formatString, arg0, arg1, fixedString32Bytes, fixedString32Bytes2);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, int arg1, string arg2, int arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, int arg1, string arg2, int arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, int arg1, string arg2, int arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, int arg1, string arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, float arg1, string arg2, int arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, float arg1, string arg2, int arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, float arg1, string arg2, int arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, float arg1, string arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, string arg1, string arg2, int arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, string arg1, string arg2, int arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, string arg1, string arg2, int arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, string arg1, string arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, T1 arg1, string arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, T1 arg1, string arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, T1 arg1, string arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, T2 arg1, string arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg2);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg3);
			ref result.AppendFormat(formatString, arg0, arg1, fixedString32Bytes, fixedString32Bytes2);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, int arg1, T1 arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, arg2, fixedString32Bytes3);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, int arg1, T1 arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, arg2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, int arg1, T1 arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, arg2, fixedString32Bytes3);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, int arg1, T2 arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg3);
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, arg2, fixedString32Bytes2);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, float arg1, T1 arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, arg2, fixedString32Bytes3);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, float arg1, T1 arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, arg2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, float arg1, T1 arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, arg2, fixedString32Bytes3);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, float arg1, T2 arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg3);
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, arg2, fixedString32Bytes2);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, string arg1, T1 arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, arg2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, string arg1, T1 arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, arg2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, string arg1, T1 arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, arg2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, string arg1, T2 arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg3);
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, arg2, fixedString32Bytes2);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, int arg0, T1 arg1, T2 arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, arg2, fixedString32Bytes2);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, float arg0, T1 arg1, T2 arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, arg2, fixedString32Bytes2);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, string arg0, T1 arg1, T2 arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, arg2, fixedString32Bytes2);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2, [IsUnmanaged] T3>(FixedString512Bytes formatString, T1 arg0, T2 arg1, T3 arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T3 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg3);
			ref result.AppendFormat(formatString, arg0, arg1, arg2, fixedString32Bytes);
			return result;
		}

		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, int arg1, int arg2, float arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, int arg1, int arg2, float arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, int arg1, int arg2, float arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, int arg1, int arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3, '.');
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, float arg1, int arg2, float arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, float arg1, int arg2, float arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, float arg1, int arg2, float arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, float arg1, int arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3, '.');
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, string arg1, int arg2, float arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, string arg1, int arg2, float arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, string arg1, int arg2, float arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, string arg1, int arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3, '.');
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, T1 arg1, int arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, T1 arg1, int arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, T1 arg1, int arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, T2 arg1, int arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg2);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg3, '.');
			ref result.AppendFormat(formatString, arg0, arg1, fixedString32Bytes, fixedString32Bytes2);
			return result;
		}

		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, int arg1, float arg2, float arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, int arg1, float arg2, float arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, int arg1, float arg2, float arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, int arg1, float arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3, '.');
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, float arg1, float arg2, float arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, float arg1, float arg2, float arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, float arg1, float arg2, float arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, float arg1, float arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3, '.');
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, string arg1, float arg2, float arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, string arg1, float arg2, float arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, string arg1, float arg2, float arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, string arg1, float arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3, '.');
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, T1 arg1, float arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, T1 arg1, float arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, T1 arg1, float arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, T2 arg1, float arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg2, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg3, '.');
			ref result.AppendFormat(formatString, arg0, arg1, fixedString32Bytes, fixedString32Bytes2);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, int arg1, string arg2, float arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, int arg1, string arg2, float arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, int arg1, string arg2, float arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, int arg1, string arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3, '.');
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, float arg1, string arg2, float arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, float arg1, string arg2, float arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, float arg1, string arg2, float arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, float arg1, string arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3, '.');
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, string arg1, string arg2, float arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, string arg1, string arg2, float arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, string arg1, string arg2, float arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, string arg1, string arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3, '.');
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, T1 arg1, string arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, T1 arg1, string arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, T1 arg1, string arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, T2 arg1, string arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg2);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg3, '.');
			ref result.AppendFormat(formatString, arg0, arg1, fixedString32Bytes, fixedString32Bytes2);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, int arg1, T1 arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, arg2, fixedString32Bytes3);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, int arg1, T1 arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, arg2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, int arg1, T1 arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, arg2, fixedString32Bytes3);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, int arg1, T2 arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg3, '.');
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, arg2, fixedString32Bytes2);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, float arg1, T1 arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, arg2, fixedString32Bytes3);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, float arg1, T1 arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, arg2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, float arg1, T1 arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, arg2, fixedString32Bytes3);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, float arg1, T2 arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg3, '.');
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, arg2, fixedString32Bytes2);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, string arg1, T1 arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, arg2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, string arg1, T1 arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, arg2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, string arg1, T1 arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, arg2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, string arg1, T2 arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg3, '.');
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, arg2, fixedString32Bytes2);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, int arg0, T1 arg1, T2 arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg3, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, arg2, fixedString32Bytes2);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, float arg0, T1 arg1, T2 arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg3, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, arg2, fixedString32Bytes2);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, string arg0, T1 arg1, T2 arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg3, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, arg2, fixedString32Bytes2);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2, [IsUnmanaged] T3>(FixedString512Bytes formatString, T1 arg0, T2 arg1, T3 arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T3 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg3, '.');
			ref result.AppendFormat(formatString, arg0, arg1, arg2, fixedString32Bytes);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, int arg1, int arg2, string arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, int arg1, int arg2, string arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, int arg1, int arg2, string arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, int arg1, int arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, float arg1, int arg2, string arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, float arg1, int arg2, string arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, float arg1, int arg2, string arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, float arg1, int arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, string arg1, int arg2, string arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, string arg1, int arg2, string arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, string arg1, int arg2, string arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, string arg1, int arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, T1 arg1, int arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, T1 arg1, int arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, T1 arg1, int arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, T2 arg1, int arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg2);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg3);
			ref result.AppendFormat(formatString, arg0, arg1, fixedString32Bytes, fixedString32Bytes2);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, int arg1, float arg2, string arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, int arg1, float arg2, string arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, int arg1, float arg2, string arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, int arg1, float arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, float arg1, float arg2, string arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, float arg1, float arg2, string arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, float arg1, float arg2, string arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, float arg1, float arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, string arg1, float arg2, string arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, string arg1, float arg2, string arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, string arg1, float arg2, string arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, string arg1, float arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, T1 arg1, float arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, T1 arg1, float arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, T1 arg1, float arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, T2 arg1, float arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg2, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg3);
			ref result.AppendFormat(formatString, arg0, arg1, fixedString32Bytes, fixedString32Bytes2);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, int arg1, string arg2, string arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, int arg1, string arg2, string arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, int arg1, string arg2, string arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, int arg1, string arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, float arg1, string arg2, string arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, float arg1, string arg2, string arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, float arg1, string arg2, string arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, float arg1, string arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, string arg1, string arg2, string arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, string arg1, string arg2, string arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, string arg1, string arg2, string arg3)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			ref fixedString32Bytes4.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, fixedString32Bytes4);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, string arg1, string arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, T1 arg1, string arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, T1 arg1, string arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, T1 arg1, string arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, T2 arg1, string arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg2);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg3);
			ref result.AppendFormat(formatString, arg0, arg1, fixedString32Bytes, fixedString32Bytes2);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, int arg1, T1 arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, arg2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, int arg1, T1 arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, arg2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, int arg1, T1 arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, arg2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, int arg1, T2 arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg3);
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, arg2, fixedString32Bytes2);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, float arg1, T1 arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, arg2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, float arg1, T1 arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, arg2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, float arg1, T1 arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, arg2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, float arg1, T2 arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg3);
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, arg2, fixedString32Bytes2);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, string arg1, T1 arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, arg2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, string arg1, T1 arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, arg2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, string arg1, T1 arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, arg2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, string arg1, T2 arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg3);
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, arg2, fixedString32Bytes2);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, int arg0, T1 arg1, T2 arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, arg2, fixedString32Bytes2);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, float arg0, T1 arg1, T2 arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, arg2, fixedString32Bytes2);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, string arg0, T1 arg1, T2 arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg3);
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, arg2, fixedString32Bytes2);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2, [IsUnmanaged] T3>(FixedString512Bytes formatString, T1 arg0, T2 arg1, T3 arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T3 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg3);
			ref result.AppendFormat(formatString, arg0, arg1, arg2, fixedString32Bytes);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, int arg1, int arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, arg3);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, int arg1, int arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, arg3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, int arg1, int arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, arg3);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, int arg1, int arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, fixedString32Bytes2, arg3);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, float arg1, int arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, arg3);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, float arg1, int arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, arg3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, float arg1, int arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, arg3);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, float arg1, int arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, fixedString32Bytes2, arg3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, string arg1, int arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, arg3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, string arg1, int arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, arg3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, string arg1, int arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, arg3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, string arg1, int arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, fixedString32Bytes2, arg3);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, int arg0, T1 arg1, int arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, fixedString32Bytes2, arg3);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, float arg0, T1 arg1, int arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, fixedString32Bytes2, arg3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, string arg0, T1 arg1, int arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, fixedString32Bytes2, arg3);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2, [IsUnmanaged] T3>(FixedString512Bytes formatString, T1 arg0, T2 arg1, int arg2, T3 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T3 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg2);
			ref result.AppendFormat(formatString, arg0, arg1, fixedString32Bytes, arg3);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, int arg1, float arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, arg3);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, int arg1, float arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, arg3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, int arg1, float arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, arg3);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, int arg1, float arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2, '.');
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, fixedString32Bytes2, arg3);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, float arg1, float arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, arg3);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, float arg1, float arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, arg3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, float arg1, float arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, arg3);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, float arg1, float arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2, '.');
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, fixedString32Bytes2, arg3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, string arg1, float arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, arg3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, string arg1, float arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, arg3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, string arg1, float arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, arg3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, string arg1, float arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2, '.');
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, fixedString32Bytes2, arg3);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, int arg0, T1 arg1, float arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, fixedString32Bytes2, arg3);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, float arg0, T1 arg1, float arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, fixedString32Bytes2, arg3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, string arg0, T1 arg1, float arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, fixedString32Bytes2, arg3);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2, [IsUnmanaged] T3>(FixedString512Bytes formatString, T1 arg0, T2 arg1, float arg2, T3 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T3 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg2, '.');
			ref result.AppendFormat(formatString, arg0, arg1, fixedString32Bytes, arg3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, int arg1, string arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, arg3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, int arg1, string arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, arg3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, int arg1, string arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, arg3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, int arg1, string arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, fixedString32Bytes2, arg3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, float arg1, string arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, arg3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, float arg1, string arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, arg3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, float arg1, string arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, arg3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, float arg1, string arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, fixedString32Bytes2, arg3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, string arg1, string arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, arg3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, string arg1, string arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, arg3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, string arg1, string arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3, arg3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, string arg1, string arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, fixedString32Bytes2, arg3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, int arg0, T1 arg1, string arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, fixedString32Bytes2, arg3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, float arg0, T1 arg1, string arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, fixedString32Bytes2, arg3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, string arg0, T1 arg1, string arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, fixedString32Bytes2, arg3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2, [IsUnmanaged] T3>(FixedString512Bytes formatString, T1 arg0, T2 arg1, string arg2, T3 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T3 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg2);
			ref result.AppendFormat(formatString, arg0, arg1, fixedString32Bytes, arg3);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, int arg0, int arg1, T1 arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, arg2, arg3);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, float arg0, int arg1, T1 arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, arg2, arg3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, string arg0, int arg1, T1 arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, arg2, arg3);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2, [IsUnmanaged] T3>(FixedString512Bytes formatString, T1 arg0, int arg1, T2 arg2, T3 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T3 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1);
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, arg2, arg3);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, int arg0, float arg1, T1 arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, arg2, arg3);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, float arg0, float arg1, T1 arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, arg2, arg3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, string arg0, float arg1, T1 arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, arg2, arg3);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2, [IsUnmanaged] T3>(FixedString512Bytes formatString, T1 arg0, float arg1, T2 arg2, T3 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T3 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1, '.');
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, arg2, arg3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, int arg0, string arg1, T1 arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, arg2, arg3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, float arg0, string arg1, T1 arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, arg2, arg3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString512Bytes formatString, string arg0, string arg1, T1 arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, arg2, arg3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2, [IsUnmanaged] T3>(FixedString512Bytes formatString, T1 arg0, string arg1, T2 arg2, T3 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T3 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1);
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, arg2, arg3);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2, [IsUnmanaged] T3>(FixedString512Bytes formatString, int arg0, T1 arg1, T2 arg2, T3 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T3 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, arg2, arg3);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2, [IsUnmanaged] T3>(FixedString512Bytes formatString, float arg0, T1 arg1, T2 arg2, T3 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T3 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, arg2, arg3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2, [IsUnmanaged] T3>(FixedString512Bytes formatString, string arg0, T1 arg1, T2 arg2, T3 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T3 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, arg2, arg3);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2, [IsUnmanaged] T3, [IsUnmanaged] T4>(FixedString512Bytes formatString, T1 arg0, T2 arg1, T3 arg2, T4 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T3 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T4 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			ref result.AppendFormat(formatString, arg0, arg1, arg2, arg3);
			return result;
		}

		public static FixedString128Bytes Format(FixedString128Bytes formatString, int arg0, int arg1, int arg2)
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		public static FixedString128Bytes Format(FixedString128Bytes formatString, float arg0, int arg1, int arg2)
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format(FixedString128Bytes formatString, string arg0, int arg1, int arg2)
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes)
		})]
		public static FixedString128Bytes Format<[IsUnmanaged] T1>(FixedString128Bytes formatString, T1 arg0, int arg1, int arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, fixedString32Bytes2);
			return result;
		}

		public static FixedString128Bytes Format(FixedString128Bytes formatString, int arg0, float arg1, int arg2)
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		public static FixedString128Bytes Format(FixedString128Bytes formatString, float arg0, float arg1, int arg2)
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format(FixedString128Bytes formatString, string arg0, float arg1, int arg2)
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes)
		})]
		public static FixedString128Bytes Format<[IsUnmanaged] T1>(FixedString128Bytes formatString, T1 arg0, float arg1, int arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, fixedString32Bytes2);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format(FixedString128Bytes formatString, int arg0, string arg1, int arg2)
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format(FixedString128Bytes formatString, float arg0, string arg1, int arg2)
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format(FixedString128Bytes formatString, string arg0, string arg1, int arg2)
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format<[IsUnmanaged] T1>(FixedString128Bytes formatString, T1 arg0, string arg1, int arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, fixedString32Bytes2);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes)
		})]
		public static FixedString128Bytes Format<[IsUnmanaged] T1>(FixedString128Bytes formatString, int arg0, T1 arg1, int arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, fixedString32Bytes2);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes)
		})]
		public static FixedString128Bytes Format<[IsUnmanaged] T1>(FixedString128Bytes formatString, float arg0, T1 arg1, int arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, fixedString32Bytes2);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format<[IsUnmanaged] T1>(FixedString128Bytes formatString, string arg0, T1 arg1, int arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, fixedString32Bytes2);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString128Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString128Bytes formatString, T1 arg0, T2 arg1, int arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg2);
			ref result.AppendFormat(formatString, arg0, arg1, fixedString32Bytes);
			return result;
		}

		public static FixedString128Bytes Format(FixedString128Bytes formatString, int arg0, int arg1, float arg2)
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		public static FixedString128Bytes Format(FixedString128Bytes formatString, float arg0, int arg1, float arg2)
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format(FixedString128Bytes formatString, string arg0, int arg1, float arg2)
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes)
		})]
		public static FixedString128Bytes Format<[IsUnmanaged] T1>(FixedString128Bytes formatString, T1 arg0, int arg1, float arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2, '.');
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, fixedString32Bytes2);
			return result;
		}

		public static FixedString128Bytes Format(FixedString128Bytes formatString, int arg0, float arg1, float arg2)
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		public static FixedString128Bytes Format(FixedString128Bytes formatString, float arg0, float arg1, float arg2)
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format(FixedString128Bytes formatString, string arg0, float arg1, float arg2)
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes)
		})]
		public static FixedString128Bytes Format<[IsUnmanaged] T1>(FixedString128Bytes formatString, T1 arg0, float arg1, float arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2, '.');
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, fixedString32Bytes2);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format(FixedString128Bytes formatString, int arg0, string arg1, float arg2)
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format(FixedString128Bytes formatString, float arg0, string arg1, float arg2)
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format(FixedString128Bytes formatString, string arg0, string arg1, float arg2)
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format<[IsUnmanaged] T1>(FixedString128Bytes formatString, T1 arg0, string arg1, float arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2, '.');
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, fixedString32Bytes2);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes)
		})]
		public static FixedString128Bytes Format<[IsUnmanaged] T1>(FixedString128Bytes formatString, int arg0, T1 arg1, float arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, fixedString32Bytes2);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes)
		})]
		public static FixedString128Bytes Format<[IsUnmanaged] T1>(FixedString128Bytes formatString, float arg0, T1 arg1, float arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, fixedString32Bytes2);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format<[IsUnmanaged] T1>(FixedString128Bytes formatString, string arg0, T1 arg1, float arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, fixedString32Bytes2);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString128Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString128Bytes formatString, T1 arg0, T2 arg1, float arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg2, '.');
			ref result.AppendFormat(formatString, arg0, arg1, fixedString32Bytes);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format(FixedString128Bytes formatString, int arg0, int arg1, string arg2)
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format(FixedString128Bytes formatString, float arg0, int arg1, string arg2)
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format(FixedString128Bytes formatString, string arg0, int arg1, string arg2)
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format<[IsUnmanaged] T1>(FixedString128Bytes formatString, T1 arg0, int arg1, string arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, fixedString32Bytes2);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format(FixedString128Bytes formatString, int arg0, float arg1, string arg2)
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format(FixedString128Bytes formatString, float arg0, float arg1, string arg2)
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format(FixedString128Bytes formatString, string arg0, float arg1, string arg2)
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format<[IsUnmanaged] T1>(FixedString128Bytes formatString, T1 arg0, float arg1, string arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, fixedString32Bytes2);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format(FixedString128Bytes formatString, int arg0, string arg1, string arg2)
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format(FixedString128Bytes formatString, float arg0, string arg1, string arg2)
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format(FixedString128Bytes formatString, string arg0, string arg1, string arg2)
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			ref fixedString32Bytes3.Append(arg2);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, fixedString32Bytes3);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format<[IsUnmanaged] T1>(FixedString128Bytes formatString, T1 arg0, string arg1, string arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, fixedString32Bytes2);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format<[IsUnmanaged] T1>(FixedString128Bytes formatString, int arg0, T1 arg1, string arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, fixedString32Bytes2);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format<[IsUnmanaged] T1>(FixedString128Bytes formatString, float arg0, T1 arg1, string arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, fixedString32Bytes2);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format<[IsUnmanaged] T1>(FixedString128Bytes formatString, string arg0, T1 arg1, string arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg2);
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, fixedString32Bytes2);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString128Bytes formatString, T1 arg0, T2 arg1, string arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg2);
			ref result.AppendFormat(formatString, arg0, arg1, fixedString32Bytes);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes)
		})]
		public static FixedString128Bytes Format<[IsUnmanaged] T1>(FixedString128Bytes formatString, int arg0, int arg1, T1 arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, arg2);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes)
		})]
		public static FixedString128Bytes Format<[IsUnmanaged] T1>(FixedString128Bytes formatString, float arg0, int arg1, T1 arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, arg2);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format<[IsUnmanaged] T1>(FixedString128Bytes formatString, string arg0, int arg1, T1 arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, arg2);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString128Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString128Bytes formatString, T1 arg0, int arg1, T2 arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1);
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, arg2);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes)
		})]
		public static FixedString128Bytes Format<[IsUnmanaged] T1>(FixedString128Bytes formatString, int arg0, float arg1, T1 arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, arg2);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes)
		})]
		public static FixedString128Bytes Format<[IsUnmanaged] T1>(FixedString128Bytes formatString, float arg0, float arg1, T1 arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, arg2);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format<[IsUnmanaged] T1>(FixedString128Bytes formatString, string arg0, float arg1, T1 arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, arg2);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString128Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString128Bytes formatString, T1 arg0, float arg1, T2 arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1, '.');
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, arg2);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format<[IsUnmanaged] T1>(FixedString128Bytes formatString, int arg0, string arg1, T1 arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, arg2);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format<[IsUnmanaged] T1>(FixedString128Bytes formatString, float arg0, string arg1, T1 arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, arg2);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format<[IsUnmanaged] T1>(FixedString128Bytes formatString, string arg0, string arg1, T1 arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2, arg2);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString128Bytes formatString, T1 arg0, string arg1, T2 arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1);
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes, arg2);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString128Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString128Bytes formatString, int arg0, T1 arg1, T2 arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, arg2);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString128Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString128Bytes formatString, float arg0, T1 arg1, T2 arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, arg2);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString128Bytes formatString, string arg0, T1 arg1, T2 arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1, arg2);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString128Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2, [IsUnmanaged] T3>(FixedString128Bytes formatString, T1 arg0, T2 arg1, T3 arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T3 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			ref result.AppendFormat(formatString, arg0, arg1, arg2);
			return result;
		}

		public static FixedString128Bytes Format(FixedString128Bytes formatString, int arg0, int arg1)
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2);
			return result;
		}

		public static FixedString128Bytes Format(FixedString128Bytes formatString, float arg0, int arg1)
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format(FixedString128Bytes formatString, string arg0, int arg1)
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes)
		})]
		public static FixedString128Bytes Format<[IsUnmanaged] T1>(FixedString128Bytes formatString, T1 arg0, int arg1) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1);
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes);
			return result;
		}

		public static FixedString128Bytes Format(FixedString128Bytes formatString, int arg0, float arg1)
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2);
			return result;
		}

		public static FixedString128Bytes Format(FixedString128Bytes formatString, float arg0, float arg1)
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format(FixedString128Bytes formatString, string arg0, float arg1)
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes)
		})]
		public static FixedString128Bytes Format<[IsUnmanaged] T1>(FixedString128Bytes formatString, T1 arg0, float arg1) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1, '.');
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format(FixedString128Bytes formatString, int arg0, string arg1)
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format(FixedString128Bytes formatString, float arg0, string arg1)
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format(FixedString128Bytes formatString, string arg0, string arg1)
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			ref fixedString32Bytes2.Append(arg1);
			ref result.AppendFormat(formatString, fixedString32Bytes, fixedString32Bytes2);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format<[IsUnmanaged] T1>(FixedString128Bytes formatString, T1 arg0, string arg1) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg1);
			ref result.AppendFormat(formatString, arg0, fixedString32Bytes);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes)
		})]
		public static FixedString128Bytes Format<[IsUnmanaged] T1>(FixedString128Bytes formatString, int arg0, T1 arg1) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes)
		})]
		public static FixedString128Bytes Format<[IsUnmanaged] T1>(FixedString128Bytes formatString, float arg0, T1 arg1) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format<[IsUnmanaged] T1>(FixedString128Bytes formatString, string arg0, T1 arg1) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			ref result.AppendFormat(formatString, fixedString32Bytes, arg1);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString128Bytes Format<[IsUnmanaged] T1, [IsUnmanaged] T2>(FixedString128Bytes formatString, T1 arg0, T2 arg1) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			ref result.AppendFormat(formatString, arg0, arg1);
			return result;
		}

		public static FixedString128Bytes Format(FixedString128Bytes formatString, int arg0)
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			ref result.AppendFormat(formatString, fixedString32Bytes);
			return result;
		}

		public static FixedString128Bytes Format(FixedString128Bytes formatString, float arg0)
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0, '.');
			ref result.AppendFormat(formatString, fixedString32Bytes);
			return result;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format(FixedString128Bytes formatString, string arg0)
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			ref fixedString32Bytes.Append(arg0);
			ref result.AppendFormat(formatString, fixedString32Bytes);
			return result;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes)
		})]
		public static FixedString128Bytes Format<[IsUnmanaged] T1>(FixedString128Bytes formatString, T1 arg0) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			ref result.AppendFormat(formatString, arg0);
			return result;
		}
	}
}
