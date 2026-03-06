using System;

namespace Unity.Cinemachine
{
	internal struct Rect64
	{
		public Rect64(long l, long t, long r, long b)
		{
			this.left = l;
			this.top = t;
			this.right = r;
			this.bottom = b;
		}

		public Rect64(Rect64 rec)
		{
			this.left = rec.left;
			this.top = rec.top;
			this.right = rec.right;
			this.bottom = rec.bottom;
		}

		public long Width
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

		public long Height
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

		public Point64 MidPoint()
		{
			return new Point64((this.left + this.right) / 2L, (this.top + this.bottom) / 2L);
		}

		public bool Contains(Point64 pt)
		{
			return pt.X > this.left && pt.X < this.right && pt.Y > this.top && pt.Y < this.bottom;
		}

		public bool Contains(Rect64 rec)
		{
			return rec.left >= this.left && rec.right <= this.right && rec.top >= this.top && rec.bottom <= this.bottom;
		}

		public long left;

		public long top;

		public long right;

		public long bottom;
	}
}
