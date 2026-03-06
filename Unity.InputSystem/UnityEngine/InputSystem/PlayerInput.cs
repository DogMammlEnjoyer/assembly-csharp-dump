using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem
{
	[AddComponentMenu("Input/Player Input")]
	[DisallowMultipleComponent]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.inputsystem@1.14/manual/PlayerInput.html")]
	public class PlayerInput : MonoBehaviour
	{
		public bool inputIsActive
		{
			get
			{
				return this.m_InputActive;
			}
		}

		[Obsolete("Use inputIsActive instead.")]
		public bool active
		{
			get
			{
				return this.inputIsActive;
			}
		}

		public int playerIndex
		{
			get
			{
				return this.m_PlayerIndex;
			}
		}

		public int splitScreenIndex
		{
			get
			{
				return this.m_SplitScreenIndex;
			}
		}

		public InputActionAsset actions
		{
			get
			{
				if (!this.m_ActionsInitialized && base.gameObject.activeInHierarchy)
				{
					this.InitializeActions();
				}
				return this.m_Actions;
			}
			set
			{
				if (this.m_Actions == value)
				{
					return;
				}
				if (this.m_Actions != null)
				{
					this.m_Actions.Disable();
					if (this.m_ActionsInitialized)
					{
						this.UninitializeActions();
					}
				}
				this.m_Actions = value;
				if (this.m_Enabled)
				{
					this.ClearCaches();
					this.AssignUserAndDevices();
					this.InitializeActions();
					if (this.m_InputActive)
					{
						this.ActivateInput();
					}
				}
			}
		}

		public string currentControlScheme
		{
			get
			{
				if (!this.m_InputUser.valid)
				{
					return null;
				}
				InputControlScheme? controlScheme = this.m_InputUser.controlScheme;
				if (controlScheme == null)
				{
					return null;
				}
				return controlScheme.GetValueOrDefault().name;
			}
		}

		public string defaultControlScheme
		{
			get
			{
				return this.m_DefaultControlScheme;
			}
			set
			{
				this.m_DefaultControlScheme = value;
			}
		}

		public bool neverAutoSwitchControlSchemes
		{
			get
			{
				return this.m_NeverAutoSwitchControlSchemes;
			}
			set
			{
				if (this.m_NeverAutoSwitchControlSchemes == value)
				{
					return;
				}
				this.m_NeverAutoSwitchControlSchemes = value;
				if (this.m_Enabled)
				{
					if (!value && !this.m_OnUnpairedDeviceUsedHooked)
					{
						this.StartListeningForUnpairedDeviceActivity();
						return;
					}
					if (value && this.m_OnUnpairedDeviceUsedHooked)
					{
						this.StopListeningForUnpairedDeviceActivity();
					}
				}
			}
		}

		public InputActionMap currentActionMap
		{
			get
			{
				return this.m_CurrentActionMap;
			}
			set
			{
				InputActionMap currentActionMap = this.m_CurrentActionMap;
				this.m_CurrentActionMap = null;
				if (currentActionMap != null)
				{
					currentActionMap.Disable();
				}
				this.m_CurrentActionMap = value;
				InputActionMap currentActionMap2 = this.m_CurrentActionMap;
				if (currentActionMap2 == null)
				{
					return;
				}
				currentActionMap2.Enable();
			}
		}

		public string defaultActionMap
		{
			get
			{
				return this.m_DefaultActionMap;
			}
			set
			{
				this.m_DefaultActionMap = value;
			}
		}

		public PlayerNotifications notificationBehavior
		{
			get
			{
				return this.m_NotificationBehavior;
			}
			set
			{
				if (this.m_NotificationBehavior == value)
				{
					return;
				}
				if (this.m_Enabled)
				{
					this.UninitializeActions();
				}
				this.m_NotificationBehavior = value;
				if (this.m_Enabled)
				{
					this.InitializeActions();
				}
			}
		}

		public ReadOnlyArray<PlayerInput.ActionEvent> actionEvents
		{
			get
			{
				return this.m_ActionEvents;
			}
			set
			{
				if (this.m_Enabled)
				{
					this.UninitializeActions();
				}
				this.m_ActionEvents = value.ToArray();
				if (this.m_Enabled)
				{
					this.InitializeActions();
				}
			}
		}

		public PlayerInput.DeviceLostEvent deviceLostEvent
		{
			get
			{
				if (this.m_DeviceLostEvent == null)
				{
					this.m_DeviceLostEvent = new PlayerInput.DeviceLostEvent();
				}
				return this.m_DeviceLostEvent;
			}
		}

		public PlayerInput.DeviceRegainedEvent deviceRegainedEvent
		{
			get
			{
				if (this.m_DeviceRegainedEvent == null)
				{
					this.m_DeviceRegainedEvent = new PlayerInput.DeviceRegainedEvent();
				}
				return this.m_DeviceRegainedEvent;
			}
		}

		public PlayerInput.ControlsChangedEvent controlsChangedEvent
		{
			get
			{
				if (this.m_ControlsChangedEvent == null)
				{
					this.m_ControlsChangedEvent = new PlayerInput.ControlsChangedEvent();
				}
				return this.m_ControlsChangedEvent;
			}
		}

		public event Action<InputAction.CallbackContext> onActionTriggered
		{
			add
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.m_ActionTriggeredCallbacks.AddCallback(value);
			}
			remove
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.m_ActionTriggeredCallbacks.RemoveCallback(value);
			}
		}

		public event Action<PlayerInput> onDeviceLost
		{
			add
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.m_DeviceLostCallbacks.AddCallback(value);
			}
			remove
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.m_DeviceLostCallbacks.RemoveCallback(value);
			}
		}

		public event Action<PlayerInput> onDeviceRegained
		{
			add
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.m_DeviceRegainedCallbacks.AddCallback(value);
			}
			remove
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.m_DeviceRegainedCallbacks.RemoveCallback(value);
			}
		}

		public event Action<PlayerInput> onControlsChanged
		{
			add
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.m_ControlsChangedCallbacks.AddCallback(value);
			}
			remove
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.m_ControlsChangedCallbacks.RemoveCallback(value);
			}
		}

		public Camera camera
		{
			get
			{
				return this.m_Camera;
			}
			set
			{
				this.m_Camera = value;
			}
		}

		public InputSystemUIInputModule uiInputModule
		{
			get
			{
				return this.m_UIInputModule;
			}
			set
			{
				if (this.m_UIInputModule == value)
				{
					return;
				}
				if (this.m_UIInputModule != null && this.m_UIInputModule.actionsAsset == this.m_Actions)
				{
					this.m_UIInputModule.actionsAsset = null;
				}
				this.m_UIInputModule = value;
				if (this.m_UIInputModule != null && this.m_Actions != null)
				{
					this.m_UIInputModule.actionsAsset = this.m_Actions;
				}
			}
		}

		public InputUser user
		{
			get
			{
				return this.m_InputUser;
			}
		}

		public ReadOnlyArray<InputDevice> devices
		{
			get
			{
				if (!this.m_InputUser.valid)
				{
					return default(ReadOnlyArray<InputDevice>);
				}
				return this.m_InputUser.pairedDevices;
			}
		}

		public bool hasMissingRequiredDevices
		{
			get
			{
				return this.user.valid && this.user.hasMissingRequiredDevices;
			}
		}

		public static ReadOnlyArray<PlayerInput> all
		{
			get
			{
				return new ReadOnlyArray<PlayerInput>(PlayerInput.s_AllActivePlayers, 0, PlayerInput.s_AllActivePlayersCount);
			}
		}

		public static bool isSinglePlayer
		{
			get
			{
				return PlayerInput.s_AllActivePlayersCount <= 1 && (PlayerInputManager.instance == null || !PlayerInputManager.instance.joiningEnabled);
			}
		}

		public TDevice GetDevice<TDevice>() where TDevice : InputDevice
		{
			foreach (InputDevice inputDevice in this.devices)
			{
				TDevice tdevice = inputDevice as TDevice;
				if (tdevice != null)
				{
					return tdevice;
				}
			}
			return default(TDevice);
		}

		public void ActivateInput()
		{
			this.UpdateDelegates();
			this.m_InputActive = true;
			if (this.m_CurrentActionMap == null && this.m_Actions != null && !string.IsNullOrEmpty(this.m_DefaultActionMap))
			{
				this.SwitchCurrentActionMap(this.m_DefaultActionMap);
				return;
			}
			InputActionMap currentActionMap = this.m_CurrentActionMap;
			if (currentActionMap == null)
			{
				return;
			}
			currentActionMap.Enable();
		}

		private void UpdateDelegates()
		{
			if (this.m_Actions == null)
			{
				this.m_AllMapsHashCode = 0;
				return;
			}
			int num = 0;
			foreach (InputActionMap inputActionMap in this.m_Actions.actionMaps)
			{
				num ^= inputActionMap.GetHashCode();
			}
			if (this.m_AllMapsHashCode != num)
			{
				if (this.m_NotificationBehavior != PlayerNotifications.InvokeUnityEvents)
				{
					this.InstallOnActionTriggeredHook();
				}
				this.CacheMessageNames();
				this.m_AllMapsHashCode = num;
			}
		}

		public void DeactivateInput()
		{
			InputActionMap currentActionMap = this.m_CurrentActionMap;
			if (currentActionMap != null)
			{
				currentActionMap.Disable();
			}
			this.m_InputActive = false;
		}

		[Obsolete("Use DeactivateInput instead.")]
		public void PassivateInput()
		{
			this.DeactivateInput();
		}

		public bool SwitchCurrentControlScheme(params InputDevice[] devices)
		{
			if (devices == null)
			{
				throw new ArgumentNullException("devices");
			}
			if (this.actions == null)
			{
				throw new InvalidOperationException("Must set actions on PlayerInput in order to be able to switch control schemes");
			}
			InputControlScheme? inputControlScheme = InputControlScheme.FindControlSchemeForDevices<InputDevice[], ReadOnlyArray<InputControlScheme>>(devices, this.actions.controlSchemes, null, false);
			if (inputControlScheme == null)
			{
				return false;
			}
			InputControlScheme value = inputControlScheme.Value;
			this.SwitchControlSchemeInternal(ref value, devices);
			return true;
		}

		public void SwitchCurrentControlScheme(string controlScheme, params InputDevice[] devices)
		{
			if (string.IsNullOrEmpty(controlScheme))
			{
				throw new ArgumentNullException("controlScheme");
			}
			if (devices == null)
			{
				throw new ArgumentNullException("devices");
			}
			InputControlScheme inputControlScheme;
			this.user.FindControlScheme(controlScheme, out inputControlScheme);
			this.SwitchControlSchemeInternal(ref inputControlScheme, devices);
		}

		public void SwitchCurrentActionMap(string mapNameOrId)
		{
			if (!this.m_Enabled)
			{
				Debug.LogError("Cannot switch to actions '" + mapNameOrId + "'; input is not enabled", this);
				return;
			}
			if (this.m_Actions == null)
			{
				Debug.LogError("Cannot switch to actions '" + mapNameOrId + "'; no actions set on PlayerInput", this);
				return;
			}
			InputActionMap inputActionMap = this.m_Actions.FindActionMap(mapNameOrId, false);
			if (inputActionMap == null)
			{
				Debug.LogError(string.Format("Cannot find action map '{0}' in actions '{1}'", mapNameOrId, this.m_Actions), this);
				return;
			}
			this.currentActionMap = inputActionMap;
		}

		public static PlayerInput GetPlayerByIndex(int playerIndex)
		{
			for (int i = 0; i < PlayerInput.s_AllActivePlayersCount; i++)
			{
				if (PlayerInput.s_AllActivePlayers[i].playerIndex == playerIndex)
				{
					return PlayerInput.s_AllActivePlayers[i];
				}
			}
			return null;
		}

		public static PlayerInput FindFirstPairedToDevice(InputDevice device)
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			for (int i = 0; i < PlayerInput.s_AllActivePlayersCount; i++)
			{
				if (PlayerInput.s_AllActivePlayers[i].devices.ContainsReference(device))
				{
					return PlayerInput.s_AllActivePlayers[i];
				}
			}
			return null;
		}

		public static PlayerInput Instantiate(GameObject prefab, int playerIndex = -1, string controlScheme = null, int splitScreenIndex = -1, InputDevice pairWithDevice = null)
		{
			if (prefab == null)
			{
				throw new ArgumentNullException("prefab");
			}
			PlayerInput.s_InitPlayerIndex = playerIndex;
			PlayerInput.s_InitSplitScreenIndex = splitScreenIndex;
			PlayerInput.s_InitControlScheme = controlScheme;
			if (pairWithDevice != null)
			{
				ArrayHelpers.AppendWithCapacity<InputDevice>(ref PlayerInput.s_InitPairWithDevices, ref PlayerInput.s_InitPairWithDevicesCount, pairWithDevice, 10);
			}
			return PlayerInput.DoInstantiate(prefab);
		}

		public static PlayerInput Instantiate(GameObject prefab, int playerIndex = -1, string controlScheme = null, int splitScreenIndex = -1, params InputDevice[] pairWithDevices)
		{
			if (prefab == null)
			{
				throw new ArgumentNullException("prefab");
			}
			PlayerInput.s_InitPlayerIndex = playerIndex;
			PlayerInput.s_InitSplitScreenIndex = splitScreenIndex;
			PlayerInput.s_InitControlScheme = controlScheme;
			if (pairWithDevices != null)
			{
				for (int i = 0; i < pairWithDevices.Length; i++)
				{
					ArrayHelpers.AppendWithCapacity<InputDevice>(ref PlayerInput.s_InitPairWithDevices, ref PlayerInput.s_InitPairWithDevicesCount, pairWithDevices[i], 10);
				}
			}
			return PlayerInput.DoInstantiate(prefab);
		}

		private static PlayerInput DoInstantiate(GameObject prefab)
		{
			bool flag = PlayerInput.s_DestroyIfDeviceSetupUnsuccessful;
			GameObject gameObject;
			try
			{
				gameObject = Object.Instantiate<GameObject>(prefab);
				gameObject.SetActive(true);
			}
			finally
			{
				PlayerInput.s_InitPairWithDevicesCount = 0;
				if (PlayerInput.s_InitPairWithDevices != null)
				{
					Array.Clear(PlayerInput.s_InitPairWithDevices, 0, PlayerInput.s_InitPairWithDevicesCount);
				}
				PlayerInput.s_InitControlScheme = null;
				PlayerInput.s_InitPlayerIndex = -1;
				PlayerInput.s_InitSplitScreenIndex = -1;
				PlayerInput.s_DestroyIfDeviceSetupUnsuccessful = false;
			}
			PlayerInput componentInChildren = gameObject.GetComponentInChildren<PlayerInput>();
			if (componentInChildren == null)
			{
				Object.DestroyImmediate(gameObject);
				Debug.LogError("The GameObject does not have a PlayerInput component", prefab);
				return null;
			}
			if (flag && (!componentInChildren.user.valid || componentInChildren.hasMissingRequiredDevices))
			{
				Object.DestroyImmediate(gameObject);
				return null;
			}
			return componentInChildren;
		}

		private void InitializeActions()
		{
			if (this.m_ActionsInitialized)
			{
				return;
			}
			if (this.m_Actions == null)
			{
				return;
			}
			for (int i = 0; i < PlayerInput.s_AllActivePlayersCount; i++)
			{
				if (PlayerInput.s_AllActivePlayers[i].m_Actions == this.m_Actions && PlayerInput.s_AllActivePlayers[i] != this)
				{
					this.CopyActionAssetAndApplyBindingOverrides();
					break;
				}
			}
			if (this.uiInputModule != null)
			{
				this.uiInputModule.actionsAsset = this.m_Actions;
			}
			switch (this.m_NotificationBehavior)
			{
			case PlayerNotifications.SendMessages:
			case PlayerNotifications.BroadcastMessages:
				this.InstallOnActionTriggeredHook();
				if (this.m_ActionMessageNames == null)
				{
					this.CacheMessageNames();
				}
				break;
			case PlayerNotifications.InvokeUnityEvents:
				if (this.m_ActionEvents != null)
				{
					foreach (PlayerInput.ActionEvent actionEvent in this.m_ActionEvents)
					{
						string actionId = actionEvent.actionId;
						if (!string.IsNullOrEmpty(actionId))
						{
							InputAction inputAction = this.m_Actions.FindAction(actionId, false);
							if (inputAction != null)
							{
								inputAction.performed += actionEvent.Invoke;
								inputAction.canceled += actionEvent.Invoke;
								inputAction.started += actionEvent.Invoke;
							}
						}
					}
				}
				break;
			case PlayerNotifications.InvokeCSharpEvents:
				this.InstallOnActionTriggeredHook();
				break;
			}
			this.m_ActionsInitialized = true;
		}

		private void CopyActionAssetAndApplyBindingOverrides()
		{
			InputActionAsset actions = this.m_Actions;
			this.m_Actions = Object.Instantiate<InputActionAsset>(this.m_Actions);
			for (int i = 0; i < actions.actionMaps.Count; i++)
			{
				for (int j = 0; j < actions.actionMaps[i].bindings.Count; j++)
				{
					this.m_Actions.actionMaps[i].ApplyBindingOverride(j, actions.actionMaps[i].bindings[j]);
				}
			}
		}

		private void UninitializeActions()
		{
			if (!this.m_ActionsInitialized)
			{
				return;
			}
			if (this.m_Actions == null)
			{
				return;
			}
			this.UninstallOnActionTriggeredHook();
			if (this.m_NotificationBehavior == PlayerNotifications.InvokeUnityEvents && this.m_ActionEvents != null)
			{
				foreach (PlayerInput.ActionEvent actionEvent in this.m_ActionEvents)
				{
					string actionId = actionEvent.actionId;
					if (!string.IsNullOrEmpty(actionId))
					{
						InputAction inputAction = this.m_Actions.FindAction(actionId, false);
						if (inputAction != null)
						{
							inputAction.performed -= actionEvent.Invoke;
							inputAction.canceled -= actionEvent.Invoke;
							inputAction.started -= actionEvent.Invoke;
						}
					}
				}
			}
			this.m_CurrentActionMap = null;
			this.m_ActionsInitialized = false;
		}

		private void InstallOnActionTriggeredHook()
		{
			if (this.m_ActionTriggeredDelegate == null)
			{
				this.m_ActionTriggeredDelegate = new Action<InputAction.CallbackContext>(this.OnActionTriggered);
			}
			foreach (InputActionMap inputActionMap in this.m_Actions.actionMaps)
			{
				inputActionMap.actionTriggered += this.m_ActionTriggeredDelegate;
			}
		}

		private void UninstallOnActionTriggeredHook()
		{
			if (this.m_ActionTriggeredDelegate != null)
			{
				foreach (InputActionMap inputActionMap in this.m_Actions.actionMaps)
				{
					inputActionMap.actionTriggered -= this.m_ActionTriggeredDelegate;
				}
			}
		}

		private void OnActionTriggered(InputAction.CallbackContext context)
		{
			if (!this.m_InputActive)
			{
				return;
			}
			PlayerNotifications notificationBehavior = this.m_NotificationBehavior;
			if (notificationBehavior > PlayerNotifications.BroadcastMessages)
			{
				if (notificationBehavior == PlayerNotifications.InvokeCSharpEvents)
				{
					DelegateHelpers.InvokeCallbacksSafe<InputAction.CallbackContext>(ref this.m_ActionTriggeredCallbacks, context, "PlayerInput.onActionTriggered", null);
					return;
				}
			}
			else
			{
				InputAction action = context.action;
				if (!context.performed && (!context.canceled || action.type != InputActionType.Value))
				{
					return;
				}
				if (this.m_ActionMessageNames == null)
				{
					this.CacheMessageNames();
				}
				string methodName = this.m_ActionMessageNames[action.m_Id];
				if (this.m_InputValueObject == null)
				{
					this.m_InputValueObject = new InputValue();
				}
				this.m_InputValueObject.m_Context = new InputAction.CallbackContext?(context);
				if (this.m_NotificationBehavior == PlayerNotifications.BroadcastMessages)
				{
					base.BroadcastMessage(methodName, this.m_InputValueObject, SendMessageOptions.DontRequireReceiver);
				}
				else
				{
					base.SendMessage(methodName, this.m_InputValueObject, SendMessageOptions.DontRequireReceiver);
				}
				this.m_InputValueObject.m_Context = null;
			}
		}

		private void CacheMessageNames()
		{
			if (this.m_Actions == null)
			{
				return;
			}
			if (this.m_ActionMessageNames != null)
			{
				this.m_ActionMessageNames.Clear();
			}
			else
			{
				this.m_ActionMessageNames = new Dictionary<string, string>();
			}
			foreach (InputAction inputAction in this.m_Actions)
			{
				inputAction.MakeSureIdIsInPlace();
				string str = CSharpCodeHelpers.MakeTypeName(inputAction.name, "");
				this.m_ActionMessageNames[inputAction.m_Id] = "On" + str;
			}
		}

		private void ClearCaches()
		{
			if (this.m_ActionMessageNames != null)
			{
				this.m_ActionMessageNames.Clear();
			}
		}

		private void AssignUserAndDevices()
		{
			if (this.m_InputUser.valid)
			{
				this.m_InputUser.UnpairDevices();
			}
			if (!(this.m_Actions == null))
			{
				if (this.m_Actions.controlSchemes.Count > 0)
				{
					if (!string.IsNullOrEmpty(PlayerInput.s_InitControlScheme))
					{
						InputControlScheme? inputControlScheme = this.m_Actions.FindControlScheme(PlayerInput.s_InitControlScheme);
						if (inputControlScheme == null)
						{
							Debug.LogError(string.Format("No control scheme '{0}' in '{1}'", PlayerInput.s_InitControlScheme, this.m_Actions), this);
						}
						else
						{
							this.TryToActivateControlScheme(inputControlScheme.Value);
						}
					}
					else if (!string.IsNullOrEmpty(this.m_DefaultControlScheme))
					{
						InputControlScheme? inputControlScheme2 = this.m_Actions.FindControlScheme(this.m_DefaultControlScheme);
						if (inputControlScheme2 == null)
						{
							Debug.LogError(string.Format("Cannot find default control scheme '{0}' in '{1}'", this.m_DefaultControlScheme, this.m_Actions), this);
						}
						else
						{
							this.TryToActivateControlScheme(inputControlScheme2.Value);
						}
					}
					if (PlayerInput.s_InitPairWithDevicesCount > 0 && (!this.m_InputUser.valid || this.m_InputUser.controlScheme == null))
					{
						InputControlScheme? inputControlScheme3 = InputControlScheme.FindControlSchemeForDevices<ReadOnlyArray<InputDevice>, ReadOnlyArray<InputControlScheme>>(new ReadOnlyArray<InputDevice>(PlayerInput.s_InitPairWithDevices, 0, PlayerInput.s_InitPairWithDevicesCount), this.m_Actions.controlSchemes, null, true);
						if (inputControlScheme3 != null)
						{
							this.TryToActivateControlScheme(inputControlScheme3.Value);
							goto IL_2D7;
						}
						goto IL_2D7;
					}
					else
					{
						if ((this.m_InputUser.valid && this.m_InputUser.controlScheme != null) || !string.IsNullOrEmpty(PlayerInput.s_InitControlScheme))
						{
							goto IL_2D7;
						}
						using (InputControlList<InputDevice> unpairedInputDevices = InputUser.GetUnpairedInputDevices())
						{
							InputControlScheme? inputControlScheme4 = InputControlScheme.FindControlSchemeForDevices<InputControlList<InputDevice>, ReadOnlyArray<InputControlScheme>>(unpairedInputDevices, this.m_Actions.controlSchemes, null, false);
							if (inputControlScheme4 != null)
							{
								this.TryToActivateControlScheme(inputControlScheme4.Value);
								goto IL_2D7;
							}
							if (InputSystem.devices.Count > 0 && unpairedInputDevices.Count == 0)
							{
								Debug.LogWarning("Cannot find matching control scheme for " + base.name + " (all control schemes are already paired to matching devices)", this);
							}
							goto IL_2D7;
						}
					}
				}
				if (PlayerInput.s_InitPairWithDevicesCount > 0)
				{
					for (int i = 0; i < PlayerInput.s_InitPairWithDevicesCount; i++)
					{
						this.m_InputUser = InputUser.PerformPairingWithDevice(PlayerInput.s_InitPairWithDevices[i], this.m_InputUser, InputUserPairingOptions.None);
					}
				}
				else
				{
					using (InputControlList<InputDevice> unpairedInputDevices2 = InputUser.GetUnpairedInputDevices())
					{
						for (int j = 0; j < unpairedInputDevices2.Count; j++)
						{
							InputDevice device = unpairedInputDevices2[j];
							if (this.HaveBindingForDevice(device))
							{
								this.m_InputUser = InputUser.PerformPairingWithDevice(device, this.m_InputUser, InputUserPairingOptions.None);
							}
						}
					}
				}
				IL_2D7:
				if (this.m_InputUser.valid)
				{
					this.m_InputUser.AssociateActionsWithUser(this.m_Actions);
				}
				return;
			}
			if (PlayerInput.s_InitPairWithDevicesCount > 0)
			{
				for (int k = 0; k < PlayerInput.s_InitPairWithDevicesCount; k++)
				{
					this.m_InputUser = InputUser.PerformPairingWithDevice(PlayerInput.s_InitPairWithDevices[k], this.m_InputUser, InputUserPairingOptions.None);
				}
				return;
			}
			this.m_InputUser = default(InputUser);
		}

		private bool HaveBindingForDevice(InputDevice device)
		{
			if (this.m_Actions == null)
			{
				return false;
			}
			ReadOnlyArray<InputActionMap> actionMaps = this.m_Actions.actionMaps;
			for (int i = 0; i < actionMaps.Count; i++)
			{
				if (actionMaps[i].IsUsableWithDevice(device))
				{
					return true;
				}
			}
			return false;
		}

		private void UnassignUserAndDevices()
		{
			if (this.m_InputUser.valid)
			{
				this.m_InputUser.UnpairDevicesAndRemoveUser();
			}
			if (this.m_Actions != null)
			{
				this.m_Actions.devices = null;
			}
		}

		private bool TryToActivateControlScheme(InputControlScheme controlScheme)
		{
			if (PlayerInput.s_InitPairWithDevicesCount > 0)
			{
				for (int i = 0; i < PlayerInput.s_InitPairWithDevicesCount; i++)
				{
					InputDevice device = PlayerInput.s_InitPairWithDevices[i];
					if (!controlScheme.SupportsDevice(device))
					{
						return false;
					}
				}
				for (int j = 0; j < PlayerInput.s_InitPairWithDevicesCount; j++)
				{
					InputDevice device2 = PlayerInput.s_InitPairWithDevices[j];
					this.m_InputUser = InputUser.PerformPairingWithDevice(device2, this.m_InputUser, InputUserPairingOptions.None);
				}
			}
			if (!this.m_InputUser.valid)
			{
				this.m_InputUser = InputUser.CreateUserWithoutPairedDevices();
			}
			this.m_InputUser.ActivateControlScheme(controlScheme).AndPairRemainingDevices();
			if (this.user.hasMissingRequiredDevices)
			{
				this.m_InputUser.ActivateControlScheme(null);
				this.m_InputUser.UnpairDevices();
				return false;
			}
			return true;
		}

		private void AssignPlayerIndex()
		{
			if (PlayerInput.s_InitPlayerIndex != -1)
			{
				this.m_PlayerIndex = PlayerInput.s_InitPlayerIndex;
				return;
			}
			int num = int.MaxValue;
			int num2 = int.MinValue;
			for (int i = 0; i < PlayerInput.s_AllActivePlayersCount; i++)
			{
				int playerIndex = PlayerInput.s_AllActivePlayers[i].playerIndex;
				num = Math.Min(num, playerIndex);
				num2 = Math.Max(num2, playerIndex);
			}
			if (num != 2147483647 && num > 0)
			{
				this.m_PlayerIndex = num - 1;
				return;
			}
			if (num2 != -2147483648)
			{
				for (int j = num; j < num2; j++)
				{
					if (PlayerInput.GetPlayerByIndex(j) == null)
					{
						this.m_PlayerIndex = j;
						return;
					}
				}
				this.m_PlayerIndex = num2 + 1;
				return;
			}
			this.m_PlayerIndex = 0;
		}

		private void OnEnable()
		{
			this.m_Enabled = true;
			using (InputActionRebindingExtensions.DeferBindingResolution())
			{
				this.AssignPlayerIndex();
				this.InitializeActions();
				this.AssignUserAndDevices();
				this.ActivateInput();
			}
			if (PlayerInput.s_InitSplitScreenIndex >= 0)
			{
				this.m_SplitScreenIndex = PlayerInput.s_InitSplitScreenIndex;
			}
			else
			{
				this.m_SplitScreenIndex = this.playerIndex;
			}
			ArrayHelpers.AppendWithCapacity<PlayerInput>(ref PlayerInput.s_AllActivePlayers, ref PlayerInput.s_AllActivePlayersCount, this, 10);
			for (int i = 1; i < PlayerInput.s_AllActivePlayersCount; i++)
			{
				int num = i;
				while (num > 0 && PlayerInput.s_AllActivePlayers[num - 1].playerIndex > PlayerInput.s_AllActivePlayers[num].playerIndex)
				{
					PlayerInput.s_AllActivePlayers.SwapElements(num, num - 1);
					num--;
				}
			}
			if (PlayerInput.s_AllActivePlayersCount == 1)
			{
				if (PlayerInput.s_UserChangeDelegate == null)
				{
					PlayerInput.s_UserChangeDelegate = new Action<InputUser, InputUserChange, InputDevice>(PlayerInput.OnUserChange);
				}
				InputUser.onChange += PlayerInput.s_UserChangeDelegate;
			}
			if (PlayerInput.isSinglePlayer)
			{
				if (this.m_Actions != null && this.m_Actions.controlSchemes.Count == 0)
				{
					this.StartListeningForDeviceChanges();
				}
				else if (!this.neverAutoSwitchControlSchemes)
				{
					this.StartListeningForUnpairedDeviceActivity();
				}
			}
			this.HandleControlsChanged();
			PlayerInputManager instance = PlayerInputManager.instance;
			if (instance == null)
			{
				return;
			}
			instance.NotifyPlayerJoined(this);
		}

		private void StartListeningForUnpairedDeviceActivity()
		{
			if (this.m_OnUnpairedDeviceUsedHooked)
			{
				return;
			}
			if (this.m_UnpairedDeviceUsedDelegate == null)
			{
				this.m_UnpairedDeviceUsedDelegate = new Action<InputControl, InputEventPtr>(this.OnUnpairedDeviceUsed);
			}
			if (this.m_PreFilterUnpairedDeviceUsedDelegate == null)
			{
				this.m_PreFilterUnpairedDeviceUsedDelegate = new Func<InputDevice, InputEventPtr, bool>(PlayerInput.OnPreFilterUnpairedDeviceUsed);
			}
			InputUser.onUnpairedDeviceUsed += this.m_UnpairedDeviceUsedDelegate;
			InputUser.onPrefilterUnpairedDeviceActivity += this.m_PreFilterUnpairedDeviceUsedDelegate;
			InputUser.listenForUnpairedDeviceActivity++;
			this.m_OnUnpairedDeviceUsedHooked = true;
		}

		private void StopListeningForUnpairedDeviceActivity()
		{
			if (!this.m_OnUnpairedDeviceUsedHooked)
			{
				return;
			}
			InputUser.onUnpairedDeviceUsed -= this.m_UnpairedDeviceUsedDelegate;
			InputUser.onPrefilterUnpairedDeviceActivity -= this.m_PreFilterUnpairedDeviceUsedDelegate;
			InputUser.listenForUnpairedDeviceActivity--;
			this.m_OnUnpairedDeviceUsedHooked = false;
		}

		private void StartListeningForDeviceChanges()
		{
			if (this.m_OnDeviceChangeHooked)
			{
				return;
			}
			if (this.m_DeviceChangeDelegate == null)
			{
				this.m_DeviceChangeDelegate = new Action<InputDevice, InputDeviceChange>(this.OnDeviceChange);
			}
			InputSystem.onDeviceChange += this.m_DeviceChangeDelegate;
			this.m_OnDeviceChangeHooked = true;
		}

		private void StopListeningForDeviceChanges()
		{
			if (!this.m_OnDeviceChangeHooked)
			{
				return;
			}
			InputSystem.onDeviceChange -= this.m_DeviceChangeDelegate;
			this.m_OnDeviceChangeHooked = false;
		}

		private void OnDisable()
		{
			this.m_Enabled = false;
			int num = PlayerInput.s_AllActivePlayers.IndexOfReference(this, PlayerInput.s_AllActivePlayersCount);
			if (num != -1)
			{
				PlayerInput.s_AllActivePlayers.EraseAtWithCapacity(ref PlayerInput.s_AllActivePlayersCount, num);
			}
			if (PlayerInput.s_AllActivePlayersCount == 0 && PlayerInput.s_UserChangeDelegate != null)
			{
				InputUser.onChange -= PlayerInput.s_UserChangeDelegate;
			}
			this.StopListeningForUnpairedDeviceActivity();
			this.StopListeningForDeviceChanges();
			PlayerInputManager instance = PlayerInputManager.instance;
			if (instance != null)
			{
				instance.NotifyPlayerLeft(this);
			}
			using (InputActionRebindingExtensions.DeferBindingResolution())
			{
				this.DeactivateInput();
				this.UnassignUserAndDevices();
				this.UninitializeActions();
			}
			this.m_PlayerIndex = -1;
		}

		public void DebugLogAction(InputAction.CallbackContext context)
		{
			Debug.Log(context.ToString());
		}

		private void HandleDeviceLost()
		{
			switch (this.m_NotificationBehavior)
			{
			case PlayerNotifications.SendMessages:
				base.SendMessage("OnDeviceLost", this, SendMessageOptions.DontRequireReceiver);
				return;
			case PlayerNotifications.BroadcastMessages:
				base.BroadcastMessage("OnDeviceLost", this, SendMessageOptions.DontRequireReceiver);
				return;
			case PlayerNotifications.InvokeUnityEvents:
			{
				PlayerInput.DeviceLostEvent deviceLostEvent = this.m_DeviceLostEvent;
				if (deviceLostEvent == null)
				{
					return;
				}
				deviceLostEvent.Invoke(this);
				return;
			}
			case PlayerNotifications.InvokeCSharpEvents:
				DelegateHelpers.InvokeCallbacksSafe<PlayerInput>(ref this.m_DeviceLostCallbacks, this, "onDeviceLost", null);
				return;
			default:
				return;
			}
		}

		private void HandleDeviceRegained()
		{
			switch (this.m_NotificationBehavior)
			{
			case PlayerNotifications.SendMessages:
				base.SendMessage("OnDeviceRegained", this, SendMessageOptions.DontRequireReceiver);
				return;
			case PlayerNotifications.BroadcastMessages:
				base.BroadcastMessage("OnDeviceRegained", this, SendMessageOptions.DontRequireReceiver);
				return;
			case PlayerNotifications.InvokeUnityEvents:
			{
				PlayerInput.DeviceRegainedEvent deviceRegainedEvent = this.m_DeviceRegainedEvent;
				if (deviceRegainedEvent == null)
				{
					return;
				}
				deviceRegainedEvent.Invoke(this);
				return;
			}
			case PlayerNotifications.InvokeCSharpEvents:
				DelegateHelpers.InvokeCallbacksSafe<PlayerInput>(ref this.m_DeviceRegainedCallbacks, this, "onDeviceRegained", null);
				return;
			default:
				return;
			}
		}

		private void HandleControlsChanged()
		{
			switch (this.m_NotificationBehavior)
			{
			case PlayerNotifications.SendMessages:
				base.SendMessage("OnControlsChanged", this, SendMessageOptions.DontRequireReceiver);
				return;
			case PlayerNotifications.BroadcastMessages:
				base.BroadcastMessage("OnControlsChanged", this, SendMessageOptions.DontRequireReceiver);
				return;
			case PlayerNotifications.InvokeUnityEvents:
			{
				PlayerInput.ControlsChangedEvent controlsChangedEvent = this.m_ControlsChangedEvent;
				if (controlsChangedEvent == null)
				{
					return;
				}
				controlsChangedEvent.Invoke(this);
				return;
			}
			case PlayerNotifications.InvokeCSharpEvents:
				DelegateHelpers.InvokeCallbacksSafe<PlayerInput>(ref this.m_ControlsChangedCallbacks, this, "onControlsChanged", null);
				return;
			default:
				return;
			}
		}

		private static void OnUserChange(InputUser user, InputUserChange change, InputDevice device)
		{
			if (change - InputUserChange.DeviceLost <= 1)
			{
				for (int i = 0; i < PlayerInput.s_AllActivePlayersCount; i++)
				{
					PlayerInput playerInput = PlayerInput.s_AllActivePlayers[i];
					if (playerInput.m_InputUser == user)
					{
						if (change == InputUserChange.DeviceLost)
						{
							playerInput.HandleDeviceLost();
						}
						else if (change == InputUserChange.DeviceRegained)
						{
							playerInput.HandleDeviceRegained();
						}
					}
				}
				return;
			}
			if (change != InputUserChange.ControlsChanged)
			{
				return;
			}
			for (int j = 0; j < PlayerInput.s_AllActivePlayersCount; j++)
			{
				PlayerInput playerInput2 = PlayerInput.s_AllActivePlayers[j];
				if (playerInput2.m_InputUser == user)
				{
					playerInput2.HandleControlsChanged();
				}
			}
		}

		private static bool OnPreFilterUnpairedDeviceUsed(InputDevice device, InputEventPtr eventPtr)
		{
			InputActionAsset actions = PlayerInput.all[0].actions;
			return actions != null && (!OnScreenControl.HasAnyActive || !(device is Pointer)) && actions.IsUsableWithDevice(device);
		}

		private void OnUnpairedDeviceUsed(InputControl control, InputEventPtr eventPtr)
		{
			if (!PlayerInput.isSinglePlayer || this.neverAutoSwitchControlSchemes)
			{
				return;
			}
			PlayerInput playerInput = PlayerInput.all[0];
			if (playerInput.m_Actions == null)
			{
				return;
			}
			InputDevice device = control.device;
			using (InputActionRebindingExtensions.DeferBindingResolution())
			{
				using (InputControlList<InputDevice> unpairedInputDevices = InputUser.GetUnpairedInputDevices())
				{
					if (unpairedInputDevices.Count > 1)
					{
						int index = unpairedInputDevices.IndexOf(device);
						unpairedInputDevices.SwapElements(0, index);
					}
					ReadOnlyArray<InputDevice> devices = playerInput.devices;
					for (int i = 0; i < devices.Count; i++)
					{
						unpairedInputDevices.Add(devices[i]);
					}
					InputControlScheme scheme;
					InputControlScheme.MatchResult matchResult;
					if (InputControlScheme.FindControlSchemeForDevices<InputControlList<InputDevice>, ReadOnlyArray<InputControlScheme>>(unpairedInputDevices, playerInput.m_Actions.controlSchemes, out scheme, out matchResult, device, false))
					{
						try
						{
							bool valid = playerInput.user.valid;
							if (valid)
							{
								playerInput.user.UnpairDevices();
							}
							InputControlList<InputDevice> devices2 = matchResult.devices;
							for (int j = 0; j < devices2.Count; j++)
							{
								playerInput.m_InputUser = InputUser.PerformPairingWithDevice(devices2[j], playerInput.m_InputUser, InputUserPairingOptions.None);
								if (!valid && playerInput.actions != null)
								{
									playerInput.m_InputUser.AssociateActionsWithUser(playerInput.actions);
								}
							}
							playerInput.user.ActivateControlScheme(scheme);
						}
						finally
						{
							matchResult.Dispose();
						}
					}
				}
			}
		}

		private void OnDeviceChange(InputDevice device, InputDeviceChange change)
		{
			if (change == InputDeviceChange.Added && PlayerInput.isSinglePlayer && this.m_Actions != null && this.m_Actions.controlSchemes.Count == 0 && this.HaveBindingForDevice(device) && this.m_InputUser.valid)
			{
				InputUser.PerformPairingWithDevice(device, this.m_InputUser, InputUserPairingOptions.None);
			}
		}

		private void SwitchControlSchemeInternal(ref InputControlScheme controlScheme, params InputDevice[] devices)
		{
			using (InputActionRebindingExtensions.DeferBindingResolution())
			{
				for (int i = this.user.pairedDevices.Count - 1; i >= 0; i--)
				{
					if (!devices.ContainsReference(this.user.pairedDevices[i]))
					{
						this.user.UnpairDevice(this.user.pairedDevices[i]);
					}
				}
				foreach (InputDevice inputDevice in devices)
				{
					if (!this.user.pairedDevices.ContainsReference(inputDevice))
					{
						InputUser.PerformPairingWithDevice(inputDevice, this.user, InputUserPairingOptions.None);
					}
				}
				if (this.user.controlScheme == null || !this.user.controlScheme.Value.Equals(controlScheme))
				{
					this.user.ActivateControlScheme(controlScheme);
				}
			}
		}

		public const string DeviceLostMessage = "OnDeviceLost";

		public const string DeviceRegainedMessage = "OnDeviceRegained";

		public const string ControlsChangedMessage = "OnControlsChanged";

		private int m_AllMapsHashCode;

		[Tooltip("Input actions associated with the player.")]
		[SerializeField]
		internal InputActionAsset m_Actions;

		[Tooltip("Determine how notifications should be sent when an input-related event associated with the player happens.")]
		[SerializeField]
		internal PlayerNotifications m_NotificationBehavior;

		[Tooltip("UI InputModule that should have it's input actions synchronized to this PlayerInput's actions.")]
		[SerializeField]
		internal InputSystemUIInputModule m_UIInputModule;

		[Tooltip("Event that is triggered when the PlayerInput loses a paired device (e.g. its battery runs out).")]
		[SerializeField]
		internal PlayerInput.DeviceLostEvent m_DeviceLostEvent;

		[SerializeField]
		internal PlayerInput.DeviceRegainedEvent m_DeviceRegainedEvent;

		[SerializeField]
		internal PlayerInput.ControlsChangedEvent m_ControlsChangedEvent;

		[SerializeField]
		internal PlayerInput.ActionEvent[] m_ActionEvents;

		[SerializeField]
		internal bool m_NeverAutoSwitchControlSchemes;

		[SerializeField]
		internal string m_DefaultControlScheme;

		[SerializeField]
		internal string m_DefaultActionMap;

		[SerializeField]
		internal int m_SplitScreenIndex = -1;

		[Tooltip("Reference to the player's view camera. Note that this is only required when using split-screen and/or per-player UIs. Otherwise it is safe to leave this property uninitialized.")]
		[SerializeField]
		internal Camera m_Camera;

		[NonSerialized]
		private InputValue m_InputValueObject;

		[NonSerialized]
		internal InputActionMap m_CurrentActionMap;

		[NonSerialized]
		private int m_PlayerIndex = -1;

		[NonSerialized]
		private bool m_InputActive;

		[NonSerialized]
		private bool m_Enabled;

		[NonSerialized]
		internal bool m_ActionsInitialized;

		[NonSerialized]
		private Dictionary<string, string> m_ActionMessageNames;

		[NonSerialized]
		private InputUser m_InputUser;

		[NonSerialized]
		private Action<InputAction.CallbackContext> m_ActionTriggeredDelegate;

		[NonSerialized]
		private CallbackArray<Action<PlayerInput>> m_DeviceLostCallbacks;

		[NonSerialized]
		private CallbackArray<Action<PlayerInput>> m_DeviceRegainedCallbacks;

		[NonSerialized]
		private CallbackArray<Action<PlayerInput>> m_ControlsChangedCallbacks;

		[NonSerialized]
		private CallbackArray<Action<InputAction.CallbackContext>> m_ActionTriggeredCallbacks;

		[NonSerialized]
		private Action<InputControl, InputEventPtr> m_UnpairedDeviceUsedDelegate;

		[NonSerialized]
		private Func<InputDevice, InputEventPtr, bool> m_PreFilterUnpairedDeviceUsedDelegate;

		[NonSerialized]
		private bool m_OnUnpairedDeviceUsedHooked;

		[NonSerialized]
		private Action<InputDevice, InputDeviceChange> m_DeviceChangeDelegate;

		[NonSerialized]
		private bool m_OnDeviceChangeHooked;

		internal static int s_AllActivePlayersCount;

		internal static PlayerInput[] s_AllActivePlayers;

		private static Action<InputUser, InputUserChange, InputDevice> s_UserChangeDelegate;

		private static int s_InitPairWithDevicesCount;

		private static InputDevice[] s_InitPairWithDevices;

		private static int s_InitPlayerIndex = -1;

		private static int s_InitSplitScreenIndex = -1;

		private static string s_InitControlScheme;

		internal static bool s_DestroyIfDeviceSetupUnsuccessful;

		[Serializable]
		public class ActionEvent : UnityEvent<InputAction.CallbackContext>
		{
			public string actionId
			{
				get
				{
					return this.m_ActionId;
				}
			}

			public string actionName
			{
				get
				{
					return this.m_ActionName;
				}
			}

			public ActionEvent()
			{
			}

			public ActionEvent(InputAction action)
			{
				if (action == null)
				{
					throw new ArgumentNullException("action");
				}
				if (action.isSingletonAction)
				{
					throw new ArgumentException(string.Format("Action must be part of an asset (given action '{0}' is a singleton)", action));
				}
				if (action.actionMap.asset == null)
				{
					throw new ArgumentException(string.Format("Action must be part of an asset (given action '{0}' is not)", action));
				}
				this.m_ActionId = action.id.ToString();
				this.m_ActionName = action.actionMap.name + "/" + action.name;
			}

			public ActionEvent(Guid actionGUID, string name = null)
			{
				this.m_ActionId = actionGUID.ToString();
				this.m_ActionName = name;
			}

			[SerializeField]
			private string m_ActionId;

			[SerializeField]
			private string m_ActionName;
		}

		[Serializable]
		public class DeviceLostEvent : UnityEvent<PlayerInput>
		{
		}

		[Serializable]
		public class DeviceRegainedEvent : UnityEvent<PlayerInput>
		{
		}

		[Serializable]
		public class ControlsChangedEvent : UnityEvent<PlayerInput>
		{
		}
	}
}
