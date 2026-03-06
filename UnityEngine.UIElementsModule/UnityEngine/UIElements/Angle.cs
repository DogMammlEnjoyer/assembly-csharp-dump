using System;
using System.Globalization;
using Unity.Properties;

namespace UnityEngine.UIElements
{
	public struct Angle : IEquatable<Angle>
	{
		public static Angle Degrees(float value)
		{
			return new Angle(value, AngleUnit.Degree);
		}

		public static Angle Gradians(float value)
		{
			return new Angle(value, AngleUnit.Gradian);
		}

		public static Angle Radians(float value)
		{
			return new Angle(value, AngleUnit.Radian);
		}

		public static Angle Turns(float value)
		{
			return new Angle(value, AngleUnit.Turn);
		}

		internal static Angle None()
		{
			return new Angle(0f, Angle.Unit.None);
		}

		public float value
		{
			get
			{
				return this.m_Value;
			}
			set
			{
				this.m_Value = value;
			}
		}

		public AngleUnit unit
		{
			get
			{
				return (AngleUnit)this.m_Unit;
			}
			set
			{
				this.m_Unit = (Angle.Unit)value;
			}
		}

		internal bool IsNone()
		{
			return this.m_Unit == Angle.Unit.None;
		}

		public Angle(float value)
		{
			this = new Angle(value, Angle.Unit.Degree);
		}

		public Angle(float value, AngleUnit unit)
		{
			this = new Angle(value, (Angle.Unit)unit);
		}

		private Angle(float value, Angle.Unit unit)
		{
			this.m_Value = value;
			this.m_Unit = unit;
		}

		public float ToDegrees()
		{
			float result;
			switch (this.m_Unit)
			{
			case Angle.Unit.Degree:
				result = this.m_Value;
				break;
			case Angle.Unit.Gradian:
				result = this.m_Value * 360f / 400f;
				break;
			case Angle.Unit.Radian:
				result = this.m_Value * 180f / 3.1415927f;
				break;
			case Angle.Unit.Turn:
				result = this.m_Value * 360f;
				break;
			case Angle.Unit.None:
				result = 0f;
				break;
			default:
				result = 0f;
				break;
			}
			return result;
		}

		public float ToGradians()
		{
			float result;
			switch (this.m_Unit)
			{
			case Angle.Unit.Degree:
				result = this.m_Value * 10f / 9f;
				break;
			case Angle.Unit.Gradian:
				result = this.m_Value;
				break;
			case Angle.Unit.Radian:
				result = this.m_Value * 200f / 3.1415927f;
				break;
			case Angle.Unit.Turn:
				result = this.m_Value * 400f;
				break;
			case Angle.Unit.None:
				result = 0f;
				break;
			default:
				result = 0f;
				break;
			}
			return result;
		}

		public float ToRadians()
		{
			float result;
			switch (this.m_Unit)
			{
			case Angle.Unit.Degree:
				result = this.m_Value * 3.1415927f / 180f;
				break;
			case Angle.Unit.Gradian:
				result = this.m_Value * 3.1415927f / 200f;
				break;
			case Angle.Unit.Radian:
				result = this.m_Value;
				break;
			case Angle.Unit.Turn:
				result = this.m_Value * 3.1415927f * 2f;
				break;
			case Angle.Unit.None:
				result = 0f;
				break;
			default:
				result = 0f;
				break;
			}
			return result;
		}

		public float ToTurns()
		{
			float result;
			switch (this.m_Unit)
			{
			case Angle.Unit.Degree:
				result = this.m_Value / 360f;
				break;
			case Angle.Unit.Gradian:
				result = this.m_Value / 400f;
				break;
			case Angle.Unit.Radian:
				result = this.m_Value / 6.2831855f;
				break;
			case Angle.Unit.Turn:
				result = this.m_Value;
				break;
			case Angle.Unit.None:
				result = 0f;
				break;
			default:
				result = 0f;
				break;
			}
			return result;
		}

		internal void ConvertTo(AngleUnit newUnit)
		{
			if (!true)
			{
			}
			float value;
			switch (newUnit)
			{
			case AngleUnit.Degree:
				value = this.ToDegrees();
				break;
			case AngleUnit.Gradian:
				value = this.ToGradians();
				break;
			case AngleUnit.Radian:
				value = this.ToRadians();
				break;
			case AngleUnit.Turn:
				value = this.ToTurns();
				break;
			default:
				throw new NotImplementedException();
			}
			if (!true)
			{
			}
			this.m_Value = value;
			this.m_Unit = (Angle.Unit)newUnit;
		}

		public static implicit operator Angle(float value)
		{
			return new Angle(value, AngleUnit.Degree);
		}

		public static bool operator ==(Angle lhs, Angle rhs)
		{
			return lhs.m_Value == rhs.m_Value && lhs.m_Unit == rhs.m_Unit;
		}

		public static bool operator !=(Angle lhs, Angle rhs)
		{
			return !(lhs == rhs);
		}

		public bool Equals(Angle other)
		{
			return other == this;
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
			return this.m_Value.GetHashCode() * 397 ^ (int)this.m_Unit;
		}

		public override string ToString()
		{
			string str = this.value.ToString(CultureInfo.InvariantCulture.NumberFormat);
			string str2 = string.Empty;
			switch (this.m_Unit)
			{
			case Angle.Unit.Degree:
			{
				bool flag = !Mathf.Approximately(0f, this.value);
				if (flag)
				{
					str2 = "deg";
				}
				break;
			}
			case Angle.Unit.Gradian:
				str2 = "grad";
				break;
			case Angle.Unit.Radian:
				str2 = "rad";
				break;
			case Angle.Unit.Turn:
				str2 = "turn";
				break;
			case Angle.Unit.None:
				str = "";
				break;
			}
			return str + str2;
		}

		private float m_Value;

		private Angle.Unit m_Unit;

		private enum Unit
		{
			Degree,
			Gradian,
			Radian,
			Turn,
			None
		}

		internal class PropertyBag : ContainerPropertyBag<Angle>
		{
			public PropertyBag()
			{
				base.AddProperty<float>(new Angle.PropertyBag.ValueProperty());
				base.AddProperty<AngleUnit>(new Angle.PropertyBag.UnitProperty());
			}

			private class ValueProperty : Property<Angle, float>
			{
				public override string Name { get; } = "value";

				public override bool IsReadOnly { get; } = 0;

				public override float GetValue(ref Angle container)
				{
					return container.value;
				}

				public override void SetValue(ref Angle container, float value)
				{
					container.value = value;
				}
			}

			private class UnitProperty : Property<Angle, AngleUnit>
			{
				public override string Name { get; } = "unit";

				public override bool IsReadOnly { get; } = 0;

				public override AngleUnit GetValue(ref Angle container)
				{
					return container.unit;
				}

				public override void SetValue(ref Angle container, AngleUnit value)
				{
					container.unit = value;
				}
			}
		}
	}
}
