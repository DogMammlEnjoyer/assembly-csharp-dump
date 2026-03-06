using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Unity.Cinemachine
{
	[NullableContext(2)]
	[Nullable(0)]
	internal class OutRec
	{
		public OutRec()
		{
			this.bounds = default(Rect64);
			this.path = new List<Point64>();
		}

		public int idx;

		public OutRec owner;

		[Nullable(new byte[]
		{
			2,
			1
		})]
		public List<OutRec> splits;

		public Active frontEdge;

		public Active backEdge;

		public OutPt pts;

		public PolyPathBase polypath;

		public Rect64 bounds;

		[Nullable(1)]
		public List<Point64> path;

		public bool isOpen;
	}
}
