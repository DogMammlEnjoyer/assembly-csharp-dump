using System;
using GorillaNetworking;
using PlayFab;
using UnityEngine;

internal static class SpeakerVoiceToLoudnessConfig
{
	public static bool EnableLoudnessLimit
	{
		get
		{
			return SpeakerVoiceToLoudnessConfig.k_config.EnableLoudnessLimit;
		}
	}

	public static float LoudnessLimitThreshold
	{
		get
		{
			return SpeakerVoiceToLoudnessConfig.k_config.LoudnessLimitThreshold;
		}
	}

	[RuntimeInitializeOnLoadMethod]
	private static void StaticLoad()
	{
		PlayFabTitleDataCache.RegisterOnLoad(new Action<PlayFabTitleDataCache>(SpeakerVoiceToLoudnessConfig.OnTitleDataCacheReady));
	}

	private static void OnTitleDataCacheReady(PlayFabTitleDataCache titleDataCache)
	{
		titleDataCache.GetTitleData("SpeakerVoiceToLoudnessConfig", new Action<string>(SpeakerVoiceToLoudnessConfig.OnTitleDataCacheRespsonse), new Action<PlayFabError>(SpeakerVoiceToLoudnessConfig.OnTitleDataCacheError), false);
	}

	private static void OnTitleDataCacheRespsonse(string json)
	{
		SpeakerVoiceToLoudnessConfig.SerializedConfig serializedConfig = default(SpeakerVoiceToLoudnessConfig.SerializedConfig);
		try
		{
			serializedConfig = JsonUtility.FromJson<SpeakerVoiceToLoudnessConfig.SerializedConfig>(json);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			serializedConfig = SpeakerVoiceToLoudnessConfig.k_config;
		}
		finally
		{
			SpeakerVoiceToLoudnessConfig.k_config = serializedConfig;
		}
	}

	private static void OnTitleDataCacheError(PlayFabError errorMsg)
	{
	}

	private static SpeakerVoiceToLoudnessConfig.SerializedConfig k_config = new SpeakerVoiceToLoudnessConfig.SerializedConfig
	{
		EnableLoudnessLimit = true,
		LoudnessLimitThreshold = 0.5f
	};

	public static StaticArrayBag<float> StaticArrays = new StaticArrayBag<float>();

	private const string k_titleDataKey = "SpeakerVoiceToLoudnessConfig";

	[Serializable]
	private struct SerializedConfig
	{
		public bool EnableLoudnessLimit;

		public float LoudnessLimitThreshold;
	}
}
