using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaTag;
using GorillaTagScripts;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Rendering;

public class VRRigCache : MonoBehaviour
{
	public static VRRigCache Instance { get; private set; }

	public Transform NetworkParent
	{
		get
		{
			return this.networkParent;
		}
	}

	public static IReadOnlyList<RigContainer> ActiveRigContainers
	{
		get
		{
			return VRRigCache.m_activeRigContainers;
		}
	}

	public static IReadOnlyList<VRRig> ActiveRigs
	{
		get
		{
			return VRRigCache.m_activeRigs;
		}
	}

	public static bool isInitialized { get; private set; }

	public static event Action OnActiveRigsChanged;

	public static event Action OnPostInitialize;

	public static event Action OnPostSpawnRig;

	public static event Action<RigContainer> OnRigActivated;

	public static event Action<RigContainer> OnRigDeactivated;

	public static event Action<RigContainer> OnRigNameChanged;

	private void Awake()
	{
		this.InitializeVRRigCache();
		if (this.localRig != null && this.localRig.Rig != null)
		{
			VRRig rig = this.localRig.Rig;
			rig.OnNameChanged = (Action<RigContainer>)Delegate.Combine(rig.OnNameChanged, VRRigCache.OnRigNameChanged);
			if (this.localRig.Rig.bodyRenderer != null)
			{
				this.localRig.Rig.bodyRenderer.SetupAsLocalPlayerBody();
			}
		}
		TickSystemTimer ensureNetworkObjectTimer = this.m_ensureNetworkObjectTimer;
		ensureNetworkObjectTimer.callback = (Action)Delegate.Combine(ensureNetworkObjectTimer.callback, new Action(this.InstantiateNetworkObject));
		NetworkedPlayerColourNotifier.SetLocalRigReference(this.localRig);
	}

	private void OnDestroy()
	{
		if (VRRigCache.Instance == this)
		{
			VRRigCache.Instance = null;
		}
		VRRigCache.isInitialized = false;
		if (this.localRig != null && this.localRig.Rig != null)
		{
			VRRig rig = this.localRig.Rig;
			rig.OnNameChanged = (Action<RigContainer>)Delegate.Remove(rig.OnNameChanged, VRRigCache.OnRigNameChanged);
		}
	}

