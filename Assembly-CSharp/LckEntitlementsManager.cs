using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Liv.Lck;
using Liv.Lck.Core.Cosmetics;
using Liv.Lck.DependencyInjection;
using Photon.Pun;
using UnityEngine;

public class LckEntitlementsManager : MonoBehaviour
{
	public static bool LckEntitlementsEnabled { get; private set; }

	public static LckEntitlementsManager Instance { get; private set; }

	private void Awake()
	{
		if (LckEntitlementsManager.Instance != null && LckEntitlementsManager.Instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		LckEntitlementsManager.Instance = this;
	}

	private void OnEnable()
	{
		this.InitializeFeatureAsync();
		this._cleanupProcessedPlayersCoroutine = base.StartCoroutine(this.CleanupProcessedPlayersCoroutine());
		this._getEntitlementsBatchingCoroutine = base.StartCoroutine(this.ProcessBatchedRemotePlayersCoroutine());
	}

	private void OnDisable()
	{
		if (this._cleanupProcessedPlayersCoroutine != null)
		{
			base.StopCoroutine(this._cleanupProcessedPlayersCoroutine);
			this._cleanupProcessedPlayersCoroutine = null;
		}
		if (this._getEntitlementsBatchingCoroutine != null)
		{
			base.StopCoroutine(this._getEntitlementsBatchingCoroutine);
			this._getEntitlementsBatchingCoroutine = null;
		}
	}

	private Task InitializeFeatureAsync()
	{
		LckEntitlementsManager.<InitializeFeatureAsync>d__27 <InitializeFeatureAsync>d__;
		<InitializeFeatureAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<InitializeFeatureAsync>d__.<>4__this = this;
		<InitializeFeatureAsync>d__.<>1__state = -1;
		<InitializeFeatureAsync>d__.<>t__builder.Start<LckEntitlementsManager.<InitializeFeatureAsync>d__27>(ref <InitializeFeatureAsync>d__);
		return <InitializeFeatureAsync>d__.<>t__builder.Task;
	}

	public void OnLocalPlayerSpawned(string localUserId)
	{
		if (!this.ShouldProcessPlayer(localUserId))
		{
			return;
		}
		base.StartCoroutine(this.ProcessLocalPlayerSpawn(localUserId));
	}

	public void OnRemotePlayerSpawned(string remoteUserId)
	{
		if (this._currentState == LckEntitlementsManager.FeatureState.Disabled)
		{
			return;
		}
		if (!this.ShouldProcessPlayer(remoteUserId))
		{
			return;
		}
		HashSet<string> remotePlayersToGetEntitlementsFor = this._remotePlayersToGetEntitlementsFor;
		lock (remotePlayersToGetEntitlementsFor)
		{
			if (this._remotePlayersToGetEntitlementsFor.Add(remoteUserId))
			{
				Debug.Log("LCK: Queued remote player " + remoteUserId + " for batched entitlements check.");
			}
		}
	}

	private IEnumerator ProcessLocalPlayerSpawn(string userId)
	{
		yield return new WaitUntil(() => this._currentState > LckEntitlementsManager.FeatureState.Checking);
		if (this._currentState == LckEntitlementsManager.FeatureState.Disabled)
		{
			yield break;
		}
		base.StartCoroutine(this.AnnouncePlayerPresenceForSession(userId));
		yield break;
	}

	private bool ShouldProcessPlayer(string userId)
	{
		LckEntitlementsManager.PlayerProcessRecord playerProcessRecord;
		if (!this._processedPlayers.TryGetValue(userId, out playerProcessRecord))
		{
			playerProcessRecord = new LckEntitlementsManager.PlayerProcessRecord();
			this._processedPlayers[userId] = playerProcessRecord;
		}
		playerProcessRecord.LastSeenTimestamp = Time.time;
		if (Time.time < playerProcessRecord.TimeoutUntilTimestamp)
		{
			Debug.LogWarning("LCK: Player " + userId + " is on a timeout. Entitlements Manager will ignore spawn event.");
			return false;
		}
		if (playerProcessRecord.AttemptCount > 3)
		{
			playerProcessRecord.AttemptCount = 0;
		}
		playerProcessRecord.AttemptCount++;
		if (playerProcessRecord.AttemptCount > 3)
		{
			playerProcessRecord.TimeoutUntilTimestamp = Time.time + 60f;
			Debug.LogWarning(string.Format("LCK: Player {0} exceeded max attempts. Applying a {1}-minute timeout.", userId, 1f));
			return false;
		}
		Debug.Log(string.Format("LCK: Processing player {0} (Attempt {1}/{2}).", userId, playerProcessRecord.AttemptCount, 3));
		return true;
	}

	private IEnumerator ProcessBatchedRemotePlayersCoroutine()
	{
		for (;;)
		{
			yield return new WaitForSeconds(15f);
			if (!this._isProcessingBatch)
			{
				HashSet<string> remotePlayersToGetEntitlementsFor = this._remotePlayersToGetEntitlementsFor;
				List<string> list;
				lock (remotePlayersToGetEntitlementsFor)
				{
					if (this._remotePlayersToGetEntitlementsFor.Count == 0)
					{
						continue;
					}
					list = this._remotePlayersToGetEntitlementsFor.ToList<string>();
					this._remotePlayersToGetEntitlementsFor.Clear();
				}
				if (list.Count > 0)
				{
					this._isProcessingBatch = true;
					this.GetCosmeticsForPlayersAsync(list, "ProcessBatchedRemotePlayers");
				}
			}
		}
		yield break;
	}

	private IEnumerator AnnouncePlayerPresenceForSession(string localPlayerId)
	{
		if (PhotonNetwork.CurrentRoom == null)
		{
			Debug.LogError("LCK: Called AnnouncePlayerPresenceForSession() but no room was found. Player not announced.");
			yield break;
		}
		string sessionId = "DefaultSessionId";
		Debug.Log(string.Concat(new string[]
		{
			"LCK: Announcing Presence for local player with UserId: ",
			localPlayerId,
			" + Session ID: ",
			sessionId,
			"."
		}));
		int num;
		for (int attempt = 1; attempt <= 2; attempt = num + 1)
		{
			LckEntitlementsManager.<>c__DisplayClass33_0 CS$<>8__locals1 = new LckEntitlementsManager.<>c__DisplayClass33_0();
			CS$<>8__locals1.announcementAsync = this._lckCosmeticsCoordinator.AnnouncePlayerPresenceForSessionAsync(localPlayerId, sessionId);
			yield return new WaitUntil(() => CS$<>8__locals1.announcementAsync.IsCompleted);
			if (!CS$<>8__locals1.announcementAsync.IsFaulted && CS$<>8__locals1.announcementAsync.Result.IsOk)
			{
				Debug.Log("LCK: Successfully set session entitlement.");
				yield break;
			}
			string arg = CS$<>8__locals1.announcementAsync.IsFaulted ? CS$<>8__locals1.announcementAsync.Exception.ToString() : CS$<>8__locals1.announcementAsync.Result.Message.ToString();
			Debug.LogError(string.Format("LCK: Error setting session entitlement (Attempt {0}/{1}): {2}", attempt, 2, arg));
			CS$<>8__locals1 = null;
			num = attempt;
		}
		Debug.LogError("LCK: All attempts to set session entitlement failed.");
		yield break;
	}

	private Task GetCosmeticsForPlayersAsync(List<string> userIdList, string methodNameForLogging)
	{
		LckEntitlementsManager.<GetCosmeticsForPlayersAsync>d__34 <GetCosmeticsForPlayersAsync>d__;
		<GetCosmeticsForPlayersAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<GetCosmeticsForPlayersAsync>d__.<>4__this = this;
		<GetCosmeticsForPlayersAsync>d__.userIdList = userIdList;
		<GetCosmeticsForPlayersAsync>d__.methodNameForLogging = methodNameForLogging;
		<GetCosmeticsForPlayersAsync>d__.<>1__state = -1;
		<GetCosmeticsForPlayersAsync>d__.<>t__builder.Start<LckEntitlementsManager.<GetCosmeticsForPlayersAsync>d__34>(ref <GetCosmeticsForPlayersAsync>d__);
		return <GetCosmeticsForPlayersAsync>d__.<>t__builder.Task;
	}

	private IEnumerator CleanupProcessedPlayersCoroutine()
	{
		List<string> playersToRemove = new List<string>();
		for (;;)
		{
			yield return new WaitForSeconds(60f);
			playersToRemove.Clear();
			float time = Time.time;
			foreach (KeyValuePair<string, LckEntitlementsManager.PlayerProcessRecord> keyValuePair in this._processedPlayers)
			{
				if (time > keyValuePair.Value.LastSeenTimestamp + 300f)
				{
					playersToRemove.Add(keyValuePair.Key);
				}
			}
			if (playersToRemove.Count > 0)
			{
				Debug.Log(string.Format("LCK: Cleaning up {0} stale player records.", playersToRemove.Count));
				using (List<string>.Enumerator enumerator2 = playersToRemove.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						string key = enumerator2.Current;
						this._processedPlayers.Remove(key);
					}
					continue;
				}
				yield break;
			}
		}
	}

