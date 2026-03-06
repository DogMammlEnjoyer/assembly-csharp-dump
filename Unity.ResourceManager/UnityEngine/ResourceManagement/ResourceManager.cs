using System;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.Exceptions;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.Util;
using UnityEngine.SceneManagement;

namespace UnityEngine.ResourceManagement
{
	public class ResourceManager : IDisposable
	{
		public static Action<AsyncOperationHandle, Exception> ExceptionHandler { get; set; }

		public Func<IResourceLocation, string> InternalIdTransformFunc { get; set; }

		public string TransformInternalId(IResourceLocation location)
		{
			if (this.InternalIdTransformFunc != null)
			{
				return this.InternalIdTransformFunc(location);
			}
			return location.InternalId;
		}

		public Action<UnityWebRequest> WebRequestOverride { get; set; }

		internal int OperationCacheCount
		{
			get
			{
				return this.m_AssetOperationCache.Count;
			}
		}

		internal int InstanceOperationCount
		{
			get
			{
				return this.m_TrackedInstanceOperations.Count;
			}
		}

		internal int DeferredCompleteCallbacksCount
		{
			get
			{
				return this.m_DeferredCompleteCallbacks.Count;
			}
		}

		internal int DeferredCallbackCount
		{
			get
			{
				List<ResourceManager.DeferredCallbackRegisterRequest> deferredCallbacksToRegister = this.m_DeferredCallbacksToRegister;
				if (deferredCallbacksToRegister == null)
				{
					return 0;
				}
				return deferredCallbacksToRegister.Count;
			}
		}

		public void AddUpdateReceiver(IUpdateReceiver receiver)
		{
			if (receiver == null)
			{
				return;
			}
			this.m_UpdateReceivers.Add(receiver);
		}

		public void RemoveUpdateReciever(IUpdateReceiver receiver)
		{
			if (receiver == null)
			{
				return;
			}
			if (this.m_UpdatingReceivers)
			{
				if (this.m_UpdateReceiversToRemove == null)
				{
					this.m_UpdateReceiversToRemove = new List<IUpdateReceiver>();
				}
				this.m_UpdateReceiversToRemove.Add(receiver);
				return;
			}
			this.m_UpdateReceivers.Remove(receiver);
		}

		public IAllocationStrategy Allocator
		{
			get
			{
				return this.m_allocator;
			}
			set
			{
				this.m_allocator = value;
			}
		}

		public IList<IResourceProvider> ResourceProviders
		{
			get
			{
				return this.m_ResourceProviders;
			}
		}

		public CertificateHandler CertificateHandlerInstance { get; set; }

		public ResourceManager(IAllocationStrategy alloc = null)
		{
			this.m_ReleaseOpNonCached = new Action<IAsyncOperation>(this.OnOperationDestroyNonCached);
			this.m_ReleaseOpCached = new Action<IAsyncOperation>(this.OnOperationDestroyCached);
			this.m_ReleaseInstanceOp = new Action<IAsyncOperation>(this.OnInstanceOperationDestroy);
			IAllocationStrategy allocator;
			if (alloc != null)
			{
				allocator = alloc;
			}
			else
			{
				IAllocationStrategy allocationStrategy = new DefaultAllocationStrategy();
				allocator = allocationStrategy;
			}
			this.m_allocator = allocator;
			this.m_ResourceProviders.OnElementAdded += new Action<IResourceProvider>(this.OnObjectAdded);
			this.m_ResourceProviders.OnElementRemoved += new Action<IResourceProvider>(this.OnObjectRemoved);
			this.m_UpdateReceivers.OnElementAdded += delegate(IUpdateReceiver x)
			{
				this.RegisterForCallbacks();
			};
		}

		private void OnObjectAdded(object obj)
		{
			IUpdateReceiver updateReceiver = obj as IUpdateReceiver;
			if (updateReceiver != null)
			{
				this.AddUpdateReceiver(updateReceiver);
			}
		}

		private void OnObjectRemoved(object obj)
		{
			IUpdateReceiver updateReceiver = obj as IUpdateReceiver;
			if (updateReceiver != null)
			{
				this.RemoveUpdateReciever(updateReceiver);
			}
		}

		internal void RegisterForCallbacks()
		{
			if (this.CallbackHooksEnabled && !this.m_RegisteredForCallbacks)
			{
				this.m_RegisteredForCallbacks = true;
				ComponentSingleton<MonoBehaviourCallbackHooks>.Instance.OnUpdateDelegate += this.Update;
			}
		}

