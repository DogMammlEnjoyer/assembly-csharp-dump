using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Lib.Wit.Runtime.Utilities.Logging;
using Meta.Voice.Audio;
using Meta.Voice.Logging;
using Meta.WitAi.Attributes;
using Meta.WitAi.Json;
using Meta.WitAi.Requests;
using Meta.WitAi.TTS.Data;
using Meta.WitAi.TTS.Events;
using Meta.WitAi.TTS.Integrations;
using Meta.WitAi.TTS.Interfaces;
using Meta.WitAi.Utilities;
using UnityEngine;

namespace Meta.WitAi.TTS
{
	[LogCategory(LogCategory.TextToSpeech)]
	public abstract class TTSService : MonoBehaviour, ILogSource
	{
		public IVLogger Logger { get; } = LoggerRegistry.Instance.GetLogger(LogCategory.TextToSpeech, null);

		public static TTSService Instance
		{
			get
			{
				if (TTSService._instance == null)
				{
					TTSService._instance = GameObjectSearchUtility.FindSceneObject<TTSService>(true);
				}
				return TTSService._instance;
			}
		}

		public IAudioSystem AudioSystem
		{
			get
			{
				return this._audioSystem as IAudioSystem;
			}
			set
			{
				this._audioSystem = this.SetInterface<IAudioSystem>(value);
			}
		}

		public ITTSRuntimeCacheHandler RuntimeCacheHandler
		{
			get
			{
				return this._runtimeCacheHandler as ITTSRuntimeCacheHandler;
			}
			set
			{
				this._runtimeCacheHandler = this.SetInterface<ITTSRuntimeCacheHandler>(value);
			}
		}

		public ITTSDiskCacheHandler DiskCacheHandler
		{
			get
			{
				return this._diskCacheHandler as ITTSDiskCacheHandler;
			}
			set
			{
				this._diskCacheHandler = this.SetInterface<ITTSDiskCacheHandler>(value);
			}
		}

		public abstract ITTSWebHandler WebHandler { get; }

		public abstract ITTSVoiceProvider VoiceProvider { get; }

		public static event Action<TTSService> OnServiceStart;

		public static event Action<TTSService> OnServiceDestroy;

		public TTSServiceEvents Events
		{
			get
			{
				return this._events;
			}
		}

		public virtual string GetInvalidError()
		{
			if (this.WebHandler == null)
			{
				return "Web Handler Missing";
			}
			if (this.VoiceProvider == null)
			{
				return "Voice Provider Missing";
			}
			return string.Empty;
		}

		protected virtual void Awake()
		{
			TTSService._instance = this;
		}

		protected virtual void Start()
		{
			Action<TTSService> onServiceStart = TTSService.OnServiceStart;
			if (onServiceStart == null)
			{
				return;
			}
			onServiceStart(this);
		}

		protected virtual void OnEnable()
		{
			this._isActive = true;
			this.SetListeners(true);
			string invalidError = this.GetInvalidError();
			if (!string.IsNullOrEmpty(invalidError))
			{
				this.Log(invalidError, null, VLoggerVerbosity.Warning);
			}
		}

		protected virtual void OnDisable()
		{
			this._isActive = false;
			this.SetListeners(false);
		}

		protected virtual void SetListeners(bool add)
		{
			if (this._hasListeners == add)
			{
				return;
			}
			this._hasListeners = add;
			if (add)
			{
				this.AudioSystem = this.GetOrCreateInterface<IAudioSystem, UnityAudioSystem>(this.AudioSystem);
				this.RuntimeCacheHandler = this.GetOrCreateInterface<ITTSRuntimeCacheHandler, TTSRuntimeLRUCache>(this.RuntimeCacheHandler);
				this.DiskCacheHandler = this.GetInterface<ITTSDiskCacheHandler>(this.DiskCacheHandler);
			}
			if (this.RuntimeCacheHandler != null)
			{
				if (add)
				{
					this.RuntimeCacheHandler.OnClipAdded += this.OnRuntimeClipAdded;
					this.RuntimeCacheHandler.OnClipRemoved += this.OnRuntimeClipRemoved;
					return;
				}
				this.RuntimeCacheHandler.OnClipAdded -= this.OnRuntimeClipAdded;
				this.RuntimeCacheHandler.OnClipRemoved -= this.OnRuntimeClipRemoved;
			}
		}

		protected TInterface GetInterface<TInterface>(TInterface current)
		{
			Object @object = current as Object;
			if (@object != null && @object)
			{
				return current;
			}
			return base.gameObject.GetComponent<TInterface>();
		}

