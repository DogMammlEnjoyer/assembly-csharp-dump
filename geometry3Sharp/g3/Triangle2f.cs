using System;

namespace g3
{
	public struct Triangle2f
	{
		public Triangle2f(Vector2f v0, Vector2f v1, Vector2f v2)
		{
			this.V0 = v0;
			this.V1 = v1;
			this.V2 = v2;
		}

		public Vector2f this[int key]
		{
			get
			{
				if (key == 0)
				{
					return this.V0;
				}
				if (key != 1)
				{
					return this.V2;
				}
				return this.V1;
			}
			set
			{
				if (key == 0)
				{
					this.V0 = value;
					return;
				}
				if (key == 1)
				{
					this.V1 = value;
					return;
				}
				this.V2 = value;
			}
		}

		public Vector2f PointAt(float bary0, float bary1, float bary2)
		{
			return bary0 * this.V0 + bary1 * this.V1 + bary2 * this.V2;
		}

		public Vector2f PointAt(Vector3f bary)
		{
			return bary.x * this.V0 + bary.y * this.V1 + bary.z * this.V2;
		}

		public Vector2f Centroid()
		{
			return this.PointAt(0.3f, 0.3f, 0.3f);
		}

		public Vector2f V0;

		public Vector2f V1;

		public Vector2f V2;
	}
}
