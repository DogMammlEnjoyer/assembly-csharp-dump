using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Cysharp.Text;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using UnityEngine;
using UnityEngine.XR;

public class GamePlayerLocal : MonoBehaviour, IDelayedExecListener
{
	private void Awake()
	{
		GamePlayerLocal.instance = this;
		this.hands = new GamePlayerLocal.HandData[2];
		this.inputData = new GamePlayerLocal.InputData[2];
		for (int i = 0; i < this.inputData.Length; i++)
		{
			this.inputData[i] = new GamePlayerLocal.InputData(32);
		}
		RoomSystem.JoinedRoomEvent += new Action(this.OnJoinRoom);
		GamePlayerLocal._LoadSnappedPlayerPrefsToCache(this.gamePlayer);
	}

	private void OnJoinRoom()
	{
		this.gamePlayer.MigrateHeldActorNumbers();
	}

	public void OnUpdateInteract()
	{
		for (int i = 0; i < this.inputData.Length; i++)
		{
			this.UpdateInput(i);
		}
		for (int j = 0; j < this.hands.Length; j++)
		{
			this.UpdateHand(this.currGameEntityManager, j);
		}
	}

	private void UpdateInput(int handIndex)
	{
		XRNode xrnode = this.GetXRNode(handIndex);
		GamePlayerLocal.InputDataMotion data = default(GamePlayerLocal.InputDataMotion);
		data.position = ControllerInputPoller.DevicePosition(xrnode);
		data.rotation = ControllerInputPoller.DeviceRotation(xrnode);
		data.velocity = ControllerInputPoller.DeviceVelocity(xrnode);
		data.angVelocity = ControllerInputPoller.DeviceAngularVelocity(xrnode);
		data.time = Time.timeAsDouble;
		this.inputData[handIndex].AddInput(data);
	}

	private void UpdateHand(GameEntityManager emptyHandManager, int handIndex)
	{
		GameEntityManager gameEntityManager;
		if (!this.gamePlayer.GetGrabbedGameEntityIdAndManager(handIndex, out gameEntityManager).IsValid())
		{
			this.UpdateHandEmpty(emptyHandManager, handIndex);
			return;
		}
		this.UpdateHandHolding(gameEntityManager, handIndex);
	}

	public void MigrateToEntityManager(GameEntityManager newEntityManager)
	{
		if (this.currGameEntityManager == newEntityManager)
		{
			return;
		}
		if (newEntityManager.IsAuthority())
		{
			this.gamePlayer.MigrateToEntityManager(newEntityManager);
		}
		this.currGameEntityManager = newEntityManager;
		List<GameEntityCreateData> entityData;
		if (GamePlayerLocal.TryGetMigrationRecoveryList(newEntityManager, out entityData))
		{
			this.currGameEntityManager.RequestMigrationRecovery(entityData);
		}
	}

	public void SetGrabbed(GameEntityId gameBallId, int handIndex)
	{
		GamePlayerLocal.HandData handData = this.hands[handIndex];
		handData.gripPressedTime = (gameBallId.IsValid() ? 0.0 : handData.gripPressedTime);
		this.hands[handIndex] = handData;
		if (handIndex == 0)
		{
			EquipmentInteractor.instance.disableLeftGrab = gameBallId.IsValid();
			return;
		}
		EquipmentInteractor.instance.disableRightGrab = gameBallId.IsValid();
	}

	public void ClearGrabbedIfHeld(GameEntityId gameBallId)
	{
		for (int i = 0; i <= 1; i++)
		{
			if (this.gamePlayer.IsInSlot(i, gameBallId.index))
			{
				this.ClearGrabbed(i);
			}
		}
	}

	public void ClearGrabbed(int handIndex)
	{
		this.SetGrabbed(GameEntityId.Invalid, handIndex);
	}

	private void UpdateStuckState()
	{
		bool disableMovement = false;
		for (int i = 0; i < this.hands.Length; i++)
		{
			if (this.gamePlayer.GetGrabbedGameEntityId(i).IsValid())
			{
				disableMovement = true;
				break;
			}
		}
		GTPlayer.Instance.disableMovement = disableMovement;
	}

