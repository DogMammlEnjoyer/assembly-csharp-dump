using System;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Voice.Windows;
using POpusCodec.Enums;
using UnityEngine;
using UnityEngine.Serialization;

namespace Photon.Voice.Unity
{
	[AddComponentMenu("Photon Voice/Recorder")]
	[HelpURL("https://doc.photonengine.com/en-us/voice/v2/getting-started/recorder")]
	[DisallowMultipleComponent]
	public class Recorder : VoiceComponent
	{
		public LocalVoice Voice
		{
			get
			{
				return this.voice;
			}
		}

		public IAudioDesc InputSource
		{
			get
			{
				return this.inputSource;
			}
		}

		internal bool MicrophoneDeviceChangeDetected
		{
			get
			{
				object obj = this.microphoneDeviceChangeDetectedLock;
				bool result;
				lock (obj)
				{
					result = this.microphoneDeviceChangeDetected;
				}
				return result;
			}
			set
			{
				object obj = this.microphoneDeviceChangeDetectedLock;
				lock (obj)
				{
					if (this.microphoneDeviceChangeDetected == value)
					{
						if (base.Logger.IsWarningEnabled)
						{
							base.Logger.LogWarning("Unexpected: MicrophoneDeviceChangeDetected to be overriden with same value: {0}", new object[]
							{
								value
							});
						}
					}
					else
					{
						this.microphoneDeviceChangeDetected = value;
					}
				}
			}
		}

		private bool subscribedToSystemChanges
		{
			get
			{
				return this.subscribedToSystemChangesUnity || this.subscribedToSystemChangesPhoton;
			}
		}

		[Obsolete("Use the generic unified non-static MicrophonesEnumerator")]
		public static IDeviceEnumerator PhotonMicrophoneEnumerator
		{
			get
			{
				if (Recorder.photonMicrophoneEnumerator == null)
				{
					Recorder.photonMicrophoneEnumerator = Recorder.CreatePhotonDeviceEnumerator(new VoiceLogger("PhotonMicrophoneEnumerator", DebugLevel.ERROR));
				}
				return Recorder.photonMicrophoneEnumerator;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return this.client != null;
			}
		}

		[Obsolete("Renamed to RequiresRestart")]
		public bool RequiresInit
		{
			get
			{
				return this.RequiresRestart;
			}
		}

		public bool RequiresRestart { get; protected set; }

		public bool TransmitEnabled
		{
			get
			{
				return this.transmitEnabled;
			}
			set
			{
				if (value != this.transmitEnabled)
				{
					this.transmitEnabled = value;
					if (this.voice != LocalVoiceAudioDummy.Dummy)
					{
						this.voice.TransmitEnabled = value;
					}
				}
			}
		}

		public bool Encrypt
		{
			get
			{
				return this.encrypt;
			}
			set
			{
				if (this.encrypt == value)
				{
					return;
				}
				this.encrypt = value;
				this.voice.Encrypt = value;
			}
		}

		public bool DebugEchoMode
		{
			get
			{
				if (this.debugEchoMode && this.InterestGroup != 0)
				{
					this.voice.DebugEchoMode = false;
					this.debugEchoMode = false;
				}
				return this.debugEchoMode;
			}
			set
			{
				if (this.debugEchoMode == value)
				{
					return;
				}
				if (this.InterestGroup != 0)
				{
					if (base.Logger.IsWarningEnabled)
					{
						base.Logger.LogWarning("Cannot enable DebugEchoMode when InterestGroup value ({0}) is different than 0.", new object[]
						{
							this.interestGroup
						});
					}
					return;
				}
				this.debugEchoMode = value;
				this.voice.DebugEchoMode = value;
			}
		}

		public bool ReliableMode
		{
			get
			{
				return this.reliableMode;
			}
			set
			{
				if (this.voice != LocalVoiceAudioDummy.Dummy)
				{
					this.voice.Reliable = value;
				}
				this.reliableMode = value;
			}
		}

		public bool VoiceDetection
		{
			get
			{
				this.GetStatusFromDetector();
				return this.voiceDetection;
			}
			set
			{
				this.voiceDetection = value;
				if (this.VoiceDetector != null)
				{
					this.VoiceDetector.On = value;
				}
			}
		}

		public float VoiceDetectionThreshold
		{
			get
			{
				this.GetThresholdFromDetector();
				return this.voiceDetectionThreshold;
			}
			set
			{
				if (this.voiceDetectionThreshold.Equals(value))
				{
					return;
				}
				if (value < 0f || value > 1f)
				{
					if (base.Logger.IsErrorEnabled)
					{
						base.Logger.LogError("Value out of range: VAD Threshold needs to be between [0..1], requested value: {0}", new object[]
						{
							value
						});
					}
					return;
				}
				this.voiceDetectionThreshold = value;
				if (this.VoiceDetector != null)
				{
					this.VoiceDetector.Threshold = this.voiceDetectionThreshold;
				}
			}
		}

		public int VoiceDetectionDelayMs
		{
			get
			{
				this.GetActivityDelayFromDetector();
				return this.voiceDetectionDelayMs;
			}
			set
			{
				if (this.voiceDetectionDelayMs == value)
				{
					return;
				}
				this.voiceDetectionDelayMs = value;
				if (this.VoiceDetector != null)
				{
					this.VoiceDetector.ActivityDelayMs = value;
				}
			}
		}

		public object UserData
		{
			get
			{
				return this.userData;
			}
			set
			{
				if (this.userData != value)
				{
					this.userData = value;
					if (this.IsRecording)
					{
						this.RequiresRestart = true;
						bool isInfoEnabled = base.Logger.IsInfoEnabled;
					}
				}
			}
		}

		public Func<IAudioDesc> InputFactory
		{
			get
			{
				return this.inputFactory;
			}
			set
			{
				if (this.inputFactory != value)
				{
					this.inputFactory = value;
					if (this.IsRecording && this.SourceType == Recorder.InputSourceType.Factory)
					{
						this.RequiresRestart = true;
						if (base.Logger.IsInfoEnabled)
						{
							base.Logger.LogInfo("Recorder.{0} changed, Recorder requires restart for this to take effect.", new object[]
							{
								"InputFactory"
							});
						}
					}
				}
			}
		}

		public AudioUtil.IVoiceDetector VoiceDetector
		{
			get
			{
				if (this.voiceAudio == null)
				{
					return null;
				}
				return this.voiceAudio.VoiceDetector;
			}
		}

		public string UnityMicrophoneDevice
		{
			get
			{
				if (!Recorder.IsValidUnityMic(this.unityMicrophoneDevice))
				{
					if (base.Logger.IsInfoEnabled)
					{
						base.Logger.LogInfo("\"{0}\" is not a valid Unity microphone device, switching to default", new object[]
						{
							this.unityMicrophoneDevice
						});
					}
					this.unityMicrophoneDevice = null;
					if (UnityMicrophone.devices.Length != 0)
					{
						this.unityMicrophoneDevice = UnityMicrophone.devices[0];
					}
				}
				return this.unityMicrophoneDevice;
			}
			set
			{
				if (!Recorder.IsValidUnityMic(value))
				{
					if (base.Logger.IsErrorEnabled)
					{
						base.Logger.LogError("\"{0}\" is not a valid Unity microphone device", new object[]
						{
							value
						});
					}
					return;
				}
				if (!Recorder.CompareUnityMicNames(this.unityMicrophoneDevice, value))
				{
					this.unityMicrophoneDevice = value;
					if (string.IsNullOrEmpty(this.unityMicrophoneDevice) && UnityMicrophone.devices.Length != 0)
					{
						this.unityMicrophoneDevice = UnityMicrophone.devices[0];
					}
					if (this.IsRecording && this.SourceType == Recorder.InputSourceType.Microphone && this.MicrophoneType == Recorder.MicType.Unity)
					{
						this.RequiresRestart = true;
						if (base.Logger.IsInfoEnabled)
						{
							base.Logger.LogInfo("Recorder.{0} changed, Recorder requires restart for this to take effect.", new object[]
							{
								"UnityMicrophoneDevice"
							});
						}
					}
					this.CheckAndSetSamplingRate();
				}
			}
		}

