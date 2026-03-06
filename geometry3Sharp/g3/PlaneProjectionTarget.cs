using System;

namespace g3
{
	public class PlaneProjectionTarget : IProjectionTarget
	{
		public Vector3d Project(Vector3d vPoint, int identifier = -1)
		{
			Vector3d v = vPoint - this.Origin;
			return this.Origin + (v - v.Dot(this.Normal) * this.Normal);
		}

		public Vector3d Origin;

		public Vector3d Normal;
	}
}