	private void UpdateHandEmpty(GameEntityManager gameEntityManager, int handIndex)
	{
		if (this.gamePlayer.IsGrabbingDisabled())
		{
			return;
		}
		if (gameEntityManager == null)
		{
			return;
		}
		GamePlayerLocal.HandData handData = this.hands[handIndex];
		bool flag = GamePlayer.IsLeftHand(handIndex);
		bool flag2 = flag ? (EquipmentInteractor.instance.isLeftGrabbing && ControllerInputPoller.GetGrab(XRNode.LeftHand)) : (EquipmentInteractor.instance.isRightGrabbing && ControllerInputPoller.GetGrab(XRNode.RightHand));
		double timeAsDouble = Time.timeAsDouble;
		if (flag2 && !handData.gripWasHeld)
		{
			handData.gripPressedTime = timeAsDouble;
		}
		double num = timeAsDouble - handData.gripPressedTime;
		handData.gripWasHeld = flag2;
		bool flag3 = flag ? ControllerInputPoller.GetIndexPressed(XRNode.LeftHand) : ControllerInputPoller.GetIndexPressed(XRNode.RightHand);
		if (flag3 && !handData.gripWasHeld)
		{
			handData.triggerPressedTime = timeAsDouble;
		}
		double num2 = timeAsDouble - handData.triggerPressedTime;
		handData.triggerWasHeld = flag3;
		this.hands[handIndex] = handData;
		if (flag2 && num < 0.15000000596046448)
		{
			Transform handTransform = this.gamePlayer.GetHandTransform(handIndex);
			Vector3 position = handTransform.position;
			Vector3 vector = Vector3.Lerp(position, this.GetFingerTransform(handIndex).position, 0.5f);
			Vector3 b = position;
			Quaternion rotation = handTransform.rotation;
			bool flag4;
			GameEntityId gameEntityId = gameEntityManager.TryGrabLocal(position, vector, flag, out b, out flag4);
			if (gameEntityId.IsValid())
			{
				Vector3 a = flag4 ? vector : position;
				Transform transform = handTransform;
				GameEntity gameEntity = gameEntityManager.GetGameEntity(gameEntityId);
				Vector3 position2 = gameEntity.transform.position + (a - b);
				Quaternion rotation2 = gameEntity.transform.rotation;
				GameGrabbable component = gameEntity.GetComponent<GameGrabbable>();
				GameGrab gameGrab;
				if (component && component.GetBestGrabPoint(position, rotation, handIndex, out gameGrab))
				{
					position2 = gameGrab.position;
					rotation2 = gameGrab.rotation;
				}
				Vector3 vector2 = transform.InverseTransformPoint(position2);
				Quaternion quaternion = Quaternion.Inverse(transform.rotation) * rotation2;
				gameEntityManager.RequestGrabEntity(gameEntityId, flag, vector2, quaternion);
				GamePlayerLocal.SetGrabSlotRecoveryData(handIndex, gameEntity.typeId, gameEntity.createData, vector2, quaternion);
			}
		}
		if (flag3 && num2 < 0.15000000596046448)
		{
			Vector3 position3 = this.gamePlayer.GetHandTransform(handIndex).position;
			GameTriggerInteractable gameTriggerInteractable = null;
			float num3 = float.MaxValue;
			int num4 = 0;
			while (num4 < GameTriggerInteractable.LocalInteractableTriggers.Count && !GameTriggerInteractable.LocalInteractableTriggers[num4].triggerInteractionActive)
			{
				if (GameTriggerInteractable.LocalInteractableTriggers[num4].PointWithinInteractableArea(position3))
				{
					float magnitude = (GameTriggerInteractable.LocalInteractableTriggers[num4].interactableCenter.position - position3).magnitude;
					if (magnitude <= num3)
					{
						num3 = magnitude;
						gameTriggerInteractable = GameTriggerInteractable.LocalInteractableTriggers[num4];
					}
				}
				num4++;
			}
			if (gameTriggerInteractable != null)
			{
				gameTriggerInteractable.BeginTriggerInteraction(handIndex);
			}
		}
		if (!flag3)
		{
			this.ClearTriggerInteractables(handIndex);
		}
	}