		public IResourceProvider GetResourceProvider(Type t, IResourceLocation location)
		{
			if (location != null)
			{
				IResourceProvider result = null;
				int key = location.ProviderId.GetHashCode() * 31 + ((t == null) ? 0 : t.GetHashCode());
				if (!this.m_providerMap.TryGetValue(key, out result))
				{
					for (int i = 0; i < this.ResourceProviders.Count; i++)
					{
						IResourceProvider resourceProvider = this.ResourceProviders[i];
						if (resourceProvider.ProviderId.Equals(location.ProviderId, StringComparison.Ordinal) && (t == null || resourceProvider.CanProvide(t, location)))
						{
							this.m_providerMap.Add(key, result = resourceProvider);
							break;
						}
					}
				}
				return result;
			}
			return null;
		}

		private Type GetDefaultTypeForLocation(IResourceLocation loc)
		{
			IResourceProvider resourceProvider = this.GetResourceProvider(null, loc);
			if (resourceProvider == null)
			{
				return typeof(object);
			}
			Type defaultType = resourceProvider.GetDefaultType(loc);
			if (!(defaultType != null))
			{
				return typeof(object);
			}
			return defaultType;
		}

		private int CalculateLocationsHash(IList<IResourceLocation> locations, Type t = null)
		{
			if (locations == null || locations.Count == 0)
			{
				return 0;
			}
			int num = 17;
			foreach (IResourceLocation resourceLocation in locations)
			{
				Type resultType = (t != null) ? t : this.GetDefaultTypeForLocation(resourceLocation);
				num = num * 31 + resourceLocation.Hash(resultType);
			}
			return num;
		}

		private AsyncOperationHandle ProvideResource(IResourceLocation location, Type desiredType = null, bool releaseDependenciesOnFailure = true)
		{
			if (location == null)
			{
				throw new ArgumentNullException("location");
			}
			IResourceProvider resourceProvider = null;
			if (desiredType == null)
			{
				resourceProvider = this.GetResourceProvider(desiredType, location);
				if (resourceProvider == null)
				{
					UnknownResourceProviderException exception = new UnknownResourceProviderException(location);
					return this.CreateCompletedOperationInternal<object>(null, false, exception, releaseDependenciesOnFailure);
				}
				desiredType = resourceProvider.GetDefaultType(location);
			}
			if (resourceProvider == null)
			{
				resourceProvider = this.GetResourceProvider(desiredType, location);
			}
			IOperationCacheKey operationCacheKey = this.CreateCacheKeyForLocation(resourceProvider, location, desiredType);
			IAsyncOperation asyncOperation;
			if (this.m_AssetOperationCache.TryGetValue(operationCacheKey, out asyncOperation))
			{
				asyncOperation.IncrementReferenceCount();
				return new AsyncOperationHandle(asyncOperation, location.ToString());
			}
			Type type;
			if (!this.m_ProviderOperationTypeCache.TryGetValue(desiredType, out type))
			{
				this.m_ProviderOperationTypeCache.Add(desiredType, type = typeof(ProviderOperation<>).MakeGenericType(new Type[]
				{
					desiredType
				}));
			}
			asyncOperation = this.CreateOperation<IAsyncOperation>(type, type.GetHashCode(), operationCacheKey, this.m_ReleaseOpCached);
			int dependencyHashCode = location.DependencyHashCode;
			AsyncOperationHandle<IList<AsyncOperationHandle>> asyncOperationHandle = location.HasDependencies ? this.ProvideResourceGroupCached(location.Dependencies, dependencyHashCode, null, null, releaseDependenciesOnFailure) : default(AsyncOperationHandle<IList<AsyncOperationHandle>>);
			((IGenericProviderOperation)asyncOperation).Init(this, resourceProvider, location, asyncOperationHandle, releaseDependenciesOnFailure);
			AsyncOperationHandle result = this.StartOperation(asyncOperation, asyncOperationHandle);
			result.LocationName = location.ToString();
			if (asyncOperationHandle.IsValid())
			{
				asyncOperationHandle.Release();
			}
			return result;
		}

		internal IAsyncOperation GetOperationFromCache(IResourceLocation location, Type desiredType)
		{
			IResourceProvider resourceProvider = this.GetResourceProvider(desiredType, location);
			IOperationCacheKey key = this.CreateCacheKeyForLocation(resourceProvider, location, desiredType);
			IAsyncOperation result;
			if (this.m_AssetOperationCache.TryGetValue(key, out result))
			{
				return result;
			}
			return null;
		}

