using System;

namespace Fusion.Protocol
{
	internal class PlayerRefMapping : Message
	{
		protected override void SerializeProtected(BitStream stream)
		{
			stream.Serialize(ref this.ActorId);
			stream.Serialize(ref this.PlayerRef);
			stream.Serialize(ref this.UniqueId);
		}

		public override string ToString()
		{
			long num = (this.UniqueId != null && this.UniqueId.Length == 8) ? BitConverter.ToInt64(this.UniqueId, 0) : 0L;
			return string.Format("[{0}: {1}={2}, {3}={4}, {5}={6}, {7}]", new object[]
			{
				"PlayerRefMapping",
				"ActorId",
				this.ActorId,
				"PlayerRef",
				this.PlayerRef,
				"UniqueId",
				num,
				base.ToString()
			});
		}

		public PlayerRefMapping() : base(ProtocolMessageVersion.V1_6_0, null)
		{
		}

		public int ActorId;

		public int PlayerRef;

		public byte[] UniqueId;
	}
}
