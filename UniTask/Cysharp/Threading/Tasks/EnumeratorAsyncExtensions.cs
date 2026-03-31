using System;
using System.Collections;
using System.Reflection;
using System.Threading;
using Cysharp.Threading.Tasks.Internal;
using UnityEngine;

namespace Cysharp.Threading.Tasks
{
	public static class EnumeratorAsyncExtensions
	{
		public static UniTask.Awaiter GetAwaiter<T>(this T enumerator) where T : IEnumerator
		{
			T t = enumerator;
			Error.ThrowArgumentNullException<IEnumerator>(t, "enumerator");
			short token;
			return new UniTask(EnumeratorAsyncExtensions.EnumeratorPromise.Create(t, PlayerLoopTiming.Update, CancellationToken.None, out token), token).GetAwaiter();
		}

		public static UniTask WithCancellation(this IEnumerator enumerator, CancellationToken cancellationToken)
		{
			Error.ThrowArgumentNullException<IEnumerator>(enumerator, "enumerator");
			short token;
			return new UniTask(EnumeratorAsyncExtensions.EnumeratorPromise.Create(enumerator, PlayerLoopTiming.Update, cancellationToken, out token), token);
		}

		public static UniTask ToUniTask(this IEnumerator enumerator, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IEnumerator>(enumerator, "enumerator");
			short token;
			return new UniTask(EnumeratorAsyncExtensions.EnumeratorPromise.Create(enumerator, timing, cancellationToken, out token), token);
		}

		public static UniTask ToUniTask(this IEnumerator enumerator, MonoBehaviour coroutineRunner)
		{
			AutoResetUniTaskCompletionSource autoResetUniTaskCompletionSource = AutoResetUniTaskCompletionSource.Create();
			coroutineRunner.StartCoroutine(EnumeratorAsyncExtensions.Core(enumerator, coroutineRunner, autoResetUniTaskCompletionSource));
			return autoResetUniTaskCompletionSource.Task;
		}

		private static IEnumerator Core(IEnumerator inner, MonoBehaviour coroutineRunner, AutoResetUniTaskCompletionSource source)
		{
			yield return coroutineRunner.StartCoroutine(inner);
			source.TrySetResult();
			yield break;
		}

		private sealed class EnumeratorPromise : IUniTaskSource, IPlayerLoopItem, ITaskPoolNode<EnumeratorAsyncExtensions.EnumeratorPromise>
		{
			public ref EnumeratorAsyncExtensions.EnumeratorPromise NextNode
			{
				get
				{
					return ref this.nextNode;
				}
			}

			static EnumeratorPromise()
			{
				TaskPool.RegisterSizeGetter(typeof(EnumeratorAsyncExtensions.EnumeratorPromise), () => EnumeratorAsyncExtensions.EnumeratorPromise.pool.Size);
			}

			private EnumeratorPromise()
			{
			}

			public static IUniTaskSource Create(IEnumerator innerEnumerator, PlayerLoopTiming timing, CancellationToken cancellationToken, out short token)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return AutoResetUniTaskCompletionSource.CreateFromCanceled(cancellationToken, out token);
				}
				EnumeratorAsyncExtensions.EnumeratorPromise enumeratorPromise;
				if (!EnumeratorAsyncExtensions.EnumeratorPromise.pool.TryPop(out enumeratorPromise))
				{
					enumeratorPromise = new EnumeratorAsyncExtensions.EnumeratorPromise();
				}
				enumeratorPromise.innerEnumerator = EnumeratorAsyncExtensions.EnumeratorPromise.ConsumeEnumerator(innerEnumerator);
				enumeratorPromise.cancellationToken = cancellationToken;
				enumeratorPromise.loopRunning = true;
				enumeratorPromise.calledGetResult = false;
				enumeratorPromise.initialFrame = -1;
				token = enumeratorPromise.core.Version;
				if (enumeratorPromise.MoveNext())
				{
					PlayerLoopHelper.AddAction(timing, enumeratorPromise);
				}
				return enumeratorPromise;
			}

			public void GetResult(short token)
			{
				try
				{
					this.calledGetResult = true;
					this.core.GetResult(token);
				}
				finally
				{
					if (!this.loopRunning)
					{
						this.TryReturn();
					}
				}
			}

			public UniTaskStatus GetStatus(short token)
			{
				return this.core.GetStatus(token);
			}

			public UniTaskStatus UnsafeGetStatus()
			{
				return this.core.UnsafeGetStatus();
			}