	public void InitializeVRRigCache()
	{
		if (VRRigCache.isInitialized || ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		if (VRRigCache.Instance != null && VRRigCache.Instance != this)
		{
			Object.Destroy(this);
			return;
		}
		VRRigCache.Instance = this;
		if (this.rigParent == null)
		{
			this.rigParent = base.transform;
		}
		if (this.networkParent == null)
		{
			this.networkParent = base.transform;
		}
		for (int i = 0; i < this.rigAmount; i++)
		{
			RigContainer rigContainer = this.SpawnRig();
			VRRigCache.freeRigs.Enqueue(rigContainer);
			rigContainer.Rig.BuildInitialize();
			rigContainer.Rig.transform.parent = null;
		}
		VRRigCache.m_activeRigContainers.Add(this.localRig);
		VRRigCache.m_activeRigs.Add(this.localRig.Rig);
		VRRigCache.isInitialized = true;
		Action onPostInitialize = VRRigCache.OnPostInitialize;
		if (onPostInitialize != null)
		{
			onPostInitialize();
		}
		Action onPostSpawnRig = VRRigCache.OnPostSpawnRig;
		if (onPostSpawnRig == null)
		{
			return;
		}
		onPostSpawnRig();
	}

	private RigContainer SpawnRig()
	{
		if (this.rigTemplate.activeSelf)
		{
			this.rigTemplate.SetActive(false);
		}
		GameObject gameObject = Object.Instantiate<GameObject>(this.rigTemplate, this.rigParent, false);
		if (gameObject == null)
		{
			return null;
		}
		return gameObject.GetComponent<RigContainer>();
	}

	internal bool TryGetVrrig(Player targetPlayer, out RigContainer playerRig)
	{
		return this.TryGetVrrig(NetworkSystem.Instance.GetPlayer(targetPlayer.ActorNumber), out playerRig);
	}

	internal bool TryGetVrrig(int targetPlayerId, out RigContainer playerRig)
	{
		return this.TryGetVrrig(NetworkSystem.Instance.GetPlayer(targetPlayerId), out playerRig);
	}

	internal bool TryGetVrrig(NetPlayer targetPlayer, out RigContainer playerRig)
	{
		playerRig = null;
		if (ApplicationQuittingState.IsQuitting)
		{
			return false;
		}
		if (targetPlayer == null || targetPlayer.IsNull)
		{
			GTDev.LogError<string>("[GT/VRRigCache]  ERROR!!!  TryGetVrrig: Supplied targetPlayer cannot be null!", null);
			return false;
		}
		if (targetPlayer.IsLocal)
		{
			playerRig = this.localRig;
			return true;
		}
		if (!targetPlayer.InRoom)
		{
			return false;
		}
		if (!VRRigCache.rigsInUse.TryGetValue(targetPlayer, out playerRig))
		{
			if (VRRigCache.freeRigs.Count <= 0)
			{
				return false;
			}
			playerRig = VRRigCache.freeRigs.Dequeue();
			playerRig.Creator = targetPlayer;
			VRRigCache.rigsInUse.Add(targetPlayer, playerRig);
			VRRigCache.m_activeRigContainers.Add(playerRig);
			VRRigCache.m_activeRigs.Add(playerRig.Rig);
			VRRig rig = playerRig.Rig;
			rig.OnNameChanged = (Action<RigContainer>)Delegate.Remove(rig.OnNameChanged, VRRigCache.OnRigNameChanged);
			VRRig rig2 = playerRig.Rig;
			rig2.OnNameChanged = (Action<RigContainer>)Delegate.Combine(rig2.OnNameChanged, VRRigCache.OnRigNameChanged);
			playerRig.gameObject.SetActive(true);
			playerRig.RigEvents.SendPostEnableEvent();
			if (!VRRigCache._isBatchingRigActivations)
			{
				GamePlayer.UpdateStaticLookupCaches();
			}
			Action<RigContainer> onRigActivated = VRRigCache.OnRigActivated;
			if (onRigActivated != null)
			{
				onRigActivated(playerRig);
			}
			if (!VRRigCache._isBatchingRigActivations)
			{
				Action onActiveRigsChanged = VRRigCache.OnActiveRigsChanged;
				if (onActiveRigsChanged != null)
				{
					onActiveRigsChanged();
				}
			}
		}
		return true;
	}

	public void OnPlayerEnteredRoom(NetPlayer newPlayer)
	{
		if (newPlayer.ActorNumber == -1)
		{
			Debug.LogError("LocalPlayer returned, vrrig no correctly initialised");
		}
		RigContainer rigContainer;
		this.TryGetVrrig(newPlayer, out rigContainer);
	}

	public void OnJoinedRoom()
	{
		VRRigCache._isBatchingRigActivations = true;
		foreach (NetPlayer targetPlayer in NetworkSystem.Instance.AllNetPlayers)
		{
			RigContainer rigContainer;
			this.TryGetVrrig(targetPlayer, out rigContainer);
		}
		VRRigCache._isBatchingRigActivations = false;
		this.m_ensureNetworkObjectTimer.Start();
		GamePlayer.UpdateStaticLookupCaches();
		Action onActiveRigsChanged = VRRigCache.OnActiveRigsChanged;
		if (onActiveRigsChanged == null)
		{
			return;
		}
		onActiveRigsChanged();
	}

	public void OnPlayerLeftRoom(NetPlayer leavingPlayer)
	{
		if (leavingPlayer.IsNull)
		{
			Debug.LogError("Leaving players NetPlayer is Null");
			this.CheckForMissingPlayer();
		}
		RigContainer rigContainer;
		if (!VRRigCache.rigsInUse.TryGetValue(leavingPlayer, out rigContainer))
		{
			this.LogError("failed to find player's vrrig who left " + leavingPlayer.UserId);
			return;
		}
		rigContainer.gameObject.Disable();
		VRRig rig = rigContainer.Rig;
		rig.OnNameChanged = (Action<RigContainer>)Delegate.Remove(rig.OnNameChanged, VRRigCache.OnRigNameChanged);
		VRRigCache.freeRigs.Enqueue(rigContainer);
		VRRigCache.rigsInUse.Remove(leavingPlayer);
		VRRigCache.m_activeRigContainers.Remove(rigContainer);
		VRRigCache.m_activeRigs.Remove(rigContainer.Rig);
		GamePlayer.UpdateStaticLookupCaches();
		Action<RigContainer> onRigDeactivated = VRRigCache.OnRigDeactivated;
		if (onRigDeactivated != null)
		{
			onRigDeactivated(rigContainer);
		}
		Action onActiveRigsChanged = VRRigCache.OnActiveRigsChanged;
		if (onActiveRigsChanged == null)
		{
			return;
		}
		onActiveRigsChanged();
	}

	private void CheckForMissingPlayer()
	{
		foreach (KeyValuePair<NetPlayer, RigContainer> keyValuePair in VRRigCache.rigsInUse)
		{
			if (keyValuePair.Key == null || keyValuePair.Value == null)
			{
				Debug.LogError("Somehow null reference in rigsInUse");
			}
			else if (!keyValuePair.Key.InRoom)
			{
				keyValuePair.Value.gameObject.Disable();
				VRRig rig = keyValuePair.Value.Rig;
				rig.OnNameChanged = (Action<RigContainer>)Delegate.Remove(rig.OnNameChanged, VRRigCache.OnRigNameChanged);
				VRRigCache.freeRigs.Enqueue(keyValuePair.Value);
				VRRigCache.rigsInUse.Remove(keyValuePair.Key);
				VRRigCache.m_activeRigContainers.Remove(keyValuePair.Value);
				VRRigCache.m_activeRigs.Remove(keyValuePair.Value.Rig);
				GamePlayer.UpdateStaticLookupCaches();
				Action<RigContainer> onRigDeactivated = VRRigCache.OnRigDeactivated;
				if (onRigDeactivated != null)
				{
					onRigDeactivated(keyValuePair.Value);
				}
				Action onActiveRigsChanged = VRRigCache.OnActiveRigsChanged;
				if (onActiveRigsChanged != null)
				{
					onActiveRigsChanged();
				}
			}
		}
	}

	public void OnLeftRoom()
	{
		this.m_ensureNetworkObjectTimer.Stop();
		Dictionary<NetPlayer, RigContainer> dictionary;
		using (DictionaryPool<NetPlayer, RigContainer>.Get(out dictionary))
		{
			dictionary.EnsureCapacity(VRRigCache.rigsInUse.Count);
			dictionary.Clear();
			foreach (KeyValuePair<NetPlayer, RigContainer> keyValuePair in VRRigCache.rigsInUse)
			{
				NetPlayer netPlayer;
				RigContainer rigContainer;
				keyValuePair.Deconstruct(out netPlayer, out rigContainer);
				NetPlayer key = netPlayer;
				RigContainer value = rigContainer;
				dictionary.Add(key, value);
			}
			foreach (KeyValuePair<NetPlayer, RigContainer> keyValuePair in dictionary)
			{
				NetPlayer netPlayer;
				RigContainer rigContainer;
				keyValuePair.Deconstruct(out netPlayer, out rigContainer);
				NetPlayer key2 = netPlayer;
				RigContainer rigContainer2 = rigContainer;
				if (!(rigContainer2 == null))
				{
					VRRig rig = VRRigCache.rigsInUse[key2].Rig;
					VRRig rig2 = rigContainer2.Rig;
					rig2.OnNameChanged = (Action<RigContainer>)Delegate.Remove(rig2.OnNameChanged, VRRigCache.OnRigNameChanged);
					rigContainer2.gameObject.Disable();
					VRRigCache.rigsInUse.Remove(key2);
					VRRigCache.freeRigs.Enqueue(rigContainer2);
				}
			}
			VRRigCache.m_activeRigContainers.Clear();
			VRRigCache.m_activeRigContainers.Add(this.localRig);
			VRRigCache.m_activeRigs.Clear();
			VRRigCache.m_activeRigs.Add(this.localRig.Rig);
			GamePlayer.UpdateStaticLookupCaches();
			if (VRRigCache.OnRigDeactivated != null)
			{
				foreach (RigContainer obj in dictionary.Values)
				{
					VRRigCache.OnRigDeactivated(obj);
				}
			}
			Action onActiveRigsChanged = VRRigCache.OnActiveRigsChanged;
			if (onActiveRigsChanged != null)
			{
				onActiveRigsChanged();
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal VRRig[] GetAllRigs()
	{
		VRRig[] array = new VRRig[VRRigCache.rigsInUse.Count + VRRigCache.freeRigs.Count];
		int num = 0;
		foreach (RigContainer rigContainer in VRRigCache.rigsInUse.Values)
		{
			array[num] = rigContainer.Rig;
			num++;
		}
		foreach (RigContainer rigContainer2 in VRRigCache.freeRigs)
		{
			array[num] = rigContainer2.Rig;
			num++;
		}
		return array;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void GetAllUsedRigs(List<VRRig> rigs)
	{
		if (rigs == null)
		{
			return;
		}
		foreach (RigContainer rigContainer in VRRigCache.rigsInUse.Values)
		{
			rigs.Add(rigContainer.Rig);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void GetActiveRigs(List<VRRig> rigsListToUpdate)
	{
		if (rigsListToUpdate == null)
		{
			return;
		}
		rigsListToUpdate.Clear();
		if (!VRRigCache.isInitialized)
		{
			return;
		}
		rigsListToUpdate.Add(VRRigCache.Instance.localRig.Rig);
		foreach (RigContainer rigContainer in VRRigCache.rigsInUse.Values)
		{
			rigsListToUpdate.Add(rigContainer.Rig);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal int GetAllRigsHash()
	{
		int num = 0;
		foreach (RigContainer rigContainer in VRRigCache.rigsInUse.Values)
		{
			num += rigContainer.GetInstanceID();
		}
		foreach (RigContainer rigContainer2 in VRRigCache.freeRigs)
		{
			num += rigContainer2.GetInstanceID();
		}
		return num;
	}

	internal void InstantiateNetworkObject()
	{
		if (this.localRig.netView.IsNotNull() || !NetworkSystem.Instance.InRoom)
		{
			return;
		}
		PrefabType prefabType;
		if (!VRRigCache.Instance.GetComponent<PhotonPrefabPool>().networkPrefabs.TryGetValue("Player Network Controller", out prefabType) || prefabType.prefab == null)
		{
			Debug.LogError("OnJoinedRoom: Unable to find player prefab to spawn");
			return;
		}
		GameObject gameObject = GTPlayer.Instance.gameObject;
		Color playerColor = this.localRig.Rig.playerColor;
		VRRigCache.rigRGBData[0] = playerColor.r;
		VRRigCache.rigRGBData[1] = playerColor.g;
		VRRigCache.rigRGBData[2] = playerColor.b;
		NetworkSystem.Instance.NetInstantiate(prefabType.prefab, gameObject.transform.position, gameObject.transform.rotation, false, 0, VRRigCache.rigRGBData, null);
	}

	internal void OnVrrigSerializerSuccesfullySpawned()
	{
		GamePlayer.UpdateStaticLookupCaches();
		Action onActiveRigsChanged = VRRigCache.OnActiveRigsChanged;
		if (onActiveRigsChanged == null)
		{
			return;
		}
		onActiveRigsChanged();
	}

	private void LogInfo(string log)
	{
	}

	private void LogWarning(string log)
	{
	}

	private void LogError(string log)
	{
	}

	private const string preLog = "[GT/VRRigCache] ";

	private const string preErr = "[GT/VRRigCache]  ERROR!!!  ";

	private const string preErrBeta = "[GT/VRRigCache]  ERROR!!!  (beta only log) ";

	private const string preErrEd = "[GT/VRRigCache]  ERROR!!!  (editor only log) ";

	public RigContainer localRig;

	[SerializeField]
	private Transform rigParent;

	[SerializeField]
	private Transform networkParent;

	[SerializeField]
	private GameObject rigTemplate;

	private int rigAmount = 19;

	[SerializeField]
	private TickSystemTimer m_ensureNetworkObjectTimer = new TickSystemTimer(0.1f);

	[OnEnterPlay_Clear]
	private static Queue<RigContainer> freeRigs = new Queue<RigContainer>(19);

	[OnEnterPlay_Clear]
	private static Dictionary<NetPlayer, RigContainer> rigsInUse = new Dictionary<NetPlayer, RigContainer>(19);

	[OnEnterPlay_Clear]
	private static readonly List<RigContainer> m_activeRigContainers = new List<RigContainer>(20);

	[OnEnterPlay_Clear]
	private static readonly List<VRRig> m_activeRigs = new List<VRRig>(20);

	[OnEnterPlay_Set(false)]
	private static bool _isBatchingRigActivations;

	private static object[] rigRGBData = new object[]
	{
		0f,
		0f,
		0f
	};
}
