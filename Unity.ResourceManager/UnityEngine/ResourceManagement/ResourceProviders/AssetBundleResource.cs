using System;
using System.IO;
using System.Threading;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.Exceptions;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.Util;

namespace UnityEngine.ResourceManagement.ResourceProviders
{
	public class AssetBundleResource : IAssetBundleResource, IUpdateReceiver
	{
		private bool HasTimedOut
		{
			get
			{
				return this.m_Options != null && this.m_TimeoutTimer >= (float)this.m_Options.Timeout && this.m_TimeoutOverFrames > 5;
			}
		}

		internal long BytesToDownload
		{
			get
			{
				if (this.m_BytesToDownload == -1L)
				{
					if (this.m_Options != null && !this.IsCached())
					{
						this.m_BytesToDownload = this.m_Options.ComputeSize(this.m_ProvideHandle.Location, this.m_ProvideHandle.ResourceManager);
					}
					else
					{
						this.m_BytesToDownload = 0L;
					}
				}
				return this.m_BytesToDownload;
			}
		}

		internal bool IsCached()
		{
			if (this.cacheStatus != AssetBundleResource.CacheStatus.Unknown)
			{
				return this.cacheStatus == AssetBundleResource.CacheStatus.Cached;
			}
			this.cacheStatus = AssetBundleResource.CacheStatus.NotCached;
			Hash128 hash = Hash128.Parse(this.m_Options.Hash);
			if (hash.isValid && Caching.IsVersionCached(new CachedAssetBundle(this.m_Options.BundleName, hash)))
			{
				this.cacheStatus = AssetBundleResource.CacheStatus.Cached;
			}
			return this.cacheStatus == AssetBundleResource.CacheStatus.Cached;
		}

		internal UnityWebRequest CreateWebRequest(IResourceLocation loc)
		{
			string url = this.m_ProvideHandle.ResourceManager.TransformInternalId(loc);
			return this.CreateWebRequest(url);
		}

		internal UnityWebRequest CreateWebRequest(string url)
		{
			Uri uri = new Uri(Uri.UnescapeDataString(url).Replace(" ", "%20"));
			if (this.m_Options == null)
			{
				this.m_Source = BundleSource.Download;
				return UnityWebRequestAssetBundle.GetAssetBundle(uri);
			}
			UnityWebRequest assetBundle;
			if (!string.IsNullOrEmpty(this.m_Options.Hash))
			{
				bool flag = this.IsCached();
				CachedAssetBundle cachedAssetBundle = new CachedAssetBundle(this.m_Options.BundleName, Hash128.Parse(this.m_Options.Hash));
				this.m_Source = (flag ? BundleSource.Cache : BundleSource.Download);
				if (this.m_Options.UseCrcForCachedBundle || this.m_Source == BundleSource.Download)
				{
					assetBundle = UnityWebRequestAssetBundle.GetAssetBundle(uri, cachedAssetBundle, this.m_Options.Crc);
				}
				else
				{
					assetBundle = UnityWebRequestAssetBundle.GetAssetBundle(uri, cachedAssetBundle, 0U);
				}
			}
			else
			{
				this.m_Source = BundleSource.Download;
				assetBundle = UnityWebRequestAssetBundle.GetAssetBundle(uri, this.m_Options.Crc);
			}
			if (this.m_Options.RedirectLimit >= 0 && this.m_Options.RedirectLimit < 129)
			{
				assetBundle.redirectLimit = this.m_Options.RedirectLimit;
			}
			if (this.m_ProvideHandle.ResourceManager.CertificateHandlerInstance != null)
			{
				assetBundle.certificateHandler = this.m_ProvideHandle.ResourceManager.CertificateHandlerInstance;
				assetBundle.disposeCertificateHandlerOnDispose = false;
			}
			Action<UnityWebRequest> webRequestOverride = this.m_ProvideHandle.ResourceManager.WebRequestOverride;
			if (webRequestOverride != null)
			{
				webRequestOverride(assetBundle);
			}
			return assetBundle;
		}

