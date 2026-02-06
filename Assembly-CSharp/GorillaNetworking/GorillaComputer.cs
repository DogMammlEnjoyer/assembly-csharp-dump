using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using GorillaGameModes;
using GorillaTagScripts;
using GorillaTagScripts.VirtualStumpCustomMaps;
using KID.Model;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using PlayFab.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace GorillaNetworking
{
	public class GorillaComputer : MonoBehaviour, IMatchmakingCallbacks, IGorillaSliceableSimple
	{
		public string versionMismatch
		{
			get
			{
				if (this._lastLocaleChecked_Version != null && this._lastLocaleChecked_Version == LocalisationManager.CurrentLanguage && !string.IsNullOrEmpty(this._cachedVersionMismatch))
				{
					return this._cachedVersionMismatch;
				}
				string defaultResult = "PLEASE UPDATE TO THE LATEST VERSION OF GORILLA TAG. YOU'RE ON AN OLD VERSION. FEEL FREE TO RUN AROUND, BUT YOU WON'T BE ABLE TO PLAY WITH ANYONE ELSE.";
				string cachedVersionMismatch;
				LocalisationManager.TryGetKeyForCurrentLocale("VERSION_MISMATCH", out cachedVersionMismatch, defaultResult);
				this._lastLocaleChecked_Version = LocalisationManager.CurrentLanguage;
				this._cachedVersionMismatch = cachedVersionMismatch;
				return this._cachedVersionMismatch;
			}
		}

		public string unableToConnect
		{
			get
			{
				if (this._lastLocaleChecked_Connect != null && this._lastLocaleChecked_Connect == LocalisationManager.CurrentLanguage && !string.IsNullOrEmpty(this._cachedUnableToConnect))
				{
					return this._cachedUnableToConnect;
				}
				string defaultResult = "UNABLE TO CONNECT TO THE INTERNET. PLEASE CHECK YOUR CONNECTION AND RESTART THE GAME.";
				string cachedUnableToConnect;
				LocalisationManager.TryGetKeyForCurrentLocale("CONNECTION_ISSUE", out cachedUnableToConnect, defaultResult);
				this._lastLocaleChecked_Connect = LocalisationManager.CurrentLanguage;
				this._cachedUnableToConnect = cachedUnableToConnect;
				return this._cachedUnableToConnect;
			}
		}

		public DateTime GetServerTime()
		{
			return this.startupTime + TimeSpan.FromSeconds((double)Time.realtimeSinceStartup);
		}

		public void AddSeverTime(int m)
		{
			this.startupTime = this.startupTime.AddMinutes((double)m);
		}

		public string[] allowedMapsToJoin
		{
			get
			{
				return this._allowedMapsToJoin;
			}
			set
			{
				this._allowedMapsToJoin = value;
			}
		}

		public string VStumpRoomPrepend
		{
			get
			{
				return this.virtualStumpRoomPrepend;
			}
		}

		public GorillaComputer.ComputerState currentState
		{
			get
			{
				GorillaComputer.ComputerState result;
				this.stateStack.TryPeek(out result);
				return result;
			}
		}

		public string NameTagPlayerPref
		{
			get
			{
				if (PlayFabAuthenticator.instance == null)
				{
					Debug.LogError("Trying to access PlayFab Authenticator Instance, but it is null. Will use a shared key for the nametag instead");
					return "nameTagsOn";
				}
				return "nameTagsOn-" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
			}
		}

		public bool NametagsEnabled { get; private set; }

		public GorillaComputer.RedemptionResult RedemptionStatus
		{
			get
			{
				return this.redemptionResult;
			}
			set
			{
				this.redemptionResult = value;
				this.UpdateScreen();
			}
		}

		public string RedemptionCode
		{
			get
			{
				return this.redemptionCode;
			}
			set
			{
				this.redemptionCode = value;
			}
		}

		public DateTimeOffset? RedemptionRestrictionTime { get; set; }

		private void Awake()
		{
			if (GorillaComputer.instance == null)
			{
				GorillaComputer.instance = this;
				GorillaComputer.hasInstance = true;
			}
			else if (GorillaComputer.instance != this)
			{
				Object.Destroy(base.gameObject);
			}
			Debug.Log(string.Concat(new string[]
			{
				"==== GORILLA TAG - VERSION: ",
				this.version,
				", BUILD NUMBER: ",
				this.buildCode,
				", BUILD DATE: ",
				this.buildDate,
				" ====\r\n.\r\n.               _______\r\n.              /       \\\r\n.             /  _____  \\\r\n.            / / _   _ \\ \\\r\n.           [ | (O) (O) | ]\r\n.            | \\  . .  / |\r\n.     _______|  | _._ |  |_______\r\n.    /        \\  \\___/  /        \\\r\n.\r\n.\r\n"
			}));
			this._activeOrderList = this.OrderList;
			this.defaultUpdateCooldown = this.updateCooldown;
		}

		private void Start()
		{
			Debug.Log("Computer Init");
			this.Initialise();
		}

		public void OnEnable()
		{
			KIDManager.RegisterSessionUpdatedCallback_VoiceChat(new Action<bool, Permission.ManagedByEnum>(this.SetVoiceChatBySafety));
			KIDManager.RegisterSessionUpdatedCallback_CustomUsernames(new Action<bool, Permission.ManagedByEnum>(this.OnKIDSessionUpdated_CustomNicknames));
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		}

		public void OnDisable()
		{
			KIDManager.UnregisterSessionUpdatedCallback_VoiceChat(new Action<bool, Permission.ManagedByEnum>(this.SetVoiceChatBySafety));
			KIDManager.UnregisterSessionUpdatedCallback_CustomUsernames(new Action<bool, Permission.ManagedByEnum>(this.OnKIDSessionUpdated_CustomNicknames));
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		}

		protected void OnDestroy()
		{
			if (GorillaComputer.instance == this)
			{
				GorillaComputer.hasInstance = false;
				GorillaComputer.instance = null;
			}
			KIDManager.UnregisterSessionUpdateCallback_AnyPermission(new Action(this.OnSessionUpdate_GorillaComputer));
		}

		public void SliceUpdate()
		{
			if ((this.internetFailure && Time.time < this.lastCheckedWifi + this.checkIfConnectedSeconds) || (!this.internetFailure && Time.time < this.lastCheckedWifi + this.checkIfDisconnectedSeconds))
			{
				if (!this.internetFailure && this.isConnectedToMaster && Time.time > this.lastUpdateTime + this.updateCooldown)
				{
					this.deltaTime = Time.time - this.lastUpdateTime;
					this.lastUpdateTime = Time.time;
					this.UpdateScreen();
				}
				return;
			}
			this.lastCheckedWifi = Time.time;
			this.stateUpdated = false;
			if (!this.CheckInternetConnection())
			{
				string defaultResult = "NO WIFI OR LAN CONNECTION DETECTED.";
				string failMessage;
				LocalisationManager.TryGetKeyForCurrentLocale("NO_CONNECTION", out failMessage, defaultResult);
				this.UpdateFailureText(failMessage);
				this.internetFailure = true;
				return;
			}
			if (this.internetFailure)
			{
				if (this.CheckInternetConnection())
				{
					this.internetFailure = false;
				}
				this.RestoreFromFailureState();
				this.UpdateScreen();
				return;
			}
			if (this.isConnectedToMaster && Time.time > this.lastUpdateTime + this.updateCooldown)
			{
				this.deltaTime = Time.time - this.lastUpdateTime;
				this.lastUpdateTime = Time.time;
				this.UpdateScreen();
			}
		}

		private void Initialise()
		{
			GameEvents.OnGorrillaKeyboardButtonPressedEvent.AddListener(new UnityAction<GorillaKeyboardBindings>(this.PressButton));
			RoomSystem.JoinedRoomEvent += new Action(GorillaComputer.OnFirstJoinedRoom_IncrementSessionCount);
			RoomSystem.JoinedRoomEvent += new Action(this.UpdateScreen);
			RoomSystem.LeftRoomEvent += new Action(this.UpdateScreen);
			RoomSystem.PlayerJoinedEvent += new Action<NetPlayer>(this.PlayerCountChangedCallback);
			RoomSystem.PlayerLeftEvent += new Action<NetPlayer>(this.PlayerCountChangedCallback);
			LocalisationManager.RegisterOnLanguageChanged(delegate
			{
				this.RefreshFunctionNames();
				this.UpdateGameModeText();
			});
			this.RefreshFunctionNames();
			this.InitialiseRoomScreens();
			this.InitialiseStrings();
			this.InitialiseAllRoomStates();
			this.UpdateScreen();
			byte[] bytes = new byte[]
			{
				Convert.ToByte(64)
			};
			this.virtualStumpRoomPrepend = Encoding.ASCII.GetString(bytes);
			this.initialized = true;
		}

		private void InitialiseRoomScreens()
		{
			this.screenText.Initialize(this.computerScreenRenderer.materials, this.wrongVersionMaterial, GameEvents.ScreenTextChangedEvent, GameEvents.ScreenTextMaterialsEvent);
			this.functionSelectText.Initialize(this.computerScreenRenderer.materials, this.wrongVersionMaterial, GameEvents.FunctionSelectTextChangedEvent, null);
		}

		private void InitialiseStrings()
		{
			this.roomToJoin = "";
			this.redText = "";
			this.blueText = "";
			this.greenText = "";
			this.currentName = "";
			this.savedName = "";
		}

		private void InitialiseAllRoomStates()
		{
			this.SwitchState(GorillaComputer.ComputerState.Startup, true);
			this.InitialiseLanguageScreen();
			this.InitializeNameState();
			this.InitializeRoomState();
			this.InitializeTurnState();
			this.InitializeStartupState();
			this.InitializeQueueState();
			this.InitializeMicState();
			this.InitializeGroupState();
			this.InitializeVoiceState();
			this.InitializeAutoMuteState();
			this.InitializeGameMode();
			this.InitializeVisualsState();
			this.InitializeCreditsState();
			this.InitializeTimeState();
			this.InitializeSupportState();
			this.InitializeTroopState();
			this.InitializeKIdState();
			this.InitializeRedeemState();
		}

		private void InitializeStartupState()
		{
		}

		private void InitializeRoomState()
		{
		}

		private void InitializeColorState()
		{
			this.redValue = PlayerPrefs.GetFloat("redValue", 0f);
			this.greenValue = PlayerPrefs.GetFloat("greenValue", 0f);
			this.blueValue = PlayerPrefs.GetFloat("blueValue", 0f);
			this.blueText = Mathf.Floor(this.blueValue * 9f).ToString();
			this.redText = Mathf.Floor(this.redValue * 9f).ToString();
			this.greenText = Mathf.Floor(this.greenValue * 9f).ToString();
			this.colorCursorLine = 0;
			GorillaTagger.Instance.UpdateColor(this.redValue, this.greenValue, this.blueValue);
		}

		private void InitializeNameState()
		{
			int @int = PlayerPrefs.GetInt("nameTagsOn", -1);
			Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Custom_Nametags);
			switch (permissionDataByFeature.ManagedBy)
			{
			case Permission.ManagedByEnum.PLAYER:
				if (@int == -1)
				{
					this.NametagsEnabled = permissionDataByFeature.Enabled;
				}
				else
				{
					this.NametagsEnabled = (@int > 0);
				}
				break;
			case Permission.ManagedByEnum.GUARDIAN:
				this.NametagsEnabled = (permissionDataByFeature.Enabled && @int > 0);
				break;
			case Permission.ManagedByEnum.PROHIBITED:
				this.NametagsEnabled = false;
				break;
			}
			this.savedName = PlayerPrefs.GetString("playerName", "gorilla");
			NetworkSystem.Instance.SetMyNickName(this.savedName);
			this.currentName = this.savedName;
			VRRigCache.Instance.localRig.Rig.UpdateName();
			this.exactOneWeek = this.exactOneWeekFile.text.Split('\n', StringSplitOptions.None);
			this.anywhereOneWeek = this.anywhereOneWeekFile.text.Split('\n', StringSplitOptions.None);
			this.anywhereTwoWeek = this.anywhereTwoWeekFile.text.Split('\n', StringSplitOptions.None);
			for (int i = 0; i < this.exactOneWeek.Length; i++)
			{
				this.exactOneWeek[i] = this.exactOneWeek[i].ToLower().TrimEnd(new char[]
				{
					'\r',
					'\n'
				});
			}
			for (int j = 0; j < this.anywhereOneWeek.Length; j++)
			{
				this.anywhereOneWeek[j] = this.anywhereOneWeek[j].ToLower().TrimEnd(new char[]
				{
					'\r',
					'\n'
				});
			}
			for (int k = 0; k < this.anywhereTwoWeek.Length; k++)
			{
				this.anywhereTwoWeek[k] = this.anywhereTwoWeek[k].ToLower().TrimEnd(new char[]
				{
					'\r',
					'\n'
				});
			}
		}

		private void InitializeTurnState()
		{
			GorillaSnapTurn.LoadSettingsFromPlayerPrefs();
		}

		private void InitializeMicState()
		{
			this.pttType = PlayerPrefs.GetString("pttType", "OPEN MIC");
			if (this.pttType == "ALL CHAT")
			{
				this.pttType = "OPEN MIC";
				PlayerPrefs.SetString("pttType", this.pttType);
				PlayerPrefs.Save();
			}
		}

		private void InitializeAutoMuteState()
		{
			int @int = PlayerPrefs.GetInt("autoMute", 1);
			if (@int == 0)
			{
				this.autoMuteType = "OFF";
				return;
			}
			if (@int == 1)
			{
				this.autoMuteType = "MODERATE";
				return;
			}
			if (@int == 2)
			{
				this.autoMuteType = "AGGRESSIVE";
			}
		}

		private void InitializeQueueState()
		{
			this.currentQueue = PlayerPrefs.GetString("currentQueue", "DEFAULT");
			this.allowedInCompetitive = (PlayerPrefs.GetInt("allowedInCompetitive", 0) == 1);
			if (!this.allowedInCompetitive && this.currentQueue == "COMPETITIVE")
			{
				PlayerPrefs.SetString("currentQueue", "DEFAULT");
				PlayerPrefs.Save();
				this.currentQueue = "DEFAULT";
			}
		}

		private void InitializeGroupState()
		{
			this.groupMapJoin = PlayerPrefs.GetString("groupMapJoin", "FOREST");
			this.groupMapJoinIndex = PlayerPrefs.GetInt("groupMapJoinIndex", 0);
			this.allowedMapsToJoin = this.friendJoinCollider.myAllowedMapsToJoin;
		}

		private void InitializeTroopState()
		{
			bool flag = false;
			this.troopToJoin = (this.troopName = PlayerPrefs.GetString("troopName", string.Empty));
			if (!this.rememberTroopQueueState)
			{
				bool flag2 = PlayerPrefs.GetInt("troopQueueActive", 0) == 1;
				bool flag3 = this.currentQueue != "DEFAULT" && this.currentQueue != "COMPETITIVE" && this.currentQueue != "MINIGAMES";
				if (flag2 || flag3)
				{
					this.currentQueue = "DEFAULT";
					PlayerPrefs.SetInt("troopQueueActive", 0);
					PlayerPrefs.SetString("currentQueue", this.currentQueue);
					PlayerPrefs.Save();
				}
			}
			this.troopQueueActive = (PlayerPrefs.GetInt("troopQueueActive", 0) == 1);
			if (this.troopQueueActive && !this.IsValidTroopName(this.troopName))
			{
				this.troopQueueActive = false;
				PlayerPrefs.SetInt("troopQueueActive", this.troopQueueActive ? 1 : 0);
				this.currentQueue = "DEFAULT";
				PlayerPrefs.SetString("currentQueue", this.currentQueue);
				flag = true;
			}
			if (this.troopQueueActive)
			{
				base.StartCoroutine(this.HandleInitialTroopQueueState());
			}
			if (flag)
			{
				PlayerPrefs.Save();
			}
		}

		private IEnumerator HandleInitialTroopQueueState()
		{
			Debug.Log("HandleInitialTroopQueueState()");
			while (!PlayFabCloudScriptAPI.IsEntityLoggedIn())
			{
				yield return null;
			}
			this.RequestTroopPopulation(false);
			while (this.currentTroopPopulation < 0)
			{
				yield return null;
			}
			if (this.currentTroopPopulation < 2)
			{
				Debug.Log("Low population - starting in DEFAULT queue");
				this.JoinDefaultQueue();
			}
			yield break;
		}

		private void InitializeVoiceState()
		{
			Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Voice_Chat);
			string text = PlayerPrefs.GetString("voiceChatOn", "");
			string defaultValue = "FALSE";
			switch (permissionDataByFeature.ManagedBy)
			{
			case Permission.ManagedByEnum.PLAYER:
				if (string.IsNullOrEmpty(text))
				{
					defaultValue = (permissionDataByFeature.Enabled ? "TRUE" : "FALSE");
				}
				else
				{
					defaultValue = text;
				}
				break;
			case Permission.ManagedByEnum.GUARDIAN:
				if (permissionDataByFeature.Enabled)
				{
					text = (string.IsNullOrEmpty(text) ? "FALSE" : text);
					defaultValue = text;
				}
				else
				{
					defaultValue = "FALSE";
				}
				break;
			case Permission.ManagedByEnum.PROHIBITED:
				defaultValue = "FALSE";
				break;
			}
			this.voiceChatOn = PlayerPrefs.GetString("voiceChatOn", defaultValue);
		}

		public void InitializeGameMode(string gameMode)
		{
			this.leftHanded = (PlayerPrefs.GetInt("leftHanded", 0) == 1);
			this.OnModeSelectButtonPress(gameMode, this.leftHanded);
			GameModePages.SetSelectedGameModeShared(gameMode);
			this.didInitializeGameMode = true;
		}

		private void InitializeGameMode()
		{
			if (this.didInitializeGameMode)
			{
				return;
			}
			GorillaComputer.sessionCount = PlayerPrefs.GetInt("sessionCount", -1);
			string text = PlayerPrefs.GetString("currentGameModePostSI");
			if (GorillaComputer.sessionCount == -1)
			{
				GorillaComputer.sessionCount = ((text.Length == 0) ? 0 : 100);
				PlayerPrefs.SetInt("sessionCount", GorillaComputer.sessionCount);
				text = GameModeType.Infection.ToString();
				PlayerPrefs.SetString("currentGameModePostSI", text);
				PlayerPrefs.Save();
			}
			else if (GorillaComputer.sessionCount == 3)
			{
				GorillaComputer.sessionCount++;
				PlayerPrefs.SetInt("sessionCount", GorillaComputer.sessionCount);
				if (!text.StartsWith("Super"))
				{
					text = ((text == GameModeType.Casual.ToString()) ? GameModeType.SuperCasual.ToString() : GameModeType.SuperInfect.ToString());
					PlayerPrefs.SetString("currentGameModePostSI", text);
				}
				PlayerPrefs.Save();
			}
			GameModeType gameModeType;
			try
			{
				gameModeType = Enum.Parse<GameModeType>(text, true);
			}
			catch
			{
				gameModeType = GameModeType.SuperInfect;
				text = GameModeType.SuperInfect.ToString();
			}
			if (!GameMode.GameModeZoneMapping.AllModes.Contains(gameModeType) || gameModeType == GameModeType.None || gameModeType == GameModeType.Count)
			{
				Debug.Log("[GT/GorillaComputer]  InitializeGameMode: Falling back to default game mode " + string.Format("\"{0}\" because stored game mode \"{1}\" is not available in any zone.", GameModeType.SuperInfect, gameModeType));
				PlayerPrefs.SetString("currentGameModePostSI", GameModeType.SuperInfect.ToString());
				PlayerPrefs.Save();
				text = GameModeType.SuperInfect.ToString();
			}
			this.leftHanded = (PlayerPrefs.GetInt("leftHanded", 0) == 1);
			this.OnModeSelectButtonPress(text, this.leftHanded);
			GameModePages.SetSelectedGameModeShared(text);
		}

		private void InitializeCreditsState()
		{
		}

		private void InitializeTimeState()
		{
			BetterDayNightManager.instance.currentSetting = TimeSettings.Normal;
		}

		private void InitializeSupportState()
		{
			this.displaySupport = false;
		}

		private void InitializeVisualsState()
		{
			this.disableParticles = (PlayerPrefs.GetString("disableParticles", "FALSE") == "TRUE");
			GorillaTagger.Instance.ShowCosmeticParticles(!this.disableParticles);
			this.instrumentVolume = PlayerPrefs.GetFloat("instrumentVolume", 0.1f);
		}

		private void InitializeRedeemState()
		{
			this.RedemptionStatus = GorillaComputer.RedemptionResult.Empty;
		}

		private bool CheckInternetConnection()
		{
			return Application.internetReachability > NetworkReachability.NotReachable;
		}

		public void OnConnectedToMasterStuff()
		{
			if (!this.isConnectedToMaster)
			{
				this.isConnectedToMaster = true;
				GorillaServer.Instance.ReturnCurrentVersion(new ReturnCurrentVersionRequest
				{
					CurrentVersion = NetworkSystemConfig.AppVersionStripped,
					UpdatedSynchTest = new int?(this.includeUpdatedServerSynchTest)
				}, new Action<ExecuteFunctionResult>(this.OnReturnCurrentVersion), new Action<PlayFabError>(GorillaComputer.OnErrorShared));
				if (this.startupMillis == 0L && !this.tryGetTimeAgain)
				{
					this.GetCurrentTime();
				}
				RuntimePlatform platform = Application.platform;
				this.SaveModAccountData();
				bool safety = PlayFabAuthenticator.instance.GetSafety();
				if (!KIDManager.KidEnabledAndReady && !KIDManager.HasSession)
				{
					this.SetComputerSettingsBySafety(safety, new GorillaComputer.ComputerState[]
					{
						GorillaComputer.ComputerState.Voice,
						GorillaComputer.ComputerState.AutoMute,
						GorillaComputer.ComputerState.Name,
						GorillaComputer.ComputerState.Group
					}, false);
				}
			}
		}

		private void OnReturnCurrentVersion(ExecuteFunctionResult result)
		{
			JsonObject jsonObject = (JsonObject)result.FunctionResult;
			if (jsonObject == null)
			{
				this.GeneralFailureMessage(this.versionMismatch);
				return;
			}
			object obj;
			if (jsonObject.TryGetValue("SynchTime", out obj))
			{
				Debug.Log("message value is: " + (string)obj);
			}
			if (jsonObject.TryGetValue("Fail", out obj) && (bool)obj)
			{
				this.GeneralFailureMessage(this.versionMismatch);
				return;
			}
			if (jsonObject.TryGetValue("ResultCode", out obj) && (ulong)obj != 0UL)
			{
				this.GeneralFailureMessage(this.versionMismatch);
				return;
			}
			if (jsonObject.TryGetValue("QueueStats", out obj) && ((JsonObject)obj).TryGetValue("TopTroops", out obj))
			{
				this.topTroops.Clear();
				foreach (object obj2 in ((JsonArray)obj))
				{
					this.topTroops.Add(obj2.ToString());
				}
			}
			if (jsonObject.TryGetValue("BannedUsers", out obj))
			{
				this.usersBanned = int.Parse((string)obj);
			}
			this.UpdateScreen();
		}

		public void SaveModAccountData()
		{
			string path = Application.persistentDataPath + "/DoNotShareWithAnyoneEVERNoMatterWhatTheySay.txt";
			if (File.Exists(path))
			{
				return;
			}
			GorillaServer.Instance.ReturnMyOculusHash(delegate(ExecuteFunctionResult result)
			{
				object obj;
				if (((JsonObject)result.FunctionResult).TryGetValue("oculusHash", out obj))
				{
					StreamWriter streamWriter = new StreamWriter(path);
					streamWriter.Write(PlayFabAuthenticator.instance.GetPlayFabPlayerId() + "." + (string)obj);
					streamWriter.Close();
				}
			}, delegate(PlayFabError error)
			{
				if (error.Error == PlayFabErrorCode.NotAuthenticated)
				{
					PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
					return;
				}
				if (error.Error == PlayFabErrorCode.AccountBanned)
				{
					GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
				}
			});
		}

		public void PressButton(GorillaKeyboardBindings buttonPressed)
		{
			if (this.currentState == GorillaComputer.ComputerState.Startup)
			{
				this.ProcessStartupState(buttonPressed);
				this.UpdateScreen();
				return;
			}
			this.RequestTroopPopulation(false);
			bool flag = true;
			if (buttonPressed == GorillaKeyboardBindings.up)
			{
				flag = false;
				this.DecreaseState();
			}
			else if (buttonPressed == GorillaKeyboardBindings.down)
			{
				flag = false;
				this.IncreaseState();
			}
			if (flag)
			{
				switch (this.currentState)
				{
				case GorillaComputer.ComputerState.Name:
					this.ProcessNameState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Turn:
					this.ProcessTurnState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Mic:
					this.ProcessMicState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Room:
					this.ProcessRoomState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Queue:
					this.ProcessQueueState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Group:
					this.ProcessGroupState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Voice:
					this.ProcessVoiceState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.AutoMute:
					this.ProcessAutoMuteState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Credits:
					this.ProcessCreditsState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Visuals:
					this.ProcessVisualsState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.NameWarning:
					this.ProcessNameWarningState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Support:
					this.ProcessSupportState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Troop:
					this.ProcessTroopState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.KID:
					this.ProcessKIdState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Redemption:
					this.ProcessRedemptionState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Language:
					this.ProcessLanguageState(buttonPressed);
					break;
				}
			}
			this.UpdateScreen();
		}

		public void OnModeSelectButtonPress(string gameMode, bool leftHand)
		{
			this.lastPressedGameMode = gameMode;
			this.lastPressedGameModeType = (GameModeType)GameMode.gameModeKeyByName.GetValueOrDefault(gameMode, 11);
			PlayerPrefs.SetString("currentGameModePostSI", gameMode);
			if (leftHand != this.leftHanded)
			{
				PlayerPrefs.SetInt("leftHanded", leftHand ? 1 : 0);
				this.leftHanded = leftHand;
			}
			PlayerPrefs.Save();
			if (FriendshipGroupDetection.Instance.IsInParty)
			{
				FriendshipGroupDetection.Instance.SendRequestPartyGameMode(gameMode);
				return;
			}
			this.SetGameModeWithoutButton(gameMode);
		}

		public void SetGameModeWithoutButton(string gameMode)
		{
			this.currentGameMode.Value = gameMode;
			this.UpdateGameModeText();
			PhotonNetworkController.Instance.UpdateTriggerScreens();
		}

		public void RegisterPrimaryJoinTrigger(GorillaNetworkJoinTrigger trigger)
		{
			this.primaryTriggersByZone[trigger.networkZone] = trigger;
		}

		private GorillaNetworkJoinTrigger GetSelectedMapJoinTrigger()
		{
			GorillaNetworkJoinTrigger result;
			this.primaryTriggersByZone.TryGetValue(this.allowedMapsToJoin[Mathf.Min(this.allowedMapsToJoin.Length - 1, this.groupMapJoinIndex)], out result);
			return result;
		}

		public GorillaNetworkJoinTrigger GetJoinTriggerForZone(string zone)
		{
			GorillaNetworkJoinTrigger result;
			this.primaryTriggersByZone.TryGetValue(zone, out result);
			return result;
		}

		public GorillaNetworkJoinTrigger GetJoinTriggerFromFullGameModeString(string gameModeString)
		{
			foreach (KeyValuePair<string, GorillaNetworkJoinTrigger> keyValuePair in this.primaryTriggersByZone)
			{
				if (gameModeString.StartsWith(keyValuePair.Key))
				{
					return keyValuePair.Value;
				}
			}
			return null;
		}

		public void OnGroupJoinButtonPress(int mapJoinIndex, GorillaFriendCollider chosenFriendJoinCollider)
		{
			Debug.Log("On Group button press. Map:" + mapJoinIndex.ToString() + " - collider: " + chosenFriendJoinCollider.name);
			if (mapJoinIndex >= this.allowedMapsToJoin.Length)
			{
				this.roomNotAllowed = true;
				this.currentStateIndex = 0;
				this.SwitchState(this.GetState(this.currentStateIndex), true);
				return;
			}
			GorillaNetworkJoinTrigger selectedMapJoinTrigger = this.GetSelectedMapJoinTrigger();
			if (!FriendshipGroupDetection.Instance.IsInParty)
			{
				if (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.SessionIsPrivate)
				{
					PhotonNetworkController.Instance.FriendIDList = new List<string>(chosenFriendJoinCollider.playerIDsCurrentlyTouching);
					foreach (string str in this.networkController.FriendIDList)
					{
						Debug.Log("Friend ID:" + str);
					}
					PhotonNetworkController.Instance.shuffler = Random.Range(0, 99).ToString().PadLeft(2, '0') + Random.Range(0, 99999999).ToString().PadLeft(8, '0');
					PhotonNetworkController.Instance.keyStr = Random.Range(0, 99999999).ToString().PadLeft(8, '0');
					RoomSystem.SendNearbyFollowCommand(chosenFriendJoinCollider, PhotonNetworkController.Instance.shuffler, PhotonNetworkController.Instance.keyStr);
					PhotonNetwork.SendAllOutgoingCommands();
					PhotonNetworkController.Instance.AttemptToJoinPublicRoom(selectedMapJoinTrigger, JoinType.JoinWithNearby, null);
					this.currentStateIndex = 0;
					this.SwitchState(this.GetState(this.currentStateIndex), true);
				}
				return;
			}
			if (selectedMapJoinTrigger != null && selectedMapJoinTrigger.CanPartyJoin())
			{
				PhotonNetworkController.Instance.AttemptToJoinPublicRoom(selectedMapJoinTrigger, JoinType.ForceJoinWithParty, null);
				this.currentStateIndex = 0;
				this.SwitchState(this.GetState(this.currentStateIndex), true);
				return;
			}
			this.UpdateScreen();
		}

		public void CompQueueUnlockButtonPress()
		{
			this.allowedInCompetitive = true;
			PlayerPrefs.SetInt("allowedInCompetitive", 1);
			PlayerPrefs.Save();
			if (RankedProgressionManager.Instance != null)
			{
				RankedProgressionManager.Instance.RequestUnlockCompetitiveQueue(true);
			}
		}

		private void SwitchState(GorillaComputer.ComputerState newState, bool clearStack = true)
		{
			if (this.currentComputerState == GorillaComputer.ComputerState.Mic && this.currentComputerState != newState)
			{
				this.updateCooldown = this.defaultUpdateCooldown;
			}
			else if (newState == GorillaComputer.ComputerState.Mic)
			{
				this.updateCooldown = this.micUpdateCooldown;
			}
			if (this.previousComputerState != this.currentComputerState)
			{
				this.previousComputerState = this.currentComputerState;
			}
			this.currentComputerState = newState;
			if (this.LoadingRoutine != null)
			{
				base.StopCoroutine(this.LoadingRoutine);
			}
			if (clearStack)
			{
				this.stateStack.Clear();
			}
			this.stateStack.Push(newState);
		}

		private void PopState()
		{
			this.currentComputerState = this.previousComputerState;
			if (this.stateStack.Count <= 1)
			{
				Debug.LogError("Can't pop into an empty stack");
				return;
			}
			this.stateStack.Pop();
			this.UpdateScreen();
		}

		private void SwitchToWarningState()
		{
			this.warningConfirmationInputString = string.Empty;
			this.SwitchState(GorillaComputer.ComputerState.NameWarning, false);
		}

		private void SwitchToLoadingState()
		{
			this.SwitchState(GorillaComputer.ComputerState.Loading, false);
		}

		private void ProcessStartupState(GorillaKeyboardBindings buttonPressed)
		{
			this.SwitchState(this.GetState(this.currentStateIndex), true);
		}

		private void ProcessColorState(GorillaKeyboardBindings buttonPressed)
		{
			switch (buttonPressed)
			{
			case GorillaKeyboardBindings.enter:
				return;
			case GorillaKeyboardBindings.option1:
				this.colorCursorLine = 0;
				return;
			case GorillaKeyboardBindings.option2:
				this.colorCursorLine = 1;
				return;
			case GorillaKeyboardBindings.option3:
				this.colorCursorLine = 2;
				return;
			default:
			{
				int num = (int)buttonPressed;
				if (num < 10)
				{
					switch (this.colorCursorLine)
					{
					case 0:
						this.redText = num.ToString();
						this.redValue = (float)num / 9f;
						PlayerPrefs.SetFloat("redValue", this.redValue);
						break;
					case 1:
						this.greenText = num.ToString();
						this.greenValue = (float)num / 9f;
						PlayerPrefs.SetFloat("greenValue", this.greenValue);
						break;
					case 2:
						this.blueText = num.ToString();
						this.blueValue = (float)num / 9f;
						PlayerPrefs.SetFloat("blueValue", this.blueValue);
						break;
					}
					GorillaTagger.Instance.UpdateColor(this.redValue, this.greenValue, this.blueValue);
					PlayerPrefs.Save();
					if (NetworkSystem.Instance.InRoom)
					{
						GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, new object[]
						{
							this.redValue,
							this.greenValue,
							this.blueValue
						});
					}
				}
				return;
			}
			}
		}

		public void ProcessNameState(GorillaKeyboardBindings buttonPressed)
		{
			if (KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags))
			{
				switch (buttonPressed)
				{
				case GorillaKeyboardBindings.delete:
					if (this.currentName.Length > 0 && this.NametagsEnabled)
					{
						this.currentName = this.currentName.Substring(0, this.currentName.Length - 1);
						return;
					}
					break;
				case GorillaKeyboardBindings.enter:
					if (this.currentName != this.savedName && this.currentName != "" && this.NametagsEnabled)
					{
						this.CheckAutoBanListForPlayerName(this.currentName);
						return;
					}
					break;
				case GorillaKeyboardBindings.option1:
					this.UpdateNametagSetting(!this.NametagsEnabled, true);
					return;
				default:
					if (this.NametagsEnabled && this.currentName.Length < 12 && (buttonPressed < GorillaKeyboardBindings.up || buttonPressed > GorillaKeyboardBindings.option3))
					{
						string str = this.currentName;
						string str2;
						if (buttonPressed >= GorillaKeyboardBindings.up)
						{
							str2 = buttonPressed.ToString();
						}
						else
						{
							int num = (int)buttonPressed;
							str2 = num.ToString();
						}
						this.currentName = str + str2;
					}
					break;
				}
			}
		}

		private void ProcessRoomState(GorillaKeyboardBindings buttonPressed)
		{
			if (this.limitOnlineScreens)
			{
				return;
			}
			bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Groups) && KIDManager.HasPermissionToUseFeature(EKIDFeatures.Multiplayer);
			switch (buttonPressed)
			{
			case GorillaKeyboardBindings.delete:
				if (flag && ((this.playerInVirtualStump && this.roomToJoin.Length > 1) || (!this.playerInVirtualStump && this.roomToJoin.Length > 0)))
				{
					this.roomToJoin = this.roomToJoin.Substring(0, this.roomToJoin.Length - 1);
					return;
				}
				break;
			case GorillaKeyboardBindings.enter:
				if (flag && ((!this.playerInVirtualStump && this.roomToJoin != "") || (this.playerInVirtualStump && this.roomToJoin.Length > 1)))
				{
					this.CheckAutoBanListForRoomName(this.roomToJoin);
					return;
				}
				break;
			case GorillaKeyboardBindings.option1:
				if (FriendshipGroupDetection.Instance.IsInParty)
				{
					FriendshipGroupDetection.Instance.LeaveParty();
					this.DisconnectAfterDelay(1f);
					return;
				}
				NetworkSystem.Instance.ReturnToSinglePlayer();
				return;
			case GorillaKeyboardBindings.option2:
				this.RequestUpdatedPermissions();
				return;
			case GorillaKeyboardBindings.option3:
				break;
			default:
				if (flag && this.roomToJoin.Length < 10)
				{
					string str = this.roomToJoin;
					string str2;
					if (buttonPressed >= GorillaKeyboardBindings.up)
					{
						str2 = buttonPressed.ToString();
					}
					else
					{
						int num = (int)buttonPressed;
						str2 = num.ToString();
					}
					this.roomToJoin = str + str2;
				}
				break;
			}
		}

		private void DisconnectAfterDelay(float seconds)
		{
			GorillaComputer.<DisconnectAfterDelay>d__377 <DisconnectAfterDelay>d__;
			<DisconnectAfterDelay>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<DisconnectAfterDelay>d__.seconds = seconds;
			<DisconnectAfterDelay>d__.<>1__state = -1;
			<DisconnectAfterDelay>d__.<>t__builder.Start<GorillaComputer.<DisconnectAfterDelay>d__377>(ref <DisconnectAfterDelay>d__);
		}

		private void ProcessTurnState(GorillaKeyboardBindings buttonPressed)
		{
			if (buttonPressed < GorillaKeyboardBindings.up)
			{
				GorillaSnapTurn.UpdateAndSaveTurnFactor((int)buttonPressed);
				return;
			}
			string text = string.Empty;
			switch (buttonPressed)
			{
			case GorillaKeyboardBindings.option1:
				text = "SNAP";
				break;
			case GorillaKeyboardBindings.option2:
				text = "SMOOTH";
				break;
			case GorillaKeyboardBindings.option3:
				text = "NONE";
				break;
			}
			if (text.Length > 0)
			{
				GorillaSnapTurn.UpdateAndSaveTurnType(text);
			}
		}

		private void ProcessMicState(GorillaKeyboardBindings buttonPressed)
		{
			switch (buttonPressed)
			{
			case GorillaKeyboardBindings.option1:
				this.pttType = "OPEN MIC";
				PlayerPrefs.SetString("pttType", this.pttType);
				PlayerPrefs.Save();
				return;
			case GorillaKeyboardBindings.option2:
				this.pttType = "PUSH TO TALK";
				PlayerPrefs.SetString("pttType", this.pttType);
				PlayerPrefs.Save();
				return;
			case GorillaKeyboardBindings.option3:
				this.pttType = "PUSH TO MUTE";
				PlayerPrefs.SetString("pttType", this.pttType);
				PlayerPrefs.Save();
				return;
			default:
				return;
			}
		}

		private void ProcessQueueState(GorillaKeyboardBindings buttonPressed)
		{
			if (this.limitOnlineScreens)
			{
				return;
			}
			switch (buttonPressed)
			{
			case GorillaKeyboardBindings.option1:
				this.JoinQueue("DEFAULT", false);
				return;
			case GorillaKeyboardBindings.option2:
				this.JoinQueue("MINIGAMES", false);
				return;
			case GorillaKeyboardBindings.option3:
				if (this.allowedInCompetitive)
				{
					this.JoinQueue("COMPETITIVE", false);
				}
				return;
			default:
				return;
			}
		}

		public void JoinTroop(string newTroopName)
		{
			if (this.IsValidTroopName(newTroopName))
			{
				this.currentTroopPopulation = -1;
				this.troopName = newTroopName;
				PlayerPrefs.SetString("troopName", this.troopName);
				if (this.troopQueueActive)
				{
					this.currentQueue = this.GetQueueNameForTroop(this.troopName);
					PlayerPrefs.SetString("currentQueue", this.currentQueue);
				}
				PlayerPrefs.Save();
				this.JoinTroopQueue();
			}
		}

		public void JoinTroopQueue()
		{
			if (this.IsValidTroopName(this.troopName))
			{
				this.currentTroopPopulation = -1;
				this.JoinQueue(this.GetQueueNameForTroop(this.troopName), true);
				this.RequestTroopPopulation(true);
			}
		}

		private void RequestTroopPopulation(bool forceUpdate = false)
		{
			if (!PlayFabCloudScriptAPI.IsEntityLoggedIn())
			{
				return;
			}
			if (!this.hasRequestedInitialTroopPopulation || forceUpdate)
			{
				if (this.nextPopulationCheckTime > Time.time)
				{
					return;
				}
				this.nextPopulationCheckTime = Time.time + this.troopPopulationCheckCooldown;
				this.hasRequestedInitialTroopPopulation = true;
				GorillaServer.Instance.ReturnQueueStats(new ReturnQueueStatsRequest
				{
					queueName = this.troopName
				}, delegate(ExecuteFunctionResult result)
				{
					Debug.Log("Troop pop received");
					object obj;
					if (((JsonObject)result.FunctionResult).TryGetValue("PlayerCount", out obj))
					{
						this.currentTroopPopulation = int.Parse(obj.ToString());
						if (this.currentComputerState == GorillaComputer.ComputerState.Queue)
						{
							this.UpdateScreen();
							return;
						}
					}
					else
					{
						this.currentTroopPopulation = 0;
					}
				}, delegate(PlayFabError error)
				{
					Debug.LogError(string.Format("Error requesting troop population: {0}", error));
					this.currentTroopPopulation = -1;
				});
			}
		}

		public void JoinDefaultQueue()
		{
			this.JoinQueue("DEFAULT", false);
		}

		public void LeaveTroop()
		{
			if (this.IsValidTroopName(this.troopName))
			{
				this.troopToJoin = this.troopName;
			}
			this.currentTroopPopulation = -1;
			this.troopName = string.Empty;
			PlayerPrefs.SetString("troopName", this.troopName);
			if (this.troopQueueActive)
			{
				this.JoinDefaultQueue();
			}
			PlayerPrefs.Save();
		}

		public string GetCurrentTroop()
		{
			if (this.troopQueueActive)
			{
				return this.troopName;
			}
			return this.currentQueue;
		}

		public int GetCurrentTroopPopulation()
		{
			if (this.troopQueueActive)
			{
				return this.currentTroopPopulation;
			}
			return -1;
		}

		private void JoinQueue(string queueName, bool isTroopQueue = false)
		{
			this.currentQueue = queueName;
			this.troopQueueActive = isTroopQueue;
			this.currentTroopPopulation = -1;
			PlayerPrefs.SetString("currentQueue", this.currentQueue);
			PlayerPrefs.SetInt("troopQueueActive", this.troopQueueActive ? 1 : 0);
			PlayerPrefs.Save();
		}

		private void ProcessGroupState(GorillaKeyboardBindings buttonPressed)
		{
			if (this.limitOnlineScreens)
			{
				return;
			}
			switch (buttonPressed)
			{
			case GorillaKeyboardBindings.one:
				this.groupMapJoin = "FOREST";
				this.groupMapJoinIndex = 0;
				PlayerPrefs.SetString("groupMapJoin", this.groupMapJoin);
				PlayerPrefs.SetInt("groupMapJoinIndex", this.groupMapJoinIndex);
				PlayerPrefs.Save();
				break;
			case GorillaKeyboardBindings.two:
				this.groupMapJoin = "CAVE";
				this.groupMapJoinIndex = 1;
				PlayerPrefs.SetString("groupMapJoin", this.groupMapJoin);
				PlayerPrefs.SetInt("groupMapJoinIndex", this.groupMapJoinIndex);
				PlayerPrefs.Save();
				break;
			case GorillaKeyboardBindings.three:
				this.groupMapJoin = "CANYON";
				this.groupMapJoinIndex = 2;
				PlayerPrefs.SetString("groupMapJoin", this.groupMapJoin);
				PlayerPrefs.SetInt("groupMapJoinIndex", this.groupMapJoinIndex);
				PlayerPrefs.Save();
				break;
			case GorillaKeyboardBindings.four:
				this.groupMapJoin = "CITY";
				this.groupMapJoinIndex = 3;
				PlayerPrefs.SetString("groupMapJoin", this.groupMapJoin);
				PlayerPrefs.SetInt("groupMapJoinIndex", this.groupMapJoinIndex);
				PlayerPrefs.Save();
				break;
			case GorillaKeyboardBindings.five:
				this.groupMapJoin = "CLOUDS";
				this.groupMapJoinIndex = 4;
				PlayerPrefs.SetString("groupMapJoin", this.groupMapJoin);
				PlayerPrefs.SetInt("groupMapJoinIndex", this.groupMapJoinIndex);
				PlayerPrefs.Save();
				break;
			default:
				if (buttonPressed == GorillaKeyboardBindings.enter)
				{
					this.OnGroupJoinButtonPress(Mathf.Min(this.allowedMapsToJoin.Length - 1, this.groupMapJoinIndex), this.friendJoinCollider);
				}
				break;
			}
			this.roomFull = false;
		}

		private void ProcessTroopState(GorillaKeyboardBindings buttonPressed)
		{
			if (this.limitOnlineScreens)
			{
				return;
			}
			bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Groups);
			bool flag2 = this.IsValidTroopName(this.troopName);
			if (flag)
			{
				switch (buttonPressed)
				{
				case GorillaKeyboardBindings.delete:
					if (!flag2 && this.troopToJoin.Length > 0)
					{
						this.troopToJoin = this.troopToJoin.Substring(0, this.troopToJoin.Length - 1);
						return;
					}
					break;
				case GorillaKeyboardBindings.enter:
					if (!flag2)
					{
						this.CheckAutoBanListForTroopName(this.troopToJoin);
						return;
					}
					break;
				case GorillaKeyboardBindings.option1:
					this.JoinTroopQueue();
					return;
				case GorillaKeyboardBindings.option2:
					this.JoinDefaultQueue();
					return;
				case GorillaKeyboardBindings.option3:
					this.LeaveTroop();
					return;
				default:
					if (!flag2 && this.troopToJoin.Length < 12)
					{
						string str = this.troopToJoin;
						string str2;
						if (buttonPressed >= GorillaKeyboardBindings.up)
						{
							str2 = buttonPressed.ToString();
						}
						else
						{
							int num = (int)buttonPressed;
							str2 = num.ToString();
						}
						this.troopToJoin = str + str2;
						return;
					}
					break;
				}
			}
			else
			{
				switch (buttonPressed)
				{
				case GorillaKeyboardBindings.option1:
					break;
				case GorillaKeyboardBindings.option2:
					if (this._currentScreentState != GorillaComputer.EKidScreenState.Ready)
					{
						this.ProcessScreen_SetupKID();
						return;
					}
					this.RequestUpdatedPermissions();
					return;
				case GorillaKeyboardBindings.option3:
					if (this._currentScreentState != GorillaComputer.EKidScreenState.Show_OTP)
					{
						return;
					}
					this.ProcessScreen_SetupKID();
					break;
				default:
					return;
				}
			}
		}

		private bool IsValidTroopName(string troop)
		{
			return !string.IsNullOrEmpty(troop) && troop.Length <= 12 && (this.allowedInCompetitive || troop != "COMPETITIVE");
		}

		private string GetQueueNameForTroop(string troop)
		{
			return troop;
		}

		private void ProcessVoiceState(GorillaKeyboardBindings buttonPressed)
		{
			if (KIDManager.HasPermissionToUseFeature(EKIDFeatures.Voice_Chat))
			{
				if (buttonPressed != GorillaKeyboardBindings.option1)
				{
					if (buttonPressed == GorillaKeyboardBindings.option2)
					{
						this.SetVoice(false, true);
					}
				}
				else
				{
					this.SetVoice(true, true);
				}
			}
			else if (buttonPressed != GorillaKeyboardBindings.option2)
			{
				if (buttonPressed == GorillaKeyboardBindings.option3)
				{
					if (this._currentScreentState != GorillaComputer.EKidScreenState.Show_OTP)
					{
						return;
					}
					this.ProcessScreen_SetupKID();
				}
			}
			else if (this._currentScreentState != GorillaComputer.EKidScreenState.Ready)
			{
				this.ProcessScreen_SetupKID();
			}
			else
			{
				this.RequestUpdatedPermissions();
			}
			RigContainer.RefreshAllRigVoices();
		}

		private void ProcessAutoMuteState(GorillaKeyboardBindings buttonPressed)
		{
			switch (buttonPressed)
			{
			case GorillaKeyboardBindings.option1:
				this.autoMuteType = "AGGRESSIVE";
				PlayerPrefs.SetInt("autoMute", 2);
				PlayerPrefs.Save();
				RigContainer.RefreshAllRigVoices();
				break;
			case GorillaKeyboardBindings.option2:
				this.autoMuteType = "MODERATE";
				PlayerPrefs.SetInt("autoMute", 1);
				PlayerPrefs.Save();
				RigContainer.RefreshAllRigVoices();
				break;
			case GorillaKeyboardBindings.option3:
				this.autoMuteType = "OFF";
				PlayerPrefs.SetInt("autoMute", 0);
				PlayerPrefs.Save();
				RigContainer.RefreshAllRigVoices();
				break;
			}
			this.UpdateScreen();
		}

		private void ProcessVisualsState(GorillaKeyboardBindings buttonPressed)
		{
			if (buttonPressed < GorillaKeyboardBindings.up)
			{
				this.instrumentVolume = (float)buttonPressed / 50f;
				PlayerPrefs.SetFloat("instrumentVolume", this.instrumentVolume);
				PlayerPrefs.Save();
				return;
			}
			if (buttonPressed == GorillaKeyboardBindings.option1)
			{
				this.disableParticles = false;
				PlayerPrefs.SetString("disableParticles", "FALSE");
				PlayerPrefs.Save();
				GorillaTagger.Instance.ShowCosmeticParticles(!this.disableParticles);
				return;
			}
			if (buttonPressed != GorillaKeyboardBindings.option2)
			{
				return;
			}
			this.disableParticles = true;
			PlayerPrefs.SetString("disableParticles", "TRUE");
			PlayerPrefs.Save();
			GorillaTagger.Instance.ShowCosmeticParticles(!this.disableParticles);
		}

		private void ProcessCreditsState(GorillaKeyboardBindings buttonPressed)
		{
			if (buttonPressed == GorillaKeyboardBindings.enter)
			{
				this.creditsView.ProcessButtonPress(buttonPressed);
			}
		}

		private void ProcessSupportState(GorillaKeyboardBindings buttonPressed)
		{
			if (buttonPressed == GorillaKeyboardBindings.enter)
			{
				this.displaySupport = true;
			}
		}

		private void ProcessRedemptionState(GorillaKeyboardBindings buttonPressed)
		{
			if (this.RedemptionStatus == GorillaComputer.RedemptionResult.Checking)
			{
				return;
			}
			if (buttonPressed != GorillaKeyboardBindings.delete)
			{
				if (buttonPressed == GorillaKeyboardBindings.enter)
				{
					if (this.redemptionCode != "")
					{
						if (this.redemptionCode.Length < 8)
						{
							this.RedemptionStatus = GorillaComputer.RedemptionResult.Invalid;
							return;
						}
						CodeRedemption.Instance.HandleCodeRedemption(this.redemptionCode);
						this.RedemptionStatus = GorillaComputer.RedemptionResult.Checking;
						return;
					}
					else if (this.RedemptionStatus != GorillaComputer.RedemptionResult.Success)
					{
						this.RedemptionStatus = GorillaComputer.RedemptionResult.Empty;
						return;
					}
				}
				else if (this.redemptionCode.Length < 8 && (buttonPressed < GorillaKeyboardBindings.up || buttonPressed > GorillaKeyboardBindings.option3))
				{
					string str = this.redemptionCode;
					string str2;
					if (buttonPressed >= GorillaKeyboardBindings.up)
					{
						str2 = buttonPressed.ToString();
					}
					else
					{
						int num = (int)buttonPressed;
						str2 = num.ToString();
					}
					this.redemptionCode = str + str2;
				}
			}
			else if (this.redemptionCode.Length > 0)
			{
				this.redemptionCode = this.redemptionCode.Substring(0, this.redemptionCode.Length - 1);
				return;
			}
		}

		private void ProcessNameWarningState(GorillaKeyboardBindings buttonPressed)
		{
			if (this.warningConfirmationInputString.ToLower() == "yes")
			{
				this.PopState();
				return;
			}
			if (buttonPressed == GorillaKeyboardBindings.delete)
			{
				if (this.warningConfirmationInputString.Length > 0)
				{
					this.warningConfirmationInputString = this.warningConfirmationInputString.Substring(0, this.warningConfirmationInputString.Length - 1);
					return;
				}
			}
			else if (this.warningConfirmationInputString.Length < 3)
			{
				this.warningConfirmationInputString += buttonPressed.ToString();
			}
		}

		public void UpdateScreen()
		{
			if (NetworkSystem.Instance != null && !NetworkSystem.Instance.WrongVersion)
			{
				this.UpdateFunctionScreen();
				switch (this.currentState)
				{
				case GorillaComputer.ComputerState.Startup:
					this.StartupScreen();
					break;
				case GorillaComputer.ComputerState.Name:
					this.NameScreen();
					break;
				case GorillaComputer.ComputerState.Turn:
					this.TurnScreen();
					break;
				case GorillaComputer.ComputerState.Mic:
					this.MicScreen();
					break;
				case GorillaComputer.ComputerState.Room:
					this.RoomScreen();
					break;
				case GorillaComputer.ComputerState.Queue:
					this.QueueScreen();
					break;
				case GorillaComputer.ComputerState.Group:
					this.GroupScreen();
					break;
				case GorillaComputer.ComputerState.Voice:
					this.VoiceScreen();
					break;
				case GorillaComputer.ComputerState.AutoMute:
					this.AutomuteScreen();
					break;
				case GorillaComputer.ComputerState.Credits:
					this.CreditsScreen();
					break;
				case GorillaComputer.ComputerState.Visuals:
					this.VisualsScreen();
					break;
				case GorillaComputer.ComputerState.Time:
					this.TimeScreen();
					break;
				case GorillaComputer.ComputerState.NameWarning:
					this.NameWarningScreen();
					break;
				case GorillaComputer.ComputerState.Loading:
					this.LoadingScreen();
					break;
				case GorillaComputer.ComputerState.Support:
					this.SupportScreen();
					break;
				case GorillaComputer.ComputerState.Troop:
					this.TroopScreen();
					break;
				case GorillaComputer.ComputerState.KID:
					this.KIdScreen();
					break;
				case GorillaComputer.ComputerState.Redemption:
					this.RedemptionScreen();
					break;
				case GorillaComputer.ComputerState.Language:
					this.LanguageScreen();
					break;
				}
			}
			this.UpdateGameModeText();
		}

		private void LoadingScreen()
		{
			GorillaComputer.<>c__DisplayClass403_0 CS$<>8__locals1 = new GorillaComputer.<>c__DisplayClass403_0();
			CS$<>8__locals1.<>4__this = this;
			string defaultResult = "LOADING";
			LocalisationManager.TryGetKeyForCurrentLocale("LOADING_SCREEN", out CS$<>8__locals1.result, defaultResult);
			this.screenText.Set(CS$<>8__locals1.result);
			this.LoadingRoutine = base.StartCoroutine(CS$<>8__locals1.<LoadingScreen>g__LoadingScreenLocal|0());
		}

		private void NameWarningScreen()
		{
			string defaultResult = "<color=red>WARNING: PLEASE CHOOSE A BETTER NAME\n\nENTERING ANOTHER BAD NAME WILL RESULT IN A BAN</color>";
			string text;
			LocalisationManager.TryGetKeyForCurrentLocale("WARNING_SCREEN", out text, defaultResult);
			this.screenText.Set(text);
			if (this.warningConfirmationInputString.ToLower() == "yes")
			{
				defaultResult = "\n\nPRESS ANY KEY TO CONTINUE";
				LocalisationManager.TryGetKeyForCurrentLocale("WARNING_SCREEN_CONFIRMATION", out text, defaultResult);
				this.screenText.Append(text);
				return;
			}
			defaultResult = "\n\nTYPE 'YES' TO CONFIRM:";
			LocalisationManager.TryGetKeyForCurrentLocale("WARNING_SCREEN_TYPE_YES", out text, defaultResult);
			this.screenText.Append(text.TrailingSpace());
			this.screenText.Append(this.warningConfirmationInputString);
		}

		private void SupportScreen()
		{
			this.screenText.Set("");
			if (this.displaySupport)
			{
				string text = PlayFabAuthenticator.instance.platform.ToString().ToUpper();
				string text2;
				if (text == "PC")
				{
					text2 = "OCULUS PC";
				}
				else
				{
					text2 = text;
				}
				text = text2;
				string key;
				if (!(text == "OCULUS PC"))
				{
					if (!(text == "STEAM"))
					{
						if (!(text == "PSVR"))
						{
							if (!(text == "PICO"))
							{
								if (!(text == "QUEST"))
								{
									key = "UNKNOWN_PLATFORM";
								}
								else
								{
									key = "PLATFORM_QUEST";
								}
							}
							else
							{
								key = "PLATFORM_PICO";
							}
						}
						else
						{
							key = "PLATFORM_PSVR";
						}
					}
					else
					{
						key = "PLATFORM_STEAM";
					}
				}
				else
				{
					key = "PLATFORM_OCULUS_PC";
				}
				string text3;
				LocalisationManager.TryGetKeyForCurrentLocale(key, out text3, text);
				text = text3;
				string defaultResult = "SUPPORT";
				LocalisationManager.TryGetKeyForCurrentLocale("SUPPORT_SCREEN_INTRO", out text3, defaultResult);
				this.screenText.Append(text3);
				defaultResult = "\n\nPLAYER ID";
				LocalisationManager.TryGetKeyForCurrentLocale("SUPPORT_SCREEN_DETAILS_PLAYERID", out text3, defaultResult);
				this.screenText.Append(text3 + "  ");
				this.screenText.Append(PlayFabAuthenticator.instance.GetPlayFabPlayerId());
				defaultResult = "\nVERSION";
				LocalisationManager.TryGetKeyForCurrentLocale("SUPPORT_SCREEN_DETAILS_VERSION", out text3, defaultResult);
				this.screenText.Append(text3 + " ");
				this.screenText.Append(this.version.ToUpper());
				defaultResult = "\nPLATFORM";
				LocalisationManager.TryGetKeyForCurrentLocale("SUPPORT_SCREEN_DETAILS_PLATFORM", out text3, defaultResult);
				this.screenText.Append(text3 + " ");
				this.screenText.Append(text);
				defaultResult = "\nBUILD DATE";
				LocalisationManager.TryGetKeyForCurrentLocale("SUPPORT_SCREEN_DETAILS_BUILD_DATE", out text3, defaultResult);
				this.screenText.Append(text3 + " ");
				this.screenText.Append(this.buildDate);
				defaultResult = "\nSESSION ID";
				LocalisationManager.TryGetKeyForCurrentLocale("SUPPORT_SCREEN_DETAILS_MOTHERSHIP_SESSION_ID", out text3, defaultResult);
				string sessionId = MothershipClientApiUnity.SessionId;
				string str = sessionId;
				int num = sessionId.LastIndexOf('-');
				if (num >= 0)
				{
					string str2 = sessionId.Substring(0, num);
					string str3 = "\n            ";
					text2 = sessionId;
					int num2 = num + 1;
					str = str2 + str3 + text2.Substring(num2, text2.Length - num2);
				}
				this.screenText.Append(text3 + " ");
				this.screenText.Append(str);
				if (KIDManager.KidEnabled)
				{
					defaultResult = "\nk-ID ACCOUNT TYPE:";
					LocalisationManager.TryGetKeyForCurrentLocale("SUPPORT_KID_ACCOUNT_TYPE", out text3, defaultResult);
					this.screenText.Append(text3.TrailingSpace());
					this.screenText.Append(KIDManager.GetActiveAccountStatusNiceString().ToUpper());
					return;
				}
			}
			else
			{
				string defaultResult2 = "SUPPORT";
				string str4;
				LocalisationManager.TryGetKeyForCurrentLocale("SUPPORT_SCREEN_INTRO", out str4, defaultResult2);
				this.screenText.Append(str4);
				defaultResult2 = "\n\nPRESS ENTER TO DISPLAY SUPPORT AND ACCOUNT INFORMATION";
				LocalisationManager.TryGetKeyForCurrentLocale("SUPPORT_SCREEN_INITIAL", out str4, defaultResult2);
				this.screenText.Append(str4);
				defaultResult2 = "\n\n\n\n<color=red>DO NOT SHARE ACCOUNT INFORMATION WITH ANYONE OTHER THAN ANOTHER AXIOM</color>";
				LocalisationManager.TryGetKeyForCurrentLocale("SUPPORT_SCREEN_INITIAL_WARNING", out str4, defaultResult2);
				this.screenText.Append(str4);
			}
		}

		private void TimeScreen()
		{
			string defaultResult = "UPDATE TIME SETTINGS. (LOCALLY ONLY). \nPRESS OPTION 1 FOR NORMAL MODE. \nPRESS OPTION 2 FOR STATIC MODE. \nPRESS 1-10 TO CHANGE TIME OF DAY. \nCURRENT MODE: {currentSetting}.\nTIME OF DAY: {currentTimeOfDay}.\n";
			string text;
			LocalisationManager.TryGetKeyForCurrentLocale("TIME_SCREEN", out text, defaultResult);
			text = text.Replace("{currentSetting}", BetterDayNightManager.instance.currentSetting.ToString().ToUpper()).Replace("{currentTimeOfDay}", BetterDayNightManager.instance.currentTimeOfDay.ToUpper());
			this.screenText.Set(text);
		}

		private void CreditsScreen()
		{
			this.screenText.Set(this.creditsView.GetScreenText());
		}

		private void VisualsScreen()
		{
			string defaultResult = "UPDATE ITEMS SETTINGS.";
			string text;
			LocalisationManager.TryGetKeyForCurrentLocale("VISUALS_SCREEN_INTRO", out text, defaultResult);
			this.screenText.Set(text.TrailingSpace());
			defaultResult = "PRESS OPTION 1 TO ENABLE ITEM PARTICLES. PRESS OPTION 2 TO DISABLE ITEM PARTICLES. PRESS 1-10 TO CHANGE INSTRUMENT VOLUME FOR OTHER PLAYERS.";
			LocalisationManager.TryGetKeyForCurrentLocale("VISUALS_SCREEN_OPTIONS", out text, defaultResult);
			this.screenText.Append(text);
			defaultResult = "\n\nITEM PARTICLES ON:";
			LocalisationManager.TryGetKeyForCurrentLocale("VISUALS_SCREEN_CURRENT", out text, defaultResult);
			this.screenText.Append(text.TrailingSpace());
			string text2 = this.disableParticles ? "FALSE" : "TRUE";
			LocalisationManager.TryGetKeyForCurrentLocale(text2, out text, text2);
			this.screenText.Append(text);
			defaultResult = "\nINSTRUMENT VOLUME:";
			LocalisationManager.TryGetKeyForCurrentLocale("VISUALS_SCREEN_VOLUME", out text, defaultResult);
			this.screenText.Append(text.TrailingSpace());
			this.screenText.Append(Mathf.CeilToInt(this.instrumentVolume * 50f).ToString());
		}

		private void VoiceScreen()
		{
			Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Voice_Chat);
			if (KIDManager.HasPermissionToUseFeature(EKIDFeatures.Voice_Chat))
			{
				string defaultResult = "CHOOSE WHICH TYPE OF VOICE YOU WANT TO HEAR AND SPEAK.";
				string text;
				LocalisationManager.TryGetKeyForCurrentLocale("VOICE_CHAT_SCREEN_INTRO", out text, defaultResult);
				this.screenText.Set(text);
				defaultResult = "\nPRESS OPTION 1 = HUMAN VOICES.\nPRESS OPTION 2 = MONKE VOICES.";
				LocalisationManager.TryGetKeyForCurrentLocale("VOICE_CHAT_SCREEN_OPTIONS", out text, defaultResult);
				this.screenText.Append(text);
				defaultResult = "\n\nVOICE TYPE:";
				LocalisationManager.TryGetKeyForCurrentLocale("VOICE_CHAT_SCREEN_CURRENT", out text, defaultResult);
				this.screenText.Append(text.TrailingSpace());
				string key = (this.voiceChatOn == "TRUE") ? "VOICE_OPTION_HUMAN" : ((this.voiceChatOn == "FALSE") ? "VOICE_OPTION_MONKE" : "VOICE_OPTION_OFF");
				defaultResult = ((this.voiceChatOn == "TRUE") ? "HUMAN" : ((this.voiceChatOn == "FALSE") ? "MONKE" : "OFF"));
				LocalisationManager.TryGetKeyForCurrentLocale(key, out text, defaultResult);
				this.screenText.Append(text);
				return;
			}
			if (permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.PROHIBITED)
			{
				this.VoiceScreen_KIdProhibited();
				return;
			}
			this.VoiceScreen_Permission();
		}

		private void AutomuteScreen()
		{
			string defaultResult = "AUTOMOD AUTOMATICALLY MUTES PLAYERS WHEN THEY JOIN YOUR ROOM IF A LOT OF OTHER PLAYERS HAVE MUTED THEM";
			string text;
			LocalisationManager.TryGetKeyForCurrentLocale("AUTOMOD_SCREEN_INTRO", out text, defaultResult);
			this.screenText.Set(text);
			defaultResult = "\nPRESS OPTION 1 FOR AGGRESSIVE MUTING\nPRESS OPTION 2 FOR MODERATE MUTING\nPRESS OPTION 3 TO TURN AUTOMOD OFF";
			LocalisationManager.TryGetKeyForCurrentLocale("AUTOMOD_SCREEN_OPTIONS", out text, defaultResult);
			this.screenText.Append(text);
			defaultResult = "\n\nCURRENT AUTOMOD LEVEL: ";
			LocalisationManager.TryGetKeyForCurrentLocale("AUTOMOD_SCREEN_CURRENT", out text, defaultResult);
			this.screenText.Append(text.TrailingSpace());
			string key = "AUTOMOD_OFF";
			string a = this.autoMuteType;
			if (!(a == "OFF"))
			{
				if (!(a == "MODERATE"))
				{
					if (a == "AGGRESSIVE")
					{
						key = "AUTOMOD_AGGRESSIVE";
					}
				}
				else
				{
					key = "AUTOMOD_MODERATE";
				}
			}
			else
			{
				key = "AUTOMOD_OFF";
			}
			LocalisationManager.TryGetKeyForCurrentLocale(key, out text, this.autoMuteType);
			this.screenText.Append(text);
		}

		private void GroupScreen()
		{
			if (this.limitOnlineScreens)
			{
				this.LimitedOnlineFunctionalityScreen();
				return;
			}
			string text = "";
			string str = (this.allowedMapsToJoin.Length > 1) ? this.groupMapJoin : this.allowedMapsToJoin[0].ToUpper();
			string str2 = "";
			string defaultResult;
			if (this.allowedMapsToJoin.Length > 1)
			{
				defaultResult = "\n\nUSE NUMBER KEYS TO SELECT DESTINATION\n1: FOREST, 2: CAVE, 3: CANYON, 4: CITY, 5: CLOUDS.";
				LocalisationManager.TryGetKeyForCurrentLocale("GROUP_SCREEN_DESTINATIONS", out text, defaultResult);
				str2 = text;
			}
			defaultResult = "\n\nACTIVE ZONE WILL BE:";
			LocalisationManager.TryGetKeyForCurrentLocale("GROUP_SCREEN_ACTIVE_ZONES", out text, defaultResult);
			string text2 = text.TrailingSpace();
			text2 = text2 + str + str2;
			if (FriendshipGroupDetection.Instance.IsInParty)
			{
				GorillaNetworkJoinTrigger selectedMapJoinTrigger = this.GetSelectedMapJoinTrigger();
				string str3 = "";
				if (selectedMapJoinTrigger.CanPartyJoin())
				{
					defaultResult = "\n\n<color=red>CANNOT JOIN BECAUSE YOUR GROUP IS NOT HERE</color>";
					LocalisationManager.TryGetKeyForCurrentLocale("GROUP_SCREEN_CANNOT_JOIN", out text, defaultResult);
					str3 = text;
				}
				defaultResult = "PRESS ENTER TO JOIN A PUBLIC GAME WITH YOUR FRIENDSHIP GROUP.";
				LocalisationManager.TryGetKeyForCurrentLocale("GROUP_SCREEN_ENTER_PARTY", out text, defaultResult);
				this.screenText.Set(text);
				text2 += str3;
				this.screenText.Append(text2);
				return;
			}
			defaultResult = "PRESS ENTER TO JOIN A PUBLIC GAME AND BRING EVERYONE IN THIS ROOM WITH YOU.";
			LocalisationManager.TryGetKeyForCurrentLocale("GROUP_SCREEN_ENTER_NOPARTY", out text, defaultResult);
			this.screenText.Set(text);
			this.screenText.Append(text2);
		}

		private void MicScreen()
		{
			if (KIDManager.GetPermissionDataByFeature(EKIDFeatures.Voice_Chat).ManagedBy == Permission.ManagedByEnum.PROHIBITED)
			{
				this.MicScreen_KIdProhibited();
				return;
			}
			bool flag = false;
			string str = "";
			if (Microphone.devices.Length == 0)
			{
				flag = true;
				str = "NO MICROPHONE DETECTED";
			}
			if (flag)
			{
				string str2;
				LocalisationManager.TryGetKeyForCurrentLocale("MIC_SCREEN_MIC_DISABLED", out str2, "MIC DISABLED: ");
				this.screenText.Set(str2 + str);
				return;
			}
			string defaultResult = "PRESS OPTION 1 = ALL CHAT.\nPRESS OPTION 2 = PUSH TO TALK.\nPRESS OPTION 3 = PUSH TO MUTE.";
			string text;
			LocalisationManager.TryGetKeyForCurrentLocale("MIC_SCREEN_OPTIONS", out text, defaultResult);
			this.screenText.Set(text);
			defaultResult = "\n\nCURRENT MIC SETTING:";
			LocalisationManager.TryGetKeyForCurrentLocale("MIC_SCREEN_CURRENT", out text, defaultResult);
			this.screenText.Append(text.TrailingSpace());
			string key = "";
			string a = this.pttType;
			if (!(a == "PUSH TO MUTE"))
			{
				if (!(a == "PUSH TO TALK"))
				{
					if (!(a == "OPEN MIC"))
					{
						if (a == "ALL CHAT")
						{
							key = "OPEN_MIC";
						}
					}
					else
					{
						key = "OPEN_MIC";
					}
				}
				else
				{
					key = "PUSH_TO_TALK_MIC";
				}
			}
			else
			{
				key = "PUSH_TO_MUTE_MIC";
			}
			LocalisationManager.TryGetKeyForCurrentLocale(key, out text, this.pttType);
			this.screenText.Append(text);
			if (this.pttType == "PUSH TO MUTE")
			{
				defaultResult = "- MIC IS OPEN.\n- HOLD ANY FACE BUTTON TO MUTE.\n\n";
				LocalisationManager.TryGetKeyForCurrentLocale("MIC_SCREEN_PUSH_TO_MUTE_TOOLTIP", out text, defaultResult);
				this.screenText.Append(text);
			}
			else if (this.pttType == "PUSH TO TALK")
			{
				defaultResult = "- MIC IS MUTED.\n- HOLD ANY FACE BUTTON TO TALK.\n\n";
				LocalisationManager.TryGetKeyForCurrentLocale("MIC_SCREEN_PUSH_TO_TALK_TOOLTIP", out text, defaultResult);
				this.screenText.Append(text);
			}
			else
			{
				this.screenText.Append("\n\n\n");
			}
			if (this.speakerLoudness == null)
			{
				this.speakerLoudness = GorillaTagger.Instance.offlineVRRig.GetComponent<GorillaSpeakerLoudness>();
			}
			if (this.speakerLoudness != null)
			{
				float num = Mathf.Sqrt(this.speakerLoudness.LoudnessNormalized);
				if (num <= 0.01f)
				{
					this.micInputTestTimer += this.deltaTime;
				}
				else
				{
					this.micInputTestTimer = 0f;
				}
				if (this.pttType != "OPEN MIC")
				{
					bool flag2 = ControllerInputPoller.PrimaryButtonPress(XRNode.RightHand);
					bool flag3 = ControllerInputPoller.SecondaryButtonPress(XRNode.RightHand);
					bool flag4 = ControllerInputPoller.PrimaryButtonPress(XRNode.LeftHand);
					bool flag5 = ControllerInputPoller.SecondaryButtonPress(XRNode.LeftHand);
					bool flag6 = flag2 || flag3 || flag4 || flag5;
					if (flag6 && this.pttType == "PUSH TO MUTE")
					{
						defaultResult = "INPUT TEST: ";
						LocalisationManager.TryGetKeyForCurrentLocale("MIC_SCREEN_INPUT_TEST_LABEL", out text, defaultResult);
						this.screenText.Append(text);
						return;
					}
					if (!flag6 && this.pttType == "PUSH TO TALK")
					{
						defaultResult = "INPUT TEST: ";
						LocalisationManager.TryGetKeyForCurrentLocale("MIC_SCREEN_INPUT_TEST_LABEL", out text, defaultResult);
						this.screenText.Append(text);
						return;
					}
				}
				if (this.micInputTestTimer >= this.micInputTestTimerThreshold)
				{
					defaultResult = "NO MIC INPUT DETECTED. CHECK MIC SETTINGS IN THE OPERATING SYSTEM.";
					LocalisationManager.TryGetKeyForCurrentLocale("MIC_SCREEN_INPUT_TEST_NO_MIC", out text, defaultResult);
					this.screenText.Append(text);
					return;
				}
				defaultResult = "INPUT TEST: ";
				LocalisationManager.TryGetKeyForCurrentLocale("MIC_SCREEN_INPUT_TEST_LABEL", out text, defaultResult);
				this.screenText.Append(text);
				for (int i = 0; i < Mathf.FloorToInt(num * 50f); i++)
				{
					this.screenText.Append("|");
				}
			}
		}

		private void QueueScreen()
		{
			if (this.limitOnlineScreens)
			{
				this.LimitedOnlineFunctionalityScreen();
				return;
			}
			string defaultResult = "THIS OPTION AFFECTS WHO YOU PLAY WITH. DEFAULT IS FOR ANYONE TO PLAY NORMALLY. MINIGAMES IS FOR PEOPLE LOOKING TO PLAY WITH THEIR OWN MADE UP RULES.";
			string text;
			LocalisationManager.TryGetKeyForCurrentLocale("QUEUE_SCREEN", out text, defaultResult);
			this.screenText.Set(text.TrailingSpace());
			if (this.allowedInCompetitive)
			{
				defaultResult = "COMPETITIVE IS FOR PLAYERS WHO WANT TO PLAY THE GAME AND TRY AS HARD AS THEY CAN.";
				LocalisationManager.TryGetKeyForCurrentLocale("COMPETITIVE_DESC", out text, defaultResult);
				this.screenText.Append(text.TrailingSpace());
				defaultResult = "PRESS OPTION 1 FOR DEFAULT, OPTION 2 FOR MINIGAMES, OR OPTION 3 FOR COMPETITIVE.";
				LocalisationManager.TryGetKeyForCurrentLocale("QUEUE_SCREEN_ALL_QUEUES", out text, defaultResult);
				this.screenText.Append(text);
			}
			else
			{
				defaultResult = "BEAT THE OBSTACLE COURSE IN CITY TO ALLOW COMPETITIVE PLAY.";
				LocalisationManager.TryGetKeyForCurrentLocale("BEAT_OBSTACLE_COURSE", out text, defaultResult);
				this.screenText.Append(text.TrailingSpace());
				defaultResult = "PRESS OPTION 1 FOR DEFAULT, OR OPTION 2 FOR MINIGAMES.";
				LocalisationManager.TryGetKeyForCurrentLocale("QUEUE_SCREEN_DEFAULT_QUEUES", out text, defaultResult);
				this.screenText.Append(text);
			}
			defaultResult = "\n\nCURRENT QUEUE:";
			LocalisationManager.TryGetKeyForCurrentLocale("CURRENT_QUEUE", out text, defaultResult);
			this.screenText.Append(text.TrailingSpace());
			string a = this.currentQueue;
			string key;
			if (!(a == "DEFAULT"))
			{
				if (a == "COMPETITIVE")
				{
					key = "COMPETITIVE_QUEUE";
					goto IL_137;
				}
				if (a == "MINIGAMES")
				{
					key = "MINIGAMES_QUEUE";
					goto IL_137;
				}
			}
			key = "DEFAULT_QUEUE";
			IL_137:
			defaultResult = this.currentQueue;
			LocalisationManager.TryGetKeyForCurrentLocale(key, out text, defaultResult);
			this.screenText.Append(text);
		}

		private void TroopScreen()
		{
			if (this.limitOnlineScreens)
			{
				this.LimitedOnlineFunctionalityScreen();
				return;
			}
			Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Groups);
			Permission permissionDataByFeature2 = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Multiplayer);
			bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Groups) && KIDManager.HasPermissionToUseFeature(EKIDFeatures.Multiplayer);
			bool flag2 = this.IsValidTroopName(this.troopName);
			this.screenText.Set(string.Empty);
			string text = "";
			string defaultResult;
			if (flag)
			{
				defaultResult = "PLAY WITH A PERSISTENT GROUP ACROSS MULTIPLE ROOMS.";
				LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_INTRO", out text, defaultResult);
				this.screenText.Set(text);
				if (!flag2)
				{
					defaultResult = " PRESS ENTER TO JOIN OR CREATE A TROOP.";
					LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_INSTRUCTIONS", out text, defaultResult);
					this.screenText.Append(text);
				}
			}
			defaultResult = "\n\nCURRENT TROOP: ";
			LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_CURRENT_TROOP", out text, defaultResult);
			this.screenText.Append(text.TrailingSpace());
			if (flag2)
			{
				this.screenText.Append(this.troopName ?? "");
				if (flag)
				{
					bool flag3 = this.currentTroopPopulation > -1;
					if (this.troopQueueActive)
					{
						defaultResult = "\n  -IN TROOP QUEUE-";
						LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_IN_QUEUE", out text, defaultResult);
						this.screenText.Append(text);
						if (flag3)
						{
							defaultResult = "\n\nPLAYERS IN TROOP: ";
							LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_PLAYERS_IN_TROOP", out text, defaultResult);
							this.screenText.Append(text.TrailingSpace());
							this.screenText.Append(Mathf.Max(1, this.currentTroopPopulation).ToString());
						}
						defaultResult = "\n\nPRESS OPTION 2 FOR DEFAULT QUEUE.";
						LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_DEFAULT_QUEUE", out text, defaultResult);
						this.screenText.Append(text);
					}
					else
					{
						defaultResult = "\n  -IN {currentQueue} QUEUE-";
						LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_CURRENT_QUEUE", out text, defaultResult);
						string a = this.currentQueue;
						string key;
						if (!(a == "DEFAULT"))
						{
							if (a == "MINIGAMES")
							{
								key = "MINIGAMES_QUEUE";
								goto IL_209;
							}
							if (a == "COMPETITIVE")
							{
								key = "COMPETITIVE_QUEUE";
								goto IL_209;
							}
						}
						key = "DEFAULT_QUEUE";
						IL_209:
						defaultResult = this.currentQueue;
						string newValue;
						LocalisationManager.TryGetKeyForCurrentLocale(key, out newValue, defaultResult);
						text = text.Replace("{currentQueue}", newValue);
						this.screenText.Append(text);
						if (flag3)
						{
							defaultResult = "\n\nPLAYERS IN TROOP: ";
							LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_PLAYERS_IN_TROOP", out text, defaultResult);
							this.screenText.Append(text.TrailingSpace());
							this.screenText.Append(Mathf.Max(1, this.currentTroopPopulation).ToString());
						}
						defaultResult = "\n\nPRESS OPTION 1 FOR TROOP QUEUE.";
						LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_TROOP_QUEUE", out text, defaultResult);
						this.screenText.Append(text);
					}
					defaultResult = "\nPRESS OPTION 3 TO LEAVE YOUR TROOP.";
					LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_LEAVE", out text, defaultResult);
					this.screenText.Append(text);
				}
			}
			else
			{
				defaultResult = "-NOT IN TROOP-";
				LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_NOT_IN_TROOP", out text, defaultResult);
				this.screenText.Append(text);
			}
			if (flag)
			{
				if (!flag2)
				{
					defaultResult = "\n\nTROOP TO JOIN: ";
					LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_JOIN_TROOP", out text, defaultResult);
					this.screenText.Append(text.TrailingSpace());
					this.screenText.Append(this.troopToJoin);
					return;
				}
			}
			else
			{
				if (permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.PROHIBITED || permissionDataByFeature2.ManagedBy == Permission.ManagedByEnum.PROHIBITED)
				{
					this.TroopScreen_KIdProhibited();
					return;
				}
				this.TroopScreen_Permission();
			}
		}

		private void TurnScreen()
		{
			string defaultResult = "PRESS OPTION 1 TO USE SNAP TURN. PRESS OPTION 2 TO USE SMOOTH TURN. PRESS OPTION 3 TO USE NO ARTIFICIAL TURNING.";
			string text = "";
			string text2;
			LocalisationManager.TryGetKeyForCurrentLocale("TURN_SCREEN", out text2, defaultResult);
			text += text2.TrailingSpace();
			defaultResult = "PRESS THE NUMBER KEYS TO CHOOSE A TURNING SPEED.";
			LocalisationManager.TryGetKeyForCurrentLocale("TURN_SCREEN_TURNING_SPEED", out text2, defaultResult);
			text += text2;
			defaultResult = "\n CURRENT TURN TYPE: ";
			LocalisationManager.TryGetKeyForCurrentLocale("TURN_SCREEN_TURN_TYPE", out text2, defaultResult);
			text += text2;
			string key = "TURN_TYPE_NO_TURN";
			string turnType = GorillaSnapTurn.CachedSnapTurnRef.turnType;
			if (!(turnType == "SNAP"))
			{
				if (!(turnType == "SMOOTH"))
				{
					if (!(turnType == "NONE"))
					{
						Debug.LogError("[LOCALIZATION::GORILLA_COMPUTER::TURN] Could not match [" + GorillaSnapTurn.CachedSnapTurnRef.turnType + "] to any case. Defaulting to NO_TURN");
					}
					else
					{
						key = "TURN_TYPE_NO_TURN";
					}
				}
				else
				{
					key = "TURN_TYPE_SMOOTH_TURN";
				}
			}
			else
			{
				key = "TURN_TYPE_SNAP_TURN";
			}
			LocalisationManager.TryGetKeyForCurrentLocale(key, out text2, GorillaSnapTurn.CachedSnapTurnRef.turnType);
			text += text2;
			defaultResult = "\nCURRENT TURN SPEED: ";
			LocalisationManager.TryGetKeyForCurrentLocale("TURN_SCREEN_TURN_SPEED", out text2, defaultResult);
			text += text2;
			text += GorillaSnapTurn.CachedSnapTurnRef.turnFactor.ToString();
			this.screenText.Set(text);
		}

		private void NameScreen()
		{
			Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Custom_Nametags);
			if (KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags))
			{
				string defaultResult = "PRESS ENTER TO CHANGE YOUR NAME TO THE ENTERED NEW NAME.\n\n";
				string text;
				LocalisationManager.TryGetKeyForCurrentLocale("NAME_SCREEN", out text, defaultResult);
				this.screenText.Set(text);
				defaultResult = "CURRENT NAME: ";
				LocalisationManager.TryGetKeyForCurrentLocale("CURRENT_NAME", out text, defaultResult);
				this.screenText.Append(text.TrailingSpace());
				this.screenText.Append(this.savedName);
				if (this.NametagsEnabled)
				{
					defaultResult = "NEW NAME: ";
					LocalisationManager.TryGetKeyForCurrentLocale("NEW_NAME", out text, defaultResult);
					this.screenText.Append(text.TrailingSpace());
					this.screenText.Append(this.currentName);
				}
				defaultResult = "PRESS OPTION 1 TO TOGGLE NAMETAGS.\nCURRENTLY NAMETAGS ARE: ";
				LocalisationManager.TryGetKeyForCurrentLocale("NAME_SCREEN_TOGGLE_NAMETAGS", out text, defaultResult);
				string key = this.NametagsEnabled ? "ON_KEY" : "OFF_KEY";
				this.screenText.Append(text.TrailingSpace());
				defaultResult = (this.NametagsEnabled ? "ON" : "OFF");
				LocalisationManager.TryGetKeyForCurrentLocale(key, out text, defaultResult);
				this.screenText.Append(text);
				return;
			}
			if (permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.PROHIBITED)
			{
				this.NameScreen_KIdProhibited();
				return;
			}
			this.NameScreen_Permission();
		}

		private void StartupScreen()
		{
			string text = string.Empty;
			if (KIDManager.GetActiveAccountStatus() == AgeStatusType.DIGITALMINOR)
			{
				text = "YOU ARE PLAYING ON A MANAGED ACCOUNT. SOME SETTINGS MAY BE DISABLED WITHOUT PARENT OR GUARDIAN APPROVAL\n\n";
				string text2;
				if (LocalisationManager.TryGetKeyForCurrentLocale("STARTUP_MANAGED", out text2, text))
				{
					text = text2;
				}
			}
			string empty = string.Empty;
			string text3;
			LocalisationManager.TryGetKeyForCurrentLocale("STARTUP_INTRO", out text3, "GORILLA OS\n\n");
			this.screenText.Set(text3);
			this.screenText.Append(text);
			LocalisationManager.TryGetKeyForCurrentLocale("STARTUP_PLAYERS_ONLINE", out text3, "{playersOnline} PLAYERS ONLINE\n\n");
			this.screenText.Append(text3.Replace("{playersOnline}", HowManyMonke.ThisMany.ToString()));
			LocalisationManager.TryGetKeyForCurrentLocale("STARTUP_USERS_BANNED", out text3, "{usersBanned} USERS BANNED YESTERDAY\n\n");
			this.screenText.Append(text3.Replace("{usersBanned}", this.usersBanned.ToString()));
			LocalisationManager.TryGetKeyForCurrentLocale("STARTUP_PRESS_KEY", out text3, "PRESS ANY KEY TO BEGIN");
			this.screenText.Append(text3);
		}

		private void ColourScreen()
		{
			string str;
			LocalisationManager.TryGetKeyForCurrentLocale("COLOR_SELECT_INTRO", out str, "USE THE OPTIONS BUTTONS TO SELECT THE COLOR TO UPDATE, THEN PRESS 0-9 TO SET A NEW VALUE.");
			this.screenText.Set(str);
			LocalisationManager.TryGetKeyForCurrentLocale("COLOR_RED", out str, "RED");
			this.screenText.Append("\n\n");
			this.screenText.Append(str);
			this.screenText.Append(Mathf.FloorToInt(this.redValue * 9f).ToString() + ((this.colorCursorLine == 0) ? "<--" : ""));
			LocalisationManager.TryGetKeyForCurrentLocale("COLOR_GREEN", out str, "GREEN");
			this.screenText.Append("\n\n");
			this.screenText.Append(str);
			this.screenText.Append(Mathf.FloorToInt(this.greenValue * 9f).ToString() + ((this.colorCursorLine == 1) ? "<--" : ""));
			LocalisationManager.TryGetKeyForCurrentLocale("COLOR_BLUE", out str, "BLUE");
			this.screenText.Append("\n\n");
			this.screenText.Append(str);
			this.screenText.Append(Mathf.FloorToInt(this.blueValue * 9f).ToString() + ((this.colorCursorLine == 2) ? "<--" : ""));
		}

		private void RoomScreen()
		{
			if (this.limitOnlineScreens)
			{
				this.LimitedOnlineFunctionalityScreen();
				return;
			}
			Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Groups);
			Permission permissionDataByFeature2 = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Multiplayer);
			bool item = KIDManager.CheckFeatureOptIn(EKIDFeatures.Multiplayer, null).Item2;
			bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Groups) && KIDManager.HasPermissionToUseFeature(EKIDFeatures.Multiplayer) && item;
			this.screenText.Set("");
			string text = "";
			string defaultResult;
			if (flag)
			{
				defaultResult = "PRESS ENTER TO JOIN OR CREATE A CUSTOM ROOM WITH THE ENTERED CODE.";
				LocalisationManager.TryGetKeyForCurrentLocale("ROOM_INTRO", out text, defaultResult);
				this.screenText.Append(text.TrailingSpace());
			}
			defaultResult = "PRESS OPTION 1 TO DISCONNECT FROM THE CURRENT ROOM.";
			LocalisationManager.TryGetKeyForCurrentLocale("ROOM_OPTION", out text, defaultResult);
			this.screenText.Append(text.TrailingSpace());
			if (FriendshipGroupDetection.Instance.IsInParty)
			{
				if (FriendshipGroupDetection.Instance.IsPartyWithinCollider(this.friendJoinCollider))
				{
					defaultResult = "YOUR GROUP WILL TRAVEL WITH YOU.";
					LocalisationManager.TryGetKeyForCurrentLocale("ROOM_GROUP_TRAVEL", out text, defaultResult);
					this.screenText.Append(text.TrailingSpace());
				}
				else
				{
					defaultResult = "<color=red>YOU WILL LEAVE YOUR PARTY UNLESS YOU GATHER THEM HERE FIRST!</color> ";
					LocalisationManager.TryGetKeyForCurrentLocale("ROOM_PARTY_WARNING", out text, defaultResult);
					this.screenText.Append(text);
				}
			}
			defaultResult = "\n\nCURRENT ROOM:";
			LocalisationManager.TryGetKeyForCurrentLocale("ROOM_TEXT_CURRENT_ROOM", out text, defaultResult);
			this.screenText.Append(text.TrailingSpace());
			if (NetworkSystem.Instance.InRoom)
			{
				this.screenText.Append(NetworkSystem.Instance.RoomName.TrailingSpace());
				if (NetworkSystem.Instance.SessionIsPrivate)
				{
					GorillaGameManager activeGameMode = GameMode.ActiveGameMode;
					string text2 = (activeGameMode != null) ? activeGameMode.GameModeNameRoomLabel() : null;
					if (!string.IsNullOrEmpty(text2))
					{
						this.screenText.Append(text2 ?? "");
					}
				}
				defaultResult = "\n\nPLAYERS IN ROOM:";
				LocalisationManager.TryGetKeyForCurrentLocale("PLAYERS_IN_ROOM", out text, defaultResult);
				this.screenText.Append(text.TrailingSpace());
				this.screenText.Append(NetworkSystem.Instance.RoomPlayerCount.ToString());
			}
			else
			{
				defaultResult = "-NOT IN ROOM-";
				LocalisationManager.TryGetKeyForCurrentLocale("NOT_IN_ROOM", out text, defaultResult);
				this.screenText.Append(text);
				defaultResult = "\n\nPLAYERS ONLINE:";
				LocalisationManager.TryGetKeyForCurrentLocale("PLAYERS_ONLINE", out text, defaultResult);
				this.screenText.Append(text.TrailingSpace());
				this.screenText.Append(HowManyMonke.ThisMany.ToString());
			}
			if (flag)
			{
				defaultResult = "\n\nROOM TO JOIN:";
				LocalisationManager.TryGetKeyForCurrentLocale("ROOM_TO_JOIN", out text, defaultResult);
				this.screenText.Append(text.TrailingSpace());
				this.screenText.Append(this.roomToJoin);
				if (this.roomFull)
				{
					defaultResult = "\n\nROOM FULL. JOIN ROOM FAILED.";
					LocalisationManager.TryGetKeyForCurrentLocale("ROOM_FULL", out text, defaultResult);
					this.screenText.Append(text);
					return;
				}
				if (this.roomNotAllowed)
				{
					defaultResult = "\n\nCANNOT JOIN ROOM TYPE FROM HERE.";
					LocalisationManager.TryGetKeyForCurrentLocale("ROOM_JOIN_NOT_ALLOWED", out text, defaultResult);
					this.screenText.Append(text);
					return;
				}
			}
			else
			{
				if (permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.PROHIBITED || permissionDataByFeature2.ManagedBy == Permission.ManagedByEnum.PROHIBITED)
				{
					this.RoomScreen_KIdProhibited();
					return;
				}
				this.RoomScreen_Permission();
			}
		}

		private void RedemptionScreen()
		{
			string defaultResult = "TYPE REDEMPTION CODE AND PRESS ENTER";
			string text;
			LocalisationManager.TryGetKeyForCurrentLocale("REDEMPTION_INTRO", out text, defaultResult);
			this.screenText.Set(text);
			defaultResult = "\n\nCODE: " + this.redemptionCode;
			LocalisationManager.TryGetKeyForCurrentLocale("REDEMPTION_CODE_LABEL", out text, defaultResult);
			this.screenText.Append(text.TrailingSpace());
			this.screenText.Append(this.redemptionCode);
			switch (this.RedemptionStatus)
			{
			case GorillaComputer.RedemptionResult.Empty:
				break;
			case GorillaComputer.RedemptionResult.Invalid:
				defaultResult = "\n\nINVALID CODE";
				LocalisationManager.TryGetKeyForCurrentLocale("REDEMPTION_CODE_INVALID", out text, defaultResult);
				this.screenText.Append(text);
				return;
			case GorillaComputer.RedemptionResult.Checking:
				defaultResult = "\n\nVALIDATING...";
				LocalisationManager.TryGetKeyForCurrentLocale("REDEMPTION_CODE_VALIDATING", out text, defaultResult);
				this.screenText.Append(text);
				return;
			case GorillaComputer.RedemptionResult.AlreadyUsed:
				defaultResult = "\n\nCODE ALREADY CLAIMED";
				LocalisationManager.TryGetKeyForCurrentLocale("REDEMPTION_CODE_ALREADY_USED", out text, defaultResult);
				this.screenText.Append(text);
				return;
			case GorillaComputer.RedemptionResult.TooEarly:
				defaultResult = "CODE IS NOT REDEEMABLE UNTIL";
				LocalisationManager.TryGetKeyForCurrentLocale("REDEMPTION_CODE_TOO_EARLY", out text, defaultResult);
				this.screenText.Append((this.RedemptionRestrictionTime != null) ? ("\n\n" + text + "\n" + this.RedemptionRestrictionTime.Value.ToLocalTime().ToString("f").ToUpper()) : ("\n\n" + text + "\n[MISSING]"));
				return;
			case GorillaComputer.RedemptionResult.TooLate:
				defaultResult = "CODE EXPIRED";
				LocalisationManager.TryGetKeyForCurrentLocale("REDEMPTION_CODE_TOO_LATE", out text, defaultResult);
				this.screenText.Append((this.RedemptionRestrictionTime != null) ? ("\n\n" + text + "\n" + this.RedemptionRestrictionTime.Value.ToLocalTime().ToString("f").ToUpper()) : ("\n\n" + text + "\n[MISSING]"));
				return;
			case GorillaComputer.RedemptionResult.Success:
				defaultResult = "\n\nSUCCESSFULLY CLAIMED!";
				LocalisationManager.TryGetKeyForCurrentLocale("REDEMPTION_CODE_SUCCESS", out text, defaultResult);
				this.screenText.Append(text);
				break;
			default:
				return;
			}
		}

		private void LimitedOnlineFunctionalityScreen()
		{
			string defaultResult = "NOT AVAILABLE IN RANKED PLAY";
			string str;
			LocalisationManager.TryGetKeyForCurrentLocale("LIMITED_ONLINE_FUNC", out str, defaultResult);
			this.screenText.Set(str);
		}

		private void UpdateGameModeText()
		{
			string defaultResult = "CURRENT MODE";
			string text;
			LocalisationManager.TryGetKeyForCurrentLocale("CURRENT_MODE", out text, defaultResult);
			this.currentGameModeText.Value = text;
			if (!NetworkSystem.Instance.InRoom || GorillaGameManager.instance == null)
			{
				defaultResult = "-NOT IN ROOM-";
				LocalisationManager.TryGetKeyForCurrentLocale("NOT_IN_ROOM", out text, defaultResult);
				WatchableStringSO watchableStringSO = this.currentGameModeText;
				watchableStringSO.Value += text;
				return;
			}
			WatchableStringSO watchableStringSO2 = this.currentGameModeText;
			watchableStringSO2.Value = watchableStringSO2.Value + "\n" + GorillaGameManager.instance.GameModeName();
		}

		private void UpdateFunctionScreen()
		{
			this.functionSelectText.Set(this.GetOrderListForScreen(this.currentState));
		}

		private void CheckAutoBanListForRoomName(string nameToCheck)
		{
			this.SwitchToLoadingState();
			this.CheckForBadRoomName(nameToCheck);
		}

		private void CheckAutoBanListForPlayerName(string nameToCheck)
		{
			this.SwitchToLoadingState();
			this.CheckForBadPlayerName(nameToCheck);
		}

		private void CheckAutoBanListForTroopName(string nameToCheck)
		{
			if (this.IsValidTroopName(this.troopToJoin))
			{
				this.SwitchToLoadingState();
				this.CheckForBadTroopName(nameToCheck);
			}
		}

		private void CheckForBadRoomName(string nameToCheck)
		{
			GorillaServer.Instance.CheckForBadName(new CheckForBadNameRequest
			{
				name = nameToCheck,
				forRoom = true,
				forTroop = false
			}, new Action<ExecuteFunctionResult>(this.OnRoomNameChecked), new Action<PlayFabError>(this.OnErrorNameCheck));
		}

		private void CheckForBadPlayerName(string nameToCheck)
		{
			GorillaServer.Instance.CheckForBadName(new CheckForBadNameRequest
			{
				name = nameToCheck,
				forRoom = false,
				forTroop = false
			}, new Action<ExecuteFunctionResult>(this.OnPlayerNameChecked), new Action<PlayFabError>(this.OnErrorNameCheck));
		}

		private void CheckForBadTroopName(string nameToCheck)
		{
			GorillaServer.Instance.CheckForBadName(new CheckForBadNameRequest
			{
				name = nameToCheck,
				forRoom = false,
				forTroop = true
			}, new Action<ExecuteFunctionResult>(this.OnTroopNameChecked), new Action<PlayFabError>(this.OnErrorNameCheck));
		}

		private void OnRoomNameChecked(ExecuteFunctionResult result)
		{
			object obj;
			if (((JsonObject)result.FunctionResult).TryGetValue("result", out obj))
			{
				switch (int.Parse(obj.ToString()))
				{
				case 0:
					if (FriendshipGroupDetection.Instance.IsInParty && !FriendshipGroupDetection.Instance.IsPartyWithinCollider(this.friendJoinCollider))
					{
						FriendshipGroupDetection.Instance.LeaveParty();
					}
					if (this.playerInVirtualStump)
					{
						CustomMapManager.UnloadMap(false);
					}
					this.networkController.AttemptToJoinSpecificRoom(this.roomToJoin, FriendshipGroupDetection.Instance.IsInParty ? JoinType.JoinWithParty : JoinType.Solo);
					break;
				case 1:
					this.roomToJoin = "";
					this.roomToJoin += (this.playerInVirtualStump ? this.virtualStumpRoomPrepend : "");
					this.SwitchToWarningState();
					break;
				case 2:
					this.roomToJoin = "";
					this.roomToJoin += (this.playerInVirtualStump ? this.virtualStumpRoomPrepend : "");
					GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
					break;
				}
			}
			if (this.currentState == GorillaComputer.ComputerState.Loading)
			{
				this.PopState();
			}
		}

		private void OnPlayerNameChecked(ExecuteFunctionResult result)
		{
			object obj;
			if (((JsonObject)result.FunctionResult).TryGetValue("result", out obj))
			{
				switch (int.Parse(obj.ToString()))
				{
				case 0:
					NetworkSystem.Instance.SetMyNickName(this.currentName);
					CustomMapsTerminal.RequestDriverNickNameRefresh();
					break;
				case 1:
					NetworkSystem.Instance.SetMyNickName("gorilla");
					CustomMapsTerminal.RequestDriverNickNameRefresh();
					this.currentName = "gorilla";
					this.SwitchToWarningState();
					break;
				case 2:
					NetworkSystem.Instance.SetMyNickName("gorilla");
					CustomMapsTerminal.RequestDriverNickNameRefresh();
					this.currentName = "gorilla";
					GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
					break;
				}
			}
			this.SetLocalNameTagText(this.currentName);
			this.savedName = this.currentName;
			PlayerPrefs.SetString("playerName", this.currentName);
			PlayerPrefs.Save();
			if (NetworkSystem.Instance.InRoom)
			{
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, new object[]
				{
					this.redValue,
					this.greenValue,
					this.blueValue
				});
			}
			if (this.currentState == GorillaComputer.ComputerState.Loading)
			{
				this.PopState();
			}
		}

		private void OnTroopNameChecked(ExecuteFunctionResult result)
		{
			object obj;
			if (((JsonObject)result.FunctionResult).TryGetValue("result", out obj))
			{
				switch (int.Parse(obj.ToString()))
				{
				case 0:
					this.JoinTroop(this.troopToJoin);
					break;
				case 1:
					this.troopToJoin = string.Empty;
					this.SwitchToWarningState();
					break;
				case 2:
					this.troopToJoin = string.Empty;
					GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
					break;
				}
			}
			if (this.currentState == GorillaComputer.ComputerState.Loading)
			{
				this.PopState();
			}
		}

		private void OnErrorNameCheck(PlayFabError error)
		{
			if (this.currentState == GorillaComputer.ComputerState.Loading)
			{
				this.PopState();
			}
			GorillaComputer.OnErrorShared(error);
		}

		public bool CheckAutoBanListForName(string nameToCheck)
		{
			nameToCheck = nameToCheck.ToLower();
			nameToCheck = new string(Array.FindAll<char>(nameToCheck.ToCharArray(), (char c) => char.IsLetterOrDigit(c)));
			foreach (string value in this.anywhereTwoWeek)
			{
				if (nameToCheck.IndexOf(value) >= 0)
				{
					return false;
				}
			}
			foreach (string value2 in this.anywhereOneWeek)
			{
				if (nameToCheck.IndexOf(value2) >= 0 && !nameToCheck.Contains("fagol"))
				{
					return false;
				}
			}
			string[] array = this.exactOneWeek;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == nameToCheck)
				{
					return false;
				}
			}
			return true;
		}

		public void UpdateColor(float red, float green, float blue)
		{
			this.redValue = Mathf.Clamp(red, 0f, 1f);
			this.greenValue = Mathf.Clamp(green, 0f, 1f);
			this.blueValue = Mathf.Clamp(blue, 0f, 1f);
		}

		public void UpdateFailureText(string failMessage)
		{
			GorillaScoreboardTotalUpdater.instance.SetOfflineFailureText(failMessage);
			PhotonNetworkController.Instance.UpdateTriggerScreens();
			this.screenText.EnableFailedState(failMessage);
			this.functionSelectText.EnableFailedState(failMessage);
		}

		private void RestoreFromFailureState()
		{
			GorillaScoreboardTotalUpdater.instance.ClearOfflineFailureText();
			PhotonNetworkController.Instance.UpdateTriggerScreens();
			this.screenText.DisableFailedState();
			this.functionSelectText.DisableFailedState();
		}

		public void GeneralFailureMessage(string failMessage)
		{
			this.isConnectedToMaster = false;
			NetworkSystem.Instance.SetWrongVersion();
			this.UpdateFailureText(failMessage);
			this.UpdateScreen();
		}

		private static void OnErrorShared(PlayFabError error)
		{
			if (error.Error == PlayFabErrorCode.NotAuthenticated)
			{
				PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
			}
			else if (error.Error == PlayFabErrorCode.AccountBanned)
			{
				GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
			}
			if (error.ErrorMessage == "The account making this request is currently banned")
			{
				using (Dictionary<string, List<string>>.Enumerator enumerator = error.ErrorDetails.GetEnumerator())
				{
					if (!enumerator.MoveNext())
					{
						return;
					}
					KeyValuePair<string, List<string>> keyValuePair = enumerator.Current;
					if (keyValuePair.Value[0] != "Indefinite")
					{
						GorillaComputer.instance.GeneralFailureMessage(string.Concat(new string[]
						{
							"YOUR ACCOUNT ",
							PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
							" HAS BEEN BANNED. YOU WILL NOT BE ABLE TO PLAY UNTIL THE BAN EXPIRES.\nREASON: ",
							keyValuePair.Key,
							"\nHOURS LEFT: ",
							((int)((DateTime.Parse(keyValuePair.Value[0]) - DateTime.UtcNow).TotalHours + 1.0)).ToString()
						}));
						return;
					}
					GorillaComputer.instance.GeneralFailureMessage("YOUR ACCOUNT " + PlayFabAuthenticator.instance.GetPlayFabPlayerId() + " HAS BEEN BANNED INDEFINITELY.\nREASON: " + keyValuePair.Key);
					return;
				}
			}
			if (error.ErrorMessage == "The IP making this request is currently banned")
			{
				using (Dictionary<string, List<string>>.Enumerator enumerator = error.ErrorDetails.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						KeyValuePair<string, List<string>> keyValuePair2 = enumerator.Current;
						if (keyValuePair2.Value[0] != "Indefinite")
						{
							GorillaComputer.instance.GeneralFailureMessage("THIS IP HAS BEEN BANNED. YOU WILL NOT BE ABLE TO PLAY UNTIL THE BAN EXPIRES.\nREASON: " + keyValuePair2.Key + "\nHOURS LEFT: " + ((int)((DateTime.Parse(keyValuePair2.Value[0]) - DateTime.UtcNow).TotalHours + 1.0)).ToString());
						}
						else
						{
							GorillaComputer.instance.GeneralFailureMessage("THIS IP HAS BEEN BANNED INDEFINITELY.\nREASON: " + keyValuePair2.Key);
						}
					}
				}
			}
		}

		private void DecreaseState()
		{
			this.currentStateIndex--;
			if (this.GetState(this.currentStateIndex) == GorillaComputer.ComputerState.Time)
			{
				this.currentStateIndex--;
			}
			if (this.currentStateIndex < 0)
			{
				this.currentStateIndex = this.FunctionsCount - 1;
			}
			this.SwitchState(this.GetState(this.currentStateIndex), true);
		}

		private void IncreaseState()
		{
			this.currentStateIndex++;
			if (this.GetState(this.currentStateIndex) == GorillaComputer.ComputerState.Time)
			{
				this.currentStateIndex++;
			}
			if (this.currentStateIndex >= this.FunctionsCount)
			{
				this.currentStateIndex = 0;
			}
			this.SwitchState(this.GetState(this.currentStateIndex), true);
		}

		public GorillaComputer.ComputerState GetState(int index)
		{
			GorillaComputer.ComputerState state;
			try
			{
				state = this._activeOrderList[index].State;
			}
			catch
			{
				state = this._activeOrderList[0].State;
			}
			return state;
		}

		public int GetStateIndex(GorillaComputer.ComputerState state)
		{
			return this._activeOrderList.FindIndex((GorillaComputer.StateOrderItem s) => s.State == state);
		}

		public string GetOrderListForScreen(GorillaComputer.ComputerState currentState)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int stateIndex = this.GetStateIndex(currentState);
			for (int i = 0; i < this.FunctionsCount; i++)
			{
				stringBuilder.Append(this.FunctionNames[i]);
				if (i == stateIndex)
				{
					stringBuilder.Append(this.Pointer);
				}
				if (i < this.FunctionsCount - 1)
				{
					stringBuilder.Append("\n");
				}
			}
			return stringBuilder.ToString();
		}

		private void GetCurrentTime()
		{
			this.tryGetTimeAgain = true;
			PlayFabClientAPI.GetTime(new GetTimeRequest(), new Action<GetTimeResult>(this.OnGetTimeSuccess), new Action<PlayFabError>(this.OnGetTimeFailure), null, null);
		}

		private void OnGetTimeSuccess(GetTimeResult result)
		{
			this.startupMillis = (long)(TimeSpan.FromTicks(result.Time.Ticks).TotalMilliseconds - (double)(Time.realtimeSinceStartup * 1000f));
			this.startupTime = result.Time - TimeSpan.FromSeconds((double)Time.realtimeSinceStartup);
			Action onServerTimeUpdated = this.OnServerTimeUpdated;
			if (onServerTimeUpdated == null)
			{
				return;
			}
			onServerTimeUpdated();
		}

		private void OnGetTimeFailure(PlayFabError error)
		{
			this.startupMillis = (long)(TimeSpan.FromTicks(DateTime.UtcNow.Ticks).TotalMilliseconds - (double)(Time.realtimeSinceStartup * 1000f));
			this.startupTime = DateTime.UtcNow - TimeSpan.FromSeconds((double)Time.realtimeSinceStartup);
			Action onServerTimeUpdated = this.OnServerTimeUpdated;
			if (onServerTimeUpdated != null)
			{
				onServerTimeUpdated();
			}
			if (error.Error == PlayFabErrorCode.NotAuthenticated)
			{
				PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
				return;
			}
			if (error.Error == PlayFabErrorCode.AccountBanned)
			{
				GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
			}
		}

		private void PlayerCountChangedCallback(NetPlayer player)
		{
			this.UpdateScreen();
		}

		private static void OnFirstJoinedRoom_IncrementSessionCount()
		{
			RoomSystem.JoinedRoomEvent -= new Action(GorillaComputer.OnFirstJoinedRoom_IncrementSessionCount);
			GorillaComputer.sessionCount++;
			PlayerPrefs.SetInt("sessionCount", GorillaComputer.sessionCount);
			PlayerPrefs.Save();
		}

		public void SetNameBySafety(bool isSafety)
		{
			if (!isSafety)
			{
				return;
			}
			PlayerPrefs.SetString("playerNameBackup", this.currentName);
			this.currentName = "gorilla" + Random.Range(0, 9999).ToString().PadLeft(4, '0');
			this.savedName = this.currentName;
			NetworkSystem.Instance.SetMyNickName(this.currentName);
			this.SetLocalNameTagText(this.currentName);
			PlayerPrefs.SetString("playerName", this.currentName);
			PlayerPrefs.Save();
			if (NetworkSystem.Instance.InRoom)
			{
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, new object[]
				{
					this.redValue,
					this.greenValue,
					this.blueValue
				});
			}
		}

		public void SetLocalNameTagText(string newName)
		{
			VRRig.LocalRig.SetNameTagText(newName);
		}

		public void SetComputerSettingsBySafety(bool isSafety, GorillaComputer.ComputerState[] toFilterOut, bool shouldHide)
		{
			this._activeOrderList = this.OrderList;
			if (!isSafety)
			{
				this._activeOrderList = this.OrderList;
				if (this._filteredStates.Count > 0 && toFilterOut.Length != 0)
				{
					for (int i = 0; i < toFilterOut.Length; i++)
					{
						if (this._filteredStates.Contains(toFilterOut[i]))
						{
							this._filteredStates.Remove(toFilterOut[i]);
						}
					}
				}
			}
			else if (shouldHide)
			{
				for (int j = 0; j < toFilterOut.Length; j++)
				{
					if (!this._filteredStates.Contains(toFilterOut[j]))
					{
						this._filteredStates.Add(toFilterOut[j]);
					}
				}
			}
			if (this._filteredStates.Count > 0)
			{
				int k = 0;
				int num = this._activeOrderList.Count;
				while (k < num)
				{
					if (this._filteredStates.Contains(this._activeOrderList[k].State))
					{
						this._activeOrderList.RemoveAt(k);
						k--;
						num--;
					}
					k++;
				}
			}
			this.FunctionsCount = this._activeOrderList.Count;
			this.FunctionNames.Clear();
			this._activeOrderList.ForEach(delegate(GorillaComputer.StateOrderItem s)
			{
				string name = s.GetName();
				if (name.Length > this.highestCharacterCount)
				{
					this.highestCharacterCount = name.Length;
				}
				this.FunctionNames.Add(name);
			});
			for (int l = 0; l < this.FunctionsCount; l++)
			{
				int num2 = this.highestCharacterCount - this.FunctionNames[l].Length;
				for (int m = 0; m < num2; m++)
				{
					List<string> functionNames = this.FunctionNames;
					int index = l;
					functionNames[index] += " ";
				}
			}
			this.UpdateScreen();
		}

		public void KID_SetVoiceChatSettingOnStart(bool voiceChatEnabled, Permission.ManagedByEnum managedBy, bool hasOptedInPreviously)
		{
			if (managedBy == Permission.ManagedByEnum.PROHIBITED)
			{
				return;
			}
			this.SetVoice(voiceChatEnabled, !hasOptedInPreviously);
		}

		private void SetVoice(bool setting, bool saveSetting = true)
		{
			this.voiceChatOn = (setting ? "TRUE" : "FALSE");
			if (setting && !KIDManager.CheckFeatureOptIn(EKIDFeatures.Voice_Chat, null).Item2)
			{
				KIDManager.SetFeatureOptIn(EKIDFeatures.Voice_Chat, true);
				KIDManager.SendOptInPermissions();
			}
			if (!saveSetting)
			{
				return;
			}
			PlayerPrefs.SetString("voiceChatOn", this.voiceChatOn);
			PlayerPrefs.Save();
		}

		public bool CheckVoiceChatEnabled()
		{
			return this.voiceChatOn == "TRUE";
		}

		private void SetVoiceChatBySafety(bool voiceChatEnabled, Permission.ManagedByEnum managedBy)
		{
			bool isSafety = !voiceChatEnabled;
			this.SetComputerSettingsBySafety(isSafety, new GorillaComputer.ComputerState[]
			{
				GorillaComputer.ComputerState.Voice,
				GorillaComputer.ComputerState.AutoMute,
				GorillaComputer.ComputerState.Mic
			}, false);
			string value = PlayerPrefs.GetString("voiceChatOn", "");
			if (KIDManager.KidEnabledAndReady)
			{
				Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Voice_Chat);
				if (permissionDataByFeature != null)
				{
					ValueTuple<bool, bool> valueTuple = KIDManager.CheckFeatureOptIn(EKIDFeatures.Voice_Chat, permissionDataByFeature);
					if (valueTuple.Item1 && !valueTuple.Item2)
					{
						value = "FALSE";
					}
				}
				else
				{
					Debug.LogErrorFormat("[KID] Could not find permission data for [" + EKIDFeatures.Voice_Chat.ToStandardisedString() + "]", Array.Empty<object>());
				}
			}
			switch (managedBy)
			{
			case Permission.ManagedByEnum.PLAYER:
				if (string.IsNullOrEmpty(value))
				{
					this.voiceChatOn = (voiceChatEnabled ? "TRUE" : "FALSE");
				}
				else
				{
					this.voiceChatOn = value;
				}
				break;
			case Permission.ManagedByEnum.GUARDIAN:
				if (KIDManager.GetPermissionDataByFeature(EKIDFeatures.Voice_Chat).Enabled)
				{
					if (string.IsNullOrEmpty(value))
					{
						this.voiceChatOn = "TRUE";
					}
					else
					{
						this.voiceChatOn = value;
					}
				}
				else
				{
					this.voiceChatOn = "FALSE";
				}
				break;
			case Permission.ManagedByEnum.PROHIBITED:
				this.voiceChatOn = "FALSE";
				break;
			}
			RigContainer.RefreshAllRigVoices();
			Debug.Log("[KID] On Session Update - Voice Chat Permission changed - Has enabled voiceChat? [" + voiceChatEnabled.ToString() + "]");
		}

		public void SetNametagSetting(bool setting, Permission.ManagedByEnum managedBy, bool hasOptedInPreviously)
		{
			if (managedBy == Permission.ManagedByEnum.PROHIBITED)
			{
				return;
			}
			if (managedBy == Permission.ManagedByEnum.GUARDIAN)
			{
				int @int = PlayerPrefs.GetInt(this.NameTagPlayerPref, 1);
				setting = (setting && @int == 1);
				this.UpdateNametagSetting(setting, false);
				return;
			}
			setting = (PlayerPrefs.GetInt(this.NameTagPlayerPref, setting ? 1 : 0) == 1);
			this.UpdateNametagSetting(setting, !hasOptedInPreviously && setting);
		}

		public static void RegisterOnNametagSettingChanged(Action<bool> callback)
		{
			GorillaComputer.onNametagSettingChangedAction = (Action<bool>)Delegate.Combine(GorillaComputer.onNametagSettingChangedAction, callback);
		}

		public static void UnregisterOnNametagSettingChanged(Action<bool> callback)
		{
			GorillaComputer.onNametagSettingChangedAction = (Action<bool>)Delegate.Remove(GorillaComputer.onNametagSettingChangedAction, callback);
		}

		private void UpdateNametagSetting(bool newSettingValue, bool saveSetting = true)
		{
			if (newSettingValue)
			{
				KIDManager.SetFeatureOptIn(EKIDFeatures.Custom_Nametags, true);
			}
			this.NametagsEnabled = newSettingValue;
			NetworkSystem.Instance.SetMyNickName(this.NametagsEnabled ? this.savedName : NetworkSystem.Instance.GetMyDefaultName());
			if (NetworkSystem.Instance.InRoom)
			{
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, new object[]
				{
					this.redValue,
					this.greenValue,
					this.blueValue
				});
			}
			Action<bool> action = GorillaComputer.onNametagSettingChangedAction;
			if (action != null)
			{
				action(this.NametagsEnabled);
			}
			if (!saveSetting)
			{
				return;
			}
			int value = this.NametagsEnabled ? 1 : 0;
			PlayerPrefs.SetInt(this.NameTagPlayerPref, value);
			PlayerPrefs.Save();
		}

		void IMatchmakingCallbacks.OnFriendListUpdate(List<Photon.Realtime.FriendInfo> friendList)
		{
		}

		void IMatchmakingCallbacks.OnCreatedRoom()
		{
		}

		void IMatchmakingCallbacks.OnCreateRoomFailed(short returnCode, string message)
		{
		}

		void IMatchmakingCallbacks.OnJoinedRoom()
		{
		}

		void IMatchmakingCallbacks.OnJoinRandomFailed(short returnCode, string message)
		{
		}

		void IMatchmakingCallbacks.OnLeftRoom()
		{
		}

		void IMatchmakingCallbacks.OnPreLeavingRoom()
		{
		}

		void IMatchmakingCallbacks.OnJoinRoomFailed(short returnCode, string message)
		{
			if (returnCode == 32765)
			{
				this.roomFull = true;
			}
		}

		public void SetInVirtualStump(bool inVirtualStump)
		{
			this.playerInVirtualStump = inVirtualStump;
			this.roomToJoin = (this.playerInVirtualStump ? (this.virtualStumpRoomPrepend + this.roomToJoin) : this.roomToJoin.RemoveAll(this.virtualStumpRoomPrepend, StringComparison.OrdinalIgnoreCase));
		}

		public bool IsPlayerInVirtualStump()
		{
			return this.playerInVirtualStump;
		}

		public void SetLimitOnlineScreens(bool isLimited)
		{
			this.limitOnlineScreens = isLimited;
			this.UpdateScreen();
		}

		private void InitializeKIdState()
		{
			KIDManager.RegisterSessionUpdateCallback_AnyPermission(new Action(this.OnSessionUpdate_GorillaComputer));
		}

		private void UpdateKidState()
		{
			this._currentScreentState = GorillaComputer.EKidScreenState.Ready;
		}

		private void RequestUpdatedPermissions()
		{
			if (!KIDManager.KidEnabledAndReady)
			{
				return;
			}
			if (this._waitingForUpdatedSession)
			{
				return;
			}
			if (Time.time < this._nextUpdateAttemptTime)
			{
				return;
			}
			this._waitingForUpdatedSession = true;
			this.UpdateSession();
		}

		private void UpdateSession()
		{
			GorillaComputer.<UpdateSession>d__486 <UpdateSession>d__;
			<UpdateSession>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<UpdateSession>d__.<>4__this = this;
			<UpdateSession>d__.<>1__state = -1;
			<UpdateSession>d__.<>t__builder.Start<GorillaComputer.<UpdateSession>d__486>(ref <UpdateSession>d__);
		}

		private void OnSessionUpdate_GorillaComputer()
		{
			this.UpdateKidState();
			this.UpdateScreen();
		}

		private void ProcessScreen_SetupKID()
		{
			if (!KIDManager.KidEnabledAndReady)
			{
				Debug.LogError("[KID] Unable to start k-ID Flow. Kid is disabled");
				return;
			}
		}

		private bool GuardianConsentMessage(string setupKIDButtonName, string featureDescription)
		{
			string defaultResult = "PARENT/GUARDIAN PERMISSION REQUIRED TO ";
			string text;
			LocalisationManager.TryGetKeyForCurrentLocale("KID_PERMISSION_NEEDED", out text, defaultResult);
			this.screenText.Append(text);
			this.screenText.Append(featureDescription + "!");
			if (this._waitingForUpdatedSession)
			{
				defaultResult = "\n\nWAITING FOR PARENT/GUARDIAN CONSENT!";
				LocalisationManager.TryGetKeyForCurrentLocale("KID_WAITING_PERMISSION", out text, defaultResult);
				this.screenText.Append(text);
				return true;
			}
			if (Time.time >= this._nextUpdateAttemptTime)
			{
				defaultResult = "\n\nPRESS OPTION 2 TO REFRESH PERMISSIONS!";
				LocalisationManager.TryGetKeyForCurrentLocale("KID_REFRESH_PERMISSIONS", out text, defaultResult);
				this.screenText.Append(text);
			}
			else
			{
				defaultResult = "CHECK AGAIN IN {time} SECONDS!";
				LocalisationManager.TryGetKeyForCurrentLocale("KID_CHECK_AGAIN_COOLDOWN", out text, defaultResult);
				text = text.Replace("{time}", ((int)(this._nextUpdateAttemptTime - Time.time)).ToString());
				this.screenText.Append(text);
			}
			return false;
		}

		private void ProhibitedMessage(string verb)
		{
			"\n\nYOU ARE NOT ALLOWED TO " + verb + " IN YOUR JURISDICTION.";
			string text;
			LocalisationManager.TryGetKeyForCurrentLocale("KID_PROHIBITED_MESSAGE", out text, "SET CUSTOM NICKNAMES");
			text = text.Replace("{verb}", verb);
			this.screenText.Append(text);
		}

		private void RoomScreen_Permission()
		{
			if (!KIDManager.KidEnabled)
			{
				string defaultResult = "YOU CANNOT USE THE PRIVATE ROOM FEATURE RIGHT NOW";
				string str;
				LocalisationManager.TryGetKeyForCurrentLocale("ROOM_SCREEN_DISABLED", out str, defaultResult);
				this.screenText.Set(str);
				return;
			}
			this.screenText.Set("");
			string defaultResult2 = "CREATE OR JOIN PRIVATE ROOMS";
			string featureDescription;
			LocalisationManager.TryGetKeyForCurrentLocale("ROOM_SCREEN_KID_PROHIBITED_VERB", out featureDescription, defaultResult2);
			this.GuardianConsentMessage("OPTION 3", featureDescription);
		}

		private void RoomScreen_KIdProhibited()
		{
			string defaultResult = "CREATE OR JOIN PRIVATE ROOMS";
			string verb;
			LocalisationManager.TryGetKeyForCurrentLocale("ROOM_SCREEN_KID_PROHIBITED_VERB", out verb, defaultResult);
			this.ProhibitedMessage(verb);
		}

		private void VoiceScreen_Permission()
		{
			string defaultResult = "VOICE TYPE: \"MONKE\"\n\n";
			string text;
			LocalisationManager.TryGetKeyForCurrentLocale("VOICE_SCREEN_KID_CURRENT_VOICE", out text, defaultResult);
			this.screenText.Set(text);
			if (!KIDManager.KidEnabled)
			{
				defaultResult = "YOU CANNOT USE THE HUMAN VOICE TYPE FEATURE RIGHT NOW";
				LocalisationManager.TryGetKeyForCurrentLocale("VOICE_SCREEN_DISABLED", out text, defaultResult);
				this.screenText.Append(text);
				return;
			}
			defaultResult = "ENABLE HUMAN VOICE CHAT";
			LocalisationManager.TryGetKeyForCurrentLocale("VOICE_SCREEN_GUARDIAN_FEATURE_DESC", out text, defaultResult);
			this.GuardianConsentMessage("OPTION 3", text);
		}

		private void VoiceScreen_KIdProhibited()
		{
			string defaultResult = "USE THE VOICE CHAT";
			string verb;
			LocalisationManager.TryGetKeyForCurrentLocale("VOICE_SCREEN_KID_PROHIBITED_VERB", out verb, defaultResult);
			this.ProhibitedMessage(verb);
		}

		private void MicScreen_Permission()
		{
			this.screenText.Set("");
			string defaultResult = "ENABLE HUMAN VOICE CHAT";
			string featureDescription;
			LocalisationManager.TryGetKeyForCurrentLocale("VOICE_SCREEN_GUARDIAN_FEATURE_DESC", out featureDescription, defaultResult);
			this.GuardianConsentMessage("OPTION 3", featureDescription);
		}

		private void MicScreen_KIdProhibited()
		{
			this.VoiceScreen_KIdProhibited();
		}

		private void NameScreen_Permission()
		{
			if (!KIDManager.KidEnabled)
			{
				string defaultResult = "YOU CANNOT USE THE CUSTOM NICKNAME FEATURE RIGHT NOW";
				string str;
				LocalisationManager.TryGetKeyForCurrentLocale("NAME_SCREEN_DISABLED", out str, defaultResult);
				this.screenText.Append(str);
				return;
			}
			this.screenText.Set("");
			string featureDescription;
			LocalisationManager.TryGetKeyForCurrentLocale("NAME_SCREEN_KID_PROHIBITED_VERB", out featureDescription, "SET CUSTOM NICKNAMES");
			this.GuardianConsentMessage("OPTION 3", featureDescription);
		}

		private void NameScreen_KIdProhibited()
		{
			string verb;
			LocalisationManager.TryGetKeyForCurrentLocale("NAME_SCREEN_KID_PROHIBITED_VERB", out verb, "SET CUSTOM NICKNAMES");
			this.ProhibitedMessage(verb);
		}

		private void OnKIDSessionUpdated_CustomNicknames(bool showCustomNames, Permission.ManagedByEnum managedBy)
		{
			bool flag = (showCustomNames || managedBy == Permission.ManagedByEnum.PLAYER) && managedBy != Permission.ManagedByEnum.PROHIBITED;
			this.SetComputerSettingsBySafety(!flag, new GorillaComputer.ComputerState[]
			{
				GorillaComputer.ComputerState.Name
			}, false);
			int @int = PlayerPrefs.GetInt(this.NameTagPlayerPref, -1);
			bool flag2 = @int > 0;
			switch (managedBy)
			{
			case Permission.ManagedByEnum.PLAYER:
				if (showCustomNames)
				{
					this.NametagsEnabled = (@int == -1 || flag2);
				}
				else
				{
					this.NametagsEnabled = (@int != -1 && flag2);
				}
				break;
			case Permission.ManagedByEnum.GUARDIAN:
				this.NametagsEnabled = (showCustomNames && (flag2 || @int == -1));
				break;
			case Permission.ManagedByEnum.PROHIBITED:
				this.NametagsEnabled = false;
				break;
			}
			if (this.NametagsEnabled)
			{
				NetworkSystem.Instance.SetMyNickName(this.savedName);
			}
			Action<bool> action = GorillaComputer.onNametagSettingChangedAction;
			if (action == null)
			{
				return;
			}
			action(this.NametagsEnabled);
		}

		private void TroopScreen_Permission()
		{
			this.screenText.Set("");
			if (!KIDManager.KidEnabled)
			{
				string str;
				LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_DISABLED", out str, "YOU CANNOT USE THE TROOPS FEATURE RIGHT NOW");
				this.screenText.Append(str);
				return;
			}
			string featureDescription;
			LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_KID_DESC", out featureDescription, "JOIN TROOPS");
			this.GuardianConsentMessage("OPTION 3", featureDescription);
		}

		private void TroopScreen_KIdProhibited()
		{
			string verb;
			LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_KID_PROHIBITED_VERB", out verb, "CREATE OR JOIN TROOPS");
			this.ProhibitedMessage(verb);
		}

		private void ProcessKIdState(GorillaKeyboardBindings buttonPressed)
		{
			if (buttonPressed == GorillaKeyboardBindings.option1 && this._currentScreentState == GorillaComputer.EKidScreenState.Ready)
			{
				this.RequestUpdatedPermissions();
			}
		}

		private void KIdScreen()
		{
			if (!KIDManager.KidEnabledAndReady)
			{
				return;
			}
			if (!KIDManager.HasSession)
			{
				this.GuardianConsentMessage("OPTION 3", "");
				return;
			}
			this.KIdScreen_DisplayPermissions();
		}

		private void KIdScreen_DisplayPermissions()
		{
			AgeStatusType activeAccountStatus = KIDManager.GetActiveAccountStatus();
			string str = (!KIDManager.InitialisationSuccessful) ? "NOT READY" : activeAccountStatus.ToString();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("k-ID Account Status:\t" + str);
			if (activeAccountStatus == (AgeStatusType)0)
			{
				stringBuilder.AppendLine("\nPress 'OPTION 1' to get permissions!");
				this.screenText.Set(stringBuilder.ToString());
				return;
			}
			if (this._waitingForUpdatedSession)
			{
				stringBuilder.AppendLine("\nWAITING FOR PARENT/GUARDIAN CONSENT!");
				this.screenText.Set(stringBuilder.ToString());
				return;
			}
			stringBuilder.AppendLine("\nPermissions:");
			List<Permission> allPermissionsData = KIDManager.GetAllPermissionsData();
			int count = allPermissionsData.Count;
			int num = 1;
			for (int i = 0; i < count; i++)
			{
				if (this._interestedPermissionNames.Contains(allPermissionsData[i].Name))
				{
					string text = allPermissionsData[i].Enabled ? "<color=#85ffa5>" : "<color=\"RED\">";
					stringBuilder.AppendLine(string.Concat(new string[]
					{
						"[",
						num.ToString(),
						"] ",
						text,
						allPermissionsData[i].Name,
						"</color>"
					}));
					num++;
				}
			}
			stringBuilder.AppendLine("\nTO REFRESH PERMISSIONS PRESS OPTION 1!");
			this.screenText.Set(stringBuilder.ToString());
		}

		private string GetLocalisedLanguageScreen()
		{
			return this.GetLanguageScreenLocalisation();
		}

		private void GetLangaugesList(ref string langStr)
		{
			this._languagesDisplaySB.Clear();
			int maxLength = 12;
			int num = 3;
			int num2 = 0;
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<int, Locale> keyValuePair in LocalisationManager.GetAllBindings())
			{
				num2++;
				string text = LocalisationManager.LocaleToFriendlyString(keyValuePair.Value, false).ToUpper();
				string value = string.Format("{0}) {1}", keyValuePair.Key, text);
				stringBuilder.Append(value);
				int remainingChars = this.GetRemainingChars(text, maxLength);
				stringBuilder.Append(' ', remainingChars);
				if (num2 >= num)
				{
					this._languagesDisplaySB.AppendLine(stringBuilder.ToString());
					stringBuilder.Clear();
					num2 = 0;
				}
			}
			this._languagesDisplaySB.AppendLine(stringBuilder.ToString());
			langStr = langStr + this._languagesDisplaySB.ToString() + "\n";
		}

		private int GetRemainingChars(string value, int maxLength)
		{
			int result;
			if (value == "日本語")
			{
				result = ((LocalisationManager.CurrentLanguage.Identifier.Code == "ja") ? 7 : 7);
			}
			else
			{
				result = Mathf.Clamp(maxLength - value.Length, 0, maxLength);
			}
			return result;
		}

		private string GetLanguageScreenLocalisation()
		{
			string text = "";
			string text2;
			LocalisationManager.TryGetKeyForCurrentLocale("LANG_SCREEN_TITLE", out text2, "CHOOSE YOUR LANGUAGE\n");
			text += text2;
			this.GetLangaugesList(ref text);
			LocalisationManager.TryGetKeyForCurrentLocale("LANG_SCREEN_INSTRUCTIONS", out text2, "PRESS NUMBER KEYS TO CHOOSE A LANGUAGE\n");
			text += text2;
			LocalisationManager.TryGetKeyForCurrentLocale("LANG_SCREEN_CURRENT_LANGUAGE", out text2, "CURRENT LANGUAGE: ");
			text = text + text2.TrailingSpace() + LocalisationManager.LocaleToFriendlyString(null, false).ToUpper();
			return text;
		}

		private void InitialiseLanguageScreen()
		{
			this._previousLocalisationSetting = LocalisationManager.CurrentLanguage;
			LocalisationManager.RegisterOnLanguageChanged(new Action(this.OnLanguageChanged));
		}

		private void LanguageScreen()
		{
			this.screenText.Set(this.GetLocalisedLanguageScreen());
		}

		private void ProcessLanguageState(GorillaKeyboardBindings buttonPressed)
		{
			int binding;
			if (!buttonPressed.FromNumberBindingToInt(out binding))
			{
				return;
			}
			Locale locale;
			if (!LocalisationManager.TryGetLocaleBinding(binding, out locale))
			{
				return;
			}
			LocalisationManager.Instance.OnLanguageButtonPressed(locale.Identifier.Code, true);
			this.RefreshFunctionNames();
		}

		private void OnLanguageChanged()
		{
			if (this._previousLocalisationSetting == LocalisationManager.CurrentLanguage)
			{
				Debug.Log("[LOCALISATION::GORILLA_COMPUTER] Language changed, but no different to previous setting [" + this._previousLocalisationSetting.ToString() + "]");
				return;
			}
			this._previousLocalisationSetting = LocalisationManager.CurrentLanguage;
			this.RefreshFunctionNames();
		}

		private void RefreshFunctionNames()
		{
			this.FunctionNames.Clear();
			this.FunctionsCount = this.OrderList.Count;
			this.highestCharacterCount = int.MinValue;
			this.OrderList.ForEach(delegate(GorillaComputer.StateOrderItem s)
			{
				string name = s.GetName();
				if (name.Length > this.highestCharacterCount)
				{
					this.highestCharacterCount = name.Length;
				}
				this.FunctionNames.Add(name);
			});
			for (int i = 0; i < this.FunctionsCount; i++)
			{
				int num = this.highestCharacterCount - this.FunctionNames[i].Length;
				for (int j = 0; j < num; j++)
				{
					List<string> functionNames = this.FunctionNames;
					int index = i;
					functionNames[index] += " ";
				}
			}
		}

		private const string VERSION_MISMATCH_KEY = "VERSION_MISMATCH";

		private const string CONNECTION_ISSUE_KEY = "CONNECTION_ISSUE";

		private const string NO_CONNECTION_KEY = "NO_CONNECTION";

		private const string STARTUP_INTRO_KEY = "STARTUP_INTRO";

		private const string STARTUP_PLAYERS_ONLINE_KEY = "STARTUP_PLAYERS_ONLINE";

		private const string STARTUP_USERS_BANNED_KEY = "STARTUP_USERS_BANNED";

		private const string STARTUP_PRESS_KEY_KEY = "STARTUP_PRESS_KEY";

		private const string STARTUP_PRESS_KEY_SHORT_KEY = "STARTUP_PRESS_KEY_SHORT";

		private const string STARTUP_MANAGED_KEY = "STARTUP_MANAGED";

		private const string COLOR_SELECT_INTRO_KEY = "COLOR_SELECT_INTRO";

		private const string CURRENT_SELECTED_LANGUAGE_KEY = "CURRENT_SELECTED_LANGUAGE";

		private const string CHANGE_TO_KEY = "CHANGE_TO";

		private const string CONFIRM_LANGUAGE_KEY = "CONFIRM_LANGUAGE";

		private const string COLOR_RED_KEY = "COLOR_RED";

		private const string COLOR_GREEN_KEY = "COLOR_GREEN";

		private const string COLOR_BLUE_KEY = "COLOR_BLUE";

		private const string ROOM_INTRO_KEY = "ROOM_INTRO";

		private const string ROOM_OPTION_KEY = "ROOM_OPTION";

		private const string ROOM_TEXT_CURRENT_ROOM_KEY = "ROOM_TEXT_CURRENT_ROOM";

		private const string PLAYERS_IN_ROOM_KEY = "PLAYERS_IN_ROOM";

		private const string NOT_IN_ROOM_KEY = "NOT_IN_ROOM";

		private const string PLAYERS_ONLINE_KEY = "PLAYERS_ONLINE";

		private const string ROOM_TO_JOIN_KEY = "ROOM_TO_JOIN";

		private const string ROOM_FULL_KEY = "ROOM_FULL";

		private const string ROOM_JOIN_NOT_ALLOWED_KEY = "ROOM_JOIN_NOT_ALLOWED";

		private const string LANGUAGE_KEY = "LANGUAGE";

		private const string NAME_SCREEN_KEY = "NAME_SCREEN";

		private const string CURRENT_NAME_KEY = "CURRENT_NAME";

		private const string NEW_NAME_KEY = "NEW_NAME";

		private const string TURN_SCREEN_KEY = "TURN_SCREEN";

		private const string TURN_SCREEN_TURNING_SPEED_KEY = "TURN_SCREEN_TURNING_SPEED";

		private const string TURN_SCREEN_TURN_TYPE_KEY = "TURN_SCREEN_TURN_TYPE";

		private const string TURN_SCREEN_TURN_SPEED_KEY = "TURN_SCREEN_TURN_SPEED";

		private const string TURN_TYPE_SNAP_TURN_KEY = "TURN_TYPE_SNAP_TURN";

		private const string TURN_TYPE_SMOOTH_TURN_KEY = "TURN_TYPE_SMOOTH_TURN";

		private const string TURN_TYPE_NO_TURN_KEY = "TURN_TYPE_NO_TURN";

		private const string QUEUE_SCREEN_KEY = "QUEUE_SCREEN";

		private const string BEAT_OBSTACLE_COURSE_KEY = "BEAT_OBSTACLE_COURSE";

		private const string COMPETITIVE_DESC_KEY = "COMPETITIVE_DESC";

		private const string QUEUE_SCREEN_ALL_QUEUES_KEY = "QUEUE_SCREEN_ALL_QUEUES";

		private const string QUEUE_SCREEN_DEFAULT_QUEUES_KEY = "QUEUE_SCREEN_DEFAULT_QUEUES";

		private const string CURRENT_QUEUE_KEY = "CURRENT_QUEUE";

		private const string DEFAULT_QUEUE_KEY = "DEFAULT_QUEUE";

		private const string MINIGAMES_QUEUE_KEY = "MINIGAMES_QUEUE";

		private const string COMPETITIVE_QUEUE_KEY = "COMPETITIVE_QUEUE";

		private const string MIC_SCREEN_INTRO_KEY = "MIC_SCREEN_INTRO";

		private const string MIC_SCREEN_OPTIONS_KEY = "MIC_SCREEN_OPTIONS";

		private const string MIC_SCREEN_CURRENT_KEY = "MIC_SCREEN_CURRENT";

		private const string MIC_SCREEN_PUSH_TO_MUTE_TOOLTIP_KEY = "MIC_SCREEN_PUSH_TO_MUTE_TOOLTIP";

		private const string MIC_SCREEN_MIC_DISABLED_KEY = "MIC_SCREEN_MIC_DISABLED";

		private const string MIC_SCREEN_NO_MIC_KEY = "MIC_SCREEN_NO_MIC";

		private const string MIC_SCREEN_NO_PERMISSIONS_KEY = "MIC_SCREEN_NO_PERMISSIONS";

		private const string MIC_SCREEN_PUSH_TO_TALK_TOOLTIP_KEY = "MIC_SCREEN_PUSH_TO_TALK_TOOLTIP";

		private const string MIC_SCREEN_INPUT_TEST_LABEL_KEY = "MIC_SCREEN_INPUT_TEST_LABEL";

		private const string MIC_SCREEN_INPUT_TEST_NO_MIC_KEY = "MIC_SCREEN_INPUT_TEST_NO_MIC";

		private const string ALL_CHAT_MIC_KEY = "ALL_CHAT_MIC";

		private const string PUSH_TO_TALK_MIC_KEY = "PUSH_TO_TALK_MIC";

		private const string PUSH_TO_MUTE_MIC_KEY = "PUSH_TO_MUTE_MIC";

		private const string OPEN_MIC_KEY = "OPEN_MIC";

		private const string AUTOMOD_SCREEN_INTRO_KEY = "AUTOMOD_SCREEN_INTRO";

		private const string AUTOMOD_SCREEN_OPTIONS_KEY = "AUTOMOD_SCREEN_OPTIONS";

		private const string AUTOMOD_SCREEN_CURRENT_KEY = "AUTOMOD_SCREEN_CURRENT";

		private const string AUTOMOD_AGGRESSIVE_KEY = "AUTOMOD_AGGRESSIVE";

		private const string AUTOMOD_MODERATE_KEY = "AUTOMOD_MODERATE";

		private const string AUTOMOD_OFF_KEY = "AUTOMOD_OFF";

		private const string VOICE_CHAT_SCREEN_INTRO_OLD_KEY = "VOICE_CHAT_SCREEN_INTRO_OLD";

		private const string VOICE_CHAT_SCREEN_OPTIONS_OLD_KEY = "VOICE_CHAT_SCREEN_OPTIONS_OLD";

		private const string VOICE_CHAT_SCREEN_CURRENT_OLD_KEY = "VOICE_CHAT_SCREEN_CURRENT_OLD";

		private const string TRUE_KEY = "TRUE";

		private const string FALSE_KEY = "FALSE";

		private const string VOICE_CHAT_SCREEN_INTRO_KEY = "VOICE_CHAT_SCREEN_INTRO";

		private const string VOICE_CHAT_SCREEN_OPTIONS_KEY = "VOICE_CHAT_SCREEN_OPTIONS";

		private const string VOICE_CHAT_SCREEN_CURRENT_KEY = "VOICE_CHAT_SCREEN_CURRENT";

		private const string VOICE_OPTION_HUMAN_KEY = "VOICE_OPTION_HUMAN";

		private const string VOICE_OPTION_MONKE_KEY = "VOICE_OPTION_MONKE";

		private const string VOICE_OPTION_OFF_KEY = "VOICE_OPTION_OFF";

		private const string VISUALS_SCREEN_INTRO_KEY = "VISUALS_SCREEN_INTRO";

		private const string VISUALS_SCREEN_OPTIONS_KEY = "VISUALS_SCREEN_OPTIONS";

		private const string VISUALS_SCREEN_PERF_KEY = "VISUALS_SCREEN_PERF";

		private const string VISUALS_SCREEN_CURRENT_KEY = "VISUALS_SCREEN_CURRENT";

		private const string VISUALS_SCREEN_VOLUME_KEY = "VISUALS_SCREEN_VOLUME";

		private const string CREDITS_KEY = "CREDITS";

		private const string CREDITS_PRESS_ENTER_KEY = "CREDITS_PRESS_ENTER";

		private const string CREDITS_CONTINUED_KEY = "CREDITS_CONTINUED";

		private const string TIME_SCREEN_KEY = "TIME_SCREEN";

		private const string GROUP_SCREEN_LIMITED_OLD_KEY = "GROUP_SCREEN_LIMITED_OLD";

		private const string GROUP_SCREEN_FULL_OLD_KEY = "GROUP_SCREEN_FULL_OLD";

		private const string GROUP_SCREEN_SELECTION_OLD_KEY = "GROUP_SCREEN_SELECTION_OLD";

		private const string PLATFORM_STEAM_KEY = "PLATFORM_STEAM";

		private const string PLATFORM_QUEST_KEY = "PLATFORM_QUEST";

		private const string PLATFORM_PSVR_KEY = "PLATFORM_PSVR";

		private const string PLATFORM_PICO_KEY = "PLATFORM_PICO";

		private const string PLATFORM_OCULUS_PC_KEY = "PLATFORM_OCULUS_PC";

		private const string SUPPORT_SCREEN_INTRO_KEY = "SUPPORT_SCREEN_INTRO";

		private const string SUPPORT_SCREEN_DETAILS_PLAYER_ID_KEY = "SUPPORT_SCREEN_DETAILS_PLAYERID";

		private const string SUPPORT_SCREEN_DETAILS_VERSION_KEY = "SUPPORT_SCREEN_DETAILS_VERSION";

		private const string SUPPORT_SCREEN_DETAILS_PLATFORM_KEY = "SUPPORT_SCREEN_DETAILS_PLATFORM";

		private const string SUPPORT_SCREEN_DETAILS_BUILD_DATE_KEY = "SUPPORT_SCREEN_DETAILS_BUILD_DATE";

		private const string SUPPORT_SCREEN_DETAILS_MOTHERSHIP_SESSION_ID_KEY = "SUPPORT_SCREEN_DETAILS_MOTHERSHIP_SESSION_ID";

		private const string SUPPORT_SCREEN_INITIAL_KEY = "SUPPORT_SCREEN_INITIAL";

		private const string SUPPORT_SCREEN_INITIAL_WARNING_KEY = "SUPPORT_SCREEN_INITIAL_WARNING";

		private const string OCULUS_BUILD_CODE_KEY = "OCULUS_BUILD_CODE";

		private const string LOADING_SCREEN_KEY = "LOADING_SCREEN";

		private const string WARNING_SCREEN_KEY = "WARNING_SCREEN";

		private const string WARNING_SCREEN_CONFIRMATION_KEY = "WARNING_SCREEN_CONFIRMATION";

		private const string WARNING_SCREEN_TYPE_YES_KEY = "WARNING_SCREEN_TYPE_YES";

		private const string FUNCTION_ROOM_KEY = "FUNCTION_ROOM";

		private const string FUNCTION_NAME_KEY = "FUNCTION_NAME";

		private const string FUNCTION_COLOR_KEY = "FUNCTION_COLOR";

		private const string FUNCTION_TURN_KEY = "FUNCTION_TURN";

		private const string FUNCTION_MIC_KEY = "FUNCTION_MIC";

		private const string FUNCTION_QUEUE_KEY = "FUNCTION_QUEUE";

		private const string FUNCTION_GROUP_KEY = "FUNCTION_GROUP";

		private const string FUNCTION_VOICE_KEY = "FUNCTION_VOICE";

		private const string FUNCTION_AUTOMOD_KEY = "FUNCTION_AUTOMOD";

		private const string FUNCTION_ITEMS_KEY = "FUNCTION_ITEMS";

		private const string FUNCTION_CREDITS_KEY = "FUNCTION_CREDITS";

		private const string FUNCTION_LANGUAGE_KEY = "FUNCTION_LANGUAGE";

		private const string FUNCTION_SUPPORT_KEY = "FUNCTION_SUPPORT";

		private const string COMPUTER_KEYBOARD_DELETE_KEY = "COMPUTER_KEYBOARD_DELETE";

		private const string COMPUTER_KEYBOARD_ENTER_KEY = "COMPUTER_KEYBOARD_ENTER";

		private const string COMPUTER_KEYBOARD_OPTION1_KEY = "COMPUTER_KEYBOARD_OPTION1";

		private const string COMPUTER_KEYBOARD_OPTION2_KEY = "COMPUTER_KEYBOARD_OPTION2";

		private const string COMPUTER_KEYBOARD_OPTION3_KEY = "COMPUTER_KEYBOARD_OPTION3";

		private const string WARNING_SCREEN_YES_INPUT_KEY = "WARNING_SCREEN_YES_INPUT";

		private const string GROUP_SCREEN_ENTER_PARTY_KEY = "GROUP_SCREEN_ENTER_PARTY";

		private const string GROUP_SCREEN_ENTER_NOPARTY_KEY = "GROUP_SCREEN_ENTER_NOPARTY";

		private const string GROUP_SCREEN_CANNOT_JOIN_KEY = "GROUP_SCREEN_CANNOT_JOIN";

		private const string GROUP_SCREEN_ACTIVE_ZONES_KEY = "GROUP_SCREEN_ACTIVE_ZONES";

		private const string GROUP_SCREEN_DESTINATIONS_KEY = "GROUP_SCREEN_DESTINATIONS";

		private const string NAME_SCREEN_TOGGLE_NAMETAGS_KEY = "NAME_SCREEN_TOGGLE_NAMETAGS";

		private const string NAME_SCREEN_KID_PROHIBITED_VERB_KEY = "NAME_SCREEN_KID_PROHIBITED_VERB";

		private const string NAME_SCREEN_DISABLED_KEY = "NAME_SCREEN_DISABLED";

		private const string ON_KEY = "ON_KEY";

		private const string OFF_KEY = "OFF_KEY";

		private const string KID_PROHIBITED_MESSAGE_KEY = "KID_PROHIBITED_MESSAGE";

		private const string KID_PERMISSION_NEEDED_KEY = "KID_PERMISSION_NEEDED";

		private const string KID_WAITING_PERMISSION_KEY = "KID_WAITING_PERMISSION";

		private const string KID_REFRESH_PERMISSIONS_KEY = "KID_REFRESH_PERMISSIONS";

		private const string KID_CHECK_AGAIN_COOLDOWN_KEY = "KID_CHECK_AGAIN_COOLDOWN";

		private const string STARTUP_TROOP_TEXT_KEY = "STARTUP_TROOP_TEXT";

		private const string ROOM_GROUP_TRAVEL_KEY = "ROOM_GROUP_TRAVEL";

		private const string ROOM_PARTY_WARNING_KEY = "ROOM_PARTY_WARNING";

		private const string ROOM_GAME_LABEL_KEY = "ROOM_GAME_LABEL";

		private const string ROOM_SCREEN_KID_PROHIBITED_VERB_KEY = "ROOM_SCREEN_KID_PROHIBITED_VERB";

		private const string ROOM_SCREEN_DISABLED_KEY = "ROOM_SCREEN_DISABLED";

		private const string REDEMPTION_INTRO_KEY = "REDEMPTION_INTRO";

		private const string REDEMPTION_CODE_LABEL_KEY = "REDEMPTION_CODE_LABEL";

		private const string REDEMPTION_CODE_INVALID_KEY = "REDEMPTION_CODE_INVALID";

		private const string REDEMPTION_CODE_VALIDATING_KEY = "REDEMPTION_CODE_VALIDATING";

		private const string REDEMPTION_CODE_ALREADY_USED_KEY = "REDEMPTION_CODE_ALREADY_USED";

		private const string REDEMPTION_CODE_TOO_EARLY_KEY = "REDEMPTION_CODE_TOO_EARLY";

		private const string REDEMPTION_CODE_TOO_LATE_KEY = "REDEMPTION_CODE_TOO_LATE";

		private const string REDEMPTION_CODE_SUCCESS_KEY = "REDEMPTION_CODE_SUCCESS";

		private const string LIMITED_ONLINE_FUNC_KEY = "LIMITED_ONLINE_FUNC";

		private const string CURRENT_MODE_KEY = "CURRENT_MODE";

		private const string SUPPORT_META_ACCOUNT_TYPE_KEY = "SUPPORT_META_ACCOUNT_TYPE";

		private const string SUPPORT_FINAL_QUEST_ONE_KEY = "SUPPORT_FINAL_QUEST_ONE";

		private const string SUPPORT_KID_ACCOUNT_TYPE_KEY = "SUPPORT_KID_ACCOUNT_TYPE";

		private const string VOICE_SCREEN_KID_PROHIBITED_VERB_KEY = "VOICE_SCREEN_KID_PROHIBITED_VERB";

		private const string VOICE_SCREEN_DISABLED_KEY = "VOICE_SCREEN_DISABLED";

		private const string MIC_SCREEN_GUARDIAN_FEATURE_DESC_KEY = "VOICE_SCREEN_GUARDIAN_FEATURE_DESC";

		private const string VOICE_SCREEN_KID_CURRENT_VOICE_KEY = "VOICE_SCREEN_KID_CURRENT_VOICE";

		private const string MIC_SCREEN_PUSH_KEY_INSTRUCTIONS_KEY = "MIC_SCREEN_PUSH_KEY_INSTRUCTIONS";

		private const string TROOP_SCREEN_INTRO_KEY = "TROOP_SCREEN_INTRO";

		private const string TROOP_SCREEN_INSTRUCTIONS_KEY = "TROOP_SCREEN_INSTRUCTIONS";

		private const string TROOP_SCREEN_CURRENT_TROOP_KEY = "TROOP_SCREEN_CURRENT_TROOP";

		private const string TROOP_SCREEN_IN_QUEUE_KEY = "TROOP_SCREEN_IN_QUEUE";

		private const string TROOP_SCREEN_PLAYERS_IN_TROOP_KEY = "TROOP_SCREEN_PLAYERS_IN_TROOP";

		private const string TROOP_SCREEN_DEFAULT_QUEUE_KEY = "TROOP_SCREEN_DEFAULT_QUEUE";

		private const string TROOP_SCREEN_CURRENT_QUEUE_KEY = "TROOP_SCREEN_CURRENT_QUEUE";

		private const string TROOP_SCREEN_TROOP_QUEUE_KEY = "TROOP_SCREEN_TROOP_QUEUE";

		private const string TROOP_SCREEN_LEAVE_KEY = "TROOP_SCREEN_LEAVE";

		private const string TROOP_SCREEN_NOT_IN_TROOP_KEY = "TROOP_SCREEN_NOT_IN_TROOP";

		private const string TROOP_SCREEN_JOIN_TROOP_KEY = "TROOP_SCREEN_JOIN_TROOP";

		private const string TROOP_SCREEN_KID_PROHIBITED_VERB_KEY = "TROOP_SCREEN_KID_PROHIBITED_VERB";

		private const string TROOP_SCREEN_DISABLED_KEY = "TROOP_SCREEN_DISABLED";

		private const string TROOP_SCREEN_KID_DESC_KEY = "TROOP_SCREEN_KID_DESC";

		private const bool HIDE_SCREENS = false;

		public const string NAMETAG_PLAYER_PREF_KEY = "nameTagsOn";

		[OnEnterPlay_SetNull]
		public static volatile GorillaComputer instance;

		[OnEnterPlay_Set(false)]
		public static bool hasInstance = false;

		[OnEnterPlay_SetNull]
		private static Action<bool> onNametagSettingChangedAction;

		public bool tryGetTimeAgain;

		public Material unpressedMaterial;

		public Material pressedMaterial;

		public string currentTextField;

		public float buttonFadeTime;

		public string offlineTextInitialString;

		public GorillaText screenText;

		public GorillaText functionSelectText;

		public GorillaText wallScreenText;

		private Locale _lastLocaleChecked_Version;

		private Locale _lastLocaleChecked_Connect;

		private string _cachedVersionMismatch = "PLEASE UPDATE TO THE LATEST VERSION OF GORILLA TAG. YOU'RE ON AN OLD VERSION. FEEL FREE TO RUN AROUND, BUT YOU WON'T BE ABLE TO PLAY WITH ANYONE ELSE.";

		private string _cachedUnableToConnect = "UNABLE TO CONNECT TO THE INTERNET. PLEASE CHECK YOUR CONNECTION AND RESTART THE GAME.";

		public Material wrongVersionMaterial;

		public MeshRenderer wallScreenRenderer;

		public MeshRenderer computerScreenRenderer;

		public long startupMillis;

		public DateTime startupTime;

		public GameModeType lastPressedGameModeType;

		public string lastPressedGameMode;

		public WatchableStringSO currentGameMode;

		public WatchableStringSO currentGameModeText;

		public int includeUpdatedServerSynchTest;

		public PhotonNetworkController networkController;

		public float updateCooldown = 1f;

		private float defaultUpdateCooldown;

		private float micUpdateCooldown = 0.01f;

		public float lastUpdateTime;

		private float deltaTime;

		public bool isConnectedToMaster;

		public bool internetFailure;

		public string[] _allowedMapsToJoin;

		public bool limitOnlineScreens;

		[Header("State vars")]
		public bool stateUpdated;

		public bool screenChanged;

		public bool initialized;

		public List<GorillaComputer.StateOrderItem> OrderList = new List<GorillaComputer.StateOrderItem>
		{
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Room),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Name),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Language, "Lang"),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Turn),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Mic),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Queue),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Troop),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Group),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Voice),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.AutoMute, "Automod"),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Visuals, "Items"),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Credits),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Support)
		};

		public string Pointer = "<-";

		public int highestCharacterCount;

		public List<string> FunctionNames = new List<string>();

		public int FunctionsCount;

		[Header("Room vars")]
		public string roomToJoin;

		public bool roomFull;

		public bool roomNotAllowed;

		[Header("Mic vars")]
		public string pttType;

		private GorillaSpeakerLoudness speakerLoudness;

		private float micInputTestTimer;

		public float micInputTestTimerThreshold = 10f;

		[Header("Automute vars")]
		public string autoMuteType;

		[Header("Queue vars")]
		public string currentQueue;

		public bool allowedInCompetitive;

		[Header("Group Vars")]
		public string groupMapJoin;

		public int groupMapJoinIndex;

		public GorillaFriendCollider friendJoinCollider;

		[Header("Troop vars")]
		public string troopName;

		public bool troopQueueActive;

		public string troopToJoin;

		private bool rememberTroopQueueState;

		[Header("Join Triggers")]
		public Dictionary<string, GorillaNetworkJoinTrigger> primaryTriggersByZone = new Dictionary<string, GorillaNetworkJoinTrigger>();

		public string voiceChatOn;

		[Header("Mode select vars")]
		public ModeSelectButton[] modeSelectButtons;

		public string version;

		public string buildDate;

		public string buildCode;

		[Header("Cosmetics")]
		public bool disableParticles;

		public float instrumentVolume;

		[Header("Credits")]
		public CreditsView creditsView;

		[Header("Handedness")]
		public bool leftHanded;

		[Header("Name state vars")]
		public string savedName;

		public string currentName;

		public TextAsset exactOneWeekFile;

		public TextAsset anywhereOneWeekFile;

		public TextAsset anywhereTwoWeekFile;

		private List<GorillaComputer.ComputerState> _filteredStates = new List<GorillaComputer.ComputerState>();

		private List<GorillaComputer.StateOrderItem> _activeOrderList = new List<GorillaComputer.StateOrderItem>();

		private Stack<GorillaComputer.ComputerState> stateStack = new Stack<GorillaComputer.ComputerState>();

		private GorillaComputer.ComputerState currentComputerState;

		private GorillaComputer.ComputerState previousComputerState;

		private int currentStateIndex;

		private int usersBanned;

		private float redValue;

		private string redText;

		private float blueValue;

		private string blueText;

		private float greenValue;

		private string greenText;

		private int colorCursorLine;

		private string warningConfirmationInputString = string.Empty;

		private bool displaySupport;

		private string[] exactOneWeek;

		private string[] anywhereOneWeek;

		private string[] anywhereTwoWeek;

		private GorillaComputer.RedemptionResult redemptionResult;

		private string redemptionCode = "";

		private bool playerInVirtualStump;

		private string virtualStumpRoomPrepend = "";

		private WaitForSeconds waitOneSecond = new WaitForSeconds(1f);

		private Coroutine LoadingRoutine;

		private List<string> topTroops = new List<string>();

		private bool hasRequestedInitialTroopPopulation;

		private int currentTroopPopulation = -1;

		private float lastCheckedWifi;

		private float checkIfDisconnectedSeconds = 10f;

		private float checkIfConnectedSeconds = 1f;

		private bool didInitializeGameMode;

		private static int sessionCount = -1;

		private const bool k_debug_shouldResetSessionCount = false;

		private const bool k_debug_shouldResetGameMode = false;

		private const string k_sessionCountKey = "sessionCount";

		internal const GameModeType k_defaultGameMode = GameModeType.SuperInfect;

		internal const GameModeType k_noobGameMode = GameModeType.Infection;

		private const int k_noobSessionCountThreshold = 4;

		private float troopPopulationCheckCooldown = 3f;

		private float nextPopulationCheckTime;

		public Action OnServerTimeUpdated;

		private const string ENABLED_COLOUR = "#85ffa5";

		private const string DISABLED_COLOUR = "\"RED\"";

		private const string FAMILY_PORTAL_URL = "k-id.com/code";

		private float _updateAttemptCooldown = 15f;

		private float _nextUpdateAttemptTime;

		private bool _waitingForUpdatedSession;

		private GorillaComputer.EKidScreenState _currentScreentState = GorillaComputer.EKidScreenState.Show_OTP;

		private string[] _interestedPermissionNames = new string[]
		{
			"custom-username",
			"voice-chat",
			"join-groups"
		};

		private const string LANG_SCREEN_TITLE_KEY = "LANG_SCREEN_TITLE";

		private const string LANG_SCREEN_INSTRUCTIONS_KEY = "LANG_SCREEN_INSTRUCTIONS";

		private const string LANG_SCREEN_CURRENT_LANGUAGE_KEY = "LANG_SCREEN_CURRENT_LANGUAGE";

		private StringBuilder _languagesDisplaySB = new StringBuilder();

		private Locale _previousLocalisationSetting;

		public enum ComputerState
		{
			Startup,
			Color,
			Name,
			Turn,
			Mic,
			Room,
			Queue,
			Group,
			Voice,
			AutoMute,
			Credits,
			Visuals,
			Time,
			NameWarning,
			Loading,
			Support,
			Troop,
			KID,
			Redemption,
			Language
		}

		private enum NameCheckResult
		{
			Success,
			Warning,
			Ban
		}

		public enum RedemptionResult
		{
			Empty,
			Invalid,
			Checking,
			AlreadyUsed,
			TooEarly,
			TooLate,
			Success
		}

		[Serializable]
		public class StateOrderItem
		{
			public StateOrderItem()
			{
			}

			public StateOrderItem(GorillaComputer.ComputerState state)
			{
				this.State = state;
			}

			public StateOrderItem(GorillaComputer.ComputerState state, string overrideName)
			{
				this.State = state;
				this.OverrideName = overrideName;
			}

			public string GetName()
			{
				if (this._previousLocale == LocalizationSettings.SelectedLocale && !string.IsNullOrEmpty(this._cachedTranslation))
				{
					return this._cachedTranslation;
				}
				if (this.StringReference == null || this.StringReference.IsEmpty)
				{
					return this.GetPreLocalisedName();
				}
				this._previousLocale = LocalizationSettings.SelectedLocale;
				string localizedString = this.StringReference.GetLocalizedString();
				this._cachedTranslation = ((localizedString != null) ? localizedString.ToUpper() : null);
				if (string.IsNullOrEmpty(this._cachedTranslation))
				{
					if (LocalisationManager.ApplicationRunning)
					{
						string[] array = new string[5];
						array[0] = "[LOCALIZATION::STATE_ORDER_ITEM] Failed to get translation for selected locale [";
						int num = 1;
						Locale previousLocale = this._previousLocale;
						array[num] = (((previousLocale != null) ? previousLocale.LocaleName : null) ?? "NULL");
						array[2] = ", for item [";
						array[3] = this.State.GetName<GorillaComputer.ComputerState>();
						array[4] = "]";
						Debug.LogError(string.Concat(array));
					}
					this._cachedTranslation = "";
				}
				return this._cachedTranslation;
			}

			public string GetPreLocalisedName()
			{
				if (!string.IsNullOrEmpty(this.OverrideName))
				{
					return this.OverrideName.ToUpper();
				}
				return this.State.ToString().ToUpper();
			}

			public GorillaComputer.ComputerState State;

			[Tooltip("Case not important - ToUpper applied at runtime")]
			public string OverrideName = "";

			public LocalizedString StringReference;

			private Locale _previousLocale;

			private string _cachedTranslation = "";
		}

		private enum EKidScreenState
		{
			Ready,
			Show_OTP,
			Show_Setup_Screen
		}
	}
}
