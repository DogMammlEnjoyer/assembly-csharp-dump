using System;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

internal class ConnectedControllerHandler : MonoBehaviour, IGorillaSliceableSimple
{
	public static ConnectedControllerHandler Instance { get; private set; }

	[SerializeField]
	private bool rightValid
	{
		get
		{
			return this.overrideRightEnable || (ControllerInputPoller.instance.RightHandValid && !this.overriddenControllers.HasFlag(OverrideControllers.RightController));
		}
	}

	[SerializeField]
	private bool leftValid
	{
		get
		{
			return this.overrideLeftEnable || (ControllerInputPoller.instance.LeftHandValid && !this.overriddenControllers.HasFlag(OverrideControllers.LeftController));
		}
	}

	public bool RightValid
	{
		get
		{
			return this.rightValid;
		}
	}

	public bool LeftValid
	{
		get
		{
			return this.leftValid;
		}
	}

	private void Awake()
	{
		if (ConnectedControllerHandler.Instance != null && ConnectedControllerHandler.Instance != this)
		{
			Object.Destroy(this);
			return;
		}
		ConnectedControllerHandler.Instance = this;
		if (this.leftHandFollower == null || this.rightHandFollower == null || this.rightXRController == null || this.leftXRController == null || this.snapTurnController == null)
		{
			base.enabled = false;
			return;
		}
		this.rightControllerList = new List<XRController>();
		this.leftcontrollerList = new List<XRController>();
		this.rightControllerList.Add(this.rightXRController);
		this.leftcontrollerList.Add(this.leftXRController);
		this.UpdateControllerStates();
	}

	private void Start()
	{
		if (this.leftHandFollower == null || this.rightHandFollower == null || this.leftXRController == null || this.rightXRController == null || this.snapTurnController == null)
		{
			return;
		}
		this.playerHandler = GTPlayer.Instance;
		this.rightHandFollower.followTransform = GorillaTagger.Instance.offlineVRRig.transform;
		this.leftHandFollower.followTransform = GorillaTagger.Instance.offlineVRRig.transform;
	}

	public void SetRightHandOffsets(Vector3 positionOffset, Quaternion rotationOffset)
	{
		this.rightHandFollower.positionOffset = positionOffset;
		this.rightHandFollower.rotationOffset = rotationOffset;
	}

	public void SetLeftHandOffsets(Vector3 positionOffset, Quaternion rotationOffset)
	{
		this.leftHandFollower.positionOffset = positionOffset;
		this.leftHandFollower.rotationOffset = rotationOffset;
	}

	public void SetOculusOffsets(bool rightHand = true, bool leftHand = true)
	{
		if (rightHand)
		{
			this.SetRightHandOffsets(this.oculusRightPosOffset, this.oculusRightRotOffset);
		}
		if (leftHand)
		{
			this.SetLeftHandOffsets(this.oculusLeftPosOffset, this.oculusLeftRotOffset);
		}
	}

