using System;
using System.Xml;

namespace System.Configuration
{
	/// <summary>Provides programmatic access to the <see langword="configProtectedData" /> configuration section. This class cannot be inherited.</summary>
	public sealed class ProtectedConfigurationSection : ConfigurationSection
	{
		static ProtectedConfigurationSection()
		{
			ProtectedConfigurationSection.properties.Add(ProtectedConfigurationSection.defaultProviderProp);
			ProtectedConfigurationSection.properties.Add(ProtectedConfigurationSection.providersProp);
		}

		/// <summary>Gets or sets the name of the default <see cref="T:System.Configuration.ProtectedConfigurationProvider" /> object in the <see cref="P:System.Configuration.ProtectedConfigurationSection.Providers" /> collection property.</summary>
		/// <returns>The name of the default <see cref="T:System.Configuration.ProtectedConfigurationProvider" /> object in the <see cref="P:System.Configuration.ProtectedConfigurationSection.Providers" /> collection property.</returns>
		[ConfigurationProperty("defaultProvider", DefaultValue = "RsaProtectedConfigurationProvider")]
		public string DefaultProvider
		{
			get
			{
				return (string)base[ProtectedConfigurationSection.defaultProviderProp];
			}
			set
			{
				base[ProtectedConfigurationSection.defaultProviderProp] = value;
			}
		}

		/// <summary>Gets a <see cref="T:System.Configuration.ProviderSettingsCollection" /> collection of all the <see cref="T:System.Configuration.ProtectedConfigurationProvider" /> objects in all participating configuration files.</summary>
		/// <returns>A <see cref="T:System.Configuration.ProviderSettingsCollection" /> collection of all the <see cref="T:System.Configuration.ProtectedConfigurationProvider" /> objects in all participating configuration files.</returns>
		[ConfigurationProperty("providers")]
		public ProviderSettingsCollection Providers
		{
			get
			{
				return (ProviderSettingsCollection)base[ProtectedConfigurationSection.providersProp];
			}
		}

		protected internal override ConfigurationPropertyCollection Properties
		{
			get
			{
				return ProtectedConfigurationSection.properties;
			}
		}

		internal string EncryptSection(string clearXml, ProtectedConfigurationProvider protectionProvider)
		{
			XmlDocument xmlDocument = new ConfigurationXmlDocument();
			xmlDocument.LoadXml(clearXml);
			return protectionProvider.Encrypt(xmlDocument.DocumentElement).OuterXml;
		}

		internal string DecryptSection(string encryptedXml, ProtectedConfigurationProvider protectionProvider)
		{
			return protectionProvider.Decrypt(new ConfigurationXmlDocument
			{
				InnerXml = encryptedXml
			}.DocumentElement).OuterXml;
		}

		internal ProtectedConfigurationProviderCollection GetAllProviders()
		{
			if (this.providers == null)
			{
				this.providers = new ProtectedConfigurationProviderCollection();
				foreach (object obj in this.Providers)
				{
					ProviderSettings ps = (ProviderSettings)obj;
					this.providers.Add(this.InstantiateProvider(ps));
				}
			}
			return this.providers;
		}

		private ProtectedConfigurationProvider InstantiateProvider(ProviderSettings ps)
		{
			ProtectedConfigurationProvider protectedConfigurationProvider = Activator.CreateInstance(Type.GetType(ps.Type, true)) as ProtectedConfigurationProvider;
			if (protectedConfigurationProvider == null)
			{
				throw new Exception("The type specified does not extend ProtectedConfigurationProvider class.");
			}
			protectedConfigurationProvider.Initialize(ps.Name, ps.Parameters);
			return protectedConfigurationProvider;
		}

		private static ConfigurationProperty defaultProviderProp = new ConfigurationProperty("defaultProvider", typeof(string), "RsaProtectedConfigurationProvider");

		private static ConfigurationProperty providersProp = new ConfigurationProperty("providers", typeof(ProviderSettingsCollection), null);

		private static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();

		private ProtectedConfigurationProviderCollection providers;
	}
}
