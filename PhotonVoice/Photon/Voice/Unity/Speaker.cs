using System;
using Photon.Realtime;
using UnityEngine;

namespace Photon.Voice.Unity
{
	[RequireComponent(typeof(AudioSource))]
	[AddComponentMenu("Photon Voice/Speaker")]
	[DisallowMultipleComponent]
	public class Speaker : VoiceComponent
	{
		[Obsolete("Use SetPlaybackDelaySettings methods instead")]
		public int PlayDelayMs
		{
			get
			{
				return this.playbackDelaySettings.MinDelaySoft;
			}
			set
			{
				if (value >= 0 && value < this.playbackDelaySettings.MaxDelaySoft)
				{
					this.playbackDelaySettings.MinDelaySoft = value;
				}
			}
		}

		public bool IsPlaying
		{
			get
			{
				return this.IsInitialized && this.audioOutput.IsPlaying;
			}
		}

		public int Lag
		{
			get
			{
				if (!this.IsPlaying)
				{
					return -1;
				}
				return this.audioOutput.Lag;
			}
		}

		public Action<Speaker> OnRemoteVoiceRemoveAction { get; set; }

		public Player Actor { get; protected internal set; }

		public bool IsLinked
		{
			get
			{
				return this.remoteVoiceLink != null;
			}
		}

		internal RemoteVoiceLink RemoteVoiceLink
		{
			get
			{
				return this.remoteVoiceLink;
			}
		}

		public bool PlaybackOnlyWhenEnabled
		{
			get
			{
				return this.playbackOnlyWhenEnabled;
			}
			set
			{
				if (this.playbackOnlyWhenEnabled != value)
				{
					this.playbackOnlyWhenEnabled = value;
					if (this.IsLinked)
					{
						if (this.playbackOnlyWhenEnabled)
						{
							if (base.isActiveAndEnabled != this.PlaybackStarted)
							{
								if (!base.isActiveAndEnabled)
								{
									this.StopPlaying(false);
									return;
								}
								if (!this.playbackExplicitlyStopped)
								{
									this.StartPlaying();
									return;
								}
							}
						}
						else if (!this.PlaybackStarted && !this.playbackExplicitlyStopped)
						{
							this.StartPlaying();
						}
					}
				}
			}
		}

		public bool PlaybackStarted { get; private set; }

		public int PlaybackDelayMinSoft
		{
			get
			{
				return this.playbackDelaySettings.MinDelaySoft;
			}
		}

		public int PlaybackDelayMaxSoft
		{
			get
			{
				return this.playbackDelaySettings.MaxDelaySoft;
			}
		}

		public int PlaybackDelayMaxHard
		{
			get
			{
				return this.playbackDelaySettings.MaxDelayHard;
			}
		}

		protected bool IsInitialized
		{
			get
			{
				return this.audioOutput != null;
			}
		}

		private void OnEnable()
		{
			if (this.IsLinked && !this.PlaybackStarted && !this.playbackExplicitlyStopped)
			{
				this.StartPlaying();
			}
		}

		private void OnDisable()
		{
			if (this.PlaybackOnlyWhenEnabled && this.PlaybackStarted)
			{
				this.StopPlaying(false);
			}
		}

		protected virtual void Initialize()
		{
			if (this.IsInitialized)
			{
				if (base.Logger.IsWarningEnabled)
				{
					base.Logger.LogWarning("Already initialized.", Array.Empty<object>());
				}
				return;
			}
			if (base.Logger.IsDebugEnabled)
			{
				base.Logger.LogDebug("Initializing.", Array.Empty<object>());
			}
			Func<IAudioOut<float>> func;
			if (this.CustomAudioOutFactory != null)
			{
				func = this.CustomAudioOutFactory;
			}
			else
			{
				func = this.GetDefaultAudioOutFactory();
			}
			this.audioOutput = func();
			if (base.Logger.IsDebugEnabled)
			{
				base.Logger.LogDebug("Initialized.", Array.Empty<object>());
			}
		}

		internal Func<IAudioOut<float>> GetDefaultAudioOutFactory()
		{
			AudioOutDelayControl.PlayDelayConfig pdc = new AudioOutDelayControl.PlayDelayConfig
			{
				Low = this.playbackDelaySettings.MinDelaySoft,
				High = this.playbackDelaySettings.MaxDelaySoft,
				Max = this.playbackDelaySettings.MaxDelayHard
			};
			return () => new UnityAudioOut(this.GetComponent<AudioSource>(), pdc, this.Logger, string.Empty, this.Logger.IsDebugEnabled);
		}

