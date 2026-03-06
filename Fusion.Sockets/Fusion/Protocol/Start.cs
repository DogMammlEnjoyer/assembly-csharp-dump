using System;

namespace Fusion.Protocol
{
	internal class Start : Message
	{
		public Start() : base(ProtocolMessageVersion.V1_6_0, null)
		{
		}

		public Start(int remoteServerId, StartRequests startRequests, ProtocolMessageVersion protocolVersion = ProtocolMessageVersion.V1_6_0, Version serializationVersion = null) : base(protocolVersion, serializationVersion)
		{
			this.RemoteServerID = remoteServerId;
			this.StartRequests = startRequests;
		}

		protected override void SerializeProtected(BitStream stream)
		{
			uint startRequests = (uint)this.StartRequests;
			stream.Serialize(ref this.RemoteServerID);
			stream.Serialize(ref startRequests);
			this.StartRequests = (StartRequests)startRequests;
		}

		public override string ToString()
		{
			return string.Format("[{0}: {1}={2}, {3}={4}, {5}]", new object[]
			{
				"Start",
				"RemoteServerID",
				this.RemoteServerID,
				"StartRequests",
				this.StartRequests,
				base.ToString()
			});
		}

		public int RemoteServerID;

		public StartRequests StartRequests;
	}
}
