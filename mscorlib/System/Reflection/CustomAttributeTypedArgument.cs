using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace System.Reflection
{
	/// <summary>Represents an argument of a custom attribute in the reflection-only context, or an element of an array argument.</summary>
	public struct CustomAttributeTypedArgument
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Reflection.CustomAttributeTypedArgument" /> class with the specified value.</summary>
		/// <param name="value">The value of the custom attribute argument.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="value" /> is <see langword="null" />.</exception>
		public CustomAttributeTypedArgument(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this.Value = CustomAttributeTypedArgument.CanonicalizeValue(value);
			this.ArgumentType = value.GetType();
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Reflection.CustomAttributeTypedArgument" /> class with the specified type and value.</summary>
		/// <param name="argumentType">The type of the custom attribute argument.</param>
		/// <param name="value">The value of the custom attribute argument.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="argumentType" /> is <see langword="null" />.</exception>
		public CustomAttributeTypedArgument(Type argumentType, object value)
		{
			if (argumentType == null)
			{
				throw new ArgumentNullException("argumentType");
			}
			this.Value = ((value == null) ? null : CustomAttributeTypedArgument.CanonicalizeValue(value));
			this.ArgumentType = argumentType;
			Array array = value as Array;
			if (array != null)
			{
				Type elementType = array.GetType().GetElementType();
				CustomAttributeTypedArgument[] array2 = new CustomAttributeTypedArgument[array.GetLength(0)];
				for (int i = 0; i < array2.Length; i++)
				{
					object value2 = array.GetValue(i);
					Type argumentType2 = (elementType == typeof(object) && value2 != null) ? value2.GetType() : elementType;
					array2[i] = new CustomAttributeTypedArgument(argumentType2, value2);
				}
				this.Value = new ReadOnlyCollection<CustomAttributeTypedArgument>(array2);
			}
		}

		/// <summary>Gets the type of the argument or of the array argument element.</summary>
		/// <returns>A <see cref="T:System.Type" /> object representing the type of the argument or of the array element.</returns>
		public readonly Type ArgumentType { get; }

		/// <summary>Gets the value of the argument for a simple argument or for an element of an array argument; gets a collection of values for an array argument.</summary>
		/// <returns>An object that represents the value of the argument or element, or a generic <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" /> of <see cref="T:System.Reflection.CustomAttributeTypedArgument" /> objects that represent the values of an array-type argument.</returns>
		public readonly object Value { get; }

		/// <summary>Indicates whether this instance and a specified object are equal.</summary>
		/// <param name="obj">Another object to compare to.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			return obj == this;
		}

		/// <summary>Returns the hash code for this instance.</summary>
		/// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>Tests whether two <see cref="T:System.Reflection.CustomAttributeTypedArgument" /> structures are equivalent.</summary>
		/// <param name="left">The <see cref="T:System.Reflection.CustomAttributeTypedArgument" /> structure to the left of the equality operator.</param>
		/// <param name="right">The <see cref="T:System.Reflection.CustomAttributeTypedArgument" /> structure to the right of the equality operator.</param>
		/// <returns>
		///   <see langword="true" /> if the two <see cref="T:System.Reflection.CustomAttributeTypedArgument" /> structures are equal; otherwise, <see langword="false" />.</returns>
		public static bool operator ==(CustomAttributeTypedArgument left, CustomAttributeTypedArgument right)
		{
			return left.Equals(right);
		}

		/// <summary>Tests whether two <see cref="T:System.Reflection.CustomAttributeTypedArgument" /> structures are different.</summary>
		/// <param name="left">The <see cref="T:System.Reflection.CustomAttributeTypedArgument" /> structure to the left of the inequality operator.</param>
		/// <param name="right">The <see cref="T:System.Reflection.CustomAttributeTypedArgument" /> structure to the right of the inequality operator.</param>
		/// <returns>
		///   <see langword="true" /> if the two <see cref="T:System.Reflection.CustomAttributeTypedArgument" /> structures are different; otherwise, <see langword="false" />.</returns>
		public static bool operator !=(CustomAttributeTypedArgument left, CustomAttributeTypedArgument right)
		{
			return !left.Equals(right);
		}

		/// <summary>Returns a string consisting of the argument name, the equal sign, and a string representation of the argument value.</summary>
		/// <returns>A string consisting of the argument name, the equal sign, and a string representation of the argument value.</returns>
		public override string ToString()
		{
			return this.ToString(false);
		}

		internal string ToString(bool typed)
		{
			if (this.ArgumentType == null)
			{
				return base.ToString();
			}
			string result;
			try
			{
				if (this.ArgumentType.IsEnum)
				{
					result = string.Format(CultureInfo.CurrentCulture, typed ? "{0}" : "({1}){0}", this.Value, this.ArgumentType.FullNameOrDefault);
				}
				else if (this.Value == null)
				{
					result = string.Format(CultureInfo.CurrentCulture, typed ? "null" : "({0})null", this.ArgumentType.NameOrDefault);
				}
				else if (this.ArgumentType == typeof(string))
				{
					result = string.Format(CultureInfo.CurrentCulture, "\"{0}\"", this.Value);
				}
				else if (this.ArgumentType == typeof(char))
				{
					result = string.Format(CultureInfo.CurrentCulture, "'{0}'", this.Value);
				}
				else if (this.ArgumentType == typeof(Type))
				{
					result = string.Format(CultureInfo.CurrentCulture, "typeof({0})", ((Type)this.Value).FullNameOrDefault);
				}
				else if (this.ArgumentType.IsArray)
				{
					IList<CustomAttributeTypedArgument> list = this.Value as IList<CustomAttributeTypedArgument>;
					Type elementType = this.ArgumentType.GetElementType();
					string str = string.Format(CultureInfo.CurrentCulture, "new {0}[{1}] {{ ", elementType.IsEnum ? elementType.FullNameOrDefault : elementType.NameOrDefault, list.Count);
					for (int i = 0; i < list.Count; i++)
					{
						str += string.Format(CultureInfo.CurrentCulture, (i == 0) ? "{0}" : ", {0}", list[i].ToString(elementType != typeof(object)));
					}
					result = str + " }";
				}
				else
				{
					result = string.Format(CultureInfo.CurrentCulture, typed ? "{0}" : "({1}){0}", this.Value, this.ArgumentType.NameOrDefault);
				}
			}
			catch (MissingMetadataException)
			{
				result = base.ToString();
			}
			return result;
		}

		private static object CanonicalizeValue(object value)
		{
			if (value.GetType().IsEnum)
			{
				return ((Enum)value).GetValue();
			}
			return value;
		}
	}
}
