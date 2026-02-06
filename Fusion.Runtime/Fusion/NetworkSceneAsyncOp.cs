using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Fusion
{
	public readonly struct NetworkSceneAsyncOp : IEnumerator
	{
		private NetworkSceneAsyncOp(SceneRef sceneRef, object data)
		{
			this.SceneRef = sceneRef;
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			this._data = data;
		}

		private NetworkSceneAsyncOp(SceneRef sceneRef)
		{
			this.SceneRef = sceneRef;
			this._data = null;
		}

		public bool IsValid
		{
			get
			{
				return this.SceneRef != default(SceneRef);
			}
		}

		public bool IsDone
		{
			get
			{
				object data = this._data;
				object obj = data;
				AsyncOperation asyncOperation = obj as AsyncOperation;
				bool result;
				if (asyncOperation == null)
				{
					ICoroutine coroutine = obj as ICoroutine;
					if (coroutine == null)
					{
						if (!(obj is ExceptionDispatchInfo))
						{
							Task task = obj as Task;
							result = (task == null || task.IsCompleted);
						}
						else
						{
							result = true;
						}
					}
					else
					{
						result = coroutine.IsDone;
					}
				}
				else
				{
					result = asyncOperation.isDone;
				}
				return result;
			}
		}

		public Exception Error
		{
			get
			{
				object data = this._data;
				object obj = data;
				ICoroutine coroutine = obj as ICoroutine;
				Exception result;
				if (coroutine == null)
				{
					Task task = obj as Task;
					if (task == null)
					{
						ExceptionDispatchInfo exceptionDispatchInfo = obj as ExceptionDispatchInfo;
						if (exceptionDispatchInfo == null)
						{
							result = null;
						}
						else
						{
							result = exceptionDispatchInfo.SourceException;
						}
					}
					else
					{
						result = task.Exception;
					}
				}
				else
				{
					ExceptionDispatchInfo error = coroutine.Error;
					result = ((error != null) ? error.SourceException : null);
				}
				return result;
			}
		}

		internal void ThrowIfError()
		{
			object data = this._data;
			object obj = data;
			ICoroutine coroutine = obj as ICoroutine;
			if (coroutine == null)
			{
				ExceptionDispatchInfo exceptionDispatchInfo = obj as ExceptionDispatchInfo;
				if (exceptionDispatchInfo == null)
				{
					Task task = obj as Task;
					if (task != null)
					{
						bool isFaulted = task.IsFaulted;
						if (isFaulted)
						{
							task.GetAwaiter().GetResult();
							Assert.AlwaysFail("Expected to have thrown");
						}
					}
				}
				else
				{
					exceptionDispatchInfo.Throw();
					Assert.AlwaysFail("Expected to have thrown");
				}
			}
			else
			{
				bool flag = coroutine.Error != null;
				if (flag)
				{
					coroutine.Error.Throw();
					Assert.AlwaysFail("Expected to have thrown");
				}
			}
		}

		public static NetworkSceneAsyncOp FromAsyncOperation(SceneRef sceneRef, AsyncOperation asyncOp)
		{
			bool flag = asyncOp == null;
			if (flag)
			{
				throw new ArgumentNullException("asyncOp");
			}
			return new NetworkSceneAsyncOp(sceneRef, asyncOp);
		}

		public static NetworkSceneAsyncOp FromCoroutine(SceneRef sceneRef, ICoroutine coroutine)
		{
			bool flag = coroutine == null;
			if (flag)
			{
				throw new ArgumentNullException("coroutine");
			}
			return new NetworkSceneAsyncOp(sceneRef, coroutine);
		}

		public static NetworkSceneAsyncOp FromTask(SceneRef sceneRef, Task task)
		{
			bool flag = task == null;
			if (flag)
			{
				throw new ArgumentNullException("task");
			}
			return new NetworkSceneAsyncOp(sceneRef, task);
		}

		public static NetworkSceneAsyncOp FromError(SceneRef sceneRef, Exception error)
		{
			bool flag = error == null;
			if (flag)
			{
				throw new ArgumentNullException("error");
			}
			return new NetworkSceneAsyncOp(sceneRef, ExceptionDispatchInfo.Capture(error));
		}

		public static NetworkSceneAsyncOp FromCompleted(SceneRef sceneRef)
		{
			return new NetworkSceneAsyncOp(sceneRef);
		}

		internal static NetworkSceneAsyncOp FromDeferred(SceneRef sceneRef, Task blockingTask, Func<SceneRef, NetworkSceneAsyncOp> op)
		{
			return NetworkSceneAsyncOp.FromTask(sceneRef, NetworkSceneAsyncOp.CreateDeferredOpTask(sceneRef, blockingTask, op));
		}

		[DebuggerStepThrough]
		private static Task CreateDeferredOpTask(SceneRef sceneRef, Task blockingTask, Func<SceneRef, NetworkSceneAsyncOp> op)
		{
			NetworkSceneAsyncOp.<CreateDeferredOpTask>d__17 <CreateDeferredOpTask>d__ = new NetworkSceneAsyncOp.<CreateDeferredOpTask>d__17();
			<CreateDeferredOpTask>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<CreateDeferredOpTask>d__.sceneRef = sceneRef;
			<CreateDeferredOpTask>d__.blockingTask = blockingTask;
			<CreateDeferredOpTask>d__.op = op;
			<CreateDeferredOpTask>d__.<>1__state = -1;
			<CreateDeferredOpTask>d__.<>t__builder.Start<NetworkSceneAsyncOp.<CreateDeferredOpTask>d__17>(ref <CreateDeferredOpTask>d__);
			return <CreateDeferredOpTask>d__.<>t__builder.Task;
		}

		public void AddOnCompleted(Action<NetworkSceneAsyncOp> action)
		{
			object data = this._data;
			object obj = data;
			AsyncOperation asyncOperation = obj as AsyncOperation;
			if (asyncOperation == null)
			{
				ICoroutine coroutine = obj as ICoroutine;
				if (coroutine == null)
				{
					Task task = obj as Task;
					if (task == null)
					{
						if (!(obj is ExceptionDispatchInfo))
						{
							action(this);
						}
						else
						{
							action(this);
						}
					}
					else
					{
						task.ContinueWith(delegate(Task _)
						{
							action(this);
						});
					}
				}
				else
				{
					coroutine.Completed += delegate(IAsyncOperation _)
					{
						action(this);
					};
				}
			}
			else
			{
				asyncOperation.completed += delegate(AsyncOperation _)
				{
					action(this);
				};
			}
		}

		public NetworkSceneAsyncOp.Awaiter GetAwaiter()
		{
			return new NetworkSceneAsyncOp.Awaiter(ref this);
		}

		bool IEnumerator.MoveNext()
		{
			return !this.IsDone;
		}

		void IEnumerator.Reset()
		{
		}

		object IEnumerator.Current
		{
			get
			{
				return null;
			}
		}

		public readonly SceneRef SceneRef;

		private readonly object _data;

		public struct Awaiter : INotifyCompletion
		{
			public Awaiter(in NetworkSceneAsyncOp op)
			{
				this._op = op;
			}

			public bool IsCompleted
			{
				get
				{
					return this._op.IsDone;
				}
			}

			public void GetResult()
			{
				bool flag = !this.IsCompleted;
				if (flag)
				{
					SpinWait spinWait = default(SpinWait);
					while (!this.IsCompleted)
					{
						spinWait.SpinOnce();
					}
				}
				this._op.ThrowIfError();
			}

			public void OnCompleted(Action continuation)
			{
				bool isCompleted = this.IsCompleted;
				if (isCompleted)
				{
					continuation();
				}
				else
				{
					SynchronizationContext capturedContext = SynchronizationContext.Current;
					SendOrPostCallback <>9__1;
					this._op.AddOnCompleted(delegate(NetworkSceneAsyncOp _)
					{
						SynchronizationContext capturedContext;
						bool flag = capturedContext != null;
						if (flag)
						{
							capturedContext = capturedContext;
							SendOrPostCallback d;
							if ((d = <>9__1) == null)
							{
								d = (<>9__1 = delegate(object __)
								{
									continuation();
								});
							}
							capturedContext.Post(d, null);
						}
						else
						{
							continuation();
						}
					});
				}
			}

			private NetworkSceneAsyncOp _op;
		}
	}
}
