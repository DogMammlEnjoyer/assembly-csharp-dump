using System;
using System.Collections.Generic;
using System.Text;
using ExitGames.Client.Photon;

namespace Fusion.Photon.Realtime
{
	internal class RegionHandler
	{
		public List<Region> EnabledRegions { get; protected internal set; }

		public Region BestRegion
		{
			get
			{
				bool flag = this.EnabledRegions == null;
				Region result;
				if (flag)
				{
					result = null;
				}
				else
				{
					bool flag2 = this.bestRegionCache != null;
					if (flag2)
					{
						result = this.bestRegionCache;
					}
					else
					{
						this.EnabledRegions.Sort((Region a, Region b) => a.Ping.CompareTo(b.Ping));
						int num = (int)((float)this.EnabledRegions[0].Ping * this.pingSimilarityFactor);
						Region region = this.EnabledRegions[0];
						foreach (Region region2 in this.EnabledRegions)
						{
							bool flag3 = region2.Ping <= num && region2.Code.CompareTo(region.Code) < 0;
							if (flag3)
							{
								region = region2;
							}
						}
						this.bestRegionCache = region;
						result = this.bestRegionCache;
					}
				}
				return result;
			}
		}

		public string SummaryToCache
		{
			get
			{
				bool flag = this.BestRegion != null && this.BestRegion.Ping < RegionPinger.MaxMillisecondsPerPing;
				string result;
				if (flag)
				{
					result = string.Concat(new string[]
					{
						this.BestRegion.Code,
						";",
						this.BestRegion.Ping.ToString(),
						";",
						this.availableRegionCodes
					});
				}
				else
				{
					result = this.availableRegionCodes;
				}
				return result;
			}
		}

		public string GetResults()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("Region Pinging Result: {0}\n", this.BestRegion.ToString(false));
			foreach (RegionPinger regionPinger in this.pingerList)
			{
				stringBuilder.AppendLine(regionPinger.GetResults());
			}
			stringBuilder.AppendFormat("Previous summary: {0}", this.previousSummaryProvided);
			return stringBuilder.ToString();
		}

		public void SetRegions(OperationResponse opGetRegions, LoadBalancingClient loadBalancingClient = null)
		{
			bool flag = opGetRegions.OperationCode != 220;
			if (!flag)
			{
				bool flag2 = opGetRegions.ReturnCode != 0;
				if (!flag2)
				{
					string[] array = opGetRegions[210] as string[];
					string[] array2 = opGetRegions[230] as string[];
					bool flag3 = array == null || array2 == null || array.Length != array2.Length;
					if (flag3)
					{
						bool flag4 = loadBalancingClient != null;
						if (flag4)
						{
							loadBalancingClient.DebugReturn(DebugLevel.ERROR, "RegionHandler.SetRegions() failed. Received regions and servers must be non null and of equal length. Could not read regions.");
						}
					}
					else
					{
						this.bestRegionCache = null;
						this.EnabledRegions = new List<Region>(array.Length);
						for (int i = 0; i < array.Length; i++)
						{
							string text = array2[i];
							bool flag5 = RegionHandler.PortToPingOverride > 0;
							if (flag5)
							{
								text = LoadBalancingClient.ReplacePortWithAlternative(array2[i], RegionHandler.PortToPingOverride);
							}
							bool flag6 = loadBalancingClient != null && loadBalancingClient.AddressRewriter != null;
							if (flag6)
							{
								text = loadBalancingClient.AddressRewriter(text, ServerConnection.MasterServer);
							}
							Region region = new Region(array[i], text);
							bool flag7 = string.IsNullOrEmpty(region.Code);
							if (!flag7)
							{
								this.EnabledRegions.Add(region);
							}
						}
						Array.Sort<string>(array);
						this.availableRegionCodes = string.Join(",", array);
					}
				}
			}
		}

		public bool IsPinging { get; private set; }

		public bool Aborted { get; private set; }

		public RegionHandler(ushort masterServerPortOverride = 0)
		{
			RegionHandler.PortToPingOverride = masterServerPortOverride;
		}

