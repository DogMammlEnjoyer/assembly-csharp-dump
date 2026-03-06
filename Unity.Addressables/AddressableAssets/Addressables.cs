using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.Util;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;

namespace UnityEngine.AddressableAssets
{
	public static class Addressables
	{
		private static AddressablesImpl m_Addressables
		{
			get
			{
				return Addressables.m_AddressablesInstance;
			}
		}

		public static string Version
		{
			get
			{
				return "";
			}
		}

		public static ResourceManager ResourceManager
		{
			get
			{
				return Addressables.m_Addressables.ResourceManager;
			}
		}

		internal static AddressablesImpl Instance
		{
			get
			{
				return Addressables.m_Addressables;
			}
		}

		public static IInstanceProvider InstanceProvider
		{
			get
			{
				return Addressables.m_Addressables.InstanceProvider;
			}
		}

		public static string ResolveInternalId(string id)
		{
			return Addressables.m_Addressables.ResolveInternalId(id);
		}

		public static Func<IResourceLocation, string> InternalIdTransformFunc
		{
			get
			{
				return Addressables.m_Addressables.InternalIdTransformFunc;
			}
			set
			{
				Addressables.m_Addressables.InternalIdTransformFunc = value;
			}
		}

		public static Action<UnityWebRequest> WebRequestOverride
		{
			get
			{
				return Addressables.m_Addressables.WebRequestOverride;
			}
			set
			{
				Addressables.m_Addressables.WebRequestOverride = value;
			}
		}

		public static string StreamingAssetsSubFolder
		{
			get
			{
				return Addressables.m_Addressables.StreamingAssetsSubFolder;
			}
		}

		public static string BuildPath
		{
			get
			{
				return Addressables.m_Addressables.BuildPath;
			}
		}

		public static string PlayerBuildDataPath
		{
			get
			{
				return Addressables.m_Addressables.PlayerBuildDataPath;
			}
		}

		[Preserve]
		public static string RuntimePath
		{
			get
			{
				return Addressables.m_Addressables.RuntimePath;
			}
		}

		public static IEnumerable<IResourceLocator> ResourceLocators
		{
			get
			{
				return Addressables.m_Addressables.ResourceLocators;
			}
		}

		[Conditional("ADDRESSABLES_LOG_ALL")]
		internal static void InternalSafeSerializationLog(string msg, LogType logType = LogType.Log)
		{
			if (Addressables.m_AddressablesInstance == null)
			{
				return;
			}
			switch (logType)
			{
			case LogType.Error:
				Addressables.m_AddressablesInstance.LogError(msg);
				return;
			case LogType.Assert:
				break;
			case LogType.Warning:
				Addressables.m_AddressablesInstance.LogWarning(msg);
				return;
			case LogType.Log:
				Addressables.m_AddressablesInstance.Log(msg);
				break;
			default:
				return;
			}
		}

		[Conditional("ADDRESSABLES_LOG_ALL")]
		internal static void InternalSafeSerializationLogFormat(string format, LogType logType = LogType.Log, params object[] args)
		{
			if (Addressables.m_AddressablesInstance == null)
			{
				return;
			}
			switch (logType)
			{
			case LogType.Error:
				Addressables.m_AddressablesInstance.LogErrorFormat(format, args);
				return;
			case LogType.Assert:
				break;
			case LogType.Warning:
				Addressables.m_AddressablesInstance.LogWarningFormat(format, args);
				return;
			case LogType.Log:
				Addressables.m_AddressablesInstance.LogFormat(format, args);
				break;
			default:
				return;
			}
		}

		[Conditional("ADDRESSABLES_LOG_ALL")]
		public static void Log(string msg)
		{
			Addressables.m_Addressables.Log(msg);
		}

		[Conditional("ADDRESSABLES_LOG_ALL")]
		public static void LogFormat(string format, params object[] args)
		{
			Addressables.m_Addressables.LogFormat(format, args);
		}

		public static void LogWarning(string msg)
		{
			Addressables.m_Addressables.LogWarning(msg);
		}

		public static void LogWarningFormat(string format, params object[] args)
		{
			Addressables.m_Addressables.LogWarningFormat(format, args);
		}

		public static void LogError(string msg)
		{
			Addressables.m_Addressables.LogError(msg);
		}

		public static void LogException(AsyncOperationHandle op, Exception ex)
		{
			Addressables.m_Addressables.LogException(op, ex);
		}

		public static void LogException(Exception ex)
		{
			Addressables.m_Addressables.LogException(ex);
		}

