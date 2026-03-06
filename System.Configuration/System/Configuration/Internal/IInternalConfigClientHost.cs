using System;
using System.Runtime.InteropServices;

namespace System.Configuration.Internal
{
	/// <summary>Defines interfaces that allow the internal .NET Framework infrastructure to customize configuration.</summary>
	[ComVisible(false)]
	public interface IInternalConfigClientHost
	{
		/// <summary>Returns the path to the application configuration file.</summary>
		/// <returns>A string representing the path to the application configuration file.</returns>
		string GetExeConfigPath();

		/// <summary>Returns a string representing the path to the known local user configuration file.</summary>
		/// <returns>A string representing the path to the known local user configuration file.</returns>
		string GetLocalUserConfigPath();

		/// <summary>Returns a string representing the path to the known roaming user configuration file.</summary>
		/// <returns>A string representing the path to the known roaming user configuration file.</returns>
		string GetRoamingUserConfigPath();

		/// <summary>Returns a value indicating whether a configuration file path is the same as a currently known application configuration file path.</summary>
		/// <param name="configPath">A string representing the path to the application configuration file.</param>
		/// <returns>
		///   <see langword="true" /> if a string representing a configuration path is the same as a path to the application configuration file; <see langword="false" /> if a string representing a configuration path is not the same as a path to the application configuration file.</returns>
		bool IsExeConfig(string configPath);

		/// <summary>Returns a value indicating whether a configuration file path is the same as the configuration file path for the currently known local user.</summary>
		/// <param name="configPath">A string representing the path to the application configuration file.</param>
		/// <returns>
		///   <see langword="true" /> if a string representing a configuration path is the same as a path to a known local user configuration file; otherwise, <see langword="false" />.</returns>
		bool IsLocalUserConfig(string configPath);

		/// <summary>Returns a value indicating whether a configuration file path is the same as the configuration file path for the currently known roaming user.</summary>
		/// <param name="configPath">A string representing the path to an application configuration file.</param>
		/// <returns>
		///   <see langword="true" /> if a string representing a configuration path is the same as a path to a known roaming user configuration file; otherwise, <see langword="false" />.</returns>
		bool IsRoamingUserConfig(string configPath);
	}
}
