using System;
using UnityEngine;

namespace Photon.Voice.Unity
{
	[RequireComponent(typeof(Recorder))]
	public class AudioChangesHandler : VoiceComponent
	{
		protected override void Awake()
		{
			base.Awake();
			this.recorder = base.GetComponent<Recorder>();
			this.recorder.ReactOnSystemChanges = false;
			this.audioConfiguration = AudioSettings.GetConfiguration();
			this.SubscribeToSystemChanges();
		}

		private void OnDestroy()
		{
			this.UnsubscribeFromSystemChanges();
		}

		private void OnDeviceChange()
		{
			if (!this.recorder.IsRecording)
			{
				if (this.StartWhenDeviceChange)
				{
					this.recorder.MicrophoneDeviceChangeDetected = true;
					if (base.Logger.IsInfoEnabled)
					{
						base.Logger.LogInfo("An attempt to auto start recording should follow shortly.", Array.Empty<object>());
						return;
					}
				}
				else if (base.Logger.IsInfoEnabled)
				{
					base.Logger.LogInfo("Device change detected but will not try to start recording as StartWhenDeviceChange is false.", Array.Empty<object>());
					return;
				}
			}
			else
			{
				if (this.HandleDeviceChange)
				{
					this.recorder.MicrophoneDeviceChangeDetected = true;
					return;
				}
				if (base.Logger.IsInfoEnabled)
				{
					base.Logger.LogInfo("Device change detected but will not try to handle this as HandleDeviceChange is false.", Array.Empty<object>());
				}
			}
		}

		private void SubscribeToSystemChanges()
		{
			if (base.Logger.IsDebugEnabled)
			{
				base.Logger.LogDebug("Subscribing to system (audio) changes.", Array.Empty<object>());
			}
			if (base.Logger.IsInfoEnabled)
			{
				base.Logger.LogInfo("Skipped subscribing to audio change notifications via Photon's AudioInChangeNotifier as not supported on current platform: {0}", new object[]
				{
					VoiceComponent.CurrentPlatform
				});
			}
			if (this.subscribedToSystemChangesPhoton && base.Logger.IsErrorEnabled)
			{
				base.Logger.LogError("Unexpected: subscribedToSystemChangesPhoton is set to true while platform is not supported!.", Array.Empty<object>());
			}
			if (this.subscribedToSystemChangesUnity)
			{
				if (base.Logger.IsWarningEnabled)
				{
					base.Logger.LogWarning("Already subscribed to audio changes via Unity OnAudioConfigurationChanged callback.", Array.Empty<object>());
					return;
				}
			}
			else
			{
				AudioSettings.OnAudioConfigurationChanged += this.OnAudioConfigChanged;
				this.subscribedToSystemChangesUnity = true;
				if (base.Logger.IsInfoEnabled)
				{
					base.Logger.LogInfo("Subscribed to audio configuration changes via Unity OnAudioConfigurationChanged callback.", Array.Empty<object>());
				}
			}
		}

