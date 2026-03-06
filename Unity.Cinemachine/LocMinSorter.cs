using System;
using System.Collections.Generic;

namespace Unity.Cinemachine
{
	internal struct LocMinSorter : IComparer<LocalMinima>
	{
		public int Compare(LocalMinima locMin1, LocalMinima locMin2)
		{
			return locMin2.vertex.pt.Y.CompareTo(locMin1.vertex.pt.Y);
		}
	}
}
