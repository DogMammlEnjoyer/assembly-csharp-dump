using System;
using System.Collections;

namespace System.Security.AccessControl
{
	/// <summary>Represents a collection of <see cref="T:System.Security.AccessControl.AuthorizationRule" /> objects.</summary>
	public sealed class AuthorizationRuleCollection : ReadOnlyCollectionBase
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Security.AccessControl.AuthorizationRuleCollection" /> class.</summary>
		public AuthorizationRuleCollection()
		{
		}

		internal AuthorizationRuleCollection(AuthorizationRule[] rules)
		{
			base.InnerList.AddRange(rules);
		}

		/// <summary>Adds an <see cref="T:System.Security.AccessControl.AuthorizationRule" /> object to the collection.</summary>
		/// <param name="rule">The <see cref="T:System.Security.AccessControl.AuthorizationRule" /> object to add to the collection.</param>
		public void AddRule(AuthorizationRule rule)
		{
			base.InnerList.Add(rule);
		}

		/// <summary>Gets the <see cref="T:System.Security.AccessControl.AuthorizationRule" /> object at the specified index of the collection.</summary>
		/// <param name="index">The zero-based index of the <see cref="T:System.Security.AccessControl.AuthorizationRule" /> object to get.</param>
		/// <returns>The <see cref="T:System.Security.AccessControl.AuthorizationRule" /> object at the specified index.</returns>
		public AuthorizationRule this[int index]
		{
			get
			{
				return (AuthorizationRule)base.InnerList[index];
			}
		}

		/// <summary>Copies the contents of the collection to an array.</summary>
		/// <param name="rules">An array to which to copy the contents of the collection.</param>
		/// <param name="index">The zero-based index from which to begin copying.</param>
		public void CopyTo(AuthorizationRule[] rules, int index)
		{
			base.InnerList.CopyTo(rules, index);
		}
	}
}
