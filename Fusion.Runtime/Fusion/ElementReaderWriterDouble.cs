using System;
using System.Runtime.CompilerServices;

namespace Fusion
{
	internal struct ElementReaderWriterDouble : IElementReaderWriter<double>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe double Read(byte* data, int index)
		{
			return *(double*)(data + index * 8);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe ref double ReadRef(byte* data, int index)
		{
			return ref *(double*)(data + index * 8);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void Write(byte* data, int index, double val)
		{
			*(double*)(data + index * 8) = val;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetElementWordCount()
		{
			return 2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetElementHashCode(double val)
		{
			return val.GetHashCode();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IElementReaderWriter<double> GetInstance()
		{
			bool flag = ElementReaderWriterDouble._instance == null;
			if (flag)
			{
				ElementReaderWriterDouble._instance = default(ElementReaderWriterDouble);
			}
			return ElementReaderWriterDouble._instance;
		}

		private static IElementReaderWriter<double> _instance;
	}
}
