using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Fusion
{
	[NetworkStructWeaved(1)]
	[Serializable]
	[StructLayout(LayoutKind.Explicit)]
	public struct NetworkBool : INetworkStruct, IEquatable<NetworkBool>
	{
		public NetworkBool(bool value)
		{
			this._value = (value ? 1 : 0);
		}

		public bool Equals(NetworkBool other)
		{
			return this._value == other._value;
		}

		public override string ToString()
		{
			return (this._value == 0) ? "false" : "true";
		}

		public override bool Equals(object obj)
		{
			bool result;
			if (obj is NetworkBool)
			{
				NetworkBool other = (NetworkBool)obj;
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
			return this._value;
		}

		public static implicit operator bool(NetworkBool val)
		{
			return val._value == 1;
		}

		public static implicit operator NetworkBool(bool val)
		{
			NetworkBool result;
			result._value = (val ? 1 : 0);
			return result;
		}

		public const int SIZE = 4;

		[SerializeField]
		[FieldOffset(0)]
		private int _value;
	}
}
