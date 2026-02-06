using System;
using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace Photon.Pun.UtilityScripts
{
	public class CountdownTimer : MonoBehaviourPunCallbacks
	{
		public static event CountdownTimer.CountdownTimerHasExpired OnCountdownTimerHasExpired;

		public void Start()
		{
			if (this.Text == null)
			{
				Debug.LogError("Reference to 'Text' is not set. Please set a valid reference.", this);
			}
		}

		public override void OnEnable()
		{
			Debug.Log("OnEnable CountdownTimer");
			base.OnEnable();
			this.Initialize();
		}

		public override void OnDisable()
		{
			base.OnDisable();
			Debug.Log("OnDisable CountdownTimer");
		}

		public void Update()
		{
			if (!this.isTimerRunning)
			{
				return;
			}
			float num = this.TimeRemaining();
			this.Text.text = string.Format("Game starts in {0} seconds", num.ToString("n0"));
			if (num > 0f)
			{
				return;
			}
			this.OnTimerEnds();
		}

		private void OnTimerRuns()
		{
			this.isTimerRunning = true;
			base.enabled = true;
		}

		private void OnTimerEnds()
		{
			this.isTimerRunning = false;
			base.enabled = false;
			Debug.Log("Emptying info text.", this.Text);
			this.Text.text = string.Empty;
			if (CountdownTimer.OnCountdownTimerHasExpired != null)
			{
				CountdownTimer.OnCountdownTimerHasExpired();
			}
		}

		public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
		{
			Debug.Log("CountdownTimer.OnRoomPropertiesUpdate " + propertiesThatChanged.ToStringFull());
			this.Initialize();
		}

		private void Initialize()
		{
			int num;
			if (CountdownTimer.TryGetStartTime(out num))
			{
				this.startTime = num;
				Debug.Log(string.Concat(new string[]
				{
					"Initialize sets StartTime ",
					this.startTime.ToString(),
					" server time now: ",
					PhotonNetwork.ServerTimestamp.ToString(),
					" remain: ",
					this.TimeRemaining().ToString()
				}));
				this.isTimerRunning = (this.TimeRemaining() > 0f);
				if (this.isTimerRunning)
				{
					this.OnTimerRuns();
					return;
				}
				this.OnTimerEnds();
			}
		}

		private float TimeRemaining()
		{
			int num = PhotonNetwork.ServerTimestamp - this.startTime;
			return this.Countdown - (float)num / 1000f;
		}

		public static bool TryGetStartTime(out int startTimestamp)
		{
			startTimestamp = PhotonNetwork.ServerTimestamp;
			object obj;
			if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("StartTime", out obj))
			{
				startTimestamp = (int)obj;
				return true;
			}
			return false;
		}

		public static void SetStartTime()
		{
			int num = 0;
			bool flag = CountdownTimer.TryGetStartTime(out num);
			Hashtable hashtable = new Hashtable
			{
				{
					"StartTime",
					PhotonNetwork.ServerTimestamp
				}
			};
			PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable, null, null);
			Debug.Log("Set Custom Props for Time: " + hashtable.ToStringFull() + " wasSet: " + flag.ToString());
		}

		public const string CountdownStartTime = "StartTime";

		[Header("Countdown time in seconds")]
		public float Countdown = 5f;

		private bool isTimerRunning;

		private int startTime;

		[Header("Reference to a Text component for visualizing the countdown")]
		public Text Text;

		public delegate void CountdownTimerHasExpired();
	}
}
