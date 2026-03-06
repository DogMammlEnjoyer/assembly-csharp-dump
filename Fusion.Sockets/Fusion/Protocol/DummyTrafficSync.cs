using System;

namespace Fusion.Protocol
{
	internal class DummyTrafficSync : Message
	{
		public int SendInterval { get; private set; } = 5000;

		public int Size { get; private set; } = 2;

		public override bool IsValid
		{
			get
			{
				return base.IsValid && this.SendInterval >= 100 && this.SendInterval <= 5000 && this.Size >= 2 && this.Size <= 128;
			}
		}

		public DummyTrafficSync() : base(ProtocolMessageVersion.V1_6_0, null)
		{
		}

		public DummyTrafficSync(int sendInterval, int size, ProtocolMessageVersion protocolVersion = ProtocolMessageVersion.V1_6_0, Version serializationVersion = null) : base(protocolVersion, serializationVersion)
		{
			this.SendInterval = Math.Max(Math.Min(sendInterval, 5000), 100);
			this.Size = Math.Max(Math.Min(size, 128), 2);
		}

		protected override void SerializeProtected(BitStream stream)
		{
			int sendInterval = this.SendInterval;
			int size = this.Size;
			stream.Serialize(ref sendInterval);
			stream.Serialize(ref size);
			this.SendInterval = Math.Max(Math.Min(sendInterval, 5000), 100);
			this.Size = Math.Max(Math.Min(size, 128), 2);
		}

		public override string ToString()
		{
			return string.Format("[{0}: {1}={2}, {3}={4}, {5}]", new object[]
			{
				"DummyTrafficSync",
				"SendInterval",
				this.SendInterval,
				"Size",
				this.Size,
				base.ToString()
			});
		}

		private const int DummySendIntervalMax = 5000;

		private const int DummySendIntervalMin = 100;

		private const int DummySizeMax = 128;

		private const int DummySizeMin = 2;
	}
}
