using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using ExitGames.Client.Photon;

namespace Fusion.Photon.Realtime
{
	internal class RegionPinger
	{
		public bool Done { get; private set; }

		public bool Aborted { get; internal set; }

		public RegionPinger(Region region, Action<Region> onDoneCallback)
		{
			this.region = region;
			this.region.Ping = RegionPinger.PingWhenFailed;
			this.Done = false;
			this.onDoneCall = onDoneCallback;
		}

		private PhotonPing GetPingImplementation()
		{
			PhotonPing photonPing = null;
			bool isUNITY_WEBGL = RuntimeUnityFlagsSetup.IsUNITY_WEBGL;
			if (isUNITY_WEBGL)
			{
				bool flag = RegionHandler.PingImplementation == null || RegionHandler.PingImplementation == typeof(PingHttp);
				if (flag)
				{
					photonPing = new PingHttp();
				}
			}
			else
			{
				bool flag2 = RegionHandler.PingImplementation == null || RegionHandler.PingImplementation == typeof(PingMono);
				if (flag2)
				{
					photonPing = new PingMono();
				}
			}
			bool flag3 = photonPing == null;
			if (flag3)
			{
				bool flag4 = RegionHandler.PingImplementation != null;
				if (flag4)
				{
					photonPing = (PhotonPing)Activator.CreateInstance(RegionHandler.PingImplementation);
				}
			}
			return photonPing;
		}

		public bool Start()
		{
			this.ping = this.GetPingImplementation();
			this.Done = false;
			this.CurrentAttempt = 0;
			this.rttResults = new List<int>(RegionPinger.Attempts);
			bool aborted = this.Aborted;
			bool result;
			if (aborted)
			{
				result = false;
			}
			else
			{
				bool isUNITY_WEBGL = RuntimeUnityFlagsSetup.IsUNITY_WEBGL;
				if (isUNITY_WEBGL)
				{
					MonoBehaviourEmpty.BuildInstance("RegionPing_" + this.region.Code).StartCoroutineAndDestroy(this.RegionPingCoroutine());
				}
				else
				{
					bool flag = false;
					try
					{
						flag = ThreadPool.QueueUserWorkItem(delegate(object o)
						{
							this.RegionPingThreaded();
						});
					}
					catch
					{
						flag = false;
					}
					bool flag2 = !flag;
					if (flag2)
					{
						SupportClass.StartBackgroundCalls(new Func<bool>(this.RegionPingThreaded), 0, "RegionPing_" + this.region.Code + "_" + this.region.Cluster);
					}
				}
				result = true;
			}
			return result;
		}

		protected internal void Abort()
		{
			this.Aborted = true;
			bool flag = this.ping != null;
			if (flag)
			{
				this.ping.Dispose();
			}
		}