		public AssetBundleRequest GetAssetPreloadRequest()
		{
			if (this.m_PreloadCompleted || this.GetAssetBundle() == null || this.m_Options == null)
			{
				return null;
			}
			if (this.m_Options.AssetLoadMode == AssetLoadMode.AllPackedAssetsAndDependencies)
			{
				if (this.m_PreloadRequest == null)
				{
					this.m_PreloadRequest = this.m_AssetBundle.LoadAllAssetsAsync();
					this.m_PreloadRequest.completed += delegate(AsyncOperation operation)
					{
						this.m_PreloadCompleted = true;
					};
				}
				return this.m_PreloadRequest;
			}
			return null;
		}

		private float PercentComplete()
		{
			if (this.m_RequestOperation == null)
			{
				return 0f;
			}
			return this.m_RequestOperation.progress;
		}

		private DownloadStatus GetDownloadStatus()
		{
			if (this.m_Options == null)
			{
				return default(DownloadStatus);
			}
			DownloadStatus result = new DownloadStatus
			{
				TotalBytes = this.BytesToDownload,
				IsDone = (this.PercentComplete() >= 1f)
			};
			if (this.BytesToDownload > 0L)
			{
				if (this.m_WebRequestQueueOperation != null && string.IsNullOrEmpty(this.m_WebRequestQueueOperation.m_WebRequest.error))
				{
					this.m_DownloadedBytes = (long)this.m_WebRequestQueueOperation.m_WebRequest.downloadedBytes;
				}
				else if (this.m_RequestOperation != null)
				{
					UnityWebRequestAsyncOperation unityWebRequestAsyncOperation = this.m_RequestOperation as UnityWebRequestAsyncOperation;
					if (unityWebRequestAsyncOperation != null && string.IsNullOrEmpty(unityWebRequestAsyncOperation.webRequest.error))
					{
						this.m_DownloadedBytes = (long)unityWebRequestAsyncOperation.webRequest.downloadedBytes;
					}
				}
			}
			result.DownloadedBytes = this.m_DownloadedBytes;
			return result;
		}

		public AssetBundle GetAssetBundle()
		{
			bool isValid = this.m_ProvideHandle.IsValid;
			return this.m_AssetBundle;
		}

		private void OnUnloadOperationComplete(AsyncOperation op)
		{
			this.m_UnloadOperation = null;
			this.BeginOperation();
		}

		public void Start(ProvideHandle provideHandle, AssetBundleUnloadOperation unloadOp, Func<UnityWebRequestResult, bool> requestRetryCallback)
		{
			this.m_Retries = 0;
			this.m_AssetBundle = null;
			this.m_RequestOperation = null;
			this.m_RequestCompletedCallbackCalled = false;
			this.m_ProvideHandle = provideHandle;
			this.m_Options = (this.m_ProvideHandle.Location.Data as AssetBundleRequestOptions);
			this.m_BytesToDownload = -1L;
			this.m_DownloadOnly = (this.m_ProvideHandle.Location is DownloadOnlyLocation);
			if (this.m_DownloadOnly && this.m_Options == null)
			{
				this.m_ProvideHandle.Complete<AssetBundleResource>(null, false, new RemoteProviderException("Attempt made to download bundle with stripped AssetBundleRequestOptions.  Ensure that StripDownloadOptions is not enabled for this bundle's group. '" + this.m_TransformedInternalId + "'.", null, null, null));
				return;
			}
			this.m_ProvideHandle.SetProgressCallback(new Func<float>(this.PercentComplete));
			this.m_ProvideHandle.SetDownloadProgressCallbacks(new Func<DownloadStatus>(this.GetDownloadStatus));
			this.m_ProvideHandle.SetWaitForCompletionCallback(new Func<bool>(this.WaitForCompletionHandler));
			this.m_RequestRetryCallback = requestRetryCallback;
			this.m_UnloadOperation = unloadOp;
			if (this.m_UnloadOperation != null && !this.m_UnloadOperation.isDone)
			{
				this.m_UnloadOperation.completed += this.OnUnloadOperationComplete;
				return;
			}
			this.BeginOperation();
		}

