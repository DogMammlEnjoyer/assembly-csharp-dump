using System;
using System.Globalization;
using Unity.Properties;

namespace UnityEngine.UIElements
{
	[Serializable]
	public struct Length : IEquatable<Length>
	{
		public static Length Pixels(float value)
		{
			return new Length(value, LengthUnit.Pixel);
		}

		public static Length Percent(float value)
		{
			return new Length(value, LengthUnit.Percent);
		}

		public static Length Auto()
		{
			return new Length(0f, Length.Unit.Auto);
		}

		public static Length None()
		{
			return new Length(0f, Length.Unit.None);
		}

		public float value
		{
			get
			{
				return this.m_Value;
			}
			set
			{
				this.m_Value = Mathf.Clamp(value, -8388608f, 8388608f);
			}
		}

		public LengthUnit unit
		{
			get
			{
				return (LengthUnit)this.m_Unit;
			}
			set
			{
				this.m_Unit = (Length.Unit)value;
			}
		}

		public bool IsAuto()
		{
			return this.m_Unit == Length.Unit.Auto;
		}

		public bool IsNone()
		{
			return this.m_Unit == Length.Unit.None;
		}

		public Length(float value)
		{
			this = new Length(value, Length.Unit.Pixel);
		}

		public Length(float value, LengthUnit unit)
		{
			this = new Length(value, (Length.Unit)unit);
		}

		private Length(float value, Length.Unit unit)
		{
			this = default(Length);
			this.value = value;
			this.m_Unit = unit;
		}

		public static implicit operator Length(float value)
		{
			return new Length(value, LengthUnit.Pixel);
		}

		public static bool operator ==(Length lhs, Length rhs)
		{
			return lhs.m_Value == rhs.m_Value && lhs.m_Unit == rhs.m_Unit;
		}

		public static bool operator !=(Length lhs, Length rhs)
		{
			return !(lhs == rhs);
		}

		public bool Equals(Length other)
		{
			return other == this;
		}

		public override bool Equals(object obj)
		{
			bool result;
			if (obj is Length)
			{
				Length other = (Length)obj;
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
			case Length.Unit.Pixel:
			{
				bool flag = !Mathf.Approximately(0f, this.value);
				if (flag)
				{
					str2 = "px";
				}
				break;
			}
			case Length.Unit.Percent:
				str2 = "%";
				break;
			case Length.Unit.Auto:
				str = "auto";
				break;
			case Length.Unit.None:
				str = "none";
				break;
			}
			return str + str2;
		}

		internal static Length ParseString(string str, Length defaultValue = default(Length))
		{
			bool flag = string.IsNullOrEmpty(str);
			Length result;
			if (flag)
			{
				result = defaultValue;
			}
			else
			{
				str = str.ToLowerInvariant().Trim();
				Length length = defaultValue;
				bool flag2 = char.IsLetter(str[0]);
				if (flag2)
				{
					bool flag3 = str == "auto";
					if (flag3)
					{
						length = Length.Auto();
					}
					else
					{
						bool flag4 = str == "none";
						if (flag4)
						{
							length = Length.None();
						}
					}
				}
				else
				{
					int num = 0;
					int num2 = -1;
					int i = 0;
					while (i < str.Length)
					{
						char c = str[i];
						bool flag5 = char.IsNumber(c) || c == '.';
						if (flag5)
						{
							num++;
							i++;
						}
						else
						{
							bool flag6 = char.IsLetter(c) || c == '%';
							if (flag6)
							{
								num2 = i;
								break;
							}
							return defaultValue;
						}
					}
					string s = str.Substring(0, num);
					string text = string.Empty;
					bool flag7 = num2 > 0;
					if (flag7)
					{
						text = str.Substring(num2, str.Length - num2);
					}
					else
					{
						text = "px";
					}
					float value = defaultValue.value;
					LengthUnit unit = defaultValue.unit;
					float num3;
					bool flag8 = float.TryParse(s, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, CultureInfo.InvariantCulture.NumberFormat, out num3);
					if (flag8)
					{
						value = num3;
					}
					string text2 = text;
					string a = text2;
					if (!(a == "px"))
					{
						if (a == "%")
						{
							unit = LengthUnit.Percent;
						}
					}
					else
					{
						unit = LengthUnit.Pixel;
					}
					length = new Length(value, unit);
				}
				result = length;
			}
			return result;
		}

		internal const float k_MaxValue = 8388608f;

		[SerializeField]
		private float m_Value;

		[SerializeField]
		private Length.Unit m_Unit;

		private enum Unit
		{
			Pixel,
			Percent,
			Auto,
			None
		}

		internal class PropertyBag : ContainerPropertyBag<Length>
		{
			public PropertyBag()
			{
				base.AddProperty<float>(new Length.PropertyBag.ValueProperty());
				base.AddProperty<LengthUnit>(new Length.PropertyBag.UnitProperty());
			}

			private class ValueProperty : Property<Length, float>
			{
				public override string Name { get; } = "value";

				public override bool IsReadOnly { get; } = 0;

				public override float GetValue(ref Length container)
				{
					return container.value;
				}

				public override void SetValue(ref Length container, float value)
				{
					container.value = value;
				}
			}

			private class UnitProperty : Property<Length, LengthUnit>
			{
				public override string Name { get; } = "unit";

				public override bool IsReadOnly { get; } = 0;

				public override LengthUnit GetValue(ref Length container)
				{
					return container.unit;
				}

				public override void SetValue(ref Length container, LengthUnit value)
				{
					container.unit = value;
				}
			}
		}
	}
}
