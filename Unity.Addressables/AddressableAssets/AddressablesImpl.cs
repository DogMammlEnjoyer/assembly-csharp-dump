using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine.AddressableAssets.Initialization;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.AddressableAssets.ResourceProviders;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.Util;
using UnityEngine.SceneManagement;

namespace UnityEngine.AddressableAssets
{
	internal class AddressablesImpl : IEqualityComparer<IResourceLocation>
	{
		public IInstanceProvider InstanceProvider
		{
			get
			{
				return this.m_InstanceProvider;
			}
			set
			{
				this.m_InstanceProvider = value;
				IUpdateReceiver updateReceiver = this.m_InstanceProvider as IUpdateReceiver;
				if (updateReceiver != null)
				{
					this.m_ResourceManager.AddUpdateReceiver(updateReceiver);
				}
			}
		}

		public ResourceManager ResourceManager
		{
			get
			{
				return this.m_ResourceManager;
			}
		}

		public int CatalogRequestsTimeout
		{
			get
			{
				return this.m_CatalogRequestsTimeout;
			}
			set
			{
				this.m_CatalogRequestsTimeout = value;
			}
		}

		internal int ActiveSceneInstances
		{
			get
			{
				return this.m_SceneInstances.Count;
			}
		}

		internal int TrackedHandleCount
		{
			get
			{
				return this.m_resultToHandle.Count;
			}
		}

		public AddressablesImpl(IAllocationStrategy alloc)
		{
			this.m_ResourceManager = new ResourceManager(alloc);
			SceneManager.sceneUnloaded += this.OnSceneUnloaded;
		}

		internal void ReleaseSceneManagerOperation()
		{
			SceneManager.sceneUnloaded -= this.OnSceneUnloaded;
		}

		public Func<IResourceLocation, string> InternalIdTransformFunc
		{
			get
			{
				return this.ResourceManager.InternalIdTransformFunc;
			}
			set
			{
				this.ResourceManager.InternalIdTransformFunc = value;
			}
		}

		public Action<UnityWebRequest> WebRequestOverride
		{
			get
			{
				return this.ResourceManager.WebRequestOverride;
			}
			set
			{
				this.ResourceManager.WebRequestOverride = value;
			}
		}

		public AsyncOperationHandle ChainOperation
		{
			get
			{
				if (!this.hasStartedInitialization)
				{
					return this.InitializeAsync();
				}
				if (this.m_InitializationOperation.IsValid() && !this.m_InitializationOperation.IsDone)
				{
					return this.m_InitializationOperation;
				}
				if (this.m_ActiveUpdateOperation.IsValid() && !this.m_ActiveUpdateOperation.IsDone)
				{
					return this.m_ActiveUpdateOperation;
				}
				Debug.LogWarning("ChainOperation property should not be accessed unless ShouldChainRequest is true.");
				return default(AsyncOperationHandle);
			}
		}

		internal bool ShouldChainRequest
		{
			get
			{
				return !this.hasStartedInitialization || (this.m_InitializationOperation.IsValid() && !this.m_InitializationOperation.IsDone) || (this.m_ActiveUpdateOperation.IsValid() && !this.m_ActiveUpdateOperation.IsDone);
			}
		}

		internal void OnSceneUnloaded(Scene scene)
		{
			foreach (AsyncOperationHandle item in this.m_SceneInstances)
			{
				if (!item.IsValid())
				{
					this.m_SceneInstances.Remove(item);
					break;
				}
				AsyncOperationHandle<SceneInstance> sceneLoadHandle = item.Convert<SceneInstance>();
				if (sceneLoadHandle.Result.Scene == scene)
				{
					this.m_SceneInstances.Remove(item);
					this.m_resultToHandle.Remove(item.Result);
					if (sceneLoadHandle.Result.ReleaseSceneOnSceneUnloaded)
					{
						this.SceneProvider.ReleaseScene(this.m_ResourceManager, sceneLoadHandle).ReleaseHandleOnCompletion();
						break;
					}
					break;
				}
			}
			this.m_ResourceManager.CleanupSceneInstances(scene);
		}

		public string StreamingAssetsSubFolder
		{
			get
			{
				return "aa";
			}
		}

		public string BuildPath
		{
			get
			{
				return Addressables.LibraryPath + this.StreamingAssetsSubFolder + "/" + PlatformMappingService.GetPlatformPathSubFolder();
			}
		}

		public string PlayerBuildDataPath
		{
			get
			{
				return Application.streamingAssetsPath + "/" + this.StreamingAssetsSubFolder;
			}
		}

		public string RuntimePath
		{
			get
			{
				return this.PlayerBuildDataPath;
			}
		}

		public void Log(string msg)
		{
			Debug.Log(msg);
		}

		public void LogFormat(string format, params object[] args)
		{
			Debug.LogFormat(format, args);
		}

		public void LogWarning(string msg)
		{
			Debug.LogWarning(msg);
		}

		public void LogWarningFormat(string format, params object[] args)
		{
			Debug.LogWarningFormat(format, args);
		}

		public void LogError(string msg)
		{
			Debug.LogError(msg);
		}

		public void LogException(AsyncOperationHandle op, Exception ex)
		{
			if (op.Status == AsyncOperationStatus.Failed)
			{
				Debug.LogError(ex.ToString());
			}
		}

		public void LogException(Exception ex)
		{
		}

		public void LogErrorFormat(string format, params object[] args)
		{
			Debug.LogErrorFormat(format, args);
		}

		public string ResolveInternalId(string id)
		{
			string text = AddressablesRuntimeProperties.EvaluateString(id);
			if (text.Length >= 260 && text.StartsWith(Application.dataPath, StringComparison.Ordinal))
			{
				text = text.Substring(Application.dataPath.Length + 1);
			}
			return text;
		}

		public IEnumerable<IResourceLocator> ResourceLocators
		{
			get
			{
				return from l in this.m_ResourceLocators
				select l.Locator;
			}
		}

		public void AddResourceLocator(IResourceLocator loc, string localCatalogHash = null, IResourceLocation remoteCatalogLocation = null)
		{
			this.m_ResourceLocators.Add(new ResourceLocatorInfo(loc, localCatalogHash, remoteCatalogLocation));
		}

		public void RemoveResourceLocator(IResourceLocator loc)
		{
			this.m_ResourceLocators.RemoveAll((ResourceLocatorInfo l) => l.Locator == loc);
		}

		public void ClearResourceLocators()
		{
			this.m_ResourceLocators.Clear();
		}

		internal bool GetResourceLocations(object key, Type type, out IList<IResourceLocation> locations)
		{
			if (type == null && key is AssetReference)
			{
				type = (key as AssetReference).SubObjectType;
			}
			key = this.EvaluateKey(key);
			locations = null;
			HashSet<IResourceLocation> hashSet = null;
			using (List<ResourceLocatorInfo>.Enumerator enumerator = this.m_ResourceLocators.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					IList<IResourceLocation> list;
					if (enumerator.Current.Locator.Locate(key, type, out list))
					{
						if (locations == null)
						{
							locations = list;
						}
						else
						{
							if (hashSet == null)
							{
								hashSet = new HashSet<IResourceLocation>();
								foreach (IResourceLocation item in locations)
								{
									hashSet.Add(item);
								}
							}
							hashSet.UnionWith(list);
						}
					}
				}
			}
			if (hashSet == null)
			{
				return locations != null;
			}
			locations = new List<IResourceLocation>(hashSet);
			return true;
		}