		protected TInterface GetOrCreateInterface<TInterface, TDefault>(TInterface current) where TDefault : MonoBehaviour, TInterface
		{
			TInterface @interface = this.GetInterface<TInterface>(current);
			Object @object = @interface as Object;
			if (@object != null && @object)
			{
				return @interface;
			}
			return (TInterface)((object)base.gameObject.AddComponent<TDefault>());
		}

		private Object SetInterface<TInterface>(TInterface newValue)
		{
			Object @object = newValue as Object;
			if (@object != null)
			{
				return @object;
			}
			if (newValue != null)
			{
				this.Logger.Error("Set {0} Failed\nCannot set {1} to a UnityEngine.Object property", new object[]
				{
					typeof(TInterface).Name,
					newValue.GetType().Name
				});
			}
			return null;
		}

		protected virtual void OnDestroy()
		{
			if (TTSService._instance == this)
			{
				TTSService._instance = null;
			}
			this.UnloadAll();
			Action<TTSService> onServiceDestroy = TTSService.OnServiceDestroy;
			if (onServiceDestroy == null)
			{
				return;
			}
			onServiceDestroy(this);
		}

		private void Log(string logMessage, TTSClipData clipData = null, VLoggerVerbosity logLevel = VLoggerVerbosity.Verbose)
		{
			this.Logger.Log(this.Logger.CorrelationID, logLevel, "{0}\n{1}", new object[]
			{
				logMessage,
				(clipData == null) ? "" : clipData
			});
		}

		private void LogState(TTSClipData clipData, string message, bool fromDisk, string error = null)
		{
			if (!string.IsNullOrEmpty(error))
			{
				ICoreLogger logger = this.Logger;
				string message2 = "{0} {1}\nText: {2}\nVoice: {3}\nReady: {4:0.00} seconds\nRequest Id: {5}\nError: {6}";
				object[] array = new object[7];
				array[0] = (fromDisk ? "Disk" : "Web");
				array[1] = message;
				array[2] = (((clipData != null) ? clipData.textToSpeak : null) ?? "Null");
				int num = 3;
				object obj;
				if (clipData == null)
				{
					obj = null;
				}
				else
				{
					TTSVoiceSettings voiceSettings = clipData.voiceSettings;
					obj = ((voiceSettings != null) ? voiceSettings.SettingsId : null);
				}
				array[num] = (obj ?? "Null");
				array[4] = ((clipData != null) ? clipData.readyDuration : 0f);
				array[5] = (((clipData != null) ? clipData.queryRequestId : null) ?? "Null");
				array[6] = error;
				logger.Warning(message2, array);
				return;
			}
			if (this.verboseLogging)
			{
				ICoreLogger logger2 = this.Logger;
				string message3 = "{0} {1}\nText: {2}\nVoice: {3}\nReady: {4:0.00} seconds\nRequest Id: {5}";
				object[] array2 = new object[6];
				array2[0] = (fromDisk ? "Disk" : "Web");
				array2[1] = message;
				array2[2] = (((clipData != null) ? clipData.textToSpeak : null) ?? "Null");
				int num2 = 3;
				object obj2;
				if (clipData == null)
				{
					obj2 = null;
				}
				else
				{
					TTSVoiceSettings voiceSettings2 = clipData.voiceSettings;
					obj2 = ((voiceSettings2 != null) ? voiceSettings2.SettingsId : null);
				}
				array2[num2] = (obj2 ?? "Null");
				array2[4] = ((clipData != null) ? clipData.readyDuration : 0f);
				array2[5] = (((clipData != null) ? clipData.queryRequestId : null) ?? "Null");
				logger2.Verbose(message3, array2);
			}
		}

		public virtual string GetFinalText(string textToSpeak, TTSVoiceSettings voiceSettings)
		{
			if (voiceSettings == null)
			{
				ITTSVoiceProvider voiceProvider = this.VoiceProvider;
				voiceSettings = ((voiceProvider != null) ? voiceProvider.VoiceDefaultSettings : null);
			}
			if (voiceSettings == null || string.IsNullOrEmpty(textToSpeak) || (string.IsNullOrEmpty(voiceSettings.PrependedText) && string.IsNullOrEmpty(voiceSettings.AppendedText)))
			{
				return textToSpeak;
			}
			return voiceSettings.PrependedText + textToSpeak + voiceSettings.AppendedText;
		}

