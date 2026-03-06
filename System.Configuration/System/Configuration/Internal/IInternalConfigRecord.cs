using System;
using System.Runtime.InteropServices;

namespace System.Configuration.Internal
{
	/// <summary>Defines interfaces used by internal .NET structures to support creation of new configuration records.</summary>
	[ComVisible(false)]
	public interface IInternalConfigRecord
	{
		/// <summary>Returns an object representing a section of a configuration from the last-known-good (LKG) configuration.</summary>
		/// <param name="configKey">A string representing a key to a configuration section.</param>
		/// <returns>An <see cref="T:System.Object" /> instance representing the section of the last-known-good configuration specified by <paramref name="configKey" />.</returns>
		object GetLkgSection(string configKey);

		/// <summary>Returns an <see cref="T:System.Object" /> instance representing a section of a configuration file.</summary>
		/// <param name="configKey">A string representing a key to a configuration section.</param>
		/// <returns>An <see cref="T:System.Object" /> instance representing a section of a configuration file.</returns>
		object GetSection(string configKey);

		/// <summary>Causes a specified section of the configuration object to be reinitialized.</summary>
		/// <param name="configKey">A string representing a key to a configuration section that is to be refreshed.</param>
		void RefreshSection(string configKey);

		/// <summary>Removes a configuration record.</summary>
		void Remove();

		/// <summary>Grants the configuration object the permission to throw an exception if an error occurs during initialization.</summary>
		void ThrowIfInitErrors();

		/// <summary>Gets a string representing a configuration file path.</summary>
		/// <returns>A string representing a configuration file path.</returns>
		string ConfigPath { get; }

		/// <summary>Returns a value indicating whether an error occurred during initialization of a configuration object.</summary>
		/// <returns>
		///   <see langword="true" /> if an error occurred during initialization of a configuration object; otherwise, <see langword="false" />.</returns>
		bool HasInitErrors { get; }

		/// <summary>Returns the name of a <see cref="T:System.IO.Stream" /> object performing I/O tasks on the configuration file.</summary>
		/// <returns>A string representing the name of a <see cref="T:System.IO.Stream" /> object performing I/O tasks on the configuration file.</returns>
		string StreamName { get; }
	}
}
