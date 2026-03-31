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
				LckLog.LogError("Cannot initialize plugins with null LCK service", "InitializePlugins", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\LCKPluginIntegration.cs", 20);
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
					LckLog.LogError("Failed to shutdown plugin " + ilckplugin.PluginName + ": " + ex.Message, "ShutdownPlugins", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\LCKPluginIntegration.cs", 41);
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
			LckLog.Log(string.Format("Registered plugins ({0}):", allPlugins.Count<ILCKPlugin>()), "LogPluginInfo", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\LCKPluginIntegration.cs", 72);
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
				}), "LogPluginInfo", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\LCKPluginIntegration.cs", 76);
			}
		}
	}
}
