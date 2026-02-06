using System;
using System.Collections.Generic;
using UnityEngine;

namespace Photon.Voice.Unity
{
	[RequireComponent(typeof(Recorder))]
	[DisallowMultipleComponent]
	public class WebRtcAudioDsp : VoiceComponent
	{
		public bool AEC
		{
			get
			{
				object obj = this.threadSafety;
				lock (obj)
				{
					if (this.IsInitialized && (!this.aecOnlyWhenEnabled || base.isActiveAndEnabled))
					{
						return this.aecStarted;
					}
				}
				return this.aec;
			}
			set
			{
				if (value == this.aec)
				{
					return;
				}
				this.aec = value;
				object obj = this.threadSafety;
				lock (obj)
				{
					this.ToggleAec();
				}
			}
		}

		[Obsolete("Use AEC instead on all platforms, internally according AEC will be used either mobile or not.")]
		public bool AECMobile
		{
			get
			{
				return this.AEC;
			}
			set
			{
				this.AEC = value;
			}
		}

		public bool AecHighPass
		{
			get
			{
				return this.aecHighPass;
			}
			set
			{
				if (value == this.aecHighPass)
				{
					return;
				}
				this.aecHighPass = value;
				object obj = this.threadSafety;
				lock (obj)
				{
					if (this.IsInitialized)
					{
						this.proc.AECHighPass = this.aecHighPass;
					}
				}
			}
		}

		public int ReverseStreamDelayMs
		{
			get
			{
				return this.reverseStreamDelayMs;
			}
			set
			{
				if (this.reverseStreamDelayMs == value)
				{
					return;
				}
				this.reverseStreamDelayMs = value;
				object obj = this.threadSafety;
				lock (obj)
				{
					if (this.IsInitialized)
					{
						this.proc.AECStreamDelayMs = this.reverseStreamDelayMs;
					}
				}
			}
		}

		public bool NoiseSuppression
		{
			get
			{
				return this.noiseSuppression;
			}
			set
			{
				if (value == this.noiseSuppression)
				{
					return;
				}
				this.noiseSuppression = value;
				object obj = this.threadSafety;
				lock (obj)
				{
					if (this.IsInitialized)
					{
						this.proc.NoiseSuppression = this.noiseSuppression;
					}
				}
			}
		}

		public bool HighPass
		{
			get
			{
				return this.highPass;
			}
			set
			{
				if (value == this.highPass)
				{
					return;
				}
				this.highPass = value;
				object obj = this.threadSafety;
				lock (obj)
				{
					if (this.IsInitialized)
					{
						this.proc.HighPass = this.highPass;
					}
				}
			}
		}

		public bool Bypass
		{
			get
			{
				return this.bypass;
			}
			set
			{
				if (value == this.bypass)
				{
					return;
				}
				this.bypass = value;
				if (this.IsInitialized)
				{
					this.proc.Bypass = this.bypass;
				}
			}
		}

		public bool AGC
		{
			get
			{
				return this.agc;
			}
			set
			{
				if (value == this.agc)
				{
					return;
				}
				this.agc = value;
				object obj = this.threadSafety;
				lock (obj)
				{
					if (this.IsInitialized)
					{
						this.proc.AGC = this.agc;
					}
				}
			}
		}

		public int AgcCompressionGain
		{
			get
			{
				return this.agcCompressionGain;
			}
			set
			{
				if (this.agcCompressionGain == value)
				{
					return;
				}
				if (value < 0 || value > 90)
				{
					if (base.Logger.IsErrorEnabled)
					{
						base.Logger.LogError("AgcCompressionGain value {0} not in range [0..90]", new object[]
						{
							value
						});
					}
					return;
				}
				this.agcCompressionGain = value;
				object obj = this.threadSafety;
				lock (obj)
				{
					if (this.IsInitialized)
					{
						this.proc.AGCCompressionGain = this.agcCompressionGain;
					}
				}
			}
		}

