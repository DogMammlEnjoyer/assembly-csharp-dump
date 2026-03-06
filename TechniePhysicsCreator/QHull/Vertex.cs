using System;

namespace Technie.PhysicsCreator.QHull
{
	public class Vertex
	{
		public Vertex()
		{
			this.pnt = new Point3d();
		}

		public Vertex(double x, double y, double z, int idx)
		{
			this.pnt = new Point3d(x, y, z);
			this.index = idx;
		}

		public Point3d pnt;

		public int index;

		public Vertex prev;

		public Vertex next;

		public Face face;
	}
}
