using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Meta.XR.BuildingBlocks;
using Meta.XR.ImmersiveDebugger;
using UnityEngine;
using UnityEngine.Events;

namespace Meta.XR.MultiplayerBlocks.Shared
{
	[ExecuteAlways]
	public class CustomMatchmaking : MonoBehaviour
	{
		public string LobbyName
		{
			get
			{
				return this.lobbyName;
			}
			set
			{
				this.lobbyName = value;
			}
		}

		public bool IsPrivate
		{
			get
			{
				return this.isPrivate;
			}
			set
			{
				this.isPrivate = value;
			}
		}

		public int MaxPlayersPerRoom
		{
			get
			{
				return this.maxPlayersPerRoom;
			}
			set
			{
				this.maxPlayersPerRoom = value;
			}
		}

		public bool IsPasswordProtected
		{
			get
			{
				return this.isPasswordProtected;
			}
			set
			{
				this.isPasswordProtected = value;
			}
		}

		private void OnEnable()
		{
			this.MatchmakingBehaviour = this.GetInterfaceComponent<CustomMatchmaking.ICustomMatchmakingBehaviour>();
			if (this.MatchmakingBehaviour == null && Application.isPlaying)
			{
				throw new InvalidOperationException("Using CustomMatchmaking without an ICustomMatchmakingBehaviour present in the game object.");
			}
		}

		[DebugMember(DebugColor.Gray, Category = "Custom Matchmaking")]
		public Task<CustomMatchmaking.RoomOperationResult> CreateRoom()
		{
			CustomMatchmaking.<CreateRoom>d__25 <CreateRoom>d__;
			<CreateRoom>d__.<>t__builder = AsyncTaskMethodBuilder<CustomMatchmaking.RoomOperationResult>.Create();
			<CreateRoom>d__.<>4__this = this;
			<CreateRoom>d__.<>1__state = -1;
			<CreateRoom>d__.<>t__builder.Start<CustomMatchmaking.<CreateRoom>d__25>(ref <CreateRoom>d__);
			return <CreateRoom>d__.<>t__builder.Task;
		}

		public Task<CustomMatchmaking.RoomOperationResult> CreateRoom(CustomMatchmaking.RoomCreationOptions options)
		{
			CustomMatchmaking.<CreateRoom>d__26 <CreateRoom>d__;
			<CreateRoom>d__.<>t__builder = AsyncTaskMethodBuilder<CustomMatchmaking.RoomOperationResult>.Create();
			<CreateRoom>d__.<>4__this = this;
			<CreateRoom>d__.options = options;
			<CreateRoom>d__.<>1__state = -1;
			<CreateRoom>d__.<>t__builder.Start<CustomMatchmaking.<CreateRoom>d__26>(ref <CreateRoom>d__);
			return <CreateRoom>d__.<>t__builder.Task;
		}

		public Task<CustomMatchmaking.RoomOperationResult> JoinRoom(string roomToken, string roomPassword)
		{
			CustomMatchmaking.<JoinRoom>d__27 <JoinRoom>d__;
			<JoinRoom>d__.<>t__builder = AsyncTaskMethodBuilder<CustomMatchmaking.RoomOperationResult>.Create();
			<JoinRoom>d__.<>4__this = this;
			<JoinRoom>d__.roomToken = roomToken;
			<JoinRoom>d__.roomPassword = roomPassword;
			<JoinRoom>d__.<>1__state = -1;
			<JoinRoom>d__.<>t__builder.Start<CustomMatchmaking.<JoinRoom>d__27>(ref <JoinRoom>d__);
			return <JoinRoom>d__.<>t__builder.Task;
		}

		public Task<CustomMatchmaking.RoomOperationResult> JoinOpenRoom(string roomLobby)
		{
			CustomMatchmaking.<JoinOpenRoom>d__28 <JoinOpenRoom>d__;
			<JoinOpenRoom>d__.<>t__builder = AsyncTaskMethodBuilder<CustomMatchmaking.RoomOperationResult>.Create();
			<JoinOpenRoom>d__.<>4__this = this;
			<JoinOpenRoom>d__.roomLobby = roomLobby;
			<JoinOpenRoom>d__.<>1__state = -1;
			<JoinOpenRoom>d__.<>t__builder.Start<CustomMatchmaking.<JoinOpenRoom>d__28>(ref <JoinOpenRoom>d__);
			return <JoinOpenRoom>d__.<>t__builder.Task;
		}

