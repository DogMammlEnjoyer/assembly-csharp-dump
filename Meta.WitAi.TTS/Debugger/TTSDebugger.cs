using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Lib.Wit.Runtime.Utilities.Logging;
using Meta.Voice.Audio;
using Meta.Voice.Logging;
using Meta.WitAi.Json;
using Meta.WitAi.TTS.Data;
using UnityEngine;
using UnityEngine.Events;

namespace Meta.WitAi.TTS.Debugger
{
	public class TTSDebugger : MonoBehaviour, ILogSource
	{
		public IVLogger Logger { get; } = LoggerRegistry.Instance.GetLogger(LogCategory.Logging, null);

		private void Reset()
		{
			if (!this._service)
			{
				this._service = base.gameObject.GetComponent<TTSService>();
			}
		}

		private static void SetupRegex()
		{
			if (TTSDebugger._fileCleanupRegex != null)
			{
				return;
			}
			string str = new string(Path.GetInvalidFileNameChars());
			TTSDebugger._fileCleanupRegex = new Regex("[" + Regex.Escape(str) + "]");
		}

		private void OnEnable()
		{
			if (!this._service)
			{
				this._service = base.gameObject.GetComponentInChildren<TTSService>();
			}
			this.SetListeners(true);
		}

		private void OnDisable()
		{
			this.SetListeners(false);
		}

		private void SetListeners(bool add)
		{
			if (!this._service)
			{
				return;
			}
			this._service.Events.Stream.OnStreamBegin.SetListener(new UnityAction<TTSClipData>(this.OnStreamBegin), add);
			this._service.Events.Stream.OnStreamComplete.SetListener(new UnityAction<TTSClipData>(this.OnStreamComplete), add);
		}

		private string GetClipName(TTSClipData clipData)
		{
			return clipData.clipID + clipData.extension;
		}

		private void OnStreamBegin(TTSClipData clipData)
		{
			string clipName = this.GetClipName(clipData);
			string text = Application.persistentDataPath + "/" + this._outputDirectory;
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			TTSDebugger.SetupRegex();
			string text2 = TTSDebugger._fileCleanupRegex.Replace(clipData.clipID, string.Empty).ToLower();
			DateTime now = DateTime.Now;
			TTSDebugger.TTSDebuggerFileStream ttsdebuggerFileStream = new TTSDebugger.TTSDebuggerFileStream(string.Format("{0}/{1}_{2}_{3:0000}{4:00}{5:00}_{6:00}{7:00}", new object[]
			{
				text,
				text2,
				clipData.extension.Substring(1),
				now.Year,
				now.Month,
				now.Day,
				now.Hour,
				now.Minute
			}));
			ICoreLogger logger = this.Logger;
			string message = "TTS Debugger - Begin\nId: {0}\nText: {1}\nVoice: {2}\nFile Type: {3}\nPath: {4}\n{5}{6}";
			object p = text2;
			object p2 = ((clipData != null) ? clipData.textToSpeak : null) ?? "Null";
			object obj;
			if (clipData == null)
			{
				obj = null;
			}
			else
			{
				TTSVoiceSettings voiceSettings = clipData.voiceSettings;
				obj = ((voiceSettings != null) ? voiceSettings.UniqueId : null);
			}
			logger.Info(message, p, p2, obj ?? "Null", ((clipData != null) ? clipData.extension : null) ?? "Null", ttsdebuggerFileStream.FilePath, ".\\Library\\PackageCache\\com.meta.xr.sdk.voice@d3f6f37b8e1c\\Lib\\Wit.ai\\Features\\TTS\\Scripts\\Runtime\\Debugger\\TTSDebugger.cs", 142);
			IAudioClipStream clipStream = clipData.clipStream;
			clipStream.OnAddSamples = (AudioClipStreamSampleDelegate)Delegate.Combine(clipStream.OnAddSamples, new AudioClipStreamSampleDelegate(ttsdebuggerFileStream.AddSamples));
			clipData.Events.OnEventJsonAdded += ttsdebuggerFileStream.AddEvent;
			this._streams[clipName] = ttsdebuggerFileStream;
		}