	private void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this);
	}

	private void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this);
	}

	private void OnDestroy()
	{
		if (ConnectedControllerHandler.Instance != null && ConnectedControllerHandler.Instance == this)
		{
			ConnectedControllerHandler.Instance = null;
		}
	}

	private void LateUpdate()
	{
		if (!this.rightValid)
		{
			this.rightHandFollower.UpdatePositionRotation();
		}
		if (!this.leftValid)
		{
			this.leftHandFollower.UpdatePositionRotation();
		}
	}

	public void SliceUpdate()
	{
		if (this.playerHandler.inOverlay)
		{
			return;
		}
		this.updateControllers = false;
		if (ControllerInputPoller.instance.RightHandValid)
		{
			this.tempRightPos = ControllerInputPoller.DevicePosition(XRNode.RightHand);
			if (this.tempRightPos == this.lastRightPos)
			{
				if (Time.time > this.timeStoppedMovingRight + this.stoppedDurationMinimum && !this.overriddenControllers.HasFlag(OverrideControllers.RightController))
				{
					this.overriddenControllers |= OverrideControllers.RightController;
					this.updateControllers = true;
				}
			}
			else
			{
				this.timeStoppedMovingRight = Time.time;
				if (this.overriddenControllers.HasFlag(OverrideControllers.RightController))
				{
					this.overriddenControllers &= ~OverrideControllers.RightController;
					this.updateControllers = true;
				}
			}
			this.lastRightPos = this.tempRightPos;
		}
		if (ControllerInputPoller.instance.LeftHandValid)
		{
			this.tempLeftPos = ControllerInputPoller.DevicePosition(XRNode.LeftHand);
			if (this.tempLeftPos == this.lastLeftPos)
			{
				if (Time.time > this.timeStoppedMovingLeft + this.stoppedDurationMinimum && !this.overriddenControllers.HasFlag(OverrideControllers.LeftController))
				{
					this.overriddenControllers |= OverrideControllers.LeftController;
					this.updateControllers = true;
				}
			}
			else
			{
				this.timeStoppedMovingLeft = Time.time;
				if (this.overriddenControllers.HasFlag(OverrideControllers.LeftController))
				{
					this.overriddenControllers &= ~OverrideControllers.LeftController;
					this.updateControllers = true;
				}
			}
			this.lastLeftPos = this.tempLeftPos;
		}
		if ((!this.leftXRController.enabled && this.leftValid) || (!this.rightXRController.enabled && this.rightValid))
		{
			this.updateControllers = true;
		}
		if (this.updateControllers)
		{
			this.overrideEnabled = (this.overriddenControllers > OverrideControllers.None);
			this.UpdateControllerStates();
		}
	}

	private void UpdateControllerStates()
	{
		this.leftXRController.enabled = this.leftValid;
		this.rightXRController.enabled = this.rightValid;
		this.AssignSnapturnController();
	}

	private void AssignSnapturnController()
	{
		if (!this.leftValid && this.rightValid)
		{
			this.snapTurnController.controllers = this.rightControllerList;
			return;
		}
		if (!this.rightValid && this.leftValid)
		{
			this.snapTurnController.controllers = this.leftcontrollerList;
			return;
		}
		this.snapTurnController.controllers = this.rightControllerList;
	}

	public bool GetValidForXRNode(XRNode controllerNode)
	{
		bool result;
		if (controllerNode != XRNode.LeftHand)
		{
			result = (controllerNode != XRNode.RightHand || this.rightValid);
		}
		else
		{
			result = this.leftValid;
		}
		return result;
	}

	[SerializeField]
	private HandTransformFollowOffset rightHandFollower;

	[SerializeField]
	private HandTransformFollowOffset leftHandFollower;

	[SerializeField]
	private XRController rightXRController;

	[SerializeField]
	private XRController leftXRController;

	[SerializeField]
	private GorillaSnapTurn snapTurnController;

	private List<XRController> rightControllerList;

	private List<XRController> leftcontrollerList;

	[SerializeField]
	private bool overrideEnabled;

	private bool overrideLeftEnable;

	private bool overrideRightEnable;

	[SerializeField]
	private Vector3 lastRightPos;

	[SerializeField]
	private Vector3 lastLeftPos;

	private Vector3 tempRightPos;

	private Vector3 tempLeftPos;

	private bool updateControllers;

	private GTPlayer playerHandler;

	[Tooltip("The rate at which controllers are checked to be moving, if they not moving, overrides and enables one hand mode")]
	[SerializeField]
	private float stoppedDurationMinimum = 5f;

	[SerializeField]
	private OverrideControllers overriddenControllers;

	private float timeStoppedMovingLeft;

	private float timeStoppedMovingRight;

	public Vector3 oculusRightPosOffset = new Vector3(0f, -0.27f, 0.09f);

	public Quaternion oculusRightRotOffset = Quaternion.Euler(275f, 270f, -5f);

	public Vector3 oculusLeftPosOffset = new Vector3(--0f, -0.27f, 0.09f);

	public Quaternion oculusLeftRotOffset = Quaternion.Euler(275f, 90f, 5f);
}
