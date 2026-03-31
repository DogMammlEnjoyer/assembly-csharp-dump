using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;
using Cysharp.Threading.Tasks.Internal;
using UnityEngine;
using UnityEngine.Events;

namespace Cysharp.Threading.Tasks
{
	[AsyncMethodBuilder(typeof(AsyncUniTaskMethodBuilder))]
	[StructLayout(LayoutKind.Auto)]
	public readonly struct UniTask
	{
		public static IEnumerator ToCoroutine(Func<UniTask> taskFactory)
		{
			return taskFactory().ToCoroutine(null);
		}

		[DebuggerHidden]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public UniTask(IUniTaskSource source, short token)
		{
			this.source = source;
			this.token = token;
		}

		public UniTaskStatus Status
		{
			[DebuggerHidden]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				if (this.source == null)
				{
					return UniTaskStatus.Succeeded;
				}
				return this.source.GetStatus(this.token);
			}
		}

		[DebuggerHidden]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public UniTask.Awaiter GetAwaiter()
		{
			return new UniTask.Awaiter(ref this);
		}

		public UniTask<bool> SuppressCancellationThrow()
		{
			UniTaskStatus status = this.Status;
			if (status == UniTaskStatus.Succeeded)
			{
				return CompletedTasks.False;
			}
			if (status == UniTaskStatus.Canceled)
			{
				return CompletedTasks.True;
			}
			return new UniTask<bool>(new UniTask.IsCanceledSource(this.source), this.token);
		}

		public override string ToString()
		{
			if (this.source == null)
			{
				return "()";
			}
			return "(" + this.source.UnsafeGetStatus().ToString() + ")";
		}

		public UniTask Preserve()
		{
			if (this.source == null)
			{
				return this;
			}
			return new UniTask(new UniTask.MemoizeSource(this.source), this.token);
		}

		public UniTask<AsyncUnit> AsAsyncUnitUniTask()
		{
			if (this.source == null)
			{
				return CompletedTasks.AsyncUnit;
			}
			if (this.source.GetStatus(this.token).IsCompletedSuccessfully())
			{
				this.source.GetResult(this.token);
				return CompletedTasks.AsyncUnit;
			}
			IUniTaskSource<AsyncUnit> uniTaskSource = this.source as IUniTaskSource<AsyncUnit>;
			if (uniTaskSource != null)
			{
				return new UniTask<AsyncUnit>(uniTaskSource, this.token);
			}
			return new UniTask<AsyncUnit>(new UniTask.AsyncUnitSource(this.source), this.token);
		}

		public static YieldAwaitable Yield()
		{
			return new YieldAwaitable(PlayerLoopTiming.Update);
		}

		public static YieldAwaitable Yield(PlayerLoopTiming timing)
		{
			return new YieldAwaitable(timing);
		}

		public static UniTask Yield(CancellationToken cancellationToken)
		{
			short num;
			return new UniTask(UniTask.YieldPromise.Create(PlayerLoopTiming.Update, cancellationToken, out num), num);
		}

		public static UniTask Yield(PlayerLoopTiming timing, CancellationToken cancellationToken)
		{
			short num;
			return new UniTask(UniTask.YieldPromise.Create(timing, cancellationToken, out num), num);
		}

		public static UniTask NextFrame()
		{
			short num;
			return new UniTask(UniTask.NextFramePromise.Create(PlayerLoopTiming.Update, CancellationToken.None, out num), num);
		}

		public static UniTask NextFrame(PlayerLoopTiming timing)
		{
			short num;
			return new UniTask(UniTask.NextFramePromise.Create(timing, CancellationToken.None, out num), num);
		}

		public static UniTask NextFrame(CancellationToken cancellationToken)
		{
			short num;
			return new UniTask(UniTask.NextFramePromise.Create(PlayerLoopTiming.Update, cancellationToken, out num), num);
		}

		public static UniTask NextFrame(PlayerLoopTiming timing, CancellationToken cancellationToken)
		{
			short num;
			return new UniTask(UniTask.NextFramePromise.Create(timing, cancellationToken, out num), num);
		}

		[Obsolete("Use WaitForEndOfFrame(MonoBehaviour) instead or UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate). Equivalent for coroutine's WaitForEndOfFrame requires MonoBehaviour(runner of Coroutine).")]
		public static YieldAwaitable WaitForEndOfFrame()
		{
			return UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
		}

