using System;
using Unity;

namespace System.Configuration
{
	/// <summary>Represents the Uri section within a configuration file.</summary>
	public sealed class UriSection : ConfigurationSection
	{
		static UriSection()
		{
			UriSection.properties = new ConfigurationPropertyCollection();
			UriSection.properties.Add(UriSection.idn_prop);
			UriSection.properties.Add(UriSection.iriParsing_prop);
		}

		/// <summary>Gets an <see cref="T:System.Configuration.IdnElement" /> object that contains the configuration setting for International Domain Name (IDN) processing in the <see cref="T:System.Uri" /> class.</summary>
		/// <returns>The configuration setting for International Domain Name (IDN) processing in the <see cref="T:System.Uri" /> class.</returns>
		[ConfigurationProperty("idn")]
		public IdnElement Idn
		{
			get
			{
				return (IdnElement)base[UriSection.idn_prop];
			}
		}

		/// <summary>Gets an <see cref="T:System.Configuration.IriParsingElement" /> object that contains the configuration setting for International Resource Identifiers (IRI) parsing in the <see cref="T:System.Uri" /> class.</summary>
		/// <returns>The configuration setting for International Resource Identifiers (IRI) parsing in the <see cref="T:System.Uri" /> class.</returns>
		[ConfigurationProperty("iriParsing")]
		public IriParsingElement IriParsing
		{
			get
			{
				return (IriParsingElement)base[UriSection.iriParsing_prop];
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return UriSection.properties;
			}
		}

		/// <summary>Gets a <see cref="T:System.Configuration.SchemeSettingElementCollection" /> object that contains the configuration settings for scheme parsing in the <see cref="T:System.Uri" /> class.</summary>
		/// <returns>The configuration settings for scheme parsing in the <see cref="T:System.Uri" /> class</returns>
		public SchemeSettingElementCollection SchemeSettings
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		private static ConfigurationPropertyCollection properties;

		private static ConfigurationProperty idn_prop = new ConfigurationProperty("idn", typeof(IdnElement), null);

		private static ConfigurationProperty iriParsing_prop = new ConfigurationProperty("iriParsing", typeof(IriParsingElement), null);
	}
}