		private bool WaitForCompletionHandler()
		{
			if (this.m_UnloadOperation != null && !this.m_UnloadOperation.isDone)
			{
				this.m_UnloadOperation.completed -= this.OnUnloadOperationComplete;
				this.m_UnloadOperation.WaitForCompletion();
				this.m_UnloadOperation = null;
				this.BeginOperation();
			}
			if (this.m_RequestOperation == null)
			{
				if (this.m_WebRequestQueueOperation == null)
				{
					return false;
				}
				WebRequestQueue.WaitForRequestToBeActive(this.m_WebRequestQueueOperation, 1);
			}
			UnityWebRequestAsyncOperation unityWebRequestAsyncOperation = this.m_RequestOperation as UnityWebRequestAsyncOperation;
			if (unityWebRequestAsyncOperation != null)
			{
				while (!UnityWebRequestUtilities.IsAssetBundleDownloaded(unityWebRequestAsyncOperation))
				{
					Thread.Sleep(1);
				}
				if (this.m_Source == BundleSource.Cache)
				{
					object obj;
					if (unityWebRequestAsyncOperation == null)
					{
						obj = null;
					}
					else
					{
						UnityWebRequest webRequest = unityWebRequestAsyncOperation.webRequest;
						obj = ((webRequest != null) ? webRequest.downloadHandler : null);
					}
					DownloadHandlerAssetBundle downloadHandlerAssetBundle = (DownloadHandlerAssetBundle)obj;
					if (downloadHandlerAssetBundle.autoLoadAssetBundle)
					{
						this.m_AssetBundle = downloadHandlerAssetBundle.assetBundle;
					}
				}
				WebRequestQueue.DequeueRequest(unityWebRequestAsyncOperation);
				if (!this.m_RequestCompletedCallbackCalled)
				{
					this.m_RequestOperation.completed -= this.WebRequestOperationCompleted;
					this.WebRequestOperationCompleted(this.m_RequestOperation);
				}
			}
			if (!this.m_Completed && this.m_Source == BundleSource.Local && !this.m_RequestCompletedCallbackCalled)
			{
				this.m_RequestOperation.completed -= this.LocalRequestOperationCompleted;
				this.LocalRequestOperationCompleted(this.m_RequestOperation);
			}
			if (!this.m_Completed && this.m_RequestOperation.isDone)
			{
				this.m_ProvideHandle.Complete<AssetBundleResource>(this, this.m_AssetBundle != null, null);
				this.m_Completed = true;
			}
			return this.m_Completed;
		}

		private void AddCallbackInvokeIfDone(AsyncOperation operation, Action<AsyncOperation> callback)
		{
			if (operation.isDone)
			{
				callback(operation);
				return;
			}
			operation.completed += callback;
		}

		public static void GetLoadInfo(ProvideHandle handle, out AssetBundleResource.LoadType loadType, out string path)
		{
			AssetBundleResource.GetLoadInfo(handle.Location, handle.ResourceManager, out loadType, out path);
		}

		internal static void GetLoadInfo(IResourceLocation location, ResourceManager resourceManager, out AssetBundleResource.LoadType loadType, out string path)
		{
			AssetBundleRequestOptions assetBundleRequestOptions = ((location != null) ? location.Data : null) as AssetBundleRequestOptions;
			if (assetBundleRequestOptions == null)
			{
				loadType = AssetBundleResource.LoadType.Local;
				path = resourceManager.TransformInternalId(location);
				if (ResourceManagerConfig.ShouldPathUseWebRequest(path))
				{
					Debug.LogWarning(string.Format("Location {0} appears to be remote but the download option have been stripped.  Ensure that the group that contains this bundle does not have StripDownloadOptions enabled.", location));
				}
				return;
			}
			path = resourceManager.TransformInternalId(location);
			if (Application.platform == RuntimePlatform.Android && path.StartsWith("jar:", StringComparison.Ordinal))
			{
				loadType = (assetBundleRequestOptions.UseUnityWebRequestForLocalBundles ? AssetBundleResource.LoadType.Web : AssetBundleResource.LoadType.Local);
			}
			else if (ResourceManagerConfig.ShouldPathUseWebRequest(path))
			{
				loadType = AssetBundleResource.LoadType.Web;
			}
			else if (assetBundleRequestOptions.UseUnityWebRequestForLocalBundles)
			{
				path = "file:///" + Path.GetFullPath(path);
				loadType = AssetBundleResource.LoadType.Web;
			}
			else
			{
				loadType = AssetBundleResource.LoadType.Local;
			}
			if (loadType == AssetBundleResource.LoadType.Web)
			{
				path = path.Replace('\\', '/');
			}
		}

