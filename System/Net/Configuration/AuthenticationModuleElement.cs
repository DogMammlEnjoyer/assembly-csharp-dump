using System;
using System.Configuration;

namespace System.Net.Configuration
{
	/// <summary>Represents the type information for an authentication module. This class cannot be inherited.</summary>
	public sealed class AuthenticationModuleElement : ConfigurationElement
	{
		static AuthenticationModuleElement()
		{
			AuthenticationModuleElement.properties = new ConfigurationPropertyCollection();
			AuthenticationModuleElement.properties.Add(AuthenticationModuleElement.typeProp);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Configuration.AuthenticationModuleElement" /> class.</summary>
		public AuthenticationModuleElement()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Configuration.AuthenticationModuleElement" /> class with the specified type information.</summary>
		/// <param name="typeName">A string that identifies the type and the assembly that contains it.</param>
		public AuthenticationModuleElement(string typeName)
		{
			this.Type = typeName;
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return AuthenticationModuleElement.properties;
			}
		}

		/// <summary>Gets or sets the type and assembly information for the current instance.</summary>
		/// <returns>A string that identifies a type that implements an authentication module or <see langword="null" /> if no value has been specified.</returns>
		[ConfigurationProperty("type", Options = (ConfigurationPropertyOptions.IsRequired | ConfigurationPropertyOptions.IsKey))]
		public string Type
		{
			get
			{
				return (string)base[AuthenticationModuleElement.typeProp];
			}
			set
			{
				base[AuthenticationModuleElement.typeProp] = value;
			}
		}

		private static ConfigurationPropertyCollection properties;

		private static ConfigurationProperty typeProp = new ConfigurationProperty("type", typeof(string), null, ConfigurationPropertyOptions.IsRequired | ConfigurationPropertyOptions.IsKey);
	}
}
