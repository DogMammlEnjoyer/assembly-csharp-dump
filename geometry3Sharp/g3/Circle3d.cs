using System;

namespace g3
{
	public class Circle3d
	{
		public Circle3d(Vector3d center, double radius, Vector3d axis0, Vector3d axis1, Vector3d normal)
		{
			this.IsReversed = false;
			this.Center = center;
			this.Normal = normal;
			this.PlaneX = axis0;
			this.PlaneY = axis1;
			this.Radius = radius;
		}

		public Circle3d(Frame3f frame, double radius, int nNormalAxis = 1)
		{
			this.IsReversed = false;
			this.Center = frame.Origin;
			this.Normal = frame.GetAxis(nNormalAxis);
			this.PlaneX = frame.GetAxis((nNormalAxis + 1) % 3);
			this.PlaneY = frame.GetAxis((nNormalAxis + 2) % 3);
			this.Radius = radius;
		}

		public Circle3d(Vector3d center, double radius)
		{
			this.IsReversed = false;
			this.Center = center;
			this.Normal = Vector3d.AxisY;
			this.PlaneX = Vector3d.AxisX;
			this.PlaneY = Vector3d.AxisZ;
			this.Radius = radius;
		}

		public bool IsClosed
		{
			get
			{
				return true;
			}
		}

		public void Reverse()
		{
			this.IsReversed = !this.IsReversed;
		}

		public Vector3d SampleDeg(double degrees)
		{
			double num = degrees * 0.017453292519943295;
			double num2 = Math.Cos(num);
			double num3 = Math.Sin(num);
			return this.Center + num2 * this.Radius * this.PlaneX + num3 * this.Radius * this.PlaneY;
		}

		public Vector3d SampleRad(double radians)
		{
			double num = Math.Cos(radians);
			double num2 = Math.Sin(radians);
			return this.Center + num * this.Radius * this.PlaneX + num2 * this.Radius * this.PlaneY;
		}

		public double ParamLength
		{
			get
			{
				return 1.0;
			}
		}

		public Vector3d SampleT(double t)
		{
			double num = this.IsReversed ? (-t * 6.283185307179586) : (t * 6.283185307179586);
			double num2 = Math.Cos(num);
			double num3 = Math.Sin(num);
			return this.Center + num2 * this.Radius * this.PlaneX + num3 * this.Radius * this.PlaneY;
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
				return 6.283185307179586 * this.Radius;
			}
		}

		public Vector3d SampleArcLength(double a)
		{
			double num = a / this.ArcLength;
			double num2 = this.IsReversed ? (-num * 6.283185307179586) : (num * 6.283185307179586);
			double num3 = Math.Cos(num2);
			double num4 = Math.Sin(num2);
			return this.Center + num3 * this.Radius * this.PlaneX + num4 * this.Radius * this.PlaneY;
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

		public double Area
		{
			get
			{
				return 3.141592653589793 * this.Radius * this.Radius;
			}
		}

		public Vector3d Center;

		public Vector3d Normal;

		public Vector3d PlaneX;

		public Vector3d PlaneY;

		public double Radius;

		public bool IsReversed;
	}
}
