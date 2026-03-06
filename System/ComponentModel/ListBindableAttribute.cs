using System;

namespace System.ComponentModel
{
	/// <summary>Specifies that a list can be used as a data source. A visual designer should use this attribute to determine whether to display a particular list in a data-binding picker. This class cannot be inherited.</summary>
	[AttributeUsage(AttributeTargets.All)]
	public sealed class ListBindableAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.ListBindableAttribute" /> class using a value to indicate whether the list is bindable.</summary>
		/// <param name="listBindable">
		///   <see langword="true" /> if the list is bindable; otherwise, <see langword="false" />.</param>
		public ListBindableAttribute(bool listBindable)
		{
			this.ListBindable = listBindable;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.ListBindableAttribute" /> class using <see cref="T:System.ComponentModel.BindableSupport" /> to indicate whether the list is bindable.</summary>
		/// <param name="flags">A <see cref="T:System.ComponentModel.BindableSupport" /> that indicates whether the list is bindable.</param>
		public ListBindableAttribute(BindableSupport flags)
		{
			this.ListBindable = (flags > BindableSupport.No);
			this._isDefault = (flags == BindableSupport.Default);
		}

		/// <summary>Gets whether the list is bindable.</summary>
		/// <returns>
		///   <see langword="true" /> if the list is bindable; otherwise, <see langword="false" />.</returns>
		public bool ListBindable { get; }

		/// <summary>Returns whether the object passed is equal to this <see cref="T:System.ComponentModel.ListBindableAttribute" />.</summary>
		/// <param name="obj">The object to test equality with.</param>
		/// <returns>
		///   <see langword="true" /> if the object passed is equal to this <see cref="T:System.ComponentModel.ListBindableAttribute" />; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			ListBindableAttribute listBindableAttribute = obj as ListBindableAttribute;
			return listBindableAttribute != null && listBindableAttribute.ListBindable == this.ListBindable;
		}

		/// <summary>Returns the hash code for this instance.</summary>
		/// <returns>A hash code for the current <see cref="T:System.ComponentModel.ListBindableAttribute" />.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>Returns whether <see cref="P:System.ComponentModel.ListBindableAttribute.ListBindable" /> is set to the default value.</summary>
		/// <returns>
		///   <see langword="true" /> if <see cref="P:System.ComponentModel.ListBindableAttribute.ListBindable" /> is set to the default value; otherwise, <see langword="false" />.</returns>
		public override bool IsDefaultAttribute()
		{
			return this.Equals(ListBindableAttribute.Default) || this._isDefault;
		}

		/// <summary>Specifies that the list is bindable. This <see langword="static" /> field is read-only.</summary>
		public static readonly ListBindableAttribute Yes = new ListBindableAttribute(true);

		/// <summary>Specifies that the list is not bindable. This <see langword="static" /> field is read-only.</summary>
		public static readonly ListBindableAttribute No = new ListBindableAttribute(false);

		/// <summary>Represents the default value for <see cref="T:System.ComponentModel.ListBindableAttribute" />.</summary>
		public static readonly ListBindableAttribute Default = ListBindableAttribute.Yes;

		private bool _isDefault;
	}
}
