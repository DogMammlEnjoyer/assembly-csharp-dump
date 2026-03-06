using System;

namespace g3
{
	public class CylinderProjectionTarget : IProjectionTarget
	{
		public Vector3d Project(Vector3d vPoint, int identifer = -1)
		{
			DistPoint3Cylinder3 distPoint3Cylinder = new DistPoint3Cylinder3(vPoint, this.Cylinder);
			distPoint3Cylinder.GetSquared();
			return distPoint3Cylinder.CylinderClosest;
		}

		public Cylinder3d Cylinder;
	}
}
