using System;

namespace g3
{
	public class Polygon2DCurve : IParametricCurve2d
	{
		public bool IsClosed
		{
			get
			{
				return true;
			}
		}

		public double ParamLength
		{
			get
			{
				return (double)this.Polygon.VertexCount;
			}
		}

		public Vector2d SampleT(double t)
		{
			int num = (int)t;
			if (num >= this.Polygon.VertexCount - 1)
			{
				return this.Polygon[this.Polygon.VertexCount - 1];
			}
			Vector2d a = this.Polygon[num];
			Vector2d a2 = this.Polygon[num + 1];
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
				return this.Polygon.ArcLength;
			}
		}

		public Vector2d SampleArcLength(double a)
		{
			throw new NotImplementedException("Polygon2dCurve.SampleArcLength");
		}

		public void Reverse()
		{
			this.Polygon.Reverse();
		}

		public IParametricCurve2d Clone()
		{
			return new Polygon2DCurve
			{
				Polygon = new Polygon2d(this.Polygon)
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
			this.Polygon.Transform(xform);
		}

		public Polygon2d Polygon;
	}
}
