using System;
using System.Collections.Generic;
using Fusion.Sockets;

namespace Fusion
{
	public class SimulationInput
	{
		public PlayerRef Player
		{
			get
			{
				Assert.Check(!this._pooled);
				return this._player;
			}
			set
			{
				Assert.Check(!this._pooled);
				this._player = value;
			}
		}

		public unsafe SimulationInputHeader* Header
		{
			get
			{
				Assert.Check(!this._pooled);
				return (SimulationInputHeader*)this._ptr;
			}
		}

		public unsafe int* Data
		{
			get
			{
				Assert.Check(!this._pooled);
				return this._ptr + 4;
			}
		}

		public int Sent
		{
			get
			{
				Assert.Check(!this._pooled);
				return this._sent;
			}
			set
			{
				Assert.Check(!this._pooled);
				this._sent = value;
			}
		}

		public unsafe void Clear(int wordCount)
		{
			Assert.Check(!this._pooled);
			Native.MemClear((void*)this._ptr, wordCount * 4);
		}

		public unsafe void CopyFrom(SimulationInput source, int wordCount)
		{
			Assert.Check(!this._pooled);
			Native.MemCpy((void*)this._ptr, (void*)source._ptr, wordCount * 4);
		}

		internal unsafe void Serialize(SimulationInput previous, SimulationConfig config, NetBitBufferSerializer serializer)
		{
			Assert.Check(!this._pooled);
			int* ptr = this._ptr;
			int* ptr2 = previous._ptr;
			bool flag = config.Topology == Topologies.Shared;
			if (flag)
			{
				bool writing = serializer.Writing;
				if (writing)
				{
					serializer.Buffer->WriteInt32VarLength(this.Header->Tick, 8);
				}
				else
				{
					this.Header->Tick = serializer.Buffer->ReadInt32VarLength(8);
				}
			}
			else
			{
				bool writing2 = serializer.Writing;
				if (writing2)
				{
					for (int i = 0; i < config.InputTotalWordCount; i++)
					{
						bool flag2 = ptr[i] != ptr2[i];
						if (flag2)
						{
							serializer.Buffer->WriteBoolean(true);
							serializer.Buffer->WriteInt32VarLength(i, 8);
							serializer.Buffer->WriteInt64VarLength(Maths.ZigZagEncode((long)ptr[i] - (long)ptr2[i]), 8);
						}
					}
					serializer.Buffer->WriteBoolean(false);
				}
				else
				{
					Native.MemCpy((void*)ptr, (void*)ptr2, config.InputTotalWordCount * 4);
					while (serializer.Buffer->ReadBoolean())
					{
						ptr[serializer.Buffer->ReadInt32VarLength(8)] += (int)Maths.ZigZagDecode(serializer.Buffer->ReadInt64VarLength(8));
					}
				}
			}
		}

		internal void Dispose(Allocator allocator)
		{
			Allocator.Free<int>(allocator, ref this._ptr);
		}

		private int _sent;

		private bool _pooled;

		private PlayerRef _player;

		internal unsafe int* _ptr;

		internal SimulationInput Prev;

		internal SimulationInput Next;

		public class Buffer
		{
			public int Count
			{
				get
				{
					return this._map.Count;
				}
			}

			public bool Full
			{
				get
				{
					return this._map.Count == this._rate.Client;
				}
			}

			public Buffer(NetworkProjectConfig cfg)
			{
				this._cfg = cfg;
				this._rate = TickRate.Resolve(this._cfg.Simulation.TickRateSelection);
				this._map = new Dictionary<Tick, SimulationInput>(new Tick.EqualityComparer());
				this._time = new Dictionary<Tick, double>();
			}

			public void Clear()
			{
				this._map.Clear();
				this._time.Clear();
			}

			public int CopySortedTo(SimulationInput[] array)
			{
				Assert.Always(array.Length >= this.Count, "Array too small");
				Array.Clear(array, 0, array.Length);
				this._map.Values.CopyTo(array, 0);
				ArraySpecialized.Sort(array, 0, this._map.Count);
				for (int i = 0; i < this._map.Count; i++)
				{
					Assert.Check(array[i]);
				}
				return this._map.Count;
			}

