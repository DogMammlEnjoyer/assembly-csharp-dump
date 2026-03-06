using System;
using System.Collections.Generic;
using System.Reflection;

namespace UnityEngine.InputSystem.Utilities
{
	public struct NamedValue : IEquatable<NamedValue>
	{
		public string name { readonly get; set; }

		public PrimitiveValue value { readonly get; set; }

		public TypeCode type
		{
			get
			{
				return this.value.type;
			}
		}

		public NamedValue ConvertTo(TypeCode type)
		{
			return new NamedValue
			{
				name = this.name,
				value = this.value.ConvertTo(type)
			};
		}

		public static NamedValue From<TValue>(string name, TValue value) where TValue : struct
		{
			return new NamedValue
			{
				name = name,
				value = PrimitiveValue.From<TValue>(value)
			};
		}

		public override string ToString()
		{
			return string.Format("{0}={1}", this.name, this.value);
		}

		public bool Equals(NamedValue other)
		{
			return string.Equals(this.name, other.name, StringComparison.InvariantCultureIgnoreCase) && this.value == other.value;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj is NamedValue)
			{
				NamedValue other = (NamedValue)obj;
				return this.Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return ((this.name != null) ? this.name.GetHashCode() : 0) * 397 ^ this.value.GetHashCode();
		}

		public static bool operator ==(NamedValue left, NamedValue right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(NamedValue left, NamedValue right)
		{
			return !left.Equals(right);
		}

		public static NamedValue[] ParseMultiple(string parameterString)
		{
			if (parameterString == null)
			{
				throw new ArgumentNullException("parameterString");
			}
			parameterString = parameterString.Trim();
			if (string.IsNullOrEmpty(parameterString))
			{
				return null;
			}
			int num = parameterString.CountOccurrences(","[0]) + 1;
			NamedValue[] array = new NamedValue[num];
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				NamedValue namedValue = NamedValue.ParseParameter(parameterString, ref num2);
				array[i] = namedValue;
			}
			return array;
		}

		public static NamedValue Parse(string str)
		{
			int num = 0;
			return NamedValue.ParseParameter(str, ref num);
		}

		private static NamedValue ParseParameter(string parameterString, ref int index)
		{
			NamedValue result = default(NamedValue);
			int length = parameterString.Length;
			while (index < length && char.IsWhiteSpace(parameterString[index]))
			{
				index++;
			}
			int num = index;
			while (index < length)
			{
				char c = parameterString[index];
				if (c == '=' || c == ","[0] || char.IsWhiteSpace(c))
				{
					break;
				}
				index++;
			}
			result.name = parameterString.Substring(num, index - num);
			while (index < length && char.IsWhiteSpace(parameterString[index]))
			{
				index++;
			}
			if (index == length || parameterString[index] != '=')
			{
				result.value = true;
			}
			else
			{
				index++;
				while (index < length && char.IsWhiteSpace(parameterString[index]))
				{
					index++;
				}
				int num2 = index;
				while (index < length && parameterString[index] != ","[0] && !char.IsWhiteSpace(parameterString[index]))
				{
					index++;
				}
				string value = parameterString.Substring(num2, index - num2);
				result.value = PrimitiveValue.FromString(value);
			}
			if (index < length && parameterString[index] == ","[0])
			{
				index++;
			}
			return result;
		}

		public void ApplyToObject(object instance)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			Type type = instance.GetType();
			FieldInfo field = type.GetField(this.name, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (field == null)
			{
				throw new ArgumentException(string.Concat(new string[]
				{
					"Cannot find public field '",
					this.name,
					"' in '",
					type.Name,
					"' (while trying to apply parameter)"
				}), "instance");
			}
			TypeCode typeCode = Type.GetTypeCode(field.FieldType);
			field.SetValue(instance, this.value.ConvertTo(typeCode).ToObject());
		}

		public static void ApplyAllToObject<TParameterList>(object instance, TParameterList parameters) where TParameterList : IEnumerable<NamedValue>
		{
			foreach (NamedValue namedValue in parameters)
			{
				namedValue.ApplyToObject(instance);
			}
		}

		public const string Separator = ",";
	}
}
