using System;

namespace Pathfinding
{
	public class PathNode
	{
		public uint cost
		{
			get
			{
				return this.flags & 268435455U;
			}
			set
			{
				this.flags = ((this.flags & 4026531840U) | value);
			}
		}

		public bool flag1
		{
			get
			{
				return (this.flags & 268435456U) > 0U;
			}
			set
			{
				this.flags = ((this.flags & 4026531839U) | (value ? 268435456U : 0U));
			}
		}

		public bool flag2
		{
			get
			{
				return (this.flags & 536870912U) > 0U;
			}
			set
			{
				this.flags = ((this.flags & 3758096383U) | (value ? 536870912U : 0U));
			}
		}

		public uint G
		{
			get
			{
				return this.g;
			}
			set
			{
				this.g = value;
			}
		}

		public uint H
		{
			get
			{
				return this.h;
			}
			set
			{
				this.h = value;
			}
		}

		public uint F
		{
			get
			{
				return this.g + this.h;
			}
		}

		public void UpdateG(Path path)
		{
			this.g = this.parent.g + this.cost + path.GetTraversalCost(this.node);
		}

		public GraphNode node;

		public PathNode parent;

		public ushort pathID;

		public ushort heapIndex = ushort.MaxValue;

		private uint flags;

		private const uint CostMask = 268435455U;

		private const int Flag1Offset = 28;

		private const uint Flag1Mask = 268435456U;

		private const int Flag2Offset = 29;

		private const uint Flag2Mask = 536870912U;

		private uint g;

		private uint h;
	}
}
