using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.AddressableAssets.ResourceProviders;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.Util;

namespace UnityEngine.AddressableAssets.Initialization
{
	internal class InitializationOperation : AsyncOperationBase<IResourceLocator>
	{
		public InitializationOperation(AddressablesImpl aa)
		{
			this.m_Addressables = aa;
		}

		protected override float Progress
		{
			get
			{
				if (this.m_rtdOp.IsValid())
				{
					return this.m_rtdOp.PercentComplete;
				}
				return 0f;
			}
		}

		protected override string DebugName
		{
			get
			{
				return "InitializationOperation";
			}
		}

		internal static AsyncOperationHandle<IResourceLocator> CreateInitializationOperation(AddressablesImpl aa, string playerSettingsLocation, string providerSuffix)
		{
			JsonAssetProvider item = new JsonAssetProvider();
			aa.ResourceManager.ResourceProviders.Add(item);
			TextDataProvider item2 = new TextDataProvider();
			aa.ResourceManager.ResourceProviders.Add(item2);
			aa.ResourceManager.ResourceProviders.Add(new ContentCatalogProvider(aa.ResourceManager));
			ResourceLocationBase location = new ResourceLocationBase("RuntimeData", playerSettingsLocation, typeof(JsonAssetProvider).FullName, typeof(ResourceManagerRuntimeData), Array.Empty<IResourceLocation>());
			InitializationOperation initializationOperation = new InitializationOperation(aa)
			{
				m_rtdOp = aa.ResourceManager.ProvideResource<ResourceManagerRuntimeData>(location),
				m_ProviderSuffix = providerSuffix,
				m_InitGroupOps = new InitalizationObjectsOperation()
			};
			initializationOperation.m_InitGroupOps.Init(initializationOperation.m_rtdOp, aa);
			AsyncOperationHandle<bool> obj = aa.ResourceManager.StartOperation<bool>(initializationOperation.m_InitGroupOps, initializationOperation.m_rtdOp);
			return aa.ResourceManager.StartOperation<IResourceLocator>(initializationOperation, obj);
		}

		protected override bool InvokeWaitForCompletion()
		{
			if (base.IsDone)
			{
				return true;
			}
			if (this.m_rtdOp.IsValid() && !this.m_rtdOp.IsDone)
			{
				this.m_rtdOp.WaitForCompletion();
			}
			ResourceManager rm = this.m_RM;
			if (rm != null)
			{
				rm.Update(Time.unscaledDeltaTime);
			}
			if (!this.HasExecuted)
			{
				base.InvokeExecute();
			}
			if (this.m_loadCatalogOp.IsValid() && !this.m_loadCatalogOp.IsDone)
			{
				this.m_loadCatalogOp.WaitForCompletion();
				ResourceManager rm2 = this.m_RM;
				if (rm2 != null)
				{
					rm2.Update(Time.unscaledDeltaTime);
				}
			}
			return this.m_rtdOp.IsDone && this.m_loadCatalogOp.IsDone;
		}

