using System;

namespace g3
{
	public class Hexagon2d
	{
		public Hexagon2d(Vector2d center, double radius, Hexagon2d.TopModes mode = Hexagon2d.TopModes.Flat)
		{
			this.Center = center;
			this.Radius = radius;
			this.TopMode = mode;
		}

		public bool IsClosed
		{
			get
			{
				return true;
			}
		}

		public Hexagon2d Clone()
		{
			return new Hexagon2d(this.Center, this.Radius, this.TopMode);
		}

		public double InnerRadius
		{
			get
			{
				return 1.7320508075688772 * this.Radius / 2.0;
			}
			set
			{
				this.Radius = 2.0 * value / 1.7320508075688772;
			}
		}

		public Vector2d Corner(int i)
		{
			double num = 60.0 * (double)i;
			if (this.TopMode == Hexagon2d.TopModes.Tip)
			{
				num += 30.0;
			}
			double num2 = num * 0.017453292519943295;
			return new Vector2d(this.Center.x + this.Radius * Math.Cos(num2), this.Center.y + this.Radius * Math.Sin(num2));
		}

		public double Width
		{
			get
			{
				if (this.TopMode != Hexagon2d.TopModes.Flat)
				{
					return 0.8660254037844386 * this.Height;
				}
				return this.Radius * 2.0;
			}
		}

		public double Height
		{
			get
			{
				if (this.TopMode != Hexagon2d.TopModes.Flat)
				{
					return this.Radius * 2.0;
				}
				return 0.8660254037844386 * this.Width;
			}
		}

		public double VertSpacing
		{
			get
			{
				if (this.TopMode != Hexagon2d.TopModes.Flat)
				{
					return this.Height * 3.0 / 4.0;
				}
				return this.Height;
			}
		}

		public double HorzSpacing
		{
			get
			{
				if (this.TopMode != Hexagon2d.TopModes.Flat)
				{
					return this.Width;
				}
				return this.Width * 3.0 / 4.0;
			}
		}

		public AxisAlignedBox2d Bounds
		{
			get
			{
				return new AxisAlignedBox2d(this.Center, this.Width / 2.0, this.Height / 2.0);
			}
		}

		public Vector2d Center;

		public double Radius;

		public Hexagon2d.TopModes TopMode;

		public enum TopModes
		{
			Flat,
			Tip
		}
	}
}
