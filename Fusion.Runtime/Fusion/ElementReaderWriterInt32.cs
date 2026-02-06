using System;
using System.Runtime.CompilerServices;

namespace Fusion
{
	internal struct ElementReaderWriterInt32 : IElementReaderWriter<int>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe int Read(byte* data, int index)
		{
			return *(int*)(data + index * 4);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe ref int ReadRef(byte* data, int index)
		{
			return ref *(int*)(data + index * 4);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void Write(byte* data, int index, int val)
		{
			*(int*)(data + index * 4) = val;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetElementWordCount()
		{
			return 1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetElementHashCode(int val)
		{
			return val.GetHashCode();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IElementReaderWriter<int> GetInstance()
		{
			bool flag = ElementReaderWriterInt32._instance == null;
			if (flag)
			{
				ElementReaderWriterInt32._instance = default(ElementReaderWriterInt32);
			}
			return ElementReaderWriterInt32._instance;
		}

		private static IElementReaderWriter<int> _instance;
	}
}
