using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace UnityEngine.AddressableAssets.ResourceLocators
{
	public class ResourceLocationMap : IResourceLocator
	{
		public ResourceLocationMap(string id, int capacity = 0)
		{
			this.LocatorId = id;
			this.locations = new Dictionary<object, IList<IResourceLocation>>((capacity == 0) ? 100 : capacity);
		}

		public string LocatorId { get; private set; }

		public ResourceLocationMap(string id, IList<ResourceLocationData> locations)
		{
			this.LocatorId = id;
			if (locations == null)
			{
				return;
			}
			this.locations = new Dictionary<object, IList<IResourceLocation>>(locations.Count * 2);
			Dictionary<string, ResourceLocationBase> dictionary = new Dictionary<string, ResourceLocationBase>();
			Dictionary<string, ResourceLocationData> dictionary2 = new Dictionary<string, ResourceLocationData>();
			for (int i = 0; i < locations.Count; i++)
			{
				ResourceLocationData resourceLocationData = locations[i];
				if (resourceLocationData.Keys == null || resourceLocationData.Keys.Length < 1)
				{
					Addressables.LogErrorFormat("Address with id '{0}' does not have any valid keys, skipping...", new object[]
					{
						resourceLocationData.InternalId
					});
				}
				else if (dictionary.ContainsKey(resourceLocationData.Keys[0]))
				{
					Addressables.LogErrorFormat("Duplicate address '{0}' with id '{1}' found, skipping...", new object[]
					{
						resourceLocationData.Keys[0],
						resourceLocationData.InternalId
					});
				}
				else
				{
					ResourceLocationBase resourceLocationBase = new ResourceLocationBase(resourceLocationData.Keys[0], Addressables.ResolveInternalId(resourceLocationData.InternalId), resourceLocationData.Provider, resourceLocationData.ResourceType, Array.Empty<IResourceLocation>());
					resourceLocationBase.Data = resourceLocationData.Data;
					dictionary.Add(resourceLocationData.Keys[0], resourceLocationBase);
					dictionary2.Add(resourceLocationData.Keys[0], resourceLocationData);
				}
			}
			foreach (KeyValuePair<string, ResourceLocationBase> keyValuePair in dictionary)
			{
				ResourceLocationData resourceLocationData2 = dictionary2[keyValuePair.Key];
				if (resourceLocationData2.Dependencies != null)
				{
					foreach (string key in resourceLocationData2.Dependencies)
					{
						keyValuePair.Value.Dependencies.Add(dictionary[key]);
					}
					keyValuePair.Value.ComputeDependencyHash();
				}
			}
			foreach (KeyValuePair<string, ResourceLocationBase> keyValuePair2 in dictionary)
			{
				foreach (string key2 in dictionary2[keyValuePair2.Key].Keys)
				{
					this.Add(key2, keyValuePair2.Value);
				}
			}
		}

		public IEnumerable<IResourceLocation> AllLocations
		{
			get
			{
				return this.locations.SelectMany((KeyValuePair<object, IList<IResourceLocation>> k) => k.Value);
			}
		}

		public Dictionary<object, IList<IResourceLocation>> Locations
		{
			get
			{
				return this.locations;
			}
		}

		public IEnumerable<object> Keys
		{
			get
			{
				return this.locations.Keys;
			}
		}

		public bool Locate(object key, Type type, out IList<IResourceLocation> locations)
		{
			IList<IResourceLocation> list = null;
			if (!this.locations.TryGetValue(key, out list))
			{
				locations = null;
				return false;
			}
			if (type == null)
			{
				locations = list;
				return true;
			}
			int num = 0;
			foreach (IResourceLocation resourceLocation in list)
			{
				if (type.IsAssignableFrom(resourceLocation.ResourceType))
				{
					num++;
				}
			}
			if (num == 0)
			{
				locations = null;
				return false;
			}
			if (num == list.Count)
			{
				locations = list;
				return true;
			}
			locations = new List<IResourceLocation>();
			foreach (IResourceLocation resourceLocation2 in list)
			{
				if (type.IsAssignableFrom(resourceLocation2.ResourceType))
				{
					locations.Add(resourceLocation2);
				}
			}
			return true;
		}

		public void Add(object key, IResourceLocation location)
		{
			IList<IResourceLocation> list;
			if (!this.locations.TryGetValue(key, out list))
			{
				this.locations.Add(key, list = new List<IResourceLocation>());
			}
			list.Add(location);
		}

		public void Add(object key, IList<IResourceLocation> locations)
		{
			this.locations.Add(key, locations);
		}

		private Dictionary<object, IList<IResourceLocation>> locations;
	}
}
