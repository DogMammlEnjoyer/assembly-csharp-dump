using System;
using System.Collections.Concurrent;
using UnityEngine;

namespace Meta.WitAi.Data.Configuration
{
	public class WitConfigurationCache
	{
		public string GetCacheId(WitConfigurationCacheKey key)
		{
			return key.clientAccessToken + "_" + key.versionTag;
		}

		public WitConfigurationCacheKey GetCacheKey(WitConfiguration configuration)
		{
			return new WitConfigurationCacheKey
			{
				clientAccessToken = ((configuration != null) ? configuration.GetClientAccessToken() : null),
				versionTag = ((configuration != null) ? configuration.GetVersionTag() : null)
			};
		}

		public string GetCacheId(WitConfiguration configuration)
		{
			return this.GetCacheId(this.GetCacheKey(configuration));
		}

		public WitConfiguration Get(WitConfigurationCacheKey key, Action<WitConfiguration> onSetup = null)
		{
			string cacheId = this.GetCacheId(key);
			if (string.IsNullOrEmpty(cacheId))
			{
				return null;
			}
			WitConfiguration result;
			if (this._configurations.TryGetValue(cacheId, out result))
			{
				ConcurrentDictionary<string, int> references = this._references;
				string key2 = cacheId;
				int num = references[key2];
				references[key2] = num + 1;
				return result;
			}
			WitConfiguration witConfiguration = ScriptableObject.CreateInstance<WitConfiguration>();
			witConfiguration.SetClientAccessToken(key.clientAccessToken);
			witConfiguration.editorVersionTag = key.versionTag;
			witConfiguration.buildVersionTag = key.versionTag;
			this._configurations[cacheId] = witConfiguration;
			this._references[cacheId] = 1;
			if (onSetup != null)
			{
				onSetup(witConfiguration);
			}
			return witConfiguration;
		}

		public bool Return(WitConfiguration configuration, Action<WitConfiguration> onDestroy = null)
		{
			if (configuration == null)
			{
				return false;
			}
			string cacheId = this.GetCacheId(configuration);
			int num;
			if (string.IsNullOrEmpty(cacheId) || !this._references.TryGetValue(cacheId, out num))
			{
				return false;
			}
			num--;
			if (num > 0)
			{
				this._references[cacheId] = num;
				return false;
			}
			WitConfiguration witConfiguration;
			this._configurations.TryRemove(cacheId, out witConfiguration);
			int num2;
			this._references.TryRemove(cacheId, out num2);
			if (onDestroy != null)
			{
				onDestroy(configuration);
			}
			Object.Destroy(configuration);
			return true;
		}

		private ConcurrentDictionary<string, WitConfiguration> _configurations = new ConcurrentDictionary<string, WitConfiguration>();

		private ConcurrentDictionary<string, int> _references = new ConcurrentDictionary<string, int>();
	}
}
