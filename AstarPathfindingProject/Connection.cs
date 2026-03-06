using System;

namespace Pathfinding
{
	public struct Connection
	{
		public Connection(GraphNode node, uint cost, byte shapeEdge = 255)
		{
			this.node = node;
			this.cost = cost;
			this.shapeEdge = shapeEdge;
		}

		public override int GetHashCode()
		{
			return this.node.GetHashCode() ^ (int)this.cost;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			Connection connection = (Connection)obj;
			return connection.node == this.node && connection.cost == this.cost && connection.shapeEdge == this.shapeEdge;
		}

		public GraphNode node;

		public uint cost;

		public byte shapeEdge;

		public const byte NoSharedEdge = 255;
	}
}
