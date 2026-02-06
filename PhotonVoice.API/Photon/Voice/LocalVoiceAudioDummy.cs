using System;

namespace Photon.Voice
{
	public class LocalVoiceAudioDummy : LocalVoice, ILocalVoiceAudio
	{
		public AudioUtil.IVoiceDetector VoiceDetector
		{
			get
			{
				return this.voiceDetector;
			}
		}

		public AudioUtil.ILevelMeter LevelMeter
		{
			get
			{
				return this.levelMeter;
			}
		}

		public bool VoiceDetectorCalibrating
		{
			get
			{
				return false;
			}
		}

		public void VoiceDetectorCalibrate(int durationMs, Action<float> onCalibrated = null)
		{
		}

		public LocalVoiceAudioDummy()
		{
			this.voiceDetector = new AudioUtil.VoiceDetectorDummy();
			this.levelMeter = new AudioUtil.LevelMeterDummy();
		}

		private AudioUtil.VoiceDetectorDummy voiceDetector;

		private AudioUtil.LevelMeterDummy levelMeter;

		public static LocalVoiceAudioDummy Dummy = new LocalVoiceAudioDummy();
	}
}