		public void LeaveRoom()
		{
			this.MatchmakingBehaviour.LeaveRoom();
			UnityEvent unityEvent = this.onRoomLeaveFinished;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}

		[DebugMember(DebugColor.Gray, Category = "Custom Matchmaking", Tweakable = false)]
		public bool IsConnected
		{
			get
			{
				CustomMatchmaking.ICustomMatchmakingBehaviour matchmakingBehaviour = this.MatchmakingBehaviour;
				return matchmakingBehaviour != null && matchmakingBehaviour.IsConnected;
			}
		}

		[DebugMember(DebugColor.Gray, Category = "Custom Matchmaking", Tweakable = false)]
		public string ConnectedRoomToken
		{
			get
			{
				CustomMatchmaking.ICustomMatchmakingBehaviour matchmakingBehaviour = this.MatchmakingBehaviour;
				return ((matchmakingBehaviour != null) ? matchmakingBehaviour.ConnectedRoomToken : null) ?? string.Empty;
			}
		}

		protected virtual string GenerateRoomPassword()
		{
			return RunTimeUtils.GenerateRandomString(16, true, true, true, false);
		}

		internal bool SupportsRoomPassword
		{
			get
			{
				CustomMatchmaking.ICustomMatchmakingBehaviour matchmakingBehaviour = this.MatchmakingBehaviour;
				return matchmakingBehaviour != null && matchmakingBehaviour.SupportsRoomPassword;
			}
		}

		[HideInInspector]
		[Tooltip("Event called when a CreateRoom operation finished")]
		public UnityEvent<CustomMatchmaking.RoomOperationResult> onRoomCreationFinished;

		[HideInInspector]
		[Tooltip("Event called when a JoinRoom operation finished")]
		public UnityEvent<CustomMatchmaking.RoomOperationResult> onRoomJoinFinished;

		[HideInInspector]
		[Tooltip("Event called when a LeaveRoom operation finished")]
		public UnityEvent onRoomLeaveFinished;

		[SerializeField]
		[HideInInspector]
		[Tooltip("Name of the game lobby the created room belongs to.")]
		private string lobbyName = "myLobby";

		[SerializeField]
		[HideInInspector]
		[Tooltip("Indicates whether this game room is private.")]
		private bool isPrivate;

		[SerializeField]
		[HideInInspector]
		[Tooltip("The maximum number of players allowed in this game room.")]
		private int maxPlayersPerRoom = 4;

		[SerializeField]
		[HideInInspector]
		[Tooltip("Indicates whether a password should be required for other players to be able to join this game room.")]
		private bool isPasswordProtected;

		protected CustomMatchmaking.ICustomMatchmakingBehaviour MatchmakingBehaviour;

		private const string DebugCategory = "Custom Matchmaking";

		public interface ICustomMatchmakingBehaviour
		{
			Task<CustomMatchmaking.RoomOperationResult> CreateRoom(CustomMatchmaking.RoomCreationOptions options);

			Task<CustomMatchmaking.RoomOperationResult> JoinRoom(string roomToken, string roomPassword = null);

			Task<CustomMatchmaking.RoomOperationResult> JoinOpenRoom(string lobbyName);

			void LeaveRoom();

			bool IsConnected { get; }

			string ConnectedRoomToken { get; }

			bool SupportsRoomPassword { get; }
		}

		public struct RoomCreationOptions
		{
			public string RoomPassword;

			public int MaxPlayersPerRoom;

			public bool IsPrivate;

			public string LobbyName;
		}

		public struct RoomOperationResult
		{
			public bool IsSuccess
			{
				get
				{
					return string.IsNullOrEmpty(this.ErrorMessage);
				}
			}

			public string ErrorMessage;

			public string RoomToken;

			public string RoomPassword;
		}
	}
}
