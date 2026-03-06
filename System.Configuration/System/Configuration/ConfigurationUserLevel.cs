using System;

namespace System.Configuration
{
	/// <summary>Used to specify which configuration file is to be represented by the Configuration object.</summary>
	public enum ConfigurationUserLevel
	{
		/// <summary>Gets the <see cref="T:System.Configuration.Configuration" /> that applies to all users.</summary>
		None,
		/// <summary>Gets the roaming <see cref="T:System.Configuration.Configuration" /> that applies to the current user.</summary>
		PerUserRoaming = 10,
		/// <summary>Gets the local <see cref="T:System.Configuration.Configuration" /> that applies to the current user.</summary>
		PerUserRoamingAndLocal = 20
	}
}