	private void UpdateHandHolding(GameEntityManager gameEntityManager, int handIndex)
	{
		if (gameEntityManager == null)
		{
			return;
		}
		XRNode xrnode = this.GetXRNode(handIndex);
		bool flag = GamePlayer.IsLeftHand(handIndex);
		if (!(flag ? (EquipmentInteractor.instance.isLeftGrabbing && ControllerInputPoller.GetGrab(XRNode.LeftHand)) : (EquipmentInteractor.instance.isRightGrabbing && ControllerInputPoller.GetGrab(XRNode.RightHand))))
		{
			GameEntityId grabbedGameEntityId = this.gamePlayer.GetGrabbedGameEntityId(handIndex);
			GameEntity gameEntity = gameEntityManager.GetGameEntity(grabbedGameEntityId);
			GamePlayerLocal.SetSlotRecoveryData(handIndex, -1, 0L);
			GameSnappable component = gameEntity.GetComponent<GameSnappable>();
			if (component != null)
			{
				SuperInfectionSnapPoint superInfectionSnapPoint = component.BestSnapPoint();
				if (superInfectionSnapPoint != null)
				{
					gameEntityManager.RequestSnapEntity(grabbedGameEntityId, flag, superInfectionSnapPoint.jointType);
					int slot;
					if (GameSnappable.TryGetJointToSnapIndex(superInfectionSnapPoint.jointType, out slot))
					{
						GamePlayerLocal.SetSlotRecoveryData(slot, gameEntity.typeId, gameEntity.createData);
						GamePlayerLocal.SaveSnapSlotsRateLimited();
					}
					return;
				}
			}
			GameDockable component2 = gameEntity.GetComponent<GameDockable>();
			if (component2 != null)
			{
				GameEntityId gameEntityId = component2.BestDock();
				if (gameEntityId != GameEntityId.Invalid)
				{
					Transform dockablePoint = component2.GetDockablePoint();
					Quaternion quaternion = Quaternion.Inverse(Quaternion.Inverse(component2.transform.rotation) * dockablePoint.rotation);
					Vector3 vector = quaternion * -component2.transform.InverseTransformPoint(dockablePoint.position);
					GameEntity gameEntity2 = gameEntityManager.GetGameEntity(gameEntityId);
					if (gameEntity2 != null)
					{
						GameDock component3 = gameEntity2.GetComponent<GameDock>();
						if (component3 != null)
						{
							Transform dockMarker = component3.dockMarker;
							Vector3 position = dockMarker.transform.TransformPoint(vector);
							vector = gameEntity2.transform.InverseTransformPoint(position);
							Quaternion rhs = dockMarker.rotation * quaternion;
							quaternion = Quaternion.Inverse(gameEntity2.transform.rotation) * rhs;
						}
					}
					gameEntityManager.RequestAttachEntity(grabbedGameEntityId, gameEntityId, 0, vector, quaternion);
					return;
				}
			}
			Vector3 vector2 = ControllerInputPoller.DeviceAngularVelocity(xrnode);
			Quaternion rhs2 = ControllerInputPoller.DeviceRotation(xrnode);
			Quaternion handRotOffset = GTPlayer.Instance.GetHandRotOffset(flag);
			Transform transform = GorillaTagger.Instance.offlineVRRig.transform;
			Quaternion rotation = GTPlayer.Instance.turnParent.transform.rotation;
			GamePlayerLocal.InputData inputData = this.inputData[handIndex];
			Vector3 vector3 = inputData.GetMaxSpeed(0f, 0.05f) * inputData.GetAvgVel(0f, 0.05f).normalized;
			vector3 = rotation * vector3;
			vector3 *= transform.localScale.x;
			vector2 = rotation * rhs2 * handRotOffset * vector2;
			this.gamePlayer.GetGrabbedGameEntityId(handIndex);
			GorillaVelocityTracker bodyVelocityTracker = GTPlayer.Instance.bodyVelocityTracker;
			vector3 += bodyVelocityTracker.GetAverageVelocity(true, 0.05f, false);
			gameEntityManager.RequestThrowEntity(grabbedGameEntityId, flag, GTPlayer.Instance.HeadCenterPosition, vector3, vector2);
		}
		this.ClearTriggerInteractables(handIndex);
	}

