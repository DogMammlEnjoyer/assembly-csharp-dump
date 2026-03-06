using System;
using System.Globalization;
using Internal.Runtime.Augments;

namespace System.Reflection
{
	/// <summary>Represents a named argument of a custom attribute in the reflection-only context.</summary>
	public struct CustomAttributeNamedArgument
	{
		internal CustomAttributeNamedArgument(Type attributeType, string memberName, bool isField, CustomAttributeTypedArgument typedValue)
		{
			this.IsField = isField;
			this.MemberName = memberName;
			this.TypedValue = typedValue;
			this._attributeType = attributeType;
			this._lazyMemberInfo = null;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Reflection.CustomAttributeNamedArgument" /> class, which represents the specified field or property of the custom attribute, and specifies the value of the field or property.</summary>
		/// <param name="memberInfo">A field or property of the custom attribute. The new <see cref="T:System.Reflection.CustomAttributeNamedArgument" /> object represents this member and its value.</param>
		/// <param name="value">The value of the field or property of the custom attribute.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="memberInfo" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="memberInfo" /> is not a field or property of the custom attribute.</exception>
		public CustomAttributeNamedArgument(MemberInfo memberInfo, object value)
		{
			if (memberInfo == null)
			{
				throw new ArgumentNullException("memberInfo");
			}
			FieldInfo fieldInfo = memberInfo as FieldInfo;
			PropertyInfo propertyInfo = memberInfo as PropertyInfo;
			Type argumentType;
			if (fieldInfo != null)
			{
				argumentType = fieldInfo.FieldType;
			}
			else
			{
				if (!(propertyInfo != null))
				{
					throw new ArgumentException("The member must be either a field or a property.");
				}
				argumentType = propertyInfo.PropertyType;
			}
			this._lazyMemberInfo = memberInfo;
			this._attributeType = memberInfo.DeclaringType;
			if (value is CustomAttributeTypedArgument)
			{
				CustomAttributeTypedArgument customAttributeTypedArgument = (CustomAttributeTypedArgument)value;
				this.TypedValue = customAttributeTypedArgument;
			}
			else
			{
				this.TypedValue = new CustomAttributeTypedArgument(argumentType, value);
			}
			this.IsField = (fieldInfo != null);
			this.MemberName = memberInfo.Name;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Reflection.CustomAttributeNamedArgument" /> class, which represents the specified field or property of the custom attribute, and specifies a <see cref="T:System.Reflection.CustomAttributeTypedArgument" /> object that describes the type and value of the field or property.</summary>
		/// <param name="memberInfo">A field or property of the custom attribute. The new <see cref="T:System.Reflection.CustomAttributeNamedArgument" /> object represents this member and its value.</param>
		/// <param name="typedArgument">An object that describes the type and value of the field or property.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="memberInfo" /> is <see langword="null" />.</exception>
		public CustomAttributeNamedArgument(MemberInfo memberInfo, CustomAttributeTypedArgument typedArgument)
		{
			if (memberInfo == null)
			{
				throw new ArgumentNullException("memberInfo");
			}
			this._lazyMemberInfo = memberInfo;
			this._attributeType = memberInfo.DeclaringType;
			this.TypedValue = typedArgument;
			this.IsField = (memberInfo is FieldInfo);
			this.MemberName = memberInfo.Name;
		}

		/// <summary>Gets a <see cref="T:System.Reflection.CustomAttributeTypedArgument" /> structure that can be used to obtain the type and value of the current named argument.</summary>
		/// <returns>A structure that can be used to obtain the type and value of the current named argument.</returns>
		public readonly CustomAttributeTypedArgument TypedValue { get; }

		/// <summary>Gets a value that indicates whether the named argument is a field.</summary>
		/// <returns>
		///   <see langword="true" /> if the named argument is a field; otherwise, <see langword="false" />.</returns>
		public readonly bool IsField { get; }

		/// <summary>Gets the name of the attribute member that would be used to set the named argument.</summary>
		/// <returns>The name of the attribute member that would be used to set the named argument.</returns>
		public readonly string MemberName { get; }

		/// <summary>Gets the attribute member that would be used to set the named argument.</summary>
		/// <returns>The attribute member that would be used to set the named argument.</returns>
		public MemberInfo MemberInfo
		{
			get
			{
				MemberInfo memberInfo = this._lazyMemberInfo;
				if (memberInfo == null)
				{
					if (this.IsField)
					{
						memberInfo = this._attributeType.GetField(this.MemberName, BindingFlags.Instance | BindingFlags.Public);
					}
					else
					{
						memberInfo = this._attributeType.GetProperty(this.MemberName, BindingFlags.Instance | BindingFlags.Public);
					}
					if (memberInfo == null)
					{
						throw RuntimeAugments.Callbacks.CreateMissingMetadataException(this._attributeType);
					}
					this._lazyMemberInfo = memberInfo;
				}
				return memberInfo;
			}
		}

		/// <summary>Returns a value that indicates whether this instance is equal to a specified object.</summary>
		/// <param name="obj">An object to compare with this instance, or <see langword="null" />.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="obj" /> equals the type and value of this instance; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			return obj == this;
		}

		/// <summary>Returns the hash code for this instance.</summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>Tests whether two <see cref="T:System.Reflection.CustomAttributeNamedArgument" /> structures are equivalent.</summary>
		/// <param name="left">The structure to the left of the equality operator.</param>
		/// <param name="right">The structure to the right of the equality operator.</param>
		/// <returns>
		///   <see langword="true" /> if the two <see cref="T:System.Reflection.CustomAttributeNamedArgument" /> structures are equal; otherwise, <see langword="false" />.</returns>
		public static bool operator ==(CustomAttributeNamedArgument left, CustomAttributeNamedArgument right)
		{
			return left.Equals(right);
		}

		/// <summary>Tests whether two <see cref="T:System.Reflection.CustomAttributeNamedArgument" /> structures are different.</summary>
		/// <param name="left">The structure to the left of the inequality operator.</param>
		/// <param name="right">The structure to the right of the inequality operator.</param>
		/// <returns>
		///   <see langword="true" /> if the two <see cref="T:System.Reflection.CustomAttributeNamedArgument" /> structures are different; otherwise, <see langword="false" />.</returns>
		public static bool operator !=(CustomAttributeNamedArgument left, CustomAttributeNamedArgument right)
		{
			return !left.Equals(right);
		}

		/// <summary>Returns a string that consists of the argument name, the equal sign, and a string representation of the argument value.</summary>
		/// <returns>A string that consists of the argument name, the equal sign, and a string representation of the argument value.</returns>
		public override string ToString()
		{
			if (this._attributeType == null)
			{
				return base.ToString();
			}
			string result;
			try
			{
				bool typed = this._lazyMemberInfo == null || (this.IsField ? ((FieldInfo)this._lazyMemberInfo).FieldType : ((PropertyInfo)this._lazyMemberInfo).PropertyType) != typeof(object);
				result = string.Format(CultureInfo.CurrentCulture, "{0} = {1}", this.MemberName, this.TypedValue.ToString(typed));
			}
			catch (MissingMetadataException)
			{
				result = base.ToString();
			}
			return result;
		}

		private readonly Type _attributeType;

		private volatile MemberInfo _lazyMemberInfo;
	}
}
