using System;
using System.Collections.Generic;

namespace UnityEngine.Animations.Rigging
{
	public class AnimationJobCacheBuilder
	{
		public AnimationJobCacheBuilder()
		{
			this.m_Data = new List<float>();
		}

		public CacheIndex Add(float v)
		{
			this.m_Data.Add(v);
			return new CacheIndex
			{
				idx = this.m_Data.Count - 1
			};
		}

		public CacheIndex Add(Vector2 v)
		{
			this.m_Data.Add(v.x);
			this.m_Data.Add(v.y);
			return new CacheIndex
			{
				idx = this.m_Data.Count - 2
			};
		}

		public CacheIndex Add(Vector3 v)
		{
			this.m_Data.Add(v.x);
			this.m_Data.Add(v.y);
			this.m_Data.Add(v.z);
			return new CacheIndex
			{
				idx = this.m_Data.Count - 3
			};
		}

		public CacheIndex Add(Vector4 v)
		{
			this.m_Data.Add(v.x);
			this.m_Data.Add(v.y);
			this.m_Data.Add(v.z);
			this.m_Data.Add(v.w);
			return new CacheIndex
			{
				idx = this.m_Data.Count - 4
			};
		}

		public CacheIndex Add(Quaternion v)
		{
			return this.Add(new Vector4(v.x, v.y, v.z, v.w));
		}

		public CacheIndex Add(AffineTransform tx)
		{
			this.Add(tx.translation);
			this.Add(tx.rotation);
			return new CacheIndex
			{
				idx = this.m_Data.Count - 7
			};
		}

		public CacheIndex AllocateChunk(int size)
		{
			this.m_Data.AddRange(new float[size]);
			return new CacheIndex
			{
				idx = this.m_Data.Count - size
			};
		}

		public void SetValue(CacheIndex index, int offset, float value)
		{
			if (index.idx + offset < this.m_Data.Count)
			{
				this.m_Data[index.idx + offset] = value;
			}
		}

		public AnimationJobCache Build()
		{
			return new AnimationJobCache(this.m_Data.ToArray());
		}

		private List<float> m_Data;
	}
}
