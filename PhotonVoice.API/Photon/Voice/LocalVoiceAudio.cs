using System;

namespace Photon.Voice
{
	public abstract class LocalVoiceAudio<T> : LocalVoiceFramed<T>, ILocalVoiceAudio
	{
		public static LocalVoiceAudio<T> Create(VoiceClient voiceClient, byte voiceId, IEncoder encoder, VoiceInfo voiceInfo, IAudioDesc audioSourceDesc, int channelId)
		{
			if (typeof(T) == typeof(float))
			{
				return new LocalVoiceAudioFloat(voiceClient, encoder, voiceId, voiceInfo, audioSourceDesc, channelId) as LocalVoiceAudio<T>;
			}
			if (typeof(T) == typeof(short))
			{
				return new LocalVoiceAudioShort(voiceClient, encoder, voiceId, voiceInfo, audioSourceDesc, channelId) as LocalVoiceAudio<T>;
			}
			throw new UnsupportedSampleTypeException(typeof(T));
		}

		public virtual AudioUtil.IVoiceDetector VoiceDetector
		{
			get
			{
				return this.voiceDetector;
			}
		}

		public virtual AudioUtil.ILevelMeter LevelMeter
		{
			get
			{
				return this.levelMeter;
			}
		}

		public void VoiceDetectorCalibrate(int durationMs, Action<float> onCalibrated = null)
		{
			this.voiceDetectorCalibration.Calibrate(durationMs, onCalibrated);
		}

		public bool VoiceDetectorCalibrating
		{
			get
			{
				return this.voiceDetectorCalibration.IsCalibrating;
			}
		}

		internal LocalVoiceAudio(VoiceClient voiceClient, IEncoder encoder, byte id, VoiceInfo voiceInfo, IAudioDesc audioSourceDesc, int channelId) : base(voiceClient, encoder, id, voiceInfo, channelId, (voiceInfo.SamplingRate != 0) ? (voiceInfo.FrameSize * audioSourceDesc.SamplingRate / voiceInfo.SamplingRate) : voiceInfo.FrameSize)
		{
			this.channels = voiceInfo.Channels;
			if (audioSourceDesc.SamplingRate != voiceInfo.SamplingRate)
			{
				this.resampleSource = true;
				this.voiceClient.logger.LogWarning(string.Concat(new string[]
				{
					"[PV] Local voice #",
					this.id.ToString(),
					" audio source frequency ",
					audioSourceDesc.SamplingRate.ToString(),
					" and encoder sampling rate ",
					voiceInfo.SamplingRate.ToString(),
					" do not match. Resampling will occur before encoding."
				}), Array.Empty<object>());
			}
		}

		protected void initBuiltinProcessors()
		{
			if (this.resampleSource)
			{
				base.AddPostProcessor(new IProcessor<T>[]
				{
					new AudioUtil.Resampler<T>(this.info.FrameSize, this.channels)
				});
			}
			this.voiceDetectorCalibration = new AudioUtil.VoiceDetectorCalibration<T>(this.voiceDetector, this.levelMeter, this.info.SamplingRate, this.channels);
			base.AddPostProcessor(new IProcessor<T>[]
			{
				this.levelMeter,
				this.voiceDetectorCalibration,
				this.voiceDetector
			});
		}

		protected AudioUtil.VoiceDetector<T> voiceDetector;

		protected AudioUtil.VoiceDetectorCalibration<T> voiceDetectorCalibration;

		protected AudioUtil.LevelMeter<T> levelMeter;

		protected int channels;

		protected bool resampleSource;
	}
}