		private void BeginOperation()
		{
			this.m_DownloadedBytes = 0L;
			this.m_RequestCompletedCallbackCalled = false;
			AssetBundleResource.LoadType loadType;
			AssetBundleResource.GetLoadInfo(this.m_ProvideHandle, out loadType, out this.m_TransformedInternalId);
			bool flag = this.m_ProvideHandle.Location is DownloadOnlyLocation;
			if (loadType == AssetBundleResource.LoadType.Local)
			{
				if (flag)
				{
					this.m_Source = BundleSource.Local;
					this.m_RequestOperation = null;
					this.m_ProvideHandle.Complete<AssetBundleResource>(null, true, null);
					this.m_Completed = true;
					return;
				}
				this.LoadLocalBundle();
				return;
			}
			else
			{
				bool useCrcForCachedBundle = this.m_Options.UseCrcForCachedBundle;
				new CachedAssetBundle(this.m_Options.BundleName, Hash128.Parse(this.m_Options.Hash));
				bool flag2 = this.IsCached();
				if (loadType == AssetBundleResource.LoadType.Web && flag && flag2 && !useCrcForCachedBundle)
				{
					this.m_Source = BundleSource.Cache;
					this.m_RequestOperation = null;
					this.m_ProvideHandle.Complete<AssetBundleResource>(null, true, null);
					this.m_Completed = true;
					return;
				}
				if (loadType == AssetBundleResource.LoadType.Web)
				{
					this.m_WebRequestQueueOperation = this.EnqueueWebRequest(this.m_TransformedInternalId);
					this.AddBeginWebRequestHandler(this.m_WebRequestQueueOperation);
					return;
				}
				this.m_Source = BundleSource.None;
				this.m_RequestOperation = null;
				this.m_ProvideHandle.Complete<AssetBundleResource>(null, false, new RemoteProviderException(string.Format("Invalid path in AssetBundleProvider: '{0}'.", this.m_TransformedInternalId), this.m_ProvideHandle.Location, null, null));
				this.m_Completed = true;
				return;
			}
		}

		private void LoadLocalBundle()
		{
			this.m_Source = BundleSource.Local;
			this.m_RequestOperation = AssetBundle.LoadFromFileAsync(this.m_TransformedInternalId, (this.m_Options == null) ? 0U : this.m_Options.Crc);
			this.AddCallbackInvokeIfDone(this.m_RequestOperation, new Action<AsyncOperation>(this.LocalRequestOperationCompleted));
		}

		internal WebRequestQueueOperation EnqueueWebRequest(string internalId)
		{
			UnityWebRequest unityWebRequest = this.CreateWebRequest(internalId);
			((DownloadHandlerAssetBundle)unityWebRequest.downloadHandler).autoLoadAssetBundle = !(this.m_ProvideHandle.Location is DownloadOnlyLocation);
			unityWebRequest.disposeDownloadHandlerOnDispose = false;
			return WebRequestQueue.QueueRequest(unityWebRequest);
		}

		internal void AddBeginWebRequestHandler(WebRequestQueueOperation webRequestQueueOperation)
		{
			if (webRequestQueueOperation.IsDone)
			{
				this.BeginWebRequestOperation(webRequestQueueOperation.Result);
				return;
			}
			webRequestQueueOperation.OnComplete = (Action<UnityWebRequestAsyncOperation>)Delegate.Combine(webRequestQueueOperation.OnComplete, new Action<UnityWebRequestAsyncOperation>(delegate(UnityWebRequestAsyncOperation asyncOp)
			{
				this.BeginWebRequestOperation(asyncOp);
			}));
		}

		private void BeginWebRequestOperation(AsyncOperation asyncOp)
		{
			this.m_TimeoutTimer = 0f;
			this.m_TimeoutOverFrames = 0;
			this.m_LastDownloadedByteCount = 0UL;
			this.m_RequestOperation = asyncOp;
			if (this.m_RequestOperation == null || this.m_RequestOperation.isDone)
			{
				this.WebRequestOperationCompleted(this.m_RequestOperation);
				return;
			}
			if (this.m_Options != null && this.m_Options.Timeout > 0)
			{
				this.m_ProvideHandle.ResourceManager.AddUpdateReceiver(this);
			}
			this.m_RequestOperation.completed += this.WebRequestOperationCompleted;
		}