		public bool VAD
		{
			get
			{
				return this.vad;
			}
			set
			{
				if (value == this.vad)
				{
					return;
				}
				this.vad = value;
				object obj = this.threadSafety;
				lock (obj)
				{
					if (this.IsInitialized)
					{
						this.proc.VAD = this.vad;
					}
				}
			}
		}

		public bool IsInitialized
		{
			get
			{
				return this.proc != null;
			}
		}

		public bool AecOnlyWhenEnabled
		{
			get
			{
				return this.aecOnlyWhenEnabled;
			}
			set
			{
				if (this.aecOnlyWhenEnabled != value)
				{
					this.aecOnlyWhenEnabled = value;
					object obj = this.threadSafety;
					lock (obj)
					{
						this.ToggleAec();
					}
				}
			}
		}

		protected override void Awake()
		{
			base.Awake();
			AudioSettings.OnAudioConfigurationChanged += this.OnAudioConfigurationChanged;
			if (this.SupportedPlatformCheck())
			{
				this.recorder = base.GetComponent<Recorder>();
				if (this.recorder == null || !this.recorder)
				{
					if (base.Logger.IsErrorEnabled)
					{
						base.Logger.LogError("A Recorder component needs to be attached to the same GameObject", Array.Empty<object>());
					}
					base.enabled = false;
					return;
				}
				if (!base.IgnoreGlobalLogLevel)
				{
					base.LogLevel = this.recorder.LogLevel;
				}
			}
		}

		private void OnEnable()
		{
			object obj = this.threadSafety;
			lock (obj)
			{
				if (this.SupportedPlatformCheck())
				{
					if (this.IsInitialized)
					{
						this.ToggleAec();
					}
					else if (this.recorder.IsRecording)
					{
						if (base.Logger.IsWarningEnabled)
						{
							base.Logger.LogWarning("WebRtcAudioDsp is added after recording has started, restarting recording to take effect", Array.Empty<object>());
						}
						this.recorder.RestartRecording(true);
					}
				}
			}
		}

		private void OnDisable()
		{
			object obj = this.threadSafety;
			lock (obj)
			{
				if (this.aecOnlyWhenEnabled && this.aecStarted)
				{
					this.ToggleAecOutputListener(false);
				}
			}
		}

		private bool SupportedPlatformCheck()
		{
			return true;
		}

		private void ToggleAec()
		{
			if (this.IsInitialized && (!this.aecOnlyWhenEnabled || base.isActiveAndEnabled) && this.aec != this.aecStarted)
			{
				if (base.Logger.IsDebugEnabled)
				{
					base.Logger.LogDebug("Toggling AEC to {0}", new object[]
					{
						this.aec
					});
				}
				if (!this.ToggleAecOutputListener(this.aec))
				{
					if (base.Logger.IsErrorEnabled)
					{
						base.Logger.LogError("AEC failed to be toggled to {0}", new object[]
						{
							this.aec
						});
						return;
					}
				}
				else if (base.Logger.IsDebugEnabled)
				{
					base.Logger.LogDebug("AEC successfully toggled to {0}", new object[]
					{
						this.aec
					});
				}
			}
		}

