using System;
using System.Net.Sockets;

namespace Photon.Realtime
{
	public class PingMono : PhotonPing
	{
		public override bool StartPing(string ip)
		{
			base.Init();
			try
			{
				if (this.sock == null)
				{
					if (ip.Contains("."))
					{
						this.sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
					}
					else
					{
						this.sock = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp);
					}
					this.sock.ReceiveTimeout = 5000;
					int port = (int)((RegionHandler.PortToPingOverride != 0) ? RegionHandler.PortToPingOverride : 5055);
					this.sock.Connect(ip, port);
				}
				this.PingBytes[this.PingBytes.Length - 1] = this.PingId;
				this.sock.Send(this.PingBytes);
				this.PingBytes[this.PingBytes.Length - 1] = this.PingId + 1;
			}
			catch (Exception value)
			{
				this.sock = null;
				Console.WriteLine(value);
			}
			return false;
		}

		public override bool Done()
		{
			if (this.GotResult || this.sock == null)
			{
				return true;
			}
			int num = 0;
			try
			{
				if (!this.sock.Poll(0, SelectMode.SelectRead))
				{
					return false;
				}
				num = this.sock.Receive(this.PingBytes, SocketFlags.None);
			}
			catch (Exception ex)
			{
				if (this.sock != null)
				{
					this.sock.Close();
					this.sock = null;
				}
				string debugString = this.DebugString;
				string str = " Exception of socket! ";
				Type type = ex.GetType();
				this.DebugString = debugString + str + ((type != null) ? type.ToString() : null) + " ";
				return true;
			}
			bool flag = this.PingBytes[this.PingBytes.Length - 1] == this.PingId && num == this.PingLength;
			if (!flag)
			{
				this.DebugString += " ReplyMatch is false! ";
			}
			this.Successful = flag;
			this.GotResult = true;
			return true;
		}

		public override void Dispose()
		{
			try
			{
				this.sock.Close();
			}
			catch
			{
			}
			this.sock = null;
		}

		private Socket sock;
	}
}