		public void Update(float unscaledDeltaTime)
		{
			if (this.m_RequestOperation != null)
			{
				UnityWebRequestAsyncOperation unityWebRequestAsyncOperation = this.m_RequestOperation as UnityWebRequestAsyncOperation;
				if (unityWebRequestAsyncOperation != null && !unityWebRequestAsyncOperation.isDone)
				{
					if (this.m_LastDownloadedByteCount != unityWebRequestAsyncOperation.webRequest.downloadedBytes)
					{
						this.m_TimeoutTimer = 0f;
						this.m_TimeoutOverFrames = 0;
						this.m_LastDownloadedByteCount = unityWebRequestAsyncOperation.webRequest.downloadedBytes;
						this.m_LastFrameCount = -1;
						this.m_TimeSecSinceLastUpdate = 0f;
						return;
					}
					float num = unscaledDeltaTime;
					if (this.m_LastFrameCount == Time.frameCount)
					{
						num = Time.realtimeSinceStartup - this.m_TimeSecSinceLastUpdate;
					}
					this.m_TimeoutTimer += num;
					if (this.HasTimedOut)
					{
						unityWebRequestAsyncOperation.webRequest.Abort();
					}
					this.m_TimeoutOverFrames++;
					this.m_LastFrameCount = Time.frameCount;
					this.m_TimeSecSinceLastUpdate = Time.realtimeSinceStartup;
				}
			}
		}

		private void LocalRequestOperationCompleted(AsyncOperation op)
		{
			if (this.m_RequestCompletedCallbackCalled)
			{
				return;
			}
			this.m_RequestCompletedCallbackCalled = true;
			UnityWebRequestUtilities.LogOperationResult(op);
			this.CompleteBundleLoad((op as AssetBundleCreateRequest).assetBundle);
		}

		private void CompleteBundleLoad(AssetBundle bundle)
		{
			this.m_AssetBundle = bundle;
			if (this.m_AssetBundle != null)
			{
				this.m_ProvideHandle.Complete<AssetBundleResource>(this, true, null);
			}
			else
			{
				this.m_ProvideHandle.Complete<AssetBundleResource>(null, false, new RemoteProviderException(string.Format("Invalid path in AssetBundleProvider: '{0}'.", this.m_TransformedInternalId), this.m_ProvideHandle.Location, null, null));
			}
			this.m_Completed = true;
		}