		protected override void Execute()
		{
			if (this.m_rtdOp.Result == null)
			{
				Addressables.LogWarningFormat("Addressables - Unable to load runtime data at location {0}.", new object[]
				{
					this.m_rtdOp
				});
				base.Complete(base.Result, false, string.Format("Addressables - Unable to load runtime data at location {0}.", this.m_rtdOp));
				return;
			}
			ResourceManagerRuntimeData result = this.m_rtdOp.Result;
			WebRequestQueue.SetMaxConcurrentRequests(result.MaxConcurrentWebRequests);
			this.m_Addressables.CatalogRequestsTimeout = result.CatalogRequestsTimeout;
			foreach (ResourceLocationData resourceLocationData in result.CatalogLocations)
			{
				if (resourceLocationData.Data != null)
				{
					ProviderLoadRequestOptions providerLoadRequestOptions = resourceLocationData.Data as ProviderLoadRequestOptions;
					if (providerLoadRequestOptions != null)
					{
						providerLoadRequestOptions.WebRequestTimeout = result.CatalogRequestsTimeout;
					}
				}
			}
			this.m_rtdOp.Release();
			if (result.CertificateHandlerType != null)
			{
				this.m_Addressables.ResourceManager.CertificateHandlerInstance = (Activator.CreateInstance(result.CertificateHandlerType) as CertificateHandler);
			}
			if (!result.LogResourceManagerExceptions)
			{
				ResourceManager.ExceptionHandler = null;
			}
			ContentCatalogProvider contentCatalogProvider = this.m_Addressables.ResourceManager.ResourceProviders.FirstOrDefault((IResourceProvider rp) => rp.GetType() == typeof(ContentCatalogProvider)) as ContentCatalogProvider;
			if (contentCatalogProvider != null)
			{
				contentCatalogProvider.DisableCatalogUpdateOnStart = result.DisableCatalogUpdateOnStartup;
				contentCatalogProvider.IsLocalCatalogInBundle = result.IsLocalCatalogInBundle;
			}
			ResourceLocationMap resourceLocationMap = new ResourceLocationMap("CatalogLocator", result.CatalogLocations);
			this.m_Addressables.AddResourceLocator(resourceLocationMap, null, null);
			IList<IResourceLocation> list;
			if (!resourceLocationMap.Locate("AddressablesMainContentCatalog", typeof(ContentCatalogData), out list))
			{
				Addressables.LogWarningFormat("Addressables - Unable to find any catalog locations in the runtime data.", Array.Empty<object>());
				this.m_Addressables.RemoveResourceLocator(resourceLocationMap);
				base.Complete(base.Result, false, "Addressables - Unable to find any catalog locations in the runtime data.");
				return;
			}
			IResourceLocation remoteHashLocation = null;
			if (list[0].Dependencies.Count == 3 && result.DisableCatalogUpdateOnStartup)
			{
				remoteHashLocation = list[0].Dependencies[0];
				list[0].Dependencies[0] = list[0].Dependencies[1];
			}
			this.m_loadCatalogOp = this.LoadContentCatalogInternal(list, 0, resourceLocationMap, remoteHashLocation);
		}

		private static void LoadProvider(AddressablesImpl addressables, ObjectInitializationData providerData, string providerSuffix)
		{
			int num = -1;
			string text = string.IsNullOrEmpty(providerSuffix) ? providerData.Id : (providerData.Id + providerSuffix);
			for (int i = 0; i < addressables.ResourceManager.ResourceProviders.Count; i++)
			{
				if (addressables.ResourceManager.ResourceProviders[i].ProviderId == text)
				{
					num = i;
					break;
				}
			}
			if (num >= 0 && string.IsNullOrEmpty(providerSuffix))
			{
				return;
			}
			IResourceProvider resourceProvider = providerData.CreateInstance<IResourceProvider>(text);
			if (resourceProvider == null)
			{
				Addressables.LogWarningFormat("Addressables - Unable to load resource provider from {0}.", new object[]
				{
					providerData
				});
				return;
			}
			if (num < 0 || !string.IsNullOrEmpty(providerSuffix))
			{
				addressables.ResourceManager.ResourceProviders.Add(resourceProvider);
				return;
			}
			addressables.ResourceManager.ResourceProviders[num] = resourceProvider;
		}

		private static AsyncOperationHandle<IResourceLocator> OnCatalogDataLoaded(AddressablesImpl addressables, AsyncOperationHandle<ContentCatalogData> op, string providerSuffix, IResourceLocation remoteHashLocation)
		{
			ContentCatalogData result = op.Result;
			if (result == null)
			{
				Exception exception = (op.OperationException != null) ? new Exception("Failed to load content catalog.", op.OperationException) : new Exception("Failed to load content catalog.");
				op.Release();
				return addressables.ResourceManager.CreateCompletedOperationWithException<IResourceLocator>(null, exception);
			}
			op.Release();
			if (result.ResourceProviderData != null)
			{
				foreach (ObjectInitializationData providerData in result.ResourceProviderData)
				{
					InitializationOperation.LoadProvider(addressables, providerData, providerSuffix);
				}
			}
			if (addressables.InstanceProvider == null)
			{
				IInstanceProvider instanceProvider = result.InstanceProviderData.CreateInstance<IInstanceProvider>(null);
				if (instanceProvider != null)
				{
					addressables.InstanceProvider = instanceProvider;
				}
			}
			if (addressables.SceneProvider == null)
			{
				ISceneProvider sceneProvider = result.SceneProviderData.CreateInstance<ISceneProvider>(null);
				if (sceneProvider != null)
				{
					addressables.SceneProvider = sceneProvider;
				}
			}
			if (remoteHashLocation != null)
			{
				result.location.Dependencies[0] = remoteHashLocation;
			}
			IResourceLocator resourceLocator = result.CreateCustomLocator(result.location.PrimaryKey, providerSuffix);
			addressables.AddResourceLocator(resourceLocator, result.LocalHash, result.location);
			addressables.AddResourceLocator(new DynamicResourceLocator(addressables), null, null);
			return addressables.ResourceManager.CreateCompletedOperation<IResourceLocator>(resourceLocator, string.Empty);
		}

