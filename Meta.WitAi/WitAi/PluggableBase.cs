using System;
using System.Collections.Generic;
using Meta.WitAi.Utilities;
using UnityEngine;

namespace Meta.WitAi
{
	[Serializable]
	public abstract class PluggableBase<T>
	{
		protected static void CheckForPlugins()
		{
			if (PluggableBase<T>._pluginTypes != null)
			{
				return;
			}
			PluggableBase<T>.FindPlugins();
		}

		protected void EnsurePluginsAreLoaded()
		{
			PluggableBase<T>.CheckForPlugins();
			this.LoadedPlugins = new List<T>(PluggableBase<T>.BuildPlugins());
		}

		private static void FindPlugins()
		{
			PluggableBase<T>._pluginTypes = ReflectionUtils.GetAllAssignableTypes<T>();
		}

		private static IEnumerable<T> BuildPlugins()
		{
			T[] array = new T[PluggableBase<T>._pluginTypes.Length];
			for (int i = 0; i < array.Length; i++)
			{
				object obj = Activator.CreateInstance(PluggableBase<T>._pluginTypes[i]);
				if (obj is T)
				{
					T t = (T)((object)obj);
					array[i] = t;
				}
			}
			return array;
		}

		public TPluginType Get<TPluginType>() where TPluginType : T
		{
			if (this.LoadedPlugins == null)
			{
				this.EnsurePluginsAreLoaded();
			}
			return (TPluginType)((object)this.LoadedPlugins.Find((T path) => path is TPluginType));
		}

		public TPluginType[] GetAll<TPluginType>() where TPluginType : T
		{
			if (this.LoadedPlugins == null)
			{
				this.EnsurePluginsAreLoaded();
			}
			if (this.LoadedPlugins == null)
			{
				return Array.Empty<TPluginType>();
			}
			List<TPluginType> list = new List<TPluginType>();
			foreach (T t in this.LoadedPlugins)
			{
				if (t is TPluginType)
				{
					TPluginType item = (TPluginType)((object)t);
					list.Add(item);
				}
			}
			return list.ToArray();
		}

		private static Type[] _pluginTypes;

		[SerializeField]
		protected List<T> LoadedPlugins;
	}
}
