using System;

namespace System.ComponentModel
{
	/// <summary>Specifies the default binding property for a component. This class cannot be inherited.</summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class DefaultBindingPropertyAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.DefaultBindingPropertyAttribute" /> class using no parameters.</summary>
		public DefaultBindingPropertyAttribute()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.DefaultBindingPropertyAttribute" /> class using the specified property name.</summary>
		/// <param name="name">The name of the default binding property.</param>
		public DefaultBindingPropertyAttribute(string name)
		{
			this.Name = name;
		}

		/// <summary>Gets the name of the default binding property for the component to which the <see cref="T:System.ComponentModel.DefaultBindingPropertyAttribute" /> is bound.</summary>
		/// <returns>The name of the default binding property for the component to which the <see cref="T:System.ComponentModel.DefaultBindingPropertyAttribute" /> is bound.</returns>
		public string Name { get; }

		/// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.ComponentModel.DefaultBindingPropertyAttribute" /> instance.</summary>
		/// <param name="obj">The <see cref="T:System.Object" /> to compare with the current <see cref="T:System.ComponentModel.DefaultBindingPropertyAttribute" /> instance</param>
		/// <returns>
		///   <see langword="true" /> if the object is equal to the current instance; otherwise, <see langword="false" />, indicating they are not equal.</returns>
		public override bool Equals(object obj)
		{
			DefaultBindingPropertyAttribute defaultBindingPropertyAttribute = obj as DefaultBindingPropertyAttribute;
			return defaultBindingPropertyAttribute != null && defaultBindingPropertyAttribute.Name == this.Name;
		}

		/// <summary>Returns the hash code for this instance.</summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>Represents the default value for the <see cref="T:System.ComponentModel.DefaultBindingPropertyAttribute" /> class.</summary>
		public static readonly DefaultBindingPropertyAttribute Default = new DefaultBindingPropertyAttribute();
	}
}
