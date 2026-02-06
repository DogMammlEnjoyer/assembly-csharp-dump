using System;
using System.Runtime.CompilerServices;

namespace Fusion
{
	internal struct ElementReaderWriterUInt16 : IElementReaderWriter<ushort>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe ushort Read(byte* data, int index)
		{
			return *(ushort*)(data + index * 4);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe ref ushort ReadRef(byte* data, int index)
		{
			return ref *(ushort*)(data + index * 4);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void Write(byte* data, int index, ushort val)
		{
			*(short*)(data + index * 4) = (short)val;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetElementWordCount()
		{
			return 1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetElementHashCode(ushort val)
		{
			return val.GetHashCode();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IElementReaderWriter<ushort> GetInstance()
		{
			bool flag = ElementReaderWriterUInt16._instance == null;
			if (flag)
			{
				ElementReaderWriterUInt16._instance = default(ElementReaderWriterUInt16);
			}
			return ElementReaderWriterUInt16._instance;
		}

		private static IElementReaderWriter<ushort> _instance;
	}
}
