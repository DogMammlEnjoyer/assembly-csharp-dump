using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;

namespace System.ComponentModel
{
	/// <summary>Specifies the default value for a property.</summary>
	[AttributeUsage(AttributeTargets.All)]
	public class DefaultValueAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.DefaultValueAttribute" /> class, converting the specified value to the specified type, and using an invariant culture as the translation context.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> that represents the type to convert the value to.</param>
		/// <param name="value">A <see cref="T:System.String" /> that can be converted to the type using the <see cref="T:System.ComponentModel.TypeConverter" /> for the type and the U.S. English culture.</param>
		public DefaultValueAttribute(Type type, string value)
		{
			try
			{
				object value2;
				if (DefaultValueAttribute.<.ctor>g__TryConvertFromInvariantString|2_0(type, value, out value2))
				{
					this._value = value2;
				}
				else if (type.IsSubclassOf(typeof(Enum)))
				{
					this._value = Enum.Parse(type, value, true);
				}
				else if (type == typeof(TimeSpan))
				{
					this._value = TimeSpan.Parse(value);
				}
				else
				{
					this._value = Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
				}
			}
			catch
			{
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.DefaultValueAttribute" /> class using a Unicode character.</summary>
		/// <param name="value">A Unicode character that is the default value.</param>
		public DefaultValueAttribute(char value)
		{
			this._value = value;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.DefaultValueAttribute" /> class using an 8-bit unsigned integer.</summary>
		/// <param name="value">An 8-bit unsigned integer that is the default value.</param>
		public DefaultValueAttribute(byte value)
		{
			this._value = value;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.DefaultValueAttribute" /> class using a 16-bit signed integer.</summary>
		/// <param name="value">A 16-bit signed integer that is the default value.</param>
		public DefaultValueAttribute(short value)
		{
			this._value = value;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.DefaultValueAttribute" /> class using a 32-bit signed integer.</summary>
		/// <param name="value">A 32-bit signed integer that is the default value.</param>
		public DefaultValueAttribute(int value)
		{
			this._value = value;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.DefaultValueAttribute" /> class using a 64-bit signed integer.</summary>
		/// <param name="value">A 64-bit signed integer that is the default value.</param>
		public DefaultValueAttribute(long value)
		{
			this._value = value;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.DefaultValueAttribute" /> class using a single-precision floating point number.</summary>
		/// <param name="value">A single-precision floating point number that is the default value.</param>
		public DefaultValueAttribute(float value)
		{
			this._value = value;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.DefaultValueAttribute" /> class using a double-precision floating point number.</summary>
		/// <param name="value">A double-precision floating point number that is the default value.</param>
		public DefaultValueAttribute(double value)
		{
			this._value = value;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.DefaultValueAttribute" /> class using a <see cref="T:System.Boolean" /> value.</summary>
		/// <param name="value">A <see cref="T:System.Boolean" /> that is the default value.</param>
		public DefaultValueAttribute(bool value)
		{
			this._value = value;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.DefaultValueAttribute" /> class using a <see cref="T:System.String" />.</summary>
		/// <param name="value">A <see cref="T:System.String" /> that is the default value.</param>
		public DefaultValueAttribute(string value)
		{
			this._value = value;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.DefaultValueAttribute" /> class.</summary>
		/// <param name="value">An <see cref="T:System.Object" /> that represents the default value.</param>
		public DefaultValueAttribute(object value)
		{
			this._value = value;
		}

		[CLSCompliant(false)]
		public DefaultValueAttribute(sbyte value)
		{
			this._value = value;
		}

		[CLSCompliant(false)]
		public DefaultValueAttribute(ushort value)
		{
			this._value = value;
		}

		[CLSCompliant(false)]
		public DefaultValueAttribute(uint value)
		{
			this._value = value;
		}

		[CLSCompliant(false)]
		public DefaultValueAttribute(ulong value)
		{
			this._value = value;
		}

		/// <summary>Gets the default value of the property this attribute is bound to.</summary>
		/// <returns>An <see cref="T:System.Object" /> that represents the default value of the property this attribute is bound to.</returns>
		public virtual object Value
		{
			get
			{
				return this._value;
			}
		}

		/// <summary>Returns whether the value of the given object is equal to the current <see cref="T:System.ComponentModel.DefaultValueAttribute" />.</summary>
		/// <param name="obj">The object to test the value equality of.</param>
		/// <returns>
		///   <see langword="true" /> if the value of the given object is equal to that of the current; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			DefaultValueAttribute defaultValueAttribute = obj as DefaultValueAttribute;
			if (defaultValueAttribute == null)
			{
				return false;
			}
			if (this.Value != null)
			{
				return this.Value.Equals(defaultValueAttribute.Value);
			}
			return defaultValueAttribute.Value == null;
		}

		/// <summary>Returns the hash code for this instance.</summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>Sets the default value for the property to which this attribute is bound.</summary>
		/// <param name="value">The default value.</param>
		protected void SetValue(object value)
		{
			this._value = value;
		}

		[CompilerGenerated]
		internal static bool <.ctor>g__TryConvertFromInvariantString|2_0(Type typeToConvert, string stringValue, out object conversionResult)
		{
			conversionResult = null;
			if (DefaultValueAttribute.s_convertFromInvariantString == null)
			{
				Type type = Type.GetType("System.ComponentModel.TypeDescriptor, System.ComponentModel.TypeConverter", false);
				Volatile.Write<object>(ref DefaultValueAttribute.s_convertFromInvariantString, (type == null) ? new object() : Delegate.CreateDelegate(typeof(Func<Type, string, object>), type, "ConvertFromInvariantString", false));
			}
			Func<Type, string, object> func = DefaultValueAttribute.s_convertFromInvariantString as Func<Type, string, object>;
			if (func == null)
			{
				return false;
			}
			conversionResult = func(typeToConvert, stringValue);
			return true;
		}

		private object _value;

		private static object s_convertFromInvariantString;
	}
}