		protected internal bool RegionPingThreaded()
		{
			this.region.Ping = RegionPinger.PingWhenFailed;
			int num = 0;
			int num2 = 0;
			Stopwatch stopwatch = new Stopwatch();
			try
			{
				string text = this.region.HostAndPort;
				int num3 = text.LastIndexOf(':');
				bool flag = num3 > 1;
				if (flag)
				{
					text = text.Substring(0, num3);
				}
				stopwatch.Start();
				this.regionAddress = RegionPinger.ResolveHost(text);
				stopwatch.Stop();
				bool flag2 = stopwatch.ElapsedMilliseconds > 100L;
				if (flag2)
				{
					Debug.WriteLine(string.Format("RegionPingThreaded.ResolveHost() took: {0}ms", stopwatch.ElapsedMilliseconds));
				}
			}
			catch (Exception arg)
			{
				Debug.WriteLine(string.Format("RegionPingThreaded ResolveHost failed for {0}. Caught: {1}", this.region, arg));
				this.Aborted = true;
			}
			this.CurrentAttempt = 0;
			while (this.CurrentAttempt < RegionPinger.Attempts)
			{
				bool aborted = this.Aborted;
				if (aborted)
				{
					break;
				}
				stopwatch.Reset();
				stopwatch.Start();
				try
				{
					this.ping.StartPing(this.regionAddress);
				}
				catch (Exception ex)
				{
					string[] array = new string[6];
					array[0] = "RegionPinger.RegionPingThreaded() caught exception for ping.StartPing(). Exception: ";
					int num4 = 1;
					Exception ex2 = ex;
					array[num4] = ((ex2 != null) ? ex2.ToString() : null);
					array[2] = " Source: ";
					array[3] = ex.Source;
					array[4] = " Message: ";
					array[5] = ex.Message;
					Debug.WriteLine(string.Concat(array));
					break;
				}
				while (!this.ping.Done())
				{
					bool flag3 = stopwatch.ElapsedMilliseconds >= (long)RegionPinger.MaxMillisecondsPerPing;
					if (flag3)
					{
						break;
					}
					Thread.Sleep(1);
				}
				stopwatch.Stop();
				int num5 = this.ping.Successful ? ((int)stopwatch.ElapsedMilliseconds) : RegionPinger.MaxMillisecondsPerPing;
				this.rttResults.Add(num5);
				num += num5;
				num2++;
				this.region.Ping = num / num2;
				int num6 = 4;
				while (!this.ping.Done() && num6 > 0)
				{
					num6--;
					Thread.Sleep(100);
				}
				Thread.Sleep(10);
				this.CurrentAttempt++;
			}
			this.Done = true;
			this.ping.Dispose();
			bool flag4 = this.rttResults.Count > 1 && num2 > 0;
			if (flag4)
			{
				int num7 = this.rttResults.Min();
				int num8 = this.rttResults.Max();
				int num9 = num - num8 + num7;
				this.region.Ping = num9 / num2;
			}
			this.onDoneCall(this.region);
			return false;
		}

		protected internal IEnumerator RegionPingCoroutine()
		{
			RegionPinger.<RegionPingCoroutine>d__22 <RegionPingCoroutine>d__ = new RegionPinger.<RegionPingCoroutine>d__22(0);
			<RegionPingCoroutine>d__.<>4__this = this;
			return <RegionPingCoroutine>d__;
		}

		public string GetResults()
		{
			return string.Format("{0}: {1} ({2})", this.region.Code, this.region.Ping, this.rttResults.ToStringFull<int>());
		}

		public static string ResolveHost(string hostName)
		{
			bool flag = hostName.StartsWith("wss://");
			if (flag)
			{
				hostName = hostName.Substring(6);
			}
			bool flag2 = hostName.StartsWith("ws://");
			if (flag2)
			{
				hostName = hostName.Substring(5);
			}
			string text = string.Empty;
			try
			{
				bool isUNITY_WEBGL = RuntimeUnityFlagsSetup.IsUNITY_WEBGL;
				if (isUNITY_WEBGL)
				{
					return hostName;
				}
				IPAddress[] hostAddresses = Dns.GetHostAddresses(hostName);
				bool flag3 = hostAddresses.Length == 1;
				if (flag3)
				{
					return hostAddresses[0].ToString();
				}
				foreach (IPAddress ipaddress in hostAddresses)
				{
					bool flag4 = ipaddress != null;
					if (flag4)
					{
						bool flag5 = ipaddress.ToString().Contains(":");
						if (flag5)
						{
							return ipaddress.ToString();
						}
						bool flag6 = string.IsNullOrEmpty(text);
						if (flag6)
						{
							text = hostAddresses.ToString();
						}
					}
				}
			}
			catch (Exception ex)
			{
				string[] array = new string[6];
				array[0] = "RegionPinger.ResolveHost() caught an exception for Dns.GetHostAddresses(). Exception: ";
				int num = 1;
				Exception ex2 = ex;
				array[num] = ((ex2 != null) ? ex2.ToString() : null);
				array[2] = " Source: ";
				array[3] = ex.Source;
				array[4] = " Message: ";
				array[5] = ex.Message;
				Debug.WriteLine(string.Concat(array));
			}
			return text;
		}

		public static int Attempts = 5;

		public static int MaxMillisecondsPerPing = 800;

		public static int PingWhenFailed = RegionPinger.Attempts * RegionPinger.MaxMillisecondsPerPing;

		public int CurrentAttempt = 0;

		private Action<Region> onDoneCall;

		private PhotonPing ping;

		private List<int> rttResults;

		private Region region;

		private string regionAddress;
	}
}
