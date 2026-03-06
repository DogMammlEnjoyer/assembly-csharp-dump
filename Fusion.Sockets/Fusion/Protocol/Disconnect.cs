using System;

namespace Fusion.Protocol
{
	internal class Disconnect : Message
	{
		public Disconnect() : base(ProtocolMessageVersion.V1_6_0, null)
		{
		}

		public Disconnect(DisconnectReason reason, ProtocolMessageVersion protocolVersion = ProtocolMessageVersion.V1_6_0, Version serializationVersion = null) : base(protocolVersion, serializationVersion)
		{
			this.DisconnectReason = reason;
		}

		protected override void SerializeProtected(BitStream stream)
		{
			string text = this.DisconnectReason.ToString();
			byte disconnectReason = (byte)this.DisconnectReason;
			bool flag = this.ProtocolVersion >= ProtocolMessageVersion.V1_1_0;
			if (flag)
			{
				stream.Serialize(ref disconnectReason);
			}
			else
			{
				stream.Serialize(ref text);
			}
			this.DisconnectReason = (DisconnectReason)disconnectReason;
		}

		public override string ToString()
		{
			return string.Format("[{0}: {1}={2}, {3}]", new object[]
			{
				"Disconnect",
				"DisconnectReason",
				this.DisconnectReason,
				base.ToString()
			});
		}

		public DisconnectReason DisconnectReason;
	}
}
