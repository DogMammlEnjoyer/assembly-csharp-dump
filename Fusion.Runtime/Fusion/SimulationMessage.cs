using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Fusion
{
	[StructLayout(LayoutKind.Explicit)]
	public struct SimulationMessage : ILogDumpable
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReferenceCountAdd()
		{
			this.References++;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool ReferenceCountSub()
		{
			this.References--;
			Assert.Check(this.References >= 0);
			return this.References == 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetTarget(PlayerRef target)
		{
			this.Target = target;
			this.Flags |= (target.IsNone ? 32 : 16);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetStatic()
		{
			this.Flags |= 4;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetUnreliable()
		{
			this.Flags |= 8;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetNotTickAligned()
		{
			this.Flags |= 128;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetDummy()
		{
			this.Flags |= 256;
			this.Offset = 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool GetFlag(int flag)
		{
			return (this.Flags & flag) == flag;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsTargeted()
		{
			return (this.Flags & 48) != 0;
		}

		public bool IsUnreliable
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return (this.Flags & 8) == 8;
			}
		}

		public unsafe static SimulationMessage* Clone(Simulation sim, SimulationMessage* message)
		{
			int num = Maths.BytesRequiredForBits(message->Capacity);
			SimulationMessage* ptr = SimulationMessage.Allocate(sim, num);
			Native.MemCpy((void*)ptr, (void*)message, 28 + num);
			ptr->Tick = 0;
			ptr->References = 0;
			return ptr;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static void Free(Simulation sim, ref SimulationMessage* message)
		{
			TraceLogStream logTraceSimulationMessage = InternalLogStreams.LogTraceSimulationMessage;
			if (logTraceSimulationMessage != null)
			{
				logTraceSimulationMessage.Log(sim, string.Format("Freeing {0}", LogUtils.GetDump<SimulationMessage>(message)));
			}
			Assert.Always(message.References == 0, "Message is still referenced");
			sim.TempFree<SimulationMessage>(ref message);
		}

		[Obsolete("Use GetRawData instead")]
		public unsafe static byte* GetData(SimulationMessage* message)
		{
			return (byte*)(message + 28 / sizeof(SimulationMessage));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static Span<byte> GetRawData(SimulationMessage* message)
		{
			Assert.Check(sizeof(SimulationMessage) == 28);
			return new Span<byte>((void*)(message + 28 / sizeof(SimulationMessage)), Maths.BytesRequiredForBits(message->Capacity));
		}

		[return: NotNull]
		public unsafe static SimulationMessage* Allocate(Simulation sim, int capacityInBytes)
		{
			Assert.Check(sizeof(SimulationMessage) == 28);
			Assert.Always<int>(capacityInBytes >= 0 && capacityInBytes < 512, "Invalid capacity: {0}", capacityInBytes);
			SimulationMessage* ptr = (SimulationMessage*)sim.TempAlloc(28 + capacityInBytes);
			ptr->Capacity = capacityInBytes * 8;
			TraceLogStream logTraceSimulationMessage = InternalLogStreams.LogTraceSimulationMessage;
			if (logTraceSimulationMessage != null)
			{
				logTraceSimulationMessage.Log(sim, string.Format("Allocated SimulationMessage: {0} {1}", ptr->Capacity, LogUtils.GetDump<SimulationMessage>(ptr)));
			}
			return ptr;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool CanAllocateUserPayload(int capacityInBytes)
		{
			return capacityInBytes <= 512;
		}

		public override string ToString()
		{
			return this.ToString(true);
		}

		public string ToString(bool useBrackets)
		{
			return string.Format("{0}{1}={2}, {3}={4}, {5}={6}, {7}={8}, {9}={10}, {11}={12}, Flags={13}, UserFlags={14}{15}", new object[]
			{
				useBrackets ? "[SimulationMessage: " : "",
				"Tick",
				this.Tick,
				"Source",
				this.Source,
				"Capacity",
				this.Capacity,
				"Offset",
				this.Offset,
				"References",
				this.References,
				"Target",
				this.Target,
				(SimulationMessage.BuiltInFlags)(this.Flags & 65535),
				this.Flags & -65536,
				useBrackets ? "]" : ""
			});
		}

		internal unsafe static string DumpContents(SimulationMessage* message)
		{
			Span<byte> rawData = SimulationMessage.GetRawData(message);
			bool flag = message->GetFlag(1);
			string result;
			if (flag)
			{
				result = BinUtils.BytesToHex(rawData, Maths.BytesRequiredForBits(message->Capacity));
			}
			else
			{
				RpcHeader rpcHeader = rawData.Read<RpcHeader>();
				result = string.Format("{0} {1}", rpcHeader, BinUtils.BytesToHex(rawData, 16));
			}
			return result;
		}

		unsafe void ILogDumpable.Dump(StringBuilder builder)
		{
			builder.Append(this.ToString());
			builder.Append("\n");
			fixed (SimulationMessage* ptr = &this)
			{
				SimulationMessage* message = ptr;
				builder.Append(SimulationMessage.DumpContents(message));
			}
		}

		public const int SIZE = 28;

		public const int MAX_PAYLOAD_SIZE = 512;

		public const int FLAG_USER_MESSAGE = 1;

		public const int FLAG_REMOTE = 2;

		public const int FLAG_STATIC = 4;

		public const int FLAG_UNRELIABLE = 8;

		public const int FLAG_TARGET_PLAYER = 16;

		public const int FLAG_TARGET_SERVER = 32;

		public const int FLAG_INTERNAL = 64;

		public const int FLAG_NOT_TICK_ALIGNED = 128;

		public const int FLAG_DUMMY = 256;

		public const int FLAG_USER_FLAGS_START = 65536;

		public const int FLAGS_RESERVED = 65535;

		public const int FLAGS_RESERVED_BITS = 16;

		[FieldOffset(0)]
		public int Tick;

		[FieldOffset(4)]
		public PlayerRef Source;

		[FieldOffset(8)]
		public int Capacity;

		[FieldOffset(12)]
		public int Offset;

		[FieldOffset(16)]
		public int References;

		[FieldOffset(20)]
		public int Flags;

		[FieldOffset(24)]
		public PlayerRef Target;

		[Flags]
		private enum BuiltInFlags
		{
			USER_MESSAGE = 1,
			REMOTE = 2,
			STATIC = 4,
			UNRELIABLE = 8,
			TARGET_PLAYER = 16,
			TARGET_SERVER = 32,
			INTERNAL = 64,
			NOT_TICK_ALIGNED = 128,
			DUMMY = 256
		}
	}
}
