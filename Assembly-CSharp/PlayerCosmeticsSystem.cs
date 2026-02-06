using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using GorillaExtensions;
using GorillaNetworking;
using GorillaTagScripts;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

internal class PlayerCosmeticsSystem : MonoBehaviour, ITickSystemPre
{
	bool ITickSystemPre.PreTickRunning { get; set; }

	private void Awake()
	{
		if (PlayerCosmeticsSystem.instance == null)
		{
			PlayerCosmeticsSystem.instance = this;
			base.transform.SetParent(null, true);
			Object.DontDestroyOnLoad(this);
			this.inventory = new List<string>();
			this.inventory.Add("Inventory");
			this.inventory.Add(PlayerCosmeticsSystem.subscriptionKey);
			NetworkSystem.Instance.OnRaiseEvent += this.OnNetEvent;
			return;
		}
		Object.Destroy(this);
	}

	private void Start()
	{
		this.playerLookUpCooldown = Mathf.Max(this.playerLookUpCooldown, 3f);
		PlayFabTitleDataCache.Instance.GetTitleData("EnableTempCosmeticUnlocks", delegate(string data)
		{
			bool tempUnlocksEnabled;
			if (bool.TryParse(data, out tempUnlocksEnabled))
			{
				PlayerCosmeticsSystem.TempUnlocksEnabled = tempUnlocksEnabled;
				return;
			}
			Debug.LogError("PlayerCosmeticsSystem: error parsing EnableTempCosmeticUnlocks data");
		}, delegate(PlayFabError error)
		{
		}, false);
	}

	private void OnDestroy()
	{
		if (PlayerCosmeticsSystem.instance == this)
		{
			PlayerCosmeticsSystem.instance = null;
		}
	}

	private void LookUpPlayerCosmetics(bool wait = false)
	{
		if (!this.isLookingUp)
		{
			TickSystem<object>.AddPreTickCallback(this);
			if (wait)
			{
				this.startSearchingTime = Time.time;
				return;
			}
			this.startSearchingTime = float.MinValue;
		}
	}

	public void PreTick()
	{
		if (PlayerCosmeticsSystem.playersToLookUp.Count < 1)
		{
			TickSystem<object>.RemovePreTickCallback(this);
			this.startSearchingTime = float.MinValue;
			this.isLookingUp = false;
			return;
		}
		this.isLookingUp = true;
		if (this.startSearchingTime + this.playerLookUpCooldown > Time.time)
		{
			return;
		}
		if (GorillaServer.Instance.NewCosmeticsPathShouldReadSharedGroupData())
		{
			this.NewCosmeticsPath();
			return;
		}
		PlayerCosmeticsSystem.playerIDsList.Clear();
		while (PlayerCosmeticsSystem.playersToLookUp.Count > 0)
		{
			NetPlayer netPlayer = PlayerCosmeticsSystem.playersToLookUp.Dequeue();
			string item = netPlayer.ActorNumber.ToString();
			if (netPlayer.InRoom() && !PlayerCosmeticsSystem.playerIDsList.Contains(item))
			{
				if (PlayerCosmeticsSystem.playerIDsList.Count == 0)
				{
					int actorNumber = netPlayer.ActorNumber;
				}
				PlayerCosmeticsSystem.playerIDsList.Add(item);
				PlayerCosmeticsSystem.playersWaiting.AddSortedUnique(netPlayer.ActorNumber);
			}
		}
		if (PlayerCosmeticsSystem.playerIDsList.Count > 0)
		{
			PlayFab.ClientModels.GetSharedGroupDataRequest getSharedGroupDataRequest = new PlayFab.ClientModels.GetSharedGroupDataRequest();
			getSharedGroupDataRequest.Keys = PlayerCosmeticsSystem.playerIDsList;
			getSharedGroupDataRequest.SharedGroupId = NetworkSystem.Instance.RoomName + Regex.Replace(NetworkSystem.Instance.CurrentRegion, "[^a-zA-Z0-9]", "").ToUpper();
			PlayFabClientAPI.GetSharedGroupData(getSharedGroupDataRequest, new Action<GetSharedGroupDataResult>(this.OnGetsharedGroupData), delegate(PlayFabError error)
			{
				Debug.Log(error.GenerateErrorReport());
				if (error.Error == PlayFabErrorCode.NotAuthenticated)
				{
					PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
					return;
				}
				if (error.Error == PlayFabErrorCode.AccountBanned)
				{
					GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
				}
			}, null, null);
		}
		this.isLookingUp = false;
	}

