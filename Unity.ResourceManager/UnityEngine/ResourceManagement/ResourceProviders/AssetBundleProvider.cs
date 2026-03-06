using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.Util;

namespace UnityEngine.ResourceManagement.ResourceProviders
{
	[DisplayName("AssetBundle Provider")]
	public class AssetBundleProvider : ResourceProviderBase
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void Init()
		{
			AssetBundleProvider.m_UnloadingBundles = new Dictionary<string, AssetBundleUnloadOperation>();
		}

		protected internal static Dictionary<string, AssetBundleUnloadOperation> UnloadingBundles
		{
			get
			{
				return AssetBundleProvider.m_UnloadingBundles;
			}
			internal set
			{
				AssetBundleProvider.m_UnloadingBundles = value;
			}
		}

		internal static int UnloadingAssetBundleCount
		{
			get
			{
				return AssetBundleProvider.m_UnloadingBundles.Count;
			}
		}

		internal static int AssetBundleCount
		{
			get
			{
				return AssetBundle.GetAllLoadedAssetBundles().Count<AssetBundle>() - AssetBundleProvider.UnloadingAssetBundleCount;
			}
		}

		internal static void WaitForAllUnloadingBundlesToComplete()
		{
			if (AssetBundleProvider.UnloadingAssetBundleCount > 0)
			{
				AssetBundleUnloadOperation[] array = AssetBundleProvider.m_UnloadingBundles.Values.ToArray<AssetBundleUnloadOperation>();
				for (int i = 0; i < array.Length; i++)
				{
					array[i].WaitForCompletion();
				}
			}
		}

		public override void Provide(ProvideHandle providerInterface)
		{
			AssetBundleUnloadOperation assetBundleUnloadOperation;
			if (AssetBundleProvider.m_UnloadingBundles.TryGetValue(providerInterface.Location.InternalId, out assetBundleUnloadOperation) && assetBundleUnloadOperation.isDone)
			{
				assetBundleUnloadOperation = null;
			}
			new AssetBundleResource().Start(providerInterface, assetBundleUnloadOperation, new Func<UnityWebRequestResult, bool>(this.ShouldRetryDownloadError));
		}

		public override Type GetDefaultType(IResourceLocation location)
		{
			return typeof(IAssetBundleResource);
		}

		public override void Release(IResourceLocation location, object asset)
		{
			if (location == null)
			{
				throw new ArgumentNullException("location");
			}
			if (asset == null)
			{
				if (!(location is DownloadOnlyLocation))
				{
					Debug.LogWarningFormat("Releasing null asset bundle from location {0}.  This is an indication that the bundle failed to load.", new object[]
					{
						location
					});
				}
				return;
			}
			AssetBundleResource assetBundleResource = asset as AssetBundleResource;
			AssetBundleUnloadOperation assetBundleUnloadOperation;
			if (assetBundleResource != null && assetBundleResource.Unload(out assetBundleUnloadOperation))
			{
				AssetBundleProvider.m_UnloadingBundles.Add(location.InternalId, assetBundleUnloadOperation);
				assetBundleUnloadOperation.completed += delegate(AsyncOperation op)
				{
					AssetBundleProvider.m_UnloadingBundles.Remove(location.InternalId);
				};
			}
		}

		public virtual bool ShouldRetryDownloadError(UnityWebRequestResult uwrResult)
		{
			return uwrResult.ShouldRetryDownloadError();
		}

		internal virtual IOperationCacheKey CreateCacheKeyForLocation(ResourceManager rm, IResourceLocation location, Type desiredType)
		{
			return new IdCacheKey(location.GetType(), rm.TransformInternalId(location));
		}

		internal static Dictionary<string, AssetBundleUnloadOperation> m_UnloadingBundles = new Dictionary<string, AssetBundleUnloadOperation>();
	}
}
