using System;

namespace Cysharp.Threading.Tasks
{
	public abstract class MoveNextSource : IUniTaskSource<bool>, IUniTaskSource
	{
		public bool GetResult(short token)
		{
			return this.completionSource.GetResult(token);
		}

		public UniTaskStatus GetStatus(short token)
		{
			return this.completionSource.GetStatus(token);
		}

		public void OnCompleted(Action<object> continuation, object state, short token)
		{
			this.completionSource.OnCompleted(continuation, state, token);
		}

		public UniTaskStatus UnsafeGetStatus()
		{
			return this.completionSource.UnsafeGetStatus();
		}

		void IUniTaskSource.GetResult(short token)
		{
			this.completionSource.GetResult(token);
		}

		protected bool TryGetResult<T>(UniTask<T>.Awaiter awaiter, out T result)
		{
			bool result2;
			try
			{
				result = awaiter.GetResult();
				result2 = true;
			}
			catch (Exception error)
			{
				this.completionSource.TrySetException(error);
				result = default(T);
				result2 = false;
			}
			return result2;
		}

		protected bool TryGetResult(UniTask.Awaiter awaiter)
		{
			bool result;
			try
			{
				awaiter.GetResult();
				result = true;
			}
			catch (Exception error)
			{
				this.completionSource.TrySetException(error);
				result = false;
			}
			return result;
		}

		protected UniTaskCompletionSourceCore<bool> completionSource;
	}
}
