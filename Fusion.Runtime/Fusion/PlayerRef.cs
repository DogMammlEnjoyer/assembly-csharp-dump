using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Fusion.Sockets;

namespace Fusion
{
	[NetworkStructWeaved(1)]
	[Serializable]
	[StructLayout(LayoutKind.Explicit)]
	public struct PlayerRef : INetworkStruct, IEquatable<PlayerRef>
	{
		public static IEqualityComparer<PlayerRef> Comparer { get; } = new PlayerRef.IndexEqualityComparer();

		public static PlayerRef Invalid
		{
			get
			{
				PlayerRef result;
				result._index = -10;
				return result;
			}
		}

		public static PlayerRef None
		{
			get
			{
				return default(PlayerRef);
			}
		}

		public static PlayerRef MasterClient
		{
			get
			{
				PlayerRef result;
				result._index = -1;
				return result;
			}
		}

		public bool IsRealPlayer
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this._index > 0;
			}
		}

		public bool IsNone
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this._index == 0;
			}
		}

		public bool IsMasterClient
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this._index == -1;
			}
		}

		public int RawEncoded
		{
			get
			{
				return this._index;
			}
		}

		public int AsIndex
		{
			get
			{
				return this._index - 1;
			}
		}

		public int PlayerId
		{
			get
			{
				return this._index - 1;
			}
		}

		public override bool Equals(object obj)
		{
			bool result;
			if (obj is PlayerRef)
			{
				PlayerRef other = (PlayerRef)obj;
				result = this.Equals(other);
			}
			else
			{
				result = false;
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
		{
			return this._index;
		}

		public override string ToString()
		{
			return (this._index > 0) ? string.Format("[Player:{0}]", this._index - 1) : ((this._index == -1) ? "[Player:MasterClient]" : "[Player:None]");
		}

		public static PlayerRef FromEncoded(int encoded)
		{
			PlayerRef result;
			result._index = encoded;
			return result;
		}

		public static PlayerRef FromIndex(int index)
		{
			PlayerRef result;
			result._index = index + 1;
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(PlayerRef a, PlayerRef b)
		{
			return a._index == b._index;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(PlayerRef a, PlayerRef b)
		{
			return a._index != b._index;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void Write(NetBitBuffer* buffer, PlayerRef playerRef)
		{
			bool flag = buffer->WriteBoolean(playerRef.IsRealPlayer);
			if (flag)
			{
				buffer->WriteInt32VarLength(playerRef.AsIndex);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void Write<[IsUnmanaged] T>(T* buffer, PlayerRef playerRef) where T : struct, ValueType, INetBitWriteStream
		{
			bool flag = buffer->WriteBoolean(playerRef.IsRealPlayer);
			if (flag)
			{
				buffer->WriteInt32VarLength(playerRef.AsIndex);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static PlayerRef Read(NetBitBuffer* buffer)
		{
			bool flag = buffer->ReadBoolean();
			PlayerRef result;
			if (flag)
			{
				PlayerRef playerRef = PlayerRef.FromIndex(buffer->ReadInt32VarLength());
				Assert.Check(!playerRef.IsNone);
				result = playerRef;
			}
			else
			{
				result = default(PlayerRef);
			}
			return result;
		}

		public bool Equals(PlayerRef other)
		{
			return this._index == other._index;
		}

		public const int SIZE = 4;

		private const int MASTER_CLIENT_RAW = -1;

		private const int INVALID_RAW = -10;

		[FieldOffset(0)]
		private int _index;

		private sealed class IndexEqualityComparer : IEqualityComparer<PlayerRef>
		{
			public bool Equals(PlayerRef x, PlayerRef y)
			{
				return x._index == y._index;
			}

			public int GetHashCode(PlayerRef obj)
			{
				return obj._index;
			}
		}
	}
}
