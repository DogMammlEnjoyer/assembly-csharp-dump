using System;
using System.Runtime.CompilerServices;

public struct OVRTaskBuilder<T>
{
	public OVRTask<T> Task
	{
		get
		{
			if (this._task != null)
			{
				return this._task.Value;
			}
			OVRTask<T>? ovrtask2;
			if (this._pooledStateMachine != null)
			{
				OVRTaskBuilder<T>.PooledStateMachine pooledStateMachine = this._pooledStateMachine;
				OVRTask<T> ovrtask = pooledStateMachine.Task.GetValueOrDefault();
				OVRTask<T> value;
				if (pooledStateMachine.Task == null)
				{
					ovrtask = OVRTask.FromGuid<T>(Guid.NewGuid());
					pooledStateMachine.Task = new OVRTask<T>?(ovrtask);
					value = ovrtask;
				}
				else
				{
					value = ovrtask;
				}
				ovrtask2 = (this._task = new OVRTask<T>?(value));
				return ovrtask2.Value;
			}
			ovrtask2 = (this._task = new OVRTask<T>?(OVRTask.FromGuid<T>(Guid.NewGuid())));
			return ovrtask2.Value;
		}
	}

	public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : INotifyCompletion where TStateMachine : IAsyncStateMachine
	{
		OVRTaskBuilder<T>.PooledStateMachine pooledStateMachine = this.GetPooledStateMachine<TStateMachine>();
		((OVRTaskBuilder<T>.PooledStateMachine<TStateMachine>)pooledStateMachine).StateMachine = stateMachine;
		awaiter.OnCompleted(pooledStateMachine.MoveNext);
	}

	public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
	{
		OVRTaskBuilder<T>.PooledStateMachine pooledStateMachine = this.GetPooledStateMachine<TStateMachine>();
		((OVRTaskBuilder<T>.PooledStateMachine<TStateMachine>)pooledStateMachine).StateMachine = stateMachine;
		awaiter.UnsafeOnCompleted(pooledStateMachine.MoveNext);
	}

	public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
	{
		((OVRTaskBuilder<T>.PooledStateMachine<TStateMachine>)this.GetPooledStateMachine<TStateMachine>()).StateMachine = stateMachine;
		stateMachine.MoveNext();
	}

	public static OVRTaskBuilder<T> Create()
	{
		return default(OVRTaskBuilder<T>);
	}

	private OVRTaskBuilder<T>.PooledStateMachine GetPooledStateMachine<TStateMachine>() where TStateMachine : IAsyncStateMachine
	{
		if (this._pooledStateMachine == null)
		{
			this._pooledStateMachine = OVRTaskBuilder<T>.PooledStateMachine<TStateMachine>.Get();
			this._pooledStateMachine.Task = this._task;
		}
		return this._pooledStateMachine;
	}

	public void SetException(Exception exception)
	{
		this.Task.SetException(exception);
		OVRTaskBuilder<T>.PooledStateMachine pooledStateMachine = this._pooledStateMachine;
		if (pooledStateMachine != null)
		{
			pooledStateMachine.Dispose();
		}
		this._pooledStateMachine = null;
	}

	public void SetResult(T result)
	{
		this.Task.SetResult(result);
		OVRTaskBuilder<T>.PooledStateMachine pooledStateMachine = this._pooledStateMachine;
		if (pooledStateMachine != null)
		{
			pooledStateMachine.Dispose();
		}
		this._pooledStateMachine = null;
	}

	public void SetStateMachine(IAsyncStateMachine stateMachine)
	{
	}

	private OVRTaskBuilder<T>.PooledStateMachine _pooledStateMachine;

	private OVRTask<T>? _task;

	private abstract class PooledStateMachine : IDisposable
	{
		public abstract void Dispose();

		public OVRTask<T>? Task;

		public Action MoveNext;
	}

	private class PooledStateMachine<TStateMachine> : OVRTaskBuilder<T>.PooledStateMachine, OVRObjectPool.IPoolObject where TStateMachine : IAsyncStateMachine
	{
		public static OVRTaskBuilder<T>.PooledStateMachine<TStateMachine> Get()
		{
			return OVRObjectPool.Get<OVRTaskBuilder<T>.PooledStateMachine<TStateMachine>>();
		}

		public override void Dispose()
		{
			OVRObjectPool.Return<OVRTaskBuilder<T>.PooledStateMachine<TStateMachine>>(this);
		}

		public PooledStateMachine()
		{
			this.MoveNext = new Action(this.ExecuteMoveNext);
		}

		private void ExecuteMoveNext()
		{
			this.StateMachine.MoveNext();
		}

		void OVRObjectPool.IPoolObject.OnGet()
		{
			this.StateMachine = default(TStateMachine);
			this.Task = null;
		}

		void OVRObjectPool.IPoolObject.OnReturn()
		{
			this.StateMachine = default(TStateMachine);
			this.Task = null;
		}

		public TStateMachine StateMachine;
	}
}
