using System;

namespace System.ComponentModel.Composition
{
	/// <summary>Specifies metadata for a type, property, field, or method marked with the <see cref="T:System.ComponentModel.Composition.ExportAttribute" />.</summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
	public sealed class ExportMetadataAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Composition.ExportMetadataAttribute" /> with the specified name and metadata value.</summary>
		/// <param name="name">A string that contains the name of the metadata value, or <see langword="null" /> to set the <see cref="P:System.ComponentModel.Composition.ExportMetadataAttribute.Name" /> property to an empty string ("").</param>
		/// <param name="value">An object that contains the metadata value. This can be <see langword="null" />.</param>
		public ExportMetadataAttribute(string name, object value)
		{
			this.Name = (name ?? string.Empty);
			this.Value = value;
		}

		/// <summary>Gets the name of the metadata value.</summary>
		/// <returns>A string that contains the name of the metadata value.</returns>
		public string Name { get; private set; }

		/// <summary>Gets the metadata value.</summary>
		/// <returns>An object that contains the metadata value.</returns>
		public object Value { get; private set; }

		/// <summary>Gets or sets a value that indicates whether this item is marked with this attribute more than once.</summary>
		/// <returns>
		///   <see langword="true" /> if the item is marked more than once; otherwise, <see langword="false" />.</returns>
		public bool IsMultiple { get; set; }
	}
}
