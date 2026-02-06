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
	public struct NetworkId : INetworkStruct, IEquatable<NetworkId>, IComparable, IComparable<NetworkId>
	{
		public static NetworkId.EqualityComparer Comparer { get; } = new NetworkId.EqualityComparer();

		public bool IsValid
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.Raw > 0U;
			}
		}

		public bool IsReserved
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.Raw > 0U && this.Raw <= 1023U;
			}
		}

		internal static NetworkId RuntimeConfig
		{
			get
			{
				return new NetworkId(1U);
			}
		}

		internal static NetworkId SceneInfo
		{
			get
			{
				return new NetworkId(3U);
			}
		}

		internal static NetworkId PhysicsInfo
		{
			get
			{
				return new NetworkId(4U);
			}
		}

		internal NetworkId(uint raw)
		{
			this.Raw = raw;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(NetworkId other)
		{
			return this.Raw == other.Raw;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int CompareTo(NetworkId other)
		{
			return (int)(this.Raw - other.Raw);
		}

		public override bool Equals(object obj)
		{
			bool result;
			if (obj is NetworkId)
			{
				NetworkId networkId = (NetworkId)obj;
				result = (this.Raw == networkId.Raw);
			}
			else
			{
				result = false;
			}
			return result;
		}

		int IComparable.CompareTo(object obj)
		{
			int result;
			if (obj is NetworkId)
			{
				NetworkId other = (NetworkId)obj;
				result = this.CompareTo(other);
			}
			else
			{
				result = 0;
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(NetworkId a, NetworkId b)
		{
			return a.Raw == b.Raw;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(NetworkId a, NetworkId b)
		{
			return a.Raw != b.Raw;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator bool(NetworkId id)
		{
			return id.Raw > 0U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void Write(NetBitBuffer* buffer, NetworkId id)
		{
			buffer->WriteUInt32VarLength(id.Raw, 8);
		}

		public unsafe static NetworkId Read(NetBitBuffer* buffer)
		{
			NetworkId result;
			result.Raw = buffer->ReadUInt32VarLength(8);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void Write(NetBitBuffer* buffer)
		{
			NetworkId.Write(buffer, this);
		}

		public override int GetHashCode()
		{
			return (int)this.Raw;
		}

		public override string ToString()
		{
			bool isValid = this.IsValid;
			string result;
			if (isValid)
			{
				switch (this.Raw)
				{
				case 1U:
					result = "[Id:RuntimeConfig]";
					break;
				case 2U:
					result = "[Id:PlayerDataArray]";
					break;
				case 3U:
					result = "[Id:SceneInfo]";
					break;
				case 4U:
					result = "[Id:Physics]";
					break;
				default:
					result = string.Format("[Id:{0}]", this.Raw);
					break;
				}
			}
			else
			{
				result = "[Id:None]";
			}
			return result;
		}

		public string ToNamePrefixString()
		{
			return this.IsValid ? string.Format("[{0}] ", this.Raw) : "[Invalid] ";
		}

		public const int BLOCK_SIZE = 8;

		public const int SIZE = 4;

		public const int ALIGNMENT = 4;

		[FieldOffset(0)]
		public uint Raw;

		internal const int MAX_RESERVED_ID = 1023;

		private const uint RAW_RUNTIME_CONFIG = 1U;

		private const uint RAW_PLAYER_REF_DATA_ARRAY = 2U;

		private const uint RAW_SCENE_INFO = 3U;

		private const uint RAW_PHYSICS_INFO = 4U;

		public sealed class EqualityComparer : IEqualityComparer<NetworkId>
		{
			public bool Equals(NetworkId a, NetworkId b)
			{
				return a.Raw == b.Raw;
			}

			public int GetHashCode(NetworkId id)
			{
				return (int)id.Raw;
			}
		}
	}
}