		private bool ToggleAecOutputListener(bool on)
		{
			if (on != this.aecStarted)
			{
				if (on)
				{
					if (this.aecOnlyWhenEnabled && !base.isActiveAndEnabled)
					{
						if (base.Logger.IsErrorEnabled)
						{
							base.Logger.LogError("Could not start AEC because AecOnlyWhenEnabled is true and isActiveAndEnabled is false", Array.Empty<object>());
						}
						return false;
					}
					if (this.audioOutCapture == null || !this.audioOutCapture)
					{
						if (!this.InitAudioOutCapture())
						{
							if (base.Logger.IsErrorEnabled)
							{
								base.Logger.LogError("Could not start AEC OutputListener because a valid AudioOutCapture could not be set.", Array.Empty<object>());
							}
							return false;
						}
					}
					else
					{
						if (!this.AudioOutCaptureChecks(this.audioOutCapture, true, true))
						{
							if (base.Logger.IsErrorEnabled)
							{
								base.Logger.LogError("Could not start AEC OutputListener because AudioOutCapture provided is not valid.", Array.Empty<object>());
							}
							return false;
						}
						AudioListener component = this.audioOutCapture.GetComponent<AudioListener>();
						if (this.audioListener != component)
						{
							if (base.Logger.IsWarningEnabled)
							{
								base.Logger.LogWarning("Unexpected: AudioListener changed but AudioOutCapture did not.", Array.Empty<object>());
							}
							this.audioListener = component;
						}
					}
					if (this.IsInitialized)
					{
						this.StartAec();
					}
				}
				else
				{
					if (this.UnsubscribeFromAudioOutCapture(this.autoDestroyAudioOutCapture))
					{
						if (base.Logger.IsDebugEnabled)
						{
							base.Logger.LogDebug("AEC OutputListener stopped.", Array.Empty<object>());
						}
					}
					else if (base.Logger.IsWarningEnabled)
					{
						base.Logger.LogWarning("Unexpected: AudioOutCapture is null but aecStarted == true", Array.Empty<object>());
					}
					if (this.IsInitialized)
					{
						this.proc.AEC = false;
						this.proc.AECMobile = false;
					}
					else if (base.Logger.IsWarningEnabled)
					{
						base.Logger.LogWarning("Unexpected: proc is null but aecStarted was true.", Array.Empty<object>());
					}
					this.aecStarted = false;
				}
				return true;
			}
			return false;
		}

		private void StartAec()
		{
			this.proc.AECStreamDelayMs = this.reverseStreamDelayMs;
			this.proc.AECHighPass = this.aecHighPass;
			this.proc.AEC = true;
			this.proc.AECMobile = false;
			this.aecStarted = true;
			this.audioOutCapture.OnAudioFrame += this.OnAudioOutFrameFloat;
			if (base.Logger.IsDebugEnabled)
			{
				base.Logger.LogDebug("AEC OutputListener started.", Array.Empty<object>());
			}
		}

		private void OnAudioConfigurationChanged(bool deviceWasChanged)
		{
			object obj = this.threadSafety;
			lock (obj)
			{
				if (this.IsInitialized)
				{
					bool flag2 = false;
					if (this.outputSampleRate != AudioSettings.outputSampleRate)
					{
						if (base.Logger.IsInfoEnabled)
						{
							base.Logger.LogInfo("AudioConfigChange: outputSampleRate from {0} to {1}. WebRtcAudioDsp will be restarted.", new object[]
							{
								this.outputSampleRate,
								AudioSettings.outputSampleRate
							});
						}
						this.outputSampleRate = AudioSettings.outputSampleRate;
						flag2 = true;
					}
					if (this.reverseChannels != WebRtcAudioDsp.channelsMap[AudioSettings.speakerMode])
					{
						if (base.Logger.IsInfoEnabled)
						{
							base.Logger.LogInfo("AudioConfigChange: speakerMode channels from {0} to {1}. WebRtcAudioDsp will be restarted.", new object[]
							{
								this.reverseChannels,
								WebRtcAudioDsp.channelsMap[AudioSettings.speakerMode]
							});
						}
						this.reverseChannels = WebRtcAudioDsp.channelsMap[AudioSettings.speakerMode];
						flag2 = true;
					}
					if (flag2)
					{
						this.Restart();
					}
				}
			}
		}

