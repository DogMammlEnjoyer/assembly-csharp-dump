using System;

namespace Photon.Voice
{
	public class LocalVoiceAudioFloat : LocalVoiceAudio<float>
	{
		internal LocalVoiceAudioFloat(VoiceClient voiceClient, IEncoder encoder, byte id, VoiceInfo voiceInfo, IAudioDesc audioSourceDesc, int channelId) : base(voiceClient, encoder, id, voiceInfo, audioSourceDesc, channelId)
		{
			this.levelMeter = new AudioUtil.LevelMeterFloat(this.info.SamplingRate, this.info.Channels);
			this.voiceDetector = new AudioUtil.VoiceDetectorFloat(this.info.SamplingRate, this.info.Channels);
			base.initBuiltinProcessors();
		}
	}
}