		internal bool GetResourceLocations(IEnumerable keys, Type type, Addressables.MergeMode merge, out IList<IResourceLocation> locations)
		{
			locations = null;
			HashSet<IResourceLocation> hashSet = null;
			foreach (object key in keys)
			{
				IList<IResourceLocation> list;
				if (this.GetResourceLocations(key, type, out list))
				{
					if (locations == null)
					{
						locations = list;
						if (merge == Addressables.MergeMode.None)
						{
							return true;
						}
					}
					else
					{
						if (hashSet == null)
						{
							hashSet = new HashSet<IResourceLocation>(locations, this);
						}
						if (merge == Addressables.MergeMode.Intersection)
						{
							hashSet.IntersectWith(list);
						}
						else if (merge == Addressables.MergeMode.Union)
						{
							hashSet.UnionWith(list);
						}
					}
				}
				else if (merge == Addressables.MergeMode.Intersection)
				{
					locations = null;
					return false;
				}
			}
			if (hashSet == null)
			{
				return locations != null;
			}
			if (hashSet.Count == 0)
			{
				locations = null;
				return false;
			}
			locations = new List<IResourceLocation>(hashSet);
			return true;
		}

		public AsyncOperationHandle<IResourceLocator> InitializeAsync(string runtimeDataPath, string providerSuffix = null, bool autoReleaseHandle = true)
		{
			if (this.hasStartedInitialization)
			{
				if (this.m_InitializationOperation.IsValid())
				{
					return this.m_InitializationOperation;
				}
				AsyncOperationHandle<IResourceLocator> result = this.ResourceManager.CreateCompletedOperation<IResourceLocator>(this.m_ResourceLocators[0].Locator, null);
				if (autoReleaseHandle)
				{
					result.ReleaseHandleOnCompletion();
				}
				return result;
			}
			else
			{
				if (ResourceManager.ExceptionHandler == null)
				{
					ResourceManager.ExceptionHandler = new Action<AsyncOperationHandle, Exception>(this.LogException);
				}
				this.hasStartedInitialization = true;
				if (this.m_InitializationOperation.IsValid())
				{
					return this.m_InitializationOperation;
				}
				GC.KeepAlive(Application.streamingAssetsPath);
				GC.KeepAlive(Application.persistentDataPath);
				if (string.IsNullOrEmpty(runtimeDataPath))
				{
					return this.ResourceManager.CreateCompletedOperation<IResourceLocator>(null, string.Format("Invalid Key: {0}", runtimeDataPath));
				}
				this.m_OnHandleCompleteAction = new Action<AsyncOperationHandle>(this.OnHandleCompleted);
				this.m_OnSceneHandleCompleteAction = new Action<AsyncOperationHandle>(this.OnSceneHandleCompleted);
				this.m_OnHandleDestroyedAction = new Action<AsyncOperationHandle>(this.OnHandleDestroyed);
				if (!this.m_InitializationOperation.IsValid())
				{
					this.m_InitializationOperation = InitializationOperation.CreateInitializationOperation(this, runtimeDataPath, providerSuffix);
				}
				if (autoReleaseHandle)
				{
					this.m_InitializationOperation.ReleaseHandleOnCompletion();
				}
				return this.m_InitializationOperation;
			}
		}

		public AsyncOperationHandle<IResourceLocator> InitializeAsync()
		{
			string id = this.RuntimePath + "/settings.json";
			return this.InitializeAsync(this.ResolveInternalId(id), null, true);
		}

		public AsyncOperationHandle<IResourceLocator> InitializeAsync(bool autoReleaseHandle)
		{
			string id = this.RuntimePath + "/settings.json";
			return this.InitializeAsync(this.ResolveInternalId(id), null, autoReleaseHandle);
		}

		public ResourceLocationBase CreateCatalogLocationWithHashDependencies<T>(IResourceLocation catalogLocation) where T : IResourceProvider
		{
			return this.CreateCatalogLocationWithHashDependencies<T>(catalogLocation.InternalId);
		}

		public ResourceLocationBase CreateCatalogLocationWithHashDependencies<T>(string catalogLocation) where T : IResourceProvider
		{
			string hashFilePath = catalogLocation.Replace(".bin", ".hash");
			return this.CreateCatalogLocationWithHashDependencies<T>(catalogLocation, hashFilePath);
		}

		public ResourceLocationBase CreateCatalogLocationWithHashDependencies<T>(string catalogPath, string hashFilePath) where T : IResourceProvider
		{
			ResourceLocationBase resourceLocationBase = new ResourceLocationBase(catalogPath, catalogPath, typeof(T).FullName, typeof(IResourceLocator), Array.Empty<IResourceLocation>())
			{
				Data = new ProviderLoadRequestOptions
				{
					IgnoreFailures = false,
					WebRequestTimeout = this.CatalogRequestsTimeout
				}
			};
			if (!string.IsNullOrEmpty(hashFilePath))
			{
				ProviderLoadRequestOptions providerLoadRequestOptions = new ProviderLoadRequestOptions
				{
					IgnoreFailures = true,
					WebRequestTimeout = this.CatalogRequestsTimeout
				};
				string text = hashFilePath;
				if (ResourceManagerConfig.IsPathRemote(hashFilePath))
				{
					text = ResourceManagerConfig.StripQueryParameters(hashFilePath);
				}
				ResourceLocationBase item = new ResourceLocationBase(hashFilePath, hashFilePath, typeof(TextDataProvider).FullName, typeof(string), Array.Empty<IResourceLocation>())
				{
					Data = providerLoadRequestOptions.Copy()
				};
				resourceLocationBase.Dependencies.Add(item);
				string text2 = this.ResolveInternalId("{UnityEngine.Application.persistentDataPath}/com.unity.addressables/" + text.GetHashCode().ToString() + ".hash");
				ResourceLocationBase item2 = new ResourceLocationBase(text2, text2, typeof(TextDataProvider).FullName, typeof(string), Array.Empty<IResourceLocation>())
				{
					Data = providerLoadRequestOptions.Copy()
				};
				resourceLocationBase.Dependencies.Add(item2);
				resourceLocationBase.Dependencies.Add(item2);
			}
			return resourceLocationBase;
		}

		[Conditional("UNITY_EDITOR")]
		private void QueueEditorUpdateIfNeeded()
		{
		}

		public AsyncOperationHandle<IResourceLocator> LoadContentCatalogAsync(string catalogPath, bool autoReleaseHandle = true, string providerSuffix = null)
		{
			ResourceLocationBase loc = this.CreateCatalogLocationWithHashDependencies<ContentCatalogProvider>(catalogPath);
			if (this.ShouldChainRequest)
			{
				return this.ResourceManager.CreateChainOperation<IResourceLocator>(this.ChainOperation, (AsyncOperationHandle op) => this.LoadContentCatalogAsync(catalogPath, autoReleaseHandle, providerSuffix));
			}
			AsyncOperationHandle<IResourceLocator> result = InitializationOperation.LoadContentCatalog(this, loc, providerSuffix, null);
			if (autoReleaseHandle)
			{
				result.ReleaseHandleOnCompletion();
			}
			return result;
		}

		private AsyncOperationHandle<SceneInstance> TrackHandle(AsyncOperationHandle<SceneInstance> handle)
		{
			handle.Completed += delegate(AsyncOperationHandle<SceneInstance> sceneHandle)
			{
				this.m_OnSceneHandleCompleteAction(sceneHandle);
			};
			return handle;
		}

		private AsyncOperationHandle<TObject> TrackHandle<TObject>(AsyncOperationHandle<TObject> handle)
		{
			handle.CompletedTypeless += this.m_OnHandleCompleteAction;
			return handle;
		}

		private AsyncOperationHandle TrackHandle(AsyncOperationHandle handle)
		{
			handle.Completed += this.m_OnHandleCompleteAction;
			return handle;
		}

		internal void ClearTrackHandles()
		{
			this.m_resultToHandle.Clear();
		}