		internal bool OnRemoteVoiceInfo(RemoteVoiceLink stream)
		{
			if (stream == null)
			{
				if (base.Logger.IsErrorEnabled)
				{
					base.Logger.LogError("RemoteVoiceLink is null, cancelled linking", Array.Empty<object>());
				}
				return false;
			}
			if (!this.IsInitialized)
			{
				this.Initialize();
			}
			if (base.Logger.IsDebugEnabled)
			{
				base.Logger.LogDebug("OnRemoteVoiceInfo {0}", new object[]
				{
					stream
				});
			}
			if (this.IsLinked)
			{
				if (base.Logger.IsWarningEnabled)
				{
					base.Logger.LogWarning("Speaker already linked to {0}, cancelled linking to {1}", new object[]
					{
						this.remoteVoiceLink,
						stream
					});
				}
				return false;
			}
			if (stream.Info.Channels <= 0)
			{
				if (base.Logger.IsErrorEnabled)
				{
					base.Logger.LogError("Received voice info channels is not expected (<= 0), cancelled linking to {0}", new object[]
					{
						stream
					});
				}
				return false;
			}
			this.remoteVoiceLink = stream;
			this.remoteVoiceLink.RemoteVoiceRemoved += this.OnRemoteVoiceRemove;
			return this.IsInitialized && ((this.PlaybackOnlyWhenEnabled && !base.isActiveAndEnabled) || this.StartPlayback());
		}

		internal void OnRemoteVoiceRemove()
		{
			if (base.Logger.IsDebugEnabled)
			{
				base.Logger.LogDebug("OnRemoteVoiceRemove {0}", new object[]
				{
					this.remoteVoiceLink
				});
			}
			this.StopPlaying(false);
			if (this.OnRemoteVoiceRemoveAction != null)
			{
				this.OnRemoteVoiceRemoveAction(this);
			}
			this.CleanUp();
		}

		protected virtual void OnAudioFrame(FrameOut<float> frame)
		{
			this.audioOutput.Push(frame.Buf);
			if (frame.EndOfStream)
			{
				this.audioOutput.Flush();
			}
		}

		private bool StartPlaying()
		{
			if (!this.IsLinked)
			{
				if (base.Logger.IsWarningEnabled)
				{
					base.Logger.LogWarning("Cannot start playback because speaker is not linked", Array.Empty<object>());
				}
				return false;
			}
			if (this.PlaybackStarted)
			{
				if (base.Logger.IsWarningEnabled)
				{
					base.Logger.LogWarning("Playback is already started", Array.Empty<object>());
				}
				return false;
			}
			if (!this.IsInitialized)
			{
				if (base.Logger.IsWarningEnabled)
				{
					base.Logger.LogWarning("Cannot start playback because not initialized yet", Array.Empty<object>());
				}
				return false;
			}
			if (!base.isActiveAndEnabled && this.PlaybackOnlyWhenEnabled)
			{
				if (base.Logger.IsWarningEnabled)
				{
					base.Logger.LogWarning("Cannot start playback because PlaybackOnlyWhenEnabled is true and Speaker is not enabled or its GameObject is not active in the hierarchy.", Array.Empty<object>());
				}
				return false;
			}
			VoiceInfo info = this.remoteVoiceLink.Info;
			if (info.Channels == 0)
			{
				if (base.Logger.IsErrorEnabled)
				{
					base.Logger.LogError("Cannot start playback because Channels == 0, stream {0}", new object[]
					{
						this.remoteVoiceLink
					});
				}
				return false;
			}
			if (base.Logger.IsInfoEnabled)
			{
				base.Logger.LogInfo("Speaker about to start playback stream {0}, delay {1}", new object[]
				{
					this.remoteVoiceLink,
					this.playbackDelaySettings
				});
			}
			this.AudioOutputStart(info.SamplingRate, info.Channels, info.FrameDurationSamples);
			this.remoteVoiceLink.FloatFrameDecoded += this.OnAudioFrame;
			this.PlaybackStarted = true;
			this.playbackExplicitlyStopped = false;
			return true;
		}

		protected virtual void AudioOutputStart(int frequency, int channels, int frameSamplesPerChannel)
		{
			this.audioOutput.Start(frequency, channels, frameSamplesPerChannel);
		}

		private void OnDestroy()
		{
			if (base.Logger.IsDebugEnabled)
			{
				base.Logger.LogDebug("OnDestroy", Array.Empty<object>());
			}
			this.StopPlaying(true);
			this.CleanUp();
		}