		public int PhotonMicrophoneDeviceId
		{
			get
			{
				if (!this.IsValidPhotonMic())
				{
					if (base.Logger.IsInfoEnabled)
					{
						base.Logger.LogInfo("\"{0}\" is not a valid Photon microphone device ID, switching to default (-1)", new object[]
						{
							this.photonMicrophoneDeviceId
						});
					}
					this.photonMicrophoneDeviceId = -1;
				}
				return this.photonMicrophoneDeviceId;
			}
			set
			{
				if (!this.IsValidPhotonMic(value))
				{
					if (base.Logger.IsErrorEnabled)
					{
						base.Logger.LogError("\"{0}\" is not a valid Photon microphone device ID", new object[]
						{
							value
						});
					}
					return;
				}
				if (this.photonMicrophoneDeviceId != value)
				{
					this.photonMicrophoneDeviceId = value;
					if (this.IsRecording && this.SourceType == Recorder.InputSourceType.Microphone && this.MicrophoneType == Recorder.MicType.Photon)
					{
						this.RequiresRestart = true;
						if (base.Logger.IsInfoEnabled)
						{
							base.Logger.LogInfo("Recorder.{0} changed, Recorder requires restart for this to take effect.", new object[]
							{
								"PhotonMicrophoneDeviceId"
							});
						}
					}
				}
			}
		}

		[Obsolete("Use InterestGroup instead")]
		public byte AudioGroup
		{
			get
			{
				return this.InterestGroup;
			}
			set
			{
				this.InterestGroup = value;
			}
		}

		public byte InterestGroup
		{
			get
			{
				if (this.isRecording && this.voice.InterestGroup != this.interestGroup)
				{
					this.interestGroup = this.voice.InterestGroup;
					if (this.debugEchoMode && this.interestGroup != 0)
					{
						this.debugEchoMode = false;
					}
				}
				return this.interestGroup;
			}
			set
			{
				if (this.interestGroup == value)
				{
					return;
				}
				if (this.debugEchoMode && value != 0)
				{
					this.debugEchoMode = false;
					if (base.Logger.IsWarningEnabled)
					{
						base.Logger.LogWarning("DebugEchoMode disabled because InterestGroup changed to {0}. DebugEchoMode works only with Interest Group 0.", new object[]
						{
							value
						});
					}
				}
				this.interestGroup = value;
				this.voice.InterestGroup = value;
			}
		}

		public bool IsCurrentlyTransmitting
		{
			get
			{
				return this.IsRecording && this.TransmitEnabled && this.voice.IsCurrentlyTransmitting;
			}
		}

		public AudioUtil.ILevelMeter LevelMeter
		{
			get
			{
				if (this.voiceAudio == null)
				{
					return null;
				}
				return this.voiceAudio.LevelMeter;
			}
		}

		public bool VoiceDetectorCalibrating
		{
			get
			{
				return this.voiceAudio != null && this.TransmitEnabled && this.voiceAudio.VoiceDetectorCalibrating;
			}
		}

		protected ILocalVoiceAudio voiceAudio
		{
			get
			{
				return this.voice as ILocalVoiceAudio;
			}
		}

		public Recorder.InputSourceType SourceType
		{
			get
			{
				return this.sourceType;
			}
			set
			{
				if (this.sourceType != value)
				{
					this.sourceType = value;
					if (this.IsRecording)
					{
						this.RequiresRestart = true;
						if (base.Logger.IsInfoEnabled)
						{
							base.Logger.LogInfo("Recorder.{0} changed, Recorder requires restart for this to take effect.", new object[]
							{
								"Source"
							});
						}
					}
					this.CheckAndSetSamplingRate();
				}
			}
		}

		public Recorder.MicType MicrophoneType
		{
			get
			{
				return this.microphoneType;
			}
			set
			{
				if (this.microphoneType != value)
				{
					this.microphoneType = value;
					if (this.IsRecording && this.SourceType == Recorder.InputSourceType.Microphone)
					{
						this.RequiresRestart = true;
						if (base.Logger.IsInfoEnabled)
						{
							base.Logger.LogInfo("Recorder.{0} changed, Recorder requires restart for this to take effect.", new object[]
							{
								"MicrophoneType"
							});
						}
					}
					this.CheckAndSetSamplingRate();
				}
			}
		}

		[Obsolete("No longer used. Implicit conversion is done internally when needed.")]
		public Recorder.SampleTypeConv TypeConvert { get; set; }

		public AudioClip AudioClip
		{
			get
			{
				return this.audioClip;
			}
			set
			{
				if (this.audioClip != value)
				{
					this.audioClip = value;
					if (this.IsRecording && this.SourceType == Recorder.InputSourceType.AudioClip)
					{
						this.RequiresRestart = true;
						if (base.Logger.IsInfoEnabled)
						{
							base.Logger.LogInfo("Recorder.{0} changed, Recorder requires restart for this to take effect.", new object[]
							{
								"AudioClip"
							});
						}
					}
					this.CheckAndSetSamplingRate();
				}
			}
		}

		public bool LoopAudioClip
		{
			get
			{
				return this.loopAudioClip;
			}
			set
			{
				if (this.loopAudioClip != value)
				{
					this.loopAudioClip = value;
					if (this.IsRecording && this.SourceType == Recorder.InputSourceType.AudioClip)
					{
						AudioClipWrapper audioClipWrapper = this.inputSource as AudioClipWrapper;
						if (audioClipWrapper != null)
						{
							audioClipWrapper.Loop = value;
							return;
						}
						if (base.Logger.IsErrorEnabled)
						{
							base.Logger.LogError("Unexpected: Recorder inputSource is not of AudioClipWrapper type or is null.", Array.Empty<object>());
						}
					}
				}
			}
		}

		public SamplingRate SamplingRate
		{
			get
			{
				return this.samplingRate;
			}
			set
			{
				this.CheckAndSetSamplingRate(value);
			}
		}

		public OpusCodec.FrameDuration FrameDuration
		{
			get
			{
				return this.frameDuration;
			}
			set
			{
				if (this.frameDuration != value)
				{
					this.frameDuration = value;
					if (this.IsRecording)
					{
						this.RequiresRestart = true;
						if (base.Logger.IsInfoEnabled)
						{
							base.Logger.LogInfo("Recorder.{0} changed, Recorder requires restart for this to take effect.", new object[]
							{
								"FrameDuration"
							});
						}
					}
				}
			}
		}

		public int Bitrate
		{
			get
			{
				return this.bitrate;
			}
			set
			{
				if (this.bitrate != value)
				{
					if (value < 6000 || value > 510000)
					{
						if (base.Logger.IsErrorEnabled)
						{
							base.Logger.LogError("Unsupported bitrate value {0}, valid range: {1}-{2}", new object[]
							{
								value,
								6000,
								510000
							});
							return;
						}
					}
					else
					{
						this.bitrate = value;
						if (this.IsRecording)
						{
							this.RequiresRestart = true;
							if (base.Logger.IsInfoEnabled)
							{
								base.Logger.LogInfo("Recorder.{0} changed, Recorder requires restart for this to take effect.", new object[]
								{
									"Bitrate"
								});
							}
						}
					}
				}
			}
		}

