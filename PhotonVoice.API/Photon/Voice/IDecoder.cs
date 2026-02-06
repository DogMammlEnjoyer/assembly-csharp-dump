using System;

namespace Photon.Voice
{
	public interface IDecoder : IDisposable
	{
		void Open(VoiceInfo info);

		string Error { get; }

		void Input(ref FrameBuffer buf);
	}
}