		public static void LogErrorFormat(string format, params object[] args)
		{
			Addressables.m_Addressables.LogErrorFormat(format, args);
		}

		public static AsyncOperationHandle<IResourceLocator> InitializeAsync()
		{
			return Addressables.m_Addressables.InitializeAsync();
		}

		public static AsyncOperationHandle<IResourceLocator> InitializeAsync(bool autoReleaseHandle)
		{
			return Addressables.m_Addressables.InitializeAsync(autoReleaseHandle);
		}

		public static AsyncOperationHandle<IResourceLocator> LoadContentCatalogAsync(string catalogPath, string providerSuffix = null)
		{
			return Addressables.m_Addressables.LoadContentCatalogAsync(catalogPath, false, providerSuffix);
		}

		public static AsyncOperationHandle<IResourceLocator> LoadContentCatalogAsync(string catalogPath, bool autoReleaseHandle, string providerSuffix = null)
		{
			return Addressables.m_Addressables.LoadContentCatalogAsync(catalogPath, autoReleaseHandle, providerSuffix);
		}

		public static AsyncOperationHandle<TObject> LoadAssetAsync<TObject>(IResourceLocation location)
		{
			return Addressables.m_Addressables.LoadAssetAsync<TObject>(location);
		}

		public static AsyncOperationHandle<TObject> LoadAssetAsync<TObject>(object key)
		{
			return Addressables.m_Addressables.LoadAssetAsync<TObject>(key);
		}

		public static AsyncOperationHandle<IList<IResourceLocation>> LoadResourceLocationsAsync(IEnumerable keys, Addressables.MergeMode mode, Type type = null)
		{
			return Addressables.m_Addressables.LoadResourceLocationsAsync(keys, mode, type);
		}

		public static AsyncOperationHandle<IList<IResourceLocation>> LoadResourceLocationsAsync(object key, Type type = null)
		{
			return Addressables.m_Addressables.LoadResourceLocationsAsync(key, type);
		}

		public static AsyncOperationHandle<IList<TObject>> LoadAssetsAsync<TObject>(IList<IResourceLocation> locations, Action<TObject> callback)
		{
			return Addressables.m_Addressables.LoadAssetsAsync<TObject>(locations, callback, true);
		}

		public static AsyncOperationHandle<IList<TObject>> LoadAssetsAsync<TObject>(IList<IResourceLocation> locations, Action<TObject> callback, bool releaseDependenciesOnFailure)
		{
			return Addressables.m_Addressables.LoadAssetsAsync<TObject>(locations, callback, releaseDependenciesOnFailure);
		}

		public static AsyncOperationHandle<IList<TObject>> LoadAssetsAsync<TObject>(IEnumerable keys, Action<TObject> callback, Addressables.MergeMode mode)
		{
			return Addressables.m_Addressables.LoadAssetsAsync<TObject>(keys, callback, mode, true);
		}

		public static AsyncOperationHandle<IList<TObject>> LoadAssetsAsync<TObject>(string key, Action<TObject> callback = null)
		{
			return Addressables.m_Addressables.LoadAssetsAsync<TObject>(new List<string>
			{
				key
			}, callback, Addressables.MergeMode.None, true);
		}

		public static AsyncOperationHandle<IList<TObject>> LoadAssetsAsync<TObject>(IEnumerable keys, Action<TObject> callback, Addressables.MergeMode mode, bool releaseDependenciesOnFailure)
		{
			return Addressables.m_Addressables.LoadAssetsAsync<TObject>(keys, callback, mode, releaseDependenciesOnFailure);
		}

		public static AsyncOperationHandle<IList<TObject>> LoadAssetsAsync<TObject>(string key, bool releaseDependenciesOnFailure, Action<TObject> callback = null)
		{
			return Addressables.m_Addressables.LoadAssetsAsync<TObject>(new List<string>
			{
				key
			}, callback, Addressables.MergeMode.None, releaseDependenciesOnFailure);
		}

		public static AsyncOperationHandle<IList<TObject>> LoadAssetsAsync<TObject>(object key, Action<TObject> callback)
		{
			return Addressables.m_Addressables.LoadAssetsAsync<TObject>(key, callback, true);
		}

		public static AsyncOperationHandle<IList<TObject>> LoadAssetsAsync<TObject>(object key, Action<TObject> callback, bool releaseDependenciesOnFailure)
		{
			return Addressables.m_Addressables.LoadAssetsAsync<TObject>(key, callback, releaseDependenciesOnFailure);
		}

		public static void Release<TObject>(TObject obj)
		{
			Addressables.m_Addressables.Release<TObject>(obj);
		}

