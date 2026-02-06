using System;

namespace Fusion
{
	internal struct SimulationPacketEnvelope
	{
		public unsafe void AddObjectPacketData(Simulation sim, NetworkId id, Tick tick, NetworkObjectPacketFlags flags)
		{
			Assert.Check(this.ObjectDataCapacity >= 64);
			bool flag = this.ObjectDataCount == this.ObjectDataCapacity;
			if (flag)
			{
				this.ObjectData = sim.TempDoubleArray<NetworkObjectPacketData>(ref this.ObjectData, this.ObjectDataCapacity);
				this.ObjectDataCapacity *= 2;
			}
			this.ObjectData[this.ObjectDataCount].Id = id;
			this.ObjectData[this.ObjectDataCount].ResetTick = tick;
			this.ObjectData[this.ObjectDataCount].Flags = flags;
			this.ObjectDataCount++;
			Assert.Check(this.ObjectDataCount <= this.ObjectDataCapacity);
		}

		internal unsafe static void Free(Simulation sim, ref SimulationPacketEnvelope* envelope)
		{
			bool flag = envelope == (IntPtr)((UIntPtr)0);
			if (!flag)
			{
				Assert.Check(envelope.Messages.Count == 0);
				Assert.Check(envelope.Messages.Head == null);
				Assert.Check(envelope.Messages.Tail == null);
				bool flag2 = envelope.ObjectData != null;
				if (flag2)
				{
					sim.TempFree<NetworkObjectPacketData>(ref envelope.ObjectData);
				}
				sim.TempFree<SimulationPacketEnvelope>(ref envelope);
			}
		}

		internal unsafe static SimulationPacketEnvelope* Alloc(Simulation sim)
		{
			SimulationPacketEnvelope* ptr = sim.TempAlloc<SimulationPacketEnvelope>();
			ptr->ObjectData = sim.TempAllocArray<NetworkObjectPacketData>(64);
			ptr->ObjectDataCount = 0;
			ptr->ObjectDataCapacity = 64;
			return ptr;
		}

		private const int MIN_CAPACITY = 64;

		public Tick Tick;

		public SimulationMessageList Messages;

		public unsafe NetworkObjectPacketData* ObjectData;

		public int ObjectDataCount;

		public int ObjectDataCapacity;
	}
}