		internal IOperationCacheKey CreateCacheKeyForLocation(IResourceProvider provider, IResourceLocation location, Type desiredType = null)
		{
			AssetBundleProvider assetBundleProvider = provider as AssetBundleProvider;
			if (assetBundleProvider != null)
			{
				return assetBundleProvider.CreateCacheKeyForLocation(this, location, desiredType);
			}
			return new LocationCacheKey(location, desiredType);
		}

		public AsyncOperationHandle<TObject> ProvideResource<TObject>(IResourceLocation location)
		{
			return this.ProvideResource(location, typeof(TObject), true).Convert<TObject>();
		}

		public AsyncOperationHandle<TObject> StartOperation<TObject>(AsyncOperationBase<TObject> operation, AsyncOperationHandle dependency)
		{
			operation.Start(this, dependency, this.m_UpdateCallbacks);
			return operation.Handle;
		}

		internal AsyncOperationHandle StartOperation(IAsyncOperation operation, AsyncOperationHandle dependency)
		{
			operation.Start(this, dependency, this.m_UpdateCallbacks);
			return operation.Handle;
		}

		private void OnInstanceOperationDestroy(IAsyncOperation o)
		{
			this.m_TrackedInstanceOperations.Remove(o as ResourceManager.InstanceOperation);
			this.Allocator.Release(o.GetType().GetHashCode(), o);
		}

		private void OnOperationDestroyNonCached(IAsyncOperation o)
		{
			this.Allocator.Release(o.GetType().GetHashCode(), o);
		}

		private void OnOperationDestroyCached(IAsyncOperation o)
		{
			this.Allocator.Release(o.GetType().GetHashCode(), o);
			ICachable cachable = o as ICachable;
			if (((cachable != null) ? cachable.Key : null) != null)
			{
				this.RemoveOperationFromCache(cachable.Key);
				cachable.Key = null;
			}
		}

		internal T CreateOperation<T>(Type actualType, int typeHash, IOperationCacheKey cacheKey, Action<IAsyncOperation> onDestroyAction) where T : IAsyncOperation
		{
			if (cacheKey == null)
			{
				T result = (T)((object)this.Allocator.New(actualType, typeHash));
				result.OnDestroy = onDestroyAction;
				return result;
			}
			T t = (T)((object)this.Allocator.New(actualType, typeHash));
			t.OnDestroy = onDestroyAction;
			ICachable cachable = t as ICachable;
			if (cachable != null)
			{
				cachable.Key = cacheKey;
				this.AddOperationToCache(cacheKey, t);
			}
			return t;
		}

		internal void AddOperationToCache(IOperationCacheKey key, IAsyncOperation operation)
		{
			if (!this.IsOperationCached(key))
			{
				this.m_AssetOperationCache.Add(key, operation);
			}
		}

		internal bool RemoveOperationFromCache(IOperationCacheKey key)
		{
			return !this.IsOperationCached(key) || this.m_AssetOperationCache.Remove(key);
		}

		internal bool IsOperationCached(IOperationCacheKey key)
		{
			return this.m_AssetOperationCache.ContainsKey(key);
		}

		internal int CachedOperationCount()
		{
			return this.m_AssetOperationCache.Count;
		}

		internal void ClearOperationCache()
		{
			this.m_AssetOperationCache.Clear();
		}

		public AsyncOperationHandle<TObject> CreateCompletedOperation<TObject>(TObject result, string errorMsg)
		{
			bool flag = string.IsNullOrEmpty(errorMsg);
			return this.CreateCompletedOperationInternal<TObject>(result, flag, (!flag) ? new Exception(errorMsg) : null, true);
		}

		public AsyncOperationHandle<TObject> CreateCompletedOperationWithException<TObject>(TObject result, Exception exception)
		{
			return this.CreateCompletedOperationInternal<TObject>(result, exception == null, exception, true);
		}

		internal AsyncOperationHandle<TObject> CreateCompletedOperationInternal<TObject>(TObject result, bool success, Exception exception, bool releaseDependenciesOnFailure = true)
		{
			ResourceManager.CompletedOperation<TObject> completedOperation = this.CreateOperation<ResourceManager.CompletedOperation<TObject>>(typeof(ResourceManager.CompletedOperation<TObject>), typeof(ResourceManager.CompletedOperation<TObject>).GetHashCode(), null, this.m_ReleaseOpNonCached);
			completedOperation.Init(result, success, exception, releaseDependenciesOnFailure);
			return this.StartOperation<TObject>(completedOperation, default(AsyncOperationHandle));
		}

		public void Release(AsyncOperationHandle handle)
		{
			handle.Release();
		}

		public AsyncOperationHandle<TObject> Acquire<TObject>(AsyncOperationHandle<TObject> handle)
		{
			return handle.Acquire();
		}

