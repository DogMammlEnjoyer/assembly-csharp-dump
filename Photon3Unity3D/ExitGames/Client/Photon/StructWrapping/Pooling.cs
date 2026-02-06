using System;

namespace ExitGames.Client.Photon.StructWrapping
{
	public enum Pooling
	{
		Disconnected,
		Connected,
		ReleaseOnUnwrap,
		Readonly = 4,
		CheckedOut = 8
	}
}
