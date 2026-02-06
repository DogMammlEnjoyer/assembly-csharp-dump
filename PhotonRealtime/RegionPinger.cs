using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using ExitGames.Client.Photon;
using UnityEngine;

namespace Photon.Realtime
{
	public class RegionPinger
	{
		public bool Done { get; private set; }

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
			if (RegionHandler.PingImplementation == null || RegionHandler.PingImplementation == typeof(PingMono))
			{
				photonPing = new PingMono();
			}
			if (photonPing == null && RegionHandler.PingImplementation != null)
			{
				photonPing = (PhotonPing)Activator.CreateInstance(RegionHandler.PingImplementation);
			}
			return photonPing;
		}

		public bool Start()
		{
			string text = this.region.HostAndPort;
			int num = text.LastIndexOf(':');
			if (num > 1)
			{
				text = text.Substring(0, num);
			}
			this.regionAddress = RegionPinger.ResolveHost(text);
			this.ping = this.GetPingImplementation();
			this.Done = false;
			this.CurrentAttempt = 0;
			this.rttResults = new List<int>(RegionPinger.Attempts);
			bool flag = false;
			try
			{
				flag = ThreadPool.QueueUserWorkItem(new WaitCallback(this.RegionPingPooled));
			}
			catch
			{
				flag = false;
			}
			if (!flag)
			{
				SupportClass.StartBackgroundCalls(new Func<bool>(this.RegionPingThreaded), 0, "RegionPing_" + this.region.Code + "_" + this.region.Cluster);
			}
			return true;
		}

		protected internal void RegionPingPooled(object context)
		{
			this.RegionPingThreaded();
		}

		protected internal bool RegionPingThreaded()
		{
			this.region.Ping = RegionPinger.PingWhenFailed;
			float num = 0f;
			int num2 = 0;
			Stopwatch stopwatch = new Stopwatch();
			this.CurrentAttempt = 0;
			while (this.CurrentAttempt < RegionPinger.Attempts)
			{
				bool flag = false;
				stopwatch.Reset();
				stopwatch.Start();
				try
				{
					this.ping.StartPing(this.regionAddress);
					goto IL_6A;
				}
				catch (Exception)
				{
					break;
				}
				goto IL_52;
				IL_77:
				stopwatch.Stop();
				int num3 = (int)stopwatch.ElapsedMilliseconds;
				this.rttResults.Add(num3);
				if ((!RegionPinger.IgnoreInitialAttempt || this.CurrentAttempt != 0) && this.ping.Successful && !flag)
				{
					num += (float)num3;
					num2++;
					this.region.Ping = (int)(num / (float)num2);
				}
				Thread.Sleep(10);
				this.CurrentAttempt++;
				continue;
				IL_52:
				if (stopwatch.ElapsedMilliseconds >= (long)RegionPinger.MaxMilliseconsPerPing)
				{
					flag = true;
					goto IL_77;
				}
				Thread.Sleep(0);
				IL_6A:
				if (this.ping.Done())
				{
					goto IL_77;
				}
				goto IL_52;
			}
			this.Done = true;
			this.ping.Dispose();
			this.onDoneCall(this.region);
			return false;
		}

		protected internal IEnumerator RegionPingCoroutine()
		{
			this.region.Ping = RegionPinger.PingWhenFailed;
			float rttSum = 0f;
			int replyCount = 0;
			Stopwatch sw = new Stopwatch();
			this.CurrentAttempt = 0;
			while (this.CurrentAttempt < RegionPinger.Attempts)
			{
				bool overtime = false;
				sw.Reset();
				sw.Start();
				try
				{
					this.ping.StartPing(this.regionAddress);
					goto IL_F1;
				}
				catch (Exception ex)
				{
					string str = "catched: ";
					Exception ex2 = ex;
					Debug.Log(str + ((ex2 != null) ? ex2.ToString() : null));
					break;
				}
				goto IL_B9;
				IL_FE:
				sw.Stop();
				int num = (int)sw.ElapsedMilliseconds;
				this.rttResults.Add(num);
				if ((!RegionPinger.IgnoreInitialAttempt || this.CurrentAttempt != 0) && this.ping.Successful && !overtime)
				{
					rttSum += (float)num;
					int num2 = replyCount;
					replyCount = num2 + 1;
					this.region.Ping = (int)(rttSum / (float)replyCount);
				}
				yield return new WaitForSeconds(0.1f);
				this.CurrentAttempt++;
				continue;
				IL_B9:
				if (sw.ElapsedMilliseconds >= (long)RegionPinger.MaxMilliseconsPerPing)
				{
					overtime = true;
					goto IL_FE;
				}
				yield return 0;
				IL_F1:
				if (this.ping.Done())
				{
					goto IL_FE;
				}
				goto IL_B9;
			}
			this.Done = true;
			this.ping.Dispose();
			this.onDoneCall(this.region);
			yield return null;
			yield break;
		}

		public string GetResults()
		{
			return string.Format("{0}: {1} ({2})", this.region.Code, this.region.Ping, this.rttResults.ToStringFull<int>());
		}

		public static string ResolveHost(string hostName)
		{
			if (hostName.StartsWith("wss://"))
			{
				hostName = hostName.Substring(6);
			}
			if (hostName.StartsWith("ws://"))
			{
				hostName = hostName.Substring(5);
			}
			string text = string.Empty;
			try
			{
				IPAddress[] hostAddresses = Dns.GetHostAddresses(hostName);
				if (hostAddresses.Length == 1)
				{
					return hostAddresses[0].ToString();
				}
				foreach (IPAddress ipaddress in hostAddresses)
				{
					if (ipaddress != null)
					{
						if (ipaddress.ToString().Contains(":"))
						{
							return ipaddress.ToString();
						}
						if (string.IsNullOrEmpty(text))
						{
							text = hostAddresses.ToString();
						}
					}
				}
			}
			catch (Exception)
			{
			}
			return text;
		}

		public static int Attempts = 5;

		public static bool IgnoreInitialAttempt = true;

		public static int MaxMilliseconsPerPing = 800;

		public static int PingWhenFailed = RegionPinger.Attempts * RegionPinger.MaxMilliseconsPerPing;

		private Region region;

		private string regionAddress;

		public int CurrentAttempt;

		private Action<Region> onDoneCall;

		private PhotonPing ping;

		private List<int> rttResults;
	}
}
