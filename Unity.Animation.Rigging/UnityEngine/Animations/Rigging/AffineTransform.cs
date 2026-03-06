using System;

namespace UnityEngine.Animations.Rigging
{
	[Serializable]
	public struct AffineTransform
	{
		public AffineTransform(Vector3 t, Quaternion r)
		{
			this.translation = t;
			this.rotation = r;
		}

		public void Set(Vector3 t, Quaternion r)
		{
			this.translation = t;
			this.rotation = r;
		}

		public Vector3 Transform(Vector3 p)
		{
			return this.rotation * p + this.translation;
		}

		public Vector3 InverseTransform(Vector3 p)
		{
			return Quaternion.Inverse(this.rotation) * (p - this.translation);
		}

		public AffineTransform Inverse()
		{
			Quaternion r = Quaternion.Inverse(this.rotation);
			return new AffineTransform(r * -this.translation, r);
		}

		public AffineTransform InverseMul(AffineTransform transform)
		{
			Quaternion lhs = Quaternion.Inverse(this.rotation);
			return new AffineTransform(lhs * (transform.translation - this.translation), lhs * transform.rotation);
		}

		public static Vector3 operator *(AffineTransform lhs, Vector3 rhs)
		{
			return lhs.rotation * rhs + lhs.translation;
		}

		public static AffineTransform operator *(AffineTransform lhs, AffineTransform rhs)
		{
			return new AffineTransform(lhs.Transform(rhs.translation), lhs.rotation * rhs.rotation);
		}

		public static AffineTransform operator *(Quaternion lhs, AffineTransform rhs)
		{
			return new AffineTransform(lhs * rhs.translation, lhs * rhs.rotation);
		}

		public static AffineTransform operator *(AffineTransform lhs, Quaternion rhs)
		{
			return new AffineTransform(lhs.translation, lhs.rotation * rhs);
		}

		public static AffineTransform identity { get; } = new AffineTransform(Vector3.zero, Quaternion.identity);

		public Vector3 translation;

		public Quaternion rotation;
	}
}
