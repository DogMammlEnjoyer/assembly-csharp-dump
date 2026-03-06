using System;

namespace g3
{
	public struct Line2f
	{
		public Line2f(Vector2f origin, Vector2f direction)
		{
			this.Origin = origin;
			this.Direction = direction;
		}

		public Vector2f PointAt(float d)
		{
			return this.Origin + d * this.Direction;
		}

		public float Project(Vector2f p)
		{
			return (p - this.Origin).Dot(this.Direction);
		}

		public float DistanceSquared(Vector2f p)
		{
			float f = (p - this.Origin).Dot(this.Direction);
			return (this.Origin + f * this.Direction - p).LengthSquared;
		}

		public Vector2f Origin;

		public Vector2f Direction;
	}
}
