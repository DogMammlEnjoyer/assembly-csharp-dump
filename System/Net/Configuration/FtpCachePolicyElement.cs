using System;
using System.Configuration;
using System.Net.Cache;
using System.Xml;

namespace System.Net.Configuration
{
	/// <summary>Represents the default FTP cache policy for network resources. This class cannot be inherited.</summary>
	public sealed class FtpCachePolicyElement : ConfigurationElement
	{
		static FtpCachePolicyElement()
		{
			FtpCachePolicyElement.properties.Add(FtpCachePolicyElement.policyLevelProp);
		}

		/// <summary>Gets or sets FTP caching behavior for the local machine.</summary>
		/// <returns>A <see cref="T:System.Net.Cache.RequestCacheLevel" /> value that specifies the cache behavior.</returns>
		[ConfigurationProperty("policyLevel", DefaultValue = "Default")]
		public RequestCacheLevel PolicyLevel
		{
			get
			{
				return (RequestCacheLevel)base[FtpCachePolicyElement.policyLevelProp];
			}
			set
			{
				base[FtpCachePolicyElement.policyLevelProp] = value;
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return FtpCachePolicyElement.properties;
			}
		}

		[MonoTODO]
		protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		protected override void Reset(ConfigurationElement parentElement)
		{
			throw new NotImplementedException();
		}

		private static ConfigurationProperty policyLevelProp = new ConfigurationProperty("policyLevel", typeof(RequestCacheLevel), RequestCacheLevel.Default);

		private static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
	}
}
