using System;
using System.Configuration.Provider;
using System.Reflection;
using Unity;

namespace System.Configuration
{
	/// <summary>Maintains a collection of <see cref="T:System.Configuration.ConfigurationBuilder" /> objects by name.</summary>
	[DefaultMember("Item")]
	public class ConfigurationBuilderCollection : ProviderCollection
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.ConfigurationBuilderCollection" /> class.</summary>
		public ConfigurationBuilderCollection()
		{
			ThrowStub.ThrowNotSupportedException();
		}
	}
}
