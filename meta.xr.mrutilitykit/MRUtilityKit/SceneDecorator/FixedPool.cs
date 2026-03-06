using System;
using System.Collections.Generic;
using Meta.XR.Util;

namespace Meta.XR.MRUtilityKit.SceneDecorator
{
	[Feature(Feature.Scene)]
	public class FixedPool<T> : Pool<T> where T : class
	{
		protected override int CountAll
		{
			get
			{
				return this.pool.Length;
			}
		}

		protected override int CountActive
		{
			get
			{
				return this.index;
			}
		}

		public FixedPool(T primitive, int size, Pool<T>.Callbacks callbacks)
		{
			this.pool = new Pool<T>.Entry[size];
			this.indices = new Dictionary<T, int>(size);
			this.index = 0;
			this.callbacks = callbacks;
			for (int i = 0; i < size; i++)
			{
				T t = callbacks.Create(primitive);
				this.pool[i].t = t;
				this.indices[t] = i;
			}
		}

		public override T Get()
		{
			if (this.index >= this.pool.Length)
			{
				return default(T);
			}
			T t = this.pool[this.index].t;
			this.pool[this.index].active = true;
			if (this.callbacks.OnGet != null)
			{
				this.callbacks.OnGet(t);
			}
			this.index++;
			return t;
		}

		public override void Release(T t)
		{
			int num = this.indices[t];
			if (!this.pool[num].active)
			{
				return;
			}
			this.pool[num].active = false;
			this.index--;
			base.Swap(num, this.index);
			if (this.callbacks.OnRelease != null)
			{
				this.callbacks.OnRelease(t);
			}
		}
	}
}
