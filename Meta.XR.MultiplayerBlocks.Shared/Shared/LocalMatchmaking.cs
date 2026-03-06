using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Meta.XR.MultiplayerBlocks.Shared
{
	public class LocalMatchmaking : MonoBehaviour
	{
		private void Awake()
		{
			this._customMatchmaking = Object.FindObjectOfType<CustomMatchmaking>();
			if (this._customMatchmaking == null)
			{
				throw new InvalidOperationException("LocalMatchmaking] No CustomMatchmaking component was found in the scene");
			}
		}

		private void OnEnable()
		{
			if (this._customMatchmaking == null)
			{
				return;
			}
			this._customMatchmaking.onRoomCreationFinished.AddListener(new UnityAction<CustomMatchmaking.RoomOperationResult>(this.OnRoomCreationFinished));
		}

		private void OnDisable()
		{
			if (this._customMatchmaking == null)
			{
				return;
			}
			this._customMatchmaking.onRoomCreationFinished.RemoveListener(new UnityAction<CustomMatchmaking.RoomOperationResult>(this.OnRoomCreationFinished));
		}

		private void Start()
		{
			if (this.automaticHostOrJoin)
			{
				this.HostOrJoinSessionAutomatically();
			}
		}

		public Task StartAsHost()
		{
			LocalMatchmaking.<StartAsHost>d__14 <StartAsHost>d__;
			<StartAsHost>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<StartAsHost>d__.<>4__this = this;
			<StartAsHost>d__.<>1__state = -1;
			<StartAsHost>d__.<>t__builder.Start<LocalMatchmaking.<StartAsHost>d__14>(ref <StartAsHost>d__);
			return <StartAsHost>d__.<>t__builder.Task;
		}

		public Task StartAsGuest(bool stopAfterTimeout = true)
		{
			LocalMatchmaking.<StartAsGuest>d__15 <StartAsGuest>d__;
			<StartAsGuest>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<StartAsGuest>d__.<>4__this = this;
			<StartAsGuest>d__.stopAfterTimeout = stopAfterTimeout;
			<StartAsGuest>d__.<>1__state = -1;
			<StartAsGuest>d__.<>t__builder.Start<LocalMatchmaking.<StartAsGuest>d__15>(ref <StartAsGuest>d__);
			return <StartAsGuest>d__.<>t__builder.Task;
		}

		private void HostOrJoinSessionAutomatically()
		{
			LocalMatchmaking.<HostOrJoinSessionAutomatically>d__16 <HostOrJoinSessionAutomatically>d__;
			<HostOrJoinSessionAutomatically>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<HostOrJoinSessionAutomatically>d__.<>4__this = this;
			<HostOrJoinSessionAutomatically>d__.<>1__state = -1;
			<HostOrJoinSessionAutomatically>d__.<>t__builder.Start<LocalMatchmaking.<HostOrJoinSessionAutomatically>d__16>(ref <HostOrJoinSessionAutomatically>d__);
		}

		private void OnRoomCreationFinished(CustomMatchmaking.RoomOperationResult result)
		{
			if (result.IsSuccess)
			{
				LocalMatchmaking.StartAdvertisingColocationSession(Encoding.UTF8.GetBytes(CustomMatchmakingUtils.EncodeMatchInfoWithStruct(result.RoomToken, result.RoomPassword, LocalMatchmaking.ExtraData)));
			}
		}

		private void OnColocationSessionFound(OVRColocationSession.Data data)
		{
			LocalMatchmaking.<OnColocationSessionFound>d__18 <OnColocationSessionFound>d__;
			<OnColocationSessionFound>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<OnColocationSessionFound>d__.<>4__this = this;
			<OnColocationSessionFound>d__.data = data;
			<OnColocationSessionFound>d__.<>1__state = -1;
			<OnColocationSessionFound>d__.<>t__builder.Start<LocalMatchmaking.<OnColocationSessionFound>d__18>(ref <OnColocationSessionFound>d__);
		}

		public static void StartAdvertisingColocationSession(byte[] data)
		{
			LocalMatchmaking.<StartAdvertisingColocationSession>d__19 <StartAdvertisingColocationSession>d__;
			<StartAdvertisingColocationSession>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<StartAdvertisingColocationSession>d__.data = data;
			<StartAdvertisingColocationSession>d__.<>1__state = -1;
			<StartAdvertisingColocationSession>d__.<>t__builder.Start<LocalMatchmaking.<StartAdvertisingColocationSession>d__19>(ref <StartAdvertisingColocationSession>d__);
		}

		public static void StopAdvertisingColocationSession()
		{
			LocalMatchmaking.<StopAdvertisingColocationSession>d__20 <StopAdvertisingColocationSession>d__;
			<StopAdvertisingColocationSession>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<StopAdvertisingColocationSession>d__.<>1__state = -1;
			<StopAdvertisingColocationSession>d__.<>t__builder.Start<LocalMatchmaking.<StopAdvertisingColocationSession>d__20>(ref <StopAdvertisingColocationSession>d__);
		}

		public static void StartDiscoveringColocationSessions(Action<OVRColocationSession.Data> onGroupFound)
		{
			LocalMatchmaking.<StartDiscoveringColocationSessions>d__21 <StartDiscoveringColocationSessions>d__;
			<StartDiscoveringColocationSessions>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<StartDiscoveringColocationSessions>d__.onGroupFound = onGroupFound;
			<StartDiscoveringColocationSessions>d__.<>1__state = -1;
			<StartDiscoveringColocationSessions>d__.<>t__builder.Start<LocalMatchmaking.<StartDiscoveringColocationSessions>d__21>(ref <StartDiscoveringColocationSessions>d__);
		}

		public static void StopDiscoveringColocationSessions(Action<OVRColocationSession.Data> onGroupFound)
		{
			LocalMatchmaking.<StopDiscoveringColocationSessions>d__22 <StopDiscoveringColocationSessions>d__;
			<StopDiscoveringColocationSessions>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<StopDiscoveringColocationSessions>d__.onGroupFound = onGroupFound;
			<StopDiscoveringColocationSessions>d__.<>1__state = -1;
			<StopDiscoveringColocationSessions>d__.<>t__builder.Start<LocalMatchmaking.<StopDiscoveringColocationSessions>d__22>(ref <StopDiscoveringColocationSessions>d__);
		}

		private static void ReportDiscoverEvent(OVRColocationSession.Data data)
		{
			UnityEvent<Guid> onSessionDiscoverSucceeded = LocalMatchmaking.OnSessionDiscoverSucceeded;
			if (onSessionDiscoverSucceeded == null)
			{
				return;
			}
			onSessionDiscoverSucceeded.Invoke(data.AdvertisementUuid);
		}

		[Tooltip("On Start(), players will automatically discover local sessions and start hosting if no sessions found.")]
		[SerializeField]
		private bool automaticHostOrJoin = true;

		[Tooltip("Seconds to wait for discovering local sessions, if not found then creating their own session")]
		[SerializeField]
		private int timeDiscoveringInSec = 5;

		public static readonly UnityEvent<Guid> OnSessionCreateSucceeded = new UnityEvent<Guid>();

		public static readonly UnityEvent<string> OnSessionCreateFailed = new UnityEvent<string>();

		public static readonly UnityEvent<Guid> OnSessionDiscoverSucceeded = new UnityEvent<Guid>();

		public static readonly UnityEvent<string> OnSessionDiscoverFailed = new UnityEvent<string>();

		internal static Func<Task<bool>> BeforeStartHost;

		internal static string ExtraData = null;

		private CustomMatchmaking _customMatchmaking;

		private bool _discoveredLocalSessionAsGuest;
	}
}