		public bool IsRecording
		{
			get
			{
				return this.isRecording;
			}
			set
			{
				if (this.isRecording != value)
				{
					if (this.isRecording)
					{
						this.StopRecording();
						return;
					}
					this.StartRecording();
				}
			}
		}

		public bool ReactOnSystemChanges
		{
			get
			{
				return this.reactOnSystemChanges;
			}
			set
			{
				if (this.reactOnSystemChanges != value)
				{
					this.reactOnSystemChanges = value;
					if (this.IsRecording)
					{
						if (this.reactOnSystemChanges)
						{
							if (!this.subscribedToSystemChanges)
							{
								this.SubscribeToSystemChanges();
								return;
							}
						}
						else if (this.subscribedToSystemChanges)
						{
							this.UnsubscribeFromSystemChanges();
						}
					}
				}
			}
		}

		public bool AutoStart
		{
			get
			{
				return this.autoStart;
			}
			set
			{
				if (this.autoStart != value)
				{
					this.autoStart = value;
					this.CheckAndAutoStart();
				}
			}
		}

		public bool RecordOnlyWhenEnabled
		{
			get
			{
				return this.recordOnlyWhenEnabled;
			}
			set
			{
				if (this.recordOnlyWhenEnabled != value)
				{
					this.recordOnlyWhenEnabled = value;
					if (this.recordOnlyWhenEnabled)
					{
						if (!base.isActiveAndEnabled && this.IsRecording)
						{
							this.StopRecordingInternal();
							return;
						}
					}
					else
					{
						this.CheckAndAutoStart();
					}
				}
			}
		}

		public bool SkipDeviceChangeChecks
		{
			get
			{
				return this.skipDeviceChangeChecks;
			}
			set
			{
				this.skipDeviceChangeChecks = value;
			}
		}

		public bool StopRecordingWhenPaused
		{
			get
			{
				return this.stopRecordingWhenPaused;
			}
			set
			{
				this.stopRecordingWhenPaused = value;
			}
		}

		public bool UseOnAudioFilterRead
		{
			get
			{
				return this.useOnAudioFilterRead;
			}
			set
			{
				if (this.useOnAudioFilterRead != value)
				{
					this.useOnAudioFilterRead = value;
					if (this.IsRecording && this.SourceType == Recorder.InputSourceType.Microphone && this.MicrophoneType == Recorder.MicType.Unity)
					{
						this.RequiresRestart = true;
						if (base.Logger.IsInfoEnabled)
						{
							base.Logger.LogInfo("Recorder.{0} changed, Recorder requires restart for this to take effect.", new object[]
							{
								"UseOnAudioFilterRead"
							});
						}
					}
				}
			}
		}

		public bool TrySamplingRateMatch
		{
			get
			{
				return this.trySamplingRateMatch;
			}
			set
			{
				if (this.trySamplingRateMatch != value)
				{
					this.trySamplingRateMatch = value;
					if (this.trySamplingRateMatch)
					{
						this.CheckAndSetSamplingRate();
					}
				}
			}
		}

		public bool UseMicrophoneTypeFallback
		{
			get
			{
				return this.useMicrophoneTypeFallback;
			}
			set
			{
				this.useMicrophoneTypeFallback = value;
			}
		}

		public bool RecordOnlyWhenJoined
		{
			get
			{
				return this.recordOnlyWhenJoined;
			}
			set
			{
				if (this.recordOnlyWhenJoined != value)
				{
					this.recordOnlyWhenJoined = value;
					if (this.recordOnlyWhenJoined)
					{
						if (this.IsRecording && this.voiceConnection.Client != null && !this.voiceConnection.Client.InRoom)
						{
							this.StopRecordingInternal();
							return;
						}
					}
					else
					{
						this.CheckAndAutoStart();
					}
				}
			}
		}

		public IDeviceEnumerator MicrophonesEnumerator
		{
			get
			{
				return this.GetMicrophonesEnumerator(this.MicrophoneType);
			}
		}

		public DeviceInfo MicrophoneDevice
		{
			get
			{
				Recorder.MicType micType = this.MicrophoneType;
				if (micType != Recorder.MicType.Unity)
				{
					if (micType == Recorder.MicType.Photon)
					{
						int num = this.PhotonMicrophoneDeviceId;
						if (num != -1)
						{
							return this.GetDeviceById(num);
						}
					}
					return DeviceInfo.Default;
				}
				string text = this.UnityMicrophoneDevice;
				if (string.IsNullOrEmpty(text))
				{
					return this.MicrophonesEnumerator.First<DeviceInfo>();
				}
				return this.GetDeviceById(text);
			}
			set
			{
				Recorder.MicType micType = this.MicrophoneType;
				if (micType == Recorder.MicType.Unity)
				{
					this.UnityMicrophoneDevice = value.IDString;
					return;
				}
				if (micType != Recorder.MicType.Photon)
				{
					return;
				}
				this.PhotonMicrophoneDeviceId = (value.IsDefault ? -1 : value.IDInt);
			}
		}

		public void Init(VoiceConnection connection)
		{
			if (connection == null)
			{
				if (base.Logger.IsErrorEnabled)
				{
					base.Logger.LogError("voiceConnection is null.", Array.Empty<object>());
				}
				return;
			}
			if (!connection)
			{
				if (base.Logger.IsErrorEnabled)
				{
					base.Logger.LogError("voiceConnection is destroyed.", Array.Empty<object>());
				}
				return;
			}
			if (!base.IgnoreGlobalLogLevel)
			{
				base.LogLevel = connection.GlobalRecordersLogLevel;
			}
			if (this.IsInitialized)
			{
				if (base.Logger.IsWarningEnabled)
				{
					base.Logger.LogWarning("Recorder already initialized.", Array.Empty<object>());
				}
				return;
			}
			if (connection.VoiceClient == null)
			{
				if (base.Logger.IsErrorEnabled)
				{
					base.Logger.LogError("voiceConnection.VoiceClient is null.", Array.Empty<object>());
				}
				return;
			}
			this.voiceConnection = connection;
			this.client = connection.VoiceClient;
			this.voiceConnection.AddInitializedRecorder(this);
			this.CheckAndAutoStart();
		}

		[Obsolete("Renamed to RestartRecording")]
		public void ReInit()
		{
			this.RestartRecording(false);
		}

		public void RestartRecording(bool force = false)
		{
			if (!force && !this.RequiresRestart)
			{
				if (base.Logger.IsWarningEnabled)
				{
					base.Logger.LogWarning("Recorder does not require restart.", Array.Empty<object>());
				}
				return;
			}
			if (base.Logger.IsDebugEnabled)
			{
				base.Logger.LogDebug("Restarting recording, RequiresRestart?={0} forcedRestart?={1}", new object[]
				{
					this.RequiresRestart,
					force
				});
			}
			this.StopRecording();
			this.StartRecording();
		}

		public void VoiceDetectorCalibrate(int durationMs, Action<float> detectionEndedCallback = null)
		{
			if (this.voiceAudio != null)
			{
				if (!this.TransmitEnabled)
				{
					if (base.Logger.IsWarningEnabled)
					{
						base.Logger.LogWarning("Cannot start voice detection calibration when transmission is not enabled", Array.Empty<object>());
					}
					return;
				}
				this.voiceAudio.VoiceDetectorCalibrate(durationMs, delegate(float newThreshold)
				{
					this.GetThresholdFromDetector();
					if (detectionEndedCallback != null)
					{
						detectionEndedCallback(this.voiceDetectionThreshold);
					}
				});
			}
		}

