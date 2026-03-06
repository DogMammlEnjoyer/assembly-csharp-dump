using System;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UnityEngine.Localization.Operations
{
	internal class LoadAssetOperation<TObject> : WaitForCurrentOperationAsyncOperationBase<TObject> where TObject : Object
	{
		public LoadAssetOperation()
		{
			this.m_AssetLoadedAction = new Action<AsyncOperationHandle<TObject>>(this.AssetLoaded);
		}

		public void Init(AsyncOperationHandle<LocalizedDatabase<AssetTable, AssetTableEntry>.TableEntryResult> loadTableEntryOperation, bool autoRelease)
		{
			this.m_TableEntryOperation = loadTableEntryOperation;
			AddressablesInterface.Acquire(this.m_TableEntryOperation);
			this.m_AutoRelease = autoRelease;
		}

		protected override void Execute()
		{
			if (this.m_TableEntryOperation.Status != AsyncOperationStatus.Succeeded)
			{
				base.Complete(default(TObject), false, "Load Table Entry Operation Failed");
				AddressablesInterface.Release(this.m_TableEntryOperation);
				return;
			}
			if (this.m_TableEntryOperation.Result.Table == null || this.m_TableEntryOperation.Result.Entry == null)
			{
				this.CompleteAndRelease(default(TObject), true, null);
				return;
			}
			this.m_LoadAssetOperation = this.m_TableEntryOperation.Result.Table.GetAssetAsync<TObject>(this.m_TableEntryOperation.Result.Entry);
			AddressablesInterface.Acquire(this.m_LoadAssetOperation);
			if (this.m_LoadAssetOperation.IsDone)
			{
				this.AssetLoaded(this.m_LoadAssetOperation);
				return;
			}
			base.CurrentOperation = this.m_LoadAssetOperation;
			this.m_LoadAssetOperation.Completed += this.m_AssetLoadedAction;
		}

		private void AssetLoaded(AsyncOperationHandle<TObject> handle)
		{
			if (handle.Status != AsyncOperationStatus.Succeeded)
			{
				this.CompleteAndRelease(default(TObject), false, "GetAssetAsync failed to load the asset.");
				return;
			}
			this.CompleteAndRelease(handle.Result, true, null);
		}

		public void CompleteAndRelease(TObject result, bool success, string errorMsg)
		{
			base.Complete(result, success, errorMsg);
			AddressablesInterface.Release(this.m_TableEntryOperation);
			if (this.m_AutoRelease && LocalizationSettings.Instance.IsPlaying)
			{
				LocalizationBehaviour.ReleaseNextFrame(base.Handle);
			}
		}

		protected override void Destroy()
		{
			AddressablesInterface.ReleaseAndReset<TObject>(ref this.m_LoadAssetOperation);
			base.Destroy();
			LoadAssetOperation<TObject>.Pool.Release(this);
		}

		private readonly Action<AsyncOperationHandle<TObject>> m_AssetLoadedAction;

		private AsyncOperationHandle<LocalizedDatabase<AssetTable, AssetTableEntry>.TableEntryResult> m_TableEntryOperation;

		private AsyncOperationHandle<TObject> m_LoadAssetOperation;

		private bool m_AutoRelease;

		public static readonly ObjectPool<LoadAssetOperation<TObject>> Pool = new ObjectPool<LoadAssetOperation<TObject>>(() => new LoadAssetOperation<TObject>(), null, null, null, false, 10, 10000);
	}
}
