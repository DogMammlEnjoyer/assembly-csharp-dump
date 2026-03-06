using System;

namespace g3
{
	public class DCurveProjectionTarget : IProjectionTarget
	{
		public DCurveProjectionTarget(DCurve3 curve)
		{
			this.Curve = curve;
		}

		public Vector3d Project(Vector3d vPoint, int identifier = -1)
		{
			Vector3d result = Vector3d.Zero;
			double num = double.MaxValue;
			int vertexCount = this.Curve.VertexCount;
			int num2 = this.Curve.Closed ? vertexCount : (vertexCount - 1);
			for (int i = 0; i < num2; i++)
			{
				Segment3d segment3d = new Segment3d(this.Curve[i], this.Curve[(i + 1) % vertexCount]);
				Vector3d vector3d = segment3d.NearestPoint(vPoint);
				double num3 = vector3d.DistanceSquared(vPoint);
				if (num3 < num)
				{
					num = num3;
					result = vector3d;
				}
			}
			if (num >= 1.7976931348623157E+308)
			{
				return vPoint;
			}
			return result;
		}

		public DCurve3 Curve;
	}
}
