using System;

namespace g3
{
	public class PolyLine2DCurve : IParametricCurve2d
	{
		public bool IsClosed
		{
			get
			{
				return false;
			}
		}

		public double ParamLength
		{
			get
			{
				return (double)this.Polyline.VertexCount;
			}
		}

		public Vector2d SampleT(double t)
		{
			int num = (int)t;
			if (num >= this.Polyline.VertexCount - 1)
			{
				return this.Polyline[this.Polyline.VertexCount - 1];
			}
			Vector2d a = this.Polyline[num];
			Vector2d a2 = this.Polyline[num + 1];
			double num2 = t - (double)num;
			return (1.0 - num2) * a + num2 * a2;
		}

		public Vector2d TangentT(double t)
		{
			throw new NotImplementedException("Polygon2dCurve.TangentT");
		}

		public bool HasArcLength
		{
			get
			{
				return true;
			}
		}

		public double ArcLength
		{
			get
			{
				return this.Polyline.ArcLength;
			}
		}

		public Vector2d SampleArcLength(double a)
		{
			throw new NotImplementedException("Polygon2dCurve.SampleArcLength");
		}

		public void Reverse()
		{
			this.Polyline.Reverse();
		}

		public IParametricCurve2d Clone()
		{
			return new PolyLine2DCurve
			{
				Polyline = new PolyLine2d(this.Polyline)
			};
		}

		public bool IsTransformable
		{
			get
			{
				return true;
			}
		}

		public void Transform(ITransform2 xform)
		{
			this.Polyline.Transform(xform);
		}

		public PolyLine2d Polyline;
	}
}
