using System;
using Meta.WitAi.Data.Configuration;
using Meta.WitAi.Data.Info;

namespace Meta.WitAi
{
	public class WitRuntimeRequestConfiguration : IWitRequestConfiguration, IWitRequestEndpointInfo
	{
		public WitRuntimeRequestConfiguration(string userToken)
		{
			this._userToken = userToken;
			this._appInfo = default(WitAppInfo);
		}

		public string GetConfigurationId()
		{
			return null;
		}

		public string GetApplicationId()
		{
			return this._appInfo.id;
		}

		public WitAppInfo GetApplicationInfo()
		{
			return this._appInfo;
		}

		public WitConfigurationAssetData[] GetConfigData()
		{
			return this._configurationData;
		}

		public IWitRequestEndpointInfo GetEndpointInfo()
		{
			return this;
		}

		public string GetClientAccessToken()
		{
			return this._userToken;
		}

		public void SetClientAccessToken(string newToken)
		{
			this._userToken = newToken;
		}

		public string GetServerAccessToken()
		{
			throw new NotImplementedException();
		}

		public string GetVersionTag()
		{
			return string.Empty;
		}

		public void SetApplicationInfo(WitAppInfo newInfo)
		{
			this._appInfo = newInfo;
		}

		public void SetConfigData(WitConfigurationAssetData[] configData)
		{
			this._configurationData = configData;
		}

		public void UpdateDataAssets()
		{
		}

		public WitRequestType RequestType { get; set; }

		public int RequestTimeoutMs
		{
			get
			{
				return 10000;
			}
		}

		public string UriScheme
		{
			get
			{
				return "https";
			}
		}

		public string Authority
		{
			get
			{
				return "graph.wit.ai/myprofile";
			}
		}

		public string WitApiVersion
		{
			get
			{
				return "20250213";
			}
		}

		public int Port
		{
			get
			{
				return -1;
			}
		}

		public string Message
		{
			get
			{
				return "message";
			}
		}

		public string Speech
		{
			get
			{
				return "speech";
			}
		}

		public string Dictation
		{
			get
			{
				return "dictation";
			}
		}

		public string Synthesize
		{
			get
			{
				return "synthesize";
			}
		}

		public string Event
		{
			get
			{
				return "event";
			}
		}

		public string Converse
		{
			get
			{
				return "converse";
			}
		}

		private WitAppInfo _appInfo;

		private string _userToken;

		private WitConfigurationAssetData[] _configurationData = Array.Empty<WitConfigurationAssetData>();
	}
}
