using System;

namespace Fusion.Sockets.Stun
{
	public enum NATType : byte
	{
		Invalid,
		UdpBlocked,
		OpenInternet,
		FullCone = 4,
		Symmetric = 8
	}
}
