using System;

namespace UnityEngine.Splines
{
	[Serializable]
	public struct SplineKnotIndex : IEquatable<SplineKnotIndex>
	{
		public SplineKnotIndex(int spline, int knot)
		{
			this.Spline = spline;
			this.Knot = knot;
		}

		public static bool operator ==(SplineKnotIndex indexA, SplineKnotIndex indexB)
		{
			return indexA.Equals(indexB);
		}

		public static bool operator !=(SplineKnotIndex indexA, SplineKnotIndex indexB)
		{
			return !indexA.Equals(indexB);
		}

		public bool Equals(SplineKnotIndex otherIndex)
		{
			return this.Spline == otherIndex.Spline && this.Knot == otherIndex.Knot;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj is SplineKnotIndex)
			{
				SplineKnotIndex otherIndex = (SplineKnotIndex)obj;
				return this.Equals(otherIndex);
			}
			return false;
		}

		public bool IsValid()
		{
			return this.Spline >= 0 && this.Knot >= 0;
		}

		public override int GetHashCode()
		{
			return this.Spline * 397 ^ this.Knot;
		}

		public override string ToString()
		{
			return string.Format("{{{0}, {1}}}", this.Spline, this.Knot);
		}

		public static SplineKnotIndex Invalid = new SplineKnotIndex(-1, -1);

		public int Spline;

		public int Knot;
	}
}
