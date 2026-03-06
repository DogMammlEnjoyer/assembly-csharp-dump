using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;

namespace System.Configuration
{
	/// <summary>Converts a comma-delimited string value to and from a <see cref="T:System.Configuration.CommaDelimitedStringCollection" /> object. This class cannot be inherited.</summary>
	public sealed class CommaDelimitedStringCollectionConverter : ConfigurationConverterBase
	{
		/// <summary>Converts a <see cref="T:System.String" /> object to a <see cref="T:System.Configuration.CommaDelimitedStringCollection" /> object.</summary>
		/// <param name="ctx">The <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> used for type conversions.</param>
		/// <param name="ci">The <see cref="T:System.Globalization.CultureInfo" /> used during conversion.</param>
		/// <param name="data">The comma-separated <see cref="T:System.String" /> to convert.</param>
		/// <returns>A <see cref="T:System.Configuration.CommaDelimitedStringCollection" /> containing the converted value.</returns>
		public override object ConvertFrom(ITypeDescriptorContext ctx, CultureInfo ci, object data)
		{
			CommaDelimitedStringCollection commaDelimitedStringCollection = new CommaDelimitedStringCollection();
			foreach (string text in ((string)data).Split(',', StringSplitOptions.None))
			{
				commaDelimitedStringCollection.Add(text.Trim());
			}
			commaDelimitedStringCollection.UpdateStringHash();
			return commaDelimitedStringCollection;
		}

		/// <summary>Converts a <see cref="T:System.Configuration.CommaDelimitedStringCollection" /> object to a <see cref="T:System.String" /> object.</summary>
		/// <param name="ctx">The <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> used for type conversions.</param>
		/// <param name="ci">The <see cref="T:System.Globalization.CultureInfo" /> used during conversion.</param>
		/// <param name="value">The value to convert.</param>
		/// <param name="type">The conversion type.</param>
		/// <returns>The <see cref="T:System.String" /> representing the converted <paramref name="value" /> parameter, which is a <see cref="T:System.Configuration.CommaDelimitedStringCollection" />.</returns>
		public override object ConvertTo(ITypeDescriptorContext ctx, CultureInfo ci, object value, Type type)
		{
			if (value == null)
			{
				return null;
			}
			if (!typeof(StringCollection).IsAssignableFrom(value.GetType()))
			{
				throw new ArgumentException();
			}
			return value.ToString();
		}
	}
}
