using System;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.XR;

namespace UnityEngine.InputSystem.XR
{
	[AddComponentMenu("XR/Tracked Pose Driver (Input System)")]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.inputsystem@1.14/manual/TrackedInputDevices.html#tracked-pose-driver")]
	[Serializable]
	public class TrackedPoseDriver : MonoBehaviour, ISerializationCallbackReceiver
	{
		public TrackedPoseDriver.TrackingType trackingType
		{
			get
			{
				return this.m_TrackingType;
			}
			set
			{
				this.m_TrackingType = value;
			}
		}

		public TrackedPoseDriver.UpdateType updateType
		{
			get
			{
				return this.m_UpdateType;
			}
			set
			{
				this.m_UpdateType = value;
			}
		}

		public bool ignoreTrackingState
		{
			get
			{
				return this.m_IgnoreTrackingState;
			}
			set
			{
				this.m_IgnoreTrackingState = value;
			}
		}

		public InputActionProperty positionInput
		{
			get
			{
				return this.m_PositionInput;
			}
			set
			{
				if (Application.isPlaying)
				{
					this.UnbindPosition();
				}
				this.m_PositionInput = value;
				if (Application.isPlaying && base.isActiveAndEnabled)
				{
					this.BindPosition();
				}
			}
		}

		public InputActionProperty rotationInput
		{
			get
			{
				return this.m_RotationInput;
			}
			set
			{
				if (Application.isPlaying)
				{
					this.UnbindRotation();
				}
				this.m_RotationInput = value;
				if (Application.isPlaying && base.isActiveAndEnabled)
				{
					this.BindRotation();
				}
			}
		}

		public InputActionProperty trackingStateInput
		{
			get
			{
				return this.m_TrackingStateInput;
			}
			set
			{
				if (Application.isPlaying)
				{
					this.UnbindTrackingState();
				}
				this.m_TrackingStateInput = value;
				if (Application.isPlaying && base.isActiveAndEnabled)
				{
					this.BindTrackingState();
				}
			}
		}

		private void BindActions()
		{
			this.BindPosition();
			this.BindRotation();
			this.BindTrackingState();
		}

		private void UnbindActions()
		{
			this.UnbindPosition();
			this.UnbindRotation();
			this.UnbindTrackingState();
		}

		private void BindPosition()
		{
			if (this.m_PositionBound)
			{
				return;
			}
			InputAction action = this.m_PositionInput.action;
			if (action == null)
			{
				return;
			}
			action.performed += this.OnPositionPerformed;
			action.canceled += this.OnPositionCanceled;
			this.m_PositionBound = true;
			if (this.m_PositionInput.reference == null)
			{
				TrackedPoseDriver.RenameAndEnable(action, base.gameObject.name + " - TPD - Position");
			}
		}

		private void BindRotation()
		{
			if (this.m_RotationBound)
			{
				return;
			}
			InputAction action = this.m_RotationInput.action;
			if (action == null)
			{
				return;
			}
			action.performed += this.OnRotationPerformed;
			action.canceled += this.OnRotationCanceled;
			this.m_RotationBound = true;
			if (this.m_RotationInput.reference == null)
			{
				TrackedPoseDriver.RenameAndEnable(action, base.gameObject.name + " - TPD - Rotation");
			}
		}

		private void BindTrackingState()
		{
			if (this.m_TrackingStateBound)
			{
				return;
			}
			InputAction action = this.m_TrackingStateInput.action;
			if (action == null)
			{
				return;
			}
			action.performed += this.OnTrackingStatePerformed;
			action.canceled += this.OnTrackingStateCanceled;
			this.m_TrackingStateBound = true;
			if (this.m_TrackingStateInput.reference == null)
			{
				TrackedPoseDriver.RenameAndEnable(action, base.gameObject.name + " - TPD - Tracking State");
			}
		}

		private static void RenameAndEnable(InputAction action, string name)
		{
			action.Rename(name);
			action.Enable();
		}