		public void StartRecording()
		{
			if (this.IsRecording)
			{
				if (base.Logger.IsWarningEnabled)
				{
					base.Logger.LogWarning("Recorder is already started.", Array.Empty<object>());
				}
				return;
			}
			if (!this.IsInitialized)
			{
				if (base.Logger.IsWarningEnabled)
				{
					base.Logger.LogWarning("Recording can't be started if Recorder is not initialized. Call Recorder.Init(VoiceConnection) first.", Array.Empty<object>());
				}
				return;
			}
			if (this.RecordOnlyWhenEnabled && !base.isActiveAndEnabled)
			{
				if (base.Logger.IsWarningEnabled)
				{
					base.Logger.LogWarning("Recording can't be started because RecordOnlyWhenEnabled is true and Recorder is not enabled or its GameObject is not active in hierarchy.", Array.Empty<object>());
				}
				return;
			}
			if (this.RecordOnlyWhenJoined && this.voiceConnection.Client != null && !this.voiceConnection.Client.InRoom)
			{
				if (base.Logger.IsWarningEnabled)
				{
					base.Logger.LogWarning("Recording can't be started because RecordOnlyWhenJoined is true and voice networking client is not joined to a room.", Array.Empty<object>());
				}
				return;
			}
			this.StartRecordingInternal();
		}

		public void StopRecording()
		{
			this.wasRecordingBeforePause = false;
			if (!this.IsRecording)
			{
				if (base.Logger.IsWarningEnabled)
				{
					base.Logger.LogWarning("Recorder is not started.", Array.Empty<object>());
				}
				return;
			}
			this.StopRecordingInternal();
			this.recordingStoppedExplicitly = true;
		}

		public bool ResetLocalAudio()
		{
			if (this.inputSource != null && this.inputSource is IResettable)
			{
				if (base.Logger.IsInfoEnabled)
				{
					base.Logger.LogInfo("Resetting local audio.", Array.Empty<object>());
				}
				(this.inputSource as IResettable).Reset();
				return true;
			}
			if (base.Logger.IsDebugEnabled)
			{
				base.Logger.LogDebug("InputSource is null or not resettable.", Array.Empty<object>());
			}
			return false;
		}

		public static bool CompareUnityMicNames(string mic1, string mic2)
		{
			return (Recorder.IsDefaultUnityMic(mic1) && Recorder.IsDefaultUnityMic(mic2)) || (mic1 != null && mic1.Equals(mic2));
		}

		public static bool IsDefaultUnityMic(string mic)
		{
			return string.IsNullOrEmpty(mic) || Array.IndexOf<string>(UnityMicrophone.devices, mic) == 0;
		}

		private void Setup()
		{
			this.voice = this.CreateLocalVoiceAudioAndSource();
			if (this.voice == LocalVoiceAudioDummy.Dummy)
			{
				if (base.Logger.IsErrorEnabled)
				{
					base.Logger.LogError("Local input source setup and voice stream creation failed. No recording or transmission will be happening. See previous error log messages for more details.", Array.Empty<object>());
				}
				if (this.inputSource != null)
				{
					this.inputSource.Dispose();
					this.inputSource = null;
				}
				if (this.MicrophoneDeviceChangeDetected)
				{
					this.MicrophoneDeviceChangeDetected = false;
				}
				return;
			}
			this.SubscribeToSystemChanges();
			if (this.VoiceDetector != null)
			{
				this.VoiceDetector.Threshold = this.voiceDetectionThreshold;
				this.VoiceDetector.ActivityDelayMs = this.voiceDetectionDelayMs;
				this.VoiceDetector.On = this.voiceDetection;
			}
			this.voice.InterestGroup = this.InterestGroup;
			this.voice.DebugEchoMode = this.DebugEchoMode;
			this.voice.Encrypt = this.Encrypt;
			this.voice.Reliable = this.ReliableMode;
			this.RequiresRestart = false;
			this.isRecording = true;
			this.SendPhotonVoiceCreatedMessage();
			this.voice.TransmitEnabled = this.TransmitEnabled;
		}

		private LocalVoice CreateLocalVoiceAudioAndSource()
		{
			SamplingRate samplingRate = this.samplingRate;
			int num = (int)samplingRate;
			switch (this.SourceType)
			{
			case Recorder.InputSourceType.Microphone:
			{
				if (!this.CheckIfThereIsAtLeastOneMic())
				{
					if (base.Logger.IsErrorEnabled)
					{
						base.Logger.LogError("No microphone detected.", Array.Empty<object>());
					}
					return LocalVoiceAudioDummy.Dummy;
				}
				bool flag = false;
				Recorder.MicType micType = this.MicrophoneType;
				if (micType != Recorder.MicType.Unity)
				{
					if (micType != Recorder.MicType.Photon)
					{
						if (base.Logger.IsErrorEnabled)
						{
							base.Logger.LogError("unknown MicrophoneType value {0}", new object[]
							{
								this.MicrophoneType
							});
						}
						return LocalVoiceAudioDummy.Dummy;
					}
					goto IL_14D;
				}
				IL_75:
				string text = this.UnityMicrophoneDevice;
				bool isInfoEnabled = base.Logger.IsInfoEnabled;
				if (this.UseOnAudioFilterRead)
				{
					Debug.Log("Using MicWrapperPusher");
					this.inputSource = new MicWrapperPusher(text, base.transform, num, base.Logger, true);
				}
				else
				{
					this.inputSource = this.CreateMicWrapper(text, num, base.Logger);
				}
				if (this.inputSource != null)
				{
					if (this.inputSource.Error == null)
					{
						break;
					}
					if (base.Logger.IsErrorEnabled)
					{
						base.Logger.LogError("Unity microphone input source creation failure: {0}", new object[]
						{
							this.inputSource.Error
						});
					}
				}
				if (!this.UseMicrophoneTypeFallback || flag)
				{
					break;
				}
				flag = true;
				if (base.Logger.IsErrorEnabled)
				{
					base.Logger.LogError("Unity microphone failed. Falling back to Photon microphone", Array.Empty<object>());
				}
				IL_14D:
				DeviceInfo microphoneDevice = this.MicrophoneDevice;
				int deviceID = microphoneDevice.IsDefault ? -1 : microphoneDevice.IDInt;
				if (base.Logger.IsInfoEnabled)
				{
					base.Logger.LogInfo("Setting recorder's source to Photon microphone device={0}", new object[]
					{
						microphoneDevice
					});
				}
				if (base.Logger.IsInfoEnabled)
				{
					base.Logger.LogInfo("Setting recorder's source to WindowsAudioInPusher", Array.Empty<object>());
				}
				this.inputSource = new WindowsAudioInPusher(deviceID, base.Logger);
				if (this.inputSource != null)
				{
					if (this.inputSource.Error == null)
					{
						break;
					}
					if (base.Logger.IsErrorEnabled)
					{
						base.Logger.LogError("Photon microphone input source creation failure: {0}", new object[]
						{
							this.inputSource.Error
						});
					}
				}
				if (this.UseMicrophoneTypeFallback && !flag)
				{
					flag = true;
					if (base.Logger.IsErrorEnabled)
					{
						base.Logger.LogError("Photon microphone failed. Falling back to Unity microphone", Array.Empty<object>());
						goto IL_75;
					}
					goto IL_75;
				}
				break;
			}
			case Recorder.InputSourceType.AudioClip:
				if (this.AudioClip == null)
				{
					if (base.Logger.IsErrorEnabled)
					{
						base.Logger.LogError("AudioClip property must be set for AudioClip audio source", Array.Empty<object>());
					}
					return LocalVoiceAudioDummy.Dummy;
				}
				this.inputSource = new AudioClipWrapper(this.AudioClip)
				{
					Loop = this.LoopAudioClip
				};
				break;
			case Recorder.InputSourceType.Factory:
				if (this.InputFactory == null)
				{
					if (base.Logger.IsErrorEnabled)
					{
						base.Logger.LogError("Recorder.InputFactory must be specified if Recorder.Source set to Factory", Array.Empty<object>());
					}
					return LocalVoiceAudioDummy.Dummy;
				}
				this.inputSource = this.InputFactory();
				if (this.inputSource.Error != null && base.Logger.IsErrorEnabled)
				{
					base.Logger.LogError("InputFactory creation failure: {0}.", new object[]
					{
						this.inputSource.Error
					});
				}
				break;
			default:
				if (base.Logger.IsErrorEnabled)
				{
					base.Logger.LogError("unknown Source value {0}", new object[]
					{
						this.SourceType
					});
				}
				return LocalVoiceAudioDummy.Dummy;
			}
			if (this.inputSource == null || this.inputSource.Error != null)
			{
				return LocalVoiceAudioDummy.Dummy;
			}
			if (this.inputSource.Channels == 0)
			{
				if (base.Logger.IsErrorEnabled)
				{
					base.Logger.LogError("inputSource.Channels is zero", Array.Empty<object>());
				}
				return LocalVoiceAudioDummy.Dummy;
			}
			if (this.TrySamplingRateMatch && this.inputSource.SamplingRate != num)
			{
				samplingRate = this.GetSupportedSamplingRate(this.inputSource.SamplingRate);
				if (samplingRate != this.samplingRate && base.Logger.IsWarningEnabled)
				{
					base.Logger.LogWarning("Sampling rate requested ({0}Hz) is not used, input source is expecting {1}Hz instead so switching to the closest supported value: {1}Hz.", new object[]
					{
						num,
						this.inputSource.SamplingRate,
						(int)samplingRate
					});
				}
			}
			AudioSampleType sampleType = AudioSampleType.Source;
			WebRtcAudioDsp component = base.GetComponent<WebRtcAudioDsp>();
			if (component != null && component && component.enabled)
			{
				sampleType = AudioSampleType.Short;
				if (base.Logger.IsInfoEnabled)
				{
					base.Logger.LogInfo("Type Conversion set to Short. Audio samples will be converted if source samples types differ.", Array.Empty<object>());
				}
				if (samplingRate != SamplingRate.Sampling12000)
				{
					if (samplingRate == SamplingRate.Sampling24000)
					{
						if (base.Logger.IsWarningEnabled)
						{
							base.Logger.LogWarning("Sampling rate requested (24kHz) is not supported by WebRTC Audio DSP, switching to the closest supported value: 48kHz.", Array.Empty<object>());
						}
						samplingRate = SamplingRate.Sampling48000;
					}
				}
				else
				{
					if (base.Logger.IsWarningEnabled)
					{
						base.Logger.LogWarning("Sampling rate requested (12kHz) is not supported by WebRTC Audio DSP, switching to the closest supported value: 16kHz.", Array.Empty<object>());
					}
					samplingRate = SamplingRate.Sampling16000;
				}
				OpusCodec.FrameDuration frameDuration = this.FrameDuration;
				if (frameDuration == OpusCodec.FrameDuration.Frame2dot5ms || frameDuration == OpusCodec.FrameDuration.Frame5ms)
				{
					if (base.Logger.IsWarningEnabled)
					{
						base.Logger.LogWarning("Frame duration requested ({0}ms) is not supported by WebRTC Audio DSP (it needs to be N x 10ms), switching to the closest supported value: 10ms.", new object[]
						{
							(int)(this.FrameDuration / (OpusCodec.FrameDuration)1000)
						});
					}
					this.FrameDuration = OpusCodec.FrameDuration.Frame10ms;
				}
			}
			this.samplingRate = samplingRate;
			VoiceInfo voiceInfo = VoiceInfo.CreateAudioOpus(samplingRate, this.inputSource.Channels, this.FrameDuration, this.Bitrate, this.UserData);
			return this.client.CreateLocalVoiceAudioFromSource(voiceInfo, this.inputSource, sampleType, null, 0);
		}

