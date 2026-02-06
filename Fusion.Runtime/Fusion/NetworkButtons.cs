using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;

namespace Fusion
{
	[NetworkStructWeaved(1)]
	[StructLayout(LayoutKind.Explicit)]
	public struct NetworkButtons : INetworkStruct, IEquatable<NetworkButtons>
	{
		public int Bits
		{
			get
			{
				return this._bits;
			}
		}

		public NetworkButtons(int buttons)
		{
			this._bits = buttons;
		}

		public bool IsSet(int button)
		{
			Assert.Check(button < 32);
			return (this._bits & 1 << button) != 0;
		}

		public void SetDown(int button)
		{
			Assert.Check(button < 32);
			this._bits |= 1 << button;
		}

		public void SetUp(int button)
		{
			Assert.Check(button < 32);
			this._bits &= ~(1 << button);
		}

		public void Set(int button, bool state)
		{
			Assert.Check(button < 32);
			if (state)
			{
				this.SetDown(button);
			}
			else
			{
				this.SetUp(button);
			}
		}

		public void SetAllUp()
		{
			this._bits = 0;
		}

		public void SetAllDown()
		{
			this._bits = -1;
		}

		public bool IsSet<[IsUnmanaged] T>(T button) where T : struct, ValueType, Enum
		{
			Assert.Check(typeof(T).GetEnumUnderlyingType() == typeof(int));
			return this.IsSet(UnsafeUtility.EnumToInt<T>(button));
		}

		public void SetDown<[IsUnmanaged] T>(T button) where T : struct, ValueType, Enum
		{
			Assert.Check(typeof(T).GetEnumUnderlyingType() == typeof(int));
			this.SetDown(UnsafeUtility.EnumToInt<T>(button));
		}

		public void SetUp<[IsUnmanaged] T>(T button) where T : struct, ValueType, Enum
		{
			Assert.Check(typeof(T).GetEnumUnderlyingType() == typeof(int));
			this.SetUp(UnsafeUtility.EnumToInt<T>(button));
		}

		public void Set<[IsUnmanaged] T>(T button, bool state) where T : struct, ValueType, Enum
		{
			Assert.Check(typeof(T).GetEnumUnderlyingType() == typeof(int));
			this.Set(UnsafeUtility.EnumToInt<T>(button), state);
		}

		public ValueTuple<NetworkButtons, NetworkButtons> GetPressedOrReleased(NetworkButtons previous)
		{
			return new ValueTuple<NetworkButtons, NetworkButtons>(this.GetPressed(previous), this.GetReleased(previous));
		}

		public NetworkButtons GetPressed(NetworkButtons previous)
		{
			previous._bits = ((previous._bits ^ this._bits) & this._bits);
			return previous;
		}

		public NetworkButtons GetReleased(NetworkButtons previous)
		{
			previous._bits = ((previous._bits ^ this._bits) & previous._bits);
			return previous;
		}

		public bool WasPressed(NetworkButtons previous, int button)
		{
			return this.GetPressed(previous).IsSet(button);
		}

		public bool WasReleased(NetworkButtons previous, int button)
		{
			return this.GetReleased(previous).IsSet(button);
		}

		public bool WasPressed<[IsUnmanaged] T>(NetworkButtons previous, T button) where T : struct, ValueType, Enum
		{
			return this.GetPressed(previous).IsSet<T>(button);
		}

		public bool WasReleased<[IsUnmanaged] T>(NetworkButtons previous, T button) where T : struct, ValueType, Enum
		{
			return this.GetReleased(previous).IsSet<T>(button);
		}

		public bool Equals(NetworkButtons other)
		{
			return this._bits == other._bits;
		}

		public override bool Equals(object obj)
		{
			bool result;
			if (obj is NetworkButtons)
			{
				NetworkButtons other = (NetworkButtons)obj;
				result = this.Equals(other);
			}
			else
			{
				result = false;
			}
			return result;
		}

		public override int GetHashCode()
		{
			return this._bits;
		}

		[FieldOffset(0)]
		private int _bits;
	}
}
