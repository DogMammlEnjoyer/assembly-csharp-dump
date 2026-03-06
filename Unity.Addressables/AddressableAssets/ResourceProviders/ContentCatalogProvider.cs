using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.Util;

namespace UnityEngine.AddressableAssets.ResourceProviders
{
	[DisplayName("Content Catalog Provider")]
	public class ContentCatalogProvider : ResourceProviderBase
	{
		public ContentCatalogProvider(ResourceManager resourceManagerInstance)
		{
			this.m_BehaviourFlags = ProviderBehaviourFlags.CanProvideWithFailedDependencies;
		}

		public override void Release(IResourceLocation location, object obj)
		{
			if (this.m_LocationToCatalogLoadOpMap.ContainsKey(location))
			{
				this.m_LocationToCatalogLoadOpMap[location].Release();
				this.m_LocationToCatalogLoadOpMap.Remove(location);
			}
			base.Release(location, obj);
		}

		public override void Provide(ProvideHandle providerInterface)
		{
			if (!this.m_LocationToCatalogLoadOpMap.ContainsKey(providerInterface.Location))
			{
				this.m_LocationToCatalogLoadOpMap.Add(providerInterface.Location, new ContentCatalogProvider.InternalOp());
			}
			this.m_LocationToCatalogLoadOpMap[providerInterface.Location].Start(providerInterface, this.DisableCatalogUpdateOnStart, this.IsLocalCatalogInBundle);
		}

		public bool DisableCatalogUpdateOnStart;

		public bool IsLocalCatalogInBundle;

		internal Dictionary<IResourceLocation, ContentCatalogProvider.InternalOp> m_LocationToCatalogLoadOpMap = new Dictionary<IResourceLocation, ContentCatalogProvider.InternalOp>();

		public enum DependencyHashIndex
		{
			Remote,
			Cache,
			Local,
			Count
		}

		internal class InternalOp
		{
			public void Start(ProvideHandle providerInterface, bool disableCatalogUpdateOnStart, bool isLocalCatalogInBundle)
			{
				this.m_ProviderInterface = providerInterface;
				this.m_DisableCatalogUpdateOnStart = disableCatalogUpdateOnStart;
				this.m_IsLocalCatalogInBundle = isLocalCatalogInBundle;
				this.m_ProviderInterface.SetWaitForCompletionCallback(new Func<bool>(this.WaitForCompletionCallback));
				this.m_LocalDataPath = null;
				this.m_RemoteHashValue = null;
				List<object> list = new List<object>();
				this.m_ProviderInterface.GetDependencies(list);
				string idToLoad = this.DetermineIdToLoad(this.m_ProviderInterface.Location, list, disableCatalogUpdateOnStart);
				bool loadCatalogFromLocalBundle = isLocalCatalogInBundle && this.CanLoadCatalogFromBundle(idToLoad, this.m_ProviderInterface.Location);
				this.LoadCatalog(idToLoad, loadCatalogFromLocalBundle);
			}

			private bool WaitForCompletionCallback()
			{
				if (this.m_ContentCatalogData != null)
				{
					return true;
				}
				bool flag;
				if (this.m_BundledCatalog != null)
				{
					flag = this.m_BundledCatalog.WaitForCompletion();
				}
				else
				{
					flag = this.m_ContentCatalogDataLoadOp.IsDone;
					if (!flag)
					{
						this.m_ContentCatalogDataLoadOp.WaitForCompletion();
					}
				}
				if (flag && this.m_ContentCatalogData == null)
				{
					this.m_ProviderInterface.ResourceManager.Update(Time.unscaledDeltaTime);
				}
				return flag;
			}

			public void Release()
			{
				ContentCatalogData contentCatalogData = this.m_ContentCatalogData;
				if (contentCatalogData == null)
				{
					return;
				}
				contentCatalogData.CleanData();
			}