		protected virtual MicWrapper CreateMicWrapper(string micDev, int samplingRateInt, VoiceLogger logger)
		{
			return new MicWrapper(micDev, samplingRateInt, logger);
		}

		protected virtual void SendPhotonVoiceCreatedMessage()
		{
			base.gameObject.SendMessage("PhotonVoiceCreated", new Photon.Voice.Unity.PhotonVoiceCreatedParams
			{
				Voice = this.voice,
				AudioDesc = this.inputSource
			}, SendMessageOptions.DontRequireReceiver);
		}

		private void OnDestroy()
		{
			if (base.Logger.IsDebugEnabled)
			{
				base.Logger.LogDebug("Recorder is about to be destroyed, removing local voice.", Array.Empty<object>());
			}
			this.RemoveVoice();
			if (this.IsInitialized)
			{
				this.voiceConnection.RemoveInitializedRecorder(this);
			}
		}

		private void RemoveVoice()
		{
			if (base.Logger.IsDebugEnabled)
			{
				base.Logger.LogDebug("RemovingVoice()", Array.Empty<object>());
			}
			if (this.subscribedToSystemChanges)
			{
				this.UnsubscribeFromSystemChanges();
			}
			this.GetThresholdFromDetector();
			this.GetStatusFromDetector();
			this.GetActivityDelayFromDetector();
			if (this.voice != LocalVoiceAudioDummy.Dummy)
			{
				this.interestGroup = this.voice.InterestGroup;
				if (this.debugEchoMode && this.interestGroup != 0)
				{
					this.debugEchoMode = false;
				}
				this.voice.RemoveSelf();
				this.voice = LocalVoiceAudioDummy.Dummy;
			}
			if (this.inputSource != null)
			{
				this.inputSource.Dispose();
				this.inputSource = null;
			}
			base.gameObject.SendMessage("PhotonVoiceRemoved", SendMessageOptions.DontRequireReceiver);
			this.isRecording = false;
			this.RequiresRestart = false;
		}

		private void OnAudioConfigChanged(bool deviceWasChanged)
		{
			if (base.Logger.IsInfoEnabled)
			{
				base.Logger.LogInfo("OnAudioConfigChanged deviceWasChanged={0}", new object[]
				{
					deviceWasChanged
				});
			}
			if (this.SkipDeviceChangeChecks || deviceWasChanged)
			{
				this.MicrophoneDeviceChangeDetected = true;
			}
		}

		private void PhotonMicrophoneChangeDetected()
		{
			if (base.Logger.IsInfoEnabled)
			{
				base.Logger.LogInfo("Microphones change detected by Photon native plugin", Array.Empty<object>());
			}
			this.MicrophoneDeviceChangeDetected = true;
		}

		internal void HandleDeviceChange()
		{
			if (!this.MicrophoneDeviceChangeDetected && base.Logger.IsWarningEnabled)
			{
				base.Logger.LogWarning("Unexpected: HandleDeviceChange called while MicrophoneDeviceChangedDetected is false.", Array.Empty<object>());
			}
			if (Recorder.photonMicrophoneEnumerator != null)
			{
				Recorder.photonMicrophoneEnumerator.Refresh();
			}
			if (this.photonMicrophonesEnumerator != null)
			{
				this.photonMicrophonesEnumerator.Refresh();
			}
			if (this.unityMicrophonesEnumerator != null)
			{
				this.unityMicrophonesEnumerator.Refresh();
			}
			if (this.IsRecording)
			{
				bool flag = false;
				if (this.SkipDeviceChangeChecks)
				{
					flag = true;
				}
				else if (this.SourceType == Recorder.InputSourceType.Microphone)
				{
					if (this.MicrophoneType == Recorder.MicType.Photon)
					{
						flag = (this.photonMicrophoneDeviceId == -1 || !this.IsValidPhotonMic());
					}
					else
					{
						flag = (string.IsNullOrEmpty(this.unityMicrophoneDevice) || !Recorder.IsValidUnityMic(this.unityMicrophoneDevice));
					}
				}
				if (flag)
				{
					if (!this.ResetLocalAudio())
					{
						this.RequiresRestart = true;
						if (base.Logger.IsInfoEnabled)
						{
							base.Logger.LogInfo("Restarting Recording as a result of audio config/device change.", Array.Empty<object>());
						}
						this.RestartRecording(false);
						return;
					}
					this.MicrophoneDeviceChangeDetected = false;
					if (base.Logger.IsInfoEnabled)
					{
						base.Logger.LogInfo("Local audio reset as a result of audio config/device change.", Array.Empty<object>());
						return;
					}
				}
			}
			else
			{
				if (base.Logger.IsInfoEnabled)
				{
					base.Logger.LogInfo("A microphone device may have been made available: will check auto start conditions and if all good will attempt to start recording.", Array.Empty<object>());
				}
				this.CheckAndAutoStart(true);
			}
		}