		public static void Release<TObject>(AsyncOperationHandle<TObject> handle)
		{
			handle.Release();
		}

		public static void Release(AsyncOperationHandle handle)
		{
			handle.Release();
		}

		public static bool ReleaseInstance(GameObject instance)
		{
			return Addressables.m_Addressables.ReleaseInstance(instance);
		}

		public static bool ReleaseInstance(AsyncOperationHandle handle)
		{
			handle.Release();
			return true;
		}

		public static bool ReleaseInstance(AsyncOperationHandle<GameObject> handle)
		{
			handle.Release();
			return true;
		}

		public static AsyncOperationHandle<long> GetDownloadSizeAsync(object key)
		{
			return Addressables.m_Addressables.GetDownloadSizeAsync(key);
		}

		public static AsyncOperationHandle<long> GetDownloadSizeAsync(string key)
		{
			return Addressables.m_Addressables.GetDownloadSizeAsync(key);
		}

		public static AsyncOperationHandle<long> GetDownloadSizeAsync(IEnumerable keys)
		{
			return Addressables.m_Addressables.GetDownloadSizeAsync(keys);
		}

		public static AsyncOperationHandle DownloadDependenciesAsync(object key, bool autoReleaseHandle = false)
		{
			return Addressables.m_Addressables.DownloadDependenciesAsync(key, autoReleaseHandle);
		}

		public static AsyncOperationHandle DownloadDependenciesAsync(IList<IResourceLocation> locations, bool autoReleaseHandle = false)
		{
			return Addressables.m_Addressables.DownloadDependenciesAsync(locations, autoReleaseHandle);
		}

		public static AsyncOperationHandle DownloadDependenciesAsync(IEnumerable keys, Addressables.MergeMode mode, bool autoReleaseHandle = false)
		{
			return Addressables.m_Addressables.DownloadDependenciesAsync(keys, mode, autoReleaseHandle);
		}

		public static void ClearDependencyCacheAsync(object key)
		{
			Addressables.m_Addressables.ClearDependencyCacheAsync(key, true);
		}

		public static void ClearDependencyCacheAsync(IList<IResourceLocation> locations)
		{
			Addressables.m_Addressables.ClearDependencyCacheAsync(locations, true);
		}

		public static void ClearDependencyCacheAsync(IEnumerable keys)
		{
			Addressables.m_Addressables.ClearDependencyCacheAsync(keys, true);
		}

		public static void ClearDependencyCacheAsync(string key)
		{
			Addressables.m_Addressables.ClearDependencyCacheAsync(key, true);
		}

		public static AsyncOperationHandle<bool> ClearDependencyCacheAsync(object key, bool autoReleaseHandle)
		{
			return Addressables.m_Addressables.ClearDependencyCacheAsync(key, autoReleaseHandle);
		}

		public static AsyncOperationHandle<bool> ClearDependencyCacheAsync(IList<IResourceLocation> locations, bool autoReleaseHandle)
		{
			return Addressables.m_Addressables.ClearDependencyCacheAsync(locations, autoReleaseHandle);
		}

		public static AsyncOperationHandle<bool> ClearDependencyCacheAsync(IEnumerable keys, bool autoReleaseHandle)
		{
			return Addressables.m_Addressables.ClearDependencyCacheAsync(keys, autoReleaseHandle);
		}

		public static AsyncOperationHandle<bool> ClearDependencyCacheAsync(string key, bool autoReleaseHandle)
		{
			return Addressables.m_Addressables.ClearDependencyCacheAsync(key, autoReleaseHandle);
		}

		public static ResourceLocatorInfo GetLocatorInfo(string locatorId)
		{
			return Addressables.m_Addressables.GetLocatorInfo(locatorId);
		}

		public static ResourceLocatorInfo GetLocatorInfo(IResourceLocator locator)
		{
			return Addressables.m_Addressables.GetLocatorInfo(locator.LocatorId);
		}

		public static AsyncOperationHandle<GameObject> InstantiateAsync(IResourceLocation location, Transform parent = null, bool instantiateInWorldSpace = false, bool trackHandle = true)
		{
			return Addressables.m_Addressables.InstantiateAsync(location, new InstantiationParameters(parent, instantiateInWorldSpace), trackHandle);
		}

		public static AsyncOperationHandle<GameObject> InstantiateAsync(IResourceLocation location, Vector3 position, Quaternion rotation, Transform parent = null, bool trackHandle = true)
		{
			return Addressables.m_Addressables.InstantiateAsync(location, position, rotation, parent, trackHandle);
		}

