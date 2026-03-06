using System;
using System.ComponentModel;

namespace System.Configuration
{
	/// <summary>The base class for the configuration converter types.</summary>
	public abstract class ConfigurationConverterBase : TypeConverter
	{
		/// <summary>Determines whether the conversion is allowed.</summary>
		/// <param name="ctx">The <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> object used for type conversions.</param>
		/// <param name="type">The <see cref="T:System.Type" /> to convert from.</param>
		/// <returns>
		///   <see langword="true" /> if the conversion is allowed; otherwise, <see langword="false" />.</returns>
		public override bool CanConvertFrom(ITypeDescriptorContext ctx, Type type)
		{
			return type == typeof(string) || base.CanConvertFrom(ctx, type);
		}

		/// <summary>Determines whether the conversion is allowed.</summary>
		/// <param name="ctx">The <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> object used for type conversion.</param>
		/// <param name="type">The type to convert to.</param>
		/// <returns>
		///   <see langword="true" /> if the conversion is allowed; otherwise, <see langword="false" />.</returns>
		public override bool CanConvertTo(ITypeDescriptorContext ctx, Type type)
		{
			return type == typeof(string) || base.CanConvertTo(ctx, type);
		}
	}
}
