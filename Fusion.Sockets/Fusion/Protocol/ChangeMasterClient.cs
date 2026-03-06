using System;

namespace Fusion.Protocol
{
	internal class ChangeMasterClient : Message
	{
		public ChangeMasterClient() : base(ProtocolMessageVersion.V1_6_0, null)
		{
		}

		public ChangeMasterClient(int newMasterClientCandidate, ProtocolMessageVersion protocolVersion = ProtocolMessageVersion.V1_6_0, Version serializationVersion = null) : base(protocolVersion, serializationVersion)
		{
			this.NewMasterClientCandidate = newMasterClientCandidate;
		}

		protected override void SerializeProtected(BitStream stream)
		{
			stream.Serialize(ref this.NewMasterClientCandidate);
		}

		public override string ToString()
		{
			return string.Format("[{0}: {1}={2}, {3}]", new object[]
			{
				"ChangeMasterClient",
				"NewMasterClientCandidate",
				this.NewMasterClientCandidate,
				base.ToString()
			});
		}

		public int NewMasterClientCandidate;
	}
}
