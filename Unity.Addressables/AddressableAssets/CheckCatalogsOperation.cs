using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AddressableAssets.ResourceProviders;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace UnityEngine.AddressableAssets
{
	internal class CheckCatalogsOperation : AsyncOperationBase<List<string>>
	{
		public CheckCatalogsOperation(AddressablesImpl aa)
		{
			this.m_Addressables = aa;
		}

		public AsyncOperationHandle<List<string>> Start(List<ResourceLocatorInfo> locatorInfos)
		{
			this.m_LocatorInfos = new List<ResourceLocatorInfo>(locatorInfos.Count);
			this.m_LocalHashes = new List<string>(locatorInfos.Count);
			List<IResourceLocation> list = new List<IResourceLocation>(locatorInfos.Count);
			foreach (ResourceLocatorInfo resourceLocatorInfo in locatorInfos)
			{
				if (resourceLocatorInfo.CanUpdateContent)
				{
					list.Add(resourceLocatorInfo.HashLocation);
					this.m_LocalHashes.Add(resourceLocatorInfo.LocalHash);
					this.m_LocatorInfos.Add(resourceLocatorInfo);
				}
			}
			ContentCatalogProvider contentCatalogProvider = this.m_Addressables.ResourceManager.ResourceProviders.FirstOrDefault((IResourceProvider rp) => rp.GetType() == typeof(ContentCatalogProvider)) as ContentCatalogProvider;
			if (contentCatalogProvider != null)
			{
				contentCatalogProvider.DisableCatalogUpdateOnStart = false;
			}
			this.m_DepOp = this.m_Addressables.ResourceManager.CreateGroupOperation<string>(list);
			return this.m_Addressables.ResourceManager.StartOperation<List<string>>(this, this.m_DepOp);
		}

		protected override bool InvokeWaitForCompletion()
		{
			if (base.IsDone)
			{
				return true;
			}
			if (this.m_DepOp.IsValid() && !this.m_DepOp.IsDone)
			{
				this.m_DepOp.WaitForCompletion();
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
			ResourceManager rm2 = this.m_RM;
			if (rm2 != null)
			{
				rm2.Update(Time.unscaledDeltaTime);
			}
			return base.IsDone;
		}

		protected override void Destroy()
		{
			this.m_DepOp.Release();
		}

		public override void GetDependencies(List<AsyncOperationHandle> dependencies)
		{
			dependencies.Add(this.m_DepOp);
		}

		internal static List<string> ProcessDependentOpResults(IList<AsyncOperationHandle> results, List<ResourceLocatorInfo> locatorInfos, List<string> localHashes, out string errorString, out bool success)
		{
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			for (int i = 0; i < results.Count; i++)
			{
				AsyncOperationHandle asyncOperationHandle = results[i];
				string text = asyncOperationHandle.Result as string;
				if (!string.IsNullOrEmpty(text) && text != localHashes[i])
				{
					list.Add(locatorInfos[i].Locator.LocatorId);
					locatorInfos[i].ContentUpdateAvailable = true;
				}
				else if (asyncOperationHandle.OperationException != null)
				{
					list.Add(null);
					locatorInfos[i].ContentUpdateAvailable = false;
					list2.Add(asyncOperationHandle.OperationException.Message);
				}
			}
			errorString = null;
			if (list2.Count > 0)
			{
				if (list2.Count == list.Count)
				{
					list = null;
					errorString = "CheckCatalogsOperation failed with the following errors: ";
				}
				else
				{
					errorString = "Partial success in CheckCatalogsOperation with the following errors: ";
				}
				foreach (string str in list2)
				{
					errorString = errorString + "\n" + str;
				}
			}
			success = (list2.Count == 0);
			return list;
		}

		protected override void Execute()
		{
			string errorMsg;
			bool success;
			List<string> result = CheckCatalogsOperation.ProcessDependentOpResults(this.m_DepOp.Result, this.m_LocatorInfos, this.m_LocalHashes, out errorMsg, out success);
			base.Complete(result, success, errorMsg);
		}

		private AddressablesImpl m_Addressables;

		private List<string> m_LocalHashes;

		private List<ResourceLocatorInfo> m_LocatorInfos;

		private AsyncOperationHandle<IList<AsyncOperationHandle>> m_DepOp;
	}
}
