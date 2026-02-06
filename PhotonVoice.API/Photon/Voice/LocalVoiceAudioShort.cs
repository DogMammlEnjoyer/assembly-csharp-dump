using System;

namespace Photon.Voice
{
	public class LocalVoiceAudioShort : LocalVoiceAudio<short>
	{
		internal LocalVoiceAudioShort(VoiceClient voiceClient, IEncoder encoder, byte id, VoiceInfo voiceInfo, IAudioDesc audioSourceDesc, int channelId) : base(voiceClient, encoder, id, voiceInfo, audioSourceDesc, channelId)
		{
			this.levelMeter = new AudioUtil.LevelMeterShort(this.info.SamplingRate, this.info.Channels);
			this.voiceDetector = new AudioUtil.VoiceDetectorShort(this.info.SamplingRate, this.info.Channels);
			base.initBuiltinProcessors();
		}
	}
}
