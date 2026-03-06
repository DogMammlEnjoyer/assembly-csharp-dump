using System;

namespace Microsoft.Win32
{
	/// <summary>Specifies which registry view to target on a 64-bit operating system.</summary>
	public enum RegistryView
	{
		/// <summary>The default view.</summary>
		Default,
		/// <summary>The 64-bit view.</summary>
		Registry64 = 256,
		/// <summary>The 32-bit view.</summary>
		Registry32 = 512
	}
}
