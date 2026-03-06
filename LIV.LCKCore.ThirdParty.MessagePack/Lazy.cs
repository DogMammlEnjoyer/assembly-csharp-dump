using System;

namespace SouthPointe.Serialization.MessagePack
{
	public sealed class Lazy<T>
	{
		public T Value
		{
			get
			{
				if (!this.isValueCreated)
				{
					object obj = this.padlock;
					lock (obj)
					{
						if (!this.isValueCreated)
						{
							this.value = this.createValue();
							this.isValueCreated = true;
						}
					}
				}
				return this.value;
			}
		}

		public bool IsValueCreated
		{
			get
			{
				object obj = this.padlock;
				bool result;
				lock (obj)
				{
					result = this.isValueCreated;
				}
				return result;
			}
		}

		public Lazy(Func<T> createValue)
		{
			if (createValue == null)
			{
				throw new ArgumentNullException("createValue");
			}
			this.createValue = createValue;
		}

		public override string ToString()
		{
			T t = this.Value;
			return t.ToString();
		}

		private readonly object padlock = new object();

		private readonly Func<T> createValue;

		private bool isValueCreated;

		private T value;
	}
}
