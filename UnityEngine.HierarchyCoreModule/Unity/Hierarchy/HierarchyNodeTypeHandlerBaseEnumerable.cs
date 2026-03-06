using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Unity.Hierarchy
{
	public readonly struct HierarchyNodeTypeHandlerBaseEnumerable
	{
		internal HierarchyNodeTypeHandlerBaseEnumerable(Hierarchy hierarchy)
		{
			this.m_Hierarchy = hierarchy;
		}

		public HierarchyNodeTypeHandlerBaseEnumerable.Enumerator GetEnumerator()
		{
			return new HierarchyNodeTypeHandlerBaseEnumerable.Enumerator(this.m_Hierarchy);
		}

		private readonly Hierarchy m_Hierarchy;

		public struct Enumerator : IDisposable
		{
			internal Enumerator(Hierarchy hierarchy)
			{
				this.m_Handlers = MemoryPool<IntPtr>.Shared.Rent(hierarchy.GetNodeTypeHandlersBaseCount());
				this.m_Count = hierarchy.GetNodeTypeHandlersBaseSpan(this.m_Handlers.Memory.Span);
				this.m_Index = -1;
			}

			public void Dispose()
			{
				this.m_Handlers.Dispose();
			}

			public unsafe HierarchyNodeTypeHandlerBase Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return HierarchyNodeTypeHandlerBase.FromIntPtr(*this.m_Handlers.Memory.Span[this.m_Index]);
				}
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool MoveNext()
			{
				int num = this.m_Index + 1;
				this.m_Index = num;
				return num < this.m_Count;
			}

			private readonly IMemoryOwner<IntPtr> m_Handlers;

			private readonly int m_Count;

			private int m_Index;
		}
	}
}
