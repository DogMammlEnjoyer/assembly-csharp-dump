using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System.Threading
{
	/// <summary>Propagates notification that operations should be canceled.</summary>
	[DebuggerDisplay("IsCancellationRequested = {IsCancellationRequested}")]
	public readonly struct CancellationToken
	{
		/// <summary>Returns an empty <see cref="T:System.Threading.CancellationToken" /> value.</summary>
		/// <returns>An empty cancellation token.</returns>
		public static CancellationToken None
		{
			get
			{
				return default(CancellationToken);
			}
		}

		/// <summary>Gets whether cancellation has been requested for this token.</summary>
		/// <returns>
		///   <see langword="true" /> if cancellation has been requested for this token; otherwise, <see langword="false" />.</returns>
		public bool IsCancellationRequested
		{
			get
			{
				return this._source != null && this._source.IsCancellationRequested;
			}
		}

		/// <summary>Gets whether this token is capable of being in the canceled state.</summary>
		/// <returns>
		///   <see langword="true" /> if this token is capable of being in the canceled state; otherwise, <see langword="false" />.</returns>
		public bool CanBeCanceled
		{
			get
			{
				return this._source != null;
			}
		}

		/// <summary>Gets a <see cref="T:System.Threading.WaitHandle" /> that is signaled when the token is canceled.</summary>
		/// <returns>A <see cref="T:System.Threading.WaitHandle" /> that is signaled when the token is canceled.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The associated <see cref="T:System.Threading.CancellationTokenSource" /> has been disposed.</exception>
		public WaitHandle WaitHandle
		{
			get
			{
				return (this._source ?? CancellationTokenSource.s_neverCanceledSource).WaitHandle;
			}
		}

		internal CancellationToken(CancellationTokenSource source)
		{
			this._source = source;
		}

		/// <summary>Initializes the <see cref="T:System.Threading.CancellationToken" />.</summary>
		/// <param name="canceled">The canceled state for the token.</param>
		public CancellationToken(bool canceled)
		{
			this = new CancellationToken(canceled ? CancellationTokenSource.s_canceledSource : null);
		}

		/// <summary>Registers a delegate that will be called when this <see cref="T:System.Threading.CancellationToken" /> is canceled.</summary>
		/// <param name="callback">The delegate to be executed when the <see cref="T:System.Threading.CancellationToken" /> is canceled.</param>
		/// <returns>The <see cref="T:System.Threading.CancellationTokenRegistration" /> instance that can be used to unregister the callback.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The associated <see cref="T:System.Threading.CancellationTokenSource" /> has been disposed.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="callback" /> is null.</exception>
		public CancellationTokenRegistration Register(Action callback)
		{
			Action<object> callback2 = CancellationToken.s_actionToActionObjShunt;
			if (callback == null)
			{
				throw new ArgumentNullException("callback");
			}
			return this.Register(callback2, callback, false, true);
		}

		/// <summary>Registers a delegate that will be called when this <see cref="T:System.Threading.CancellationToken" /> is canceled.</summary>
		/// <param name="callback">The delegate to be executed when the <see cref="T:System.Threading.CancellationToken" /> is canceled.</param>
		/// <param name="useSynchronizationContext">A value that indicates whether to capture the current <see cref="T:System.Threading.SynchronizationContext" /> and use it when invoking the <paramref name="callback" />.</param>
		/// <returns>The <see cref="T:System.Threading.CancellationTokenRegistration" /> instance that can be used to unregister the callback.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The associated <see cref="T:System.Threading.CancellationTokenSource" /> has been disposed.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="callback" /> is null.</exception>
		public CancellationTokenRegistration Register(Action callback, bool useSynchronizationContext)
		{
			Action<object> callback2 = CancellationToken.s_actionToActionObjShunt;
			if (callback == null)
			{
				throw new ArgumentNullException("callback");
			}
			return this.Register(callback2, callback, useSynchronizationContext, true);
		}

		/// <summary>Registers a delegate that will be called when this <see cref="T:System.Threading.CancellationToken" /> is canceled.</summary>
		/// <param name="callback">The delegate to be executed when the <see cref="T:System.Threading.CancellationToken" /> is canceled.</param>
		/// <param name="state">The state to pass to the <paramref name="callback" /> when the delegate is invoked. This may be null.</param>
		/// <returns>The <see cref="T:System.Threading.CancellationTokenRegistration" /> instance that can be used to unregister the callback.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The associated <see cref="T:System.Threading.CancellationTokenSource" /> has been disposed.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="callback" /> is null.</exception>
		public CancellationTokenRegistration Register(Action<object> callback, object state)
		{
			return this.Register(callback, state, false, true);
		}

		/// <summary>Registers a delegate that will be called when this <see cref="T:System.Threading.CancellationToken" /> is canceled.</summary>
		/// <param name="callback">The delegate to be executed when the <see cref="T:System.Threading.CancellationToken" /> is canceled.</param>
		/// <param name="state">The state to pass to the <paramref name="callback" /> when the delegate is invoked. This may be null.</param>
		/// <param name="useSynchronizationContext">A Boolean value that indicates whether to capture the current <see cref="T:System.Threading.SynchronizationContext" /> and use it when invoking the <paramref name="callback" />.</param>
		/// <returns>The <see cref="T:System.Threading.CancellationTokenRegistration" /> instance that can be used to unregister the callback.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The associated <see cref="T:System.Threading.CancellationTokenSource" /> has been disposed.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="callback" /> is null.</exception>
		public CancellationTokenRegistration Register(Action<object> callback, object state, bool useSynchronizationContext)
		{
			return this.Register(callback, state, useSynchronizationContext, true);
		}

		internal CancellationTokenRegistration InternalRegisterWithoutEC(Action<object> callback, object state)
		{
			return this.Register(callback, state, false, false);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public CancellationTokenRegistration Register(Action<object> callback, object state, bool useSynchronizationContext, bool useExecutionContext)
		{
			if (callback == null)
			{
				throw new ArgumentNullException("callback");
			}
			CancellationTokenSource source = this._source;
			if (source == null)
			{
				return default(CancellationTokenRegistration);
			}
			return source.InternalRegister(callback, state, useSynchronizationContext ? SynchronizationContext.Current : null, useExecutionContext ? ExecutionContext.Capture() : null);
		}

		/// <summary>Determines whether the current <see cref="T:System.Threading.CancellationToken" /> instance is equal to the specified token.</summary>
		/// <param name="other">The other <see cref="T:System.Threading.CancellationToken" /> to compare with this instance.</param>
		/// <returns>
		///   <see langword="true" /> if the instances are equal; otherwise, <see langword="false" />. See the Remarks section for more information.</returns>
		public bool Equals(CancellationToken other)
		{
			return this._source == other._source;
		}

		/// <summary>Determines whether the current <see cref="T:System.Threading.CancellationToken" /> instance is equal to the specified <see cref="T:System.Object" />.</summary>
		/// <param name="other">The other object to compare with this instance.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="other" /> is a <see cref="T:System.Threading.CancellationToken" /> and if the two instances are equal; otherwise, <see langword="false" />. See the Remarks section for more information.</returns>
		/// <exception cref="T:System.ObjectDisposedException">An associated <see cref="T:System.Threading.CancellationTokenSource" /> has been disposed.</exception>
		public override bool Equals(object other)
		{
			return other is CancellationToken && this.Equals((CancellationToken)other);
		}

		/// <summary>Serves as a hash function for a <see cref="T:System.Threading.CancellationToken" />.</summary>
		/// <returns>A hash code for the current <see cref="T:System.Threading.CancellationToken" /> instance.</returns>
		public override int GetHashCode()
		{
			return (this._source ?? CancellationTokenSource.s_neverCanceledSource).GetHashCode();
		}

		/// <summary>Determines whether two <see cref="T:System.Threading.CancellationToken" /> instances are equal.</summary>
		/// <param name="left">The first instance.</param>
		/// <param name="right">The second instance.</param>
		/// <returns>
		///   <see langword="true" /> if the instances are equal; otherwise, <see langword="false" /> See the Remarks section for more information.</returns>
		/// <exception cref="T:System.ObjectDisposedException">An associated <see cref="T:System.Threading.CancellationTokenSource" /> has been disposed.</exception>
		public static bool operator ==(CancellationToken left, CancellationToken right)
		{
			return left.Equals(right);
		}

		/// <summary>Determines whether two <see cref="T:System.Threading.CancellationToken" /> instances are not equal.</summary>
		/// <param name="left">The first instance.</param>
		/// <param name="right">The second instance.</param>
		/// <returns>
		///   <see langword="true" /> if the instances are not equal; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ObjectDisposedException">An associated <see cref="T:System.Threading.CancellationTokenSource" /> has been disposed.</exception>
		public static bool operator !=(CancellationToken left, CancellationToken right)
		{
			return !left.Equals(right);
		}

		/// <summary>Throws a <see cref="T:System.OperationCanceledException" /> if this token has had cancellation requested.</summary>
		/// <exception cref="T:System.OperationCanceledException">The token has had cancellation requested.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The associated <see cref="T:System.Threading.CancellationTokenSource" /> has been disposed.</exception>
		public void ThrowIfCancellationRequested()
		{
			if (this.IsCancellationRequested)
			{
				this.ThrowOperationCanceledException();
			}
		}

		private void ThrowOperationCanceledException()
		{
			throw new OperationCanceledException("The operation was canceled.", this);
		}

		private readonly CancellationTokenSource _source;

		private static readonly Action<object> s_actionToActionObjShunt = delegate(object obj)
		{
			((Action)obj)();
		};
	}
}
