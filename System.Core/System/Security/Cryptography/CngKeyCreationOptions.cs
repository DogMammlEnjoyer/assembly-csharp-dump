using System;

namespace System.Security.Cryptography
{
	/// <summary>Specifies options used for key creation.</summary>
	[Flags]
	public enum CngKeyCreationOptions
	{
		/// <summary>No key creation options are used.</summary>
		None = 0,
		/// <summary>A machine-wide key is created.</summary>
		MachineKey = 32,
		/// <summary>The existing key is overwritten during key creation.</summary>
		OverwriteExistingKey = 128
	}
}
