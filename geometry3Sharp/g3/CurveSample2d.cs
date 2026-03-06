using System;

namespace g3
{
	public struct CurveSample2d
	{
		public CurveSample2d(Vector2d p, Vector2d t)
		{
			this.position = p;
			this.tangent = t;
		}

		public Vector2d position;

		public Vector2d tangent;
	}
}