		private void OnAudioOutFrameFloat(float[] data, int outChannels)
		{
			object obj = this.threadSafety;
			lock (obj)
			{
				if (!this.IsInitialized)
				{
					if (base.Logger.IsErrorEnabled)
					{
						base.Logger.LogError("Unexpected: OnAudioOutFrame called while WebRtcAudioDsp is not initialized (proc == null).", Array.Empty<object>());
					}
				}
				else
				{
					if (!this.aecStarted && base.Logger.IsErrorEnabled)
					{
						base.Logger.LogError("Unexpected: OnAudioOutFrame called while aecStarted is false.", Array.Empty<object>());
					}
					if (outChannels != this.reverseChannels)
					{
						if (base.Logger.IsErrorEnabled)
						{
							base.Logger.LogError("Unexpected: OnAudioOutFrame channel count {0} != initialized {1}. Switching channels and restarting.", new object[]
							{
								outChannels,
								this.reverseChannels
							});
						}
						if (this.AutoRestartOnAudioChannelsMismatch)
						{
							this.reverseChannels = outChannels;
							this.Restart();
						}
					}
					else
					{
						this.proc.OnAudioOutFrameFloat(data);
					}
				}
			}
		}

		private void PhotonVoiceCreated(PhotonVoiceCreatedParams p)
		{
			object obj = this.threadSafety;
			lock (obj)
			{
				if (!base.enabled)
				{
					if (base.Logger.IsInfoEnabled)
					{
						base.Logger.LogInfo("Skipped PhotonVoiceCreated message because component is disabled.", Array.Empty<object>());
					}
				}
				else
				{
					if (this.recorder != null && this.recorder.SourceType != Recorder.InputSourceType.Microphone && base.Logger.IsWarningEnabled)
					{
						base.Logger.LogWarning("WebRtcAudioDsp is better suited to be used with Microphone as Recorder Input Source Type.", Array.Empty<object>());
					}
					if (p.Voice.Info.Channels != 1)
					{
						if (base.Logger.IsErrorEnabled)
						{
							base.Logger.LogError("Only mono audio signals supported. WebRtcAudioDsp component will be disabled.", Array.Empty<object>());
						}
						base.enabled = false;
					}
					else
					{
						LocalVoiceAudioShort localVoiceAudioShort = p.Voice as LocalVoiceAudioShort;
						if (localVoiceAudioShort != null)
						{
							this.localVoice = localVoiceAudioShort;
							this.reverseChannels = WebRtcAudioDsp.channelsMap[AudioSettings.speakerMode];
							this.outputSampleRate = AudioSettings.outputSampleRate;
							this.Init();
							this.localVoice.AddPostProcessor(new IProcessor<short>[]
							{
								this.proc
							});
							this.ToggleAec();
						}
						else
						{
							if (base.Logger.IsErrorEnabled)
							{
								base.Logger.LogError("Only short audio voice supported. WebRtcAudioDsp component will be disabled.", Array.Empty<object>());
							}
							base.enabled = false;
						}
					}
				}
			}
		}

		private void PhotonVoiceRemoved()
		{
			this.StopAllProcessing();
		}

		private void OnDestroy()
		{
			this.StopAllProcessing();
			AudioSettings.OnAudioConfigurationChanged -= this.OnAudioConfigurationChanged;
		}

		private void StopAllProcessing()
		{
			object obj = this.threadSafety;
			lock (obj)
			{
				this.ToggleAecOutputListener(false);
				if (this.IsInitialized)
				{
					this.proc.Dispose();
					this.proc = null;
				}
				this.localVoice = null;
			}
		}