			internal bool CanLoadCatalogFromBundle(string idToLoad, IResourceLocation location)
			{
				return Path.GetExtension(idToLoad) == ".bundle" && idToLoad.Equals(this.GetTransformedInternalId(location));
			}

			internal void LoadCatalog(string idToLoad, bool loadCatalogFromLocalBundle)
			{
				try
				{
					ProviderLoadRequestOptions providerLoadRequestOptions = null;
					ProviderLoadRequestOptions providerLoadRequestOptions2 = this.m_ProviderInterface.Location.Data as ProviderLoadRequestOptions;
					if (providerLoadRequestOptions2 != null)
					{
						providerLoadRequestOptions = providerLoadRequestOptions2.Copy();
					}
					if (loadCatalogFromLocalBundle)
					{
						int webRequestTimeout = (providerLoadRequestOptions != null) ? providerLoadRequestOptions.WebRequestTimeout : 0;
						this.m_BundledCatalog = new ContentCatalogProvider.InternalOp.BundledCatalog(idToLoad, webRequestTimeout);
						this.m_BundledCatalog.OnLoaded += delegate(ContentCatalogData ccd)
						{
							this.m_ContentCatalogData = ccd;
							this.OnCatalogLoaded(ccd);
						};
						this.m_BundledCatalog.LoadCatalogFromBundleAsync();
					}
					else if (Path.GetExtension(idToLoad) == ".json")
					{
						this.m_ProviderInterface.Complete<ContentCatalogData>(null, false, new Exception("Expecting to load catalogs in binary format but the catalog provided is in .json format. To load it enable Addressable Asset Settings > Catalog > Enable Json Catalog."));
					}
					else
					{
						ResourceLocationBase resourceLocationBase = new ResourceLocationBase(idToLoad, idToLoad, typeof(BinaryAssetProvider<ContentCatalogData.Serializer>).FullName, typeof(ContentCatalogData), Array.Empty<IResourceLocation>());
						resourceLocationBase.Data = providerLoadRequestOptions;
						this.m_ProviderInterface.ResourceManager.ResourceProviders.Add(new BinaryAssetProvider<ContentCatalogData.Serializer>());
						this.m_ContentCatalogDataLoadOp = this.m_ProviderInterface.ResourceManager.ProvideResource<ContentCatalogData>(resourceLocationBase);
						this.m_ContentCatalogDataLoadOp.Completed += this.CatalogLoadOpCompleteCallback;
					}
				}
				catch (Exception exception)
				{
					this.m_ProviderInterface.Complete<ContentCatalogData>(null, false, exception);
				}
			}

			private void CatalogLoadOpCompleteCallback(AsyncOperationHandle<ContentCatalogData> op)
			{
				this.m_ContentCatalogData = op.Result;
				op.Release();
				this.OnCatalogLoaded(this.m_ContentCatalogData);
			}

			private string GetTransformedInternalId(IResourceLocation loc)
			{
				if (this.m_ProviderInterface.ResourceManager == null)
				{
					return loc.InternalId;
				}
				return this.m_ProviderInterface.ResourceManager.TransformInternalId(loc);
			}