		public static AsyncOperationHandle<GameObject> InstantiateAsync(object key, Transform parent = null, bool instantiateInWorldSpace = false, bool trackHandle = true)
		{
			return Addressables.m_Addressables.InstantiateAsync(key, parent, instantiateInWorldSpace, trackHandle);
		}

		public static AsyncOperationHandle<GameObject> InstantiateAsync(object key, Vector3 position, Quaternion rotation, Transform parent = null, bool trackHandle = true)
		{
			return Addressables.m_Addressables.InstantiateAsync(key, position, rotation, parent, trackHandle);
		}

		public static AsyncOperationHandle<GameObject> InstantiateAsync(object key, InstantiationParameters instantiateParameters, bool trackHandle = true)
		{
			return Addressables.m_Addressables.InstantiateAsync(key, instantiateParameters, trackHandle);
		}

		public static AsyncOperationHandle<GameObject> InstantiateAsync(IResourceLocation location, InstantiationParameters instantiateParameters, bool trackHandle = true)
		{
			return Addressables.m_Addressables.InstantiateAsync(location, instantiateParameters, trackHandle);
		}

		public static AsyncOperationHandle<SceneInstance> LoadSceneAsync(object key, LoadSceneMode loadMode = LoadSceneMode.Single, bool activateOnLoad = true, int priority = 100, SceneReleaseMode releaseMode = SceneReleaseMode.ReleaseSceneWhenSceneUnloaded)
		{
			return Addressables.m_Addressables.LoadSceneAsync(key, new LoadSceneParameters(loadMode), releaseMode, activateOnLoad, priority, true);
		}

		public static AsyncOperationHandle<SceneInstance> LoadSceneAsync(object key, LoadSceneMode loadMode, SceneReleaseMode releaseMode, bool activateOnLoad = true, int priority = 100)
		{
			return Addressables.m_Addressables.LoadSceneAsync(key, new LoadSceneParameters(loadMode), releaseMode, activateOnLoad, priority, true);
		}

		public static AsyncOperationHandle<SceneInstance> LoadSceneAsync(object key, LoadSceneParameters loadSceneParameters, bool activateOnLoad = true, int priority = 100)
		{
			return Addressables.m_Addressables.LoadSceneAsync(key, loadSceneParameters, SceneReleaseMode.ReleaseSceneWhenSceneUnloaded, activateOnLoad, priority, true);
		}

		public static AsyncOperationHandle<SceneInstance> LoadSceneAsync(object key, LoadSceneParameters loadSceneParameters, SceneReleaseMode releaseMode, bool activateOnLoad = true, int priority = 100)
		{
			return Addressables.m_Addressables.LoadSceneAsync(key, loadSceneParameters, releaseMode, activateOnLoad, priority, true);
		}

		public static AsyncOperationHandle<SceneInstance> LoadSceneAsync(IResourceLocation location, LoadSceneMode loadMode = LoadSceneMode.Single, bool activateOnLoad = true, int priority = 100)
		{
			return Addressables.m_Addressables.LoadSceneAsync(location, new LoadSceneParameters(loadMode), SceneReleaseMode.ReleaseSceneWhenSceneUnloaded, activateOnLoad, priority, true);
		}

		public static AsyncOperationHandle<SceneInstance> LoadSceneAsync(IResourceLocation location, LoadSceneMode loadMode, SceneReleaseMode releaseMode, bool activateOnLoad = true, int priority = 100)
		{
			return Addressables.m_Addressables.LoadSceneAsync(location, new LoadSceneParameters(loadMode), releaseMode, activateOnLoad, priority, true);
		}

		public static AsyncOperationHandle<SceneInstance> LoadSceneAsync(IResourceLocation location, LoadSceneParameters loadSceneParameters, bool activateOnLoad = true, int priority = 100)
		{
			return Addressables.m_Addressables.LoadSceneAsync(location, loadSceneParameters, SceneReleaseMode.ReleaseSceneWhenSceneUnloaded, activateOnLoad, priority, true);
		}

		public static AsyncOperationHandle<SceneInstance> LoadSceneAsync(IResourceLocation location, LoadSceneParameters loadSceneParameters, SceneReleaseMode releaseMode, bool activateOnLoad = true, int priority = 100)
		{
			return Addressables.m_Addressables.LoadSceneAsync(location, loadSceneParameters, releaseMode, activateOnLoad, priority, true);
		}

		public static AsyncOperationHandle<SceneInstance> UnloadSceneAsync(SceneInstance scene, UnloadSceneOptions unloadOptions, bool autoReleaseHandle = true)
		{
			return Addressables.m_Addressables.UnloadSceneAsync(scene, unloadOptions, autoReleaseHandle);
		}

