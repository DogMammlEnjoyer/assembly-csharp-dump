using System;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.Util;

namespace UnityEngine.ResourceManagement.ResourceProviders
{
	[Serializable]
	public class BinaryCatalogInitialization : IInitializableObject
	{
		public static int BinaryStorageBufferCacheSize
		{
			get
			{
				return BinaryCatalogInitialization.s_BinaryStorageBufferCacheSize;
			}
		}

		public static int CatalogLocationCacheSize
		{
			get
			{
				return BinaryCatalogInitialization.s_CatalogLocationCacheSize;
			}
		}

		public static void ResetToDefaults()
		{
			BinaryCatalogInitialization.s_BinaryStorageBufferCacheSize = 128;
			BinaryCatalogInitialization.s_CatalogLocationCacheSize = 32;
		}

		public bool Initialize(string id, string dataStr)
		{
			try
			{
				BinaryCatalogInitializationData binaryCatalogInitializationData = JsonUtility.FromJson<BinaryCatalogInitializationData>(dataStr);
				if (binaryCatalogInitializationData != null)
				{
					BinaryCatalogInitialization.s_BinaryStorageBufferCacheSize = binaryCatalogInitializationData.m_BinaryStorageBufferCacheSize;
					BinaryCatalogInitialization.s_CatalogLocationCacheSize = binaryCatalogInitializationData.m_CatalogLocationCacheSize;
				}
			}
			catch (Exception exception)
			{
				Debug.LogWarning("Failed to initialize BinaryCatalog cache size - invalid data.");
				Debug.LogException(exception);
			}
			return true;
		}

		public AsyncOperationHandle<bool> InitializeAsync(ResourceManager resourceManager, string id, string dataStr)
		{
			return resourceManager.CreateCompletedOperation<bool>(this.Initialize(id, dataStr), null);
		}

		public const int kDefaultBinaryStorageBufferCacheSize = 128;

		public const int kCatalogLocationCacheSize = 32;

		private static int s_BinaryStorageBufferCacheSize = 128;

		private static int s_CatalogLocationCacheSize = 32;
	}
}
