using System;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace UnityEngine.AddressableAssets
{
	public class ResourceLocatorInfo
	{
		public IResourceLocator Locator { get; private set; }

		public string LocalHash { get; private set; }

		public IResourceLocation CatalogLocation { get; private set; }

		internal bool ContentUpdateAvailable { get; set; }

		public ResourceLocatorInfo(IResourceLocator loc, string localHash, IResourceLocation remoteCatalogLocation)
		{
			this.Locator = loc;
			this.LocalHash = localHash;
			this.CatalogLocation = remoteCatalogLocation;
		}

		public IResourceLocation HashLocation
		{
			get
			{
				return this.CatalogLocation.Dependencies[0];
			}
		}

		public bool CanUpdateContent
		{
			get
			{
				return !string.IsNullOrEmpty(this.LocalHash) && this.CatalogLocation != null && this.CatalogLocation.HasDependencies && this.CatalogLocation.Dependencies.Count == 3;
			}
		}

		internal void UpdateContent(IResourceLocator locator, string hash, IResourceLocation loc)
		{
			this.LocalHash = hash;
			this.CatalogLocation = loc;
			this.Locator = locator;
		}
	}
}