		public static AsyncOperationHandle<SceneInstance> UnloadSceneAsync(AsyncOperationHandle handle, UnloadSceneOptions unloadOptions, bool autoReleaseHandle = true)
		{
			return Addressables.m_Addressables.UnloadSceneAsync(handle, unloadOptions, autoReleaseHandle);
		}

		public static AsyncOperationHandle<SceneInstance> UnloadSceneAsync(SceneInstance scene, bool autoReleaseHandle = true)
		{
			return Addressables.m_Addressables.UnloadSceneAsync(scene, UnloadSceneOptions.None, autoReleaseHandle);
		}

		public static AsyncOperationHandle<SceneInstance> UnloadSceneAsync(AsyncOperationHandle handle, bool autoReleaseHandle = true)
		{
			return Addressables.m_Addressables.UnloadSceneAsync(handle, UnloadSceneOptions.None, autoReleaseHandle);
		}

		public static AsyncOperationHandle<SceneInstance> UnloadSceneAsync(AsyncOperationHandle<SceneInstance> handle, bool autoReleaseHandle = true)
		{
			return Addressables.m_Addressables.UnloadSceneAsync(handle, UnloadSceneOptions.None, autoReleaseHandle);
		}

		public static AsyncOperationHandle<List<string>> CheckForCatalogUpdates(bool autoReleaseHandle = true)
		{
			return Addressables.m_Addressables.CheckForCatalogUpdates(autoReleaseHandle);
		}

		public static AsyncOperationHandle<List<IResourceLocator>> UpdateCatalogs(IEnumerable<string> catalogs = null, bool autoReleaseHandle = true)
		{
			return Addressables.m_Addressables.UpdateCatalogs(catalogs, autoReleaseHandle, false);
		}

		public static AsyncOperationHandle<List<IResourceLocator>> UpdateCatalogs(bool autoCleanBundleCache, IEnumerable<string> catalogs = null, bool autoReleaseHandle = true)
		{
			return Addressables.m_Addressables.UpdateCatalogs(catalogs, autoReleaseHandle, autoCleanBundleCache);
		}

		public static void AddResourceLocator(IResourceLocator locator, string localCatalogHash = null, IResourceLocation remoteCatalogLocation = null)
		{
			Addressables.m_Addressables.AddResourceLocator(locator, localCatalogHash, remoteCatalogLocation);
		}

		public static void RemoveResourceLocator(IResourceLocator locator)
		{
			Addressables.m_Addressables.RemoveResourceLocator(locator);
		}

		public static void ClearResourceLocators()
		{
			Addressables.m_Addressables.ClearResourceLocators();
		}

		public static AsyncOperationHandle<bool> CleanBundleCache(IEnumerable<string> catalogsIds = null)
		{
			return Addressables.m_Addressables.CleanBundleCache(catalogsIds, false);
		}

		public static ResourceLocationBase CreateCatalogLocationWithHashDependencies<T>(string remoteCatalogPath) where T : IResourceProvider
		{
			return Addressables.m_Addressables.CreateCatalogLocationWithHashDependencies<T>(remoteCatalogPath);
		}

		public static ResourceLocationBase CreateCatalogLocationWithHashDependencies<T>(IResourceLocation remoteCatalogLocation) where T : IResourceProvider
		{
			return Addressables.m_Addressables.CreateCatalogLocationWithHashDependencies<T>(remoteCatalogLocation);
		}

		public static ResourceLocationBase CreateCatalogLocationWithHashDependencies<T>(string remoteCatalogPath, string remoteHashPath) where T : IResourceProvider
		{
			return Addressables.m_Addressables.CreateCatalogLocationWithHashDependencies<T>(remoteCatalogPath, remoteHashPath);
		}

		internal static bool reinitializeAddressables = true;

		internal static AddressablesImpl m_AddressablesInstance = new AddressablesImpl(new DefaultAllocationStrategy());

		public const string kAddressablesRuntimeDataPath = "AddressablesRuntimeDataPath";

		private const string k_AddressablesLogConditional = "ADDRESSABLES_LOG_ALL";

		public const string kAddressablesRuntimeBuildLogPath = "AddressablesRuntimeBuildLog";

		public static string LibraryPath = "Library/com.unity.addressables/";

		public static string BuildReportPath = "Library/com.unity.addressables/BuildReports/";

		public enum MergeMode
		{
			None,
			UseFirst = 0,
			Union,
			Intersection
		}
	}
}
