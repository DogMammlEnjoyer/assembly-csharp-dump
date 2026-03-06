using System;

namespace Unity.Cinemachine
{
	internal struct RectD
	{
		public RectD(double l, double t, double r, double b)
		{
			this.left = l;
			this.top = t;
			this.right = r;
			this.bottom = b;
		}

		public RectD(RectD rec)
		{
			this.left = rec.left;
			this.top = rec.top;
			this.right = rec.right;
			this.bottom = rec.bottom;
		}

		public double Width
		{
			get
			{
				return this.right - this.left;
			}
			set
			{
				this.right = this.left + value;
			}
		}

		public double Height
		{
			get
			{
				return this.bottom - this.top;
			}
			set
			{
				this.bottom = this.top + value;
			}
		}

		public bool IsEmpty()
		{
			return this.bottom <= this.top || this.right <= this.left;
		}

		public PointD MidPoint()
		{
			return new PointD((this.left + this.right) / 2.0, (this.top + this.bottom) / 2.0);
		}

		public bool PtIsInside(PointD pt)
		{
			return pt.x > this.left && pt.x < this.right && pt.y > this.top && pt.y < this.bottom;
		}

		public double left;

		public double top;

		public double right;

		public double bottom;
	}
}
