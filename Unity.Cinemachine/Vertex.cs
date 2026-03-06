using System;
using System.Runtime.CompilerServices;

namespace Unity.Cinemachine
{
	[NullableContext(2)]
	[Nullable(0)]
	internal class Vertex
	{
		public Vertex(Point64 pt, VertexFlags flags, Vertex prev)
		{
			this.pt = pt;
			this.flags = flags;
			this.next = null;
			this.prev = prev;
		}

		public readonly Point64 pt;

		public Vertex next;

		public Vertex prev;

		public VertexFlags flags;
	}
}
