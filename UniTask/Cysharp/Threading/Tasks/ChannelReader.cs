using System;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks
{
	public abstract class ChannelReader<T>
	{
		public abstract bool TryRead(out T item);

		public abstract UniTask<bool> WaitToReadAsync(CancellationToken cancellationToken = default(CancellationToken));

		public abstract UniTask Completion { get; }

		public virtual UniTask<T> ReadAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			T value;
			if (this.TryRead(out value))
			{
				return UniTask.FromResult<T>(value);
			}
			return this.ReadAsyncCore(cancellationToken);
		}

		private UniTask<T> ReadAsyncCore(CancellationToken cancellationToken = default(CancellationToken))
		{
			ChannelReader<T>.<ReadAsyncCore>d__5 <ReadAsyncCore>d__;
			<ReadAsyncCore>d__.<>t__builder = AsyncUniTaskMethodBuilder<T>.Create();
			<ReadAsyncCore>d__.<>4__this = this;
			<ReadAsyncCore>d__.cancellationToken = cancellationToken;
			<ReadAsyncCore>d__.<>1__state = -1;
			<ReadAsyncCore>d__.<>t__builder.Start<ChannelReader<T>.<ReadAsyncCore>d__5>(ref <ReadAsyncCore>d__);
			return <ReadAsyncCore>d__.<>t__builder.Task;
		}

		public abstract IUniTaskAsyncEnumerable<T> ReadAllAsync(CancellationToken cancellationToken = default(CancellationToken));
	}
}
