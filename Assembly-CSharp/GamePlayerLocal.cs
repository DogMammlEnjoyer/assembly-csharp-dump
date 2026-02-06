using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using UnityEngine;
using UnityEngine.XR;

public class GamePlayerLocal : MonoBehaviour
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
	}

	public void SetGrabbed(GameEntityId gameBallId, int handIndex)
	{
		GamePlayerLocal.HandData handData = this.hands[handIndex];
		handData.gripPressedTime = (gameBallId.IsValid() ? 0.0 : handData.gripPressedTime);
		handData.grabbedGameBallId = gameBallId;
		this.hands[handIndex] = handData;
		if (handIndex == 0)
		{
			EquipmentInteractor.instance.disableLeftGrab = gameBallId.IsValid();
			return;
		}
		if (handIndex == 1)
		{
			EquipmentInteractor.instance.disableRightGrab = gameBallId.IsValid();
		}
	}

	public void ClearGrabbedIfHeld(GameEntityId gameBallId)
	{
		for (int i = 0; i < 2; i++)
		{
			if (this.hands[i].grabbedGameBallId == gameBallId)
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
		bool flag = GamePlayerLocal.IsLeftHand(handIndex) ? (EquipmentInteractor.instance.isLeftGrabbing && ControllerInputPoller.GetGrab(XRNode.LeftHand)) : (EquipmentInteractor.instance.isRightGrabbing && ControllerInputPoller.GetGrab(XRNode.RightHand));
		double timeAsDouble = Time.timeAsDouble;
		if (flag && !handData.gripWasHeld)
		{
			handData.gripPressedTime = timeAsDouble;
		}
		double num = timeAsDouble - handData.gripPressedTime;
		handData.gripWasHeld = flag;
		bool flag2 = GamePlayerLocal.IsLeftHand(handIndex) ? ControllerInputPoller.GetIndexPressed(XRNode.LeftHand) : ControllerInputPoller.GetIndexPressed(XRNode.RightHand);
		if (flag2 && !handData.gripWasHeld)
		{
			handData.triggerPressedTime = timeAsDouble;
		}
		double num2 = timeAsDouble - handData.triggerPressedTime;
		handData.triggerWasHeld = flag2;
		this.hands[handIndex] = handData;
		if (flag && num < 0.15000000596046448)
		{
			Transform handTransform = this.GetHandTransform(handIndex);
			Vector3 position = handTransform.position;
			Vector3 b = position;
			Quaternion rotation = handTransform.rotation;
			bool isLeftHand = GamePlayerLocal.IsLeftHand(handIndex);
			GameEntityId gameEntityId = gameEntityManager.TryGrabLocal(position, isLeftHand, out b);
			if (gameEntityId.IsValid())
			{
				Transform handTransform2 = this.GetHandTransform(handIndex);
				GameEntity gameEntity = gameEntityManager.GetGameEntity(gameEntityId);
				Vector3 position2 = gameEntity.transform.position + (position - b);
				Quaternion rotation2 = gameEntity.transform.rotation;
				GameGrabbable component = gameEntity.GetComponent<GameGrabbable>();
				GameGrab gameGrab;
				if (component && component.GetBestGrabPoint(position, rotation, handIndex, out gameGrab))
				{
					position2 = gameGrab.position;
					rotation2 = gameGrab.rotation;
				}
				Vector3 localPosition = handTransform2.InverseTransformPoint(position2);
				Quaternion localRotation = Quaternion.Inverse(handTransform2.rotation) * rotation2;
				gameEntityManager.RequestGrabEntity(gameEntityId, isLeftHand, localPosition, localRotation);
			}
		}
		if (flag2 && num2 < 0.15000000596046448)
		{
			Vector3 position3 = this.GetHandTransform(handIndex).position;
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
		if (!flag2)
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
		if (!(GamePlayerLocal.IsLeftHand(handIndex) ? (EquipmentInteractor.instance.isLeftGrabbing && ControllerInputPoller.GetGrab(XRNode.LeftHand)) : (EquipmentInteractor.instance.isRightGrabbing && ControllerInputPoller.GetGrab(XRNode.RightHand))))
		{
			GameEntityId grabbedGameEntityId = this.gamePlayer.GetGrabbedGameEntityId(handIndex);
			GameEntity gameEntity = gameEntityManager.GetGameEntity(grabbedGameEntityId);
			GameSnappable component = gameEntity.GetComponent<GameSnappable>();
			if (component != null)
			{
				SuperInfectionSnapPoint superInfectionSnapPoint = component.BestSnapPoint();
				if (superInfectionSnapPoint != null)
				{
					gameEntityManager.RequestSnapEntity(grabbedGameEntityId, GamePlayerLocal.IsLeftHand(handIndex), superInfectionSnapPoint.jointType);
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
			Quaternion handRotOffset = GTPlayer.Instance.GetHandRotOffset(GamePlayerLocal.IsLeftHand(handIndex));
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
			gameEntityManager.RequestThrowEntity(grabbedGameEntityId, GamePlayerLocal.IsLeftHand(handIndex), GTPlayer.Instance.HeadCenterPosition, vector3, vector2);
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

	public Transform GetHandTransform(int handIndex)
	{
		return GamePlayer.GetHandTransform(GorillaTagger.Instance.offlineVRRig, handIndex);
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

	public static bool IsLeftHand(int handIndex)
	{
		return handIndex == 0;
	}

	public static int GetHandIndex(bool leftHand)
	{
		if (!leftHand)
		{
			return 1;
		}
		return 0;
	}

	public static bool IsHandHolding(int handIndex)
	{
		GameEntityManager gameEntityManager;
		return GamePlayerLocal.instance.gamePlayer.GetGrabbedGameEntityIdAndManager(handIndex, out gameEntityManager).IsValid();
	}

	public static bool IsHandHolding(XRNode xrNode)
	{
		return GamePlayerLocal.IsHandHolding((xrNode == XRNode.LeftHand) ? 0 : 1);
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

	public GamePlayer gamePlayer;

	private GamePlayerLocal.HandData[] hands;

	public const int MAX_INPUT_HISTORY = 32;

	private GamePlayerLocal.InputData[] inputData;

	[OnEnterPlay_SetNull]
	public static volatile GamePlayerLocal instance;

	[NonSerialized]
	public GameEntityManager currGameEntityManager;

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

		public GameEntityId grabbedGameBallId;
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
}
