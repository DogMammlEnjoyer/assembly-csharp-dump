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
		public static bool LocalSubscriptionDataInitialized
		{
			get
			{
				return SubscriptionManager._localSubscriptionDataInitialized;
			}
		}

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

		public static string GetSubsFeatureKey(SubscriptionManager.SubscriptionFeatures feature)
		{
			return SubscriptionManager.SUBS_KEYS[(int)feature];
		}

		private void Awake()
		{
			SubscriptionManager.<Awake>d__24 <Awake>d__;
			<Awake>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Awake>d__.<>4__this = this;
			<Awake>d__.<>1__state = -1;
			<Awake>d__.<>t__builder.Start<SubscriptionManager.<Awake>d__24>(ref <Awake>d__);
		}

		protected void OnEnable()
		{
			RoomSystem.PlayerJoinedEvent += new Action<NetPlayer>(this.OnPlayerJoinedRoom);
			RoomSystem.PlayerLeftEvent += new Action<NetPlayer>(this.OnPlayerLeft);
			SubscriptionManager.InitializePersonalSubscriptionData();
		}

		public static void InitializePersonalSubscriptionData()
		{
			SubscriptionManager.<InitializePersonalSubscriptionData>d__32 <InitializePersonalSubscriptionData>d__;
			<InitializePersonalSubscriptionData>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<InitializePersonalSubscriptionData>d__.<>1__state = -1;
			<InitializePersonalSubscriptionData>d__.<>t__builder.Start<SubscriptionManager.<InitializePersonalSubscriptionData>d__32>(ref <InitializePersonalSubscriptionData>d__);
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
				return default(SubscriptionManager.SubscriptionDetails);
			}
			return SubscriptionManager.GetSubscriptionDetails(SubscriptionManager.Instance.rigs[rig]);
		}

		public static SubscriptionManager.SubscriptionDetails GetSubscriptionDetails(NetPlayer np)
		{
			SubscriptionManager.SubscriptionDetails result;
			if (SubscriptionManager.Instance == null || !SubscriptionManager.Instance.subData.TryGetValue(np, out result))
			{
				return default(SubscriptionManager.SubscriptionDetails);
			}
			return result;
		}

		public static bool IsPlayerSubscribed(VRRig rig)
		{
			return SubscriptionManager.GetSubscriptionDetails(rig).active;
		}

		public static bool IsPlayerSubscribed(NetPlayer np)
		{
			return SubscriptionManager.GetSubscriptionDetails(np).active;
		}

		public static SubscriptionManager.SubscriptionDetails GetSubscriptionDetails()
		{
			SubscriptionManager.SubscriptionDetails result;
			if (SubscriptionManager.Instance == null || !SubscriptionManager.Instance.subData.TryGetValue(VRRig.LocalRig.creator, out result))
			{
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
			return !(SubscriptionManager.Instance == null) && !(VRRig.LocalRig == null) && VRRig.LocalRig.creator != null && SubscriptionManager.Instance.subData.TryGetValue(VRRig.LocalRig.creator, out subscriptionDetails) && subscriptionDetails.active;
		}

		public static void ForceRecheck()
		{
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
				return;
			}
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(player.ActorNumber, out rigContainer))
			{
				this.rigs[rigContainer.Rig] = player;
			}
			if (player != NetworkSystem.Instance.LocalPlayer)
			{
				bool flag = player == VRRig.LocalRig.creator;
			}
			bool flag2 = false;
			int daysAccrued2 = 0;
			int tier = 0;
			if (isSubscribed != null)
			{
				flag2 = isSubscribed.Value;
				tier = (flag2 ? 1 : 0);
				daysAccrued2 = daysAccrued.GetValueOrDefault();
			}
			SubscriptionManager.SubscriptionDetails value = new SubscriptionManager.SubscriptionDetails
			{
				active = flag2,
				tier = tier,
				daysAccrued = daysAccrued2
			};
			this.subData[player] = value;
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
			if (SubscriptionManager._localSubscriptionDataInitialized)
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
						active = (DateTime.Now < purchase.ExpirationTime),
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

		public static void SetSubscriptionSettingValue(SubscriptionManager.SubscriptionFeatures feature, int settingValue)
		{
			string subsFeatureKey = SubscriptionManager.GetSubsFeatureKey(feature);
			PlayerPrefs.SetInt(subsFeatureKey, settingValue);
			SubscriptionManager.subSettings[subsFeatureKey] = settingValue;
			PlayerPrefs.Save();
		}

		public static int GetSubscriptionSettingValue(SubscriptionManager.SubscriptionFeatures feature)
		{
			string subsFeatureKey = SubscriptionManager.GetSubsFeatureKey(feature);
			int result;
			if (SubscriptionManager.subSettings.TryGetValue(subsFeatureKey, out result))
			{
				return result;
			}
			SubscriptionManager.subSettings[subsFeatureKey] = PlayerPrefs.GetInt(subsFeatureKey, 1);
			return SubscriptionManager.subSettings[subsFeatureKey];
		}

		public static bool GetSubscriptionSettingBool(SubscriptionManager.SubscriptionFeatures feature)
		{
			return SubscriptionManager.GetSubscriptionSettingValue(feature) >= 1;
		}

		public static bool IsSubscriptionFeatureAvailable(SubscriptionManager.SubscriptionFeatures feature)
		{
			if (feature != SubscriptionManager.SubscriptionFeatures.IOBT)
			{
				return feature != SubscriptionManager.SubscriptionFeatures.HandTracking || UnityEngine.Application.platform == RuntimePlatform.Android;
			}
			if (UnityEngine.Application.platform != RuntimePlatform.Android)
			{
				return false;
			}
			OVRPlugin.SystemHeadset systemHeadsetType = OVRPlugin.GetSystemHeadsetType();
			return systemHeadsetType == OVRPlugin.SystemHeadset.Meta_Quest_3 || systemHeadsetType == OVRPlugin.SystemHeadset.Meta_Quest_3S || systemHeadsetType == OVRPlugin.SystemHeadset.Meta_Link_Quest_3 || systemHeadsetType == OVRPlugin.SystemHeadset.Meta_Link_Quest_3S;
		}

		public static bool CheckSubscriptionFeaturePermission(SubscriptionManager.SubscriptionFeatures feature)
		{
			if (feature != SubscriptionManager.SubscriptionFeatures.IOBT)
			{
				return feature != SubscriptionManager.SubscriptionFeatures.HandTracking || OVRPermissionsRequester.IsPermissionGranted(OVRPermissionsRequester.Permission.BodyTracking);
			}
			return OVRPermissionsRequester.IsPermissionGranted(OVRPermissionsRequester.Permission.BodyTracking);
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

		private static bool _localSubscriptionDataInitialized;

		public const string SUB_PREFIX = "SMKEYPREFIX";

		public static string[] SUBS_KEYS;

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
			HandTracking,
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
