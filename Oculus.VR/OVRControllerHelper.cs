using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

[HelpURL("https://developer.oculus.com/documentation/unity/controller-animations/")]
public class OVRControllerHelper : MonoBehaviour, OVRInputModule.InputSource
{
	private void Start()
	{
		if (OVRManager.OVRManagerinitialized)
		{
			this.InitializeControllerModels();
		}
	}

	private void OnEnable()
	{
		OVRInputModule.TrackInputSource(this);
		SceneManager.activeSceneChanged += this.OnSceneChanged;
	}

	private void OnDisable()
	{
		OVRInputModule.UntrackInputSource(this);
		SceneManager.activeSceneChanged -= this.OnSceneChanged;
	}

	private void OnSceneChanged(Scene unloading, Scene loading)
	{
		OVRInputModule.TrackInputSource(this);
	}

	private void InitializeControllerModels()
	{
		if (this.m_controllerModelsInitialized)
		{
			return;
		}
		OVRPlugin.SystemHeadset systemHeadsetType = OVRPlugin.GetSystemHeadsetType();
		OVRPlugin.Hand hand = (this.m_controller == OVRInput.Controller.LTouch) ? OVRPlugin.Hand.HandLeft : OVRPlugin.Hand.HandRight;
		OVRPlugin.InteractionProfile interactionProfile = OVRPlugin.GetCurrentInteractionProfile(hand);
		if (OVRPlugin.IsMultimodalHandsControllersSupported())
		{
			OVRPlugin.InteractionProfile currentDetachedInteractionProfile = OVRPlugin.GetCurrentDetachedInteractionProfile(hand);
			if (currentDetachedInteractionProfile != OVRPlugin.InteractionProfile.None)
			{
				interactionProfile = currentDetachedInteractionProfile;
			}
		}
		switch (systemHeadsetType)
		{
		case OVRPlugin.SystemHeadset.Oculus_Quest_2:
			if (interactionProfile == OVRPlugin.InteractionProfile.TouchPro)
			{
				this.activeControllerType = OVRControllerHelper.ControllerType.TouchPro;
				goto IL_E2;
			}
			this.activeControllerType = OVRControllerHelper.ControllerType.Quest2;
			goto IL_E2;
		case OVRPlugin.SystemHeadset.Meta_Quest_Pro:
			this.activeControllerType = OVRControllerHelper.ControllerType.TouchPro;
			goto IL_E2;
		case OVRPlugin.SystemHeadset.Meta_Quest_3:
		case OVRPlugin.SystemHeadset.Meta_Quest_3S:
			break;
		default:
			switch (systemHeadsetType)
			{
			case OVRPlugin.SystemHeadset.Rift_CV1:
				this.activeControllerType = OVRControllerHelper.ControllerType.Rift;
				goto IL_E2;
			case OVRPlugin.SystemHeadset.Oculus_Link_Quest_2:
				if (interactionProfile == OVRPlugin.InteractionProfile.TouchPro)
				{
					this.activeControllerType = OVRControllerHelper.ControllerType.TouchPro;
					goto IL_E2;
				}
				this.activeControllerType = OVRControllerHelper.ControllerType.Quest2;
				goto IL_E2;
			case OVRPlugin.SystemHeadset.Meta_Link_Quest_Pro:
				this.activeControllerType = OVRControllerHelper.ControllerType.TouchPro;
				goto IL_E2;
			case OVRPlugin.SystemHeadset.Meta_Link_Quest_3:
			case OVRPlugin.SystemHeadset.Meta_Link_Quest_3S:
				goto IL_C5;
			}
			this.activeControllerType = OVRControllerHelper.ControllerType.QuestAndRiftS;
			goto IL_E2;
		}
		IL_C5:
		if (interactionProfile == OVRPlugin.InteractionProfile.TouchPro)
		{
			this.activeControllerType = OVRControllerHelper.ControllerType.TouchPro;
		}
		else
		{
			this.activeControllerType = OVRControllerHelper.ControllerType.TouchPlus;
		}
		IL_E2:
		Debug.LogFormat("OVRControllerHelp: Active controller type: {0} for product {1} (headset {2}, hand {3})", new object[]
		{
			this.activeControllerType,
			OVRPlugin.productName,
			systemHeadsetType,
			hand
		});
		this.m_modelOculusTouchQuestAndRiftSLeftController.SetActive(false);
		this.m_modelOculusTouchQuestAndRiftSRightController.SetActive(false);
		this.m_modelOculusTouchRiftLeftController.SetActive(false);
		this.m_modelOculusTouchRiftRightController.SetActive(false);
		this.m_modelOculusTouchQuest2LeftController.SetActive(false);
		this.m_modelOculusTouchQuest2RightController.SetActive(false);
		this.m_modelMetaTouchProLeftController.SetActive(false);
		this.m_modelMetaTouchProRightController.SetActive(false);
		this.m_modelMetaTouchPlusLeftController.SetActive(false);
		this.m_modelMetaTouchPlusRightController.SetActive(false);
		OVRManager.InputFocusAcquired += this.InputFocusAquired;
		OVRManager.InputFocusLost += this.InputFocusLost;
		this.m_controllerModelsInitialized = true;
	}

