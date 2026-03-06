using System;
using System.Runtime.CompilerServices;

namespace Unity.Collections
{
	internal sealed class FixedList128BytesDebugView<[IsUnmanaged] T> where T : struct, ValueType
	{
		public FixedList128BytesDebugView(FixedList128Bytes<T> list)
		{
			this.m_List = list;
		}

		public T[] Items
		{
			get
			{
				return this.m_List.ToArray();
			}
		}

		private FixedList128Bytes<T> m_List;
	}
}
