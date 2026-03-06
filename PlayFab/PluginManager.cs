using System;
using System.Collections.Generic;
using PlayFab.Internal;
using PlayFab.Json;

namespace PlayFab
{
	public class PluginManager
	{
		private PluginManager()
		{
		}

		public static T GetPlugin<T>(PluginContract contract, string instanceName = "") where T : IPlayFabPlugin
		{
			return (T)((object)PluginManager.Instance.GetPluginInternal(contract, instanceName));
		}

		public static void SetPlugin(IPlayFabPlugin plugin, PluginContract contract, string instanceName = "")
		{
			PluginManager.Instance.SetPluginInternal(plugin, contract, instanceName);
		}

		private IPlayFabPlugin GetPluginInternal(PluginContract contract, string instanceName)
		{
			PluginContractKey key = new PluginContractKey
			{
				_pluginContract = contract,
				_pluginName = instanceName
			};
			IPlayFabPlugin playFabPlugin;
			if (!this.plugins.TryGetValue(key, out playFabPlugin))
			{
				if (contract != PluginContract.PlayFab_Serializer)
				{
					if (contract != PluginContract.PlayFab_Transport)
					{
						throw new ArgumentException("This contract is not supported", "contract");
					}
					playFabPlugin = this.CreatePlayFabTransportPlugin();
				}
				else
				{
					playFabPlugin = this.CreatePlugin<SimpleJsonInstance>();
				}
				this.plugins[key] = playFabPlugin;
			}
			return playFabPlugin;
		}

		private void SetPluginInternal(IPlayFabPlugin plugin, PluginContract contract, string instanceName)
		{
			if (plugin == null)
			{
				throw new ArgumentNullException("plugin", "Plugin instance cannot be null");
			}
			PluginContractKey key = new PluginContractKey
			{
				_pluginContract = contract,
				_pluginName = instanceName
			};
			this.plugins[key] = plugin;
		}

		private IPlayFabPlugin CreatePlugin<T>() where T : IPlayFabPlugin, new()
		{
			return (IPlayFabPlugin)Activator.CreateInstance(typeof(T));
		}

		private ITransportPlugin CreatePlayFabTransportPlugin()
		{
			ITransportPlugin transportPlugin = null;
			if (PlayFabSettings.RequestType == WebRequestType.HttpWebRequest)
			{
				transportPlugin = new PlayFabWebRequest();
			}
			if (transportPlugin == null)
			{
				transportPlugin = new PlayFabUnityHttp();
			}
			return transportPlugin;
		}

		private Dictionary<PluginContractKey, IPlayFabPlugin> plugins = new Dictionary<PluginContractKey, IPlayFabPlugin>(new PluginContractKeyComparator());

		private static readonly PluginManager Instance = new PluginManager();
	}
}
