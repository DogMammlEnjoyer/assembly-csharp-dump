using System;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.Util;
using UnityEngine.Serialization;

namespace UnityEngine.ResourceManagement.ResourceProviders
{
	[Serializable]
	public class AssetBundleRequestOptions : ILocationSizeData
	{
		public string Hash
		{
			get
			{
				return this.m_Hash;
			}
			set
			{
				this.m_Hash = value;
			}
		}

		public uint Crc
		{
			get
			{
				return this.m_Crc;
			}
			set
			{
				this.m_Crc = value;
			}
		}

		public int Timeout
		{
			get
			{
				return this.m_Timeout;
			}
			set
			{
				this.m_Timeout = value;
			}
		}

		public bool ChunkedTransfer
		{
			get
			{
				return this.m_ChunkedTransfer;
			}
			set
			{
				this.m_ChunkedTransfer = value;
			}
		}

		public int RedirectLimit
		{
			get
			{
				if (this.m_RedirectLimit <= 128)
				{
					return this.m_RedirectLimit;
				}
				return 128;
			}
			set
			{
				this.m_RedirectLimit = value;
			}
		}

		public int RetryCount
		{
			get
			{
				return this.m_RetryCount;
			}
			set
			{
				this.m_RetryCount = value;
			}
		}

		public string BundleName
		{
			get
			{
				return this.m_BundleName;
			}
			set
			{
				this.m_BundleName = value;
			}
		}

		public AssetLoadMode AssetLoadMode
		{
			get
			{
				return this.m_AssetLoadMode;
			}
			set
			{
				this.m_AssetLoadMode = value;
			}
		}

		public long BundleSize
		{
			get
			{
				return this.m_BundleSize;
			}
			set
			{
				this.m_BundleSize = value;
			}
		}

		public bool UseCrcForCachedBundle
		{
			get
			{
				return this.m_UseCrcForCachedBundles;
			}
			set
			{
				this.m_UseCrcForCachedBundles = value;
			}
		}

		public bool UseUnityWebRequestForLocalBundles
		{
			get
			{
				return this.m_UseUWRForLocalBundles;
			}
			set
			{
				this.m_UseUWRForLocalBundles = value;
			}
		}

		public bool ClearOtherCachedVersionsWhenLoaded
		{
			get
			{
				return this.m_ClearOtherCachedVersionsWhenLoaded;
			}
			set
			{
				this.m_ClearOtherCachedVersionsWhenLoaded = value;
			}
		}

		public virtual long ComputeSize(IResourceLocation location, ResourceManager resourceManager)
		{
			if (!ResourceManagerConfig.IsPathRemote((resourceManager == null) ? location.InternalId : resourceManager.TransformInternalId(location)))
			{
				return 0L;
			}
			Hash128 hash = Hash128.Parse(this.Hash);
			if (hash.isValid && Caching.IsVersionCached(new CachedAssetBundle(this.BundleName, hash)))
			{
				return 0L;
			}
			return this.BundleSize;
		}

		[FormerlySerializedAs("m_hash")]
		[SerializeField]
		private string m_Hash = "";

		[FormerlySerializedAs("m_crc")]
		[SerializeField]
		private uint m_Crc;

		[FormerlySerializedAs("m_timeout")]
		[SerializeField]
		private int m_Timeout;

		[FormerlySerializedAs("m_chunkedTransfer")]
		[SerializeField]
		private bool m_ChunkedTransfer;

		[FormerlySerializedAs("m_redirectLimit")]
		[SerializeField]
		private int m_RedirectLimit = -1;

		[FormerlySerializedAs("m_retryCount")]
		[SerializeField]
		private int m_RetryCount;

		[SerializeField]
		private string m_BundleName;

		[SerializeField]
		private AssetLoadMode m_AssetLoadMode;

		[SerializeField]
		private long m_BundleSize;

		[SerializeField]
		private bool m_UseCrcForCachedBundles;

		[SerializeField]
		private bool m_UseUWRForLocalBundles;

		[SerializeField]
		private bool m_ClearOtherCachedVersionsWhenLoaded;
	}
}