	[InjectLck]
	private ILckCosmeticsCoordinator _lckCosmeticsCoordinator;

	[InjectLck]
	private ILckCosmeticsFeatureFlagManager _featureFlagManager;

	private const int MAX_API_CALL_ATTEMPTS = 2;

	private const int MAX_CONSECUTIVE_ATTEMPTS = 3;

	private const float ABUSE_TIMEOUT_MINUTES = 1f;

	private const float BATCH_GET_ENTITLEMENTS_INTERVAL_SECONDS = 15f;

	private const float STALE_PLAYER_TIMEOUT_MINUTES = 5f;

	private const string DEFAULT_SESSION_ID = "DefaultSessionId";

	private LckEntitlementsManager.FeatureState _currentState;

	private readonly HashSet<string> _remotePlayersToGetEntitlementsFor = new HashSet<string>();

	private Coroutine _getEntitlementsBatchingCoroutine;

	private readonly Dictionary<string, LckEntitlementsManager.PlayerProcessRecord> _processedPlayers = new Dictionary<string, LckEntitlementsManager.PlayerProcessRecord>();

	private Coroutine _cleanupProcessedPlayersCoroutine;

	private bool _isProcessingBatch;

	private class PlayerProcessRecord
	{
		public int AttemptCount;

		public float TimeoutUntilTimestamp;

		public float LastSeenTimestamp;
	}

	private enum FeatureState
	{
		Checking,
		Enabled,
		Disabled
	}
}
