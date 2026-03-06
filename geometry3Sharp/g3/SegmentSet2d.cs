using System;
using System.Collections.Generic;

namespace g3
{
	public class SegmentSet2d
	{
		public SegmentSet2d()
		{
			this.Segments = new List<Segment2d>();
		}

		public SegmentSet2d(GeneralPolygon2d poly)
		{
			this.Segments = new List<Segment2d>(poly.Outer.SegmentItr());
			foreach (Polygon2d polygon2d in poly.Holes)
			{
				this.Segments.AddRange(polygon2d.SegmentItr());
			}
		}

		public SegmentSet2d(List<GeneralPolygon2d> polys)
		{
			this.Segments = new List<Segment2d>();
			foreach (GeneralPolygon2d generalPolygon2d in polys)
			{
				this.Segments.AddRange(generalPolygon2d.Outer.SegmentItr());
				foreach (Polygon2d polygon2d in generalPolygon2d.Holes)
				{
					this.Segments.AddRange(polygon2d.SegmentItr());
				}
			}
		}

		public IntrSegment2Segment2 FindAnyIntersection(Segment2d seg, out int iSegment)
		{
			int count = this.Segments.Count;
			for (iSegment = 0; iSegment < count; iSegment++)
			{
				IntrSegment2Segment2 intrSegment2Segment = new IntrSegment2Segment2(seg, this.Segments[iSegment]);
				if (intrSegment2Segment.Find())
				{
					return intrSegment2Segment;
				}
			}
			return null;
		}

		public void FindAllIntersections(Segment2d seg, List<double> segmentTs, List<int> indices = null, List<IntrSegment2Segment2> tests = null, bool bOnlySimple = true)
		{
			int count = this.Segments.Count;
			for (int i = 0; i < count; i++)
			{
				IntrSegment2Segment2 intrSegment2Segment = new IntrSegment2Segment2(seg, this.Segments[i])
				{
					IntervalThreshold = 1E-08
				};
				if (intrSegment2Segment.Find() && (!bOnlySimple || intrSegment2Segment.IsSimpleIntersection))
				{
					if (tests != null)
					{
						tests.Add(intrSegment2Segment);
					}
					if (indices != null)
					{
						indices.Add(i);
					}
					if (segmentTs != null)
					{
						segmentTs.Add(intrSegment2Segment.Parameter0);
						if (!intrSegment2Segment.IsSimpleIntersection)
						{
							segmentTs.Add(intrSegment2Segment.Parameter1);
						}
					}
				}
			}
		}

		private List<Segment2d> Segments;
	}
}
