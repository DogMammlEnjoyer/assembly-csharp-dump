using System;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UnityEngine.Localization.Operations
{
	internal class LoadSubAssetOperation<TObject> : WaitForCurrentOperationAsyncOperationBase<TObject> where TObject : Object
	{
		public LoadSubAssetOperation()
		{
			this.m_AssetLoadedAction = new Action<AsyncOperationHandle<TObject>>(this.AssetLoaded);
		}

		public void Init(AsyncOperationHandle<Object[]> preloadOperations, string address, bool isSubAsset, string subAssetName)
		{
			base.Dependency = preloadOperations;
			this.m_PreloadOperations = preloadOperations;
			if (this.m_PreloadOperations.IsValid())
			{
				AddressablesInterface.Acquire(this.m_PreloadOperations);
			}
			this.m_Address = address;
			this.m_IsSubAsset = isSubAsset;
			this.m_SubAssetName = subAssetName;
		}

		protected override void Execute()
		{
			if (this.m_PreloadOperations.IsValid())
			{
				if (this.m_PreloadOperations.Status != AsyncOperationStatus.Succeeded)
				{
					base.Complete(default(TObject), false, this.m_PreloadOperations.OperationException.Message);
					return;
				}
				foreach (Object @object in this.m_PreloadOperations.Result)
				{
					TObject tobject = @object as TObject;
					if (tobject != null && (!this.m_IsSubAsset || !(this.m_SubAssetName != @object.name)))
					{
						base.Complete(tobject, true, null);
						return;
					}
				}
			}
			this.m_AssetOperation = AddressablesInterface.LoadAssetFromGUID<TObject>(this.m_Address);
			if (this.m_AssetOperation.IsDone)
			{
				this.AssetLoaded(this.m_AssetOperation);
				return;
			}
			base.CurrentOperation = this.m_AssetOperation;
			this.m_AssetOperation.Completed += this.m_AssetLoadedAction;
		}

		private void AssetLoaded(AsyncOperationHandle<TObject> handle)
		{
			if (handle.Status != AsyncOperationStatus.Succeeded)
			{
				base.Complete(default(TObject), false, string.Format("Failed to load sub-asset {0} from the address {1}.", this.m_SubAssetName, this.m_Address));
				return;
			}
			base.Complete(handle.Result, true, null);
		}

		protected override void Destroy()
		{
			AddressablesInterface.ReleaseAndReset<Object[]>(ref this.m_PreloadOperations);
			AddressablesInterface.ReleaseAndReset<TObject>(ref this.m_AssetOperation);
			base.Destroy();
			LoadSubAssetOperation<TObject>.Pool.Release(this);
		}

		private readonly Action<AsyncOperationHandle<TObject>> m_AssetLoadedAction;

		private AsyncOperationHandle<TObject> m_AssetOperation;

		private AsyncOperationHandle<Object[]> m_PreloadOperations;

		private string m_Address;

		private bool m_IsSubAsset;

		private string m_SubAssetName;

		public static readonly ObjectPool<LoadSubAssetOperation<TObject>> Pool = new ObjectPool<LoadSubAssetOperation<TObject>>(() => new LoadSubAssetOperation<TObject>(), null, null, null, false, 10, 10000);
	}
}
