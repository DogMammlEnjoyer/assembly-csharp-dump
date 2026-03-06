using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.Util;
using UnityEngine.U2D;

namespace UnityEngine.AddressableAssets
{
	internal class DynamicResourceLocator : IResourceLocator
	{
		public string LocatorId
		{
			get
			{
				return "DynamicResourceLocator";
			}
		}

		public virtual IEnumerable<object> Keys
		{
			get
			{
				return new object[0];
			}
		}

		private string AtlasSpriteProviderId
		{
			get
			{
				if (!string.IsNullOrEmpty(this.m_AtlasSpriteProviderId))
				{
					return this.m_AtlasSpriteProviderId;
				}
				foreach (IResourceProvider resourceProvider in this.m_Addressables.ResourceManager.ResourceProviders)
				{
					if (resourceProvider is AtlasSpriteProvider)
					{
						this.m_AtlasSpriteProviderId = resourceProvider.ProviderId;
						return this.m_AtlasSpriteProviderId;
					}
				}
				return typeof(AtlasSpriteProvider).FullName;
			}
		}

		public IEnumerable<IResourceLocation> AllLocations
		{
			get
			{
				return new IResourceLocation[0];
			}
		}

		public DynamicResourceLocator(AddressablesImpl addr)
		{
			this.m_Addressables = addr;
		}

		public bool Locate(object key, Type type, out IList<IResourceLocation> locations)
		{
			locations = null;
			string key2;
			string subKey;
			if (ResourceManagerConfig.ExtractKeyAndSubKey(key, out key2, out subKey))
			{
				IList<IResourceLocation> list;
				if (!this.m_Addressables.GetResourceLocations(key2, type, out list) && type == typeof(Sprite))
				{
					this.m_Addressables.GetResourceLocations(key2, typeof(SpriteAtlas), out list);
				}
				if (list != null && list.Count > 0)
				{
					locations = new List<IResourceLocation>(list.Count);
					foreach (IResourceLocation mainLoc in list)
					{
						this.CreateDynamicLocations(type, locations, key as string, subKey, mainLoc);
					}
					return true;
				}
			}
			return false;
		}

		internal void CreateDynamicLocations(Type type, IList<IResourceLocation> locations, string locName, string subKey, IResourceLocation mainLoc)
		{
			if (type == typeof(Sprite) && mainLoc.ResourceType == typeof(SpriteAtlas))
			{
				locations.Add(new ResourceLocationBase(locName, mainLoc.InternalId + "[" + subKey + "]", this.AtlasSpriteProviderId, type, new IResourceLocation[]
				{
					mainLoc
				}));
				return;
			}
			if (mainLoc.HasDependencies)
			{
				locations.Add(new ResourceLocationBase(locName, mainLoc.InternalId + "[" + subKey + "]", mainLoc.ProviderId, mainLoc.ResourceType, mainLoc.Dependencies.ToArray<IResourceLocation>()));
				return;
			}
			locations.Add(new ResourceLocationBase(locName, mainLoc.InternalId + "[" + subKey + "]", mainLoc.ProviderId, mainLoc.ResourceType, Array.Empty<IResourceLocation>()));
		}

		private AddressablesImpl m_Addressables;

		private string m_AtlasSpriteProviderId;
	}
}
