using System;
using System.Collections.Generic;

namespace UnityEngine.InputSystem.Utilities
{
	public struct Vector2MagnitudeComparer : IComparer<Vector2>
	{
		public int Compare(Vector2 x, Vector2 y)
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
