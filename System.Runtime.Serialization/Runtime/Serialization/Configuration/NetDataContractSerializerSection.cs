using System;
using System.Configuration;
using System.Security;
using System.Security.Permissions;

namespace System.Runtime.Serialization.Configuration
{
	/// <summary>Handles the XML elements used to configure serialization by the <see cref="T:System.Runtime.Serialization.NetDataContractSerializer" />.</summary>
	public sealed class NetDataContractSerializerSection : ConfigurationSection
	{
		[SecurityCritical]
		[ConfigurationPermission(SecurityAction.Assert, Unrestricted = true)]
		internal static bool TryUnsafeGetSection(out NetDataContractSerializerSection section)
		{
			section = (NetDataContractSerializerSection)ConfigurationManager.GetSection(ConfigurationStrings.NetDataContractSerializerSectionPath);
			return section != null;
		}

		/// <summary>Gets a value that indicates whether unsafe type forwarding is enabled.</summary>
		/// <returns>
		///   <see langword="true" /> if unsafe type forwarding is enabled; otherwise, <see langword="false" />.</returns>
		[ConfigurationProperty("enableUnsafeTypeForwarding", DefaultValue = false)]
		public bool EnableUnsafeTypeForwarding
		{
			get
			{
				return (bool)base["enableUnsafeTypeForwarding"];
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				if (this.properties == null)
				{
					this.properties = new ConfigurationPropertyCollection
					{
						new ConfigurationProperty("enableUnsafeTypeForwarding", typeof(bool), false, null, null, ConfigurationPropertyOptions.None)
					};
				}
				return this.properties;
			}
		}

		private ConfigurationPropertyCollection properties;
	}
}
