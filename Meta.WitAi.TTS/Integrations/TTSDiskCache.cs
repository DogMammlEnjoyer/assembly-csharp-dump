using System;
using System.IO;
using Meta.Voice.Logging;
using Meta.WitAi.TTS.Data;
using Meta.WitAi.TTS.Interfaces;
using Meta.WitAi.Utilities;
using UnityEngine;

namespace Meta.WitAi.TTS.Integrations
{
	[LogCategory(LogCategory.TextToSpeech)]
	public class TTSDiskCache : MonoBehaviour, ITTSDiskCacheHandler
	{
		public string DiskPath
		{
			get
			{
				return this._diskPath;
			}
		}

		public TTSDiskCacheSettings DiskCacheDefaultSettings
		{
			get
			{
				return this._defaultSettings;
			}
		}

		public string GetDiskCachePath(TTSClipData clipData)
		{
			if (!this.ShouldCacheToDisk(clipData))
			{
				return string.Empty;
			}
			TTSDiskCacheLocation diskCacheLocation = clipData.diskCacheSettings.DiskCacheLocation;
			string text = string.Empty;
			switch (diskCacheLocation)
			{
			case TTSDiskCacheLocation.Preload:
				text = Application.streamingAssetsPath;
				break;
			case TTSDiskCacheLocation.Persistent:
				text = Application.persistentDataPath;
				break;
			case TTSDiskCacheLocation.Temporary:
				text = Application.temporaryCachePath;
				break;
			}
			if (string.IsNullOrEmpty(text))
			{
				return string.Empty;
			}
			text = Path.Combine(text, this.DiskPath);
			if ((diskCacheLocation != TTSDiskCacheLocation.Preload || !Application.isPlaying) && !IOUtility.CreateDirectory(text, true))
			{
				VLog.E(string.Format("Failed to create tts directory\nPath: {0}\nLocation: {1}", text, diskCacheLocation), null);
				return string.Empty;
			}
			return Path.Combine(text, clipData.clipID + clipData.extension);
		}

		public bool ShouldCacheToDisk(TTSClipData clipData)
		{
			return clipData != null && clipData.diskCacheSettings.DiskCacheLocation != TTSDiskCacheLocation.Stream && !string.IsNullOrEmpty(clipData.clipID);
		}

		[Header("Disk Cache Settings")]
		[SerializeField]
		private string _diskPath = "TTS/";

		[SerializeField]
		private TTSDiskCacheSettings _defaultSettings = new TTSDiskCacheSettings();
	}
}