		public void Acquire(AsyncOperationHandle handle)
		{
			handle.Acquire();
		}

		private GroupOperation AcquireGroupOpFromCache(IOperationCacheKey key)
		{
			IAsyncOperation asyncOperation;
			if (this.m_AssetOperationCache.TryGetValue(key, out asyncOperation))
			{
				asyncOperation.IncrementReferenceCount();
				return (GroupOperation)asyncOperation;
			}
			return null;
		}

		public AsyncOperationHandle<IList<AsyncOperationHandle>> CreateGroupOperation<T>(IList<IResourceLocation> locations)
		{
			GroupOperation groupOperation = this.CreateOperation<GroupOperation>(typeof(GroupOperation), ResourceManager.s_GroupOperationTypeHash, null, this.m_ReleaseOpNonCached);
			List<AsyncOperationHandle> list = new List<AsyncOperationHandle>(locations.Count);
			foreach (IResourceLocation location in locations)
			{
				list.Add(this.ProvideResource<T>(location));
			}
			groupOperation.Init(list, true, false);
			return this.StartOperation<IList<AsyncOperationHandle>>(groupOperation, default(AsyncOperationHandle));
		}

		internal AsyncOperationHandle<IList<AsyncOperationHandle>> CreateGroupOperation<T>(IList<IResourceLocation> locations, bool allowFailedDependencies)
		{
			GroupOperation groupOperation = this.CreateOperation<GroupOperation>(typeof(GroupOperation), ResourceManager.s_GroupOperationTypeHash, null, this.m_ReleaseOpNonCached);
			List<AsyncOperationHandle> list = new List<AsyncOperationHandle>(locations.Count);
			foreach (IResourceLocation location in locations)
			{
				list.Add(this.ProvideResource<T>(location));
			}
			GroupOperation.GroupOperationSettings groupOperationSettings = GroupOperation.GroupOperationSettings.None;
			if (allowFailedDependencies)
			{
				groupOperationSettings |= GroupOperation.GroupOperationSettings.AllowFailedDependencies;
			}
			groupOperation.Init(list, groupOperationSettings);
			return this.StartOperation<IList<AsyncOperationHandle>>(groupOperation, default(AsyncOperationHandle));
		}

		public AsyncOperationHandle<IList<AsyncOperationHandle>> CreateGenericGroupOperation(List<AsyncOperationHandle> operations, bool releasedCachedOpOnComplete = false)
		{
			GroupOperation groupOperation = this.CreateOperation<GroupOperation>(typeof(GroupOperation), ResourceManager.s_GroupOperationTypeHash, new AsyncOpHandlesCacheKey(operations), releasedCachedOpOnComplete ? this.m_ReleaseOpCached : this.m_ReleaseOpNonCached);
			groupOperation.Init(operations, true, false);
			return this.StartOperation<IList<AsyncOperationHandle>>(groupOperation, default(AsyncOperationHandle));
		}

		internal AsyncOperationHandle<IList<AsyncOperationHandle>> ProvideResourceGroupCached(IList<IResourceLocation> locations, int groupHash, Type desiredType, Action<AsyncOperationHandle> callback, bool releaseDependenciesOnFailure = true)
		{
			DependenciesCacheKey dependenciesCacheKey = new DependenciesCacheKey(locations, groupHash);
			GroupOperation groupOperation = this.AcquireGroupOpFromCache(dependenciesCacheKey);
			AsyncOperationHandle<IList<AsyncOperationHandle>> result;
			if (groupOperation == null)
			{
				groupOperation = this.CreateOperation<GroupOperation>(typeof(GroupOperation), ResourceManager.s_GroupOperationTypeHash, dependenciesCacheKey, this.m_ReleaseOpCached);
				List<AsyncOperationHandle> list = new List<AsyncOperationHandle>(locations.Count);
				foreach (IResourceLocation location in locations)
				{
					list.Add(this.ProvideResource(location, desiredType, releaseDependenciesOnFailure));
				}
				groupOperation.Init(list, releaseDependenciesOnFailure, false);
				result = this.StartOperation<IList<AsyncOperationHandle>>(groupOperation, default(AsyncOperationHandle));
			}
			else
			{
				result = groupOperation.Handle;
			}
			if (callback != null)
			{
				IList<AsyncOperationHandle> dependentOps = groupOperation.GetDependentOps();
				for (int i = 0; i < dependentOps.Count; i++)
				{
					dependentOps[i].Completed += callback;
				}
			}
			return result;
		}

