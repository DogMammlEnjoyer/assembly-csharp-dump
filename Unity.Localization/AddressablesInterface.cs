using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.Localization.Operations;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace UnityEngine.Localization
{
	internal class AddressablesInterface
	{
		public static AddressablesInterface Instance
		{
			get
			{
				if (AddressablesInterface.s_Instance == null)
				{
					AddressablesInterface.s_Instance = new AddressablesInterface();
				}
				return AddressablesInterface.s_Instance;
			}
			set
			{
				AddressablesInterface.s_Instance = value;
			}
		}

		public static ResourceManager ResourceManager
		{
			get
			{
				return Addressables.ResourceManager;
			}
		}

		public static void Acquire(AsyncOperationHandle handle)
		{
			AddressablesInterface.Instance.AcquireInternal(handle);
		}

		public static void Release(AsyncOperationHandle handle)
		{
			AddressablesInterface.Instance.ReleaseInternal(handle);
		}

		public static void SafeRelease(AsyncOperationHandle handle)
		{
			if (handle.IsValid())
			{
				AddressablesInterface.Instance.ReleaseInternal(handle);
			}
		}

		public static void ReleaseAndReset<TObject>(ref AsyncOperationHandle<TObject> handle)
		{
			if (handle.IsValid())
			{
				AddressablesInterface.Instance.ReleaseInternal(handle);
				handle = default(AsyncOperationHandle<TObject>);
			}
		}

		public static AsyncOperationHandle<IList<AsyncOperationHandle>> CreateGroupOperation(List<AsyncOperationHandle> asyncOperations)
		{
			foreach (AsyncOperationHandle handle in asyncOperations)
			{
				AddressablesInterface.Acquire(handle);
			}
			LocalizationGroupOperation localizationGroupOperation = LocalizationGroupOperation.Pool.Get();
			localizationGroupOperation.Init(asyncOperations, true, false);
			return AddressablesInterface.ResourceManager.StartOperation<IList<AsyncOperationHandle>>(localizationGroupOperation, default(AsyncOperationHandle));
		}

		public static AsyncOperationHandle<IList<IResourceLocation>> LoadResourceLocationsWithLabelsAsync(IEnumerable labels, Addressables.MergeMode mode, Type type = null)
		{
			return AddressablesInterface.Instance.LoadResourceLocationsWithLabelsAsyncInternal(labels, mode, type);
		}

		public static AsyncOperationHandle<IList<IResourceLocation>> LoadTableLocationsAsync(string tableName, LocaleIdentifier id, Type type)
		{
			return AddressablesInterface.Instance.LoadTableLocationsAsyncInternal(tableName, id, type);
		}

		public static AsyncOperationHandle<IList<TObject>> LoadAssetsFromLocations<TObject>(IList<IResourceLocation> locations, Action<TObject> callback)
		{
			return AddressablesInterface.Instance.LoadAssetsFromLocationsInternal<TObject>(locations, callback);
		}

		public static AsyncOperationHandle<TObject> LoadAssetFromGUID<TObject>(string guid) where TObject : class
		{
			return AddressablesInterface.Instance.LoadAssetFromGUIDInternal<TObject>(guid);
		}

		public static AsyncOperationHandle<TObject> LoadAssetFromName<TObject>(string name) where TObject : class
		{
			return AddressablesInterface.Instance.LoadAssetFromNameInternal<TObject>(name);
		}

		public static AsyncOperationHandle<TObject> LoadTableFromLocation<TObject>(IResourceLocation location) where TObject : class
		{
			return AddressablesInterface.Instance.LoadTableFromLocationInternal<TObject>(location);
		}

		public static AsyncOperationHandle<IList<TObject>> LoadAssetsWithLabel<TObject>(string label, Action<TObject> callback) where TObject : class
		{
			return AddressablesInterface.Instance.LoadAssetsWithLabelInternal<TObject>(label, callback);
		}

		internal virtual void AcquireInternal(AsyncOperationHandle handle)
		{
			AddressablesInterface.ResourceManager.Acquire(handle);
		}

		internal virtual void ReleaseInternal(AsyncOperationHandle handle)
		{
			Addressables.Release(handle);
		}

		internal virtual AsyncOperationHandle<IList<IResourceLocation>> LoadResourceLocationsWithLabelsAsyncInternal(IEnumerable labels, Addressables.MergeMode mode, Type type = null)
		{
			return Addressables.LoadResourceLocationsAsync(labels, mode, type);
		}

		internal virtual AsyncOperationHandle<IList<IResourceLocation>> LoadTableLocationsAsyncInternal(string tableName, LocaleIdentifier id, Type type)
		{
			return Addressables.LoadResourceLocationsAsync(AddressHelper.GetTableAddress(tableName, id), type);
		}

		internal virtual AsyncOperationHandle<IList<TObject>> LoadAssetsFromLocationsInternal<TObject>(IList<IResourceLocation> locations, Action<TObject> callback)
		{
			return Addressables.LoadAssetsAsync<TObject>(locations, callback);
		}

		internal virtual AsyncOperationHandle<TObject> LoadAssetFromGUIDInternal<TObject>(string guid) where TObject : class
		{
			return Addressables.LoadAssetAsync<TObject>(guid);
		}

		internal virtual AsyncOperationHandle<TObject> LoadAssetFromNameInternal<TObject>(string name) where TObject : class
		{
			return Addressables.LoadAssetAsync<TObject>(name);
		}

		internal virtual AsyncOperationHandle<TObject> LoadTableFromLocationInternal<TObject>(IResourceLocation location) where TObject : class
		{
			return Addressables.LoadAssetAsync<TObject>(location);
		}

		internal virtual AsyncOperationHandle<IList<TObject>> LoadAssetsWithLabelInternal<TObject>(string label, Action<TObject> callback) where TObject : class
		{
			return Addressables.LoadAssetsAsync<TObject>(label, callback);
		}

		internal virtual AsyncOperationHandle<IResourceLocator> InitializeAddressablesAsync()
		{
			return Addressables.InitializeAsync();
		}

		private static AddressablesInterface s_Instance;
	}
}
