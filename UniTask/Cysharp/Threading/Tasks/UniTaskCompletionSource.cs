using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace Cysharp.Threading.Tasks
{
	public class UniTaskCompletionSource : IUniTaskSource, IPromise, IResolvePromise, IRejectPromise, ICancelPromise
	{
		[DebuggerHidden]
		internal void MarkHandled()
		{
			if (!this.handled)
			{
				this.handled = true;
			}
		}

		public UniTask Task
		{
			[DebuggerHidden]
			get
			{
				return new UniTask(this, 0);
			}
		}

		[DebuggerHidden]
		public bool TrySetResult()
		{
			return this.TrySignalCompletion(UniTaskStatus.Succeeded);
		}

		[DebuggerHidden]
		public bool TrySetCanceled(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (this.UnsafeGetStatus() != UniTaskStatus.Pending)
			{
				return false;
			}
			this.cancellationToken = cancellationToken;
			return this.TrySignalCompletion(UniTaskStatus.Canceled);
		}

		[DebuggerHidden]
		public bool TrySetException(Exception exception)
		{
			OperationCanceledException ex = exception as OperationCanceledException;
			if (ex != null)
			{
				return this.TrySetCanceled(ex.CancellationToken);
			}
			if (this.UnsafeGetStatus() != UniTaskStatus.Pending)
			{
				return false;
			}
			this.exception = new ExceptionHolder(ExceptionDispatchInfo.Capture(exception));
			return this.TrySignalCompletion(UniTaskStatus.Faulted);
		}

		[DebuggerHidden]
		public void GetResult(short token)
		{
			this.MarkHandled();
			switch (this.intStatus)
			{
			case 1:
				return;
			case 2:
				this.exception.GetException().Throw();
				return;
			case 3:
				throw new OperationCanceledException(this.cancellationToken);
			}
			throw new InvalidOperationException("not yet completed.");
		}

		[DebuggerHidden]
		public UniTaskStatus GetStatus(short token)
		{
			return (UniTaskStatus)this.intStatus;
		}

		[DebuggerHidden]
		public UniTaskStatus UnsafeGetStatus()
		{
			return (UniTaskStatus)this.intStatus;
		}

		[DebuggerHidden]
		public void OnCompleted(Action<object> continuation, object state, short token)
		{
			if (this.gate == null)
			{
				Interlocked.CompareExchange(ref this.gate, new object(), null);
			}
			object obj = Thread.VolatileRead(ref this.gate);
			lock (obj)
			{
				if (this.intStatus != 0)
				{
					continuation(state);
				}
				else if (this.singleContinuation == null)
				{
					this.singleContinuation = continuation;
					this.singleState = state;
				}
				else
				{
					if (this.secondaryContinuationList == null)
					{
						this.secondaryContinuationList = new List<ValueTuple<Action<object>, object>>();
					}
					this.secondaryContinuationList.Add(new ValueTuple<Action<object>, object>(continuation, state));
				}
			}
		}

		[DebuggerHidden]
		private bool TrySignalCompletion(UniTaskStatus status)
		{
			if (Interlocked.CompareExchange(ref this.intStatus, (int)status, 0) == 0)
			{
				if (this.gate == null)
				{
					Interlocked.CompareExchange(ref this.gate, new object(), null);
				}
				object obj = Thread.VolatileRead(ref this.gate);
				lock (obj)
				{
					if (this.singleContinuation != null)
					{
						try
						{
							this.singleContinuation(this.singleState);
						}
						catch (Exception ex)
						{
							UniTaskScheduler.PublishUnobservedTaskException(ex);
						}
					}
					if (this.secondaryContinuationList != null)
					{
						foreach (ValueTuple<Action<object>, object> valueTuple in this.secondaryContinuationList)
						{
							Action<object> item = valueTuple.Item1;
							object item2 = valueTuple.Item2;
							try
							{
								item(item2);
							}
							catch (Exception ex2)
							{
								UniTaskScheduler.PublishUnobservedTaskException(ex2);
							}
						}
					}
					this.singleContinuation = null;
					this.singleState = null;
					this.secondaryContinuationList = null;
				}
				return true;
			}
			return false;
		}

		private CancellationToken cancellationToken;

		private ExceptionHolder exception;

		private object gate;

		private Action<object> singleContinuation;

		private object singleState;

		private List<ValueTuple<Action<object>, object>> secondaryContinuationList;

		private int intStatus;

		private bool handled;
	}
}
