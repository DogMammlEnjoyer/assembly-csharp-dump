using System;
using UnityEngine.Events;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem
{
	[AddComponentMenu("Input/Player Input Manager")]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.inputsystem@1.14/manual/PlayerInputManager.html")]
	public class PlayerInputManager : MonoBehaviour
	{
		public bool splitScreen
		{
			get
			{
				return this.m_SplitScreen;
			}
			set
			{
				if (this.m_SplitScreen == value)
				{
					return;
				}
				this.m_SplitScreen = value;
				if (!this.m_SplitScreen)
				{
					using (ReadOnlyArray<PlayerInput>.Enumerator enumerator = PlayerInput.all.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							PlayerInput playerInput = enumerator.Current;
							Camera camera = playerInput.camera;
							if (camera != null)
							{
								camera.rect = new Rect(0f, 0f, 1f, 1f);
							}
						}
						return;
					}
				}
				this.UpdateSplitScreen();
			}
		}

		public bool maintainAspectRatioInSplitScreen
		{
			get
			{
				return this.m_MaintainAspectRatioInSplitScreen;
			}
		}

		public int fixedNumberOfSplitScreens
		{
			get
			{
				return this.m_FixedNumberOfSplitScreens;
			}
		}

		public Rect splitScreenArea
		{
			get
			{
				return this.m_SplitScreenRect;
			}
		}

		public int playerCount
		{
			get
			{
				return PlayerInput.s_AllActivePlayersCount;
			}
		}

		public int maxPlayerCount
		{
			get
			{
				return this.m_MaxPlayerCount;
			}
		}

		public bool joiningEnabled
		{
			get
			{
				return this.m_AllowJoining;
			}
		}

		public PlayerJoinBehavior joinBehavior
		{
			get
			{
				return this.m_JoinBehavior;
			}
			set
			{
				if (this.m_JoinBehavior == value)
				{
					return;
				}
				bool allowJoining = this.m_AllowJoining;
				if (allowJoining)
				{
					this.DisableJoining();
				}
				this.m_JoinBehavior = value;
				if (allowJoining)
				{
					this.EnableJoining();
				}
			}
		}

		public InputActionProperty joinAction
		{
			get
			{
				return this.m_JoinAction;
			}
			set
			{
				if (this.m_JoinAction == value)
				{
					return;
				}
				bool flag = this.m_AllowJoining && this.m_JoinBehavior == PlayerJoinBehavior.JoinPlayersWhenJoinActionIsTriggered;
				if (flag)
				{
					this.DisableJoining();
				}
				this.m_JoinAction = value;
				if (flag)
				{
					this.EnableJoining();
				}
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
				this.m_NotificationBehavior = value;
			}
		}

		public PlayerInputManager.PlayerJoinedEvent playerJoinedEvent
		{
			get
			{
				if (this.m_PlayerJoinedEvent == null)
				{
					this.m_PlayerJoinedEvent = new PlayerInputManager.PlayerJoinedEvent();
				}
				return this.m_PlayerJoinedEvent;
			}
		}

		public PlayerInputManager.PlayerLeftEvent playerLeftEvent
		{
			get
			{
				if (this.m_PlayerLeftEvent == null)
				{
					this.m_PlayerLeftEvent = new PlayerInputManager.PlayerLeftEvent();
				}
				return this.m_PlayerLeftEvent;
			}
		}

		public event Action<PlayerInput> onPlayerJoined
		{
			add
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.m_PlayerJoinedCallbacks.AddCallback(value);
			}
			remove
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.m_PlayerJoinedCallbacks.RemoveCallback(value);
			}
		}

		public event Action<PlayerInput> onPlayerLeft
		{
			add
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.m_PlayerLeftCallbacks.AddCallback(value);
			}
			remove
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.m_PlayerLeftCallbacks.RemoveCallback(value);
			}
		}

		public GameObject playerPrefab
		{
			get
			{
				return this.m_PlayerPrefab;
			}
			set
			{
				this.m_PlayerPrefab = value;
			}
		}

		public static PlayerInputManager instance { get; private set; }

		public void EnableJoining()
		{
			PlayerJoinBehavior joinBehavior = this.m_JoinBehavior;
			if (joinBehavior != PlayerJoinBehavior.JoinPlayersWhenButtonIsPressed)
			{
				if (joinBehavior == PlayerJoinBehavior.JoinPlayersWhenJoinActionIsTriggered)
				{
					if (this.m_JoinAction.action != null)
					{
						if (!this.m_JoinActionDelegateHooked)
						{
							if (this.m_JoinActionDelegate == null)
							{
								this.m_JoinActionDelegate = new Action<InputAction.CallbackContext>(this.JoinPlayerFromActionIfNotAlreadyJoined);
							}
							this.m_JoinAction.action.performed += this.m_JoinActionDelegate;
							this.m_JoinActionDelegateHooked = true;
						}
						this.m_JoinAction.action.Enable();
					}
					else
					{
						Debug.LogError("No join action configured on PlayerInputManager but join behavior is set to JoinPlayersWhenJoinActionIsTriggered", this);
					}
				}
			}
			else
			{
				this.ValidateInputActionAsset();
				if (!this.m_UnpairedDeviceUsedDelegateHooked)
				{
					if (this.m_UnpairedDeviceUsedDelegate == null)
					{
						this.m_UnpairedDeviceUsedDelegate = new Action<InputControl, InputEventPtr>(this.OnUnpairedDeviceUsed);
					}
					InputUser.onUnpairedDeviceUsed += this.m_UnpairedDeviceUsedDelegate;
					this.m_UnpairedDeviceUsedDelegateHooked = true;
					InputUser.listenForUnpairedDeviceActivity++;
				}
			}
			this.m_AllowJoining = true;
		}

		public void DisableJoining()
		{
			PlayerJoinBehavior joinBehavior = this.m_JoinBehavior;
			if (joinBehavior != PlayerJoinBehavior.JoinPlayersWhenButtonIsPressed)
			{
				if (joinBehavior == PlayerJoinBehavior.JoinPlayersWhenJoinActionIsTriggered)
				{
					if (this.m_JoinActionDelegateHooked)
					{
						if (this.m_JoinAction.action != null)
						{
							this.m_JoinAction.action.performed -= this.m_JoinActionDelegate;
						}
						this.m_JoinActionDelegateHooked = false;
					}
					InputAction action = this.m_JoinAction.action;
					if (action != null)
					{
						action.Disable();
					}
				}
			}
			else if (this.m_UnpairedDeviceUsedDelegateHooked)
			{
				InputUser.onUnpairedDeviceUsed -= this.m_UnpairedDeviceUsedDelegate;
				this.m_UnpairedDeviceUsedDelegateHooked = false;
				InputUser.listenForUnpairedDeviceActivity--;
			}
			this.m_AllowJoining = false;
		}

		internal void JoinPlayerFromUI()
		{
			if (!this.CheckIfPlayerCanJoin(-1))
			{
				return;
			}
			throw new NotImplementedException();
		}

		public void JoinPlayerFromAction(InputAction.CallbackContext context)
		{
			if (!this.CheckIfPlayerCanJoin(-1))
			{
				return;
			}
			InputDevice device = context.control.device;
			this.JoinPlayer(-1, -1, null, device);
		}

		public void JoinPlayerFromActionIfNotAlreadyJoined(InputAction.CallbackContext context)
		{
			if (!this.CheckIfPlayerCanJoin(-1))
			{
				return;
			}
			InputDevice device = context.control.device;
			if (PlayerInput.FindFirstPairedToDevice(device) != null)
			{
				return;
			}
			this.JoinPlayer(-1, -1, null, device);
		}

		public PlayerInput JoinPlayer(int playerIndex = -1, int splitScreenIndex = -1, string controlScheme = null, InputDevice pairWithDevice = null)
		{
			if (!this.CheckIfPlayerCanJoin(playerIndex))
			{
				return null;
			}
			PlayerInput.s_DestroyIfDeviceSetupUnsuccessful = true;
			return PlayerInput.Instantiate(this.m_PlayerPrefab, playerIndex, controlScheme, splitScreenIndex, pairWithDevice);
		}

		public PlayerInput JoinPlayer(int playerIndex = -1, int splitScreenIndex = -1, string controlScheme = null, params InputDevice[] pairWithDevices)
		{
			if (!this.CheckIfPlayerCanJoin(playerIndex))
			{
				return null;
			}
			PlayerInput.s_DestroyIfDeviceSetupUnsuccessful = true;
			return PlayerInput.Instantiate(this.m_PlayerPrefab, playerIndex, controlScheme, splitScreenIndex, pairWithDevices);
		}

		internal static string[] messages
		{
			get
			{
				return new string[]
				{
					"OnPlayerJoined",
					"OnPlayerLeft"
				};
			}
		}

		private bool CheckIfPlayerCanJoin(int playerIndex = -1)
		{
			if (this.m_PlayerPrefab == null)
			{
				Debug.LogError("playerPrefab must be set in order to be able to join new players", this);
				return false;
			}
			if (this.m_MaxPlayerCount >= 0 && this.playerCount >= this.m_MaxPlayerCount)
			{
				Debug.LogWarning("Maximum number of supported players reached: " + this.maxPlayerCount.ToString(), this);
				return false;
			}
			if (playerIndex != -1)
			{
				for (int i = 0; i < PlayerInput.s_AllActivePlayersCount; i++)
				{
					if (PlayerInput.s_AllActivePlayers[i].playerIndex == playerIndex)
					{
						Debug.LogError(string.Format("Player index #{0} is already taken by player {1}", playerIndex, PlayerInput.s_AllActivePlayers[i]), PlayerInput.s_AllActivePlayers[i]);
						return false;
					}
				}
			}
			return true;
		}

		private void OnUnpairedDeviceUsed(InputControl control, InputEventPtr eventPtr)
		{
			if (!this.m_AllowJoining)
			{
				return;
			}
			if (this.m_JoinBehavior == PlayerJoinBehavior.JoinPlayersWhenButtonIsPressed)
			{
				if (!(control is ButtonControl))
				{
					return;
				}
				if (!this.IsDeviceUsableWithPlayerActions(control.device))
				{
					return;
				}
				this.JoinPlayer(-1, -1, null, control.device);
			}
		}

		private void OnEnable()
		{
			if (PlayerInputManager.instance == null)
			{
				PlayerInputManager.instance = this;
				if (this.joinAction.reference != null)
				{
					InputAction action = this.joinAction.action;
					Object x;
					if (action == null)
					{
						x = null;
					}
					else
					{
						InputActionMap actionMap = action.actionMap;
						x = ((actionMap != null) ? actionMap.asset : null);
					}
					if (x != null)
					{
						InputActionReference reference = InputActionReference.Create(Object.Instantiate<InputActionAsset>(this.joinAction.action.actionMap.asset).FindAction(this.joinAction.action.name, false));
						this.joinAction = new InputActionProperty(reference);
					}
				}
				for (int i = 0; i < PlayerInput.s_AllActivePlayersCount; i++)
				{
					this.NotifyPlayerJoined(PlayerInput.s_AllActivePlayers[i]);
				}
				if (this.m_AllowJoining)
				{
					this.EnableJoining();
				}
				return;
			}
			Debug.LogWarning("Multiple PlayerInputManagers in the game. There should only be one PlayerInputManager", this);
		}

		private void OnDisable()
		{
			if (PlayerInputManager.instance == this)
			{
				PlayerInputManager.instance = null;
			}
			if (this.m_AllowJoining)
			{
				this.DisableJoining();
			}
		}

		private void UpdateSplitScreen()
		{
			if (!this.m_SplitScreen)
			{
				return;
			}
			int num = 0;
			foreach (PlayerInput playerInput in PlayerInput.all)
			{
				if (playerInput.playerIndex >= num)
				{
					num = playerInput.playerIndex + 1;
				}
			}
			if (this.m_FixedNumberOfSplitScreens > 0)
			{
				if (this.m_FixedNumberOfSplitScreens < num)
				{
					Debug.LogWarning(string.Format("Highest playerIndex of {0} exceeds fixed number of split-screens of {1}", num, this.m_FixedNumberOfSplitScreens), this);
				}
				num = this.m_FixedNumberOfSplitScreens;
			}
			int num2 = Mathf.CeilToInt(Mathf.Sqrt((float)num));
			int num3 = num2;
			if (!this.m_MaintainAspectRatioInSplitScreen && num2 * (num2 - 1) >= num)
			{
				num3--;
			}
			foreach (PlayerInput playerInput2 in PlayerInput.all)
			{
				int splitScreenIndex = playerInput2.splitScreenIndex;
				if (splitScreenIndex >= num2 * num3)
				{
					Debug.LogError(string.Format("Split-screen index of {0} on player is out of range (have {1} screens); resetting to playerIndex", splitScreenIndex, num2 * num3), playerInput2);
					playerInput2.m_SplitScreenIndex = playerInput2.playerIndex;
				}
				Camera camera = playerInput2.camera;
				if (camera == null)
				{
					Debug.LogError("Player has no camera associated with it. Cannot set up split-screen. Point PlayerInput.camera to camera for player.", playerInput2);
				}
				else
				{
					int num4 = splitScreenIndex % num2;
					int num5 = splitScreenIndex / num2;
					Rect rect = new Rect
					{
						width = this.m_SplitScreenRect.width / (float)num2,
						height = this.m_SplitScreenRect.height / (float)num3
					};
					rect.x = this.m_SplitScreenRect.x + (float)num4 * rect.width;
					rect.y = this.m_SplitScreenRect.y + this.m_SplitScreenRect.height - (float)(num5 + 1) * rect.height;
					camera.rect = rect;
				}
			}
		}

		private bool IsDeviceUsableWithPlayerActions(InputDevice device)
		{
			if (this.m_PlayerPrefab == null)
			{
				return true;
			}
			PlayerInput componentInChildren = this.m_PlayerPrefab.GetComponentInChildren<PlayerInput>();
			if (componentInChildren == null)
			{
				return true;
			}
			InputActionAsset actions = componentInChildren.actions;
			if (actions == null)
			{
				return true;
			}
			if (actions.controlSchemes.Count > 0)
			{
				using (InputControlList<InputDevice> unpairedInputDevices = InputUser.GetUnpairedInputDevices())
				{
					if (InputControlScheme.FindControlSchemeForDevices<InputControlList<InputDevice>, ReadOnlyArray<InputControlScheme>>(unpairedInputDevices, actions.controlSchemes, device, false) == null)
					{
						return false;
					}
				}
				return true;
			}
			using (ReadOnlyArray<InputActionMap>.Enumerator enumerator = actions.actionMaps.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.IsUsableWithDevice(device))
					{
						return true;
					}
				}
			}
			return false;
		}

		private void ValidateInputActionAsset()
		{
		}

		internal void NotifyPlayerJoined(PlayerInput player)
		{
			this.UpdateSplitScreen();
			switch (this.m_NotificationBehavior)
			{
			case PlayerNotifications.SendMessages:
				base.SendMessage("OnPlayerJoined", player, SendMessageOptions.DontRequireReceiver);
				return;
			case PlayerNotifications.BroadcastMessages:
				base.BroadcastMessage("OnPlayerJoined", player, SendMessageOptions.DontRequireReceiver);
				return;
			case PlayerNotifications.InvokeUnityEvents:
			{
				PlayerInputManager.PlayerJoinedEvent playerJoinedEvent = this.m_PlayerJoinedEvent;
				if (playerJoinedEvent == null)
				{
					return;
				}
				playerJoinedEvent.Invoke(player);
				return;
			}
			case PlayerNotifications.InvokeCSharpEvents:
				DelegateHelpers.InvokeCallbacksSafe<PlayerInput>(ref this.m_PlayerJoinedCallbacks, player, "onPlayerJoined", null);
				return;
			default:
				return;
			}
		}

		internal void NotifyPlayerLeft(PlayerInput player)
		{
			this.UpdateSplitScreen();
			switch (this.m_NotificationBehavior)
			{
			case PlayerNotifications.SendMessages:
				base.SendMessage("OnPlayerLeft", player, SendMessageOptions.DontRequireReceiver);
				return;
			case PlayerNotifications.BroadcastMessages:
				base.BroadcastMessage("OnPlayerLeft", player, SendMessageOptions.DontRequireReceiver);
				return;
			case PlayerNotifications.InvokeUnityEvents:
			{
				PlayerInputManager.PlayerLeftEvent playerLeftEvent = this.m_PlayerLeftEvent;
				if (playerLeftEvent == null)
				{
					return;
				}
				playerLeftEvent.Invoke(player);
				return;
			}
			case PlayerNotifications.InvokeCSharpEvents:
				DelegateHelpers.InvokeCallbacksSafe<PlayerInput>(ref this.m_PlayerLeftCallbacks, player, "onPlayerLeft", null);
				return;
			default:
				return;
			}
		}

		public const string PlayerJoinedMessage = "OnPlayerJoined";

		public const string PlayerLeftMessage = "OnPlayerLeft";

		[SerializeField]
		internal PlayerNotifications m_NotificationBehavior;

		[Tooltip("Set a limit for the maximum number of players who are able to join.")]
		[SerializeField]
		internal int m_MaxPlayerCount = -1;

		[SerializeField]
		internal bool m_AllowJoining = true;

		[SerializeField]
		internal PlayerJoinBehavior m_JoinBehavior;

		[SerializeField]
		internal PlayerInputManager.PlayerJoinedEvent m_PlayerJoinedEvent;

		[SerializeField]
		internal PlayerInputManager.PlayerLeftEvent m_PlayerLeftEvent;

		[SerializeField]
		internal InputActionProperty m_JoinAction;

		[SerializeField]
		internal GameObject m_PlayerPrefab;

		[SerializeField]
		internal bool m_SplitScreen;

		[SerializeField]
		internal bool m_MaintainAspectRatioInSplitScreen;

		[Tooltip("Explicitly set a fixed number of screens or otherwise allow the screen to be divided automatically to best fit the number of players.")]
		[SerializeField]
		internal int m_FixedNumberOfSplitScreens = -1;

		[SerializeField]
		internal Rect m_SplitScreenRect = new Rect(0f, 0f, 1f, 1f);

		[NonSerialized]
		private bool m_JoinActionDelegateHooked;

		[NonSerialized]
		private bool m_UnpairedDeviceUsedDelegateHooked;

		[NonSerialized]
		private Action<InputAction.CallbackContext> m_JoinActionDelegate;

		[NonSerialized]
		private Action<InputControl, InputEventPtr> m_UnpairedDeviceUsedDelegate;

		[NonSerialized]
		private CallbackArray<Action<PlayerInput>> m_PlayerJoinedCallbacks;

		[NonSerialized]
		private CallbackArray<Action<PlayerInput>> m_PlayerLeftCallbacks;

		[Serializable]
		public class PlayerJoinedEvent : UnityEvent<PlayerInput>
		{
		}

		[Serializable]
		public class PlayerLeftEvent : UnityEvent<PlayerInput>
		{
		}
	}
}
