using System;
using System.Collections.Generic;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.Util;
using UnityEngine.Serialization;

namespace UnityEngine.AddressableAssets.Initialization
{
	[Serializable]
	public class ResourceManagerRuntimeData
	{
		public string BuildTarget
		{
			get
			{
				return this.m_buildTarget;
			}
			set
			{
				this.m_buildTarget = value;
			}
		}

		public string SettingsHash
		{
			get
			{
				return this.m_SettingsHash;
			}
			set
			{
				this.m_SettingsHash = value;
			}
		}

		public List<ResourceLocationData> CatalogLocations
		{
			get
			{
				return this.m_CatalogLocations;
			}
		}

		public bool LogResourceManagerExceptions
		{
			get
			{
				return this.m_LogResourceManagerExceptions;
			}
			set
			{
				this.m_LogResourceManagerExceptions = value;
			}
		}

		public List<ObjectInitializationData> InitializationObjects
		{
			get
			{
				return this.m_ExtraInitializationData;
			}
		}

		public bool DisableCatalogUpdateOnStartup
		{
			get
			{
				return this.m_DisableCatalogUpdateOnStart;
			}
			set
			{
				this.m_DisableCatalogUpdateOnStart = value;
			}
		}

		public bool IsLocalCatalogInBundle
		{
			get
			{
				return this.m_IsLocalCatalogInBundle;
			}
			set
			{
				this.m_IsLocalCatalogInBundle = value;
			}
		}

		public Type CertificateHandlerType
		{
			get
			{
				return this.m_CertificateHandlerType.Value;
			}
			set
			{
				this.m_CertificateHandlerType.Value = value;
			}
		}

		public string AddressablesVersion
		{
			get
			{
				return this.m_AddressablesVersion;
			}
			set
			{
				this.m_AddressablesVersion = value;
			}
		}

		public int MaxConcurrentWebRequests
		{
			get
			{
				return this.m_maxConcurrentWebRequests;
			}
			set
			{
				this.m_maxConcurrentWebRequests = Mathf.Clamp(value, 1, 1024);
			}
		}

		public int CatalogRequestsTimeout
		{
			get
			{
				return this.m_CatalogRequestsTimeout;
			}
			set
			{
				this.m_CatalogRequestsTimeout = ((value < 0) ? 0 : value);
			}
		}

		public const string kCatalogAddress = "AddressablesMainContentCatalog";

		[SerializeField]
		private string m_buildTarget;

		[FormerlySerializedAs("m_settingsHash")]
		[SerializeField]
		private string m_SettingsHash;

		[FormerlySerializedAs("m_catalogLocations")]
		[SerializeField]
		private List<ResourceLocationData> m_CatalogLocations = new List<ResourceLocationData>();

		[FormerlySerializedAs("m_logResourceManagerExceptions")]
		[SerializeField]
		private bool m_LogResourceManagerExceptions = true;

		[FormerlySerializedAs("m_extraInitializationData")]
		[SerializeField]
		private List<ObjectInitializationData> m_ExtraInitializationData = new List<ObjectInitializationData>();

		[SerializeField]
		private bool m_DisableCatalogUpdateOnStart;

		[SerializeField]
		private bool m_IsLocalCatalogInBundle;

		[SerializeField]
		private SerializedType m_CertificateHandlerType;

		[SerializeField]
		private string m_AddressablesVersion;

		[SerializeField]
		private int m_maxConcurrentWebRequests = 500;

		[SerializeField]
		private int m_CatalogRequestsTimeout;
	}
}
