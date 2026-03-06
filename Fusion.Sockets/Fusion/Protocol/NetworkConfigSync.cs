using System;

namespace Fusion.Protocol
{
	internal class NetworkConfigSync : Message
	{
		public NetworkConfigSync() : base(ProtocolMessageVersion.V1_6_0, null)
		{
		}

		public NetworkConfigSync(SyncType type, string serializedNetworkConfig, ProtocolMessageVersion protocolVersion = ProtocolMessageVersion.V1_6_0, Version serializationVersion = null) : base(protocolVersion, serializationVersion)
		{
			this.Type = type;
			this.NetworkConfig = serializedNetworkConfig;
		}

		protected override void SerializeProtected(BitStream stream)
		{
			byte type = (byte)this.Type;
			stream.Serialize(ref type);
			stream.Serialize(ref this.NetworkConfig);
			this.Type = (SyncType)type;
		}

		public override string ToString()
		{
			return string.Format("[{0}: {1}={2}, {3}={4}, {5}]", new object[]
			{
				"NetworkConfigSync",
				"Type",
				this.Type,
				"NetworkConfig",
				this.NetworkConfig,
				base.ToString()
			});
		}

		public SyncType Type;

		public string NetworkConfig;
	}
}
