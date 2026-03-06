using System;
using System.Collections;
using System.Collections.Generic;
using Lib.Wit.Runtime.Utilities.Logging;
using Meta.Voice;
using Meta.Voice.Logging;
using UnityEngine;
using UnityEngine.Serialization;

namespace Meta.WitAi.Lib
{
	[LogCategory(LogCategory.Audio, LogCategory.Input)]
	public class Mic : BaseAudioClipInput, ILogSource
	{
		public IVLogger Logger { get; } = LoggerRegistry.Instance.GetLogger(LogCategory.Input, null);

		public override AudioClip Clip
		{
			get
			{
				return this._audioClip;
			}
		}

		public override int ClipPosition
		{
			get
			{
				return this.MicrophoneGetPosition(this.CurrentDeviceName);
			}
		}

		public override bool CanActivateAudio
		{
			get
			{
				return true;
			}
		}

		public override bool ActivateOnEnable
		{
			get
			{
				return this._activateOnEnable;
			}
		}

		public override int AudioSampleRate
		{
			get
			{
				return this._micSampleRate;
			}
		}

		public void SetAudioSampleRate(int newSampleRate)
		{
			if (base.ActivationState == VoiceAudioInputState.On)
			{
				VLog.E(base.GetType().Name, string.Format("Cannot set audio sample rate while Mic is {0}", base.ActivationState), null);
				return;
			}
			this._micSampleRate = newSampleRate;
		}

		protected override IEnumerator HandleActivation()
		{
			DateTime utcNow = DateTime.UtcNow;
			DateTime start = utcNow;
			DateTime lastRefresh = DateTime.MinValue;
			while (string.IsNullOrEmpty(this.CurrentDeviceName) && (utcNow - start).TotalSeconds < (double)this.MicStartTimeout)
			{
				if ((utcNow - lastRefresh).TotalSeconds > 0.5)
				{
					lastRefresh = utcNow;
					this.RefreshMicDevices();
					if (this._devices.Count > 0 && this.CurrentDeviceIndex < 0)
					{
						this.CurrentDeviceIndex = 0;
					}
				}
				if (string.IsNullOrEmpty(this.CurrentDeviceName))
				{
					yield return null;
					utcNow = DateTime.UtcNow;
				}
			}
			if (string.IsNullOrEmpty(this.CurrentDeviceName))
			{
				VLog.W(base.GetType().Name, string.Format("No mics found after {0} seconds", this.MicStartTimeout), null);
				base.SetActivationState(VoiceAudioInputState.Off);
				yield break;
			}
			this.StartMicrophone();
			if (this._audioClip == null)
			{
				base.SetActivationState(VoiceAudioInputState.Off);
			}
			yield break;
		}

		private void StartMicrophone()
		{
			string currentDeviceName = this.CurrentDeviceName;
			if (string.IsNullOrEmpty(currentDeviceName))
			{
				return;
			}
			this.MicBufferLength = Mathf.Max(1, this.MicBufferLength);
			this.Logger.Info("Start Microphone '{0}'", currentDeviceName, null, null, null, "StartMicrophone", ".\\Library\\PackageCache\\com.meta.xr.sdk.voice@d3f6f37b8e1c\\Lib\\Wit.ai\\Scripts\\Runtime\\Lib\\Mic\\Other\\Mic.cs", 184);
			this._audioClip = this.MicrophoneStart(currentDeviceName, true, this.MicBufferLength, this.AudioSampleRate);
			if (this._audioClip == null)
			{
				VLog.W(base.GetType().Name, "Microphone.Start() did not return an AudioClip\nMic Name: " + currentDeviceName, null);
			}
		}

		protected override void HandleDeactivation()
		{
			this.StopMicrophone();
		}

		private void StopMicrophone()
		{
			string currentDeviceName = this.CurrentDeviceName;
			if (string.IsNullOrEmpty(currentDeviceName))
			{
				return;
			}
			if (this.MicrophoneIsRecording(currentDeviceName))
			{
				this.Logger.Info("Stop Microphone '{0}'", currentDeviceName, null, null, null, "StopMicrophone", ".\\Library\\PackageCache\\com.meta.xr.sdk.voice@d3f6f37b8e1c\\Lib\\Wit.ai\\Scripts\\Runtime\\Lib\\Mic\\Other\\Mic.cs", 217);
				this.MicrophoneEnd(currentDeviceName);
			}
			if (this._audioClip != null)
			{
				Object.DestroyImmediate(this._audioClip);
				this._audioClip = null;
			}
		}

		public List<string> Devices
		{
			get
			{
				if (this._devices == null || this._devices.Count == 0)
				{
					this.RefreshMicDevices();
				}
				return this._devices;
			}
		}

		public int CurrentDeviceIndex { get; private set; } = -1;

		public string CurrentDeviceName
		{
			get
			{
				if (this._devices == null || this.CurrentDeviceIndex < 0 || this.CurrentDeviceIndex >= this._devices.Count)
				{
					return string.Empty;
				}
				return this._devices[this.CurrentDeviceIndex];
			}
		}

		private void RefreshMicDevices()
		{
			string currentDeviceName = this.CurrentDeviceName;
			this._devices.Clear();
			string[] array = this.MicrophoneGetDevices();
			if (array != null)
			{
				this._devices.AddRange(array);
			}
			this.CurrentDeviceIndex = this._devices.IndexOf(currentDeviceName);
		}

		public void ChangeMicDevice(int index)
		{
			this.StopMicrophone();
			this.CurrentDeviceIndex = index;
			this.StartMicrophone();
		}

		private AudioClip MicrophoneStart(string deviceName, bool loop, int lengthSeconds, int frequency)
		{
			return Microphone.Start(deviceName, loop, lengthSeconds, frequency);
		}

		private void MicrophoneEnd(string deviceName)
		{
			Microphone.End(deviceName);
		}

		private bool MicrophoneIsRecording(string device)
		{
			return !string.IsNullOrEmpty(device) && Microphone.IsRecording(device);
		}

		private string[] MicrophoneGetDevices()
		{
			return Microphone.devices;
		}

		private int MicrophoneGetPosition(string device)
		{
			return Microphone.GetPosition(device);
		}

		private AudioClip _audioClip;

		[SerializeField]
		private bool _activateOnEnable = true;

		[SerializeField]
		[Tooltip("Searches for mics for this long following an activation request.")]
		public float MicStartTimeout = 5f;

		private const float MIC_CHECK = 0.5f;

		[SerializeField]
		[Tooltip("Total amount of seconds included within the mic audio clip buffer")]
		public int MicBufferLength = 2;

		[SerializeField]
		[Tooltip("Sample rate for mic audio capture in samples per second.")]
		[FormerlySerializedAs("_audioClipSampleRate")]
		private int _micSampleRate = 16000;

		private List<string> _devices = new List<string>();
	}
}