		public string GetClipID(string textToSpeak, TTSVoiceSettings voiceSettings)
		{
			string finalText = this.GetFinalText(textToSpeak, voiceSettings);
			return this.GetClipIDWithFinalText(finalText, voiceSettings);
		}

		protected virtual string GetClipIDWithFinalText(string formattedText, TTSVoiceSettings voiceSettings)
		{
			if (string.IsNullOrEmpty(formattedText))
			{
				return "EMPTY";
			}
			string text = formattedText;
			ITTSVoiceProvider voiceProvider = this.VoiceProvider;
			if (((voiceProvider != null) ? voiceProvider.PresetVoiceSettings : null) != null && this.VoiceProvider.PresetVoiceSettings.Length != 0)
			{
				if (voiceSettings == null)
				{
					ITTSVoiceProvider voiceProvider2 = this.VoiceProvider;
					voiceSettings = ((voiceProvider2 != null) ? voiceProvider2.VoiceDefaultSettings : null);
				}
				if (voiceSettings != null)
				{
					text = text + "|" + voiceSettings.UniqueId;
				}
			}
			if (this.DiskCacheHandler != null)
			{
				int hashCode = text.GetHashCode();
				text = string.Format("tts_{0}{1}", (hashCode < 0) ? "n" : "p", Mathf.Abs(hashCode));
			}
			return text;
		}

		public TTSClipData GetClipData(string textToSpeak, TTSVoiceSettings voiceSettings, TTSDiskCacheSettings diskCacheSettings)
		{
			this.SetListeners(true);
			if (voiceSettings == null)
			{
				ITTSVoiceProvider voiceProvider = this.VoiceProvider;
				voiceSettings = ((voiceProvider != null) ? voiceProvider.VoiceDefaultSettings : null);
			}
			if (diskCacheSettings == null)
			{
				ITTSDiskCacheHandler diskCacheHandler = this.DiskCacheHandler;
				diskCacheSettings = ((diskCacheHandler != null) ? diskCacheHandler.DiskCacheDefaultSettings : null);
			}
			string finalText = this.GetFinalText(textToSpeak, voiceSettings);
			string clipIDWithFinalText = this.GetClipIDWithFinalText(finalText, voiceSettings);
			TTSClipData runtimeCachedClip = this.GetRuntimeCachedClip(clipIDWithFinalText);
			if (runtimeCachedClip != null && string.Equals(runtimeCachedClip.clipID, clipIDWithFinalText))
			{
				return runtimeCachedClip;
			}
			return this.WebHandler.CreateClipData(clipIDWithFinalText, finalText, voiceSettings, diskCacheSettings);
		}

		protected virtual void SetClipLoadState(TTSClipData clipData, TTSClipLoadState loadState)
		{
			clipData.loadState = loadState;
			this.RaiseEvents(delegate
			{
				Action<TTSClipData, TTSClipLoadState> onStateChange = clipData.onStateChange;
				if (onStateChange == null)
				{
					return;
				}
				onStateChange(clipData, clipData.loadState);
			});
		}

		public bool DecodeTts(WitResponseNode responseNode, out string textToSpeak, out TTSVoiceSettings voiceSettings)
		{
			return this.WebHandler.DecodeTtsFromJson(responseNode, out textToSpeak, out voiceSettings);
		}

		public TTSClipData Load(string textToSpeak, string presetVoiceId = null, TTSDiskCacheSettings diskCacheSettings = null, Action<TTSClipData> onStreamReady = null, Action<TTSClipData, string> onStreamComplete = null)
		{
			return this.Load(textToSpeak, this.GetPresetVoiceSettings(presetVoiceId), diskCacheSettings, onStreamReady, onStreamComplete);
		}

		public TTSClipData Load(string textToSpeak, TTSVoiceSettings voiceSettings, TTSDiskCacheSettings diskCacheSettings = null, Action<TTSClipData> onStreamReady = null, Action<TTSClipData, string> onStreamComplete = null)
		{
			TTSClipData clipData = this.GetClipData(textToSpeak, voiceSettings, diskCacheSettings);
			this.LoadAsync(clipData, onStreamReady, onStreamComplete).WrapErrors();
			return clipData;
		}

