using System;
using System.Runtime.CompilerServices;

namespace Fusion
{
	internal struct ElementReaderWriterByte : IElementReaderWriter<byte>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe byte Read(byte* data, int index)
		{
			return data[index * 4];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe ref byte ReadRef(byte* data, int index)
		{
			return ref data[index * 4];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void Write(byte* data, int index, byte val)
		{
			data[index * 4] = val;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetElementWordCount()
		{
			return 1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetElementHashCode(byte val)
		{
			return val.GetHashCode();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IElementReaderWriter<byte> GetInstance()
		{
			bool flag = ElementReaderWriterByte._instance == null;
			if (flag)
			{
				ElementReaderWriterByte._instance = default(ElementReaderWriterByte);
			}
			return ElementReaderWriterByte._instance;
		}

		private static IElementReaderWriter<byte> _instance;
	}
}
