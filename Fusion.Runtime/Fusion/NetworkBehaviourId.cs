using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fusion
{
	[NetworkStructWeaved(2)]
	[Serializable]
	[StructLayout(LayoutKind.Explicit)]
	public struct NetworkBehaviourId : INetworkStruct, IEquatable<NetworkBehaviourId>
	{
		public bool IsValid
		{
			get
			{
				return this.Object.IsValid && this.Behaviour >= 0;
			}
		}

		public static NetworkBehaviourId None
		{
			get
			{
				return default(NetworkBehaviourId);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(NetworkBehaviourId other)
		{
			return this.Object.Equals(other.Object) && this.Behaviour == other.Behaviour;
		}

		public override bool Equals(object obj)
		{
			bool result;
			if (obj is NetworkBehaviourId)
			{
				NetworkBehaviourId other = (NetworkBehaviourId)obj;
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
			return this.Object.GetHashCode() * 397 ^ this.Behaviour;
		}

		public override string ToString()
		{
			return string.Format("[Object:{0}, Behaviour:{1}]", this.Object, this.Behaviour);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(NetworkBehaviourId a, NetworkBehaviourId b)
		{
			return a.Equals(b);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(NetworkBehaviourId a, NetworkBehaviourId b)
		{
			return !a.Equals(b);
		}

		public const int SIZE = 8;

		[FieldOffset(0)]
		public NetworkId Object;

		[FieldOffset(4)]
		public int Behaviour;
	}
}
