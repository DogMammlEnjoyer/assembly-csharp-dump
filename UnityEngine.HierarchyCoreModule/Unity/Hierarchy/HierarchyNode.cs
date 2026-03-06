using System;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace Unity.Hierarchy
{
	[NativeHeader("Modules/HierarchyCore/Public/HierarchyNode.h")]
	public readonly struct HierarchyNode : IEquatable<HierarchyNode>
	{
		public static ref readonly HierarchyNode Null
		{
			get
			{
				return ref HierarchyNode.s_Null;
			}
		}

		public int Id
		{
			get
			{
				return this.m_Id;
			}
		}

		public int Version
		{
			get
			{
				return this.m_Version;
			}
		}

		public HierarchyNode()
		{
			this.m_Id = 0;
			this.m_Version = 0;
		}

		[ExcludeFromDocs]
		public static bool operator ==(in HierarchyNode lhs, in HierarchyNode rhs)
		{
			return lhs.Id == rhs.Id && lhs.Version == rhs.Version;
		}

		[ExcludeFromDocs]
		public static bool operator !=(in HierarchyNode lhs, in HierarchyNode rhs)
		{
			return !(lhs == rhs);
		}

		[ExcludeFromDocs]
		public bool Equals(HierarchyNode other)
		{
			return other.Id == this.Id && other.Version == this.Version;
		}

		[ExcludeFromDocs]
		public override string ToString()
		{
			return "HierarchyNode(" + ((this == HierarchyNode.Null) ? "Null" : string.Format("{0}:{1}", this.Id, this.Version)) + ")";
		}

		[ExcludeFromDocs]
		public override bool Equals(object obj)
		{
			bool result;
			if (obj is HierarchyNode)
			{
				HierarchyNode other = (HierarchyNode)obj;
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
			return HashCode.Combine<int, int>(this.Id, this.Version);
		}

		private const int k_HierarchyNodeIdNull = 0;

		private const int k_HierarchyNodeVersionNull = 0;

		private static readonly HierarchyNode s_Null;

		private readonly int m_Id;

		private readonly int m_Version;
	}
}
