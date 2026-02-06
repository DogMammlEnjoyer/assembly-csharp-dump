using System;

namespace Photon.Voice
{
	public class RemoteVoiceInfo
	{
		internal RemoteVoiceInfo(int channelId, int playerId, byte voiceId, VoiceInfo info)
		{
			this.ChannelId = channelId;
			this.PlayerId = playerId;
			this.VoiceId = voiceId;
			this.Info = info;
		}

		public VoiceInfo Info { get; private set; }

		public int ChannelId { get; private set; }

		public int PlayerId { get; private set; }

		public byte VoiceId { get; private set; }
	}
}