	private void NewCosmeticsPath()
	{
		if (this.isLookingUpNew)
		{
			return;
		}
		base.StartCoroutine(this.NewCosmeticsPathCoroutine());
	}

	private IEnumerator NewCosmeticsPathCoroutine()
	{
		this.isLookingUpNew = true;
		NetPlayer player = null;
		PlayerCosmeticsSystem.playerIDsList.Clear();
		PlayerCosmeticsSystem.playerActorNumberList.Clear();
		while (PlayerCosmeticsSystem.playersToLookUp.Count > 0)
		{
			player = PlayerCosmeticsSystem.playersToLookUp.Dequeue();
			string item = player.ActorNumber.ToString();
			if (player.InRoom() && !PlayerCosmeticsSystem.playerIDsList.Contains(item))
			{
				PlayerCosmeticsSystem.playerIDsList.Add(player.UserId);
				PlayerCosmeticsSystem.playerActorNumberList.Add(player.ActorNumber);
			}
		}
		int num;
		for (int i = 0; i < PlayerCosmeticsSystem.playerIDsList.Count; i = num + 1)
		{
			int j = i;
			PlayFab.ClientModels.GetSharedGroupDataRequest getSharedGroupDataRequest = new PlayFab.ClientModels.GetSharedGroupDataRequest();
			getSharedGroupDataRequest.Keys = this.inventory;
			getSharedGroupDataRequest.SharedGroupId = PlayerCosmeticsSystem.playerIDsList[j] + "Inventory";
			PlayFabClientAPI.GetSharedGroupData(getSharedGroupDataRequest, delegate(GetSharedGroupDataResult result)
			{
				if (!NetworkSystem.Instance.InRoom)
				{
					PlayerCosmeticsSystem.playersWaiting.Clear();
					return;
				}
				bool flag = false;
				foreach (KeyValuePair<string, PlayFab.ClientModels.SharedGroupDataRecord> keyValuePair in result.Data)
				{
					if (keyValuePair.Key == "Inventory")
					{
						int j;
						if (!Utils.PlayerInRoom(PlayerCosmeticsSystem.playerActorNumberList[j]))
						{
							continue;
						}
						this.tempCosmetics = keyValuePair.Value.Value;
						IUserCosmeticsCallback userCosmeticsCallback;
						if (!PlayerCosmeticsSystem.userCosmeticCallback.TryGetValue(PlayerCosmeticsSystem.playerActorNumberList[j], out userCosmeticsCallback))
						{
							PlayerCosmeticsSystem.userCosmeticsWaiting[PlayerCosmeticsSystem.playerActorNumberList[j]] = this.tempCosmetics;
						}
						else
						{
							userCosmeticsCallback.PendingUpdate = false;
							if (!userCosmeticsCallback.OnGetUserCosmetics(this.tempCosmetics))
							{
								PlayerCosmeticsSystem.playersToLookUp.Enqueue(player);
								userCosmeticsCallback.PendingUpdate = true;
							}
						}
					}
					else if (keyValuePair.Key == PlayerCosmeticsSystem.subscriptionKey)
					{
						flag = true;
						NetPlayer netPlayer = null;
						NetPlayer[] allNetPlayers = NetworkSystem.Instance.AllNetPlayers;
						for (int j = 0; j < allNetPlayers.Length; j++)
						{
							NetPlayer netPlayer2 = allNetPlayers[j];
							if (netPlayer2.ActorNumber == PlayerCosmeticsSystem.playerActorNumberList[j])
							{
								netPlayer = netPlayer2;
								break;
							}
						}
						if (netPlayer != null)
						{
							bool isSubscribed = false;
							if (!string.IsNullOrEmpty(keyValuePair.Value.Value))
							{
								try
								{
									isSubscribed = JsonConvert.DeserializeObject<PlayerCosmeticsSystem.PlayFabSubscriptionData>(keyValuePair.Value.Value).IsActive;
								}
								catch (Exception ex)
								{
									Debug.LogError("Failed to deserialize subscription data for " + netPlayer.NickName + ": " + ex.Message);
								}
							}
							SubscriptionManager.UpdatePlayerSubscriptionData(netPlayer, isSubscribed, 0);
						}
					}
					if (!flag)
					{
						NetPlayer netPlayer3 = null;
						NetPlayer[] allNetPlayers = NetworkSystem.Instance.AllNetPlayers;
						for (int j = 0; j < allNetPlayers.Length; j++)
						{
							NetPlayer netPlayer4 = allNetPlayers[j];
							if (netPlayer4.ActorNumber == PlayerCosmeticsSystem.playerActorNumberList[j])
							{
								netPlayer3 = netPlayer4;
								break;
							}
						}
						if (netPlayer3 != null)
						{
							SubscriptionManager.UpdatePlayerSubscriptionData(netPlayer3, false, 0);
						}
					}
				}
			}, delegate(PlayFabError error)
			{
				if (error.Error == PlayFabErrorCode.NotAuthenticated)
				{
					PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
					return;
				}
				if (error.Error == PlayFabErrorCode.AccountBanned)
				{
					GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
				}
			}, null, null);
			yield return new WaitForSeconds(this.getSharedGroupDataCooldown);
			num = i;
		}
		this.isLookingUpNew = false;
		yield break;
	}

