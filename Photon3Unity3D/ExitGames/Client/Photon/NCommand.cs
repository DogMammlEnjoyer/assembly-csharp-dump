using System;

namespace ExitGames.Client.Photon
{
	internal class NCommand : IComparable<NCommand>
	{
		protected internal int SizeOfPayload
		{
			get
			{
				return (this.Payload != null) ? this.Payload.Length : 0;
			}
		}

		protected internal bool IsFlaggedUnsequenced
		{
			get
			{
				return (this.commandFlags & 2) > 0;
			}
		}

		protected internal bool IsFlaggedReliable
		{
			get
			{
				return (this.commandFlags & 1) > 0;
			}
		}

		internal static void CreateAck(byte[] buffer, int offset, NCommand commandToAck, int sentTime)
		{
			buffer[offset++] = (commandToAck.IsFlaggedUnsequenced ? 16 : 1);
			buffer[offset++] = commandToAck.commandChannelID;
			buffer[offset++] = 0;
			buffer[offset++] = 4;
			Protocol.Serialize(20, buffer, ref offset);
			Protocol.Serialize(0, buffer, ref offset);
			Protocol.Serialize(commandToAck.reliableSequenceNumber, buffer, ref offset);
			Protocol.Serialize(sentTime, buffer, ref offset);
		}

		internal NCommand(EnetPeer peer, byte commandType, StreamBuffer payload, byte channel)
		{
			this.Initialize(peer, commandType, payload, channel);
		}

		internal void Initialize(EnetPeer peer, byte commandType, StreamBuffer payload, byte channel)
		{
			this.commandType = commandType;
			this.commandFlags = 1;
			this.commandChannelID = channel;
			this.Payload = payload;
			this.Size = 12;
			switch (this.commandType)
			{
			case 2:
			{
				this.Size = 44;
				byte[] array = new byte[32];
				array[0] = 0;
				array[1] = 0;
				int num = 2;
				Protocol.Serialize((short)peer.mtu, array, ref num);
				array[4] = 0;
				array[5] = 0;
				array[6] = 128;
				array[7] = 0;
				array[11] = peer.ChannelCount;
				array[15] = 0;
				array[19] = 0;
				array[22] = 19;
				array[23] = 136;
				array[27] = 2;
				array[31] = 2;
				this.Payload = new StreamBuffer(array);
				break;
			}
			case 4:
			{
				this.Size = 12;
				bool flag = peer.peerConnectionState != ConnectionStateValue.Connected;
				if (flag)
				{
					this.commandFlags = 2;
					this.reservedByte = ((peer.peerConnectionState == ConnectionStateValue.Zombie) ? 2 : 4);
				}
				break;
			}
			case 6:
				this.Size = 12 + payload.Length;
				break;
			case 7:
				this.Size = 16 + payload.Length;
				this.commandFlags = 0;
				break;
			case 8:
				this.Size = 32 + payload.Length;
				break;
			case 11:
				this.Size = 16 + payload.Length;
				this.commandFlags = 2;
				break;
			case 14:
				this.Size = 12 + payload.Length;
				this.commandFlags = 3;
				break;
			case 15:
				this.Size = 32 + payload.Length;
				this.commandFlags = 3;
				break;
			}
		}

		internal NCommand(EnetPeer peer, byte[] inBuff, ref int readingOffset)
		{
			this.Initialize(peer, inBuff, ref readingOffset);
		}

