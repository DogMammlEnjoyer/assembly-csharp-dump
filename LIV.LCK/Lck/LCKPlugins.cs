using System;
using System.Collections.Generic;
using System.Linq;

namespace Liv.Lck
{
	public class LCKPlugins
	{
		public static LCKPlugins Instance
		{
			get
			{
				if (LCKPlugins._instance == null)
				{
					object @lock = LCKPlugins._lock;
					lock (@lock)
					{
						if (LCKPlugins._instance == null)
						{
							LCKPlugins._instance = new LCKPlugins();
						}
					}
				}
				return LCKPlugins._instance;
			}
		}

		private LCKPlugins()
		{
			this._pluginsByType = new Dictionary<Type, ILCKPlugin>();
			this._pluginsByName = new Dictionary<string, ILCKPlugin>();
			this._isInitialized = false;
		}

		public void Initialize(LckService lckService)
		{
			if (this._isInitialized)
			{
				LckLog.LogWarning("LCKPlugins already initialized", "Initialize", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\LCKPlugins.cs", 56);
				return;
			}
			LckLog.Log(string.Format("Initializing {0} plugins", this._pluginsByType.Count), "Initialize", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\LCKPlugins.cs", 60);
			foreach (ILCKPlugin ilckplugin in this._pluginsByType.Values)
			{
				try
				{
					ilckplugin.Initialize(lckService);
					LckLog.Log("Initialized plugin: " + ilckplugin.GetType().Name, "Initialize", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\LCKPlugins.cs", 67);
				}
				catch (Exception ex)
				{
					LckLog.LogError("Failed to initialize plugin " + ilckplugin.GetType().Name + ": " + ex.Message, "Initialize", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\LCKPlugins.cs", 71);
				}
			}
			this._isInitialized = true;
			LckLog.Log("LCKPlugins initialization complete", "Initialize", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\LCKPlugins.cs", 76);
		}

		internal void RegisterPlugin(ILCKPlugin plugin)
		{
			if (plugin == null)
			{
				LckLog.LogError("Attempted to register null plugin", "RegisterPlugin", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\LCKPlugins.cs", 87);
				return;
			}
			Type type = plugin.GetType();
			string pluginName = plugin.PluginName;
			if (this._pluginsByType.ContainsKey(type))
			{
				LckLog.LogWarning("Plugin of type " + type.Name + " is already registered", "RegisterPlugin", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\LCKPlugins.cs", 96);
				return;
			}
			if (this._pluginsByName.ContainsKey(pluginName))
			{
				LckLog.LogWarning("Plugin with name '" + pluginName + "' is already registered", "RegisterPlugin", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\LCKPlugins.cs", 102);
				return;
			}
			this._pluginsByType[type] = plugin;
			this._pluginsByName[pluginName] = plugin;
			LckLog.Log(string.Concat(new string[]
			{
				"Registered plugin: ",
				pluginName,
				" (",
				type.Name,
				")"
			}), "RegisterPlugin", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\LCKPlugins.cs", 109);
		}

		public bool HasPlugin<T>() where T : class, ILCKPlugin
		{
			return this._pluginsByType.ContainsKey(typeof(T));
		}

		public bool HasPlugin(string pluginName)
		{
			return this._pluginsByName.ContainsKey(pluginName);
		}

		public T GetPlugin<T>() where T : class, ILCKPlugin
		{
			ILCKPlugin ilckplugin;
			if (this._pluginsByType.TryGetValue(typeof(T), out ilckplugin))
			{
				return ilckplugin as T;
			}
			return default(T);
		}

		public ILCKPlugin GetPlugin(string pluginName)
		{
			ILCKPlugin result;
			this._pluginsByName.TryGetValue(pluginName, out result);
			return result;
		}

		public IEnumerable<ILCKPlugin> GetAllPlugins()
		{
			return this._pluginsByType.Values;
		}

		public IEnumerable<T> GetPluginsOfType<T>() where T : class, ILCKPlugin
		{
			return this._pluginsByType.Values.OfType<T>();
		}

		public void UnregisterPlugin(ILCKPlugin plugin)
		{
			if (plugin == null)
			{
				return;
			}
			Type type = plugin.GetType();
			string pluginName = plugin.PluginName;
			this._pluginsByType.Remove(type);
			this._pluginsByName.Remove(pluginName);
			LckLog.Log(string.Concat(new string[]
			{
				"Unregistered plugin: ",
				pluginName,
				" (",
				type.Name,
				")"
			}), "UnregisterPlugin", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\LCKPlugins.cs", 190);
		}

		public void Clear()
		{
			this._pluginsByType.Clear();
			this._pluginsByName.Clear();
			this._isInitialized = false;
			LckLog.Log("Cleared all registered plugins", "Clear", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\LCKPlugins.cs", 201);
		}

		public int PluginCount
		{
			get
			{
				return this._pluginsByType.Count;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return this._isInitialized;
			}
		}

		private static LCKPlugins _instance;

		private static readonly object _lock = new object();

		private Dictionary<Type, ILCKPlugin> _pluginsByType;

		private Dictionary<string, ILCKPlugin> _pluginsByName;

		private bool _isInitialized;
	}
}
