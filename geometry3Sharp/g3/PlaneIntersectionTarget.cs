using System;

namespace g3
{
	public class PlaneIntersectionTarget : IIntersectionTarget
	{
		public bool HasNormal
		{
			get
			{
				return true;
			}
		}

		public bool RayIntersect(Ray3d ray, out Vector3d vHit, out Vector3d vHitNormal)
		{
			Vector3f vector3f = this.PlaneFrame.RayPlaneIntersection((Vector3f)ray.Origin, (Vector3f)ray.Direction, this.NormalAxis);
			vHit = vector3f;
			vHitNormal = Vector3f.AxisY;
			return vector3f != Vector3f.Invalid;
		}

		public Frame3f PlaneFrame;

		public int NormalAxis = 2;
	}
}
