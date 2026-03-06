using System;

namespace g3
{
	public abstract class CurveGenerator
	{
		public abstract void Generate();

		public void Make(DCurve3 c)
		{
			int count = this.vertices.Count;
			for (int i = 0; i < count; i++)
			{
				c.AppendVertex(this.vertices[i]);
			}
			c.Closed = this.closed;
		}

		public VectorArray3d vertices;

		public bool closed;
	}
}