	private XRNode GetXRNode(int handIndex)
	{
		if (handIndex != 0)
		{
			return XRNode.RightHand;
		}
		return XRNode.LeftHand;
	}

	private Transform GetFingerTransform(int handIndex)
	{
		GorillaTagger gorillaTagger = GorillaTagger.Instance;
		Transform result;
		if (handIndex != 0)
		{
			if (handIndex != 1)
			{
				result = null;
			}
			else
			{
				result = gorillaTagger.rightHandTriggerCollider.transform;
			}
		}
		else
		{
			result = gorillaTagger.leftHandTriggerCollider.transform;
		}
		return result;
	}

	public Vector3 GetHandVelocity(int handIndex)
	{
		Quaternion rotation = GTPlayer.Instance.turnParent.transform.rotation;
		GamePlayerLocal.InputData inputData = this.inputData[handIndex];
		Vector3 vector = inputData.GetMaxSpeed(0f, 0.05f) * inputData.GetAvgVel(0f, 0.05f).normalized;
		vector = rotation * vector;
		return vector * base.transform.localScale.x;
	}

	public Vector3 GetHandAngularVelocity(int handIndex)
	{
		object obj = (handIndex == 0) ? 4 : 5;
		Quaternion rotation = GTPlayer.Instance.turnParent.transform.rotation;
		object node = obj;
		Quaternion rotation2 = ControllerInputPoller.DeviceRotation(node);
		Vector3 point = ControllerInputPoller.DeviceAngularVelocity(node);
		return rotation * -(Quaternion.Inverse(rotation2) * point);
	}

	public float GetHandSpeed(int handIndex)
	{
		return this.inputData[handIndex].GetMaxSpeed(0f, 0.05f);
	}

	public static bool IsHandHolding(XRNode xrNode)
	{
		return GamePlayerLocal.instance.gamePlayer.IsSlotOccupied((xrNode == XRNode.LeftHand) ? 0 : 1);
	}

	public void PlayCatchFx(bool isLeftHand)
	{
		GorillaTagger.Instance.StartVibration(isLeftHand, GorillaTagger.Instance.tapHapticStrength, 0.1f);
	}

	public void PlayThrowFx(bool isLeftHand)
	{
		GorillaTagger.Instance.StartVibration(isLeftHand, GorillaTagger.Instance.tapHapticStrength * 0.15f, 0.1f);
	}