		private void WebRequestOperationCompleted(AsyncOperation op)
		{
			if (this.m_RequestCompletedCallbackCalled)
			{
				return;
			}
			this.m_RequestCompletedCallbackCalled = true;
			if (this.m_Options != null && this.m_Options.Timeout > 0)
			{
				this.m_ProvideHandle.ResourceManager.RemoveUpdateReciever(this);
			}
			UnityWebRequestAsyncOperation unityWebRequestAsyncOperation = op as UnityWebRequestAsyncOperation;
			UnityWebRequest unityWebRequest = (unityWebRequestAsyncOperation != null) ? unityWebRequestAsyncOperation.webRequest : null;
			DownloadHandlerAssetBundle downloadHandlerAssetBundle = ((unityWebRequest != null) ? unityWebRequest.downloadHandler : null) as DownloadHandlerAssetBundle;
			UnityWebRequestResult unityWebRequestResult = null;
			if (unityWebRequest != null && !UnityWebRequestUtilities.RequestHasErrors(unityWebRequest, out unityWebRequestResult))
			{
				if (!this.m_Completed)
				{
					if (!(this.m_ProvideHandle.Location is DownloadOnlyLocation))
					{
						this.m_AssetBundle = downloadHandlerAssetBundle.assetBundle;
					}
					downloadHandlerAssetBundle.Dispose();
					this.m_ProvideHandle.Complete<AssetBundleResource>(this, true, null);
					this.m_Completed = true;
				}
				if (this.m_Options != null && !string.IsNullOrEmpty(this.m_Options.Hash) && this.m_Options.ClearOtherCachedVersionsWhenLoaded)
				{
					Caching.ClearOtherCachedVersions(this.m_Options.BundleName, Hash128.Parse(this.m_Options.Hash));
				}
			}
			else
			{
				if (this.HasTimedOut)
				{
					unityWebRequestResult.Error = "Request timeout";
				}
				unityWebRequest = this.m_WebRequestQueueOperation.m_WebRequest;
				if (unityWebRequestResult == null)
				{
					unityWebRequestResult = new UnityWebRequestResult(this.m_WebRequestQueueOperation.m_WebRequest);
				}
				downloadHandlerAssetBundle = (unityWebRequest.downloadHandler as DownloadHandlerAssetBundle);
				downloadHandlerAssetBundle.Dispose();
				if (this.m_Options != null)
				{
					bool flag = false;
					string text = string.Format("Web request failed, retrying ({0}/{1})...\n{2}", this.m_Retries, this.m_Options.RetryCount, unityWebRequestResult);
					bool flag2 = this.m_RequestRetryCallback(unityWebRequestResult);
					if (!string.IsNullOrEmpty(this.m_Options.Hash) && this.m_Source == BundleSource.Cache)
					{
						text = string.Format("Web request failed to load from cache. The cached AssetBundle will be cleared from the cache and re-downloaded. Retrying...\n{0}", unityWebRequestResult);
						Caching.ClearCachedVersion(this.m_Options.BundleName, Hash128.Parse(this.m_Options.Hash));
						if (this.m_Retries == 0 && flag2)
						{
							Debug.LogFormat(text, Array.Empty<object>());
							this.BeginOperation();
							this.m_Retries++;
							flag = true;
						}
					}
					if (!flag)
					{
						if (this.m_Retries < this.m_Options.RetryCount && flag2)
						{
							this.m_Retries++;
							Debug.LogFormat(text, Array.Empty<object>());
							this.BeginOperation();
						}
						else
						{
							text = "Unable to load asset bundle from : " + unityWebRequest.url;
							if (!flag2 && this.m_Options.RetryCount > 0)
							{
								text += string.Format("\nRetry count set to {0} but cannot retry request due to error {1}. To override use a custom AssetBundle provider.", this.m_Options.RetryCount, unityWebRequestResult.Error);
							}
							RemoteProviderException exception = new RemoteProviderException(text, this.m_ProvideHandle.Location, unityWebRequestResult, null);
							this.m_ProvideHandle.Complete<AssetBundleResource>(null, false, exception);
							this.m_Completed = true;
						}
					}
				}
			}
			unityWebRequest.Dispose();
		}

		public bool Unload(out AssetBundleUnloadOperation unloadOp)
		{
			unloadOp = null;
			if (this.m_AssetBundle != null)
			{
				unloadOp = this.m_AssetBundle.UnloadAsync(true);
				this.m_AssetBundle = null;
			}
			this.m_RequestOperation = null;
			return unloadOp != null;
		}

		private AssetBundle m_AssetBundle;

		private AsyncOperation m_RequestOperation;

		internal WebRequestQueueOperation m_WebRequestQueueOperation;

		internal ProvideHandle m_ProvideHandle;

		internal AssetBundleRequestOptions m_Options;

		internal AssetBundleResource.CacheStatus cacheStatus;

		[NonSerialized]
		private bool m_RequestCompletedCallbackCalled;

		private int m_Retries;

		private BundleSource m_Source;

		private long m_BytesToDownload;

		private long m_DownloadedBytes;

		private bool m_Completed;

		private AssetBundleUnloadOperation m_UnloadOperation;

		private const int k_WaitForWebRequestMainThreadSleep = 1;

		private string m_TransformedInternalId;

		private AssetBundleRequest m_PreloadRequest;

		private bool m_PreloadCompleted;

		private ulong m_LastDownloadedByteCount;

		private float m_TimeoutTimer;

		private int m_TimeoutOverFrames;

		internal bool m_DownloadOnly;

		private int m_LastFrameCount = -1;

		private float m_TimeSecSinceLastUpdate;

		internal Func<UnityWebRequestResult, bool> m_RequestRetryCallback = (UnityWebRequestResult x) => x.ShouldRetryDownloadError();

		public enum LoadType
		{
			None,
			Local,
			Web
		}

		internal enum CacheStatus
		{
			Unknown,
			Cached,
			NotCached
		}
	}
}
