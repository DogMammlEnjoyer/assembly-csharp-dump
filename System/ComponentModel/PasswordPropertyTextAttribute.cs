using System;

namespace System.ComponentModel
{
	/// <summary>Indicates that an object's text representation is obscured by characters such as asterisks. This class cannot be inherited.</summary>
	[AttributeUsage(AttributeTargets.All)]
	public sealed class PasswordPropertyTextAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.PasswordPropertyTextAttribute" /> class.</summary>
		public PasswordPropertyTextAttribute() : this(false)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.PasswordPropertyTextAttribute" /> class, optionally showing password text.</summary>
		/// <param name="password">
		///   <see langword="true" /> to indicate that the property should be shown as password text; otherwise, <see langword="false" />. The default is <see langword="false" />.</param>
		public PasswordPropertyTextAttribute(bool password)
		{
			this.Password = password;
		}

		/// <summary>Gets a value indicating if the property for which the <see cref="T:System.ComponentModel.PasswordPropertyTextAttribute" /> is defined should be shown as password text.</summary>
		/// <returns>
		///   <see langword="true" /> if the property should be shown as password text; otherwise, <see langword="false" />.</returns>
		public bool Password { get; }

		/// <summary>Determines whether two <see cref="T:System.ComponentModel.PasswordPropertyTextAttribute" /> instances are equal.</summary>
		/// <param name="o">The <see cref="T:System.ComponentModel.PasswordPropertyTextAttribute" /> to compare with the current <see cref="T:System.ComponentModel.PasswordPropertyTextAttribute" />.</param>
		/// <returns>
		///   <see langword="true" /> if the specified <see cref="T:System.ComponentModel.PasswordPropertyTextAttribute" /> is equal to the current <see cref="T:System.ComponentModel.PasswordPropertyTextAttribute" />; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object o)
		{
			return o is PasswordPropertyTextAttribute && ((PasswordPropertyTextAttribute)o).Password == this.Password;
		}

		/// <summary>Returns the hash code for this instance.</summary>
		/// <returns>A hash code for the current <see cref="T:System.ComponentModel.PasswordPropertyTextAttribute" />.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>Returns an indication whether the value of this instance is the default value.</summary>
		/// <returns>
		///   <see langword="true" /> if this instance is the default attribute for the class; otherwise, <see langword="false" />.</returns>
		public override bool IsDefaultAttribute()
		{
			return this.Equals(PasswordPropertyTextAttribute.Default);
		}

		/// <summary>Specifies that a text property is used as a password. This <see langword="static" /> (<see langword="Shared" /> in Visual Basic) field is read-only.</summary>
		public static readonly PasswordPropertyTextAttribute Yes = new PasswordPropertyTextAttribute(true);

		/// <summary>Specifies that a text property is not used as a password. This <see langword="static" /> (<see langword="Shared" /> in Visual Basic) field is read-only.</summary>
		public static readonly PasswordPropertyTextAttribute No = new PasswordPropertyTextAttribute(false);

		/// <summary>Specifies the default value for the <see cref="T:System.ComponentModel.PasswordPropertyTextAttribute" />.</summary>
		public static readonly PasswordPropertyTextAttribute Default = PasswordPropertyTextAttribute.No;
	}
}
