using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using GorillaLocomotion;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Rendering;

public class GamePlayer : MonoBehaviour
{
	public bool DidJoinWithItems { get; set; }

	public bool AdditionalDataInitialized { get; set; }

	private void Awake()
	{
		this.handTransforms[0] = this.leftHand;
		this.handTransforms[1] = this.rightHand;
		for (int i = 0; i < this.slots.Length; i++)
		{
			this.slots[i].entityId = GameEntityId.Invalid;
		}
		this.newJoinZoneLimiter = new CallLimiter(10, 10f, 0.5f);
		this.netImpulseLimiter = new CallLimiter(25, 1f, 0.5f);
		this.netGrabLimiter = new CallLimiter(25, 1f, 0.5f);
		this.netThrowLimiter = new CallLimiter(25, 1f, 0.5f);
		this.netStateLimiter = new CallLimiter(25, 1f, 0.5f);
		this.netSnapLimiter = new CallLimiter(25, 1f, 0.5f);
		if (this.snapPointManager == null)
		{
			this.snapPointManager = base.GetComponentInChildren<SuperInfectionSnapPointManager>(true);
			if (this.snapPointManager == null)
			{
				Debug.LogError("[GamePlayer]  ERROR!!!  Snappoints cannot function because the required `SuperInfectionSnapPointManager` could found in children.", this);
			}
		}
	}

	public void Clear()
	{
		for (int i = 0; i <= 1; i++)
		{
			if (this.slots[i].entityId != GameEntityId.Invalid && this.slots[i].entityManager != null)
			{
				this.slots[i].entityManager.RequestThrowEntity(this.slots[i].entityId, GamePlayer.IsLeftHand(i), GTPlayer.Instance.HeadCenterPosition, Vector3.zero, Vector3.zero);
			}
			this.ClearGrabbed(i);
		}
		for (int j = 2; j <= 3; j++)
		{
			if (this.slots[j].entityId != GameEntityId.Invalid && this.slots[j].entityManager != null)
			{
				bool isLeftHand = j != 2;
				GameEntityId entityId = this.slots[j].entityId;
				GameEntityManager entityManager = this.slots[j].entityManager;
				entityManager.RequestGrabEntity(entityId, isLeftHand, Vector3.zero, Quaternion.identity);
				entityManager.RequestThrowEntity(entityId, isLeftHand, GTPlayer.Instance.HeadCenterPosition, Vector3.zero, Vector3.zero);
			}
			this.ClearSlot(j);
		}
	}

	public void ResetData()
	{
		for (int i = 0; i < 4; i++)
		{
			this.ClearSlot(i);
		}
		this.DidJoinWithItems = false;
		this.AdditionalDataInitialized = false;
		this.SetInitializePlayer(false);
	}

	private void OnEnable()
	{
	}

	private void Start()
	{
		GamePlayer.InitializeStaticLookupCaches();
	}

	public void MigrateHeldActorNumbers()
	{
		int actorNumber = this.rig.OwningNetPlayer.ActorNumber;
		for (int i = 0; i < 4; i++)
		{
			if (this.slots[i].entityManager != null)
			{
				GameEntity gameEntity = this.slots[i].entityManager.GetGameEntity(this.slots[i].entityId);
				if (gameEntity != null)
				{
					if (i <= 1)
					{
						gameEntity.MigrateHeldBy(actorNumber);
					}
					else
					{
						gameEntity.MigrateSnappedBy(actorNumber);
					}
				}
			}
		}
	}

	public void SetGrabbed(GameEntityId gameBallId, int handIndex, GameEntityManager gameEntityManager)
	{
		if (handIndex < 0 || handIndex > 1)
		{
			return;
		}
		this.SetSlot(handIndex, gameBallId, gameEntityManager);
	}

	public void SetSnapped(GameEntityId entityId, int slotIndex, GameEntityManager gameEntityManager)
	{
		if (entityId.IsValid())
		{
			this.ClearSnappedIfSnapped(entityId);
			this.ClearGrabbedIfHeld(entityId);
		}
		this.SetSlot(slotIndex, entityId, gameEntityManager);
	}

