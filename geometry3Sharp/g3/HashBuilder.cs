using System;

namespace g3
{
	public struct HashBuilder
	{
		public HashBuilder(int init = -2128831035)
		{
			this.Hash = init;
		}

		public void Add(int i)
		{
			this.Hash = (this.Hash * 16777619 ^ i.GetHashCode());
		}

		public void Add(double d)
		{
			this.Hash = (this.Hash * 16777619 ^ d.GetHashCode());
		}

		public void Add(float f)
		{
			this.Hash = (this.Hash * 16777619 ^ f.GetHashCode());
		}

		public void Add(Vector2f v)
		{
			this.Hash = (this.Hash * 16777619 ^ v.x.GetHashCode());
			this.Hash = (this.Hash * 16777619 ^ v.y.GetHashCode());
		}

		public void Add(Vector2d v)
		{
			this.Hash = (this.Hash * 16777619 ^ v.x.GetHashCode());
			this.Hash = (this.Hash * 16777619 ^ v.y.GetHashCode());
		}

		public void Add(Vector3f v)
		{
			this.Hash = (this.Hash * 16777619 ^ v.x.GetHashCode());
			this.Hash = (this.Hash * 16777619 ^ v.y.GetHashCode());
			this.Hash = (this.Hash * 16777619 ^ v.z.GetHashCode());
		}

		public void Add(Vector3d v)
		{
			this.Hash = (this.Hash * 16777619 ^ v.x.GetHashCode());
			this.Hash = (this.Hash * 16777619 ^ v.y.GetHashCode());
			this.Hash = (this.Hash * 16777619 ^ v.z.GetHashCode());
		}

		public void Add(Frame3f f)
		{
			int num = this.Hash * 16777619;
			Vector3f origin = f.Origin;
			this.Hash = (num ^ origin.x.GetHashCode());
			int num2 = this.Hash * 16777619;
			origin = f.Origin;
			this.Hash = (num2 ^ origin.y.GetHashCode());
			int num3 = this.Hash * 16777619;
			origin = f.Origin;
			this.Hash = (num3 ^ origin.z.GetHashCode());
			int num4 = this.Hash * 16777619;
			Quaternionf rotation = f.Rotation;
			this.Hash = (num4 ^ rotation.x.GetHashCode());
			int num5 = this.Hash * 16777619;
			rotation = f.Rotation;
			this.Hash = (num5 ^ rotation.y.GetHashCode());
			int num6 = this.Hash * 16777619;
			rotation = f.Rotation;
			this.Hash = (num6 ^ rotation.z.GetHashCode());
			int num7 = this.Hash * 16777619;
			rotation = f.Rotation;
			this.Hash = (num7 ^ rotation.w.GetHashCode());
		}

		public void Add(Index3i v)
		{
			this.Hash = (this.Hash * 16777619 ^ v.a.GetHashCode());
			this.Hash = (this.Hash * 16777619 ^ v.b.GetHashCode());
			this.Hash = (this.Hash * 16777619 ^ v.c.GetHashCode());
		}

		public void Add(Index2i v)
		{
			this.Hash = (this.Hash * 16777619 ^ v.a.GetHashCode());
			this.Hash = (this.Hash * 16777619 ^ v.b.GetHashCode());
		}

		public int Hash;
	}
}
