using System;
using Meta.WitAi.Data.Configuration;
using UnityEngine;
using UnityEngine.Serialization;

namespace Meta.WitAi.Configuration
{
	[Serializable]
	public class WitEndpointConfig : IWitRequestEndpointInfo
	{
		public string UriScheme
		{
			get
			{
				if (!string.IsNullOrEmpty(this._uriScheme))
				{
					return this._uriScheme;
				}
				return "https";
			}
		}

		public string Authority
		{
			get
			{
				if (!string.IsNullOrEmpty(this._authority))
				{
					return this._authority;
				}
				return "api.wit.ai";
			}
		}

		public int Port
		{
			get
			{
				if (this._port > 0)
				{
					return this._port;
				}
				return -1;
			}
		}

		public string WitApiVersion
		{
			get
			{
				if (!string.IsNullOrEmpty(this._witApiVersion))
				{
					return this._witApiVersion;
				}
				return "20250213";
			}
		}

		public string Message
		{
			get
			{
				if (!string.IsNullOrEmpty(this._message))
				{
					return this._message;
				}
				return "message";
			}
		}

		public string Speech
		{
			get
			{
				if (!string.IsNullOrEmpty(this._speech))
				{
					return this._speech;
				}
				return "speech";
			}
		}

		public string Dictation
		{
			get
			{
				if (!string.IsNullOrEmpty(this._dictation))
				{
					return this._dictation;
				}
				return "dictation";
			}
		}

		public string Synthesize
		{
			get
			{
				if (!string.IsNullOrEmpty(this._synthesize))
				{
					return this._synthesize;
				}
				return "synthesize";
			}
		}

		public string Event
		{
			get
			{
				if (!string.IsNullOrEmpty(this._event))
				{
					return this._event;
				}
				return "event";
			}
		}

		public string Converse
		{
			get
			{
				if (!string.IsNullOrEmpty(this._converse))
				{
					return this._converse;
				}
				return "converse";
			}
		}

		public static WitEndpointConfig GetEndpointConfig(WitConfiguration witConfig)
		{
			if (!witConfig || witConfig.endpointConfiguration == null)
			{
				return WitEndpointConfig.defaultEndpointConfig;
			}
			return witConfig.endpointConfiguration;
		}

		[SerializeField]
		[FormerlySerializedAs("uriScheme")]
		private string _uriScheme;

		[SerializeField]
		[FormerlySerializedAs("authority")]
		private string _authority;

		[SerializeField]
		[FormerlySerializedAs("port")]
		private int _port;

		[SerializeField]
		[FormerlySerializedAs("witApiVersion")]
		private string _witApiVersion;

		[SerializeField]
		[FormerlySerializedAs("message")]
		private string _message;

		[SerializeField]
		[FormerlySerializedAs("speech")]
		private string _speech;

		[SerializeField]
		[FormerlySerializedAs("dictation")]
		private string _dictation;

		[SerializeField]
		private string _synthesize;

		[SerializeField]
		private string _event;

		[SerializeField]
		private string _converse;

		private static WitEndpointConfig defaultEndpointConfig = new WitEndpointConfig();
	}
}