			public void OnCompleted(Action<object> continuation, object state, short token)
			{
				this.core.OnCompleted(continuation, state, token);
			}

			public bool MoveNext()
			{
				if (this.calledGetResult)
				{
					this.loopRunning = false;
					this.TryReturn();
					return false;
				}
				if (this.innerEnumerator == null)
				{
					return false;
				}
				if (this.cancellationToken.IsCancellationRequested)
				{
					this.loopRunning = false;
					this.core.TrySetCanceled(this.cancellationToken);
					return false;
				}
				if (this.initialFrame == -1)
				{
					if (PlayerLoopHelper.IsMainThread)
					{
						this.initialFrame = Time.frameCount;
					}
				}
				else if (this.initialFrame == Time.frameCount)
				{
					return true;
				}
				try
				{
					if (this.innerEnumerator.MoveNext())
					{
						return true;
					}
				}
				catch (Exception error)
				{
					this.loopRunning = false;
					this.core.TrySetException(error);
					return false;
				}
				this.loopRunning = false;
				this.core.TrySetResult(null);
				return false;
			}

			private bool TryReturn()
			{
				this.core.Reset();
				this.innerEnumerator = null;
				this.cancellationToken = default(CancellationToken);
				return EnumeratorAsyncExtensions.EnumeratorPromise.pool.TryPush(this);
			}

			private static IEnumerator ConsumeEnumerator(IEnumerator enumerator)
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					if (obj == null)
					{
						yield return null;
					}
					else
					{
						CustomYieldInstruction cyi = obj as CustomYieldInstruction;
						if (cyi == null)
						{
							if (obj is YieldInstruction)
							{
								IEnumerator innerCoroutine = null;
								AsyncOperation asyncOperation = obj as AsyncOperation;
								if (asyncOperation == null)
								{
									WaitForSeconds waitForSeconds = obj as WaitForSeconds;
									if (waitForSeconds != null)
									{
										innerCoroutine = EnumeratorAsyncExtensions.EnumeratorPromise.UnwrapWaitForSeconds(waitForSeconds);
									}
								}
								else
								{
									innerCoroutine = EnumeratorAsyncExtensions.EnumeratorPromise.UnwrapWaitAsyncOperation(asyncOperation);
								}
								if (innerCoroutine != null)
								{
									while (innerCoroutine.MoveNext())
									{
										yield return null;
									}
									innerCoroutine = null;
									goto IL_159;
								}
							}
							else
							{
								IEnumerator enumerator2 = obj as IEnumerator;
								if (enumerator2 != null)
								{
									IEnumerator innerCoroutine = EnumeratorAsyncExtensions.EnumeratorPromise.ConsumeEnumerator(enumerator2);
									while (innerCoroutine.MoveNext())
									{
										yield return null;
									}
									innerCoroutine = null;
									goto IL_159;
								}
							}
							Debug.LogWarning("yield " + obj.GetType().Name + " is not supported on await IEnumerator or IEnumerator.ToUniTask(), please use ToUniTask(MonoBehaviour coroutineRunner) instead.");
							yield return null;
							continue;
						}
						while (cyi.keepWaiting)
						{
							yield return null;
						}
						IL_159:
						cyi = null;
					}
				}
				yield break;
			}

			private static IEnumerator UnwrapWaitForSeconds(WaitForSeconds waitForSeconds)
			{
				float second = (float)EnumeratorAsyncExtensions.EnumeratorPromise.waitForSeconds_Seconds.GetValue(waitForSeconds);
				float elapsed = 0f;
				do
				{
					yield return null;
					elapsed += Time.deltaTime;
				}
				while (elapsed < second);
				yield break;
			}

			private static IEnumerator UnwrapWaitAsyncOperation(AsyncOperation asyncOperation)
			{
				while (!asyncOperation.isDone)
				{
					yield return null;
				}
				yield break;
			}

			private static TaskPool<EnumeratorAsyncExtensions.EnumeratorPromise> pool;

			private EnumeratorAsyncExtensions.EnumeratorPromise nextNode;

			private IEnumerator innerEnumerator;

			private CancellationToken cancellationToken;

			private int initialFrame;

			private bool loopRunning;

			private bool calledGetResult;

			private UniTaskCompletionSourceCore<object> core;

			private static readonly FieldInfo waitForSeconds_Seconds = typeof(WaitForSeconds).GetField("m_Seconds", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
		}
	}
}
