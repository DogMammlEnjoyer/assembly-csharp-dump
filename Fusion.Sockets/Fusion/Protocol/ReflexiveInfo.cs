using System;
using Fusion.Sockets;
using Fusion.Sockets.Stun;

namespace Fusion.Protocol
{
	internal class ReflexiveInfo : Message
	{
		public override bool IsValid
		{
			get
			{
				return base.IsValid && this.PublicAddr.IsValid && this.PrivateAddr.IsValid;
			}
		}

		public ReflexiveInfo() : base(ProtocolMessageVersion.V1_6_0, null)
		{
		}

		public ReflexiveInfo(int actorNr, NetAddress publicAddr, NetAddress privateAddr, NATType stunNatType, byte[] uniqueID = null, ProtocolMessageVersion protocolVersion = ProtocolMessageVersion.V1_6_0, Version serializationVersion = null) : base(protocolVersion, serializationVersion)
		{
			this.ActorNr = actorNr;
			this.PublicAddr = publicAddr;
			this.PrivateAddr = privateAddr;
			this.NatType = stunNatType;
			this.UniqueId = uniqueID;
		}

		protected override void SerializeProtected(BitStream stream)
		{
			stream.Serialize(ref this.ActorNr);
			this.PublicAddr.Serialize(stream);
			this.PrivateAddr.Serialize(stream);
			byte natType = (byte)this.NatType;
			bool flag = this.ProtocolVersion >= ProtocolMessageVersion.V1_2_1;
			if (flag)
			{
				stream.Serialize(ref natType);
			}
			else
			{
				natType = 4;
			}
			this.NatType = (NATType)natType;
			bool flag2 = this.ProtocolVersion >= ProtocolMessageVersion.V1_2_3;
			if (flag2)
			{
				stream.Serialize(ref this.UniqueId);
			}
		}

		public override string ToString()
		{
			long num = (this.UniqueId != null && this.UniqueId.Length == 8) ? BitConverter.ToInt64(this.UniqueId, 0) : 0L;
			return string.Format("[{0}: {1}={2}, {3}={4}, {5}={6}, {7}={8}, {9}={10}, {11}]", new object[]
			{
				"ReflexiveInfo",
				"ActorNr",
				this.ActorNr,
				"PublicAddr",
				this.PublicAddr,
				"PrivateAddr",
				this.PrivateAddr,
				"NatType",
				this.NatType,
				"UniqueId",
				num,
				base.ToString()
			});
		}

		public int ActorNr;

		public NetAddress PublicAddr;

		public NetAddress PrivateAddr;

		public NATType NatType;

		public byte[] UniqueId;
	}
}
