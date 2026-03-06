using System;

namespace Unity.Collections
{
	internal sealed class NativeArrayReadOnlyDebugView<T> where T : struct
	{
		public NativeArrayReadOnlyDebugView(NativeArray<T>.ReadOnly array)
		{
			this.m_Array = array;
		}

		public T[] Items
		{
			get
			{
				bool flag = !this.m_Array.IsCreated;
				T[] result;
				if (flag)
				{
					result = null;
				}
				else
				{
					result = this.m_Array.ToArray();
				}
				return result;
			}
		}

		private NativeArray<T>.ReadOnly m_Array;
	}
}