		public Task<string> LoadAsync(TTSClipData clipData, Action<TTSClipData> onStreamReady = null, Action<TTSClipData, string> onStreamComplete = null)
		{
			TTSService.<LoadAsync>d__54 <LoadAsync>d__;
			<LoadAsync>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
			<LoadAsync>d__.<>4__this = this;
			<LoadAsync>d__.clipData = clipData;
			<LoadAsync>d__.onStreamReady = onStreamReady;
			<LoadAsync>d__.onStreamComplete = onStreamComplete;
			<LoadAsync>d__.<>1__state = -1;
			<LoadAsync>d__.<>t__builder.Start<TTSService.<LoadAsync>d__54>(ref <LoadAsync>d__);
			return <LoadAsync>d__.<>t__builder.Task;
		}

		private Task<string> PerformDownloadAndStream(TTSClipData clipData)
		{
			TTSService.<PerformDownloadAndStream>d__55 <PerformDownloadAndStream>d__;
			<PerformDownloadAndStream>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
			<PerformDownloadAndStream>d__.<>4__this = this;
			<PerformDownloadAndStream>d__.clipData = clipData;
			<PerformDownloadAndStream>d__.<>1__state = -1;
			<PerformDownloadAndStream>d__.<>t__builder.Start<TTSService.<PerformDownloadAndStream>d__55>(ref <PerformDownloadAndStream>d__);
			return <PerformDownloadAndStream>d__.<>t__builder.Task;
		}

		private Task<string> PerformStreamFromWeb(TTSClipData clipData)
		{
			TTSService.<PerformStreamFromWeb>d__56 <PerformStreamFromWeb>d__;
			<PerformStreamFromWeb>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
			<PerformStreamFromWeb>d__.<>4__this = this;
			<PerformStreamFromWeb>d__.clipData = clipData;
			<PerformStreamFromWeb>d__.<>1__state = -1;
			<PerformStreamFromWeb>d__.<>t__builder.Start<TTSService.<PerformStreamFromWeb>d__56>(ref <PerformStreamFromWeb>d__);
			return <PerformStreamFromWeb>d__.<>t__builder.Task;
		}

		private Task<string> PerformStreamFromDisk(TTSClipData clipData)
		{
			TTSService.<PerformStreamFromDisk>d__57 <PerformStreamFromDisk>d__;
			<PerformStreamFromDisk>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
			<PerformStreamFromDisk>d__.<>4__this = this;
			<PerformStreamFromDisk>d__.clipData = clipData;
			<PerformStreamFromDisk>d__.<>1__state = -1;
			<PerformStreamFromDisk>d__.<>t__builder.Start<TTSService.<PerformStreamFromDisk>d__57>(ref <PerformStreamFromDisk>d__);
			return <PerformStreamFromDisk>d__.<>t__builder.Task;
		}

		public void UnloadAll()
		{
			ITTSRuntimeCacheHandler runtimeCacheHandler = this.RuntimeCacheHandler;
			TTSClipData[] array = (runtimeCacheHandler != null) ? runtimeCacheHandler.GetClips() : null;
			if (array == null)
			{
				return;
			}
			foreach (TTSClipData clipData in new HashSet<TTSClipData>(array))
			{
				this.Unload(clipData);
			}
		}

		public void Unload(TTSClipData clipData)
		{
			if (this.RuntimeCacheHandler != null)
			{
				this.RuntimeCacheHandler.RemoveClip(clipData.clipID);
				return;
			}
			this.RaiseUnloadComplete(clipData, false);
		}

		public TTSClipData GetRuntimeCachedClip(string clipID)
		{
			ITTSRuntimeCacheHandler runtimeCacheHandler = this.RuntimeCacheHandler;
			if (runtimeCacheHandler == null)
			{
				return null;
			}
			return runtimeCacheHandler.GetClip(clipID);
		}

		public TTSClipData[] GetAllRuntimeCachedClips()
		{
			ITTSRuntimeCacheHandler runtimeCacheHandler = this.RuntimeCacheHandler;
			if (runtimeCacheHandler == null)
			{
				return null;
			}
			return runtimeCacheHandler.GetClips();
		}

		protected virtual void OnRuntimeClipAdded(TTSClipData clipData)
		{
			this.RaiseLoadBegin(clipData, false);
		}

		protected virtual void OnRuntimeClipRemoved(TTSClipData clipData)
		{
			this.RaiseUnloadComplete(clipData, false);
		}

		public bool ShouldCacheToDisk(TTSClipData clipData)
		{
			return this.DiskCacheHandler != null && this.DiskCacheHandler.ShouldCacheToDisk(clipData) && !string.IsNullOrEmpty(clipData.textToSpeak);
		}