		private bool StopPlaying(bool force = false)
		{
			if (base.Logger.IsDebugEnabled)
			{
				base.Logger.LogDebug("StopPlaying", Array.Empty<object>());
			}
			if (!force && !this.PlaybackStarted)
			{
				if (base.Logger.IsWarningEnabled)
				{
					base.Logger.LogWarning("Cannot stop playback because it's not started", Array.Empty<object>());
				}
				return false;
			}
			if (this.IsLinked)
			{
				this.remoteVoiceLink.FloatFrameDecoded -= this.OnAudioFrame;
			}
			else if (!force && base.Logger.IsWarningEnabled)
			{
				base.Logger.LogWarning("Speaker not linked while stopping playback", Array.Empty<object>());
			}
			if (this.IsInitialized)
			{
				this.AudioOutputStop();
			}
			else if (!force && base.Logger.IsWarningEnabled)
			{
				base.Logger.LogWarning("audioOutput is null while stopping playback", Array.Empty<object>());
			}
			this.PlaybackStarted = false;
			return true;
		}

		protected virtual void AudioOutputStop()
		{
			this.audioOutput.Stop();
		}

		private void CleanUp()
		{
			if (base.Logger.IsDebugEnabled)
			{
				base.Logger.LogDebug("CleanUp", Array.Empty<object>());
			}
			if (this.remoteVoiceLink != null)
			{
				this.remoteVoiceLink.RemoteVoiceRemoved -= this.OnRemoteVoiceRemove;
				this.remoteVoiceLink = null;
			}
			this.Actor = null;
		}

		internal void Service()
		{
			if (this.PlaybackStarted)
			{
				this.AudioOutputService();
			}
		}

		protected virtual void AudioOutputService()
		{
			this.audioOutput.Service();
		}

		public bool StartPlayback()
		{
			return this.StartPlaying();
		}

		public bool StopPlayback()
		{
			if (this.playbackExplicitlyStopped)
			{
				if (base.Logger.IsWarningEnabled)
				{
					base.Logger.LogWarning("Cannot stop playback because it was already been explicitly stopped.", Array.Empty<object>());
				}
				return false;
			}
			this.playbackExplicitlyStopped = this.StopPlaying(false);
			return this.playbackExplicitlyStopped;
		}

		public bool RestartPlayback(bool reinit = false)
		{
			if (!this.StopPlayback())
			{
				return false;
			}
			if (reinit)
			{
				this.audioOutput = null;
				this.Initialize();
			}
			return this.StartPlayback();
		}

		public bool SetPlaybackDelaySettings(PlaybackDelaySettings pdc)
		{
			return this.SetPlaybackDelaySettings(pdc.MinDelaySoft, pdc.MaxDelaySoft, pdc.MaxDelayHard);
		}

		public bool SetPlaybackDelaySettings(int low, int high, int max)
		{
			if (low >= 0 && low < high)
			{
				if (this.playbackDelaySettings.MaxDelaySoft != high || this.playbackDelaySettings.MinDelaySoft != low || this.playbackDelaySettings.MaxDelayHard != max)
				{
					if (max < high)
					{
						max = high;
					}
					this.playbackDelaySettings.MaxDelaySoft = high;
					this.playbackDelaySettings.MinDelaySoft = low;
					this.playbackDelaySettings.MaxDelayHard = max;
					if (this.IsPlaying)
					{
						this.RestartPlayback(true);
					}
					else if (this.IsInitialized)
					{
						this.audioOutput = null;
						this.Initialize();
					}
					return true;
				}
			}
			else if (base.Logger.IsErrorEnabled)
			{
				base.Logger.LogError("Wrong playback delay config values, make sure 0 <= Low < High, low={0}, high={1}, max={2}", new object[]
				{
					low,
					high,
					max
				});
			}
			return false;
		}

		private IAudioOut<float> audioOutput;

		private RemoteVoiceLink remoteVoiceLink;

		[SerializeField]
		private bool playbackOnlyWhenEnabled;

		[SerializeField]
		[HideInInspector]
		private int playDelayMs = 200;

		[SerializeField]
		protected PlaybackDelaySettings playbackDelaySettings = new PlaybackDelaySettings
		{
			MinDelaySoft = 200,
			MaxDelaySoft = 400,
			MaxDelayHard = 1000
		};

		private bool playbackExplicitlyStopped;

		public Func<IAudioOut<float>> CustomAudioOutFactory;
	}
}