		private void OnStreamComplete(TTSClipData clipData)
		{
			string clipName = this.GetClipName(clipData);
			TTSDebugger.TTSDebuggerFileStream ttsdebuggerFileStream;
			if (!this._streams.TryRemove(clipName, out ttsdebuggerFileStream))
			{
				return;
			}
			WitResponseClass witResponseClass = new WitResponseClass();
			witResponseClass["requestId"] = new WitResponseData(((clipData != null) ? clipData.queryRequestId : null) ?? "Null");
			WitResponseNode witResponseNode = witResponseClass;
			string aKey = "fileType";
			string text;
			if (clipData == null)
			{
				text = null;
			}
			else
			{
				string extension = clipData.extension;
				text = ((extension != null) ? extension.Substring(1) : null);
			}
			witResponseNode[aKey] = new WitResponseData(text ?? "Null");
			witResponseClass["clipId"] = new WitResponseData(((clipData != null) ? clipData.clipID : null) ?? "Null");
			witResponseClass["textToSpeak"] = new WitResponseData(((clipData != null) ? clipData.textToSpeak : null) ?? "Null");
			witResponseClass["readyDuration"] = new WitResponseData(string.Format("{0:0.00} seconds", (clipData != null) ? new float?(clipData.readyDuration) : null));
			witResponseClass["completeDuration"] = new WitResponseData(string.Format("{0:0.00} seconds", (clipData != null) ? new float?(clipData.completeDuration) : null));
			WitResponseNode witResponseNode2 = witResponseClass;
			string aKey2 = "length";
			string format = "{0:0.00} seconds";
			float? num;
			if (clipData == null)
			{
				num = null;
			}
			else
			{
				IAudioClipStream clipStream = clipData.clipStream;
				num = ((clipStream != null) ? new float?(clipStream.Length) : null);
			}
			witResponseNode2[aKey2] = new WitResponseData(string.Format(format, num));
			WitResponseClass witResponseClass2 = new WitResponseClass();
			foreach (KeyValuePair<string, string> keyValuePair in ((clipData != null) ? clipData.voiceSettings.EncodedValues : null))
			{
				witResponseClass2[keyValuePair.Key] = new WitResponseData(keyValuePair.Value);
			}
			witResponseClass["voiceSettings"] = witResponseClass2;
			WitResponseArray witResponseArray = new WitResponseArray();
			for (int i = 0; i < ttsdebuggerFileStream.EventNodes.Count; i++)
			{
				witResponseArray[i] = ttsdebuggerFileStream.EventNodes[i];
			}
			witResponseClass["events"] = witResponseArray;
			string contents = witResponseClass.ToString();
			ICoreLogger logger = this.Logger;
			string message = "TTS Debugger - Complete\nText: {0}\nVoice: {1}\nFile Type: {2}\nPath: {3}";
			object p = ((clipData != null) ? clipData.textToSpeak : null) ?? "Null";
			object obj;
			if (clipData == null)
			{
				obj = null;
			}
			else
			{
				TTSVoiceSettings voiceSettings = clipData.voiceSettings;
				obj = ((voiceSettings != null) ? voiceSettings.UniqueId : null);
			}
			logger.Info(message, p, obj ?? "Null", ((clipData != null) ? clipData.extension : null) ?? "Null", ttsdebuggerFileStream.FilePath, "OnStreamComplete", ".\\Library\\PackageCache\\com.meta.xr.sdk.voice@d3f6f37b8e1c\\Lib\\Wit.ai\\Features\\TTS\\Scripts\\Runtime\\Debugger\\TTSDebugger.cs", 186);
			string path = ttsdebuggerFileStream.FilePath + ".json";
			if (File.Exists(path))
			{
				File.Delete(path);
			}
			File.WriteAllText(path, contents);
			IAudioClipStream clipStream2 = clipData.clipStream;
			clipStream2.OnAddSamples = (AudioClipStreamSampleDelegate)Delegate.Remove(clipStream2.OnAddSamples, new AudioClipStreamSampleDelegate(ttsdebuggerFileStream.AddSamples));
			clipData.Events.OnEventJsonAdded -= ttsdebuggerFileStream.AddEvent;
			ttsdebuggerFileStream.Dispose();
		}

		[Tooltip("The TTS service that will generate tts output files")]
		[SerializeField]
		private TTSService _service;

		[Tooltip("The location within the Assets directory that will output all tts files")]
		[SerializeField]
		private string _outputDirectory = "TtsDebugger";

		private static Regex _fileCleanupRegex;

		private ConcurrentDictionary<string, TTSDebugger.TTSDebuggerFileStream> _streams = new ConcurrentDictionary<string, TTSDebugger.TTSDebuggerFileStream>();

		private class TTSDebuggerFileStream
		{
			public TTSDebuggerFileStream(string filePath)
			{
				this.FilePath = filePath;
				string path = this.FilePath + ".raw";
				if (File.Exists(path))
				{
					File.Delete(path);
				}
				this._audioStream = new FileStream(path, FileMode.Create);
				this.EventNodes = new List<WitResponseNode>();
			}

			public void AddSamples(float[] samples, int offset, int length)
			{
				for (int i = 0; i < length; i++)
				{
					short value = (short)Mathf.Clamp(samples[offset + i] * 32767f, -32768f, 32767f);
					this._audioStream.Write(BitConverter.GetBytes(value));
				}
			}

			public void AddEvent(WitResponseNode ttsEvent)
			{
				this.EventNodes.Add(ttsEvent);
			}

			public void Dispose()
			{
				this.EventNodes.Clear();
				this._audioStream.Close();
			}

			public readonly string FilePath;

			public readonly List<WitResponseNode> EventNodes;

			private readonly FileStream _audioStream;
		}
	}
}
