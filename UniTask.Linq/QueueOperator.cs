using System;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class QueueOperator<TSource> : IUniTaskAsyncEnumerable<TSource>
	{
		public QueueOperator(IUniTaskAsyncEnumerable<TSource> source)
		{
			this.source = source;
		}

		public IUniTaskAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new QueueOperator<TSource>._Queue(this.source, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TSource> source;

		private sealed class _Queue : IUniTaskAsyncEnumerator<TSource>, IUniTaskAsyncDisposable
		{
			public _Queue(IUniTaskAsyncEnumerable<TSource> source, CancellationToken cancellationToken)
			{
				this.source = source;
				this.cancellationToken = cancellationToken;
			}

			public TSource Current
			{
				get
				{
					return this.channelEnumerator.Current;
				}
			}

			public UniTask<bool> MoveNextAsync()
			{
				this.cancellationToken.ThrowIfCancellationRequested();
				if (this.sourceEnumerator == null)
				{
					this.sourceEnumerator = this.source.GetAsyncEnumerator(this.cancellationToken);
					this.channel = Channel.CreateSingleConsumerUnbounded<TSource>();
					this.channelEnumerator = this.channel.Reader.ReadAllAsync(default(CancellationToken)).GetAsyncEnumerator(this.cancellationToken);
					QueueOperator<TSource>._Queue.ConsumeAll(this, this.sourceEnumerator, this.channel).Forget();
				}
				return this.channelEnumerator.MoveNextAsync();
			}

			private static UniTaskVoid ConsumeAll(QueueOperator<TSource>._Queue self, IUniTaskAsyncEnumerator<TSource> enumerator, ChannelWriter<TSource> writer)
			{
				QueueOperator<TSource>._Queue.<ConsumeAll>d__10 <ConsumeAll>d__;
				<ConsumeAll>d__.<>t__builder = AsyncUniTaskVoidMethodBuilder.Create();
				<ConsumeAll>d__.self = self;
				<ConsumeAll>d__.enumerator = enumerator;
				<ConsumeAll>d__.writer = writer;
				<ConsumeAll>d__.<>1__state = -1;
				<ConsumeAll>d__.<>t__builder.Start<QueueOperator<TSource>._Queue.<ConsumeAll>d__10>(ref <ConsumeAll>d__);
				return <ConsumeAll>d__.<>t__builder.Task;
			}

			public UniTask DisposeAsync()
			{
				QueueOperator<TSource>._Queue.<DisposeAsync>d__11 <DisposeAsync>d__;
				<DisposeAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder.Create();
				<DisposeAsync>d__.<>4__this = this;
				<DisposeAsync>d__.<>1__state = -1;
				<DisposeAsync>d__.<>t__builder.Start<QueueOperator<TSource>._Queue.<DisposeAsync>d__11>(ref <DisposeAsync>d__);
				return <DisposeAsync>d__.<>t__builder.Task;
			}

			private readonly IUniTaskAsyncEnumerable<TSource> source;

			private CancellationToken cancellationToken;

			private Channel<TSource> channel;

			private IUniTaskAsyncEnumerator<TSource> channelEnumerator;

			private IUniTaskAsyncEnumerator<TSource> sourceEnumerator;

			private bool channelClosed;
		}
	}
}
