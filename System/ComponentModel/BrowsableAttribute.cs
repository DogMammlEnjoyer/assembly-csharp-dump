using System;

namespace System.ComponentModel
{
	/// <summary>Specifies whether a property or event should be displayed in a Properties window.</summary>
	[AttributeUsage(AttributeTargets.All)]
	public sealed class BrowsableAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.BrowsableAttribute" /> class.</summary>
		/// <param name="browsable">
		///   <see langword="true" /> if a property or event can be modified at design time; otherwise, <see langword="false" />. The default is <see langword="true" />.</param>
		public BrowsableAttribute(bool browsable)
		{
			this.Browsable = browsable;
		}

		/// <summary>Gets a value indicating whether an object is browsable.</summary>
		/// <returns>
		///   <see langword="true" /> if the object is browsable; otherwise, <see langword="false" />.</returns>
		public bool Browsable { get; }

		/// <summary>Indicates whether this instance and a specified object are equal.</summary>
		/// <param name="obj">Another object to compare to.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="obj" /> is equal to this instance; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			BrowsableAttribute browsableAttribute = obj as BrowsableAttribute;
			bool? flag = (browsableAttribute != null) ? new bool?(browsableAttribute.Browsable) : null;
			bool browsable = this.Browsable;
			return flag.GetValueOrDefault() == browsable & flag != null;
		}

		/// <summary>Returns the hash code for this instance.</summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			return this.Browsable.GetHashCode();
		}

		/// <summary>Determines if this attribute is the default.</summary>
		/// <returns>
		///   <see langword="true" /> if the attribute is the default value for this attribute class; otherwise, <see langword="false" />.</returns>
		public override bool IsDefaultAttribute()
		{
			return this.Equals(BrowsableAttribute.Default);
		}

		/// <summary>Specifies that a property or event can be modified at design time. This <see langword="static" /> field is read-only.</summary>
		public static readonly BrowsableAttribute Yes = new BrowsableAttribute(true);

		/// <summary>Specifies that a property or event cannot be modified at design time. This <see langword="static" /> field is read-only.</summary>
		public static readonly BrowsableAttribute No = new BrowsableAttribute(false);

		/// <summary>Specifies the default value for the <see cref="T:System.ComponentModel.BrowsableAttribute" />, which is <see cref="F:System.ComponentModel.BrowsableAttribute.Yes" />. This <see langword="static" /> field is read-only.</summary>
		public static readonly BrowsableAttribute Default = BrowsableAttribute.Yes;
	}
}
