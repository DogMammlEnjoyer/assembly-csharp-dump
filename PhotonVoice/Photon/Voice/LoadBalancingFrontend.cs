using System;
using ExitGames.Client.Photon;

namespace Photon.Voice
{
	[Obsolete("Class renamed. Use LoadBalancingTransport instead.")]
	public class LoadBalancingFrontend : LoadBalancingTransport
	{
		public LoadBalancingFrontend() : base(null, ConnectionProtocol.Udp)
		{
		}
	}
}