	private void UpdatePlayersWaitingAndDoLookup(bool retrying)
	{
		if (PlayerCosmeticsSystem.playersWaiting.Count > 0)
		{
			for (int i = PlayerCosmeticsSystem.playersWaiting.Count - 1; i >= 0; i--)
			{
				int num = PlayerCosmeticsSystem.playersWaiting[i];
				if (!Utils.PlayerInRoom(num))
				{
					PlayerCosmeticsSystem.playersWaiting.RemoveAt(i);
				}
				else
				{
					PlayerCosmeticsSystem.playersToLookUp.Enqueue(NetworkSystem.Instance.GetPlayer(num));
					retrying = true;
				}
			}
		}
		if (retrying)
		{
			this.LookUpPlayerCosmetics(true);
		}
	}

	private void OnGetsharedGroupData(GetSharedGroupDataResult result)
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			PlayerCosmeticsSystem.playersWaiting.Clear();
			return;
		}
		bool retrying = false;
		foreach (KeyValuePair<string, PlayFab.ClientModels.SharedGroupDataRecord> keyValuePair in result.Data)
		{
			this.playerTemp = null;
			int num;
			if (int.TryParse(keyValuePair.Key, out num))
			{
				if (!Utils.PlayerInRoom(num))
				{
					PlayerCosmeticsSystem.playersWaiting.Remove(num);
				}
				else
				{
					PlayerCosmeticsSystem.playersWaiting.Remove(num);
					this.playerTemp = NetworkSystem.Instance.GetPlayer(num);
					this.tempCosmetics = keyValuePair.Value.Value;
					IUserCosmeticsCallback userCosmeticsCallback;
					if (!PlayerCosmeticsSystem.userCosmeticCallback.TryGetValue(num, out userCosmeticsCallback))
					{
						PlayerCosmeticsSystem.userCosmeticsWaiting[num] = this.tempCosmetics;
					}
					else
					{
						userCosmeticsCallback.PendingUpdate = false;
						if (!userCosmeticsCallback.OnGetUserCosmetics(this.tempCosmetics))
						{
							Debug.Log("retrying cosmetics for " + this.playerTemp.ToStringFull());
							PlayerCosmeticsSystem.playersToLookUp.Enqueue(this.playerTemp);
							retrying = true;
							userCosmeticsCallback.PendingUpdate = true;
						}
					}
				}
			}
		}
		this.UpdatePlayersWaitingAndDoLookup(retrying);
	}

	private void OnNetEvent(byte code, object data, int source)
	{
		if (code != 199 || source < 0)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(source);
		GorillaNot.IncrementRPCCall(new PhotonMessageInfoWrapped(source, NetworkSystem.Instance.ServerTimestamp), "UpdatePlayerCosmetics");
		PlayerCosmeticsSystem.UpdatePlayerCosmetics(player);
	}

	private static bool nullInstance
	{
		get
		{
			return PlayerCosmeticsSystem.instance == null || !PlayerCosmeticsSystem.instance;
		}
	}

	public static bool TempUnlocksEnabled { get; private set; } = false;

	public static string[] TempUnlockCosmeticString { get; private set; } = Array.Empty<string>();

	public static void RegisterCosmeticCallback(int playerID, IUserCosmeticsCallback callback)
	{
		PlayerCosmeticsSystem.userCosmeticCallback[playerID] = callback;
		string cosmetics;
		if (PlayerCosmeticsSystem.userCosmeticsWaiting.TryGetValue(playerID, out cosmetics))
		{
			callback.PendingUpdate = false;
			callback.OnGetUserCosmetics(cosmetics);
			PlayerCosmeticsSystem.userCosmeticsWaiting.Remove(playerID);
		}
	}

	public static void RemoveCosmeticCallback(int playerID)
	{
		if (PlayerCosmeticsSystem.userCosmeticCallback.ContainsKey(playerID))
		{
			PlayerCosmeticsSystem.userCosmeticCallback.Remove(playerID);
		}
	}

	public static void UpdatePlayerCosmetics(NetPlayer player)
	{
		if (player == null || player.IsLocal)
		{
			return;
		}
		PlayerCosmeticsSystem.playersToLookUp.Enqueue(player);
		IUserCosmeticsCallback userCosmeticsCallback;
		if (PlayerCosmeticsSystem.userCosmeticCallback.TryGetValue(player.ActorNumber, out userCosmeticsCallback))
		{
			userCosmeticsCallback.PendingUpdate = true;
		}
		if (!PlayerCosmeticsSystem.nullInstance)
		{
			PlayerCosmeticsSystem.instance.LookUpPlayerCosmetics(true);
		}
	}

	public static void UpdatePlayerCosmetics(List<NetPlayer> players)
	{
		foreach (NetPlayer netPlayer in players)
		{
			if (netPlayer != null && !netPlayer.IsLocal)
			{
				PlayerCosmeticsSystem.playersToLookUp.Enqueue(netPlayer);
				IUserCosmeticsCallback userCosmeticsCallback;
				if (PlayerCosmeticsSystem.userCosmeticCallback.TryGetValue(netPlayer.ActorNumber, out userCosmeticsCallback))
				{
					userCosmeticsCallback.PendingUpdate = true;
				}
			}
		}
		if (!PlayerCosmeticsSystem.nullInstance)
		{
			PlayerCosmeticsSystem.instance.LookUpPlayerCosmetics(false);
		}
	}

	public static void SetRigTryOn(bool inTryon, RigContainer rigRefg)
	{
		VRRig rig = rigRefg.Rig;
		rig.inTryOnRoom = inTryon;
		if (inTryon)
		{
			if (PlayerCosmeticsSystem.sinceLastTryOnEvent.HasElapsed(0.5f, true))
			{
				GorillaTelemetry.PostShopEvent(rig, GTShopEventType.item_try_on, rig.tryOnSet.items);
			}
		}
		else if (rig.isOfflineVRRig)
		{
			rig.tryOnSet.ClearSet(CosmeticsController.instance.nullItem);
			CosmeticsController.instance.ClearCheckout(false);
			CosmeticsController.instance.UpdateShoppingCart();
			CosmeticsController.instance.UpdateWornCosmetics(true);
			rig.myBodyDockPositions.RefreshTransferrableItems();
			return;
		}
		rig.LocalUpdateCosmeticsWithTryon(rig.cosmeticSet, rig.tryOnSet, false);
		rig.myBodyDockPositions.RefreshTransferrableItems();
	}

	public static void SetRigTemporarySpace(bool enteringSpace, RigContainer rigRef, IReadOnlyList<string> cosmeticIds)
	{
		rigRef.Rig.inTempCosmSpace = enteringSpace;
		if (enteringSpace)
		{
			CosmeticsController.CosmeticSet currentWornSet = CosmeticsController.instance.currentWornSet;
			CosmeticsController.instance.tempUnlockedSet.CopyItemsIntoEmpty(currentWornSet);
			PlayerCosmeticsSystem.UnlockTemporaryCosmeticsForPlayer(rigRef, cosmeticIds);
			return;
		}
		PlayerCosmeticsSystem.LockTemporaryCosmeticsForPlayer(rigRef, cosmeticIds);
	}

	public static void UnlockTemporaryCosmeticsForPlayer(RigContainer rigRef)
	{
		PlayerCosmeticsSystem.UnlockTemporaryCosmeticsForPlayer(rigRef, PlayerCosmeticsSystem.TempUnlockCosmeticString);
	}

	public static void UnlockTemporaryCosmeticsForPlayer(RigContainer rigRef, IReadOnlyList<string> cosmeticIds)
	{
		if (cosmeticIds == null)
		{
			Debug.LogError("PlayerCosmeticsSystem failed to unlock temporary cosmetics, cosmetic IDs are null");
			return;
		}
		VRRig rig = rigRef.Rig;
		foreach (string text in cosmeticIds)
		{
			if (rig.TemporaryCosmetics.Add(text) && rig.isOfflineVRRig && !rig.HasCosmetic(text))
			{
				CosmeticsController.instance.AddTempUnlockToWardrobe(text);
			}
		}
		if (rig.isOfflineVRRig)
		{
			CosmeticsController.instance.UpdateWornCosmetics(true);
			return;
		}
		rig.RefreshCosmetics();
	}

	public static void LockTemporaryCosmeticsForPlayer(RigContainer rigRef)
	{
		PlayerCosmeticsSystem.LockTemporaryCosmeticsForPlayer(rigRef, PlayerCosmeticsSystem.TempUnlockCosmeticString);
	}

	public static void LockTemporaryCosmeticsForPlayer(RigContainer rigRef, IReadOnlyList<string> cosmeticIds)
	{
		if (cosmeticIds == null)
		{
			Debug.LogError("PlayerCosmeticsSystem failed to unlock temporary cosmetics, cosmetic IDs are null");
			return;
		}
		VRRig rig = rigRef.Rig;
		foreach (string text in cosmeticIds)
		{
			if (rig.TemporaryCosmetics.Remove(text) && rig.isOfflineVRRig && !rig.HasCosmetic(text))
			{
				CosmeticsController.instance.RemoveTempUnlockFromWardrobe(text);
			}
		}
		if (rig.isOfflineVRRig)
		{
			CosmeticsController.instance.UpdateWornCosmetics(true);
			return;
		}
		rig.RefreshCosmetics();
	}

	internal static void UnlockTemporaryCosmeticsGlobal(IReadOnlyList<string> cosmeticIds)
	{
		int count = cosmeticIds.Count;
		for (int i = 0; i < count; i++)
		{
			PlayerCosmeticsSystem.UnlockTemporaryCosmeticGlobal(cosmeticIds[i]);
		}
	}

	internal static void UnlockTemporaryCosmeticGlobal(string cosmeticId)
	{
		int num = 0;
		if (PlayerCosmeticsSystem.k_tempUnlockedCosmetics.ContainsKey(cosmeticId))
		{
			num = PlayerCosmeticsSystem.k_tempUnlockedCosmetics[cosmeticId];
		}
		num++;
		PlayerCosmeticsSystem.k_tempUnlockedCosmetics[cosmeticId] = num;
	}

	internal static void LockTemporaryCosmeticsGlobal(IReadOnlyList<string> cosmeticIds)
	{
		int count = cosmeticIds.Count;
		for (int i = 0; i < count; i++)
		{
			PlayerCosmeticsSystem.LockTemporaryCosmeticGlobal(cosmeticIds[i]);
		}
	}

	internal static void LockTemporaryCosmeticGlobal(string cosmeticId)
	{
		if (!PlayerCosmeticsSystem.k_tempUnlockedCosmetics.ContainsKey(cosmeticId))
		{
			Debug.LogError("PlayerCosmeticsSystem: Unable to lock cosmetic, ID:-" + cosmeticId + " not found!");
			return;
		}
		int num = PlayerCosmeticsSystem.k_tempUnlockedCosmetics[cosmeticId];
		num--;
		PlayerCosmeticsSystem.k_tempUnlockedCosmetics[cosmeticId] = num;
	}

	public static bool IsTemporaryCosmeticAllowed(VRRig rigRef, string cosmeticId)
	{
		int num;
		return rigRef.TemporaryCosmetics.Contains(cosmeticId) || (PlayerCosmeticsSystem.k_tempUnlockedCosmetics.TryGetValue(cosmeticId, out num) && num > 0);
	}

	public static bool LocalIsTemporaryCosmetic(string cosmeticId)
	{
		VRRig rig = VRRigCache.Instance.localRig.Rig;
		return !rig.HasCosmetic(cosmeticId) && PlayerCosmeticsSystem.IsTemporaryCosmeticAllowed(rig, cosmeticId);
	}

	public static bool LocalPlayerInTemporaryCosmeticSpace()
	{
		return VRRigCache.Instance.localRig.Rig.inTempCosmSpace;
	}

	public static void StaticReset()
	{
		PlayerCosmeticsSystem.playersToLookUp.Clear();
		PlayerCosmeticsSystem.userCosmeticCallback.Clear();
		PlayerCosmeticsSystem.userCosmeticsWaiting.Clear();
		PlayerCosmeticsSystem.playerIDsList.Clear();
		PlayerCosmeticsSystem.playersWaiting.Clear();
	}

	public float playerLookUpCooldown = 3f;

	public float getSharedGroupDataCooldown = 0.1f;

	private float startSearchingTime = float.MinValue;

	private bool isLookingUp;

	private bool isLookingUpNew;

	private string tempCosmetics;

	private NetPlayer playerTemp;

	private RigContainer tempRC;

	private List<string> inventory;

	private static readonly string subscriptionKey = "subscriptions.fan_club";

	private static PlayerCosmeticsSystem instance;

	private static Queue<NetPlayer> playersToLookUp = new Queue<NetPlayer>(20);

	private static Dictionary<int, IUserCosmeticsCallback> userCosmeticCallback = new Dictionary<int, IUserCosmeticsCallback>(20);

	private static Dictionary<int, string> userCosmeticsWaiting = new Dictionary<int, string>(5);

	private static List<string> playerIDsList = new List<string>(20);

	private static List<int> playerActorNumberList = new List<int>(20);

	private static List<int> playersWaiting = new List<int>();

	private static TimeSince sinceLastTryOnEvent = 0f;

	private static readonly Dictionary<string, int> k_tempUnlockedCosmetics = new Dictionary<string, int>(20);

	[Serializable]
	public class PlayFabSubscriptionData
	{
		public string Sku;

		public bool IsActive;
	}
}
