using System;
using System.Configuration;

namespace System.Net.Configuration
{
	/// <summary>Represents the configuration section for authentication modules. This class cannot be inherited.</summary>
	public sealed class AuthenticationModulesSection : ConfigurationSection
	{
		static AuthenticationModulesSection()
		{
			AuthenticationModulesSection.properties = new ConfigurationPropertyCollection();
			AuthenticationModulesSection.properties.Add(AuthenticationModulesSection.authenticationModulesProp);
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return AuthenticationModulesSection.properties;
			}
		}

		/// <summary>Gets the collection of authentication modules in the section.</summary>
		/// <returns>A <see cref="T:System.Net.Configuration.AuthenticationModuleElementCollection" /> that contains the registered authentication modules.</returns>
		[ConfigurationProperty("", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
		public AuthenticationModuleElementCollection AuthenticationModules
		{
			get
			{
				return (AuthenticationModuleElementCollection)base[AuthenticationModulesSection.authenticationModulesProp];
			}
		}

		[MonoTODO]
		protected override void PostDeserialize()
		{
		}

		[MonoTODO]
		protected override void InitializeDefault()
		{
		}

		private static ConfigurationPropertyCollection properties;

		private static ConfigurationProperty authenticationModulesProp = new ConfigurationProperty("", typeof(AuthenticationModuleElementCollection), null, ConfigurationPropertyOptions.IsDefaultCollection);
	}
}
