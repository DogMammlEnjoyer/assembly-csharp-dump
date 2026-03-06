using System;

namespace System.ComponentModel
{
	/// <summary>Specifies when a component property can be bound to an application setting.</summary>
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class SettingsBindableAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.SettingsBindableAttribute" /> class.</summary>
		/// <param name="bindable">
		///   <see langword="true" /> to specify that a property is appropriate to bind settings to; otherwise, <see langword="false" />.</param>
		public SettingsBindableAttribute(bool bindable)
		{
			this.Bindable = bindable;
		}

		/// <summary>Gets a value indicating whether a property is appropriate to bind settings to.</summary>
		/// <returns>
		///   <see langword="true" /> if the property is appropriate to bind settings to; otherwise, <see langword="false" />.</returns>
		public bool Bindable { get; }

		/// <summary>Returns a value that indicates whether this instance is equal to a specified object.</summary>
		/// <param name="obj">An <see cref="T:System.Object" /> to compare with this instance or a null reference (<see langword="Nothing" /> in Visual Basic).</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="obj" /> equals the type and value of this instance; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			return obj == this || (obj != null && obj is SettingsBindableAttribute && ((SettingsBindableAttribute)obj).Bindable == this.Bindable);
		}

		/// <summary>Returns the hash code for this instance.</summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			return this.Bindable.GetHashCode();
		}

		/// <summary>Specifies that a property is appropriate to bind settings to.</summary>
		public static readonly SettingsBindableAttribute Yes = new SettingsBindableAttribute(true);

		/// <summary>Specifies that a property is not appropriate to bind settings to.</summary>
		public static readonly SettingsBindableAttribute No = new SettingsBindableAttribute(false);
	}
}
