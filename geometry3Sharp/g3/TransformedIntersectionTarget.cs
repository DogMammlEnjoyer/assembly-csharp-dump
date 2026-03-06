using System;

namespace g3
{
	public class TransformedIntersectionTarget : IIntersectionTarget
	{
		public bool HasNormal
		{
			get
			{
				return this.BaseTarget.HasNormal;
			}
		}

		public bool RayIntersect(Ray3d ray, out Vector3d vHit, out Vector3d vHitNormal)
		{
			Ray3d ray2 = this.MapToBaseF(ray);
			if (this.BaseTarget.RayIntersect(ray2, out vHit, out vHitNormal))
			{
				vHit = this.MapFromBasePosF(vHit);
				vHitNormal = this.MapFromBasePosF(vHitNormal);
				return true;
			}
			return false;
		}

		private DMeshIntersectionTarget BaseTarget;

		public Func<Ray3d, Ray3d> MapToBaseF;

		public Func<Vector3d, Vector3d> MapFromBasePosF;

		public Func<Vector3d, Vector3d> MapFromBaseNormalF;
	}
}