	public void SetSlot(int slotIndex, GameEntityId entityId, GameEntityManager manager)
	{
		if (slotIndex < 0 || slotIndex >= 4)
		{
			return;
		}
		entityId.IsValid();
		GamePlayer.SlotData slotData = this.slots[slotIndex];
		slotData.entityId = entityId;
		slotData.entityManager = manager;
		this.slots[slotIndex] = slotData;
	}

	public void ClearZone(GameEntityManager manager)
	{
		for (int i = 0; i < 4; i++)
		{
			if (this.slots[i].entityId != GameEntityId.Invalid && this.slots[i].entityManager == manager)
			{
				GameEntity gameEntity = this.slots[i].entityManager.GetGameEntity(this.slots[i].entityId);
				if (gameEntity != null)
				{
					Action onReleased = gameEntity.OnReleased;
					if (onReleased != null)
					{
						onReleased();
					}
				}
				this.ClearSlot(i);
			}
		}
		if (NetworkSystem.Instance.SessionIsPrivate)
		{
			this.DidJoinWithItems = false;
		}
	}

	public void ClearGrabbedIfHeld(GameEntityId gameBallId)
	{
		for (int i = 0; i <= 1; i++)
		{
			if (this.slots[i].entityId == gameBallId)
			{
				this.ClearGrabbed(i);
			}
		}
	}

	public void ClearSnappedIfSnapped(GameEntityId gameBallId)
	{
		for (int i = 2; i <= 3; i++)
		{
			if (this.slots[i].entityId == gameBallId)
			{
				this.ClearSlot(i);
			}
		}
	}

	public void ClearGrabbed(int handIndex)
	{
		this.SetGrabbed(GameEntityId.Invalid, handIndex, null);
	}

	public void ClearSlot(int slotIndex)
	{
		this.SetSlot(slotIndex, GameEntityId.Invalid, null);
	}

	public bool IsGrabbingDisabled()
	{
		return this.grabbingDisabled;
	}

	public void DisableGrabbing(bool disable)
	{
		this.grabbingDisabled = disable;
	}

	internal bool IsSlotOccupied(int slotIndex)
	{
		return this.slots[slotIndex].entityId.index != -1;
	}

	public bool IsHoldingEntity(GameEntityId gameEntityId, bool isLeftHand)
	{
		return this.GetGrabbedGameEntityId(GamePlayer.GetHandIndex(isLeftHand)) == gameEntityId;
	}

	public bool IsHoldingEntity(GameEntityManager gameEntityManager, bool isLeftHand)
	{
		return gameEntityManager.GetGameEntity(this.GetGrabbedGameEntityId(GamePlayer.GetHandIndex(isLeftHand))) != null;
	}

	public bool IsHoldingEntity(GameEntityId gameEntityId)
	{
		return this.GetGrabbedGameEntityId(GamePlayer.GetHandIndex(true)) == gameEntityId || this.GetGrabbedGameEntityId(GamePlayer.GetHandIndex(false)) == gameEntityId;
	}

	public void RequestDropAllSnapped()
	{
		this.Clear();
		this.snapPointManager.DropAllSnappedAuthority();
	}

	public List<GameEntityId> HeldAndSnappedItems(GameEntityManager manager)
	{
		return this.IterateHeldAndSnappedItems(manager).ToList<GameEntityId>();
	}

	public IEnumerable<GameEntityId> IterateHeldAndSnappedItems(GameEntityManager manager)
	{
		int num;
		for (int i = 0; i < 4; i = num)
		{
			if (this.slots[i].entityId != GameEntityId.Invalid && this.slots[i].entityManager == manager)
			{
				yield return this.slots[i].entityId;
			}
			num = i + 1;
		}
		yield break;
	}

	public List<GameEntity> HeldAndSnappedEntities(GameEntityManager ignoreEntitiesInManager = null)
	{
		return this.IterateHeldAndSnappedEntities(ignoreEntitiesInManager).ToList<GameEntity>();
	}

	public IEnumerable<GameEntity> IterateHeldAndSnappedEntities(GameEntityManager ignoreEntitiesInManager = null)
	{
		int num;
		for (int i = 0; i < 4; i = num)
		{
			if (this.slots[i].entityId != GameEntityId.Invalid && this.slots[i].entityManager != null && this.slots[i].entityManager != ignoreEntitiesInManager)
			{
				GameEntity gameEntity = this.slots[i].entityManager.GetGameEntity(this.slots[i].entityId);
				yield return gameEntity;
			}
			num = i + 1;
		}
		yield break;
	}

