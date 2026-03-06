using System;
using System.Configuration;

namespace System.Security.Authentication.ExtendedProtection.Configuration
{
	/// <summary>The <see cref="T:System.Security.Authentication.ExtendedProtection.Configuration.ExtendedProtectionPolicyElement" /> class represents a configuration element for an <see cref="T:System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy" />.</summary>
	[MonoTODO]
	public sealed class ExtendedProtectionPolicyElement : ConfigurationElement
	{
		static ExtendedProtectionPolicyElement()
		{
			Type typeFromHandle = typeof(ExtendedProtectionPolicyElement);
			ExtendedProtectionPolicyElement.custom_service_names = ConfigUtil.BuildProperty(typeFromHandle, "CustomServiceNames");
			ExtendedProtectionPolicyElement.policy_enforcement = ConfigUtil.BuildProperty(typeFromHandle, "PolicyEnforcement");
			ExtendedProtectionPolicyElement.protection_scenario = ConfigUtil.BuildProperty(typeFromHandle, "ProtectionScenario");
			foreach (ConfigurationProperty property in new ConfigurationProperty[]
			{
				ExtendedProtectionPolicyElement.custom_service_names,
				ExtendedProtectionPolicyElement.policy_enforcement,
				ExtendedProtectionPolicyElement.protection_scenario
			})
			{
				ExtendedProtectionPolicyElement.properties.Add(property);
			}
		}

		/// <summary>Gets or sets the custom Service Provider Name (SPN) list used to match against a client's SPN for this configuration policy element.</summary>
		/// <returns>A collection that includes the custom SPN list used to match against a client's SPN.</returns>
		[ConfigurationProperty("customServiceNames")]
		public ServiceNameElementCollection CustomServiceNames
		{
			get
			{
				return (ServiceNameElementCollection)base[ExtendedProtectionPolicyElement.custom_service_names];
			}
		}

		/// <summary>Gets or sets the policy enforcement value for this configuration policy element.</summary>
		/// <returns>One of the enumeration values that indicates when the extended protection policy should be enforced.</returns>
		[ConfigurationProperty("policyEnforcement")]
		public PolicyEnforcement PolicyEnforcement
		{
			get
			{
				return (PolicyEnforcement)base[ExtendedProtectionPolicyElement.policy_enforcement];
			}
			set
			{
				base[ExtendedProtectionPolicyElement.policy_enforcement] = value;
			}
		}

		/// <summary>Gets or sets the kind of protection enforced by the extended protection policy for this configuration policy element.</summary>
		/// <returns>A <see cref="T:System.Security.Authentication.ExtendedProtection.ProtectionScenario" /> value that indicates the kind of protection enforced by the policy.</returns>
		[ConfigurationProperty("protectionScenario", DefaultValue = ProtectionScenario.TransportSelected)]
		public ProtectionScenario ProtectionScenario
		{
			get
			{
				return (ProtectionScenario)base[ExtendedProtectionPolicyElement.protection_scenario];
			}
			set
			{
				base[ExtendedProtectionPolicyElement.protection_scenario] = value;
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return ExtendedProtectionPolicyElement.properties;
			}
		}

		/// <summary>The <see cref="M:System.Security.Authentication.ExtendedProtection.Configuration.ExtendedProtectionPolicyElement.BuildPolicy" /> method builds a new <see cref="T:System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy" /> instance based on the properties set on the <see cref="T:System.Security.Authentication.ExtendedProtection.Configuration.ExtendedProtectionPolicyElement" /> class.</summary>
		/// <returns>A new <see cref="T:System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy" /> instance that represents the extended protection policy created.</returns>
		public ExtendedProtectionPolicy BuildPolicy()
		{
			throw new NotImplementedException();
		}

		private static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();

		private static ConfigurationProperty custom_service_names;

		private static ConfigurationProperty policy_enforcement;

		private static ConfigurationProperty protection_scenario;
	}
}
