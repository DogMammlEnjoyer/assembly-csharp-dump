using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.ResourceManagement.Exceptions;
using UnityEngine.ResourceManagement.Util;

namespace UnityEngine.ResourceManagement.AsyncOperations
{
	public abstract class AsyncOperationBase<TObject> : IAsyncOperation
	{
		protected abstract void Execute();

		protected virtual void Destroy()
		{
		}

		protected virtual float Progress
		{
			get
			{
				return 0f;
			}
		}

		protected virtual string DebugName
		{
			get
			{
				return this.ToString();
			}
		}

		public virtual void GetDependencies(List<AsyncOperationHandle> dependencies)
		{
		}

		public TObject Result { get; set; }

		internal int Version
		{
			get
			{
				return this.m_Version;
			}
		}

		internal bool CompletedEventHasListeners
		{
			get
			{
				return this.m_CompletedActionT != null && this.m_CompletedActionT.Count > 0;
			}
		}

		internal bool DestroyedEventHasListeners
		{
			get
			{
				return this.m_DestroyedAction != null && this.m_DestroyedAction.Count > 0;
			}
		}

		internal Action<IAsyncOperation> OnDestroy
		{
			set
			{
				this.m_OnDestroyAction = value;
			}
		}

		internal event Action Executed;

		protected internal int ReferenceCount
		{
			get
			{
				return this.m_referenceCount;
			}
		}

		public bool IsRunning { get; internal set; }

		protected AsyncOperationBase()
		{
			this.m_UpdateCallback = new Action<float>(this.UpdateCallback);
			this.m_dependencyCompleteAction = delegate(AsyncOperationHandle o)
			{
				this.InvokeExecute();
			};
		}

		internal static string ShortenPath(string p, bool keepExtension)
		{
			int num = p.LastIndexOf('/');
			if (num > 0)
			{
				p = p.Substring(num + 1);
			}
			if (!keepExtension)
			{
				num = p.LastIndexOf('.');
				if (num > 0)
				{
					p = p.Substring(0, num);
				}
			}
			return p;
		}

		public void WaitForCompletion()
		{
			if (PlatformUtilities.PlatformUsesMultiThreading(Application.platform))
			{
				while (!this.InvokeWaitForCompletion())
				{
				}
				return;
			}
			throw new Exception(string.Format("{0} does not support synchronous Addressable loading.  Please do not use WaitForCompletion on the {1} platform.", Application.platform, Application.platform));
		}

		protected virtual bool InvokeWaitForCompletion()
		{
			return true;
		}

		protected internal void IncrementReferenceCount()
		{
			if (this.m_referenceCount == 0)
			{
				throw new Exception(string.Format("Cannot increment reference count on operation {0} because it has already been destroyed", this));
			}
			this.m_referenceCount++;
		}

		protected internal void DecrementReferenceCount()
		{
			if (this.m_referenceCount <= 0)
			{
				throw new Exception(string.Format("Cannot decrement reference count for operation {0} because it is already 0", this));
			}
			this.m_referenceCount--;
			if (this.m_referenceCount == 0)
			{
				if (this.m_DestroyedAction != null)
				{
					this.m_DestroyedAction.Invoke(this.Handle);
					this.m_DestroyedAction.Clear();
				}
				this.Destroy();
				this.Result = default(TObject);
				this.m_referenceCount = 1;
				this.m_Status = AsyncOperationStatus.None;
				this.m_taskCompletionSource = null;
				this.m_taskCompletionSourceTypeless = null;
				this.m_Error = null;
				this.m_Version++;
				this.m_RM = null;
				if (this.m_OnDestroyAction != null)
				{
					this.m_OnDestroyAction(this);
					this.m_OnDestroyAction = null;
				}
			}
		}

		internal Task<TObject> Task
		{
			get
			{
				if (this.m_taskCompletionSource == null)
				{
					this.m_taskCompletionSource = new TaskCompletionSource<TObject>(TaskCreationOptions.RunContinuationsAsynchronously);
					if (this.IsDone && !this.CompletedEventHasListeners)
					{
						this.m_taskCompletionSource.SetResult(this.Result);
					}
				}
				return this.m_taskCompletionSource.Task;
			}
		}

		Task<object> IAsyncOperation.Task
		{
			get
			{
				if (this.m_taskCompletionSourceTypeless == null)
				{
					this.m_taskCompletionSourceTypeless = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
					if (this.IsDone && !this.CompletedEventHasListeners)
					{
						this.m_taskCompletionSourceTypeless.SetResult(this.Result);
					}
				}
				return this.m_taskCompletionSourceTypeless.Task;
			}
		}

		public override string ToString()
		{
			string str = "";
			Object @object = this.Result as Object;
			if (@object != null)
			{
				str = "(" + @object.GetInstanceID().ToString() + ")";
			}
			string format = "{0}, result='{1}', status='{2}'";
			object arg = base.ToString();
			Object object2 = @object;
			return string.Format(format, arg, ((object2 != null) ? object2.ToString() : null) + str, this.m_Status);
		}

