using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Fusion
{
	[NetworkStructWeaved(1)]
	[Serializable]
	[StructLayout(LayoutKind.Explicit)]
	public struct NetworkObjectNestingKey : INetworkStruct, IEquatable<NetworkObjectNestingKey>
	{
		public bool IsNone
		{
			get
			{
				return this.Value == 0;
			}
		}

		public bool IsValid
		{
			get
			{
				return this.Value > 0;
			}
		}

		public NetworkObjectNestingKey(int value)
		{
			this.Value = value;
		}

		public bool Equals(NetworkObjectNestingKey other)
		{
			return this.Value == other.Value;
		}

		public override bool Equals(object obj)
		{
			bool result;
			if (obj is NetworkObjectNestingKey)
			{
				NetworkObjectNestingKey other = (NetworkObjectNestingKey)obj;
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
			return this.Value;
		}

		public override string ToString()
		{
			return this.IsNone ? "[NestingKey:None]" : string.Format("[NestingKey:{0}]", this.Value);
		}

		public const int SIZE = 4;

		public const int ALIGNMENT = 4;

		[FieldOffset(0)]
		public int Value;

		public sealed class EqualityComparer : IEqualityComparer<NetworkObjectNestingKey>
		{
			public bool Equals(NetworkObjectNestingKey x, NetworkObjectNestingKey y)
			{
				return x.Value == y.Value;
			}

			public int GetHashCode(NetworkObjectNestingKey obj)
			{
				return obj.Value;
			}
		}
	}
}
