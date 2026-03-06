using System;

namespace System.Runtime.Serialization
{
	/// <summary>Specifies that the field is an enumeration member and should be serialized.</summary>
	[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
	public sealed class EnumMemberAttribute : Attribute
	{
		/// <summary>Gets or sets the value associated with the enumeration member the attribute is applied to.</summary>
		/// <returns>The value associated with the enumeration member.</returns>
		public string Value
		{
			get
			{
				return this.value;
			}
			set
			{
				this.value = value;
				this.isValueSetExplicitly = true;
			}
		}

		/// <summary>Gets whether the <see cref="P:System.Runtime.Serialization.EnumMemberAttribute.Value" /> has been explicitly set.</summary>
		/// <returns>
		///   <see langword="true" /> if the value has been explicitly set; otherwise, <see langword="false" />.</returns>
		public bool IsValueSetExplicitly
		{
			get
			{
				return this.isValueSetExplicitly;
			}
		}

		private string value;

		private bool isValueSetExplicitly;
	}
}
