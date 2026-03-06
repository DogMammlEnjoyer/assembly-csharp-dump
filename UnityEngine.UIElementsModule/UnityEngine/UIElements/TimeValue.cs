using System;
using System.Globalization;
using Unity.Properties;

namespace UnityEngine.UIElements
{
	public struct TimeValue : IEquatable<TimeValue>
	{
		public static TimeValue Seconds(float value)
		{
			return new TimeValue(value, TimeUnit.Second);
		}

		public static TimeValue Milliseconds(float value)
		{
			return new TimeValue(value, TimeUnit.Millisecond);
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

		public TimeUnit unit
		{
			get
			{
				return this.m_Unit;
			}
			set
			{
				this.m_Unit = value;
			}
		}

		public TimeValue(float value)
		{
			this = new TimeValue(value, TimeUnit.Second);
		}

		public TimeValue(float value, TimeUnit unit)
		{
			this.m_Value = value;
			this.m_Unit = unit;
		}

		public static implicit operator TimeValue(float value)
		{
			return new TimeValue(value, TimeUnit.Second);
		}

		public static bool operator ==(TimeValue lhs, TimeValue rhs)
		{
			return lhs.m_Value == rhs.m_Value && lhs.m_Unit == rhs.m_Unit;
		}

		public static bool operator !=(TimeValue lhs, TimeValue rhs)
		{
			return !(lhs == rhs);
		}

		public bool Equals(TimeValue other)
		{
			return other == this;
		}

		public override bool Equals(object obj)
		{
			bool result;
			if (obj is TimeValue)
			{
				TimeValue other = (TimeValue)obj;
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
			TimeUnit unit = this.unit;
			TimeUnit timeUnit = unit;
			if (timeUnit != TimeUnit.Second)
			{
				if (timeUnit == TimeUnit.Millisecond)
				{
					str2 = "ms";
				}
			}
			else
			{
				str2 = "s";
			}
			return str + str2;
		}

		private float m_Value;

		private TimeUnit m_Unit;

		internal class PropertyBag : ContainerPropertyBag<TimeValue>
		{
			public PropertyBag()
			{
				base.AddProperty<float>(new TimeValue.PropertyBag.ValueProperty());
				base.AddProperty<TimeUnit>(new TimeValue.PropertyBag.UnitProperty());
			}

			private class ValueProperty : Property<TimeValue, float>
			{
				public override string Name { get; } = "value";

				public override bool IsReadOnly { get; } = 0;

				public override float GetValue(ref TimeValue container)
				{
					return container.value;
				}

				public override void SetValue(ref TimeValue container, float value)
				{
					container.value = value;
				}
			}

			private class UnitProperty : Property<TimeValue, TimeUnit>
			{
				public override string Name { get; } = "unit";

				public override bool IsReadOnly { get; } = 0;

				public override TimeUnit GetValue(ref TimeValue container)
				{
					return container.unit;
				}

				public override void SetValue(ref TimeValue container, TimeUnit value)
				{
					container.unit = value;
				}
			}
		}
	}
}
