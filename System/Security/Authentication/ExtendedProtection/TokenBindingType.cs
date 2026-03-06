using System;

namespace System.Security.Authentication.ExtendedProtection
{
	/// <summary>Represents types of token binding.</summary>
	public enum TokenBindingType
	{
		/// <summary>Used to establish a token binding when connecting to a server.</summary>
		Provided,
		/// <summary>Used when requesting tokens to be presented to a different server.</summary>
		Referred
	}
}
