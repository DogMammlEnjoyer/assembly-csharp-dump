using System;
using System.Configuration;
using System.Net.Cache;
using System.Xml;

namespace System.Net.Configuration
{
	/// <summary>Represents the configuration section for cache behavior. This class cannot be inherited.</summary>
	public sealed class RequestCachingSection : ConfigurationSection
	{
		static RequestCachingSection()
		{
			RequestCachingSection.properties = new ConfigurationPropertyCollection();
			RequestCachingSection.properties.Add(RequestCachingSection.defaultFtpCachePolicyProp);
			RequestCachingSection.properties.Add(RequestCachingSection.defaultHttpCachePolicyProp);
			RequestCachingSection.properties.Add(RequestCachingSection.defaultPolicyLevelProp);
			RequestCachingSection.properties.Add(RequestCachingSection.disableAllCachingProp);
			RequestCachingSection.properties.Add(RequestCachingSection.isPrivateCacheProp);
			RequestCachingSection.properties.Add(RequestCachingSection.unspecifiedMaximumAgeProp);
		}

		/// <summary>Gets the default FTP caching behavior for the local computer.</summary>
		/// <returns>A <see cref="T:System.Net.Configuration.FtpCachePolicyElement" /> that defines the default cache policy.</returns>
		[ConfigurationProperty("defaultFtpCachePolicy")]
		public FtpCachePolicyElement DefaultFtpCachePolicy
		{
			get
			{
				return (FtpCachePolicyElement)base[RequestCachingSection.defaultFtpCachePolicyProp];
			}
		}

		/// <summary>Gets the default caching behavior for the local computer.</summary>
		/// <returns>A <see cref="T:System.Net.Configuration.HttpCachePolicyElement" /> that defines the default cache policy.</returns>
		[ConfigurationProperty("defaultHttpCachePolicy")]
		public HttpCachePolicyElement DefaultHttpCachePolicy
		{
			get
			{
				return (HttpCachePolicyElement)base[RequestCachingSection.defaultHttpCachePolicyProp];
			}
		}

		/// <summary>Gets or sets the default cache policy level.</summary>
		/// <returns>A <see cref="T:System.Net.Cache.RequestCacheLevel" /> enumeration value.</returns>
		[ConfigurationProperty("defaultPolicyLevel", DefaultValue = "BypassCache")]
		public RequestCacheLevel DefaultPolicyLevel
		{
			get
			{
				return (RequestCacheLevel)base[RequestCachingSection.defaultPolicyLevelProp];
			}
			set
			{
				base[RequestCachingSection.defaultPolicyLevelProp] = value;
			}
		}

		/// <summary>Gets or sets a Boolean value that enables caching on the local computer.</summary>
		/// <returns>
		///   <see langword="true" /> if caching is disabled on the local computer; otherwise, <see langword="false" />.</returns>
		[ConfigurationProperty("disableAllCaching", DefaultValue = "False")]
		public bool DisableAllCaching
		{
			get
			{
				return (bool)base[RequestCachingSection.disableAllCachingProp];
			}
			set
			{
				base[RequestCachingSection.disableAllCachingProp] = value;
			}
		}

		/// <summary>Gets or sets a Boolean value that indicates whether the local computer cache is private.</summary>
		/// <returns>
		///   <see langword="true" /> if the cache provides user isolation; otherwise, <see langword="false" />.</returns>
		[ConfigurationProperty("isPrivateCache", DefaultValue = "True")]
		public bool IsPrivateCache
		{
			get
			{
				return (bool)base[RequestCachingSection.isPrivateCacheProp];
			}
			set
			{
				base[RequestCachingSection.isPrivateCacheProp] = value;
			}
		}

		/// <summary>Gets or sets a value used as the maximum age for cached resources that do not have expiration information.</summary>
		/// <returns>A <see cref="T:System.TimeSpan" /> that provides a default maximum age for cached resources.</returns>
		[ConfigurationProperty("unspecifiedMaximumAge", DefaultValue = "1.00:00:00")]
		public TimeSpan UnspecifiedMaximumAge
		{
			get
			{
				return (TimeSpan)base[RequestCachingSection.unspecifiedMaximumAgeProp];
			}
			set
			{
				base[RequestCachingSection.unspecifiedMaximumAgeProp] = value;
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return RequestCachingSection.properties;
			}
		}

		[MonoTODO]
		protected override void PostDeserialize()
		{
			base.PostDeserialize();
		}

		[MonoTODO]
		protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
		{
			base.DeserializeElement(reader, serializeCollectionKey);
		}

		private static ConfigurationPropertyCollection properties;

		private static ConfigurationProperty defaultFtpCachePolicyProp = new ConfigurationProperty("defaultFtpCachePolicy", typeof(FtpCachePolicyElement));

		private static ConfigurationProperty defaultHttpCachePolicyProp = new ConfigurationProperty("defaultHttpCachePolicy", typeof(HttpCachePolicyElement));

		private static ConfigurationProperty defaultPolicyLevelProp = new ConfigurationProperty("defaultPolicyLevel", typeof(RequestCacheLevel), RequestCacheLevel.BypassCache);

		private static ConfigurationProperty disableAllCachingProp = new ConfigurationProperty("disableAllCaching", typeof(bool), false);

		private static ConfigurationProperty isPrivateCacheProp = new ConfigurationProperty("isPrivateCache", typeof(bool), true);

		private static ConfigurationProperty unspecifiedMaximumAgeProp = new ConfigurationProperty("unspecifiedMaximumAge", typeof(TimeSpan), new TimeSpan(1, 0, 0, 0));
	}
}
