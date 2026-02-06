using System;
using System.Runtime.CompilerServices;

namespace Fusion
{
	internal struct ElementReaderWriterUInt32 : IElementReaderWriter<uint>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe uint Read(byte* data, int index)
		{
			return *(uint*)(data + index * 4);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe ref uint ReadRef(byte* data, int index)
		{
			return ref *(uint*)(data + index * 4);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void Write(byte* data, int index, uint val)
		{
			*(int*)(data + index * 4) = (int)val;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetElementWordCount()
		{
			return 1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetElementHashCode(uint val)
		{
			return val.GetHashCode();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IElementReaderWriter<uint> GetInstance()
		{
			bool flag = ElementReaderWriterUInt32._instance == null;
			if (flag)
			{
				ElementReaderWriterUInt32._instance = default(ElementReaderWriterUInt32);
			}
			return ElementReaderWriterUInt32._instance;
		}

		private static IElementReaderWriter<uint> _instance;
	}
}
