using System;

namespace Fusion.Photon.Realtime
{
	internal class Region
	{
		public string Code { get; private set; }

		public string Cluster { get; private set; }

		public string HostAndPort { get; protected internal set; }

		public int Ping { get; set; }

		public bool WasPinged
		{
			get
			{
				return this.Ping != int.MaxValue;
			}
		}

		public Region(string code, string address)
		{
			this.SetCodeAndCluster(code);
			this.HostAndPort = address;
			this.Ping = int.MaxValue;
		}

		public Region(string code, int ping)
		{
			this.SetCodeAndCluster(code);
			this.Ping = ping;
		}

		private void SetCodeAndCluster(string codeAsString)
		{
			bool flag = codeAsString == null;
			if (flag)
			{
				this.Code = "";
				this.Cluster = "";
			}
			else
			{
				codeAsString = codeAsString.ToLower();
				int num = codeAsString.IndexOf('/');
				this.Code = ((num <= 0) ? codeAsString : codeAsString.Substring(0, num));
				this.Cluster = ((num <= 0) ? "" : codeAsString.Substring(num + 1, codeAsString.Length - num - 1));
			}
		}

		public override string ToString()
		{
			return this.ToString(false);
		}

		public string ToString(bool compact = false)
		{
			string text = this.Code;
			bool flag = !string.IsNullOrEmpty(this.Cluster);
			if (flag)
			{
				text = text + "/" + this.Cluster;
			}
			string result;
			if (compact)
			{
				result = string.Format("{0}:{1}", text, this.Ping);
			}
			else
			{
				result = string.Format("{0}[{2}]: {1}ms", text, this.Ping, this.HostAndPort);
			}
			return result;
		}
	}
}
