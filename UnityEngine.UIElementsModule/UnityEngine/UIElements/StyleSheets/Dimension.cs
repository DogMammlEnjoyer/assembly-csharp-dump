using System;
using System.Globalization;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements.StyleSheets
{
	[VisibleToOtherModules(new string[]
	{
		"UnityEditor.UIBuilderModule"
	})]
	[Serializable]
	internal struct Dimension : IEquatable<Dimension>
	{
		public Dimension(float value, Dimension.Unit unit)
		{
			this.unit = unit;
			this.value = value;
		}

		public Length ToLength()
		{
			LengthUnit lengthUnit = (this.unit == Dimension.Unit.Percent) ? LengthUnit.Percent : LengthUnit.Pixel;
			return new Length(this.value, lengthUnit);
		}

		public TimeValue ToTime()
		{
			TimeUnit timeUnit = (this.unit == Dimension.Unit.Millisecond) ? TimeUnit.Millisecond : TimeUnit.Second;
			return new TimeValue(this.value, timeUnit);
		}

		public Angle ToAngle()
		{
			Angle result;
			switch (this.unit)
			{
			case Dimension.Unit.Degree:
				result = new Angle(this.value, AngleUnit.Degree);
				break;
			case Dimension.Unit.Gradian:
				result = new Angle(this.value, AngleUnit.Gradian);
				break;
			case Dimension.Unit.Radian:
				result = new Angle(this.value, AngleUnit.Radian);
				break;
			case Dimension.Unit.Turn:
				result = new Angle(this.value, AngleUnit.Turn);
				break;
			default:
				result = new Angle(this.value, AngleUnit.Degree);
				break;
			}
			return result;
		}

		public static bool operator ==(Dimension lhs, Dimension rhs)
		{
			return lhs.value == rhs.value && lhs.unit == rhs.unit;
		}

		public static bool operator !=(Dimension lhs, Dimension rhs)
		{
			return !(lhs == rhs);
		}

		public bool Equals(Dimension other)
		{
			return other == this;
		}

		public override bool Equals(object obj)
		{
			bool flag = !(obj is Dimension);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				Dimension lhs = (Dimension)obj;
				result = (lhs == this);
			}
			return result;
		}

		public override int GetHashCode()
		{
			int num = -799583767;
			num = num * -1521134295 + this.unit.GetHashCode();
			return num * -1521134295 + this.value.GetHashCode();
		}

		public override string ToString()
		{
			string str = string.Empty;
			switch (this.unit)
			{
			case Dimension.Unit.Unitless:
				str = string.Empty;
				break;
			case Dimension.Unit.Pixel:
				str = "px";
				break;
			case Dimension.Unit.Percent:
				str = "%";
				break;
			case Dimension.Unit.Second:
				str = "s";
				break;
			case Dimension.Unit.Millisecond:
				str = "ms";
				break;
			case Dimension.Unit.Degree:
				str = "deg";
				break;
			case Dimension.Unit.Gradian:
				str = "grad";
				break;
			case Dimension.Unit.Radian:
				str = "rad";
				break;
			case Dimension.Unit.Turn:
				str = "turn";
				break;
			}
			return this.value.ToString(CultureInfo.InvariantCulture.NumberFormat) + str;
		}

		public bool IsLength()
		{
			Dimension.Unit unit = this.unit;
			return unit == Dimension.Unit.Pixel || unit == Dimension.Unit.Percent;
		}

		public bool IsTimeValue()
		{
			Dimension.Unit unit = this.unit;
			return unit == Dimension.Unit.Millisecond || unit == Dimension.Unit.Second;
		}

		public bool IsAngle()
		{
			Dimension.Unit unit = this.unit;
			return unit == Dimension.Unit.Degree || unit == Dimension.Unit.Gradian || unit == Dimension.Unit.Radian || unit == Dimension.Unit.Turn;
		}

		public bool IsUnitless()
		{
			return this.unit == Dimension.Unit.Unitless;
		}

		public Dimension.Unit unit;

		public float value;

		public enum Unit
		{
			Unitless,
			Pixel,
			Percent,
			Second,
			Millisecond,
			Degree,
			Gradian,
			Radian,
			Turn
		}
	}
}
