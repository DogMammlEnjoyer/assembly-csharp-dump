using System;
using Fusion.Sockets;

namespace Fusion
{
	[Serializable]
	public class NetworkConfiguration
	{
		public int SocketSendBufferSize
		{
			get
			{
				return 256;
			}
		}

		public int SocketRecvBufferSize
		{
			get
			{
				return 256;
			}
		}

		public int ConnectAttempts
		{
			get
			{
				return 10;
			}
		}

		public double ConnectInterval
		{
			get
			{
				return 0.5;
			}
		}

		public double ConnectionDefaultRtt
		{
			get
			{
				return 0.1;
			}
		}

		public double ConnectionPingInterval
		{
			get
			{
				return 1.0;
			}
		}

		public NetworkConfiguration Init()
		{
			return (NetworkConfiguration)base.MemberwiseClone();
		}

		internal NetConfig ToNetConfig(NetAddress address)
		{
			NetConfig defaults = NetConfig.Defaults;
			defaults.SocketSendBuffer = this.SocketSendBufferSize * 1024;
			defaults.SocketRecvBuffer = this.SocketRecvBufferSize * 1024;
			defaults.ConnectAttempts = this.ConnectAttempts;
			defaults.ConnectInterval = this.ConnectInterval;
			defaults.ConnectionDefaultRtt = this.ConnectionDefaultRtt;
			defaults.ConnectionTimeout = this.ConnectionTimeout;
			defaults.ConnectionPingInterval = this.ConnectionPingInterval;
			defaults.ConnectionShutdownTime = this.ConnectionShutdownTime;
			defaults.Address = address;
			return defaults;
		}

		[InlineHelp]
		[Unit(Units.Seconds)]
		public double ConnectionTimeout = 10.0;

		[InlineHelp]
		[Unit(Units.Seconds)]
		public double ConnectionShutdownTime = 1.0;

		[InlineHelp]
		public NetworkConfiguration.ReliableDataTransfers ReliableDataTransferModes = NetworkConfiguration.ReliableDataTransfers.ClientToServer | NetworkConfiguration.ReliableDataTransfers.ClientToClientWithServerProxy;

		[Flags]
		public enum ReliableDataTransfers
		{
			ClientToServer = 1,
			ClientToClientWithServerProxy = 2
		}
	}
}
