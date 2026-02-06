using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Fusion
{
	internal struct ElementReaderWriterVector2 : IElementReaderWriter<Vector2>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe Vector2 Read(byte* data, int index)
		{
			return *(Vector2*)(data + index * 8);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe ref Vector2 ReadRef(byte* data, int index)
		{
			return ref *(Vector2*)(data + index * 8);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void Write(byte* data, int index, Vector2 val)
		{
			*(Vector2*)(data + index * 8) = val;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetElementWordCount()
		{
			return 2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetElementHashCode(Vector2 val)
		{
			return val.GetHashCode();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IElementReaderWriter<Vector2> GetInstance()
		{
			bool flag = ElementReaderWriterVector2._instance == null;
			if (flag)
			{
				ElementReaderWriterVector2._instance = default(ElementReaderWriterVector2);
			}
			return ElementReaderWriterVector2._instance;
		}

		private static IElementReaderWriter<Vector2> _instance;
	}
}