		private void Restart()
		{
			if (base.Logger.IsDebugEnabled)
			{
				base.Logger.LogDebug("Restarting", Array.Empty<object>());
			}
			if (this.IsInitialized)
			{
				bool flag = false;
				if (this.aecStarted)
				{
					if (this.UnsubscribeFromAudioOutCapture(false))
					{
						if (base.Logger.IsDebugEnabled)
						{
							base.Logger.LogDebug("AEC OutputListener stopped.", Array.Empty<object>());
						}
						flag = true;
						this.aecStarted = false;
					}
					else if (base.Logger.IsWarningEnabled)
					{
						base.Logger.LogWarning("Unexpected: AudioOutCapture is null but aecStarted == true", Array.Empty<object>());
					}
				}
				this.proc.Dispose();
				this.proc = null;
				if (this.Init())
				{
					this.localVoice.AddPostProcessor(new IProcessor<short>[]
					{
						this.proc
					});
					if (flag)
					{
						this.StartAec();
					}
					if (base.Logger.IsInfoEnabled)
					{
						base.Logger.LogInfo("Restart complete successfully.", Array.Empty<object>());
						return;
					}
				}
				else if (base.Logger.IsErrorEnabled)
				{
					base.Logger.LogError("Restart failed because processor could not be re initialized.", Array.Empty<object>());
					return;
				}
			}
			else if (base.Logger.IsErrorEnabled)
			{
				base.Logger.LogError("Cannot restart if not initialized.", Array.Empty<object>());
			}
		}

		private bool Init()
		{
			if (this.IsInitialized)
			{
				if (base.Logger.IsErrorEnabled)
				{
					base.Logger.LogError("Already initialized", Array.Empty<object>());
				}
				return false;
			}
			this.proc = new WebRTCAudioProcessor(base.Logger, this.localVoice.Info.FrameSize, this.localVoice.Info.SamplingRate, this.localVoice.Info.Channels, this.outputSampleRate, this.reverseChannels);
			this.proc.HighPass = this.highPass;
			this.proc.NoiseSuppression = this.noiseSuppression;
			this.proc.AGC = this.agc;
			this.proc.AGCCompressionGain = this.agcCompressionGain;
			this.proc.VAD = this.vad;
			this.proc.Bypass = this.bypass;
			if (base.Logger.IsInfoEnabled)
			{
				base.Logger.LogInfo("Initialized", Array.Empty<object>());
			}
			return true;
		}

		private bool SetOrSwitchAudioListener(AudioListener listener, bool extraChecks, bool log = true)
		{
			if (extraChecks && !this.AudioListenerChecks(listener, true))
			{
				return false;
			}
			AudioOutCapture[] components = listener.GetComponents<AudioOutCapture>();
			if (components.Length > 1 && base.Logger.IsDebugEnabled)
			{
				base.Logger.LogDebug("{0} AudioOutCapture components attached to the same GameObject, is this expected?", new object[]
				{
					components.Length
				});
			}
			for (int i = 0; i < components.Length; i++)
			{
				if (this.SetOrSwitchAudioOutCapture(components[i], false, false))
				{
					this.autoDestroyAudioOutCapture = false;
					return true;
				}
			}
			AudioOutCapture audioOutCapture = listener.gameObject.AddComponent<AudioOutCapture>();
			if (this.SetOrSwitchAudioOutCapture(audioOutCapture, false, log))
			{
				if (base.Logger.IsDebugEnabled)
				{
					base.Logger.LogDebug("AudioOutCapture component added to same GameObject as AudioListener.", Array.Empty<object>());
				}
				this.autoDestroyAudioOutCapture = true;
				return true;
			}
			Object.Destroy(audioOutCapture);
			return false;
		}

		private bool SetOrSwitchAudioOutCapture(AudioOutCapture capture, bool extraChecks, bool log = true)
		{
			if (!this.AudioOutCaptureChecks(capture, extraChecks, log))
			{
				return false;
			}
			bool flag = this.aecStarted;
			bool flag2 = false;
			if (this.audioOutCapture != null && this.audioOutCapture)
			{
				if (this.audioOutCapture != capture)
				{
					if (!this.UnsubscribeFromAudioOutCapture(this.autoDestroyAudioOutCapture))
					{
						if (base.Logger.IsErrorEnabled)
						{
							base.Logger.LogError("Could not unsubscribe from previous AudioOutCapture. Switching to a new one won't happen.", Array.Empty<object>());
						}
						return false;
					}
					flag2 = true;
				}
				else if (extraChecks)
				{
					if (log && base.Logger.IsErrorEnabled)
					{
						base.Logger.LogError("The same AudioOutCapture is being used already", Array.Empty<object>());
					}
					return false;
				}
			}
			this.audioOutCapture = capture;
			this.audioListener = capture.GetComponent<AudioListener>();
			if (flag && flag2)
			{
				this.audioOutCapture.OnAudioFrame += this.OnAudioOutFrameFloat;
			}
			return true;
		}

