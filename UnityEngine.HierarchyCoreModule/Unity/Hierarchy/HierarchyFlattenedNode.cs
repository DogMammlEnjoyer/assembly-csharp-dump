using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace Unity.Hierarchy
{
	[NativeHeader("Modules/HierarchyCore/Public/HierarchyFlattenedNode.h")]
	public readonly struct HierarchyFlattenedNode : IEquatable<HierarchyFlattenedNode>
	{
		public static ref readonly HierarchyFlattenedNode Null
		{
			get
			{
				return ref HierarchyFlattenedNode.s_Null;
			}
		}

		public HierarchyNode Node
		{
			get
			{
				return this.m_Node;
			}
		}

		public HierarchyNodeType Type
		{
			get
			{
				return this.m_Type;
			}
		}

		public int ParentOffset
		{
			get
			{
				return this.m_ParentOffset;
			}
		}

		public int NextSiblingOffset
		{
			get
			{
				return this.m_NextSiblingOffset;
			}
		}

		public int ChildrenCount
		{
			get
			{
				return this.m_ChildrenCount;
			}
		}

		public int Depth
		{
			get
			{
				return this.m_Depth;
			}
		}

		public unsafe HierarchyFlattenedNode()
		{
			this.m_Node = *HierarchyNode.Null;
			this.m_Type = *HierarchyNodeType.Null;
			this.m_ParentOffset = 0;
			this.m_NextSiblingOffset = 0;
			this.m_ChildrenCount = 0;
			this.m_Depth = 0;
		}

		[ExcludeFromDocs]
		public static bool operator ==(in HierarchyFlattenedNode lhs, in HierarchyFlattenedNode rhs)
		{
			HierarchyNode node = lhs.Node;
			HierarchyNode node2 = rhs.Node;
			return node == node2;
		}

		[ExcludeFromDocs]
		public static bool operator !=(in HierarchyFlattenedNode lhs, in HierarchyFlattenedNode rhs)
		{
			return !(lhs == rhs);
		}

		[ExcludeFromDocs]
		public bool Equals(HierarchyFlattenedNode other)
		{
			HierarchyNode node = other.Node;
			HierarchyNode node2 = this.Node;
			return node == node2;
		}

		[ExcludeFromDocs]
		public override string ToString()
		{
			return "HierarchyFlattenedNode(" + ((this == HierarchyFlattenedNode.Null) ? "Null" : string.Format("{0}:{1}", this.Node.Id, this.Node.Version)) + ")";
		}

		[ExcludeFromDocs]
		public override bool Equals(object obj)
		{
			bool result;
			if (obj is HierarchyFlattenedNode)
			{
				HierarchyFlattenedNode other = (HierarchyFlattenedNode)obj;
				result = this.Equals(other);
			}
			else
			{
				result = false;
			}
			return result;
		}

		[ExcludeFromDocs]
		public override int GetHashCode()
		{
			return this.Node.GetHashCode();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static ref readonly HierarchyNode GetNodeByRef(in HierarchyFlattenedNode hierarchyFlattenedNode)
		{
			return ref hierarchyFlattenedNode.m_Node;
		}

		private static readonly HierarchyFlattenedNode s_Null;

		private readonly HierarchyNode m_Node;

		private readonly HierarchyNodeType m_Type;

		private readonly int m_ParentOffset;

		private readonly int m_NextSiblingOffset;

		private readonly int m_ChildrenCount;

		private readonly int m_Depth;
	}
}
