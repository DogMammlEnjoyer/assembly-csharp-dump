using System;
using System.Collections.Generic;
using Meta.WitAi.Json;
using Meta.WitAi.TTS.Data;
using UnityEngine;

namespace Meta.WitAi.TTS.Integrations
{
	[Serializable]
	public class TTSWitVoiceSettings : TTSVoiceSettings
	{
		public override string UniqueId
		{
			get
			{
				if (string.IsNullOrEmpty(this._uniqueId))
				{
					this.RefreshUniqueId();
				}
				return this._uniqueId;
			}
		}

		public void RefreshUniqueId()
		{
			this._uniqueId = string.Format("{0}|{1}|{2:00}|{3:00}", new object[]
			{
				this.voice,
				this.style,
				this.speed,
				this.pitch
			});
		}

		public override Dictionary<string, string> EncodedValues
		{
			get
			{
				if (this._encoded.Keys.Count == 0)
				{
					this.RefreshEncodedValues();
				}
				return this._encoded;
			}
		}

		public void RefreshEncodedValues()
		{
			this._encoded.Clear();
			this._encoded[WitConstants.TTS_VOICE] = (string.IsNullOrEmpty(this.voice) ? "Charlie" : this.voice);
			this._encoded[WitConstants.TTS_STYLE] = (string.IsNullOrEmpty(this.style) ? "default" : this.style);
			int num = Mathf.Clamp(this.speed, 50, 200);
			if (num != 100)
			{
				this._encoded[WitConstants.TTS_SPEED] = num.ToString();
			}
			num = Mathf.Clamp(this.pitch, 25, 200);
			if (num != 100)
			{
				this._encoded[WitConstants.TTS_PITCH] = num.ToString();
			}
		}

		public static bool CanDecode(WitResponseNode responseNode)
		{
			WitResponseClass witResponseClass = (responseNode != null) ? responseNode.AsObject : null;
			return witResponseClass != null && witResponseClass.HasChild("q") && witResponseClass.HasChild(WitConstants.TTS_VOICE);
		}

		public override bool SerializeObject(WitResponseClass jsonObject)
		{
			this.RefreshEncodedValues();
			Dictionary<string, string> encodedValues = this.EncodedValues;
			if (encodedValues == null)
			{
				return false;
			}
			foreach (KeyValuePair<string, string> keyValuePair in encodedValues)
			{
				jsonObject[keyValuePair.Key] = new WitResponseData(keyValuePair.Value);
			}
			return true;
		}

		public override bool DeserializeObject(WitResponseClass jsonObject)
		{
			this.voice = this.DecodeString(jsonObject, WitConstants.TTS_VOICE, "Charlie");
			this.style = this.DecodeString(jsonObject, WitConstants.TTS_STYLE, "default");
			this.speed = this.DecodeInt(jsonObject, WitConstants.TTS_SPEED, 100, 50, 200);
			this.pitch = this.DecodeInt(jsonObject, WitConstants.TTS_PITCH, 100, 25, 200);
			this.RefreshUniqueId();
			this.RefreshEncodedValues();
			this.SettingsId = this.UniqueId;
			return !string.IsNullOrEmpty(this.voice);
		}

		private string DecodeString(WitResponseClass responseClass, string id, string defaultValue)
		{
			if (responseClass.HasChild(id))
			{
				return responseClass[id];
			}
			return defaultValue;
		}

		private int DecodeInt(WitResponseClass responseClass, string id, int defaultValue, int minValue, int maxValue)
		{
			if (responseClass.HasChild(id))
			{
				return Mathf.Clamp(responseClass[id].AsInt, minValue, maxValue);
			}
			return defaultValue;
		}

		public string voice = "Charlie";

		public string style = "default";

		[Range(50f, 200f)]
		public int speed = 100;

		[Range(25f, 200f)]
		public int pitch = 100;

		private string _uniqueId;

		private Dictionary<string, string> _encoded = new Dictionary<string, string>();
	}
}
