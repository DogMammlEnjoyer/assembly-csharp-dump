using System;
using System.Runtime.InteropServices;
using Liv.Lck.Collections;
using Liv.Lck.Utilities;
using UnityEngine;

namespace Liv.Lck
{
	internal class LckAudioCaptureFMODAndUnity : MonoBehaviour, ILckAudioSource
	{
		public bool IsCapturing()
		{
			return this._isCapturing;
		}

		private static void TryAppendToBuffer(float[] srcDataBuffer, int srcStartIdx, int srcDataLength, AudioBuffer destBuffer)
		{
			if (!destBuffer.TryExtendFrom(srcDataBuffer, srcStartIdx, srcDataLength))
			{
				LckLog.LogWarning("LCK Audio Capture (FMOD + Unity) losing data. Expecting this to be a lag spike.", "TryAppendToBuffer", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\Components\\LckAudioCaptureFMODAndUnity.cs", 44);
			}
		}

		private static void TryAppendToBuffer(AudioBuffer srcBuffer, AudioBuffer destBuffer)
		{
			if (!destBuffer.TryExtendFrom(srcBuffer))
			{
				LckLog.LogWarning("LCK Audio Capture (FMOD + Unity) losing data. Expecting this to be a lag spike.", "TryAppendToBuffer", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\Components\\LckAudioCaptureFMODAndUnity.cs", 52);
			}
		}

		private static void AppendToBufferAsStereo(float[] sourceAudioBuffer, int sourceAudioStartIdx, int sourceAudioLength, int sourceChannels, AudioBuffer destBuffer, AudioBuffer remixBuffer)
		{
			if (sourceChannels == 1)
			{
				LckLog.Log("LCK Audio Capture (FMOD + Unity): Got mono input. Remixing to stereo.", "AppendToBufferAsStereo", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\Components\\LckAudioCaptureFMODAndUnity.cs", 66);
				ChannelMixingUtils.ConvertMonoToStereo(sourceAudioBuffer, sourceAudioStartIdx, sourceAudioLength, remixBuffer);
				LckAudioCaptureFMODAndUnity.TryAppendToBuffer(remixBuffer, destBuffer);
				return;
			}
			if (sourceChannels == 2)
			{
				LckLog.LogWarning("LCK Audio Capture (FMOD + Unity): Got stereo input. No remixing necessary.", "AppendToBufferAsStereo", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\Components\\LckAudioCaptureFMODAndUnity.cs", 62);
				LckAudioCaptureFMODAndUnity.TryAppendToBuffer(sourceAudioBuffer, sourceAudioStartIdx, sourceAudioLength, destBuffer);
				return;
			}
			if (sourceChannels != 6)
			{
				LckLog.LogError("LCK Audio Capture (FMOD + Unity): LCK only supports Mono, Stereo or 5.1 input at this time. " + string.Format("Got: {0} channels", sourceChannels), "AppendToBufferAsStereo", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\Components\\LckAudioCaptureFMODAndUnity.cs", 78);
				return;
			}
			LckLog.Log("LCK Audio Capture (FMOD + Unity): Got 5.1 input. Remixing to stereo.", "AppendToBufferAsStereo", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\Components\\LckAudioCaptureFMODAndUnity.cs", 71);
			ChannelMixingUtils.ConvertFiveOneToStereo(sourceAudioBuffer, sourceAudioStartIdx, sourceAudioLength, remixBuffer);
			LckAudioCaptureFMODAndUnity.TryAppendToBuffer(remixBuffer, destBuffer);
		}

		protected virtual void OnAudioFilterRead(float[] data, int channels)
		{
			if (!this._isCapturing)
			{
				return;
			}
			object audioThreadLock = this._audioThreadLock;
			lock (audioThreadLock)
			{
				LckAudioCaptureFMODAndUnity.AppendToBufferAsStereo(data, 0, data.Length, channels, this._unityBuffer, this._tmpRemixBuffer);
			}
		}

		private void Start()
		{
			this._unitySampleRate = AudioSettings.outputSampleRate;
			if (this._unitySampleRate != this._fmodSampleRate)
			{
				LckLog.LogError(string.Format("LCK Audio Capture (FMOD + Unity): Unity sample rate ({0}) and FMOD ", this._unitySampleRate) + string.Format("sample rate ({0}) do not match - this is not currently supported, so ", this._fmodSampleRate) + "audio pitch may be incorrect in captures", "Start", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\Components\\LckAudioCaptureFMODAndUnity.cs", 192);
			}
		}

		private void OnDestroy()
		{
		}

		public void GetAudioData(ILckAudioSource.AudioDataCallbackDelegate callback)
		{
			object audioThreadLock = this._audioThreadLock;
			lock (audioThreadLock)
			{
				this._mixBuffer.Clear();
				int count = this._fmodBuffer.Count;
				int count2 = this._unityBuffer.Count;
				int num = Math.Min(count, count2);
				if (num > 0)
				{
					for (int i = 0; i < num; i++)
					{
						float value = this._fmodBuffer[i] + this._unityBuffer[i];
						this._mixBuffer.TryAdd(value);
					}
				}
				callback(this._mixBuffer);
				if (num > 0)
				{
					this._fmodBuffer.SkipAudioSamples(num);
					this._unityBuffer.SkipAudioSamples(num);
				}
				this._mixBuffer.Clear();
			}
		}

		public void EnableCapture()
		{
			this._isCapturing = true;
			this._fmodBuffer.Clear();
			this._unityBuffer.Clear();
			this._mixBuffer.Clear();
		}

		public void DisableCapture()
		{
			this._isCapturing = false;
			this._fmodBuffer.Clear();
			this._unityBuffer.Clear();
			this._mixBuffer.Clear();
		}

		private GCHandle _mObjHandle;

		private readonly AudioBuffer _tmpRemixBuffer = new AudioBuffer(98000);

		private readonly AudioBuffer _tmpAudio = new AudioBuffer(98000);

		private readonly AudioBuffer _fmodBuffer = new AudioBuffer(98000);

		private readonly AudioBuffer _unityBuffer = new AudioBuffer(98000);

		private readonly AudioBuffer _mixBuffer = new AudioBuffer(98000);

		private int _fmodSampleRate;

		private int _unitySampleRate;

		private bool _isCapturing;

		private readonly object _audioThreadLock = new object();
	}
}