		public AsyncOperationHandle<IList<TObject>> ProvideResources<TObject>(IList<IResourceLocation> locations, Action<TObject> callback = null)
		{
			return this.ProvideResources<TObject>(locations, true, callback);
		}

		public AsyncOperationHandle<IList<TObject>> ProvideResources<TObject>(IList<IResourceLocation> locations, bool releaseDependenciesOnFailure, Action<TObject> callback = null)
		{
			if (locations == null)
			{
				return this.CreateCompletedOperation<IList<TObject>>(null, "Null Location");
			}
			Action<AsyncOperationHandle> callback2 = null;
			if (callback != null)
			{
				callback2 = delegate(AsyncOperationHandle x)
				{
					callback((TObject)((object)x.Result));
				};
			}
			AsyncOperationHandle<IList<AsyncOperationHandle>> obj = this.ProvideResourceGroupCached(locations, this.CalculateLocationsHash(locations, typeof(TObject)), typeof(TObject), callback2, releaseDependenciesOnFailure);
			AsyncOperationHandle<IList<TObject>> result = this.CreateChainOperation<IList<TObject>>(obj, delegate(AsyncOperationHandle resultHandle)
			{
				AsyncOperationHandle<IList<AsyncOperationHandle>> asyncOperationHandle = resultHandle.Convert<IList<AsyncOperationHandle>>();
				List<TObject> list = new List<TObject>();
				Exception ex = null;
				if (asyncOperationHandle.Status == AsyncOperationStatus.Succeeded)
				{
					using (IEnumerator<AsyncOperationHandle> enumerator = asyncOperationHandle.Result.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							AsyncOperationHandle asyncOperationHandle2 = enumerator.Current;
							list.Add(asyncOperationHandle2.Convert<TObject>().Result);
						}
						goto IL_F5;
					}
				}
				bool flag = false;
				if (!releaseDependenciesOnFailure)
				{
					foreach (AsyncOperationHandle asyncOperationHandle3 in asyncOperationHandle.Result)
					{
						if (asyncOperationHandle3.Status == AsyncOperationStatus.Succeeded)
						{
							list.Add(asyncOperationHandle3.Convert<TObject>().Result);
							flag = true;
						}
						else
						{
							list.Add(default(TObject));
						}
					}
				}
				if (!flag)
				{
					list = null;
					ex = new ResourceManagerException("ProvideResources failed", asyncOperationHandle.OperationException);
				}
				else
				{
					ex = new ResourceManagerException("Partial success in ProvideResources.  Some items failed to load. See earlier logs for more info.", asyncOperationHandle.OperationException);
				}
				IL_F5:
				return this.CreateCompletedOperationInternal<IList<TObject>>(list, ex == null, ex, releaseDependenciesOnFailure);
			}, releaseDependenciesOnFailure);
			obj.Release();
			return result;
		}

		public AsyncOperationHandle<TObject> CreateChainOperation<TObject, TObjectDependency>(AsyncOperationHandle<TObjectDependency> dependentOp, Func<AsyncOperationHandle<TObjectDependency>, AsyncOperationHandle<TObject>> callback)
		{
			ChainOperation<TObject, TObjectDependency> chainOperation = this.CreateOperation<ChainOperation<TObject, TObjectDependency>>(typeof(ChainOperation<TObject, TObjectDependency>), typeof(ChainOperation<TObject, TObjectDependency>).GetHashCode(), null, null);
			chainOperation.Init(dependentOp, callback, true);
			return this.StartOperation<TObject>(chainOperation, dependentOp);
		}

		public AsyncOperationHandle<TObject> CreateChainOperation<TObject>(AsyncOperationHandle dependentOp, Func<AsyncOperationHandle, AsyncOperationHandle<TObject>> callback)
		{
			ChainOperationTypelessDepedency<TObject> chainOperationTypelessDepedency = new ChainOperationTypelessDepedency<TObject>();
			chainOperationTypelessDepedency.Init(dependentOp, callback, true);
			return this.StartOperation<TObject>(chainOperationTypelessDepedency, dependentOp);
		}

		public AsyncOperationHandle<TObject> CreateChainOperation<TObject, TObjectDependency>(AsyncOperationHandle<TObjectDependency> dependentOp, Func<AsyncOperationHandle<TObjectDependency>, AsyncOperationHandle<TObject>> callback, bool releaseDependenciesOnFailure = true)
		{
			ChainOperation<TObject, TObjectDependency> chainOperation = this.CreateOperation<ChainOperation<TObject, TObjectDependency>>(typeof(ChainOperation<TObject, TObjectDependency>), typeof(ChainOperation<TObject, TObjectDependency>).GetHashCode(), null, null);
			chainOperation.Init(dependentOp, callback, releaseDependenciesOnFailure);
			return this.StartOperation<TObject>(chainOperation, dependentOp);
		}

