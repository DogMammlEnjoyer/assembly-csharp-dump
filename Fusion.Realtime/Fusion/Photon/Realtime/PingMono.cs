using System;
using System.Diagnostics;
using System.Net.Sockets;

namespace Fusion.Photon.Realtime
{
	internal class PingMono : PhotonPing
	{
		public override bool StartPing(string ip)
		{
			base.Init();
			try
			{
				bool flag = this.sock == null;
				if (flag)
				{
					bool flag2 = ip.Contains(".");
					if (flag2)
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
			catch (Exception ex)
			{
				this.sock = null;
				Debug.WriteLine(ex.ToString());
				throw;
			}
			return false;
		}

		public override bool Done()
		{
			bool flag = this.GotResult || this.sock == null;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				int num = 0;
				try
				{
					bool flag2 = !this.sock.Poll(0, SelectMode.SelectRead);
					if (flag2)
					{
						return false;
					}
					num = this.sock.Receive(this.PingBytes, SocketFlags.None);
				}
				catch (Exception ex)
				{
					bool flag3 = this.sock != null;
					if (flag3)
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
				bool flag4 = this.PingBytes[this.PingBytes.Length - 1] == this.PingId && num == this.PingLength;
				bool flag5 = !flag4;
				if (flag5)
				{
					this.DebugString += " ReplyMatch is false! ";
				}
				this.Successful = flag4;
				this.GotResult = true;
				result = true;
			}
			return result;
		}

		public override void Dispose()
		{
			bool flag = this.sock == null;
			if (!flag)
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
		}

		private Socket sock;
	}
}
