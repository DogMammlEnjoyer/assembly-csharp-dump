using System;
using System.Runtime.InteropServices;

namespace Fusion
{
	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 1)]
	public readonly struct NetworkSceneLoadId : IEquatable<NetworkSceneLoadId>
	{
		public NetworkSceneLoadId(byte value)
		{
			this.Value = value;
		}

		public bool Equals(NetworkSceneLoadId other)
		{
			return this.Value == other.Value;
		}

		public override bool Equals(object obj)
		{
			bool result;
			if (obj is NetworkSceneLoadId)
			{
				NetworkSceneLoadId other = (NetworkSceneLoadId)obj;
				result = this.Equals(other);
			}
			else
			{
				result = false;
			}
			return result;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public static bool operator ==(NetworkSceneLoadId left, NetworkSceneLoadId right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(NetworkSceneLoadId left, NetworkSceneLoadId right)
		{
			return !left.Equals(right);
		}

		public static implicit operator NetworkSceneLoadId(byte value)
		{
			return new NetworkSceneLoadId(value);
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}

		public readonly byte Value;
	}
}