		public AsyncOperationHandle<TObject> CreateChainOperation<TObject>(AsyncOperationHandle dependentOp, Func<AsyncOperationHandle, AsyncOperationHandle<TObject>> callback, bool releaseDependenciesOnFailure = true)
		{
			ChainOperationTypelessDepedency<TObject> chainOperationTypelessDepedency = new ChainOperationTypelessDepedency<TObject>();
			chainOperationTypelessDepedency.Init(dependentOp, callback, releaseDependenciesOnFailure);
			return this.StartOperation<TObject>(chainOperationTypelessDepedency, dependentOp);
		}

		public AsyncOperationHandle<SceneInstance> ProvideScene(ISceneProvider sceneProvider, IResourceLocation location, LoadSceneMode loadSceneMode, bool activateOnLoad, int priority)
		{
			if (sceneProvider == null)
			{
				throw new NullReferenceException("sceneProvider is null");
			}
			return sceneProvider.ProvideScene(this, location, new LoadSceneParameters(loadSceneMode), SceneReleaseMode.ReleaseSceneWhenSceneUnloaded, activateOnLoad, priority);
		}

		public AsyncOperationHandle<SceneInstance> ProvideScene(ISceneProvider sceneProvider, IResourceLocation location, LoadSceneParameters loadSceneParameters, bool activateOnLoad, int priority)
		{
			if (sceneProvider == null)
			{
				throw new NullReferenceException("sceneProvider is null");
			}
			return sceneProvider.ProvideScene(this, location, loadSceneParameters, SceneReleaseMode.ReleaseSceneWhenSceneUnloaded, activateOnLoad, priority);
		}

		public AsyncOperationHandle<SceneInstance> ProvideScene(ISceneProvider sceneProvider, IResourceLocation location, LoadSceneParameters loadSceneParameters, SceneReleaseMode releaseMode, bool activateOnLoad, int priority)
		{
			if (sceneProvider == null)
			{
				throw new NullReferenceException("sceneProvider is null");
			}
			return sceneProvider.ProvideScene(this, location, loadSceneParameters, releaseMode, activateOnLoad, priority);
		}

		public AsyncOperationHandle<SceneInstance> ReleaseScene(ISceneProvider sceneProvider, AsyncOperationHandle<SceneInstance> sceneLoadHandle)
		{
			if (sceneProvider == null)
			{
				throw new NullReferenceException("sceneProvider is null");
			}
			return sceneProvider.ReleaseScene(this, sceneLoadHandle);
		}

		public AsyncOperationHandle<GameObject> ProvideInstance(IInstanceProvider provider, IResourceLocation location, InstantiationParameters instantiateParameters)
		{
			if (provider == null)
			{
				throw new NullReferenceException("provider is null.  Assign a valid IInstanceProvider object before using.");
			}
			if (location == null)
			{
				throw new ArgumentNullException("location");
			}
			AsyncOperationHandle<GameObject> asyncOperationHandle = this.ProvideResource<GameObject>(location);
			ResourceManager.InstanceOperation instanceOperation = this.CreateOperation<ResourceManager.InstanceOperation>(typeof(ResourceManager.InstanceOperation), ResourceManager.s_InstanceOperationTypeHash, null, this.m_ReleaseInstanceOp);
			instanceOperation.Init(this, provider, instantiateParameters, asyncOperationHandle);
			this.m_TrackedInstanceOperations.Add(instanceOperation);
			return this.StartOperation<GameObject>(instanceOperation, asyncOperationHandle);
		}

		public void CleanupSceneInstances(Scene scene)
		{
			List<ResourceManager.InstanceOperation> list = null;
			foreach (ResourceManager.InstanceOperation instanceOperation in this.m_TrackedInstanceOperations)
			{
				if (instanceOperation.Result == null && scene == instanceOperation.InstanceScene())
				{
					if (list == null)
					{
						list = new List<ResourceManager.InstanceOperation>();
					}
					list.Add(instanceOperation);
				}
			}
			if (list != null)
			{
				foreach (ResourceManager.InstanceOperation instanceOperation2 in list)
				{
					this.m_TrackedInstanceOperations.Remove(instanceOperation2);
					instanceOperation2.DecrementReferenceCount();
				}
			}
		}