		private void RegisterForDeferredCallbackEvent(bool incrementReferenceCount = true)
		{
			if (this.IsDone && !this.m_InDeferredCallbackQueue)
			{
				this.m_InDeferredCallbackQueue = true;
				ResourceManager rm = this.m_RM;
				if (rm == null)
				{
					return;
				}
				rm.RegisterForDeferredCallback(this, incrementReferenceCount);
			}
		}

		internal event Action<AsyncOperationHandle<TObject>> Completed
		{
			add
			{
				if (this.m_CompletedActionT == null)
				{
					this.m_CompletedActionT = DelegateList<AsyncOperationHandle<TObject>>.CreateWithGlobalCache();
				}
				this.m_CompletedActionT.Add(value);
				this.RegisterForDeferredCallbackEvent(true);
			}
			remove
			{
				DelegateList<AsyncOperationHandle<TObject>> completedActionT = this.m_CompletedActionT;
				if (completedActionT == null)
				{
					return;
				}
				completedActionT.Remove(value);
			}
		}

		internal event Action<AsyncOperationHandle> Destroyed
		{
			add
			{
				if (this.m_DestroyedAction == null)
				{
					this.m_DestroyedAction = DelegateList<AsyncOperationHandle>.CreateWithGlobalCache();
				}
				this.m_DestroyedAction.Add(value);
			}
			remove
			{
				DelegateList<AsyncOperationHandle> destroyedAction = this.m_DestroyedAction;
				if (destroyedAction == null)
				{
					return;
				}
				destroyedAction.Remove(value);
			}
		}

		internal event Action<AsyncOperationHandle> CompletedTypeless
		{
			add
			{
				this.Completed += delegate(AsyncOperationHandle<TObject> s)
				{
					value(s);
				};
			}
			remove
			{
				this.Completed -= delegate(AsyncOperationHandle<TObject> s)
				{
					value(s);
				};
			}
		}

		internal AsyncOperationStatus Status
		{
			get
			{
				return this.m_Status;
			}
		}

		internal Exception OperationException
		{
			get
			{
				return this.m_Error;
			}
			private set
			{
				this.m_Error = value;
				if (this.m_Error != null && ResourceManager.ExceptionHandler != null)
				{
					ResourceManager.ExceptionHandler(new AsyncOperationHandle(this), value);
				}
			}
		}

		internal bool MoveNext()
		{
			return !this.IsDone;
		}

		internal void Reset()
		{
		}

		internal object Current
		{
			get
			{
				return null;
			}
		}

		internal bool IsDone
		{
			get
			{
				return this.Status == AsyncOperationStatus.Failed || this.Status == AsyncOperationStatus.Succeeded;
			}
		}

		internal float PercentComplete
		{
			get
			{
				if (this.m_Status == AsyncOperationStatus.None)
				{
					try
					{
						return this.Progress;
					}
					catch
					{
						return 0f;
					}
				}
				return 1f;
			}
		}

		internal void InvokeCompletionEvent()
		{
			if (this.m_CompletedActionT != null)
			{
				this.m_CompletedActionT.Invoke(this.Handle);
				this.m_CompletedActionT.Clear();
			}
			if (this.m_taskCompletionSource != null)
			{
				this.m_taskCompletionSource.TrySetResult(this.Result);
			}
			if (this.m_taskCompletionSourceTypeless != null)
			{
				this.m_taskCompletionSourceTypeless.TrySetResult(this.Result);
			}
			this.m_InDeferredCallbackQueue = false;
		}

		internal AsyncOperationHandle<TObject> Handle
		{
			get
			{
				return new AsyncOperationHandle<TObject>(this);
			}
		}

		private void UpdateCallback(float unscaledDeltaTime)
		{
			(this as IUpdateReceiver).Update(unscaledDeltaTime);
		}

		public void Complete(TObject result, bool success, string errorMsg)
		{
			this.Complete(result, success, errorMsg, true);
		}

		public void Complete(TObject result, bool success, string errorMsg, bool releaseDependenciesOnFailure)
		{
			this.Complete(result, success, (!string.IsNullOrEmpty(errorMsg)) ? new OperationException(errorMsg, null) : null, releaseDependenciesOnFailure);
		}

		public void Complete(TObject result, bool success, Exception exception, bool releaseDependenciesOnFailure = true)
		{
			if (this.IsDone)
			{
				return;
			}
			IUpdateReceiver updateReceiver = this as IUpdateReceiver;
			if (this.m_UpdateCallbacks != null && updateReceiver != null)
			{
				this.m_UpdateCallbacks.Remove(this.m_UpdateCallback);
			}
			this.Result = result;
			this.m_Status = (success ? AsyncOperationStatus.Succeeded : AsyncOperationStatus.Failed);
			if (this.m_Status == AsyncOperationStatus.Failed || exception != null)
			{
				if (exception == null || string.IsNullOrEmpty(exception.Message))
				{
					this.OperationException = new OperationException("Unknown error in AsyncOperation : " + this.DebugName, null);
				}
				else
				{
					this.OperationException = exception;
				}
			}
			if (this.m_Status == AsyncOperationStatus.Failed)
			{
				if (releaseDependenciesOnFailure)
				{
					this.ReleaseDependencies();
				}
				ICachable cachable = this as ICachable;
				if (((cachable != null) ? cachable.Key : null) != null)
				{
					ResourceManager rm = this.m_RM;
					if (rm != null)
					{
						rm.RemoveOperationFromCache(cachable.Key);
					}
				}
				this.RegisterForDeferredCallbackEvent(false);
			}
			else
			{
				this.InvokeCompletionEvent();
				this.DecrementReferenceCount();
			}
			this.IsRunning = false;
		}

