using System;

namespace System.Net.NetworkInformation
{
	/// <summary>The scope level for an IPv6 address.</summary>
	public enum ScopeLevel
	{
		/// <summary>The scope level is not specified.</summary>
		None,
		/// <summary>The scope is interface-level.</summary>
		Interface,
		/// <summary>The scope is link-level.</summary>
		Link,
		/// <summary>The scope is subnet-level.</summary>
		Subnet,
		/// <summary>The scope is admin-level.</summary>
		Admin,
		/// <summary>The scope is site-level.</summary>
		Site,
		/// <summary>The scope is organization-level.</summary>
		Organization = 8,
		/// <summary>The scope is global.</summary>
		Global = 14
	}
}
