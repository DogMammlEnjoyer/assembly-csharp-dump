using System;

namespace Photon.Realtime
{
	public enum EncryptionMode
	{
		PayloadEncryption,
		DatagramEncryption = 10,
		DatagramEncryptionRandomSequence,
		DatagramEncryptionGCM = 13
	}
}
