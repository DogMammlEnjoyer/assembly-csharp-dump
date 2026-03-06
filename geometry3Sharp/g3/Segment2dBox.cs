using System;

namespace g3
{
	public class Segment2dBox
	{
		public Segment2dBox()
		{
		}

		public Segment2dBox(Segment2d seg)
		{
			this.Segment = seg;
		}

		public static implicit operator Segment2d(Segment2dBox box)
		{
			return box.Segment;
		}

		public Segment2d Segment;
	}
}