		public string GetDiskCachePath(string textToSpeak, string clipID, TTSVoiceSettings voiceSettings, TTSDiskCacheSettings diskCacheSettings)
		{
			ITTSDiskCacheHandler diskCacheHandler = this.DiskCacheHandler;
			if (diskCacheHandler == null)
			{
				return null;
			}
			return diskCacheHandler.GetDiskCachePath(this.GetClipData(textToSpeak, voiceSettings, diskCacheSettings));
		}

		public TTSClipData DownloadToDiskCache(string textToSpeak, string presetVoiceId, TTSDiskCacheSettings diskCacheSettings = null, Action<TTSClipData, string, string> onDownloadComplete = null)
		{
			return this.DownloadToDiskCache(textToSpeak, this.GetPresetVoiceSettings(presetVoiceId), diskCacheSettings, onDownloadComplete);
		}

		public TTSClipData DownloadToDiskCache(string textToSpeak, TTSVoiceSettings voiceSettings, TTSDiskCacheSettings diskCacheSettings = null, Action<TTSClipData, string, string> onDownloadComplete = null)
		{
			TTSClipData clipData = this.GetClipData(textToSpeak, voiceSettings, diskCacheSettings);
			this.DownloadAsync(clipData, onDownloadComplete).WrapErrors();
			return clipData;
		}

		public Task<string> DownloadAsync(string textToSpeak, TTSVoiceSettings voiceSettings, TTSDiskCacheSettings diskCacheSettings)
		{
			TTSService.<DownloadAsync>d__68 <DownloadAsync>d__;
			<DownloadAsync>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
			<DownloadAsync>d__.<>4__this = this;
			<DownloadAsync>d__.textToSpeak = textToSpeak;
			<DownloadAsync>d__.voiceSettings = voiceSettings;
			<DownloadAsync>d__.diskCacheSettings = diskCacheSettings;
			<DownloadAsync>d__.<>1__state = -1;
			<DownloadAsync>d__.<>t__builder.Start<TTSService.<DownloadAsync>d__68>(ref <DownloadAsync>d__);
			return <DownloadAsync>d__.<>t__builder.Task;
		}

		private Task<string> DownloadAsync(TTSClipData clipData, Action<TTSClipData, string, string> onDownloadComplete = null)
		{
			TTSService.<DownloadAsync>d__69 <DownloadAsync>d__;
			<DownloadAsync>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
			<DownloadAsync>d__.<>4__this = this;
			<DownloadAsync>d__.clipData = clipData;
			<DownloadAsync>d__.onDownloadComplete = onDownloadComplete;
			<DownloadAsync>d__.<>1__state = -1;
			<DownloadAsync>d__.<>t__builder.Start<TTSService.<DownloadAsync>d__69>(ref <DownloadAsync>d__);
			return <DownloadAsync>d__.<>t__builder.Task;
		}

		private Task<Tuple<bool, string>> ShouldDownload(TTSClipData clipData, string downloadPath)
		{
			TTSService.<ShouldDownload>d__70 <ShouldDownload>d__;
			<ShouldDownload>d__.<>t__builder = AsyncTaskMethodBuilder<Tuple<bool, string>>.Create();
			<ShouldDownload>d__.<>4__this = this;
			<ShouldDownload>d__.clipData = clipData;
			<ShouldDownload>d__.downloadPath = downloadPath;
			<ShouldDownload>d__.<>1__state = -1;
			<ShouldDownload>d__.<>t__builder.Start<TTSService.<ShouldDownload>d__70>(ref <ShouldDownload>d__);
			return <ShouldDownload>d__.<>t__builder.Task;
		}

		public TTSVoiceSettings[] GetAllPresetVoiceSettings()
		{
			ITTSVoiceProvider voiceProvider = this.VoiceProvider;
			if (voiceProvider == null)
			{
				return null;
			}
			return voiceProvider.PresetVoiceSettings;
		}

		public TTSVoiceSettings GetPresetVoiceSettings(string presetVoiceId)
		{
			if (this.VoiceProvider == null || this.VoiceProvider.PresetVoiceSettings == null)
			{
				return null;
			}
			return Array.Find<TTSVoiceSettings>(this.VoiceProvider.PresetVoiceSettings, (TTSVoiceSettings v) => string.Equals(v.SettingsId, presetVoiceId, StringComparison.CurrentCultureIgnoreCase));
		}

