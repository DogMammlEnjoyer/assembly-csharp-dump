using System;
using System.Globalization;

namespace System.ComponentModel
{
	/// <summary>Provides a base type converter for nonfloating-point numerical types.</summary>
	public abstract class BaseNumberConverter : TypeConverter
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.BaseNumberConverter" /> class.</summary>
		internal BaseNumberConverter()
		{
		}

		internal virtual bool AllowHex
		{
			get
			{
				return true;
			}
		}

		internal abstract Type TargetType { get; }

		internal abstract object FromString(string value, int radix);

		internal abstract object FromString(string value, NumberFormatInfo formatInfo);

		internal abstract string ToString(object value, NumberFormatInfo formatInfo);

		/// <summary>Determines if this converter can convert an object in the given source type to the native type of the converter.</summary>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
		/// <param name="sourceType">A <see cref="T:System.Type" /> that represents the type from which you want to convert.</param>
		/// <returns>
		///   <see langword="true" /> if this converter can perform the operation; otherwise, <see langword="false" />.</returns>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
		}

		/// <summary>Converts the given object to the converter's native type.</summary>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
		/// <param name="culture">A <see cref="T:System.Globalization.CultureInfo" /> that specifies the culture to represent the number.</param>
		/// <param name="value">The object to convert.</param>
		/// <returns>An <see cref="T:System.Object" /> that represents the converted value.</returns>
		/// <exception cref="T:System.Exception">
		///   <paramref name="value" /> is not a valid value for the target type.</exception>
		/// <exception cref="T:System.NotSupportedException">The conversion cannot be performed.</exception>
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			string text = value as string;
			if (text != null)
			{
				text = text.Trim();
				try
				{
					if (this.AllowHex && text[0] == '#')
					{
						return this.FromString(text.Substring(1), 16);
					}
					if ((this.AllowHex && text.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) || text.StartsWith("&h", StringComparison.OrdinalIgnoreCase))
					{
						return this.FromString(text.Substring(2), 16);
					}
					if (culture == null)
					{
						culture = CultureInfo.CurrentCulture;
					}
					NumberFormatInfo formatInfo = (NumberFormatInfo)culture.GetFormat(typeof(NumberFormatInfo));
					return this.FromString(text, formatInfo);
				}
				catch (Exception innerException)
				{
					throw new ArgumentException(SR.Format("{0} is not a valid value for {1}.", text, this.TargetType.Name), "value", innerException);
				}
			}
			return base.ConvertFrom(context, culture, value);
		}

		/// <summary>Converts the specified object to another type.</summary>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
		/// <param name="culture">A <see cref="T:System.Globalization.CultureInfo" /> that specifies the culture to represent the number.</param>
		/// <param name="value">The object to convert.</param>
		/// <param name="destinationType">The type to convert the object to.</param>
		/// <returns>An <see cref="T:System.Object" /> that represents the converted value.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="destinationType" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.NotSupportedException">The conversion cannot be performed.</exception>
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == null)
			{
				throw new ArgumentNullException("destinationType");
			}
			if (destinationType == typeof(string) && value != null && this.TargetType.IsInstanceOfType(value))
			{
				if (culture == null)
				{
					culture = CultureInfo.CurrentCulture;
				}
				NumberFormatInfo formatInfo = (NumberFormatInfo)culture.GetFormat(typeof(NumberFormatInfo));
				return this.ToString(value, formatInfo);
			}
			if (destinationType.IsPrimitive)
			{
				return Convert.ChangeType(value, destinationType, culture);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		/// <summary>Returns a value indicating whether this converter can convert an object to the given destination type using the context.</summary>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
		/// <param name="t">A <see cref="T:System.Type" /> that represents the type to which you want to convert.</param>
		/// <returns>
		///   <see langword="true" /> if this converter can perform the operation; otherwise, <see langword="false" />.</returns>
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return base.CanConvertTo(context, destinationType) || destinationType.IsPrimitive;
		}
	}
}