			internal string DetermineIdToLoad(IResourceLocation location, IList<object> dependencyObjects, bool disableCatalogUpdateOnStart = false)
			{
				string result = this.GetTransformedInternalId(location);
				if (dependencyObjects != null && location.Dependencies != null && dependencyObjects.Count == 3 && location.Dependencies.Count == 3)
				{
					string text = dependencyObjects[0] as string;
					this.m_LocalHashValue = (dependencyObjects[1] as string);
					if (string.IsNullOrEmpty(this.m_LocalHashValue))
					{
						this.m_LocalHashValue = (dependencyObjects[2] as string);
					}
					if (string.IsNullOrEmpty(text) || disableCatalogUpdateOnStart)
					{
						if (!string.IsNullOrEmpty(this.m_LocalHashValue) && !this.m_Retried && !string.IsNullOrEmpty(Application.persistentDataPath))
						{
							if (string.IsNullOrEmpty(dependencyObjects[1] as string))
							{
								result = this.GetTransformedInternalId(location.Dependencies[2]).Replace(".hash", ".bin");
							}
							else
							{
								result = this.GetTransformedInternalId(location.Dependencies[1]).Replace(".hash", ".bin");
							}
						}
					}
					else if (text == this.m_LocalHashValue && !this.m_Retried)
					{
						if (string.IsNullOrEmpty(dependencyObjects[1] as string))
						{
							result = this.GetTransformedInternalId(location.Dependencies[2]).Replace(".hash", ".bin");
						}
						else
						{
							result = this.GetTransformedInternalId(location.Dependencies[1]).Replace(".hash", ".bin");
						}
					}
					else
					{
						result = this.GetTransformedInternalId(location.Dependencies[0]).Replace(".hash", ".bin");
						this.m_RemoteHashValue = text;
						if (!string.IsNullOrEmpty(Application.persistentDataPath))
						{
							this.m_LocalDataPath = this.GetTransformedInternalId(location.Dependencies[1]).Replace(".hash", ".bin");
						}
					}
				}
				return result;
			}

			private void OnCatalogLoaded(ContentCatalogData ccd)
			{
				if (ccd != null)
				{
					ccd.location = this.m_ProviderInterface.Location;
					ccd.LocalHash = this.m_LocalHashValue;
					if (!string.IsNullOrEmpty(this.m_RemoteHashValue) && !string.IsNullOrEmpty(this.m_LocalDataPath))
					{
						string directoryName = Path.GetDirectoryName(this.m_LocalDataPath);
						string localDataPath = this.m_LocalDataPath;
						try
						{
							if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
							{
								Directory.CreateDirectory(directoryName);
							}
							File.WriteAllBytes(localDataPath, ccd.GetBytes());
							File.WriteAllText(localDataPath.Replace(".bin", ".hash"), this.m_RemoteHashValue);
						}
						catch (UnauthorizedAccessException ex)
						{
							Addressables.LogWarning("Did not save cached content catalog. Missing access permissions for location " + localDataPath + " : " + ex.Message);
							this.m_ProviderInterface.Complete<ContentCatalogData>(ccd, true, null);
							return;
						}
						catch (Exception innerException)
						{
							string transformedInternalId = this.GetTransformedInternalId(this.m_ProviderInterface.Location.Dependencies[0]);
							string message = string.Concat(new string[]
							{
								"Unable to load ContentCatalogData from location ",
								transformedInternalId,
								". Failed to cache catalog to location ",
								localDataPath,
								"."
							});
							ccd = null;
							this.m_ProviderInterface.Complete<ContentCatalogData>(ccd, false, new Exception(message, innerException));
							return;
						}
						ccd.LocalHash = this.m_RemoteHashValue;
					}
					else if (string.IsNullOrEmpty(this.m_LocalDataPath) && string.IsNullOrEmpty(Application.persistentDataPath))
					{
						Addressables.LogWarning("Did not save cached content catalog because Application.persistentDataPath is an empty path.");
					}
					this.m_ProviderInterface.Complete<ContentCatalogData>(ccd, true, null);
					return;
				}
				string text = string.Format("Unable to load ContentCatalogData from location {0}", this.m_ProviderInterface.Location);
				if (!this.m_Retried)
				{
					this.m_Retried = true;
					string transformedInternalId2 = this.GetTransformedInternalId(this.m_ProviderInterface.Location.Dependencies[1]);
					if (this.m_ContentCatalogDataLoadOp.LocationName == transformedInternalId2.Replace(".hash", ".bin"))
					{
						try
						{
							File.Delete(transformedInternalId2);
						}
						catch (Exception)
						{
							text = text + ". Unable to delete cache data from location " + transformedInternalId2;
							this.m_ProviderInterface.Complete<ContentCatalogData>(ccd, false, new Exception(text));
							return;
						}
					}
					Addressables.LogWarning(text + ". Attempting to retry...");
					this.Start(this.m_ProviderInterface, this.m_DisableCatalogUpdateOnStart, this.m_IsLocalCatalogInBundle);
					return;
				}
				this.m_ProviderInterface.Complete<ContentCatalogData>(ccd, false, new Exception(text + " on second attempt."));
			}

