using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.Splines
{
	[Serializable]
	internal class SplinePathRef
	{
		public SplinePathRef()
		{
		}

		public SplinePathRef(IEnumerable<SplinePathRef.SliceRef> slices)
		{
			this.Splines = slices.ToArray<SplinePathRef.SliceRef>();
		}

		[SerializeField]
		public SplinePathRef.SliceRef[] Splines;

		[Serializable]
		public class SliceRef
		{
			public SliceRef(int splineIndex, SplineRange range)
			{
				this.Index = splineIndex;
				this.Range = range;
			}

			[SerializeField]
			public int Index;

			[SerializeField]
			public SplineRange Range;
		}
	}
}
