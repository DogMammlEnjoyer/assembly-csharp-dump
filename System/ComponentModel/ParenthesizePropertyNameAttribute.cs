using System;

namespace System.ComponentModel
{
	/// <summary>Indicates whether the name of the associated property is displayed with parentheses in the Properties window. This class cannot be inherited.</summary>
	[AttributeUsage(AttributeTargets.All)]
	public sealed class ParenthesizePropertyNameAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.ParenthesizePropertyNameAttribute" /> class that indicates that the associated property should not be shown with parentheses.</summary>
		public ParenthesizePropertyNameAttribute() : this(false)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.ParenthesizePropertyNameAttribute" /> class, using the specified value to indicate whether the attribute is displayed with parentheses.</summary>
		/// <param name="needParenthesis">
		///   <see langword="true" /> if the name should be enclosed in parentheses; otherwise, <see langword="false" />.</param>
		public ParenthesizePropertyNameAttribute(bool needParenthesis)
		{
			this.needParenthesis = needParenthesis;
		}

		/// <summary>Gets a value indicating whether the Properties window displays the name of the property in parentheses in the Properties window.</summary>
		/// <returns>
		///   <see langword="true" /> if the property is displayed with parentheses; otherwise, <see langword="false" />.</returns>
		public bool NeedParenthesis
		{
			get
			{
				return this.needParenthesis;
			}
		}

		/// <summary>Compares the specified object to this object and tests for equality.</summary>
		/// <param name="o">The object to be compared.</param>
		/// <returns>
		///   <see langword="true" /> if equal; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object o)
		{
			return o is ParenthesizePropertyNameAttribute && ((ParenthesizePropertyNameAttribute)o).NeedParenthesis == this.needParenthesis;
		}

		/// <summary>Gets the hash code for this object.</summary>
		/// <returns>The hash code for the object the attribute belongs to.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>Gets a value indicating whether the current value of the attribute is the default value for the attribute.</summary>
		/// <returns>
		///   <see langword="true" /> if the current value of the attribute is the default value of the attribute; otherwise, <see langword="false" />.</returns>
		public override bool IsDefaultAttribute()
		{
			return this.Equals(ParenthesizePropertyNameAttribute.Default);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.ParenthesizePropertyNameAttribute" /> class with a default value that indicates that the associated property should not be shown with parentheses. This field is read-only.</summary>
		public static readonly ParenthesizePropertyNameAttribute Default = new ParenthesizePropertyNameAttribute();

		private bool needParenthesis;
	}
}