	public void DeleteGrabbedEntityLocal(int handIndex)
	{
		if (this.slots[handIndex].entityId != GameEntityId.Invalid && this.slots[handIndex].entityManager != null)
		{
			GameEntity gameEntity = this.slots[handIndex].entityManager.GetGameEntity(this.slots[handIndex].entityId);
			if (gameEntity != null)
			{
				if (gameEntity != null)
				{
					Action onReleased = gameEntity.OnReleased;
					if (onReleased != null)
					{
						onReleased();
					}
				}
				this.slots[handIndex].entityManager.DestroyItemLocal(this.slots[handIndex].entityId);
			}
		}
	}

	public int MigrateToEntityManager(GameEntityManager newEntityManager)
	{
		int num = 0;
		for (int i = 0; i < 4; i++)
		{
			GameEntityId entityId = this.slots[i].entityId;
			if (entityId != GameEntityId.Invalid && this.slots[i].entityManager != newEntityManager)
			{
				GameEntity gameEntity = this.slots[i].entityManager.GetGameEntity(entityId);
				if (gameEntity != null && gameEntity.IsValidToMigrate())
				{
					GameEntityId entityId2 = gameEntity.MigrateToEntityManager(newEntityManager);
					GamePlayer.SlotData slotData = this.slots[i];
					slotData.entityManager = newEntityManager;
					slotData.entityId = entityId2;
					this.slots[i] = slotData;
					num++;
				}
			}
		}
		return num;
	}

	internal bool IsInSlot(int slotIndex, int entityIndex)
	{
		return this.slots[slotIndex].entityId.index == entityIndex;
	}

	internal bool TryGetSlotData(int slotIndex, out GamePlayer.SlotData out_slotData)
	{
		out_slotData = this.slots[slotIndex];
		return out_slotData.entityId.index != -1;
	}

	internal bool TryGetSlotEntity(int slotIndex, out GameEntity out_entity)
	{
		GamePlayer.SlotData slotData;
		if (!this.TryGetSlotData(slotIndex, out slotData))
		{
			out_entity = null;
			return false;
		}
		out_entity = slotData.entityManager.GetGameEntity(slotData.entityId);
		return true;
	}

	public GameEntityId GetGameEntityId(bool isLeftHand)
	{
		return this.GetGrabbedGameEntityId(GamePlayer.GetHandIndex(isLeftHand));
	}

	public GameEntityId GetGrabbedGameEntityId(int handIndex)
	{
		if (handIndex < 0 || handIndex > 1)
		{
			return GameEntityId.Invalid;
		}
		return this.slots[handIndex].entityId;
	}

	public GameEntityId GetGrabbedGameEntityIdAndManager(int handIndex, out GameEntityManager manager)
	{
		if (handIndex < 0 || handIndex > 1)
		{
			manager = null;
			return GameEntityId.Invalid;
		}
		manager = this.slots[handIndex].entityManager;
		return this.slots[handIndex].entityId;
	}

	public GameEntity GetGrabbedGameEntity(int handIndex)
	{
		if (handIndex < 0 || handIndex > 1 || this.slots[handIndex].entityManager == null)
		{
			return null;
		}
		return this.slots[handIndex].entityManager.GetGameEntity(this.GetGrabbedGameEntityId(handIndex));
	}

	public int FindSlotIndex(GameEntityId entityId)
	{
		int num = -1;
		int num2 = 0;
		while (num2 < 4 && num == -1)
		{
			num = ((this.slots[num2].entityId == entityId) ? num2 : -1);
			num2++;
		}
		return num;
	}

	public int FindHandIndex(GameEntityId entityId)
	{
		for (int i = 0; i <= 1; i++)
		{
			if (this.slots[i].entityId == entityId)
			{
				return i;
			}
		}
		return -1;
	}

