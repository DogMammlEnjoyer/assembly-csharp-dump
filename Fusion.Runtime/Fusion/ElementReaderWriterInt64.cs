using System;
using System.Runtime.CompilerServices;

namespace Fusion
{
	internal struct ElementReaderWriterInt64 : IElementReaderWriter<long>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe long Read(byte* data, int index)
		{
			return *(long*)(data + index * 8);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe ref long ReadRef(byte* data, int index)
		{
			return ref *(long*)(data + index * 8);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void Write(byte* data, int index, long val)
		{
			*(long*)(data + index * 8) = val;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetElementWordCount()
		{
			return 2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetElementHashCode(long val)
		{
			return val.GetHashCode();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IElementReaderWriter<long> GetInstance()
		{
			bool flag = ElementReaderWriterInt64._instance == null;
			if (flag)
			{
				ElementReaderWriterInt64._instance = default(ElementReaderWriterInt64);
			}
			return ElementReaderWriterInt64._instance;
		}

		private static IElementReaderWriter<long> _instance;
	}
}
