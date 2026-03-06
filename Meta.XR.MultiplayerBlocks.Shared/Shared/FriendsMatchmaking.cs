using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Meta.XR.ImmersiveDebugger;
using Oculus.Platform;
using Oculus.Platform.Models;
using UnityEngine;
using UnityEngine.Events;

namespace Meta.XR.MultiplayerBlocks.Shared
{
	public class FriendsMatchmaking : MonoBehaviour
	{
		public string DestinationApi
		{
			get
			{
				return this.destinationApi;
			}
			set
			{
				this.destinationApi = value;
			}
		}

		public string InviteMessage
		{
			get
			{
				return this.inviteMessage;
			}
			set
			{
				this.inviteMessage = value;
			}
		}

		public uint MaxRetries
		{
			get
			{
				return this.maxRetries;
			}
			set
			{
				this.maxRetries = value;
			}
		}

		private void Awake()
		{
			this._customMatchmaking = Object.FindObjectOfType<CustomMatchmaking>();
			if (this._customMatchmaking == null)
			{
				throw new InvalidOperationException("FriendsMatchmaking] No CustomMatchmaking component was found in the scene as is a requirement of FriendsMatchmaking");
			}
			PlatformInit.GetEntitlementInformation(new Action<PlatformInfo>(this.OnEntitlementFinished));
		}

		private void OnEnable()
		{
			this._customMatchmaking.onRoomLeaveFinished.AddListener(new UnityAction(this.ClearGroupPresenceCallback));
			this._customMatchmaking.onRoomCreationFinished.AddListener(new UnityAction<CustomMatchmaking.RoomOperationResult>(this.OnRoomOperationResult));
			this._customMatchmaking.onRoomJoinFinished.AddListener(new UnityAction<CustomMatchmaking.RoomOperationResult>(this.OnRoomOperationResult));
		}

		private void OnDisable()
		{
			this._customMatchmaking.onRoomLeaveFinished.RemoveListener(new UnityAction(this.ClearGroupPresenceCallback));
			this._customMatchmaking.onRoomCreationFinished.RemoveListener(new UnityAction<CustomMatchmaking.RoomOperationResult>(this.OnRoomOperationResult));
			this._customMatchmaking.onRoomJoinFinished.RemoveListener(new UnityAction<CustomMatchmaking.RoomOperationResult>(this.OnRoomOperationResult));
		}

		[DebugMember(DebugColor.Gray, Category = "Friends Matchmaking")]
		public void LaunchFriendsInvitePanel()
		{
			this.LaunchFriendsInvitePanelAsync(null);
		}

		public Task<Message<InvitePanelResultInfo>> LaunchFriendsInvitePanelAsync(InviteOptions inviteOptions = null)
		{
			TaskCompletionSource<Message<InvitePanelResultInfo>> tcs = new TaskCompletionSource<Message<InvitePanelResultInfo>>();
			GroupPresence.LaunchInvitePanel(inviteOptions ?? new InviteOptions()).OnComplete(delegate(Message<InvitePanelResultInfo> message)
			{
				if (message.IsError)
				{
					Debug.LogError("[FriendsMatchmaking] LaunchFriendsInvitePanelAsync failed: " + message.GetError().Message);
				}
				tcs.SetResult(message);
			});
			return tcs.Task;
		}

		[DebugMember(DebugColor.Gray, Category = "Friends Matchmaking")]
		public void LaunchRosterPanel()
		{
			this.LaunchRosterPanelAsync(null);
		}

		public Task<Message> LaunchRosterPanelAsync(RosterOptions rosterOptions = null)
		{
			TaskCompletionSource<Message> tcs = new TaskCompletionSource<Message>();
			GroupPresence.LaunchRosterPanel(rosterOptions ?? new RosterOptions()).OnComplete(delegate(Message message)
			{
				if (message.IsError)
				{
					Debug.LogError("[FriendsMatchmaking] LaunchRosterPanelAsync failed: " + message.GetError().Message);
				}
				tcs.SetResult(message);
			});
			return tcs.Task;
		}

		protected virtual void OnRoomOperationResult(CustomMatchmaking.RoomOperationResult result)
		{
			FriendsMatchmaking.<OnRoomOperationResult>d__24 <OnRoomOperationResult>d__;
			<OnRoomOperationResult>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<OnRoomOperationResult>d__.<>4__this = this;
			<OnRoomOperationResult>d__.result = result;
			<OnRoomOperationResult>d__.<>1__state = -1;
			<OnRoomOperationResult>d__.<>t__builder.Start<FriendsMatchmaking.<OnRoomOperationResult>d__24>(ref <OnRoomOperationResult>d__);
		}

		protected virtual Task JoinRoom(string roomId, string roomPassword)
		{
			FriendsMatchmaking.<JoinRoom>d__25 <JoinRoom>d__;
			<JoinRoom>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<JoinRoom>d__.<>4__this = this;
			<JoinRoom>d__.roomId = roomId;
			<JoinRoom>d__.roomPassword = roomPassword;
			<JoinRoom>d__.<>1__state = -1;
			<JoinRoom>d__.<>t__builder.Start<FriendsMatchmaking.<JoinRoom>d__25>(ref <JoinRoom>d__);
			return <JoinRoom>d__.<>t__builder.Task;
		}

