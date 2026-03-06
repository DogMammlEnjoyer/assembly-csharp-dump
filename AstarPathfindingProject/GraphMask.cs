using System;

namespace Pathfinding
{
	[Serializable]
	public struct GraphMask
	{
		public static GraphMask everything
		{
			get
			{
				return new GraphMask(-1);
			}
		}

		public GraphMask(int value)
		{
			this.value = value;
		}

		public static implicit operator int(GraphMask mask)
		{
			return mask.value;
		}

		public static implicit operator GraphMask(int mask)
		{
			return new GraphMask(mask);
		}

		public static GraphMask operator &(GraphMask lhs, GraphMask rhs)
		{
			return new GraphMask(lhs.value & rhs.value);
		}

		public static GraphMask operator |(GraphMask lhs, GraphMask rhs)
		{
			return new GraphMask(lhs.value | rhs.value);
		}

		public static GraphMask operator ~(GraphMask lhs)
		{
			return new GraphMask(~lhs.value);
		}

		public bool Contains(int graphIndex)
		{
			return (this.value >> graphIndex & 1) != 0;
		}

		public static GraphMask FromGraph(NavGraph graph)
		{
			return 1 << (int)graph.graphIndex;
		}

		public override string ToString()
		{
			return this.value.ToString();
		}

		public static GraphMask FromGraphName(string graphName)
		{
			NavGraph navGraph = AstarData.active.data.FindGraph((NavGraph g) => g.name == graphName);
			if (navGraph == null)
			{
				throw new ArgumentException("Could not find any graph with the name '" + graphName + "'");
			}
			return GraphMask.FromGraph(navGraph);
		}

		public int value;
	}
}
