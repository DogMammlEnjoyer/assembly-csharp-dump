using System;

namespace Fusion.Protocol
{
	internal class Snapshot : Message
	{
		public int Tick { get; private set; }

		public uint NetworkID { get; private set; }

		public SnapshotType SnapshotType { get; private set; }

		public int TotalSize { get; private set; }

		public override bool IsValid
		{
			get
			{
				return base.IsValid && this.CRC == Snapshot.ComputeCRC(this.Data, this.TotalSize);
			}
		}

		public byte[] Data { get; private set; }

		private ulong CRC { get; set; }

		public Snapshot() : base(ProtocolMessageVersion.V1_6_0, null)
		{
		}

		public Snapshot(int tick, uint networkID, SnapshotType snapshotType, int snapshotSize, byte[] data, ProtocolMessageVersion protocolVersion = ProtocolMessageVersion.V1_6_0, Version serializationVersion = null) : base(protocolVersion, serializationVersion)
		{
			this.Tick = tick;
			this.NetworkID = networkID;
			this.SnapshotType = snapshotType;
			this.TotalSize = snapshotSize;
			this.Data = data;
			this.CRC = Snapshot.ComputeCRC(this.Data, this.TotalSize);
		}

		protected override void SerializeProtected(BitStream stream)
		{
			int tick = this.Tick;
			uint networkID = this.NetworkID;
			byte snapshotType = (byte)this.SnapshotType;
			ulong crc = this.CRC;
			int totalSize = this.TotalSize;
			byte[] data = this.Data;
			stream.Serialize(ref tick);
			stream.Serialize(ref networkID);
			stream.Serialize(ref snapshotType);
			int num = 0;
			stream.Serialize(ref num);
			stream.Serialize(ref crc);
			stream.Serialize(ref totalSize);
			stream.Serialize(ref data, ref totalSize);
			this.Tick = tick;
			this.NetworkID = networkID;
			this.SnapshotType = (SnapshotType)snapshotType;
			this.CRC = crc;
			this.TotalSize = totalSize;
			this.Data = data;
		}

		private unsafe static ulong ComputeCRC(byte[] data, int length)
		{
			bool flag = data == null;
			ulong result;
			if (flag)
			{
				result = 0UL;
			}
			else
			{
				byte* data2;
				if (data == null || data.Length == 0)
				{
					data2 = null;
				}
				else
				{
					data2 = &data[0];
				}
				result = CRC64.Compute(data2, length);
			}
			return result;
		}

		public override Message Clone()
		{
			return new Snapshot();
		}

		public override string ToString()
		{
			return string.Format("[{0}: {1}={2}, {3}={4}, {5}={6}, {7}={8}, CRC={9}, {10}={11}, {12}]", new object[]
			{
				"Snapshot",
				"Tick",
				this.Tick,
				"NetworkID",
				this.NetworkID,
				"SnapshotType",
				this.SnapshotType,
				"TotalSize",
				this.TotalSize,
				this.CRC,
				"IsValid",
				this.IsValid,
				base.ToString()
			});
		}
	}
}
