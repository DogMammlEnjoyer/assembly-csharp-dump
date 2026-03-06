using System;
using System.Collections.Generic;

namespace UnityEngine.Splines
{
	public class SplinePath : SplinePath<SplineSlice<Spline>>
	{
		public SplinePath(IEnumerable<SplineSlice<Spline>> slices) : base(slices)
		{
		}
	}
}
