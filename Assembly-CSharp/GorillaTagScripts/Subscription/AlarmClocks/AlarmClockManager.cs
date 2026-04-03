using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GorillaLocomotion;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Subscription.AlarmClocks
{
	public sealed class AlarmClockManager : MonoBehaviour
	{
		public static AlarmClockManager Instance { get; private set; }

		public string ActiveKey { get; private set; } = "";

		private void Awake()
		{
			if (AlarmClockManager.Instance != null)
			{
				Debug.LogError("Duplicate instance of singleton class AlarmClockManager.");
				Object.Destroy(this);
				return;
			}
			if (this._defaultSpawn == null)
			{
				Debug.LogError("No default spawn set in AlarmClockManager.");
				Object.Destroy(this);
				return;
			}
			AlarmClockManager.Instance = this;
			this.ActiveKey = PlayerPrefs.GetString("AlarmClock");
			if (!string.IsNullOrEmpty(this.ActiveKey))
			{
				base.StartCoroutine(this.ConnectToPlayerSpawned());
			}
		}

		private IEnumerator ConnectToPlayerSpawned()
		{
			while (GorillaTagger.Instance == null || GorillaTagger.Instance.mainCamera == null)
			{
				yield return null;
			}
			PrivateUIRoom.ForceStartOverlay(this._loadingMessage);
			yield return new WaitForSeconds(1f);
			GorillaTagger.OnPlayerSpawned(new Action(this.OnPlayerSpawned));
			yield break;
		}

		public static void ToggleAlarmClock(AlarmClock clock)
		{
			AlarmClockManager.Instance.ToggleAlarmClockInternal(clock);
		}

		private void ToggleAlarmClockInternal(AlarmClock clock)
		{
			if (this.ActiveKey == clock.Key)
			{
				AlarmClock activeClock = this._activeClock;
				if (activeClock != null)
				{
					UnityEvent onDeactivate = activeClock.OnDeactivate;
					if (onDeactivate != null)
					{
						onDeactivate.Invoke();
					}
				}
				this._activeClock = null;
				this.ActiveKey = "";
			}
			else
			{
				AlarmClock activeClock2 = this._activeClock;
				if (activeClock2 != null)
				{
					UnityEvent onDeactivate2 = activeClock2.OnDeactivate;
					if (onDeactivate2 != null)
					{
						onDeactivate2.Invoke();
					}
				}
				this._activeClock = clock;
				AlarmClock activeClock3 = this._activeClock;
				if (activeClock3 != null)
				{
					UnityEvent onActivate = activeClock3.OnActivate;
					if (onActivate != null)
					{
						onActivate.Invoke();
					}
				}
				this.ActiveKey = clock.Key;
			}
			PlayerPrefs.SetString("AlarmClock", this.ActiveKey);
			Debug.Log("Alarm clock data set to \"" + this.ActiveKey + "\".");
		}

		private void OnDestroy()
		{
			if (AlarmClockManager.Instance == this)
			{
				AlarmClockManager.Instance = null;
			}
		}

		[UsedImplicitly]
		private bool AllUniqueClockKeys()
		{
			HashSet<string> hashSet = new HashSet<string>();
			foreach (AlarmClockManager.AlarmClockData alarmClockData in this._clockData)
			{
				if (!hashSet.Add(alarmClockData.Key))
				{
					return false;
				}
			}
			return true;
		}

		private void OnPlayerSpawned()
		{
			Debug.Log("Loaded alarm clock value " + this.ActiveKey);
			if (!string.IsNullOrEmpty(this.ActiveKey))
			{
				AlarmClockManager.AlarmClockData alarmClockData = this._clockData.FirstOrDefault((AlarmClockManager.AlarmClockData c) => c.Key == this.ActiveKey);
				if (alarmClockData != null)
				{
					this._activeClockData = alarmClockData;
					this._teleportTarget = alarmClockData.SpawnPoint;
					PrivateUIRoom.ForceStartOverlay(this._loadingMessage);
					ZoneManagement.SetActiveZones(alarmClockData.Zones);
					base.StartCoroutine(this.OnZoneLoaded());
					base.StartCoroutine(this.ClearUnsubPlayerData());
				}
			}
		}

		private IEnumerator ClearUnsubPlayerData()
		{
			while (!MothershipClientApiUnity.IsClientLoggedIn())
			{
				yield return null;
			}
			if (SubscriptionManager.LocalSubscriptionStatus() == SubscriptionManager.SubscriptionStatus.Active)
			{
				yield break;
			}
			PlayerPrefs.SetString("AlarmClock", "");
			GTPlayer.Instance.TeleportTo(this._defaultSpawn.position, this._defaultSpawn.rotation);
			yield break;
		}

		private IEnumerator OnZoneLoaded()
		{
			yield return null;
			if (this._activeClockData.Zones.Length <= 1)
			{
				if (this._activeClockData.Zones[0] == GTZone.forest)
				{
					goto IL_84;
				}
			}
			while (ZoneManagement.instance.ZonesSetting)
			{
				yield return null;
			}
			IL_84:
			if (GTPlayer.hasInstance)
			{
				GTPlayer.Instance.TeleportTo(this._teleportTarget.position, this._teleportTarget.rotation);
			}
			PrivateUIRoom.StopForcedOverlay();
			UnityEvent onWakeUp = this.OnWakeUp;
			if (onWakeUp != null)
			{
				onWakeUp.Invoke();
			}
			yield break;
		}

		public const string SaveDataKey = "AlarmClock";

		[SerializeField]
		private string _loadingMessage = "";

		[SerializeField]
		private AlarmClockManager.AlarmClockData[] _clockData = new AlarmClockManager.AlarmClockData[0];

		[SerializeField]
		private Transform _defaultSpawn;

		public UnityEvent OnWakeUp;

		private Transform _teleportTarget;

		private AlarmClockManager.AlarmClockData _activeClockData;

		[CanBeNull]
		private AlarmClock _activeClock;

		[Serializable]
		public class AlarmClockData
		{
			public string Key;

			public GTZone[] Zones;

			public Transform SpawnPoint;
		}
	}
}
