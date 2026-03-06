using System;
using System.Security;
using System.Security.Permissions;
using Unity;

namespace System.Diagnostics.PerformanceData
{
	/// <summary>Contains the raw data for a counter.</summary>
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public sealed class CounterData
	{
		internal CounterData()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Sets or gets the raw counter data.</summary>
		/// <returns>The raw counter data.</returns>
		public long RawValue
		{
			[SecurityCritical]
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return 0L;
			}
			[SecurityCritical]
			set
			{
				ThrowStub.ThrowNotSupportedException();
			}
		}

		/// <summary>Sets or gets the counter data.</summary>
		/// <returns>The counter data.</returns>
		public long Value
		{
			[SecurityCritical]
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return 0L;
			}
			[SecurityCritical]
			set
			{
				ThrowStub.ThrowNotSupportedException();
			}
		}

		/// <summary>Decrements the counter value by 1.</summary>
		[SecurityCritical]
		public void Decrement()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Increments the counter value by 1.</summary>
		[SecurityCritical]
		public void Increment()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Increments the counter value by the specified amount.</summary>
		/// <param name="value">The amount by which to increment the counter value. The increment value can be positive or negative.</param>
		[SecurityCritical]
		public void IncrementBy(long value)
		{
			ThrowStub.ThrowNotSupportedException();
		}
	}
}
