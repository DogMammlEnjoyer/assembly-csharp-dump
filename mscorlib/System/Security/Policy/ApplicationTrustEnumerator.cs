using System;
using System.Collections;
using System.Runtime.InteropServices;
using Unity;

namespace System.Security.Policy
{
	/// <summary>Represents the enumerator for <see cref="T:System.Security.Policy.ApplicationTrust" /> objects in the <see cref="T:System.Security.Policy.ApplicationTrustCollection" /> collection.</summary>
	[ComVisible(true)]
	public sealed class ApplicationTrustEnumerator : IEnumerator
	{
		internal ApplicationTrustEnumerator(ApplicationTrustCollection atc)
		{
			this.trusts = atc;
			this.current = -1;
		}

		/// <summary>Gets the current <see cref="T:System.Security.Policy.ApplicationTrust" /> object in the <see cref="T:System.Security.Policy.ApplicationTrustCollection" /> collection.</summary>
		/// <returns>The current <see cref="T:System.Security.Policy.ApplicationTrust" /> in the <see cref="T:System.Security.Policy.ApplicationTrustCollection" />.</returns>
		public ApplicationTrust Current
		{
			get
			{
				return this.trusts[this.current];
			}
		}

		/// <summary>Gets the current <see cref="T:System.Object" /> in the <see cref="T:System.Security.Policy.ApplicationTrustCollection" /> collection.</summary>
		/// <returns>The current <see cref="T:System.Object" /> in the <see cref="T:System.Security.Policy.ApplicationTrustCollection" />.</returns>
		object IEnumerator.Current
		{
			get
			{
				return this.trusts[this.current];
			}
		}

		/// <summary>Resets the enumerator to the beginning of the <see cref="T:System.Security.Policy.ApplicationTrustCollection" /> collection.</summary>
		public void Reset()
		{
			this.current = -1;
		}

		/// <summary>Moves to the next element in the <see cref="T:System.Security.Policy.ApplicationTrustCollection" /> collection.</summary>
		/// <returns>
		///   <see langword="true" /> if the enumerator was successfully advanced to the next element; <see langword="false" /> if the enumerator has passed the end of the collection.</returns>
		[SecuritySafeCritical]
		public bool MoveNext()
		{
			if (this.current == this.trusts.Count - 1)
			{
				return false;
			}
			this.current++;
			return true;
		}

		internal ApplicationTrustEnumerator()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private ApplicationTrustCollection trusts;

		private int current;
	}
}
