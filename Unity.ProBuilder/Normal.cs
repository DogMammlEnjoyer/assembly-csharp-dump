using System;

namespace UnityEngine.ProBuilder
{
	public struct Normal : IEquatable<Normal>
	{
		public Vector3 normal { readonly get; set; }

		public Vector4 tangent { readonly get; set; }

		public Vector3 bitangent { readonly get; set; }

		public override bool Equals(object obj)
		{
			return obj is Normal && this.Equals((Normal)obj);
		}

		public override int GetHashCode()
		{
			return (VectorHash.GetHashCode(this.normal) * 397 ^ VectorHash.GetHashCode(this.tangent)) * 397 ^ VectorHash.GetHashCode(this.bitangent);
		}

		public bool Equals(Normal other)
		{
			return this.normal.Approx3(other.normal, 0.0001f) && this.tangent.Approx3(other.tangent, 0.0001f) && this.bitangent.Approx3(other.bitangent, 0.0001f);
		}

		public static bool operator ==(Normal a, Normal b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Normal a, Normal b)
		{
			return !(a == b);
		}
	}
}
