using System;
using System.Security;
using System.Security.Permissions;
using Unity;

namespace System.Diagnostics.PerformanceData
{
	/// <summary>Contains the collection of counter values.</summary>
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public sealed class CounterSetInstanceCounterDataSet : IDisposable
	{
		internal CounterSetInstanceCounterDataSet()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		public CounterData get_Item(int counterId)
		{
			ThrowStub.ThrowNotSupportedException();
			return null;
		}

		/// <summary>Accesses a counter value in the collection by using the specified counter name.</summary>
		/// <param name="counterName">Name of the counter. This is the name that you used when you added the counter to the counter set.</param>
		/// <returns>The counter data.</returns>
		public CounterData this[string counterName]
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Releases all unmanaged resources used by this object.</summary>
		[SecurityCritical]
		public void Dispose()
		{
			ThrowStub.ThrowNotSupportedException();
		}
	}
}
