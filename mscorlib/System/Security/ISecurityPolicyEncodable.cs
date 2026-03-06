using System;
using System.Security.Policy;

namespace System.Security
{
	/// <summary>Supports the methods that convert permission object state to and from an XML element representation.</summary>
	public interface ISecurityPolicyEncodable
	{
		/// <summary>Reconstructs a security object with a specified state from an XML encoding.</summary>
		/// <param name="e">The XML encoding to use to reconstruct the security object.</param>
		/// <param name="level">The policy-level context to resolve named permission set references.</param>
		void FromXml(SecurityElement e, PolicyLevel level);

		/// <summary>Creates an XML encoding of the security object and its current state.</summary>
		/// <param name="level">The policy-level context to resolve named permission set references.</param>
		/// <returns>The root element of the XML representation of the policy object.</returns>
		SecurityElement ToXml(PolicyLevel level);
	}
}
