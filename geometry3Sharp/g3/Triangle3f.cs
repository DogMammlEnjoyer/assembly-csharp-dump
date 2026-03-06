using System;

namespace g3
{
	public struct Triangle3f
	{
		public Triangle3f(Vector3f v0, Vector3f v1, Vector3f v2)
		{
			this.V0 = v0;
			this.V1 = v1;
			this.V2 = v2;
		}

		public Vector3f this[int key]
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

		public Vector3f PointAt(float bary0, float bary1, float bary2)
		{
			return bary0 * this.V0 + bary1 * this.V1 + bary2 * this.V2;
		}

		public Vector3f PointAt(Vector3f bary)
		{
			return bary.x * this.V0 + bary.y * this.V1 + bary.z * this.V2;
		}

		public Vector3f BarycentricCoords(Vector3f point)
		{
			return (Vector3f)MathUtil.BarycentricCoords(point, this.V0, this.V1, this.V2);
		}

		public Vector3f V0;

		public Vector3f V1;

		public Vector3f V2;
	}
}
