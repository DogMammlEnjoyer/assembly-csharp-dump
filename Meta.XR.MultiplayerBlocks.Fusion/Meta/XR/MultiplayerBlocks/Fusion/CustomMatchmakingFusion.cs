using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Fusion;
using Meta.XR.MultiplayerBlocks.Shared;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Meta.XR.MultiplayerBlocks.Fusion
{
	public class CustomMatchmakingFusion : MonoBehaviour, CustomMatchmaking.ICustomMatchmakingBehaviour
	{
		public GameMode GameMode
		{
			get
			{
				return this.gameMode;
			}
			set
			{
				this.gameMode = value;
			}
		}

		private void Awake()
		{
			this._runnerPrefab = Object.FindFirstObjectByType<NetworkRunner>();
			if (this._runnerPrefab == null)
			{
				throw new InvalidOperationException("Fusion NetworkRunner not found");
			}
		}

		private void OnEnable()
		{
			FusionBBEvents.OnSessionListUpdated += this.OnSessionListUpdated;
		}

		private void OnDisable()
		{
			FusionBBEvents.OnSessionListUpdated -= this.OnSessionListUpdated;
		}

		private NetworkRunner InitializeNetworkRunner()
		{
			this._runnerPrefab.gameObject.SetActive(false);
			NetworkRunner networkRunner = Object.Instantiate<NetworkRunner>(this._runnerPrefab);
			networkRunner.gameObject.SetActive(true);
			Object.DontDestroyOnLoad(networkRunner);
			networkRunner.name = "Temporary Runner Prefab";
			return networkRunner;
		}

		public Task<CustomMatchmaking.RoomOperationResult> CreateRoom(CustomMatchmaking.RoomCreationOptions options)
		{
			CustomMatchmakingFusion.<CreateRoom>d__11 <CreateRoom>d__;
			<CreateRoom>d__.<>t__builder = AsyncTaskMethodBuilder<CustomMatchmaking.RoomOperationResult>.Create();
			<CreateRoom>d__.<>4__this = this;
			<CreateRoom>d__.options = options;
			<CreateRoom>d__.<>1__state = -1;
			<CreateRoom>d__.<>t__builder.Start<CustomMatchmakingFusion.<CreateRoom>d__11>(ref <CreateRoom>d__);
			return <CreateRoom>d__.<>t__builder.Task;
		}

		public Task<CustomMatchmaking.RoomOperationResult> JoinRoom(string roomToken, string roomPassword = null)
		{
			CustomMatchmakingFusion.<JoinRoom>d__12 <JoinRoom>d__;
			<JoinRoom>d__.<>t__builder = AsyncTaskMethodBuilder<CustomMatchmaking.RoomOperationResult>.Create();
			<JoinRoom>d__.<>4__this = this;
			<JoinRoom>d__.roomToken = roomToken;
			<JoinRoom>d__.roomPassword = roomPassword;
			<JoinRoom>d__.<>1__state = -1;
			<JoinRoom>d__.<>t__builder.Start<CustomMatchmakingFusion.<JoinRoom>d__12>(ref <JoinRoom>d__);
			return <JoinRoom>d__.<>t__builder.Task;
		}

		public Task<CustomMatchmaking.RoomOperationResult> JoinOpenRoom(string lobbyName)
		{
			CustomMatchmakingFusion.<JoinOpenRoom>d__13 <JoinOpenRoom>d__;
			<JoinOpenRoom>d__.<>t__builder = AsyncTaskMethodBuilder<CustomMatchmaking.RoomOperationResult>.Create();
			<JoinOpenRoom>d__.<>4__this = this;
			<JoinOpenRoom>d__.lobbyName = lobbyName;
			<JoinOpenRoom>d__.<>1__state = -1;
			<JoinOpenRoom>d__.<>t__builder.Start<CustomMatchmakingFusion.<JoinOpenRoom>d__13>(ref <JoinOpenRoom>d__);
			return <JoinOpenRoom>d__.<>t__builder.Task;
		}

		public void LeaveRoom()
		{
			for (int i = NetworkRunner.Instances.Count - 1; i >= 0; i--)
			{
				NetworkRunner networkRunner = NetworkRunner.Instances[i];
				if (!(networkRunner == null) && networkRunner.IsRunning)
				{
					networkRunner.Shutdown(true, ShutdownReason.Ok, false);
					Object.Destroy(networkRunner.gameObject);
				}
			}
		}

		public bool SupportsRoomPassword
		{
			get
			{
				return false;
			}
		}

		public bool IsConnected
		{
			get
			{
				return CustomMatchmakingFusion.GetActiveNetworkRunner() != null;
			}
		}

		public string ConnectedRoomToken
		{
			get
			{
				NetworkRunner activeNetworkRunner = CustomMatchmakingFusion.GetActiveNetworkRunner();
				if (activeNetworkRunner == null)
				{
					return null;
				}
				return activeNetworkRunner.SessionInfo.Name;
			}
		}

		private static NetworkRunner GetActiveNetworkRunner()
		{
			for (int i = NetworkRunner.Instances.Count - 1; i >= 0; i--)
			{
				NetworkRunner networkRunner = NetworkRunner.Instances[i];
				if (networkRunner != null && networkRunner.IsRunning)
				{
					return networkRunner;
				}
			}
			return null;
		}

		private static NetworkSceneInfo GetSceneInfo()
		{
			SceneRef sceneRef = default(SceneRef);
			SceneRef sceneRef2;
			if (CustomMatchmakingFusion.TryGetActiveSceneRef(out sceneRef2))
			{
				sceneRef = sceneRef2;
			}
			NetworkSceneInfo result = default(NetworkSceneInfo);
			if (sceneRef.IsValid)
			{
				result.AddSceneRef(sceneRef, LoadSceneMode.Additive, LocalPhysicsMode.None, false);
			}
			return result;
		}

		private static bool TryGetActiveSceneRef(out SceneRef sceneRef)
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

		private void ClearSessionList()
		{
			this._sessionList = null;
		}

		private Task<List<SessionInfo>> GetSessionList(float timeoutS)
		{
			CustomMatchmakingFusion.<GetSessionList>d__25 <GetSessionList>d__;
			<GetSessionList>d__.<>t__builder = AsyncTaskMethodBuilder<List<SessionInfo>>.Create();
			<GetSessionList>d__.<>4__this = this;
			<GetSessionList>d__.timeoutS = timeoutS;
			<GetSessionList>d__.<>1__state = -1;
			<GetSessionList>d__.<>t__builder.Start<CustomMatchmakingFusion.<GetSessionList>d__25>(ref <GetSessionList>d__);
			return <GetSessionList>d__.<>t__builder.Task;
		}

		protected virtual SessionInfo SelectSessionToJoinFromList(List<SessionInfo> sessionList)
		{
			if (sessionList.Count != 0)
			{
				return sessionList[0];
			}
			return null;
		}

		private void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
		{
			this._sessionList = sessionList;
		}

		[SerializeField]
		[Tooltip("Indicates the chosen game mode to be used.")]
		private GameMode gameMode = GameMode.Shared;

		[SerializeField]
		[Tooltip("Amount of time in seconds to wait for receiving the session list of a lobby before timing out.")]
		private int getSessionListTimeoutS = 10;

		private NetworkRunner _runnerPrefab;

		private List<SessionInfo> _sessionList;
	}
}
