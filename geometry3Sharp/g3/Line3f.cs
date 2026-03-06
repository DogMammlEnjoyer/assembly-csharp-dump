using System;

namespace g3
{
	public struct Line3f
	{
		public Line3f(Vector3f origin, Vector3f direction)
		{
			this.Origin = origin;
			this.Direction = direction;
		}

		public Vector3f PointAt(float d)
		{
			return this.Origin + d * this.Direction;
		}

		public float Project(Vector3f p)
		{
			return (p - this.Origin).Dot(this.Direction);
		}

		public float DistanceSquared(Vector3f p)
		{
			float f = (p - this.Origin).Dot(this.Direction);
			return (this.Origin + f * this.Direction - p).LengthSquared;
		}

		public Vector3f ClosestPoint(Vector3f p)
		{
			float f = (p - this.Origin).Dot(this.Direction);
			return this.Origin + f * this.Direction;
		}

		public Vector3f Origin;

		public Vector3f Direction;
	}
}