		private void RaiseLoadBegin(TTSClipData clipData, bool download = false)
		{
			this.SetClipLoadState(clipData, TTSClipLoadState.Preparing);
			this.RaiseEvents(delegate
			{
				if (this.verboseLogging)
				{
					this.Logger.Verbose("Clip Loading\nText: {0}", clipData.textToSpeak, null, null, null, "RaiseLoadBegin", ".\\Library\\PackageCache\\com.meta.xr.sdk.voice@d3f6f37b8e1c\\Lib\\Wit.ai\\Features\\TTS\\Scripts\\Runtime\\TTSService.cs", 905);
				}
				TTSServiceEvents events = this.Events;
				if (events == null)
				{
					return;
				}
				TTSClipEvent onClipCreated = events.OnClipCreated;
				if (onClipCreated == null)
				{
					return;
				}
				onClipCreated.Invoke(clipData);
			});
		}

		private void RaiseUnloadComplete(TTSClipData clipData, bool download = false)
		{
			ITTSWebHandler webHandler = this.WebHandler;
			if (webHandler != null)
			{
				webHandler.CancelRequests(clipData);
			}
			IAudioClipStream clipStream = clipData.clipStream;
			if (clipStream != null)
			{
				clipStream.Unload();
			}
			clipData.clipStream = null;
			if (clipData.loadState == TTSClipLoadState.Preparing)
			{
				clipData.LoadStatusCode = -6;
				clipData.LoadError = "Cancelled";
			}
			if (clipData.loadState != TTSClipLoadState.Error)
			{
				this.SetClipLoadState(clipData, TTSClipLoadState.Unloaded);
			}
			this.RaiseEvents(delegate
			{
				if (this.verboseLogging)
				{
					this.Logger.Verbose("Clip Unloaded\nText: {0}", clipData.textToSpeak, null, null, null, "RaiseUnloadComplete", ".\\Library\\PackageCache\\com.meta.xr.sdk.voice@d3f6f37b8e1c\\Lib\\Wit.ai\\Features\\TTS\\Scripts\\Runtime\\TTSService.cs", 930);
				}
				TTSServiceEvents events = this.Events;
				if (events == null)
				{
					return;
				}
				TTSClipEvent onClipUnloaded = events.OnClipUnloaded;
				if (onClipUnloaded == null)
				{
					return;
				}
				onClipUnloaded.Invoke(clipData);
			});
		}

		private void RaiseDiskStreamBegin(TTSClipData clipData)
		{
			this.RaiseStreamBegin(clipData, true);
		}

		private void RaiseWebStreamBegin(TTSClipData clipData)
		{
			this.RaiseStreamBegin(clipData, false);
		}

		private void RaiseStreamBegin(TTSClipData clipData, bool fromDisk)
		{
			this.RaiseEvents(delegate
			{
				this.LogState(clipData, "Stream Begin", fromDisk, null);
				TTSServiceEvents events = this.Events;
				if (events == null)
				{
					return;
				}
				TTSStreamEvents stream = events.Stream;
				if (stream == null)
				{
					return;
				}
				TTSClipEvent onStreamBegin = stream.OnStreamBegin;
				if (onStreamBegin == null)
				{
					return;
				}
				onStreamBegin.Invoke(clipData);
			});
		}

		private void RaiseDiskStreamError(TTSClipData clipData, int errorCode, string error)
		{
			this.RaiseStreamError(clipData, errorCode, error, true);
		}

		private void RaiseWebStreamError(TTSClipData clipData, int errorCode, string error)
		{
			this.RaiseStreamError(clipData, errorCode, error, false);
		}

		private void RaiseStreamError(TTSClipData clipData, int errorCode, string error, bool fromDisk)
		{
			if (error.Equals("Cancelled"))
			{
				this.RaiseStreamCancel(clipData, fromDisk);
				return;
			}
			clipData.LoadStatusCode = errorCode;
			clipData.LoadError = error;
			this.SetClipLoadState(clipData, TTSClipLoadState.Error);
			this.RaiseEvents(delegate
			{
				this.LogState(clipData, "Stream Error", fromDisk, error);
				TTSServiceEvents events = this.Events;
				if (events != null)
				{
					TTSStreamEvents stream = events.Stream;
					if (stream != null)
					{
						TTSClipErrorEvent onStreamError = stream.OnStreamError;
						if (onStreamError != null)
						{
							onStreamError.Invoke(clipData, error);
						}
					}
				}
				if (!clipData.LoadReady.Task.IsCompleted)
				{
					clipData.LoadReady.SetResult(false);
				}
				this.RaiseStreamComplete(clipData, fromDisk);
			});
		}