		public AsyncOperationHandle<TObject> LoadAssetAsync<TObject>(IResourceLocation location)
		{
			if (this.ShouldChainRequest)
			{
				return this.TrackHandle<TObject>(this.LoadAssetWithChain<TObject>(this.ChainOperation, location));
			}
			return this.TrackHandle<TObject>(this.ResourceManager.ProvideResource<TObject>(location));
		}

		private AsyncOperationHandle<TObject> LoadAssetWithChain<TObject>(AsyncOperationHandle dep, IResourceLocation loc)
		{
			return this.ResourceManager.CreateChainOperation<TObject>(dep, (AsyncOperationHandle op) => this.LoadAssetAsync<TObject>(loc));
		}

		private AsyncOperationHandle<TObject> LoadAssetWithChain<TObject>(AsyncOperationHandle dep, object key)
		{
			return this.ResourceManager.CreateChainOperation<TObject>(dep, (AsyncOperationHandle op) => this.LoadAssetAsync<TObject>(key));
		}

		public AsyncOperationHandle<TObject> LoadAssetAsync<TObject>(object key)
		{
			if (this.ShouldChainRequest)
			{
				return this.TrackHandle<TObject>(this.LoadAssetWithChain<TObject>(this.ChainOperation, key));
			}
			key = this.EvaluateKey(key);
			Type type = typeof(TObject);
			if (type.IsArray)
			{
				type = type.GetElementType();
			}
			else if (type.IsGenericType && typeof(IList<>) == type.GetGenericTypeDefinition())
			{
				type = type.GetGenericArguments()[0];
			}
			using (List<ResourceLocatorInfo>.Enumerator enumerator = this.m_ResourceLocators.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					IList<IResourceLocation> list;
					if (enumerator.Current.Locator.Locate(key, type, out list))
					{
						foreach (IResourceLocation location in list)
						{
							if (this.ResourceManager.GetResourceProvider(typeof(TObject), location) != null)
							{
								return this.TrackHandle<TObject>(this.ResourceManager.ProvideResource<TObject>(location));
							}
						}
					}
				}
			}
			return this.ResourceManager.CreateCompletedOperationWithException<TObject>(default(TObject), new InvalidKeyException(key, type, this));
		}

		public AsyncOperationHandle<IList<IResourceLocation>> LoadResourceLocationsWithChain(AsyncOperationHandle dep, IEnumerable keys, Addressables.MergeMode mode, Type type)
		{
			return this.ResourceManager.CreateChainOperation<IList<IResourceLocation>>(dep, (AsyncOperationHandle op) => this.LoadResourceLocationsAsync(keys, mode, type));
		}

		public AsyncOperationHandle<IList<IResourceLocation>> LoadResourceLocationsAsync(IEnumerable keys, Addressables.MergeMode mode, Type type = null)
		{
			if (this.ShouldChainRequest)
			{
				return this.TrackHandle<IList<IResourceLocation>>(this.LoadResourceLocationsWithChain(this.ChainOperation, keys, mode, type));
			}
			AddressablesImpl.LoadResourceLocationKeysOp loadResourceLocationKeysOp = new AddressablesImpl.LoadResourceLocationKeysOp();
			loadResourceLocationKeysOp.Init(this, type, keys, mode);
			return this.TrackHandle<IList<IResourceLocation>>(this.ResourceManager.StartOperation<IList<IResourceLocation>>(loadResourceLocationKeysOp, default(AsyncOperationHandle)));
		}

		public AsyncOperationHandle<IList<IResourceLocation>> LoadResourceLocationsWithChain(AsyncOperationHandle dep, object key, Type type)
		{
			return this.ResourceManager.CreateChainOperation<IList<IResourceLocation>>(dep, (AsyncOperationHandle op) => this.LoadResourceLocationsAsync(key, type));
		}

		public AsyncOperationHandle<IList<IResourceLocation>> LoadResourceLocationsAsync(object key, Type type = null)
		{
			if (this.ShouldChainRequest)
			{
				return this.TrackHandle<IList<IResourceLocation>>(this.LoadResourceLocationsWithChain(this.ChainOperation, key, type));
			}
			AddressablesImpl.LoadResourceLocationKeyOp loadResourceLocationKeyOp = new AddressablesImpl.LoadResourceLocationKeyOp();
			loadResourceLocationKeyOp.Init(this, type, key);
			return this.TrackHandle<IList<IResourceLocation>>(this.ResourceManager.StartOperation<IList<IResourceLocation>>(loadResourceLocationKeyOp, default(AsyncOperationHandle)));
		}

		public AsyncOperationHandle<IList<TObject>> LoadAssetsAsync<TObject>(IList<IResourceLocation> locations, Action<TObject> callback, bool releaseDependenciesOnFailure)
		{
			if (this.ShouldChainRequest)
			{
				return this.TrackHandle<IList<TObject>>(this.LoadAssetsWithChain<TObject>(this.ChainOperation, locations, callback, releaseDependenciesOnFailure));
			}
			return this.TrackHandle<IList<TObject>>(this.ResourceManager.ProvideResources<TObject>(locations, releaseDependenciesOnFailure, callback));
		}

		private AsyncOperationHandle<IList<TObject>> LoadAssetsWithChain<TObject>(AsyncOperationHandle dep, IList<IResourceLocation> locations, Action<TObject> callback, bool releaseDependenciesOnFailure)
		{
			return this.ResourceManager.CreateChainOperation<IList<TObject>>(dep, (AsyncOperationHandle op) => this.LoadAssetsAsync<TObject>(locations, callback, releaseDependenciesOnFailure));
		}

		private AsyncOperationHandle<IList<TObject>> LoadAssetsWithChain<TObject>(AsyncOperationHandle dep, IEnumerable keys, Action<TObject> callback, Addressables.MergeMode mode, bool releaseDependenciesOnFailure)
		{
			return this.ResourceManager.CreateChainOperation<IList<TObject>>(dep, (AsyncOperationHandle op) => this.LoadAssetsAsync<TObject>(keys, callback, mode, releaseDependenciesOnFailure));
		}

		public AsyncOperationHandle<IList<TObject>> LoadAssetsAsync<TObject>(IEnumerable keys, Action<TObject> callback, Addressables.MergeMode mode, bool releaseDependenciesOnFailure)
		{
			string text = keys as string;
			if (text != null)
			{
				keys = new string[]
				{
					text
				};
			}
			if (this.ShouldChainRequest)
			{
				return this.TrackHandle<IList<TObject>>(this.LoadAssetsWithChain<TObject>(this.ChainOperation, keys, callback, mode, releaseDependenciesOnFailure));
			}
			IList<IResourceLocation> locations;
			if (!this.GetResourceLocations(keys, typeof(TObject), mode, out locations))
			{
				return this.ResourceManager.CreateCompletedOperationWithException<IList<TObject>>(null, new InvalidKeyException(keys, typeof(TObject), mode, this));
			}
			return this.LoadAssetsAsync<TObject>(locations, callback, releaseDependenciesOnFailure);
		}

		private AsyncOperationHandle<IList<TObject>> LoadAssetsWithChain<TObject>(AsyncOperationHandle dep, object key, Action<TObject> callback, bool releaseDependenciesOnFailure)
		{
			return this.ResourceManager.CreateChainOperation<IList<TObject>>(dep, (AsyncOperationHandle op2) => this.LoadAssetsAsync<TObject>(key, callback, releaseDependenciesOnFailure));
		}

		public AsyncOperationHandle<IList<TObject>> LoadAssetsAsync<TObject>(object key, Action<TObject> callback, bool releaseDependenciesOnFailure)
		{
			if (this.ShouldChainRequest)
			{
				return this.TrackHandle<IList<TObject>>(this.LoadAssetsWithChain<TObject>(this.ChainOperation, key, callback, releaseDependenciesOnFailure));
			}
			IList<IResourceLocation> locations;
			if (!this.GetResourceLocations(key, typeof(TObject), out locations))
			{
				return this.ResourceManager.CreateCompletedOperationWithException<IList<TObject>>(null, new InvalidKeyException(key, typeof(TObject), this));
			}
			return this.LoadAssetsAsync<TObject>(locations, callback, releaseDependenciesOnFailure);
		}

		private void OnHandleDestroyed(AsyncOperationHandle handle)
		{
			if (handle.Status == AsyncOperationStatus.Succeeded)
			{
				this.m_resultToHandle.Remove(handle.Result);
			}
		}

		private void OnSceneHandleCompleted(AsyncOperationHandle handle)
		{
			if (handle.Status == AsyncOperationStatus.Succeeded)
			{
				this.m_SceneInstances.Add(handle);
				if (this.m_resultToHandle.TryAdd(handle.Result, handle))
				{
					handle.Destroyed += this.m_OnHandleDestroyedAction;
				}
			}
		}

		private void OnHandleCompleted(AsyncOperationHandle handle)
		{
			if (handle.Status == AsyncOperationStatus.Succeeded && this.m_resultToHandle.TryAdd(handle.Result, handle))
			{
				handle.Destroyed += this.m_OnHandleDestroyedAction;
			}
		}

		public void Release<TObject>(TObject obj)
		{
			if (obj == null)
			{
				this.LogWarning("Addressables.Release() - trying to release null object.");
				return;
			}
			AsyncOperationHandle asyncOperationHandle;
			if (this.m_resultToHandle.TryGetValue(obj, out asyncOperationHandle))
			{
				asyncOperationHandle.Release();
				return;
			}
			this.LogError("Addressables.Release was called on an object that Addressables was not previously aware of.  Thus nothing is being released");
		}

		public void Release<TObject>(AsyncOperationHandle<TObject> handle)
		{
			this.m_ResourceManager.Release(handle);
		}

		public void Release(AsyncOperationHandle handle)
		{
			this.m_ResourceManager.Release(handle);
		}

		private AsyncOperationHandle<long> GetDownloadSizeWithChain(AsyncOperationHandle dep, object key)
		{
			return this.ResourceManager.CreateChainOperation<long>(dep, (AsyncOperationHandle op) => this.GetDownloadSizeAsync(key));
		}

		private AsyncOperationHandle<long> ComputeCatalogSizeWithChain(IResourceLocation catalogLoc)
		{
			if (!catalogLoc.HasDependencies)
			{
				return this.ResourceManager.CreateCompletedOperation<long>(0L, "Attempting to get the remote header size of a content catalog, but no dependencies pointing to a remote location could be found for location " + catalogLoc.InternalId + ". Catalog location dependencies can be setup using CreateCatalogLocationWithHashDependencies");
			}
			AsyncOperationHandle dependentOp = this.ResourceManager.ProvideResource<string>(catalogLoc.Dependencies[0]);
			return this.ResourceManager.CreateChainOperation<long>(dependentOp, delegate(AsyncOperationHandle op)
			{
				try
				{
					Hash128 remoteHash = Hash128.Parse(op.Result.ToString());
					if (!this.IsCatalogCached(catalogLoc, remoteHash))
					{
						return this.GetRemoteCatalogHeaderSize(catalogLoc);
					}
				}
				catch (Exception arg)
				{
					return this.ResourceManager.CreateCompletedOperation<long>(0L, string.Format("Fetching the remote catalog size failed. {0}", arg));
				}
				return this.ResourceManager.CreateCompletedOperation<long>(0L, string.Empty);
			});
		}

		internal bool IsCatalogCached(IResourceLocation catalogLoc, Hash128 remoteHash)
		{
			return catalogLoc.HasDependencies && catalogLoc.Dependencies.Count == 2 && File.Exists(catalogLoc.Dependencies[1].InternalId) && !(remoteHash != Hash128.Parse(File.ReadAllText(catalogLoc.Dependencies[1].InternalId)));
		}

		internal AsyncOperationHandle<long> GetRemoteCatalogHeaderSize(IResourceLocation catalogLoc)
		{
			if (!catalogLoc.HasDependencies)
			{
				return this.ResourceManager.CreateCompletedOperation<long>(0L, "Attempting to get the remote header size of a content catalog, but no dependencies pointing to a remote location could be found for location " + catalogLoc.InternalId + ". Catalog location dependencies can be setup using CreateCatalogLocationWithHashDependencies");
			}
			AsyncOperationBase<UnityWebRequest> operation = new UnityWebRequestOperation(new UnityWebRequest(catalogLoc.Dependencies[0].InternalId.Replace(".hash", ".bin"), "HEAD"));
			return this.ResourceManager.CreateChainOperation<long, UnityWebRequest>(this.ResourceManager.StartOperation<UnityWebRequest>(operation, default(AsyncOperationHandle)), delegate(AsyncOperationHandle<UnityWebRequest> getOp)
			{
				UnityWebRequest result = getOp.Result;
				string text = (result != null) ? result.GetResponseHeader("Content-Length") : null;
				long result2;
				if (text != null && long.TryParse(text, out result2))
				{
					return this.ResourceManager.CreateCompletedOperation<long>(result2, "");
				}
				return this.ResourceManager.CreateCompletedOperation<long>(0L, "Attempting to get the remote header of a catalog failed.");
			});
		}

		private AsyncOperationHandle<long> GetDownloadSizeWithChain(AsyncOperationHandle dep, IEnumerable keys)
		{
			return this.ResourceManager.CreateChainOperation<long>(dep, (AsyncOperationHandle op) => this.GetDownloadSizeAsync(keys));
		}

		public AsyncOperationHandle<long> GetDownloadSizeAsync(object key)
		{
			return this.GetDownloadSizeAsync(new object[]
			{
				key
			});
		}

		public AsyncOperationHandle<long> GetDownloadSizeAsync(IEnumerable keys)
		{
			if (this.ShouldChainRequest)
			{
				return this.TrackHandle<long>(this.GetDownloadSizeWithChain(this.ChainOperation, keys));
			}
			List<IResourceLocation> list = new List<IResourceLocation>();
			foreach (object obj in keys)
			{
				IList<IResourceLocation> list2;
				if (obj is IList<IResourceLocation>)
				{
					list2 = (obj as IList<IResourceLocation>);
				}
				else if (obj is IResourceLocation)
				{
					using (List<ResourceLocatorInfo>.Enumerator enumerator2 = this.m_ResourceLocators.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							if (enumerator2.Current.CatalogLocation == obj as IResourceLocation)
							{
								return this.ComputeCatalogSizeWithChain(obj as IResourceLocation);
							}
						}
					}
					list2 = new List<IResourceLocation>(1)
					{
						obj as IResourceLocation
					};
				}
				else if (!this.GetResourceLocations(obj, typeof(object), out list2))
				{
					return this.ResourceManager.CreateCompletedOperationWithException<long>(0L, new InvalidKeyException(obj, typeof(object), this));
				}
				foreach (IResourceLocation resourceLocation in list2)
				{
					if (resourceLocation.HasDependencies)
					{
						list.AddRange(resourceLocation.Dependencies);
					}
				}
			}
			GetDownloadSizeOperation getDownloadSizeOperation = new GetDownloadSizeOperation();
			getDownloadSizeOperation.Init(list.Distinct(new ResourceLocationComparer()), this.ResourceManager);
			return this.ResourceManager.StartOperation<long>(getDownloadSizeOperation, default(AsyncOperationHandle));
		}

		private AsyncOperationHandle DownloadDependenciesAsyncWithChain(AsyncOperationHandle dep, object key, bool autoReleaseHandle)
		{
			AsyncOperationHandle<IList<IAssetBundleResource>> obj = this.ResourceManager.CreateChainOperation<IList<IAssetBundleResource>>(dep, (AsyncOperationHandle op) => this.DownloadDependenciesAsync(key, false).Convert<IList<IAssetBundleResource>>());
			if (autoReleaseHandle)
			{
				obj.ReleaseHandleOnCompletion();
			}
			return obj;
		}

		internal static void WrapAsDownloadLocations(List<IResourceLocation> locations)
		{
			for (int i = 0; i < locations.Count; i++)
			{
				locations[i] = new DownloadOnlyLocation(locations[i]);
			}
		}

		private static List<IResourceLocation> GatherDependenciesFromLocations(IList<IResourceLocation> locations)
		{
			HashSet<IResourceLocation> hashSet = new HashSet<IResourceLocation>(new ResourceLocationComparer());
			foreach (IResourceLocation resourceLocation in locations)
			{
				if (resourceLocation.ResourceType == typeof(IAssetBundleResource))
				{
					hashSet.Add(resourceLocation);
				}
				if (resourceLocation.HasDependencies)
				{
					foreach (IResourceLocation resourceLocation2 in resourceLocation.Dependencies)
					{
						if (resourceLocation2.ResourceType == typeof(IAssetBundleResource))
						{
							hashSet.Add(resourceLocation2);
						}
					}
				}
			}
			return new List<IResourceLocation>(hashSet);
		}

		public AsyncOperationHandle DownloadDependenciesAsync(object key, bool autoReleaseHandle = false)
		{
			if (this.ShouldChainRequest)
			{
				return this.DownloadDependenciesAsyncWithChain(this.ChainOperation, key, autoReleaseHandle);
			}
			IList<IResourceLocation> locations;
			if (!this.GetResourceLocations(key, typeof(object), out locations))
			{
				AsyncOperationHandle<IList<IAssetBundleResource>> obj = this.ResourceManager.CreateCompletedOperationWithException<IList<IAssetBundleResource>>(null, new InvalidKeyException(key, typeof(object), this));
				if (autoReleaseHandle)
				{
					obj.ReleaseHandleOnCompletion();
				}
				return obj;
			}
			List<IResourceLocation> locations2 = AddressablesImpl.GatherDependenciesFromLocations(locations);
			AddressablesImpl.WrapAsDownloadLocations(locations2);
			AsyncOperationHandle<IList<IAssetBundleResource>> obj2 = this.LoadAssetsAsync<IAssetBundleResource>(locations2, null, true);
			if (autoReleaseHandle)
			{
				obj2.ReleaseHandleOnCompletion();
			}
			return obj2;
		}

		private AsyncOperationHandle DownloadDependenciesAsyncWithChain(AsyncOperationHandle dep, IList<IResourceLocation> locations, bool autoReleaseHandle)
		{
			AsyncOperationHandle<IList<IAssetBundleResource>> obj = this.ResourceManager.CreateChainOperation<IList<IAssetBundleResource>>(dep, (AsyncOperationHandle op) => this.DownloadDependenciesAsync(locations, false).Convert<IList<IAssetBundleResource>>());
			if (autoReleaseHandle)
			{
				obj.ReleaseHandleOnCompletion();
			}
			return obj;
		}

		public AsyncOperationHandle DownloadDependenciesAsync(IList<IResourceLocation> locations, bool autoReleaseHandle = false)
		{
			if (this.ShouldChainRequest)
			{
				return this.DownloadDependenciesAsyncWithChain(this.ChainOperation, locations, autoReleaseHandle);
			}
			List<IResourceLocation> locations2 = AddressablesImpl.GatherDependenciesFromLocations(locations);
			AddressablesImpl.WrapAsDownloadLocations(locations2);
			AsyncOperationHandle<IList<IAssetBundleResource>> obj = this.LoadAssetsAsync<IAssetBundleResource>(locations2, null, true);
			if (autoReleaseHandle)
			{
				obj.ReleaseHandleOnCompletion();
			}
			return obj;
		}

		private AsyncOperationHandle DownloadDependenciesAsyncWithChain(AsyncOperationHandle dep, IEnumerable keys, Addressables.MergeMode mode, bool autoReleaseHandle)
		{
			AsyncOperationHandle<IList<IAssetBundleResource>> obj = this.ResourceManager.CreateChainOperation<IList<IAssetBundleResource>>(dep, (AsyncOperationHandle op) => this.DownloadDependenciesAsync(keys, mode, false).Convert<IList<IAssetBundleResource>>());
			if (autoReleaseHandle)
			{
				obj.ReleaseHandleOnCompletion();
			}
			return obj;
		}

		public AsyncOperationHandle DownloadDependenciesAsync(IEnumerable keys, Addressables.MergeMode mode, bool autoReleaseHandle = false)
		{
			if (this.ShouldChainRequest)
			{
				return this.DownloadDependenciesAsyncWithChain(this.ChainOperation, keys, mode, autoReleaseHandle);
			}
			IList<IResourceLocation> locations;
			if (!this.GetResourceLocations(keys, typeof(object), mode, out locations))
			{
				AsyncOperationHandle<IList<IAssetBundleResource>> obj = this.ResourceManager.CreateCompletedOperationWithException<IList<IAssetBundleResource>>(null, new InvalidKeyException(keys, typeof(object), mode, this));
				if (autoReleaseHandle)
				{
					obj.ReleaseHandleOnCompletion();
				}
				return obj;
			}
			List<IResourceLocation> locations2 = AddressablesImpl.GatherDependenciesFromLocations(locations);
			AddressablesImpl.WrapAsDownloadLocations(locations2);
			AsyncOperationHandle<IList<IAssetBundleResource>> obj2 = this.LoadAssetsAsync<IAssetBundleResource>(locations2, null, true);
			if (autoReleaseHandle)
			{
				obj2.ReleaseHandleOnCompletion();
			}
			return obj2;
		}

		internal bool ClearDependencyCacheForKey(object key)
		{
			bool flag = true;
			IList<IResourceLocation> list = null;
			IList<IResourceLocation> locations;
			if (key is IResourceLocation && (key as IResourceLocation).HasDependencies)
			{
				list = AddressablesImpl.GatherDependenciesFromLocations((key as IResourceLocation).Dependencies);
			}
			else if (this.GetResourceLocations(key, typeof(object), out locations))
			{
				list = AddressablesImpl.GatherDependenciesFromLocations(locations);
			}
			if (list != null)
			{
				foreach (IResourceLocation resourceLocation in list)
				{
					AssetBundleRequestOptions assetBundleRequestOptions = resourceLocation.Data as AssetBundleRequestOptions;
					if (assetBundleRequestOptions != null)
					{
						string bundleName = assetBundleRequestOptions.BundleName;
						if (this.m_ResourceManager.GetOperationFromCache(resourceLocation, typeof(IAssetBundleResource)) != null)
						{
							Debug.LogWarning(string.Concat(new string[]
							{
								"Attempting to clear cached version including ",
								bundleName,
								", while ",
								bundleName,
								" is currently loaded."
							}));
							if (!string.IsNullOrEmpty(assetBundleRequestOptions.Hash))
							{
								Hash128 hash = Hash128.Parse(assetBundleRequestOptions.Hash);
								Caching.ClearOtherCachedVersions(bundleName, hash);
							}
						}
						else
						{
							flag = (flag && Caching.ClearAllCachedVersions(bundleName));
						}
					}
				}
			}
			return flag;
		}

		internal void AutoReleaseHandleOnTypelessCompletion<TObject>(AsyncOperationHandle<TObject> handle)
		{
			handle.CompletedTypeless += delegate(AsyncOperationHandle op)
			{
				op.Release();
			};
		}

		public AsyncOperationHandle<bool> ClearDependencyCacheAsync(object key, bool autoReleaseHandle)
		{
			if (this.ShouldChainRequest)
			{
				AsyncOperationHandle<bool> result = this.ResourceManager.CreateChainOperation<bool>(this.ChainOperation, (AsyncOperationHandle op) => this.ClearDependencyCacheAsync(key, autoReleaseHandle));
				if (autoReleaseHandle)
				{
					result.ReleaseHandleOnCompletion();
				}
				return result;
			}
			bool flag = this.ClearDependencyCacheForKey(key);
			AsyncOperationHandle<bool> result2 = this.ResourceManager.CreateCompletedOperation<bool>(flag, flag ? string.Empty : "Unable to clear the cache.  AssetBundle's may still be loaded for the given key.");
			if (autoReleaseHandle)
			{
				result2.ReleaseHandleOnCompletion();
			}
			return result2;
		}

		public AsyncOperationHandle<bool> ClearDependencyCacheAsync(IList<IResourceLocation> locations, bool autoReleaseHandle)
		{
			if (this.ShouldChainRequest)
			{
				AsyncOperationHandle<bool> result = this.ResourceManager.CreateChainOperation<bool>(this.ChainOperation, (AsyncOperationHandle op) => this.ClearDependencyCacheAsync(locations, autoReleaseHandle));
				if (autoReleaseHandle)
				{
					result.ReleaseHandleOnCompletion();
				}
				return result;
			}
			bool flag = true;
			foreach (IResourceLocation key in locations)
			{
				flag = (flag && this.ClearDependencyCacheForKey(key));
			}
			AsyncOperationHandle<bool> result2 = this.ResourceManager.CreateCompletedOperation<bool>(flag, flag ? string.Empty : "Unable to clear the cache.  AssetBundle's may still be loaded for the given key(s).");
			if (autoReleaseHandle)
			{
				result2.ReleaseHandleOnCompletion();
			}
			return result2;
		}

		public AsyncOperationHandle<bool> ClearDependencyCacheAsync(IEnumerable keys, bool autoReleaseHandle)
		{
			if (this.ShouldChainRequest)
			{
				AsyncOperationHandle<bool> result = this.ResourceManager.CreateChainOperation<bool>(this.ChainOperation, (AsyncOperationHandle op) => this.ClearDependencyCacheAsync(keys, autoReleaseHandle));
				if (autoReleaseHandle)
				{
					result.ReleaseHandleOnCompletion();
				}
				return result;
			}
			bool flag = true;
			foreach (object key in keys)
			{
				flag = (flag && this.ClearDependencyCacheForKey(key));
			}
			AsyncOperationHandle<bool> result2 = this.ResourceManager.CreateCompletedOperation<bool>(flag, flag ? string.Empty : "Unable to clear the cache.  AssetBundle's may still be loaded for the given key(s).");
			if (autoReleaseHandle)
			{
				result2.ReleaseHandleOnCompletion();
			}
			return result2;
		}

		public AsyncOperationHandle<GameObject> InstantiateAsync(IResourceLocation location, Transform parent = null, bool instantiateInWorldSpace = false, bool trackHandle = true)
		{
			return this.InstantiateAsync(location, new InstantiationParameters(parent, instantiateInWorldSpace), trackHandle);
		}

		public AsyncOperationHandle<GameObject> InstantiateAsync(IResourceLocation location, Vector3 position, Quaternion rotation, Transform parent = null, bool trackHandle = true)
		{
			return this.InstantiateAsync(location, new InstantiationParameters(position, rotation, parent), trackHandle);
		}

		public AsyncOperationHandle<GameObject> InstantiateAsync(object key, Transform parent = null, bool instantiateInWorldSpace = false, bool trackHandle = true)
		{
			return this.InstantiateAsync(key, new InstantiationParameters(parent, instantiateInWorldSpace), trackHandle);
		}

		public AsyncOperationHandle<GameObject> InstantiateAsync(object key, Vector3 position, Quaternion rotation, Transform parent = null, bool trackHandle = true)
		{
			return this.InstantiateAsync(key, new InstantiationParameters(position, rotation, parent), trackHandle);
		}

		private AsyncOperationHandle<GameObject> InstantiateWithChain(AsyncOperationHandle dep, object key, InstantiationParameters instantiateParameters, bool trackHandle = true)
		{
			AsyncOperationHandle<GameObject> result = this.ResourceManager.CreateChainOperation<GameObject>(dep, (AsyncOperationHandle op) => this.InstantiateAsync(key, instantiateParameters, false));
			if (trackHandle)
			{
				result.CompletedTypeless += this.m_OnHandleCompleteAction;
			}
			return result;
		}

		public AsyncOperationHandle<GameObject> InstantiateAsync(object key, InstantiationParameters instantiateParameters, bool trackHandle = true)
		{
			if (this.ShouldChainRequest)
			{
				return this.InstantiateWithChain(this.ChainOperation, key, instantiateParameters, trackHandle);
			}
			key = this.EvaluateKey(key);
			using (List<ResourceLocatorInfo>.Enumerator enumerator = this.m_ResourceLocators.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					IList<IResourceLocation> list;
					if (enumerator.Current.Locator.Locate(key, typeof(GameObject), out list))
					{
						return this.InstantiateAsync(list[0], instantiateParameters, trackHandle);
					}
				}
			}
			return this.ResourceManager.CreateCompletedOperationWithException<GameObject>(null, new InvalidKeyException(key, typeof(GameObject), this));
		}

		private AsyncOperationHandle<GameObject> InstantiateWithChain(AsyncOperationHandle dep, IResourceLocation location, InstantiationParameters instantiateParameters, bool trackHandle = true)
		{
			AsyncOperationHandle<GameObject> result = this.ResourceManager.CreateChainOperation<GameObject>(dep, (AsyncOperationHandle op) => this.InstantiateAsync(location, instantiateParameters, false));
			if (trackHandle)
			{
				result.CompletedTypeless += this.m_OnHandleCompleteAction;
			}
			return result;
		}

		public AsyncOperationHandle<GameObject> InstantiateAsync(IResourceLocation location, InstantiationParameters instantiateParameters, bool trackHandle = true)
		{
			if (this.ShouldChainRequest)
			{
				return this.InstantiateWithChain(this.ChainOperation, location, instantiateParameters, trackHandle);
			}
			AsyncOperationHandle<GameObject> result = this.ResourceManager.ProvideInstance(this.InstanceProvider, location, instantiateParameters);
			if (!trackHandle)
			{
				return result;
			}
			result.CompletedTypeless += this.m_OnHandleCompleteAction;
			return result;
		}

		public bool ReleaseInstance(GameObject instance)
		{
			if (instance == null)
			{
				this.LogWarning("Addressables.ReleaseInstance() - trying to release null object.");
				return false;
			}
			AsyncOperationHandle asyncOperationHandle;
			if (this.m_resultToHandle.TryGetValue(instance, out asyncOperationHandle))
			{
				asyncOperationHandle.Release();
				return true;
			}
			return false;
		}

		internal AsyncOperationHandle<SceneInstance> LoadSceneWithChain(AsyncOperationHandle dep, object key, LoadSceneParameters loadSceneParameters, SceneReleaseMode releaseMode = SceneReleaseMode.ReleaseSceneWhenSceneUnloaded, bool activateOnLoad = true, int priority = 100)
		{
			return this.TrackHandle(this.ResourceManager.CreateChainOperation<SceneInstance>(dep, (AsyncOperationHandle op) => this.LoadSceneAsync(key, loadSceneParameters, releaseMode, activateOnLoad, priority, false)));
		}

		internal AsyncOperationHandle<SceneInstance> LoadSceneWithChain(AsyncOperationHandle dep, IResourceLocation key, LoadSceneParameters loadSceneParameters, SceneReleaseMode releaseMode = SceneReleaseMode.ReleaseSceneWhenSceneUnloaded, bool activateOnLoad = true, int priority = 100)
		{
			return this.TrackHandle(this.ResourceManager.CreateChainOperation<SceneInstance>(dep, (AsyncOperationHandle op) => this.LoadSceneAsync(key, loadSceneParameters, releaseMode, activateOnLoad, priority, false)));
		}

		public AsyncOperationHandle<SceneInstance> LoadSceneAsync(object key, LoadSceneParameters loadSceneParameters, SceneReleaseMode releaseMode = SceneReleaseMode.ReleaseSceneWhenSceneUnloaded, bool activateOnLoad = true, int priority = 100, bool trackHandle = true)
		{
			if (this.ShouldChainRequest)
			{
				return this.LoadSceneWithChain(this.ChainOperation, key, loadSceneParameters, releaseMode, activateOnLoad, priority);
			}
			IList<IResourceLocation> list;
			if (!this.GetResourceLocations(key, typeof(SceneInstance), out list))
			{
				return this.ResourceManager.CreateCompletedOperationWithException<SceneInstance>(default(SceneInstance), new InvalidKeyException(key, typeof(SceneInstance), this));
			}
			return this.LoadSceneAsync(list[0], loadSceneParameters, releaseMode, activateOnLoad, priority, trackHandle);
		}

		public AsyncOperationHandle<SceneInstance> LoadSceneAsync(IResourceLocation location, LoadSceneParameters loadSceneParameters, SceneReleaseMode releaseMode = SceneReleaseMode.ReleaseSceneWhenSceneUnloaded, bool activateOnLoad = true, int priority = 100, bool trackHandle = true)
		{
			if (this.ShouldChainRequest)
			{
				return this.LoadSceneWithChain(this.ChainOperation, location, loadSceneParameters, releaseMode, activateOnLoad, priority);
			}
			AsyncOperationHandle<SceneInstance> asyncOperationHandle = this.ResourceManager.ProvideScene(this.SceneProvider, location, loadSceneParameters, releaseMode, activateOnLoad, priority);
			if (trackHandle)
			{
				return this.TrackHandle(asyncOperationHandle);
			}
			return asyncOperationHandle;
		}

		public AsyncOperationHandle<SceneInstance> UnloadSceneAsync(SceneInstance scene, UnloadSceneOptions unloadOptions = UnloadSceneOptions.None, bool autoReleaseHandle = true)
		{
			AsyncOperationHandle asyncOperationHandle;
			if (!this.m_resultToHandle.TryGetValue(scene, out asyncOperationHandle))
			{
				string text = string.Format("Addressables.UnloadSceneAsync() - Cannot find handle for scene {0}", scene);
				this.LogWarning(text);
				return this.ResourceManager.CreateCompletedOperation<SceneInstance>(scene, text);
			}
			if (asyncOperationHandle.m_InternalOp.IsRunning)
			{
				return this.CreateUnloadSceneWithChain(asyncOperationHandle, unloadOptions, autoReleaseHandle);
			}
			return this.UnloadSceneAsync(asyncOperationHandle, unloadOptions, autoReleaseHandle);
		}

		public AsyncOperationHandle<SceneInstance> UnloadSceneAsync(AsyncOperationHandle handle, UnloadSceneOptions unloadOptions = UnloadSceneOptions.None, bool autoReleaseHandle = true)
		{
			if (handle.m_InternalOp.IsRunning)
			{
				return this.CreateUnloadSceneWithChain(handle, unloadOptions, autoReleaseHandle);
			}
			return this.UnloadSceneAsync(handle.Convert<SceneInstance>(), unloadOptions, autoReleaseHandle);
		}

		public AsyncOperationHandle<SceneInstance> UnloadSceneAsync(AsyncOperationHandle<SceneInstance> handle, UnloadSceneOptions unloadOptions = UnloadSceneOptions.None, bool autoReleaseHandle = true)
		{
			if (handle.m_InternalOp.IsRunning)
			{
				return this.CreateUnloadSceneWithChain(handle, unloadOptions, autoReleaseHandle);
			}
			return this.InternalUnloadScene(handle, unloadOptions, autoReleaseHandle);
		}

		internal AsyncOperationHandle<SceneInstance> CreateUnloadSceneWithChain(AsyncOperationHandle handle, UnloadSceneOptions unloadOptions, bool autoReleaseHandle)
		{
			return this.m_ResourceManager.CreateChainOperation<SceneInstance>(handle, (AsyncOperationHandle completedHandle) => this.InternalUnloadScene(completedHandle.Convert<SceneInstance>(), unloadOptions, autoReleaseHandle));
		}

		internal AsyncOperationHandle<SceneInstance> CreateUnloadSceneWithChain(AsyncOperationHandle<SceneInstance> handle, UnloadSceneOptions unloadOptions, bool autoReleaseHandle)
		{
			return this.m_ResourceManager.CreateChainOperation<SceneInstance, SceneInstance>(handle, (AsyncOperationHandle<SceneInstance> completedHandle) => this.InternalUnloadScene(completedHandle, unloadOptions, autoReleaseHandle));
		}

		internal AsyncOperationHandle<SceneInstance> InternalUnloadScene(AsyncOperationHandle<SceneInstance> handle, UnloadSceneOptions unloadOptions, bool autoReleaseHandle)
		{
			AsyncOperationHandle<SceneInstance> result = this.SceneProvider.ReleaseScene(this.ResourceManager, handle, unloadOptions);
			if (autoReleaseHandle)
			{
				result.ReleaseHandleOnCompletion();
			}
			return result;
		}

		private object EvaluateKey(object obj)
		{
			if (obj is IKeyEvaluator)
			{
				return (obj as IKeyEvaluator).RuntimeKey;
			}
			return obj;
		}

		internal AsyncOperationHandle<List<string>> CheckForCatalogUpdates(bool autoReleaseHandle = true)
		{
			if (this.ShouldChainRequest)
			{
				return this.CheckForCatalogUpdatesWithChain(autoReleaseHandle);
			}
			if (this.m_ActiveCheckUpdateOperation.IsValid())
			{
				this.m_ActiveCheckUpdateOperation.Release();
			}
			this.m_ActiveCheckUpdateOperation = new CheckCatalogsOperation(this).Start(this.m_ResourceLocators);
			if (autoReleaseHandle)
			{
				this.AutoReleaseHandleOnTypelessCompletion<List<string>>(this.m_ActiveCheckUpdateOperation);
			}
			return this.m_ActiveCheckUpdateOperation;
		}

		internal AsyncOperationHandle<List<string>> CheckForCatalogUpdatesWithChain(bool autoReleaseHandle)
		{
			return this.ResourceManager.CreateChainOperation<List<string>>(this.ChainOperation, (AsyncOperationHandle op) => this.CheckForCatalogUpdates(autoReleaseHandle));
		}

		public ResourceLocatorInfo GetLocatorInfo(string c)
		{
			foreach (ResourceLocatorInfo resourceLocatorInfo in this.m_ResourceLocators)
			{
				if (resourceLocatorInfo.Locator.LocatorId == c)
				{
					return resourceLocatorInfo;
				}
			}
			return null;
		}

		internal IEnumerable<string> CatalogsWithAvailableUpdates
		{
			get
			{
				return from s in this.m_ResourceLocators
				where s.ContentUpdateAvailable
				select s.Locator.LocatorId;
			}
		}

		internal AsyncOperationHandle<List<IResourceLocator>> UpdateCatalogs(IEnumerable<string> catalogIds = null, bool autoReleaseHandle = true, bool autoCleanBundleCache = false)
		{
			if (this.m_ActiveUpdateOperation.IsValid())
			{
				return this.m_ActiveUpdateOperation;
			}
			if (catalogIds == null && !this.CatalogsWithAvailableUpdates.Any<string>())
			{
				return this.m_ResourceManager.CreateChainOperation<List<IResourceLocator>, List<string>>(this.CheckForCatalogUpdates(true), (AsyncOperationHandle<List<string>> depOp) => this.UpdateCatalogs(this.CatalogsWithAvailableUpdates, autoReleaseHandle, autoCleanBundleCache));
			}
			AsyncOperationHandle<List<IResourceLocator>> asyncOperationHandle = new UpdateCatalogsOperation(this).Start((catalogIds == null) ? this.CatalogsWithAvailableUpdates : catalogIds, autoCleanBundleCache);
			if (autoReleaseHandle)
			{
				this.AutoReleaseHandleOnTypelessCompletion<List<IResourceLocator>>(asyncOperationHandle);
			}
			return asyncOperationHandle;
		}

		public bool Equals(IResourceLocation x, IResourceLocation y)
		{
			return x.PrimaryKey.Equals(y.PrimaryKey) && x.ResourceType.Equals(y.ResourceType) && x.InternalId.Equals(y.InternalId);
		}

		public int GetHashCode(IResourceLocation loc)
		{
			return loc.PrimaryKey.GetHashCode() * 31 + loc.ResourceType.GetHashCode();
		}

		internal AsyncOperationHandle<bool> CleanBundleCache(IEnumerable<string> catalogIds, bool forceSingleThreading)
		{
			if (this.ShouldChainRequest)
			{
				return this.CleanBundleCacheWithChain(catalogIds, forceSingleThreading);
			}
			if (catalogIds == null)
			{
				catalogIds = from s in this.m_ResourceLocators
				select s.Locator.LocatorId;
			}
			List<IResourceLocation> list = new List<IResourceLocation>();
			foreach (string text in catalogIds)
			{
				if (text != null)
				{
					ResourceLocatorInfo locatorInfo = this.GetLocatorInfo(text);
					if (locatorInfo != null && locatorInfo.CatalogLocation != null)
					{
						list.Add(locatorInfo.CatalogLocation);
					}
				}
			}
			if (list.Count == 0)
			{
				return this.ResourceManager.CreateCompletedOperation<bool>(false, "Provided catalogs do not load data from a catalog file. This can occur when using the \"Use Asset Database (fastest)\" playmode script. Bundle cache was not modified.");
			}
			return this.CleanBundleCache(this.ResourceManager.CreateGroupOperation<object>(list), forceSingleThreading);
		}

		internal AsyncOperationHandle<bool> CleanBundleCache(AsyncOperationHandle<IList<AsyncOperationHandle>> depOp, bool forceSingleThreading)
		{
			if (this.ShouldChainRequest)
			{
				return this.CleanBundleCacheWithChain(depOp, forceSingleThreading);
			}
			if (this.m_ActiveCleanBundleCacheOperation.IsValid() && !this.m_ActiveCleanBundleCacheOperation.IsDone)
			{
				return this.ResourceManager.CreateCompletedOperation<bool>(false, "Bundle cache is already being cleaned.");
			}
			this.m_ActiveCleanBundleCacheOperation = new CleanBundleCacheOperation(this, forceSingleThreading).Start(depOp);
			return this.m_ActiveCleanBundleCacheOperation;
		}

		internal AsyncOperationHandle<bool> CleanBundleCacheWithChain(AsyncOperationHandle<IList<AsyncOperationHandle>> depOp, bool forceSingleThreading)
		{
			return this.ResourceManager.CreateChainOperation<bool>(this.ChainOperation, (AsyncOperationHandle op) => this.CleanBundleCache(depOp, forceSingleThreading));
		}

		internal AsyncOperationHandle<bool> CleanBundleCacheWithChain(IEnumerable<string> catalogIds, bool forceSingleThreading)
		{
			return this.ResourceManager.CreateChainOperation<bool>(this.ChainOperation, (AsyncOperationHandle op) => this.CleanBundleCache(catalogIds, forceSingleThreading));
		}

		private ResourceManager m_ResourceManager;

		private IInstanceProvider m_InstanceProvider;

		private int m_CatalogRequestsTimeout;

		internal const string kCacheDataFolder = "{UnityEngine.Application.persistentDataPath}/com.unity.addressables/";

		public ISceneProvider SceneProvider;

		internal List<ResourceLocatorInfo> m_ResourceLocators = new List<ResourceLocatorInfo>();

		private AsyncOperationHandle<IResourceLocator> m_InitializationOperation;

		private AsyncOperationHandle<List<string>> m_ActiveCheckUpdateOperation;

		internal AsyncOperationHandle<List<IResourceLocator>> m_ActiveUpdateOperation;

		private Action<AsyncOperationHandle> m_OnHandleCompleteAction;

		private Action<AsyncOperationHandle> m_OnSceneHandleCompleteAction;

		private Action<AsyncOperationHandle> m_OnHandleDestroyedAction;

		private Dictionary<object, AsyncOperationHandle> m_resultToHandle = new Dictionary<object, AsyncOperationHandle>();

		internal HashSet<AsyncOperationHandle> m_SceneInstances = new HashSet<AsyncOperationHandle>();

		private AsyncOperationHandle<bool> m_ActiveCleanBundleCacheOperation;

		internal bool hasStartedInitialization;

		private class LoadResourceLocationKeyOp : AsyncOperationBase<IList<IResourceLocation>>
		{
			protected override string DebugName
			{
				get
				{
					return this.m_Keys.ToString();
				}
			}

			public void Init(AddressablesImpl aa, Type t, object keys)
			{
				this.m_Keys = keys;
				this.m_ResourceType = t;
				this.m_Addressables = aa;
			}

			protected override bool InvokeWaitForCompletion()
			{
				ResourceManager rm = this.m_RM;
				if (rm != null)
				{
					rm.Update(Time.unscaledDeltaTime);
				}
				if (!this.HasExecuted)
				{
					base.InvokeExecute();
				}
				return true;
			}

			protected override void Execute()
			{
				this.m_Addressables.GetResourceLocations(this.m_Keys, this.m_ResourceType, out this.m_locations);
				if (this.m_locations == null)
				{
					this.m_locations = new List<IResourceLocation>();
				}
				base.Complete(this.m_locations, true, string.Empty);
			}

			private object m_Keys;

			private IList<IResourceLocation> m_locations;

			private AddressablesImpl m_Addressables;

			private Type m_ResourceType;
		}

		private class LoadResourceLocationKeysOp : AsyncOperationBase<IList<IResourceLocation>>
		{
			protected override string DebugName
			{
				get
				{
					return "LoadResourceLocationKeysOp";
				}
			}

			public void Init(AddressablesImpl aa, Type t, IEnumerable key, Addressables.MergeMode mergeMode)
			{
				this.m_Key = key;
				this.m_ResourceType = t;
				this.m_MergeMode = mergeMode;
				this.m_Addressables = aa;
			}

			protected override void Execute()
			{
				this.m_Addressables.GetResourceLocations(this.m_Key, this.m_ResourceType, this.m_MergeMode, out this.m_locations);
				if (this.m_locations == null)
				{
					this.m_locations = new List<IResourceLocation>();
				}
				base.Complete(this.m_locations, true, string.Empty);
			}

			protected override bool InvokeWaitForCompletion()
			{
				ResourceManager rm = this.m_RM;
				if (rm != null)
				{
					rm.Update(Time.unscaledDeltaTime);
				}
				if (!this.HasExecuted)
				{
					base.InvokeExecute();
				}
				return true;
			}

			private IEnumerable m_Key;

			private Addressables.MergeMode m_MergeMode;

			private IList<IResourceLocation> m_locations;

			private AddressablesImpl m_Addressables;

			private Type m_ResourceType;
		}
	}
}