		private void UnbindPosition()
		{
			if (!this.m_PositionBound)
			{
				return;
			}
			InputAction action = this.m_PositionInput.action;
			if (action == null)
			{
				return;
			}
			if (this.m_PositionInput.reference == null)
			{
				action.Disable();
			}
			action.performed -= this.OnPositionPerformed;
			action.canceled -= this.OnPositionCanceled;
			this.m_PositionBound = false;
		}

		private void UnbindRotation()
		{
			if (!this.m_RotationBound)
			{
				return;
			}
			InputAction action = this.m_RotationInput.action;
			if (action == null)
			{
				return;
			}
			if (this.m_RotationInput.reference == null)
			{
				action.Disable();
			}
			action.performed -= this.OnRotationPerformed;
			action.canceled -= this.OnRotationCanceled;
			this.m_RotationBound = false;
		}

		private void UnbindTrackingState()
		{
			if (!this.m_TrackingStateBound)
			{
				return;
			}
			InputAction action = this.m_TrackingStateInput.action;
			if (action == null)
			{
				return;
			}
			if (this.m_TrackingStateInput.reference == null)
			{
				action.Disable();
			}
			action.performed -= this.OnTrackingStatePerformed;
			action.canceled -= this.OnTrackingStateCanceled;
			this.m_TrackingStateBound = false;
		}

		private void OnPositionPerformed(InputAction.CallbackContext context)
		{
			this.m_CurrentPosition = context.ReadValue<Vector3>();
		}

		private void OnPositionCanceled(InputAction.CallbackContext context)
		{
			this.m_CurrentPosition = Vector3.zero;
		}

		private void OnRotationPerformed(InputAction.CallbackContext context)
		{
			this.m_CurrentRotation = context.ReadValue<Quaternion>();
		}

		private void OnRotationCanceled(InputAction.CallbackContext context)
		{
			this.m_CurrentRotation = Quaternion.identity;
		}

		private void OnTrackingStatePerformed(InputAction.CallbackContext context)
		{
			this.m_CurrentTrackingState = (TrackedPoseDriver.TrackingStates)context.ReadValue<int>();
		}

		private void OnTrackingStateCanceled(InputAction.CallbackContext context)
		{
			this.m_CurrentTrackingState = TrackedPoseDriver.TrackingStates.None;
		}

		protected void Reset()
		{
			this.m_PositionInput = new InputActionProperty(new InputAction("Position", InputActionType.Value, null, null, null, "Vector3"));
			this.m_RotationInput = new InputActionProperty(new InputAction("Rotation", InputActionType.Value, null, null, null, "Quaternion"));
			this.m_TrackingStateInput = new InputActionProperty(new InputAction("Tracking State", InputActionType.Value, null, null, null, "Integer"));
		}

		protected virtual void Awake()
		{
			Camera camera;
			if (this.HasStereoCamera(out camera))
			{
				XRDevice.DisableAutoXRCameraTracking(camera, true);
			}
		}

		protected void OnEnable()
		{
			InputSystem.onAfterUpdate += this.UpdateCallback;
			InputSystem.onDeviceChange += this.OnDeviceChanged;
			this.BindActions();
			this.m_IsFirstUpdate = true;
		}

		protected void OnDisable()
		{
			this.UnbindActions();
			InputSystem.onAfterUpdate -= this.UpdateCallback;
			InputSystem.onDeviceChange -= this.OnDeviceChanged;
		}

		protected virtual void OnDestroy()
		{
			Camera camera;
			if (this.HasStereoCamera(out camera))
			{
				XRDevice.DisableAutoXRCameraTracking(camera, false);
			}
		}

		protected void UpdateCallback()
		{
			if (this.m_IsFirstUpdate)
			{
				if (TrackedPoseDriver.HasResolvedControl(this.m_PositionInput.action))
				{
					this.m_CurrentPosition = this.m_PositionInput.action.ReadValue<Vector3>();
				}
				else
				{
					this.m_CurrentPosition = base.transform.localPosition;
				}
				if (TrackedPoseDriver.HasResolvedControl(this.m_RotationInput.action))
				{
					this.m_CurrentRotation = this.m_RotationInput.action.ReadValue<Quaternion>();
				}
				else
				{
					this.m_CurrentRotation = base.transform.localRotation;
				}
				this.ReadTrackingState();
				this.m_IsFirstUpdate = false;
			}
			if (InputState.currentUpdateType == InputUpdateType.BeforeRender)
			{
				this.OnBeforeRender();
				return;
			}
			this.OnUpdate();
		}