		public static AsyncOperationHandle<IResourceLocator> LoadContentCatalog(AddressablesImpl addressables, IResourceLocation loc, string providerSuffix, IResourceLocation remoteHashLocation = null)
		{
			Type typeFromHandle = typeof(ProviderOperation<ContentCatalogData>);
			ProviderOperation<ContentCatalogData> providerOperation = addressables.ResourceManager.CreateOperation<ProviderOperation<ContentCatalogData>>(typeFromHandle, typeFromHandle.GetHashCode(), null, null);
			IResourceProvider provider = null;
			foreach (IResourceProvider resourceProvider in addressables.ResourceManager.ResourceProviders)
			{
				if (resourceProvider is ContentCatalogProvider)
				{
					provider = resourceProvider;
					break;
				}
			}
			AsyncOperationHandle<IList<AsyncOperationHandle>> asyncOperationHandle = addressables.ResourceManager.CreateGroupOperation<string>(loc.Dependencies, true);
			providerOperation.Init(addressables.ResourceManager, provider, loc, asyncOperationHandle, true);
			AsyncOperationHandle<ContentCatalogData> dependentOp = addressables.ResourceManager.StartOperation<ContentCatalogData>(providerOperation, asyncOperationHandle);
			asyncOperationHandle.Release();
			return addressables.ResourceManager.CreateChainOperation<IResourceLocator, ContentCatalogData>(dependentOp, (AsyncOperationHandle<ContentCatalogData> res) => InitializationOperation.OnCatalogDataLoaded(addressables, res, providerSuffix, remoteHashLocation));
		}

		public AsyncOperationHandle<IResourceLocator> LoadContentCatalog(IResourceLocation loc, string providerSuffix, IResourceLocation remoteHashLocation)
		{
			return InitializationOperation.LoadContentCatalog(this.m_Addressables, loc, providerSuffix, remoteHashLocation);
		}

		internal AsyncOperationHandle<IResourceLocator> LoadContentCatalogInternal(IList<IResourceLocation> catalogs, int index, ResourceLocationMap locMap, IResourceLocation remoteHashLocation)
		{
			AsyncOperationHandle<IResourceLocator> asyncOperationHandle = this.LoadContentCatalog(catalogs[index], this.m_ProviderSuffix, remoteHashLocation);
			if (asyncOperationHandle.IsDone)
			{
				this.LoadOpComplete(asyncOperationHandle, catalogs, locMap, index, remoteHashLocation);
			}
			else
			{
				asyncOperationHandle.Completed += delegate(AsyncOperationHandle<IResourceLocator> op)
				{
					this.LoadOpComplete(op, catalogs, locMap, index, remoteHashLocation);
				};
			}
			return asyncOperationHandle;
		}

		private void LoadOpComplete(AsyncOperationHandle<IResourceLocator> op, IList<IResourceLocation> catalogs, ResourceLocationMap locMap, int index, IResourceLocation remoteHashLocation)
		{
			if (op.Result != null)
			{
				this.m_Addressables.RemoveResourceLocator(locMap);
				base.Result = op.Result;
				base.Complete(base.Result, true, string.Empty);
				op.Release();
				return;
			}
			if (index + 1 >= catalogs.Count)
			{
				Addressables.LogWarningFormat("Addressables - initialization failed.", new object[]
				{
					op
				});
				this.m_Addressables.RemoveResourceLocator(locMap);
				if (op.OperationException != null)
				{
					base.Complete(base.Result, false, op.OperationException, true);
				}
				else
				{
					base.Complete(base.Result, false, "LoadContentCatalogInternal");
				}
				op.Release();
				return;
			}
			this.m_loadCatalogOp = this.LoadContentCatalogInternal(catalogs, index + 1, locMap, remoteHashLocation);
			op.Release();
		}

		private AsyncOperationHandle<ResourceManagerRuntimeData> m_rtdOp;

		private AsyncOperationHandle<IResourceLocator> m_loadCatalogOp;

		private string m_ProviderSuffix;

		private AddressablesImpl m_Addressables;

		private InitalizationObjectsOperation m_InitGroupOps;
	}
}
