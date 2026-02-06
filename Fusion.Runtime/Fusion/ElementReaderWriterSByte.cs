using System;
using System.Runtime.CompilerServices;

namespace Fusion
{
	internal struct ElementReaderWriterSByte : IElementReaderWriter<sbyte>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe sbyte Read(byte* data, int index)
		{
			return *(sbyte*)(data + index * 4);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe ref sbyte ReadRef(byte* data, int index)
		{
			return ref *(sbyte*)(data + index * 4);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void Write(byte* data, int index, sbyte val)
		{
			data[index * 4] = (byte)val;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetElementWordCount()
		{
			return 1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetElementHashCode(sbyte val)
		{
			return val.GetHashCode();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IElementReaderWriter<sbyte> GetInstance()
		{
			bool flag = ElementReaderWriterSByte._instance == null;
			if (flag)
			{
				ElementReaderWriterSByte._instance = default(ElementReaderWriterSByte);
			}
			return ElementReaderWriterSByte._instance;
		}

		private static IElementReaderWriter<sbyte> _instance;
	}
}
