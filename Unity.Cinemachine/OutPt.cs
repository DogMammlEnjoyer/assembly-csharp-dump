using System;
using System.Runtime.CompilerServices;

namespace Unity.Cinemachine
{
	[NullableContext(1)]
	[Nullable(0)]
	internal class OutPt
	{
		public OutPt(Point64 pt, OutRec outrec)
		{
			this.pt = pt;
			this.outrec = outrec;
			this.next = this;
			this.prev = this;
			this.joiner = null;
		}

		public Point64 pt;

		[Nullable(2)]
		public OutPt next;

		public OutPt prev;

		public OutRec outrec;

		[Nullable(2)]
		public Joiner joiner;
	}
}
