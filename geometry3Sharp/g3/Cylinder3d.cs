using System;

namespace g3
{
	public class Cylinder3d
	{
		public Cylinder3d(Line3d axis, double radius, double height)
		{
			this.Axis = axis;
			this.Radius = radius;
			this.Height = height;
		}

		public Cylinder3d(Vector3d center, Vector3d axis, double radius, double height)
		{
			this.Axis = new Line3d(center, axis);
			this.Radius = radius;
			this.Height = height;
		}

		public Cylinder3d(Frame3f frame, double radius, double height, int nNormalAxis = 1)
		{
			this.Axis = new Line3d(frame.Origin, frame.GetAxis(nNormalAxis));
			this.Radius = radius;
			this.Height = height;
		}

		public Cylinder3d(double radius, double height)
		{
			this.Axis = new Line3d(Vector3d.Zero, Vector3d.AxisY);
			this.Radius = radius;
			this.Height = height;
		}

		public double Circumference
		{
			get
			{
				return 6.283185307179586 * this.Radius;
			}
		}

		public double Diameter
		{
			get
			{
				return 2.0 * this.Radius;
			}
		}

		public double Volume
		{
			get
			{
				return 3.141592653589793 * this.Radius * this.Radius * this.Height;
			}
		}

		public Line3d Axis;

		public double Radius;

		public double Height;
	}
}
