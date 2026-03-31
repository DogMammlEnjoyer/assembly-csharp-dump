using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using AOT;
using Liv.Lck.Core.Serialization;
using UnityEngine;
using UnityEngine.Scripting;

namespace Liv.Lck.Core.Cosmetics
{
	[Preserve]
	public class LckCoreCosmeticsCoordinator : ILckCosmeticsCoordinator
	{
		private static LckCoreCosmeticsCoordinator Instance { get; set; }

		public event Action<LckAvailableCosmeticInfo> OnCosmeticAvailable;

		[Preserve]
		public LckCoreCosmeticsCoordinator(ILckSerializer serializer, ILckCosmeticsFeatureFlagManager featureFlagManager)
		{
			LckCoreCosmeticsCoordinator.Instance = this;
			this._serializer = serializer;
			this._featureFlagManager = featureFlagManager;
			this.InitializeLocalCosmeticsAsync();
		}

		public Task InitializeLocalCosmeticsAsync()
		{
			object @lock = this._lock;
			Task requestLocalUserCosmeticsTask;
			lock (@lock)
			{
				if (this._requestLocalUserCosmeticsTask == null || this._requestLocalUserCosmeticsTask.IsCompleted)
				{
					this._requestLocalUserCosmeticsTask = this.RequestLocalUserCosmeticsAsyncDelayed();
				}
				requestLocalUserCosmeticsTask = this._requestLocalUserCosmeticsTask;
			}
			return requestLocalUserCosmeticsTask;
		}

		private Task RequestLocalUserCosmeticsAsyncDelayed()
		{
			LckCoreCosmeticsCoordinator.<RequestLocalUserCosmeticsAsyncDelayed>d__16 <RequestLocalUserCosmeticsAsyncDelayed>d__;
			<RequestLocalUserCosmeticsAsyncDelayed>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<RequestLocalUserCosmeticsAsyncDelayed>d__.<>4__this = this;
			<RequestLocalUserCosmeticsAsyncDelayed>d__.<>1__state = -1;
			<RequestLocalUserCosmeticsAsyncDelayed>d__.<>t__builder.Start<LckCoreCosmeticsCoordinator.<RequestLocalUserCosmeticsAsyncDelayed>d__16>(ref <RequestLocalUserCosmeticsAsyncDelayed>d__);
			return <RequestLocalUserCosmeticsAsyncDelayed>d__.<>t__builder.Task;
		}

		private Task<Result<bool>> GetLocalUserCosmeticsAsync()
		{
			LckCoreCosmeticsCoordinator.<GetLocalUserCosmeticsAsync>d__17 <GetLocalUserCosmeticsAsync>d__;
			<GetLocalUserCosmeticsAsync>d__.<>t__builder = AsyncTaskMethodBuilder<Result<bool>>.Create();
			<GetLocalUserCosmeticsAsync>d__.<>4__this = this;
			<GetLocalUserCosmeticsAsync>d__.<>1__state = -1;
			<GetLocalUserCosmeticsAsync>d__.<>t__builder.Start<LckCoreCosmeticsCoordinator.<GetLocalUserCosmeticsAsync>d__17>(ref <GetLocalUserCosmeticsAsync>d__);
			return <GetLocalUserCosmeticsAsync>d__.<>t__builder.Task;
		}

		public Task<Result<bool>> GetUserCosmeticsForSessionAsync(IEnumerable<string> playerIds, string sessionId)
		{
			LckCoreCosmeticsCoordinator.<GetUserCosmeticsForSessionAsync>d__18 <GetUserCosmeticsForSessionAsync>d__;
			<GetUserCosmeticsForSessionAsync>d__.<>t__builder = AsyncTaskMethodBuilder<Result<bool>>.Create();
			<GetUserCosmeticsForSessionAsync>d__.<>4__this = this;
			<GetUserCosmeticsForSessionAsync>d__.playerIds = playerIds;
			<GetUserCosmeticsForSessionAsync>d__.sessionId = sessionId;
			<GetUserCosmeticsForSessionAsync>d__.<>1__state = -1;
			<GetUserCosmeticsForSessionAsync>d__.<>t__builder.Start<LckCoreCosmeticsCoordinator.<GetUserCosmeticsForSessionAsync>d__18>(ref <GetUserCosmeticsForSessionAsync>d__);
			return <GetUserCosmeticsForSessionAsync>d__.<>t__builder.Task;
		}

		public Task<Result<bool>> AnnouncePlayerPresenceForSessionAsync(string playerId, string sessionId)
		{
			LckCoreCosmeticsCoordinator.<AnnouncePlayerPresenceForSessionAsync>d__19 <AnnouncePlayerPresenceForSessionAsync>d__;
			<AnnouncePlayerPresenceForSessionAsync>d__.<>t__builder = AsyncTaskMethodBuilder<Result<bool>>.Create();
			<AnnouncePlayerPresenceForSessionAsync>d__.<>4__this = this;
			<AnnouncePlayerPresenceForSessionAsync>d__.playerId = playerId;
			<AnnouncePlayerPresenceForSessionAsync>d__.sessionId = sessionId;
			<AnnouncePlayerPresenceForSessionAsync>d__.<>1__state = -1;
			<AnnouncePlayerPresenceForSessionAsync>d__.<>t__builder.Start<LckCoreCosmeticsCoordinator.<AnnouncePlayerPresenceForSessionAsync>d__19>(ref <AnnouncePlayerPresenceForSessionAsync>d__);
			return <AnnouncePlayerPresenceForSessionAsync>d__.<>t__builder.Task;
		}