		private void SubscribeToSystemChanges()
		{
			if (base.Logger.IsDebugEnabled)
			{
				base.Logger.LogDebug("Subscribing to system (audio) changes.", Array.Empty<object>());
			}
			if (!this.ReactOnSystemChanges)
			{
				if (base.Logger.IsDebugEnabled)
				{
					base.Logger.LogDebug("ReactOnSystemChanges is false, not subscribed to system (audio) changes.", Array.Empty<object>());
				}
				return;
			}
			if (this.subscribedToSystemChanges)
			{
				if (base.Logger.IsWarningEnabled)
				{
					base.Logger.LogWarning("Already subscribed to system (audio) changes.", Array.Empty<object>());
				}
				return;
			}
			this.photonMicChangeNotifier = Platform.CreateAudioInChangeNotifier(new Action(this.PhotonMicrophoneChangeDetected), base.Logger);
			if (this.photonMicChangeNotifier.IsSupported)
			{
				if (this.photonMicChangeNotifier.Error == null)
				{
					this.subscribedToSystemChangesPhoton = true;
					if (base.Logger.IsInfoEnabled)
					{
						base.Logger.LogInfo("Subscribed to audio in change notifications via Photon plugin.", Array.Empty<object>());
					}
					return;
				}
				if (base.Logger.IsErrorEnabled)
				{
					base.Logger.LogError("Error creating instance of photonMicChangeNotifier: {0}", new object[]
					{
						this.photonMicChangeNotifier.Error
					});
				}
			}
			this.photonMicChangeNotifier.Dispose();
			this.photonMicChangeNotifier = null;
			AudioSettings.OnAudioConfigurationChanged += this.OnAudioConfigChanged;
			this.subscribedToSystemChangesUnity = true;
			if (base.Logger.IsInfoEnabled)
			{
				base.Logger.LogInfo("Subscribed to audio configuration changes via Unity callback.", Array.Empty<object>());
			}
		}

		private void UnsubscribeFromSystemChanges()
		{
			if (this.subscribedToSystemChangesUnity)
			{
				AudioSettings.OnAudioConfigurationChanged -= this.OnAudioConfigChanged;
				this.subscribedToSystemChangesUnity = false;
				if (base.Logger.IsInfoEnabled)
				{
					base.Logger.LogInfo("Unsubscribed from audio configuration changes via Unity callback.", Array.Empty<object>());
				}
			}
			if (this.subscribedToSystemChangesPhoton)
			{
				if (this.photonMicChangeNotifier == null)
				{
					if (base.Logger.IsErrorEnabled)
					{
						base.Logger.LogError("Unexpected: photonMicChangeNotifier is null while subscribedToSystemChangesPhoton is true.", Array.Empty<object>());
					}
				}
				else
				{
					this.photonMicChangeNotifier.Dispose();
					this.photonMicChangeNotifier = null;
				}
				this.subscribedToSystemChangesPhoton = false;
				if (base.Logger.IsInfoEnabled)
				{
					base.Logger.LogInfo("Unsubscribed from audio in change notifications via Photon plugin.", Array.Empty<object>());
				}
			}
		}

		private void GetThresholdFromDetector()
		{
			if (this.IsRecording && this.VoiceDetector != null && !this.voiceDetectionThreshold.Equals(this.VoiceDetector.Threshold))
			{
				if (this.VoiceDetector.Threshold <= 1f && this.VoiceDetector.Threshold >= 0f)
				{
					if (base.Logger.IsDebugEnabled)
					{
						base.Logger.LogDebug("VoiceDetectionThreshold automatically changed from {0} to {1}", new object[]
						{
							this.voiceDetectionThreshold,
							this.VoiceDetector.Threshold
						});
					}
					this.voiceDetectionThreshold = this.VoiceDetector.Threshold;
					return;
				}
				if (base.Logger.IsWarningEnabled)
				{
					base.Logger.LogWarning("VoiceDetector.Threshold has unexpected value {0}", new object[]
					{
						this.VoiceDetector.Threshold
					});
				}
			}
		}

		private void GetActivityDelayFromDetector()
		{
			if (this.IsRecording && this.VoiceDetector != null && this.voiceDetectionDelayMs != this.VoiceDetector.ActivityDelayMs)
			{
				if (base.Logger.IsDebugEnabled)
				{
					base.Logger.LogDebug("VoiceDetectionDelayMs automatically changed from {0} to {1}", new object[]
					{
						this.voiceDetectionDelayMs,
						this.VoiceDetector.ActivityDelayMs
					});
				}
				this.voiceDetectionDelayMs = this.VoiceDetector.ActivityDelayMs;
			}
		}

		private void GetStatusFromDetector()
		{
			if (this.IsRecording && this.VoiceDetector != null && this.voiceDetection != this.VoiceDetector.On)
			{
				if (base.Logger.IsDebugEnabled)
				{
					base.Logger.LogDebug("VoiceDetection automatically changed from {0} to {1}", new object[]
					{
						this.voiceDetection,
						this.VoiceDetector.On
					});
				}
				this.voiceDetection = this.VoiceDetector.On;
			}
		}

		private static bool IsValidUnityMic(string mic)
		{
			return string.IsNullOrEmpty(mic) || UnityMicrophone.devices.Contains(mic);
		}

		private void OnEnable()
		{
			this.wasRecordingBeforePause = false;
			this.isPausedOrInBackground = false;
			this.CheckAndAutoStart();
		}

		private void OnDisable()
		{
			if (this.RecordOnlyWhenEnabled && this.IsRecording)
			{
				this.StopRecordingInternal();
			}
		}

		private bool IsValidPhotonMic()
		{
			return this.IsValidPhotonMic(this.photonMicrophoneDeviceId);
		}