		[Obsolete("Use WaitForEndOfFrame(MonoBehaviour) instead or UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate). Equivalent for coroutine's WaitForEndOfFrame requires MonoBehaviour(runner of Coroutine).")]
		public static UniTask WaitForEndOfFrame(CancellationToken cancellationToken)
		{
			return UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, cancellationToken);
		}

		public static UniTask WaitForEndOfFrame(MonoBehaviour coroutineRunner, CancellationToken cancellationToken = default(CancellationToken))
		{
			short num;
			return new UniTask(UniTask.WaitForEndOfFramePromise.Create(coroutineRunner, cancellationToken, out num), num);
		}

		public static YieldAwaitable WaitForFixedUpdate()
		{
			return UniTask.Yield(PlayerLoopTiming.LastFixedUpdate);
		}

		public static UniTask WaitForFixedUpdate(CancellationToken cancellationToken)
		{
			return UniTask.Yield(PlayerLoopTiming.LastFixedUpdate, cancellationToken);
		}

		public static UniTask DelayFrame(int delayFrameCount, PlayerLoopTiming delayTiming = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (delayFrameCount < 0)
			{
				throw new ArgumentOutOfRangeException("Delay does not allow minus delayFrameCount. delayFrameCount:" + delayFrameCount.ToString());
			}
			short num;
			return new UniTask(UniTask.DelayFramePromise.Create(delayFrameCount, delayTiming, cancellationToken, out num), num);
		}

		public static UniTask Delay(int millisecondsDelay, bool ignoreTimeScale = false, PlayerLoopTiming delayTiming = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken))
		{
			return UniTask.Delay(TimeSpan.FromMilliseconds((double)millisecondsDelay), ignoreTimeScale, delayTiming, cancellationToken);
		}

		public static UniTask Delay(TimeSpan delayTimeSpan, bool ignoreTimeScale = false, PlayerLoopTiming delayTiming = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken))
		{
			DelayType delayType = ignoreTimeScale ? DelayType.UnscaledDeltaTime : DelayType.DeltaTime;
			return UniTask.Delay(delayTimeSpan, delayType, delayTiming, cancellationToken);
		}

		public static UniTask Delay(int millisecondsDelay, DelayType delayType, PlayerLoopTiming delayTiming = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken))
		{
			return UniTask.Delay(TimeSpan.FromMilliseconds((double)millisecondsDelay), delayType, delayTiming, cancellationToken);
		}

		public static UniTask Delay(TimeSpan delayTimeSpan, DelayType delayType, PlayerLoopTiming delayTiming = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (delayTimeSpan < TimeSpan.Zero)
			{
				throw new ArgumentOutOfRangeException("Delay does not allow minus delayTimeSpan. delayTimeSpan:" + delayTimeSpan.ToString());
			}
			switch (delayType)
			{
			case DelayType.UnscaledDeltaTime:
			{
				short num;
				return new UniTask(UniTask.DelayIgnoreTimeScalePromise.Create(delayTimeSpan, delayTiming, cancellationToken, out num), num);
			}
			case DelayType.Realtime:
			{
				short num2;
				return new UniTask(UniTask.DelayRealtimePromise.Create(delayTimeSpan, delayTiming, cancellationToken, out num2), num2);
			}
			}
			short num3;
			return new UniTask(UniTask.DelayPromise.Create(delayTimeSpan, delayTiming, cancellationToken, out num3), num3);
		}

		public static UniTask FromException(Exception ex)
		{
			OperationCanceledException ex2 = ex as OperationCanceledException;
			if (ex2 != null)
			{
				return UniTask.FromCanceled(ex2.CancellationToken);
			}
			return new UniTask(new UniTask.ExceptionResultSource(ex), 0);
		}

		public static UniTask<T> FromException<T>(Exception ex)
		{
			OperationCanceledException ex2 = ex as OperationCanceledException;
			if (ex2 != null)
			{
				return UniTask.FromCanceled<T>(ex2.CancellationToken);
			}
			return new UniTask<T>(new UniTask.ExceptionResultSource<T>(ex), 0);
		}

		public static UniTask<T> FromResult<T>(T value)
		{
			return new UniTask<T>(value);
		}

		public static UniTask FromCanceled(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken == CancellationToken.None)
			{
				return UniTask.CanceledUniTask;
			}
			return new UniTask(new UniTask.CanceledResultSource(cancellationToken), 0);
		}

		public static UniTask<T> FromCanceled<T>(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken == CancellationToken.None)
			{
				return UniTask.CanceledUniTaskCache<T>.Task;
			}
			return new UniTask<T>(new UniTask.CanceledResultSource<T>(cancellationToken), 0);
		}

		public static UniTask Create(Func<UniTask> factory)
		{
			return factory();
		}

		public static UniTask<T> Create<T>(Func<UniTask<T>> factory)
		{
			return factory();
		}

		public static AsyncLazy Lazy(Func<UniTask> factory)
		{
			return new AsyncLazy(factory);
		}

		public static AsyncLazy<T> Lazy<T>(Func<UniTask<T>> factory)
		{
			return new AsyncLazy<T>(factory);
		}

		public static void Void(Func<UniTaskVoid> asyncAction)
		{
			asyncAction().Forget();
		}

		public static void Void(Func<CancellationToken, UniTaskVoid> asyncAction, CancellationToken cancellationToken)
		{
			asyncAction(cancellationToken).Forget();
		}

		public static void Void<T>(Func<T, UniTaskVoid> asyncAction, T state)
		{
			asyncAction(state).Forget();
		}

		public static Action Action(Func<UniTaskVoid> asyncAction)
		{
			return delegate()
			{
				asyncAction().Forget();
			};
		}

		public static Action Action(Func<CancellationToken, UniTaskVoid> asyncAction, CancellationToken cancellationToken)
		{
			return delegate()
			{
				asyncAction(cancellationToken).Forget();
			};
		}

		public static UnityAction UnityAction(Func<UniTaskVoid> asyncAction)
		{
			return delegate()
			{
				asyncAction().Forget();
			};
		}

		public static UnityAction UnityAction(Func<CancellationToken, UniTaskVoid> asyncAction, CancellationToken cancellationToken)
		{
			return delegate()
			{
				asyncAction(cancellationToken).Forget();
			};
		}

		public static UniTask Defer(Func<UniTask> factory)
		{
			return new UniTask(new UniTask.DeferPromise(factory), 0);
		}

		public static UniTask<T> Defer<T>(Func<UniTask<T>> factory)
		{
			return new UniTask<T>(new UniTask.DeferPromise<T>(factory), 0);
		}

		public static UniTask Never(CancellationToken cancellationToken)
		{
			return new UniTask<AsyncUnit>(new UniTask.NeverPromise<AsyncUnit>(cancellationToken), 0);
		}

		public static UniTask<T> Never<T>(CancellationToken cancellationToken)
		{
			return new UniTask<T>(new UniTask.NeverPromise<T>(cancellationToken), 0);
		}

		[Obsolete("UniTask.Run is similar as Task.Run, it uses ThreadPool. For equivalent behaviour, use UniTask.RunOnThreadPool instead. If you don't want to use ThreadPool, you can use UniTask.Void(async void) or UniTask.Create(async UniTask) too.")]
		public static UniTask Run(Action action, bool configureAwait = true, CancellationToken cancellationToken = default(CancellationToken))
		{
			return UniTask.RunOnThreadPool(action, configureAwait, cancellationToken);
		}

		[Obsolete("UniTask.Run is similar as Task.Run, it uses ThreadPool. For equivalent behaviour, use UniTask.RunOnThreadPool instead. If you don't want to use ThreadPool, you can use UniTask.Void(async void) or UniTask.Create(async UniTask) too.")]
		public static UniTask Run(Action<object> action, object state, bool configureAwait = true, CancellationToken cancellationToken = default(CancellationToken))
		{
			return UniTask.RunOnThreadPool(action, state, configureAwait, cancellationToken);
		}

		[Obsolete("UniTask.Run is similar as Task.Run, it uses ThreadPool. For equivalent behaviour, use UniTask.RunOnThreadPool instead. If you don't want to use ThreadPool, you can use UniTask.Void(async void) or UniTask.Create(async UniTask) too.")]
		public static UniTask Run(Func<UniTask> action, bool configureAwait = true, CancellationToken cancellationToken = default(CancellationToken))
		{
			return UniTask.RunOnThreadPool(action, configureAwait, cancellationToken);
		}

		[Obsolete("UniTask.Run is similar as Task.Run, it uses ThreadPool. For equivalent behaviour, use UniTask.RunOnThreadPool instead. If you don't want to use ThreadPool, you can use UniTask.Void(async void) or UniTask.Create(async UniTask) too.")]
		public static UniTask Run(Func<object, UniTask> action, object state, bool configureAwait = true, CancellationToken cancellationToken = default(CancellationToken))
		{
			return UniTask.RunOnThreadPool(action, state, configureAwait, cancellationToken);
		}

		[Obsolete("UniTask.Run is similar as Task.Run, it uses ThreadPool. For equivalent behaviour, use UniTask.RunOnThreadPool instead. If you don't want to use ThreadPool, you can use UniTask.Void(async void) or UniTask.Create(async UniTask) too.")]
		public static UniTask<T> Run<T>(Func<T> func, bool configureAwait = true, CancellationToken cancellationToken = default(CancellationToken))
		{
			return UniTask.RunOnThreadPool<T>(func, configureAwait, cancellationToken);
		}

		[Obsolete("UniTask.Run is similar as Task.Run, it uses ThreadPool. For equivalent behaviour, use UniTask.RunOnThreadPool instead. If you don't want to use ThreadPool, you can use UniTask.Void(async void) or UniTask.Create(async UniTask) too.")]
		public static UniTask<T> Run<T>(Func<UniTask<T>> func, bool configureAwait = true, CancellationToken cancellationToken = default(CancellationToken))
		{
			return UniTask.RunOnThreadPool<T>(func, configureAwait, cancellationToken);
		}

		[Obsolete("UniTask.Run is similar as Task.Run, it uses ThreadPool. For equivalent behaviour, use UniTask.RunOnThreadPool instead. If you don't want to use ThreadPool, you can use UniTask.Void(async void) or UniTask.Create(async UniTask) too.")]
		public static UniTask<T> Run<T>(Func<object, T> func, object state, bool configureAwait = true, CancellationToken cancellationToken = default(CancellationToken))
		{
			return UniTask.RunOnThreadPool<T>(func, state, configureAwait, cancellationToken);
		}

		[Obsolete("UniTask.Run is similar as Task.Run, it uses ThreadPool. For equivalent behaviour, use UniTask.RunOnThreadPool instead. If you don't want to use ThreadPool, you can use UniTask.Void(async void) or UniTask.Create(async UniTask) too.")]
		public static UniTask<T> Run<T>(Func<object, UniTask<T>> func, object state, bool configureAwait = true, CancellationToken cancellationToken = default(CancellationToken))
		{
			return UniTask.RunOnThreadPool<T>(func, state, configureAwait, cancellationToken);
		}

		public static UniTask RunOnThreadPool(Action action, bool configureAwait = true, CancellationToken cancellationToken = default(CancellationToken))
		{
			UniTask.<RunOnThreadPool>d__78 <RunOnThreadPool>d__;
			<RunOnThreadPool>d__.<>t__builder = AsyncUniTaskMethodBuilder.Create();
			<RunOnThreadPool>d__.action = action;
			<RunOnThreadPool>d__.configureAwait = configureAwait;
			<RunOnThreadPool>d__.cancellationToken = cancellationToken;
			<RunOnThreadPool>d__.<>1__state = -1;
			<RunOnThreadPool>d__.<>t__builder.Start<UniTask.<RunOnThreadPool>d__78>(ref <RunOnThreadPool>d__);
			return <RunOnThreadPool>d__.<>t__builder.Task;
		}

		public static UniTask RunOnThreadPool(Action<object> action, object state, bool configureAwait = true, CancellationToken cancellationToken = default(CancellationToken))
		{
			UniTask.<RunOnThreadPool>d__79 <RunOnThreadPool>d__;
			<RunOnThreadPool>d__.<>t__builder = AsyncUniTaskMethodBuilder.Create();
			<RunOnThreadPool>d__.action = action;
			<RunOnThreadPool>d__.state = state;
			<RunOnThreadPool>d__.configureAwait = configureAwait;
			<RunOnThreadPool>d__.cancellationToken = cancellationToken;
			<RunOnThreadPool>d__.<>1__state = -1;
			<RunOnThreadPool>d__.<>t__builder.Start<UniTask.<RunOnThreadPool>d__79>(ref <RunOnThreadPool>d__);
			return <RunOnThreadPool>d__.<>t__builder.Task;
		}

		public static UniTask RunOnThreadPool(Func<UniTask> action, bool configureAwait = true, CancellationToken cancellationToken = default(CancellationToken))
		{
			UniTask.<RunOnThreadPool>d__80 <RunOnThreadPool>d__;
			<RunOnThreadPool>d__.<>t__builder = AsyncUniTaskMethodBuilder.Create();
			<RunOnThreadPool>d__.action = action;
			<RunOnThreadPool>d__.configureAwait = configureAwait;
			<RunOnThreadPool>d__.cancellationToken = cancellationToken;
			<RunOnThreadPool>d__.<>1__state = -1;
			<RunOnThreadPool>d__.<>t__builder.Start<UniTask.<RunOnThreadPool>d__80>(ref <RunOnThreadPool>d__);
			return <RunOnThreadPool>d__.<>t__builder.Task;
		}

		public static UniTask RunOnThreadPool(Func<object, UniTask> action, object state, bool configureAwait = true, CancellationToken cancellationToken = default(CancellationToken))
		{
			UniTask.<RunOnThreadPool>d__81 <RunOnThreadPool>d__;
			<RunOnThreadPool>d__.<>t__builder = AsyncUniTaskMethodBuilder.Create();
			<RunOnThreadPool>d__.action = action;
			<RunOnThreadPool>d__.state = state;
			<RunOnThreadPool>d__.configureAwait = configureAwait;
			<RunOnThreadPool>d__.cancellationToken = cancellationToken;
			<RunOnThreadPool>d__.<>1__state = -1;
			<RunOnThreadPool>d__.<>t__builder.Start<UniTask.<RunOnThreadPool>d__81>(ref <RunOnThreadPool>d__);
			return <RunOnThreadPool>d__.<>t__builder.Task;
		}

		public static UniTask<T> RunOnThreadPool<T>(Func<T> func, bool configureAwait = true, CancellationToken cancellationToken = default(CancellationToken))
		{
			UniTask.<RunOnThreadPool>d__82<T> <RunOnThreadPool>d__;
			<RunOnThreadPool>d__.<>t__builder = AsyncUniTaskMethodBuilder<T>.Create();
			<RunOnThreadPool>d__.func = func;
			<RunOnThreadPool>d__.configureAwait = configureAwait;
			<RunOnThreadPool>d__.cancellationToken = cancellationToken;
			<RunOnThreadPool>d__.<>1__state = -1;
			<RunOnThreadPool>d__.<>t__builder.Start<UniTask.<RunOnThreadPool>d__82<T>>(ref <RunOnThreadPool>d__);
			return <RunOnThreadPool>d__.<>t__builder.Task;
		}

		public static UniTask<T> RunOnThreadPool<T>(Func<UniTask<T>> func, bool configureAwait = true, CancellationToken cancellationToken = default(CancellationToken))
		{
			UniTask.<RunOnThreadPool>d__83<T> <RunOnThreadPool>d__;
			<RunOnThreadPool>d__.<>t__builder = AsyncUniTaskMethodBuilder<T>.Create();
			<RunOnThreadPool>d__.func = func;
			<RunOnThreadPool>d__.configureAwait = configureAwait;
			<RunOnThreadPool>d__.cancellationToken = cancellationToken;
			<RunOnThreadPool>d__.<>1__state = -1;
			<RunOnThreadPool>d__.<>t__builder.Start<UniTask.<RunOnThreadPool>d__83<T>>(ref <RunOnThreadPool>d__);
			return <RunOnThreadPool>d__.<>t__builder.Task;
		}

		public static UniTask<T> RunOnThreadPool<T>(Func<object, T> func, object state, bool configureAwait = true, CancellationToken cancellationToken = default(CancellationToken))
		{
			UniTask.<RunOnThreadPool>d__84<T> <RunOnThreadPool>d__;
			<RunOnThreadPool>d__.<>t__builder = AsyncUniTaskMethodBuilder<T>.Create();
			<RunOnThreadPool>d__.func = func;
			<RunOnThreadPool>d__.state = state;
			<RunOnThreadPool>d__.configureAwait = configureAwait;
			<RunOnThreadPool>d__.cancellationToken = cancellationToken;
			<RunOnThreadPool>d__.<>1__state = -1;
			<RunOnThreadPool>d__.<>t__builder.Start<UniTask.<RunOnThreadPool>d__84<T>>(ref <RunOnThreadPool>d__);
			return <RunOnThreadPool>d__.<>t__builder.Task;
		}

		public static UniTask<T> RunOnThreadPool<T>(Func<object, UniTask<T>> func, object state, bool configureAwait = true, CancellationToken cancellationToken = default(CancellationToken))
		{
			UniTask.<RunOnThreadPool>d__85<T> <RunOnThreadPool>d__;
			<RunOnThreadPool>d__.<>t__builder = AsyncUniTaskMethodBuilder<T>.Create();
			<RunOnThreadPool>d__.func = func;
			<RunOnThreadPool>d__.state = state;
			<RunOnThreadPool>d__.configureAwait = configureAwait;
			<RunOnThreadPool>d__.cancellationToken = cancellationToken;
			<RunOnThreadPool>d__.<>1__state = -1;
			<RunOnThreadPool>d__.<>t__builder.Start<UniTask.<RunOnThreadPool>d__85<T>>(ref <RunOnThreadPool>d__);
			return <RunOnThreadPool>d__.<>t__builder.Task;
		}

		public static SwitchToMainThreadAwaitable SwitchToMainThread(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new SwitchToMainThreadAwaitable(PlayerLoopTiming.Update, cancellationToken);
		}

		public static SwitchToMainThreadAwaitable SwitchToMainThread(PlayerLoopTiming timing, CancellationToken cancellationToken = default(CancellationToken))
		{
			return new SwitchToMainThreadAwaitable(timing, cancellationToken);
		}

		public static ReturnToMainThread ReturnToMainThread(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new ReturnToMainThread(PlayerLoopTiming.Update, cancellationToken);
		}

		public static ReturnToMainThread ReturnToMainThread(PlayerLoopTiming timing, CancellationToken cancellationToken = default(CancellationToken))
		{
			return new ReturnToMainThread(timing, cancellationToken);
		}

		public static void Post(Action action, PlayerLoopTiming timing = PlayerLoopTiming.Update)
		{
			PlayerLoopHelper.AddContinuation(timing, action);
		}

		public static SwitchToThreadPoolAwaitable SwitchToThreadPool()
		{
			return default(SwitchToThreadPoolAwaitable);
		}

		public static SwitchToTaskPoolAwaitable SwitchToTaskPool()
		{
			return default(SwitchToTaskPoolAwaitable);
		}

		public static SwitchToSynchronizationContextAwaitable SwitchToSynchronizationContext(SynchronizationContext synchronizationContext, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<SynchronizationContext>(synchronizationContext, "synchronizationContext");
			return new SwitchToSynchronizationContextAwaitable(synchronizationContext, cancellationToken);
		}

		public static ReturnToSynchronizationContext ReturnToSynchronizationContext(SynchronizationContext synchronizationContext, CancellationToken cancellationToken = default(CancellationToken))
		{
			return new ReturnToSynchronizationContext(synchronizationContext, false, cancellationToken);
		}

		public static ReturnToSynchronizationContext ReturnToCurrentSynchronizationContext(bool dontPostWhenSameContext = true, CancellationToken cancellationToken = default(CancellationToken))
		{
			return new ReturnToSynchronizationContext(SynchronizationContext.Current, dontPostWhenSameContext, cancellationToken);
		}

		public static UniTask WaitUntil(Func<bool> predicate, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken))
		{
			short num;
			return new UniTask(UniTask.WaitUntilPromise.Create(predicate, timing, cancellationToken, out num), num);
		}

		public static UniTask WaitWhile(Func<bool> predicate, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken))
		{
			short num;
			return new UniTask(UniTask.WaitWhilePromise.Create(predicate, timing, cancellationToken, out num), num);
		}

		public static UniTask WaitUntilCanceled(CancellationToken cancellationToken, PlayerLoopTiming timing = PlayerLoopTiming.Update)
		{
			short num;
			return new UniTask(UniTask.WaitUntilCanceledPromise.Create(cancellationToken, timing, out num), num);
		}

		public static UniTask<U> WaitUntilValueChanged<T, U>(T target, Func<T, U> monitorFunction, PlayerLoopTiming monitorTiming = PlayerLoopTiming.Update, IEqualityComparer<U> equalityComparer = null, CancellationToken cancellationToken = default(CancellationToken)) where T : class
		{
			short num;
			return new UniTask<U>((target is Object) ? UniTask.WaitUntilValueChangedUnityObjectPromise<T, U>.Create(target, monitorFunction, equalityComparer, monitorTiming, cancellationToken, out num) : UniTask.WaitUntilValueChangedStandardObjectPromise<T, U>.Create(target, monitorFunction, equalityComparer, monitorTiming, cancellationToken, out num), num);
		}

		public static UniTask<T[]> WhenAll<T>(params UniTask<T>[] tasks)
		{
			if (tasks.Length == 0)
			{
				return UniTask.FromResult<T[]>(Array.Empty<T>());
			}
			return new UniTask<T[]>(new UniTask.WhenAllPromise<T>(tasks, tasks.Length), 0);
		}

		public static UniTask<T[]> WhenAll<T>(IEnumerable<UniTask<T>> tasks)
		{
			UniTask<T[]> result;
			using (ArrayPoolUtil.RentArray<UniTask<T>> rentArray = ArrayPoolUtil.Materialize<UniTask<T>>(tasks))
			{
				result = new UniTask<T[]>(new UniTask.WhenAllPromise<T>(rentArray.Array, rentArray.Length), 0);
			}
			return result;
		}

		public static UniTask WhenAll(params UniTask[] tasks)
		{
			if (tasks.Length == 0)
			{
				return UniTask.CompletedTask;
			}
			return new UniTask(new UniTask.WhenAllPromise(tasks, tasks.Length), 0);
		}

		public static UniTask WhenAll(IEnumerable<UniTask> tasks)
		{
			UniTask result;
			using (ArrayPoolUtil.RentArray<UniTask> rentArray = ArrayPoolUtil.Materialize<UniTask>(tasks))
			{
				result = new UniTask(new UniTask.WhenAllPromise(rentArray.Array, rentArray.Length), 0);
			}
			return result;
		}

		public static UniTask<ValueTuple<T1, T2>> WhenAll<T1, T2>(UniTask<T1> task1, UniTask<T2> task2)
		{
			if (task1.Status.IsCompletedSuccessfully() && task2.Status.IsCompletedSuccessfully())
			{
				return new UniTask<ValueTuple<T1, T2>>(new ValueTuple<T1, T2>(task1.GetAwaiter().GetResult(), task2.GetAwaiter().GetResult()));
			}
			return new UniTask<ValueTuple<T1, T2>>(new UniTask.WhenAllPromise<T1, T2>(task1, task2), 0);
		}

		public static UniTask<ValueTuple<T1, T2, T3>> WhenAll<T1, T2, T3>(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3)
		{
			if (task1.Status.IsCompletedSuccessfully() && task2.Status.IsCompletedSuccessfully() && task3.Status.IsCompletedSuccessfully())
			{
				return new UniTask<ValueTuple<T1, T2, T3>>(new ValueTuple<T1, T2, T3>(task1.GetAwaiter().GetResult(), task2.GetAwaiter().GetResult(), task3.GetAwaiter().GetResult()));
			}
			return new UniTask<ValueTuple<T1, T2, T3>>(new UniTask.WhenAllPromise<T1, T2, T3>(task1, task2, task3), 0);
		}

		public static UniTask<ValueTuple<T1, T2, T3, T4>> WhenAll<T1, T2, T3, T4>(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4)
		{
			if (task1.Status.IsCompletedSuccessfully() && task2.Status.IsCompletedSuccessfully() && task3.Status.IsCompletedSuccessfully() && task4.Status.IsCompletedSuccessfully())
			{
				return new UniTask<ValueTuple<T1, T2, T3, T4>>(new ValueTuple<T1, T2, T3, T4>(task1.GetAwaiter().GetResult(), task2.GetAwaiter().GetResult(), task3.GetAwaiter().GetResult(), task4.GetAwaiter().GetResult()));
			}
			return new UniTask<ValueTuple<T1, T2, T3, T4>>(new UniTask.WhenAllPromise<T1, T2, T3, T4>(task1, task2, task3, task4), 0);
		}

		public static UniTask<ValueTuple<T1, T2, T3, T4, T5>> WhenAll<T1, T2, T3, T4, T5>(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4, UniTask<T5> task5)
		{
			if (task1.Status.IsCompletedSuccessfully() && task2.Status.IsCompletedSuccessfully() && task3.Status.IsCompletedSuccessfully() && task4.Status.IsCompletedSuccessfully() && task5.Status.IsCompletedSuccessfully())
			{
				return new UniTask<ValueTuple<T1, T2, T3, T4, T5>>(new ValueTuple<T1, T2, T3, T4, T5>(task1.GetAwaiter().GetResult(), task2.GetAwaiter().GetResult(), task3.GetAwaiter().GetResult(), task4.GetAwaiter().GetResult(), task5.GetAwaiter().GetResult()));
			}
			return new UniTask<ValueTuple<T1, T2, T3, T4, T5>>(new UniTask.WhenAllPromise<T1, T2, T3, T4, T5>(task1, task2, task3, task4, task5), 0);
		}

		public static UniTask<ValueTuple<T1, T2, T3, T4, T5, T6>> WhenAll<T1, T2, T3, T4, T5, T6>(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4, UniTask<T5> task5, UniTask<T6> task6)
		{
			if (task1.Status.IsCompletedSuccessfully() && task2.Status.IsCompletedSuccessfully() && task3.Status.IsCompletedSuccessfully() && task4.Status.IsCompletedSuccessfully() && task5.Status.IsCompletedSuccessfully() && task6.Status.IsCompletedSuccessfully())
			{
				return new UniTask<ValueTuple<T1, T2, T3, T4, T5, T6>>(new ValueTuple<T1, T2, T3, T4, T5, T6>(task1.GetAwaiter().GetResult(), task2.GetAwaiter().GetResult(), task3.GetAwaiter().GetResult(), task4.GetAwaiter().GetResult(), task5.GetAwaiter().GetResult(), task6.GetAwaiter().GetResult()));
			}
			return new UniTask<ValueTuple<T1, T2, T3, T4, T5, T6>>(new UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6>(task1, task2, task3, task4, task5, task6), 0);
		}

		public static UniTask<ValueTuple<T1, T2, T3, T4, T5, T6, T7>> WhenAll<T1, T2, T3, T4, T5, T6, T7>(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4, UniTask<T5> task5, UniTask<T6> task6, UniTask<T7> task7)
		{
			if (task1.Status.IsCompletedSuccessfully() && task2.Status.IsCompletedSuccessfully() && task3.Status.IsCompletedSuccessfully() && task4.Status.IsCompletedSuccessfully() && task5.Status.IsCompletedSuccessfully() && task6.Status.IsCompletedSuccessfully() && task7.Status.IsCompletedSuccessfully())
			{
				return new UniTask<ValueTuple<T1, T2, T3, T4, T5, T6, T7>>(new ValueTuple<T1, T2, T3, T4, T5, T6, T7>(task1.GetAwaiter().GetResult(), task2.GetAwaiter().GetResult(), task3.GetAwaiter().GetResult(), task4.GetAwaiter().GetResult(), task5.GetAwaiter().GetResult(), task6.GetAwaiter().GetResult(), task7.GetAwaiter().GetResult()));
			}
			return new UniTask<ValueTuple<T1, T2, T3, T4, T5, T6, T7>>(new UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7>(task1, task2, task3, task4, task5, task6, task7), 0);
		}

		public static UniTask<ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8>>> WhenAll<T1, T2, T3, T4, T5, T6, T7, T8>(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4, UniTask<T5> task5, UniTask<T6> task6, UniTask<T7> task7, UniTask<T8> task8)
		{
			if (task1.Status.IsCompletedSuccessfully() && task2.Status.IsCompletedSuccessfully() && task3.Status.IsCompletedSuccessfully() && task4.Status.IsCompletedSuccessfully() && task5.Status.IsCompletedSuccessfully() && task6.Status.IsCompletedSuccessfully() && task7.Status.IsCompletedSuccessfully() && task8.Status.IsCompletedSuccessfully())
			{
				return new UniTask<ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8>>>(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8>>(task1.GetAwaiter().GetResult(), task2.GetAwaiter().GetResult(), task3.GetAwaiter().GetResult(), task4.GetAwaiter().GetResult(), task5.GetAwaiter().GetResult(), task6.GetAwaiter().GetResult(), task7.GetAwaiter().GetResult(), new ValueTuple<T8>(task8.GetAwaiter().GetResult())));
			}
			return new UniTask<ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8>>>(new UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8>(task1, task2, task3, task4, task5, task6, task7, task8), 0);
		}

		public static UniTask<ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9>>> WhenAll<T1, T2, T3, T4, T5, T6, T7, T8, T9>(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4, UniTask<T5> task5, UniTask<T6> task6, UniTask<T7> task7, UniTask<T8> task8, UniTask<T9> task9)
		{
			if (task1.Status.IsCompletedSuccessfully() && task2.Status.IsCompletedSuccessfully() && task3.Status.IsCompletedSuccessfully() && task4.Status.IsCompletedSuccessfully() && task5.Status.IsCompletedSuccessfully() && task6.Status.IsCompletedSuccessfully() && task7.Status.IsCompletedSuccessfully() && task8.Status.IsCompletedSuccessfully() && task9.Status.IsCompletedSuccessfully())
			{
				return new UniTask<ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9>>>(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9>>(task1.GetAwaiter().GetResult(), task2.GetAwaiter().GetResult(), task3.GetAwaiter().GetResult(), task4.GetAwaiter().GetResult(), task5.GetAwaiter().GetResult(), task6.GetAwaiter().GetResult(), task7.GetAwaiter().GetResult(), new ValueTuple<T8, T9>(task8.GetAwaiter().GetResult(), task9.GetAwaiter().GetResult())));
			}
			return new UniTask<ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9>>>(new UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>(task1, task2, task3, task4, task5, task6, task7, task8, task9), 0);
		}

		public static UniTask<ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10>>> WhenAll<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4, UniTask<T5> task5, UniTask<T6> task6, UniTask<T7> task7, UniTask<T8> task8, UniTask<T9> task9, UniTask<T10> task10)
		{
			if (task1.Status.IsCompletedSuccessfully() && task2.Status.IsCompletedSuccessfully() && task3.Status.IsCompletedSuccessfully() && task4.Status.IsCompletedSuccessfully() && task5.Status.IsCompletedSuccessfully() && task6.Status.IsCompletedSuccessfully() && task7.Status.IsCompletedSuccessfully() && task8.Status.IsCompletedSuccessfully() && task9.Status.IsCompletedSuccessfully() && task10.Status.IsCompletedSuccessfully())
			{
				return new UniTask<ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10>>>(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10>>(task1.GetAwaiter().GetResult(), task2.GetAwaiter().GetResult(), task3.GetAwaiter().GetResult(), task4.GetAwaiter().GetResult(), task5.GetAwaiter().GetResult(), task6.GetAwaiter().GetResult(), task7.GetAwaiter().GetResult(), new ValueTuple<T8, T9, T10>(task8.GetAwaiter().GetResult(), task9.GetAwaiter().GetResult(), task10.GetAwaiter().GetResult())));
			}
			return new UniTask<ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10>>>(new UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(task1, task2, task3, task4, task5, task6, task7, task8, task9, task10), 0);
		}

		public static UniTask<ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11>>> WhenAll<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4, UniTask<T5> task5, UniTask<T6> task6, UniTask<T7> task7, UniTask<T8> task8, UniTask<T9> task9, UniTask<T10> task10, UniTask<T11> task11)
		{
			if (task1.Status.IsCompletedSuccessfully() && task2.Status.IsCompletedSuccessfully() && task3.Status.IsCompletedSuccessfully() && task4.Status.IsCompletedSuccessfully() && task5.Status.IsCompletedSuccessfully() && task6.Status.IsCompletedSuccessfully() && task7.Status.IsCompletedSuccessfully() && task8.Status.IsCompletedSuccessfully() && task9.Status.IsCompletedSuccessfully() && task10.Status.IsCompletedSuccessfully() && task11.Status.IsCompletedSuccessfully())
			{
				return new UniTask<ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11>>>(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11>>(task1.GetAwaiter().GetResult(), task2.GetAwaiter().GetResult(), task3.GetAwaiter().GetResult(), task4.GetAwaiter().GetResult(), task5.GetAwaiter().GetResult(), task6.GetAwaiter().GetResult(), task7.GetAwaiter().GetResult(), new ValueTuple<T8, T9, T10, T11>(task8.GetAwaiter().GetResult(), task9.GetAwaiter().GetResult(), task10.GetAwaiter().GetResult(), task11.GetAwaiter().GetResult())));
			}
			return new UniTask<ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11>>>(new UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(task1, task2, task3, task4, task5, task6, task7, task8, task9, task10, task11), 0);
		}

		public static UniTask<ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12>>> WhenAll<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4, UniTask<T5> task5, UniTask<T6> task6, UniTask<T7> task7, UniTask<T8> task8, UniTask<T9> task9, UniTask<T10> task10, UniTask<T11> task11, UniTask<T12> task12)
		{
			if (task1.Status.IsCompletedSuccessfully() && task2.Status.IsCompletedSuccessfully() && task3.Status.IsCompletedSuccessfully() && task4.Status.IsCompletedSuccessfully() && task5.Status.IsCompletedSuccessfully() && task6.Status.IsCompletedSuccessfully() && task7.Status.IsCompletedSuccessfully() && task8.Status.IsCompletedSuccessfully() && task9.Status.IsCompletedSuccessfully() && task10.Status.IsCompletedSuccessfully() && task11.Status.IsCompletedSuccessfully() && task12.Status.IsCompletedSuccessfully())
			{
				return new UniTask<ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12>>>(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12>>(task1.GetAwaiter().GetResult(), task2.GetAwaiter().GetResult(), task3.GetAwaiter().GetResult(), task4.GetAwaiter().GetResult(), task5.GetAwaiter().GetResult(), task6.GetAwaiter().GetResult(), task7.GetAwaiter().GetResult(), new ValueTuple<T8, T9, T10, T11, T12>(task8.GetAwaiter().GetResult(), task9.GetAwaiter().GetResult(), task10.GetAwaiter().GetResult(), task11.GetAwaiter().GetResult(), task12.GetAwaiter().GetResult())));
			}
			return new UniTask<ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12>>>(new UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(task1, task2, task3, task4, task5, task6, task7, task8, task9, task10, task11, task12), 0);
		}

		public static UniTask<ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13>>> WhenAll<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4, UniTask<T5> task5, UniTask<T6> task6, UniTask<T7> task7, UniTask<T8> task8, UniTask<T9> task9, UniTask<T10> task10, UniTask<T11> task11, UniTask<T12> task12, UniTask<T13> task13)
		{
			if (task1.Status.IsCompletedSuccessfully() && task2.Status.IsCompletedSuccessfully() && task3.Status.IsCompletedSuccessfully() && task4.Status.IsCompletedSuccessfully() && task5.Status.IsCompletedSuccessfully() && task6.Status.IsCompletedSuccessfully() && task7.Status.IsCompletedSuccessfully() && task8.Status.IsCompletedSuccessfully() && task9.Status.IsCompletedSuccessfully() && task10.Status.IsCompletedSuccessfully() && task11.Status.IsCompletedSuccessfully() && task12.Status.IsCompletedSuccessfully() && task13.Status.IsCompletedSuccessfully())
			{
				return new UniTask<ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13>>>(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13>>(task1.GetAwaiter().GetResult(), task2.GetAwaiter().GetResult(), task3.GetAwaiter().GetResult(), task4.GetAwaiter().GetResult(), task5.GetAwaiter().GetResult(), task6.GetAwaiter().GetResult(), task7.GetAwaiter().GetResult(), new ValueTuple<T8, T9, T10, T11, T12, T13>(task8.GetAwaiter().GetResult(), task9.GetAwaiter().GetResult(), task10.GetAwaiter().GetResult(), task11.GetAwaiter().GetResult(), task12.GetAwaiter().GetResult(), task13.GetAwaiter().GetResult())));
			}
			return new UniTask<ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13>>>(new UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(task1, task2, task3, task4, task5, task6, task7, task8, task9, task10, task11, task12, task13), 0);
		}

		public static UniTask<ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14>>> WhenAll<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4, UniTask<T5> task5, UniTask<T6> task6, UniTask<T7> task7, UniTask<T8> task8, UniTask<T9> task9, UniTask<T10> task10, UniTask<T11> task11, UniTask<T12> task12, UniTask<T13> task13, UniTask<T14> task14)
		{
			if (task1.Status.IsCompletedSuccessfully() && task2.Status.IsCompletedSuccessfully() && task3.Status.IsCompletedSuccessfully() && task4.Status.IsCompletedSuccessfully() && task5.Status.IsCompletedSuccessfully() && task6.Status.IsCompletedSuccessfully() && task7.Status.IsCompletedSuccessfully() && task8.Status.IsCompletedSuccessfully() && task9.Status.IsCompletedSuccessfully() && task10.Status.IsCompletedSuccessfully() && task11.Status.IsCompletedSuccessfully() && task12.Status.IsCompletedSuccessfully() && task13.Status.IsCompletedSuccessfully() && task14.Status.IsCompletedSuccessfully())
			{
				return new UniTask<ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14>>>(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14>>(task1.GetAwaiter().GetResult(), task2.GetAwaiter().GetResult(), task3.GetAwaiter().GetResult(), task4.GetAwaiter().GetResult(), task5.GetAwaiter().GetResult(), task6.GetAwaiter().GetResult(), task7.GetAwaiter().GetResult(), new ValueTuple<T8, T9, T10, T11, T12, T13, T14>(task8.GetAwaiter().GetResult(), task9.GetAwaiter().GetResult(), task10.GetAwaiter().GetResult(), task11.GetAwaiter().GetResult(), task12.GetAwaiter().GetResult(), task13.GetAwaiter().GetResult(), task14.GetAwaiter().GetResult())));
			}
			return new UniTask<ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14>>>(new UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(task1, task2, task3, task4, task5, task6, task7, task8, task9, task10, task11, task12, task13, task14), 0);
		}

		public static UniTask<ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15>>>> WhenAll<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4, UniTask<T5> task5, UniTask<T6> task6, UniTask<T7> task7, UniTask<T8> task8, UniTask<T9> task9, UniTask<T10> task10, UniTask<T11> task11, UniTask<T12> task12, UniTask<T13> task13, UniTask<T14> task14, UniTask<T15> task15)
		{
			if (task1.Status.IsCompletedSuccessfully() && task2.Status.IsCompletedSuccessfully() && task3.Status.IsCompletedSuccessfully() && task4.Status.IsCompletedSuccessfully() && task5.Status.IsCompletedSuccessfully() && task6.Status.IsCompletedSuccessfully() && task7.Status.IsCompletedSuccessfully() && task8.Status.IsCompletedSuccessfully() && task9.Status.IsCompletedSuccessfully() && task10.Status.IsCompletedSuccessfully() && task11.Status.IsCompletedSuccessfully() && task12.Status.IsCompletedSuccessfully() && task13.Status.IsCompletedSuccessfully() && task14.Status.IsCompletedSuccessfully() && task15.Status.IsCompletedSuccessfully())
			{
				return new UniTask<ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15>>>>(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15>>>(task1.GetAwaiter().GetResult(), task2.GetAwaiter().GetResult(), task3.GetAwaiter().GetResult(), task4.GetAwaiter().GetResult(), task5.GetAwaiter().GetResult(), task6.GetAwaiter().GetResult(), task7.GetAwaiter().GetResult(), new ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15>>(task8.GetAwaiter().GetResult(), task9.GetAwaiter().GetResult(), task10.GetAwaiter().GetResult(), task11.GetAwaiter().GetResult(), task12.GetAwaiter().GetResult(), task13.GetAwaiter().GetResult(), task14.GetAwaiter().GetResult(), new ValueTuple<T15>(task15.GetAwaiter().GetResult()))));
			}
			return new UniTask<ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15>>>>(new UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(task1, task2, task3, task4, task5, task6, task7, task8, task9, task10, task11, task12, task13, task14, task15), 0);
		}

		[return: TupleElementNames(new string[]
		{
			"hasResultLeft",
			"result"
		})]
		public static UniTask<ValueTuple<bool, T>> WhenAny<T>(UniTask<T> leftTask, UniTask rightTask)
		{
			return new UniTask<ValueTuple<bool, T>>(new UniTask.WhenAnyLRPromise<T>(leftTask, rightTask), 0);
		}

		[return: TupleElementNames(new string[]
		{
			"winArgumentIndex",
			"result"
		})]
		public static UniTask<ValueTuple<int, T>> WhenAny<T>(params UniTask<T>[] tasks)
		{
			return new UniTask<ValueTuple<int, T>>(new UniTask.WhenAnyPromise<T>(tasks, tasks.Length), 0);
		}

		[return: TupleElementNames(new string[]
		{
			"winArgumentIndex",
			"result"
		})]
		public static UniTask<ValueTuple<int, T>> WhenAny<T>(IEnumerable<UniTask<T>> tasks)
		{
			UniTask<ValueTuple<int, T>> result;
			using (ArrayPoolUtil.RentArray<UniTask<T>> rentArray = ArrayPoolUtil.Materialize<UniTask<T>>(tasks))
			{
				result = new UniTask<ValueTuple<int, T>>(new UniTask.WhenAnyPromise<T>(rentArray.Array, rentArray.Length), 0);
			}
			return result;
		}

		public static UniTask<int> WhenAny(params UniTask[] tasks)
		{
			return new UniTask<int>(new UniTask.WhenAnyPromise(tasks, tasks.Length), 0);
		}

		public static UniTask<int> WhenAny(IEnumerable<UniTask> tasks)
		{
			UniTask<int> result;
			using (ArrayPoolUtil.RentArray<UniTask> rentArray = ArrayPoolUtil.Materialize<UniTask>(tasks))
			{
				result = new UniTask<int>(new UniTask.WhenAnyPromise(rentArray.Array, rentArray.Length), 0);
			}
			return result;
		}

		[return: TupleElementNames(new string[]
		{
			"winArgumentIndex",
			"result1",
			"result2"
		})]
		public static UniTask<ValueTuple<int, T1, T2>> WhenAny<T1, T2>(UniTask<T1> task1, UniTask<T2> task2)
		{
			return new UniTask<ValueTuple<int, T1, T2>>(new UniTask.WhenAnyPromise<T1, T2>(task1, task2), 0);
		}

		[return: TupleElementNames(new string[]
		{
			"winArgumentIndex",
			"result1",
			"result2",
			"result3"
		})]
		public static UniTask<ValueTuple<int, T1, T2, T3>> WhenAny<T1, T2, T3>(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3)
		{
			return new UniTask<ValueTuple<int, T1, T2, T3>>(new UniTask.WhenAnyPromise<T1, T2, T3>(task1, task2, task3), 0);
		}

		[return: TupleElementNames(new string[]
		{
			"winArgumentIndex",
			"result1",
			"result2",
			"result3",
			"result4"
		})]
		public static UniTask<ValueTuple<int, T1, T2, T3, T4>> WhenAny<T1, T2, T3, T4>(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4)
		{
			return new UniTask<ValueTuple<int, T1, T2, T3, T4>>(new UniTask.WhenAnyPromise<T1, T2, T3, T4>(task1, task2, task3, task4), 0);
		}

		[return: TupleElementNames(new string[]
		{
			"winArgumentIndex",
			"result1",
			"result2",
			"result3",
			"result4",
			"result5"
		})]
		public static UniTask<ValueTuple<int, T1, T2, T3, T4, T5>> WhenAny<T1, T2, T3, T4, T5>(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4, UniTask<T5> task5)
		{
			return new UniTask<ValueTuple<int, T1, T2, T3, T4, T5>>(new UniTask.WhenAnyPromise<T1, T2, T3, T4, T5>(task1, task2, task3, task4, task5), 0);
		}

		[return: TupleElementNames(new string[]
		{
			"winArgumentIndex",
			"result1",
			"result2",
			"result3",
			"result4",
			"result5",
			"result6"
		})]
		public static UniTask<ValueTuple<int, T1, T2, T3, T4, T5, T6>> WhenAny<T1, T2, T3, T4, T5, T6>(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4, UniTask<T5> task5, UniTask<T6> task6)
		{
			return new UniTask<ValueTuple<int, T1, T2, T3, T4, T5, T6>>(new UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6>(task1, task2, task3, task4, task5, task6), 0);
		}

		[return: TupleElementNames(new string[]
		{
			"winArgumentIndex",
			"result1",
			"result2",
			"result3",
			"result4",
			"result5",
			"result6",
			"result7",
			null
		})]
		public static UniTask<ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7>>> WhenAny<T1, T2, T3, T4, T5, T6, T7>(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4, UniTask<T5> task5, UniTask<T6> task6, UniTask<T7> task7)
		{
			return new UniTask<ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7>>>(new UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7>(task1, task2, task3, task4, task5, task6, task7), 0);
		}

		[return: TupleElementNames(new string[]
		{
			"winArgumentIndex",
			"result1",
			"result2",
			"result3",
			"result4",
			"result5",
			"result6",
			"result7",
			"result8",
			null,
			null
		})]
		public static UniTask<ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8>>> WhenAny<T1, T2, T3, T4, T5, T6, T7, T8>(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4, UniTask<T5> task5, UniTask<T6> task6, UniTask<T7> task7, UniTask<T8> task8)
		{
			return new UniTask<ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8>>>(new UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8>(task1, task2, task3, task4, task5, task6, task7, task8), 0);
		}

		[return: TupleElementNames(new string[]
		{
			"winArgumentIndex",
			"result1",
			"result2",
			"result3",
			"result4",
			"result5",
			"result6",
			"result7",
			"result8",
			"result9",
			null,
			null,
			null
		})]
		public static UniTask<ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9>>> WhenAny<T1, T2, T3, T4, T5, T6, T7, T8, T9>(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4, UniTask<T5> task5, UniTask<T6> task6, UniTask<T7> task7, UniTask<T8> task8, UniTask<T9> task9)
		{
			return new UniTask<ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9>>>(new UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>(task1, task2, task3, task4, task5, task6, task7, task8, task9), 0);
		}

		[return: TupleElementNames(new string[]
		{
			"winArgumentIndex",
			"result1",
			"result2",
			"result3",
			"result4",
			"result5",
			"result6",
			"result7",
			"result8",
			"result9",
			"result10",
			null,
			null,
			null,
			null
		})]
		public static UniTask<ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10>>> WhenAny<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4, UniTask<T5> task5, UniTask<T6> task6, UniTask<T7> task7, UniTask<T8> task8, UniTask<T9> task9, UniTask<T10> task10)
		{
			return new UniTask<ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10>>>(new UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(task1, task2, task3, task4, task5, task6, task7, task8, task9, task10), 0);
		}

		[return: TupleElementNames(new string[]
		{
			"winArgumentIndex",
			"result1",
			"result2",
			"result3",
			"result4",
			"result5",
			"result6",
			"result7",
			"result8",
			"result9",
			"result10",
			"result11",
			null,
			null,
			null,
			null,
			null
		})]
		public static UniTask<ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11>>> WhenAny<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4, UniTask<T5> task5, UniTask<T6> task6, UniTask<T7> task7, UniTask<T8> task8, UniTask<T9> task9, UniTask<T10> task10, UniTask<T11> task11)
		{
			return new UniTask<ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11>>>(new UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(task1, task2, task3, task4, task5, task6, task7, task8, task9, task10, task11), 0);
		}

		[return: TupleElementNames(new string[]
		{
			"winArgumentIndex",
			"result1",
			"result2",
			"result3",
			"result4",
			"result5",
			"result6",
			"result7",
			"result8",
			"result9",
			"result10",
			"result11",
			"result12",
			null,
			null,
			null,
			null,
			null,
			null
		})]
		public static UniTask<ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12>>> WhenAny<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4, UniTask<T5> task5, UniTask<T6> task6, UniTask<T7> task7, UniTask<T8> task8, UniTask<T9> task9, UniTask<T10> task10, UniTask<T11> task11, UniTask<T12> task12)
		{
			return new UniTask<ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12>>>(new UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(task1, task2, task3, task4, task5, task6, task7, task8, task9, task10, task11, task12), 0);
		}

		[return: TupleElementNames(new string[]
		{
			"winArgumentIndex",
			"result1",
			"result2",
			"result3",
			"result4",
			"result5",
			"result6",
			"result7",
			"result8",
			"result9",
			"result10",
			"result11",
			"result12",
			"result13",
			null,
			null,
			null,
			null,
			null,
			null,
			null
		})]
		public static UniTask<ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13>>> WhenAny<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4, UniTask<T5> task5, UniTask<T6> task6, UniTask<T7> task7, UniTask<T8> task8, UniTask<T9> task9, UniTask<T10> task10, UniTask<T11> task11, UniTask<T12> task12, UniTask<T13> task13)
		{
			return new UniTask<ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13>>>(new UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(task1, task2, task3, task4, task5, task6, task7, task8, task9, task10, task11, task12, task13), 0);
		}

		[return: TupleElementNames(new string[]
		{
			"winArgumentIndex",
			"result1",
			"result2",
			"result3",
			"result4",
			"result5",
			"result6",
			"result7",
			"result8",
			"result9",
			"result10",
			"result11",
			"result12",
			"result13",
			"result14",
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null
		})]
		public static UniTask<ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14>>>> WhenAny<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4, UniTask<T5> task5, UniTask<T6> task6, UniTask<T7> task7, UniTask<T8> task8, UniTask<T9> task9, UniTask<T10> task10, UniTask<T11> task11, UniTask<T12> task12, UniTask<T13> task13, UniTask<T14> task14)
		{
			return new UniTask<ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14>>>>(new UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(task1, task2, task3, task4, task5, task6, task7, task8, task9, task10, task11, task12, task13, task14), 0);
		}

		[return: TupleElementNames(new string[]
		{
			"winArgumentIndex",
			"result1",
			"result2",
			"result3",
			"result4",
			"result5",
			"result6",
			"result7",
			"result8",
			"result9",
			"result10",
			"result11",
			"result12",
			"result13",
			"result14",
			"result15",
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null
		})]
		public static UniTask<ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14, T15>>>> WhenAny<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4, UniTask<T5> task5, UniTask<T6> task6, UniTask<T7> task7, UniTask<T8> task8, UniTask<T9> task9, UniTask<T10> task10, UniTask<T11> task11, UniTask<T12> task12, UniTask<T13> task13, UniTask<T14> task14, UniTask<T15> task15)
		{
			return new UniTask<ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14, T15>>>>(new UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(task1, task2, task3, task4, task5, task6, task7, task8, task9, task10, task11, task12, task13, task14, task15), 0);
		}

		private readonly IUniTaskSource source;

		private readonly short token;

		private static readonly UniTask CanceledUniTask = (() => new UniTask(new UniTask.CanceledResultSource(CancellationToken.None), 0))();

		public static readonly UniTask CompletedTask = default(UniTask);

		private sealed class AsyncUnitSource : IUniTaskSource<AsyncUnit>, IUniTaskSource
		{
			public AsyncUnitSource(IUniTaskSource source)
			{
				this.source = source;
			}

			public AsyncUnit GetResult(short token)
			{
				this.source.GetResult(token);
				return AsyncUnit.Default;
			}

			public UniTaskStatus GetStatus(short token)
			{
				return this.source.GetStatus(token);
			}

			public void OnCompleted(Action<object> continuation, object state, short token)
			{
				this.source.OnCompleted(continuation, state, token);
			}

			public UniTaskStatus UnsafeGetStatus()
			{
				return this.source.UnsafeGetStatus();
			}

			void IUniTaskSource.GetResult(short token)
			{
				this.GetResult(token);
			}

			private readonly IUniTaskSource source;
		}

		private sealed class IsCanceledSource : IUniTaskSource<bool>, IUniTaskSource
		{
			public IsCanceledSource(IUniTaskSource source)
			{
				this.source = source;
			}

			public bool GetResult(short token)
			{
				if (this.source.GetStatus(token) == UniTaskStatus.Canceled)
				{
					return true;
				}
				this.source.GetResult(token);
				return false;
			}

			void IUniTaskSource.GetResult(short token)
			{
				this.GetResult(token);
			}

			public UniTaskStatus GetStatus(short token)
			{
				return this.source.GetStatus(token);
			}

			public UniTaskStatus UnsafeGetStatus()
			{
				return this.source.UnsafeGetStatus();
			}

			public void OnCompleted(Action<object> continuation, object state, short token)
			{
				this.source.OnCompleted(continuation, state, token);
			}

			private readonly IUniTaskSource source;
		}

		private sealed class MemoizeSource : IUniTaskSource
		{
			public MemoizeSource(IUniTaskSource source)
			{
				this.source = source;
			}

			public void GetResult(short token)
			{
				if (this.source == null)
				{
					if (this.exception != null)
					{
						this.exception.Throw();
						return;
					}
				}
				else
				{
					try
					{
						this.source.GetResult(token);
						this.status = UniTaskStatus.Succeeded;
					}
					catch (Exception ex)
					{
						this.exception = ExceptionDispatchInfo.Capture(ex);
						if (ex is OperationCanceledException)
						{
							this.status = UniTaskStatus.Canceled;
						}
						else
						{
							this.status = UniTaskStatus.Faulted;
						}
						throw;
					}
					finally
					{
						this.source = null;
					}
				}
			}

			public UniTaskStatus GetStatus(short token)
			{
				if (this.source == null)
				{
					return this.status;
				}
				return this.source.GetStatus(token);
			}

			public void OnCompleted(Action<object> continuation, object state, short token)
			{
				if (this.source == null)
				{
					continuation(state);
					return;
				}
				this.source.OnCompleted(continuation, state, token);
			}

			public UniTaskStatus UnsafeGetStatus()
			{
				if (this.source == null)
				{
					return this.status;
				}
				return this.source.UnsafeGetStatus();
			}

			private IUniTaskSource source;

			private ExceptionDispatchInfo exception;

			private UniTaskStatus status;
		}

		public readonly struct Awaiter : ICriticalNotifyCompletion, INotifyCompletion
		{
			[DebuggerHidden]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Awaiter(in UniTask task)
			{
				this.task = task;
			}

			public bool IsCompleted
			{
				[DebuggerHidden]
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return this.task.Status.IsCompleted();
				}
			}

			[DebuggerHidden]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void GetResult()
			{
				if (this.task.source == null)
				{
					return;
				}
				this.task.source.GetResult(this.task.token);
			}

			[DebuggerHidden]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void OnCompleted(Action continuation)
			{
				if (this.task.source == null)
				{
					continuation();
					return;
				}
				this.task.source.OnCompleted(AwaiterActions.InvokeContinuationDelegate, continuation, this.task.token);
			}

			[DebuggerHidden]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void UnsafeOnCompleted(Action continuation)
			{
				if (this.task.source == null)
				{
					continuation();
					return;
				}
				this.task.source.OnCompleted(AwaiterActions.InvokeContinuationDelegate, continuation, this.task.token);
			}

			[DebuggerHidden]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void SourceOnCompleted(Action<object> continuation, object state)
			{
				if (this.task.source == null)
				{
					continuation(state);
					return;
				}
				this.task.source.OnCompleted(continuation, state, this.task.token);
			}

			private readonly UniTask task;
		}

		private sealed class YieldPromise : IUniTaskSource, IPlayerLoopItem, ITaskPoolNode<UniTask.YieldPromise>
		{
			public ref UniTask.YieldPromise NextNode
			{
				get
				{
					return ref this.nextNode;
				}
			}

			static YieldPromise()
			{
				TaskPool.RegisterSizeGetter(typeof(UniTask.YieldPromise), () => UniTask.YieldPromise.pool.Size);
			}

			private YieldPromise()
			{
			}

			public static IUniTaskSource Create(PlayerLoopTiming timing, CancellationToken cancellationToken, out short token)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return AutoResetUniTaskCompletionSource.CreateFromCanceled(cancellationToken, out token);
				}
				UniTask.YieldPromise yieldPromise;
				if (!UniTask.YieldPromise.pool.TryPop(out yieldPromise))
				{
					yieldPromise = new UniTask.YieldPromise();
				}
				yieldPromise.cancellationToken = cancellationToken;
				PlayerLoopHelper.AddAction(timing, yieldPromise);
				token = yieldPromise.core.Version;
				return yieldPromise;
			}

			public void GetResult(short token)
			{
				try
				{
					this.core.GetResult(token);
				}
				finally
				{
					this.TryReturn();
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
				if (this.cancellationToken.IsCancellationRequested)
				{
					this.core.TrySetCanceled(this.cancellationToken);
					return false;
				}
				this.core.TrySetResult(null);
				return false;
			}

			private bool TryReturn()
			{
				this.core.Reset();
				this.cancellationToken = default(CancellationToken);
				return UniTask.YieldPromise.pool.TryPush(this);
			}

			private static TaskPool<UniTask.YieldPromise> pool;

			private UniTask.YieldPromise nextNode;

			private CancellationToken cancellationToken;

			private UniTaskCompletionSourceCore<object> core;
		}

		private sealed class NextFramePromise : IUniTaskSource, IPlayerLoopItem, ITaskPoolNode<UniTask.NextFramePromise>
		{
			public ref UniTask.NextFramePromise NextNode
			{
				get
				{
					return ref this.nextNode;
				}
			}

			static NextFramePromise()
			{
				TaskPool.RegisterSizeGetter(typeof(UniTask.NextFramePromise), () => UniTask.NextFramePromise.pool.Size);
			}

			private NextFramePromise()
			{
			}

			public static IUniTaskSource Create(PlayerLoopTiming timing, CancellationToken cancellationToken, out short token)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return AutoResetUniTaskCompletionSource.CreateFromCanceled(cancellationToken, out token);
				}
				UniTask.NextFramePromise nextFramePromise;
				if (!UniTask.NextFramePromise.pool.TryPop(out nextFramePromise))
				{
					nextFramePromise = new UniTask.NextFramePromise();
				}
				nextFramePromise.frameCount = (PlayerLoopHelper.IsMainThread ? Time.frameCount : -1);
				nextFramePromise.cancellationToken = cancellationToken;
				PlayerLoopHelper.AddAction(timing, nextFramePromise);
				token = nextFramePromise.core.Version;
				return nextFramePromise;
			}

			public void GetResult(short token)
			{
				try
				{
					this.core.GetResult(token);
				}
				finally
				{
					this.TryReturn();
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
				if (this.cancellationToken.IsCancellationRequested)
				{
					this.core.TrySetCanceled(this.cancellationToken);
					return false;
				}
				if (this.frameCount == Time.frameCount)
				{
					return true;
				}
				this.core.TrySetResult(AsyncUnit.Default);
				return false;
			}

			private bool TryReturn()
			{
				this.core.Reset();
				this.cancellationToken = default(CancellationToken);
				return UniTask.NextFramePromise.pool.TryPush(this);
			}

			private static TaskPool<UniTask.NextFramePromise> pool;

			private UniTask.NextFramePromise nextNode;

			private int frameCount;

			private CancellationToken cancellationToken;

			private UniTaskCompletionSourceCore<AsyncUnit> core;
		}

		private sealed class WaitForEndOfFramePromise : IUniTaskSource, ITaskPoolNode<UniTask.WaitForEndOfFramePromise>, IEnumerator
		{
			public ref UniTask.WaitForEndOfFramePromise NextNode
			{
				get
				{
					return ref this.nextNode;
				}
			}

			static WaitForEndOfFramePromise()
			{
				TaskPool.RegisterSizeGetter(typeof(UniTask.WaitForEndOfFramePromise), () => UniTask.WaitForEndOfFramePromise.pool.Size);
			}

			private WaitForEndOfFramePromise()
			{
			}

			public static IUniTaskSource Create(MonoBehaviour coroutineRunner, CancellationToken cancellationToken, out short token)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return AutoResetUniTaskCompletionSource.CreateFromCanceled(cancellationToken, out token);
				}
				UniTask.WaitForEndOfFramePromise waitForEndOfFramePromise;
				if (!UniTask.WaitForEndOfFramePromise.pool.TryPop(out waitForEndOfFramePromise))
				{
					waitForEndOfFramePromise = new UniTask.WaitForEndOfFramePromise();
				}
				waitForEndOfFramePromise.cancellationToken = cancellationToken;
				coroutineRunner.StartCoroutine(waitForEndOfFramePromise);
				token = waitForEndOfFramePromise.core.Version;
				return waitForEndOfFramePromise;
			}

			public void GetResult(short token)
			{
				try
				{
					this.core.GetResult(token);
				}
				finally
				{
					this.TryReturn();
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

			private bool TryReturn()
			{
				this.core.Reset();
				this.Reset();
				this.cancellationToken = default(CancellationToken);
				return UniTask.WaitForEndOfFramePromise.pool.TryPush(this);
			}

			object IEnumerator.Current
			{
				get
				{
					return UniTask.WaitForEndOfFramePromise.waitForEndOfFrameYieldInstruction;
				}
			}

			bool IEnumerator.MoveNext()
			{
				if (this.isFirst)
				{
					this.isFirst = false;
					return true;
				}
				if (this.cancellationToken.IsCancellationRequested)
				{
					this.core.TrySetCanceled(this.cancellationToken);
					return false;
				}
				this.core.TrySetResult(null);
				return false;
			}

			public void Reset()
			{
				this.isFirst = true;
			}

			private static TaskPool<UniTask.WaitForEndOfFramePromise> pool;

			private UniTask.WaitForEndOfFramePromise nextNode;

			private CancellationToken cancellationToken;

			private UniTaskCompletionSourceCore<object> core;

			private static readonly WaitForEndOfFrame waitForEndOfFrameYieldInstruction = new WaitForEndOfFrame();

			private bool isFirst = true;
		}

		private sealed class DelayFramePromise : IUniTaskSource, IPlayerLoopItem, ITaskPoolNode<UniTask.DelayFramePromise>
		{
			public ref UniTask.DelayFramePromise NextNode
			{
				get
				{
					return ref this.nextNode;
				}
			}

			static DelayFramePromise()
			{
				TaskPool.RegisterSizeGetter(typeof(UniTask.DelayFramePromise), () => UniTask.DelayFramePromise.pool.Size);
			}

			private DelayFramePromise()
			{
			}

			public static IUniTaskSource Create(int delayFrameCount, PlayerLoopTiming timing, CancellationToken cancellationToken, out short token)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return AutoResetUniTaskCompletionSource.CreateFromCanceled(cancellationToken, out token);
				}
				UniTask.DelayFramePromise delayFramePromise;
				if (!UniTask.DelayFramePromise.pool.TryPop(out delayFramePromise))
				{
					delayFramePromise = new UniTask.DelayFramePromise();
				}
				delayFramePromise.delayFrameCount = delayFrameCount;
				delayFramePromise.cancellationToken = cancellationToken;
				delayFramePromise.initialFrame = (PlayerLoopHelper.IsMainThread ? Time.frameCount : -1);
				PlayerLoopHelper.AddAction(timing, delayFramePromise);
				token = delayFramePromise.core.Version;
				return delayFramePromise;
			}

			public void GetResult(short token)
			{
				try
				{
					this.core.GetResult(token);
				}
				finally
				{
					this.TryReturn();
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
				if (this.cancellationToken.IsCancellationRequested)
				{
					this.core.TrySetCanceled(this.cancellationToken);
					return false;
				}
				if (this.currentFrameCount == 0)
				{
					if (this.delayFrameCount == 0)
					{
						this.core.TrySetResult(AsyncUnit.Default);
						return false;
					}
					if (this.initialFrame == Time.frameCount)
					{
						return true;
					}
				}
				int num = this.currentFrameCount + 1;
				this.currentFrameCount = num;
				if (num >= this.delayFrameCount)
				{
					this.core.TrySetResult(AsyncUnit.Default);
					return false;
				}
				return true;
			}

			private bool TryReturn()
			{
				this.core.Reset();
				this.currentFrameCount = 0;
				this.delayFrameCount = 0;
				this.cancellationToken = default(CancellationToken);
				return UniTask.DelayFramePromise.pool.TryPush(this);
			}

			private static TaskPool<UniTask.DelayFramePromise> pool;

			private UniTask.DelayFramePromise nextNode;

			private int initialFrame;

			private int delayFrameCount;

			private CancellationToken cancellationToken;

			private int currentFrameCount;

			private UniTaskCompletionSourceCore<AsyncUnit> core;
		}

		private sealed class DelayPromise : IUniTaskSource, IPlayerLoopItem, ITaskPoolNode<UniTask.DelayPromise>
		{
			public ref UniTask.DelayPromise NextNode
			{
				get
				{
					return ref this.nextNode;
				}
			}

			static DelayPromise()
			{
				TaskPool.RegisterSizeGetter(typeof(UniTask.DelayPromise), () => UniTask.DelayPromise.pool.Size);
			}

			private DelayPromise()
			{
			}

			public static IUniTaskSource Create(TimeSpan delayTimeSpan, PlayerLoopTiming timing, CancellationToken cancellationToken, out short token)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return AutoResetUniTaskCompletionSource.CreateFromCanceled(cancellationToken, out token);
				}
				UniTask.DelayPromise delayPromise;
				if (!UniTask.DelayPromise.pool.TryPop(out delayPromise))
				{
					delayPromise = new UniTask.DelayPromise();
				}
				delayPromise.elapsed = 0f;
				delayPromise.delayTimeSpan = (float)delayTimeSpan.TotalSeconds;
				delayPromise.cancellationToken = cancellationToken;
				delayPromise.initialFrame = (PlayerLoopHelper.IsMainThread ? Time.frameCount : -1);
				PlayerLoopHelper.AddAction(timing, delayPromise);
				token = delayPromise.core.Version;
				return delayPromise;
			}

			public void GetResult(short token)
			{
				try
				{
					this.core.GetResult(token);
				}
				finally
				{
					this.TryReturn();
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
				if (this.cancellationToken.IsCancellationRequested)
				{
					this.core.TrySetCanceled(this.cancellationToken);
					return false;
				}
				if (this.elapsed == 0f && this.initialFrame == Time.frameCount)
				{
					return true;
				}
				this.elapsed += Time.deltaTime;
				if (this.elapsed >= this.delayTimeSpan)
				{
					this.core.TrySetResult(null);
					return false;
				}
				return true;
			}

			private bool TryReturn()
			{
				this.core.Reset();
				this.delayTimeSpan = 0f;
				this.elapsed = 0f;
				this.cancellationToken = default(CancellationToken);
				return UniTask.DelayPromise.pool.TryPush(this);
			}

			private static TaskPool<UniTask.DelayPromise> pool;

			private UniTask.DelayPromise nextNode;

			private int initialFrame;

			private float delayTimeSpan;

			private float elapsed;

			private CancellationToken cancellationToken;

			private UniTaskCompletionSourceCore<object> core;
		}

		private sealed class DelayIgnoreTimeScalePromise : IUniTaskSource, IPlayerLoopItem, ITaskPoolNode<UniTask.DelayIgnoreTimeScalePromise>
		{
			public ref UniTask.DelayIgnoreTimeScalePromise NextNode
			{
				get
				{
					return ref this.nextNode;
				}
			}

			static DelayIgnoreTimeScalePromise()
			{
				TaskPool.RegisterSizeGetter(typeof(UniTask.DelayIgnoreTimeScalePromise), () => UniTask.DelayIgnoreTimeScalePromise.pool.Size);
			}

			private DelayIgnoreTimeScalePromise()
			{
			}

			public static IUniTaskSource Create(TimeSpan delayFrameTimeSpan, PlayerLoopTiming timing, CancellationToken cancellationToken, out short token)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return AutoResetUniTaskCompletionSource.CreateFromCanceled(cancellationToken, out token);
				}
				UniTask.DelayIgnoreTimeScalePromise delayIgnoreTimeScalePromise;
				if (!UniTask.DelayIgnoreTimeScalePromise.pool.TryPop(out delayIgnoreTimeScalePromise))
				{
					delayIgnoreTimeScalePromise = new UniTask.DelayIgnoreTimeScalePromise();
				}
				delayIgnoreTimeScalePromise.elapsed = 0f;
				delayIgnoreTimeScalePromise.delayFrameTimeSpan = (float)delayFrameTimeSpan.TotalSeconds;
				delayIgnoreTimeScalePromise.initialFrame = (PlayerLoopHelper.IsMainThread ? Time.frameCount : -1);
				delayIgnoreTimeScalePromise.cancellationToken = cancellationToken;
				PlayerLoopHelper.AddAction(timing, delayIgnoreTimeScalePromise);
				token = delayIgnoreTimeScalePromise.core.Version;
				return delayIgnoreTimeScalePromise;
			}

			public void GetResult(short token)
			{
				try
				{
					this.core.GetResult(token);
				}
				finally
				{
					this.TryReturn();
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
				if (this.cancellationToken.IsCancellationRequested)
				{
					this.core.TrySetCanceled(this.cancellationToken);
					return false;
				}
				if (this.elapsed == 0f && this.initialFrame == Time.frameCount)
				{
					return true;
				}
				this.elapsed += Time.unscaledDeltaTime;
				if (this.elapsed >= this.delayFrameTimeSpan)
				{
					this.core.TrySetResult(null);
					return false;
				}
				return true;
			}

			private bool TryReturn()
			{
				this.core.Reset();
				this.delayFrameTimeSpan = 0f;
				this.elapsed = 0f;
				this.cancellationToken = default(CancellationToken);
				return UniTask.DelayIgnoreTimeScalePromise.pool.TryPush(this);
			}

			private static TaskPool<UniTask.DelayIgnoreTimeScalePromise> pool;

			private UniTask.DelayIgnoreTimeScalePromise nextNode;

			private float delayFrameTimeSpan;

			private float elapsed;

			private int initialFrame;

			private CancellationToken cancellationToken;

			private UniTaskCompletionSourceCore<object> core;
		}

		private sealed class DelayRealtimePromise : IUniTaskSource, IPlayerLoopItem, ITaskPoolNode<UniTask.DelayRealtimePromise>
		{
			public ref UniTask.DelayRealtimePromise NextNode
			{
				get
				{
					return ref this.nextNode;
				}
			}

			static DelayRealtimePromise()
			{
				TaskPool.RegisterSizeGetter(typeof(UniTask.DelayRealtimePromise), () => UniTask.DelayRealtimePromise.pool.Size);
			}

			private DelayRealtimePromise()
			{
			}

			public static IUniTaskSource Create(TimeSpan delayTimeSpan, PlayerLoopTiming timing, CancellationToken cancellationToken, out short token)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return AutoResetUniTaskCompletionSource.CreateFromCanceled(cancellationToken, out token);
				}
				UniTask.DelayRealtimePromise delayRealtimePromise;
				if (!UniTask.DelayRealtimePromise.pool.TryPop(out delayRealtimePromise))
				{
					delayRealtimePromise = new UniTask.DelayRealtimePromise();
				}
				delayRealtimePromise.stopwatch = ValueStopwatch.StartNew();
				delayRealtimePromise.delayTimeSpanTicks = delayTimeSpan.Ticks;
				delayRealtimePromise.cancellationToken = cancellationToken;
				PlayerLoopHelper.AddAction(timing, delayRealtimePromise);
				token = delayRealtimePromise.core.Version;
				return delayRealtimePromise;
			}

			public void GetResult(short token)
			{
				try
				{
					this.core.GetResult(token);
				}
				finally
				{
					this.TryReturn();
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
				if (this.cancellationToken.IsCancellationRequested)
				{
					this.core.TrySetCanceled(this.cancellationToken);
					return false;
				}
				if (this.stopwatch.IsInvalid)
				{
					this.core.TrySetResult(AsyncUnit.Default);
					return false;
				}
				if (this.stopwatch.ElapsedTicks >= this.delayTimeSpanTicks)
				{
					this.core.TrySetResult(AsyncUnit.Default);
					return false;
				}
				return true;
			}

			private bool TryReturn()
			{
				this.core.Reset();
				this.stopwatch = default(ValueStopwatch);
				this.cancellationToken = default(CancellationToken);
				return UniTask.DelayRealtimePromise.pool.TryPush(this);
			}

			private static TaskPool<UniTask.DelayRealtimePromise> pool;

			private UniTask.DelayRealtimePromise nextNode;

			private long delayTimeSpanTicks;

			private ValueStopwatch stopwatch;

			private CancellationToken cancellationToken;

			private UniTaskCompletionSourceCore<AsyncUnit> core;
		}

		private static class CanceledUniTaskCache<T>
		{
			public static readonly UniTask<T> Task = new UniTask<T>(new UniTask.CanceledResultSource<T>(CancellationToken.None), 0);
		}

		private sealed class ExceptionResultSource : IUniTaskSource
		{
			public ExceptionResultSource(Exception exception)
			{
				this.exception = ExceptionDispatchInfo.Capture(exception);
			}

			public void GetResult(short token)
			{
				if (!this.calledGet)
				{
					this.calledGet = true;
					GC.SuppressFinalize(this);
				}
				this.exception.Throw();
			}

			public UniTaskStatus GetStatus(short token)
			{
				return UniTaskStatus.Faulted;
			}

			public UniTaskStatus UnsafeGetStatus()
			{
				return UniTaskStatus.Faulted;
			}

			public void OnCompleted(Action<object> continuation, object state, short token)
			{
				continuation(state);
			}

			~ExceptionResultSource()
			{
				if (!this.calledGet)
				{
					UniTaskScheduler.PublishUnobservedTaskException(this.exception.SourceException);
				}
			}

			private readonly ExceptionDispatchInfo exception;

			private bool calledGet;
		}

		private sealed class ExceptionResultSource<T> : IUniTaskSource<!0>, IUniTaskSource
		{
			public ExceptionResultSource(Exception exception)
			{
				this.exception = ExceptionDispatchInfo.Capture(exception);
			}

			public T GetResult(short token)
			{
				if (!this.calledGet)
				{
					this.calledGet = true;
					GC.SuppressFinalize(this);
				}
				this.exception.Throw();
				return default(T);
			}

			void IUniTaskSource.GetResult(short token)
			{
				if (!this.calledGet)
				{
					this.calledGet = true;
					GC.SuppressFinalize(this);
				}
				this.exception.Throw();
			}

			public UniTaskStatus GetStatus(short token)
			{
				return UniTaskStatus.Faulted;
			}

			public UniTaskStatus UnsafeGetStatus()
			{
				return UniTaskStatus.Faulted;
			}

			public void OnCompleted(Action<object> continuation, object state, short token)
			{
				continuation(state);
			}

			~ExceptionResultSource()
			{
				if (!this.calledGet)
				{
					UniTaskScheduler.PublishUnobservedTaskException(this.exception.SourceException);
				}
			}

			private readonly ExceptionDispatchInfo exception;

			private bool calledGet;
		}

		private sealed class CanceledResultSource : IUniTaskSource
		{
			public CanceledResultSource(CancellationToken cancellationToken)
			{
				this.cancellationToken = cancellationToken;
			}

			public void GetResult(short token)
			{
				throw new OperationCanceledException(this.cancellationToken);
			}

			public UniTaskStatus GetStatus(short token)
			{
				return UniTaskStatus.Canceled;
			}

			public UniTaskStatus UnsafeGetStatus()
			{
				return UniTaskStatus.Canceled;
			}

			public void OnCompleted(Action<object> continuation, object state, short token)
			{
				continuation(state);
			}

			private readonly CancellationToken cancellationToken;
		}

		private sealed class CanceledResultSource<T> : IUniTaskSource<!0>, IUniTaskSource
		{
			public CanceledResultSource(CancellationToken cancellationToken)
			{
				this.cancellationToken = cancellationToken;
			}

			public T GetResult(short token)
			{
				throw new OperationCanceledException(this.cancellationToken);
			}

			void IUniTaskSource.GetResult(short token)
			{
				throw new OperationCanceledException(this.cancellationToken);
			}

			public UniTaskStatus GetStatus(short token)
			{
				return UniTaskStatus.Canceled;
			}

			public UniTaskStatus UnsafeGetStatus()
			{
				return UniTaskStatus.Canceled;
			}

			public void OnCompleted(Action<object> continuation, object state, short token)
			{
				continuation(state);
			}

			private readonly CancellationToken cancellationToken;
		}

		private sealed class DeferPromise : IUniTaskSource
		{
			public DeferPromise(Func<UniTask> factory)
			{
				this.factory = factory;
			}

			public void GetResult(short token)
			{
				this.awaiter.GetResult();
			}

			public UniTaskStatus GetStatus(short token)
			{
				Func<UniTask> func = Interlocked.Exchange<Func<UniTask>>(ref this.factory, null);
				if (func != null)
				{
					this.task = func();
					this.awaiter = this.task.GetAwaiter();
				}
				return this.task.Status;
			}

			public void OnCompleted(Action<object> continuation, object state, short token)
			{
				this.awaiter.SourceOnCompleted(continuation, state);
			}

			public UniTaskStatus UnsafeGetStatus()
			{
				return this.task.Status;
			}

			private Func<UniTask> factory;

			private UniTask task;

			private UniTask.Awaiter awaiter;
		}

		private sealed class DeferPromise<T> : IUniTaskSource<!0>, IUniTaskSource
		{
			public DeferPromise(Func<UniTask<T>> factory)
			{
				this.factory = factory;
			}

			public T GetResult(short token)
			{
				return this.awaiter.GetResult();
			}

			void IUniTaskSource.GetResult(short token)
			{
				this.awaiter.GetResult();
			}

			public UniTaskStatus GetStatus(short token)
			{
				Func<UniTask<T>> func = Interlocked.Exchange<Func<UniTask<T>>>(ref this.factory, null);
				if (func != null)
				{
					this.task = func();
					this.awaiter = this.task.GetAwaiter();
				}
				return this.task.Status;
			}

			public void OnCompleted(Action<object> continuation, object state, short token)
			{
				this.awaiter.SourceOnCompleted(continuation, state);
			}

			public UniTaskStatus UnsafeGetStatus()
			{
				return this.task.Status;
			}

			private Func<UniTask<T>> factory;

			private UniTask<T> task;

			private UniTask<T>.Awaiter awaiter;
		}

		private sealed class NeverPromise<T> : IUniTaskSource<!0>, IUniTaskSource
		{
			public NeverPromise(CancellationToken cancellationToken)
			{
				this.cancellationToken = cancellationToken;
				if (this.cancellationToken.CanBeCanceled)
				{
					this.cancellationToken.RegisterWithoutCaptureExecutionContext(UniTask.NeverPromise<T>.cancellationCallback, this);
				}
			}

			private static void CancellationCallback(object state)
			{
				UniTask.NeverPromise<T> neverPromise = (UniTask.NeverPromise<T>)state;
				neverPromise.core.TrySetCanceled(neverPromise.cancellationToken);
			}

			public T GetResult(short token)
			{
				return this.core.GetResult(token);
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

			void IUniTaskSource.GetResult(short token)
			{
				this.core.GetResult(token);
			}

			private static readonly Action<object> cancellationCallback = new Action<object>(UniTask.NeverPromise<T>.CancellationCallback);

			private CancellationToken cancellationToken;

			private UniTaskCompletionSourceCore<T> core;
		}

		private sealed class WaitUntilPromise : IUniTaskSource, IPlayerLoopItem, ITaskPoolNode<UniTask.WaitUntilPromise>
		{
			public ref UniTask.WaitUntilPromise NextNode
			{
				get
				{
					return ref this.nextNode;
				}
			}

			static WaitUntilPromise()
			{
				TaskPool.RegisterSizeGetter(typeof(UniTask.WaitUntilPromise), () => UniTask.WaitUntilPromise.pool.Size);
			}

			private WaitUntilPromise()
			{
			}

			public static IUniTaskSource Create(Func<bool> predicate, PlayerLoopTiming timing, CancellationToken cancellationToken, out short token)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return AutoResetUniTaskCompletionSource.CreateFromCanceled(cancellationToken, out token);
				}
				UniTask.WaitUntilPromise waitUntilPromise;
				if (!UniTask.WaitUntilPromise.pool.TryPop(out waitUntilPromise))
				{
					waitUntilPromise = new UniTask.WaitUntilPromise();
				}
				waitUntilPromise.predicate = predicate;
				waitUntilPromise.cancellationToken = cancellationToken;
				PlayerLoopHelper.AddAction(timing, waitUntilPromise);
				token = waitUntilPromise.core.Version;
				return waitUntilPromise;
			}

			public void GetResult(short token)
			{
				try
				{
					this.core.GetResult(token);
				}
				finally
				{
					this.TryReturn();
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
				if (this.cancellationToken.IsCancellationRequested)
				{
					this.core.TrySetCanceled(this.cancellationToken);
					return false;
				}
				try
				{
					if (!this.predicate())
					{
						return true;
					}
				}
				catch (Exception error)
				{
					this.core.TrySetException(error);
					return false;
				}
				this.core.TrySetResult(null);
				return false;
			}

			private bool TryReturn()
			{
				this.core.Reset();
				this.predicate = null;
				this.cancellationToken = default(CancellationToken);
				return UniTask.WaitUntilPromise.pool.TryPush(this);
			}

			private static TaskPool<UniTask.WaitUntilPromise> pool;

			private UniTask.WaitUntilPromise nextNode;

			private Func<bool> predicate;

			private CancellationToken cancellationToken;

			private UniTaskCompletionSourceCore<object> core;
		}

		private sealed class WaitWhilePromise : IUniTaskSource, IPlayerLoopItem, ITaskPoolNode<UniTask.WaitWhilePromise>
		{
			public ref UniTask.WaitWhilePromise NextNode
			{
				get
				{
					return ref this.nextNode;
				}
			}

			static WaitWhilePromise()
			{
				TaskPool.RegisterSizeGetter(typeof(UniTask.WaitWhilePromise), () => UniTask.WaitWhilePromise.pool.Size);
			}

			private WaitWhilePromise()
			{
			}

			public static IUniTaskSource Create(Func<bool> predicate, PlayerLoopTiming timing, CancellationToken cancellationToken, out short token)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return AutoResetUniTaskCompletionSource.CreateFromCanceled(cancellationToken, out token);
				}
				UniTask.WaitWhilePromise waitWhilePromise;
				if (!UniTask.WaitWhilePromise.pool.TryPop(out waitWhilePromise))
				{
					waitWhilePromise = new UniTask.WaitWhilePromise();
				}
				waitWhilePromise.predicate = predicate;
				waitWhilePromise.cancellationToken = cancellationToken;
				PlayerLoopHelper.AddAction(timing, waitWhilePromise);
				token = waitWhilePromise.core.Version;
				return waitWhilePromise;
			}

			public void GetResult(short token)
			{
				try
				{
					this.core.GetResult(token);
				}
				finally
				{
					this.TryReturn();
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
				if (this.cancellationToken.IsCancellationRequested)
				{
					this.core.TrySetCanceled(this.cancellationToken);
					return false;
				}
				try
				{
					if (this.predicate())
					{
						return true;
					}
				}
				catch (Exception error)
				{
					this.core.TrySetException(error);
					return false;
				}
				this.core.TrySetResult(null);
				return false;
			}

			private bool TryReturn()
			{
				this.core.Reset();
				this.predicate = null;
				this.cancellationToken = default(CancellationToken);
				return UniTask.WaitWhilePromise.pool.TryPush(this);
			}

			private static TaskPool<UniTask.WaitWhilePromise> pool;

			private UniTask.WaitWhilePromise nextNode;

			private Func<bool> predicate;

			private CancellationToken cancellationToken;

			private UniTaskCompletionSourceCore<object> core;
		}

		private sealed class WaitUntilCanceledPromise : IUniTaskSource, IPlayerLoopItem, ITaskPoolNode<UniTask.WaitUntilCanceledPromise>
		{
			public ref UniTask.WaitUntilCanceledPromise NextNode
			{
				get
				{
					return ref this.nextNode;
				}
			}

			static WaitUntilCanceledPromise()
			{
				TaskPool.RegisterSizeGetter(typeof(UniTask.WaitUntilCanceledPromise), () => UniTask.WaitUntilCanceledPromise.pool.Size);
			}

			private WaitUntilCanceledPromise()
			{
			}

			public static IUniTaskSource Create(CancellationToken cancellationToken, PlayerLoopTiming timing, out short token)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return AutoResetUniTaskCompletionSource.CreateFromCanceled(cancellationToken, out token);
				}
				UniTask.WaitUntilCanceledPromise waitUntilCanceledPromise;
				if (!UniTask.WaitUntilCanceledPromise.pool.TryPop(out waitUntilCanceledPromise))
				{
					waitUntilCanceledPromise = new UniTask.WaitUntilCanceledPromise();
				}
				waitUntilCanceledPromise.cancellationToken = cancellationToken;
				PlayerLoopHelper.AddAction(timing, waitUntilCanceledPromise);
				token = waitUntilCanceledPromise.core.Version;
				return waitUntilCanceledPromise;
			}

			public void GetResult(short token)
			{
				try
				{
					this.core.GetResult(token);
				}
				finally
				{
					this.TryReturn();
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
				if (this.cancellationToken.IsCancellationRequested)
				{
					this.core.TrySetResult(null);
					return false;
				}
				return true;
			}

			private bool TryReturn()
			{
				this.core.Reset();
				this.cancellationToken = default(CancellationToken);
				return UniTask.WaitUntilCanceledPromise.pool.TryPush(this);
			}

			private static TaskPool<UniTask.WaitUntilCanceledPromise> pool;

			private UniTask.WaitUntilCanceledPromise nextNode;

			private CancellationToken cancellationToken;

			private UniTaskCompletionSourceCore<object> core;
		}

		private sealed class WaitUntilValueChangedUnityObjectPromise<T, U> : IUniTaskSource<U>, IUniTaskSource, IPlayerLoopItem, ITaskPoolNode<UniTask.WaitUntilValueChangedUnityObjectPromise<T, U>>
		{
			public ref UniTask.WaitUntilValueChangedUnityObjectPromise<T, U> NextNode
			{
				get
				{
					return ref this.nextNode;
				}
			}

			static WaitUntilValueChangedUnityObjectPromise()
			{
				TaskPool.RegisterSizeGetter(typeof(UniTask.WaitUntilValueChangedUnityObjectPromise<T, U>), () => UniTask.WaitUntilValueChangedUnityObjectPromise<T, U>.pool.Size);
			}

			private WaitUntilValueChangedUnityObjectPromise()
			{
			}

			public static IUniTaskSource<U> Create(T target, Func<T, U> monitorFunction, IEqualityComparer<U> equalityComparer, PlayerLoopTiming timing, CancellationToken cancellationToken, out short token)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return AutoResetUniTaskCompletionSource<U>.CreateFromCanceled(cancellationToken, out token);
				}
				UniTask.WaitUntilValueChangedUnityObjectPromise<T, U> waitUntilValueChangedUnityObjectPromise;
				if (!UniTask.WaitUntilValueChangedUnityObjectPromise<T, U>.pool.TryPop(out waitUntilValueChangedUnityObjectPromise))
				{
					waitUntilValueChangedUnityObjectPromise = new UniTask.WaitUntilValueChangedUnityObjectPromise<T, U>();
				}
				waitUntilValueChangedUnityObjectPromise.target = target;
				waitUntilValueChangedUnityObjectPromise.targetAsUnityObject = (target as Object);
				waitUntilValueChangedUnityObjectPromise.monitorFunction = monitorFunction;
				waitUntilValueChangedUnityObjectPromise.currentValue = monitorFunction(target);
				waitUntilValueChangedUnityObjectPromise.equalityComparer = (equalityComparer ?? UnityEqualityComparer.GetDefault<U>());
				waitUntilValueChangedUnityObjectPromise.cancellationToken = cancellationToken;
				PlayerLoopHelper.AddAction(timing, waitUntilValueChangedUnityObjectPromise);
				token = waitUntilValueChangedUnityObjectPromise.core.Version;
				return waitUntilValueChangedUnityObjectPromise;
			}

			public U GetResult(short token)
			{
				U result;
				try
				{
					result = this.core.GetResult(token);
				}
				finally
				{
					this.TryReturn();
				}
				return result;
			}

			void IUniTaskSource.GetResult(short token)
			{
				this.GetResult(token);
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
				if (this.cancellationToken.IsCancellationRequested || this.targetAsUnityObject == null)
				{
					this.core.TrySetCanceled(this.cancellationToken);
					return false;
				}
				U u = default(U);
				try
				{
					u = this.monitorFunction(this.target);
					if (this.equalityComparer.Equals(this.currentValue, u))
					{
						return true;
					}
				}
				catch (Exception error)
				{
					this.core.TrySetException(error);
					return false;
				}
				this.core.TrySetResult(u);
				return false;
			}

			private bool TryReturn()
			{
				this.core.Reset();
				this.target = default(T);
				this.currentValue = default(U);
				this.monitorFunction = null;
				this.equalityComparer = null;
				this.cancellationToken = default(CancellationToken);
				return UniTask.WaitUntilValueChangedUnityObjectPromise<T, U>.pool.TryPush(this);
			}

			private static TaskPool<UniTask.WaitUntilValueChangedUnityObjectPromise<T, U>> pool;

			private UniTask.WaitUntilValueChangedUnityObjectPromise<T, U> nextNode;

			private T target;

			private Object targetAsUnityObject;

			private U currentValue;

			private Func<T, U> monitorFunction;

			private IEqualityComparer<U> equalityComparer;

			private CancellationToken cancellationToken;

			private UniTaskCompletionSourceCore<U> core;
		}

		private sealed class WaitUntilValueChangedStandardObjectPromise<T, U> : IUniTaskSource<U>, IUniTaskSource, IPlayerLoopItem, ITaskPoolNode<UniTask.WaitUntilValueChangedStandardObjectPromise<T, U>> where T : class
		{
			public ref UniTask.WaitUntilValueChangedStandardObjectPromise<T, U> NextNode
			{
				get
				{
					return ref this.nextNode;
				}
			}

			static WaitUntilValueChangedStandardObjectPromise()
			{
				TaskPool.RegisterSizeGetter(typeof(UniTask.WaitUntilValueChangedStandardObjectPromise<T, U>), () => UniTask.WaitUntilValueChangedStandardObjectPromise<T, U>.pool.Size);
			}

			private WaitUntilValueChangedStandardObjectPromise()
			{
			}

			public static IUniTaskSource<U> Create(T target, Func<T, U> monitorFunction, IEqualityComparer<U> equalityComparer, PlayerLoopTiming timing, CancellationToken cancellationToken, out short token)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return AutoResetUniTaskCompletionSource<U>.CreateFromCanceled(cancellationToken, out token);
				}
				UniTask.WaitUntilValueChangedStandardObjectPromise<T, U> waitUntilValueChangedStandardObjectPromise;
				if (!UniTask.WaitUntilValueChangedStandardObjectPromise<T, U>.pool.TryPop(out waitUntilValueChangedStandardObjectPromise))
				{
					waitUntilValueChangedStandardObjectPromise = new UniTask.WaitUntilValueChangedStandardObjectPromise<T, U>();
				}
				waitUntilValueChangedStandardObjectPromise.target = new WeakReference<T>(target, false);
				waitUntilValueChangedStandardObjectPromise.monitorFunction = monitorFunction;
				waitUntilValueChangedStandardObjectPromise.currentValue = monitorFunction(target);
				waitUntilValueChangedStandardObjectPromise.equalityComparer = (equalityComparer ?? UnityEqualityComparer.GetDefault<U>());
				waitUntilValueChangedStandardObjectPromise.cancellationToken = cancellationToken;
				PlayerLoopHelper.AddAction(timing, waitUntilValueChangedStandardObjectPromise);
				token = waitUntilValueChangedStandardObjectPromise.core.Version;
				return waitUntilValueChangedStandardObjectPromise;
			}

			public U GetResult(short token)
			{
				U result;
				try
				{
					result = this.core.GetResult(token);
				}
				finally
				{
					this.TryReturn();
				}
				return result;
			}

			void IUniTaskSource.GetResult(short token)
			{
				this.GetResult(token);
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
				T arg;
				if (this.cancellationToken.IsCancellationRequested || !this.target.TryGetTarget(out arg))
				{
					this.core.TrySetCanceled(this.cancellationToken);
					return false;
				}
				U u = default(U);
				try
				{
					u = this.monitorFunction(arg);
					if (this.equalityComparer.Equals(this.currentValue, u))
					{
						return true;
					}
				}
				catch (Exception error)
				{
					this.core.TrySetException(error);
					return false;
				}
				this.core.TrySetResult(u);
				return false;
			}

			private bool TryReturn()
			{
				this.core.Reset();
				this.target = null;
				this.currentValue = default(U);
				this.monitorFunction = null;
				this.equalityComparer = null;
				this.cancellationToken = default(CancellationToken);
				return UniTask.WaitUntilValueChangedStandardObjectPromise<T, U>.pool.TryPush(this);
			}

			private static TaskPool<UniTask.WaitUntilValueChangedStandardObjectPromise<T, U>> pool;

			private UniTask.WaitUntilValueChangedStandardObjectPromise<T, U> nextNode;

			private WeakReference<T> target;

			private U currentValue;

			private Func<T, U> monitorFunction;

			private IEqualityComparer<U> equalityComparer;

			private CancellationToken cancellationToken;

			private UniTaskCompletionSourceCore<U> core;
		}

		private sealed class WhenAllPromise<T> : IUniTaskSource<T[]>, IUniTaskSource
		{
			public WhenAllPromise(UniTask<T>[] tasks, int tasksLength)
			{
				this.completeCount = 0;
				if (tasksLength == 0)
				{
					this.result = Array.Empty<T>();
					this.core.TrySetResult(this.result);
					return;
				}
				this.result = new T[tasksLength];
				int i = 0;
				while (i < tasksLength)
				{
					UniTask<T>.Awaiter awaiter;
					try
					{
						awaiter = tasks[i].GetAwaiter();
					}
					catch (Exception error)
					{
						this.core.TrySetException(error);
						goto IL_A0;
					}
					goto IL_5E;
					IL_A0:
					i++;
					continue;
					IL_5E:
					if (awaiter.IsCompleted)
					{
						UniTask.WhenAllPromise<T>.TryInvokeContinuation(this, awaiter, i);
						goto IL_A0;
					}
					awaiter.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T>, UniTask<T>.Awaiter, int> stateTuple = (StateTuple<UniTask.WhenAllPromise<T>, UniTask<T>.Awaiter, int>)state)
						{
							UniTask.WhenAllPromise<T>.TryInvokeContinuation(stateTuple.Item1, stateTuple.Item2, stateTuple.Item3);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T>, UniTask<T>.Awaiter, int>(this, awaiter, i));
					goto IL_A0;
				}
			}

			private static void TryInvokeContinuation(UniTask.WhenAllPromise<T> self, in UniTask<T>.Awaiter awaiter, int i)
			{
				try
				{
					self.result[i] = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completeCount) == self.result.Length)
				{
					self.core.TrySetResult(self.result);
				}
			}

			public T[] GetResult(short token)
			{
				GC.SuppressFinalize(this);
				return this.core.GetResult(token);
			}

			void IUniTaskSource.GetResult(short token)
			{
				this.GetResult(token);
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

			private T[] result;

			private int completeCount;

			private UniTaskCompletionSourceCore<T[]> core;
		}

		private sealed class WhenAllPromise : IUniTaskSource
		{
			public WhenAllPromise(UniTask[] tasks, int tasksLength)
			{
				this.tasksLength = tasksLength;
				this.completeCount = 0;
				if (tasksLength == 0)
				{
					this.core.TrySetResult(AsyncUnit.Default);
					return;
				}
				int i = 0;
				while (i < tasksLength)
				{
					UniTask.Awaiter awaiter;
					try
					{
						awaiter = tasks[i].GetAwaiter();
					}
					catch (Exception error)
					{
						this.core.TrySetException(error);
						goto IL_8D;
					}
					goto IL_4D;
					IL_8D:
					i++;
					continue;
					IL_4D:
					if (awaiter.IsCompleted)
					{
						UniTask.WhenAllPromise.TryInvokeContinuation(this, awaiter);
						goto IL_8D;
					}
					awaiter.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise, UniTask.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise, UniTask.Awaiter>)state)
						{
							UniTask.WhenAllPromise.TryInvokeContinuation(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise, UniTask.Awaiter>(this, awaiter));
					goto IL_8D;
				}
			}

			private static void TryInvokeContinuation(UniTask.WhenAllPromise self, in UniTask.Awaiter awaiter)
			{
				try
				{
					awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completeCount) == self.tasksLength)
				{
					self.core.TrySetResult(AsyncUnit.Default);
				}
			}

			public void GetResult(short token)
			{
				GC.SuppressFinalize(this);
				this.core.GetResult(token);
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

			private int completeCount;

			private int tasksLength;

			private UniTaskCompletionSourceCore<AsyncUnit> core;
		}

		private sealed class WhenAllPromise<T1, T2> : IUniTaskSource<ValueTuple<T1, T2>>, IUniTaskSource
		{
			public WhenAllPromise(UniTask<T1> task1, UniTask<T2> task2)
			{
				this.completedCount = 0;
				UniTask<T1>.Awaiter awaiter = task1.GetAwaiter();
				if (awaiter.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2>.TryInvokeContinuationT1(this, awaiter);
				}
				else
				{
					awaiter.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2>, UniTask<T1>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2>, UniTask<T1>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2>.TryInvokeContinuationT1(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2>, UniTask<T1>.Awaiter>(this, awaiter));
				}
				UniTask<T2>.Awaiter awaiter2 = task2.GetAwaiter();
				if (awaiter2.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2>.TryInvokeContinuationT2(this, awaiter2);
					return;
				}
				awaiter2.SourceOnCompleted(delegate(object state)
				{
					using (StateTuple<UniTask.WhenAllPromise<T1, T2>, UniTask<T2>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2>, UniTask<T2>.Awaiter>)state)
					{
						UniTask.WhenAllPromise<T1, T2>.TryInvokeContinuationT2(stateTuple.Item1, stateTuple.Item2);
					}
				}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2>, UniTask<T2>.Awaiter>(this, awaiter2));
			}

			private static void TryInvokeContinuationT1(UniTask.WhenAllPromise<T1, T2> self, in UniTask<T1>.Awaiter awaiter)
			{
				try
				{
					self.t1 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 2)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2>(self.t1, self.t2));
				}
			}

			private static void TryInvokeContinuationT2(UniTask.WhenAllPromise<T1, T2> self, in UniTask<T2>.Awaiter awaiter)
			{
				try
				{
					self.t2 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 2)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2>(self.t1, self.t2));
				}
			}

			public ValueTuple<T1, T2> GetResult(short token)
			{
				GC.SuppressFinalize(this);
				return this.core.GetResult(token);
			}

			void IUniTaskSource.GetResult(short token)
			{
				this.GetResult(token);
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

			private T1 t1;

			private T2 t2;

			private int completedCount;

			private UniTaskCompletionSourceCore<ValueTuple<T1, T2>> core;
		}

		private sealed class WhenAllPromise<T1, T2, T3> : IUniTaskSource<ValueTuple<T1, T2, T3>>, IUniTaskSource
		{
			public WhenAllPromise(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3)
			{
				this.completedCount = 0;
				UniTask<T1>.Awaiter awaiter = task1.GetAwaiter();
				if (awaiter.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3>.TryInvokeContinuationT1(this, awaiter);
				}
				else
				{
					awaiter.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3>, UniTask<T1>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3>, UniTask<T1>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3>.TryInvokeContinuationT1(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3>, UniTask<T1>.Awaiter>(this, awaiter));
				}
				UniTask<T2>.Awaiter awaiter2 = task2.GetAwaiter();
				if (awaiter2.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3>.TryInvokeContinuationT2(this, awaiter2);
				}
				else
				{
					awaiter2.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3>, UniTask<T2>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3>, UniTask<T2>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3>.TryInvokeContinuationT2(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3>, UniTask<T2>.Awaiter>(this, awaiter2));
				}
				UniTask<T3>.Awaiter awaiter3 = task3.GetAwaiter();
				if (awaiter3.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3>.TryInvokeContinuationT3(this, awaiter3);
					return;
				}
				awaiter3.SourceOnCompleted(delegate(object state)
				{
					using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3>, UniTask<T3>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3>, UniTask<T3>.Awaiter>)state)
					{
						UniTask.WhenAllPromise<T1, T2, T3>.TryInvokeContinuationT3(stateTuple.Item1, stateTuple.Item2);
					}
				}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3>, UniTask<T3>.Awaiter>(this, awaiter3));
			}

			private static void TryInvokeContinuationT1(UniTask.WhenAllPromise<T1, T2, T3> self, in UniTask<T1>.Awaiter awaiter)
			{
				try
				{
					self.t1 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 3)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3>(self.t1, self.t2, self.t3));
				}
			}

			private static void TryInvokeContinuationT2(UniTask.WhenAllPromise<T1, T2, T3> self, in UniTask<T2>.Awaiter awaiter)
			{
				try
				{
					self.t2 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 3)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3>(self.t1, self.t2, self.t3));
				}
			}

			private static void TryInvokeContinuationT3(UniTask.WhenAllPromise<T1, T2, T3> self, in UniTask<T3>.Awaiter awaiter)
			{
				try
				{
					self.t3 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 3)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3>(self.t1, self.t2, self.t3));
				}
			}

			public ValueTuple<T1, T2, T3> GetResult(short token)
			{
				GC.SuppressFinalize(this);
				return this.core.GetResult(token);
			}

			void IUniTaskSource.GetResult(short token)
			{
				this.GetResult(token);
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

			private T1 t1;

			private T2 t2;

			private T3 t3;

			private int completedCount;

			private UniTaskCompletionSourceCore<ValueTuple<T1, T2, T3>> core;
		}

		private sealed class WhenAllPromise<T1, T2, T3, T4> : IUniTaskSource<ValueTuple<T1, T2, T3, T4>>, IUniTaskSource
		{
			public WhenAllPromise(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4)
			{
				this.completedCount = 0;
				UniTask<T1>.Awaiter awaiter = task1.GetAwaiter();
				if (awaiter.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4>.TryInvokeContinuationT1(this, awaiter);
				}
				else
				{
					awaiter.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4>, UniTask<T1>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4>, UniTask<T1>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4>.TryInvokeContinuationT1(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4>, UniTask<T1>.Awaiter>(this, awaiter));
				}
				UniTask<T2>.Awaiter awaiter2 = task2.GetAwaiter();
				if (awaiter2.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4>.TryInvokeContinuationT2(this, awaiter2);
				}
				else
				{
					awaiter2.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4>, UniTask<T2>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4>, UniTask<T2>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4>.TryInvokeContinuationT2(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4>, UniTask<T2>.Awaiter>(this, awaiter2));
				}
				UniTask<T3>.Awaiter awaiter3 = task3.GetAwaiter();
				if (awaiter3.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4>.TryInvokeContinuationT3(this, awaiter3);
				}
				else
				{
					awaiter3.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4>, UniTask<T3>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4>, UniTask<T3>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4>.TryInvokeContinuationT3(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4>, UniTask<T3>.Awaiter>(this, awaiter3));
				}
				UniTask<T4>.Awaiter awaiter4 = task4.GetAwaiter();
				if (awaiter4.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4>.TryInvokeContinuationT4(this, awaiter4);
					return;
				}
				awaiter4.SourceOnCompleted(delegate(object state)
				{
					using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4>, UniTask<T4>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4>, UniTask<T4>.Awaiter>)state)
					{
						UniTask.WhenAllPromise<T1, T2, T3, T4>.TryInvokeContinuationT4(stateTuple.Item1, stateTuple.Item2);
					}
				}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4>, UniTask<T4>.Awaiter>(this, awaiter4));
			}

			private static void TryInvokeContinuationT1(UniTask.WhenAllPromise<T1, T2, T3, T4> self, in UniTask<T1>.Awaiter awaiter)
			{
				try
				{
					self.t1 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 4)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4>(self.t1, self.t2, self.t3, self.t4));
				}
			}

			private static void TryInvokeContinuationT2(UniTask.WhenAllPromise<T1, T2, T3, T4> self, in UniTask<T2>.Awaiter awaiter)
			{
				try
				{
					self.t2 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 4)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4>(self.t1, self.t2, self.t3, self.t4));
				}
			}

			private static void TryInvokeContinuationT3(UniTask.WhenAllPromise<T1, T2, T3, T4> self, in UniTask<T3>.Awaiter awaiter)
			{
				try
				{
					self.t3 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 4)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4>(self.t1, self.t2, self.t3, self.t4));
				}
			}

			private static void TryInvokeContinuationT4(UniTask.WhenAllPromise<T1, T2, T3, T4> self, in UniTask<T4>.Awaiter awaiter)
			{
				try
				{
					self.t4 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 4)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4>(self.t1, self.t2, self.t3, self.t4));
				}
			}

			public ValueTuple<T1, T2, T3, T4> GetResult(short token)
			{
				GC.SuppressFinalize(this);
				return this.core.GetResult(token);
			}

			void IUniTaskSource.GetResult(short token)
			{
				this.GetResult(token);
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

			private T1 t1;

			private T2 t2;

			private T3 t3;

			private T4 t4;

			private int completedCount;

			private UniTaskCompletionSourceCore<ValueTuple<T1, T2, T3, T4>> core;
		}

		private sealed class WhenAllPromise<T1, T2, T3, T4, T5> : IUniTaskSource<ValueTuple<T1, T2, T3, T4, T5>>, IUniTaskSource
		{
			public WhenAllPromise(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4, UniTask<T5> task5)
			{
				this.completedCount = 0;
				UniTask<T1>.Awaiter awaiter = task1.GetAwaiter();
				if (awaiter.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5>.TryInvokeContinuationT1(this, awaiter);
				}
				else
				{
					awaiter.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5>, UniTask<T1>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5>, UniTask<T1>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5>.TryInvokeContinuationT1(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5>, UniTask<T1>.Awaiter>(this, awaiter));
				}
				UniTask<T2>.Awaiter awaiter2 = task2.GetAwaiter();
				if (awaiter2.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5>.TryInvokeContinuationT2(this, awaiter2);
				}
				else
				{
					awaiter2.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5>, UniTask<T2>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5>, UniTask<T2>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5>.TryInvokeContinuationT2(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5>, UniTask<T2>.Awaiter>(this, awaiter2));
				}
				UniTask<T3>.Awaiter awaiter3 = task3.GetAwaiter();
				if (awaiter3.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5>.TryInvokeContinuationT3(this, awaiter3);
				}
				else
				{
					awaiter3.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5>, UniTask<T3>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5>, UniTask<T3>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5>.TryInvokeContinuationT3(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5>, UniTask<T3>.Awaiter>(this, awaiter3));
				}
				UniTask<T4>.Awaiter awaiter4 = task4.GetAwaiter();
				if (awaiter4.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5>.TryInvokeContinuationT4(this, awaiter4);
				}
				else
				{
					awaiter4.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5>, UniTask<T4>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5>, UniTask<T4>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5>.TryInvokeContinuationT4(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5>, UniTask<T4>.Awaiter>(this, awaiter4));
				}
				UniTask<T5>.Awaiter awaiter5 = task5.GetAwaiter();
				if (awaiter5.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5>.TryInvokeContinuationT5(this, awaiter5);
					return;
				}
				awaiter5.SourceOnCompleted(delegate(object state)
				{
					using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5>, UniTask<T5>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5>, UniTask<T5>.Awaiter>)state)
					{
						UniTask.WhenAllPromise<T1, T2, T3, T4, T5>.TryInvokeContinuationT5(stateTuple.Item1, stateTuple.Item2);
					}
				}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5>, UniTask<T5>.Awaiter>(this, awaiter5));
			}

			private static void TryInvokeContinuationT1(UniTask.WhenAllPromise<T1, T2, T3, T4, T5> self, in UniTask<T1>.Awaiter awaiter)
			{
				try
				{
					self.t1 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 5)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5>(self.t1, self.t2, self.t3, self.t4, self.t5));
				}
			}

			private static void TryInvokeContinuationT2(UniTask.WhenAllPromise<T1, T2, T3, T4, T5> self, in UniTask<T2>.Awaiter awaiter)
			{
				try
				{
					self.t2 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 5)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5>(self.t1, self.t2, self.t3, self.t4, self.t5));
				}
			}

			private static void TryInvokeContinuationT3(UniTask.WhenAllPromise<T1, T2, T3, T4, T5> self, in UniTask<T3>.Awaiter awaiter)
			{
				try
				{
					self.t3 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 5)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5>(self.t1, self.t2, self.t3, self.t4, self.t5));
				}
			}

			private static void TryInvokeContinuationT4(UniTask.WhenAllPromise<T1, T2, T3, T4, T5> self, in UniTask<T4>.Awaiter awaiter)
			{
				try
				{
					self.t4 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 5)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5>(self.t1, self.t2, self.t3, self.t4, self.t5));
				}
			}

			private static void TryInvokeContinuationT5(UniTask.WhenAllPromise<T1, T2, T3, T4, T5> self, in UniTask<T5>.Awaiter awaiter)
			{
				try
				{
					self.t5 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 5)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5>(self.t1, self.t2, self.t3, self.t4, self.t5));
				}
			}

			public ValueTuple<T1, T2, T3, T4, T5> GetResult(short token)
			{
				GC.SuppressFinalize(this);
				return this.core.GetResult(token);
			}

			void IUniTaskSource.GetResult(short token)
			{
				this.GetResult(token);
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

			private T1 t1;

			private T2 t2;

			private T3 t3;

			private T4 t4;

			private T5 t5;

			private int completedCount;

			private UniTaskCompletionSourceCore<ValueTuple<T1, T2, T3, T4, T5>> core;
		}

		private sealed class WhenAllPromise<T1, T2, T3, T4, T5, T6> : IUniTaskSource<ValueTuple<T1, T2, T3, T4, T5, T6>>, IUniTaskSource
		{
			public WhenAllPromise(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4, UniTask<T5> task5, UniTask<T6> task6)
			{
				this.completedCount = 0;
				UniTask<T1>.Awaiter awaiter = task1.GetAwaiter();
				if (awaiter.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6>.TryInvokeContinuationT1(this, awaiter);
				}
				else
				{
					awaiter.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6>, UniTask<T1>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6>, UniTask<T1>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6>.TryInvokeContinuationT1(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6>, UniTask<T1>.Awaiter>(this, awaiter));
				}
				UniTask<T2>.Awaiter awaiter2 = task2.GetAwaiter();
				if (awaiter2.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6>.TryInvokeContinuationT2(this, awaiter2);
				}
				else
				{
					awaiter2.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6>, UniTask<T2>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6>, UniTask<T2>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6>.TryInvokeContinuationT2(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6>, UniTask<T2>.Awaiter>(this, awaiter2));
				}
				UniTask<T3>.Awaiter awaiter3 = task3.GetAwaiter();
				if (awaiter3.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6>.TryInvokeContinuationT3(this, awaiter3);
				}
				else
				{
					awaiter3.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6>, UniTask<T3>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6>, UniTask<T3>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6>.TryInvokeContinuationT3(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6>, UniTask<T3>.Awaiter>(this, awaiter3));
				}
				UniTask<T4>.Awaiter awaiter4 = task4.GetAwaiter();
				if (awaiter4.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6>.TryInvokeContinuationT4(this, awaiter4);
				}
				else
				{
					awaiter4.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6>, UniTask<T4>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6>, UniTask<T4>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6>.TryInvokeContinuationT4(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6>, UniTask<T4>.Awaiter>(this, awaiter4));
				}
				UniTask<T5>.Awaiter awaiter5 = task5.GetAwaiter();
				if (awaiter5.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6>.TryInvokeContinuationT5(this, awaiter5);
				}
				else
				{
					awaiter5.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6>, UniTask<T5>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6>, UniTask<T5>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6>.TryInvokeContinuationT5(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6>, UniTask<T5>.Awaiter>(this, awaiter5));
				}
				UniTask<T6>.Awaiter awaiter6 = task6.GetAwaiter();
				if (awaiter6.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6>.TryInvokeContinuationT6(this, awaiter6);
					return;
				}
				awaiter6.SourceOnCompleted(delegate(object state)
				{
					using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6>, UniTask<T6>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6>, UniTask<T6>.Awaiter>)state)
					{
						UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6>.TryInvokeContinuationT6(stateTuple.Item1, stateTuple.Item2);
					}
				}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6>, UniTask<T6>.Awaiter>(this, awaiter6));
			}

			private static void TryInvokeContinuationT1(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6> self, in UniTask<T1>.Awaiter awaiter)
			{
				try
				{
					self.t1 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 6)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6));
				}
			}

			private static void TryInvokeContinuationT2(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6> self, in UniTask<T2>.Awaiter awaiter)
			{
				try
				{
					self.t2 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 6)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6));
				}
			}

			private static void TryInvokeContinuationT3(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6> self, in UniTask<T3>.Awaiter awaiter)
			{
				try
				{
					self.t3 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 6)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6));
				}
			}

			private static void TryInvokeContinuationT4(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6> self, in UniTask<T4>.Awaiter awaiter)
			{
				try
				{
					self.t4 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 6)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6));
				}
			}

			private static void TryInvokeContinuationT5(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6> self, in UniTask<T5>.Awaiter awaiter)
			{
				try
				{
					self.t5 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 6)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6));
				}
			}

			private static void TryInvokeContinuationT6(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6> self, in UniTask<T6>.Awaiter awaiter)
			{
				try
				{
					self.t6 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 6)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6));
				}
			}

			public ValueTuple<T1, T2, T3, T4, T5, T6> GetResult(short token)
			{
				GC.SuppressFinalize(this);
				return this.core.GetResult(token);
			}

			void IUniTaskSource.GetResult(short token)
			{
				this.GetResult(token);
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

			private T1 t1;

			private T2 t2;

			private T3 t3;

			private T4 t4;

			private T5 t5;

			private T6 t6;

			private int completedCount;

			private UniTaskCompletionSourceCore<ValueTuple<T1, T2, T3, T4, T5, T6>> core;
		}

		private sealed class WhenAllPromise<T1, T2, T3, T4, T5, T6, T7> : IUniTaskSource<ValueTuple<T1, T2, T3, T4, T5, T6, T7>>, IUniTaskSource
		{
			public WhenAllPromise(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4, UniTask<T5> task5, UniTask<T6> task6, UniTask<T7> task7)
			{
				this.completedCount = 0;
				UniTask<T1>.Awaiter awaiter = task1.GetAwaiter();
				if (awaiter.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7>.TryInvokeContinuationT1(this, awaiter);
				}
				else
				{
					awaiter.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7>, UniTask<T1>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7>, UniTask<T1>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7>.TryInvokeContinuationT1(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7>, UniTask<T1>.Awaiter>(this, awaiter));
				}
				UniTask<T2>.Awaiter awaiter2 = task2.GetAwaiter();
				if (awaiter2.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7>.TryInvokeContinuationT2(this, awaiter2);
				}
				else
				{
					awaiter2.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7>, UniTask<T2>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7>, UniTask<T2>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7>.TryInvokeContinuationT2(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7>, UniTask<T2>.Awaiter>(this, awaiter2));
				}
				UniTask<T3>.Awaiter awaiter3 = task3.GetAwaiter();
				if (awaiter3.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7>.TryInvokeContinuationT3(this, awaiter3);
				}
				else
				{
					awaiter3.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7>, UniTask<T3>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7>, UniTask<T3>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7>.TryInvokeContinuationT3(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7>, UniTask<T3>.Awaiter>(this, awaiter3));
				}
				UniTask<T4>.Awaiter awaiter4 = task4.GetAwaiter();
				if (awaiter4.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7>.TryInvokeContinuationT4(this, awaiter4);
				}
				else
				{
					awaiter4.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7>, UniTask<T4>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7>, UniTask<T4>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7>.TryInvokeContinuationT4(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7>, UniTask<T4>.Awaiter>(this, awaiter4));
				}
				UniTask<T5>.Awaiter awaiter5 = task5.GetAwaiter();
				if (awaiter5.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7>.TryInvokeContinuationT5(this, awaiter5);
				}
				else
				{
					awaiter5.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7>, UniTask<T5>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7>, UniTask<T5>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7>.TryInvokeContinuationT5(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7>, UniTask<T5>.Awaiter>(this, awaiter5));
				}
				UniTask<T6>.Awaiter awaiter6 = task6.GetAwaiter();
				if (awaiter6.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7>.TryInvokeContinuationT6(this, awaiter6);
				}
				else
				{
					awaiter6.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7>, UniTask<T6>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7>, UniTask<T6>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7>.TryInvokeContinuationT6(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7>, UniTask<T6>.Awaiter>(this, awaiter6));
				}
				UniTask<T7>.Awaiter awaiter7 = task7.GetAwaiter();
				if (awaiter7.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7>.TryInvokeContinuationT7(this, awaiter7);
					return;
				}
				awaiter7.SourceOnCompleted(delegate(object state)
				{
					using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7>, UniTask<T7>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7>, UniTask<T7>.Awaiter>)state)
					{
						UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7>.TryInvokeContinuationT7(stateTuple.Item1, stateTuple.Item2);
					}
				}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7>, UniTask<T7>.Awaiter>(this, awaiter7));
			}

			private static void TryInvokeContinuationT1(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7> self, in UniTask<T1>.Awaiter awaiter)
			{
				try
				{
					self.t1 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 7)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7));
				}
			}

			private static void TryInvokeContinuationT2(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7> self, in UniTask<T2>.Awaiter awaiter)
			{
				try
				{
					self.t2 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 7)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7));
				}
			}

			private static void TryInvokeContinuationT3(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7> self, in UniTask<T3>.Awaiter awaiter)
			{
				try
				{
					self.t3 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 7)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7));
				}
			}

			private static void TryInvokeContinuationT4(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7> self, in UniTask<T4>.Awaiter awaiter)
			{
				try
				{
					self.t4 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 7)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7));
				}
			}

			private static void TryInvokeContinuationT5(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7> self, in UniTask<T5>.Awaiter awaiter)
			{
				try
				{
					self.t5 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 7)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7));
				}
			}

			private static void TryInvokeContinuationT6(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7> self, in UniTask<T6>.Awaiter awaiter)
			{
				try
				{
					self.t6 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 7)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7));
				}
			}

			private static void TryInvokeContinuationT7(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7> self, in UniTask<T7>.Awaiter awaiter)
			{
				try
				{
					self.t7 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 7)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7));
				}
			}

			public ValueTuple<T1, T2, T3, T4, T5, T6, T7> GetResult(short token)
			{
				GC.SuppressFinalize(this);
				return this.core.GetResult(token);
			}

			void IUniTaskSource.GetResult(short token)
			{
				this.GetResult(token);
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

			private T1 t1;

			private T2 t2;

			private T3 t3;

			private T4 t4;

			private T5 t5;

			private T6 t6;

			private T7 t7;

			private int completedCount;

			private UniTaskCompletionSourceCore<ValueTuple<T1, T2, T3, T4, T5, T6, T7>> core;
		}

		private sealed class WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8> : IUniTaskSource<ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8>>>, IUniTaskSource
		{
			public WhenAllPromise(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4, UniTask<T5> task5, UniTask<T6> task6, UniTask<T7> task7, UniTask<T8> task8)
			{
				this.completedCount = 0;
				UniTask<T1>.Awaiter awaiter = task1.GetAwaiter();
				if (awaiter.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8>.TryInvokeContinuationT1(this, awaiter);
				}
				else
				{
					awaiter.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8>, UniTask<T1>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8>, UniTask<T1>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8>.TryInvokeContinuationT1(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8>, UniTask<T1>.Awaiter>(this, awaiter));
				}
				UniTask<T2>.Awaiter awaiter2 = task2.GetAwaiter();
				if (awaiter2.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8>.TryInvokeContinuationT2(this, awaiter2);
				}
				else
				{
					awaiter2.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8>, UniTask<T2>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8>, UniTask<T2>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8>.TryInvokeContinuationT2(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8>, UniTask<T2>.Awaiter>(this, awaiter2));
				}
				UniTask<T3>.Awaiter awaiter3 = task3.GetAwaiter();
				if (awaiter3.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8>.TryInvokeContinuationT3(this, awaiter3);
				}
				else
				{
					awaiter3.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8>, UniTask<T3>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8>, UniTask<T3>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8>.TryInvokeContinuationT3(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8>, UniTask<T3>.Awaiter>(this, awaiter3));
				}
				UniTask<T4>.Awaiter awaiter4 = task4.GetAwaiter();
				if (awaiter4.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8>.TryInvokeContinuationT4(this, awaiter4);
				}
				else
				{
					awaiter4.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8>, UniTask<T4>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8>, UniTask<T4>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8>.TryInvokeContinuationT4(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8>, UniTask<T4>.Awaiter>(this, awaiter4));
				}
				UniTask<T5>.Awaiter awaiter5 = task5.GetAwaiter();
				if (awaiter5.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8>.TryInvokeContinuationT5(this, awaiter5);
				}
				else
				{
					awaiter5.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8>, UniTask<T5>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8>, UniTask<T5>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8>.TryInvokeContinuationT5(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8>, UniTask<T5>.Awaiter>(this, awaiter5));
				}
				UniTask<T6>.Awaiter awaiter6 = task6.GetAwaiter();
				if (awaiter6.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8>.TryInvokeContinuationT6(this, awaiter6);
				}
				else
				{
					awaiter6.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8>, UniTask<T6>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8>, UniTask<T6>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8>.TryInvokeContinuationT6(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8>, UniTask<T6>.Awaiter>(this, awaiter6));
				}
				UniTask<T7>.Awaiter awaiter7 = task7.GetAwaiter();
				if (awaiter7.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8>.TryInvokeContinuationT7(this, awaiter7);
				}
				else
				{
					awaiter7.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8>, UniTask<T7>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8>, UniTask<T7>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8>.TryInvokeContinuationT7(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8>, UniTask<T7>.Awaiter>(this, awaiter7));
				}
				UniTask<T8>.Awaiter awaiter8 = task8.GetAwaiter();
				if (awaiter8.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8>.TryInvokeContinuationT8(this, awaiter8);
					return;
				}
				awaiter8.SourceOnCompleted(delegate(object state)
				{
					using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8>, UniTask<T8>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8>, UniTask<T8>.Awaiter>)state)
					{
						UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8>.TryInvokeContinuationT8(stateTuple.Item1, stateTuple.Item2);
					}
				}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8>, UniTask<T8>.Awaiter>(this, awaiter8));
			}

			private static void TryInvokeContinuationT1(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8> self, in UniTask<T1>.Awaiter awaiter)
			{
				try
				{
					self.t1 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 8)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8>(self.t8)));
				}
			}

			private static void TryInvokeContinuationT2(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8> self, in UniTask<T2>.Awaiter awaiter)
			{
				try
				{
					self.t2 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 8)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8>(self.t8)));
				}
			}

			private static void TryInvokeContinuationT3(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8> self, in UniTask<T3>.Awaiter awaiter)
			{
				try
				{
					self.t3 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 8)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8>(self.t8)));
				}
			}

			private static void TryInvokeContinuationT4(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8> self, in UniTask<T4>.Awaiter awaiter)
			{
				try
				{
					self.t4 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 8)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8>(self.t8)));
				}
			}

			private static void TryInvokeContinuationT5(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8> self, in UniTask<T5>.Awaiter awaiter)
			{
				try
				{
					self.t5 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 8)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8>(self.t8)));
				}
			}

			private static void TryInvokeContinuationT6(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8> self, in UniTask<T6>.Awaiter awaiter)
			{
				try
				{
					self.t6 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 8)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8>(self.t8)));
				}
			}

			private static void TryInvokeContinuationT7(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8> self, in UniTask<T7>.Awaiter awaiter)
			{
				try
				{
					self.t7 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 8)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8>(self.t8)));
				}
			}

			private static void TryInvokeContinuationT8(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8> self, in UniTask<T8>.Awaiter awaiter)
			{
				try
				{
					self.t8 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 8)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8>(self.t8)));
				}
			}

			public ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8>> GetResult(short token)
			{
				GC.SuppressFinalize(this);
				return this.core.GetResult(token);
			}

			void IUniTaskSource.GetResult(short token)
			{
				this.GetResult(token);
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

			private T1 t1;

			private T2 t2;

			private T3 t3;

			private T4 t4;

			private T5 t5;

			private T6 t6;

			private T7 t7;

			private T8 t8;

			private int completedCount;

			private UniTaskCompletionSourceCore<ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8>>> core;
		}

		private sealed class WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9> : IUniTaskSource<ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9>>>, IUniTaskSource
		{
			public WhenAllPromise(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4, UniTask<T5> task5, UniTask<T6> task6, UniTask<T7> task7, UniTask<T8> task8, UniTask<T9> task9)
			{
				this.completedCount = 0;
				UniTask<T1>.Awaiter awaiter = task1.GetAwaiter();
				if (awaiter.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>.TryInvokeContinuationT1(this, awaiter);
				}
				else
				{
					awaiter.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T1>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T1>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>.TryInvokeContinuationT1(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T1>.Awaiter>(this, awaiter));
				}
				UniTask<T2>.Awaiter awaiter2 = task2.GetAwaiter();
				if (awaiter2.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>.TryInvokeContinuationT2(this, awaiter2);
				}
				else
				{
					awaiter2.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T2>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T2>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>.TryInvokeContinuationT2(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T2>.Awaiter>(this, awaiter2));
				}
				UniTask<T3>.Awaiter awaiter3 = task3.GetAwaiter();
				if (awaiter3.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>.TryInvokeContinuationT3(this, awaiter3);
				}
				else
				{
					awaiter3.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T3>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T3>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>.TryInvokeContinuationT3(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T3>.Awaiter>(this, awaiter3));
				}
				UniTask<T4>.Awaiter awaiter4 = task4.GetAwaiter();
				if (awaiter4.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>.TryInvokeContinuationT4(this, awaiter4);
				}
				else
				{
					awaiter4.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T4>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T4>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>.TryInvokeContinuationT4(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T4>.Awaiter>(this, awaiter4));
				}
				UniTask<T5>.Awaiter awaiter5 = task5.GetAwaiter();
				if (awaiter5.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>.TryInvokeContinuationT5(this, awaiter5);
				}
				else
				{
					awaiter5.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T5>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T5>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>.TryInvokeContinuationT5(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T5>.Awaiter>(this, awaiter5));
				}
				UniTask<T6>.Awaiter awaiter6 = task6.GetAwaiter();
				if (awaiter6.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>.TryInvokeContinuationT6(this, awaiter6);
				}
				else
				{
					awaiter6.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T6>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T6>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>.TryInvokeContinuationT6(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T6>.Awaiter>(this, awaiter6));
				}
				UniTask<T7>.Awaiter awaiter7 = task7.GetAwaiter();
				if (awaiter7.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>.TryInvokeContinuationT7(this, awaiter7);
				}
				else
				{
					awaiter7.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T7>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T7>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>.TryInvokeContinuationT7(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T7>.Awaiter>(this, awaiter7));
				}
				UniTask<T8>.Awaiter awaiter8 = task8.GetAwaiter();
				if (awaiter8.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>.TryInvokeContinuationT8(this, awaiter8);
				}
				else
				{
					awaiter8.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T8>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T8>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>.TryInvokeContinuationT8(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T8>.Awaiter>(this, awaiter8));
				}
				UniTask<T9>.Awaiter awaiter9 = task9.GetAwaiter();
				if (awaiter9.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>.TryInvokeContinuationT9(this, awaiter9);
					return;
				}
				awaiter9.SourceOnCompleted(delegate(object state)
				{
					using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T9>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T9>.Awaiter>)state)
					{
						UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>.TryInvokeContinuationT9(stateTuple.Item1, stateTuple.Item2);
					}
				}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T9>.Awaiter>(this, awaiter9));
			}

			private static void TryInvokeContinuationT1(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9> self, in UniTask<T1>.Awaiter awaiter)
			{
				try
				{
					self.t1 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 9)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9>(self.t8, self.t9)));
				}
			}

			private static void TryInvokeContinuationT2(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9> self, in UniTask<T2>.Awaiter awaiter)
			{
				try
				{
					self.t2 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 9)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9>(self.t8, self.t9)));
				}
			}

			private static void TryInvokeContinuationT3(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9> self, in UniTask<T3>.Awaiter awaiter)
			{
				try
				{
					self.t3 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 9)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9>(self.t8, self.t9)));
				}
			}

			private static void TryInvokeContinuationT4(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9> self, in UniTask<T4>.Awaiter awaiter)
			{
				try
				{
					self.t4 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 9)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9>(self.t8, self.t9)));
				}
			}

			private static void TryInvokeContinuationT5(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9> self, in UniTask<T5>.Awaiter awaiter)
			{
				try
				{
					self.t5 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 9)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9>(self.t8, self.t9)));
				}
			}

			private static void TryInvokeContinuationT6(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9> self, in UniTask<T6>.Awaiter awaiter)
			{
				try
				{
					self.t6 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 9)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9>(self.t8, self.t9)));
				}
			}

			private static void TryInvokeContinuationT7(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9> self, in UniTask<T7>.Awaiter awaiter)
			{
				try
				{
					self.t7 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 9)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9>(self.t8, self.t9)));
				}
			}

			private static void TryInvokeContinuationT8(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9> self, in UniTask<T8>.Awaiter awaiter)
			{
				try
				{
					self.t8 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 9)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9>(self.t8, self.t9)));
				}
			}

			private static void TryInvokeContinuationT9(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9> self, in UniTask<T9>.Awaiter awaiter)
			{
				try
				{
					self.t9 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 9)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9>(self.t8, self.t9)));
				}
			}

			public ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9>> GetResult(short token)
			{
				GC.SuppressFinalize(this);
				return this.core.GetResult(token);
			}

			void IUniTaskSource.GetResult(short token)
			{
				this.GetResult(token);
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

			private T1 t1;

			private T2 t2;

			private T3 t3;

			private T4 t4;

			private T5 t5;

			private T6 t6;

			private T7 t7;

			private T8 t8;

			private T9 t9;

			private int completedCount;

			private UniTaskCompletionSourceCore<ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9>>> core;
		}

		private sealed class WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : IUniTaskSource<ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10>>>, IUniTaskSource
		{
			public WhenAllPromise(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4, UniTask<T5> task5, UniTask<T6> task6, UniTask<T7> task7, UniTask<T8> task8, UniTask<T9> task9, UniTask<T10> task10)
			{
				this.completedCount = 0;
				UniTask<T1>.Awaiter awaiter = task1.GetAwaiter();
				if (awaiter.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.TryInvokeContinuationT1(this, awaiter);
				}
				else
				{
					awaiter.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T1>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T1>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.TryInvokeContinuationT1(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T1>.Awaiter>(this, awaiter));
				}
				UniTask<T2>.Awaiter awaiter2 = task2.GetAwaiter();
				if (awaiter2.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.TryInvokeContinuationT2(this, awaiter2);
				}
				else
				{
					awaiter2.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T2>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T2>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.TryInvokeContinuationT2(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T2>.Awaiter>(this, awaiter2));
				}
				UniTask<T3>.Awaiter awaiter3 = task3.GetAwaiter();
				if (awaiter3.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.TryInvokeContinuationT3(this, awaiter3);
				}
				else
				{
					awaiter3.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T3>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T3>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.TryInvokeContinuationT3(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T3>.Awaiter>(this, awaiter3));
				}
				UniTask<T4>.Awaiter awaiter4 = task4.GetAwaiter();
				if (awaiter4.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.TryInvokeContinuationT4(this, awaiter4);
				}
				else
				{
					awaiter4.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T4>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T4>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.TryInvokeContinuationT4(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T4>.Awaiter>(this, awaiter4));
				}
				UniTask<T5>.Awaiter awaiter5 = task5.GetAwaiter();
				if (awaiter5.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.TryInvokeContinuationT5(this, awaiter5);
				}
				else
				{
					awaiter5.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T5>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T5>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.TryInvokeContinuationT5(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T5>.Awaiter>(this, awaiter5));
				}
				UniTask<T6>.Awaiter awaiter6 = task6.GetAwaiter();
				if (awaiter6.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.TryInvokeContinuationT6(this, awaiter6);
				}
				else
				{
					awaiter6.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T6>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T6>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.TryInvokeContinuationT6(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T6>.Awaiter>(this, awaiter6));
				}
				UniTask<T7>.Awaiter awaiter7 = task7.GetAwaiter();
				if (awaiter7.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.TryInvokeContinuationT7(this, awaiter7);
				}
				else
				{
					awaiter7.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T7>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T7>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.TryInvokeContinuationT7(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T7>.Awaiter>(this, awaiter7));
				}
				UniTask<T8>.Awaiter awaiter8 = task8.GetAwaiter();
				if (awaiter8.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.TryInvokeContinuationT8(this, awaiter8);
				}
				else
				{
					awaiter8.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T8>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T8>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.TryInvokeContinuationT8(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T8>.Awaiter>(this, awaiter8));
				}
				UniTask<T9>.Awaiter awaiter9 = task9.GetAwaiter();
				if (awaiter9.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.TryInvokeContinuationT9(this, awaiter9);
				}
				else
				{
					awaiter9.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T9>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T9>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.TryInvokeContinuationT9(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T9>.Awaiter>(this, awaiter9));
				}
				UniTask<T10>.Awaiter awaiter10 = task10.GetAwaiter();
				if (awaiter10.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.TryInvokeContinuationT10(this, awaiter10);
					return;
				}
				awaiter10.SourceOnCompleted(delegate(object state)
				{
					using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T10>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T10>.Awaiter>)state)
					{
						UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.TryInvokeContinuationT10(stateTuple.Item1, stateTuple.Item2);
					}
				}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T10>.Awaiter>(this, awaiter10));
			}

			private static void TryInvokeContinuationT1(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> self, in UniTask<T1>.Awaiter awaiter)
			{
				try
				{
					self.t1 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 10)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10>(self.t8, self.t9, self.t10)));
				}
			}

			private static void TryInvokeContinuationT2(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> self, in UniTask<T2>.Awaiter awaiter)
			{
				try
				{
					self.t2 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 10)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10>(self.t8, self.t9, self.t10)));
				}
			}

			private static void TryInvokeContinuationT3(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> self, in UniTask<T3>.Awaiter awaiter)
			{
				try
				{
					self.t3 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 10)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10>(self.t8, self.t9, self.t10)));
				}
			}

			private static void TryInvokeContinuationT4(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> self, in UniTask<T4>.Awaiter awaiter)
			{
				try
				{
					self.t4 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 10)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10>(self.t8, self.t9, self.t10)));
				}
			}

			private static void TryInvokeContinuationT5(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> self, in UniTask<T5>.Awaiter awaiter)
			{
				try
				{
					self.t5 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 10)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10>(self.t8, self.t9, self.t10)));
				}
			}

			private static void TryInvokeContinuationT6(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> self, in UniTask<T6>.Awaiter awaiter)
			{
				try
				{
					self.t6 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 10)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10>(self.t8, self.t9, self.t10)));
				}
			}

			private static void TryInvokeContinuationT7(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> self, in UniTask<T7>.Awaiter awaiter)
			{
				try
				{
					self.t7 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 10)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10>(self.t8, self.t9, self.t10)));
				}
			}

			private static void TryInvokeContinuationT8(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> self, in UniTask<T8>.Awaiter awaiter)
			{
				try
				{
					self.t8 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 10)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10>(self.t8, self.t9, self.t10)));
				}
			}

			private static void TryInvokeContinuationT9(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> self, in UniTask<T9>.Awaiter awaiter)
			{
				try
				{
					self.t9 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 10)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10>(self.t8, self.t9, self.t10)));
				}
			}

			private static void TryInvokeContinuationT10(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> self, in UniTask<T10>.Awaiter awaiter)
			{
				try
				{
					self.t10 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 10)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10>(self.t8, self.t9, self.t10)));
				}
			}

			public ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10>> GetResult(short token)
			{
				GC.SuppressFinalize(this);
				return this.core.GetResult(token);
			}

			void IUniTaskSource.GetResult(short token)
			{
				this.GetResult(token);
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

			private T1 t1;

			private T2 t2;

			private T3 t3;

			private T4 t4;

			private T5 t5;

			private T6 t6;

			private T7 t7;

			private T8 t8;

			private T9 t9;

			private T10 t10;

			private int completedCount;

			private UniTaskCompletionSourceCore<ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10>>> core;
		}

		private sealed class WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : IUniTaskSource<ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11>>>, IUniTaskSource
		{
			public WhenAllPromise(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4, UniTask<T5> task5, UniTask<T6> task6, UniTask<T7> task7, UniTask<T8> task8, UniTask<T9> task9, UniTask<T10> task10, UniTask<T11> task11)
			{
				this.completedCount = 0;
				UniTask<T1>.Awaiter awaiter = task1.GetAwaiter();
				if (awaiter.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.TryInvokeContinuationT1(this, awaiter);
				}
				else
				{
					awaiter.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T1>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T1>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.TryInvokeContinuationT1(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T1>.Awaiter>(this, awaiter));
				}
				UniTask<T2>.Awaiter awaiter2 = task2.GetAwaiter();
				if (awaiter2.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.TryInvokeContinuationT2(this, awaiter2);
				}
				else
				{
					awaiter2.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T2>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T2>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.TryInvokeContinuationT2(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T2>.Awaiter>(this, awaiter2));
				}
				UniTask<T3>.Awaiter awaiter3 = task3.GetAwaiter();
				if (awaiter3.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.TryInvokeContinuationT3(this, awaiter3);
				}
				else
				{
					awaiter3.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T3>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T3>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.TryInvokeContinuationT3(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T3>.Awaiter>(this, awaiter3));
				}
				UniTask<T4>.Awaiter awaiter4 = task4.GetAwaiter();
				if (awaiter4.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.TryInvokeContinuationT4(this, awaiter4);
				}
				else
				{
					awaiter4.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T4>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T4>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.TryInvokeContinuationT4(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T4>.Awaiter>(this, awaiter4));
				}
				UniTask<T5>.Awaiter awaiter5 = task5.GetAwaiter();
				if (awaiter5.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.TryInvokeContinuationT5(this, awaiter5);
				}
				else
				{
					awaiter5.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T5>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T5>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.TryInvokeContinuationT5(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T5>.Awaiter>(this, awaiter5));
				}
				UniTask<T6>.Awaiter awaiter6 = task6.GetAwaiter();
				if (awaiter6.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.TryInvokeContinuationT6(this, awaiter6);
				}
				else
				{
					awaiter6.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T6>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T6>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.TryInvokeContinuationT6(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T6>.Awaiter>(this, awaiter6));
				}
				UniTask<T7>.Awaiter awaiter7 = task7.GetAwaiter();
				if (awaiter7.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.TryInvokeContinuationT7(this, awaiter7);
				}
				else
				{
					awaiter7.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T7>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T7>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.TryInvokeContinuationT7(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T7>.Awaiter>(this, awaiter7));
				}
				UniTask<T8>.Awaiter awaiter8 = task8.GetAwaiter();
				if (awaiter8.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.TryInvokeContinuationT8(this, awaiter8);
				}
				else
				{
					awaiter8.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T8>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T8>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.TryInvokeContinuationT8(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T8>.Awaiter>(this, awaiter8));
				}
				UniTask<T9>.Awaiter awaiter9 = task9.GetAwaiter();
				if (awaiter9.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.TryInvokeContinuationT9(this, awaiter9);
				}
				else
				{
					awaiter9.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T9>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T9>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.TryInvokeContinuationT9(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T9>.Awaiter>(this, awaiter9));
				}
				UniTask<T10>.Awaiter awaiter10 = task10.GetAwaiter();
				if (awaiter10.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.TryInvokeContinuationT10(this, awaiter10);
				}
				else
				{
					awaiter10.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T10>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T10>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.TryInvokeContinuationT10(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T10>.Awaiter>(this, awaiter10));
				}
				UniTask<T11>.Awaiter awaiter11 = task11.GetAwaiter();
				if (awaiter11.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.TryInvokeContinuationT11(this, awaiter11);
					return;
				}
				awaiter11.SourceOnCompleted(delegate(object state)
				{
					using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T11>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T11>.Awaiter>)state)
					{
						UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.TryInvokeContinuationT11(stateTuple.Item1, stateTuple.Item2);
					}
				}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T11>.Awaiter>(this, awaiter11));
			}

			private static void TryInvokeContinuationT1(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> self, in UniTask<T1>.Awaiter awaiter)
			{
				try
				{
					self.t1 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 11)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11>(self.t8, self.t9, self.t10, self.t11)));
				}
			}

			private static void TryInvokeContinuationT2(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> self, in UniTask<T2>.Awaiter awaiter)
			{
				try
				{
					self.t2 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 11)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11>(self.t8, self.t9, self.t10, self.t11)));
				}
			}

			private static void TryInvokeContinuationT3(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> self, in UniTask<T3>.Awaiter awaiter)
			{
				try
				{
					self.t3 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 11)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11>(self.t8, self.t9, self.t10, self.t11)));
				}
			}

			private static void TryInvokeContinuationT4(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> self, in UniTask<T4>.Awaiter awaiter)
			{
				try
				{
					self.t4 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 11)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11>(self.t8, self.t9, self.t10, self.t11)));
				}
			}

			private static void TryInvokeContinuationT5(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> self, in UniTask<T5>.Awaiter awaiter)
			{
				try
				{
					self.t5 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 11)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11>(self.t8, self.t9, self.t10, self.t11)));
				}
			}

			private static void TryInvokeContinuationT6(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> self, in UniTask<T6>.Awaiter awaiter)
			{
				try
				{
					self.t6 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 11)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11>(self.t8, self.t9, self.t10, self.t11)));
				}
			}

			private static void TryInvokeContinuationT7(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> self, in UniTask<T7>.Awaiter awaiter)
			{
				try
				{
					self.t7 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 11)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11>(self.t8, self.t9, self.t10, self.t11)));
				}
			}

			private static void TryInvokeContinuationT8(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> self, in UniTask<T8>.Awaiter awaiter)
			{
				try
				{
					self.t8 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 11)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11>(self.t8, self.t9, self.t10, self.t11)));
				}
			}

			private static void TryInvokeContinuationT9(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> self, in UniTask<T9>.Awaiter awaiter)
			{
				try
				{
					self.t9 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 11)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11>(self.t8, self.t9, self.t10, self.t11)));
				}
			}

			private static void TryInvokeContinuationT10(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> self, in UniTask<T10>.Awaiter awaiter)
			{
				try
				{
					self.t10 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 11)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11>(self.t8, self.t9, self.t10, self.t11)));
				}
			}

			private static void TryInvokeContinuationT11(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> self, in UniTask<T11>.Awaiter awaiter)
			{
				try
				{
					self.t11 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 11)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11>(self.t8, self.t9, self.t10, self.t11)));
				}
			}

			public ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11>> GetResult(short token)
			{
				GC.SuppressFinalize(this);
				return this.core.GetResult(token);
			}

			void IUniTaskSource.GetResult(short token)
			{
				this.GetResult(token);
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

			private T1 t1;

			private T2 t2;

			private T3 t3;

			private T4 t4;

			private T5 t5;

			private T6 t6;

			private T7 t7;

			private T8 t8;

			private T9 t9;

			private T10 t10;

			private T11 t11;

			private int completedCount;

			private UniTaskCompletionSourceCore<ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11>>> core;
		}

		private sealed class WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : IUniTaskSource<ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12>>>, IUniTaskSource
		{
			public WhenAllPromise(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4, UniTask<T5> task5, UniTask<T6> task6, UniTask<T7> task7, UniTask<T8> task8, UniTask<T9> task9, UniTask<T10> task10, UniTask<T11> task11, UniTask<T12> task12)
			{
				this.completedCount = 0;
				UniTask<T1>.Awaiter awaiter = task1.GetAwaiter();
				if (awaiter.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.TryInvokeContinuationT1(this, awaiter);
				}
				else
				{
					awaiter.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T1>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T1>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.TryInvokeContinuationT1(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T1>.Awaiter>(this, awaiter));
				}
				UniTask<T2>.Awaiter awaiter2 = task2.GetAwaiter();
				if (awaiter2.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.TryInvokeContinuationT2(this, awaiter2);
				}
				else
				{
					awaiter2.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T2>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T2>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.TryInvokeContinuationT2(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T2>.Awaiter>(this, awaiter2));
				}
				UniTask<T3>.Awaiter awaiter3 = task3.GetAwaiter();
				if (awaiter3.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.TryInvokeContinuationT3(this, awaiter3);
				}
				else
				{
					awaiter3.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T3>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T3>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.TryInvokeContinuationT3(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T3>.Awaiter>(this, awaiter3));
				}
				UniTask<T4>.Awaiter awaiter4 = task4.GetAwaiter();
				if (awaiter4.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.TryInvokeContinuationT4(this, awaiter4);
				}
				else
				{
					awaiter4.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T4>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T4>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.TryInvokeContinuationT4(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T4>.Awaiter>(this, awaiter4));
				}
				UniTask<T5>.Awaiter awaiter5 = task5.GetAwaiter();
				if (awaiter5.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.TryInvokeContinuationT5(this, awaiter5);
				}
				else
				{
					awaiter5.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T5>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T5>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.TryInvokeContinuationT5(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T5>.Awaiter>(this, awaiter5));
				}
				UniTask<T6>.Awaiter awaiter6 = task6.GetAwaiter();
				if (awaiter6.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.TryInvokeContinuationT6(this, awaiter6);
				}
				else
				{
					awaiter6.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T6>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T6>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.TryInvokeContinuationT6(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T6>.Awaiter>(this, awaiter6));
				}
				UniTask<T7>.Awaiter awaiter7 = task7.GetAwaiter();
				if (awaiter7.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.TryInvokeContinuationT7(this, awaiter7);
				}
				else
				{
					awaiter7.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T7>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T7>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.TryInvokeContinuationT7(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T7>.Awaiter>(this, awaiter7));
				}
				UniTask<T8>.Awaiter awaiter8 = task8.GetAwaiter();
				if (awaiter8.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.TryInvokeContinuationT8(this, awaiter8);
				}
				else
				{
					awaiter8.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T8>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T8>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.TryInvokeContinuationT8(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T8>.Awaiter>(this, awaiter8));
				}
				UniTask<T9>.Awaiter awaiter9 = task9.GetAwaiter();
				if (awaiter9.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.TryInvokeContinuationT9(this, awaiter9);
				}
				else
				{
					awaiter9.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T9>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T9>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.TryInvokeContinuationT9(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T9>.Awaiter>(this, awaiter9));
				}
				UniTask<T10>.Awaiter awaiter10 = task10.GetAwaiter();
				if (awaiter10.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.TryInvokeContinuationT10(this, awaiter10);
				}
				else
				{
					awaiter10.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T10>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T10>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.TryInvokeContinuationT10(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T10>.Awaiter>(this, awaiter10));
				}
				UniTask<T11>.Awaiter awaiter11 = task11.GetAwaiter();
				if (awaiter11.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.TryInvokeContinuationT11(this, awaiter11);
				}
				else
				{
					awaiter11.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T11>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T11>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.TryInvokeContinuationT11(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T11>.Awaiter>(this, awaiter11));
				}
				UniTask<T12>.Awaiter awaiter12 = task12.GetAwaiter();
				if (awaiter12.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.TryInvokeContinuationT12(this, awaiter12);
					return;
				}
				awaiter12.SourceOnCompleted(delegate(object state)
				{
					using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T12>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T12>.Awaiter>)state)
					{
						UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.TryInvokeContinuationT12(stateTuple.Item1, stateTuple.Item2);
					}
				}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T12>.Awaiter>(this, awaiter12));
			}

			private static void TryInvokeContinuationT1(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> self, in UniTask<T1>.Awaiter awaiter)
			{
				try
				{
					self.t1 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 12)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12>(self.t8, self.t9, self.t10, self.t11, self.t12)));
				}
			}

			private static void TryInvokeContinuationT2(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> self, in UniTask<T2>.Awaiter awaiter)
			{
				try
				{
					self.t2 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 12)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12>(self.t8, self.t9, self.t10, self.t11, self.t12)));
				}
			}

			private static void TryInvokeContinuationT3(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> self, in UniTask<T3>.Awaiter awaiter)
			{
				try
				{
					self.t3 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 12)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12>(self.t8, self.t9, self.t10, self.t11, self.t12)));
				}
			}

			private static void TryInvokeContinuationT4(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> self, in UniTask<T4>.Awaiter awaiter)
			{
				try
				{
					self.t4 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 12)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12>(self.t8, self.t9, self.t10, self.t11, self.t12)));
				}
			}

			private static void TryInvokeContinuationT5(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> self, in UniTask<T5>.Awaiter awaiter)
			{
				try
				{
					self.t5 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 12)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12>(self.t8, self.t9, self.t10, self.t11, self.t12)));
				}
			}

			private static void TryInvokeContinuationT6(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> self, in UniTask<T6>.Awaiter awaiter)
			{
				try
				{
					self.t6 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 12)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12>(self.t8, self.t9, self.t10, self.t11, self.t12)));
				}
			}

			private static void TryInvokeContinuationT7(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> self, in UniTask<T7>.Awaiter awaiter)
			{
				try
				{
					self.t7 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 12)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12>(self.t8, self.t9, self.t10, self.t11, self.t12)));
				}
			}

			private static void TryInvokeContinuationT8(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> self, in UniTask<T8>.Awaiter awaiter)
			{
				try
				{
					self.t8 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 12)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12>(self.t8, self.t9, self.t10, self.t11, self.t12)));
				}
			}

			private static void TryInvokeContinuationT9(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> self, in UniTask<T9>.Awaiter awaiter)
			{
				try
				{
					self.t9 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 12)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12>(self.t8, self.t9, self.t10, self.t11, self.t12)));
				}
			}

			private static void TryInvokeContinuationT10(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> self, in UniTask<T10>.Awaiter awaiter)
			{
				try
				{
					self.t10 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 12)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12>(self.t8, self.t9, self.t10, self.t11, self.t12)));
				}
			}

			private static void TryInvokeContinuationT11(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> self, in UniTask<T11>.Awaiter awaiter)
			{
				try
				{
					self.t11 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 12)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12>(self.t8, self.t9, self.t10, self.t11, self.t12)));
				}
			}

			private static void TryInvokeContinuationT12(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> self, in UniTask<T12>.Awaiter awaiter)
			{
				try
				{
					self.t12 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 12)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12>(self.t8, self.t9, self.t10, self.t11, self.t12)));
				}
			}

			public ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12>> GetResult(short token)
			{
				GC.SuppressFinalize(this);
				return this.core.GetResult(token);
			}

			void IUniTaskSource.GetResult(short token)
			{
				this.GetResult(token);
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

			private T1 t1;

			private T2 t2;

			private T3 t3;

			private T4 t4;

			private T5 t5;

			private T6 t6;

			private T7 t7;

			private T8 t8;

			private T9 t9;

			private T10 t10;

			private T11 t11;

			private T12 t12;

			private int completedCount;

			private UniTaskCompletionSourceCore<ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12>>> core;
		}

		private sealed class WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : IUniTaskSource<ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13>>>, IUniTaskSource
		{
			public WhenAllPromise(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4, UniTask<T5> task5, UniTask<T6> task6, UniTask<T7> task7, UniTask<T8> task8, UniTask<T9> task9, UniTask<T10> task10, UniTask<T11> task11, UniTask<T12> task12, UniTask<T13> task13)
			{
				this.completedCount = 0;
				UniTask<T1>.Awaiter awaiter = task1.GetAwaiter();
				if (awaiter.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.TryInvokeContinuationT1(this, awaiter);
				}
				else
				{
					awaiter.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T1>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T1>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.TryInvokeContinuationT1(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T1>.Awaiter>(this, awaiter));
				}
				UniTask<T2>.Awaiter awaiter2 = task2.GetAwaiter();
				if (awaiter2.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.TryInvokeContinuationT2(this, awaiter2);
				}
				else
				{
					awaiter2.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T2>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T2>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.TryInvokeContinuationT2(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T2>.Awaiter>(this, awaiter2));
				}
				UniTask<T3>.Awaiter awaiter3 = task3.GetAwaiter();
				if (awaiter3.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.TryInvokeContinuationT3(this, awaiter3);
				}
				else
				{
					awaiter3.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T3>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T3>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.TryInvokeContinuationT3(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T3>.Awaiter>(this, awaiter3));
				}
				UniTask<T4>.Awaiter awaiter4 = task4.GetAwaiter();
				if (awaiter4.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.TryInvokeContinuationT4(this, awaiter4);
				}
				else
				{
					awaiter4.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T4>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T4>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.TryInvokeContinuationT4(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T4>.Awaiter>(this, awaiter4));
				}
				UniTask<T5>.Awaiter awaiter5 = task5.GetAwaiter();
				if (awaiter5.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.TryInvokeContinuationT5(this, awaiter5);
				}
				else
				{
					awaiter5.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T5>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T5>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.TryInvokeContinuationT5(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T5>.Awaiter>(this, awaiter5));
				}
				UniTask<T6>.Awaiter awaiter6 = task6.GetAwaiter();
				if (awaiter6.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.TryInvokeContinuationT6(this, awaiter6);
				}
				else
				{
					awaiter6.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T6>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T6>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.TryInvokeContinuationT6(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T6>.Awaiter>(this, awaiter6));
				}
				UniTask<T7>.Awaiter awaiter7 = task7.GetAwaiter();
				if (awaiter7.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.TryInvokeContinuationT7(this, awaiter7);
				}
				else
				{
					awaiter7.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T7>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T7>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.TryInvokeContinuationT7(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T7>.Awaiter>(this, awaiter7));
				}
				UniTask<T8>.Awaiter awaiter8 = task8.GetAwaiter();
				if (awaiter8.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.TryInvokeContinuationT8(this, awaiter8);
				}
				else
				{
					awaiter8.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T8>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T8>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.TryInvokeContinuationT8(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T8>.Awaiter>(this, awaiter8));
				}
				UniTask<T9>.Awaiter awaiter9 = task9.GetAwaiter();
				if (awaiter9.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.TryInvokeContinuationT9(this, awaiter9);
				}
				else
				{
					awaiter9.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T9>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T9>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.TryInvokeContinuationT9(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T9>.Awaiter>(this, awaiter9));
				}
				UniTask<T10>.Awaiter awaiter10 = task10.GetAwaiter();
				if (awaiter10.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.TryInvokeContinuationT10(this, awaiter10);
				}
				else
				{
					awaiter10.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T10>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T10>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.TryInvokeContinuationT10(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T10>.Awaiter>(this, awaiter10));
				}
				UniTask<T11>.Awaiter awaiter11 = task11.GetAwaiter();
				if (awaiter11.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.TryInvokeContinuationT11(this, awaiter11);
				}
				else
				{
					awaiter11.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T11>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T11>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.TryInvokeContinuationT11(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T11>.Awaiter>(this, awaiter11));
				}
				UniTask<T12>.Awaiter awaiter12 = task12.GetAwaiter();
				if (awaiter12.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.TryInvokeContinuationT12(this, awaiter12);
				}
				else
				{
					awaiter12.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T12>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T12>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.TryInvokeContinuationT12(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T12>.Awaiter>(this, awaiter12));
				}
				UniTask<T13>.Awaiter awaiter13 = task13.GetAwaiter();
				if (awaiter13.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.TryInvokeContinuationT13(this, awaiter13);
					return;
				}
				awaiter13.SourceOnCompleted(delegate(object state)
				{
					using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T13>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T13>.Awaiter>)state)
					{
						UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.TryInvokeContinuationT13(stateTuple.Item1, stateTuple.Item2);
					}
				}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T13>.Awaiter>(this, awaiter13));
			}

			private static void TryInvokeContinuationT1(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> self, in UniTask<T1>.Awaiter awaiter)
			{
				try
				{
					self.t1 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 13)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12, T13>(self.t8, self.t9, self.t10, self.t11, self.t12, self.t13)));
				}
			}

			private static void TryInvokeContinuationT2(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> self, in UniTask<T2>.Awaiter awaiter)
			{
				try
				{
					self.t2 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 13)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12, T13>(self.t8, self.t9, self.t10, self.t11, self.t12, self.t13)));
				}
			}

			private static void TryInvokeContinuationT3(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> self, in UniTask<T3>.Awaiter awaiter)
			{
				try
				{
					self.t3 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 13)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12, T13>(self.t8, self.t9, self.t10, self.t11, self.t12, self.t13)));
				}
			}

			private static void TryInvokeContinuationT4(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> self, in UniTask<T4>.Awaiter awaiter)
			{
				try
				{
					self.t4 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 13)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12, T13>(self.t8, self.t9, self.t10, self.t11, self.t12, self.t13)));
				}
			}

			private static void TryInvokeContinuationT5(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> self, in UniTask<T5>.Awaiter awaiter)
			{
				try
				{
					self.t5 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 13)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12, T13>(self.t8, self.t9, self.t10, self.t11, self.t12, self.t13)));
				}
			}

			private static void TryInvokeContinuationT6(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> self, in UniTask<T6>.Awaiter awaiter)
			{
				try
				{
					self.t6 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 13)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12, T13>(self.t8, self.t9, self.t10, self.t11, self.t12, self.t13)));
				}
			}

			private static void TryInvokeContinuationT7(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> self, in UniTask<T7>.Awaiter awaiter)
			{
				try
				{
					self.t7 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 13)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12, T13>(self.t8, self.t9, self.t10, self.t11, self.t12, self.t13)));
				}
			}

			private static void TryInvokeContinuationT8(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> self, in UniTask<T8>.Awaiter awaiter)
			{
				try
				{
					self.t8 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 13)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12, T13>(self.t8, self.t9, self.t10, self.t11, self.t12, self.t13)));
				}
			}

			private static void TryInvokeContinuationT9(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> self, in UniTask<T9>.Awaiter awaiter)
			{
				try
				{
					self.t9 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 13)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12, T13>(self.t8, self.t9, self.t10, self.t11, self.t12, self.t13)));
				}
			}

			private static void TryInvokeContinuationT10(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> self, in UniTask<T10>.Awaiter awaiter)
			{
				try
				{
					self.t10 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 13)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12, T13>(self.t8, self.t9, self.t10, self.t11, self.t12, self.t13)));
				}
			}

			private static void TryInvokeContinuationT11(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> self, in UniTask<T11>.Awaiter awaiter)
			{
				try
				{
					self.t11 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 13)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12, T13>(self.t8, self.t9, self.t10, self.t11, self.t12, self.t13)));
				}
			}

			private static void TryInvokeContinuationT12(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> self, in UniTask<T12>.Awaiter awaiter)
			{
				try
				{
					self.t12 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 13)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12, T13>(self.t8, self.t9, self.t10, self.t11, self.t12, self.t13)));
				}
			}

			private static void TryInvokeContinuationT13(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> self, in UniTask<T13>.Awaiter awaiter)
			{
				try
				{
					self.t13 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 13)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12, T13>(self.t8, self.t9, self.t10, self.t11, self.t12, self.t13)));
				}
			}

			public ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13>> GetResult(short token)
			{
				GC.SuppressFinalize(this);
				return this.core.GetResult(token);
			}

			void IUniTaskSource.GetResult(short token)
			{
				this.GetResult(token);
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

			private T1 t1;

			private T2 t2;

			private T3 t3;

			private T4 t4;

			private T5 t5;

			private T6 t6;

			private T7 t7;

			private T8 t8;

			private T9 t9;

			private T10 t10;

			private T11 t11;

			private T12 t12;

			private T13 t13;

			private int completedCount;

			private UniTaskCompletionSourceCore<ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13>>> core;
		}

		private sealed class WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> : IUniTaskSource<ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14>>>, IUniTaskSource
		{
			public WhenAllPromise(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4, UniTask<T5> task5, UniTask<T6> task6, UniTask<T7> task7, UniTask<T8> task8, UniTask<T9> task9, UniTask<T10> task10, UniTask<T11> task11, UniTask<T12> task12, UniTask<T13> task13, UniTask<T14> task14)
			{
				this.completedCount = 0;
				UniTask<T1>.Awaiter awaiter = task1.GetAwaiter();
				if (awaiter.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT1(this, awaiter);
				}
				else
				{
					awaiter.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T1>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T1>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT1(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T1>.Awaiter>(this, awaiter));
				}
				UniTask<T2>.Awaiter awaiter2 = task2.GetAwaiter();
				if (awaiter2.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT2(this, awaiter2);
				}
				else
				{
					awaiter2.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T2>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T2>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT2(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T2>.Awaiter>(this, awaiter2));
				}
				UniTask<T3>.Awaiter awaiter3 = task3.GetAwaiter();
				if (awaiter3.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT3(this, awaiter3);
				}
				else
				{
					awaiter3.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T3>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T3>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT3(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T3>.Awaiter>(this, awaiter3));
				}
				UniTask<T4>.Awaiter awaiter4 = task4.GetAwaiter();
				if (awaiter4.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT4(this, awaiter4);
				}
				else
				{
					awaiter4.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T4>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T4>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT4(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T4>.Awaiter>(this, awaiter4));
				}
				UniTask<T5>.Awaiter awaiter5 = task5.GetAwaiter();
				if (awaiter5.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT5(this, awaiter5);
				}
				else
				{
					awaiter5.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T5>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T5>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT5(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T5>.Awaiter>(this, awaiter5));
				}
				UniTask<T6>.Awaiter awaiter6 = task6.GetAwaiter();
				if (awaiter6.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT6(this, awaiter6);
				}
				else
				{
					awaiter6.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T6>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T6>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT6(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T6>.Awaiter>(this, awaiter6));
				}
				UniTask<T7>.Awaiter awaiter7 = task7.GetAwaiter();
				if (awaiter7.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT7(this, awaiter7);
				}
				else
				{
					awaiter7.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T7>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T7>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT7(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T7>.Awaiter>(this, awaiter7));
				}
				UniTask<T8>.Awaiter awaiter8 = task8.GetAwaiter();
				if (awaiter8.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT8(this, awaiter8);
				}
				else
				{
					awaiter8.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T8>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T8>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT8(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T8>.Awaiter>(this, awaiter8));
				}
				UniTask<T9>.Awaiter awaiter9 = task9.GetAwaiter();
				if (awaiter9.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT9(this, awaiter9);
				}
				else
				{
					awaiter9.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T9>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T9>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT9(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T9>.Awaiter>(this, awaiter9));
				}
				UniTask<T10>.Awaiter awaiter10 = task10.GetAwaiter();
				if (awaiter10.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT10(this, awaiter10);
				}
				else
				{
					awaiter10.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T10>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T10>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT10(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T10>.Awaiter>(this, awaiter10));
				}
				UniTask<T11>.Awaiter awaiter11 = task11.GetAwaiter();
				if (awaiter11.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT11(this, awaiter11);
				}
				else
				{
					awaiter11.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T11>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T11>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT11(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T11>.Awaiter>(this, awaiter11));
				}
				UniTask<T12>.Awaiter awaiter12 = task12.GetAwaiter();
				if (awaiter12.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT12(this, awaiter12);
				}
				else
				{
					awaiter12.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T12>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T12>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT12(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T12>.Awaiter>(this, awaiter12));
				}
				UniTask<T13>.Awaiter awaiter13 = task13.GetAwaiter();
				if (awaiter13.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT13(this, awaiter13);
				}
				else
				{
					awaiter13.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T13>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T13>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT13(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T13>.Awaiter>(this, awaiter13));
				}
				UniTask<T14>.Awaiter awaiter14 = task14.GetAwaiter();
				if (awaiter14.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT14(this, awaiter14);
					return;
				}
				awaiter14.SourceOnCompleted(delegate(object state)
				{
					using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T14>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T14>.Awaiter>)state)
					{
						UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT14(stateTuple.Item1, stateTuple.Item2);
					}
				}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T14>.Awaiter>(this, awaiter14));
			}

			private static void TryInvokeContinuationT1(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> self, in UniTask<T1>.Awaiter awaiter)
			{
				try
				{
					self.t1 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 14)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12, T13, T14>(self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14)));
				}
			}

			private static void TryInvokeContinuationT2(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> self, in UniTask<T2>.Awaiter awaiter)
			{
				try
				{
					self.t2 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 14)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12, T13, T14>(self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14)));
				}
			}

			private static void TryInvokeContinuationT3(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> self, in UniTask<T3>.Awaiter awaiter)
			{
				try
				{
					self.t3 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 14)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12, T13, T14>(self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14)));
				}
			}

			private static void TryInvokeContinuationT4(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> self, in UniTask<T4>.Awaiter awaiter)
			{
				try
				{
					self.t4 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 14)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12, T13, T14>(self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14)));
				}
			}

			private static void TryInvokeContinuationT5(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> self, in UniTask<T5>.Awaiter awaiter)
			{
				try
				{
					self.t5 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 14)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12, T13, T14>(self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14)));
				}
			}

			private static void TryInvokeContinuationT6(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> self, in UniTask<T6>.Awaiter awaiter)
			{
				try
				{
					self.t6 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 14)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12, T13, T14>(self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14)));
				}
			}

			private static void TryInvokeContinuationT7(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> self, in UniTask<T7>.Awaiter awaiter)
			{
				try
				{
					self.t7 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 14)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12, T13, T14>(self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14)));
				}
			}

			private static void TryInvokeContinuationT8(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> self, in UniTask<T8>.Awaiter awaiter)
			{
				try
				{
					self.t8 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 14)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12, T13, T14>(self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14)));
				}
			}

			private static void TryInvokeContinuationT9(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> self, in UniTask<T9>.Awaiter awaiter)
			{
				try
				{
					self.t9 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 14)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12, T13, T14>(self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14)));
				}
			}

			private static void TryInvokeContinuationT10(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> self, in UniTask<T10>.Awaiter awaiter)
			{
				try
				{
					self.t10 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 14)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12, T13, T14>(self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14)));
				}
			}

			private static void TryInvokeContinuationT11(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> self, in UniTask<T11>.Awaiter awaiter)
			{
				try
				{
					self.t11 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 14)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12, T13, T14>(self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14)));
				}
			}

			private static void TryInvokeContinuationT12(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> self, in UniTask<T12>.Awaiter awaiter)
			{
				try
				{
					self.t12 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 14)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12, T13, T14>(self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14)));
				}
			}

			private static void TryInvokeContinuationT13(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> self, in UniTask<T13>.Awaiter awaiter)
			{
				try
				{
					self.t13 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 14)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12, T13, T14>(self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14)));
				}
			}

			private static void TryInvokeContinuationT14(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> self, in UniTask<T14>.Awaiter awaiter)
			{
				try
				{
					self.t14 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 14)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12, T13, T14>(self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14)));
				}
			}

			public ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14>> GetResult(short token)
			{
				GC.SuppressFinalize(this);
				return this.core.GetResult(token);
			}

			void IUniTaskSource.GetResult(short token)
			{
				this.GetResult(token);
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

			private T1 t1;

			private T2 t2;

			private T3 t3;

			private T4 t4;

			private T5 t5;

			private T6 t6;

			private T7 t7;

			private T8 t8;

			private T9 t9;

			private T10 t10;

			private T11 t11;

			private T12 t12;

			private T13 t13;

			private T14 t14;

			private int completedCount;

			private UniTaskCompletionSourceCore<ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14>>> core;
		}

		private sealed class WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> : IUniTaskSource<ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15>>>>, IUniTaskSource
		{
			public WhenAllPromise(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4, UniTask<T5> task5, UniTask<T6> task6, UniTask<T7> task7, UniTask<T8> task8, UniTask<T9> task9, UniTask<T10> task10, UniTask<T11> task11, UniTask<T12> task12, UniTask<T13> task13, UniTask<T14> task14, UniTask<T15> task15)
			{
				this.completedCount = 0;
				UniTask<T1>.Awaiter awaiter = task1.GetAwaiter();
				if (awaiter.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT1(this, awaiter);
				}
				else
				{
					awaiter.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T1>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T1>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT1(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T1>.Awaiter>(this, awaiter));
				}
				UniTask<T2>.Awaiter awaiter2 = task2.GetAwaiter();
				if (awaiter2.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT2(this, awaiter2);
				}
				else
				{
					awaiter2.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T2>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T2>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT2(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T2>.Awaiter>(this, awaiter2));
				}
				UniTask<T3>.Awaiter awaiter3 = task3.GetAwaiter();
				if (awaiter3.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT3(this, awaiter3);
				}
				else
				{
					awaiter3.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T3>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T3>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT3(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T3>.Awaiter>(this, awaiter3));
				}
				UniTask<T4>.Awaiter awaiter4 = task4.GetAwaiter();
				if (awaiter4.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT4(this, awaiter4);
				}
				else
				{
					awaiter4.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T4>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T4>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT4(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T4>.Awaiter>(this, awaiter4));
				}
				UniTask<T5>.Awaiter awaiter5 = task5.GetAwaiter();
				if (awaiter5.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT5(this, awaiter5);
				}
				else
				{
					awaiter5.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T5>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T5>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT5(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T5>.Awaiter>(this, awaiter5));
				}
				UniTask<T6>.Awaiter awaiter6 = task6.GetAwaiter();
				if (awaiter6.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT6(this, awaiter6);
				}
				else
				{
					awaiter6.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T6>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T6>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT6(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T6>.Awaiter>(this, awaiter6));
				}
				UniTask<T7>.Awaiter awaiter7 = task7.GetAwaiter();
				if (awaiter7.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT7(this, awaiter7);
				}
				else
				{
					awaiter7.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T7>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T7>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT7(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T7>.Awaiter>(this, awaiter7));
				}
				UniTask<T8>.Awaiter awaiter8 = task8.GetAwaiter();
				if (awaiter8.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT8(this, awaiter8);
				}
				else
				{
					awaiter8.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T8>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T8>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT8(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T8>.Awaiter>(this, awaiter8));
				}
				UniTask<T9>.Awaiter awaiter9 = task9.GetAwaiter();
				if (awaiter9.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT9(this, awaiter9);
				}
				else
				{
					awaiter9.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T9>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T9>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT9(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T9>.Awaiter>(this, awaiter9));
				}
				UniTask<T10>.Awaiter awaiter10 = task10.GetAwaiter();
				if (awaiter10.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT10(this, awaiter10);
				}
				else
				{
					awaiter10.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T10>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T10>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT10(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T10>.Awaiter>(this, awaiter10));
				}
				UniTask<T11>.Awaiter awaiter11 = task11.GetAwaiter();
				if (awaiter11.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT11(this, awaiter11);
				}
				else
				{
					awaiter11.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T11>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T11>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT11(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T11>.Awaiter>(this, awaiter11));
				}
				UniTask<T12>.Awaiter awaiter12 = task12.GetAwaiter();
				if (awaiter12.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT12(this, awaiter12);
				}
				else
				{
					awaiter12.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T12>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T12>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT12(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T12>.Awaiter>(this, awaiter12));
				}
				UniTask<T13>.Awaiter awaiter13 = task13.GetAwaiter();
				if (awaiter13.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT13(this, awaiter13);
				}
				else
				{
					awaiter13.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T13>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T13>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT13(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T13>.Awaiter>(this, awaiter13));
				}
				UniTask<T14>.Awaiter awaiter14 = task14.GetAwaiter();
				if (awaiter14.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT14(this, awaiter14);
				}
				else
				{
					awaiter14.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T14>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T14>.Awaiter>)state)
						{
							UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT14(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T14>.Awaiter>(this, awaiter14));
				}
				UniTask<T15>.Awaiter awaiter15 = task15.GetAwaiter();
				if (awaiter15.IsCompleted)
				{
					UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT15(this, awaiter15);
					return;
				}
				awaiter15.SourceOnCompleted(delegate(object state)
				{
					using (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T15>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T15>.Awaiter>)state)
					{
						UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT15(stateTuple.Item1, stateTuple.Item2);
					}
				}, StateTuple.Create<UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T15>.Awaiter>(this, awaiter15));
			}

			private static void TryInvokeContinuationT1(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> self, in UniTask<T1>.Awaiter awaiter)
			{
				try
				{
					self.t1 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 15)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15>>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15>>(self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14, new ValueTuple<T15>(self.t15))));
				}
			}

			private static void TryInvokeContinuationT2(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> self, in UniTask<T2>.Awaiter awaiter)
			{
				try
				{
					self.t2 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 15)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15>>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15>>(self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14, new ValueTuple<T15>(self.t15))));
				}
			}

			private static void TryInvokeContinuationT3(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> self, in UniTask<T3>.Awaiter awaiter)
			{
				try
				{
					self.t3 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 15)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15>>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15>>(self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14, new ValueTuple<T15>(self.t15))));
				}
			}

			private static void TryInvokeContinuationT4(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> self, in UniTask<T4>.Awaiter awaiter)
			{
				try
				{
					self.t4 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 15)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15>>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15>>(self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14, new ValueTuple<T15>(self.t15))));
				}
			}

			private static void TryInvokeContinuationT5(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> self, in UniTask<T5>.Awaiter awaiter)
			{
				try
				{
					self.t5 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 15)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15>>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15>>(self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14, new ValueTuple<T15>(self.t15))));
				}
			}

			private static void TryInvokeContinuationT6(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> self, in UniTask<T6>.Awaiter awaiter)
			{
				try
				{
					self.t6 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 15)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15>>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15>>(self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14, new ValueTuple<T15>(self.t15))));
				}
			}

			private static void TryInvokeContinuationT7(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> self, in UniTask<T7>.Awaiter awaiter)
			{
				try
				{
					self.t7 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 15)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15>>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15>>(self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14, new ValueTuple<T15>(self.t15))));
				}
			}

			private static void TryInvokeContinuationT8(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> self, in UniTask<T8>.Awaiter awaiter)
			{
				try
				{
					self.t8 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 15)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15>>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15>>(self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14, new ValueTuple<T15>(self.t15))));
				}
			}

			private static void TryInvokeContinuationT9(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> self, in UniTask<T9>.Awaiter awaiter)
			{
				try
				{
					self.t9 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 15)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15>>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15>>(self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14, new ValueTuple<T15>(self.t15))));
				}
			}

			private static void TryInvokeContinuationT10(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> self, in UniTask<T10>.Awaiter awaiter)
			{
				try
				{
					self.t10 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 15)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15>>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15>>(self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14, new ValueTuple<T15>(self.t15))));
				}
			}

			private static void TryInvokeContinuationT11(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> self, in UniTask<T11>.Awaiter awaiter)
			{
				try
				{
					self.t11 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 15)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15>>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15>>(self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14, new ValueTuple<T15>(self.t15))));
				}
			}

			private static void TryInvokeContinuationT12(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> self, in UniTask<T12>.Awaiter awaiter)
			{
				try
				{
					self.t12 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 15)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15>>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15>>(self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14, new ValueTuple<T15>(self.t15))));
				}
			}

			private static void TryInvokeContinuationT13(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> self, in UniTask<T13>.Awaiter awaiter)
			{
				try
				{
					self.t13 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 15)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15>>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15>>(self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14, new ValueTuple<T15>(self.t15))));
				}
			}

			private static void TryInvokeContinuationT14(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> self, in UniTask<T14>.Awaiter awaiter)
			{
				try
				{
					self.t14 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 15)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15>>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15>>(self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14, new ValueTuple<T15>(self.t15))));
				}
			}

			private static void TryInvokeContinuationT15(UniTask.WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> self, in UniTask<T15>.Awaiter awaiter)
			{
				try
				{
					self.t15 = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 15)
				{
					self.core.TrySetResult(new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15>>>(self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, new ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15>>(self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14, new ValueTuple<T15>(self.t15))));
				}
			}

			public ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15>>> GetResult(short token)
			{
				GC.SuppressFinalize(this);
				return this.core.GetResult(token);
			}

			void IUniTaskSource.GetResult(short token)
			{
				this.GetResult(token);
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

			private T1 t1;

			private T2 t2;

			private T3 t3;

			private T4 t4;

			private T5 t5;

			private T6 t6;

			private T7 t7;

			private T8 t8;

			private T9 t9;

			private T10 t10;

			private T11 t11;

			private T12 t12;

			private T13 t13;

			private T14 t14;

			private T15 t15;

			private int completedCount;

			private UniTaskCompletionSourceCore<ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15>>>> core;
		}

		private sealed class WhenAnyLRPromise<T> : IUniTaskSource<ValueTuple<bool, T>>, IUniTaskSource
		{
			public WhenAnyLRPromise(UniTask<T> leftTask, UniTask rightTask)
			{
				UniTask<T>.Awaiter awaiter;
				try
				{
					awaiter = leftTask.GetAwaiter();
				}
				catch (Exception error)
				{
					this.core.TrySetException(error);
					goto IL_60;
				}
				if (awaiter.IsCompleted)
				{
					UniTask.WhenAnyLRPromise<T>.TryLeftInvokeContinuation(this, awaiter);
				}
				else
				{
					awaiter.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyLRPromise<T>, UniTask<T>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyLRPromise<T>, UniTask<T>.Awaiter>)state)
						{
							UniTask.WhenAnyLRPromise<T>.TryLeftInvokeContinuation(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyLRPromise<T>, UniTask<T>.Awaiter>(this, awaiter));
				}
				IL_60:
				UniTask.Awaiter awaiter2;
				try
				{
					awaiter2 = rightTask.GetAwaiter();
				}
				catch (Exception error2)
				{
					this.core.TrySetException(error2);
					return;
				}
				if (awaiter2.IsCompleted)
				{
					UniTask.WhenAnyLRPromise<T>.TryRightInvokeContinuation(this, awaiter2);
					return;
				}
				awaiter2.SourceOnCompleted(delegate(object state)
				{
					using (StateTuple<UniTask.WhenAnyLRPromise<T>, UniTask.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyLRPromise<T>, UniTask.Awaiter>)state)
					{
						UniTask.WhenAnyLRPromise<T>.TryRightInvokeContinuation(stateTuple.Item1, stateTuple.Item2);
					}
				}, StateTuple.Create<UniTask.WhenAnyLRPromise<T>, UniTask.Awaiter>(this, awaiter2));
			}

			private static void TryLeftInvokeContinuation(UniTask.WhenAnyLRPromise<T> self, in UniTask<T>.Awaiter awaiter)
			{
				T result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<bool, T>(true, result));
				}
			}

			private static void TryRightInvokeContinuation(UniTask.WhenAnyLRPromise<T> self, in UniTask.Awaiter awaiter)
			{
				try
				{
					awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<bool, T>(false, default(T)));
				}
			}

			public ValueTuple<bool, T> GetResult(short token)
			{
				GC.SuppressFinalize(this);
				return this.core.GetResult(token);
			}

			public UniTaskStatus GetStatus(short token)
			{
				return this.core.GetStatus(token);
			}

			public void OnCompleted(Action<object> continuation, object state, short token)
			{
				this.core.OnCompleted(continuation, state, token);
			}

			public UniTaskStatus UnsafeGetStatus()
			{
				return this.core.UnsafeGetStatus();
			}

			void IUniTaskSource.GetResult(short token)
			{
				this.GetResult(token);
			}

			private int completedCount;

			private UniTaskCompletionSourceCore<ValueTuple<bool, T>> core;
		}

		private sealed class WhenAnyPromise<T> : IUniTaskSource<ValueTuple<int, T>>, IUniTaskSource
		{
			public WhenAnyPromise(UniTask<T>[] tasks, int tasksLength)
			{
				if (tasksLength == 0)
				{
					throw new ArgumentException("The tasks argument contains no tasks.");
				}
				int i = 0;
				while (i < tasksLength)
				{
					UniTask<T>.Awaiter awaiter;
					try
					{
						awaiter = tasks[i].GetAwaiter();
					}
					catch (Exception error)
					{
						this.core.TrySetException(error);
						goto IL_7A;
					}
					goto IL_38;
					IL_7A:
					i++;
					continue;
					IL_38:
					if (awaiter.IsCompleted)
					{
						UniTask.WhenAnyPromise<T>.TryInvokeContinuation(this, awaiter, i);
						goto IL_7A;
					}
					awaiter.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T>, UniTask<T>.Awaiter, int> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T>, UniTask<T>.Awaiter, int>)state)
						{
							UniTask.WhenAnyPromise<T>.TryInvokeContinuation(stateTuple.Item1, stateTuple.Item2, stateTuple.Item3);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T>, UniTask<T>.Awaiter, int>(this, awaiter, i));
					goto IL_7A;
				}
			}

			private static void TryInvokeContinuation(UniTask.WhenAnyPromise<T> self, in UniTask<T>.Awaiter awaiter, int i)
			{
				T result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T>(i, result));
				}
			}

			public ValueTuple<int, T> GetResult(short token)
			{
				GC.SuppressFinalize(this);
				return this.core.GetResult(token);
			}

			public UniTaskStatus GetStatus(short token)
			{
				return this.core.GetStatus(token);
			}

			public void OnCompleted(Action<object> continuation, object state, short token)
			{
				this.core.OnCompleted(continuation, state, token);
			}

			public UniTaskStatus UnsafeGetStatus()
			{
				return this.core.UnsafeGetStatus();
			}

			void IUniTaskSource.GetResult(short token)
			{
				this.GetResult(token);
			}

			private int completedCount;

			private UniTaskCompletionSourceCore<ValueTuple<int, T>> core;
		}

		private sealed class WhenAnyPromise : IUniTaskSource<int>, IUniTaskSource
		{
			public WhenAnyPromise(UniTask[] tasks, int tasksLength)
			{
				if (tasksLength == 0)
				{
					throw new ArgumentException("The tasks argument contains no tasks.");
				}
				int i = 0;
				while (i < tasksLength)
				{
					UniTask.Awaiter awaiter;
					try
					{
						awaiter = tasks[i].GetAwaiter();
					}
					catch (Exception error)
					{
						this.core.TrySetException(error);
						goto IL_7A;
					}
					goto IL_38;
					IL_7A:
					i++;
					continue;
					IL_38:
					if (awaiter.IsCompleted)
					{
						UniTask.WhenAnyPromise.TryInvokeContinuation(this, awaiter, i);
						goto IL_7A;
					}
					awaiter.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise, UniTask.Awaiter, int> stateTuple = (StateTuple<UniTask.WhenAnyPromise, UniTask.Awaiter, int>)state)
						{
							UniTask.WhenAnyPromise.TryInvokeContinuation(stateTuple.Item1, stateTuple.Item2, stateTuple.Item3);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise, UniTask.Awaiter, int>(this, awaiter, i));
					goto IL_7A;
				}
			}

			private static void TryInvokeContinuation(UniTask.WhenAnyPromise self, in UniTask.Awaiter awaiter, int i)
			{
				try
				{
					awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(i);
				}
			}

			public int GetResult(short token)
			{
				GC.SuppressFinalize(this);
				return this.core.GetResult(token);
			}

			public UniTaskStatus GetStatus(short token)
			{
				return this.core.GetStatus(token);
			}

			public void OnCompleted(Action<object> continuation, object state, short token)
			{
				this.core.OnCompleted(continuation, state, token);
			}

			public UniTaskStatus UnsafeGetStatus()
			{
				return this.core.UnsafeGetStatus();
			}

			void IUniTaskSource.GetResult(short token)
			{
				this.GetResult(token);
			}

			private int completedCount;

			private UniTaskCompletionSourceCore<int> core;
		}

		private sealed class WhenAnyPromise<T1, T2> : IUniTaskSource<ValueTuple<int, T1, T2>>, IUniTaskSource
		{
			public WhenAnyPromise(UniTask<T1> task1, UniTask<T2> task2)
			{
				this.completedCount = 0;
				UniTask<T1>.Awaiter awaiter = task1.GetAwaiter();
				if (awaiter.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2>.TryInvokeContinuationT1(this, awaiter);
				}
				else
				{
					awaiter.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2>, UniTask<T1>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2>, UniTask<T1>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2>.TryInvokeContinuationT1(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2>, UniTask<T1>.Awaiter>(this, awaiter));
				}
				UniTask<T2>.Awaiter awaiter2 = task2.GetAwaiter();
				if (awaiter2.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2>.TryInvokeContinuationT2(this, awaiter2);
					return;
				}
				awaiter2.SourceOnCompleted(delegate(object state)
				{
					using (StateTuple<UniTask.WhenAnyPromise<T1, T2>, UniTask<T2>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2>, UniTask<T2>.Awaiter>)state)
					{
						UniTask.WhenAnyPromise<T1, T2>.TryInvokeContinuationT2(stateTuple.Item1, stateTuple.Item2);
					}
				}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2>, UniTask<T2>.Awaiter>(this, awaiter2));
			}

			private static void TryInvokeContinuationT1(UniTask.WhenAnyPromise<T1, T2> self, in UniTask<T1>.Awaiter awaiter)
			{
				T1 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2>(0, result, default(T2)));
				}
			}

			private static void TryInvokeContinuationT2(UniTask.WhenAnyPromise<T1, T2> self, in UniTask<T2>.Awaiter awaiter)
			{
				T2 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2>(1, default(T1), result));
				}
			}

			[return: TupleElementNames(new string[]
			{
				null,
				"result1",
				"result2"
			})]
			public ValueTuple<int, T1, T2> GetResult(short token)
			{
				GC.SuppressFinalize(this);
				return this.core.GetResult(token);
			}

			public UniTaskStatus GetStatus(short token)
			{
				return this.core.GetStatus(token);
			}

			public void OnCompleted(Action<object> continuation, object state, short token)
			{
				this.core.OnCompleted(continuation, state, token);
			}

			public UniTaskStatus UnsafeGetStatus()
			{
				return this.core.UnsafeGetStatus();
			}

			void IUniTaskSource.GetResult(short token)
			{
				this.GetResult(token);
			}

			private int completedCount;

			[TupleElementNames(new string[]
			{
				null,
				"result1",
				"result2"
			})]
			private UniTaskCompletionSourceCore<ValueTuple<int, T1, T2>> core;
		}

		private sealed class WhenAnyPromise<T1, T2, T3> : IUniTaskSource<ValueTuple<int, T1, T2, T3>>, IUniTaskSource
		{
			public WhenAnyPromise(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3)
			{
				this.completedCount = 0;
				UniTask<T1>.Awaiter awaiter = task1.GetAwaiter();
				if (awaiter.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3>.TryInvokeContinuationT1(this, awaiter);
				}
				else
				{
					awaiter.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3>, UniTask<T1>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3>, UniTask<T1>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3>.TryInvokeContinuationT1(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3>, UniTask<T1>.Awaiter>(this, awaiter));
				}
				UniTask<T2>.Awaiter awaiter2 = task2.GetAwaiter();
				if (awaiter2.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3>.TryInvokeContinuationT2(this, awaiter2);
				}
				else
				{
					awaiter2.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3>, UniTask<T2>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3>, UniTask<T2>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3>.TryInvokeContinuationT2(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3>, UniTask<T2>.Awaiter>(this, awaiter2));
				}
				UniTask<T3>.Awaiter awaiter3 = task3.GetAwaiter();
				if (awaiter3.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3>.TryInvokeContinuationT3(this, awaiter3);
					return;
				}
				awaiter3.SourceOnCompleted(delegate(object state)
				{
					using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3>, UniTask<T3>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3>, UniTask<T3>.Awaiter>)state)
					{
						UniTask.WhenAnyPromise<T1, T2, T3>.TryInvokeContinuationT3(stateTuple.Item1, stateTuple.Item2);
					}
				}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3>, UniTask<T3>.Awaiter>(this, awaiter3));
			}

			private static void TryInvokeContinuationT1(UniTask.WhenAnyPromise<T1, T2, T3> self, in UniTask<T1>.Awaiter awaiter)
			{
				T1 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3>(0, result, default(T2), default(T3)));
				}
			}

			private static void TryInvokeContinuationT2(UniTask.WhenAnyPromise<T1, T2, T3> self, in UniTask<T2>.Awaiter awaiter)
			{
				T2 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3>(1, default(T1), result, default(T3)));
				}
			}

			private static void TryInvokeContinuationT3(UniTask.WhenAnyPromise<T1, T2, T3> self, in UniTask<T3>.Awaiter awaiter)
			{
				T3 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3>(2, default(T1), default(T2), result));
				}
			}

			[return: TupleElementNames(new string[]
			{
				null,
				"result1",
				"result2",
				"result3"
			})]
			public ValueTuple<int, T1, T2, T3> GetResult(short token)
			{
				GC.SuppressFinalize(this);
				return this.core.GetResult(token);
			}

			public UniTaskStatus GetStatus(short token)
			{
				return this.core.GetStatus(token);
			}

			public void OnCompleted(Action<object> continuation, object state, short token)
			{
				this.core.OnCompleted(continuation, state, token);
			}

			public UniTaskStatus UnsafeGetStatus()
			{
				return this.core.UnsafeGetStatus();
			}

			void IUniTaskSource.GetResult(short token)
			{
				this.GetResult(token);
			}

			private int completedCount;

			[TupleElementNames(new string[]
			{
				null,
				"result1",
				"result2",
				"result3"
			})]
			private UniTaskCompletionSourceCore<ValueTuple<int, T1, T2, T3>> core;
		}

		private sealed class WhenAnyPromise<T1, T2, T3, T4> : IUniTaskSource<ValueTuple<int, T1, T2, T3, T4>>, IUniTaskSource
		{
			public WhenAnyPromise(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4)
			{
				this.completedCount = 0;
				UniTask<T1>.Awaiter awaiter = task1.GetAwaiter();
				if (awaiter.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4>.TryInvokeContinuationT1(this, awaiter);
				}
				else
				{
					awaiter.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4>, UniTask<T1>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4>, UniTask<T1>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4>.TryInvokeContinuationT1(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4>, UniTask<T1>.Awaiter>(this, awaiter));
				}
				UniTask<T2>.Awaiter awaiter2 = task2.GetAwaiter();
				if (awaiter2.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4>.TryInvokeContinuationT2(this, awaiter2);
				}
				else
				{
					awaiter2.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4>, UniTask<T2>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4>, UniTask<T2>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4>.TryInvokeContinuationT2(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4>, UniTask<T2>.Awaiter>(this, awaiter2));
				}
				UniTask<T3>.Awaiter awaiter3 = task3.GetAwaiter();
				if (awaiter3.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4>.TryInvokeContinuationT3(this, awaiter3);
				}
				else
				{
					awaiter3.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4>, UniTask<T3>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4>, UniTask<T3>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4>.TryInvokeContinuationT3(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4>, UniTask<T3>.Awaiter>(this, awaiter3));
				}
				UniTask<T4>.Awaiter awaiter4 = task4.GetAwaiter();
				if (awaiter4.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4>.TryInvokeContinuationT4(this, awaiter4);
					return;
				}
				awaiter4.SourceOnCompleted(delegate(object state)
				{
					using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4>, UniTask<T4>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4>, UniTask<T4>.Awaiter>)state)
					{
						UniTask.WhenAnyPromise<T1, T2, T3, T4>.TryInvokeContinuationT4(stateTuple.Item1, stateTuple.Item2);
					}
				}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4>, UniTask<T4>.Awaiter>(this, awaiter4));
			}

			private static void TryInvokeContinuationT1(UniTask.WhenAnyPromise<T1, T2, T3, T4> self, in UniTask<T1>.Awaiter awaiter)
			{
				T1 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4>(0, result, default(T2), default(T3), default(T4)));
				}
			}

			private static void TryInvokeContinuationT2(UniTask.WhenAnyPromise<T1, T2, T3, T4> self, in UniTask<T2>.Awaiter awaiter)
			{
				T2 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4>(1, default(T1), result, default(T3), default(T4)));
				}
			}

			private static void TryInvokeContinuationT3(UniTask.WhenAnyPromise<T1, T2, T3, T4> self, in UniTask<T3>.Awaiter awaiter)
			{
				T3 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4>(2, default(T1), default(T2), result, default(T4)));
				}
			}

			private static void TryInvokeContinuationT4(UniTask.WhenAnyPromise<T1, T2, T3, T4> self, in UniTask<T4>.Awaiter awaiter)
			{
				T4 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4>(3, default(T1), default(T2), default(T3), result));
				}
			}

			[return: TupleElementNames(new string[]
			{
				null,
				"result1",
				"result2",
				"result3",
				"result4"
			})]
			public ValueTuple<int, T1, T2, T3, T4> GetResult(short token)
			{
				GC.SuppressFinalize(this);
				return this.core.GetResult(token);
			}

			public UniTaskStatus GetStatus(short token)
			{
				return this.core.GetStatus(token);
			}

			public void OnCompleted(Action<object> continuation, object state, short token)
			{
				this.core.OnCompleted(continuation, state, token);
			}

			public UniTaskStatus UnsafeGetStatus()
			{
				return this.core.UnsafeGetStatus();
			}

			void IUniTaskSource.GetResult(short token)
			{
				this.GetResult(token);
			}

			private int completedCount;

			[TupleElementNames(new string[]
			{
				null,
				"result1",
				"result2",
				"result3",
				"result4"
			})]
			private UniTaskCompletionSourceCore<ValueTuple<int, T1, T2, T3, T4>> core;
		}

		private sealed class WhenAnyPromise<T1, T2, T3, T4, T5> : IUniTaskSource<ValueTuple<int, T1, T2, T3, T4, T5>>, IUniTaskSource
		{
			public WhenAnyPromise(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4, UniTask<T5> task5)
			{
				this.completedCount = 0;
				UniTask<T1>.Awaiter awaiter = task1.GetAwaiter();
				if (awaiter.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5>.TryInvokeContinuationT1(this, awaiter);
				}
				else
				{
					awaiter.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5>, UniTask<T1>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5>, UniTask<T1>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5>.TryInvokeContinuationT1(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5>, UniTask<T1>.Awaiter>(this, awaiter));
				}
				UniTask<T2>.Awaiter awaiter2 = task2.GetAwaiter();
				if (awaiter2.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5>.TryInvokeContinuationT2(this, awaiter2);
				}
				else
				{
					awaiter2.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5>, UniTask<T2>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5>, UniTask<T2>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5>.TryInvokeContinuationT2(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5>, UniTask<T2>.Awaiter>(this, awaiter2));
				}
				UniTask<T3>.Awaiter awaiter3 = task3.GetAwaiter();
				if (awaiter3.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5>.TryInvokeContinuationT3(this, awaiter3);
				}
				else
				{
					awaiter3.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5>, UniTask<T3>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5>, UniTask<T3>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5>.TryInvokeContinuationT3(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5>, UniTask<T3>.Awaiter>(this, awaiter3));
				}
				UniTask<T4>.Awaiter awaiter4 = task4.GetAwaiter();
				if (awaiter4.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5>.TryInvokeContinuationT4(this, awaiter4);
				}
				else
				{
					awaiter4.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5>, UniTask<T4>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5>, UniTask<T4>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5>.TryInvokeContinuationT4(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5>, UniTask<T4>.Awaiter>(this, awaiter4));
				}
				UniTask<T5>.Awaiter awaiter5 = task5.GetAwaiter();
				if (awaiter5.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5>.TryInvokeContinuationT5(this, awaiter5);
					return;
				}
				awaiter5.SourceOnCompleted(delegate(object state)
				{
					using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5>, UniTask<T5>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5>, UniTask<T5>.Awaiter>)state)
					{
						UniTask.WhenAnyPromise<T1, T2, T3, T4, T5>.TryInvokeContinuationT5(stateTuple.Item1, stateTuple.Item2);
					}
				}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5>, UniTask<T5>.Awaiter>(this, awaiter5));
			}

			private static void TryInvokeContinuationT1(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5> self, in UniTask<T1>.Awaiter awaiter)
			{
				T1 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5>(0, result, default(T2), default(T3), default(T4), default(T5)));
				}
			}

			private static void TryInvokeContinuationT2(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5> self, in UniTask<T2>.Awaiter awaiter)
			{
				T2 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5>(1, default(T1), result, default(T3), default(T4), default(T5)));
				}
			}

			private static void TryInvokeContinuationT3(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5> self, in UniTask<T3>.Awaiter awaiter)
			{
				T3 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5>(2, default(T1), default(T2), result, default(T4), default(T5)));
				}
			}

			private static void TryInvokeContinuationT4(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5> self, in UniTask<T4>.Awaiter awaiter)
			{
				T4 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5>(3, default(T1), default(T2), default(T3), result, default(T5)));
				}
			}

			private static void TryInvokeContinuationT5(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5> self, in UniTask<T5>.Awaiter awaiter)
			{
				T5 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5>(4, default(T1), default(T2), default(T3), default(T4), result));
				}
			}

			[return: TupleElementNames(new string[]
			{
				null,
				"result1",
				"result2",
				"result3",
				"result4",
				"result5"
			})]
			public ValueTuple<int, T1, T2, T3, T4, T5> GetResult(short token)
			{
				GC.SuppressFinalize(this);
				return this.core.GetResult(token);
			}

			public UniTaskStatus GetStatus(short token)
			{
				return this.core.GetStatus(token);
			}

			public void OnCompleted(Action<object> continuation, object state, short token)
			{
				this.core.OnCompleted(continuation, state, token);
			}

			public UniTaskStatus UnsafeGetStatus()
			{
				return this.core.UnsafeGetStatus();
			}

			void IUniTaskSource.GetResult(short token)
			{
				this.GetResult(token);
			}

			private int completedCount;

			[TupleElementNames(new string[]
			{
				null,
				"result1",
				"result2",
				"result3",
				"result4",
				"result5"
			})]
			private UniTaskCompletionSourceCore<ValueTuple<int, T1, T2, T3, T4, T5>> core;
		}

		private sealed class WhenAnyPromise<T1, T2, T3, T4, T5, T6> : IUniTaskSource<ValueTuple<int, T1, T2, T3, T4, T5, T6>>, IUniTaskSource
		{
			public WhenAnyPromise(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4, UniTask<T5> task5, UniTask<T6> task6)
			{
				this.completedCount = 0;
				UniTask<T1>.Awaiter awaiter = task1.GetAwaiter();
				if (awaiter.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6>.TryInvokeContinuationT1(this, awaiter);
				}
				else
				{
					awaiter.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6>, UniTask<T1>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6>, UniTask<T1>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6>.TryInvokeContinuationT1(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6>, UniTask<T1>.Awaiter>(this, awaiter));
				}
				UniTask<T2>.Awaiter awaiter2 = task2.GetAwaiter();
				if (awaiter2.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6>.TryInvokeContinuationT2(this, awaiter2);
				}
				else
				{
					awaiter2.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6>, UniTask<T2>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6>, UniTask<T2>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6>.TryInvokeContinuationT2(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6>, UniTask<T2>.Awaiter>(this, awaiter2));
				}
				UniTask<T3>.Awaiter awaiter3 = task3.GetAwaiter();
				if (awaiter3.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6>.TryInvokeContinuationT3(this, awaiter3);
				}
				else
				{
					awaiter3.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6>, UniTask<T3>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6>, UniTask<T3>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6>.TryInvokeContinuationT3(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6>, UniTask<T3>.Awaiter>(this, awaiter3));
				}
				UniTask<T4>.Awaiter awaiter4 = task4.GetAwaiter();
				if (awaiter4.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6>.TryInvokeContinuationT4(this, awaiter4);
				}
				else
				{
					awaiter4.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6>, UniTask<T4>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6>, UniTask<T4>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6>.TryInvokeContinuationT4(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6>, UniTask<T4>.Awaiter>(this, awaiter4));
				}
				UniTask<T5>.Awaiter awaiter5 = task5.GetAwaiter();
				if (awaiter5.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6>.TryInvokeContinuationT5(this, awaiter5);
				}
				else
				{
					awaiter5.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6>, UniTask<T5>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6>, UniTask<T5>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6>.TryInvokeContinuationT5(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6>, UniTask<T5>.Awaiter>(this, awaiter5));
				}
				UniTask<T6>.Awaiter awaiter6 = task6.GetAwaiter();
				if (awaiter6.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6>.TryInvokeContinuationT6(this, awaiter6);
					return;
				}
				awaiter6.SourceOnCompleted(delegate(object state)
				{
					using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6>, UniTask<T6>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6>, UniTask<T6>.Awaiter>)state)
					{
						UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6>.TryInvokeContinuationT6(stateTuple.Item1, stateTuple.Item2);
					}
				}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6>, UniTask<T6>.Awaiter>(this, awaiter6));
			}

			private static void TryInvokeContinuationT1(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6> self, in UniTask<T1>.Awaiter awaiter)
			{
				T1 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6>(0, result, default(T2), default(T3), default(T4), default(T5), default(T6)));
				}
			}

			private static void TryInvokeContinuationT2(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6> self, in UniTask<T2>.Awaiter awaiter)
			{
				T2 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6>(1, default(T1), result, default(T3), default(T4), default(T5), default(T6)));
				}
			}

			private static void TryInvokeContinuationT3(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6> self, in UniTask<T3>.Awaiter awaiter)
			{
				T3 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6>(2, default(T1), default(T2), result, default(T4), default(T5), default(T6)));
				}
			}

			private static void TryInvokeContinuationT4(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6> self, in UniTask<T4>.Awaiter awaiter)
			{
				T4 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6>(3, default(T1), default(T2), default(T3), result, default(T5), default(T6)));
				}
			}

			private static void TryInvokeContinuationT5(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6> self, in UniTask<T5>.Awaiter awaiter)
			{
				T5 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6>(4, default(T1), default(T2), default(T3), default(T4), result, default(T6)));
				}
			}

			private static void TryInvokeContinuationT6(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6> self, in UniTask<T6>.Awaiter awaiter)
			{
				T6 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6>(5, default(T1), default(T2), default(T3), default(T4), default(T5), result));
				}
			}

			[return: TupleElementNames(new string[]
			{
				null,
				"result1",
				"result2",
				"result3",
				"result4",
				"result5",
				"result6"
			})]
			public ValueTuple<int, T1, T2, T3, T4, T5, T6> GetResult(short token)
			{
				GC.SuppressFinalize(this);
				return this.core.GetResult(token);
			}

			public UniTaskStatus GetStatus(short token)
			{
				return this.core.GetStatus(token);
			}

			public void OnCompleted(Action<object> continuation, object state, short token)
			{
				this.core.OnCompleted(continuation, state, token);
			}

			public UniTaskStatus UnsafeGetStatus()
			{
				return this.core.UnsafeGetStatus();
			}

			void IUniTaskSource.GetResult(short token)
			{
				this.GetResult(token);
			}

			private int completedCount;

			[TupleElementNames(new string[]
			{
				null,
				"result1",
				"result2",
				"result3",
				"result4",
				"result5",
				"result6"
			})]
			private UniTaskCompletionSourceCore<ValueTuple<int, T1, T2, T3, T4, T5, T6>> core;
		}

		private sealed class WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7> : IUniTaskSource<ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7>>>, IUniTaskSource
		{
			public WhenAnyPromise(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4, UniTask<T5> task5, UniTask<T6> task6, UniTask<T7> task7)
			{
				this.completedCount = 0;
				UniTask<T1>.Awaiter awaiter = task1.GetAwaiter();
				if (awaiter.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7>.TryInvokeContinuationT1(this, awaiter);
				}
				else
				{
					awaiter.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7>, UniTask<T1>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7>, UniTask<T1>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7>.TryInvokeContinuationT1(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7>, UniTask<T1>.Awaiter>(this, awaiter));
				}
				UniTask<T2>.Awaiter awaiter2 = task2.GetAwaiter();
				if (awaiter2.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7>.TryInvokeContinuationT2(this, awaiter2);
				}
				else
				{
					awaiter2.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7>, UniTask<T2>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7>, UniTask<T2>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7>.TryInvokeContinuationT2(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7>, UniTask<T2>.Awaiter>(this, awaiter2));
				}
				UniTask<T3>.Awaiter awaiter3 = task3.GetAwaiter();
				if (awaiter3.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7>.TryInvokeContinuationT3(this, awaiter3);
				}
				else
				{
					awaiter3.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7>, UniTask<T3>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7>, UniTask<T3>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7>.TryInvokeContinuationT3(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7>, UniTask<T3>.Awaiter>(this, awaiter3));
				}
				UniTask<T4>.Awaiter awaiter4 = task4.GetAwaiter();
				if (awaiter4.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7>.TryInvokeContinuationT4(this, awaiter4);
				}
				else
				{
					awaiter4.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7>, UniTask<T4>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7>, UniTask<T4>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7>.TryInvokeContinuationT4(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7>, UniTask<T4>.Awaiter>(this, awaiter4));
				}
				UniTask<T5>.Awaiter awaiter5 = task5.GetAwaiter();
				if (awaiter5.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7>.TryInvokeContinuationT5(this, awaiter5);
				}
				else
				{
					awaiter5.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7>, UniTask<T5>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7>, UniTask<T5>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7>.TryInvokeContinuationT5(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7>, UniTask<T5>.Awaiter>(this, awaiter5));
				}
				UniTask<T6>.Awaiter awaiter6 = task6.GetAwaiter();
				if (awaiter6.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7>.TryInvokeContinuationT6(this, awaiter6);
				}
				else
				{
					awaiter6.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7>, UniTask<T6>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7>, UniTask<T6>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7>.TryInvokeContinuationT6(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7>, UniTask<T6>.Awaiter>(this, awaiter6));
				}
				UniTask<T7>.Awaiter awaiter7 = task7.GetAwaiter();
				if (awaiter7.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7>.TryInvokeContinuationT7(this, awaiter7);
					return;
				}
				awaiter7.SourceOnCompleted(delegate(object state)
				{
					using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7>, UniTask<T7>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7>, UniTask<T7>.Awaiter>)state)
					{
						UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7>.TryInvokeContinuationT7(stateTuple.Item1, stateTuple.Item2);
					}
				}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7>, UniTask<T7>.Awaiter>(this, awaiter7));
			}

			private static void TryInvokeContinuationT1(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7> self, in UniTask<T1>.Awaiter awaiter)
			{
				T1 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7>>(0, result, default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7>(default(T7))));
				}
			}

			private static void TryInvokeContinuationT2(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7> self, in UniTask<T2>.Awaiter awaiter)
			{
				T2 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7>>(1, default(T1), result, default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7>(default(T7))));
				}
			}

			private static void TryInvokeContinuationT3(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7> self, in UniTask<T3>.Awaiter awaiter)
			{
				T3 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7>>(2, default(T1), default(T2), result, default(T4), default(T5), default(T6), new ValueTuple<T7>(default(T7))));
				}
			}

			private static void TryInvokeContinuationT4(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7> self, in UniTask<T4>.Awaiter awaiter)
			{
				T4 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7>>(3, default(T1), default(T2), default(T3), result, default(T5), default(T6), new ValueTuple<T7>(default(T7))));
				}
			}

			private static void TryInvokeContinuationT5(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7> self, in UniTask<T5>.Awaiter awaiter)
			{
				T5 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7>>(4, default(T1), default(T2), default(T3), default(T4), result, default(T6), new ValueTuple<T7>(default(T7))));
				}
			}

			private static void TryInvokeContinuationT6(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7> self, in UniTask<T6>.Awaiter awaiter)
			{
				T6 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7>>(5, default(T1), default(T2), default(T3), default(T4), default(T5), result, new ValueTuple<T7>(default(T7))));
				}
			}

			private static void TryInvokeContinuationT7(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7> self, in UniTask<T7>.Awaiter awaiter)
			{
				T7 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7>>(6, default(T1), default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7>(result)));
				}
			}

			[return: TupleElementNames(new string[]
			{
				null,
				"result1",
				"result2",
				"result3",
				"result4",
				"result5",
				"result6",
				"result7",
				null
			})]
			public ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7>> GetResult(short token)
			{
				GC.SuppressFinalize(this);
				return this.core.GetResult(token);
			}

			public UniTaskStatus GetStatus(short token)
			{
				return this.core.GetStatus(token);
			}

			public void OnCompleted(Action<object> continuation, object state, short token)
			{
				this.core.OnCompleted(continuation, state, token);
			}

			public UniTaskStatus UnsafeGetStatus()
			{
				return this.core.UnsafeGetStatus();
			}

			void IUniTaskSource.GetResult(short token)
			{
				this.GetResult(token);
			}

			private int completedCount;

			[TupleElementNames(new string[]
			{
				null,
				"result1",
				"result2",
				"result3",
				"result4",
				"result5",
				"result6",
				"result7",
				null
			})]
			private UniTaskCompletionSourceCore<ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7>>> core;
		}

		private sealed class WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8> : IUniTaskSource<ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8>>>, IUniTaskSource
		{
			public WhenAnyPromise(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4, UniTask<T5> task5, UniTask<T6> task6, UniTask<T7> task7, UniTask<T8> task8)
			{
				this.completedCount = 0;
				UniTask<T1>.Awaiter awaiter = task1.GetAwaiter();
				if (awaiter.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8>.TryInvokeContinuationT1(this, awaiter);
				}
				else
				{
					awaiter.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8>, UniTask<T1>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8>, UniTask<T1>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8>.TryInvokeContinuationT1(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8>, UniTask<T1>.Awaiter>(this, awaiter));
				}
				UniTask<T2>.Awaiter awaiter2 = task2.GetAwaiter();
				if (awaiter2.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8>.TryInvokeContinuationT2(this, awaiter2);
				}
				else
				{
					awaiter2.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8>, UniTask<T2>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8>, UniTask<T2>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8>.TryInvokeContinuationT2(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8>, UniTask<T2>.Awaiter>(this, awaiter2));
				}
				UniTask<T3>.Awaiter awaiter3 = task3.GetAwaiter();
				if (awaiter3.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8>.TryInvokeContinuationT3(this, awaiter3);
				}
				else
				{
					awaiter3.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8>, UniTask<T3>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8>, UniTask<T3>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8>.TryInvokeContinuationT3(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8>, UniTask<T3>.Awaiter>(this, awaiter3));
				}
				UniTask<T4>.Awaiter awaiter4 = task4.GetAwaiter();
				if (awaiter4.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8>.TryInvokeContinuationT4(this, awaiter4);
				}
				else
				{
					awaiter4.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8>, UniTask<T4>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8>, UniTask<T4>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8>.TryInvokeContinuationT4(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8>, UniTask<T4>.Awaiter>(this, awaiter4));
				}
				UniTask<T5>.Awaiter awaiter5 = task5.GetAwaiter();
				if (awaiter5.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8>.TryInvokeContinuationT5(this, awaiter5);
				}
				else
				{
					awaiter5.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8>, UniTask<T5>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8>, UniTask<T5>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8>.TryInvokeContinuationT5(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8>, UniTask<T5>.Awaiter>(this, awaiter5));
				}
				UniTask<T6>.Awaiter awaiter6 = task6.GetAwaiter();
				if (awaiter6.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8>.TryInvokeContinuationT6(this, awaiter6);
				}
				else
				{
					awaiter6.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8>, UniTask<T6>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8>, UniTask<T6>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8>.TryInvokeContinuationT6(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8>, UniTask<T6>.Awaiter>(this, awaiter6));
				}
				UniTask<T7>.Awaiter awaiter7 = task7.GetAwaiter();
				if (awaiter7.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8>.TryInvokeContinuationT7(this, awaiter7);
				}
				else
				{
					awaiter7.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8>, UniTask<T7>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8>, UniTask<T7>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8>.TryInvokeContinuationT7(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8>, UniTask<T7>.Awaiter>(this, awaiter7));
				}
				UniTask<T8>.Awaiter awaiter8 = task8.GetAwaiter();
				if (awaiter8.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8>.TryInvokeContinuationT8(this, awaiter8);
					return;
				}
				awaiter8.SourceOnCompleted(delegate(object state)
				{
					using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8>, UniTask<T8>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8>, UniTask<T8>.Awaiter>)state)
					{
						UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8>.TryInvokeContinuationT8(stateTuple.Item1, stateTuple.Item2);
					}
				}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8>, UniTask<T8>.Awaiter>(this, awaiter8));
			}

			private static void TryInvokeContinuationT1(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8> self, in UniTask<T1>.Awaiter awaiter)
			{
				T1 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8>>(0, result, default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8>(default(T7), default(T8))));
				}
			}

			private static void TryInvokeContinuationT2(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8> self, in UniTask<T2>.Awaiter awaiter)
			{
				T2 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8>>(1, default(T1), result, default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8>(default(T7), default(T8))));
				}
			}

			private static void TryInvokeContinuationT3(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8> self, in UniTask<T3>.Awaiter awaiter)
			{
				T3 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8>>(2, default(T1), default(T2), result, default(T4), default(T5), default(T6), new ValueTuple<T7, T8>(default(T7), default(T8))));
				}
			}

			private static void TryInvokeContinuationT4(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8> self, in UniTask<T4>.Awaiter awaiter)
			{
				T4 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8>>(3, default(T1), default(T2), default(T3), result, default(T5), default(T6), new ValueTuple<T7, T8>(default(T7), default(T8))));
				}
			}

			private static void TryInvokeContinuationT5(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8> self, in UniTask<T5>.Awaiter awaiter)
			{
				T5 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8>>(4, default(T1), default(T2), default(T3), default(T4), result, default(T6), new ValueTuple<T7, T8>(default(T7), default(T8))));
				}
			}

			private static void TryInvokeContinuationT6(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8> self, in UniTask<T6>.Awaiter awaiter)
			{
				T6 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8>>(5, default(T1), default(T2), default(T3), default(T4), default(T5), result, new ValueTuple<T7, T8>(default(T7), default(T8))));
				}
			}

			private static void TryInvokeContinuationT7(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8> self, in UniTask<T7>.Awaiter awaiter)
			{
				T7 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8>>(6, default(T1), default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8>(result, default(T8))));
				}
			}

			private static void TryInvokeContinuationT8(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8> self, in UniTask<T8>.Awaiter awaiter)
			{
				T8 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8>>(7, default(T1), default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8>(default(T7), result)));
				}
			}

			[return: TupleElementNames(new string[]
			{
				null,
				"result1",
				"result2",
				"result3",
				"result4",
				"result5",
				"result6",
				"result7",
				"result8",
				null,
				null
			})]
			public ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8>> GetResult(short token)
			{
				GC.SuppressFinalize(this);
				return this.core.GetResult(token);
			}

			public UniTaskStatus GetStatus(short token)
			{
				return this.core.GetStatus(token);
			}

			public void OnCompleted(Action<object> continuation, object state, short token)
			{
				this.core.OnCompleted(continuation, state, token);
			}

			public UniTaskStatus UnsafeGetStatus()
			{
				return this.core.UnsafeGetStatus();
			}

			void IUniTaskSource.GetResult(short token)
			{
				this.GetResult(token);
			}

			private int completedCount;

			[TupleElementNames(new string[]
			{
				null,
				"result1",
				"result2",
				"result3",
				"result4",
				"result5",
				"result6",
				"result7",
				"result8",
				null,
				null
			})]
			private UniTaskCompletionSourceCore<ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8>>> core;
		}

		private sealed class WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9> : IUniTaskSource<ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9>>>, IUniTaskSource
		{
			public WhenAnyPromise(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4, UniTask<T5> task5, UniTask<T6> task6, UniTask<T7> task7, UniTask<T8> task8, UniTask<T9> task9)
			{
				this.completedCount = 0;
				UniTask<T1>.Awaiter awaiter = task1.GetAwaiter();
				if (awaiter.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>.TryInvokeContinuationT1(this, awaiter);
				}
				else
				{
					awaiter.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T1>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T1>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>.TryInvokeContinuationT1(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T1>.Awaiter>(this, awaiter));
				}
				UniTask<T2>.Awaiter awaiter2 = task2.GetAwaiter();
				if (awaiter2.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>.TryInvokeContinuationT2(this, awaiter2);
				}
				else
				{
					awaiter2.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T2>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T2>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>.TryInvokeContinuationT2(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T2>.Awaiter>(this, awaiter2));
				}
				UniTask<T3>.Awaiter awaiter3 = task3.GetAwaiter();
				if (awaiter3.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>.TryInvokeContinuationT3(this, awaiter3);
				}
				else
				{
					awaiter3.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T3>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T3>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>.TryInvokeContinuationT3(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T3>.Awaiter>(this, awaiter3));
				}
				UniTask<T4>.Awaiter awaiter4 = task4.GetAwaiter();
				if (awaiter4.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>.TryInvokeContinuationT4(this, awaiter4);
				}
				else
				{
					awaiter4.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T4>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T4>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>.TryInvokeContinuationT4(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T4>.Awaiter>(this, awaiter4));
				}
				UniTask<T5>.Awaiter awaiter5 = task5.GetAwaiter();
				if (awaiter5.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>.TryInvokeContinuationT5(this, awaiter5);
				}
				else
				{
					awaiter5.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T5>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T5>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>.TryInvokeContinuationT5(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T5>.Awaiter>(this, awaiter5));
				}
				UniTask<T6>.Awaiter awaiter6 = task6.GetAwaiter();
				if (awaiter6.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>.TryInvokeContinuationT6(this, awaiter6);
				}
				else
				{
					awaiter6.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T6>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T6>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>.TryInvokeContinuationT6(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T6>.Awaiter>(this, awaiter6));
				}
				UniTask<T7>.Awaiter awaiter7 = task7.GetAwaiter();
				if (awaiter7.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>.TryInvokeContinuationT7(this, awaiter7);
				}
				else
				{
					awaiter7.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T7>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T7>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>.TryInvokeContinuationT7(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T7>.Awaiter>(this, awaiter7));
				}
				UniTask<T8>.Awaiter awaiter8 = task8.GetAwaiter();
				if (awaiter8.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>.TryInvokeContinuationT8(this, awaiter8);
				}
				else
				{
					awaiter8.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T8>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T8>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>.TryInvokeContinuationT8(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T8>.Awaiter>(this, awaiter8));
				}
				UniTask<T9>.Awaiter awaiter9 = task9.GetAwaiter();
				if (awaiter9.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>.TryInvokeContinuationT9(this, awaiter9);
					return;
				}
				awaiter9.SourceOnCompleted(delegate(object state)
				{
					using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T9>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T9>.Awaiter>)state)
					{
						UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>.TryInvokeContinuationT9(stateTuple.Item1, stateTuple.Item2);
					}
				}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, UniTask<T9>.Awaiter>(this, awaiter9));
			}

			private static void TryInvokeContinuationT1(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9> self, in UniTask<T1>.Awaiter awaiter)
			{
				T1 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9>>(0, result, default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9>(default(T7), default(T8), default(T9))));
				}
			}

			private static void TryInvokeContinuationT2(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9> self, in UniTask<T2>.Awaiter awaiter)
			{
				T2 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9>>(1, default(T1), result, default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9>(default(T7), default(T8), default(T9))));
				}
			}

			private static void TryInvokeContinuationT3(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9> self, in UniTask<T3>.Awaiter awaiter)
			{
				T3 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9>>(2, default(T1), default(T2), result, default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9>(default(T7), default(T8), default(T9))));
				}
			}

			private static void TryInvokeContinuationT4(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9> self, in UniTask<T4>.Awaiter awaiter)
			{
				T4 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9>>(3, default(T1), default(T2), default(T3), result, default(T5), default(T6), new ValueTuple<T7, T8, T9>(default(T7), default(T8), default(T9))));
				}
			}

			private static void TryInvokeContinuationT5(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9> self, in UniTask<T5>.Awaiter awaiter)
			{
				T5 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9>>(4, default(T1), default(T2), default(T3), default(T4), result, default(T6), new ValueTuple<T7, T8, T9>(default(T7), default(T8), default(T9))));
				}
			}

			private static void TryInvokeContinuationT6(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9> self, in UniTask<T6>.Awaiter awaiter)
			{
				T6 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9>>(5, default(T1), default(T2), default(T3), default(T4), default(T5), result, new ValueTuple<T7, T8, T9>(default(T7), default(T8), default(T9))));
				}
			}

			private static void TryInvokeContinuationT7(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9> self, in UniTask<T7>.Awaiter awaiter)
			{
				T7 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9>>(6, default(T1), default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9>(result, default(T8), default(T9))));
				}
			}

			private static void TryInvokeContinuationT8(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9> self, in UniTask<T8>.Awaiter awaiter)
			{
				T8 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9>>(7, default(T1), default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9>(default(T7), result, default(T9))));
				}
			}

			private static void TryInvokeContinuationT9(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9> self, in UniTask<T9>.Awaiter awaiter)
			{
				T9 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9>>(8, default(T1), default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9>(default(T7), default(T8), result)));
				}
			}

			[return: TupleElementNames(new string[]
			{
				null,
				"result1",
				"result2",
				"result3",
				"result4",
				"result5",
				"result6",
				"result7",
				"result8",
				"result9",
				null,
				null,
				null
			})]
			public ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9>> GetResult(short token)
			{
				GC.SuppressFinalize(this);
				return this.core.GetResult(token);
			}

			public UniTaskStatus GetStatus(short token)
			{
				return this.core.GetStatus(token);
			}

			public void OnCompleted(Action<object> continuation, object state, short token)
			{
				this.core.OnCompleted(continuation, state, token);
			}

			public UniTaskStatus UnsafeGetStatus()
			{
				return this.core.UnsafeGetStatus();
			}

			void IUniTaskSource.GetResult(short token)
			{
				this.GetResult(token);
			}

			private int completedCount;

			[TupleElementNames(new string[]
			{
				null,
				"result1",
				"result2",
				"result3",
				"result4",
				"result5",
				"result6",
				"result7",
				"result8",
				"result9",
				null,
				null,
				null
			})]
			private UniTaskCompletionSourceCore<ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9>>> core;
		}

		private sealed class WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : IUniTaskSource<ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10>>>, IUniTaskSource
		{
			public WhenAnyPromise(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4, UniTask<T5> task5, UniTask<T6> task6, UniTask<T7> task7, UniTask<T8> task8, UniTask<T9> task9, UniTask<T10> task10)
			{
				this.completedCount = 0;
				UniTask<T1>.Awaiter awaiter = task1.GetAwaiter();
				if (awaiter.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.TryInvokeContinuationT1(this, awaiter);
				}
				else
				{
					awaiter.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T1>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T1>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.TryInvokeContinuationT1(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T1>.Awaiter>(this, awaiter));
				}
				UniTask<T2>.Awaiter awaiter2 = task2.GetAwaiter();
				if (awaiter2.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.TryInvokeContinuationT2(this, awaiter2);
				}
				else
				{
					awaiter2.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T2>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T2>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.TryInvokeContinuationT2(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T2>.Awaiter>(this, awaiter2));
				}
				UniTask<T3>.Awaiter awaiter3 = task3.GetAwaiter();
				if (awaiter3.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.TryInvokeContinuationT3(this, awaiter3);
				}
				else
				{
					awaiter3.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T3>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T3>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.TryInvokeContinuationT3(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T3>.Awaiter>(this, awaiter3));
				}
				UniTask<T4>.Awaiter awaiter4 = task4.GetAwaiter();
				if (awaiter4.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.TryInvokeContinuationT4(this, awaiter4);
				}
				else
				{
					awaiter4.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T4>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T4>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.TryInvokeContinuationT4(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T4>.Awaiter>(this, awaiter4));
				}
				UniTask<T5>.Awaiter awaiter5 = task5.GetAwaiter();
				if (awaiter5.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.TryInvokeContinuationT5(this, awaiter5);
				}
				else
				{
					awaiter5.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T5>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T5>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.TryInvokeContinuationT5(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T5>.Awaiter>(this, awaiter5));
				}
				UniTask<T6>.Awaiter awaiter6 = task6.GetAwaiter();
				if (awaiter6.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.TryInvokeContinuationT6(this, awaiter6);
				}
				else
				{
					awaiter6.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T6>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T6>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.TryInvokeContinuationT6(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T6>.Awaiter>(this, awaiter6));
				}
				UniTask<T7>.Awaiter awaiter7 = task7.GetAwaiter();
				if (awaiter7.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.TryInvokeContinuationT7(this, awaiter7);
				}
				else
				{
					awaiter7.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T7>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T7>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.TryInvokeContinuationT7(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T7>.Awaiter>(this, awaiter7));
				}
				UniTask<T8>.Awaiter awaiter8 = task8.GetAwaiter();
				if (awaiter8.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.TryInvokeContinuationT8(this, awaiter8);
				}
				else
				{
					awaiter8.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T8>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T8>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.TryInvokeContinuationT8(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T8>.Awaiter>(this, awaiter8));
				}
				UniTask<T9>.Awaiter awaiter9 = task9.GetAwaiter();
				if (awaiter9.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.TryInvokeContinuationT9(this, awaiter9);
				}
				else
				{
					awaiter9.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T9>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T9>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.TryInvokeContinuationT9(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T9>.Awaiter>(this, awaiter9));
				}
				UniTask<T10>.Awaiter awaiter10 = task10.GetAwaiter();
				if (awaiter10.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.TryInvokeContinuationT10(this, awaiter10);
					return;
				}
				awaiter10.SourceOnCompleted(delegate(object state)
				{
					using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T10>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T10>.Awaiter>)state)
					{
						UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.TryInvokeContinuationT10(stateTuple.Item1, stateTuple.Item2);
					}
				}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, UniTask<T10>.Awaiter>(this, awaiter10));
			}

			private static void TryInvokeContinuationT1(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> self, in UniTask<T1>.Awaiter awaiter)
			{
				T1 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10>>(0, result, default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10>(default(T7), default(T8), default(T9), default(T10))));
				}
			}

			private static void TryInvokeContinuationT2(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> self, in UniTask<T2>.Awaiter awaiter)
			{
				T2 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10>>(1, default(T1), result, default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10>(default(T7), default(T8), default(T9), default(T10))));
				}
			}

			private static void TryInvokeContinuationT3(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> self, in UniTask<T3>.Awaiter awaiter)
			{
				T3 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10>>(2, default(T1), default(T2), result, default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10>(default(T7), default(T8), default(T9), default(T10))));
				}
			}

			private static void TryInvokeContinuationT4(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> self, in UniTask<T4>.Awaiter awaiter)
			{
				T4 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10>>(3, default(T1), default(T2), default(T3), result, default(T5), default(T6), new ValueTuple<T7, T8, T9, T10>(default(T7), default(T8), default(T9), default(T10))));
				}
			}

			private static void TryInvokeContinuationT5(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> self, in UniTask<T5>.Awaiter awaiter)
			{
				T5 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10>>(4, default(T1), default(T2), default(T3), default(T4), result, default(T6), new ValueTuple<T7, T8, T9, T10>(default(T7), default(T8), default(T9), default(T10))));
				}
			}

			private static void TryInvokeContinuationT6(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> self, in UniTask<T6>.Awaiter awaiter)
			{
				T6 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10>>(5, default(T1), default(T2), default(T3), default(T4), default(T5), result, new ValueTuple<T7, T8, T9, T10>(default(T7), default(T8), default(T9), default(T10))));
				}
			}

			private static void TryInvokeContinuationT7(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> self, in UniTask<T7>.Awaiter awaiter)
			{
				T7 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10>>(6, default(T1), default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10>(result, default(T8), default(T9), default(T10))));
				}
			}

			private static void TryInvokeContinuationT8(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> self, in UniTask<T8>.Awaiter awaiter)
			{
				T8 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10>>(7, default(T1), default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10>(default(T7), result, default(T9), default(T10))));
				}
			}

			private static void TryInvokeContinuationT9(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> self, in UniTask<T9>.Awaiter awaiter)
			{
				T9 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10>>(8, default(T1), default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10>(default(T7), default(T8), result, default(T10))));
				}
			}

			private static void TryInvokeContinuationT10(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> self, in UniTask<T10>.Awaiter awaiter)
			{
				T10 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10>>(9, default(T1), default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10>(default(T7), default(T8), default(T9), result)));
				}
			}

			[return: TupleElementNames(new string[]
			{
				null,
				"result1",
				"result2",
				"result3",
				"result4",
				"result5",
				"result6",
				"result7",
				"result8",
				"result9",
				"result10",
				null,
				null,
				null,
				null
			})]
			public ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10>> GetResult(short token)
			{
				GC.SuppressFinalize(this);
				return this.core.GetResult(token);
			}

			public UniTaskStatus GetStatus(short token)
			{
				return this.core.GetStatus(token);
			}

			public void OnCompleted(Action<object> continuation, object state, short token)
			{
				this.core.OnCompleted(continuation, state, token);
			}

			public UniTaskStatus UnsafeGetStatus()
			{
				return this.core.UnsafeGetStatus();
			}

			void IUniTaskSource.GetResult(short token)
			{
				this.GetResult(token);
			}

			private int completedCount;

			[TupleElementNames(new string[]
			{
				null,
				"result1",
				"result2",
				"result3",
				"result4",
				"result5",
				"result6",
				"result7",
				"result8",
				"result9",
				"result10",
				null,
				null,
				null,
				null
			})]
			private UniTaskCompletionSourceCore<ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10>>> core;
		}

		private sealed class WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : IUniTaskSource<ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11>>>, IUniTaskSource
		{
			public WhenAnyPromise(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4, UniTask<T5> task5, UniTask<T6> task6, UniTask<T7> task7, UniTask<T8> task8, UniTask<T9> task9, UniTask<T10> task10, UniTask<T11> task11)
			{
				this.completedCount = 0;
				UniTask<T1>.Awaiter awaiter = task1.GetAwaiter();
				if (awaiter.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.TryInvokeContinuationT1(this, awaiter);
				}
				else
				{
					awaiter.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T1>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T1>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.TryInvokeContinuationT1(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T1>.Awaiter>(this, awaiter));
				}
				UniTask<T2>.Awaiter awaiter2 = task2.GetAwaiter();
				if (awaiter2.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.TryInvokeContinuationT2(this, awaiter2);
				}
				else
				{
					awaiter2.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T2>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T2>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.TryInvokeContinuationT2(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T2>.Awaiter>(this, awaiter2));
				}
				UniTask<T3>.Awaiter awaiter3 = task3.GetAwaiter();
				if (awaiter3.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.TryInvokeContinuationT3(this, awaiter3);
				}
				else
				{
					awaiter3.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T3>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T3>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.TryInvokeContinuationT3(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T3>.Awaiter>(this, awaiter3));
				}
				UniTask<T4>.Awaiter awaiter4 = task4.GetAwaiter();
				if (awaiter4.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.TryInvokeContinuationT4(this, awaiter4);
				}
				else
				{
					awaiter4.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T4>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T4>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.TryInvokeContinuationT4(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T4>.Awaiter>(this, awaiter4));
				}
				UniTask<T5>.Awaiter awaiter5 = task5.GetAwaiter();
				if (awaiter5.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.TryInvokeContinuationT5(this, awaiter5);
				}
				else
				{
					awaiter5.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T5>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T5>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.TryInvokeContinuationT5(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T5>.Awaiter>(this, awaiter5));
				}
				UniTask<T6>.Awaiter awaiter6 = task6.GetAwaiter();
				if (awaiter6.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.TryInvokeContinuationT6(this, awaiter6);
				}
				else
				{
					awaiter6.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T6>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T6>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.TryInvokeContinuationT6(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T6>.Awaiter>(this, awaiter6));
				}
				UniTask<T7>.Awaiter awaiter7 = task7.GetAwaiter();
				if (awaiter7.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.TryInvokeContinuationT7(this, awaiter7);
				}
				else
				{
					awaiter7.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T7>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T7>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.TryInvokeContinuationT7(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T7>.Awaiter>(this, awaiter7));
				}
				UniTask<T8>.Awaiter awaiter8 = task8.GetAwaiter();
				if (awaiter8.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.TryInvokeContinuationT8(this, awaiter8);
				}
				else
				{
					awaiter8.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T8>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T8>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.TryInvokeContinuationT8(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T8>.Awaiter>(this, awaiter8));
				}
				UniTask<T9>.Awaiter awaiter9 = task9.GetAwaiter();
				if (awaiter9.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.TryInvokeContinuationT9(this, awaiter9);
				}
				else
				{
					awaiter9.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T9>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T9>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.TryInvokeContinuationT9(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T9>.Awaiter>(this, awaiter9));
				}
				UniTask<T10>.Awaiter awaiter10 = task10.GetAwaiter();
				if (awaiter10.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.TryInvokeContinuationT10(this, awaiter10);
				}
				else
				{
					awaiter10.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T10>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T10>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.TryInvokeContinuationT10(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T10>.Awaiter>(this, awaiter10));
				}
				UniTask<T11>.Awaiter awaiter11 = task11.GetAwaiter();
				if (awaiter11.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.TryInvokeContinuationT11(this, awaiter11);
					return;
				}
				awaiter11.SourceOnCompleted(delegate(object state)
				{
					using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T11>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T11>.Awaiter>)state)
					{
						UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.TryInvokeContinuationT11(stateTuple.Item1, stateTuple.Item2);
					}
				}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, UniTask<T11>.Awaiter>(this, awaiter11));
			}

			private static void TryInvokeContinuationT1(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> self, in UniTask<T1>.Awaiter awaiter)
			{
				T1 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11>>(0, result, default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11>(default(T7), default(T8), default(T9), default(T10), default(T11))));
				}
			}

			private static void TryInvokeContinuationT2(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> self, in UniTask<T2>.Awaiter awaiter)
			{
				T2 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11>>(1, default(T1), result, default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11>(default(T7), default(T8), default(T9), default(T10), default(T11))));
				}
			}

			private static void TryInvokeContinuationT3(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> self, in UniTask<T3>.Awaiter awaiter)
			{
				T3 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11>>(2, default(T1), default(T2), result, default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11>(default(T7), default(T8), default(T9), default(T10), default(T11))));
				}
			}

			private static void TryInvokeContinuationT4(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> self, in UniTask<T4>.Awaiter awaiter)
			{
				T4 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11>>(3, default(T1), default(T2), default(T3), result, default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11>(default(T7), default(T8), default(T9), default(T10), default(T11))));
				}
			}

			private static void TryInvokeContinuationT5(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> self, in UniTask<T5>.Awaiter awaiter)
			{
				T5 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11>>(4, default(T1), default(T2), default(T3), default(T4), result, default(T6), new ValueTuple<T7, T8, T9, T10, T11>(default(T7), default(T8), default(T9), default(T10), default(T11))));
				}
			}

			private static void TryInvokeContinuationT6(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> self, in UniTask<T6>.Awaiter awaiter)
			{
				T6 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11>>(5, default(T1), default(T2), default(T3), default(T4), default(T5), result, new ValueTuple<T7, T8, T9, T10, T11>(default(T7), default(T8), default(T9), default(T10), default(T11))));
				}
			}

			private static void TryInvokeContinuationT7(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> self, in UniTask<T7>.Awaiter awaiter)
			{
				T7 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11>>(6, default(T1), default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11>(result, default(T8), default(T9), default(T10), default(T11))));
				}
			}

			private static void TryInvokeContinuationT8(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> self, in UniTask<T8>.Awaiter awaiter)
			{
				T8 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11>>(7, default(T1), default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11>(default(T7), result, default(T9), default(T10), default(T11))));
				}
			}

			private static void TryInvokeContinuationT9(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> self, in UniTask<T9>.Awaiter awaiter)
			{
				T9 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11>>(8, default(T1), default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11>(default(T7), default(T8), result, default(T10), default(T11))));
				}
			}

			private static void TryInvokeContinuationT10(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> self, in UniTask<T10>.Awaiter awaiter)
			{
				T10 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11>>(9, default(T1), default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11>(default(T7), default(T8), default(T9), result, default(T11))));
				}
			}

			private static void TryInvokeContinuationT11(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> self, in UniTask<T11>.Awaiter awaiter)
			{
				T11 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11>>(10, default(T1), default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11>(default(T7), default(T8), default(T9), default(T10), result)));
				}
			}

			[return: TupleElementNames(new string[]
			{
				null,
				"result1",
				"result2",
				"result3",
				"result4",
				"result5",
				"result6",
				"result7",
				"result8",
				"result9",
				"result10",
				"result11",
				null,
				null,
				null,
				null,
				null
			})]
			public ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11>> GetResult(short token)
			{
				GC.SuppressFinalize(this);
				return this.core.GetResult(token);
			}

			public UniTaskStatus GetStatus(short token)
			{
				return this.core.GetStatus(token);
			}

			public void OnCompleted(Action<object> continuation, object state, short token)
			{
				this.core.OnCompleted(continuation, state, token);
			}

			public UniTaskStatus UnsafeGetStatus()
			{
				return this.core.UnsafeGetStatus();
			}

			void IUniTaskSource.GetResult(short token)
			{
				this.GetResult(token);
			}

			private int completedCount;

			[TupleElementNames(new string[]
			{
				null,
				"result1",
				"result2",
				"result3",
				"result4",
				"result5",
				"result6",
				"result7",
				"result8",
				"result9",
				"result10",
				"result11",
				null,
				null,
				null,
				null,
				null
			})]
			private UniTaskCompletionSourceCore<ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11>>> core;
		}

		private sealed class WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : IUniTaskSource<ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12>>>, IUniTaskSource
		{
			public WhenAnyPromise(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4, UniTask<T5> task5, UniTask<T6> task6, UniTask<T7> task7, UniTask<T8> task8, UniTask<T9> task9, UniTask<T10> task10, UniTask<T11> task11, UniTask<T12> task12)
			{
				this.completedCount = 0;
				UniTask<T1>.Awaiter awaiter = task1.GetAwaiter();
				if (awaiter.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.TryInvokeContinuationT1(this, awaiter);
				}
				else
				{
					awaiter.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T1>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T1>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.TryInvokeContinuationT1(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T1>.Awaiter>(this, awaiter));
				}
				UniTask<T2>.Awaiter awaiter2 = task2.GetAwaiter();
				if (awaiter2.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.TryInvokeContinuationT2(this, awaiter2);
				}
				else
				{
					awaiter2.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T2>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T2>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.TryInvokeContinuationT2(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T2>.Awaiter>(this, awaiter2));
				}
				UniTask<T3>.Awaiter awaiter3 = task3.GetAwaiter();
				if (awaiter3.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.TryInvokeContinuationT3(this, awaiter3);
				}
				else
				{
					awaiter3.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T3>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T3>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.TryInvokeContinuationT3(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T3>.Awaiter>(this, awaiter3));
				}
				UniTask<T4>.Awaiter awaiter4 = task4.GetAwaiter();
				if (awaiter4.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.TryInvokeContinuationT4(this, awaiter4);
				}
				else
				{
					awaiter4.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T4>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T4>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.TryInvokeContinuationT4(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T4>.Awaiter>(this, awaiter4));
				}
				UniTask<T5>.Awaiter awaiter5 = task5.GetAwaiter();
				if (awaiter5.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.TryInvokeContinuationT5(this, awaiter5);
				}
				else
				{
					awaiter5.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T5>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T5>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.TryInvokeContinuationT5(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T5>.Awaiter>(this, awaiter5));
				}
				UniTask<T6>.Awaiter awaiter6 = task6.GetAwaiter();
				if (awaiter6.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.TryInvokeContinuationT6(this, awaiter6);
				}
				else
				{
					awaiter6.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T6>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T6>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.TryInvokeContinuationT6(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T6>.Awaiter>(this, awaiter6));
				}
				UniTask<T7>.Awaiter awaiter7 = task7.GetAwaiter();
				if (awaiter7.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.TryInvokeContinuationT7(this, awaiter7);
				}
				else
				{
					awaiter7.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T7>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T7>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.TryInvokeContinuationT7(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T7>.Awaiter>(this, awaiter7));
				}
				UniTask<T8>.Awaiter awaiter8 = task8.GetAwaiter();
				if (awaiter8.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.TryInvokeContinuationT8(this, awaiter8);
				}
				else
				{
					awaiter8.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T8>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T8>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.TryInvokeContinuationT8(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T8>.Awaiter>(this, awaiter8));
				}
				UniTask<T9>.Awaiter awaiter9 = task9.GetAwaiter();
				if (awaiter9.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.TryInvokeContinuationT9(this, awaiter9);
				}
				else
				{
					awaiter9.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T9>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T9>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.TryInvokeContinuationT9(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T9>.Awaiter>(this, awaiter9));
				}
				UniTask<T10>.Awaiter awaiter10 = task10.GetAwaiter();
				if (awaiter10.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.TryInvokeContinuationT10(this, awaiter10);
				}
				else
				{
					awaiter10.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T10>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T10>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.TryInvokeContinuationT10(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T10>.Awaiter>(this, awaiter10));
				}
				UniTask<T11>.Awaiter awaiter11 = task11.GetAwaiter();
				if (awaiter11.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.TryInvokeContinuationT11(this, awaiter11);
				}
				else
				{
					awaiter11.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T11>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T11>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.TryInvokeContinuationT11(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T11>.Awaiter>(this, awaiter11));
				}
				UniTask<T12>.Awaiter awaiter12 = task12.GetAwaiter();
				if (awaiter12.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.TryInvokeContinuationT12(this, awaiter12);
					return;
				}
				awaiter12.SourceOnCompleted(delegate(object state)
				{
					using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T12>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T12>.Awaiter>)state)
					{
						UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.TryInvokeContinuationT12(stateTuple.Item1, stateTuple.Item2);
					}
				}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, UniTask<T12>.Awaiter>(this, awaiter12));
			}

			private static void TryInvokeContinuationT1(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> self, in UniTask<T1>.Awaiter awaiter)
			{
				T1 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12>>(0, result, default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11, T12>(default(T7), default(T8), default(T9), default(T10), default(T11), default(T12))));
				}
			}

			private static void TryInvokeContinuationT2(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> self, in UniTask<T2>.Awaiter awaiter)
			{
				T2 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12>>(1, default(T1), result, default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11, T12>(default(T7), default(T8), default(T9), default(T10), default(T11), default(T12))));
				}
			}

			private static void TryInvokeContinuationT3(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> self, in UniTask<T3>.Awaiter awaiter)
			{
				T3 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12>>(2, default(T1), default(T2), result, default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11, T12>(default(T7), default(T8), default(T9), default(T10), default(T11), default(T12))));
				}
			}

			private static void TryInvokeContinuationT4(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> self, in UniTask<T4>.Awaiter awaiter)
			{
				T4 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12>>(3, default(T1), default(T2), default(T3), result, default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11, T12>(default(T7), default(T8), default(T9), default(T10), default(T11), default(T12))));
				}
			}

			private static void TryInvokeContinuationT5(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> self, in UniTask<T5>.Awaiter awaiter)
			{
				T5 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12>>(4, default(T1), default(T2), default(T3), default(T4), result, default(T6), new ValueTuple<T7, T8, T9, T10, T11, T12>(default(T7), default(T8), default(T9), default(T10), default(T11), default(T12))));
				}
			}

			private static void TryInvokeContinuationT6(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> self, in UniTask<T6>.Awaiter awaiter)
			{
				T6 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12>>(5, default(T1), default(T2), default(T3), default(T4), default(T5), result, new ValueTuple<T7, T8, T9, T10, T11, T12>(default(T7), default(T8), default(T9), default(T10), default(T11), default(T12))));
				}
			}

			private static void TryInvokeContinuationT7(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> self, in UniTask<T7>.Awaiter awaiter)
			{
				T7 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12>>(6, default(T1), default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11, T12>(result, default(T8), default(T9), default(T10), default(T11), default(T12))));
				}
			}

			private static void TryInvokeContinuationT8(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> self, in UniTask<T8>.Awaiter awaiter)
			{
				T8 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12>>(7, default(T1), default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11, T12>(default(T7), result, default(T9), default(T10), default(T11), default(T12))));
				}
			}

			private static void TryInvokeContinuationT9(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> self, in UniTask<T9>.Awaiter awaiter)
			{
				T9 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12>>(8, default(T1), default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11, T12>(default(T7), default(T8), result, default(T10), default(T11), default(T12))));
				}
			}

			private static void TryInvokeContinuationT10(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> self, in UniTask<T10>.Awaiter awaiter)
			{
				T10 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12>>(9, default(T1), default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11, T12>(default(T7), default(T8), default(T9), result, default(T11), default(T12))));
				}
			}

			private static void TryInvokeContinuationT11(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> self, in UniTask<T11>.Awaiter awaiter)
			{
				T11 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12>>(10, default(T1), default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11, T12>(default(T7), default(T8), default(T9), default(T10), result, default(T12))));
				}
			}

			private static void TryInvokeContinuationT12(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> self, in UniTask<T12>.Awaiter awaiter)
			{
				T12 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12>>(11, default(T1), default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11, T12>(default(T7), default(T8), default(T9), default(T10), default(T11), result)));
				}
			}

			[return: TupleElementNames(new string[]
			{
				null,
				"result1",
				"result2",
				"result3",
				"result4",
				"result5",
				"result6",
				"result7",
				"result8",
				"result9",
				"result10",
				"result11",
				"result12",
				null,
				null,
				null,
				null,
				null,
				null
			})]
			public ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12>> GetResult(short token)
			{
				GC.SuppressFinalize(this);
				return this.core.GetResult(token);
			}

			public UniTaskStatus GetStatus(short token)
			{
				return this.core.GetStatus(token);
			}

			public void OnCompleted(Action<object> continuation, object state, short token)
			{
				this.core.OnCompleted(continuation, state, token);
			}

			public UniTaskStatus UnsafeGetStatus()
			{
				return this.core.UnsafeGetStatus();
			}

			void IUniTaskSource.GetResult(short token)
			{
				this.GetResult(token);
			}

			private int completedCount;

			[TupleElementNames(new string[]
			{
				null,
				"result1",
				"result2",
				"result3",
				"result4",
				"result5",
				"result6",
				"result7",
				"result8",
				"result9",
				"result10",
				"result11",
				"result12",
				null,
				null,
				null,
				null,
				null,
				null
			})]
			private UniTaskCompletionSourceCore<ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12>>> core;
		}

		private sealed class WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : IUniTaskSource<ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13>>>, IUniTaskSource
		{
			public WhenAnyPromise(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4, UniTask<T5> task5, UniTask<T6> task6, UniTask<T7> task7, UniTask<T8> task8, UniTask<T9> task9, UniTask<T10> task10, UniTask<T11> task11, UniTask<T12> task12, UniTask<T13> task13)
			{
				this.completedCount = 0;
				UniTask<T1>.Awaiter awaiter = task1.GetAwaiter();
				if (awaiter.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.TryInvokeContinuationT1(this, awaiter);
				}
				else
				{
					awaiter.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T1>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T1>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.TryInvokeContinuationT1(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T1>.Awaiter>(this, awaiter));
				}
				UniTask<T2>.Awaiter awaiter2 = task2.GetAwaiter();
				if (awaiter2.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.TryInvokeContinuationT2(this, awaiter2);
				}
				else
				{
					awaiter2.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T2>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T2>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.TryInvokeContinuationT2(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T2>.Awaiter>(this, awaiter2));
				}
				UniTask<T3>.Awaiter awaiter3 = task3.GetAwaiter();
				if (awaiter3.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.TryInvokeContinuationT3(this, awaiter3);
				}
				else
				{
					awaiter3.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T3>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T3>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.TryInvokeContinuationT3(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T3>.Awaiter>(this, awaiter3));
				}
				UniTask<T4>.Awaiter awaiter4 = task4.GetAwaiter();
				if (awaiter4.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.TryInvokeContinuationT4(this, awaiter4);
				}
				else
				{
					awaiter4.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T4>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T4>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.TryInvokeContinuationT4(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T4>.Awaiter>(this, awaiter4));
				}
				UniTask<T5>.Awaiter awaiter5 = task5.GetAwaiter();
				if (awaiter5.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.TryInvokeContinuationT5(this, awaiter5);
				}
				else
				{
					awaiter5.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T5>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T5>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.TryInvokeContinuationT5(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T5>.Awaiter>(this, awaiter5));
				}
				UniTask<T6>.Awaiter awaiter6 = task6.GetAwaiter();
				if (awaiter6.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.TryInvokeContinuationT6(this, awaiter6);
				}
				else
				{
					awaiter6.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T6>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T6>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.TryInvokeContinuationT6(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T6>.Awaiter>(this, awaiter6));
				}
				UniTask<T7>.Awaiter awaiter7 = task7.GetAwaiter();
				if (awaiter7.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.TryInvokeContinuationT7(this, awaiter7);
				}
				else
				{
					awaiter7.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T7>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T7>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.TryInvokeContinuationT7(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T7>.Awaiter>(this, awaiter7));
				}
				UniTask<T8>.Awaiter awaiter8 = task8.GetAwaiter();
				if (awaiter8.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.TryInvokeContinuationT8(this, awaiter8);
				}
				else
				{
					awaiter8.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T8>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T8>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.TryInvokeContinuationT8(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T8>.Awaiter>(this, awaiter8));
				}
				UniTask<T9>.Awaiter awaiter9 = task9.GetAwaiter();
				if (awaiter9.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.TryInvokeContinuationT9(this, awaiter9);
				}
				else
				{
					awaiter9.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T9>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T9>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.TryInvokeContinuationT9(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T9>.Awaiter>(this, awaiter9));
				}
				UniTask<T10>.Awaiter awaiter10 = task10.GetAwaiter();
				if (awaiter10.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.TryInvokeContinuationT10(this, awaiter10);
				}
				else
				{
					awaiter10.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T10>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T10>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.TryInvokeContinuationT10(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T10>.Awaiter>(this, awaiter10));
				}
				UniTask<T11>.Awaiter awaiter11 = task11.GetAwaiter();
				if (awaiter11.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.TryInvokeContinuationT11(this, awaiter11);
				}
				else
				{
					awaiter11.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T11>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T11>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.TryInvokeContinuationT11(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T11>.Awaiter>(this, awaiter11));
				}
				UniTask<T12>.Awaiter awaiter12 = task12.GetAwaiter();
				if (awaiter12.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.TryInvokeContinuationT12(this, awaiter12);
				}
				else
				{
					awaiter12.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T12>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T12>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.TryInvokeContinuationT12(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T12>.Awaiter>(this, awaiter12));
				}
				UniTask<T13>.Awaiter awaiter13 = task13.GetAwaiter();
				if (awaiter13.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.TryInvokeContinuationT13(this, awaiter13);
					return;
				}
				awaiter13.SourceOnCompleted(delegate(object state)
				{
					using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T13>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T13>.Awaiter>)state)
					{
						UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.TryInvokeContinuationT13(stateTuple.Item1, stateTuple.Item2);
					}
				}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, UniTask<T13>.Awaiter>(this, awaiter13));
			}

			private static void TryInvokeContinuationT1(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> self, in UniTask<T1>.Awaiter awaiter)
			{
				T1 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13>>(0, result, default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11, T12, T13>(default(T7), default(T8), default(T9), default(T10), default(T11), default(T12), default(T13))));
				}
			}

			private static void TryInvokeContinuationT2(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> self, in UniTask<T2>.Awaiter awaiter)
			{
				T2 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13>>(1, default(T1), result, default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11, T12, T13>(default(T7), default(T8), default(T9), default(T10), default(T11), default(T12), default(T13))));
				}
			}

			private static void TryInvokeContinuationT3(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> self, in UniTask<T3>.Awaiter awaiter)
			{
				T3 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13>>(2, default(T1), default(T2), result, default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11, T12, T13>(default(T7), default(T8), default(T9), default(T10), default(T11), default(T12), default(T13))));
				}
			}

			private static void TryInvokeContinuationT4(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> self, in UniTask<T4>.Awaiter awaiter)
			{
				T4 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13>>(3, default(T1), default(T2), default(T3), result, default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11, T12, T13>(default(T7), default(T8), default(T9), default(T10), default(T11), default(T12), default(T13))));
				}
			}

			private static void TryInvokeContinuationT5(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> self, in UniTask<T5>.Awaiter awaiter)
			{
				T5 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13>>(4, default(T1), default(T2), default(T3), default(T4), result, default(T6), new ValueTuple<T7, T8, T9, T10, T11, T12, T13>(default(T7), default(T8), default(T9), default(T10), default(T11), default(T12), default(T13))));
				}
			}

			private static void TryInvokeContinuationT6(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> self, in UniTask<T6>.Awaiter awaiter)
			{
				T6 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13>>(5, default(T1), default(T2), default(T3), default(T4), default(T5), result, new ValueTuple<T7, T8, T9, T10, T11, T12, T13>(default(T7), default(T8), default(T9), default(T10), default(T11), default(T12), default(T13))));
				}
			}

			private static void TryInvokeContinuationT7(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> self, in UniTask<T7>.Awaiter awaiter)
			{
				T7 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13>>(6, default(T1), default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11, T12, T13>(result, default(T8), default(T9), default(T10), default(T11), default(T12), default(T13))));
				}
			}

			private static void TryInvokeContinuationT8(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> self, in UniTask<T8>.Awaiter awaiter)
			{
				T8 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13>>(7, default(T1), default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11, T12, T13>(default(T7), result, default(T9), default(T10), default(T11), default(T12), default(T13))));
				}
			}

			private static void TryInvokeContinuationT9(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> self, in UniTask<T9>.Awaiter awaiter)
			{
				T9 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13>>(8, default(T1), default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11, T12, T13>(default(T7), default(T8), result, default(T10), default(T11), default(T12), default(T13))));
				}
			}

			private static void TryInvokeContinuationT10(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> self, in UniTask<T10>.Awaiter awaiter)
			{
				T10 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13>>(9, default(T1), default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11, T12, T13>(default(T7), default(T8), default(T9), result, default(T11), default(T12), default(T13))));
				}
			}

			private static void TryInvokeContinuationT11(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> self, in UniTask<T11>.Awaiter awaiter)
			{
				T11 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13>>(10, default(T1), default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11, T12, T13>(default(T7), default(T8), default(T9), default(T10), result, default(T12), default(T13))));
				}
			}

			private static void TryInvokeContinuationT12(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> self, in UniTask<T12>.Awaiter awaiter)
			{
				T12 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13>>(11, default(T1), default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11, T12, T13>(default(T7), default(T8), default(T9), default(T10), default(T11), result, default(T13))));
				}
			}

			private static void TryInvokeContinuationT13(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> self, in UniTask<T13>.Awaiter awaiter)
			{
				T13 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13>>(12, default(T1), default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11, T12, T13>(default(T7), default(T8), default(T9), default(T10), default(T11), default(T12), result)));
				}
			}

			[return: TupleElementNames(new string[]
			{
				null,
				"result1",
				"result2",
				"result3",
				"result4",
				"result5",
				"result6",
				"result7",
				"result8",
				"result9",
				"result10",
				"result11",
				"result12",
				"result13",
				null,
				null,
				null,
				null,
				null,
				null,
				null
			})]
			public ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13>> GetResult(short token)
			{
				GC.SuppressFinalize(this);
				return this.core.GetResult(token);
			}

			public UniTaskStatus GetStatus(short token)
			{
				return this.core.GetStatus(token);
			}

			public void OnCompleted(Action<object> continuation, object state, short token)
			{
				this.core.OnCompleted(continuation, state, token);
			}

			public UniTaskStatus UnsafeGetStatus()
			{
				return this.core.UnsafeGetStatus();
			}

			void IUniTaskSource.GetResult(short token)
			{
				this.GetResult(token);
			}

			private int completedCount;

			[TupleElementNames(new string[]
			{
				null,
				"result1",
				"result2",
				"result3",
				"result4",
				"result5",
				"result6",
				"result7",
				"result8",
				"result9",
				"result10",
				"result11",
				"result12",
				"result13",
				null,
				null,
				null,
				null,
				null,
				null,
				null
			})]
			private UniTaskCompletionSourceCore<ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13>>> core;
		}

		private sealed class WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> : IUniTaskSource<ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14>>>>, IUniTaskSource
		{
			public WhenAnyPromise(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4, UniTask<T5> task5, UniTask<T6> task6, UniTask<T7> task7, UniTask<T8> task8, UniTask<T9> task9, UniTask<T10> task10, UniTask<T11> task11, UniTask<T12> task12, UniTask<T13> task13, UniTask<T14> task14)
			{
				this.completedCount = 0;
				UniTask<T1>.Awaiter awaiter = task1.GetAwaiter();
				if (awaiter.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT1(this, awaiter);
				}
				else
				{
					awaiter.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T1>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T1>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT1(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T1>.Awaiter>(this, awaiter));
				}
				UniTask<T2>.Awaiter awaiter2 = task2.GetAwaiter();
				if (awaiter2.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT2(this, awaiter2);
				}
				else
				{
					awaiter2.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T2>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T2>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT2(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T2>.Awaiter>(this, awaiter2));
				}
				UniTask<T3>.Awaiter awaiter3 = task3.GetAwaiter();
				if (awaiter3.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT3(this, awaiter3);
				}
				else
				{
					awaiter3.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T3>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T3>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT3(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T3>.Awaiter>(this, awaiter3));
				}
				UniTask<T4>.Awaiter awaiter4 = task4.GetAwaiter();
				if (awaiter4.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT4(this, awaiter4);
				}
				else
				{
					awaiter4.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T4>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T4>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT4(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T4>.Awaiter>(this, awaiter4));
				}
				UniTask<T5>.Awaiter awaiter5 = task5.GetAwaiter();
				if (awaiter5.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT5(this, awaiter5);
				}
				else
				{
					awaiter5.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T5>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T5>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT5(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T5>.Awaiter>(this, awaiter5));
				}
				UniTask<T6>.Awaiter awaiter6 = task6.GetAwaiter();
				if (awaiter6.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT6(this, awaiter6);
				}
				else
				{
					awaiter6.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T6>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T6>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT6(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T6>.Awaiter>(this, awaiter6));
				}
				UniTask<T7>.Awaiter awaiter7 = task7.GetAwaiter();
				if (awaiter7.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT7(this, awaiter7);
				}
				else
				{
					awaiter7.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T7>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T7>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT7(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T7>.Awaiter>(this, awaiter7));
				}
				UniTask<T8>.Awaiter awaiter8 = task8.GetAwaiter();
				if (awaiter8.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT8(this, awaiter8);
				}
				else
				{
					awaiter8.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T8>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T8>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT8(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T8>.Awaiter>(this, awaiter8));
				}
				UniTask<T9>.Awaiter awaiter9 = task9.GetAwaiter();
				if (awaiter9.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT9(this, awaiter9);
				}
				else
				{
					awaiter9.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T9>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T9>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT9(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T9>.Awaiter>(this, awaiter9));
				}
				UniTask<T10>.Awaiter awaiter10 = task10.GetAwaiter();
				if (awaiter10.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT10(this, awaiter10);
				}
				else
				{
					awaiter10.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T10>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T10>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT10(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T10>.Awaiter>(this, awaiter10));
				}
				UniTask<T11>.Awaiter awaiter11 = task11.GetAwaiter();
				if (awaiter11.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT11(this, awaiter11);
				}
				else
				{
					awaiter11.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T11>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T11>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT11(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T11>.Awaiter>(this, awaiter11));
				}
				UniTask<T12>.Awaiter awaiter12 = task12.GetAwaiter();
				if (awaiter12.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT12(this, awaiter12);
				}
				else
				{
					awaiter12.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T12>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T12>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT12(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T12>.Awaiter>(this, awaiter12));
				}
				UniTask<T13>.Awaiter awaiter13 = task13.GetAwaiter();
				if (awaiter13.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT13(this, awaiter13);
				}
				else
				{
					awaiter13.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T13>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T13>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT13(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T13>.Awaiter>(this, awaiter13));
				}
				UniTask<T14>.Awaiter awaiter14 = task14.GetAwaiter();
				if (awaiter14.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT14(this, awaiter14);
					return;
				}
				awaiter14.SourceOnCompleted(delegate(object state)
				{
					using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T14>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T14>.Awaiter>)state)
					{
						UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.TryInvokeContinuationT14(stateTuple.Item1, stateTuple.Item2);
					}
				}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, UniTask<T14>.Awaiter>(this, awaiter14));
			}

			private static void TryInvokeContinuationT1(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> self, in UniTask<T1>.Awaiter awaiter)
			{
				T1 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14>>>(0, result, default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14>>(default(T7), default(T8), default(T9), default(T10), default(T11), default(T12), default(T13), new ValueTuple<T14>(default(T14)))));
				}
			}

			private static void TryInvokeContinuationT2(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> self, in UniTask<T2>.Awaiter awaiter)
			{
				T2 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14>>>(1, default(T1), result, default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14>>(default(T7), default(T8), default(T9), default(T10), default(T11), default(T12), default(T13), new ValueTuple<T14>(default(T14)))));
				}
			}

			private static void TryInvokeContinuationT3(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> self, in UniTask<T3>.Awaiter awaiter)
			{
				T3 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14>>>(2, default(T1), default(T2), result, default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14>>(default(T7), default(T8), default(T9), default(T10), default(T11), default(T12), default(T13), new ValueTuple<T14>(default(T14)))));
				}
			}

			private static void TryInvokeContinuationT4(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> self, in UniTask<T4>.Awaiter awaiter)
			{
				T4 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14>>>(3, default(T1), default(T2), default(T3), result, default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14>>(default(T7), default(T8), default(T9), default(T10), default(T11), default(T12), default(T13), new ValueTuple<T14>(default(T14)))));
				}
			}

			private static void TryInvokeContinuationT5(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> self, in UniTask<T5>.Awaiter awaiter)
			{
				T5 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14>>>(4, default(T1), default(T2), default(T3), default(T4), result, default(T6), new ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14>>(default(T7), default(T8), default(T9), default(T10), default(T11), default(T12), default(T13), new ValueTuple<T14>(default(T14)))));
				}
			}

			private static void TryInvokeContinuationT6(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> self, in UniTask<T6>.Awaiter awaiter)
			{
				T6 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14>>>(5, default(T1), default(T2), default(T3), default(T4), default(T5), result, new ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14>>(default(T7), default(T8), default(T9), default(T10), default(T11), default(T12), default(T13), new ValueTuple<T14>(default(T14)))));
				}
			}

			private static void TryInvokeContinuationT7(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> self, in UniTask<T7>.Awaiter awaiter)
			{
				T7 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14>>>(6, default(T1), default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14>>(result, default(T8), default(T9), default(T10), default(T11), default(T12), default(T13), new ValueTuple<T14>(default(T14)))));
				}
			}

			private static void TryInvokeContinuationT8(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> self, in UniTask<T8>.Awaiter awaiter)
			{
				T8 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14>>>(7, default(T1), default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14>>(default(T7), result, default(T9), default(T10), default(T11), default(T12), default(T13), new ValueTuple<T14>(default(T14)))));
				}
			}

			private static void TryInvokeContinuationT9(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> self, in UniTask<T9>.Awaiter awaiter)
			{
				T9 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14>>>(8, default(T1), default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14>>(default(T7), default(T8), result, default(T10), default(T11), default(T12), default(T13), new ValueTuple<T14>(default(T14)))));
				}
			}

			private static void TryInvokeContinuationT10(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> self, in UniTask<T10>.Awaiter awaiter)
			{
				T10 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14>>>(9, default(T1), default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14>>(default(T7), default(T8), default(T9), result, default(T11), default(T12), default(T13), new ValueTuple<T14>(default(T14)))));
				}
			}

			private static void TryInvokeContinuationT11(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> self, in UniTask<T11>.Awaiter awaiter)
			{
				T11 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14>>>(10, default(T1), default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14>>(default(T7), default(T8), default(T9), default(T10), result, default(T12), default(T13), new ValueTuple<T14>(default(T14)))));
				}
			}

			private static void TryInvokeContinuationT12(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> self, in UniTask<T12>.Awaiter awaiter)
			{
				T12 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14>>>(11, default(T1), default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14>>(default(T7), default(T8), default(T9), default(T10), default(T11), result, default(T13), new ValueTuple<T14>(default(T14)))));
				}
			}

			private static void TryInvokeContinuationT13(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> self, in UniTask<T13>.Awaiter awaiter)
			{
				T13 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14>>>(12, default(T1), default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14>>(default(T7), default(T8), default(T9), default(T10), default(T11), default(T12), result, new ValueTuple<T14>(default(T14)))));
				}
			}

			private static void TryInvokeContinuationT14(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> self, in UniTask<T14>.Awaiter awaiter)
			{
				T14 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14>>>(13, default(T1), default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14>>(default(T7), default(T8), default(T9), default(T10), default(T11), default(T12), default(T13), new ValueTuple<T14>(result))));
				}
			}

			[return: TupleElementNames(new string[]
			{
				null,
				"result1",
				"result2",
				"result3",
				"result4",
				"result5",
				"result6",
				"result7",
				"result8",
				"result9",
				"result10",
				"result11",
				"result12",
				"result13",
				"result14",
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			})]
			public ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14>>> GetResult(short token)
			{
				GC.SuppressFinalize(this);
				return this.core.GetResult(token);
			}

			public UniTaskStatus GetStatus(short token)
			{
				return this.core.GetStatus(token);
			}

			public void OnCompleted(Action<object> continuation, object state, short token)
			{
				this.core.OnCompleted(continuation, state, token);
			}

			public UniTaskStatus UnsafeGetStatus()
			{
				return this.core.UnsafeGetStatus();
			}

			void IUniTaskSource.GetResult(short token)
			{
				this.GetResult(token);
			}

			private int completedCount;

			[TupleElementNames(new string[]
			{
				null,
				"result1",
				"result2",
				"result3",
				"result4",
				"result5",
				"result6",
				"result7",
				"result8",
				"result9",
				"result10",
				"result11",
				"result12",
				"result13",
				"result14",
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			})]
			private UniTaskCompletionSourceCore<ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14>>>> core;
		}

		private sealed class WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> : IUniTaskSource<ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14, T15>>>>, IUniTaskSource
		{
			public WhenAnyPromise(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4, UniTask<T5> task5, UniTask<T6> task6, UniTask<T7> task7, UniTask<T8> task8, UniTask<T9> task9, UniTask<T10> task10, UniTask<T11> task11, UniTask<T12> task12, UniTask<T13> task13, UniTask<T14> task14, UniTask<T15> task15)
			{
				this.completedCount = 0;
				UniTask<T1>.Awaiter awaiter = task1.GetAwaiter();
				if (awaiter.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT1(this, awaiter);
				}
				else
				{
					awaiter.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T1>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T1>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT1(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T1>.Awaiter>(this, awaiter));
				}
				UniTask<T2>.Awaiter awaiter2 = task2.GetAwaiter();
				if (awaiter2.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT2(this, awaiter2);
				}
				else
				{
					awaiter2.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T2>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T2>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT2(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T2>.Awaiter>(this, awaiter2));
				}
				UniTask<T3>.Awaiter awaiter3 = task3.GetAwaiter();
				if (awaiter3.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT3(this, awaiter3);
				}
				else
				{
					awaiter3.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T3>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T3>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT3(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T3>.Awaiter>(this, awaiter3));
				}
				UniTask<T4>.Awaiter awaiter4 = task4.GetAwaiter();
				if (awaiter4.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT4(this, awaiter4);
				}
				else
				{
					awaiter4.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T4>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T4>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT4(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T4>.Awaiter>(this, awaiter4));
				}
				UniTask<T5>.Awaiter awaiter5 = task5.GetAwaiter();
				if (awaiter5.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT5(this, awaiter5);
				}
				else
				{
					awaiter5.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T5>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T5>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT5(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T5>.Awaiter>(this, awaiter5));
				}
				UniTask<T6>.Awaiter awaiter6 = task6.GetAwaiter();
				if (awaiter6.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT6(this, awaiter6);
				}
				else
				{
					awaiter6.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T6>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T6>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT6(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T6>.Awaiter>(this, awaiter6));
				}
				UniTask<T7>.Awaiter awaiter7 = task7.GetAwaiter();
				if (awaiter7.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT7(this, awaiter7);
				}
				else
				{
					awaiter7.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T7>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T7>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT7(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T7>.Awaiter>(this, awaiter7));
				}
				UniTask<T8>.Awaiter awaiter8 = task8.GetAwaiter();
				if (awaiter8.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT8(this, awaiter8);
				}
				else
				{
					awaiter8.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T8>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T8>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT8(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T8>.Awaiter>(this, awaiter8));
				}
				UniTask<T9>.Awaiter awaiter9 = task9.GetAwaiter();
				if (awaiter9.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT9(this, awaiter9);
				}
				else
				{
					awaiter9.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T9>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T9>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT9(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T9>.Awaiter>(this, awaiter9));
				}
				UniTask<T10>.Awaiter awaiter10 = task10.GetAwaiter();
				if (awaiter10.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT10(this, awaiter10);
				}
				else
				{
					awaiter10.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T10>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T10>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT10(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T10>.Awaiter>(this, awaiter10));
				}
				UniTask<T11>.Awaiter awaiter11 = task11.GetAwaiter();
				if (awaiter11.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT11(this, awaiter11);
				}
				else
				{
					awaiter11.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T11>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T11>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT11(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T11>.Awaiter>(this, awaiter11));
				}
				UniTask<T12>.Awaiter awaiter12 = task12.GetAwaiter();
				if (awaiter12.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT12(this, awaiter12);
				}
				else
				{
					awaiter12.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T12>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T12>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT12(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T12>.Awaiter>(this, awaiter12));
				}
				UniTask<T13>.Awaiter awaiter13 = task13.GetAwaiter();
				if (awaiter13.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT13(this, awaiter13);
				}
				else
				{
					awaiter13.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T13>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T13>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT13(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T13>.Awaiter>(this, awaiter13));
				}
				UniTask<T14>.Awaiter awaiter14 = task14.GetAwaiter();
				if (awaiter14.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT14(this, awaiter14);
				}
				else
				{
					awaiter14.SourceOnCompleted(delegate(object state)
					{
						using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T14>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T14>.Awaiter>)state)
						{
							UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT14(stateTuple.Item1, stateTuple.Item2);
						}
					}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T14>.Awaiter>(this, awaiter14));
				}
				UniTask<T15>.Awaiter awaiter15 = task15.GetAwaiter();
				if (awaiter15.IsCompleted)
				{
					UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT15(this, awaiter15);
					return;
				}
				awaiter15.SourceOnCompleted(delegate(object state)
				{
					using (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T15>.Awaiter> stateTuple = (StateTuple<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T15>.Awaiter>)state)
					{
						UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.TryInvokeContinuationT15(stateTuple.Item1, stateTuple.Item2);
					}
				}, StateTuple.Create<UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, UniTask<T15>.Awaiter>(this, awaiter15));
			}

			private static void TryInvokeContinuationT1(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> self, in UniTask<T1>.Awaiter awaiter)
			{
				T1 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14, T15>>>(0, result, default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14, T15>>(default(T7), default(T8), default(T9), default(T10), default(T11), default(T12), default(T13), new ValueTuple<T14, T15>(default(T14), default(T15)))));
				}
			}

			private static void TryInvokeContinuationT2(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> self, in UniTask<T2>.Awaiter awaiter)
			{
				T2 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14, T15>>>(1, default(T1), result, default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14, T15>>(default(T7), default(T8), default(T9), default(T10), default(T11), default(T12), default(T13), new ValueTuple<T14, T15>(default(T14), default(T15)))));
				}
			}

			private static void TryInvokeContinuationT3(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> self, in UniTask<T3>.Awaiter awaiter)
			{
				T3 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14, T15>>>(2, default(T1), default(T2), result, default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14, T15>>(default(T7), default(T8), default(T9), default(T10), default(T11), default(T12), default(T13), new ValueTuple<T14, T15>(default(T14), default(T15)))));
				}
			}

			private static void TryInvokeContinuationT4(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> self, in UniTask<T4>.Awaiter awaiter)
			{
				T4 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14, T15>>>(3, default(T1), default(T2), default(T3), result, default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14, T15>>(default(T7), default(T8), default(T9), default(T10), default(T11), default(T12), default(T13), new ValueTuple<T14, T15>(default(T14), default(T15)))));
				}
			}

			private static void TryInvokeContinuationT5(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> self, in UniTask<T5>.Awaiter awaiter)
			{
				T5 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14, T15>>>(4, default(T1), default(T2), default(T3), default(T4), result, default(T6), new ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14, T15>>(default(T7), default(T8), default(T9), default(T10), default(T11), default(T12), default(T13), new ValueTuple<T14, T15>(default(T14), default(T15)))));
				}
			}

			private static void TryInvokeContinuationT6(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> self, in UniTask<T6>.Awaiter awaiter)
			{
				T6 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14, T15>>>(5, default(T1), default(T2), default(T3), default(T4), default(T5), result, new ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14, T15>>(default(T7), default(T8), default(T9), default(T10), default(T11), default(T12), default(T13), new ValueTuple<T14, T15>(default(T14), default(T15)))));
				}
			}

			private static void TryInvokeContinuationT7(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> self, in UniTask<T7>.Awaiter awaiter)
			{
				T7 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14, T15>>>(6, default(T1), default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14, T15>>(result, default(T8), default(T9), default(T10), default(T11), default(T12), default(T13), new ValueTuple<T14, T15>(default(T14), default(T15)))));
				}
			}

			private static void TryInvokeContinuationT8(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> self, in UniTask<T8>.Awaiter awaiter)
			{
				T8 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14, T15>>>(7, default(T1), default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14, T15>>(default(T7), result, default(T9), default(T10), default(T11), default(T12), default(T13), new ValueTuple<T14, T15>(default(T14), default(T15)))));
				}
			}

			private static void TryInvokeContinuationT9(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> self, in UniTask<T9>.Awaiter awaiter)
			{
				T9 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14, T15>>>(8, default(T1), default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14, T15>>(default(T7), default(T8), result, default(T10), default(T11), default(T12), default(T13), new ValueTuple<T14, T15>(default(T14), default(T15)))));
				}
			}

			private static void TryInvokeContinuationT10(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> self, in UniTask<T10>.Awaiter awaiter)
			{
				T10 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14, T15>>>(9, default(T1), default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14, T15>>(default(T7), default(T8), default(T9), result, default(T11), default(T12), default(T13), new ValueTuple<T14, T15>(default(T14), default(T15)))));
				}
			}

			private static void TryInvokeContinuationT11(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> self, in UniTask<T11>.Awaiter awaiter)
			{
				T11 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14, T15>>>(10, default(T1), default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14, T15>>(default(T7), default(T8), default(T9), default(T10), result, default(T12), default(T13), new ValueTuple<T14, T15>(default(T14), default(T15)))));
				}
			}

			private static void TryInvokeContinuationT12(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> self, in UniTask<T12>.Awaiter awaiter)
			{
				T12 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14, T15>>>(11, default(T1), default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14, T15>>(default(T7), default(T8), default(T9), default(T10), default(T11), result, default(T13), new ValueTuple<T14, T15>(default(T14), default(T15)))));
				}
			}

			private static void TryInvokeContinuationT13(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> self, in UniTask<T13>.Awaiter awaiter)
			{
				T13 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14, T15>>>(12, default(T1), default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14, T15>>(default(T7), default(T8), default(T9), default(T10), default(T11), default(T12), result, new ValueTuple<T14, T15>(default(T14), default(T15)))));
				}
			}

			private static void TryInvokeContinuationT14(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> self, in UniTask<T14>.Awaiter awaiter)
			{
				T14 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14, T15>>>(13, default(T1), default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14, T15>>(default(T7), default(T8), default(T9), default(T10), default(T11), default(T12), default(T13), new ValueTuple<T14, T15>(result, default(T15)))));
				}
			}

			private static void TryInvokeContinuationT15(UniTask.WhenAnyPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> self, in UniTask<T15>.Awaiter awaiter)
			{
				T15 result;
				try
				{
					result = awaiter.GetResult();
				}
				catch (Exception error)
				{
					self.core.TrySetException(error);
					return;
				}
				if (Interlocked.Increment(ref self.completedCount) == 1)
				{
					self.core.TrySetResult(new ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14, T15>>>(14, default(T1), default(T2), default(T3), default(T4), default(T5), default(T6), new ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14, T15>>(default(T7), default(T8), default(T9), default(T10), default(T11), default(T12), default(T13), new ValueTuple<T14, T15>(default(T14), result))));
				}
			}

			[return: TupleElementNames(new string[]
			{
				null,
				"result1",
				"result2",
				"result3",
				"result4",
				"result5",
				"result6",
				"result7",
				"result8",
				"result9",
				"result10",
				"result11",
				"result12",
				"result13",
				"result14",
				"result15",
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			})]
			public ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14, T15>>> GetResult(short token)
			{
				GC.SuppressFinalize(this);
				return this.core.GetResult(token);
			}

			public UniTaskStatus GetStatus(short token)
			{
				return this.core.GetStatus(token);
			}

			public void OnCompleted(Action<object> continuation, object state, short token)
			{
				this.core.OnCompleted(continuation, state, token);
			}

			public UniTaskStatus UnsafeGetStatus()
			{
				return this.core.UnsafeGetStatus();
			}

			void IUniTaskSource.GetResult(short token)
			{
				this.GetResult(token);
			}

			private int completedCount;

			[TupleElementNames(new string[]
			{
				null,
				"result1",
				"result2",
				"result3",
				"result4",
				"result5",
				"result6",
				"result7",
				"result8",
				"result9",
				"result10",
				"result11",
				"result12",
				"result13",
				"result14",
				"result15",
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			})]
			private UniTaskCompletionSourceCore<ValueTuple<int, T1, T2, T3, T4, T5, T6, ValueTuple<T7, T8, T9, T10, T11, T12, T13, ValueTuple<T14, T15>>>> core;
		}
	}
}
