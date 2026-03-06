using System;
using System.Configuration;
using System.Net.Security;
using Unity;

namespace System.Net.Configuration
{
	/// <summary>Represents the default settings used to create connections to a remote computer. This class cannot be inherited.</summary>
	public sealed class ServicePointManagerElement : ConfigurationElement
	{
		static ServicePointManagerElement()
		{
			ServicePointManagerElement.properties = new ConfigurationPropertyCollection();
			ServicePointManagerElement.properties.Add(ServicePointManagerElement.checkCertificateNameProp);
			ServicePointManagerElement.properties.Add(ServicePointManagerElement.checkCertificateRevocationListProp);
			ServicePointManagerElement.properties.Add(ServicePointManagerElement.dnsRefreshTimeoutProp);
			ServicePointManagerElement.properties.Add(ServicePointManagerElement.enableDnsRoundRobinProp);
			ServicePointManagerElement.properties.Add(ServicePointManagerElement.expect100ContinueProp);
			ServicePointManagerElement.properties.Add(ServicePointManagerElement.useNagleAlgorithmProp);
		}

		/// <summary>Gets or sets a Boolean value that controls checking host name information in an X509 certificate.</summary>
		/// <returns>
		///   <see langword="true" /> to specify host name checking; otherwise, <see langword="false" />.</returns>
		[ConfigurationProperty("checkCertificateName", DefaultValue = "True")]
		public bool CheckCertificateName
		{
			get
			{
				return (bool)base[ServicePointManagerElement.checkCertificateNameProp];
			}
			set
			{
				base[ServicePointManagerElement.checkCertificateNameProp] = value;
			}
		}

		/// <summary>Gets or sets a Boolean value that indicates whether the certificate is checked against the certificate authority revocation list.</summary>
		/// <returns>
		///   <see langword="true" /> if the certificate revocation list is checked; otherwise, <see langword="false" />.The default value is <see langword="false" />.</returns>
		[ConfigurationProperty("checkCertificateRevocationList", DefaultValue = "False")]
		public bool CheckCertificateRevocationList
		{
			get
			{
				return (bool)base[ServicePointManagerElement.checkCertificateRevocationListProp];
			}
			set
			{
				base[ServicePointManagerElement.checkCertificateRevocationListProp] = value;
			}
		}

		/// <summary>Gets or sets the amount of time after which address information is refreshed.</summary>
		/// <returns>A <see cref="T:System.TimeSpan" /> that specifies when addresses are resolved using DNS.</returns>
		[ConfigurationProperty("dnsRefreshTimeout", DefaultValue = "120000")]
		public int DnsRefreshTimeout
		{
			get
			{
				return (int)base[ServicePointManagerElement.dnsRefreshTimeoutProp];
			}
			set
			{
				base[ServicePointManagerElement.dnsRefreshTimeoutProp] = value;
			}
		}

		/// <summary>Gets or sets a Boolean value that controls using different IP addresses on connections to the same server.</summary>
		/// <returns>
		///   <see langword="true" /> to enable DNS round-robin behavior; otherwise, <see langword="false" />.</returns>
		[ConfigurationProperty("enableDnsRoundRobin", DefaultValue = "False")]
		public bool EnableDnsRoundRobin
		{
			get
			{
				return (bool)base[ServicePointManagerElement.enableDnsRoundRobinProp];
			}
			set
			{
				base[ServicePointManagerElement.enableDnsRoundRobinProp] = value;
			}
		}

		/// <summary>Gets or sets a Boolean value that determines whether 100-Continue behavior is used.</summary>
		/// <returns>
		///   <see langword="true" /> to expect 100-Continue responses for <see langword="POST" /> requests; otherwise, <see langword="false" />. The default value is <see langword="true" />.</returns>
		[ConfigurationProperty("expect100Continue", DefaultValue = "True")]
		public bool Expect100Continue
		{
			get
			{
				return (bool)base[ServicePointManagerElement.expect100ContinueProp];
			}
			set
			{
				base[ServicePointManagerElement.expect100ContinueProp] = value;
			}
		}

		/// <summary>Gets or sets a Boolean value that determines whether the Nagle algorithm is used.</summary>
		/// <returns>
		///   <see langword="true" /> to use the Nagle algorithm; otherwise, <see langword="false" />. The default value is <see langword="true" />.</returns>
		[ConfigurationProperty("useNagleAlgorithm", DefaultValue = "True")]
		public bool UseNagleAlgorithm
		{
			get
			{
				return (bool)base[ServicePointManagerElement.useNagleAlgorithmProp];
			}
			set
			{
				base[ServicePointManagerElement.useNagleAlgorithmProp] = value;
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return ServicePointManagerElement.properties;
			}
		}

		[MonoTODO]
		protected override void PostDeserialize()
		{
		}

		/// <summary>Gets or sets the <see cref="T:System.Net.Security.EncryptionPolicy" /> to use.</summary>
		/// <returns>The encryption policy to use for a <see cref="T:System.Net.ServicePointManager" /> instance.</returns>
		public EncryptionPolicy EncryptionPolicy
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return EncryptionPolicy.RequireEncryption;
			}
			set
			{
				ThrowStub.ThrowNotSupportedException();
			}
		}

		private static ConfigurationPropertyCollection properties;

		private static ConfigurationProperty checkCertificateNameProp = new ConfigurationProperty("checkCertificateName", typeof(bool), true);

		private static ConfigurationProperty checkCertificateRevocationListProp = new ConfigurationProperty("checkCertificateRevocationList", typeof(bool), false);

		private static ConfigurationProperty dnsRefreshTimeoutProp = new ConfigurationProperty("dnsRefreshTimeout", typeof(int), 120000);

		private static ConfigurationProperty enableDnsRoundRobinProp = new ConfigurationProperty("enableDnsRoundRobin", typeof(bool), false);

		private static ConfigurationProperty expect100ContinueProp = new ConfigurationProperty("expect100Continue", typeof(bool), true);

		private static ConfigurationProperty useNagleAlgorithmProp = new ConfigurationProperty("useNagleAlgorithm", typeof(bool), true);
	}
}
