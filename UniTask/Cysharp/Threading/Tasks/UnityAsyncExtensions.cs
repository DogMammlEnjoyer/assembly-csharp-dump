using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;
using Cysharp.Threading.Tasks.Internal;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Cysharp.Threading.Tasks
{
	public static class UnityAsyncExtensions
	{
		public static UnityAsyncExtensions.AssetBundleRequestAllAssetsAwaiter AwaitForAllAssets(this AssetBundleRequest asyncOperation)
		{
			Error.ThrowArgumentNullException<AssetBundleRequest>(asyncOperation, "asyncOperation");
			return new UnityAsyncExtensions.AssetBundleRequestAllAssetsAwaiter(asyncOperation);
		}

		public static UniTask<Object[]> AwaitForAllAssets(this AssetBundleRequest asyncOperation, CancellationToken cancellationToken)
		{
			return asyncOperation.AwaitForAllAssets(null, PlayerLoopTiming.Update, cancellationToken);
		}

		public static UniTask<Object[]> AwaitForAllAssets(this AssetBundleRequest asyncOperation, IProgress<float> progress = null, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<AssetBundleRequest>(asyncOperation, "asyncOperation");
			if (cancellationToken.IsCancellationRequested)
			{
				return UniTask.FromCanceled<Object[]>(cancellationToken);
			}
			if (asyncOperation.isDone)
			{
				return UniTask.FromResult<Object[]>(asyncOperation.allAssets);
			}
			short token;
			return new UniTask<Object[]>(UnityAsyncExtensions.AssetBundleRequestAllAssetsConfiguredSource.Create(asyncOperation, timing, progress, cancellationToken, out token), token);
		}

		public static UniTask<AsyncGPUReadbackRequest>.Awaiter GetAwaiter(this AsyncGPUReadbackRequest asyncOperation)
		{
			return asyncOperation.ToUniTask(PlayerLoopTiming.Update, default(CancellationToken)).GetAwaiter();
		}

		public static UniTask<AsyncGPUReadbackRequest> WithCancellation(this AsyncGPUReadbackRequest asyncOperation, CancellationToken cancellationToken)
		{
			return asyncOperation.ToUniTask(PlayerLoopTiming.Update, cancellationToken);
		}

		public static UniTask<AsyncGPUReadbackRequest> ToUniTask(this AsyncGPUReadbackRequest asyncOperation, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (asyncOperation.done)
			{
				return UniTask.FromResult<AsyncGPUReadbackRequest>(asyncOperation);
			}
			short token;
			return new UniTask<AsyncGPUReadbackRequest>(UnityAsyncExtensions.AsyncGPUReadbackRequestAwaiterConfiguredSource.Create(asyncOperation, timing, cancellationToken, out token), token);
		}

		public static UniTask WithCancellation(this AsyncOperation asyncOperation, CancellationToken cancellationToken)
		{
			return asyncOperation.ToUniTask(null, PlayerLoopTiming.Update, cancellationToken);
		}

		public static UniTask ToUniTask(this AsyncOperation asyncOperation, IProgress<float> progress = null, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<AsyncOperation>(asyncOperation, "asyncOperation");
			if (cancellationToken.IsCancellationRequested)
			{
				return UniTask.FromCanceled(cancellationToken);
			}
			if (asyncOperation.isDone)
			{
				return UniTask.CompletedTask;
			}
			short token;
			return new UniTask(UnityAsyncExtensions.AsyncOperationConfiguredSource.Create(asyncOperation, timing, progress, cancellationToken, out token), token);
		}

		public static UnityAsyncExtensions.ResourceRequestAwaiter GetAwaiter(this ResourceRequest asyncOperation)
		{
			Error.ThrowArgumentNullException<ResourceRequest>(asyncOperation, "asyncOperation");
			return new UnityAsyncExtensions.ResourceRequestAwaiter(asyncOperation);
		}

		public static UniTask<Object> WithCancellation(this ResourceRequest asyncOperation, CancellationToken cancellationToken)
		{
			return asyncOperation.ToUniTask(null, PlayerLoopTiming.Update, cancellationToken);
		}

		public static UniTask<Object> ToUniTask(this ResourceRequest asyncOperation, IProgress<float> progress = null, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<ResourceRequest>(asyncOperation, "asyncOperation");
			if (cancellationToken.IsCancellationRequested)
			{
				return UniTask.FromCanceled<Object>(cancellationToken);
			}
			if (asyncOperation.isDone)
			{
				return UniTask.FromResult<Object>(asyncOperation.asset);
			}
			short token;
			return new UniTask<Object>(UnityAsyncExtensions.ResourceRequestConfiguredSource.Create(asyncOperation, timing, progress, cancellationToken, out token), token);
		}

		public static UnityAsyncExtensions.AssetBundleRequestAwaiter GetAwaiter(this AssetBundleRequest asyncOperation)
		{
			Error.ThrowArgumentNullException<AssetBundleRequest>(asyncOperation, "asyncOperation");
			return new UnityAsyncExtensions.AssetBundleRequestAwaiter(asyncOperation);
		}

		public static UniTask<Object> WithCancellation(this AssetBundleRequest asyncOperation, CancellationToken cancellationToken)
		{
			return asyncOperation.ToUniTask(null, PlayerLoopTiming.Update, cancellationToken);
		}

		public static UniTask<Object> ToUniTask(this AssetBundleRequest asyncOperation, IProgress<float> progress = null, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<AssetBundleRequest>(asyncOperation, "asyncOperation");
			if (cancellationToken.IsCancellationRequested)
			{
				return UniTask.FromCanceled<Object>(cancellationToken);
			}
			if (asyncOperation.isDone)
			{
				return UniTask.FromResult<Object>(asyncOperation.asset);
			}
			short token;
			return new UniTask<Object>(UnityAsyncExtensions.AssetBundleRequestConfiguredSource.Create(asyncOperation, timing, progress, cancellationToken, out token), token);
		}

		public static UnityAsyncExtensions.AssetBundleCreateRequestAwaiter GetAwaiter(this AssetBundleCreateRequest asyncOperation)
		{
			Error.ThrowArgumentNullException<AssetBundleCreateRequest>(asyncOperation, "asyncOperation");
			return new UnityAsyncExtensions.AssetBundleCreateRequestAwaiter(asyncOperation);
		}

		public static UniTask<AssetBundle> WithCancellation(this AssetBundleCreateRequest asyncOperation, CancellationToken cancellationToken)
		{
			return asyncOperation.ToUniTask(null, PlayerLoopTiming.Update, cancellationToken);
		}

		public static UniTask<AssetBundle> ToUniTask(this AssetBundleCreateRequest asyncOperation, IProgress<float> progress = null, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<AssetBundleCreateRequest>(asyncOperation, "asyncOperation");
			if (cancellationToken.IsCancellationRequested)
			{
				return UniTask.FromCanceled<AssetBundle>(cancellationToken);
			}
			if (asyncOperation.isDone)
			{
				return UniTask.FromResult<AssetBundle>(asyncOperation.assetBundle);
			}
			short token;
			return new UniTask<AssetBundle>(UnityAsyncExtensions.AssetBundleCreateRequestConfiguredSource.Create(asyncOperation, timing, progress, cancellationToken, out token), token);
		}

		public static UnityAsyncExtensions.UnityWebRequestAsyncOperationAwaiter GetAwaiter(this UnityWebRequestAsyncOperation asyncOperation)
		{
			Error.ThrowArgumentNullException<UnityWebRequestAsyncOperation>(asyncOperation, "asyncOperation");
			return new UnityAsyncExtensions.UnityWebRequestAsyncOperationAwaiter(asyncOperation);
		}

		public static UniTask<UnityWebRequest> WithCancellation(this UnityWebRequestAsyncOperation asyncOperation, CancellationToken cancellationToken)
		{
			return asyncOperation.ToUniTask(null, PlayerLoopTiming.Update, cancellationToken);
		}

		public static UniTask<UnityWebRequest> ToUniTask(this UnityWebRequestAsyncOperation asyncOperation, IProgress<float> progress = null, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<UnityWebRequestAsyncOperation>(asyncOperation, "asyncOperation");
			if (cancellationToken.IsCancellationRequested)
			{
				return UniTask.FromCanceled<UnityWebRequest>(cancellationToken);
			}
			if (!asyncOperation.isDone)
			{
				short token;
				return new UniTask<UnityWebRequest>(UnityAsyncExtensions.UnityWebRequestAsyncOperationConfiguredSource.Create(asyncOperation, timing, progress, cancellationToken, out token), token);
			}
			if (asyncOperation.webRequest.IsError())
			{
				return UniTask.FromException<UnityWebRequest>(new UnityWebRequestException(asyncOperation.webRequest));
			}
			return UniTask.FromResult<UnityWebRequest>(asyncOperation.webRequest);
		}

		public static UniTask WaitAsync(this JobHandle jobHandle, PlayerLoopTiming waitTiming, CancellationToken cancellationToken = default(CancellationToken))
		{
			UnityAsyncExtensions.<WaitAsync>d__33 <WaitAsync>d__;
			<WaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder.Create();
			<WaitAsync>d__.jobHandle = jobHandle;
			<WaitAsync>d__.waitTiming = waitTiming;
			<WaitAsync>d__.cancellationToken = cancellationToken;
			<WaitAsync>d__.<>1__state = -1;
			<WaitAsync>d__.<>t__builder.Start<UnityAsyncExtensions.<WaitAsync>d__33>(ref <WaitAsync>d__);
			return <WaitAsync>d__.<>t__builder.Task;
		}

		public static UniTask.Awaiter GetAwaiter(this JobHandle jobHandle)
		{
			short token;
			UnityAsyncExtensions.JobHandlePromise jobHandlePromise = UnityAsyncExtensions.JobHandlePromise.Create(jobHandle, out token);
			PlayerLoopHelper.AddAction(PlayerLoopTiming.EarlyUpdate, jobHandlePromise);
			PlayerLoopHelper.AddAction(PlayerLoopTiming.PreUpdate, jobHandlePromise);
			PlayerLoopHelper.AddAction(PlayerLoopTiming.Update, jobHandlePromise);
			PlayerLoopHelper.AddAction(PlayerLoopTiming.PreLateUpdate, jobHandlePromise);
			PlayerLoopHelper.AddAction(PlayerLoopTiming.PostLateUpdate, jobHandlePromise);
			return new UniTask(jobHandlePromise, token).GetAwaiter();
		}

		public static UniTask ToUniTask(this JobHandle jobHandle, PlayerLoopTiming waitTiming)
		{
			short token;
			UnityAsyncExtensions.JobHandlePromise jobHandlePromise = UnityAsyncExtensions.JobHandlePromise.Create(jobHandle, out token);
			PlayerLoopHelper.AddAction(waitTiming, jobHandlePromise);
			return new UniTask(jobHandlePromise, token);
		}

		public static UniTask StartAsyncCoroutine(this MonoBehaviour monoBehaviour, Func<CancellationToken, UniTask> asyncCoroutine)
		{
			CancellationToken cancellationTokenOnDestroy = monoBehaviour.GetCancellationTokenOnDestroy();
			return asyncCoroutine(cancellationTokenOnDestroy);
		}

		public static AsyncUnityEventHandler GetAsyncEventHandler(this UnityEvent unityEvent, CancellationToken cancellationToken)
		{
			return new AsyncUnityEventHandler(unityEvent, cancellationToken, false);
		}

		public static UniTask OnInvokeAsync(this UnityEvent unityEvent, CancellationToken cancellationToken)
		{
			return new AsyncUnityEventHandler(unityEvent, cancellationToken, true).OnInvokeAsync();
		}

		public static IUniTaskAsyncEnumerable<AsyncUnit> OnInvokeAsAsyncEnumerable(this UnityEvent unityEvent, CancellationToken cancellationToken)
		{
			return new UnityEventHandlerAsyncEnumerable(unityEvent, cancellationToken);
		}

		public static AsyncUnityEventHandler<T> GetAsyncEventHandler<T>(this UnityEvent<T> unityEvent, CancellationToken cancellationToken)
		{
			return new AsyncUnityEventHandler<T>(unityEvent, cancellationToken, false);
		}

		public static UniTask<T> OnInvokeAsync<T>(this UnityEvent<T> unityEvent, CancellationToken cancellationToken)
		{
			return new AsyncUnityEventHandler<T>(unityEvent, cancellationToken, true).OnInvokeAsync();
		}

		public static IUniTaskAsyncEnumerable<T> OnInvokeAsAsyncEnumerable<T>(this UnityEvent<T> unityEvent, CancellationToken cancellationToken)
		{
			return new UnityEventHandlerAsyncEnumerable<T>(unityEvent, cancellationToken);
		}

		public static IAsyncClickEventHandler GetAsyncClickEventHandler(this Button button)
		{
			return new AsyncUnityEventHandler(button.onClick, button.GetCancellationTokenOnDestroy(), false);
		}

		public static IAsyncClickEventHandler GetAsyncClickEventHandler(this Button button, CancellationToken cancellationToken)
		{
			return new AsyncUnityEventHandler(button.onClick, cancellationToken, false);
		}

		public static UniTask OnClickAsync(this Button button)
		{
			return new AsyncUnityEventHandler(button.onClick, button.GetCancellationTokenOnDestroy(), true).OnInvokeAsync();
		}

		public static UniTask OnClickAsync(this Button button, CancellationToken cancellationToken)
		{
			return new AsyncUnityEventHandler(button.onClick, cancellationToken, true).OnInvokeAsync();
		}

		public static IUniTaskAsyncEnumerable<AsyncUnit> OnClickAsAsyncEnumerable(this Button button)
		{
			return new UnityEventHandlerAsyncEnumerable(button.onClick, button.GetCancellationTokenOnDestroy());
		}

		public static IUniTaskAsyncEnumerable<AsyncUnit> OnClickAsAsyncEnumerable(this Button button, CancellationToken cancellationToken)
		{
			return new UnityEventHandlerAsyncEnumerable(button.onClick, cancellationToken);
		}

		public static IAsyncValueChangedEventHandler<bool> GetAsyncValueChangedEventHandler(this Toggle toggle)
		{
			return new AsyncUnityEventHandler<bool>(toggle.onValueChanged, toggle.GetCancellationTokenOnDestroy(), false);
		}

		public static IAsyncValueChangedEventHandler<bool> GetAsyncValueChangedEventHandler(this Toggle toggle, CancellationToken cancellationToken)
		{
			return new AsyncUnityEventHandler<bool>(toggle.onValueChanged, cancellationToken, false);
		}

		public static UniTask<bool> OnValueChangedAsync(this Toggle toggle)
		{
			return new AsyncUnityEventHandler<bool>(toggle.onValueChanged, toggle.GetCancellationTokenOnDestroy(), true).OnInvokeAsync();
		}

		public static UniTask<bool> OnValueChangedAsync(this Toggle toggle, CancellationToken cancellationToken)
		{
			return new AsyncUnityEventHandler<bool>(toggle.onValueChanged, cancellationToken, true).OnInvokeAsync();
		}

		public static IUniTaskAsyncEnumerable<bool> OnValueChangedAsAsyncEnumerable(this Toggle toggle)
		{
			return new UnityEventHandlerAsyncEnumerable<bool>(toggle.onValueChanged, toggle.GetCancellationTokenOnDestroy());
		}

		public static IUniTaskAsyncEnumerable<bool> OnValueChangedAsAsyncEnumerable(this Toggle toggle, CancellationToken cancellationToken)
		{
			return new UnityEventHandlerAsyncEnumerable<bool>(toggle.onValueChanged, cancellationToken);
		}

		public static IAsyncValueChangedEventHandler<float> GetAsyncValueChangedEventHandler(this Scrollbar scrollbar)
		{
			return new AsyncUnityEventHandler<float>(scrollbar.onValueChanged, scrollbar.GetCancellationTokenOnDestroy(), false);
		}

		public static IAsyncValueChangedEventHandler<float> GetAsyncValueChangedEventHandler(this Scrollbar scrollbar, CancellationToken cancellationToken)
		{
			return new AsyncUnityEventHandler<float>(scrollbar.onValueChanged, cancellationToken, false);
		}

		public static UniTask<float> OnValueChangedAsync(this Scrollbar scrollbar)
		{
			return new AsyncUnityEventHandler<float>(scrollbar.onValueChanged, scrollbar.GetCancellationTokenOnDestroy(), true).OnInvokeAsync();
		}

		public static UniTask<float> OnValueChangedAsync(this Scrollbar scrollbar, CancellationToken cancellationToken)
		{
			return new AsyncUnityEventHandler<float>(scrollbar.onValueChanged, cancellationToken, true).OnInvokeAsync();
		}

		public static IUniTaskAsyncEnumerable<float> OnValueChangedAsAsyncEnumerable(this Scrollbar scrollbar)
		{
			return new UnityEventHandlerAsyncEnumerable<float>(scrollbar.onValueChanged, scrollbar.GetCancellationTokenOnDestroy());
		}

		public static IUniTaskAsyncEnumerable<float> OnValueChangedAsAsyncEnumerable(this Scrollbar scrollbar, CancellationToken cancellationToken)
		{
			return new UnityEventHandlerAsyncEnumerable<float>(scrollbar.onValueChanged, cancellationToken);
		}

		public static IAsyncValueChangedEventHandler<Vector2> GetAsyncValueChangedEventHandler(this ScrollRect scrollRect)
		{
			return new AsyncUnityEventHandler<Vector2>(scrollRect.onValueChanged, scrollRect.GetCancellationTokenOnDestroy(), false);
		}

		public static IAsyncValueChangedEventHandler<Vector2> GetAsyncValueChangedEventHandler(this ScrollRect scrollRect, CancellationToken cancellationToken)
		{
			return new AsyncUnityEventHandler<Vector2>(scrollRect.onValueChanged, cancellationToken, false);
		}

		public static UniTask<Vector2> OnValueChangedAsync(this ScrollRect scrollRect)
		{
			return new AsyncUnityEventHandler<Vector2>(scrollRect.onValueChanged, scrollRect.GetCancellationTokenOnDestroy(), true).OnInvokeAsync();
		}

		public static UniTask<Vector2> OnValueChangedAsync(this ScrollRect scrollRect, CancellationToken cancellationToken)
		{
			return new AsyncUnityEventHandler<Vector2>(scrollRect.onValueChanged, cancellationToken, true).OnInvokeAsync();
		}

		public static IUniTaskAsyncEnumerable<Vector2> OnValueChangedAsAsyncEnumerable(this ScrollRect scrollRect)
		{
			return new UnityEventHandlerAsyncEnumerable<Vector2>(scrollRect.onValueChanged, scrollRect.GetCancellationTokenOnDestroy());
		}

		public static IUniTaskAsyncEnumerable<Vector2> OnValueChangedAsAsyncEnumerable(this ScrollRect scrollRect, CancellationToken cancellationToken)
		{
			return new UnityEventHandlerAsyncEnumerable<Vector2>(scrollRect.onValueChanged, cancellationToken);
		}

		public static IAsyncValueChangedEventHandler<float> GetAsyncValueChangedEventHandler(this Slider slider)
		{
			return new AsyncUnityEventHandler<float>(slider.onValueChanged, slider.GetCancellationTokenOnDestroy(), false);
		}

		public static IAsyncValueChangedEventHandler<float> GetAsyncValueChangedEventHandler(this Slider slider, CancellationToken cancellationToken)
		{
			return new AsyncUnityEventHandler<float>(slider.onValueChanged, cancellationToken, false);
		}

		public static UniTask<float> OnValueChangedAsync(this Slider slider)
		{
			return new AsyncUnityEventHandler<float>(slider.onValueChanged, slider.GetCancellationTokenOnDestroy(), true).OnInvokeAsync();
		}

		public static UniTask<float> OnValueChangedAsync(this Slider slider, CancellationToken cancellationToken)
		{
			return new AsyncUnityEventHandler<float>(slider.onValueChanged, cancellationToken, true).OnInvokeAsync();
		}

		public static IUniTaskAsyncEnumerable<float> OnValueChangedAsAsyncEnumerable(this Slider slider)
		{
			return new UnityEventHandlerAsyncEnumerable<float>(slider.onValueChanged, slider.GetCancellationTokenOnDestroy());
		}

		public static IUniTaskAsyncEnumerable<float> OnValueChangedAsAsyncEnumerable(this Slider slider, CancellationToken cancellationToken)
		{
			return new UnityEventHandlerAsyncEnumerable<float>(slider.onValueChanged, cancellationToken);
		}

		public static IAsyncEndEditEventHandler<string> GetAsyncEndEditEventHandler(this InputField inputField)
		{
			return new AsyncUnityEventHandler<string>(inputField.onEndEdit, inputField.GetCancellationTokenOnDestroy(), false);
		}

		public static IAsyncEndEditEventHandler<string> GetAsyncEndEditEventHandler(this InputField inputField, CancellationToken cancellationToken)
		{
			return new AsyncUnityEventHandler<string>(inputField.onEndEdit, cancellationToken, false);
		}

		public static UniTask<string> OnEndEditAsync(this InputField inputField)
		{
			return new AsyncUnityEventHandler<string>(inputField.onEndEdit, inputField.GetCancellationTokenOnDestroy(), true).OnInvokeAsync();
		}

		public static UniTask<string> OnEndEditAsync(this InputField inputField, CancellationToken cancellationToken)
		{
			return new AsyncUnityEventHandler<string>(inputField.onEndEdit, cancellationToken, true).OnInvokeAsync();
		}

		public static IUniTaskAsyncEnumerable<string> OnEndEditAsAsyncEnumerable(this InputField inputField)
		{
			return new UnityEventHandlerAsyncEnumerable<string>(inputField.onEndEdit, inputField.GetCancellationTokenOnDestroy());
		}

		public static IUniTaskAsyncEnumerable<string> OnEndEditAsAsyncEnumerable(this InputField inputField, CancellationToken cancellationToken)
		{
			return new UnityEventHandlerAsyncEnumerable<string>(inputField.onEndEdit, cancellationToken);
		}

		public static IAsyncValueChangedEventHandler<string> GetAsyncValueChangedEventHandler(this InputField inputField)
		{
			return new AsyncUnityEventHandler<string>(inputField.onValueChanged, inputField.GetCancellationTokenOnDestroy(), false);
		}

		public static IAsyncValueChangedEventHandler<string> GetAsyncValueChangedEventHandler(this InputField inputField, CancellationToken cancellationToken)
		{
			return new AsyncUnityEventHandler<string>(inputField.onValueChanged, cancellationToken, false);
		}

		public static UniTask<string> OnValueChangedAsync(this InputField inputField)
		{
			return new AsyncUnityEventHandler<string>(inputField.onValueChanged, inputField.GetCancellationTokenOnDestroy(), true).OnInvokeAsync();
		}

		public static UniTask<string> OnValueChangedAsync(this InputField inputField, CancellationToken cancellationToken)
		{
			return new AsyncUnityEventHandler<string>(inputField.onValueChanged, cancellationToken, true).OnInvokeAsync();
		}

		public static IUniTaskAsyncEnumerable<string> OnValueChangedAsAsyncEnumerable(this InputField inputField)
		{
			return new UnityEventHandlerAsyncEnumerable<string>(inputField.onValueChanged, inputField.GetCancellationTokenOnDestroy());
		}

		public static IUniTaskAsyncEnumerable<string> OnValueChangedAsAsyncEnumerable(this InputField inputField, CancellationToken cancellationToken)
		{
			return new UnityEventHandlerAsyncEnumerable<string>(inputField.onValueChanged, cancellationToken);
		}

		public static IAsyncValueChangedEventHandler<int> GetAsyncValueChangedEventHandler(this Dropdown dropdown)
		{
			return new AsyncUnityEventHandler<int>(dropdown.onValueChanged, dropdown.GetCancellationTokenOnDestroy(), false);
		}

		public static IAsyncValueChangedEventHandler<int> GetAsyncValueChangedEventHandler(this Dropdown dropdown, CancellationToken cancellationToken)
		{
			return new AsyncUnityEventHandler<int>(dropdown.onValueChanged, cancellationToken, false);
		}

		public static UniTask<int> OnValueChangedAsync(this Dropdown dropdown)
		{
			return new AsyncUnityEventHandler<int>(dropdown.onValueChanged, dropdown.GetCancellationTokenOnDestroy(), true).OnInvokeAsync();
		}

		public static UniTask<int> OnValueChangedAsync(this Dropdown dropdown, CancellationToken cancellationToken)
		{
			return new AsyncUnityEventHandler<int>(dropdown.onValueChanged, cancellationToken, true).OnInvokeAsync();
		}

		public static IUniTaskAsyncEnumerable<int> OnValueChangedAsAsyncEnumerable(this Dropdown dropdown)
		{
			return new UnityEventHandlerAsyncEnumerable<int>(dropdown.onValueChanged, dropdown.GetCancellationTokenOnDestroy());
		}

		public static IUniTaskAsyncEnumerable<int> OnValueChangedAsAsyncEnumerable(this Dropdown dropdown, CancellationToken cancellationToken)
		{
			return new UnityEventHandlerAsyncEnumerable<int>(dropdown.onValueChanged, cancellationToken);
		}

		public struct AssetBundleRequestAllAssetsAwaiter : ICriticalNotifyCompletion, INotifyCompletion
		{
			public AssetBundleRequestAllAssetsAwaiter(AssetBundleRequest asyncOperation)
			{
				this.asyncOperation = asyncOperation;
				this.continuationAction = null;
			}

			public UnityAsyncExtensions.AssetBundleRequestAllAssetsAwaiter GetAwaiter()
			{
				return this;
			}

			public bool IsCompleted
			{
				get
				{
					return this.asyncOperation.isDone;
				}
			}

			public Object[] GetResult()
			{
				if (this.continuationAction != null)
				{
					this.asyncOperation.completed -= this.continuationAction;
					this.continuationAction = null;
					Object[] allAssets = this.asyncOperation.allAssets;
					this.asyncOperation = null;
					return allAssets;
				}
				Object[] allAssets2 = this.asyncOperation.allAssets;
				this.asyncOperation = null;
				return allAssets2;
			}

			public void OnCompleted(Action continuation)
			{
				this.UnsafeOnCompleted(continuation);
			}

			public void UnsafeOnCompleted(Action continuation)
			{
				Error.ThrowWhenContinuationIsAlreadyRegistered<Action<AsyncOperation>>(this.continuationAction);
				this.continuationAction = PooledDelegate<AsyncOperation>.Create(continuation);
				this.asyncOperation.completed += this.continuationAction;
			}

			private AssetBundleRequest asyncOperation;

			private Action<AsyncOperation> continuationAction;
		}

		private sealed class AssetBundleRequestAllAssetsConfiguredSource : IUniTaskSource<Object[]>, IUniTaskSource, IPlayerLoopItem, ITaskPoolNode<UnityAsyncExtensions.AssetBundleRequestAllAssetsConfiguredSource>
		{
			public ref UnityAsyncExtensions.AssetBundleRequestAllAssetsConfiguredSource NextNode
			{
				get
				{
					return ref this.nextNode;
				}
			}

			static AssetBundleRequestAllAssetsConfiguredSource()
			{
				TaskPool.RegisterSizeGetter(typeof(UnityAsyncExtensions.AssetBundleRequestAllAssetsConfiguredSource), () => UnityAsyncExtensions.AssetBundleRequestAllAssetsConfiguredSource.pool.Size);
			}

			private AssetBundleRequestAllAssetsConfiguredSource()
			{
			}

			public static IUniTaskSource<Object[]> Create(AssetBundleRequest asyncOperation, PlayerLoopTiming timing, IProgress<float> progress, CancellationToken cancellationToken, out short token)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return AutoResetUniTaskCompletionSource<Object[]>.CreateFromCanceled(cancellationToken, out token);
				}
				UnityAsyncExtensions.AssetBundleRequestAllAssetsConfiguredSource assetBundleRequestAllAssetsConfiguredSource;
				if (!UnityAsyncExtensions.AssetBundleRequestAllAssetsConfiguredSource.pool.TryPop(out assetBundleRequestAllAssetsConfiguredSource))
				{
					assetBundleRequestAllAssetsConfiguredSource = new UnityAsyncExtensions.AssetBundleRequestAllAssetsConfiguredSource();
				}
				assetBundleRequestAllAssetsConfiguredSource.asyncOperation = asyncOperation;
				assetBundleRequestAllAssetsConfiguredSource.progress = progress;
				assetBundleRequestAllAssetsConfiguredSource.cancellationToken = cancellationToken;
				PlayerLoopHelper.AddAction(timing, assetBundleRequestAllAssetsConfiguredSource);
				token = assetBundleRequestAllAssetsConfiguredSource.core.Version;
				return assetBundleRequestAllAssetsConfiguredSource;
			}

			public Object[] GetResult(short token)
			{
				Object[] result;
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
				if (this.cancellationToken.IsCancellationRequested)
				{
					this.core.TrySetCanceled(this.cancellationToken);
					return false;
				}
				if (this.progress != null)
				{
					this.progress.Report(this.asyncOperation.progress);
				}
				if (this.asyncOperation.isDone)
				{
					this.core.TrySetResult(this.asyncOperation.allAssets);
					return false;
				}
				return true;
			}

			private bool TryReturn()
			{
				this.core.Reset();
				this.asyncOperation = null;
				this.progress = null;
				this.cancellationToken = default(CancellationToken);
				return UnityAsyncExtensions.AssetBundleRequestAllAssetsConfiguredSource.pool.TryPush(this);
			}

			private static TaskPool<UnityAsyncExtensions.AssetBundleRequestAllAssetsConfiguredSource> pool;

			private UnityAsyncExtensions.AssetBundleRequestAllAssetsConfiguredSource nextNode;

			private AssetBundleRequest asyncOperation;

			private IProgress<float> progress;

			private CancellationToken cancellationToken;

			private UniTaskCompletionSourceCore<Object[]> core;
		}

		private sealed class AsyncGPUReadbackRequestAwaiterConfiguredSource : IUniTaskSource<AsyncGPUReadbackRequest>, IUniTaskSource, IPlayerLoopItem, ITaskPoolNode<UnityAsyncExtensions.AsyncGPUReadbackRequestAwaiterConfiguredSource>
		{
			public ref UnityAsyncExtensions.AsyncGPUReadbackRequestAwaiterConfiguredSource NextNode
			{
				get
				{
					return ref this.nextNode;
				}
			}

			static AsyncGPUReadbackRequestAwaiterConfiguredSource()
			{
				TaskPool.RegisterSizeGetter(typeof(UnityAsyncExtensions.AsyncGPUReadbackRequestAwaiterConfiguredSource), () => UnityAsyncExtensions.AsyncGPUReadbackRequestAwaiterConfiguredSource.pool.Size);
			}

			private AsyncGPUReadbackRequestAwaiterConfiguredSource()
			{
			}

			public static IUniTaskSource<AsyncGPUReadbackRequest> Create(AsyncGPUReadbackRequest asyncOperation, PlayerLoopTiming timing, CancellationToken cancellationToken, out short token)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return AutoResetUniTaskCompletionSource<AsyncGPUReadbackRequest>.CreateFromCanceled(cancellationToken, out token);
				}
				UnityAsyncExtensions.AsyncGPUReadbackRequestAwaiterConfiguredSource asyncGPUReadbackRequestAwaiterConfiguredSource;
				if (!UnityAsyncExtensions.AsyncGPUReadbackRequestAwaiterConfiguredSource.pool.TryPop(out asyncGPUReadbackRequestAwaiterConfiguredSource))
				{
					asyncGPUReadbackRequestAwaiterConfiguredSource = new UnityAsyncExtensions.AsyncGPUReadbackRequestAwaiterConfiguredSource();
				}
				asyncGPUReadbackRequestAwaiterConfiguredSource.asyncOperation = asyncOperation;
				asyncGPUReadbackRequestAwaiterConfiguredSource.cancellationToken = cancellationToken;
				PlayerLoopHelper.AddAction(timing, asyncGPUReadbackRequestAwaiterConfiguredSource);
				token = asyncGPUReadbackRequestAwaiterConfiguredSource.core.Version;
				return asyncGPUReadbackRequestAwaiterConfiguredSource;
			}

			public AsyncGPUReadbackRequest GetResult(short token)
			{
				AsyncGPUReadbackRequest result;
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
				if (this.cancellationToken.IsCancellationRequested)
				{
					this.core.TrySetCanceled(this.cancellationToken);
					return false;
				}
				if (this.asyncOperation.hasError)
				{
					this.core.TrySetException(new Exception("AsyncGPUReadbackRequest.hasError = true"));
					return false;
				}
				if (this.asyncOperation.done)
				{
					this.core.TrySetResult(this.asyncOperation);
					return false;
				}
				return true;
			}

			private bool TryReturn()
			{
				this.core.Reset();
				this.asyncOperation = default(AsyncGPUReadbackRequest);
				this.cancellationToken = default(CancellationToken);
				return UnityAsyncExtensions.AsyncGPUReadbackRequestAwaiterConfiguredSource.pool.TryPush(this);
			}

			private static TaskPool<UnityAsyncExtensions.AsyncGPUReadbackRequestAwaiterConfiguredSource> pool;

			private UnityAsyncExtensions.AsyncGPUReadbackRequestAwaiterConfiguredSource nextNode;

			private AsyncGPUReadbackRequest asyncOperation;

			private CancellationToken cancellationToken;

			private UniTaskCompletionSourceCore<AsyncGPUReadbackRequest> core;
		}

		public struct AsyncOperationAwaiter : ICriticalNotifyCompletion, INotifyCompletion
		{
			public AsyncOperationAwaiter(AsyncOperation asyncOperation)
			{
				this.asyncOperation = asyncOperation;
				this.continuationAction = null;
			}

			public bool IsCompleted
			{
				get
				{
					return this.asyncOperation.isDone;
				}
			}

			public void GetResult()
			{
				if (this.continuationAction != null)
				{
					this.asyncOperation.completed -= this.continuationAction;
					this.continuationAction = null;
					this.asyncOperation = null;
					return;
				}
				this.asyncOperation = null;
			}

			public void OnCompleted(Action continuation)
			{
				this.UnsafeOnCompleted(continuation);
			}

			public void UnsafeOnCompleted(Action continuation)
			{
				Error.ThrowWhenContinuationIsAlreadyRegistered<Action<AsyncOperation>>(this.continuationAction);
				this.continuationAction = PooledDelegate<AsyncOperation>.Create(continuation);
				this.asyncOperation.completed += this.continuationAction;
			}

			private AsyncOperation asyncOperation;

			private Action<AsyncOperation> continuationAction;
		}

		private sealed class AsyncOperationConfiguredSource : IUniTaskSource, IPlayerLoopItem, ITaskPoolNode<UnityAsyncExtensions.AsyncOperationConfiguredSource>
		{
			public ref UnityAsyncExtensions.AsyncOperationConfiguredSource NextNode
			{
				get
				{
					return ref this.nextNode;
				}
			}

			static AsyncOperationConfiguredSource()
			{
				TaskPool.RegisterSizeGetter(typeof(UnityAsyncExtensions.AsyncOperationConfiguredSource), () => UnityAsyncExtensions.AsyncOperationConfiguredSource.pool.Size);
			}

			private AsyncOperationConfiguredSource()
			{
			}

			public static IUniTaskSource Create(AsyncOperation asyncOperation, PlayerLoopTiming timing, IProgress<float> progress, CancellationToken cancellationToken, out short token)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return AutoResetUniTaskCompletionSource.CreateFromCanceled(cancellationToken, out token);
				}
				UnityAsyncExtensions.AsyncOperationConfiguredSource asyncOperationConfiguredSource;
				if (!UnityAsyncExtensions.AsyncOperationConfiguredSource.pool.TryPop(out asyncOperationConfiguredSource))
				{
					asyncOperationConfiguredSource = new UnityAsyncExtensions.AsyncOperationConfiguredSource();
				}
				asyncOperationConfiguredSource.asyncOperation = asyncOperation;
				asyncOperationConfiguredSource.progress = progress;
				asyncOperationConfiguredSource.cancellationToken = cancellationToken;
				PlayerLoopHelper.AddAction(timing, asyncOperationConfiguredSource);
				token = asyncOperationConfiguredSource.core.Version;
				return asyncOperationConfiguredSource;
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
				if (this.progress != null)
				{
					this.progress.Report(this.asyncOperation.progress);
				}
				if (this.asyncOperation.isDone)
				{
					this.core.TrySetResult(AsyncUnit.Default);
					return false;
				}
				return true;
			}

			private bool TryReturn()
			{
				this.core.Reset();
				this.asyncOperation = null;
				this.progress = null;
				this.cancellationToken = default(CancellationToken);
				return UnityAsyncExtensions.AsyncOperationConfiguredSource.pool.TryPush(this);
			}

			private static TaskPool<UnityAsyncExtensions.AsyncOperationConfiguredSource> pool;

			private UnityAsyncExtensions.AsyncOperationConfiguredSource nextNode;

			private AsyncOperation asyncOperation;

			private IProgress<float> progress;

			private CancellationToken cancellationToken;

			private UniTaskCompletionSourceCore<AsyncUnit> core;
		}

		public struct ResourceRequestAwaiter : ICriticalNotifyCompletion, INotifyCompletion
		{
			public ResourceRequestAwaiter(ResourceRequest asyncOperation)
			{
				this.asyncOperation = asyncOperation;
				this.continuationAction = null;
			}

			public bool IsCompleted
			{
				get
				{
					return this.asyncOperation.isDone;
				}
			}

			public Object GetResult()
			{
				if (this.continuationAction != null)
				{
					this.asyncOperation.completed -= this.continuationAction;
					this.continuationAction = null;
					Object asset = this.asyncOperation.asset;
					this.asyncOperation = null;
					return asset;
				}
				Object asset2 = this.asyncOperation.asset;
				this.asyncOperation = null;
				return asset2;
			}

			public void OnCompleted(Action continuation)
			{
				this.UnsafeOnCompleted(continuation);
			}

			public void UnsafeOnCompleted(Action continuation)
			{
				Error.ThrowWhenContinuationIsAlreadyRegistered<Action<AsyncOperation>>(this.continuationAction);
				this.continuationAction = PooledDelegate<AsyncOperation>.Create(continuation);
				this.asyncOperation.completed += this.continuationAction;
			}

			private ResourceRequest asyncOperation;

			private Action<AsyncOperation> continuationAction;
		}

		private sealed class ResourceRequestConfiguredSource : IUniTaskSource<Object>, IUniTaskSource, IPlayerLoopItem, ITaskPoolNode<UnityAsyncExtensions.ResourceRequestConfiguredSource>
		{
			public ref UnityAsyncExtensions.ResourceRequestConfiguredSource NextNode
			{
				get
				{
					return ref this.nextNode;
				}
			}

			static ResourceRequestConfiguredSource()
			{
				TaskPool.RegisterSizeGetter(typeof(UnityAsyncExtensions.ResourceRequestConfiguredSource), () => UnityAsyncExtensions.ResourceRequestConfiguredSource.pool.Size);
			}

			private ResourceRequestConfiguredSource()
			{
			}

			public static IUniTaskSource<Object> Create(ResourceRequest asyncOperation, PlayerLoopTiming timing, IProgress<float> progress, CancellationToken cancellationToken, out short token)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return AutoResetUniTaskCompletionSource<Object>.CreateFromCanceled(cancellationToken, out token);
				}
				UnityAsyncExtensions.ResourceRequestConfiguredSource resourceRequestConfiguredSource;
				if (!UnityAsyncExtensions.ResourceRequestConfiguredSource.pool.TryPop(out resourceRequestConfiguredSource))
				{
					resourceRequestConfiguredSource = new UnityAsyncExtensions.ResourceRequestConfiguredSource();
				}
				resourceRequestConfiguredSource.asyncOperation = asyncOperation;
				resourceRequestConfiguredSource.progress = progress;
				resourceRequestConfiguredSource.cancellationToken = cancellationToken;
				PlayerLoopHelper.AddAction(timing, resourceRequestConfiguredSource);
				token = resourceRequestConfiguredSource.core.Version;
				return resourceRequestConfiguredSource;
			}

			public Object GetResult(short token)
			{
				Object result;
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
				if (this.cancellationToken.IsCancellationRequested)
				{
					this.core.TrySetCanceled(this.cancellationToken);
					return false;
				}
				if (this.progress != null)
				{
					this.progress.Report(this.asyncOperation.progress);
				}
				if (this.asyncOperation.isDone)
				{
					this.core.TrySetResult(this.asyncOperation.asset);
					return false;
				}
				return true;
			}

			private bool TryReturn()
			{
				this.core.Reset();
				this.asyncOperation = null;
				this.progress = null;
				this.cancellationToken = default(CancellationToken);
				return UnityAsyncExtensions.ResourceRequestConfiguredSource.pool.TryPush(this);
			}

			private static TaskPool<UnityAsyncExtensions.ResourceRequestConfiguredSource> pool;

			private UnityAsyncExtensions.ResourceRequestConfiguredSource nextNode;

			private ResourceRequest asyncOperation;

			private IProgress<float> progress;

			private CancellationToken cancellationToken;

			private UniTaskCompletionSourceCore<Object> core;
		}

		public struct AssetBundleRequestAwaiter : ICriticalNotifyCompletion, INotifyCompletion
		{
			public AssetBundleRequestAwaiter(AssetBundleRequest asyncOperation)
			{
				this.asyncOperation = asyncOperation;
				this.continuationAction = null;
			}

			public bool IsCompleted
			{
				get
				{
					return this.asyncOperation.isDone;
				}
			}

			public Object GetResult()
			{
				if (this.continuationAction != null)
				{
					this.asyncOperation.completed -= this.continuationAction;
					this.continuationAction = null;
					Object asset = this.asyncOperation.asset;
					this.asyncOperation = null;
					return asset;
				}
				Object asset2 = this.asyncOperation.asset;
				this.asyncOperation = null;
				return asset2;
			}

			public void OnCompleted(Action continuation)
			{
				this.UnsafeOnCompleted(continuation);
			}

			public void UnsafeOnCompleted(Action continuation)
			{
				Error.ThrowWhenContinuationIsAlreadyRegistered<Action<AsyncOperation>>(this.continuationAction);
				this.continuationAction = PooledDelegate<AsyncOperation>.Create(continuation);
				this.asyncOperation.completed += this.continuationAction;
			}

			private AssetBundleRequest asyncOperation;

			private Action<AsyncOperation> continuationAction;
		}

		private sealed class AssetBundleRequestConfiguredSource : IUniTaskSource<Object>, IUniTaskSource, IPlayerLoopItem, ITaskPoolNode<UnityAsyncExtensions.AssetBundleRequestConfiguredSource>
		{
			public ref UnityAsyncExtensions.AssetBundleRequestConfiguredSource NextNode
			{
				get
				{
					return ref this.nextNode;
				}
			}

			static AssetBundleRequestConfiguredSource()
			{
				TaskPool.RegisterSizeGetter(typeof(UnityAsyncExtensions.AssetBundleRequestConfiguredSource), () => UnityAsyncExtensions.AssetBundleRequestConfiguredSource.pool.Size);
			}

			private AssetBundleRequestConfiguredSource()
			{
			}

			public static IUniTaskSource<Object> Create(AssetBundleRequest asyncOperation, PlayerLoopTiming timing, IProgress<float> progress, CancellationToken cancellationToken, out short token)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return AutoResetUniTaskCompletionSource<Object>.CreateFromCanceled(cancellationToken, out token);
				}
				UnityAsyncExtensions.AssetBundleRequestConfiguredSource assetBundleRequestConfiguredSource;
				if (!UnityAsyncExtensions.AssetBundleRequestConfiguredSource.pool.TryPop(out assetBundleRequestConfiguredSource))
				{
					assetBundleRequestConfiguredSource = new UnityAsyncExtensions.AssetBundleRequestConfiguredSource();
				}
				assetBundleRequestConfiguredSource.asyncOperation = asyncOperation;
				assetBundleRequestConfiguredSource.progress = progress;
				assetBundleRequestConfiguredSource.cancellationToken = cancellationToken;
				PlayerLoopHelper.AddAction(timing, assetBundleRequestConfiguredSource);
				token = assetBundleRequestConfiguredSource.core.Version;
				return assetBundleRequestConfiguredSource;
			}

			public Object GetResult(short token)
			{
				Object result;
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
				if (this.cancellationToken.IsCancellationRequested)
				{
					this.core.TrySetCanceled(this.cancellationToken);
					return false;
				}
				if (this.progress != null)
				{
					this.progress.Report(this.asyncOperation.progress);
				}
				if (this.asyncOperation.isDone)
				{
					this.core.TrySetResult(this.asyncOperation.asset);
					return false;
				}
				return true;
			}

			private bool TryReturn()
			{
				this.core.Reset();
				this.asyncOperation = null;
				this.progress = null;
				this.cancellationToken = default(CancellationToken);
				return UnityAsyncExtensions.AssetBundleRequestConfiguredSource.pool.TryPush(this);
			}

			private static TaskPool<UnityAsyncExtensions.AssetBundleRequestConfiguredSource> pool;

			private UnityAsyncExtensions.AssetBundleRequestConfiguredSource nextNode;

			private AssetBundleRequest asyncOperation;

			private IProgress<float> progress;

			private CancellationToken cancellationToken;

			private UniTaskCompletionSourceCore<Object> core;
		}

		public struct AssetBundleCreateRequestAwaiter : ICriticalNotifyCompletion, INotifyCompletion
		{
			public AssetBundleCreateRequestAwaiter(AssetBundleCreateRequest asyncOperation)
			{
				this.asyncOperation = asyncOperation;
				this.continuationAction = null;
			}

			public bool IsCompleted
			{
				get
				{
					return this.asyncOperation.isDone;
				}
			}

			public AssetBundle GetResult()
			{
				if (this.continuationAction != null)
				{
					this.asyncOperation.completed -= this.continuationAction;
					this.continuationAction = null;
					AssetBundle assetBundle = this.asyncOperation.assetBundle;
					this.asyncOperation = null;
					return assetBundle;
				}
				AssetBundle assetBundle2 = this.asyncOperation.assetBundle;
				this.asyncOperation = null;
				return assetBundle2;
			}

			public void OnCompleted(Action continuation)
			{
				this.UnsafeOnCompleted(continuation);
			}

			public void UnsafeOnCompleted(Action continuation)
			{
				Error.ThrowWhenContinuationIsAlreadyRegistered<Action<AsyncOperation>>(this.continuationAction);
				this.continuationAction = PooledDelegate<AsyncOperation>.Create(continuation);
				this.asyncOperation.completed += this.continuationAction;
			}

			private AssetBundleCreateRequest asyncOperation;

			private Action<AsyncOperation> continuationAction;
		}

		private sealed class AssetBundleCreateRequestConfiguredSource : IUniTaskSource<AssetBundle>, IUniTaskSource, IPlayerLoopItem, ITaskPoolNode<UnityAsyncExtensions.AssetBundleCreateRequestConfiguredSource>
		{
			public ref UnityAsyncExtensions.AssetBundleCreateRequestConfiguredSource NextNode
			{
				get
				{
					return ref this.nextNode;
				}
			}

			static AssetBundleCreateRequestConfiguredSource()
			{
				TaskPool.RegisterSizeGetter(typeof(UnityAsyncExtensions.AssetBundleCreateRequestConfiguredSource), () => UnityAsyncExtensions.AssetBundleCreateRequestConfiguredSource.pool.Size);
			}

			private AssetBundleCreateRequestConfiguredSource()
			{
			}

			public static IUniTaskSource<AssetBundle> Create(AssetBundleCreateRequest asyncOperation, PlayerLoopTiming timing, IProgress<float> progress, CancellationToken cancellationToken, out short token)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return AutoResetUniTaskCompletionSource<AssetBundle>.CreateFromCanceled(cancellationToken, out token);
				}
				UnityAsyncExtensions.AssetBundleCreateRequestConfiguredSource assetBundleCreateRequestConfiguredSource;
				if (!UnityAsyncExtensions.AssetBundleCreateRequestConfiguredSource.pool.TryPop(out assetBundleCreateRequestConfiguredSource))
				{
					assetBundleCreateRequestConfiguredSource = new UnityAsyncExtensions.AssetBundleCreateRequestConfiguredSource();
				}
				assetBundleCreateRequestConfiguredSource.asyncOperation = asyncOperation;
				assetBundleCreateRequestConfiguredSource.progress = progress;
				assetBundleCreateRequestConfiguredSource.cancellationToken = cancellationToken;
				PlayerLoopHelper.AddAction(timing, assetBundleCreateRequestConfiguredSource);
				token = assetBundleCreateRequestConfiguredSource.core.Version;
				return assetBundleCreateRequestConfiguredSource;
			}

			public AssetBundle GetResult(short token)
			{
				AssetBundle result;
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
				if (this.cancellationToken.IsCancellationRequested)
				{
					this.core.TrySetCanceled(this.cancellationToken);
					return false;
				}
				if (this.progress != null)
				{
					this.progress.Report(this.asyncOperation.progress);
				}
				if (this.asyncOperation.isDone)
				{
					this.core.TrySetResult(this.asyncOperation.assetBundle);
					return false;
				}
				return true;
			}

			private bool TryReturn()
			{
				this.core.Reset();
				this.asyncOperation = null;
				this.progress = null;
				this.cancellationToken = default(CancellationToken);
				return UnityAsyncExtensions.AssetBundleCreateRequestConfiguredSource.pool.TryPush(this);
			}

			private static TaskPool<UnityAsyncExtensions.AssetBundleCreateRequestConfiguredSource> pool;

			private UnityAsyncExtensions.AssetBundleCreateRequestConfiguredSource nextNode;

			private AssetBundleCreateRequest asyncOperation;

			private IProgress<float> progress;

			private CancellationToken cancellationToken;

			private UniTaskCompletionSourceCore<AssetBundle> core;
		}

		public struct UnityWebRequestAsyncOperationAwaiter : ICriticalNotifyCompletion, INotifyCompletion
		{
			public UnityWebRequestAsyncOperationAwaiter(UnityWebRequestAsyncOperation asyncOperation)
			{
				this.asyncOperation = asyncOperation;
				this.continuationAction = null;
			}

			public bool IsCompleted
			{
				get
				{
					return this.asyncOperation.isDone;
				}
			}

			public UnityWebRequest GetResult()
			{
				if (this.continuationAction != null)
				{
					this.asyncOperation.completed -= this.continuationAction;
					this.continuationAction = null;
					UnityWebRequest webRequest = this.asyncOperation.webRequest;
					this.asyncOperation = null;
					if (webRequest.IsError())
					{
						throw new UnityWebRequestException(webRequest);
					}
					return webRequest;
				}
				else
				{
					UnityWebRequest webRequest2 = this.asyncOperation.webRequest;
					this.asyncOperation = null;
					if (webRequest2.IsError())
					{
						throw new UnityWebRequestException(webRequest2);
					}
					return webRequest2;
				}
			}

			public void OnCompleted(Action continuation)
			{
				this.UnsafeOnCompleted(continuation);
			}

			public void UnsafeOnCompleted(Action continuation)
			{
				Error.ThrowWhenContinuationIsAlreadyRegistered<Action<AsyncOperation>>(this.continuationAction);
				this.continuationAction = PooledDelegate<AsyncOperation>.Create(continuation);
				this.asyncOperation.completed += this.continuationAction;
			}

			private UnityWebRequestAsyncOperation asyncOperation;

			private Action<AsyncOperation> continuationAction;
		}

		private sealed class UnityWebRequestAsyncOperationConfiguredSource : IUniTaskSource<UnityWebRequest>, IUniTaskSource, IPlayerLoopItem, ITaskPoolNode<UnityAsyncExtensions.UnityWebRequestAsyncOperationConfiguredSource>
		{
			public ref UnityAsyncExtensions.UnityWebRequestAsyncOperationConfiguredSource NextNode
			{
				get
				{
					return ref this.nextNode;
				}
			}

			static UnityWebRequestAsyncOperationConfiguredSource()
			{
				TaskPool.RegisterSizeGetter(typeof(UnityAsyncExtensions.UnityWebRequestAsyncOperationConfiguredSource), () => UnityAsyncExtensions.UnityWebRequestAsyncOperationConfiguredSource.pool.Size);
			}

			private UnityWebRequestAsyncOperationConfiguredSource()
			{
			}

			public static IUniTaskSource<UnityWebRequest> Create(UnityWebRequestAsyncOperation asyncOperation, PlayerLoopTiming timing, IProgress<float> progress, CancellationToken cancellationToken, out short token)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return AutoResetUniTaskCompletionSource<UnityWebRequest>.CreateFromCanceled(cancellationToken, out token);
				}
				UnityAsyncExtensions.UnityWebRequestAsyncOperationConfiguredSource unityWebRequestAsyncOperationConfiguredSource;
				if (!UnityAsyncExtensions.UnityWebRequestAsyncOperationConfiguredSource.pool.TryPop(out unityWebRequestAsyncOperationConfiguredSource))
				{
					unityWebRequestAsyncOperationConfiguredSource = new UnityAsyncExtensions.UnityWebRequestAsyncOperationConfiguredSource();
				}
				unityWebRequestAsyncOperationConfiguredSource.asyncOperation = asyncOperation;
				unityWebRequestAsyncOperationConfiguredSource.progress = progress;
				unityWebRequestAsyncOperationConfiguredSource.cancellationToken = cancellationToken;
				PlayerLoopHelper.AddAction(timing, unityWebRequestAsyncOperationConfiguredSource);
				token = unityWebRequestAsyncOperationConfiguredSource.core.Version;
				return unityWebRequestAsyncOperationConfiguredSource;
			}

			public UnityWebRequest GetResult(short token)
			{
				UnityWebRequest result;
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
				if (this.cancellationToken.IsCancellationRequested)
				{
					this.asyncOperation.webRequest.Abort();
					this.core.TrySetCanceled(this.cancellationToken);
					return false;
				}
				if (this.progress != null)
				{
					this.progress.Report(this.asyncOperation.progress);
				}
				if (this.asyncOperation.isDone)
				{
					if (this.asyncOperation.webRequest.IsError())
					{
						this.core.TrySetException(new UnityWebRequestException(this.asyncOperation.webRequest));
					}
					else
					{
						this.core.TrySetResult(this.asyncOperation.webRequest);
					}
					return false;
				}
				return true;
			}

			private bool TryReturn()
			{
				this.core.Reset();
				this.asyncOperation = null;
				this.progress = null;
				this.cancellationToken = default(CancellationToken);
				return UnityAsyncExtensions.UnityWebRequestAsyncOperationConfiguredSource.pool.TryPush(this);
			}

			private static TaskPool<UnityAsyncExtensions.UnityWebRequestAsyncOperationConfiguredSource> pool;

			private UnityAsyncExtensions.UnityWebRequestAsyncOperationConfiguredSource nextNode;

			private UnityWebRequestAsyncOperation asyncOperation;

			private IProgress<float> progress;

			private CancellationToken cancellationToken;

			private UniTaskCompletionSourceCore<UnityWebRequest> core;
		}

		private sealed class JobHandlePromise : IUniTaskSource, IPlayerLoopItem
		{
			public static UnityAsyncExtensions.JobHandlePromise Create(JobHandle jobHandle, out short token)
			{
				UnityAsyncExtensions.JobHandlePromise jobHandlePromise = new UnityAsyncExtensions.JobHandlePromise();
				jobHandlePromise.jobHandle = jobHandle;
				token = jobHandlePromise.core.Version;
				return jobHandlePromise;
			}

			public void GetResult(short token)
			{
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

			public bool MoveNext()
			{
				if (this.jobHandle.IsCompleted | PlayerLoopHelper.IsEditorApplicationQuitting)
				{
					this.jobHandle.Complete();
					this.core.TrySetResult(AsyncUnit.Default);
					return false;
				}
				return true;
			}

			private JobHandle jobHandle;

			private UniTaskCompletionSourceCore<AsyncUnit> core;
		}
	}
}
