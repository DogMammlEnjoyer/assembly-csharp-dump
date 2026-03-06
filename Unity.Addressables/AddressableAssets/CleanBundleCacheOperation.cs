using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.Util;

namespace UnityEngine.AddressableAssets
{
	internal class CleanBundleCacheOperation : AsyncOperationBase<bool>, IUpdateReceiver
	{
		public CleanBundleCacheOperation(AddressablesImpl aa, bool forceSingleThreading)
		{
			this.m_Addressables = aa;
			this.m_UseMultiThreading = (!forceSingleThreading && PlatformUtilities.PlatformUsesMultiThreading(Application.platform));
		}

		public AsyncOperationHandle<bool> Start(AsyncOperationHandle<IList<AsyncOperationHandle>> depOp)
		{
			this.m_DepOp = depOp.Acquire();
			return this.m_Addressables.ResourceManager.StartOperation<bool>(this, this.m_DepOp);
		}

		public void CompleteInternal(bool result, bool success, string errorMsg)
		{
			this.m_DepOp.Release();
			base.Complete(result, success, errorMsg);
		}

		protected override bool InvokeWaitForCompletion()
		{
			if (!this.m_DepOp.IsDone)
			{
				this.m_DepOp.WaitForCompletion();
			}
			if (!this.HasExecuted)
			{
				base.InvokeExecute();
			}
			if (this.m_EnumerationThread != null)
			{
				this.m_EnumerationThread.Join();
				this.RemoveCacheEntries();
			}
			return base.IsDone;
		}

		protected override void Destroy()
		{
			if (this.m_DepOp.IsValid())
			{
				this.m_DepOp.Release();
			}
		}

		public override void GetDependencies(List<AsyncOperationHandle> dependencies)
		{
			dependencies.Add(this.m_DepOp);
		}

		protected override void Execute()
		{
			if (this.m_DepOp.Status == AsyncOperationStatus.Failed)
			{
				this.CompleteInternal(false, false, "Could not clean cache because a dependent catalog operation failed.");
				return;
			}
			HashSet<string> cacheDirsInUse = this.GetCacheDirsInUse(this.m_DepOp.Result);
			if (!Caching.ready)
			{
				this.CompleteInternal(false, false, "Cache is not ready to be accessed.");
			}
			this.m_BaseCachePath = Caching.currentCacheForWriting.path;
			if (this.m_UseMultiThreading)
			{
				this.m_EnumerationThread = new Thread(new ParameterizedThreadStart(this.DetermineCacheDirsNotInUse));
				this.m_EnumerationThread.Start(cacheDirsInUse);
				return;
			}
			this.DetermineCacheDirsNotInUse(cacheDirsInUse);
			this.RemoveCacheEntries();
		}

		void IUpdateReceiver.Update(float unscaledDeltaTime)
		{
			if (this.m_UseMultiThreading && !this.m_EnumerationThread.IsAlive)
			{
				this.m_EnumerationThread = null;
				this.RemoveCacheEntries();
			}
		}

		private void RemoveCacheEntries()
		{
			foreach (string path in this.m_CacheDirsForRemoval)
			{
				Caching.ClearAllCachedVersions(Path.GetFileName(path));
			}
			this.CompleteInternal(true, true, null);
		}

		private void DetermineCacheDirsNotInUse(object data)
		{
			this.DetermineCacheDirsNotInUse((HashSet<string>)data);
		}

		private void DetermineCacheDirsNotInUse(HashSet<string> cacheDirsInUse)
		{
			this.m_CacheDirsForRemoval = new List<string>();
			if (Directory.Exists(this.m_BaseCachePath))
			{
				foreach (string item in Directory.EnumerateDirectories(this.m_BaseCachePath, "*", SearchOption.TopDirectoryOnly))
				{
					if (!cacheDirsInUse.Contains(item))
					{
						this.m_CacheDirsForRemoval.Add(item);
					}
				}
			}
		}

		private HashSet<string> GetCacheDirsInUse(IList<AsyncOperationHandle> catalogOps)
		{
			HashSet<string> hashSet = new HashSet<string>();
			for (int i = 0; i < catalogOps.Count; i++)
			{
				IResourceLocator resourceLocator = catalogOps[i].Result as IResourceLocator;
				if (resourceLocator == null)
				{
					ContentCatalogData contentCatalogData = catalogOps[i].Result as ContentCatalogData;
					if (contentCatalogData == null)
					{
						return hashSet;
					}
					resourceLocator = contentCatalogData.CreateCustomLocator(contentCatalogData.location.PrimaryKey, null);
				}
				foreach (IResourceLocation resourceLocation in resourceLocator.AllLocations)
				{
					AssetBundleRequestOptions assetBundleRequestOptions = resourceLocation.Data as AssetBundleRequestOptions;
					if (assetBundleRequestOptions != null)
					{
						AssetBundleResource.LoadType loadType;
						string text;
						AssetBundleResource.GetLoadInfo(resourceLocation, this.m_Addressables.ResourceManager, out loadType, out text);
						if (loadType == AssetBundleResource.LoadType.Web)
						{
							string item = Path.Combine(Caching.currentCacheForWriting.path, assetBundleRequestOptions.BundleName);
							hashSet.Add(item);
						}
					}
				}
			}
			return hashSet;
		}

		private AddressablesImpl m_Addressables;

		private AsyncOperationHandle<IList<AsyncOperationHandle>> m_DepOp;

		private List<string> m_CacheDirsForRemoval;

		private Thread m_EnumerationThread;

		private string m_BaseCachePath;

		private bool m_UseMultiThreading;
	}
}