		internal void Start(ResourceManager rm, AsyncOperationHandle dependency, DelegateList<float> updateCallbacks)
		{
			this.m_RM = rm;
			this.IsRunning = true;
			this.HasExecuted = false;
			this.IncrementReferenceCount();
			this.m_UpdateCallbacks = updateCallbacks;
			if (dependency.IsValid() && !dependency.IsDone)
			{
				dependency.Completed += this.m_dependencyCompleteAction;
				return;
			}
			this.InvokeExecute();
		}

		internal void InvokeExecute()
		{
			this.Execute();
			this.HasExecuted = true;
			if (this is IUpdateReceiver && !this.IsDone)
			{
				this.m_UpdateCallbacks.Add(this.m_UpdateCallback);
			}
			Action executed = this.Executed;
			if (executed == null)
			{
				return;
			}
			executed();
		}

		event Action<AsyncOperationHandle> IAsyncOperation.CompletedTypeless
		{
			add
			{
				this.CompletedTypeless += value;
			}
			remove
			{
				this.CompletedTypeless -= value;
			}
		}

		event Action<AsyncOperationHandle> IAsyncOperation.Destroyed
		{
			add
			{
				this.Destroyed += value;
			}
			remove
			{
				this.Destroyed -= value;
			}
		}

		int IAsyncOperation.Version
		{
			get
			{
				return this.Version;
			}
		}

		int IAsyncOperation.ReferenceCount
		{
			get
			{
				return this.ReferenceCount;
			}
		}

		float IAsyncOperation.PercentComplete
		{
			get
			{
				return this.PercentComplete;
			}
		}

		AsyncOperationStatus IAsyncOperation.Status
		{
			get
			{
				return this.Status;
			}
		}

		Exception IAsyncOperation.OperationException
		{
			get
			{
				return this.OperationException;
			}
		}

		bool IAsyncOperation.IsDone
		{
			get
			{
				return this.IsDone;
			}
		}

		AsyncOperationHandle IAsyncOperation.Handle
		{
			get
			{
				return this.Handle;
			}
		}

		Action<IAsyncOperation> IAsyncOperation.OnDestroy
		{
			set
			{
				this.OnDestroy = value;
			}
		}

		string IAsyncOperation.DebugName
		{
			get
			{
				return this.DebugName;
			}
		}

		object IAsyncOperation.GetResultAsObject()
		{
			return this.Result;
		}

		Type IAsyncOperation.ResultType
		{
			get
			{
				return typeof(TObject);
			}
		}

		void IAsyncOperation.GetDependencies(List<AsyncOperationHandle> deps)
		{
			this.GetDependencies(deps);
		}

		void IAsyncOperation.DecrementReferenceCount()
		{
			this.DecrementReferenceCount();
		}

		void IAsyncOperation.IncrementReferenceCount()
		{
			this.IncrementReferenceCount();
		}

		void IAsyncOperation.InvokeCompletionEvent()
		{
			this.InvokeCompletionEvent();
		}

		void IAsyncOperation.Start(ResourceManager rm, AsyncOperationHandle dependency, DelegateList<float> updateCallbacks)
		{
			this.Start(rm, dependency, updateCallbacks);
		}

		internal virtual void ReleaseDependencies()
		{
		}

		DownloadStatus IAsyncOperation.GetDownloadStatus(HashSet<object> visited)
		{
			return this.GetDownloadStatus(visited);
		}

		internal virtual DownloadStatus GetDownloadStatus(HashSet<object> visited)
		{
			visited.Add(this);
			return new DownloadStatus
			{
				IsDone = this.IsDone
			};
		}

		private int m_referenceCount = 1;

		internal AsyncOperationStatus m_Status;

		internal Exception m_Error;

		internal ResourceManager m_RM;

		internal int m_Version;

		private DelegateList<AsyncOperationHandle> m_DestroyedAction;

		private DelegateList<AsyncOperationHandle<TObject>> m_CompletedActionT;

		private Action<IAsyncOperation> m_OnDestroyAction;

		private Action<AsyncOperationHandle> m_dependencyCompleteAction;

		protected internal bool HasExecuted;

		private TaskCompletionSource<TObject> m_taskCompletionSource;

		private TaskCompletionSource<object> m_taskCompletionSourceTypeless;

		private bool m_InDeferredCallbackQueue;

		private DelegateList<float> m_UpdateCallbacks;

		private Action<float> m_UpdateCallback;
	}
}
