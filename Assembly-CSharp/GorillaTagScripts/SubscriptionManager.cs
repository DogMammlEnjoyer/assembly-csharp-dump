using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Oculus.Platform;
using Oculus.Platform.Models;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts
{
	public class SubscriptionManager : MonoBehaviour
	{
		public static bool SubsOnlyMatchmaking
		{
			get
			{
				return PlayerPrefs.GetInt("subsOnlyMatchmaking") == 1;
			}
			set
			{
				PlayerPrefs.SetInt("subsOnlyMatchmaking", value ? 1 : 0);
				PlayerPrefs.Save();
			}
		}

		private void Awake()
		{
			SubscriptionManager.<Awake>d__21 <Awake>d__;
			<Awake>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Awake>d__.<>4__this = this;
			<Awake>d__.<>1__state = -1;
			<Awake>d__.<>t__builder.Start<SubscriptionManager.<Awake>d__21>(ref <Awake>d__);
		}

		protected void OnEnable()
		{
			RoomSystem.PlayerJoinedEvent += new Action<NetPlayer>(this.OnPlayerJoinedRoom);
			RoomSystem.PlayerLeftEvent += new Action<NetPlayer>(this.OnPlayerLeft);
			SubscriptionManager.InitializePersonalSubscriptionData();
		}

		public static void InitializePersonalSubscriptionData()
		{
			SubscriptionManager.<InitializePersonalSubscriptionData>d__29 <InitializePersonalSubscriptionData>d__;
			<InitializePersonalSubscriptionData>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<InitializePersonalSubscriptionData>d__.<>1__state = -1;
			<InitializePersonalSubscriptionData>d__.<>t__builder.Start<SubscriptionManager.<InitializePersonalSubscriptionData>d__29>(ref <InitializePersonalSubscriptionData>d__);
		}

		protected void OnDisable()
		{
			RoomSystem.PlayerJoinedEvent -= new Action<NetPlayer>(this.OnPlayerJoinedRoom);
			RoomSystem.PlayerLeftEvent -= new Action<NetPlayer>(this.OnPlayerLeft);
		}

		public static SubscriptionManager.SubscriptionDetails GetSubscriptionDetails(VRRig rig)
		{
			if (SubscriptionManager.Instance == null || !SubscriptionManager.Instance.rigs.ContainsKey(rig))
			{
				Debug.LogError("Failed to get subscription details via VRRig");
				return default(SubscriptionManager.SubscriptionDetails);
			}
			return SubscriptionManager.GetSubscriptionDetails(SubscriptionManager.Instance.rigs[rig]);
		}

		public static SubscriptionManager.SubscriptionDetails GetSubscriptionDetails(NetPlayer np)
		{
			SubscriptionManager.SubscriptionDetails result;
			if (SubscriptionManager.Instance == null || !SubscriptionManager.Instance.subData.TryGetValue(np, out result))
			{
				Debug.LogError("Failed to get subscription details via NetPlayer");
				return default(SubscriptionManager.SubscriptionDetails);
			}
			return result;
		}

		public static SubscriptionManager.SubscriptionDetails GetSubscriptionDetails()
		{
			SubscriptionManager.SubscriptionDetails result;
			if (SubscriptionManager.Instance == null || !SubscriptionManager.Instance.subData.TryGetValue(VRRig.LocalRig.creator, out result))
			{
				Debug.LogError("Failed to get local player's subscription details");
				return default(SubscriptionManager.SubscriptionDetails);
			}
			return result;
		}

		public static SubscriptionManager.SubscriptionStatus LocalSubscriptionStatus()
		{
			SubscriptionManager.SubscriptionDetails subscriptionDetails;
			if (SubscriptionManager.Instance == null || !SubscriptionManager.Instance.subData.TryGetValue(VRRig.LocalRig.creator, out subscriptionDetails))
			{
				return SubscriptionManager.SubscriptionStatus.Unknown;
			}
			if (!subscriptionDetails.active)
			{
				return SubscriptionManager.SubscriptionStatus.Inactive;
			}
			return SubscriptionManager.SubscriptionStatus.Active;
		}

		public static SubscriptionManager.SubscriptionDetails LocalSubscriptionDetails()
		{
			return SubscriptionManager.localSubscriptionDetails;
		}

		public static bool IsLocalSubscribed()
		{
			SubscriptionManager.SubscriptionDetails subscriptionDetails;
			if (SubscriptionManager.Instance == null || !SubscriptionManager.Instance.subData.TryGetValue(VRRig.LocalRig.creator, out subscriptionDetails))
			{
				Debug.LogError("Failed to get local player's subscription details");
				return false;
			}
			return subscriptionDetails.active;
		}

		public static void ForceRecheck()
		{
			Debug.Log("SubscriptionsManager: Running ForceRecheck");
			SubscriptionManager.Instance.OnPlayerJoinedRoom(null);
		}

		private void OnPlayerJoinedRoom(NetPlayer npl)
		{
			if (SubscriptionManager.OnSubscriptionData != null)
			{
				SubscriptionManager.OnSubscriptionData();
			}
			if (NetworkSystem.Instance.AllNetPlayers.Length > SubscriptionManager.PERF_CHANGE_ROOMSIZE)
			{
				GorillaTagger.Instance.ToggleForcedPerformanceRefresh();
				PhotonNetwork.SendRate = 20;
			}
		}

		private void UpdatePlayerSubsDetails(NetPlayer player, bool? isSubscribed = null, int? daysAccrued = null)
		{
			if (player == null)
			{
				Debug.LogWarning("SubscriptionsManager: NetPlayer is null!");
				return;
			}
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(player.ActorNumber, out rigContainer))
			{
				this.rigs[rigContainer.Rig] = player;
			}
			bool flag = player == NetworkSystem.Instance.LocalPlayer || player == VRRig.LocalRig.creator;
			Debug.Log(string.Format("SubscriptionManager: UpdatePlayerSubsDetails: {0} {1}", player.NickName, flag));
			bool flag2 = false;
			int daysAccrued2 = 0;
			int tier = 0;
			if (isSubscribed != null)
			{
				flag2 = isSubscribed.Value;
				tier = (flag2 ? 1 : 0);
				daysAccrued2 = daysAccrued.GetValueOrDefault();
				Debug.Log(string.Format("SubscriptionManager: Using networked subscription data for {0} - Active: {1}", player.NickName, flag2));
			}
			SubscriptionManager.SubscriptionDetails subscriptionDetails = new SubscriptionManager.SubscriptionDetails
			{
				active = flag2,
				tier = tier,
				daysAccrued = daysAccrued2
			};
			this.subData[player] = subscriptionDetails;
			Debug.Log(string.Format("SubscriptionManager: Final state for {0} - Active: {1}, Tier: {2}, Days: {3}", new object[]
			{
				player.NickName,
				subscriptionDetails.active,
				subscriptionDetails.tier,
				subscriptionDetails.daysAccrued
			}));
		}

		private void OnPlayerLeft(NetPlayer pl)
		{
			if (this.subData.ContainsKey(pl))
			{
				this.subData.Remove(pl);
			}
			NetPlayer[] allNetPlayers = NetworkSystem.Instance.AllNetPlayers;
			if (allNetPlayers.Length <= SubscriptionManager.PERF_CHANGE_ROOMSIZE)
			{
				GorillaTagger.Instance.ToggleDefaultPerformanceRefresh();
				PhotonNetwork.SendRate = SubscriptionManager.DEFAULT_SEND_RATE;
			}
			NetPlayer lowestNetPlayer = this.GetLowestNetPlayer(allNetPlayers);
			if (lowestNetPlayer != null && lowestNetPlayer == NetworkSystem.Instance.LocalPlayer)
			{
				byte currentRoomExpectedSize = RoomSystem.GetCurrentRoomExpectedSize();
				PhotonNetwork.CurrentRoom.MaxPlayers = currentRoomExpectedSize;
			}
		}

		private NetPlayer GetLowestNetPlayer(NetPlayer[] players)
		{
			NetPlayer result = null;
			int num = int.MaxValue;
			for (int i = 0; i < players.Length; i++)
			{
				if (players[i].ActorNumber < num)
				{
					num = players[i].ActorNumber;
					result = players[i];
				}
			}
			return result;
		}

		private void OnGetViewerPurchasesStartup(Message msg)
		{
			if (msg.IsError)
			{
				if (this.attempts < 3)
				{
					this.attempts++;
					IAP.GetViewerPurchases().OnComplete(new Message<PurchaseList>.Callback(this.OnGetViewerPurchasesStartup));
				}
				return;
			}
			if (msg.GetPurchaseList() == null)
			{
				return;
			}
			bool flag = false;
			foreach (Purchase purchase in msg.GetPurchaseList())
			{
				if (purchase.Type == ProductType.SUBSCRIPTION && purchase.Sku.Contains("fan_club"))
				{
					flag = true;
					SubscriptionManager.localSubscriptionDetails = new SubscriptionManager.SubscriptionDetails
					{
						active = true,
						subscriptionActiveUntilDate = purchase.ExpirationTime
					};
				}
			}
			if (!flag)
			{
				SubscriptionManager.localSubscriptionDetails = new SubscriptionManager.SubscriptionDetails
				{
					active = false
				};
			}
		}

		public static void SetSubscriptionSettingValue(string settingKey, int settingValue)
		{
			PlayerPrefs.SetInt(settingKey, settingValue);
			SubscriptionManager.subSettings[settingKey] = settingValue;
			PlayerPrefs.Save();
		}

		public static int GetSubscriptionSettingValue(string settingKey)
		{
			int result;
			if (SubscriptionManager.subSettings.TryGetValue(settingKey, out result))
			{
				return result;
			}
			SubscriptionManager.subSettings[settingKey] = PlayerPrefs.GetInt(settingKey, 1);
			return SubscriptionManager.subSettings[settingKey];
		}

		[RuntimeInitializeOnLoadMethod]
		private static void OnLoad()
		{
		}

		public static void UpdatePlayerSubscriptionData(NetPlayer player, bool isSubscribed, int daysAccrued = 0)
		{
			if (SubscriptionManager.Instance == null)
			{
				Debug.LogWarning("SubscriptionManager: Instance is null, cannot update player subscription data");
				return;
			}
			if (player == null)
			{
				Debug.LogWarning("SubscriptionManager: NetPlayer is null, cannot update subscription data");
				return;
			}
			Debug.Log(string.Format("SubscriptionManager: Received networked subscription update for {0} - Subscribed: {1}", player.NickName, isSubscribed));
			SubscriptionManager.Instance.UpdatePlayerSubsDetails(player, new bool?(isSubscribed), new int?(daysAccrued));
			if (SubscriptionManager.OnSubscriptionData != null)
			{
				SubscriptionManager.OnSubscriptionData();
			}
		}

		public const string FAN_CLUB_BASE_SKU = "fan_club";

		public const string SUBSCRIBER_NAME_COLOR_HEX = "#ffc600";

		public static Color SUBSCRIBER_NAME_COLOR = Color.gold;

		public const int PERF_SEND_RATE = 20;

		public static int DEFAULT_SEND_RATE = 30;

		public static int PERF_CHANGE_ROOMSIZE = 10;

		private static SubscriptionManager Instance;

		public static Action OnSubscriptionData;

		public static Action OnLocalSubscriptionData;

		private Dictionary<NetPlayer, SubscriptionManager.SubscriptionDetails> subData = new Dictionary<NetPlayer, SubscriptionManager.SubscriptionDetails>();

		private Dictionary<VRRig, NetPlayer> rigs = new Dictionary<VRRig, NetPlayer>();

		private static SubscriptionManager.SubscriptionDetails localSubscriptionDetails;

		public const string SUB_PREFIX = "SMKEYPREFIX";

		public const string GOLDEN_NAME_KEY = "SMKEYPREFIXGOLDEN_NAME_KEY";

		public const string IOBT_ENABLE_KEY = "SMKEYPREFIXIOBT_ENABLE_KEY";

		private static int maxRetries = 3;

		private int attempts;

		private static Dictionary<string, int> subSettings = new Dictionary<string, int>();

		public enum SubscriptionStatus
		{
			Active,
			Inactive,
			Unknown
		}

		public enum SubscriptionTerm
		{
			MONTHLY,
			QUARTERLY,
			SEMIANNUAL,
			ANNUAL
		}

		public enum SubscriptionFeatures
		{
			GoldenName,
			IOBT,
			SubscriptionFeatureCount
		}

		public struct SubscriptionDetails
		{
			public bool active;

			public int daysAccrued;

			public bool[] subscriptionFeatureSettings;

			public int tier;

			public DateTime subscriptionActiveUntilDate;

			public bool autoRenew;

			public int autoRenewMonths;
		}

		[Serializable]
		private class MothershipSubscription
		{
			public string SubscriptionId;

			public DateTimeOffset EarliestStartDate;

			public DateTimeOffset CurrentStartDate;

			public DateTimeOffset MostRecentBillingCycleStartDate;

			public DateTimeOffset MostRecentBillingCycleEndDate;

			public int TotalLifetimeSeconds;

			public bool IsActive;

			public bool IsCancelling;

			public string Sku;

			public string PlayerId;

			public string TrialType;

			public string ExternalServiceName;

			public string ExternalSubscriptionId;

			public string SubscriptionCatalogItemId;
		}

		[Serializable]
		private class GrantedSubscriptionBenefit
		{
			public string BenefitId;

			public DateTimeOffset GrantedTime;

			public string PlayFabItemId;
		}

		[Serializable]
		private class GetMySubscriptionsAndTheirBenefitsRequest
		{
			public bool Refresh;

			public bool? SkipBenefitsCheck;

			public bool? SkipSharedGroupDataUpdate;

			public string MothershipId;

			public string MothershipToken;

			public string MothershipEnvId;

			public string MothershipDeploymentId;
		}

		[Serializable]
		private class GetMySubscriptionsAndTheirBenefitsResponse
		{
			public List<SubscriptionManager.MothershipSubscription> Subscriptions;

			public Dictionary<string, List<SubscriptionManager.GrantedSubscriptionBenefit>> PreviouslyGrantedBenefitsBySubscriptionSku;

			public Dictionary<string, List<SubscriptionManager.GrantedSubscriptionBenefit>> NewlyGrantedBenefitsBySubscriptionSku;

			public bool? SharedGroupDataUpdateSucceeded;
		}
	}
}
