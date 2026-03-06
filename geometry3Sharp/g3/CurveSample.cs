using System;

namespace g3
{
	public struct CurveSample
	{
		public CurveSample(Vector3d p, Vector3d t)
		{
			this.position = p;
			this.tangent = t;
		}

		public Vector3d position;

		public Vector3d tangent;
	}
}
