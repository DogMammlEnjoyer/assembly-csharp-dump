using System;

namespace Fusion.Protocol
{
	internal class HostMigration : Message
	{
		public HostMigration() : base(ProtocolMessageVersion.V1_6_0, null)
		{
		}

		public HostMigration(PeerMode peerMode, ProtocolMessageVersion protocolVersion = ProtocolMessageVersion.V1_6_0, Version serializationVersion = null) : base(protocolVersion, serializationVersion)
		{
			this.PeerMode = peerMode;
		}

		protected override void SerializeProtected(BitStream stream)
		{
			byte peerMode = (byte)this.PeerMode;
			stream.Serialize(ref peerMode);
			this.PeerMode = (PeerMode)peerMode;
		}

		public override string ToString()
		{
			return string.Format("[{0}: {1}={2}, {3}]", new object[]
			{
				"HostMigration",
				"PeerMode",
				this.PeerMode,
				base.ToString()
			});
		}

		public PeerMode PeerMode;
	}
}
