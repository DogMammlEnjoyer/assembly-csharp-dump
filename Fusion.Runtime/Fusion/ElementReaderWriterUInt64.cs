using System;
using System.Runtime.CompilerServices;

namespace Fusion
{
	internal struct ElementReaderWriterUInt64 : IElementReaderWriter<ulong>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe ulong Read(byte* data, int index)
		{
			return (ulong)(*(long*)(data + index * 8));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe ref ulong ReadRef(byte* data, int index)
		{
			return ref *(ulong*)(data + index * 8);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void Write(byte* data, int index, ulong val)
		{
			*(long*)(data + index * 8) = (long)val;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetElementWordCount()
		{
			return 2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetElementHashCode(ulong val)
		{
			return val.GetHashCode();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IElementReaderWriter<ulong> GetInstance()
		{
			bool flag = ElementReaderWriterUInt64._instance == null;
			if (flag)
			{
				ElementReaderWriterUInt64._instance = default(ElementReaderWriterUInt64);
			}
			return ElementReaderWriterUInt64._instance;
		}

		private static IElementReaderWriter<ulong> _instance;
	}
}