		private void OnDeviceChanged(InputDevice inputDevice, InputDeviceChange inputDeviceChange)
		{
			if (this.m_IsFirstUpdate)
			{
				return;
			}
			this.ReadTrackingStateWithoutTrackingAction();
		}

		private void ReadTrackingStateWithoutTrackingAction()
		{
			InputAction action = this.m_TrackingStateInput.action;
			if (action != null && action.m_BindingsCount != 0)
			{
				return;
			}
			bool flag = TrackedPoseDriver.HasResolvedControl(this.m_PositionInput.action);
			bool flag2 = TrackedPoseDriver.HasResolvedControl(this.m_RotationInput.action);
			if (flag && flag2)
			{
				this.m_CurrentTrackingState = (TrackedPoseDriver.TrackingStates.Position | TrackedPoseDriver.TrackingStates.Rotation);
				return;
			}
			if (flag)
			{
				this.m_CurrentTrackingState = TrackedPoseDriver.TrackingStates.Position;
				return;
			}
			if (flag2)
			{
				this.m_CurrentTrackingState = TrackedPoseDriver.TrackingStates.Rotation;
				return;
			}
			this.m_CurrentTrackingState = TrackedPoseDriver.TrackingStates.None;
		}

		private void ReadTrackingState()
		{
			InputAction action = this.m_TrackingStateInput.action;
			if (action != null && !action.enabled)
			{
				this.m_CurrentTrackingState = TrackedPoseDriver.TrackingStates.None;
				return;
			}
			if (TrackedPoseDriver.HasResolvedControl(action))
			{
				this.m_CurrentTrackingState = (TrackedPoseDriver.TrackingStates)action.ReadValue<int>();
				return;
			}
			this.ReadTrackingStateWithoutTrackingAction();
		}

		protected virtual void OnUpdate()
		{
			if (this.m_UpdateType == TrackedPoseDriver.UpdateType.Update || this.m_UpdateType == TrackedPoseDriver.UpdateType.UpdateAndBeforeRender)
			{
				this.PerformUpdate();
			}
		}

		protected virtual void OnBeforeRender()
		{
			if (this.m_UpdateType == TrackedPoseDriver.UpdateType.BeforeRender || this.m_UpdateType == TrackedPoseDriver.UpdateType.UpdateAndBeforeRender)
			{
				this.PerformUpdate();
			}
		}

		protected virtual void PerformUpdate()
		{
			this.SetLocalTransform(this.m_CurrentPosition, this.m_CurrentRotation);
		}

		protected virtual void SetLocalTransform(Vector3 newPosition, Quaternion newRotation)
		{
			bool flag = this.m_IgnoreTrackingState || (this.m_CurrentTrackingState & TrackedPoseDriver.TrackingStates.Position) > TrackedPoseDriver.TrackingStates.None;
			bool flag2 = this.m_IgnoreTrackingState || (this.m_CurrentTrackingState & TrackedPoseDriver.TrackingStates.Rotation) > TrackedPoseDriver.TrackingStates.None;
			if (this.m_TrackingType == TrackedPoseDriver.TrackingType.RotationAndPosition && flag2 && flag)
			{
				base.transform.SetLocalPositionAndRotation(newPosition, newRotation);
				return;
			}
			if (flag2 && (this.m_TrackingType == TrackedPoseDriver.TrackingType.RotationAndPosition || this.m_TrackingType == TrackedPoseDriver.TrackingType.RotationOnly))
			{
				base.transform.localRotation = newRotation;
			}
			if (flag && (this.m_TrackingType == TrackedPoseDriver.TrackingType.RotationAndPosition || this.m_TrackingType == TrackedPoseDriver.TrackingType.PositionOnly))
			{
				base.transform.localPosition = newPosition;
			}
		}

