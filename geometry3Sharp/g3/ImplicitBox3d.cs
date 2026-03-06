using System;

namespace g3
{
	public class ImplicitBox3d : BoundedImplicitFunction3d, ImplicitFunction3d
	{
		public Box3d Box
		{
			get
			{
				return this.box;
			}
			set
			{
				this.box = value;
				this.local_aabb = new AxisAlignedBox3d(-this.Box.Extent.x, -this.Box.Extent.y, -this.Box.Extent.z, this.Box.Extent.x, this.Box.Extent.y, this.Box.Extent.z);
				this.bounds_aabb = this.box.ToAABB();
			}
		}

		public double Value(ref Vector3d pt)
		{
			double x = (pt - this.Box.Center).Dot(this.Box.AxisX);
			double y = (pt - this.Box.Center).Dot(this.Box.AxisY);
			double z = (pt - this.Box.Center).Dot(this.Box.AxisZ);
			return this.local_aabb.SignedDistance(new Vector3d(x, y, z));
		}

		public AxisAlignedBox3d Bounds()
		{
			return this.bounds_aabb;
		}

		private Box3d box;

		private AxisAlignedBox3d local_aabb;

		private AxisAlignedBox3d bounds_aabb;
	}
}
