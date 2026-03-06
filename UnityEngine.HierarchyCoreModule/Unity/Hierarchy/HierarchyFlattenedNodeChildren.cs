using System;
using System.Runtime.CompilerServices;

namespace Unity.Hierarchy
{
	public readonly struct HierarchyFlattenedNodeChildren
	{
		internal HierarchyFlattenedNodeChildren(HierarchyFlattened hierarchyFlattened, in HierarchyNode node)
		{
			bool flag = hierarchyFlattened == null;
			if (flag)
			{
				throw new ArgumentNullException("hierarchyFlattened");
			}
			bool flag2 = node == HierarchyNode.Null;
			if (flag2)
			{
				throw new ArgumentNullException("node");
			}
			bool flag3 = !hierarchyFlattened.Contains(node);
			if (flag3)
			{
				throw new InvalidOperationException(string.Format("node {0}:{1} not found", node.Id, node.Version));
			}
			this.m_HierarchyFlattened = hierarchyFlattened;
			this.m_Node = node;
			this.m_Version = hierarchyFlattened.Version;
			this.m_Count = this.m_HierarchyFlattened.GetChildrenCount(this.m_Node);
		}

		public int Count
		{
			get
			{
				this.ThrowIfVersionChanged();
				return this.m_Count;
			}
		}

		public HierarchyFlattenedNode this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				bool flag = index < 0 || index >= this.m_Count;
				if (flag)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				this.ThrowIfVersionChanged();
				return this.m_HierarchyFlattened[index];
			}
		}

		public HierarchyFlattenedNodeChildren.Enumerator GetEnumerator()
		{
			return new HierarchyFlattenedNodeChildren.Enumerator(this, this.m_Node);
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

		private readonly HierarchyNode m_Node;

		private readonly int m_Version;

		private readonly int m_Count;

		public struct Enumerator
		{
			internal Enumerator(HierarchyFlattenedNodeChildren enumerable, HierarchyNode node)
			{
				this.m_Enumerable = enumerable;
				this.m_HierarchyFlattened = enumerable.m_HierarchyFlattened;
				this.m_Node = node;
				this.m_CurrentIndex = -1;
				this.m_ChildrenIndex = 0;
				this.m_ChildrenCount = 0;
			}

			public ref readonly HierarchyNode Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					this.m_Enumerable.ThrowIfVersionChanged();
					return HierarchyFlattenedNode.GetNodeByRef(this.m_HierarchyFlattened[this.m_CurrentIndex]);
				}
			}

			public bool MoveNext()
			{
				this.m_Enumerable.ThrowIfVersionChanged();
				bool flag = this.m_CurrentIndex == -1;
				bool result;
				if (flag)
				{
					int num = this.m_HierarchyFlattened.IndexOf(this.m_Node);
					bool flag2 = num == -1;
					if (flag2)
					{
						result = false;
					}
					else
					{
						ref readonly HierarchyFlattenedNode ptr = ref this.m_HierarchyFlattened[num];
						bool flag3 = ptr == HierarchyFlattenedNode.Null || ptr.ChildrenCount <= 0;
						if (flag3)
						{
							result = false;
						}
						else
						{
							bool flag4 = num + 1 >= this.m_HierarchyFlattened.Count;
							if (flag4)
							{
								result = false;
							}
							else
							{
								this.m_CurrentIndex = num + 1;
								this.m_ChildrenIndex = 0;
								this.m_ChildrenCount = ptr.ChildrenCount;
								result = true;
							}
						}
					}
				}
				else
				{
					ref readonly HierarchyFlattenedNode ptr2 = ref this.m_HierarchyFlattened[this.m_CurrentIndex];
					bool flag5 = this.m_ChildrenIndex + 1 >= this.m_ChildrenCount || ptr2.NextSiblingOffset <= 0;
					if (flag5)
					{
						result = false;
					}
					else
					{
						this.m_CurrentIndex += ptr2.NextSiblingOffset;
						this.m_ChildrenIndex++;
						result = true;
					}
				}
				return result;
			}

			private readonly HierarchyFlattenedNodeChildren m_Enumerable;

			private readonly HierarchyFlattened m_HierarchyFlattened;

			private readonly HierarchyNode m_Node;

			private int m_CurrentIndex;

			private int m_ChildrenIndex;

			private int m_ChildrenCount;
		}
	}
}
