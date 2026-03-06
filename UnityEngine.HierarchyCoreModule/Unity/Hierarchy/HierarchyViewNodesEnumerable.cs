using System;
using System.Runtime.CompilerServices;

namespace Unity.Hierarchy
{
	public readonly struct HierarchyViewNodesEnumerable
	{
		internal HierarchyViewNodesEnumerable(HierarchyViewModel viewModel, HierarchyNodeFlags flags, HierarchyViewNodesEnumerable.Predicate predicate)
		{
			if (viewModel == null)
			{
				throw new ArgumentNullException("viewModel");
			}
			this.m_HierarchyViewModel = viewModel;
			if (predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			this.m_Predicate = predicate;
			this.m_Flags = flags;
		}

		public HierarchyViewNodesEnumerable.Enumerator GetEnumerator()
		{
			return new HierarchyViewNodesEnumerable.Enumerator(this);
		}

		private readonly HierarchyViewModel m_HierarchyViewModel;

		private readonly HierarchyViewNodesEnumerable.Predicate m_Predicate;

		private readonly HierarchyNodeFlags m_Flags;

		internal delegate bool Predicate(in HierarchyNode node, HierarchyNodeFlags flags);

		public struct Enumerator
		{
			internal Enumerator(HierarchyViewNodesEnumerable enumerable)
			{
				this.m_HierarchyFlattened = enumerable.m_HierarchyViewModel.HierarchyFlattened;
				this.m_Predicate = enumerable.m_Predicate;
				this.m_Flags = enumerable.m_Flags;
				this.m_NodesPtr = this.m_HierarchyFlattened.NodesPtr;
				this.m_NodesCount = this.m_HierarchyFlattened.Count;
				this.m_Version = this.m_HierarchyFlattened.Version;
				this.m_Index = 0;
			}

			public unsafe ref readonly HierarchyNode Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					this.ThrowIfVersionChanged();
					return HierarchyFlattenedNode.GetNodeByRef(this.m_NodesPtr[this.m_Index]);
				}
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public unsafe bool MoveNext()
			{
				this.ThrowIfVersionChanged();
				for (;;)
				{
					int num = this.m_Index + 1;
					this.m_Index = num;
					bool flag = num >= this.m_NodesCount;
					if (flag)
					{
						break;
					}
					bool flag2 = this.m_Predicate(HierarchyFlattenedNode.GetNodeByRef(this.m_NodesPtr[this.m_Index]), this.m_Flags);
					if (flag2)
					{
						goto Block_2;
					}
				}
				return false;
				Block_2:
				return true;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private void ThrowIfVersionChanged()
			{
				bool flag = this.m_Version != this.m_HierarchyFlattened.Version;
				if (flag)
				{
					throw new InvalidOperationException("HierarchyFlattened was modified.");
				}
			}

			private readonly HierarchyFlattened m_HierarchyFlattened;

			private readonly HierarchyViewNodesEnumerable.Predicate m_Predicate;

			private readonly HierarchyNodeFlags m_Flags;

			private unsafe readonly HierarchyFlattenedNode* m_NodesPtr;

			private readonly int m_NodesCount;

			private readonly int m_Version;

			private int m_Index;
		}
	}
}
