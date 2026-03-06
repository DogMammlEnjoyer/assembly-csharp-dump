using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Fusion
{
	[DisallowMultipleComponent]
	[AddComponentMenu("Fusion/Fusion Bootstrap")]
	[ScriptHelp(BackColor = ScriptHeaderBackColor.Steel)]
	public class FusionBootstrap : Behaviour
	{
		public FusionBootstrap.Stage CurrentStage
		{
			get
			{
				return this._currentStage;
			}
			internal set
			{
				this._currentStage = value;
			}
		}

		public int LastCreatedClientIndex { get; internal set; }

		public GameMode CurrentServerMode { get; internal set; }

		protected bool CanAddClients
		{
			get
			{
				return this.CurrentStage == FusionBootstrap.Stage.AllConnected && this.CurrentServerMode > (GameMode)0 && this.CurrentServerMode != GameMode.Shared && this.CurrentServerMode != GameMode.Single;
			}
		}

		protected bool CanAddSharedClients
		{
			get
			{
				return this.CurrentStage == FusionBootstrap.Stage.AllConnected && this.CurrentServerMode > (GameMode)0 && this.CurrentServerMode == GameMode.Shared;
			}
		}

		protected bool IsShutdown
		{
			get
			{
				return this.CurrentStage == FusionBootstrap.Stage.Disconnected;
			}
		}

		protected bool IsShutdownAndMultiPeer
		{
			get
			{
				return this.CurrentStage == FusionBootstrap.Stage.Disconnected && this.UsingMultiPeerMode;
			}
		}

		protected bool UsingMultiPeerMode
		{
			get
			{
				return NetworkProjectConfig.Global.PeerMode == NetworkProjectConfig.PeerModes.Multiple;
			}
		}

		protected bool ShowAutoClients
		{
			get
			{
				return this.UsingMultiPeerMode && (this.StartMode == FusionBootstrap.StartModes.UserInterface || (this.StartMode == FusionBootstrap.StartModes.Automatic && this.AutoStartAs != GameMode.Single));
			}
		}

		protected virtual void Start()
		{
			if (FusionBootstrap._initialScenePath == null)
			{
				if (string.IsNullOrEmpty(this.InitialScenePath))
				{
					Scene activeScene = SceneManager.GetActiveScene();
					if (activeScene.IsValid())
					{
						FusionBootstrap._initialScenePath = activeScene.path;
					}
					else
					{
						FusionBootstrap._initialScenePath = SceneManager.GetSceneByBuildIndex(0).path;
					}
					this.InitialScenePath = FusionBootstrap._initialScenePath;
				}
				else
				{
					FusionBootstrap._initialScenePath = this.InitialScenePath;
				}
			}
			bool flag = NetworkProjectConfig.Global.PeerMode == NetworkProjectConfig.PeerModes.Multiple;
			NetworkRunner networkRunner = Object.FindFirstObjectByType<NetworkRunner>();
			if (networkRunner && networkRunner != this.RunnerPrefab)
			{
				if (networkRunner.State != NetworkRunner.States.Shutdown)
				{
					base.enabled = false;
					FusionBootstrapDebugGUI component = base.GetComponent<FusionBootstrapDebugGUI>();
					if (component)
					{
						Object.Destroy(component);
					}
					Object.Destroy(this);
					return;
				}
				if (this.RunnerPrefab == null)
				{
					this.RunnerPrefab = networkRunner;
				}
			}
			if (FusionMppm.Status == FusionMppmStatus.VirtualInstance && this.AutoConnectVirtualInstances)
			{
				base.StartCoroutine(this.StartWithMppmVirtualInstance());
				return;
			}
			FusionBootstrap.StartModes startMode = this.StartMode;
			SceneRef sceneRef;
			if (startMode != FusionBootstrap.StartModes.Automatic)
			{
				if (startMode == FusionBootstrap.StartModes.Manual)
				{
					return;
				}
				this.ShowUserInterface();
			}
			else if (this.TryGetSceneRef(out sceneRef))
			{
				int clientCount;
				if (this.AutoStartAs == GameMode.Single)
				{
					clientCount = 0;
				}
				else if (flag)
				{
					clientCount = this.AutoClients;
				}
				else if (this.AutoStartAs == GameMode.Client || this.AutoStartAs == GameMode.Shared || this.AutoStartAs == GameMode.AutoHostOrClient)
				{
					clientCount = 1;
				}
				else
				{
					clientCount = 0;
				}
				base.StartCoroutine(this.StartWithClients(this.AutoStartAs, sceneRef, clientCount));
				return;
			}
		}

		protected void ShowUserInterface()
		{
			FusionBootstrapDebugGUI fusionBootstrapDebugGUI;
			if (!base.TryGetComponent<FusionBootstrapDebugGUI>(out fusionBootstrapDebugGUI))
			{
				fusionBootstrapDebugGUI = base.gameObject.AddComponent<FusionBootstrapDebugGUI>();
			}
			fusionBootstrapDebugGUI.enabled = true;
		}

		private bool TryGetSceneRef(out SceneRef sceneRef)
		{
			Scene activeScene = SceneManager.GetActiveScene();
			if (activeScene.buildIndex < 0 || activeScene.buildIndex >= SceneManager.sceneCountInBuildSettings)
			{
				sceneRef = default(SceneRef);
				return false;
			}
			sceneRef = SceneRef.FromIndex(activeScene.buildIndex);
			return true;
		}

		[EditorButton(EditorButtonVisibility.PlayMode, 0, false)]
		[DrawIf("IsShutdown", Hide = true)]
		public virtual void StartSinglePlayer()
		{
			SceneRef sceneRef;
			if (this.TryGetSceneRef(out sceneRef))
			{
				base.StartCoroutine(this.StartWithClients(GameMode.Single, sceneRef, 0));
			}
		}

		[EditorButton(EditorButtonVisibility.PlayMode, 0, false)]
		[DrawIf("IsShutdown", Hide = true)]
		public virtual void StartServer()
		{
			SceneRef sceneRef;
			if (this.TryGetSceneRef(out sceneRef))
			{
				base.StartCoroutine(this.StartWithClients(GameMode.Server, sceneRef, 0));
			}
		}

		[EditorButton(EditorButtonVisibility.PlayMode, 0, false)]
		[DrawIf("IsShutdown", Hide = true)]
		public virtual void StartHost()
		{
			SceneRef sceneRef;
			if (this.TryGetSceneRef(out sceneRef))
			{
				base.StartCoroutine(this.StartWithClients(GameMode.Host, sceneRef, 0));
			}
		}

		[EditorButton(EditorButtonVisibility.PlayMode, 0, false)]
		[DrawIf("IsShutdown", Hide = true)]
		public virtual void StartClient()
		{
			base.StartCoroutine(this.StartWithClients(GameMode.Client, default(SceneRef), 1));
		}

		[EditorButton(EditorButtonVisibility.PlayMode, 0, false)]
		[DrawIf("IsShutdown", Hide = true)]
		public virtual void StartSharedClient()
		{
			SceneRef sceneRef;
			if (this.TryGetSceneRef(out sceneRef))
			{
				base.StartCoroutine(this.StartWithClients(GameMode.Shared, sceneRef, 1));
			}
		}

		[EditorButton("Start Auto Host Or Client", EditorButtonVisibility.PlayMode, 0, false)]
		[DrawIf("IsShutdown", Hide = true)]
		public virtual void StartAutoClient()
		{
			SceneRef sceneRef;
			if (this.TryGetSceneRef(out sceneRef))
			{
				base.StartCoroutine(this.StartWithClients(GameMode.AutoHostOrClient, sceneRef, 1));
			}
		}

		[EditorButton(EditorButtonVisibility.PlayMode, 0, false)]
		[DrawIf("IsShutdown", Hide = true)]
		public virtual void StartServerPlusClients()
		{
			this.StartServerPlusClients(this.AutoClients);
		}

		[EditorButton(EditorButtonVisibility.PlayMode, 0, false)]
		[DrawIf("IsShutdown", Hide = true)]
		public void StartHostPlusClients()
		{
			this.StartHostPlusClients(this.AutoClients);
		}

		[EditorButton(EditorButtonVisibility.PlayMode, 0, false)]
		[DrawIf("CurrentStage", Hide = true)]
		public void Shutdown()
		{
			this.ShutdownAll();
		}

		public virtual void StartServerPlusClients(int clientCount)
		{
			if (NetworkProjectConfig.Global.PeerMode == NetworkProjectConfig.PeerModes.Multiple)
			{
				SceneRef sceneRef;
				if (this.TryGetSceneRef(out sceneRef))
				{
					base.StartCoroutine(this.StartWithClients(GameMode.Server, sceneRef, clientCount));
					return;
				}
			}
			else
			{
				Debug.LogWarning("Unable to start multiple NetworkRunners in Unique Instance mode.");
			}
		}

		public void StartHostPlusClients(int clientCount)
		{
			if (NetworkProjectConfig.Global.PeerMode == NetworkProjectConfig.PeerModes.Multiple)
			{
				SceneRef sceneRef;
				if (this.TryGetSceneRef(out sceneRef))
				{
					base.StartCoroutine(this.StartWithClients(GameMode.Host, sceneRef, clientCount));
					return;
				}
			}
			else
			{
				Debug.LogWarning("Unable to start multiple NetworkRunners in Unique Instance mode.");
			}
		}

		public void StartMultipleClients(int clientCount)
		{
			if (NetworkProjectConfig.Global.PeerMode == NetworkProjectConfig.PeerModes.Multiple)
			{
				SceneRef sceneRef;
				if (this.TryGetSceneRef(out sceneRef))
				{
					base.StartCoroutine(this.StartWithClients(GameMode.Client, sceneRef, clientCount));
					return;
				}
			}
			else
			{
				Debug.LogWarning("Unable to start multiple NetworkRunners in Unique Instance mode.");
			}
		}

		public void StartMultipleSharedClients(int clientCount)
		{
			if (NetworkProjectConfig.Global.PeerMode == NetworkProjectConfig.PeerModes.Multiple)
			{
				SceneRef sceneRef;
				if (this.TryGetSceneRef(out sceneRef))
				{
					base.StartCoroutine(this.StartWithClients(GameMode.Shared, sceneRef, clientCount));
					return;
				}
			}
			else
			{
				Debug.LogWarning("Unable to start multiple NetworkRunners in Unique Instance mode.");
			}
		}

		public void StartMultipleAutoClients(int clientCount)
		{
			if (NetworkProjectConfig.Global.PeerMode == NetworkProjectConfig.PeerModes.Multiple)
			{
				SceneRef sceneRef;
				if (this.TryGetSceneRef(out sceneRef))
				{
					base.StartCoroutine(this.StartWithClients(GameMode.AutoHostOrClient, sceneRef, clientCount));
					return;
				}
			}
			else
			{
				Debug.LogWarning("Unable to start multiple NetworkRunners in Unique Instance mode.");
			}
		}

		public void ShutdownAll()
		{
			foreach (NetworkRunner networkRunner in NetworkRunner.Instances.ToList<NetworkRunner>())
			{
				if (networkRunner != null && networkRunner.IsRunning)
				{
					networkRunner.Shutdown(true, ShutdownReason.Ok, false);
				}
			}
			SceneManager.LoadSceneAsync(FusionBootstrap._initialScenePath);
			Object.Destroy(this.RunnerPrefab.gameObject);
			Object.Destroy(base.gameObject);
			this.CurrentStage = FusionBootstrap.Stage.Disconnected;
			this.CurrentServerMode = (GameMode)0;
		}

		protected IEnumerator StartWithClients(GameMode serverMode, SceneRef sceneRef, int clientCount)
		{
			if (this.CurrentStage != FusionBootstrap.Stage.Disconnected)
			{
				yield break;
			}
			bool flag = serverMode != GameMode.Shared && serverMode != GameMode.Client && serverMode != GameMode.AutoHostOrClient;
			if (!flag && clientCount == 0)
			{
				Debug.LogError(string.Format("{0} is set to {1}, and {2} is set to zero. Starting no network runners.", "GameMode", serverMode, "clientCount"));
				yield break;
			}
			this.CurrentStage = FusionBootstrap.Stage.StartingUp;
			SceneManager.GetActiveScene();
			if (!this.RunnerPrefab)
			{
				Debug.LogError("RunnerPrefab not set, can't perform debug start.");
				yield break;
			}
			this.RunnerPrefab = Object.Instantiate<NetworkRunner>(this.RunnerPrefab);
			Object.DontDestroyOnLoad(this.RunnerPrefab);
			this.RunnerPrefab.name = "Temporary Runner Prefab";
			NetworkProjectConfig global = NetworkProjectConfig.Global;
			if (global.PeerMode != NetworkProjectConfig.PeerModes.Multiple)
			{
				int num = flag ? 0 : 1;
				if (clientCount > num)
				{
					Debug.LogWarning(string.Format("Instance mode must be set to {0} to perform a debug start multiple peers. Restricting client count to {1}.", "Multiple", num));
					clientCount = num;
				}
			}
			bool flag2 = (serverMode == GameMode.Shared || serverMode == GameMode.AutoHostOrClient || serverMode == GameMode.Server || serverMode == GameMode.Host) && clientCount > 1 && global.PeerMode == NetworkProjectConfig.PeerModes.Multiple;
			bool flag3 = FusionMppm.Status == FusionMppmStatus.MainInstance;
			if ((flag2 || flag3) && string.IsNullOrEmpty(this.DefaultRoomName))
			{
				this.DefaultRoomName = Guid.NewGuid().ToString();
				Debug.Log("Generated Session Name: " + this.DefaultRoomName);
			}
			if (base.gameObject.transform.parent)
			{
				Debug.LogWarning("FusionBootstrap can't be a child game object, un-parenting.");
				base.gameObject.transform.parent = null;
			}
			Object.DontDestroyOnLoad(base.gameObject);
			this.CurrentServerMode = serverMode;
			if (flag)
			{
				this._server = Object.Instantiate<NetworkRunner>(this.RunnerPrefab);
				this._server.name = serverMode.ToString();
				Task serverTask = this.InitializeNetworkRunner(this._server, serverMode, NetAddress.Any(this.ServerPort), sceneRef, delegate(NetworkRunner runner)
				{
				}, null);
				while (!serverTask.IsCompleted)
				{
					yield return new WaitForSeconds(1f);
				}
				if (serverTask.IsFaulted)
				{
					this.ShutdownAll();
					yield break;
				}
				yield return this.StartClients(clientCount, serverMode, sceneRef);
				serverTask = null;
			}
			else
			{
				yield return this.StartClients(clientCount, serverMode, sceneRef);
			}
			if (FusionMppm.Status == FusionMppmStatus.MainInstance && serverMode != GameMode.Single && this.VirtualInstanceConnectDelay > 0f)
			{
				yield return new WaitForSecondsRealtime(this.VirtualInstanceConnectDelay);
			}
			yield break;
		}

		protected IEnumerator StartWithMppmVirtualInstance()
		{
			while (FusionBootstrap.StartCommand.Instance == null)
			{
				yield return null;
			}
			FusionBootstrap.StartCommand instance = FusionBootstrap.StartCommand.Instance;
			FusionBootstrap.StartCommand.Instance = null;
			this.DefaultRoomName = instance.RoomName;
			yield return this.StartClients(instance.ClientCount, instance.IsShared ? GameMode.Shared : GameMode.Client, instance.InitialScene);
			yield break;
		}

		[EditorButton("Add Additional Client", EditorButtonVisibility.PlayMode, 0, false)]
		[DrawIf("CanAddClients", Hide = true)]
		public void AddClient()
		{
			SceneRef sceneRef;
			if (this.TryGetSceneRef(out sceneRef))
			{
				this.AddClient(GameMode.Client, sceneRef);
			}
		}

		[EditorButton("Add Additional Shared Client", EditorButtonVisibility.PlayMode, 0, false)]
		[DrawIf("CanAddSharedClients", Hide = true)]
		public void AddSharedClient()
		{
			SceneRef sceneRef;
			if (this.TryGetSceneRef(out sceneRef))
			{
				this.AddClient(GameMode.Shared, sceneRef);
			}
		}

		public Task AddClient(GameMode serverMode, SceneRef sceneRef)
		{
			NetworkRunner networkRunner = Object.Instantiate<NetworkRunner>(this.RunnerPrefab);
			Object.DontDestroyOnLoad(networkRunner);
			Object @object = networkRunner;
			string format = "Client {0}";
			int num = 65;
			int lastCreatedClientIndex = this.LastCreatedClientIndex;
			this.LastCreatedClientIndex = lastCreatedClientIndex + 1;
			@object.name = string.Format(format, (char)(num + lastCreatedClientIndex));
			GameMode gameMode = GameMode.Client;
			if (serverMode == GameMode.Shared || serverMode == GameMode.AutoHostOrClient)
			{
				gameMode = serverMode;
			}
			return this.InitializeNetworkRunner(networkRunner, gameMode, NetAddress.Any(0), sceneRef, null, null);
		}

		protected IEnumerator StartClients(int clientCount, GameMode serverMode, SceneRef sceneRef = default(SceneRef))
		{
			this.CurrentStage = FusionBootstrap.Stage.ConnectingClients;
			List<Task> clientTasks = new List<Task>();
			int num;
			for (int i = 0; i < clientCount; i = num)
			{
				clientTasks.Add(this.AddClient(serverMode, sceneRef));
				yield return new WaitForSeconds(this.ClientStartDelay);
				num = i + 1;
			}
			Task clientsStartTask = Task.WhenAll(clientTasks);
			while (!clientsStartTask.IsCompleted)
			{
				yield return new WaitForSeconds(1f);
			}
			if (clientsStartTask.IsFaulted)
			{
				Debug.LogWarning(clientsStartTask.Exception);
			}
			this.CurrentStage = FusionBootstrap.Stage.AllConnected;
			yield break;
		}

		protected virtual Task InitializeNetworkRunner(NetworkRunner runner, GameMode gameMode, NetAddress address, SceneRef scene, Action<NetworkRunner> onGameStarted, INetworkRunnerUpdater updater = null)
		{
			INetworkSceneManager networkSceneManager = runner.GetComponent<INetworkSceneManager>();
			if (networkSceneManager == null)
			{
				Debug.Log("NetworkRunner does not have any component implementing INetworkSceneManager interface, adding NetworkSceneManagerDefault.", runner);
				networkSceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
			}
			INetworkObjectProvider networkObjectProvider = runner.GetComponent<INetworkObjectProvider>();
			if (networkObjectProvider == null)
			{
				Debug.Log("NetworkRunner does not have any component implementing INetworkObjectProvider interface, adding NetworkObjectProviderDefault.", runner);
				networkObjectProvider = runner.gameObject.AddComponent<NetworkObjectProviderDefault>();
			}
			NetworkSceneInfo value = default(NetworkSceneInfo);
			if (scene.IsValid)
			{
				value.AddSceneRef(scene, LoadSceneMode.Additive, LocalPhysicsMode.None, false);
			}
			return runner.StartGame(new StartGameArgs
			{
				GameMode = gameMode,
				Address = new NetAddress?(address),
				Scene = new NetworkSceneInfo?(value),
				SessionName = this.DefaultRoomName,
				OnGameStarted = onGameStarted,
				SceneManager = networkSceneManager,
				Updater = updater,
				ObjectProvider = networkObjectProvider
			});
		}

		private static bool IsMPPMEnabled
		{
			get
			{
				return FusionMppm.Status > FusionMppmStatus.Disabled;
			}
		}

		public bool ShouldShowGUI
		{
			get
			{
				return this.StartMode == FusionBootstrap.StartModes.UserInterface && (!this.AutoConnectVirtualInstances || FusionMppm.Status != FusionMppmStatus.VirtualInstance);
			}
		}

		[InlineHelp]
		[WarnIf("RunnerPrefab", false, "No RunnerPrefab supplied. Will search for a NetworkRunner in the scene at startup.", CompareOperator.Equal)]
		public NetworkRunner RunnerPrefab;

		[InlineHelp]
		[WarnIf("StartMode", 2L, "Start network by calling the methods StartHost(), StartServer(), StartClient(), StartHostPlusClients(), or StartServerPlusClients()", CompareOperator.Equal)]
		public FusionBootstrap.StartModes StartMode;

		[InlineHelp]
		[FormerlySerializedAs("Server")]
		[DrawIf("StartMode", 1L, CompareOperator.Equal, DrawIfMode.ReadOnly, Hide = true)]
		public GameMode AutoStartAs = GameMode.Shared;

		[InlineHelp]
		[DrawIf("StartMode", 0L, CompareOperator.Equal, DrawIfMode.ReadOnly, Hide = true)]
		public bool AutoHideGUI = true;

		[InlineHelp]
		[DrawIf("ShowAutoClients", Hide = true)]
		public int AutoClients = 1;

		[InlineHelp]
		public float ClientStartDelay = 0.1f;

		[InlineHelp]
		public ushort ServerPort;

		[InlineHelp]
		public string DefaultRoomName = string.Empty;

		[NonSerialized]
		private NetworkRunner _server;

		[InlineHelp]
		[ScenePath]
		public string InitialScenePath;

		private static string _initialScenePath;

		[InlineHelp]
		[SerializeField]
		[ReadOnly]
		protected FusionBootstrap.Stage _currentStage;

		[DrawIf("IsMPPMEnabled", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
		[Header("Multiplayer Play Mode")]
		public bool AutoConnectVirtualInstances = true;

		[DrawIf("IsMPPMEnabled", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
		public float VirtualInstanceConnectDelay = 1f;

		public enum StartModes
		{
			UserInterface,
			Automatic,
			Manual
		}

		public enum Stage
		{
			Disconnected,
			StartingUp,
			UnloadOriginalScene,
			ConnectingServer,
			ConnectingClients,
			AllConnected
		}

		[Serializable]
		private class StartCommand : FusionMppmCommand
		{
			public override void Execute()
			{
				FusionBootstrap.StartCommand.Instance = this;
			}

			public string RoomName;

			public SceneRef InitialScene;

			public int ClientCount;

			public bool IsShared;

			public static FusionBootstrap.StartCommand Instance;
		}
	}
}
