using System;

namespace System.ComponentModel
{
	/// <summary>Specifies whether a member is typically used for binding. This class cannot be inherited.</summary>
	[AttributeUsage(AttributeTargets.All)]
	public sealed class BindableAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.BindableAttribute" /> class with a Boolean value.</summary>
		/// <param name="bindable">
		///   <see langword="true" /> to use property for binding; otherwise, <see langword="false" />.</param>
		public BindableAttribute(bool bindable) : this(bindable, BindingDirection.OneWay)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.BindableAttribute" /> class.</summary>
		/// <param name="bindable">
		///   <see langword="true" /> to use property for binding; otherwise, <see langword="false" />.</param>
		/// <param name="direction">One of the <see cref="T:System.ComponentModel.BindingDirection" /> values.</param>
		public BindableAttribute(bool bindable, BindingDirection direction)
		{
			this.Bindable = bindable;
			this.Direction = direction;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.BindableAttribute" /> class with one of the <see cref="T:System.ComponentModel.BindableSupport" /> values.</summary>
		/// <param name="flags">One of the <see cref="T:System.ComponentModel.BindableSupport" /> values.</param>
		public BindableAttribute(BindableSupport flags) : this(flags, BindingDirection.OneWay)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.BindableAttribute" /> class.</summary>
		/// <param name="flags">One of the <see cref="T:System.ComponentModel.BindableSupport" /> values.</param>
		/// <param name="direction">One of the <see cref="T:System.ComponentModel.BindingDirection" /> values.</param>
		public BindableAttribute(BindableSupport flags, BindingDirection direction)
		{
			this.Bindable = (flags > BindableSupport.No);
			this._isDefault = (flags == BindableSupport.Default);
			this.Direction = direction;
		}

		/// <summary>Gets a value indicating that a property is typically used for binding.</summary>
		/// <returns>
		///   <see langword="true" /> if the property is typically used for binding; otherwise, <see langword="false" />.</returns>
		public bool Bindable { get; }

		/// <summary>Gets a value indicating the direction or directions of this property's data binding.</summary>
		/// <returns>The direction of this property's data binding.</returns>
		public BindingDirection Direction { get; }

		/// <summary>Determines whether two <see cref="T:System.ComponentModel.BindableAttribute" /> objects are equal.</summary>
		/// <param name="obj">The object to compare.</param>
		/// <returns>
		///   <see langword="true" /> if the specified <see cref="T:System.ComponentModel.BindableAttribute" /> is equal to the current <see cref="T:System.ComponentModel.BindableAttribute" />; <see langword="false" /> if it is not equal.</returns>
		public override bool Equals(object obj)
		{
			return obj == this || (obj != null && obj is BindableAttribute && ((BindableAttribute)obj).Bindable == this.Bindable);
		}

		/// <summary>Serves as a hash function for the <see cref="T:System.ComponentModel.BindableAttribute" /> class.</summary>
		/// <returns>A hash code for the current <see cref="T:System.ComponentModel.BindableAttribute" />.</returns>
		public override int GetHashCode()
		{
			return this.Bindable.GetHashCode();
		}

		/// <summary>Determines if this attribute is the default.</summary>
		/// <returns>
		///   <see langword="true" /> if the attribute is the default value for this attribute class; otherwise, <see langword="false" />.</returns>
		public override bool IsDefaultAttribute()
		{
			return this.Equals(BindableAttribute.Default) || this._isDefault;
		}

		/// <summary>Specifies that a property is typically used for binding. This field is read-only.</summary>
		public static readonly BindableAttribute Yes = new BindableAttribute(true);

		/// <summary>Specifies that a property is not typically used for binding. This field is read-only.</summary>
		public static readonly BindableAttribute No = new BindableAttribute(false);

		/// <summary>Specifies the default value for the <see cref="T:System.ComponentModel.BindableAttribute" />, which is <see cref="F:System.ComponentModel.BindableAttribute.No" />. This field is read-only.</summary>
		public static readonly BindableAttribute Default = BindableAttribute.No;

		private bool _isDefault;
	}
}
