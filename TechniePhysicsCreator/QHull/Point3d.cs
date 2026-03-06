using System;

namespace Technie.PhysicsCreator.QHull
{
	public class Point3d : Vector3d
	{
		public Point3d()
		{
		}

		public Point3d(Vector3d v)
		{
			base.set(v);
		}

		public Point3d(double x, double y, double z)
		{
			base.set(x, y, z);
		}
	}
}
