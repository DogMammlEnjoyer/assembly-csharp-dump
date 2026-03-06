using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;
using UnityEngine;

[AsyncMethodBuilder(typeof(OVRTaskBuilder<>))]
public readonly struct OVRTask<TResult> : IEquatable<OVRTask<TResult>>, IDisposable
{
	internal OVRTask(Guid id)
	{
		this._id = id;
	}

	static OVRTask()
	{
		OVRTask.RegisterType<TResult>();
	}

	internal bool AddToPending()
	{
		return OVRTask<TResult>.Pending.Add(this._id);
	}

	internal bool IsPending
	{
		get
		{
			return OVRTask<TResult>.Pending.Contains(this._id);
		}
	}

	internal void SetInternalData<T>(T data)
	{
		OVRTask<TResult>.InternalData<T>.Set(this._id, data);
	}

	internal OVRTask<TResult> WithInternalData<T>(T data)
	{
		if (!this.HasResult)
		{
			OVRTask<TResult>.InternalData<T>.Set(this._id, data);
		}
		return this;
	}

	internal bool TryGetInternalData<T>(out T data)
	{
		return OVRTask<TResult>.InternalData<T>.TryGet(this._id, out data);
	}

	internal void SetException(Exception exception)
	{
		OVRTask<TResult>.AwaitableSource awaitableSource;
		if (OVRTask<TResult>.AwaitableSources.Remove(this._id, out awaitableSource))
		{
			awaitableSource.SetException(exception);
			return;
		}
		OVRTask<TResult>.TaskSource taskSource;
		if (OVRTask<TResult>.Sources.Remove(this._id, out taskSource))
		{
			taskSource.SetException(exception);
			return;
		}
		if (this.TryRemoveInternalData())
		{
			OVRTask<TResult>.ContinueWithInvoker continueWithInvoker;
			if (OVRTask<TResult>.ContinueWithInvokers.Remove(this._id, out continueWithInvoker))
			{
				ExceptionDispatchInfo.Capture(exception).Throw();
			}
			OVRTask<TResult>.Exceptions.Add(this._id, exception);
			this.TryInvokeContinuation();
			return;
		}
		throw new InvalidOperationException(string.Format("The exception {0} cannot be set on task {1} because it is not a valid task.", exception, this._id), exception);
	}

	private bool TryRemoveInternalData()
	{
		if (!OVRTask<TResult>.Pending.Remove(this._id))
		{
			return false;
		}
		OVRTask<TResult>.InternalDataRemover internalDataRemover;
		if (OVRTask<TResult>.InternalDataRemovers.Remove(this._id, out internalDataRemover))
		{
			internalDataRemover(this._id);
		}
		Action<Guid> action;
		if (OVRTask<TResult>.IncrementalResultSubscriberRemovers.Remove(this._id, out action))
		{
			action(this._id);
		}
		return true;
	}

	private bool TryInvokeContinuation()
	{
		Action action;
		if (OVRTask<TResult>.Continuations.Remove(this._id, out action))
		{
			action();
			return true;
		}
		return false;
	}

	internal void SetResult(TResult result)
	{
		OVRTask<TResult>.AwaitableSource awaitableSource;
		if (OVRTask<TResult>.AwaitableSources.Remove(this._id, out awaitableSource))
		{
			awaitableSource.SetResultAndReturnToPool(result);
			return;
		}
		OVRTask<TResult>.TaskSource taskSource;
		if (OVRTask<TResult>.Sources.Remove(this._id, out taskSource))
		{
			taskSource.SetResult(result);
			return;
		}
		if (this.TryRemoveInternalData())
		{
			OVRTask<TResult>.ContinueWithInvoker continueWithInvoker;
			if (OVRTask<TResult>.ContinueWithInvokers.Remove(this._id, out continueWithInvoker))
			{
				continueWithInvoker(this._id, result);
				return;
			}
			OVRTask<TResult>.Results.Add(this._id, result);
			this.TryInvokeContinuation();
		}
	}

	internal void SetIncrementalResultCallback<TIncrementalResult>(Action<TIncrementalResult> onIncrementalResultAvailable)
	{
		if (onIncrementalResultAvailable == null)
		{
			throw new ArgumentNullException("onIncrementalResultAvailable");
		}
		OVRTask<TResult>.IncrementalResultSubscriber<TIncrementalResult>.Set(this._id, onIncrementalResultAvailable);
	}

	internal void NotifyIncrementalResult<TIncrementalResult>(TIncrementalResult incrementalResult)
	{
		OVRTask<TResult>.IncrementalResultSubscriber<TIncrementalResult>.Notify(this._id, incrementalResult);
	}

	internal static OVRTask<List<TResult>> WhenAll(IEnumerable<OVRTask<TResult>> tasks, List<TResult> results)
	{
		if (tasks == null)
		{
			throw new ArgumentNullException("tasks");
		}
		if (results == null)
		{
			throw new ArgumentNullException("results");
		}
		return new OVRTask<TResult>.CombinedTaskData(tasks, results).Task;
	}

	internal static OVRTask<TResult[]> WhenAll(IEnumerable<OVRTask<TResult>> tasks)
	{
		if (tasks == null)
		{
			throw new ArgumentNullException("tasks");
		}
		OVRTask<TResult[]> ovrtask = OVRTask.FromGuid<TResult[]>(Guid.NewGuid());
		List<TResult> results = OVRObjectPool.List<TResult>();
		OVRTask<TResult>.WhenAll(tasks, results).ContinueWith<OVRTask<TResult[]>>(OVRTask<TResult>._onCombinedTaskCompleted, ovrtask);
		return ovrtask;
	}

	public bool IsCompleted
	{
		get
		{
			return !this.IsPending;
		}
	}

	public bool IsFaulted
	{
		get
		{
			return OVRTask<TResult>.Exceptions.ContainsKey(this._id);
		}
	}

	public Exception GetException()
	{
		Exception result;
		if (!OVRTask<TResult>.Exceptions.Remove(this._id, out result))
		{
			throw new InvalidOperationException(string.Format("Task {0} is not in a faulted state. Check with {1}", this._id, "IsFaulted"));
		}
		return result;
	}

	public TResult GetResult()
	{
		Exception source;
		if (OVRTask<TResult>.Exceptions.Remove(this._id, out source))
		{
			ExceptionDispatchInfo.Capture(source).Throw();
		}
		TResult result;
		if (!this.TryGetResult(out result))
		{
			throw new InvalidOperationException(string.Format("Task {0} doesn't have any available result.", this._id));
		}
		return result;
	}

	public bool HasResult
	{
		get
		{
			return OVRTask<TResult>.Results.ContainsKey(this._id);
		}
	}

	public bool TryGetResult(out TResult result)
	{
		return OVRTask<TResult>.Results.Remove(this._id, out result);
	}

	public ValueTask<TResult> ToValueTask()
	{
		TResult result;
		bool flag = OVRTask<TResult>.Results.TryGetValue(this._id, out result);
		if (!OVRTask<TResult>.Pending.Contains(this._id) && !flag)
		{
			throw new InvalidOperationException(string.Format("Task {0} is not a valid task.", this._id));
		}
		if (OVRTask<TResult>.Continuations.ContainsKey(this._id))
		{
			throw new InvalidOperationException(string.Format("Task {0} is already being used by an await call.", this._id));
		}
		if (OVRTask<TResult>.ContinueWithInvokers.ContainsKey(this._id))
		{
			throw new InvalidOperationException(string.Format("Task {0} is already being used with ContinueWith.", this._id));
		}
		ValueTask<TResult> result2;
		using (this)
		{
			if (flag)
			{
				OVRTask<TResult>.Results.Remove(this._id);
				result2 = new ValueTask<TResult>(result);
			}
			else
			{
				OVRTask<TResult>.TaskSource taskSource = OVRObjectPool.Get<OVRTask<TResult>.TaskSource>();
				OVRTask<TResult>.Sources.Add(this._id, taskSource);
				result2 = taskSource.Task;
			}
		}
		return result2;
	}

	public Awaitable<TResult> ToAwaitable()
	{
		TResult tresult;
		bool flag = OVRTask<TResult>.Results.TryGetValue(this._id, out tresult);
		if (!OVRTask<TResult>.Pending.Contains(this._id) && !flag)
		{
			throw new InvalidOperationException(string.Format("Task {0} is not a valid task.", this._id));
		}
		if (OVRTask<TResult>.Continuations.ContainsKey(this._id))
		{
			throw new InvalidOperationException(string.Format("Task {0} is already being used by an await call.", this._id));
		}
		if (OVRTask<TResult>.ContinueWithInvokers.ContainsKey(this._id))
		{
			throw new InvalidOperationException(string.Format("Task {0} is already being used with ContinueWith.", this._id));
		}
		Awaitable<TResult> awaitable;
		using (this)
		{
			OVRTask<TResult>.AwaitableSource awaitableSource = OVRObjectPool.Get<OVRTask<TResult>.AwaitableSource>();
			if (flag)
			{
				awaitableSource.SetResult(tresult);
			}
			else
			{
				OVRTask<TResult>.AwaitableSources.Add(this._id, awaitableSource);
			}
			awaitable = awaitableSource.Awaitable;
		}
		return awaitable;
	}

	public OVRTask<TResult>.Awaiter GetAwaiter()
	{
		return new OVRTask<TResult>.Awaiter(this);
	}

	private void WithContinuation(Action continuation)
	{
		this.ValidateDelegateAndThrow(continuation, "continuation");
		OVRTask<TResult>.Continuations[this._id] = continuation;
	}

	public void ContinueWith(Action<TResult> onCompleted)
	{
		this.ValidateDelegateAndThrow(onCompleted, "onCompleted");
		if (this.IsCompleted)
		{
			onCompleted(this.GetResult());
			return;
		}
		OVRTask<TResult>.Callback.Add(this._id, onCompleted);
	}

	public void ContinueWith<T>(Action<TResult, T> onCompleted, T state)
	{
		this.ValidateDelegateAndThrow(onCompleted, "onCompleted");
		if (this.IsCompleted)
		{
			onCompleted(this.GetResult(), state);
			return;
		}
		OVRTask<TResult>.CallbackWithState<T>.Add(this._id, state, onCompleted);
	}

	private void ValidateDelegateAndThrow(object @delegate, string paramName)
	{
		if (@delegate == null)
		{
			throw new ArgumentNullException(paramName);
		}
		if (OVRTask<TResult>.Continuations.ContainsKey(this._id))
		{
			throw new InvalidOperationException(string.Format("Task {0} is already being used by an await call.", this._id));
		}
		if (OVRTask<TResult>.ContinueWithInvokers.ContainsKey(this._id))
		{
			throw new InvalidOperationException(string.Format("Task {0} is already being used with ContinueWith.", this._id));
		}
		if (OVRTask<TResult>.Sources.ContainsKey(this._id))
		{
			throw new InvalidOperationException(string.Format("Task {0} is already being used as a ValueTask.", this._id));
		}
		if (OVRTask<TResult>.AwaitableSources.ContainsKey(this._id))
		{
			throw new InvalidOperationException(string.Format("Task {0} is already being used as an Awaitable.", this._id));
		}
	}

	public void Dispose()
	{
		OVRTask<TResult>.Results.Remove(this._id);
		OVRTask<TResult>.Continuations.Remove(this._id);
		OVRTask<TResult>.Pending.Remove(this._id);
		OVRTask<TResult>.ContinueWithInvokers.Remove(this._id);
		OVRTask<TResult>.ContinueWithRemover continueWithRemover;
		if (OVRTask<TResult>.ContinueWithRemovers.TryGetValue(this._id, out continueWithRemover))
		{
			OVRTask<TResult>.ContinueWithRemovers.Remove(this._id);
			continueWithRemover(this._id);
		}
		OVRTask<TResult>.InternalDataRemover internalDataRemover;
		if (OVRTask<TResult>.InternalDataRemovers.TryGetValue(this._id, out internalDataRemover))
		{
			OVRTask<TResult>.InternalDataRemovers.Remove(this._id);
			internalDataRemover(this._id);
		}
		Action<Guid> action;
		if (OVRTask<TResult>.IncrementalResultSubscriberRemovers.TryGetValue(this._id, out action))
		{
			OVRTask<TResult>.IncrementalResultSubscriberRemovers.Remove(this._id);
			action(this._id);
		}
	}

	public bool Equals(OVRTask<TResult> other)
	{
		return this._id == other._id;
	}

	public override bool Equals(object obj)
	{
		if (obj is OVRTask<TResult>)
		{
			OVRTask<TResult> other = (OVRTask<TResult>)obj;
			return this.Equals(other);
		}
		return false;
	}

	public static bool operator ==(OVRTask<TResult> lhs, OVRTask<TResult> rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(OVRTask<TResult> lhs, OVRTask<TResult> rhs)
	{
		return !lhs.Equals(rhs);
	}

	public override int GetHashCode()
	{
		return this._id.GetHashCode();
	}

	public override string ToString()
	{
		return this._id.ToString();
	}

	private static readonly HashSet<Guid> Pending = new HashSet<Guid>();

	private static readonly Dictionary<Guid, TResult> Results = new Dictionary<Guid, TResult>();

	private static readonly Dictionary<Guid, Exception> Exceptions = new Dictionary<Guid, Exception>();

	private static readonly Dictionary<Guid, OVRTask<TResult>.TaskSource> Sources = new Dictionary<Guid, OVRTask<TResult>.TaskSource>();

	private static readonly Dictionary<Guid, OVRTask<TResult>.AwaitableSource> AwaitableSources = new Dictionary<Guid, OVRTask<TResult>.AwaitableSource>();

	private static readonly Dictionary<Guid, Action> Continuations = new Dictionary<Guid, Action>();

	private static readonly Dictionary<Guid, OVRTask<TResult>.ContinueWithInvoker> ContinueWithInvokers = new Dictionary<Guid, OVRTask<TResult>.ContinueWithInvoker>();

	private static readonly Dictionary<Guid, OVRTask<TResult>.ContinueWithRemover> ContinueWithRemovers = new Dictionary<Guid, OVRTask<TResult>.ContinueWithRemover>();

	private static readonly HashSet<Action> ContinueWithClearers = new HashSet<Action>();

	private static readonly Dictionary<Guid, OVRTask<TResult>.InternalDataRemover> InternalDataRemovers = new Dictionary<Guid, OVRTask<TResult>.InternalDataRemover>();

	private static readonly HashSet<Action> InternalDataClearers = new HashSet<Action>();

	private static readonly Dictionary<Guid, Action<Guid>> IncrementalResultSubscriberRemovers = new Dictionary<Guid, Action<Guid>>();

	private static readonly HashSet<Action> IncrementalResultSubscriberClearers = new HashSet<Action>();

	internal static readonly Action Clear = delegate()
	{
		OVRTask<TResult>.Results.Clear();
		OVRTask<TResult>.Continuations.Clear();
		OVRTask<TResult>.Pending.Clear();
		OVRTask<TResult>.Exceptions.Clear();
		OVRTask<TResult>.ContinueWithInvokers.Clear();
		foreach (Action action in OVRTask<TResult>.ContinueWithClearers)
		{
			action();
		}
		OVRTask<TResult>.ContinueWithClearers.Clear();
		OVRTask<TResult>.ContinueWithRemovers.Clear();
		foreach (Action action2 in OVRTask<TResult>.InternalDataClearers)
		{
			action2();
		}
		OVRTask<TResult>.InternalDataClearers.Clear();
		OVRTask<TResult>.InternalDataRemovers.Clear();
		foreach (Action action3 in OVRTask<TResult>.IncrementalResultSubscriberClearers)
		{
			action3();
		}
		OVRTask<TResult>.IncrementalResultSubscriberClearers.Clear();
		OVRTask<TResult>.IncrementalResultSubscriberRemovers.Clear();
		foreach (OVRTask<TResult>.TaskSource obj in OVRTask<TResult>.Sources.Values)
		{
			OVRObjectPool.Return<OVRTask<TResult>.TaskSource>(obj);
		}
		OVRTask<TResult>.Sources.Clear();
		foreach (OVRTask<TResult>.AwaitableSource obj2 in OVRTask<TResult>.AwaitableSources.Values)
		{
			OVRObjectPool.Return<OVRTask<TResult>.AwaitableSource>(obj2);
		}
		OVRTask<TResult>.AwaitableSources.Clear();
	};

	internal readonly Guid _id;

	private static readonly Action<List<TResult>, OVRTask<TResult[]>> _onCombinedTaskCompleted = delegate(List<TResult> resultsFromPool, OVRTask<TResult[]> task)
	{
		TResult[] result = resultsFromPool.ToArray();
		OVRObjectPool.Return<List<TResult>>(resultsFromPool);
		task.SetResult(result);
	};

	private delegate void ContinueWithInvoker(Guid guid, TResult result);

	private delegate bool ContinueWithRemover(Guid guid);

	private delegate bool InternalDataRemover(Guid guid);

	private static class InternalData<T>
	{
		public static bool TryGet(Guid taskId, out T data)
		{
			return OVRTask<TResult>.InternalData<T>.Data.TryGetValue(taskId, out data);
		}

		public static void Set(Guid taskId, T data)
		{
			OVRTask<TResult>.InternalData<T>.Data[taskId] = data;
			OVRTask<TResult>.InternalDataRemovers.Add(taskId, OVRTask<TResult>.InternalData<T>.Remover);
			OVRTask<TResult>.InternalDataClearers.Add(OVRTask<TResult>.InternalData<T>.Clearer);
		}

		private static bool Remove(Guid taskId)
		{
			return OVRTask<TResult>.InternalData<T>.Data.Remove(taskId);
		}

		private static void Clear()
		{
			OVRTask<TResult>.InternalData<T>.Data.Clear();
		}

		private static readonly Dictionary<Guid, T> Data = new Dictionary<Guid, T>();

		private static readonly OVRTask<TResult>.InternalDataRemover Remover = new OVRTask<TResult>.InternalDataRemover(OVRTask<TResult>.InternalData<T>.Remove);

		private static readonly Action Clearer = new Action(OVRTask<TResult>.InternalData<T>.Clear);
	}

	private static class IncrementalResultSubscriber<T>
	{
		public static void Set(Guid taskId, Action<T> subscriber)
		{
			OVRTask<TResult>.IncrementalResultSubscriber<T>.Subscribers[taskId] = subscriber;
			OVRTask<TResult>.IncrementalResultSubscriberRemovers[taskId] = OVRTask<TResult>.IncrementalResultSubscriber<T>.Remover;
			OVRTask<TResult>.IncrementalResultSubscriberClearers.Add(OVRTask<TResult>.IncrementalResultSubscriber<T>.Clearer);
		}

		public static void Notify(Guid taskId, T result)
		{
			Action<T> action;
			if (OVRTask<TResult>.IncrementalResultSubscriber<T>.Subscribers.TryGetValue(taskId, out action))
			{
				action(result);
			}
		}

		private static void Remove(Guid id)
		{
			OVRTask<TResult>.IncrementalResultSubscriber<T>.Subscribers.Remove(id);
		}

		private static void Clear()
		{
			OVRTask<TResult>.IncrementalResultSubscriber<T>.Subscribers.Clear();
		}

		private static readonly Dictionary<Guid, Action<T>> Subscribers = new Dictionary<Guid, Action<T>>();

		private static readonly Action<Guid> Remover = new Action<Guid>(OVRTask<TResult>.IncrementalResultSubscriber<T>.Remove);

		private static readonly Action Clearer = new Action(OVRTask<TResult>.IncrementalResultSubscriber<T>.Clear);
	}

	private readonly struct CombinedTaskData : IDisposable
	{
		private void OnSingleTaskCompleted(Guid taskId, TResult result)
		{
			this._completedTasks.Add(taskId, result);
			this._remainingTaskIds.Remove(taskId);
			if (this._remainingTaskIds.Count == 0)
			{
				using (this)
				{
					this._userOwnedResultList.Clear();
					foreach (Guid key in this._originalTaskOrder)
					{
						this._userOwnedResultList.Add(this._completedTasks[key]);
					}
					this.Task.SetResult(this._userOwnedResultList);
				}
			}
		}

		public CombinedTaskData(IEnumerable<OVRTask<TResult>> tasks, List<TResult> userOwnedResultList)
		{
			this.Task = OVRTask.FromGuid<List<TResult>>(Guid.NewGuid());
			this._remainingTaskIds = OVRObjectPool.HashSet<Guid>();
			this._originalTaskOrder = OVRObjectPool.List<Guid>();
			this._completedTasks = OVRObjectPool.Dictionary<Guid, TResult>();
			this._userOwnedResultList = userOwnedResultList;
			this._userOwnedResultList.Clear();
			List<OVRTask<TResult>> list;
			using (new OVRObjectPool.ListScope<OVRTask<TResult>>(ref list))
			{
				foreach (OVRTask<TResult> ovrtask in tasks.ToNonAlloc<OVRTask<TResult>>())
				{
					list.Add(ovrtask);
					this._remainingTaskIds.Add(ovrtask._id);
					this._originalTaskOrder.Add(ovrtask._id);
				}
				if (list.Count == 0)
				{
					this.Task.SetResult(this._userOwnedResultList);
				}
				else
				{
					foreach (OVRTask<TResult> ovrtask2 in list)
					{
						ovrtask2.ContinueWith<OVRTask<TResult>.CombinedTaskDataWithCompletedTaskId>(OVRTask<TResult>.CombinedTaskData._onSingleTaskCompleted, new OVRTask<TResult>.CombinedTaskDataWithCompletedTaskId
						{
							CompletedTaskId = ovrtask2._id,
							CombinedData = this
						});
					}
				}
			}
		}

		public void Dispose()
		{
			OVRObjectPool.Return<Guid>(this._remainingTaskIds);
			OVRObjectPool.Return<List<Guid>>(this._originalTaskOrder);
			OVRObjectPool.Return<Dictionary<Guid, TResult>>(this._completedTasks);
		}

		public readonly OVRTask<List<TResult>> Task;

		private readonly HashSet<Guid> _remainingTaskIds;

		private readonly List<Guid> _originalTaskOrder;

		private readonly Dictionary<Guid, TResult> _completedTasks;

		private readonly List<TResult> _userOwnedResultList;

		private static readonly Action<TResult, OVRTask<TResult>.CombinedTaskDataWithCompletedTaskId> _onSingleTaskCompleted = delegate(TResult result, OVRTask<TResult>.CombinedTaskDataWithCompletedTaskId data)
		{
			data.CombinedData.OnSingleTaskCompleted(data.CompletedTaskId, result);
		};
	}

	private struct CombinedTaskDataWithCompletedTaskId
	{
		public Guid CompletedTaskId;

		public OVRTask<TResult>.CombinedTaskData CombinedData;
	}

	private class TaskSource : IValueTaskSource<TResult>, OVRObjectPool.IPoolObject
	{
		public ValueTask<TResult> Task { get; private set; }

		public TResult GetResult(short token)
		{
			TResult result;
			try
			{
				result = this._manualSource.GetResult(token);
			}
			finally
			{
				OVRObjectPool.Return<OVRTask<TResult>.TaskSource>(this);
			}
			return result;
		}

		public ValueTaskSourceStatus GetStatus(short token)
		{
			return this._manualSource.GetStatus(token);
		}

		public void OnCompleted(Action<object> continuation, object state, short token, ValueTaskSourceOnCompletedFlags flags)
		{
			this._manualSource.OnCompleted(continuation, state, token, flags);
		}

		void OVRObjectPool.IPoolObject.OnGet()
		{
			this._manualSource.Reset();
			this.Task = new ValueTask<TResult>(this, this._manualSource.Version);
		}

		void OVRObjectPool.IPoolObject.OnReturn()
		{
		}

		public void SetResult(TResult result)
		{
			this._manualSource.SetResult(result);
		}

		public void SetException(Exception exception)
		{
			this._manualSource.SetException(exception);
		}

		private ManualResetValueTaskSourceCore<TResult> _manualSource;
	}

	private class AwaitableSource : AwaitableCompletionSource<TResult>, OVRObjectPool.IPoolObject
	{
		public void OnGet()
		{
			base.Reset();
		}

		public void OnReturn()
		{
		}

		public void SetResultAndReturnToPool(in TResult result)
		{
			try
			{
				base.SetResult(result);
			}
			finally
			{
				OVRObjectPool.Return<OVRTask<TResult>.AwaitableSource>(this);
			}
		}
	}

	public readonly struct Awaiter : INotifyCompletion
	{
		internal Awaiter(OVRTask<TResult> task)
		{
			this._task = task;
		}

		public bool IsCompleted
		{
			get
			{
				return this._task.IsCompleted;
			}
		}

		void INotifyCompletion.OnCompleted(Action continuation)
		{
			this._task.WithContinuation(continuation);
		}

		public TResult GetResult()
		{
			return this._task.GetResult();
		}

		private readonly OVRTask<TResult> _task;
	}

	private readonly struct Callback
	{
		private static void Invoke(Guid taskId, TResult result)
		{
			OVRTask<TResult>.Callback callback;
			if (OVRTask<TResult>.Callback.Callbacks.TryGetValue(taskId, out callback))
			{
				OVRTask<TResult>.Callback.Callbacks.Remove(taskId);
				callback.Invoke(result);
			}
		}

		private static bool Remove(Guid taskId)
		{
			return OVRTask<TResult>.Callback.Callbacks.Remove(taskId);
		}

		private static void Clear()
		{
			OVRTask<TResult>.Callback.Callbacks.Clear();
		}

		private void Invoke(TResult result)
		{
			this._delegate(result);
		}

		private Callback(Action<TResult> @delegate)
		{
			this._delegate = @delegate;
		}

		public static void Add(Guid taskId, Action<TResult> @delegate)
		{
			OVRTask<TResult>.Callback.Callbacks.Add(taskId, new OVRTask<TResult>.Callback(@delegate));
			OVRTask<TResult>.ContinueWithInvokers.Add(taskId, OVRTask<TResult>.Callback.Invoker);
			OVRTask<TResult>.ContinueWithRemovers.Add(taskId, OVRTask<TResult>.Callback.Remover);
			OVRTask<TResult>.ContinueWithClearers.Add(OVRTask<TResult>.Callback.Clearer);
		}

		private static readonly Dictionary<Guid, OVRTask<TResult>.Callback> Callbacks = new Dictionary<Guid, OVRTask<TResult>.Callback>();

		private readonly Action<TResult> _delegate;

		public static readonly OVRTask<TResult>.ContinueWithInvoker Invoker = new OVRTask<TResult>.ContinueWithInvoker(OVRTask<TResult>.Callback.Invoke);

		public static readonly OVRTask<TResult>.ContinueWithRemover Remover = new OVRTask<TResult>.ContinueWithRemover(OVRTask<TResult>.Callback.Remove);

		public static readonly Action Clearer = new Action(OVRTask<TResult>.Callback.Clear);
	}

	private readonly struct CallbackWithState<T>
	{
		private static void Invoke(Guid taskId, TResult result)
		{
			OVRTask<TResult>.CallbackWithState<T> callbackWithState;
			if (OVRTask<TResult>.CallbackWithState<T>.Callbacks.TryGetValue(taskId, out callbackWithState))
			{
				OVRTask<TResult>.CallbackWithState<T>.Callbacks.Remove(taskId);
				callbackWithState.Invoke(result);
			}
		}

		private CallbackWithState(T data, Action<TResult, T> @delegate)
		{
			this._data = data;
			this._delegate = @delegate;
		}

		private static void Clear()
		{
			OVRTask<TResult>.CallbackWithState<T>.Callbacks.Clear();
		}

		private static bool Remove(Guid taskId)
		{
			return OVRTask<TResult>.CallbackWithState<T>.Callbacks.Remove(taskId);
		}

		private void Invoke(TResult result)
		{
			this._delegate(result, this._data);
		}

		public static void Add(Guid taskId, T data, Action<TResult, T> callback)
		{
			OVRTask<TResult>.CallbackWithState<T>.Callbacks.Add(taskId, new OVRTask<TResult>.CallbackWithState<T>(data, callback));
			OVRTask<TResult>.ContinueWithInvokers.Add(taskId, OVRTask<TResult>.CallbackWithState<T>.Invoker);
			OVRTask<TResult>.ContinueWithRemovers.Add(taskId, OVRTask<TResult>.CallbackWithState<T>.Remover);
			OVRTask<TResult>.ContinueWithClearers.Add(OVRTask<TResult>.CallbackWithState<T>.Clearer);
		}

		private static readonly Dictionary<Guid, OVRTask<TResult>.CallbackWithState<T>> Callbacks = new Dictionary<Guid, OVRTask<TResult>.CallbackWithState<T>>();

		private readonly T _data;

		private readonly Action<TResult, T> _delegate;

		private static readonly OVRTask<TResult>.ContinueWithInvoker Invoker = new OVRTask<TResult>.ContinueWithInvoker(OVRTask<TResult>.CallbackWithState<T>.Invoke);

		private static readonly OVRTask<TResult>.ContinueWithRemover Remover = new OVRTask<TResult>.ContinueWithRemover(OVRTask<TResult>.CallbackWithState<T>.Remove);

		private static readonly Action Clearer = new Action(OVRTask<TResult>.CallbackWithState<T>.Clear);
	}
}
