using System;
using System.Security;
using System.Security.Permissions;
using System.Threading;
using System.Threading.Tasks;

namespace System.Runtime.CompilerServices
{
	/// <summary>Provides the context for waiting when asynchronously switching into a target environment.</summary>
	public readonly struct YieldAwaitable
	{
		/// <summary>Retrieves a <see cref="T:System.Runtime.CompilerServices.YieldAwaitable.YieldAwaiter" /> object  for this instance of the class.</summary>
		/// <returns>The object that is used to monitor the completion of an asynchronous operation.</returns>
		public YieldAwaitable.YieldAwaiter GetAwaiter()
		{
			return default(YieldAwaitable.YieldAwaiter);
		}

		/// <summary>Provides an awaiter for switching into a target environment.</summary>
		[HostProtection(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
		public readonly struct YieldAwaiter : ICriticalNotifyCompletion, INotifyCompletion
		{
			/// <summary>Gets a value that indicates whether a yield is not required.</summary>
			/// <returns>Always <see langword="false" />, which indicates that a yield is always required for <see cref="T:System.Runtime.CompilerServices.YieldAwaitable.YieldAwaiter" />.</returns>
			public bool IsCompleted
			{
				get
				{
					return false;
				}
			}

			/// <summary>Sets the continuation to invoke.</summary>
			/// <param name="continuation">The action to invoke asynchronously.</param>
			/// <exception cref="T:System.ArgumentNullException">
			///   <paramref name="continuation" /> is <see langword="null" />.</exception>
			[SecuritySafeCritical]
			public void OnCompleted(Action continuation)
			{
				YieldAwaitable.YieldAwaiter.QueueContinuation(continuation, true);
			}

			/// <summary>Posts the <paramref name="continuation" /> back to the current context.</summary>
			/// <param name="continuation">The action to invoke asynchronously.</param>
			/// <exception cref="T:System.ArgumentNullException">The <paramref name="continuation" /> argument is <see langword="null" />.</exception>
			[SecurityCritical]
			public void UnsafeOnCompleted(Action continuation)
			{
				YieldAwaitable.YieldAwaiter.QueueContinuation(continuation, false);
			}

			[SecurityCritical]
			private static void QueueContinuation(Action continuation, bool flowContext)
			{
				if (continuation == null)
				{
					throw new ArgumentNullException("continuation");
				}
				SynchronizationContext currentNoFlow = SynchronizationContext.CurrentNoFlow;
				if (currentNoFlow != null && currentNoFlow.GetType() != typeof(SynchronizationContext))
				{
					currentNoFlow.Post(YieldAwaitable.YieldAwaiter.s_sendOrPostCallbackRunAction, continuation);
					return;
				}
				TaskScheduler taskScheduler = TaskScheduler.Current;
				if (taskScheduler != TaskScheduler.Default)
				{
					Task.Factory.StartNew(continuation, default(CancellationToken), TaskCreationOptions.PreferFairness, taskScheduler);
					return;
				}
				if (flowContext)
				{
					ThreadPool.QueueUserWorkItem(YieldAwaitable.YieldAwaiter.s_waitCallbackRunAction, continuation);
					return;
				}
				ThreadPool.UnsafeQueueUserWorkItem(YieldAwaitable.YieldAwaiter.s_waitCallbackRunAction, continuation);
			}

			private static void RunAction(object state)
			{
				((Action)state)();
			}

			/// <summary>Ends the await operation.</summary>
			public void GetResult()
			{
			}

			private static readonly WaitCallback s_waitCallbackRunAction = new WaitCallback(YieldAwaitable.YieldAwaiter.RunAction);

			private static readonly SendOrPostCallback s_sendOrPostCallbackRunAction = new SendOrPostCallback(YieldAwaitable.YieldAwaiter.RunAction);
		}
	}
}
