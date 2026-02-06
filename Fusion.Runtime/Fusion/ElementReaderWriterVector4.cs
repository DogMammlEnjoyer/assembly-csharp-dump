using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Fusion
{
	internal struct ElementReaderWriterVector4 : IElementReaderWriter<Vector4>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe Vector4 Read(byte* data, int index)
		{
			return *(Vector4*)(data + index * 16);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe ref Vector4 ReadRef(byte* data, int index)
		{
			return ref *(Vector4*)(data + index * 16);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void Write(byte* data, int index, Vector4 val)
		{
			*(Vector4*)(data + index * 16) = val;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetElementWordCount()
		{
			return 4;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetElementHashCode(Vector4 val)
		{
			return val.GetHashCode();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IElementReaderWriter<Vector4> GetInstance()
		{
			bool flag = ElementReaderWriterVector4._instance == null;
			if (flag)
			{
				ElementReaderWriterVector4._instance = default(ElementReaderWriterVector4);
			}
			return ElementReaderWriterVector4._instance;
		}

		private static IElementReaderWriter<Vector4> _instance;
	}
}
