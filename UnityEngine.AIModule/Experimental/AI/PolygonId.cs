using System;

namespace UnityEngine.Experimental.AI
{
	[Obsolete("The experimental PolygonId struct has been deprecated without replacement.")]
	public struct PolygonId : IEquatable<PolygonId>
	{
		public bool IsNull()
		{
			return this.polyRef == 0UL;
		}

		public static bool operator ==(PolygonId x, PolygonId y)
		{
			return x.polyRef == y.polyRef;
		}

		public static bool operator !=(PolygonId x, PolygonId y)
		{
			return x.polyRef != y.polyRef;
		}

		public override int GetHashCode()
		{
			return this.polyRef.GetHashCode();
		}

		public bool Equals(PolygonId rhs)
		{
			return rhs == this;
		}

		public override bool Equals(object obj)
		{
			bool flag = obj == null || !(obj is PolygonId);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				PolygonId x = (PolygonId)obj;
				result = (x == this);
			}
			return result;
		}

		internal ulong polyRef;
	}
}
