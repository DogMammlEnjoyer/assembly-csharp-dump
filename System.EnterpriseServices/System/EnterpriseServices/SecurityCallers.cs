using System;
using System.Collections;

namespace System.EnterpriseServices
{
	/// <summary>Provides an ordered collection of identities in the current call chain.</summary>
	public sealed class SecurityCallers : IEnumerable
	{
		internal SecurityCallers()
		{
		}

		internal SecurityCallers(ISecurityCallersColl collection)
		{
		}

		/// <summary>Gets the number of callers in the chain.</summary>
		/// <returns>The number of callers in the chain.</returns>
		public int Count
		{
			[MonoTODO]
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Gets the specified <see cref="T:System.EnterpriseServices.SecurityIdentity" /> item.</summary>
		/// <param name="idx">The item to access using an index number.</param>
		/// <returns>A <see cref="T:System.EnterpriseServices.SecurityIdentity" /> object.</returns>
		public SecurityIdentity this[int idx]
		{
			[MonoTODO]
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Retrieves the enumeration interface for the object.</summary>
		/// <returns>The enumerator interface for the <see langword="ISecurityCallersColl" /> collection.</returns>
		[MonoTODO]
		public IEnumerator GetEnumerator()
		{
			throw new NotImplementedException();
		}
	}
}
