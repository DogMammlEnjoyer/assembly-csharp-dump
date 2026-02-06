using System;
using System.Runtime.CompilerServices;

namespace Fusion
{
	internal struct ElementReaderWriterChar : IElementReaderWriter<char>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe char Read(byte* data, int index)
		{
			return (char)(*(ushort*)(data + index * 4));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe ref char ReadRef(byte* data, int index)
		{
			return ref *(char*)(data + index * 4);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void Write(byte* data, int index, char val)
		{
			*(short*)(data + index * 4) = (short)val;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetElementWordCount()
		{
			return 1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetElementHashCode(char val)
		{
			return val.GetHashCode();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IElementReaderWriter<char> GetInstance()
		{
			bool flag = ElementReaderWriterChar._instance == null;
			if (flag)
			{
				ElementReaderWriterChar._instance = default(ElementReaderWriterChar);
			}
			return ElementReaderWriterChar._instance;
		}

		private static IElementReaderWriter<char> _instance;
	}
}
