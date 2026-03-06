using System;
using Unity;

namespace System.Configuration
{
	/// <summary>Represents a group of configuration elements that configure the providers for the <see langword="&lt;configBuilders&gt;" /> configuration section.</summary>
	public class ConfigurationBuilderSettings : ConfigurationElement
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.ConfigurationBuilderSettings" /> class.</summary>
		public ConfigurationBuilderSettings()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Gets a collection of <see cref="T:System.Configuration.ConfigurationBuilderSettings" /> objects that represent the properties of configuration builders.</summary>
		/// <returns>The <see cref="T:System.Configuration.ConfigurationBuilder" /> objects.</returns>
		public ProviderSettingsCollection Builders
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}
	}
}
