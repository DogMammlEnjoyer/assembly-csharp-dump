using System;
using System.Runtime.InteropServices;

namespace System.Configuration.Internal
{
	/// <summary>Defines an interface used by the .NET Framework to initialize application configuration properties.</summary>
	[ComVisible(false)]
	public interface IInternalConfigSystem
	{
		/// <summary>Returns the configuration object based on the specified key.</summary>
		/// <param name="configKey">The configuration key value.</param>
		/// <returns>A configuration object.</returns>
		object GetSection(string configKey);

		/// <summary>Refreshes the configuration system based on the specified section name.</summary>
		/// <param name="sectionName">The name of the configuration section.</param>
		void RefreshConfig(string sectionName);

		/// <summary>Gets a value indicating whether the user configuration is supported.</summary>
		/// <returns>
		///   <see langword="true" /> if the user configuration is supported; otherwise, <see langword="false" />.</returns>
		bool SupportsUserConfig { get; }
	}
}
