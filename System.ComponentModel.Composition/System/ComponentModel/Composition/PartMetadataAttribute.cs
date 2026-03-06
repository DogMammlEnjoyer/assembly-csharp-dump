using System;

namespace System.ComponentModel.Composition
{
	/// <summary>Specifies metadata for a part.</summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public sealed class PartMetadataAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Composition.PartMetadataAttribute" /> class with the specified name and metadata value.</summary>
		/// <param name="name">A string that contains the name of the metadata value or <see langword="null" /> to use an empty string ("").</param>
		/// <param name="value">An object that contains the metadata value. This can be <see langword="null" />.</param>
		public PartMetadataAttribute(string name, object value)
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
	}
}
