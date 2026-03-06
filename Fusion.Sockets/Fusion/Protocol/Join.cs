using System;

namespace Fusion.Protocol
{
	internal class Join : Message
	{
		public Join() : base(ProtocolMessageVersion.V1_6_0, null)
		{
		}

		public Join(JoinMessageType type, PluginGameMode mode, PeerMode peerMode, int playerRef, JoinRequests joinRequests = JoinRequests.None, byte[] uniqueID = null, byte[] encryptionKey = null, byte[] encryptionKeySecret = null, ProtocolMessageVersion protocolVersion = ProtocolMessageVersion.V1_6_0, Version serializationVersion = null) : base(protocolVersion, serializationVersion)
		{
			bool flag = type == JoinMessageType.Request;
			if (flag)
			{
				Assert.Check(playerRef == 0);
			}
			else
			{
				Assert.Check(playerRef > 0);
			}
			this.Type = type;
			this.GameMode = mode;
			this.JoinRequests = joinRequests;
			this.PeerMode = peerMode;
			this.UniqueId = uniqueID;
			this.PlayerRef = playerRef;
			this.EncryptionKey = encryptionKey;
			this.EncryptionKeySecret = encryptionKeySecret;
		}

		protected override void SerializeProtected(BitStream stream)
		{
			byte type = (byte)this.Type;
			byte gameMode = (byte)this.GameMode;
			byte peerMode = (byte)this.PeerMode;
			uint joinRequests = (uint)this.JoinRequests;
			stream.Serialize(ref type);
			stream.Serialize(ref gameMode);
			stream.Serialize(ref peerMode);
			stream.Serialize(ref joinRequests);
			bool flag = this.ProtocolVersion >= ProtocolMessageVersion.V1_2_3;
			if (flag)
			{
				stream.Serialize(ref this.UniqueId);
			}
			bool flag2 = this.ProtocolVersion >= ProtocolMessageVersion.V1_3_0;
			if (flag2)
			{
				stream.Serialize(ref this.PlayerRef);
			}
			bool flag3 = this.ProtocolVersion >= ProtocolMessageVersion.V1_5_0;
			if (flag3)
			{
				stream.Serialize(ref this.EncryptionKey);
				stream.Serialize(ref this.EncryptionKeySecret);
			}
			this.Type = (JoinMessageType)type;
			this.GameMode = (PluginGameMode)gameMode;
			this.PeerMode = (PeerMode)peerMode;
			this.JoinRequests = (JoinRequests)joinRequests;
		}

		public override string ToString()
		{
			long num = (this.UniqueId != null && this.UniqueId.Length == 8) ? BitConverter.ToInt64(this.UniqueId, 0) : 0L;
			return string.Format("[{0}: {1}={2}, {3}={4}, {5}={6}, {7}={8}, {9}={10}, {11}]", new object[]
			{
				"Join",
				"Type",
				this.Type,
				"GameMode",
				this.GameMode,
				"PeerMode",
				this.PeerMode,
				"JoinRequests",
				this.JoinRequests,
				"UniqueId",
				num,
				base.ToString()
			});
		}

		public JoinMessageType Type;

		public PluginGameMode GameMode;

		public PeerMode PeerMode;

		public JoinRequests JoinRequests;

		public byte[] UniqueId;

		public int PlayerRef;

		public byte[] EncryptionKey;

		public byte[] EncryptionKeySecret;
	}
}
