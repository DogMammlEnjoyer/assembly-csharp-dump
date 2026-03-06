using System;
using System.Threading.Tasks;

namespace System.Threading
{
	/// <summary>Represents a callback delegate that has been registered with a <see cref="T:System.Threading.CancellationToken" />.</summary>
	public readonly struct CancellationTokenRegistration : IEquatable<CancellationTokenRegistration>, IDisposable, IAsyncDisposable
	{
		internal CancellationTokenRegistration(CancellationCallbackInfo callbackInfo, SparselyPopulatedArrayAddInfo<CancellationCallbackInfo> registrationInfo)
		{
			this.m_callbackInfo = callbackInfo;
			this.m_registrationInfo = registrationInfo;
		}

		public CancellationToken Token
		{
			get
			{
				CancellationCallbackInfo callbackInfo = this.m_callbackInfo;
				if (callbackInfo == null)
				{
					return default(CancellationToken);
				}
				return callbackInfo.CancellationTokenSource.Token;
			}
		}

		public bool Unregister()
		{
			return this.m_registrationInfo.Source != null && this.m_registrationInfo.Source.SafeAtomicRemove(this.m_registrationInfo.Index, this.m_callbackInfo) == this.m_callbackInfo;
		}

		/// <summary>Releases all resources used by the current instance of the <see cref="T:System.Threading.CancellationTokenRegistration" /> class.</summary>
		public void Dispose()
		{
			bool flag = this.Unregister();
			CancellationCallbackInfo callbackInfo = this.m_callbackInfo;
			if (callbackInfo != null)
			{
				CancellationTokenSource cancellationTokenSource = callbackInfo.CancellationTokenSource;
				if (cancellationTokenSource.IsCancellationRequested && !cancellationTokenSource.IsCancellationCompleted && !flag && cancellationTokenSource.ThreadIDExecutingCallbacks != Environment.CurrentManagedThreadId)
				{
					cancellationTokenSource.WaitForCallbackToComplete(this.m_callbackInfo);
				}
			}
		}

		/// <summary>Determines whether two <see cref="T:System.Threading.CancellationTokenRegistration" /> instances are equal.</summary>
		/// <param name="left">The first instance.</param>
		/// <param name="right">The second instance.</param>
		/// <returns>True if the instances are equal; otherwise, false.</returns>
		public static bool operator ==(CancellationTokenRegistration left, CancellationTokenRegistration right)
		{
			return left.Equals(right);
		}

		/// <summary>Determines whether two <see cref="T:System.Threading.CancellationTokenRegistration" /> instances are not equal.</summary>
		/// <param name="left">The first instance.</param>
		/// <param name="right">The second instance.</param>
		/// <returns>True if the instances are not equal; otherwise, false.</returns>
		public static bool operator !=(CancellationTokenRegistration left, CancellationTokenRegistration right)
		{
			return !left.Equals(right);
		}

		/// <summary>Determines whether the current <see cref="T:System.Threading.CancellationTokenRegistration" /> instance is equal to the specified <see cref="T:System.Threading.CancellationTokenRegistration" />.</summary>
		/// <param name="obj">The other object to which to compare this instance.</param>
		/// <returns>True, if both this and <paramref name="obj" /> are equal. False, otherwise.  
		///  Two <see cref="T:System.Threading.CancellationTokenRegistration" /> instances are equal if they both refer to the output of a single call to the same Register method of a <see cref="T:System.Threading.CancellationToken" />.</returns>
		public override bool Equals(object obj)
		{
			return obj is CancellationTokenRegistration && this.Equals((CancellationTokenRegistration)obj);
		}

		/// <summary>Determines whether the current <see cref="T:System.Threading.CancellationTokenRegistration" /> instance is equal to the specified <see cref="T:System.Threading.CancellationTokenRegistration" />.</summary>
		/// <param name="other">The other <see cref="T:System.Threading.CancellationTokenRegistration" /> to which to compare this instance.</param>
		/// <returns>True, if both this and <paramref name="other" /> are equal. False, otherwise.  
		///  Two <see cref="T:System.Threading.CancellationTokenRegistration" /> instances are equal if they both refer to the output of a single call to the same Register method of a <see cref="T:System.Threading.CancellationToken" />.</returns>
		public bool Equals(CancellationTokenRegistration other)
		{
			return this.m_callbackInfo == other.m_callbackInfo && this.m_registrationInfo.Source == other.m_registrationInfo.Source && this.m_registrationInfo.Index == other.m_registrationInfo.Index;
		}

		/// <summary>Serves as a hash function for a <see cref="T:System.Threading.CancellationTokenRegistration" />.</summary>
		/// <returns>A hash code for the current <see cref="T:System.Threading.CancellationTokenRegistration" /> instance.</returns>
		public override int GetHashCode()
		{
			if (this.m_registrationInfo.Source != null)
			{
				return this.m_registrationInfo.Source.GetHashCode() ^ this.m_registrationInfo.Index.GetHashCode();
			}
			return this.m_registrationInfo.Index.GetHashCode();
		}

		public ValueTask DisposeAsync()
		{
			this.Dispose();
			return new ValueTask(Task.FromResult<object>(null));
		}

		private readonly CancellationCallbackInfo m_callbackInfo;

		private readonly SparselyPopulatedArrayAddInfo<CancellationCallbackInfo> m_registrationInfo;
	}
}