		[MonoPInvokeCallback(typeof(LckCoreCosmeticsNative.get_user_cosmetics_for_session_on_cosmetic_available_delegate))]
		private static void OnCosmeticAvailableStatic(IntPtr serializedCosmeticDataPtr, UIntPtr serializedDataLength, SerializationType serializationType)
		{
			if (LckCoreCosmeticsCoordinator.Instance == null)
			{
				Debug.LogError("Cosmetic became available while LckCoreCosmeticsCoordinator is uninitialized");
				return;
			}
			LckCoreCosmeticsCoordinator.Instance.HandleOnCosmeticAvailable(serializedCosmeticDataPtr, serializedDataLength, serializationType);
		}

		private void HandleOnCosmeticAvailable(IntPtr serializedCosmeticDataPtr, UIntPtr serializedDataLength, SerializationType serializationType)
		{
			if (this._serializer.SerializationType != serializationType)
			{
				Debug.LogError(string.Format("Received cosmetic data in unexpected serialization format: {0} ", serializationType) + string.Format("(expected {0})", this._serializer.SerializationType));
				return;
			}
			byte[] data = InteropUtilities.CopyUnmanagedByteArray(serializedCosmeticDataPtr, (int)((uint)serializedDataLength));
			LckAvailableCosmeticInfo lckAvailableCosmeticInfo = this._serializer.Deserialize<LckAvailableCosmeticInfo>(data);
			string text = "Cosmetic available at " + lckAvailableCosmeticInfo.CosmeticInfo.CosmeticFilepath + " for players:\n";
			text = lckAvailableCosmeticInfo.PlayerIds.Aggregate(text, (string current, string playerId) => current + "  - " + playerId);
			Debug.Log(text);
			Action<LckAvailableCosmeticInfo> onCosmeticAvailable = this.OnCosmeticAvailable;
			if (onCosmeticAvailable == null)
			{
				return;
			}
			onCosmeticAvailable(lckAvailableCosmeticInfo);
		}

		[MonoPInvokeCallback(typeof(LckCoreCosmeticsNative.announce_player_presence_for_session_on_presence_expiry_received_delegate))]
		private static void OnPresenceAnnouncementExpiryReceivedStatic(ulong timeUntilExpirationSeconds)
		{
			if (LckCoreCosmeticsCoordinator.Instance == null)
			{
				Debug.LogError("Player presence was announced while LckCoreCosmeticsCoordinator is uninitialized");
				return;
			}
			LckCoreCosmeticsCoordinator.Instance.HandlePresenceAnnouncement(timeUntilExpirationSeconds);
		}

		private void HandlePresenceAnnouncement(ulong expirationTimeSeconds)
		{
			CancellationTokenSource reannounceCancellationTokenSource = this._reannounceCancellationTokenSource;
			if (reannounceCancellationTokenSource != null)
			{
				reannounceCancellationTokenSource.Cancel();
			}
			this._reannounceCancellationTokenSource = new CancellationTokenSource();
			TimeSpan reannounceDelay = TimeSpan.FromSeconds(expirationTimeSeconds * 0.9);
			this.ReannouncePresenceAfterDelay(reannounceDelay, this._reannounceCancellationTokenSource.Token);
		}

		private Task ReannouncePresenceAfterDelay(TimeSpan reannounceDelay, CancellationToken cancellationToken)
		{
			LckCoreCosmeticsCoordinator.<ReannouncePresenceAfterDelay>d__24 <ReannouncePresenceAfterDelay>d__;
			<ReannouncePresenceAfterDelay>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ReannouncePresenceAfterDelay>d__.<>4__this = this;
			<ReannouncePresenceAfterDelay>d__.reannounceDelay = reannounceDelay;
			<ReannouncePresenceAfterDelay>d__.cancellationToken = cancellationToken;
			<ReannouncePresenceAfterDelay>d__.<>1__state = -1;
			<ReannouncePresenceAfterDelay>d__.<>t__builder.Start<LckCoreCosmeticsCoordinator.<ReannouncePresenceAfterDelay>d__24>(ref <ReannouncePresenceAfterDelay>d__);
			return <ReannouncePresenceAfterDelay>d__.<>t__builder.Task;
		}

		private readonly ILckSerializer _serializer;

		private readonly ILckCosmeticsFeatureFlagManager _featureFlagManager;

		private CancellationTokenSource _reannounceCancellationTokenSource;

		private Task _requestLocalUserCosmeticsTask;

		private readonly object _lock = new object();

		private string _playerId;

		private string _sessionId;
	}
}
