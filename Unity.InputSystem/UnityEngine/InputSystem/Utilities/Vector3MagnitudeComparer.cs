using System;
using System.Collections.Generic;

namespace UnityEngine.InputSystem.Utilities
{
	public struct Vector3MagnitudeComparer : IComparer<Vector3>
	{
		public int Compare(Vector3 x, Vector3 y)
		{
			float sqrMagnitude = x.sqrMagnitude;
			float sqrMagnitude2 = y.sqrMagnitude;
			if (sqrMagnitude < sqrMagnitude2)
			{
				return -1;
			}
			if (sqrMagnitude > sqrMagnitude2)
			{
				return 1;
			}
			return 0;
		}
	}
}