		internal void Initialize(EnetPeer peer, byte[] inBuff, ref int readingOffset)
		{
			int num = readingOffset;
			readingOffset = num + 1;
			this.commandType = inBuff[num];
			num = readingOffset;
			readingOffset = num + 1;
			this.commandChannelID = inBuff[num];
			num = readingOffset;
			readingOffset = num + 1;
			this.commandFlags = inBuff[num];
			num = readingOffset;
			readingOffset = num + 1;
			this.reservedByte = inBuff[num];
			Protocol.Deserialize(out this.Size, inBuff, ref readingOffset);
			Protocol.Deserialize(out this.reliableSequenceNumber, inBuff, ref readingOffset);
			peer.bytesIn += (long)this.Size;
			int num2 = 0;
			switch (this.commandType)
			{
			case 1:
			case 16:
				Protocol.Deserialize(out this.ackReceivedReliableSequenceNumber, inBuff, ref readingOffset);
				Protocol.Deserialize(out this.ackReceivedSentTime, inBuff, ref readingOffset);
				goto IL_1DF;
			case 3:
			{
				short peerID;
				Protocol.Deserialize(out peerID, inBuff, ref readingOffset);
				readingOffset += 30;
				bool flag = peer.peerID == -1 || peer.peerID == -2;
				if (flag)
				{
					peer.peerID = peerID;
				}
				goto IL_1DF;
			}
			case 6:
			case 14:
				num2 = this.Size - 12;
				goto IL_1DF;
			case 7:
				Protocol.Deserialize(out this.unreliableSequenceNumber, inBuff, ref readingOffset);
				num2 = this.Size - 16;
				goto IL_1DF;
			case 8:
			case 15:
				Protocol.Deserialize(out this.startSequenceNumber, inBuff, ref readingOffset);
				Protocol.Deserialize(out this.fragmentCount, inBuff, ref readingOffset);
				Protocol.Deserialize(out this.fragmentNumber, inBuff, ref readingOffset);
				Protocol.Deserialize(out this.totalLength, inBuff, ref readingOffset);
				Protocol.Deserialize(out this.fragmentOffset, inBuff, ref readingOffset);
				num2 = this.Size - 32;
				this.fragmentsRemaining = this.fragmentCount;
				goto IL_1DF;
			case 11:
				Protocol.Deserialize(out this.unsequencedGroupNumber, inBuff, ref readingOffset);
				num2 = this.Size - 16;
				goto IL_1DF;
			}
			readingOffset += this.Size - 12;
			IL_1DF:
			bool flag2 = num2 != 0;
			if (flag2)
			{
				StreamBuffer streamBuffer = PeerBase.MessageBufferPoolGet();
				streamBuffer.Write(inBuff, readingOffset, num2);
				this.Payload = streamBuffer;
				this.Payload.Position = 0;
				readingOffset += num2;
			}
		}

		public void Reset()
		{
			this.commandFlags = 0;
			this.commandType = 0;
			this.commandChannelID = 0;
			this.reliableSequenceNumber = 0;
			this.unreliableSequenceNumber = 0;
			this.unsequencedGroupNumber = 0;
			this.reservedByte = 4;
			this.startSequenceNumber = 0;
			this.fragmentCount = 0;
			this.fragmentNumber = 0;
			this.totalLength = 0;
			this.fragmentOffset = 0;
			this.fragmentsRemaining = 0;
			this.commandSentTime = 0;
			this.commandSentCount = 0;
			this.roundTripTimeout = 0;
			this.timeoutTime = 0;
			this.ackReceivedReliableSequenceNumber = 0;
			this.ackReceivedSentTime = 0;
			this.Size = 0;
		}

		internal void SerializeHeader(byte[] buffer, ref int bufferIndex)
		{
			int num = bufferIndex;
			bufferIndex = num + 1;
			buffer[num] = this.commandType;
			num = bufferIndex;
			bufferIndex = num + 1;
			buffer[num] = this.commandChannelID;
			num = bufferIndex;
			bufferIndex = num + 1;
			buffer[num] = this.commandFlags;
			num = bufferIndex;
			bufferIndex = num + 1;
			buffer[num] = this.reservedByte;
			Protocol.Serialize(this.Size, buffer, ref bufferIndex);
			Protocol.Serialize(this.reliableSequenceNumber, buffer, ref bufferIndex);
			bool flag = this.commandType == 7;
			if (flag)
			{
				Protocol.Serialize(this.unreliableSequenceNumber, buffer, ref bufferIndex);
			}
			else
			{
				bool flag2 = this.commandType == 11;
				if (flag2)
				{
					Protocol.Serialize(this.unsequencedGroupNumber, buffer, ref bufferIndex);
				}
				else
				{
					bool flag3 = this.commandType == 8 || this.commandType == 15;
					if (flag3)
					{
						Protocol.Serialize(this.startSequenceNumber, buffer, ref bufferIndex);
						Protocol.Serialize(this.fragmentCount, buffer, ref bufferIndex);
						Protocol.Serialize(this.fragmentNumber, buffer, ref bufferIndex);
						Protocol.Serialize(this.totalLength, buffer, ref bufferIndex);
						Protocol.Serialize(this.fragmentOffset, buffer, ref bufferIndex);
					}
				}
			}
		}

