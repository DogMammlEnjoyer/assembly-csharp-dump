using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Fusion
{
	[NetworkStructWeaved(1)]
	[StructLayout(LayoutKind.Explicit)]
	public struct Angle : INetworkStruct, IEquatable<Angle>
	{
		public void Clamp(Angle min, Angle max)
		{
			Assert.Check(max._value >= min._value);
			bool flag = this._value < min._value;
			if (flag)
			{
				this._value = min._value;
			}
			bool flag2 = this._value > max._value;
			if (flag2)
			{
				this._value = max._value;
			}
		}

		public static Angle Min(Angle a, Angle b)
		{
			return (a._value < b._value) ? a : b;
		}

		public static Angle Max(Angle a, Angle b)
		{
			return (a._value > b._value) ? a : b;
		}

		public static Angle Lerp(Angle a, Angle b, float t)
		{
			bool flag = a._value == b._value;
			Angle result;
			if (flag)
			{
				result = a;
			}
			else
			{
				result = Mathf.LerpAngle((float)a, (float)b, t);
			}
			return result;
		}

		public static Angle Clamp(Angle value, Angle min, Angle max)
		{
			bool flag = max._value < min._value;
			if (flag)
			{
				Angle angle = max;
				max = min;
				min = angle;
			}
			bool flag2 = value._value < min._value;
			Angle result;
			if (flag2)
			{
				result = min;
			}
			else
			{
				bool flag3 = value._value > max._value;
				if (flag3)
				{
					result = max;
				}
				else
				{
					result = value;
				}
			}
			return result;
		}

		public static bool operator <(Angle a, Angle b)
		{
			return a._value < b._value;
		}

		public static bool operator <=(Angle a, Angle b)
		{
			return a._value <= b._value;
		}

		public static bool operator >(Angle a, Angle b)
		{
			return a._value > b._value;
		}

		public static bool operator >=(Angle a, Angle b)
		{
			return a._value >= b._value;
		}

		public static bool operator ==(Angle a, Angle b)
		{
			return a._value == b._value;
		}

		public static bool operator !=(Angle a, Angle b)
		{
			return a._value != b._value;
		}

		public bool Equals(Angle other)
		{
			return this._value == other._value;
		}

		public override bool Equals(object obj)
		{
			bool result;
			if (obj is Angle)
			{
				Angle other = (Angle)obj;
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
			return this._value;
		}

		public static Angle operator +(Angle a, Angle b)
		{
			Assert.Check(a._value >= 0 && a._value <= 3600000);
			Assert.Check(b._value >= 0 && b._value <= 3600000);
			a._value += b._value;
			bool flag = a._value > 3600000;
			if (flag)
			{
				a._value %= 3600000;
			}
			return a;
		}

		public static Angle operator -(Angle a, Angle b)
		{
			Assert.Check(a._value >= 0 && a._value <= 3600000);
			Assert.Check(b._value >= 0 && b._value <= 3600000);
			a._value -= b._value;
			bool flag = a._value < 0;
			if (flag)
			{
				Assert.Check(a._value >= -3600000);
				a._value = 3600000 + a._value;
			}
			return a;
		}

		public static explicit operator float(Angle value)
		{
			return (float)((double)value._value / 10000.0);
		}

		public static explicit operator double(Angle value)
		{
			return (double)value._value / 10000.0;
		}

		public static implicit operator Angle(double value)
		{
			bool flag = value > 360.0;
			if (flag)
			{
				value %= 360.0;
			}
			else
			{
				bool flag2 = value < 0.0;
				if (flag2)
				{
					bool flag3 = value < -360.0;
					if (flag3)
					{
						value = 360.0 + value % -360.0;
					}
					else
					{
						value = 360.0 + value;
					}
				}
			}
			Angle result;
			result._value = (int)(value * 10000.0 + 0.5);
			return result;
		}

		public static implicit operator Angle(float value)
		{
			return (double)value;
		}

		public static implicit operator Angle(int value)
		{
			bool flag = value > 360;
			if (flag)
			{
				value %= 360;
			}
			else
			{
				bool flag2 = value < 0;
				if (flag2)
				{
					bool flag3 = value < -360;
					if (flag3)
					{
						value = 360 + value % -360;
					}
					else
					{
						value = 360 + value;
					}
				}
			}
			Angle result;
			result._value = value * 10000;
			return result;
		}

		public override string ToString()
		{
			string text = (this._value % 10000).ToString();
			bool flag = text.Length < 4;
			if (flag)
			{
				text = new string('0', 4 - text.Length) + text;
			}
			return string.Format("[Angle:{0}.{1}]", this._value / 10000, text);
		}

		public const int SIZE = 4;

		private const int ACCURACY = 10000;

		private const int DECIMALS = 4;

		private const int _360 = 3600000;

		[FieldOffset(0)]
		private int _value;
	}
}
