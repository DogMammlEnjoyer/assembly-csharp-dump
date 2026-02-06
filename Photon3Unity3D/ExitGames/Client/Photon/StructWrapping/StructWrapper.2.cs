using System;

namespace ExitGames.Client.Photon.StructWrapping
{
	public class StructWrapper<T> : StructWrapper
	{
		public StructWrapperPool<T> ReturnPool { get; internal set; }

		public StructWrapper(Pooling releasing) : base(typeof(T), StructWrapperPool.GetWrappedType(typeof(T)))
		{
			this.pooling = releasing;
		}

		public StructWrapper(Pooling releasing, Type tType, WrappedType wType) : base(tType, wType)
		{
			this.pooling = releasing;
		}

		public StructWrapper<T> Poke(byte value)
		{
			bool flag = this.pooling == Pooling.Readonly;
			if (flag)
			{
				throw new InvalidOperationException("Trying to Poke the value of a readonly StructWrapper<byte>. Value cannot be modified.");
			}
			return this;
		}

		public StructWrapper<T> Poke(bool value)
		{
			bool flag = this.pooling == Pooling.Readonly;
			if (flag)
			{
				throw new InvalidOperationException("Trying to Poke the value of a readonly StructWrapper<bool>. Value cannot be modified.");
			}
			return this;
		}

		public StructWrapper<T> Poke(T value)
		{
			this.value = value;
			return this;
		}

		public T Unwrap()
		{
			T result = this.value;
			bool flag = this.pooling != Pooling.Readonly;
			if (flag)
			{
				this.ReturnPool.Release(this);
			}
			return result;
		}

		public T Peek()
		{
			return this.value;
		}

		public override object Box()
		{
			T t = this.value;
			bool flag = this.ReturnPool != null;
			if (flag)
			{
				this.ReturnPool.Release(this);
			}
			return t;
		}

		public override void Dispose()
		{
			bool flag = (this.pooling & Pooling.CheckedOut) == Pooling.CheckedOut;
			if (flag)
			{
				bool flag2 = this.ReturnPool != null;
				if (flag2)
				{
					this.ReturnPool.Release(this);
				}
			}
		}

		public override void DisconnectFromPool()
		{
			bool flag = this.pooling != Pooling.Readonly;
			if (flag)
			{
				this.pooling = Pooling.Disconnected;
				this.ReturnPool = null;
			}
		}

		public override string ToString()
		{
			T t = this.Unwrap();
			return t.ToString();
		}

		public override string ToString(bool writeTypeInfo)
		{
			string result;
			if (writeTypeInfo)
			{
				string format = "(StructWrapper<{0}>){1}";
				object arg = this.wrappedType;
				T t = this.Unwrap();
				result = string.Format(format, arg, t.ToString());
			}
			else
			{
				T t = this.Unwrap();
				result = t.ToString();
			}
			return result;
		}

		public static implicit operator StructWrapper<T>(T value)
		{
			return StructWrapper<T>.staticPool.Acquire(value);
		}

		internal Pooling pooling;

		internal T value;

		internal static StructWrapperPool<T> staticPool = new StructWrapperPool<T>(true);
	}
}