	private void Update()
	{
		this.m_isActive = false;
		if (!this.m_controllerModelsInitialized)
		{
			if (!OVRManager.OVRManagerinitialized)
			{
				return;
			}
			this.InitializeControllerModels();
		}
		OVRInput.ControllerInHandState controllerIsInHandState = OVRInput.GetControllerIsInHandState((this.m_controller == OVRInput.Controller.LTouch) ? OVRInput.Hand.HandLeft : OVRInput.Hand.HandRight);
		bool flag = OVRInput.IsControllerConnected(this.m_controller);
		if (flag != this.m_prevControllerConnected || !this.m_prevControllerConnectedCached || controllerIsInHandState != this.m_prevControllerInHandState || this.m_hasInputFocus != this.m_hasInputFocusPrev)
		{
			if (this.activeControllerType == OVRControllerHelper.ControllerType.Rift)
			{
				this.m_modelOculusTouchQuestAndRiftSLeftController.SetActive(false);
				this.m_modelOculusTouchQuestAndRiftSRightController.SetActive(false);
				this.m_modelOculusTouchRiftLeftController.SetActive(flag && this.m_controller == OVRInput.Controller.LTouch);
				this.m_modelOculusTouchRiftRightController.SetActive(flag && this.m_controller == OVRInput.Controller.RTouch);
				this.m_modelOculusTouchQuest2LeftController.SetActive(false);
				this.m_modelOculusTouchQuest2RightController.SetActive(false);
				this.m_modelMetaTouchProLeftController.SetActive(false);
				this.m_modelMetaTouchProRightController.SetActive(false);
				this.m_modelMetaTouchPlusLeftController.SetActive(false);
				this.m_modelMetaTouchPlusRightController.SetActive(false);
				this.m_animator = ((this.m_controller == OVRInput.Controller.LTouch) ? this.m_modelOculusTouchRiftLeftController.GetComponent<Animator>() : this.m_modelOculusTouchRiftRightController.GetComponent<Animator>());
				this.m_activeController = ((this.m_controller == OVRInput.Controller.LTouch) ? this.m_modelOculusTouchRiftLeftController : this.m_modelOculusTouchRiftRightController);
			}
			else if (this.activeControllerType == OVRControllerHelper.ControllerType.Quest2)
			{
				this.m_modelOculusTouchQuestAndRiftSLeftController.SetActive(false);
				this.m_modelOculusTouchQuestAndRiftSRightController.SetActive(false);
				this.m_modelOculusTouchRiftLeftController.SetActive(false);
				this.m_modelOculusTouchRiftRightController.SetActive(false);
				this.m_modelOculusTouchQuest2LeftController.SetActive(flag && this.m_controller == OVRInput.Controller.LTouch);
				this.m_modelOculusTouchQuest2RightController.SetActive(flag && this.m_controller == OVRInput.Controller.RTouch);
				this.m_modelMetaTouchProLeftController.SetActive(false);
				this.m_modelMetaTouchProRightController.SetActive(false);
				this.m_modelMetaTouchPlusLeftController.SetActive(false);
				this.m_modelMetaTouchPlusRightController.SetActive(false);
				this.m_animator = ((this.m_controller == OVRInput.Controller.LTouch) ? this.m_modelOculusTouchQuest2LeftController.GetComponent<Animator>() : this.m_modelOculusTouchQuest2RightController.GetComponent<Animator>());
				this.m_activeController = ((this.m_controller == OVRInput.Controller.LTouch) ? this.m_modelOculusTouchQuest2LeftController : this.m_modelOculusTouchQuest2RightController);
			}
			else if (this.activeControllerType == OVRControllerHelper.ControllerType.QuestAndRiftS)
			{
				this.m_modelOculusTouchQuestAndRiftSLeftController.SetActive(flag && this.m_controller == OVRInput.Controller.LTouch);
				this.m_modelOculusTouchQuestAndRiftSRightController.SetActive(flag && this.m_controller == OVRInput.Controller.RTouch);
				this.m_modelOculusTouchRiftLeftController.SetActive(false);
				this.m_modelOculusTouchRiftRightController.SetActive(false);
				this.m_modelOculusTouchQuest2LeftController.SetActive(false);
				this.m_modelOculusTouchQuest2RightController.SetActive(false);
				this.m_modelMetaTouchProLeftController.SetActive(false);
				this.m_modelMetaTouchProRightController.SetActive(false);
				this.m_modelMetaTouchPlusLeftController.SetActive(false);
				this.m_modelMetaTouchPlusRightController.SetActive(false);
				this.m_animator = ((this.m_controller == OVRInput.Controller.LTouch) ? this.m_modelOculusTouchQuestAndRiftSLeftController.GetComponent<Animator>() : this.m_modelOculusTouchQuestAndRiftSRightController.GetComponent<Animator>());
				this.m_activeController = ((this.m_controller == OVRInput.Controller.LTouch) ? this.m_modelOculusTouchQuestAndRiftSLeftController : this.m_modelOculusTouchQuestAndRiftSRightController);
			}
			else if (this.activeControllerType == OVRControllerHelper.ControllerType.TouchPro)
			{
				this.m_modelOculusTouchQuestAndRiftSLeftController.SetActive(false);
				this.m_modelOculusTouchQuestAndRiftSRightController.SetActive(false);
				this.m_modelOculusTouchRiftLeftController.SetActive(false);
				this.m_modelOculusTouchRiftRightController.SetActive(false);
				this.m_modelOculusTouchQuest2LeftController.SetActive(false);
				this.m_modelOculusTouchQuest2RightController.SetActive(false);
				this.m_modelMetaTouchProLeftController.SetActive(flag && this.m_controller == OVRInput.Controller.LTouch);
				this.m_modelMetaTouchProRightController.SetActive(flag && this.m_controller == OVRInput.Controller.RTouch);
				this.m_modelMetaTouchPlusLeftController.SetActive(false);
				this.m_modelMetaTouchPlusRightController.SetActive(false);
				this.m_animator = ((this.m_controller == OVRInput.Controller.LTouch) ? this.m_modelMetaTouchProLeftController.GetComponent<Animator>() : this.m_modelMetaTouchProRightController.GetComponent<Animator>());
				this.m_activeController = ((this.m_controller == OVRInput.Controller.LTouch) ? this.m_modelMetaTouchProLeftController : this.m_modelMetaTouchProRightController);
			}
			else
			{
				this.m_modelOculusTouchQuestAndRiftSLeftController.SetActive(false);
				this.m_modelOculusTouchQuestAndRiftSRightController.SetActive(false);
				this.m_modelOculusTouchRiftLeftController.SetActive(false);
				this.m_modelOculusTouchRiftRightController.SetActive(false);
				this.m_modelOculusTouchQuest2LeftController.SetActive(false);
				this.m_modelOculusTouchQuest2RightController.SetActive(false);
				this.m_modelMetaTouchProLeftController.SetActive(false);
				this.m_modelMetaTouchProRightController.SetActive(false);
				this.m_modelMetaTouchPlusLeftController.SetActive(flag && this.m_controller == OVRInput.Controller.LTouch);
				this.m_modelMetaTouchPlusRightController.SetActive(flag && this.m_controller == OVRInput.Controller.RTouch);
				this.m_animator = ((this.m_controller == OVRInput.Controller.LTouch) ? this.m_modelMetaTouchPlusLeftController.GetComponent<Animator>() : this.m_modelMetaTouchPlusRightController.GetComponent<Animator>());
				this.m_activeController = ((this.m_controller == OVRInput.Controller.LTouch) ? this.m_modelMetaTouchPlusLeftController : this.m_modelMetaTouchPlusRightController);
			}
			this.m_prevControllerConnected = flag;
			this.m_prevControllerConnectedCached = true;
			this.m_prevControllerInHandState = controllerIsInHandState;
			this.m_hasInputFocusPrev = this.m_hasInputFocus;
		}
		bool flag2 = this.m_hasInputFocus && flag;
		switch (this.m_showState)
		{
		case OVRInput.InputDeviceShowState.ControllerInHandOrNoHand:
			if (controllerIsInHandState == OVRInput.ControllerInHandState.ControllerNotInHand)
			{
				flag2 = false;
			}
			break;
		case OVRInput.InputDeviceShowState.ControllerInHand:
			if (controllerIsInHandState != OVRInput.ControllerInHandState.ControllerInHand)
			{
				flag2 = false;
			}
			break;
		case OVRInput.InputDeviceShowState.ControllerNotInHand:
			if (controllerIsInHandState != OVRInput.ControllerInHandState.ControllerNotInHand)
			{
				flag2 = false;
			}
			break;
		case OVRInput.InputDeviceShowState.NoHand:
			if (controllerIsInHandState != OVRInput.ControllerInHandState.NoHand)
			{
				flag2 = false;
			}
			break;
		}
		if (!this.showWhenHandsArePoweredByNaturalControllerPoses && OVRPlugin.IsControllerDrivenHandPosesEnabled() && OVRPlugin.AreControllerDrivenHandPosesNatural())
		{
			flag2 = false;
		}
		this.m_isActive = flag2;
		if (this.m_activeController != null)
		{
			this.m_activeController.SetActive(flag2);
		}
		if (this.RayHelper != null)
		{
			this.RayHelper.gameObject.SetActive(flag2);
		}
		if (this.m_animator != null && this.m_animator.gameObject.activeSelf)
		{
			this.m_animator.SetFloat("Button 1", OVRInput.Get(OVRInput.Button.One, this.m_controller) ? 1f : 0f);
			this.m_animator.SetFloat("Button 2", OVRInput.Get(OVRInput.Button.Two, this.m_controller) ? 1f : 0f);
			this.m_animator.SetFloat("Button 3", OVRInput.Get(OVRInput.Button.Start, this.m_controller) ? 1f : 0f);
			this.m_animator.SetFloat("Joy X", OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, this.m_controller).x);
			this.m_animator.SetFloat("Joy Y", OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, this.m_controller).y);
			this.m_animator.SetFloat("Trigger", OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, this.m_controller));
			this.m_animator.SetFloat("Grip", OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, this.m_controller));
		}
	}

	public void InputFocusAquired()
	{
		this.m_hasInputFocus = true;
	}

	public void InputFocusLost()
	{
		this.m_hasInputFocus = false;
	}

	public bool IsPressed()
	{
		return OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, this.m_controller);
	}

	public bool IsReleased()
	{
		return OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, this.m_controller);
	}

	public Transform GetPointerRayTransform()
	{
		return base.transform;
	}

	public bool IsValid()
	{
		return this != null;
	}

	public bool IsActive()
	{
		return this.m_isActive;
	}

	public OVRPlugin.Hand GetHand()
	{
		if (this.m_controller != OVRInput.Controller.LTouch)
		{
			return OVRPlugin.Hand.HandRight;
		}
		return OVRPlugin.Hand.HandLeft;
	}

	public void UpdatePointerRay(OVRInputRayData rayData)
	{
		if (this.RayHelper)
		{
			rayData.IsActive = OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, this.m_controller);
			rayData.ActivationStrength = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, this.m_controller);
			this.RayHelper.UpdatePointerRay(rayData);
		}
	}

	public GameObject m_modelOculusTouchQuestAndRiftSLeftController;

	public GameObject m_modelOculusTouchQuestAndRiftSRightController;

	public GameObject m_modelOculusTouchRiftLeftController;

	public GameObject m_modelOculusTouchRiftRightController;

	public GameObject m_modelOculusTouchQuest2LeftController;

	public GameObject m_modelOculusTouchQuest2RightController;

	public GameObject m_modelMetaTouchProLeftController;

	public GameObject m_modelMetaTouchProRightController;

	public GameObject m_modelMetaTouchPlusLeftController;

	public GameObject m_modelMetaTouchPlusRightController;

	public OVRInput.Controller m_controller;

	public OVRInput.InputDeviceShowState m_showState = OVRInput.InputDeviceShowState.ControllerInHandOrNoHand;

	public bool showWhenHandsArePoweredByNaturalControllerPoses;

	private Animator m_animator;

	public OVRRayHelper RayHelper;

	private GameObject m_activeController;

	private bool m_controllerModelsInitialized;

	private bool m_hasInputFocus = true;

	private bool m_hasInputFocusPrev;

	private bool m_isActive;

	private OVRControllerHelper.ControllerType activeControllerType = OVRControllerHelper.ControllerType.Rift;

	private bool m_prevControllerConnected;

	private bool m_prevControllerConnectedCached;

	private OVRInput.ControllerInHandState m_prevControllerInHandState;

	private enum ControllerType
	{
		QuestAndRiftS = 1,
		Rift,
		Quest2,
		TouchPro,
		TouchPlus
	}
}