		private void ExecuteDeferredCallbacks()
		{
			this.m_InsideExecuteDeferredCallbacksMethod = true;
			for (int i = 0; i < this.m_DeferredCompleteCallbacks.Count; i++)
			{
				this.m_DeferredCompleteCallbacks[i].InvokeCompletionEvent();
				this.m_DeferredCompleteCallbacks[i].DecrementReferenceCount();
			}
			this.m_DeferredCompleteCallbacks.Clear();
			this.m_InsideExecuteDeferredCallbacksMethod = false;
		}

		internal void RegisterForDeferredCallback(IAsyncOperation op, bool incrementRefCount = true)
		{
			if (this.CallbackHooksEnabled && this.m_InsideExecuteDeferredCallbacksMethod)
			{
				if (this.m_DeferredCallbacksToRegister == null)
				{
					this.m_DeferredCallbacksToRegister = new List<ResourceManager.DeferredCallbackRegisterRequest>();
				}
				this.m_DeferredCallbacksToRegister.Add(new ResourceManager.DeferredCallbackRegisterRequest
				{
					operation = op,
					incrementRefCount = incrementRefCount
				});
				return;
			}
			if (incrementRefCount)
			{
				op.IncrementReferenceCount();
			}
			this.m_DeferredCompleteCallbacks.Add(op);
			this.RegisterForCallbacks();
		}

		internal void Update(float unscaledDeltaTime)
		{
			if (this.m_InsideUpdateMethod)
			{
				throw new Exception("Reentering the Update method is not allowed.  This can happen when calling WaitForCompletion on an operation while inside of a callback.");
			}
			this.m_InsideUpdateMethod = true;
			this.m_UpdateCallbacks.Invoke(unscaledDeltaTime);
			this.m_UpdatingReceivers = true;
			for (int i = 0; i < this.m_UpdateReceivers.Count; i++)
			{
				this.m_UpdateReceivers[i].Update(unscaledDeltaTime);
			}
			this.m_UpdatingReceivers = false;
			if (this.m_UpdateReceiversToRemove != null)
			{
				foreach (IUpdateReceiver item in this.m_UpdateReceiversToRemove)
				{
					this.m_UpdateReceivers.Remove(item);
				}
				this.m_UpdateReceiversToRemove = null;
			}
			if (this.m_DeferredCallbacksToRegister != null)
			{
				foreach (ResourceManager.DeferredCallbackRegisterRequest deferredCallbackRegisterRequest in this.m_DeferredCallbacksToRegister)
				{
					this.RegisterForDeferredCallback(deferredCallbackRegisterRequest.operation, deferredCallbackRegisterRequest.incrementRefCount);
				}
				this.m_DeferredCallbacksToRegister = null;
			}
			this.ExecuteDeferredCallbacks();
			this.m_InsideUpdateMethod = false;
		}

		public void Dispose()
		{
			if (ComponentSingleton<MonoBehaviourCallbackHooks>.Exists && this.m_RegisteredForCallbacks)
			{
				ComponentSingleton<MonoBehaviourCallbackHooks>.Instance.OnUpdateDelegate -= this.Update;
				this.m_RegisteredForCallbacks = false;
			}
		}

		internal bool CallbackHooksEnabled = true;

		private ListWithEvents<IResourceProvider> m_ResourceProviders = new ListWithEvents<IResourceProvider>();

		private IAllocationStrategy m_allocator;

		internal ListWithEvents<IUpdateReceiver> m_UpdateReceivers = new ListWithEvents<IUpdateReceiver>();

		private List<IUpdateReceiver> m_UpdateReceiversToRemove;

		private bool m_UpdatingReceivers;

		private bool m_InsideUpdateMethod;

		internal Dictionary<int, IResourceProvider> m_providerMap = new Dictionary<int, IResourceProvider>();

		private Dictionary<IOperationCacheKey, IAsyncOperation> m_AssetOperationCache = new Dictionary<IOperationCacheKey, IAsyncOperation>();

		private HashSet<ResourceManager.InstanceOperation> m_TrackedInstanceOperations = new HashSet<ResourceManager.InstanceOperation>();

		internal DelegateList<float> m_UpdateCallbacks = DelegateList<float>.CreateWithGlobalCache();

		private List<IAsyncOperation> m_DeferredCompleteCallbacks = new List<IAsyncOperation>();

		private bool m_InsideExecuteDeferredCallbacksMethod;

		private List<ResourceManager.DeferredCallbackRegisterRequest> m_DeferredCallbacksToRegister;

		private Action<IAsyncOperation> m_ReleaseOpNonCached;

		private Action<IAsyncOperation> m_ReleaseOpCached;

		private Action<IAsyncOperation> m_ReleaseInstanceOp;

