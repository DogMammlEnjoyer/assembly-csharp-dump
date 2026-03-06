using System;
using System.Runtime.CompilerServices;

namespace Unity.Cinemachine
{
	[NullableContext(1)]
	[Nullable(0)]
	internal struct IntersectNode
	{
		public IntersectNode(Point64 pt, Active edge1, Active edge2)
		{
			this.pt = pt;
			this.edge1 = edge1;
			this.edge2 = edge2;
		}

		public readonly Point64 pt;

		public readonly Active edge1;

		public readonly Active edge2;
	}
}
