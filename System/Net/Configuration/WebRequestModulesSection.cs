using System;
using System.Configuration;

namespace System.Net.Configuration
{
	/// <summary>Represents the configuration section for Web request modules. This class cannot be inherited.</summary>
	public sealed class WebRequestModulesSection : ConfigurationSection
	{
		static WebRequestModulesSection()
		{
			WebRequestModulesSection.properties = new ConfigurationPropertyCollection();
			WebRequestModulesSection.properties.Add(WebRequestModulesSection.webRequestModulesProp);
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return WebRequestModulesSection.properties;
			}
		}

		/// <summary>Gets the collection of Web request modules in the section.</summary>
		/// <returns>A <see cref="T:System.Net.Configuration.WebRequestModuleElementCollection" /> containing the registered Web request modules.</returns>
		[ConfigurationProperty("", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
		public WebRequestModuleElementCollection WebRequestModules
		{
			get
			{
				return (WebRequestModuleElementCollection)base[WebRequestModulesSection.webRequestModulesProp];
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

		private static ConfigurationProperty webRequestModulesProp = new ConfigurationProperty("", typeof(WebRequestModuleElementCollection), null, ConfigurationPropertyOptions.IsDefaultCollection);
	}
}
