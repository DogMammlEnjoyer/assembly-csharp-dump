using System;
using System.Security;
using System.Security.Permissions;
using Unity;

namespace System.Diagnostics.PerformanceData
{
	/// <summary>Creates an instance of the logical counters defined in the <see cref="T:System.Diagnostics.PerformanceData.CounterSet" /> class.</summary>
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public sealed class CounterSetInstance : IDisposable
	{
		internal CounterSetInstance()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Retrieves the collection of counter data for the counter set instance.</summary>
		/// <returns>A collection of the counter data contained in the counter set instance.</returns>
		public CounterSetInstanceCounterDataSet Counters
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