		private void OnAudioConfigChanged(bool deviceWasChanged)
		{
			if (base.Logger.IsInfoEnabled)
			{
				base.Logger.LogInfo("OnAudioConfigurationChanged: {0}", new object[]
				{
					deviceWasChanged ? "Device was changed." : "AudioSettings.Reset was called."
				});
			}
			AudioConfiguration configuration = AudioSettings.GetConfiguration();
			bool flag = false;
			if (configuration.dspBufferSize != this.audioConfiguration.dspBufferSize)
			{
				if (base.Logger.IsInfoEnabled)
				{
					base.Logger.LogInfo("OnAudioConfigurationChanged: dspBufferSize old={0} new={1}", new object[]
					{
						this.audioConfiguration.dspBufferSize,
						configuration.dspBufferSize
					});
				}
				flag = true;
			}
			if (configuration.numRealVoices != this.audioConfiguration.numRealVoices)
			{
				if (base.Logger.IsInfoEnabled)
				{
					base.Logger.LogInfo("OnAudioConfigurationChanged: numRealVoices old={0} new={1}", new object[]
					{
						this.audioConfiguration.numRealVoices,
						configuration.numRealVoices
					});
				}
				flag = true;
			}
			if (configuration.numVirtualVoices != this.audioConfiguration.numVirtualVoices)
			{
				if (base.Logger.IsInfoEnabled)
				{
					base.Logger.LogInfo("OnAudioConfigurationChanged: numVirtualVoices old={0} new={1}", new object[]
					{
						this.audioConfiguration.numVirtualVoices,
						configuration.numVirtualVoices
					});
				}
				flag = true;
			}
			if (configuration.sampleRate != this.audioConfiguration.sampleRate)
			{
				if (base.Logger.IsInfoEnabled)
				{
					base.Logger.LogInfo("OnAudioConfigurationChanged: sampleRate old={0} new={1}", new object[]
					{
						this.audioConfiguration.sampleRate,
						configuration.sampleRate
					});
				}
				flag = true;
			}
			if (configuration.speakerMode != this.audioConfiguration.speakerMode)
			{
				if (base.Logger.IsInfoEnabled)
				{
					base.Logger.LogInfo("OnAudioConfigurationChanged: speakerMode old={0} new={1}", new object[]
					{
						this.audioConfiguration.speakerMode,
						configuration.speakerMode
					});
				}
				flag = true;
			}
			if (flag)
			{
				this.audioConfiguration = configuration;
			}
			if (!this.recorder.MicrophoneDeviceChangeDetected)
			{
				if (flag)
				{
					if (this.recorder.IsRecording)
					{
						if (this.HandleConfigChange)
						{
							if (base.Logger.IsInfoEnabled)
							{
								base.Logger.LogInfo("Config change detected; an attempt to auto start recording should follow shortly.", Array.Empty<object>());
							}
							this.recorder.MicrophoneDeviceChangeDetected = true;
							return;
						}
						if (base.Logger.IsInfoEnabled)
						{
							base.Logger.LogInfo("Config change detected but will not try to handle this as HandleConfigChange is false.", Array.Empty<object>());
							return;
						}
					}
					else if (base.Logger.IsInfoEnabled)
					{
						base.Logger.LogInfo("Config change detected but ignored as recording not started.", Array.Empty<object>());
						return;
					}
				}
				else if (deviceWasChanged)
				{
					if (this.UseOnAudioConfigurationChanged)
					{
						this.OnDeviceChange();
						return;
					}
					if (base.Logger.IsInfoEnabled)
					{
						base.Logger.LogInfo("Device change detected but will not try to handle this as UseOnAudioConfigurationChanged is false.", Array.Empty<object>());
					}
				}
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
					base.Logger.LogInfo("Unsubscribed from audio changes via Unity OnAudioConfigurationChanged callback.", Array.Empty<object>());
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

		private IAudioInChangeNotifier photonMicChangeNotifier;

		private AudioConfiguration audioConfiguration;

		private Recorder recorder;

		[Tooltip("Try to start recording when we get devices change notification and recording is not started.")]
		public bool StartWhenDeviceChange = true;

		[Tooltip("Try to react to device change notification when Recorder is started.")]
		public bool HandleDeviceChange = true;

		[Tooltip("Try to react to audio config change notification when Recorder is started.")]
		public bool HandleConfigChange = true;

		[Tooltip("Whether or not to make use of Photon's AudioInChangeNotifier native plugin.")]
		public bool UseNativePluginChangeNotifier = true;

		[Tooltip("Whether or not to make use of Unity's OnAudioConfigurationChanged.")]
		public bool UseOnAudioConfigurationChanged = true;

		private bool subscribedToSystemChangesPhoton;

		private bool subscribedToSystemChangesUnity;
	}
}