		private void RaiseDiskStreamCancel(TTSClipData clipData)
		{
			this.RaiseStreamCancel(clipData, true);
		}

		private void RaiseWebStreamCancel(TTSClipData clipData)
		{
			this.RaiseStreamCancel(clipData, false);
		}

		private void RaiseStreamCancel(TTSClipData clipData, bool fromDisk)
		{
			clipData.LoadStatusCode = -6;
			clipData.LoadError = "Cancelled";
			this.SetClipLoadState(clipData, TTSClipLoadState.Error);
			this.RaiseEvents(delegate
			{
				this.LogState(clipData, "Stream Cancelled", fromDisk, null);
				TTSServiceEvents events = this.Events;
				if (events != null)
				{
					TTSStreamEvents stream = events.Stream;
					if (stream != null)
					{
						TTSClipEvent onStreamCancel = stream.OnStreamCancel;
						if (onStreamCancel != null)
						{
							onStreamCancel.Invoke(clipData);
						}
					}
				}
				if (!clipData.LoadReady.Task.IsCompleted)
				{
					clipData.LoadReady.SetResult(false);
				}
				this.RaiseStreamComplete(clipData, fromDisk);
			});
		}

		private void RaiseDiskStreamReady(TTSClipData clipData)
		{
			this.RaiseStreamReady(clipData, true);
		}

		private void RaiseWebStreamReady(TTSClipData clipData)
		{
			this.RaiseStreamReady(clipData, false);
		}

		private void RaiseStreamReady(TTSClipData clipData, bool fromDisk)
		{
			if (this.RuntimeCacheHandler != null)
			{
				this.RuntimeCacheHandler.OnClipRemoved -= this.OnRuntimeClipRemoved;
				bool flag = !this.RuntimeCacheHandler.AddClip(clipData);
				this.RuntimeCacheHandler.OnClipRemoved += this.OnRuntimeClipRemoved;
				if (flag)
				{
					this.RaiseStreamError(clipData, -1, "Removed from runtime cache due to file size", fromDisk);
					this.OnRuntimeClipRemoved(clipData);
					return;
				}
			}
			this.SetClipLoadState(clipData, TTSClipLoadState.Loaded);
			this.RaiseEvents(delegate
			{
				this.LogState(clipData, "Stream Ready", fromDisk, null);
				Action<TTSClipData> onPlaybackReady = clipData.onPlaybackReady;
				if (onPlaybackReady != null)
				{
					onPlaybackReady(clipData);
				}
				clipData.onPlaybackReady = null;
				TTSServiceEvents events = this.Events;
				if (events != null)
				{
					TTSStreamEvents stream = events.Stream;
					if (stream != null)
					{
						TTSClipEvent onStreamReady = stream.OnStreamReady;
						if (onStreamReady != null)
						{
							onStreamReady.Invoke(clipData);
						}
					}
				}
				if (!clipData.LoadReady.Task.IsCompleted)
				{
					clipData.LoadReady.SetResult(true);
				}
			});
		}

		private void RaiseDiskStreamComplete(TTSClipData clipData)
		{
			this.RaiseStreamComplete(clipData, true);
		}

		private void RaiseWebStreamComplete(TTSClipData clipData)
		{
			this.RaiseStreamComplete(clipData, false);
		}

		private void RaiseStreamComplete(TTSClipData clipData, bool fromDisk)
		{
			this.RaiseEvents(delegate
			{
				this.LogState(clipData, "Stream Complete", fromDisk, null);
				TTSServiceEvents events = this.Events;
				if (events != null)
				{
					TTSStreamEvents stream = events.Stream;
					if (stream != null)
					{
						TTSClipEvent onStreamComplete = stream.OnStreamComplete;
						if (onStreamComplete != null)
						{
							onStreamComplete.Invoke(clipData);
						}
					}
				}
				if (!fromDisk)
				{
					TTSServiceEvents events2 = this.Events;
					if (events2 != null)
					{
						TTSWebRequestEvents webRequest = events2.WebRequest;
						if (webRequest != null)
						{
							webRequest.OnRequestComplete.Invoke(clipData);
						}
					}
				}
				if (!clipData.LoadCompletion.Task.IsCompleted)
				{
					clipData.LoadCompletion.SetResult(true);
				}
				if (clipData.loadState == TTSClipLoadState.Error)
				{
					this.Unload(clipData);
				}
			});
		}

