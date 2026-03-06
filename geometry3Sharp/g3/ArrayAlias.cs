using System;

namespace g3
{
	public struct ArrayAlias<T>
	{
		public ArrayAlias(T[] source, int i)
		{
			this.Source = source;
			this.Index = i;
		}

		public T this[int i]
		{
			get
			{
				return this.Source[this.Index + i];
			}
			set
			{
				this.Source[this.Index + i] = value;
			}
		}

		public T[] Source;

		public int Index;
	}
}