			private string m_LocalDataPath;

			private string m_RemoteHashValue;

			internal string m_LocalHashValue;

			private ProvideHandle m_ProviderInterface;

			internal ContentCatalogData m_ContentCatalogData;

			private AsyncOperationHandle<ContentCatalogData> m_ContentCatalogDataLoadOp;

			private ContentCatalogProvider.InternalOp.BundledCatalog m_BundledCatalog;

			private bool m_Retried;

			private bool m_DisableCatalogUpdateOnStart;

			private bool m_IsLocalCatalogInBundle;

			private const string kCatalogExt = ".bin";

			internal class BundledCatalog
			{
				public event Action<ContentCatalogData> OnLoaded;

				public bool OpInProgress
				{
					get
					{
						return this.m_OpInProgress;
					}
				}

				public bool OpIsSuccess
				{
					get
					{
						return !this.m_OpInProgress && this.m_CatalogData != null;
					}
				}

				public BundledCatalog(string bundlePath, int webRequestTimeout = 0)
				{
					if (string.IsNullOrEmpty(bundlePath))
					{
						throw new ArgumentNullException("bundlePath", "Catalog bundle path is null.");
					}
					if (!bundlePath.EndsWith(".bundle", StringComparison.OrdinalIgnoreCase))
					{
						throw new ArgumentException("You must supply a valid bundle file path.");
					}
					this.m_BundlePath = bundlePath;
					this.m_WebRequestTimeout = webRequestTimeout;
				}

				~BundledCatalog()
				{
					this.Unload();
				}

				private void Unload()
				{
					AssetBundle catalogAssetBundle = this.m_CatalogAssetBundle;
					if (catalogAssetBundle != null)
					{
						catalogAssetBundle.Unload(true);
					}
					this.m_CatalogAssetBundle = null;
				}

				public void LoadCatalogFromBundleAsync()
				{
					if (this.m_OpInProgress)
					{
						Addressables.LogError("Operation in progress : A catalog is already being loaded. Please wait for the operation to complete.");
						return;
					}
					this.m_OpInProgress = true;
					if (!ResourceManagerConfig.ShouldPathUseWebRequest(this.m_BundlePath))
					{
						this.m_LoadBundleRequest = AssetBundle.LoadFromFileAsync(this.m_BundlePath);
						this.m_LoadBundleRequest.completed += delegate(AsyncOperation loadOp)
						{
							AssetBundleCreateRequest assetBundleCreateRequest = loadOp as AssetBundleCreateRequest;
							if (assetBundleCreateRequest != null && assetBundleCreateRequest.assetBundle != null)
							{
								this.m_CatalogAssetBundle = assetBundleCreateRequest.assetBundle;
								this.m_LoadTextAssetRequest = this.m_CatalogAssetBundle.LoadAllAssetsAsync<TextAsset>();
								if (this.m_LoadTextAssetRequest.isDone)
								{
									this.LoadTextAssetRequestComplete(this.m_LoadTextAssetRequest);
								}
								this.m_LoadTextAssetRequest.completed += this.LoadTextAssetRequestComplete;
								return;
							}
							Addressables.LogError("Unable to load dependent bundle from file location : " + this.m_BundlePath);
							this.m_OpInProgress = false;
						};
						return;
					}
					UnityWebRequest assetBundle = UnityWebRequestAssetBundle.GetAssetBundle(this.m_BundlePath);
					if (this.m_WebRequestTimeout > 0)
					{
						assetBundle.timeout = this.m_WebRequestTimeout;
					}
					this.m_WebRequestQueueOperation = WebRequestQueue.QueueRequest(assetBundle);
					if (!this.m_WebRequestQueueOperation.IsDone)
					{
						WebRequestQueueOperation webRequestQueueOperation = this.m_WebRequestQueueOperation;
						webRequestQueueOperation.OnComplete = (Action<UnityWebRequestAsyncOperation>)Delegate.Combine(webRequestQueueOperation.OnComplete, new Action<UnityWebRequestAsyncOperation>(delegate(UnityWebRequestAsyncOperation asyncOp)
						{
							this.m_RequestOperation = asyncOp;
							this.m_RequestOperation.completed += this.WebRequestOperationCompleted;
						}));
						return;
					}
					this.m_RequestOperation = this.m_WebRequestQueueOperation.Result;
					if (this.m_RequestOperation.isDone)
					{
						this.WebRequestOperationCompleted(this.m_RequestOperation);
						return;
					}
					this.m_RequestOperation.completed += this.WebRequestOperationCompleted;
				}

