using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using Fusion.Sockets;

namespace Fusion
{
	internal struct SimulationMessageEnvelope : ILogDumpable
	{
		private unsafe static int WriteInternal<[IsUnmanaged] T>(SimulationMessageEnvelope* envelope, T* buffer) where T : struct, ValueType, INetBitWriteStream
		{
			buffer->WriteInt32VarLength(envelope->Message->Offset, 10);
			buffer->WriteInt32VarLength(envelope->Message->Tick, 16);
			Assert.Check(!envelope->Message->Source.IsRealPlayer || !envelope->Message->Target.IsRealPlayer);
			PlayerRef.Write<T>(buffer, envelope->Message->Source);
			PlayerRef.Write<T>(buffer, envelope->Message->Target);
			int num = envelope->Message->Flags & -3;
			int num2 = num & 65535;
			int num3 = num >> 16;
			bool flag = buffer->WriteBoolean(num2 != 0);
			if (flag)
			{
				buffer->WriteInt32(num2, 16);
				bool flag2 = (num2 & 1) == 1;
				if (flag2)
				{
					bool flag3 = buffer->WriteBoolean(num3 != 0);
					if (flag3)
					{
						LogStream logError = InternalLogStreams.LogError;
						if (logError != null)
						{
							logError.Log("Trying to write user flags");
						}
						buffer->WriteInt32(num3, -12);
					}
				}
			}
			else
			{
				Assert.Check(num3 == 0, "User flags should only be used if FLAG_USER_MESSAGE is set");
			}
			bool flag4 = (envelope->Message->Flags & 8) != 8;
			if (flag4)
			{
				buffer->WriteUInt64VarLength(envelope->Sequence, 16);
			}
			int offsetBits = buffer->OffsetBits;
			bool flag5 = envelope->Message->Offset > 0;
			if (flag5)
			{
				int length = Maths.BytesRequiredForBits(envelope->Message->Offset);
				buffer->WriteBytesAligned(SimulationMessage.GetRawData(envelope->Message).Slice(0, length));
			}
			return offsetBits;
		}

		public unsafe static void Write(SimulationMessageEnvelope* envelope, NetBitBuffer* buffer)
		{
			int offsetBits = buffer->OffsetBits;
			int num = SimulationMessageEnvelope.WriteInternal<NetBitBuffer>(envelope, buffer);
			TraceLogStream logTraceSimulationMessage = InternalLogStreams.LogTraceSimulationMessage;
			if (logTraceSimulationMessage != null)
			{
				logTraceSimulationMessage.Log(string.Format("Wrote (header={0}, payload={1}) {2}", num - offsetBits, buffer->OffsetBits - num, LogUtils.GetDump<SimulationMessageEnvelope>(envelope)));
			}
		}

		public unsafe static int GetBitCount(SimulationMessageEnvelope* envelope, NetBitBuffer* buffer)
		{
			int offsetBits = buffer->OffsetBits;
			NetBitBufferNull netBitBufferNull = new NetBitBufferNull
			{
				OffsetBits = offsetBits
			};
			SimulationMessageEnvelope.WriteInternal<NetBitBufferNull>(envelope, &netBitBufferNull);
			return netBitBufferNull.OffsetBits - offsetBits;
		}

		[return: NotNull]
		public unsafe static SimulationMessageEnvelope* Read(Simulation sim, NetBitBuffer* buffer)
		{
			int num = buffer->ReadInt32VarLength(10);
			int tick = buffer->ReadInt32VarLength(16);
			PlayerRef source = PlayerRef.Read(buffer);
			PlayerRef target = PlayerRef.Read(buffer);
			int num2 = buffer->ReadBoolean() ? buffer->ReadInt32(16) : 0;
			bool flag = (num2 & 1) == 1;
			if (flag)
			{
				bool flag2 = buffer->ReadBoolean();
				bool flag3 = flag2;
				if (flag3)
				{
					LogStream logError = InternalLogStreams.LogError;
					if (logError != null)
					{
						logError.Log("Trying to read user flags");
					}
				}
				int num3 = flag2 ? buffer->ReadInt32(-12) : 0;
				num2 |= num3 << 16;
			}
			ulong sequence = ((num2 & 8) != 8) ? buffer->ReadUInt64VarLength(16) : 0UL;
			num2 |= 2;
			int capacityInBytes = Maths.BytesRequiredForBits(num);
			SimulationMessageEnvelope* ptr = SimulationMessageEnvelope.Allocate(sim, SimulationMessage.Allocate(sim, capacityInBytes), sequence);
			ptr->Message->Capacity = num;
			ptr->Message->Offset = 0;
			ptr->Message->Tick = tick;
			ptr->Message->Source = source;
			ptr->Message->Target = target;
			ptr->Message->Flags = num2;
			bool flag4 = ptr->Message->Capacity > 0;
			if (flag4)
			{
				buffer->ReadBytesAligned(SimulationMessage.GetRawData(ptr->Message));
			}
			TraceLogStream logTraceSimulationMessage = InternalLogStreams.LogTraceSimulationMessage;
			if (logTraceSimulationMessage != null)
			{
				logTraceSimulationMessage.Log(sim, string.Format("Read {0}", LogUtils.GetDump<SimulationMessageEnvelope>(ptr)));
			}
			return ptr;
		}

		[return: NotNull]
		public unsafe static SimulationMessageEnvelope* Allocate(Simulation sim, SimulationMessage* message, ulong sequence)
		{
			SimulationMessageEnvelope* ptr = sim.TempAlloc<SimulationMessageEnvelope>();
			ptr->Message = message;
			ptr->Sequence = sequence;
			ptr->Next = null;
			ptr->Prev = null;
			message->ReferenceCountAdd();
			return ptr;
		}

		public unsafe static void Free(Simulation sim, ref SimulationMessageEnvelope* envelope)
		{
			bool flag = envelope == (IntPtr)((UIntPtr)0);
			if (!flag)
			{
				Assert.Check(envelope.Prev == null);
				Assert.Check(envelope.Next == null);
				bool flag2 = envelope.Message != null;
				if (flag2)
				{
					bool flag3 = envelope.Message->ReferenceCountSub();
					if (flag3)
					{
						SimulationMessage.Free(sim, ref envelope.Message);
					}
				}
				sim.TempFree<SimulationMessageEnvelope>(ref envelope);
			}
		}

		public unsafe override string ToString()
		{
			return string.Format("[SimulationMessageEnvelope: {0}={1}, {2}={3}]", new object[]
			{
				"Sequence",
				this.Sequence,
				"Message",
				(this.Message != null) ? this.Message->ToString(false) : "null"
			});
		}

		void ILogDumpable.Dump(StringBuilder builder)
		{
			builder.Append(this.ToString());
			bool flag = this.Message != null;
			if (flag)
			{
				builder.Append("\n");
				builder.Append(SimulationMessage.DumpContents(this.Message));
			}
		}

		public ulong Sequence;

		public unsafe SimulationMessage* Message;

		public unsafe SimulationMessageEnvelope* Prev;

		public unsafe SimulationMessageEnvelope* Next;

		private const int OffsetBlockSize = 10;

		private const int TickBlockSize = 16;

		private const int SequenceBlockSize = 16;
	}
}
