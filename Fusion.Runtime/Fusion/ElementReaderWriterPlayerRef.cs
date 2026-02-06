using System;
using System.Runtime.CompilerServices;

namespace Fusion
{
	internal struct ElementReaderWriterPlayerRef : IElementReaderWriter<PlayerRef>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe PlayerRef Read(byte* data, int index)
		{
			return *(PlayerRef*)(data + index * 4);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe ref PlayerRef ReadRef(byte* data, int index)
		{
			return ref *(PlayerRef*)(data + index * 4);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void Write(byte* data, int index, PlayerRef val)
		{
			*(PlayerRef*)(data + index * 4) = val;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetElementWordCount()
		{
			return 1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetElementHashCode(PlayerRef val)
		{
			return val.GetHashCode();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IElementReaderWriter<PlayerRef> GetInstance()
		{
			bool flag = ElementReaderWriterPlayerRef._instance == null;
			if (flag)
			{
				ElementReaderWriterPlayerRef._instance = default(ElementReaderWriterPlayerRef);
			}
			return ElementReaderWriterPlayerRef._instance;
		}

		private static IElementReaderWriter<PlayerRef> _instance;
	}
}
