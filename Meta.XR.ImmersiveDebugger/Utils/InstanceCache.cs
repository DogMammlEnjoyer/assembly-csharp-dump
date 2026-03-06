using System;
using System.Collections.Generic;
using System.Linq;
using Meta.XR.ImmersiveDebugger.UserInterface;
using UnityEngine;

namespace Meta.XR.ImmersiveDebugger.Utils
{
	internal class InstanceCache
	{
		public event Action<Type> OnCacheChangedForTypeEvent;

		public event Func<InstanceHandle, IInspector> OnInstanceAdded;

		public event Action<InstanceHandle> OnInstanceRemoved;

		public List<InstanceHandle> GetCacheDataForClass(Type classType)
		{
			List<InstanceHandle> list;
			this.CacheData.TryGetValue(classType, out list);
			return list ?? this._emptyCache;
		}

		private List<InstanceHandle> FetchObjectsHandlesOfType(Type classType)
		{
			return (from obj in Object.FindObjectsByType(classType, FindObjectsSortMode.None)
			select new InstanceHandle(classType, obj)).ToList<InstanceHandle>();
		}

		public void RegisterClassType(Type classType)
		{
			if (this.CacheData.ContainsKey(classType))
			{
				return;
			}
			this.CacheData[classType] = new List<InstanceHandle>();
		}

		public void RegisterClassTypes(IEnumerable<Type> types)
		{
			foreach (Type classType in types)
			{
				this.RegisterClassType(classType);
			}
		}

		internal void RetrieveInstances()
		{
			foreach (KeyValuePair<Type, List<InstanceHandle>> keyValuePair in new Dictionary<Type, List<InstanceHandle>>(this.CacheData))
			{
				Type key = keyValuePair.Key;
				List<InstanceHandle> value = keyValuePair.Value;
				bool flag = false;
				foreach (InstanceHandle instanceHandle in this.FetchObjectsHandlesOfType(key))
				{
					if (!value.Contains(instanceHandle))
					{
						value.Add(instanceHandle);
						flag = true;
						Func<InstanceHandle, IInspector> onInstanceAdded = this.OnInstanceAdded;
						if (onInstanceAdded != null)
						{
							onInstanceAdded(instanceHandle);
						}
					}
				}
				for (int i = value.Count - 1; i >= 0; i--)
				{
					if (!value[i].Valid)
					{
						Action<InstanceHandle> onInstanceRemoved = this.OnInstanceRemoved;
						if (onInstanceRemoved != null)
						{
							onInstanceRemoved(value[i]);
						}
						value.RemoveAt(i);
						flag = true;
					}
				}
				if (flag)
				{
					this.CacheData[key] = value;
					Action<Type> onCacheChangedForTypeEvent = this.OnCacheChangedForTypeEvent;
					if (onCacheChangedForTypeEvent != null)
					{
						onCacheChangedForTypeEvent(key);
					}
				}
			}
		}

		internal void RegisterHandle(InstanceHandle handle)
		{
			this.RegisterClassType(handle.Type);
			List<InstanceHandle> list;
			if (this.CacheData.TryGetValue(handle.Type, out list))
			{
				list.Add(handle);
			}
		}

		internal void UnregisterHandle(InstanceHandle handle)
		{
			List<InstanceHandle> list;
			if (this.CacheData.TryGetValue(handle.Type, out list))
			{
				list.Remove(handle);
			}
		}

		internal readonly Dictionary<Type, List<InstanceHandle>> CacheData = new Dictionary<Type, List<InstanceHandle>>();

		private readonly List<InstanceHandle> _emptyCache = new List<InstanceHandle>();
	}
}
