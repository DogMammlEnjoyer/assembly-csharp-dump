using System;
using System.Collections.Generic;
using UnityEngine.Localization.Settings;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace UnityEngine.Localization.Operations
{
	internal class InitializationOperation : WaitForCurrentOperationAsyncOperationBase<LocalizationSettings>
	{
		protected override float Progress
		{
			get
			{
				if (base.CurrentOperation.IsValid())
				{
					return ((float)(3 - this.m_RemainingSteps) + base.CurrentOperation.PercentComplete) / 4f;
				}
				return base.Progress;
			}
		}

		protected override string DebugName
		{
			get
			{
				return "Localization Settings Initialization";
			}
		}

		public InitializationOperation()
		{
			this.m_LoadLocalesCompletedAction = new Action<AsyncOperationHandle<Locale>>(this.LoadLocalesCompleted);
			this.m_FinishPreloadingTablesAction = delegate(AsyncOperationHandle _)
			{
				this.PreloadTablesCompleted();
			};
			this.m_LoadLocales = delegate(AsyncOperationHandle _)
			{
				this.LoadLocales();
			};
		}

		public void Init(LocalizationSettings settings)
		{
			this.m_Settings = settings;
			this.m_LoadDatabasesOperations.Clear();
			this.m_RemainingSteps = 3;
		}

		protected override void Execute()
		{
			InitializationOperation.UnloadBundlesOperation operation = GenericPool<InitializationOperation.UnloadBundlesOperation>.Get();
			this.m_UnloadBundlesOperationHandle = AddressablesInterface.ResourceManager.StartOperation<object>(operation, default(AsyncOperationHandle));
			if (!this.m_UnloadBundlesOperationHandle.IsDone)
			{
				base.CurrentOperation = this.m_UnloadBundlesOperationHandle;
				this.m_UnloadBundlesOperationHandle.Completed += this.m_LoadLocales;
				return;
			}
			this.LoadLocales();
		}

		private void LoadLocales()
		{
			AddressablesInterface.SafeRelease(this.m_UnloadBundlesOperationHandle);
			AsyncOperationHandle<Locale> selectedLocaleAsync = this.m_Settings.GetSelectedLocaleAsync();
			if (!selectedLocaleAsync.IsDone)
			{
				base.CurrentOperation = selectedLocaleAsync;
				selectedLocaleAsync.Completed += this.m_LoadLocalesCompletedAction;
				return;
			}
			this.LoadLocalesCompleted(selectedLocaleAsync);
		}

		private bool CheckOperationSucceeded(AsyncOperationHandle handle, string errorMessage)
		{
			if (handle.Status != AsyncOperationStatus.Succeeded)
			{
				bool success = false;
				Exception operationException = handle.OperationException;
				this.FinishInitializing(success, string.Format(errorMessage, (operationException != null) ? operationException.Message : null));
				return false;
			}
			return true;
		}

		private void LoadLocalesCompleted(AsyncOperationHandle<Locale> operationHandle)
		{
			if (this.CheckOperationSucceeded(operationHandle, "Failed to initialize localization, could not load the selected locale.\n{0}"))
			{
				this.PreloadTables();
			}
		}

		private void PreloadTables()
		{
			this.m_RemainingSteps--;
			IPreloadRequired assetDatabase = this.m_Settings.GetAssetDatabase();
			if (assetDatabase != null && !assetDatabase.PreloadOperation.IsDone)
			{
				this.m_LoadDatabasesOperations.Add(assetDatabase.PreloadOperation);
			}
			else
			{
				this.m_RemainingSteps--;
			}
			IPreloadRequired stringDatabase = this.m_Settings.GetStringDatabase();
			if (stringDatabase != null && !stringDatabase.PreloadOperation.IsDone)
			{
				this.m_LoadDatabasesOperations.Add(stringDatabase.PreloadOperation);
			}
			else
			{
				this.m_RemainingSteps--;
			}
			if (this.m_LoadDatabasesOperations.Count > 0)
			{
				this.m_PreloadDatabasesOperation = AddressablesInterface.CreateGroupOperation(this.m_LoadDatabasesOperations);
				base.CurrentOperation = this.m_PreloadDatabasesOperation;
				this.m_PreloadDatabasesOperation.CompletedTypeless += this.m_FinishPreloadingTablesAction;
				return;
			}
			this.PreloadTablesCompleted();
		}

		private void PreloadTablesCompleted()
		{
			IPreloadRequired assetDatabase = this.m_Settings.GetAssetDatabase();
			if (assetDatabase != null && !this.CheckOperationSucceeded(assetDatabase.PreloadOperation, "Failed to initialize localization, could not preload asset tables.\n{0}"))
			{
				return;
			}
			IPreloadRequired stringDatabase = this.m_Settings.GetStringDatabase();
			if (stringDatabase != null && !this.CheckOperationSucceeded(stringDatabase.PreloadOperation, "Failed to initialize localization, could not preload string tables.\n{0}"))
			{
				return;
			}
			this.FinishInitializing(true, null);
		}

		private void PostInitializeExtensions()
		{
			foreach (IStartupLocaleSelector startupLocaleSelector in this.m_Settings.GetStartupLocaleSelectors())
			{
				IInitialize initialize = startupLocaleSelector as IInitialize;
				if (initialize != null)
				{
					initialize.PostInitialization(this.m_Settings);
				}
			}
			IInitialize initialize2 = this.m_Settings.GetAvailableLocales() as IInitialize;
			if (initialize2 != null)
			{
				initialize2.PostInitialization(this.m_Settings);
			}
			IInitialize initialize3 = this.m_Settings.GetAssetDatabase() as IInitialize;
			if (initialize3 != null)
			{
				initialize3.PostInitialization(this.m_Settings);
			}
			IInitialize initialize4 = this.m_Settings.GetStringDatabase() as IInitialize;
			if (initialize4 == null)
			{
				return;
			}
			initialize4.PostInitialization(this.m_Settings);
		}

		private void FinishInitializing(AsyncOperationHandle op)
		{
			bool success = op.Status == AsyncOperationStatus.Succeeded;
			Exception operationException = op.OperationException;
			this.FinishInitializing(success, (operationException != null) ? operationException.Message : null);
		}

		private void FinishInitializing(bool success, string error)
		{
			AddressablesInterface.ReleaseAndReset<IList<AsyncOperationHandle>>(ref this.m_PreloadDatabasesOperation);
			this.PostInitializeExtensions();
			base.Complete(this.m_Settings, success, error);
		}

		protected override void Destroy()
		{
			base.Destroy();
			InitializationOperation.Pool.Release(this);
		}

		private AsyncOperationHandle m_UnloadBundlesOperationHandle;

		private readonly Action<AsyncOperationHandle> m_LoadLocales;

		internal const string k_LocaleError = "Failed to initialize localization, could not load the selected locale.\n{0}";

		internal const string k_PreloadAssetTablesError = "Failed to initialize localization, could not preload asset tables.\n{0}";

		internal const string k_PreloadStringTablesError = "Failed to initialize localization, could not preload string tables.\n{0}";

		private readonly Action<AsyncOperationHandle<Locale>> m_LoadLocalesCompletedAction;

		private readonly Action<AsyncOperationHandle> m_FinishPreloadingTablesAction;

		private LocalizationSettings m_Settings;

		private readonly List<AsyncOperationHandle> m_LoadDatabasesOperations = new List<AsyncOperationHandle>();

		private AsyncOperationHandle<IList<AsyncOperationHandle>> m_PreloadDatabasesOperation;

		private int m_RemainingSteps;

		private const int k_PreloadSteps = 3;

		public static readonly ObjectPool<InitializationOperation> Pool = new ObjectPool<InitializationOperation>(() => new InitializationOperation(), null, null, null, false, 10, 10000);

		private class UnloadBundlesOperation : AsyncOperationBase<object>
		{
			public UnloadBundlesOperation()
			{
				this.m_OperationCompleted = new Action<AsyncOperation>(this.OnOperationCompleted);
			}

			protected override void Execute()
			{
				if (AssetBundleProvider.UnloadingAssetBundleCount == 0)
				{
					base.Complete(null, true, null);
					return;
				}
				this.m_UnloadBundleOperations.Clear();
				foreach (AssetBundleUnloadOperation assetBundleUnloadOperation in AssetBundleProvider.UnloadingBundles.Values)
				{
					if (!assetBundleUnloadOperation.isDone)
					{
						this.m_UnloadBundleOperations.Add(assetBundleUnloadOperation);
						assetBundleUnloadOperation.completed += this.m_OperationCompleted;
					}
				}
			}

			private void OnOperationCompleted(AsyncOperation obj)
			{
				this.m_UnloadBundleOperations.Remove(obj);
				if (this.m_UnloadBundleOperations.Count == 0)
				{
					base.Complete(null, true, null);
				}
			}

			protected override bool InvokeWaitForCompletion()
			{
				AssetBundleProvider.WaitForAllUnloadingBundlesToComplete();
				return true;
			}

			protected override void Destroy()
			{
				GenericPool<InitializationOperation.UnloadBundlesOperation>.Release(this);
			}

			private readonly Action<AsyncOperation> m_OperationCompleted;

			private readonly List<AsyncOperation> m_UnloadBundleOperations = new List<AsyncOperation>();
		}
	}
}
