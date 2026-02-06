using System;
using System.Runtime.CompilerServices;

namespace Fusion
{
	internal struct ElementReaderWriterSingle : IElementReaderWriter<float>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe float Read(byte* data, int index)
		{
			return *(float*)(data + index * 4);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe ref float ReadRef(byte* data, int index)
		{
			return ref *(float*)(data + index * 4);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void Write(byte* data, int index, float val)
		{
			*(float*)(data + index * 4) = val;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetElementWordCount()
		{
			return 1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetElementHashCode(float val)
		{
			return val.GetHashCode();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IElementReaderWriter<float> GetInstance()
		{
			bool flag = ElementReaderWriterSingle._instance == null;
			if (flag)
			{
				ElementReaderWriterSingle._instance = default(ElementReaderWriterSingle);
			}
			return ElementReaderWriterSingle._instance;
		}

		private static IElementReaderWriter<float> _instance;
	}
}
