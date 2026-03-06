using System;

namespace System.ComponentModel
{
	/// <summary>Specifies the default property for a component.</summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class DefaultPropertyAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.DefaultPropertyAttribute" /> class.</summary>
		/// <param name="name">The name of the default property for the component this attribute is bound to.</param>
		public DefaultPropertyAttribute(string name)
		{
			this.Name = name;
		}

		/// <summary>Gets the name of the default property for the component this attribute is bound to.</summary>
		/// <returns>The name of the default property for the component this attribute is bound to. The default value is <see langword="null" />.</returns>
		public string Name { get; }

		/// <summary>Returns whether the value of the given object is equal to the current <see cref="T:System.ComponentModel.DefaultPropertyAttribute" />.</summary>
		/// <param name="obj">The object to test the value equality of.</param>
		/// <returns>
		///   <see langword="true" /> if the value of the given object is equal to that of the current; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			DefaultPropertyAttribute defaultPropertyAttribute = obj as DefaultPropertyAttribute;
			return defaultPropertyAttribute != null && defaultPropertyAttribute.Name == this.Name;
		}

		/// <summary>Returns the hash code for this instance.</summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>Specifies the default value for the <see cref="T:System.ComponentModel.DefaultPropertyAttribute" />, which is <see langword="null" />. This <see langword="static" /> field is read-only.</summary>
		public static readonly DefaultPropertyAttribute Default = new DefaultPropertyAttribute(null);
	}
}