			public bool Contains(Tick tick)
			{
				return this._map.ContainsKey(tick);
			}

			public unsafe bool Remove(Tick tick, out SimulationInput removed)
			{
				this._time.Remove(tick);
				bool flag = this._map.TryGetValue(tick, out removed);
				if (flag)
				{
					this._lastUsedInputHeaderData.Tick = removed.Header->Tick;
					this._lastUsedInputHeaderData.InterpFrom = removed.Header->InterpFrom;
					this._lastUsedInputHeaderData.InterpTo = removed.Header->InterpTo;
					this._lastUsedInputHeaderData.InterpAlpha = removed.Header->InterpAlpha;
				}
				return this._map.Remove(tick);
			}

			public double? GetInsertTime(Tick tick)
			{
				double value;
				bool flag = this._time.TryGetValue(tick, out value);
				double? result;
				if (flag)
				{
					result = new double?(value);
				}
				else
				{
					result = null;
				}
				return result;
			}

			public SimulationInput Get(Tick tick)
			{
				SimulationInput simulationInput;
				bool flag = this._map.TryGetValue(tick, out simulationInput);
				SimulationInput result;
				if (flag)
				{
					Assert.Check(simulationInput);
					result = simulationInput;
				}
				else
				{
					result = null;
				}
				return result;
			}

			public SimulationInputHeader GetLastUsedInputHeader()
			{
				return this._lastUsedInputHeaderData;
			}

			public unsafe bool Add(SimulationInput input, double? insertTime = null)
			{
				Assert.Check(input);
				Assert.Always(this._map.Count < TickRate.Resolve(this._cfg.Simulation.TickRateSelection).Client, "_map.Count < _cfg.Simulation.TickRate");
				bool flag = this.Contains(input.Header->Tick);
				bool result;
				if (flag)
				{
					result = false;
				}
				else
				{
					this._map.Add(input.Header->Tick, input);
					bool flag2 = insertTime != null;
					if (flag2)
					{
						this._time.Add(input.Header->Tick, insertTime.Value);
					}
					result = true;
				}
				return result;
			}

			private NetworkProjectConfig _cfg;

			private TickRate.Resolved _rate;

			private Dictionary<Tick, SimulationInput> _map;

			private Dictionary<Tick, double> _time;

			private SimulationInputHeader _lastUsedInputHeaderData;
		}

		internal class Pool
		{
			public Pool(SimulationConfig config, Allocator allocator)
			{
				this._pool = new Stack<SimulationInput>();
				this._created = new List<SimulationInput>();
				this._config = config;
				this._allocator = allocator;
				this._disposed = false;
			}

			public SimulationInput Acquire()
			{
				Assert.Check(!this._disposed);
				bool flag = this._pool.Count > 0;
				SimulationInput simulationInput;
				if (flag)
				{
					simulationInput = this._pool.Pop();
					Assert.Check(simulationInput._pooled);
					simulationInput._pooled = false;
				}
				else
				{
					simulationInput = new SimulationInput();
					simulationInput._ptr = Allocator.AllocAndClearArray<int>(this._allocator, this._config.InputTotalWordCount);
					this._created.Add(simulationInput);
				}
				Assert.Check(simulationInput._sent == 0);
				Assert.Check(!simulationInput._pooled);
				Assert.Check(simulationInput._player == PlayerRef.None);
				return simulationInput;
			}

			public unsafe void Release(SimulationInput input)
			{
				Assert.Check(!this._disposed);
				Assert.Check(!input._pooled);
				Native.MemClear((void*)input.Header, this._config.InputTotalWordCount * 4);
				input._sent = 0;
				input._pooled = true;
				input._player = default(PlayerRef);
				this._pool.Push(input);
			}

			public void Dispose()
			{
				this._disposed = true;
				for (int i = 0; i < this._created.Count; i++)
				{
					this._created[i].Dispose(this._allocator);
				}
				this._created = null;
				this._pool = null;
			}

			private Allocator _allocator;

			private Stack<SimulationInput> _pool;

			private List<SimulationInput> _created;

			private SimulationConfig _config;

			private bool _disposed;
		}
	}
}