		private bool InitAudioOutCapture()
		{
			if (this.audioOutCapture != null && this.audioOutCapture)
			{
				if (base.Logger.IsErrorEnabled)
				{
					base.Logger.LogError("AudioOutCapture is already initialized.", Array.Empty<object>());
				}
				return false;
			}
			if (this.audioListener == null)
			{
				AudioOutCapture[] array = Object.FindObjectsOfType<AudioOutCapture>();
				if (array.Length > 1 && base.Logger.IsDebugEnabled)
				{
					base.Logger.LogDebug("{0} AudioOutCapture components found, is this expected?", new object[]
					{
						array.Length
					});
				}
				foreach (AudioOutCapture capture in array)
				{
					if (this.SetOrSwitchAudioOutCapture(capture, true, false))
					{
						this.autoDestroyAudioOutCapture = false;
						return true;
					}
				}
				AudioListener[] array2 = Object.FindObjectsOfType<AudioListener>();
				if (array2.Length == 0)
				{
					if (base.Logger.IsErrorEnabled)
					{
						base.Logger.LogError("No AudioListener component found, is this expected?", Array.Empty<object>());
					}
				}
				else if (array2.Length > 1 && base.Logger.IsDebugEnabled)
				{
					base.Logger.LogDebug("{0} AudioListener components found, is this expected?", new object[]
					{
						array2.Length
					});
				}
				foreach (AudioListener listener in array2)
				{
					if (this.SetOrSwitchAudioListener(listener, true, false))
					{
						return true;
					}
				}
				if (base.Logger.IsErrorEnabled)
				{
					base.Logger.LogError("AudioListener and AudioOutCapture components are required for AEC to work.", Array.Empty<object>());
				}
				return false;
			}
			return this.SetOrSwitchAudioListener(this.audioListener, true, true);
		}

		private bool UnsubscribeFromAudioOutCapture(bool destroy)
		{
			if (this.audioOutCapture != null)
			{
				if (this.aecStarted)
				{
					this.audioOutCapture.OnAudioFrame -= this.OnAudioOutFrameFloat;
					if (base.Logger.IsDebugEnabled)
					{
						base.Logger.LogDebug("OnAudioFrame event unsubscribed.", Array.Empty<object>());
					}
				}
				if (destroy)
				{
					Object.Destroy(this.audioOutCapture);
					if (base.Logger.IsDebugEnabled)
					{
						base.Logger.LogDebug("AudioOutCapture component destroyed.", Array.Empty<object>());
					}
					this.audioOutCapture = null;
				}
				return true;
			}
			if (this.aecStarted && base.Logger.IsErrorEnabled)
			{
				base.Logger.LogError("Unexpected: audioOutCapture is null but aecStarted is true", Array.Empty<object>());
			}
			return false;
		}

		private bool AudioListenerChecks(AudioListener listener, bool log = true)
		{
			if (listener == null)
			{
				if (log && base.Logger.IsErrorEnabled)
				{
					base.Logger.LogError("AudioListener is null.", Array.Empty<object>());
				}
				return false;
			}
			if (!listener)
			{
				if (log && base.Logger.IsErrorEnabled)
				{
					base.Logger.LogError("AudioListener is destroyed.", Array.Empty<object>());
				}
				return false;
			}
			if (!listener.gameObject.activeInHierarchy)
			{
				if (log && base.Logger.IsErrorEnabled)
				{
					base.Logger.LogError("The GameObject to which the AudioListener is attached is not active in hierarchy.", Array.Empty<object>());
				}
				return false;
			}
			if (!listener.enabled)
			{
				if (log && base.Logger.IsErrorEnabled)
				{
					base.Logger.LogError("AudioListener is disabled.", Array.Empty<object>());
				}
				return false;
			}
			return true;
		}

