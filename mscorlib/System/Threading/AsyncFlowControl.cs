using System;
using System.Security;

namespace System.Threading
{
	/// <summary>Provides the functionality to restore the migration, or flow, of the execution context between threads.</summary>
	public struct AsyncFlowControl : IDisposable
	{
		[SecurityCritical]
		internal void Setup()
		{
			this.useEC = true;
			Thread currentThread = Thread.CurrentThread;
			this._ec = currentThread.GetMutableExecutionContext();
			this._ec.isFlowSuppressed = true;
			this._thread = currentThread;
		}

		/// <summary>Releases all resources used by the current instance of the <see cref="T:System.Threading.AsyncFlowControl" /> class.</summary>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Threading.AsyncFlowControl" /> structure is not used on the thread where it was created.  
		///  -or-  
		///  The <see cref="T:System.Threading.AsyncFlowControl" /> structure has already been used to call <see cref="M:System.Threading.AsyncFlowControl.Dispose" /> or <see cref="M:System.Threading.AsyncFlowControl.Undo" />.</exception>
		public void Dispose()
		{
			this.Undo();
		}

		/// <summary>Restores the flow of the execution context between threads.</summary>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Threading.AsyncFlowControl" /> structure is not used on the thread where it was created.  
		///  -or-  
		///  The <see cref="T:System.Threading.AsyncFlowControl" /> structure has already been used to call <see cref="M:System.Threading.AsyncFlowControl.Dispose" /> or <see cref="M:System.Threading.AsyncFlowControl.Undo" />.</exception>
		[SecuritySafeCritical]
		public void Undo()
		{
			if (this._thread == null)
			{
				throw new InvalidOperationException(Environment.GetResourceString("AsyncFlowControl object can be used only once to call Undo()."));
			}
			if (this._thread != Thread.CurrentThread)
			{
				throw new InvalidOperationException(Environment.GetResourceString("AsyncFlowControl object must be used on the thread where it was created."));
			}
			if (this.useEC)
			{
				if (Thread.CurrentThread.GetMutableExecutionContext() != this._ec)
				{
					throw new InvalidOperationException(Environment.GetResourceString("AsyncFlowControl objects can be used to restore flow only on the Context that had its flow suppressed."));
				}
				ExecutionContext.RestoreFlow();
			}
			this._thread = null;
		}

		/// <summary>Gets a hash code for the current <see cref="T:System.Threading.AsyncFlowControl" /> structure.</summary>
		/// <returns>A hash code for the current <see cref="T:System.Threading.AsyncFlowControl" /> structure.</returns>
		public override int GetHashCode()
		{
			if (this._thread != null)
			{
				return this._thread.GetHashCode();
			}
			return this.ToString().GetHashCode();
		}

		/// <summary>Determines whether the specified object is equal to the current <see cref="T:System.Threading.AsyncFlowControl" /> structure.</summary>
		/// <param name="obj">An object to compare with the current structure.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="obj" /> is an <see cref="T:System.Threading.AsyncFlowControl" /> structure and is equal to the current <see cref="T:System.Threading.AsyncFlowControl" /> structure; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			return obj is AsyncFlowControl && this.Equals((AsyncFlowControl)obj);
		}

		/// <summary>Determines whether the specified <see cref="T:System.Threading.AsyncFlowControl" /> structure is equal to the current <see cref="T:System.Threading.AsyncFlowControl" /> structure.</summary>
		/// <param name="obj">An <see cref="T:System.Threading.AsyncFlowControl" /> structure to compare with the current structure.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="obj" /> is equal to the current <see cref="T:System.Threading.AsyncFlowControl" /> structure; otherwise, <see langword="false" />.</returns>
		public bool Equals(AsyncFlowControl obj)
		{
			return obj.useEC == this.useEC && obj._ec == this._ec && obj._thread == this._thread;
		}

		/// <summary>Compares two <see cref="T:System.Threading.AsyncFlowControl" /> structures to determine whether they are equal.</summary>
		/// <param name="a">An <see cref="T:System.Threading.AsyncFlowControl" /> structure.</param>
		/// <param name="b">An <see cref="T:System.Threading.AsyncFlowControl" /> structure.</param>
		/// <returns>
		///   <see langword="true" /> if the two structures are equal; otherwise, <see langword="false" />.</returns>
		public static bool operator ==(AsyncFlowControl a, AsyncFlowControl b)
		{
			return a.Equals(b);
		}

		/// <summary>Compares two <see cref="T:System.Threading.AsyncFlowControl" /> structures to determine whether they are not equal.</summary>
		/// <param name="a">An <see cref="T:System.Threading.AsyncFlowControl" /> structure.</param>
		/// <param name="b">An <see cref="T:System.Threading.AsyncFlowControl" /> structure.</param>
		/// <returns>
		///   <see langword="true" /> if the structures are not equal; otherwise, <see langword="false" />.</returns>
		public static bool operator !=(AsyncFlowControl a, AsyncFlowControl b)
		{
			return !(a == b);
		}

		private bool useEC;

		private ExecutionContext _ec;

		private Thread _thread;
	}
}