	public int FindSnapIndex(GameEntityId entityId)
	{
		for (int i = 2; i <= 3; i++)
		{
			if (this.slots[i].entityId == entityId)
			{
				return i;
			}
		}
		return -1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsLeftHand(int handIndex)
	{
		return handIndex == 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetHandIndex(bool leftHand)
	{
		if (!leftHand)
		{
			return 1;
		}
		return 0;
	}

	[Obsolete("Method `GamePlayer.TryGetGamePlayer(Player)` is obsolete, use `TryGetGamePlayer(Player, out GamePlayer)` instead.")]
	public static VRRig GetRig(int actorNumber)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(actorNumber);
		if (player == null)
		{
			return null;
		}
		Room currentRoom = PhotonNetwork.CurrentRoom;
		if (currentRoom != null && currentRoom.GetPlayer(actorNumber, false) == null)
		{
			return null;
		}
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			return null;
		}
		return rigContainer.Rig;
	}

	public static GamePlayer GetGamePlayer(Player player)
	{
		GamePlayer result;
		GamePlayer.TryGetGamePlayer(player, out result);
		return result;
	}

	public static bool TryGetGamePlayer(Player player, out GamePlayer gamePlayer)
	{
		if (player == null)
		{
			gamePlayer = null;
			return false;
		}
		return GamePlayer.TryGetGamePlayer(player.ActorNumber, out gamePlayer);
	}

	[Obsolete("Method `GamePlayer.GetGamePlayer(actorNum)` is obsolete, use `TryGetGamePlayer(actorNum, out GamePlayer)` instead.")]
	public static GamePlayer GetGamePlayer(int actorNumber)
	{
		GamePlayer result;
		GamePlayer.TryGetGamePlayer(actorNumber, out result);
		return result;
	}

