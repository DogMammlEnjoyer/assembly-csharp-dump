using System;
using Unity;

namespace System.Timers
{
	/// <summary>Provides data for the <see cref="E:System.Timers.Timer.Elapsed" /> event.</summary>
	public class ElapsedEventArgs : EventArgs
	{
		internal ElapsedEventArgs(DateTime time)
		{
			this.time = time;
		}

		/// <summary>Gets the date/time when the <see cref="E:System.Timers.Timer.Elapsed" /> event was raised.</summary>
		/// <returns>The time the <see cref="E:System.Timers.Timer.Elapsed" /> event was raised.</returns>
		public DateTime SignalTime
		{
			get
			{
				return this.time;
			}
		}

		internal ElapsedEventArgs()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private DateTime time;
	}
}