				private void WebRequestOperationCompleted(AsyncOperation op)
				{
					UnityWebRequestUtilities.LogOperationResult(op);
					UnityWebRequest webRequest = (op as UnityWebRequestAsyncOperation).webRequest;
					DownloadHandlerAssetBundle downloadHandlerAssetBundle = webRequest.downloadHandler as DownloadHandlerAssetBundle;
					UnityWebRequestResult unityWebRequestResult;
					if (!UnityWebRequestUtilities.RequestHasErrors(webRequest, out unityWebRequestResult))
					{
						this.m_CatalogAssetBundle = downloadHandlerAssetBundle.assetBundle;
						this.m_LoadTextAssetRequest = this.m_CatalogAssetBundle.LoadAllAssetsAsync<TextAsset>();
						if (this.m_LoadTextAssetRequest.isDone)
						{
							this.LoadTextAssetRequestComplete(this.m_LoadTextAssetRequest);
						}
						this.m_LoadTextAssetRequest.completed += this.LoadTextAssetRequestComplete;
					}
					else
					{
						Addressables.LogError("Unable to load dependent bundle from remote location : " + this.m_BundlePath);
						this.m_OpInProgress = false;
					}
					webRequest.Dispose();
				}

				private void LoadTextAssetRequestComplete(AsyncOperation op)
				{
					AssetBundleRequest assetBundleRequest = op as AssetBundleRequest;
					if (assetBundleRequest != null)
					{
						TextAsset textAsset = assetBundleRequest.asset as TextAsset;
						if (textAsset != null && textAsset.text != null)
						{
							this.m_CatalogData = JsonUtility.FromJson<ContentCatalogData>(textAsset.text);
							Action<ContentCatalogData> onLoaded = this.OnLoaded;
							if (onLoaded == null)
							{
								goto IL_60;
							}
							onLoaded(this.m_CatalogData);
							goto IL_60;
						}
					}
					Addressables.LogError("No catalog text assets where found in bundle " + this.m_BundlePath);
					IL_60:
					this.Unload();
					this.m_OpInProgress = false;
				}

				public bool WaitForCompletion()
				{
					return !(this.m_LoadBundleRequest.assetBundle == null) && (this.m_LoadTextAssetRequest.asset != null || this.m_LoadTextAssetRequest.allAssets != null);
				}

				private readonly string m_BundlePath;

				private bool m_OpInProgress;

				private AssetBundleCreateRequest m_LoadBundleRequest;

				internal AssetBundle m_CatalogAssetBundle;

				private AssetBundleRequest m_LoadTextAssetRequest;

				private ContentCatalogData m_CatalogData;

				private WebRequestQueueOperation m_WebRequestQueueOperation;

				private AsyncOperation m_RequestOperation;

				private int m_WebRequestTimeout;
			}
		}
	}
}
