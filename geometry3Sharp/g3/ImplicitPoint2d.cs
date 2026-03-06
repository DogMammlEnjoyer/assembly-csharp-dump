using System;

namespace g3
{
	public class ImplicitPoint2d : ImplicitField2d
	{
		public ImplicitPoint2d(float x, float y)
		{
			this.m_vCenter = new Vector2f(x, y);
			this.m_radius = 1f;
		}

		public ImplicitPoint2d(float x, float y, float radius)
		{
			this.m_vCenter = new Vector2f(x, y);
			this.m_radius = radius;
		}

		public float Value(float fX, float fY)
		{
			float num = fX - this.m_vCenter.x;
			float num2 = fY - this.m_vCenter.y;
			float num3 = num * num + num2 * num2;
			num3 /= this.m_radius * this.m_radius;
			num3 = 1f - num3;
			if (num3 < 0f)
			{
				return 0f;
			}
			return num3 * num3 * num3;
		}

		public AxisAlignedBox2f Bounds
		{
			get
			{
				return new AxisAlignedBox2f(this.LowX, this.LowY, this.HighX, this.HighY);
			}
		}

		public void Gradient(float fX, float fY, ref float fGX, ref float fGY)
		{
			float num = fX - this.m_vCenter.x;
			float num2 = fY - this.m_vCenter.y;
			float num3 = num * num + num2 * num2;
			float num4 = 1f - num3;
			if (num4 < 0f)
			{
				fGX = (fGY = 0f);
				return;
			}
			float num5 = (float)Math.Sqrt((double)num3);
			float num6 = -6f * num5 * num4 * num4;
			num6 /= num5;
			fGX = num * num6;
			fGY = num2 * num6;
		}

		public bool InBounds(float x, float y)
		{
			return x >= this.LowX && x <= this.HighX && x >= this.LowY && x <= this.HighY;
		}

		public float LowX
		{
			get
			{
				return this.m_vCenter.x - this.radius;
			}
		}

		public float LowY
		{
			get
			{
				return this.m_vCenter.y - this.radius;
			}
		}

		public float HighX
		{
			get
			{
				return this.m_vCenter.x + this.radius;
			}
		}

		public float HighY
		{
			get
			{
				return this.m_vCenter.y + this.radius;
			}
		}

		public float radius
		{
			get
			{
				return this.m_radius;
			}
			set
			{
				this.m_radius = value;
			}
		}

		public float x
		{
			get
			{
				return this.m_vCenter.x;
			}
			set
			{
				this.m_vCenter.x = value;
			}
		}

		public float y
		{
			get
			{
				return this.m_vCenter.y;
			}
			set
			{
				this.m_vCenter.y = value;
			}
		}

		public Vector2f Center
		{
			get
			{
				return this.m_vCenter;
			}
			set
			{
				this.m_vCenter = value;
			}
		}

		private Vector2f m_vCenter;

		private float m_radius;
	}
}
