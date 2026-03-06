using System;

namespace g3
{
	public class CircleProjectionTarget : IProjectionTarget
	{
		public Vector3d Project(Vector3d vPoint, int identifier = -1)
		{
			DistPoint3Circle3 distPoint3Circle = new DistPoint3Circle3(vPoint, this.Circle);
			distPoint3Circle.GetSquared();
			return distPoint3Circle.CircleClosest;
		}

		public Circle3d Circle;
	}
}
