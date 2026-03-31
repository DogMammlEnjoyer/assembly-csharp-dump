using System;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class SkipUntil<TSource> : IUniTaskAsyncEnumerable<TSource>
	{
		public SkipUntil(IUniTaskAsyncEnumerable<TSource> source, UniTask other, Func<CancellationToken, UniTask> other2)
		{
			this.source = source;
			this.other = other;
			this.other2 = other2;
		}

		public IUniTaskAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (this.other2 != null)
			{
				return new SkipUntil<TSource>._SkipUntil(this.source, this.other2(cancellationToken), cancellationToken);
			}
			return new SkipUntil<TSource>._SkipUntil(this.source, this.other, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TSource> source;

		private readonly UniTask other;

		private readonly Func<CancellationToken, UniTask> other2;

		private sealed class _SkipUntil : MoveNextSource, IUniTaskAsyncEnumerator<TSource>, IUniTaskAsyncDisposable
		{
			public _SkipUntil(IUniTaskAsyncEnumerable<TSource> source, UniTask other, CancellationToken cancellationToken1)
			{
				this.source = source;
				this.cancellationToken1 = cancellationToken1;
				if (cancellationToken1.CanBeCanceled)
				{
					this.cancellationTokenRegistration1 = cancellationToken1.RegisterWithoutCaptureExecutionContext(SkipUntil<TSource>._SkipUntil.CancelDelegate1, this);
				}
				this.RunOther(other).Forget();
			}

			public TSource Current { get; private set; }

			public UniTask<bool> MoveNextAsync()
			{
				if (this.exception != null)
				{
					return UniTask.FromException<bool>(this.exception);
				}
				if (this.cancellationToken1.IsCancellationRequested)
				{
					return UniTask.FromCanceled<bool>(this.cancellationToken1);
				}
				if (this.enumerator == null)
				{
					this.enumerator = this.source.GetAsyncEnumerator(this.cancellationToken1);
				}
				this.completionSource.Reset();
				if (this.completed)
				{
					this.SourceMoveNext();
				}
				return new UniTask<bool>(this, this.completionSource.Version);
			}

			private void SourceMoveNext()
			{
				try
				{
					for (;;)
					{
						this.awaiter = this.enumerator.MoveNextAsync().GetAwaiter();
						if (!this.awaiter.IsCompleted)
						{
							break;
						}
						this.continueNext = true;
						SkipUntil<TSource>._SkipUntil.MoveNextCore(this);
						if (!this.continueNext)
						{
							goto IL_55;
						}
						this.continueNext = false;
					}
					this.awaiter.SourceOnCompleted(SkipUntil<TSource>._SkipUntil.MoveNextCoreDelegate, this);
					IL_55:;
				}
				catch (Exception error)
				{
					this.completionSource.TrySetException(error);
				}
			}

			private static void MoveNextCore(object state)
			{
				SkipUntil<TSource>._SkipUntil skipUntil = (SkipUntil<TSource>._SkipUntil)state;
				bool flag;
				if (skipUntil.TryGetResult<bool>(skipUntil.awaiter, out flag))
				{
					if (flag)
					{
						skipUntil.Current = skipUntil.enumerator.Current;
						skipUntil.completionSource.TrySetResult(true);
						if (skipUntil.continueNext)
						{
							skipUntil.SourceMoveNext();
							return;
						}
					}
					else
					{
						skipUntil.completionSource.TrySetResult(false);
					}
				}
			}

			private UniTaskVoid RunOther(UniTask other)
			{
				SkipUntil<TSource>._SkipUntil.<RunOther>d__18 <RunOther>d__;
				<RunOther>d__.<>t__builder = AsyncUniTaskVoidMethodBuilder.Create();
				<RunOther>d__.<>4__this = this;
				<RunOther>d__.other = other;
				<RunOther>d__.<>1__state = -1;
				<RunOther>d__.<>t__builder.Start<SkipUntil<TSource>._SkipUntil.<RunOther>d__18>(ref <RunOther>d__);
				return <RunOther>d__.<>t__builder.Task;
			}

			private static void OnCanceled1(object state)
			{
				SkipUntil<TSource>._SkipUntil skipUntil = (SkipUntil<TSource>._SkipUntil)state;
				skipUntil.completionSource.TrySetCanceled(skipUntil.cancellationToken1);
			}

			public UniTask DisposeAsync()
			{
				this.cancellationTokenRegistration1.Dispose();
				if (this.enumerator != null)
				{
					return this.enumerator.DisposeAsync();
				}
				return default(UniTask);
			}

			private static readonly Action<object> CancelDelegate1 = new Action<object>(SkipUntil<TSource>._SkipUntil.OnCanceled1);

			private static readonly Action<object> MoveNextCoreDelegate = new Action<object>(SkipUntil<TSource>._SkipUntil.MoveNextCore);

			private readonly IUniTaskAsyncEnumerable<TSource> source;

			private CancellationToken cancellationToken1;

			private bool completed;

			private CancellationTokenRegistration cancellationTokenRegistration1;

			private IUniTaskAsyncEnumerator<TSource> enumerator;

			private UniTask<bool>.Awaiter awaiter;

			private bool continueNext;

			private Exception exception;
		}
	}
}