	public static bool TryGetGamePlayer(int actorNumber, out GamePlayer out_gamePlayer)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(actorNumber);
		RigContainer rigContainer;
		if (player == null || VRRigCache.Instance == null || !VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			out_gamePlayer = null;
			return false;
		}
		return GamePlayer.TryGetGamePlayer(rigContainer.Rig, out out_gamePlayer);
	}

	public static bool TryGetGamePlayer(VRRig rig, out GamePlayer out_gamePlayer)
	{
		if (rig == null)
		{
			out_gamePlayer = null;
			return false;
		}
		out_gamePlayer = rig.GetComponent<GamePlayer>();
		return out_gamePlayer != null;
	}

	public static GamePlayer GetGamePlayer(Collider collider, bool bodyOnly = false)
	{
		Transform transform = collider.transform;
		while (transform != null)
		{
			GamePlayer component = transform.GetComponent<GamePlayer>();
			if (component != null)
			{
				return component;
			}
			if (bodyOnly)
			{
				break;
			}
			transform = transform.parent;
		}
		return null;
	}

	public Transform GetHandTransform(int handIndex)
	{
		if (handIndex < 0 || handIndex > 1)
		{
			return null;
		}
		return this.handTransforms[handIndex];
	}

	public bool TryGetSlotXform(int slotIndex, out Transform slotXform)
	{
		if (GamePlayer.IsGrabSlot(slotIndex))
		{
			slotXform = this.handTransforms[slotIndex];
		}
		else if (GamePlayer.IsSnapSlot(slotIndex))
		{
			SnapJointType snapIndexToJoint = GameSnappable.GetSnapIndexToJoint(slotIndex);
			SuperInfectionSnapPoint superInfectionSnapPoint = (this.snapPointManager != null) ? this.snapPointManager.FindSnapPoint(snapIndexToJoint) : null;
			slotXform = ((superInfectionSnapPoint != null) ? superInfectionSnapPoint.transform : null);
		}
		else
		{
			slotXform = null;
		}
		return slotXform != null;
	}

	public bool IsLocal()
	{
		return GamePlayerLocal.instance != null && GamePlayerLocal.instance.gamePlayer == this;
	}

	public void SerializeNetworkState(BinaryWriter writer, NetPlayer player, GameEntityManager manager)
	{
		for (int i = 0; i < 4; i++)
		{
			if (this.slots[i].entityManager == manager)
			{
				int netIdFromEntityId = manager.GetNetIdFromEntityId(this.slots[i].entityId);
				writer.Write(netIdFromEntityId);
				long value = 0L;
				if (netIdFromEntityId != -1)
				{
					GameEntity gameEntity = manager.GetGameEntity(this.slots[i].entityId);
					if (gameEntity != null)
					{
						value = BitPackUtils.PackHandPosRotForNetwork(gameEntity.transform.localPosition, gameEntity.transform.localRotation);
					}
				}
				writer.Write(value);
			}
			else
			{
				writer.Write(-1);
				writer.Write(0L);
			}
		}
		writer.Write(this.AdditionalDataInitialized);
	}

	public static void DeserializeNetworkState(BinaryReader reader, GamePlayer gamePlayer, GameEntityManager manager)
	{
		for (int i = 0; i < 4; i++)
		{
			int num = reader.ReadInt32();
			long num2 = reader.ReadInt64();
			if (num != -1)
			{
				GameEntityId entityIdFromNetId = manager.GetEntityIdFromNetId(num);
				if (entityIdFromNetId.IsValid())
				{
					GameEntity gameEntity = manager.GetGameEntity(entityIdFromNetId);
					if (num2 != 0L && !(gameEntity == null))
					{
						Vector3 localPosition;
						Quaternion localRotation;
						BitPackUtils.UnpackHandPosRotFromNetwork(num2, out localPosition, out localRotation);
						if (gamePlayer != null && gamePlayer.rig.OwningNetPlayer != null)
						{
							if (GamePlayer.IsGrabSlot(i))
							{
								manager.GrabEntityOnCreate(entityIdFromNetId, GamePlayer.IsLeftHand(i), localPosition, localRotation, gamePlayer.rig.OwningNetPlayer);
							}
							else
							{
								int jointType = -1;
								if (i == 2)
								{
									jointType = 1;
								}
								else if (i == 3)
								{
									jointType = 4;
								}
								manager.SnapEntityOnCreate(entityIdFromNetId, i == 2, localPosition, localRotation, jointType, gamePlayer.rig.OwningNetPlayer);
							}
						}
					}
				}
			}
		}
		bool initializePlayer = reader.ReadBoolean();
		if (gamePlayer != null)
		{
			gamePlayer.SetInitializePlayer(initializePlayer);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool IsSlot(int i)
	{
		return i >= 0 && i < 4;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool IsGrabSlot(int i)
	{
		return i >= 0 && i <= 1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool IsSnapSlot(int i)
	{
		return i >= 2 && i <= 3;
	}

	internal static void InitializeStaticLookupCaches()
	{
		GamePlayer.lookupCache_actorNum_to_gamePlayer = new ValueTuple<int, GamePlayer>[20];
		GamePlayer.lookupCache_rigInstanceId_to_gamePlayer = new ValueTuple<int, GamePlayer>[20];
		if (VRRigCache.isInitialized)
		{
			GamePlayer.UpdateStaticLookupCaches();
		}
	}

	internal static void UpdateStaticLookupCaches()
	{
		if (GamePlayer.lookupCache_actorNum_to_gamePlayer == null)
		{
			return;
		}
		List<VRRig> list;
		using (ListPool<VRRig>.Get(out list))
		{
			if (list.Capacity < 20)
			{
				list.Capacity = 20;
			}
			VRRigCache.Instance.GetActiveRigs(list);
			if (list.Count > GamePlayer.lookupCache_actorNum_to_gamePlayer.Length)
			{
				int newSize = list.Count * 2;
				Array.Resize<ValueTuple<int, GamePlayer>>(ref GamePlayer.lookupCache_actorNum_to_gamePlayer, newSize);
				Array.Resize<ValueTuple<int, GamePlayer>>(ref GamePlayer.lookupCache_rigInstanceId_to_gamePlayer, newSize);
			}
			GamePlayer.staticLookupCachesCount = list.Count;
			if (GamePlayer.staticLookupCachesCount >= 1)
			{
				VRRig vrrig = list[0];
				if (vrrig == null)
				{
					throw new NullReferenceException("[GT/GamePlayer::_VRRigCache_OnActiveRigsChanged]  ERROR!!!  (should never happen) The VRRig at index 0 is expected to be the local rig but is null.");
				}
				int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
				GamePlayer gamePlayer = GamePlayerLocal.instance.gamePlayer;
				GamePlayer.lookupCache_actorNum_to_gamePlayer[0] = new ValueTuple<int, GamePlayer>(actorNumber, gamePlayer);
				GamePlayer.lookupCache_rigInstanceId_to_gamePlayer[0] = new ValueTuple<int, GamePlayer>(vrrig.GetInstanceID(), gamePlayer);
			}
			for (int i = 1; i < GamePlayer.staticLookupCachesCount; i++)
			{
				VRRig vrrig2 = list[i];
				if (vrrig2 == null)
				{
					throw new NullReferenceException("[GT/GamePlayer::_VRRigCache_OnActiveRigsChanged]  ERROR!!!  (should never happen) An entry from `VRRigCache.Instance.GetActiveRigs(activeRigs)` is null but is expected to be ready and all entries not null at this stage.");
				}
				GamePlayer component = vrrig2.GetComponent<GamePlayer>();
				if (component == null)
				{
					throw new NullReferenceException("[GT/GamePlayer::_VRRigCache_OnActiveRigsChanged]  ERROR!!!  (should never happen) Could not get GamePlayer from rig which is expected to be ready at this stage.");
				}
				NetPlayer owningNetPlayer = vrrig2.OwningNetPlayer;
				int item = (owningNetPlayer != null) ? owningNetPlayer.ActorNumber : int.MinValue;
				GamePlayer.lookupCache_actorNum_to_gamePlayer[i] = new ValueTuple<int, GamePlayer>(item, component);
				GamePlayer.lookupCache_rigInstanceId_to_gamePlayer[i] = new ValueTuple<int, GamePlayer>(vrrig2.GetInstanceID(), component);
			}
			for (int j = GamePlayer.staticLookupCachesCount; j < GamePlayer.lookupCache_actorNum_to_gamePlayer.Length; j++)
			{
				GamePlayer.lookupCache_actorNum_to_gamePlayer[j] = new ValueTuple<int, GamePlayer>(0, null);
				GamePlayer.lookupCache_rigInstanceId_to_gamePlayer[j] = new ValueTuple<int, GamePlayer>(0, null);
			}
		}
	}

	public void SetInitializePlayer(bool initialized)
	{
		bool additionalDataInitialized = this.AdditionalDataInitialized;
		this.AdditionalDataInitialized = initialized;
		if (!additionalDataInitialized && this.AdditionalDataInitialized)
		{
			Action onPlayerInitialized = this.OnPlayerInitialized;
			if (onPlayerInitialized == null)
			{
				return;
			}
			onPlayerInitialized();
		}
	}

	private const string preLog = "[GamePlayer]  ";

	private const string preErr = "[GamePlayer]  ERROR!!!  ";

	public VRRig rig;

	public Transform leftHand;

	public Transform rightHand;

	public SuperInfectionSnapPointManager snapPointManager;

	private readonly Transform[] handTransforms = new Transform[2];

	private readonly GamePlayer.SlotData[] slots = new GamePlayer.SlotData[4];

	public const int MAX_HANDS = 2;

	public const int LEFT_HAND = 0;

	public const int RIGHT_HAND = 1;

	public const int GRAB_SLOT_FIRST = 0;

	public const int GRAB_SLOT_LAST = 1;

	public const int SNAP_SLOTS_COUNT = 2;

	public const int SNAP_SLOTS_FIRST = 2;

	public const int SNAP_SLOTS_LAST = 3;

	public const int SNAP_SLOT_HAND_L = 2;

	public const int SNAP_SLOT_HAND_R = 3;

	public const int SLOTS_COUNT = 4;

	public CallLimiter newJoinZoneLimiter;

	public CallLimiter netImpulseLimiter;

	public CallLimiter netGrabLimiter;

	public CallLimiter netThrowLimiter;

	public CallLimiter netStateLimiter;

	public CallLimiter netSnapLimiter;

	public Action OnPlayerInitialized;

	public Action OnPlayerLeftZone;

	private bool grabbingDisabled;

	private const bool _k_MATTO__USE_STATIC_CACHE = false;

	[OnEnterPlay_SetNull]
	private static ValueTuple<int, GamePlayer>[] lookupCache_actorNum_to_gamePlayer;

	[OnEnterPlay_SetNull]
	private static ValueTuple<int, GamePlayer>[] lookupCache_rigInstanceId_to_gamePlayer;

	[OnEnterPlay_Set(0)]
	private static int staticLookupCachesCount;

	public const int INVALID_ACTOR_NUMBER = -2147483648;

	public struct SlotData
	{
		public GameEntityId entityId;

		public GameEntityManager entityManager;
	}
}
