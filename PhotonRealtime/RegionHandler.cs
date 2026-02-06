using System;
using System.Collections.Generic;
using System.Text;
using ExitGames.Client.Photon;

namespace Photon.Realtime
{
	public class RegionHandler
	{
		public List<Region> EnabledRegions { get; protected internal set; }

		public Region BestRegion
		{
			get
			{
				if (this.EnabledRegions == null)
				{
					return null;
				}
				if (this.bestRegionCache != null)
				{
					return this.bestRegionCache;
				}
				this.EnabledRegions.Sort((Region a, Region b) => a.Ping.CompareTo(b.Ping));
				this.bestRegionCache = this.EnabledRegions[0];
				return this.bestRegionCache;
			}
		}

		public string SummaryToCache
		{
			get
			{
				if (this.BestRegion != null)
				{
					return string.Concat(new string[]
					{
						this.BestRegion.Code,
						";",
						this.BestRegion.Ping.ToString(),
						";",
						this.availableRegionCodes
					});
				}
				return this.availableRegionCodes;
			}
		}

		public string GetResults()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("Region Pinging Result: {0}\n", this.BestRegion.ToString(false));
			foreach (RegionPinger regionPinger in this.pingerList)
			{
				stringBuilder.AppendFormat(regionPinger.GetResults() + "\n", Array.Empty<object>());
			}
			stringBuilder.AppendFormat("Previous summary: {0}", this.previousSummaryProvided);
			return stringBuilder.ToString();
		}

		public void SetRegions(OperationResponse opGetRegions)
		{
			if (opGetRegions.OperationCode != 220)
			{
				return;
			}
			if (opGetRegions.ReturnCode != 0)
			{
				return;
			}
			string[] array = opGetRegions[210] as string[];
			string[] array2 = opGetRegions[230] as string[];
			if (array == null || array2 == null || array.Length != array2.Length)
			{
				return;
			}
			this.bestRegionCache = null;
			this.EnabledRegions = new List<Region>(array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				string address = array2[i];
				if (RegionHandler.PortToPingOverride != 0)
				{
					address = LoadBalancingClient.ReplacePortWithAlternative(array2[i], RegionHandler.PortToPingOverride);
				}
				Region region = new Region(array[i], address);
				if (!string.IsNullOrEmpty(region.Code))
				{
					this.EnabledRegions.Add(region);
				}
			}
			Array.Sort<string>(array);
			this.availableRegionCodes = string.Join(",", array);
		}

		public bool IsPinging { get; private set; }

		public RegionHandler(ushort masterServerPortOverride = 0)
		{
			RegionHandler.PortToPingOverride = masterServerPortOverride;
		}

		public bool PingMinimumOfRegions(Action<RegionHandler> onCompleteCallback, string previousSummary)
		{
			if (this.EnabledRegions == null || this.EnabledRegions.Count == 0)
			{
				return false;
			}
			if (this.IsPinging)
			{
				return false;
			}
			this.IsPinging = true;
			this.onCompleteCall = onCompleteCallback;
			this.previousSummaryProvided = previousSummary;
			if (string.IsNullOrEmpty(previousSummary))
			{
				return this.PingEnabledRegions();
			}
			string[] array = previousSummary.Split(';', StringSplitOptions.None);
			if (array.Length < 3)
			{
				return this.PingEnabledRegions();
			}
			int num;
			if (!int.TryParse(array[1], out num))
			{
				return this.PingEnabledRegions();
			}
			string prevBestRegionCode = array[0];
			string value = array[2];
			if (string.IsNullOrEmpty(prevBestRegionCode))
			{
				return this.PingEnabledRegions();
			}
			if (string.IsNullOrEmpty(value))
			{
				return this.PingEnabledRegions();
			}
			if (!this.availableRegionCodes.Equals(value) || !this.availableRegionCodes.Contains(prevBestRegionCode))
			{
				return this.PingEnabledRegions();
			}
			if (num >= RegionPinger.PingWhenFailed)
			{
				return this.PingEnabledRegions();
			}
			this.previousPing = num;
			RegionPinger regionPinger = new RegionPinger(this.EnabledRegions.Find((Region r) => r.Code.Equals(prevBestRegionCode)), new Action<Region>(this.OnPreferredRegionPinged));
			List<RegionPinger> obj = this.pingerList;
			lock (obj)
			{
				this.pingerList.Add(regionPinger);
			}
			regionPinger.Start();
			return true;
		}

		private void OnPreferredRegionPinged(Region preferredRegion)
		{
			if ((float)preferredRegion.Ping > (float)this.previousPing * 1.5f)
			{
				this.PingEnabledRegions();
				return;
			}
			this.IsPinging = false;
			this.onCompleteCall(this);
		}

		private bool PingEnabledRegions()
		{
			if (this.EnabledRegions == null || this.EnabledRegions.Count == 0)
			{
				return false;
			}
			List<RegionPinger> obj = this.pingerList;
			lock (obj)
			{
				this.pingerList.Clear();
				foreach (Region region in this.EnabledRegions)
				{
					RegionPinger regionPinger = new RegionPinger(region, new Action<Region>(this.OnRegionDone));
					this.pingerList.Add(regionPinger);
					regionPinger.Start();
				}
			}
			return true;
		}

		private void OnRegionDone(Region region)
		{
			List<RegionPinger> obj = this.pingerList;
			lock (obj)
			{
				if (!this.IsPinging)
				{
					return;
				}
				this.bestRegionCache = null;
				using (List<RegionPinger>.Enumerator enumerator = this.pingerList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (!enumerator.Current.Done)
						{
							return;
						}
					}
				}
				this.IsPinging = false;
			}
			this.onCompleteCall(this);
		}

		public static Type PingImplementation;

		private string availableRegionCodes;

		private Region bestRegionCache;

		private List<RegionPinger> pingerList = new List<RegionPinger>();

		private Action<RegionHandler> onCompleteCall;

		private int previousPing;

		private string previousSummaryProvided;

		protected internal static ushort PortToPingOverride;
	}
}