		private bool AudioOutCaptureChecks(AudioOutCapture capture, bool listenerChecks, bool log = true)
		{
			if (capture == null)
			{
				if (log && base.Logger.IsErrorEnabled)
				{
					base.Logger.LogError("AudioOutCapture is null.", Array.Empty<object>());
				}
				return false;
			}
			if (!capture)
			{
				if (log && base.Logger.IsErrorEnabled)
				{
					base.Logger.LogError("AudioOutCapture is destroyed.", Array.Empty<object>());
				}
				return false;
			}
			if (!listenerChecks && !capture.gameObject.activeInHierarchy)
			{
				if (log && base.Logger.IsErrorEnabled)
				{
					base.Logger.LogError("The GameObject to which the AudioOutCapture is attached is not active in hierarchy.", Array.Empty<object>());
				}
				return false;
			}
			if (!capture.enabled)
			{
				if (log && base.Logger.IsErrorEnabled)
				{
					base.Logger.LogError("AudioOutCapture is disabled.", Array.Empty<object>());
				}
				return false;
			}
			return !listenerChecks || this.AudioListenerChecks(capture.GetComponent<AudioListener>(), log);
		}

		public bool SetOrSwitchAudioListener(AudioListener listener)
		{
			object obj = this.threadSafety;
			bool result;
			lock (obj)
			{
				result = this.SetOrSwitchAudioListener(listener, true, true);
			}
			return result;
		}

		public bool SetOrSwitchAudioOutCapture(AudioOutCapture capture)
		{
			object obj = this.threadSafety;
			bool result;
			lock (obj)
			{
				if (this.SetOrSwitchAudioOutCapture(capture, true, true))
				{
					this.autoDestroyAudioOutCapture = false;
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		[SerializeField]
		private bool aec = true;

		[SerializeField]
		private bool aecHighPass;

		[SerializeField]
		private bool agc = true;

		[SerializeField]
		private int agcCompressionGain = 9;

		[SerializeField]
		private bool vad = true;

		[SerializeField]
		private bool highPass;

		[SerializeField]
		private bool bypass;

		[SerializeField]
		private bool noiseSuppression;

		[SerializeField]
		private int reverseStreamDelayMs = 120;

		private int reverseChannels;

		private WebRTCAudioProcessor proc;

		private AudioListener audioListener;

		private AudioOutCapture audioOutCapture;

		private bool aecStarted;

		private bool autoDestroyAudioOutCapture;

		private static readonly Dictionary<AudioSpeakerMode, int> channelsMap = new Dictionary<AudioSpeakerMode, int>
		{
			{
				AudioSpeakerMode.Mono,
				1
			},
			{
				AudioSpeakerMode.Stereo,
				2
			},
			{
				AudioSpeakerMode.Quad,
				4
			},
			{
				AudioSpeakerMode.Surround,
				5
			},
			{
				AudioSpeakerMode.Mode5point1,
				6
			},
			{
				AudioSpeakerMode.Mode7point1,
				8
			},
			{
				AudioSpeakerMode.Prologic,
				2
			}
		};

		private LocalVoiceAudioShort localVoice;

		private int outputSampleRate;

		private Recorder recorder;

		[SerializeField]
		private bool aecOnlyWhenEnabled = true;

		public bool AutoRestartOnAudioChannelsMismatch = true;

		private object threadSafety = new object();

		[Obsolete("Obsolete as it's not recommended to set this to true. https://forum.photonengine.com/discussion/comment/48017/#Comment_48017")]
		public bool AECMobileComfortNoise;
	}
}
