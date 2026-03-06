using System;
using JetBrains.Annotations;
using UnityEngine;

namespace MathGeoLib
{
	[PublicAPI]
	public struct Plane
	{
		public Plane(Vector3 normal, float distance)
		{
			this.Normal = normal;
			this.Distance = distance;
		}

		public override string ToString()
		{
			return string.Format("{0}: {1}, {2}: {3}", new object[]
			{
				"Normal",
				this.Normal,
				"Distance",
				this.Distance
			});
		}

		public readonly Vector3 Normal;

		public readonly float Distance;
	}
}
