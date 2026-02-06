using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using UnityEngine;
using UnityEngine.XR;

public class GameBallPlayerLocal : MonoBehaviour
{
	private void Awake()
	{
		GameBallPlayerLocal.instance = this;
		this.hands = new GameBallPlayerLocal.HandData[2];
		this.inputData = new GameBallPlayerLocal.InputData[2];
		for (int i = 0; i < this.inputData.Length; i++)
		{
			this.inputData[i] = new GameBallPlayerLocal.InputData(32);
		}
		Application.quitting += GameBallPlayerLocal._OnApplicationQuit;
	}

	private static void _OnApplicationQuit()
	{
		if (MonkeBallGame.Instance != null)
		{
			MonkeBallGame.Instance.OnPlayerDestroy();
		}
	}

	private void OnApplicationPause(bool pause)
	{
		if (pause && MonkeBallGame.Instance != null)
		{
			MonkeBallGame.Instance.OnPlayerDestroy();
		}
	}

	private void OnDestroy()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		if (MonkeBallGame.Instance != null)
		{
			MonkeBallGame.Instance.OnPlayerDestroy();
		}
	}

	public void OnUpdateInteract()
	{
		if (!ZoneManagement.IsInZone(GTZone.arena))
		{
			return;
		}
		for (int i = 0; i < this.inputData.Length; i++)
		{
			this.UpdateInput(i);
		}
		for (int j = 0; j < this.hands.Length; j++)
		{
			this.UpdateHand(j);
		}
	}

	private void UpdateInput(int handIndex)
	{
		XRNode xrnode = this.GetXRNode(handIndex);
		GameBallPlayerLocal.InputDataMotion data = default(GameBallPlayerLocal.InputDataMotion);
		InputDevice deviceAtXRNode = InputDevices.GetDeviceAtXRNode(xrnode);
		deviceAtXRNode.TryGetFeatureValue(CommonUsages.devicePosition, out data.position);
		deviceAtXRNode.TryGetFeatureValue(CommonUsages.deviceRotation, out data.rotation);
		deviceAtXRNode.TryGetFeatureValue(CommonUsages.deviceVelocity, out data.velocity);
		deviceAtXRNode.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out data.angVelocity);
		data.time = Time.timeAsDouble;
		this.inputData[handIndex].AddInput(data);
	}

	private void UpdateHand(int handIndex)
	{
		if (GameBallManager.Instance == null)
		{
			return;
		}
		if (!this.gamePlayer.GetGameBallId(handIndex).IsValid())
		{
			this.UpdateHandEmpty(handIndex);
			return;
		}
		this.UpdateHandHolding(handIndex);
	}

	public void SetGrabbed(GameBallId gameBallId, int handIndex)
	{
		GameBallPlayerLocal.HandData handData = this.hands[handIndex];
		handData.gripPressedTime = 0.0;
		this.hands[handIndex] = handData;
		this.UpdateStuckState();
	}

	public void ClearGrabbed(int handIndex)
	{
		this.SetGrabbed(GameBallId.Invalid, handIndex);
	}

	public void ClearAllGrabbed()
	{
		for (int i = 0; i < this.hands.Length; i++)
		{
			this.ClearGrabbed(i);
		}
	}

	private void UpdateStuckState()
	{
		bool disableMovement = false;
		for (int i = 0; i < this.hands.Length; i++)
		{
			if (this.gamePlayer.GetGameBallId(i).IsValid())
			{
				disableMovement = true;
				break;
			}
		}
		GTPlayer.Instance.disableMovement = disableMovement;
	}

	private void UpdateHandEmpty(int handIndex)
	{
		GameBallPlayerLocal.HandData handData = this.hands[handIndex];
		bool flag = ControllerInputPoller.GripFloat(this.GetXRNode(handIndex)) > 0.7f;
		double timeAsDouble = Time.timeAsDouble;
		if (flag && !handData.gripWasHeld)
		{
			handData.gripPressedTime = timeAsDouble;
		}
		double num = timeAsDouble - handData.gripPressedTime;
		handData.gripWasHeld = flag;
		this.hands[handIndex] = handData;
		if (flag && num < 0.15000000596046448)
		{
			Vector3 position = this.GetHandTransform(handIndex).position;
			GameBallId gameBallId = GameBallManager.Instance.TryGrabLocal(position, this.gamePlayer.teamId);
			float num2 = 0.15f;
			if (gameBallId.IsValid())
			{
				bool flag2 = GameBallPlayerLocal.IsLeftHand(handIndex);
				BodyDockPositions myBodyDockPositions = GorillaTagger.Instance.offlineVRRig.myBodyDockPositions;
				object obj = flag2 ? myBodyDockPositions.leftHandTransform : myBodyDockPositions.rightHandTransform;
				GameBall gameBall = GameBallManager.Instance.GetGameBall(gameBallId);
				Vector3 position2 = gameBall.transform.position;
				Vector3 vector = gameBall.transform.position - position;
				if (vector.sqrMagnitude > num2 * num2)
				{
					position2 = position + vector.normalized * num2;
				}
				object obj2 = obj;
				Vector3 localPosition = obj2.InverseTransformPoint(position2);
				Quaternion localRotation = Quaternion.Inverse(obj2.rotation) * gameBall.transform.rotation;
				obj2.InverseTransformPoint(gameBall.transform.position);
				GameBallManager.Instance.RequestGrabBall(gameBallId, flag2, localPosition, localRotation);
			}
		}
	}

	private void UpdateHandHolding(int handIndex)
	{
		XRNode xrnode = this.GetXRNode(handIndex);
		if (ControllerInputPoller.GripFloat(xrnode) <= 0.7f)
		{
			InputDevice deviceAtXRNode = InputDevices.GetDeviceAtXRNode(xrnode);
			Vector3 vector;
			deviceAtXRNode.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out vector);
			Quaternion rotation;
			deviceAtXRNode.TryGetFeatureValue(CommonUsages.deviceRotation, out rotation);
			Transform transform = GorillaTagger.Instance.offlineVRRig.transform;
			Quaternion rotation2 = GTPlayer.Instance.turnParent.transform.rotation;
			GameBallPlayerLocal.InputData inputData = this.inputData[handIndex];
			Vector3 vector2 = inputData.GetMaxSpeed(0f, 0.05f) * inputData.GetAvgVel(0f, 0.05f).normalized;
			vector2 = rotation2 * vector2;
			vector2 *= transform.localScale.x;
			vector = rotation2 * -(Quaternion.Inverse(rotation) * vector);
			GameBallId gameBallId = this.gamePlayer.GetGameBallId(handIndex);
			GameBall gameBall = GameBallManager.Instance.GetGameBall(gameBallId);
			if (gameBall == null)
			{
				return;
			}
			if (gameBall.IsLaunched)
			{
				return;
			}
			if (gameBall.disc)
			{
				Vector3 vector3 = gameBall.transform.rotation * gameBall.localDiscUp;
				vector3.Normalize();
				float d = Vector3.Dot(vector3, vector);
				vector = vector3 * d;
				vector *= 1.25f;
				vector2 *= 1.25f;
			}
			else
			{
				vector2 *= 1.5f;
			}
			GorillaVelocityTracker bodyVelocityTracker = GTPlayer.Instance.bodyVelocityTracker;
			vector2 += bodyVelocityTracker.GetAverageVelocity(true, 0.05f, false);
			GameBallManager.Instance.RequestThrowBall(gameBallId, GameBallPlayerLocal.IsLeftHand(handIndex), vector2, vector);
		}
	}

	private XRNode GetXRNode(int handIndex)
	{
		if (handIndex != 0)
		{
			return XRNode.RightHand;
		}
		return XRNode.LeftHand;
	}

	private Transform GetHandTransform(int handIndex)
	{
		BodyDockPositions myBodyDockPositions = GorillaTagger.Instance.offlineVRRig.myBodyDockPositions;
		return ((handIndex == 0) ? myBodyDockPositions.leftHandTransform : myBodyDockPositions.rightHandTransform).parent;
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

	public void PlayCatchFx(bool isLeftHand)
	{
		GorillaTagger.Instance.StartVibration(isLeftHand, GorillaTagger.Instance.tapHapticStrength, 0.1f);
	}

	public void PlayThrowFx(bool isLeftHand)
	{
		GorillaTagger.Instance.StartVibration(isLeftHand, GorillaTagger.Instance.tapHapticStrength * 0.15f, 0.1f);
	}

	public GameBallPlayer gamePlayer;

	private const int MAX_INPUT_HISTORY = 32;

	private GameBallPlayerLocal.HandData[] hands;

	private GameBallPlayerLocal.InputData[] inputData;

	[OnEnterPlay_SetNull]
	public static volatile GameBallPlayerLocal instance;

	private enum HandGrabState
	{
		Empty,
		Holding
	}

	private struct HandData
	{
		public GameBallPlayerLocal.HandGrabState grabState;

		public bool gripWasHeld;

		public double gripPressedTime;

		public GameBallId grabbedGameBallId;
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
			this.inputMotionHistory = new List<GameBallPlayerLocal.InputDataMotion>(maxInputs);
		}

		public void AddInput(GameBallPlayerLocal.InputDataMotion data)
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
				GameBallPlayerLocal.InputDataMotion inputDataMotion = this.inputMotionHistory[i];
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
				GameBallPlayerLocal.InputDataMotion inputDataMotion = this.inputMotionHistory[i];
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

		public List<GameBallPlayerLocal.InputDataMotion> inputMotionHistory;
	}
}
