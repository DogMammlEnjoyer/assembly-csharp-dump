using System;
using Unity;

namespace System.Configuration
{
	/// <summary>Represents an element in a <see cref="T:System.Configuration.SchemeSettingElementCollection" /> class.</summary>
	public sealed class SchemeSettingElement : ConfigurationElement
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.SchemeSettingElement" /> class.</summary>
		public SchemeSettingElement()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Gets the value of the GenericUriParserOptions entry from a <see cref="T:System.Configuration.SchemeSettingElement" /> instance.</summary>
		/// <returns>The value of GenericUriParserOptions entry.</returns>
		public GenericUriParserOptions GenericUriParserOptions
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return GenericUriParserOptions.Default;
			}
		}

		/// <summary>Gets the value of the Name entry from a <see cref="T:System.Configuration.SchemeSettingElement" /> instance.</summary>
		/// <returns>The protocol used by this schema setting.</returns>
		public string Name
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}
	}
}
