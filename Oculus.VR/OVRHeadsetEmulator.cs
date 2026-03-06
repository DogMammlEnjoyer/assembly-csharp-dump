using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class OVRHeadsetEmulator : MonoBehaviour
{
	private void Start()
	{
		this.activateKeyActions = new InputAction[this.activateKeyBindings.Length];
		for (int i = 0; i < this.activateKeyBindings.Length; i++)
		{
			this.activateKeyActions[i] = new InputAction(null, InputActionType.Value, this.activateKeyBindings[i], null, null, null);
			this.activateKeyActions[i].Enable();
		}
		this.pitchKeyActions = new InputAction[this.pitchKeyBindings.Length];
		for (int j = 0; j < this.pitchKeyBindings.Length; j++)
		{
			this.pitchKeyActions[j] = new InputAction(null, InputActionType.Value, this.pitchKeyBindings[j], null, null, null);
			this.pitchKeyActions[j].Enable();
		}
		this.middleMouseButtonAction = new InputAction(null, InputActionType.Button, "<Mouse>/middleButton", null, null, null);
		this.mouseScrollAction = new InputAction(null, InputActionType.Value, "<Mouse>/scroll/y", null, null, null);
		this.mouseMoveAction = new InputAction(null, InputActionType.Value, "<Mouse>/delta", null, null, null);
		this.middleMouseButtonAction.Enable();
		this.mouseScrollAction.Enable();
		this.mouseMoveAction.Enable();
	}

	private void Update()
	{
		if (!this.emulatorHasInitialized)
		{
			if (!OVRManager.OVRManagerinitialized)
			{
				return;
			}
			this.previousCursorLockMode = Cursor.lockState;
			this.manager = OVRManager.instance;
			this.recordedHeadPoseRelativeOffsetTranslation = this.manager.headPoseRelativeOffsetTranslation;
			this.recordedHeadPoseRelativeOffsetRotation = this.manager.headPoseRelativeOffsetRotation;
			this.emulatorHasInitialized = true;
			this.lastFrameEmulationActivated = false;
		}
		bool flag = this.IsEmulationActivated();
		if (flag)
		{
			if (!this.lastFrameEmulationActivated)
			{
				this.previousCursorLockMode = Cursor.lockState;
				Cursor.lockState = CursorLockMode.Locked;
			}
			if (!this.lastFrameEmulationActivated && this.resetHmdPoseOnRelease)
			{
				this.manager.headPoseRelativeOffsetTranslation = this.recordedHeadPoseRelativeOffsetTranslation;
				this.manager.headPoseRelativeOffsetRotation = this.recordedHeadPoseRelativeOffsetRotation;
			}
			bool flag2 = this.middleMouseButtonAction.phase == InputActionPhase.Performed;
			float num = this.mouseScrollAction.ReadValue<float>();
			Vector2 vector = this.mouseMoveAction.ReadValue<Vector2>();
			float num2 = vector.x * 0.05f;
			float num3 = vector.y * 0.05f;
			if (this.resetHmdPoseByMiddleMouseButton && flag2)
			{
				this.manager.headPoseRelativeOffsetTranslation = Vector3.zero;
				this.manager.headPoseRelativeOffsetRotation = Vector3.zero;
			}
			else
			{
				Vector3 headPoseRelativeOffsetTranslation = this.manager.headPoseRelativeOffsetTranslation;
				float num4 = num * 1f;
				headPoseRelativeOffsetTranslation.y += num4;
				this.manager.headPoseRelativeOffsetTranslation = headPoseRelativeOffsetTranslation;
				Vector3 headPoseRelativeOffsetRotation = this.manager.headPoseRelativeOffsetRotation;
				float num5 = headPoseRelativeOffsetRotation.x;
				float num6 = headPoseRelativeOffsetRotation.y;
				float num7 = headPoseRelativeOffsetRotation.z;
				if (this.IsTweakingPitch())
				{
					num7 += num2 * -2f;
				}
				else
				{
					num5 += num3 * 2f;
					num6 += num2 * -2f;
				}
				this.manager.headPoseRelativeOffsetRotation = new Vector3(num5, num6, num7);
			}
			if (!this.hasSentEvent)
			{
				OVRPlugin.SendEvent("headset_emulator", "activated", "");
				this.hasSentEvent = true;
			}
		}
		else if (this.lastFrameEmulationActivated)
		{
			Cursor.lockState = this.previousCursorLockMode;
			this.recordedHeadPoseRelativeOffsetTranslation = this.manager.headPoseRelativeOffsetTranslation;
			this.recordedHeadPoseRelativeOffsetRotation = this.manager.headPoseRelativeOffsetRotation;
			if (this.resetHmdPoseOnRelease)
			{
				this.manager.headPoseRelativeOffsetTranslation = Vector3.zero;
				this.manager.headPoseRelativeOffsetRotation = Vector3.zero;
			}
		}
		this.lastFrameEmulationActivated = flag;
	}

	private bool IsEmulationActivated()
	{
		if (this.opMode == OVRHeadsetEmulator.OpMode.Off)
		{
			return false;
		}
		if (this.opMode == OVRHeadsetEmulator.OpMode.EditorOnly && !Application.isEditor)
		{
			return false;
		}
		InputAction[] array = this.activateKeyActions;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].phase == InputActionPhase.Started)
			{
				return true;
			}
		}
		return false;
	}

	private bool IsTweakingPitch()
	{
		if (!this.IsEmulationActivated())
		{
			return false;
		}
		InputAction[] array = this.pitchKeyActions;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].phase == InputActionPhase.Started)
			{
				return true;
			}
		}
		return false;
	}

	private void OnDestroy()
	{
		if (this.activateKeyActions != null)
		{
			InputAction[] array = this.activateKeyActions;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Disable();
			}
		}
		if (this.pitchKeyActions != null)
		{
			InputAction[] array = this.pitchKeyActions;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Disable();
			}
		}
		InputAction inputAction = this.middleMouseButtonAction;
		if (inputAction != null)
		{
			inputAction.Disable();
		}
		InputAction inputAction2 = this.mouseScrollAction;
		if (inputAction2 != null)
		{
			inputAction2.Disable();
		}
		InputAction inputAction3 = this.mouseMoveAction;
		if (inputAction3 == null)
		{
			return;
		}
		inputAction3.Disable();
	}

	public OVRHeadsetEmulator()
	{
		KeyCode[] array = new KeyCode[3];
		RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.E14C30F1D2D7DD2050884D0B274F21D35706C8B9FF95D4572D452E9A2FB2834F).FieldHandle);
		this.activateKeys = array;
		this.activateKeyBindings = new string[]
		{
			"<Keyboard>/leftCtrl",
			"<Keyboard>/rightCtrl",
			"<Keyboard>/f1"
		};
		KeyCode[] array2 = new KeyCode[3];
		RuntimeHelpers.InitializeArray(array2, fieldof(<PrivateImplementationDetails>.2C9128C9E554362AC7395DBF01FD2A0839D5ABFB886DDE1BCC78BC2819349E7B).FieldHandle);
		this.pitchKeys = array2;
		this.pitchKeyBindings = new string[]
		{
			"<Keyboard>/leftAlt",
			"<Keyboard>/rightAlt",
			"<Keyboard>/f2"
		};
		base..ctor();
	}

	public OVRHeadsetEmulator.OpMode opMode = OVRHeadsetEmulator.OpMode.EditorOnly;

	public bool resetHmdPoseOnRelease = true;

	public bool resetHmdPoseByMiddleMouseButton = true;

	public KeyCode[] activateKeys;

	public string[] activateKeyBindings;

	public KeyCode[] pitchKeys;

	public string[] pitchKeyBindings;

	private InputAction[] activateKeyActions;

	private InputAction[] pitchKeyActions;

	private InputAction middleMouseButtonAction;

	private InputAction mouseScrollAction;

	private InputAction mouseMoveAction;

	private OVRManager manager;

	private const float MOUSE_SCALE_X = -2f;

	private const float MOUSE_SCALE_X_PITCH = -2f;

	private const float MOUSE_SCALE_Y = 2f;

	private const float MOUSE_SCALE_HEIGHT = 1f;

	private const float MAX_ROLL = 85f;

	private bool lastFrameEmulationActivated;

	private Vector3 recordedHeadPoseRelativeOffsetTranslation;

	private Vector3 recordedHeadPoseRelativeOffsetRotation;

	private bool hasSentEvent;

	private bool emulatorHasInitialized;

	private CursorLockMode previousCursorLockMode;

	public enum OpMode
	{
		Off,
		EditorOnly,
		AlwaysOn
	}
}