		private void RaiseDownloadBegin(TTSClipData clipData, string downloadPath)
		{
			this.RaiseEvents(delegate
			{
				this.LogState(clipData, "Download Begin", true, null);
				TTSServiceEvents events = this.Events;
				if (events == null)
				{
					return;
				}
				TTSDownloadEvents download = events.Download;
				if (download == null)
				{
					return;
				}
				TTSClipDownloadEvent onDownloadBegin = download.OnDownloadBegin;
				if (onDownloadBegin == null)
				{
					return;
				}
				onDownloadBegin.Invoke(clipData, downloadPath);
			});
		}

		private void RaiseDownloadSuccess(TTSClipData clipData, string downloadPath)
		{
			this.RaiseEvents(delegate
			{
				this.LogState(clipData, "Download Success", true, null);
				Action<string> onDownloadComplete = clipData.onDownloadComplete;
				if (onDownloadComplete != null)
				{
					onDownloadComplete(string.Empty);
				}
				clipData.onDownloadComplete = null;
				TTSServiceEvents events = this.Events;
				if (events == null)
				{
					return;
				}
				TTSDownloadEvents download = events.Download;
				if (download == null)
				{
					return;
				}
				TTSClipDownloadEvent onDownloadSuccess = download.OnDownloadSuccess;
				if (onDownloadSuccess == null)
				{
					return;
				}
				onDownloadSuccess.Invoke(clipData, downloadPath);
			});
		}

		private void RaiseDownloadCancel(TTSClipData clipData, string downloadPath)
		{
			this.RaiseEvents(delegate
			{
				this.LogState(clipData, "Download Cancelled", true, null);
				Action<string> onDownloadComplete = clipData.onDownloadComplete;
				if (onDownloadComplete != null)
				{
					onDownloadComplete("Cancelled");
				}
				clipData.onDownloadComplete = null;
				TTSServiceEvents events = this.Events;
				if (events == null)
				{
					return;
				}
				TTSDownloadEvents download = events.Download;
				if (download == null)
				{
					return;
				}
				TTSClipDownloadEvent onDownloadCancel = download.OnDownloadCancel;
				if (onDownloadCancel == null)
				{
					return;
				}
				onDownloadCancel.Invoke(clipData, downloadPath);
			});
		}

		private void RaiseDownloadError(TTSClipData clipData, string downloadPath, string error)
		{
			if (error.Equals("Cancelled"))
			{
				this.RaiseDownloadCancel(clipData, downloadPath);
				return;
			}
			this.RaiseEvents(delegate
			{
				this.LogState(clipData, "Download Failed", true, error);
				Action<string> onDownloadComplete = clipData.onDownloadComplete;
				if (onDownloadComplete != null)
				{
					onDownloadComplete(error);
				}
				clipData.onDownloadComplete = null;
				TTSServiceEvents events = this.Events;
				if (events == null)
				{
					return;
				}
				TTSDownloadEvents download = events.Download;
				if (download == null)
				{
					return;
				}
				TTSClipDownloadErrorEvent onDownloadError = download.OnDownloadError;
				if (onDownloadError == null)
				{
					return;
				}
				onDownloadError.Invoke(clipData, downloadPath, error);
			});
		}

		private void RaiseEvents(Action events)
		{
			ThreadUtility.CallOnMainThread(events).WrapErrors();
		}

		public VoiceErrorSimulationType SimulatedErrorType { get; set; } = (VoiceErrorSimulationType)(-1);

		[SerializeField]
		private bool verboseLogging;

		private static TTSService _instance;

		[Header("TTS Modules")]
		[Tooltip("Audio system to be used for obtaining audio clip streams.")]
		[SerializeField]
		[ObjectType(typeof(IAudioSystem), new Type[]
		{

		})]
		private Object _audioSystem;

		[Tooltip("Runtime cache that assists with the temporary storage of audio clips.")]
		[SerializeField]
		[ObjectType(typeof(ITTSRuntimeCacheHandler), new Type[]
		{

		})]
		private Object _runtimeCacheHandler;

		[Tooltip("Disk cache that assists with the backup and retrieval of audio clips saved to disk.")]
		[SerializeField]
		[ObjectType(typeof(ITTSDiskCacheHandler), new Type[]
		{

		})]
		private Object _diskCacheHandler;

		[Header("Event Settings")]
		[SerializeField]
		private TTSServiceEvents _events = new TTSServiceEvents();

		private bool _isActive;

		private bool _hasListeners;
	}
}