		public bool PingMinimumOfRegions(Action<RegionHandler> onCompleteCallback, string previousSummary)
		{
			bool flag = this.EnabledRegions == null || this.EnabledRegions.Count == 0;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool isPinging = this.IsPinging;
				if (isPinging)
				{
					result = false;
				}
				else
				{
					this.Aborted = false;
					this.IsPinging = true;
					this.previousSummaryProvided = previousSummary;
					bool flag2 = this.emptyMonoBehavior != null;
					if (flag2)
					{
						this.emptyMonoBehavior.SelfDestroy();
					}
					this.emptyMonoBehavior = MonoBehaviourEmpty.BuildInstance("RegionHandler");
					this.emptyMonoBehavior.onCompleteCall = onCompleteCallback;
					this.onCompleteCall = new Action<RegionHandler>(this.emptyMonoBehavior.CompleteOnMainThread);
					bool flag3 = string.IsNullOrEmpty(previousSummary);
					if (flag3)
					{
						result = this.PingEnabledRegions();
					}
					else
					{
						string[] array = previousSummary.Split(';', StringSplitOptions.None);
						bool flag4 = array.Length < 3;
						if (flag4)
						{
							result = this.PingEnabledRegions();
						}
						else
						{
							int num;
							bool flag5 = int.TryParse(array[1], out num);
							bool flag6 = !flag5;
							if (flag6)
							{
								result = this.PingEnabledRegions();
							}
							else
							{
								string prevBestRegionCode = array[0];
								string value = array[2];
								bool flag7 = string.IsNullOrEmpty(prevBestRegionCode);
								if (flag7)
								{
									result = this.PingEnabledRegions();
								}
								else
								{
									bool flag8 = string.IsNullOrEmpty(value);
									if (flag8)
									{
										result = this.PingEnabledRegions();
									}
									else
									{
										bool flag9 = !this.availableRegionCodes.Equals(value) || !this.availableRegionCodes.Contains(prevBestRegionCode);
										if (flag9)
										{
											result = this.PingEnabledRegions();
										}
										else
										{
											bool flag10 = num >= RegionPinger.PingWhenFailed;
											if (flag10)
											{
												result = this.PingEnabledRegions();
											}
											else
											{
												this.previousPing = num;
												Region region = this.EnabledRegions.Find((Region r) => r.Code.Equals(prevBestRegionCode));
												RegionPinger regionPinger = new RegionPinger(region, new Action<Region>(this.OnPreferredRegionPinged));
												List<RegionPinger> obj = this.pingerList;
												lock (obj)
												{
													this.pingerList.Clear();
													this.pingerList.Add(regionPinger);
												}
												regionPinger.Start();
												result = true;
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		public void Abort()
		{
			bool aborted = this.Aborted;
			if (!aborted)
			{
				this.Aborted = true;
				List<RegionPinger> obj = this.pingerList;
				lock (obj)
				{
					foreach (RegionPinger regionPinger in this.pingerList)
					{
						regionPinger.Abort();
					}
				}
				bool flag2 = this.emptyMonoBehavior != null;
				if (flag2)
				{
					this.emptyMonoBehavior.SelfDestroy();
				}
			}
		}

		private void OnPreferredRegionPinged(Region preferredRegion)
		{
			bool flag = preferredRegion.Ping > this.BestRegionSummaryPingLimit || (float)preferredRegion.Ping > (float)this.previousPing * this.rePingFactor;
			if (flag)
			{
				this.PingEnabledRegions();
			}
			else
			{
				this.IsPinging = false;
				this.onCompleteCall(this);
			}
		}

		private bool PingEnabledRegions()
		{
			bool flag = this.EnabledRegions == null || this.EnabledRegions.Count == 0;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
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
				result = true;
			}
			return result;
		}

		private void OnRegionDone(Region region)
		{
			List<RegionPinger> obj = this.pingerList;
			lock (obj)
			{
				bool flag2 = !this.IsPinging;
				if (flag2)
				{
					return;
				}
				this.bestRegionCache = null;
				foreach (RegionPinger regionPinger in this.pingerList)
				{
					bool flag3 = !regionPinger.Done;
					if (flag3)
					{
						return;
					}
				}
				this.IsPinging = false;
			}
			bool flag4 = !this.Aborted;
			if (flag4)
			{
				this.onCompleteCall(this);
			}
		}

		public static Type PingImplementation;

		private string availableRegionCodes;

		private Region bestRegionCache;

		private readonly List<RegionPinger> pingerList = new List<RegionPinger>();

		private Action<RegionHandler> onCompleteCall;

		private int previousPing;

		private string previousSummaryProvided;

		protected internal static ushort PortToPingOverride;

		private float rePingFactor = 1.2f;

		private float pingSimilarityFactor = 1.2f;

		public int BestRegionSummaryPingLimit = 90;

		private MonoBehaviourEmpty emptyMonoBehavior;
	}
}
