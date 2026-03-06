using System;

namespace System.Management.Instrumentation
{
	/// <summary>Defines values that specify the hosting model for the provider.Note: the WMI .NET libraries are now considered in final state, and no further development, enhancements, or updates will be available for non-security related issues affecting these libraries. The MI APIs should be used for all new development.</summary>
	public enum ManagementHostingModel
	{
		/// <summary>Activates the provider as a decoupled provider.</summary>
		Decoupled,
		/// <summary>Activates the provider in the provider host process that is running under the LocalService account.</summary>
		LocalService = 2,
		/// <summary>Activates the provider in the provider host process that is running under the LocalSystem account.</summary>
		LocalSystem,
		/// <summary>Activates the provider in the provider host process that is running under the NetworkService account.</summary>
		NetworkService = 1
	}
}
