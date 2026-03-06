using System;
using System.Runtime.InteropServices;

namespace System.Configuration.Internal
{
	/// <summary>Defines interfaces used by internal .NET structures to support a configuration root object.</summary>
	[ComVisible(false)]
	public interface IInternalConfigRoot
	{
		/// <summary>Returns an <see cref="T:System.Configuration.Internal.IInternalConfigRecord" /> object representing a configuration specified by a configuration path.</summary>
		/// <param name="configPath">A string representing the path to a configuration file.</param>
		/// <returns>An <see cref="T:System.Configuration.Internal.IInternalConfigRecord" /> object representing a configuration specified by <paramref name="configPath" />.</returns>
		IInternalConfigRecord GetConfigRecord(string configPath);

		/// <summary>Returns an <see cref="T:System.Object" /> representing the data in a section of a configuration file.</summary>
		/// <param name="section">A string representing a section of a configuration file.</param>
		/// <param name="configPath">A string representing the path to a configuration file.</param>
		/// <returns>An <see cref="T:System.Object" /> representing the data in a section of a configuration file.</returns>
		object GetSection(string section, string configPath);

		/// <summary>Returns a value representing the file path of the nearest configuration ancestor that has configuration data.</summary>
		/// <param name="configPath">The path of configuration file.</param>
		/// <returns>A string representing the file path of the nearest configuration ancestor that has configuration data.</returns>
		string GetUniqueConfigPath(string configPath);

		/// <summary>Returns an <see cref="T:System.Configuration.Internal.IInternalConfigRecord" /> object representing a unique configuration record for given configuration path.</summary>
		/// <param name="configPath">The path of the configuration file.</param>
		/// <returns>An <see cref="T:System.Configuration.Internal.IInternalConfigRecord" /> object representing a unique configuration record for a given configuration path.</returns>
		IInternalConfigRecord GetUniqueConfigRecord(string configPath);

		/// <summary>Initializes a configuration object.</summary>
		/// <param name="host">An <see cref="T:System.Configuration.Internal.IInternalConfigHost" /> object.</param>
		/// <param name="isDesignTime">
		///   <see langword="true" /> if design time; <see langword="false" /> if run time.</param>
		void Init(IInternalConfigHost host, bool isDesignTime);

		/// <summary>Returns a value indicating whether the configuration is a design-time configuration.</summary>
		/// <returns>
		///   <see langword="true" /> if the configuration is a design-time configuration; <see langword="false" /> if the configuration is not a design-time configuration.</returns>
		bool IsDesignTime { get; }

		/// <summary>Finds and removes a configuration record and all its children for a given configuration path.</summary>
		/// <param name="configPath">The path of the configuration file.</param>
		void RemoveConfig(string configPath);

		/// <summary>Represents the method that handles the <see cref="E:System.Configuration.Internal.IInternalConfigRoot.ConfigChanged" /> event of an <see cref="T:System.Configuration.Internal.IInternalConfigRoot" /> object.</summary>
		event InternalConfigEventHandler ConfigChanged;

		/// <summary>Represents the method that handles the <see cref="E:System.Configuration.Internal.IInternalConfigRoot.ConfigRemoved" /> event of a <see cref="T:System.Configuration.Internal.IInternalConfigRoot" /> object.</summary>
		event InternalConfigEventHandler ConfigRemoved;
	}
}