		private bool HasStereoCamera(out Camera cameraComponent)
		{
			return base.TryGetComponent<Camera>(out cameraComponent) && cameraComponent.stereoEnabled;
		}

		private unsafe static bool HasResolvedControl(InputAction action)
		{
			if (action == null)
			{
				return false;
			}
			InputActionMap orCreateActionMap = action.GetOrCreateActionMap();
			orCreateActionMap.ResolveBindingsIfNecessary();
			InputActionState state = orCreateActionMap.m_State;
			if (state == null)
			{
				return false;
			}
			int actionIndexInState = action.m_ActionIndexInState;
			int totalBindingCount = state.totalBindingCount;
			for (int i = 0; i < totalBindingCount; i++)
			{
				ref InputActionState.BindingState ptr = ref state.bindingStates[i];
				if (ptr.actionIndex == actionIndexInState && !ptr.isComposite && ptr.controlCount > 0)
				{
					return true;
				}
			}
			return false;
		}

		public InputAction positionAction
		{
			get
			{
				return this.m_PositionInput.action;
			}
			set
			{
				this.positionInput = new InputActionProperty(value);
			}
		}

		public InputAction rotationAction
		{
			get
			{
				return this.m_RotationInput.action;
			}
			set
			{
				this.rotationInput = new InputActionProperty(value);
			}
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			if (this.m_PositionInput.serializedReference == null && this.m_PositionInput.serializedAction == null && this.m_PositionAction != null)
			{
				this.m_PositionInput = new InputActionProperty(this.m_PositionAction);
			}
			if (this.m_RotationInput.serializedReference == null && this.m_RotationInput.serializedAction == null && this.m_RotationAction != null)
			{
				this.m_RotationInput = new InputActionProperty(this.m_RotationAction);
			}
		}

		[SerializeField]
		[Tooltip("Which Transform properties to update.")]
		private TrackedPoseDriver.TrackingType m_TrackingType;

		[SerializeField]
		[Tooltip("Updates the Transform properties after these phases of Input System event processing.")]
		private TrackedPoseDriver.UpdateType m_UpdateType;

		[SerializeField]
		[Tooltip("Ignore Tracking State and always treat the input pose as valid.")]
		private bool m_IgnoreTrackingState;

		[SerializeField]
		[Tooltip("The input action to read the position value of a tracked device. Must be a Vector 3 control type.")]
		private InputActionProperty m_PositionInput;

		[SerializeField]
		[Tooltip("The input action to read the rotation value of a tracked device. Must be a Quaternion control type.")]
		private InputActionProperty m_RotationInput;

		[SerializeField]
		[Tooltip("The input action to read the tracking state value of a tracked device. Identifies if position and rotation have valid data. Must be an Integer control type.")]
		private InputActionProperty m_TrackingStateInput;

		private Vector3 m_CurrentPosition = Vector3.zero;

		private Quaternion m_CurrentRotation = Quaternion.identity;

		private TrackedPoseDriver.TrackingStates m_CurrentTrackingState = TrackedPoseDriver.TrackingStates.Position | TrackedPoseDriver.TrackingStates.Rotation;

		private bool m_RotationBound;

		private bool m_PositionBound;

		private bool m_TrackingStateBound;

		private bool m_IsFirstUpdate = true;

		[Obsolete]
		[SerializeField]
		[HideInInspector]
		private InputAction m_PositionAction;

		[Obsolete]
		[SerializeField]
		[HideInInspector]
		private InputAction m_RotationAction;

		public enum TrackingType
		{
			RotationAndPosition,
			RotationOnly,
			PositionOnly
		}

		[Flags]
		private enum TrackingStates
		{
			None = 0,
			Position = 1,
			Rotation = 2
		}

		public enum UpdateType
		{
			UpdateAndBeforeRender,
			Update,
			BeforeRender
		}
	}
}
