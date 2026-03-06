using System;

namespace Fusion.Sockets
{
	public struct NetConfig
	{
		public int ConnectionsPerGroup
		{
			get
			{
				int num = this.MaxConnections / this.ConnectionGroups;
				bool flag = num * this.ConnectionGroups == this.MaxConnections;
				int result;
				if (flag)
				{
					result = num;
				}
				else
				{
					result = num + 1;
				}
				return result;
			}
		}

		public int PacketSizeInBits
		{
			get
			{
				return this.PacketSize * 8;
			}
		}

		public static NetConfig Defaults
		{
			get
			{
				NetConfig result;
				result.ConnectionDefaultRtt = 0.1;
				result.ConnectionSendBuffers = 4;
				result.ConnectionGroups = 12;
				result.MaxConnections = 1000;
				result.SocketSendBuffer = 262144;
				result.SocketRecvBuffer = 262144;
				result.PacketSize = 1280;
				result.ConnectAttempts = 10;
				result.ConnectInterval = 0.5;
				result.ConnectionTimeout = 10.0;
				result.ConnectionPingInterval = 1.0;
				result.ConnectionShutdownTime = 1.0;
				result.OperationExpireTime = 3.0;
				result.Notify = NetConfigNotify.Defaults;
				result.Simulation = NetConfigSimulation.Defaults;
				result.Address = default(NetAddress);
				return result;
			}
		}

		public int ConnectionSendBuffers;

		public int ConnectionGroups;

		public int MaxConnections;

		public int SocketSendBuffer;

		public int SocketRecvBuffer;

		public int PacketSize;

		public int ConnectAttempts;

		public double ConnectInterval;

		public double OperationExpireTime;

		public double ConnectionDefaultRtt;

		public double ConnectionTimeout;

		public double ConnectionPingInterval;

		public double ConnectionShutdownTime;

		public NetAddress Address;

		public NetConfigNotify Notify;

		public NetConfigSimulation Simulation;
	}
}