		internal byte[] Serialize()
		{
			return this.Payload.GetBuffer();
		}

		public void FreePayload()
		{
			bool flag = this.Payload != null;
			if (flag)
			{
				PeerBase.MessageBufferPoolPut(this.Payload);
			}
			this.Payload = null;
		}

		public void Release()
		{
			this.returnPool.Release(this);
		}

		public int CompareTo(NCommand other)
		{
			bool flag = other == null;
			int result;
			if (flag)
			{
				result = 1;
			}
			else
			{
				int num = this.reliableSequenceNumber - other.reliableSequenceNumber;
				bool flag2 = this.IsFlaggedReliable || num != 0;
				if (flag2)
				{
					result = num;
				}
				else
				{
					result = this.unreliableSequenceNumber - other.unreliableSequenceNumber;
				}
			}
			return result;
		}

		public override string ToString()
		{
			bool flag = this.commandType == 1 || this.commandType == 16;
			string result;
			if (flag)
			{
				result = string.Format("CMD({1} ack for ch#/sq#/time: {0}/{2}/{3})", new object[]
				{
					this.commandChannelID,
					this.commandType,
					this.ackReceivedReliableSequenceNumber,
					this.ackReceivedSentTime
				});
			}
			else
			{
				result = string.Format("CMD({1} ch#/sq#/usq#: {0}/{2}/{3} r#/st/tt/rt:{5}/{4}/{6}/{7})", new object[]
				{
					this.commandChannelID,
					this.commandType,
					this.reliableSequenceNumber,
					this.unreliableSequenceNumber,
					this.commandSentTime,
					this.commandSentCount,
					this.timeoutTime,
					this.roundTripTimeout
				});
			}
			return result;
		}

		internal const byte FV_UNRELIABLE = 0;

		internal const byte FV_RELIABLE = 1;

		internal const byte FV_UNRELIABLE_UNSEQUENCED = 2;

		internal const byte FV_RELIBALE_UNSEQUENCED = 3;

		internal const byte CT_NONE = 0;

		internal const byte CT_ACK = 1;

		internal const byte CT_CONNECT = 2;

		internal const byte CT_VERIFYCONNECT = 3;

		internal const byte CT_DISCONNECT = 4;

		internal const byte CT_PING = 5;

		internal const byte CT_SENDRELIABLE = 6;

		internal const byte CT_SENDUNRELIABLE = 7;

		internal const byte CT_SENDFRAGMENT = 8;

		internal const byte CT_SENDUNSEQUENCED = 11;

		internal const byte CT_EG_SERVERTIME = 12;

		internal const byte CT_EG_SEND_UNRELIABLE_PROCESSED = 13;

		internal const byte CT_EG_SEND_RELIABLE_UNSEQUENCED = 14;

		internal const byte CT_EG_SEND_FRAGMENT_UNSEQUENCED = 15;

		internal const byte CT_EG_ACK_UNSEQUENCED = 16;

		internal const int HEADER_UDP_PACK_LENGTH = 12;

		internal const int CmdSizeMinimum = 12;

		internal const int CmdSizeAck = 20;

		internal const int CmdSizeConnect = 44;

		internal const int CmdSizeVerifyConnect = 44;

		internal const int CmdSizeDisconnect = 12;

		internal const int CmdSizePing = 12;

		internal const int CmdSizeReliableHeader = 12;

		internal const int CmdSizeUnreliableHeader = 16;

		internal const int CmdSizeUnsequensedHeader = 16;

		internal const int CmdSizeFragmentHeader = 32;

		internal const int CmdSizeMaxHeader = 36;

		internal byte commandFlags;

		internal byte commandType;

		internal byte commandChannelID;

		internal int reliableSequenceNumber;

		internal int unreliableSequenceNumber;

		internal int unsequencedGroupNumber;

		internal byte reservedByte = 4;

		internal int startSequenceNumber;

		internal int fragmentCount;

		internal int fragmentNumber;

		internal int totalLength;

		internal int fragmentOffset;

		internal int fragmentsRemaining;

		internal int commandSentTime;

		internal byte commandSentCount;

		internal int roundTripTimeout;

		internal int timeoutTime;

		internal int ackReceivedReliableSequenceNumber;

		internal int ackReceivedSentTime;

		internal int Size;

		internal StreamBuffer Payload;

		internal NCommandPool returnPool;
	}
}
