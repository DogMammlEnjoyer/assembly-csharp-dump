using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Fusion.Sockets
{
	[StructLayout(LayoutKind.Explicit)]
	public struct NetConnectionId : IEquatable<NetConnectionId>
	{
		public bool Equals(NetConnectionId other)
		{
			return this.Raw == other.Raw;
		}

		public override bool Equals(object obj)
		{
			bool result;
			if (obj is NetConnectionId)
			{
				NetConnectionId other = (NetConnectionId)obj;
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
			return this.Raw.GetHashCode();
		}

		public static bool operator ==(NetConnectionId a, NetConnectionId b)
		{
			return a.Raw == b.Raw;
		}

		public static bool operator !=(NetConnectionId a, NetConnectionId b)
		{
			return a.Raw != b.Raw;
		}

		public override string ToString()
		{
			return string.Format("[NetConnectionId Group:{0}, GroupIndex:{1}, Generation:{2}]", this.Group, this.GroupIndex, this.Generation);
		}

		[FieldOffset(0)]
		internal ulong Raw;

		[FieldOffset(0)]
		public short Group;

		[FieldOffset(2)]
		public short GroupIndex;

		[FieldOffset(4)]
		internal uint Generation;

		public class EqualityComparer : IEqualityComparer<NetConnectionId>
		{
			public bool Equals(NetConnectionId x, NetConnectionId y)
			{
				return x.Raw == y.Raw;
			}

			public int GetHashCode(NetConnectionId obj)
			{
				return obj.Raw.GetHashCode();
			}
		}
	}
}
