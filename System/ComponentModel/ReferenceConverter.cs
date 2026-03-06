using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.ComponentModel
{
	/// <summary>Provides a type converter to convert object references to and from other representations.</summary>
	public class ReferenceConverter : TypeConverter
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.ReferenceConverter" /> class.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> that represents the type to associate with this reference converter.</param>
		public ReferenceConverter(Type type)
		{
			this._type = type;
		}

		/// <summary>Gets a value indicating whether this converter can convert an object in the given source type to a reference object using the specified context.</summary>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
		/// <param name="sourceType">A <see cref="T:System.Type" /> that represents the type you wish to convert from.</param>
		/// <returns>
		///   <see langword="true" /> if this object can perform the conversion; otherwise, <see langword="false" />.</returns>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return (sourceType == typeof(string) && context != null) || base.CanConvertFrom(context, sourceType);
		}

		/// <summary>Converts the given object to the reference type.</summary>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
		/// <param name="culture">A <see cref="T:System.Globalization.CultureInfo" /> that specifies the culture used to represent the font.</param>
		/// <param name="value">The <see cref="T:System.Object" /> to convert.</param>
		/// <returns>An <see cref="T:System.Object" /> that represents the converted <paramref name="value" />.</returns>
		/// <exception cref="T:System.NotSupportedException">The conversion cannot be performed.</exception>
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string)
			{
				string text = ((string)value).Trim();
				if (!string.Equals(text, ReferenceConverter.s_none) && context != null)
				{
					IReferenceService referenceService = (IReferenceService)context.GetService(typeof(IReferenceService));
					if (referenceService != null)
					{
						object reference = referenceService.GetReference(text);
						if (reference != null)
						{
							return reference;
						}
					}
					IContainer container = context.Container;
					if (container != null)
					{
						object obj = container.Components[text];
						if (obj != null)
						{
							return obj;
						}
					}
				}
				return null;
			}
			return base.ConvertFrom(context, culture, value);
		}

		/// <summary>Converts the given value object to the reference type using the specified context and arguments.</summary>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
		/// <param name="culture">A <see cref="T:System.Globalization.CultureInfo" /> that specifies the culture used to represent the font.</param>
		/// <param name="value">The <see cref="T:System.Object" /> to convert.</param>
		/// <param name="destinationType">The type to convert the object to.</param>
		/// <returns>The converted object.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="destinationType" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.NotSupportedException">The conversion cannot be performed.</exception>
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == null)
			{
				throw new ArgumentNullException("destinationType");
			}
			if (!(destinationType == typeof(string)))
			{
				return base.ConvertTo(context, culture, value, destinationType);
			}
			if (value != null)
			{
				IReferenceService referenceService = (IReferenceService)((context != null) ? context.GetService(typeof(IReferenceService)) : null);
				if (referenceService != null)
				{
					string name = referenceService.GetName(value);
					if (name != null)
					{
						return name;
					}
				}
				if (!Marshal.IsComObject(value) && value is IComponent)
				{
					ISite site = ((IComponent)value).Site;
					string text = (site != null) ? site.Name : null;
					if (text != null)
					{
						return text;
					}
				}
				return string.Empty;
			}
			return ReferenceConverter.s_none;
		}

		/// <summary>Gets a collection of standard values for the reference data type.</summary>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
		/// <returns>A <see cref="T:System.ComponentModel.TypeConverter.StandardValuesCollection" /> that holds a standard set of valid values, or <see langword="null" /> if the data type does not support a standard set of values.</returns>
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			object[] array = null;
			if (context != null)
			{
				List<object> list = new List<object>();
				list.Add(null);
				IReferenceService referenceService = (IReferenceService)context.GetService(typeof(IReferenceService));
				if (referenceService != null)
				{
					object[] references = referenceService.GetReferences(this._type);
					int num = references.Length;
					for (int i = 0; i < num; i++)
					{
						if (this.IsValueAllowed(context, references[i]))
						{
							list.Add(references[i]);
						}
					}
				}
				else
				{
					IContainer container = context.Container;
					if (container != null)
					{
						foreach (object obj in container.Components)
						{
							IComponent component = (IComponent)obj;
							if (component != null && this._type.IsInstanceOfType(component) && this.IsValueAllowed(context, component))
							{
								list.Add(component);
							}
						}
					}
				}
				array = list.ToArray();
				Array.Sort(array, 0, array.Length, new ReferenceConverter.ReferenceComparer(this));
			}
			return new TypeConverter.StandardValuesCollection(array);
		}

		/// <summary>Gets a value indicating whether the list of standard values returned from <see cref="M:System.ComponentModel.ReferenceConverter.GetStandardValues(System.ComponentModel.ITypeDescriptorContext)" /> is an exclusive list.</summary>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
		/// <returns>
		///   <see langword="true" /> because the <see cref="T:System.ComponentModel.TypeConverter.StandardValuesCollection" /> returned from <see cref="M:System.ComponentModel.ReferenceConverter.GetStandardValues(System.ComponentModel.ITypeDescriptorContext)" /> is an exhaustive list of possible values. This method never returns <see langword="false" />.</returns>
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return true;
		}

		/// <summary>Gets a value indicating whether this object supports a standard set of values that can be picked from a list.</summary>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
		/// <returns>
		///   <see langword="true" /> because <see cref="M:System.ComponentModel.ReferenceConverter.GetStandardValues(System.ComponentModel.ITypeDescriptorContext)" /> can be called to find a common set of values the object supports. This method never returns <see langword="false" />.</returns>
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		/// <summary>Returns a value indicating whether a particular value can be added to the standard values collection.</summary>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides an additional context.</param>
		/// <param name="value">The value to check.</param>
		/// <returns>
		///   <see langword="true" /> if the value is allowed and can be added to the standard values collection; <see langword="false" /> if the value cannot be added to the standard values collection.</returns>
		protected virtual bool IsValueAllowed(ITypeDescriptorContext context, object value)
		{
			return true;
		}

		private static readonly string s_none = "(none)";

		private Type _type;

		private class ReferenceComparer : IComparer
		{
			public ReferenceComparer(ReferenceConverter converter)
			{
				this._converter = converter;
			}

			public int Compare(object item1, object item2)
			{
				string strA = this._converter.ConvertToString(item1);
				string strB = this._converter.ConvertToString(item2);
				return string.Compare(strA, strB, false, CultureInfo.InvariantCulture);
			}

			private ReferenceConverter _converter;
		}
	}
}
