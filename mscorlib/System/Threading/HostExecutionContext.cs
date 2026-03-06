using System;

namespace System.Threading
{
	/// <summary>Encapsulates and propagates the host execution context across threads.</summary>
	[MonoTODO("Useless until the runtime supports it")]
	public class HostExecutionContext : IDisposable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Threading.HostExecutionContext" /> class.</summary>
		public HostExecutionContext()
		{
			this._state = null;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Threading.HostExecutionContext" /> class using the specified state.</summary>
		/// <param name="state">An object representing the host execution context state.</param>
		public HostExecutionContext(object state)
		{
			this._state = state;
		}

		/// <summary>Creates a copy of the current host execution context.</summary>
		/// <returns>A <see cref="T:System.Threading.HostExecutionContext" /> object representing the host context for the current thread.</returns>
		public virtual HostExecutionContext CreateCopy()
		{
			return new HostExecutionContext(this._state);
		}

		/// <summary>Gets or sets the state of the host execution context.</summary>
		/// <returns>An object representing the host execution context state.</returns>
		protected internal object State
		{
			get
			{
				return this._state;
			}
			set
			{
				this._state = value;
			}
		}

		/// <summary>Releases all resources used by the current instance of the <see cref="T:System.Threading.HostExecutionContext" /> class.</summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>When overridden in a derived class, releases the unmanaged resources used by the <see cref="T:System.Threading.WaitHandle" />, and optionally releases the managed resources.</summary>
		/// <param name="disposing">
		///   <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
		public virtual void Dispose(bool disposing)
		{
		}

		private object _state;
	}
}
