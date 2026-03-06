using System;
using System.Collections.Generic;
using Meta.XR.Util;

namespace Meta.XR.MRUtilityKit.SceneDecorator
{
	[Feature(Feature.Scene)]
	public abstract class Pool<T> where T : class
	{
		protected abstract int CountAll { get; }

		protected abstract int CountActive { get; }

		public virtual int CountInactive
		{
			get
			{
				return this.CountAll - this.CountActive;
			}
		}

		public abstract T Get();

		public abstract void Release(T t);

		protected void Swap(int i0, int i1)
		{
			this.indices[this.pool[i0].t] = i1;
			this.indices[this.pool[i1].t] = i0;
			ref Pool<T>.Entry ptr = ref this.pool[i0];
			Pool<T>.Entry[] array = this.pool;
			Pool<T>.Entry entry = this.pool[i1];
			Pool<T>.Entry entry2 = this.pool[i0];
			ptr = entry;
			array[i1] = entry2;
		}

		protected Pool<T>.Entry[] pool;

		protected Dictionary<T, int> indices;

		protected int index;

		public Pool<T>.Callbacks callbacks;

		protected struct Entry
		{
			public bool active;

			public T t;
		}

		public struct Callbacks
		{
			public Func<T, T> Create;

			public Action<T> OnGet;

			public Action<T> OnRelease;
		}
	}
}
