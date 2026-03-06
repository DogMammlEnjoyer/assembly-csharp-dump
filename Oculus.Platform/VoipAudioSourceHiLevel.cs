using System;
using UnityEngine;

namespace Oculus.Platform
{
	public class VoipAudioSourceHiLevel : MonoBehaviour
	{
		public ulong senderID
		{
			set
			{
				this.pcmSource.SetSenderID(value);
			}
		}

		protected void Stop()
		{
		}

		private VoipSampleRate SampleRateToEnum(int rate)
		{
			if (rate == 24000)
			{
				return VoipSampleRate.HZ24000;
			}
			if (rate == 44100)
			{
				return VoipSampleRate.HZ44100;
			}
			if (rate == 48000)
			{
				return VoipSampleRate.HZ48000;
			}
			return VoipSampleRate.Unknown;
		}

		protected void Awake()
		{
			this.CreatePCMSource();
			if (this.audioSource == null)
			{
				this.audioSource = base.gameObject.AddComponent<AudioSource>();
			}
			this.audioSource.gameObject.AddComponent<VoipAudioSourceHiLevel.FilterReadDelegate>();
			this.audioSource.gameObject.GetComponent<VoipAudioSourceHiLevel.FilterReadDelegate>().parent = this;
			this.initialPlaybackDelayMS = 40;
			VoipAudioSourceHiLevel.audioSystemPlaybackFrequency = AudioSettings.outputSampleRate;
			CAPI.ovr_Voip_SetOutputSampleRate(this.SampleRateToEnum(VoipAudioSourceHiLevel.audioSystemPlaybackFrequency));
			if (VoipAudioSourceHiLevel.verboseLogging)
			{
				Debug.LogFormat("freq {0}", new object[]
				{
					VoipAudioSourceHiLevel.audioSystemPlaybackFrequency
				});
			}
		}

		private void Start()
		{
			this.audioSource.Stop();
		}

		protected virtual void CreatePCMSource()
		{
			this.pcmSource = new VoipPCMSourceNative();
		}

		protected static int MSToElements(int ms)
		{
			return ms * VoipAudioSourceHiLevel.audioSystemPlaybackFrequency / 1000;
		}

		private void Update()
		{
			this.pcmSource.Update();
			if (!this.audioSource.isPlaying && this.pcmSource.PeekSizeElements() >= VoipAudioSourceHiLevel.MSToElements(this.initialPlaybackDelayMS))
			{
				if (VoipAudioSourceHiLevel.verboseLogging)
				{
					Debug.LogFormat("buffered {0} elements, starting playback", new object[]
					{
						this.pcmSource.PeekSizeElements()
					});
				}
				this.audioSource.Play();
			}
		}

		private int initialPlaybackDelayMS;

		public AudioSource audioSource;

		public float peakAmplitude;

		protected IVoipPCMSource pcmSource;

		private static int audioSystemPlaybackFrequency;

		private static bool verboseLogging;

		public class FilterReadDelegate : MonoBehaviour
		{
			private void Awake()
			{
				int num = (int)((uint)CAPI.ovr_Voip_GetOutputBufferMaxSize());
				this.scratchBuffer = new float[num];
			}

			private void OnAudioFilterRead(float[] data, int channels)
			{
				int num = data.Length / channels;
				int num2 = num;
				if (num2 > this.scratchBuffer.Length)
				{
					Array.Clear(data, 0, data.Length);
					throw new Exception(string.Format("Audio system tried to pull {0} bytes, max voip internal ring buffer size {1}", num, this.scratchBuffer.Length));
				}
				int num3 = this.parent.pcmSource.PeekSizeElements();
				if (num3 < num2)
				{
					if (VoipAudioSourceHiLevel.verboseLogging)
					{
						Debug.LogFormat("Voip starved! Want {0}, but only have {1} available", new object[]
						{
							num2,
							num3
						});
					}
					return;
				}
				int pcm = this.parent.pcmSource.GetPCM(this.scratchBuffer, num2);
				if (pcm < num2)
				{
					Debug.LogWarningFormat("GetPCM() returned {0} samples, expected {1}", new object[]
					{
						pcm,
						num2
					});
					return;
				}
				int num4 = 0;
				float num5 = -1f;
				for (int i = 0; i < num; i++)
				{
					float num6 = this.scratchBuffer[i];
					for (int j = 0; j < channels; j++)
					{
						data[num4++] = num6;
						if (num6 > num5)
						{
							num5 = num6;
						}
					}
				}
				this.parent.peakAmplitude = num5;
			}

			public VoipAudioSourceHiLevel parent;

			private float[] scratchBuffer;
		}
	}
}
