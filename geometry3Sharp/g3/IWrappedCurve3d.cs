using System;
using System.Collections.Generic;

namespace g3
{
	public class IWrappedCurve3d : ISampledCurve3d
	{
		public bool Closed { get; set; }

		public int VertexCount
		{
			get
			{
				if (this.VertexList != null)
				{
					return this.VertexList.Count;
				}
				return 0;
			}
		}

		public int SegmentCount
		{
			get
			{
				if (!this.Closed)
				{
					return this.VertexCount - 1;
				}
				return this.VertexCount;
			}
		}

		public Vector3d GetVertex(int i)
		{
			return this.VertexList[i];
		}

		public Segment3d GetSegment(int iSegment)
		{
			if (!this.Closed)
			{
				return new Segment3d(this.VertexList[iSegment], this.VertexList[iSegment + 1]);
			}
			return new Segment3d(this.VertexList[iSegment], this.VertexList[(iSegment + 1) % this.VertexList.Count]);
		}

		public IEnumerable<Vector3d> Vertices
		{
			get
			{
				return this.VertexList;
			}
		}

		public IList<Vector3d> VertexList;
	}
}