		private static int s_GroupOperationTypeHash = typeof(GroupOperation).GetHashCode();

		private static int s_InstanceOperationTypeHash = typeof(ResourceManager.InstanceOperation).GetHashCode();

		private bool m_RegisteredForCallbacks;

		private Dictionary<Type, Type> m_ProviderOperationTypeCache = new Dictionary<Type, Type>();

		public enum DiagnosticEventType
		{
			AsyncOperationFail,
			AsyncOperationCreate,
			AsyncOperationPercentComplete,
			AsyncOperationComplete,
			AsyncOperationReferenceCount,
			AsyncOperationDestroy
		}

		private struct DeferredCallbackRegisterRequest
		{
			internal IAsyncOperation operation;

			internal bool incrementRefCount;
		}

		private class CompletedOperation<TObject> : AsyncOperationBase<TObject>
		{
			public void Init(TObject result, bool success, string errorMsg, bool releaseDependenciesOnFailure = true)
			{
				this.Init(result, success, (!string.IsNullOrEmpty(errorMsg)) ? new Exception(errorMsg) : null, releaseDependenciesOnFailure);
			}

			public void Init(TObject result, bool success, Exception exception, bool releaseDependenciesOnFailure = true)
			{
				base.Result = result;
				this.m_Success = success;
				this.m_Exception = exception;
				this.m_ReleaseDependenciesOnFailure = releaseDependenciesOnFailure;
			}

			protected override string DebugName
			{
				get
				{
					return "CompletedOperation";
				}
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
				base.Complete(base.Result, this.m_Success, this.m_Exception, this.m_ReleaseDependenciesOnFailure);
			}

			private bool m_Success;

			private Exception m_Exception;

			private bool m_ReleaseDependenciesOnFailure;
		}

		internal class InstanceOperation : AsyncOperationBase<GameObject>
		{
			public void Init(ResourceManager rm, IInstanceProvider instanceProvider, InstantiationParameters instantiationParams, AsyncOperationHandle<GameObject> dependency)
			{
				this.m_RM = rm;
				this.m_dependency = dependency;
				this.m_instanceProvider = instanceProvider;
				this.m_instantiationParams = instantiationParams;
				this.m_scene = default(Scene);
			}

			internal override DownloadStatus GetDownloadStatus(HashSet<object> visited)
			{
				if (!this.m_dependency.IsValid())
				{
					return new DownloadStatus
					{
						IsDone = base.IsDone
					};
				}
				return this.m_dependency.InternalGetDownloadStatus(visited);
			}

			public override void GetDependencies(List<AsyncOperationHandle> deps)
			{
				deps.Add(this.m_dependency);
			}

			protected override string DebugName
			{
				get
				{
					if (this.m_instanceProvider == null)
					{
						return "Instance<Invalid>";
					}
					return string.Format("Instance<{0}>({1}", this.m_instanceProvider.GetType().Name, this.m_dependency.IsValid() ? this.m_dependency.DebugName : "Invalid");
				}
			}

			public Scene InstanceScene()
			{
				return this.m_scene;
			}

			protected override void Destroy()
			{
				this.m_instanceProvider.ReleaseInstance(this.m_RM, this.m_instance);
			}

			protected override float Progress
			{
				get
				{
					return this.m_dependency.PercentComplete;
				}
			}

			protected override bool InvokeWaitForCompletion()
			{
				if (this.m_dependency.IsValid() && !this.m_dependency.IsDone)
				{
					this.m_dependency.WaitForCompletion();
				}
				ResourceManager rm = this.m_RM;
				if (rm != null)
				{
					rm.Update(Time.unscaledDeltaTime);
				}
				if (this.m_instance == null && !this.HasExecuted)
				{
					base.InvokeExecute();
				}
				return base.IsDone;
			}

			protected override void Execute()
			{
				Exception operationException = this.m_dependency.OperationException;
				if (this.m_dependency.Status == AsyncOperationStatus.Succeeded)
				{
					this.m_instance = this.m_instanceProvider.ProvideInstance(this.m_RM, this.m_dependency, this.m_instantiationParams);
					if (this.m_instance != null)
					{
						this.m_scene = this.m_instance.scene;
					}
					base.Complete(this.m_instance, true, null);
					return;
				}
				base.Complete(this.m_instance, false, string.Format("Dependency operation failed with {0}.", operationException));
			}

			private AsyncOperationHandle<GameObject> m_dependency;

			private InstantiationParameters m_instantiationParams;

			private IInstanceProvider m_instanceProvider;

			private GameObject m_instance;

			private Scene m_scene;
		}
	}
}
