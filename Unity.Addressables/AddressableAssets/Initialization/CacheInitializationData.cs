using System;
using UnityEngine.Serialization;

namespace UnityEngine.AddressableAssets.Initialization
{
	[Serializable]
	public class CacheInitializationData
	{
		public bool CompressionEnabled
		{
			get
			{
				return this.m_CompressionEnabled;
			}
			set
			{
				this.m_CompressionEnabled = value;
			}
		}

		public string CacheDirectoryOverride
		{
			get
			{
				return this.m_CacheDirectoryOverride;
			}
			set
			{
				this.m_CacheDirectoryOverride = value;
			}
		}

		public bool LimitCacheSize
		{
			get
			{
				return this.m_LimitCacheSize;
			}
			set
			{
				this.m_LimitCacheSize = value;
			}
		}

		public long MaximumCacheSize
		{
			get
			{
				return this.m_MaximumCacheSize;
			}
			set
			{
				this.m_MaximumCacheSize = value;
			}
		}

		[FormerlySerializedAs("m_compressionEnabled")]
		[SerializeField]
		private bool m_CompressionEnabled = true;

		[FormerlySerializedAs("m_cacheDirectoryOverride")]
		[SerializeField]
		private string m_CacheDirectoryOverride = "";

		[FormerlySerializedAs("m_limitCacheSize")]
		[SerializeField]
		private bool m_LimitCacheSize;

		[FormerlySerializedAs("m_maximumCacheSize")]
		[SerializeField]
		private long m_MaximumCacheSize = long.MaxValue;
	}
}
