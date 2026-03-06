using System;

namespace UnityEngine.Rendering.Universal.UTess
{
	internal sealed class ArraySliceDebugView<T> where T : struct
	{
		public ArraySliceDebugView(ArraySlice<T> slice)
		{
			this.m_Slice = slice;
		}

		public T[] Items
		{
			get
			{
				return this.m_Slice.ToArray();
			}
		}

		private ArraySlice<T> m_Slice;
	}
}
