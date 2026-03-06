using System;

namespace Liv.Lck
{
	public abstract class LCKPluginBase : ILCKPlugin
	{
		private protected LckService LckService { protected get; private set; }

		private protected bool IsInitialized { protected get; private set; }

		public abstract string PluginName { get; }

		public abstract string PluginVersion { get; }

		protected LCKPluginBase()
		{
			LCKPlugins.Instance.RegisterPlugin(this);
		}

		public void Initialize(LckService lckService)
		{
			if (this.IsInitialized)
			{
				LckLog.LogWarning("Plugin " + this.PluginName + " is already initialized");
				return;
			}
			this.LckService = lckService;
			try
			{
				this.OnInitialize();
				this.IsInitialized = true;
				LckLog.Log("Plugin " + this.PluginName + " initialized successfully");
			}
			catch (Exception ex)
			{
				LckLog.LogError("Failed to initialize plugin " + this.PluginName + ": " + ex.Message);
				throw;
			}
		}

		public void Shutdown()
		{
			if (!this.IsInitialized)
			{
				LckLog.LogWarning("Plugin " + this.PluginName + " is not initialized");
				return;
			}
			try
			{
				this.OnShutdown();
				this.IsInitialized = false;
				this.LckService = null;
				LckLog.Log("Plugin " + this.PluginName + " shutdown successfully");
			}
			catch (Exception ex)
			{
				LckLog.LogError("Failed to shutdown plugin " + this.PluginName + ": " + ex.Message);
				throw;
			}
		}

		protected virtual void OnInitialize()
		{
		}

		protected virtual void OnShutdown()
		{
		}

		protected bool HasPlugin<T>() where T : class, ILCKPlugin
		{
			return LCKPlugins.Instance.HasPlugin<T>();
		}

		protected bool HasPlugin(string pluginName)
		{
			return LCKPlugins.Instance.HasPlugin(pluginName);
		}

		protected T GetPlugin<T>() where T : class, ILCKPlugin
		{
			return LCKPlugins.Instance.GetPlugin<T>();
		}

		protected ILCKPlugin GetPlugin(string pluginName)
		{
			return LCKPlugins.Instance.GetPlugin(pluginName);
		}
	}
}