		public static bool CheckIfMicrophoneIdIsValid(IDeviceEnumerator audioInEnumerator, int id)
		{
			if (id == -1)
			{
				return true;
			}
			if (audioInEnumerator.IsSupported && audioInEnumerator.Error == null)
			{
				foreach (DeviceInfo deviceInfo in audioInEnumerator)
				{
					if (deviceInfo.IDInt == id)
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		private bool IsValidPhotonMic(int id)
		{
			return Recorder.CheckIfMicrophoneIdIsValid(this.GetMicrophonesEnumerator(Recorder.MicType.Photon), id);
		}

		private void OnApplicationPause(bool paused)
		{
			if (base.Logger.IsDebugEnabled)
			{
				base.Logger.LogDebug("OnApplicationPause({0})", new object[]
				{
					paused
				});
			}
			this.HandleApplicationPause(paused);
		}

		private void OnApplicationFocus(bool focused)
		{
			if (base.Logger.IsDebugEnabled)
			{
				base.Logger.LogDebug("OnApplicationFocus({0})", new object[]
				{
					focused
				});
			}
			this.HandleApplicationPause(!focused);
		}

		private void HandleApplicationPause(bool paused)
		{
			if (base.Logger.IsDebugEnabled)
			{
				base.Logger.LogDebug("App paused?= {0}, isPausedOrInBackground = {1}, wasRecordingBeforePause = {2}, StopRecordingWhenPaused = {3}, IsRecording = {4}", new object[]
				{
					paused,
					this.isPausedOrInBackground,
					this.wasRecordingBeforePause,
					this.StopRecordingWhenPaused,
					this.IsRecording
				});
			}
			if (this.isPausedOrInBackground == paused)
			{
				return;
			}
			if (paused)
			{
				this.wasRecordingBeforePause = this.IsRecording;
				this.isPausedOrInBackground = true;
				if (this.StopRecordingWhenPaused && this.IsRecording)
				{
					if (base.Logger.IsInfoEnabled)
					{
						base.Logger.LogInfo("Stopping recording as application went to background or paused", Array.Empty<object>());
					}
					this.RemoveVoice();
					return;
				}
			}
			else
			{
				if (!this.StopRecordingWhenPaused)
				{
					if (this.ResetLocalAudio() && base.Logger.IsInfoEnabled)
					{
						base.Logger.LogInfo("Local audio reset as application is back from background or unpaused", Array.Empty<object>());
					}
				}
				else if (this.wasRecordingBeforePause)
				{
					if (!this.IsRecording)
					{
						if (base.Logger.IsInfoEnabled)
						{
							base.Logger.LogInfo("Starting recording as application is back from background or unpaused", Array.Empty<object>());
						}
						this.Setup();
					}
					else if (base.Logger.IsWarningEnabled)
					{
						base.Logger.LogWarning("Unexpected: Application back from background or unpaused, isPausedOrInBackground = true, wasRecordingBeforePause = true, StopRecordingWhenPaused = true, IsRecording = true", Array.Empty<object>());
					}
				}
				this.wasRecordingBeforePause = false;
				this.isPausedOrInBackground = false;
			}
		}

		private SamplingRate GetSupportedSamplingRate(int requested)
		{
			if (Enum.IsDefined(typeof(SamplingRate), requested))
			{
				return (SamplingRate)requested;
			}
			int num = int.MaxValue;
			SamplingRate result = SamplingRate.Sampling48000;
			foreach (object obj in Recorder.samplingRateValues)
			{
				SamplingRate samplingRate = (SamplingRate)obj;
				int num2 = Math.Abs(samplingRate - (SamplingRate)requested);
				if (num2 < num)
				{
					num = num2;
					result = samplingRate;
				}
			}
			return result;
		}

		private SamplingRate GetSupportedSamplingRateForUnityMicrophone(SamplingRate requested)
		{
			int minFreq;
			int maxFreq;
			UnityMicrophone.GetDeviceCaps(this.UnityMicrophoneDevice, out minFreq, out maxFreq);
			return this.GetSupportedSamplingRate(requested, minFreq, maxFreq);
		}

		private SamplingRate GetSupportedSamplingRate(SamplingRate requested, int minFreq, int maxFreq)
		{
			SamplingRate result = requested;
			int num = (int)this.samplingRate;
			if (num < minFreq || (maxFreq != 0 && num > maxFreq))
			{
				if (Enum.IsDefined(typeof(SamplingRate), maxFreq))
				{
					result = (SamplingRate)maxFreq;
				}
				else
				{
					int num2 = int.MaxValue;
					foreach (object obj in Recorder.samplingRateValues)
					{
						SamplingRate samplingRate = (SamplingRate)obj;
						int num3 = (int)samplingRate;
						if (num3 >= minFreq && (maxFreq == 0 || num3 <= maxFreq))
						{
							int num4 = Math.Abs(num3 - maxFreq);
							if (num4 < num2)
							{
								num2 = num4;
								result = samplingRate;
							}
						}
					}
				}
			}
			return result;
		}

		private SamplingRate GetSupportedSamplingRate(SamplingRate sR)
		{
			switch (this.SourceType)
			{
			case Recorder.InputSourceType.Microphone:
			{
				Recorder.MicType micType = this.MicrophoneType;
				if (micType == Recorder.MicType.Unity)
				{
					return this.GetSupportedSamplingRateForUnityMicrophone(sR);
				}
				if (micType != Recorder.MicType.Photon)
				{
					throw new ArgumentOutOfRangeException();
				}
				return SamplingRate.Sampling16000;
			}
			case Recorder.InputSourceType.AudioClip:
				if (this.AudioClip != null)
				{
					return this.GetSupportedSamplingRate(this.AudioClip.frequency);
				}
				break;
			case Recorder.InputSourceType.Factory:
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			return sR;
		}

		private void CheckAndSetSamplingRate(SamplingRate sR)
		{
			if (this.TrySamplingRateMatch)
			{
				SamplingRate supportedSamplingRate = this.GetSupportedSamplingRate(sR);
				if (supportedSamplingRate == this.samplingRate)
				{
					return;
				}
				if (supportedSamplingRate != sR && base.Logger.IsWarningEnabled)
				{
					base.Logger.LogWarning("Sampling rate requested ({0}Hz) not supported using closest value ({1}Hz)", new object[]
					{
						(int)sR,
						(int)supportedSamplingRate
					});
				}
				this.samplingRate = supportedSamplingRate;
			}
			else
			{
				if (sR == this.samplingRate)
				{
					return;
				}
				this.samplingRate = sR;
			}
			if (this.IsRecording)
			{
				this.RequiresRestart = true;
				if (base.Logger.IsInfoEnabled)
				{
					base.Logger.LogInfo("Recorder.{0} changed, Recorder requires restart for this to take effect.", new object[]
					{
						"SamplingRate"
					});
				}
			}
		}

		private void CheckAndSetSamplingRate()
		{
			this.CheckAndSetSamplingRate(this.samplingRate);
		}

		internal void StopRecordingInternal()
		{
			if (base.Logger.IsDebugEnabled)
			{
				base.Logger.LogDebug("Stopping recording", Array.Empty<object>());
			}
			this.wasRecordingBeforePause = false;
			this.RemoveVoice();
			if (this.MicrophoneDeviceChangeDetected)
			{
				this.MicrophoneDeviceChangeDetected = false;
			}
		}

		internal void CheckAndAutoStart()
		{
			this.CheckAndAutoStart(this.autoStart);
		}

		internal void CheckAndAutoStart(bool autoStartFlag)
		{
			bool flag = true;
			if (!autoStartFlag)
			{
				flag = false;
				if (base.Logger.IsDebugEnabled)
				{
					base.Logger.LogDebug("Auto start check failure: autoStart flag is false.", Array.Empty<object>());
				}
			}
			if (!this.IsInitialized)
			{
				flag = false;
				if (base.Logger.IsDebugEnabled)
				{
					base.Logger.LogDebug("Auto start check failure: recorder not initialized.", Array.Empty<object>());
				}
			}
			if (this.isRecording)
			{
				flag = false;
				if (base.Logger.IsDebugEnabled)
				{
					base.Logger.LogDebug("Auto start check failure: recorder is already started.", Array.Empty<object>());
				}
			}
			if (this.recordingStoppedExplicitly)
			{
				flag = false;
				if (base.Logger.IsDebugEnabled)
				{
					base.Logger.LogDebug("Auto start check failure: recorder was previously stopped explicitly.", Array.Empty<object>());
				}
			}
			if (this.recordOnlyWhenEnabled && !base.isActiveAndEnabled)
			{
				flag = false;
				if (base.Logger.IsDebugEnabled)
				{
					base.Logger.LogDebug("Auto start check failure: recorder not enabled and this is required.", Array.Empty<object>());
				}
			}
			if (this.recordOnlyWhenJoined && (this.voiceConnection == null || !this.voiceConnection || this.voiceConnection.Client == null || !this.voiceConnection.Client.InRoom))
			{
				flag = false;
				if (base.Logger.IsDebugEnabled)
				{
					base.Logger.LogDebug("Auto start check failure: voice client not joined to a room yet and this is required.", Array.Empty<object>());
				}
			}
			if (this.SourceType == Recorder.InputSourceType.Microphone && !this.CheckIfThereIsAtLeastOneMic())
			{
				flag = false;
				if (base.Logger.IsDebugEnabled)
				{
					base.Logger.LogDebug("Auto start check failure: no microphone detected.", Array.Empty<object>());
				}
			}
			if (flag)
			{
				if (base.Logger.IsDebugEnabled)
				{
					base.Logger.LogDebug("AutoStart requirements met: going to auto start recording", Array.Empty<object>());
				}
				this.StartRecordingInternal();
				return;
			}
			if (base.Logger.IsDebugEnabled)
			{
				base.Logger.LogDebug("AutoStart requirements NOT met: NOT going to auto start recording", Array.Empty<object>());
			}
		}

		internal void StartRecordingInternal()
		{
			if (base.Logger.IsDebugEnabled)
			{
				base.Logger.LogDebug("Starting recording", Array.Empty<object>());
			}
			this.wasRecordingBeforePause = false;
			this.recordingStoppedExplicitly = false;
			this.Setup();
		}

		private IDeviceEnumerator GetMicrophonesEnumerator(Recorder.MicType micType)
		{
			if (micType == Recorder.MicType.Unity)
			{
				if (this.unityMicrophonesEnumerator == null)
				{
					this.unityMicrophonesEnumerator = new AudioInEnumerator(base.Logger);
					if (!this.unityMicrophonesEnumerator.IsSupported && base.Logger.IsWarningEnabled)
					{
						base.Logger.LogWarning("UnityMicrophonesEnumerator is not supported on this platform {0}.", new object[]
						{
							VoiceComponent.CurrentPlatform
						});
					}
					else if (this.unityMicrophonesEnumerator.Error != null && base.Logger.IsErrorEnabled)
					{
						base.Logger.LogError(this.unityMicrophonesEnumerator.Error, Array.Empty<object>());
					}
				}
				return this.unityMicrophonesEnumerator;
			}
			if (micType != Recorder.MicType.Photon)
			{
				return null;
			}
			if (this.photonMicrophonesEnumerator == null)
			{
				this.photonMicrophonesEnumerator = Recorder.CreatePhotonDeviceEnumerator(base.Logger);
			}
			return this.photonMicrophonesEnumerator;
		}

		private DeviceInfo GetDeviceById(int id)
		{
			foreach (DeviceInfo result in this.MicrophonesEnumerator)
			{
				if (result.IDInt == id)
				{
					return result;
				}
			}
			return DeviceInfo.Default;
		}

		private DeviceInfo GetDeviceById(string id)
		{
			foreach (DeviceInfo result in this.MicrophonesEnumerator)
			{
				if (string.Equals(result.IDString, id))
				{
					return result;
				}
			}
			return DeviceInfo.Default;
		}

		private bool CheckIfThereIsAtLeastOneMic()
		{
			if (this.MicrophoneType == Recorder.MicType.Photon)
			{
				IDeviceEnumerator microphonesEnumerator = this.MicrophonesEnumerator;
				if (microphonesEnumerator != null)
				{
					return microphonesEnumerator.Any<DeviceInfo>();
				}
			}
			return UnityMicrophone.devices.Length != 0;
		}

		private static IDeviceEnumerator CreatePhotonDeviceEnumerator(VoiceLogger voiceLogger)
		{
			IDeviceEnumerator deviceEnumerator = Platform.CreateAudioInEnumerator(voiceLogger);
			if (!deviceEnumerator.IsSupported && voiceLogger.IsWarningEnabled)
			{
				voiceLogger.LogWarning("PhotonMicrophonesEnumerator is not supported on this platform {0}.", new object[]
				{
					VoiceComponent.CurrentPlatform
				});
			}
			else if (deviceEnumerator.Error != null && voiceLogger.IsErrorEnabled)
			{
				voiceLogger.LogError(deviceEnumerator.Error, Array.Empty<object>());
			}
			return deviceEnumerator;
		}

		public const int MIN_OPUS_BITRATE = 6000;

		public const int MAX_OPUS_BITRATE = 510000;

		private static readonly Array samplingRateValues = Enum.GetValues(typeof(SamplingRate));

		[SerializeField]
		private bool voiceDetection;

		[SerializeField]
		private float voiceDetectionThreshold = 0.01f;

		[SerializeField]
		private int voiceDetectionDelayMs = 500;

		private object userData;

		private LocalVoice voice = LocalVoiceAudioDummy.Dummy;

		private string unityMicrophoneDevice;

		private int photonMicrophoneDeviceId = -1;

		private IAudioDesc inputSource;

		private VoiceClient client;

		private VoiceConnection voiceConnection;

		[SerializeField]
		[FormerlySerializedAs("audioGroup")]
		private byte interestGroup;

		[SerializeField]
		private bool debugEchoMode;

		[SerializeField]
		private bool reliableMode;

		[SerializeField]
		private bool encrypt;

		[SerializeField]
		private bool transmitEnabled;

		[SerializeField]
		private SamplingRate samplingRate = SamplingRate.Sampling24000;

		[SerializeField]
		private OpusCodec.FrameDuration frameDuration = OpusCodec.FrameDuration.Frame20ms;

		[SerializeField]
		[Range(6000f, 510000f)]
		private int bitrate = 30000;

		[SerializeField]
		private Recorder.InputSourceType sourceType;

		[SerializeField]
		private Recorder.MicType microphoneType;

		[SerializeField]
		private AudioClip audioClip;

		[SerializeField]
		private bool loopAudioClip = true;

		private bool isRecording;

		private Func<IAudioDesc> inputFactory;

		[Obsolete]
		private static IDeviceEnumerator photonMicrophoneEnumerator;

		private IAudioInChangeNotifier photonMicChangeNotifier;

		[SerializeField]
		private bool reactOnSystemChanges;

		private bool subscribedToSystemChangesPhoton;

		private bool subscribedToSystemChangesUnity;

		[SerializeField]
		private bool autoStart = true;

		[SerializeField]
		private bool recordOnlyWhenEnabled;

		[SerializeField]
		private bool skipDeviceChangeChecks;

		private bool wasRecordingBeforePause;

		private bool isPausedOrInBackground;

		[SerializeField]
		private bool stopRecordingWhenPaused;

		[SerializeField]
		private bool useOnAudioFilterRead;

		[SerializeField]
		private bool trySamplingRateMatch;

		[SerializeField]
		private bool useMicrophoneTypeFallback = true;

		[SerializeField]
		private bool recordOnlyWhenJoined = true;

		private bool recordingStoppedExplicitly;

		private IDeviceEnumerator photonMicrophonesEnumerator;

		private AudioInEnumerator unityMicrophonesEnumerator;

		private object microphoneDeviceChangeDetectedLock = new object();

		internal bool microphoneDeviceChangeDetected;

		public enum InputSourceType
		{
			Microphone,
			AudioClip,
			Factory
		}

		public enum MicType
		{
			Unity,
			Photon
		}

		[Obsolete("No longer needed. Implicit conversion is done internally when needed.")]
		public enum SampleTypeConv
		{
			None,
			Short
		}

		[Obsolete("Use Photon.Voice.Unity.PhotonVoiceCreatedParams")]
		public class PhotonVoiceCreatedParams : Photon.Voice.Unity.PhotonVoiceCreatedParams
		{
		}
	}
}