	public void ClearTriggerInteractables(int handIndex)
	{
		for (int i = 0; i < GameTriggerInteractable.LocalInteractableTriggers.Count; i++)
		{
			if (GameTriggerInteractable.LocalInteractableTriggers[i].triggerInteractionActive && GameTriggerInteractable.LocalInteractableTriggers[i].handIndex == handIndex)
			{
				GameTriggerInteractable.LocalInteractableTriggers[i].EndTriggerInteraction();
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void SetSlotRecoveryData(int slot, int typeId, long createData)
	{
		if (!GamePlayer.IsSlot(slot))
		{
			return;
		}
		GamePlayerLocal.SlotRecoveryData slotRecoveryData = GamePlayerLocal.slotsRecoveryData[slot];
		slotRecoveryData.entityTypeId = typeId;
		slotRecoveryData.createData = createData;
		GamePlayerLocal.slotsRecoveryData[slot] = slotRecoveryData;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void SetGrabSlotRecoveryData(int slot, int typeId, long createData, Vector3 pos, Quaternion rot)
	{
		if (!GamePlayer.IsGrabSlot(slot))
		{
			return;
		}
		GamePlayerLocal.SetSlotRecoveryData(slot, typeId, createData);
		GamePlayerLocal.GrabSlotExtraRecoveryData grabSlotExtraRecoveryData = GamePlayerLocal.grabSlotsExtraRecoveryData[slot];
		grabSlotExtraRecoveryData.pos = pos;
		grabSlotExtraRecoveryData.rot = rot;
		GamePlayerLocal.grabSlotsExtraRecoveryData[slot] = grabSlotExtraRecoveryData;
	}

	internal static void SaveSnapSlotsRateLimited()
	{
		if (GamePlayerLocal.snapSlotsSave_isQueued)
		{
			return;
		}
		if (GamePlayerLocal.snapSlotsSave_lastTime + 2f < Time.unscaledTime)
		{
			GamePlayerLocal._SaveSnapSlotsImmediately();
			return;
		}
		GamePlayerLocal.snapSlotsSave_isQueued = true;
		GTDelayedExec.Add(GamePlayerLocal.instance, 2f, 0);
	}

	void IDelayedExecListener.OnDelayedAction(int contextId)
	{
		GamePlayerLocal._SaveSnapSlotsImmediately();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void _SaveSnapSlotsImmediately()
	{
		GamePlayerLocal.snapSlotsSave_isQueued = false;
		GamePlayerLocal.snapSlotsSave_lastTime = Time.unscaledTime;
		int num = GamePlayerLocal._SnapSlotsSave_GetHash(GamePlayerLocal.slotsRecoveryData);
		if (num == GamePlayerLocal.snapSlotsSave_lastSavedHash)
		{
			return;
		}
		GamePlayerLocal.snapSlotsSave_lastSavedHash = num;
		using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder(true))
		{
			for (int i = 2; i <= 3; i++)
			{
				GamePlayerLocal.SlotRecoveryData slotRecoveryData = GamePlayerLocal.slotsRecoveryData[i];
				if (slotRecoveryData.entityTypeId != -1)
				{
					utf16ValueStringBuilder.Append(i);
					utf16ValueStringBuilder.Append(",");
					utf16ValueStringBuilder.Append(slotRecoveryData.entityTypeId);
					utf16ValueStringBuilder.Append(",");
					utf16ValueStringBuilder.Append(slotRecoveryData.createData);
					utf16ValueStringBuilder.Append("|");
				}
			}
			Debug.Log("POOP - _SaveSnapSlotsImmediately - sb=" + utf16ValueStringBuilder.ToString() + "\n(keywords: gameentitymanager, gameplayer, save)");
			PlayerPrefs.SetString("GT_SnappedItems_V1", utf16ValueStringBuilder.ToString());
			PlayerPrefs.Save();
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void _LoadSnappedPlayerPrefsToCache(GamePlayer gamePlayer)
	{
		for (int i = 0; i < 4; i++)
		{
			GamePlayerLocal.slotsRecoveryData[i] = new GamePlayerLocal.SlotRecoveryData
			{
				entityTypeId = -1,
				createData = 0L
			};
		}
		for (int j = 0; j < 2; j++)
		{
			GamePlayerLocal.grabSlotsExtraRecoveryData[j] = new GamePlayerLocal.GrabSlotExtraRecoveryData
			{
				pos = Vector3.zero,
				rot = Quaternion.identity
			};
		}
		string @string = PlayerPrefs.GetString("GT_SnappedItems_V1");
		string[] array = @string.Split('|', StringSplitOptions.RemoveEmptyEntries);
		Debug.Log("POOP - _LoadSnappedPlayerPrefsToCache - rawString=\"" + @string + "\"\n(keywords: gameentitymanager, gameplayer, save)");
		string[] array2 = array;
		for (int k = 0; k < array2.Length; k++)
		{
			string[] array3 = array2[k].Split(',', StringSplitOptions.None);
			int num;
			int entityTypeId;
			long createData;
			if (array3.Length >= 3 && int.TryParse(array3[0], out num) && num < 4 && GamePlayer.IsSnapSlot(num) && int.TryParse(array3[1], out entityTypeId) && long.TryParse(array3[2], out createData))
			{
				GamePlayerLocal.slotsRecoveryData[num] = new GamePlayerLocal.SlotRecoveryData
				{
					entityTypeId = entityTypeId,
					createData = createData
				};
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int _SnapSlotsSave_GetHash(GamePlayerLocal.SlotRecoveryData[] slotsCache)
	{
		int num = 67466746;
		for (int i = 2; i <= 3; i++)
		{
			GamePlayerLocal.SlotRecoveryData slotRecoveryData = slotsCache[i];
			num = StaticHash.Compute(num, i.GetStaticHash(), slotRecoveryData.entityTypeId.GetStaticHash(), slotRecoveryData.createData.GetStaticHash());
		}
		return num;
	}

	public static bool TryGetMigrationRecoveryList(GameEntityManager newEntityManager, out List<GameEntityCreateData> out_recoveryList)
	{
		out_recoveryList = GamePlayerLocal._migrationRecoveryList;
		GamePlayerLocal._migrationRecoveryList.Clear();
		GamePlayer gamePlayer = GamePlayerLocal.instance.gamePlayer;
		for (int i = 0; i < 4; i++)
		{
			GamePlayerLocal.SlotRecoveryData slotRecoveryData = GamePlayerLocal.slotsRecoveryData[i];
			int entityTypeId = slotRecoveryData.entityTypeId;
			bool flag = entityTypeId != -1;
			GamePlayer.SlotData slotData;
			bool flag2 = gamePlayer.TryGetSlotData(i, out slotData);
			if (flag || flag2)
			{
				bool flag3 = newEntityManager != null && newEntityManager == slotData.entityManager;
				GameEntity gameEntity = flag3 ? newEntityManager.GetGameEntity(slotData.entityId) : null;
				bool flag4 = gameEntity != null;
				int num = flag4 ? gameEntity.typeId : -1;
				bool flag5 = num != -1;
				bool flag6 = entityTypeId == num;
				if (!flag3 || !flag4 || !flag6)
				{
					string message = flag ? "[GamePlayerLocal]  TryGetMigrationRecoveryList: Recovering from mismatch between migrated entities and recovery data." : "[GamePlayerLocal]  ERROR!!!  TryGetMigrationRecoveryList: UNRECOVERABLE mismatch between migrated entities and recovery data.";
					Debug.unityLogger.Log(flag ? LogType.Log : LogType.Error, message);
					if (flag)
					{
						GamePlayerLocal._migrationRecoveryList.Add(new GameEntityCreateData
						{
							entityTypeId = entityTypeId,
							position = (GamePlayer.IsGrabSlot(i) ? GamePlayerLocal.grabSlotsExtraRecoveryData[i].pos : Vector3.zero),
							rotation = (GamePlayer.IsGrabSlot(i) ? GamePlayerLocal.grabSlotsExtraRecoveryData[i].rot : Quaternion.identity),
							createData = slotRecoveryData.createData,
							createdByEntityId = -1,
							slotIndex = i
						});
					}
				}
			}
		}
		return GamePlayerLocal._migrationRecoveryList.Count > 0;
	}

	private const string preLog = "[GamePlayerLocal]  ";

	private const string preErr = "[GamePlayerLocal]  ERROR!!!  ";

	public GamePlayer gamePlayer;

	private GamePlayerLocal.HandData[] hands;

	public const int MAX_INPUT_HISTORY = 32;

	private GamePlayerLocal.InputData[] inputData;

	private const string SNAP_SLOTS_SAVE_KEY = "GT_SnappedItems_V1";

	private const float SNAP_SLOTS_SAVE__INTERVAL = 2f;

	[OnEnterPlay_Set(false)]
	private static bool snapSlotsSave_isQueued;

	[OnEnterPlay_Set(0)]
	private static int snapSlotsSave_lastSavedHash;

	[OnEnterPlay_Set(0)]
	private static int snapSlotsSave_frameWhenQueued;

	[OnEnterPlay_Set(0f)]
	private static float snapSlotsSave_lastTime;

	private static readonly GamePlayerLocal.SlotRecoveryData[] slotsRecoveryData = new GamePlayerLocal.SlotRecoveryData[4];

	private static readonly GamePlayerLocal.GrabSlotExtraRecoveryData[] grabSlotsExtraRecoveryData = new GamePlayerLocal.GrabSlotExtraRecoveryData[2];

	[OnEnterPlay_SetNull]
	public static volatile GamePlayerLocal instance;

	[NonSerialized]
	public GameEntityManager currGameEntityManager;

	private static readonly List<GameEntityCreateData> _migrationRecoveryList = new List<GameEntityCreateData>(4);

	private enum HandGrabState
	{
		Empty,
		Holding
	}

	private struct HandData
	{
		public GamePlayerLocal.HandGrabState grabState;

		public bool gripWasHeld;

		public bool triggerWasHeld;

		public double gripPressedTime;

		public double triggerPressedTime;
	}

	public struct InputDataMotion
	{
		public double time;

		public Vector3 position;

		public Quaternion rotation;

		public Vector3 velocity;

		public Vector3 angVelocity;
	}

	public class InputData
	{
		public InputData(int maxInputs)
		{
			this.maxInputs = maxInputs;
			this.inputMotionHistory = new List<GamePlayerLocal.InputDataMotion>(maxInputs);
		}

		public void AddInput(GamePlayerLocal.InputDataMotion data)
		{
			if (this.inputMotionHistory.Count >= this.maxInputs)
			{
				this.inputMotionHistory.RemoveAt(0);
			}
			this.inputMotionHistory.Add(data);
		}

		public float GetMaxSpeed(float ignoreRecent, float window)
		{
			double timeAsDouble = Time.timeAsDouble;
			double num = timeAsDouble - (double)ignoreRecent - (double)window;
			double num2 = timeAsDouble - (double)ignoreRecent;
			float num3 = 0f;
			for (int i = this.inputMotionHistory.Count - 1; i >= 0; i--)
			{
				GamePlayerLocal.InputDataMotion inputDataMotion = this.inputMotionHistory[i];
				if (inputDataMotion.time <= num2)
				{
					if (inputDataMotion.time < num)
					{
						break;
					}
					float sqrMagnitude = inputDataMotion.velocity.sqrMagnitude;
					if (sqrMagnitude > num3)
					{
						num3 = sqrMagnitude;
					}
				}
			}
			return Mathf.Sqrt(num3);
		}

		public Vector3 GetAvgVel(float ignoreRecent, float window)
		{
			double timeAsDouble = Time.timeAsDouble;
			double num = timeAsDouble - (double)ignoreRecent - (double)window;
			double num2 = timeAsDouble - (double)ignoreRecent;
			Vector3 a = Vector3.zero;
			int num3 = 0;
			for (int i = this.inputMotionHistory.Count - 1; i >= 0; i--)
			{
				GamePlayerLocal.InputDataMotion inputDataMotion = this.inputMotionHistory[i];
				if (inputDataMotion.time <= num2)
				{
					if (inputDataMotion.time < num)
					{
						break;
					}
					a += inputDataMotion.velocity;
					num3++;
				}
			}
			if (num3 == 0)
			{
				return Vector3.zero;
			}
			return a / (float)num3;
		}

		public int maxInputs;

		public List<GamePlayerLocal.InputDataMotion> inputMotionHistory;
	}

	public struct SlotRecoveryData
	{
		public int entityTypeId;

		public long createData;
	}

	public struct GrabSlotExtraRecoveryData
	{
		public Vector3 pos;

		public Quaternion rot;
	}
}
