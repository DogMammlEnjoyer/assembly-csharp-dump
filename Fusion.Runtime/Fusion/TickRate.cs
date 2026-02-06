using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fusion
{
	[StructLayout(LayoutKind.Explicit)]
	public struct TickRate
	{
		public int Client
		{
			get
			{
				return this._rates.FixedElementField;
			}
		}

		public int Count
		{
			get
			{
				return this._count;
			}
		}

		public int this[int index]
		{
			get
			{
				return this.GetTickRate(index);
			}
		}

		private unsafe TickRate(params int[] rates)
		{
			Assert.Check(rates.Length >= 1 && rates.Length <= 4);
			this._count = rates.Length;
			for (int i = 0; i < rates.Length; i++)
			{
				*(ref this._rates.FixedElementField + (IntPtr)i * 4) = rates[i];
			}
		}

		public int GetDivisor(int index)
		{
			Assert.Check((ulong)index < (ulong)((long)this._count));
			Assert.Check(this.Client == this.Client / this.GetTickRate(index) * this.GetTickRate(index));
			return this.Client / this.GetTickRate(index);
		}

		public unsafe int GetTickRate(int index)
		{
			Assert.Check((ulong)index < (ulong)((long)this._count));
			return *(ref this._rates.FixedElementField + (IntPtr)index * 4);
		}

		public int[] ToArray()
		{
			int[] array = new int[this._count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = this.GetTickRate(i);
			}
			return array;
		}

		private bool Validate()
		{
			bool flag = this._count == 0;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = this._count > 4;
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = this._rates.FixedElementField <= 0;
					if (flag3)
					{
						result = false;
					}
					else
					{
						for (int i = 1; i < this._count; i++)
						{
							bool flag4 = this._rates.FixedElementField != this._rates.FixedElementField / this.GetTickRate(i) * this.GetTickRate(i);
							if (flag4)
							{
								return false;
							}
						}
						result = true;
					}
				}
			}
			return result;
		}

		public TickRate.Selection ClampSelection(TickRate.Selection selection)
		{
			bool flag = !this.Validate();
			TickRate.Selection result;
			if (flag)
			{
				result = default(TickRate.Selection);
			}
			else
			{
				selection.ServerIndex = Maths.Clamp(selection.ServerIndex, 0, this._count - 1);
				selection.ServerSendIndex = Maths.Clamp(selection.ServerSendIndex, selection.ServerIndex, this._count - 1);
				selection.ClientSendIndex = Maths.Clamp(selection.ClientSendIndex, 0, this._count - 1);
				result = selection;
			}
			return result;
		}

		public TickRate.ValidateResult ValidateSelection(TickRate.Selection selected)
		{
			bool flag = !this.Validate();
			TickRate.ValidateResult result;
			if (flag)
			{
				result = TickRate.ValidateResult.InvalidTickRate;
			}
			else
			{
				bool flag2 = this.Client != selected.Client;
				if (flag2)
				{
					result = TickRate.ValidateResult.Error;
				}
				else
				{
					bool flag3 = selected.ServerIndex >= this._count;
					if (flag3)
					{
						result = TickRate.ValidateResult.ServerIndexOutOfRange;
					}
					else
					{
						bool flag4 = selected.ServerSendIndex >= this._count;
						if (flag4)
						{
							result = TickRate.ValidateResult.ServerSendIndexOutOfRange;
						}
						else
						{
							bool flag5 = selected.ClientSendIndex >= this._count;
							if (flag5)
							{
								result = TickRate.ValidateResult.ClientSendIndexOutOfRange;
							}
							else
							{
								bool flag6 = selected.ServerSendIndex < selected.ServerIndex;
								if (flag6)
								{
									result = TickRate.ValidateResult.ServerSendRateLargerThanTickRate;
								}
								else
								{
									result = TickRate.ValidateResult.Ok;
								}
							}
						}
					}
				}
			}
			return result;
		}

		internal static TickRate.Selection Default
		{
			get
			{
				return new TickRate.Selection
				{
					Client = 64,
					ClientSendIndex = 1,
					ServerIndex = 0,
					ServerSendIndex = 1
				};
			}
		}

		internal static TickRate.Selection Shared
		{
			get
			{
				return new TickRate.Selection
				{
					Client = 32,
					ClientSendIndex = 1,
					ServerIndex = 0,
					ServerSendIndex = 1
				};
			}
		}

		static TickRate()
		{
			TickRate.Init();
		}

		private static void InitChecked()
		{
			bool flag = TickRate._valid != null && TickRate._valid.Length != 0 && TickRate._lookup != null && TickRate._lookup.Count > 0;
			if (!flag)
			{
				TickRate.Init();
			}
		}

		public static void Init()
		{
			TickRate._valid = new TickRate[]
			{
				new TickRate(new int[]
				{
					8,
					4
				}),
				new TickRate(new int[]
				{
					10,
					5
				}),
				new TickRate(new int[]
				{
					16,
					8,
					4
				}),
				new TickRate(new int[]
				{
					20,
					10,
					5
				}),
				new TickRate(new int[]
				{
					24,
					12,
					6
				}),
				new TickRate(new int[]
				{
					30,
					15
				}),
				new TickRate(new int[]
				{
					32,
					16,
					8
				}),
				new TickRate(new int[]
				{
					50,
					25
				}),
				new TickRate(new int[]
				{
					60,
					30,
					15
				}),
				new TickRate(new int[]
				{
					64,
					32,
					16
				}),
				new TickRate(new int[]
				{
					100,
					50,
					25
				}),
				new TickRate(new int[]
				{
					120,
					60,
					30
				}),
				new TickRate(new int[]
				{
					128,
					64,
					32
				}),
				new TickRate(new int[]
				{
					240,
					120,
					60,
					30
				}),
				new TickRate(new int[]
				{
					256,
					128,
					64,
					32
				})
			};
			TickRate._validReadOnly = new ReadOnlyCollection<TickRate>(TickRate._valid);
			TickRate._lookup = new Dictionary<int, TickRate>();
			foreach (TickRate value in TickRate._valid)
			{
				TickRate._lookup.Add(value.Client, value);
			}
		}

		public static bool IsValid(TickRate rate)
		{
			return TickRate.IsValid(rate.Client);
		}

		public static bool IsValid(int rate)
		{
			TickRate.InitChecked();
			return TickRate._lookup.ContainsKey(rate);
		}

		public static TickRate Get(int rate)
		{
			TickRate.InitChecked();
			TickRate result;
			bool flag = TickRate._lookup.TryGetValue(rate, out result);
			if (flag)
			{
				return result;
			}
			throw new InvalidOperationException("invalid tickrate");
		}

		public static TickRate.Resolved Resolve(TickRate.Selection selection)
		{
			TickRate.InitChecked();
			Assert.Always(TickRate.IsValid(selection.Client), "IsValid(selection.Client)");
			TickRate tickRate = TickRate._lookup[selection.Client];
			TickRate.ValidateResult validateResult = tickRate.ValidateSelection(selection);
			Assert.Always(validateResult == TickRate.ValidateResult.Ok, "result != ValidateResult.Ok");
			return new TickRate.Resolved(tickRate.Client, tickRate.GetTickRate(selection.ClientSendIndex), tickRate.GetTickRate(selection.ServerIndex), tickRate.GetTickRate(selection.ServerSendIndex));
		}

		public static IReadOnlyList<TickRate> Available
		{
			get
			{
				TickRate.InitChecked();
				return TickRate._validReadOnly;
			}
		}

		[FieldOffset(0)]
		private int _count;

		[FixedBuffer(typeof(int), 4)]
		[FieldOffset(4)]
		private TickRate.<_rates>e__FixedBuffer _rates;

		private static TickRate[] _valid;

		private static ReadOnlyCollection<TickRate> _validReadOnly;

		private static Dictionary<int, TickRate> _lookup;

		[Serializable]
		[StructLayout(LayoutKind.Explicit)]
		public struct Selection
		{
			[FieldOffset(0)]
			public int Client;

			[FieldOffset(4)]
			public int ServerIndex;

			[FieldOffset(8)]
			public int ClientSendIndex;

			[FieldOffset(12)]
			public int ServerSendIndex;
		}

		[StructLayout(LayoutKind.Explicit)]
		public struct Resolved
		{
			internal Resolved(int client, int clientSend, int server, int serverSend)
			{
				this.Client = client;
				this.ClientSend = clientSend;
				this.Server = server;
				this.ServerSend = serverSend;
			}

			public double ServerTickDelta
			{
				get
				{
					return TickRate.Resolved.Inverse(this.Server);
				}
			}

			public double ServerSendDelta
			{
				get
				{
					return TickRate.Resolved.Inverse(this.ServerSend);
				}
			}

			public int ServerTickStride
			{
				get
				{
					return this.Client / this.Server;
				}
			}

			public double ClientTickDelta
			{
				get
				{
					return TickRate.Resolved.Inverse(this.Client);
				}
			}

			public double ClientSendDelta
			{
				get
				{
					return TickRate.Resolved.Inverse(this.ClientSend);
				}
			}

			public int ClientTickStride
			{
				get
				{
					return 1;
				}
			}

			public override string ToString()
			{
				return string.Format("[ClientTickRate = {0}, ClientSendRate = {1}, ServerTickRate = {2}, ServerSendRate = {3}]", new object[]
				{
					this.Client,
					this.ClientSend,
					this.Server,
					this.ServerSend
				});
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private static double Inverse(int rate)
			{
				return (rate == 0) ? 0.0 : (1.0 / (double)rate);
			}

			public const int SIZE = 16;

			public const int WORDS = 4;

			[FieldOffset(0)]
			public int Client;

			[FieldOffset(4)]
			public int ClientSend;

			[FieldOffset(8)]
			public int Server;

			[FieldOffset(12)]
			public int ServerSend;
		}

		public enum ValidateResult
		{
			Ok,
			Error,
			NotFound,
			InvalidTickRate,
			ServerIndexOutOfRange,
			ClientSendIndexOutOfRange,
			ServerSendIndexOutOfRange,
			ServerSendRateLargerThanTickRate
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 16)]
		public struct <_rates>e__FixedBuffer
		{
			public int FixedElementField;
		}
	}
}
