using System;
using System.Collections.Generic;
using Meta.XR.Util;

namespace Meta.XR.MRUtilityKit.SceneDecorator
{
	[Feature(Feature.Scene)]
	public class CircularPool<T> : Pool<T> where T : class
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
				return this.active;
			}
		}

		public CircularPool(T primitive, int size, Pool<T>.Callbacks callbacks)
		{
			this.pool = new Pool<T>.Entry[size];
			this.indices = new Dictionary<T, int>(size);
			this.index = 0;
			this.active = 0;
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
				this.index = 0;
			}
			Pool<T>.Entry entry = this.pool[this.index];
			if (entry.active && this.callbacks.OnRelease != null)
			{
				this.callbacks.OnRelease(entry.t);
			}
			else
			{
				this.pool[this.index].active = true;
				this.active++;
			}
			if (this.callbacks.OnGet != null)
			{
				this.callbacks.OnGet(entry.t);
			}
			this.index++;
			return entry.t;
		}

		public override void Release(T t)
		{
			int num = this.indices[t];
			if (this.pool[num].active)
			{
				this.pool[num].active = false;
				this.active--;
				this.index--;
				if (this.index < 0)
				{
					this.index = this.pool.Length - 1;
				}
				base.Swap(num, this.index);
				if (this.callbacks.OnRelease != null)
				{
					this.callbacks.OnRelease(t);
				}
			}
		}

		private int active;
	}
}
