using System;

namespace System.ComponentModel
{
	/// <summary>Specifies a description for a property or event.</summary>
	[AttributeUsage(AttributeTargets.All)]
	public class DescriptionAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.DescriptionAttribute" /> class with no parameters.</summary>
		public DescriptionAttribute() : this(string.Empty)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.DescriptionAttribute" /> class with a description.</summary>
		/// <param name="description">The description text.</param>
		public DescriptionAttribute(string description)
		{
			this.DescriptionValue = description;
		}

		/// <summary>Gets the description stored in this attribute.</summary>
		/// <returns>The description stored in this attribute.</returns>
		public virtual string Description
		{
			get
			{
				return this.DescriptionValue;
			}
		}

		/// <summary>Gets or sets the string stored as the description.</summary>
		/// <returns>The string stored as the description. The default value is an empty string ("").</returns>
		protected string DescriptionValue { get; set; }

		/// <summary>Returns whether the value of the given object is equal to the current <see cref="T:System.ComponentModel.DescriptionAttribute" />.</summary>
		/// <param name="obj">The object to test the value equality of.</param>
		/// <returns>
		///   <see langword="true" /> if the value of the given object is equal to that of the current; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			DescriptionAttribute descriptionAttribute = obj as DescriptionAttribute;
			return descriptionAttribute != null && descriptionAttribute.Description == this.Description;
		}

		/// <summary>Returns the hash code for this instance.</summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			return this.Description.GetHashCode();
		}

		/// <summary>Returns a value indicating whether this is the default <see cref="T:System.ComponentModel.DescriptionAttribute" /> instance.</summary>
		/// <returns>
		///   <see langword="true" />, if this is the default <see cref="T:System.ComponentModel.DescriptionAttribute" /> instance; otherwise, <see langword="false" />.</returns>
		public override bool IsDefaultAttribute()
		{
			return this.Equals(DescriptionAttribute.Default);
		}

		/// <summary>Specifies the default value for the <see cref="T:System.ComponentModel.DescriptionAttribute" />, which is an empty string (""). This <see langword="static" /> field is read-only.</summary>
		public static readonly DescriptionAttribute Default = new DescriptionAttribute();
	}
}
