using System;
using UnityEngine;

namespace ExitGames.Client.Photon.StructWrapping
{
	public abstract class StructWrapper : IDisposable
	{
		public StructWrapper(Type ttype, WrappedType wrappedType)
		{
			this.ttype = ttype;
			this.wrappedType = wrappedType;
		}

		public abstract object Box();

		public abstract void DisconnectFromPool();

		public abstract void Dispose();

		public abstract string ToString(bool writeType);

		public static implicit operator StructWrapper(bool value)
		{
			return value.Wrap();
		}

		public static implicit operator StructWrapper(byte value)
		{
			return value.Wrap();
		}

		public static implicit operator StructWrapper(float value)
		{
			return value.Wrap<float>();
		}

		public static implicit operator StructWrapper(double value)
		{
			return value.Wrap<double>();
		}

		public static implicit operator StructWrapper(short value)
		{
			return value.Wrap<short>();
		}

		public static implicit operator StructWrapper(int value)
		{
			return value.Wrap<int>();
		}

		public static implicit operator StructWrapper(long value)
		{
			return value.Wrap<long>();
		}

		public static implicit operator bool(StructWrapper wrapper)
		{
			return (wrapper as StructWrapper<bool>).Unwrap();
		}

		public static implicit operator byte(StructWrapper wrapper)
		{
			return (wrapper as StructWrapper<byte>).Unwrap();
		}

		public static implicit operator float(StructWrapper wrapper)
		{
			return (wrapper as StructWrapper<float>).Unwrap();
		}

		public static implicit operator double(StructWrapper wrapper)
		{
			return (wrapper as StructWrapper<double>).Unwrap();
		}

		public static implicit operator short(StructWrapper wrapper)
		{
			return (wrapper as StructWrapper<short>).Unwrap();
		}

		public static implicit operator int(StructWrapper wrapper)
		{
			return (wrapper as StructWrapper<int>).Unwrap();
		}

		public static implicit operator long(StructWrapper wrapper)
		{
			return (wrapper as StructWrapper<long>).Unwrap();
		}

		public static implicit operator StructWrapper(Vector2 value)
		{
			return value.Wrap<Vector2>();
		}

		public static implicit operator StructWrapper(Vector3 value)
		{
			return value.Wrap<Vector3>();
		}

		public static implicit operator StructWrapper(Quaternion value)
		{
			return value.Wrap<Quaternion>();
		}

		public static implicit operator Vector2(StructWrapper wrapper)
		{
			return (wrapper as StructWrapper<Vector2>).Unwrap();
		}

		public static implicit operator Vector3(StructWrapper wrapper)
		{
			return (wrapper as StructWrapper<Vector3>).Unwrap();
		}

		public static implicit operator Quaternion(StructWrapper wrapper)
		{
			return (wrapper as StructWrapper<Quaternion>).Unwrap();
		}

		public readonly WrappedType wrappedType;

		public readonly Type ttype;
	}
}
