using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Fusion
{
	[InlineHelp]
	[NetworkStructWeaved(20)]
	[StructLayout(LayoutKind.Explicit)]
	public struct NetworkObjectHeader : INetworkStruct, IEquatable<NetworkObjectHeader>
	{
		public NetworkObjectHeader(NetworkId id, short wordCount, short behaviourCount, NetworkObjectTypeId type, NetworkId nestingRoot, NetworkObjectNestingKey nestingKey, NetworkObjectHeaderFlags flags)
		{
			this.InputAuthority = default(PlayerRef);
			this.StateAuthority = default(PlayerRef);
			this.PlayerData = default(NetworkObjectHeader.PlayerUniqueData);
			this.Id = id;
			this.WordCount = wordCount;
			this.BehaviourCount = behaviourCount;
			this.Type = type;
			this.NestingRoot = nestingRoot;
			this.NestingKey = nestingKey;
			this.Flags = flags;
		}

		public int ByteCount
		{
			get
			{
				return (int)(this.WordCount * 4);
			}
		}

		[Obsolete("Use NetworkObjectMeta instead")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int* GetDataPointer(NetworkObjectHeader* header)
		{
			return (int*)(header + (IntPtr)20 * 4 / (IntPtr)sizeof(NetworkObjectHeader));
		}

		[Obsolete("Use NetworkObjectMeta instead")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int GetDataWordCount(NetworkObjectHeader* header)
		{
			return (int)(header->WordCount - (20 + header->BehaviourCount));
		}

		[Obsolete("Use NetworkObjectMeta instead")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int* GetBehaviourChangedTickArray(NetworkObjectHeader* header)
		{
			return (int*)(header + (IntPtr)(header->WordCount - header->BehaviourCount) * 4 / (IntPtr)sizeof(NetworkObjectHeader));
		}

		[Obsolete("Use NetworkObjectMeta instead")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static bool HasMainNetworkTRSP(NetworkObjectHeader* header)
		{
			return (header->Flags & NetworkObjectHeaderFlags.HasMainNetworkTRSP) == NetworkObjectHeaderFlags.HasMainNetworkTRSP;
		}

		[Obsolete("Use NetworkObjectMeta instead")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static NetworkTRSPData* GetMainNetworkTRSPData(NetworkObjectHeader* header)
		{
			bool flag = NetworkObjectHeader.HasMainNetworkTRSP(header);
			NetworkTRSPData* result;
			if (flag)
			{
				result = (NetworkTRSPData*)(header + (IntPtr)20 * 4 / (IntPtr)sizeof(NetworkObjectHeader));
			}
			else
			{
				result = null;
			}
			return result;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("[");
			stringBuilder.Append("Id").Append(": ").Append(this.Id.ToString());
			stringBuilder.Append(", ").Append("WordCount").Append(": ").Append(this.WordCount);
			stringBuilder.Append(", ").Append("BehaviourCount").Append(": ").Append(this.BehaviourCount);
			bool isValid = this.Type.IsValid;
			if (isValid)
			{
				stringBuilder.Append(", ").Append("Type").Append(": ").Append(this.Type.ToString());
			}
			bool isValid2 = this.NestingRoot.IsValid;
			if (isValid2)
			{
				stringBuilder.Append(", ").Append("NestingRoot").Append(": ").Append(this.NestingRoot.ToString());
			}
			bool isValid3 = this.NestingKey.IsValid;
			if (isValid3)
			{
				stringBuilder.Append(", ").Append("NestingKey").Append(": ").Append(this.NestingKey.ToString());
			}
			bool flag = this.WordCount != 0;
			if (flag)
			{
				stringBuilder.Append(", ").Append("WordCount").Append(": ").Append(this.WordCount);
			}
			bool flag2 = this.Flags > (NetworkObjectHeaderFlags)0;
			if (flag2)
			{
				stringBuilder.Append(", ").Append("Flags").Append(": ").Append(this.Flags.ToString());
			}
			bool flag3 = this.InputAuthority != default(PlayerRef);
			if (flag3)
			{
				stringBuilder.Append(", ").Append("InputAuthority").Append(": ").Append(this.InputAuthority.ToString());
			}
			bool flag4 = this.StateAuthority != default(PlayerRef);
			if (flag4)
			{
				stringBuilder.Append(", ").Append("StateAuthority").Append(": ").Append(this.StateAuthority.ToString());
			}
			bool flag5 = this.PlayerData.Flags > (NetworkObjectHeaderPlayerDataFlags)0;
			if (flag5)
			{
				stringBuilder.Append(", ").Append("PlayerData").Append(": ").Append(this.PlayerData.Flags.ToString());
			}
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}

		public bool Equals(NetworkObjectHeader other)
		{
			return this == other;
		}

		public override bool Equals(object obj)
		{
			bool result;
			if (obj is NetworkObjectHeader)
			{
				NetworkObjectHeader other = (NetworkObjectHeader)obj;
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
			int num = this.Id.GetHashCode();
			num = (num * 397 ^ (int)this.WordCount);
			num = (num * 397 ^ (int)this.BehaviourCount);
			num = (num * 397 ^ this.Type.GetHashCode());
			num = (num * 397 ^ this.NestingRoot.GetHashCode());
			num = (num * 397 ^ this.NestingKey.GetHashCode());
			num = (num * 397 ^ (int)this.Flags);
			num = (num * 397 ^ this.InputAuthority.GetHashCode());
			num = (num * 397 ^ this.StateAuthority.GetHashCode());
			return num * 397 ^ this.PlayerData.GetHashCode();
		}

		public unsafe static bool operator ==(NetworkObjectHeader left, NetworkObjectHeader right)
		{
			return Native.MemCmp((void*)(&left), (void*)(&right), 80) == 0;
		}

		public unsafe static bool operator !=(NetworkObjectHeader left, NetworkObjectHeader right)
		{
			return Native.MemCmp((void*)(&left), (void*)(&right), 80) != 0;
		}

		public const int SIZE = 80;

		public const int WORDS = 20;

		public const int PLAYER_DATA_WORD = 9;

		[FieldOffset(0)]
		public readonly NetworkId Id;

		[FieldOffset(4)]
		public readonly short WordCount;

		[FieldOffset(6)]
		public readonly short BehaviourCount;

		[FieldOffset(8)]
		public readonly NetworkObjectTypeId Type;

		[FieldOffset(16)]
		public readonly NetworkId NestingRoot;

		[FieldOffset(20)]
		public readonly NetworkObjectNestingKey NestingKey;

		[FieldOffset(24)]
		public readonly NetworkObjectHeaderFlags Flags;

		internal const int READ_ONLY_WORD_COUNT = 7;

		[FieldOffset(28)]
		public PlayerRef InputAuthority;

		[FieldOffset(32)]
		public PlayerRef StateAuthority;

		[FieldOffset(36)]
		internal NetworkObjectHeader.PlayerUniqueData PlayerData;

		[FixedBuffer(typeof(int), 10)]
		[FieldOffset(40)]
		private NetworkObjectHeader.<_reserved>e__FixedBuffer _reserved;

		[StructLayout(LayoutKind.Explicit)]
		internal struct PlayerUniqueData
		{
			public bool HasFlag(NetworkObjectHeaderPlayerDataFlags flag)
			{
				return (this.Flags & flag) == flag;
			}

			public void SetFlag(NetworkObjectHeaderPlayerDataFlags flag)
			{
				this.Flags |= flag;
			}

			public void ClearFlag(NetworkObjectHeaderPlayerDataFlags flag)
			{
				this.Flags &= ~flag;
			}

			public const int SIZE = 4;

			public const int WORDS = 1;

			public const int FLAGS_WORD_INDEX = 0;

			[FieldOffset(0)]
			public NetworkObjectHeaderPlayerDataFlags Flags;
		}

		[StructLayout(LayoutKind.Explicit)]
		internal struct PlayerUniqueDataChanges
		{
			public unsafe int MaxTick
			{
				get
				{
					int num = this.Changes.FixedElementField;
					for (int i = 1; i < 1; i++)
					{
						bool flag = *(ref this.Changes.FixedElementField + (IntPtr)i * 4) > num;
						if (flag)
						{
							num = *(ref this.Changes.FixedElementField + (IntPtr)i * 4);
						}
					}
					return num;
				}
			}

			[FixedBuffer(typeof(int), 1)]
			[FieldOffset(0)]
			public NetworkObjectHeader.PlayerUniqueDataChanges.<Changes>e__FixedBuffer Changes;

			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 4)]
			public struct <Changes>e__FixedBuffer
			{
				public int FixedElementField;
			}
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 40)]
		public struct <_reserved>e__FixedBuffer
		{
			public int FixedElementField;
		}
	}
}