		protected virtual void ClearGroupPresenceCallback()
		{
			FriendsMatchmaking.ClearGroupPresence();
		}

		private Task RegisterGameRoom(string roomId, string roomPassword = null)
		{
			FriendsMatchmaking.<RegisterGameRoom>d__27 <RegisterGameRoom>d__;
			<RegisterGameRoom>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<RegisterGameRoom>d__.<>4__this = this;
			<RegisterGameRoom>d__.roomId = roomId;
			<RegisterGameRoom>d__.roomPassword = roomPassword;
			<RegisterGameRoom>d__.<>1__state = -1;
			<RegisterGameRoom>d__.<>t__builder.Start<FriendsMatchmaking.<RegisterGameRoom>d__27>(ref <RegisterGameRoom>d__);
			return <RegisterGameRoom>d__.<>t__builder.Task;
		}

		private static Task<Message> ClearGroupPresence()
		{
			TaskCompletionSource<Message> tcs = new TaskCompletionSource<Message>();
			GroupPresence.Clear().OnComplete(delegate(Message message)
			{
				if (message.IsError)
				{
					Debug.LogError("[FriendsMatchmaking] ClearGroupPresence failed: " + message.GetError().Message);
				}
				tcs.SetResult(message);
			});
			return tcs.Task;
		}

		private static Task<Message> SetGroupPresence(GroupPresenceOptions groupPresenceOptions)
		{
			TaskCompletionSource<Message> tcs = new TaskCompletionSource<Message>();
			GroupPresence.Set(groupPresenceOptions).OnComplete(delegate(Message message)
			{
				tcs.SetResult(message);
			});
			return tcs.Task;
		}

		private void OnEntitlementFinished(PlatformInfo info)
		{
			GroupPresence.SetJoinIntentReceivedNotificationCallback(new Message<GroupPresenceJoinIntent>.Callback(this.OnJoinIntentReceived));
			GroupPresence.SetInvitationsSentNotificationCallback(new Message<LaunchInvitePanelFlowResult>.Callback(this.OnInvitationsSent));
			GroupPresence.SetLeaveIntentReceivedNotificationCallback(new Message<GroupPresenceLeaveIntent>.Callback(this.OnLeaveIntentNotification));
		}

		protected virtual void OnJoinIntentReceived(Message<GroupPresenceJoinIntent> message)
		{
			FriendsMatchmaking.<OnJoinIntentReceived>d__31 <OnJoinIntentReceived>d__;
			<OnJoinIntentReceived>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<OnJoinIntentReceived>d__.<>4__this = this;
			<OnJoinIntentReceived>d__.message = message;
			<OnJoinIntentReceived>d__.<>1__state = -1;
			<OnJoinIntentReceived>d__.<>t__builder.Start<FriendsMatchmaking.<OnJoinIntentReceived>d__31>(ref <OnJoinIntentReceived>d__);
		}

		private void OnInvitationsSent(Message<LaunchInvitePanelFlowResult> message)
		{
			UnityEvent<Message<LaunchInvitePanelFlowResult>> unityEvent = this.onInvitationsSent;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(message);
		}

		private void OnLeaveIntentNotification(Message<GroupPresenceLeaveIntent> message)
		{
			UnityEvent<Message<GroupPresenceLeaveIntent>> unityEvent = this.onLeaveIntentReceived;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(message);
		}

		protected virtual GroupPresenceOptions GetGroupPresenceOptions(string roomId, string roomPassword = null)
		{
			GroupPresenceOptions groupPresenceOptions = new GroupPresenceOptions();
			groupPresenceOptions.SetIsJoinable(true);
			groupPresenceOptions.SetDestinationApiName(this.DestinationApi);
			string text = CustomMatchmakingUtils.EncodeMatchInfoToSessionId(roomId, roomPassword);
			groupPresenceOptions.SetLobbySessionId(text);
			groupPresenceOptions.SetMatchSessionId(text);
			if (!string.IsNullOrEmpty(this.InviteMessage))
			{
				groupPresenceOptions.SetDeeplinkMessageOverride(this.InviteMessage);
			}
			return groupPresenceOptions;
		}

		[SerializeField]
		[Tooltip("Destination's API name obtained from developer.oculus.com under Engagement > Destinations.")]
		private string destinationApi = "destinationApi";

		[SerializeField]
		[Tooltip("Optional message to be sent when inviting friends to join a game room.")]
		private string inviteMessage = "Let's play together!";

		[SerializeField]
		[Tooltip("Maximum number of retries should a Platform SDK request fail.")]
		private uint maxRetries = 3U;

		[SerializeField]
		private UnityEvent<CustomMatchmaking.RoomOperationResult> onMatchRequestFound;

		[SerializeField]
		private UnityEvent<Message<LaunchInvitePanelFlowResult>> onInvitationsSent;

		[SerializeField]
		private UnityEvent<Message<GroupPresenceLeaveIntent>> onLeaveIntentReceived;

		private CustomMatchmaking _customMatchmaking;

		private const string DebugCategory = "Friends Matchmaking";
	}
}
