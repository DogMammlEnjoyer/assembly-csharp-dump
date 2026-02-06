using System;
using System.Runtime.CompilerServices;

namespace Fusion
{
	internal struct ElementReaderWriterInt16 : IElementReaderWriter<short>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe short Read(byte* data, int index)
		{
			return *(short*)(data + index * 4);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe ref short ReadRef(byte* data, int index)
		{
			return ref *(short*)(data + index * 4);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void Write(byte* data, int index, short val)
		{
			*(short*)(data + index * 4) = val;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetElementWordCount()
		{
			return 1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetElementHashCode(short val)
		{
			return val.GetHashCode();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IElementReaderWriter<short> GetInstance()
		{
			bool flag = ElementReaderWriterInt16._instance == null;
			if (flag)
			{
				ElementReaderWriterInt16._instance = default(ElementReaderWriterInt16);
			}
			return ElementReaderWriterInt16._instance;
		}

		private static IElementReaderWriter<short> _instance;
	}
}
