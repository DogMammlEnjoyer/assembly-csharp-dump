using System;
using System.Collections.Generic;
using System.Linq;

namespace Liv.Lck
{
	public static class LCKPluginIntegration
	{
		public static void InitializePlugins(LckService lckService)
		{
			if (lckService == null)
			{
				LckLog.LogError("Cannot initialize plugins with null LCK service");
				return;
			}
			LCKPlugins.Instance.Initialize(lckService);
		}

		public static void ShutdownPlugins()
		{
			foreach (ILCKPlugin ilckplugin in LCKPlugins.Instance.GetAllPlugins())
			{
				try
				{
					ilckplugin.Shutdown();
				}
				catch (Exception ex)
				{
					LckLog.LogError("Failed to shutdown plugin " + ilckplugin.PluginName + ": " + ex.Message);
				}
			}
		}

		public static T GetPlugin<T>() where T : class, ILCKPlugin
		{
			return LCKPlugins.Instance.GetPlugin<T>();
		}

		public static bool HasPlugin<T>() where T : class, ILCKPlugin
		{
			return LCKPlugins.Instance.HasPlugin<T>();
		}

		public static void LogPluginInfo()
		{
			IEnumerable<ILCKPlugin> allPlugins = LCKPlugins.Instance.GetAllPlugins();
			LckLog.Log(string.Format("Registered plugins ({0}):", allPlugins.Count<ILCKPlugin>()));
			foreach (ILCKPlugin ilckplugin in allPlugins)
			{
				LckLog.Log(string.Concat(new string[]
				{
					"  - ",
					ilckplugin.PluginName,
					" v",
					ilckplugin.PluginVersion,
					" (",
					ilckplugin.GetType().Name,
					")"
				}));
			}
		}
	}
}
