using System;
using JetBrains.Annotations;
using UnityEngine;

namespace MathGeoLib
{
	[PublicAPI]
	public struct Line3
	{
		public Line3(Vector3 point1, Vector3 point2)
		{
			this.Point1 = point1;
			this.Point2 = point2;
		}

		public override string ToString()
		{
			return string.Format("{0}: {1}, {2}: {3}", new object[]
			{
				"Point1",
				this.Point1,
				"Point2",
				this.Point2
			});
		}

		public readonly Vector3 Point1;

		public readonly Vector3 Point2;
	}
}
