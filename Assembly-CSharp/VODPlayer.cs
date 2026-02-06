using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using GorillaNetworking;
using Newtonsoft.Json;
using PlayFab;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;

public class VODPlayer : MonoBehaviour, IGorillaSliceableSimple
{
	public void OnEnable()
	{
		VODPlayer.<OnEnable>d__20 <OnEnable>d__;
		<OnEnable>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<OnEnable>d__.<>4__this = this;
		<OnEnable>d__.<>1__state = -1;
		<OnEnable>d__.<>t__builder.Start<VODPlayer.<OnEnable>d__20>(ref <OnEnable>d__);
	}

	private void waitOnServerTime()
	{
		VODPlayer.<waitOnServerTime>d__21 <waitOnServerTime>d__;
		<waitOnServerTime>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<waitOnServerTime>d__.<>1__state = -1;
		<waitOnServerTime>d__.<>t__builder.Start<VODPlayer.<waitOnServerTime>d__21>(ref <waitOnServerTime>d__);
	}

	private void VODTarget_AlertEnabled(VODTarget o)
	{
		if (!this.targets.Contains(o))
		{
			this.targets.Add(o);
			o.gameObject.SetActive(VODPlayer.state != VODPlayer.State.CRASHED);
			if (VODPlayer.state == VODPlayer.State.RUNNING && this.player.isPlaying)
			{
				o.Renderer.material = this.playBackMaterial;
				return;
			}
			o.Renderer.material = ((o.StandbyOverride == null) ? this.standbyMaterial : o.StandbyOverride);
		}
	}

	private void VODTarget_AlertDisabled(VODTarget o)
	{
		if (this.targets.Contains(o))
		{
			this.targets.Remove(o);
			o.Renderer.material = ((o.StandbyOverride == null) ? this.disconnectedMaterial : o.StandbyOverride);
			if (o.UpNextText != null)
			{
				o.UpNextText.text = string.Empty;
			}
		}
	}

	private void Player_loopPointReached(VideoPlayer source)
	{
		if (!this.playerBusy)
		{
			this.player.Stop();
			for (int i = 0; i < this.targets.Count; i++)
			{
				this.targets[i].Renderer.material = ((this.targets[i].StandbyOverride == null) ? this.standbyMaterial : this.targets[i].StandbyOverride);
			}
			this.nextStream = this.NextStream();
		}
	}

	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		this.player.loopPointReached -= this.Player_loopPointReached;
		VODTarget.AlertEnabled = (Action<VODTarget>)Delegate.Remove(VODTarget.AlertEnabled, new Action<VODTarget>(this.VODTarget_AlertEnabled));
		VODTarget.AlertDisabled = (Action<VODTarget>)Delegate.Remove(VODTarget.AlertDisabled, new Action<VODTarget>(this.VODTarget_AlertDisabled));
	}

	private void OnDestroy()
	{
		VODTarget.AlertEnabled = (Action<VODTarget>)Delegate.Remove(VODTarget.AlertEnabled, new Action<VODTarget>(this.VODTarget_AlertEnabled));
		VODTarget.AlertDisabled = (Action<VODTarget>)Delegate.Remove(VODTarget.AlertDisabled, new Action<VODTarget>(this.VODTarget_AlertDisabled));
	}

	void IGorillaSliceableSimple.SliceUpdate()
	{
		switch (VODPlayer.state)
		{
		case VODPlayer.State.INITIALIZING:
		case VODPlayer.State.CRASHED:
			return;
		case VODPlayer.State.IDLE:
			if (this.targets.Count > 0)
			{
				this.nextStream = this.NextStream();
				this.PlayPreviouStream();
				VODPlayer.state = VODPlayer.State.RUNNING;
			}
			return;
		case VODPlayer.State.RUNNING:
		{
			if (this.targets.Count == 0)
			{
				if (!this.playerBusy)
				{
					this.player.Stop();
				}
				VODPlayer.state = VODPlayer.State.IDLE;
				return;
			}
			if (this.player.isPlaying)
			{
				this.PositionAudio();
			}
			if (this.imageClearTime > 0f && this.imageClearTime < Time.time)
			{
				for (int i = 0; i < this.targets.Count; i++)
				{
					this.targets[i].Renderer.material = this.standbyMaterial;
				}
				this.imageClearTime = 0f;
			}
			DateTime serverTime = GorillaComputer.instance.GetServerTime();
			DayOfWeek dayOfWeek = serverTime.DayOfWeek;
			int hour = serverTime.Hour;
			int minute = serverTime.Minute;
			if (this.imageClearTime == 0f && this.nextStream != null && !this.playerBusy && !this.player.isPlaying && this.nextStream.Title != string.Empty)
			{
				TimeSpan timeSpan = this.nextStream.StartTime - serverTime;
				if (timeSpan.TotalMinutes > 0.0 && timeSpan.TotalMinutes <= 60.0)
				{
					for (int j = 0; j < this.targets.Count; j++)
					{
						if (this.targets[j].UpNextText != null)
						{
							this.targets[j].UpNextText.text = string.Format("next: {0} - {1:00}:{2:00}", this.nextStream.Title, timeSpan.Minutes, timeSpan.Seconds);
						}
					}
				}
			}
			if (minute == this.lastCheck)
			{
				return;
			}
			this.lastCheck = minute;
			for (int k = 0; k < this.schedule.hourly.Length; k++)
			{
				if (this.schedule.hourly[k].minute - minute == 0 && this.schedule.hourly[k].IsDateInRange(serverTime))
				{
					this.StartPlayback(this.schedule.hourly[k].stream, 1.0);
					return;
				}
			}
			return;
		}
		default:
			return;
		}
	}

	private VODPlayer.VODNextStream NextStream()
	{
		DateTime serverTime = GorillaComputer.instance.GetServerTime();
		List<VODPlayer.VODNextStream> list = new List<VODPlayer.VODNextStream>();
		for (int i = 0; i < this.schedule.hourly.Length; i++)
		{
			DateTime dateTime = new DateTime(serverTime.Year, serverTime.Month, serverTime.Day, serverTime.Hour, this.schedule.hourly[i].minute, 0);
			list.Add(new VODPlayer.VODNextStream(this.schedule.hourly[i].stream.name, this.schedule.hourly[i].ClampedDateTime(dateTime), this.schedule.hourly[i].stream.url));
			list.Add(new VODPlayer.VODNextStream(this.schedule.hourly[i].stream.name, this.schedule.hourly[i].ClampedDateTime(dateTime.AddHours(1.0)), this.schedule.hourly[i].stream.url));
		}
		list.Sort();
		for (int j = 0; j < list.Count; j++)
		{
			if (list[j].StartTime > serverTime)
			{
				this.cacheVOD(list[j].Url);
				return list[j];
			}
		}
		return null;
	}

	private void cacheVOD(string url)
	{
		string text = this.UrlToCachePath(url, "mp4");
		if (File.Exists(text) || this._cr_cacheVOD != null)
		{
			return;
		}
		this._cr_cacheVOD = base.StartCoroutine(this.cr_cacheVOD(text, url));
	}

	private string UrlToCachePath(string url, string extension)
	{
		return Application.persistentDataPath + Path.DirectorySeparatorChar.ToString() + string.Format("V{0:X}.{1}", url.GetHashCode(), extension);
	}

	private IEnumerator cr_cacheVOD(string file, string url)
	{
		UnityWebRequest www = new UnityWebRequest(url);
		www.downloadHandler = new DownloadHandlerBuffer();
		yield return www.SendWebRequest();
		if (www.result != UnityWebRequest.Result.Success)
		{
			Debug.LogError("VOD :: error :: " + www.error);
		}
		else
		{
			File.WriteAllBytes(file, www.downloadHandler.data);
			this.cache.Add(file);
			PlayerPrefs.SetString("_VODCache_", JsonConvert.SerializeObject(this.cache));
		}
		this._cr_cacheVOD = null;
		yield break;
	}

	private void Start()
	{
		this.cache = new List<string>();
		string @string = PlayerPrefs.GetString("_VODCache_");
		if (@string.IsNullOrEmpty())
		{
			return;
		}
		List<string> list = JsonConvert.DeserializeObject<List<string>>(@string);
		for (int i = 0; i < list.Count; i++)
		{
			if (File.Exists(list[i]))
			{
				if ((DateTime.Now - File.GetCreationTime(list[i])).TotalDays > 30.0)
				{
					File.Delete(list[i]);
				}
				else
				{
					this.cache.Add(list[i]);
				}
			}
		}
		PlayerPrefs.SetString("_VODCache_", JsonConvert.SerializeObject(this.cache));
	}

	private void PositionAudio()
	{
		float num = float.MaxValue;
		VODTarget vodtarget = null;
		for (int i = 0; i < this.targets.Count; i++)
		{
			if (this.targets[i].AudioSettings.volume > 0f)
			{
				float sqrMagnitude = (VRRig.LocalRig.transform.position - this.targets[i].transform.position).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					vodtarget = this.targets[i];
					num = sqrMagnitude;
				}
			}
		}
		if (vodtarget == null)
		{
			return;
		}
		this.audioSource.transform.position = vodtarget.transform.position;
		this.audioSource.volume = vodtarget.AudioSettings.volume;
		this.audioSource.dopplerLevel = vodtarget.AudioSettings.dopplerLevel;
		this.audioSource.rolloffMode = vodtarget.AudioSettings.rolloffMode;
		this.audioSource.minDistance = vodtarget.AudioSettings.minDistance;
		this.audioSource.maxDistance = vodtarget.AudioSettings.maxDistance;
	}

	private void PlayPreviouStream()
	{
		DateTime serverTime = GorillaComputer.instance.GetServerTime();
		Debug.Log(string.Format("VOD :: serverTime={0}", serverTime));
		int hour = serverTime.Hour;
		int minute = serverTime.Minute;
		DateTime dateTime = new DateTime(serverTime.Year, serverTime.Month, serverTime.Day, hour, minute, 0);
		int num = -1;
		for (int i = 0; i < this.schedule.hourly.Length; i++)
		{
			if (this.schedule.hourly[i].minute <= minute && this.schedule.hourly[i].IsDateInRange(serverTime))
			{
				num = i;
			}
		}
		if (num >= 0)
		{
			int num2 = minute - this.schedule.hourly[num].minute;
			this.StartPlayback(this.schedule.hourly[num].stream, serverTime.Subtract(dateTime.AddMinutes((double)(-(double)num2))).TotalSeconds);
		}
	}

	private void StartPlayback(VODPlayer.VODStream str, double time = 0.0)
	{
		this.imageClearTime = 0f;
		VODPlayer.VODStream.VODStreamType type = str.type;
		if (type == VODPlayer.VODStream.VODStreamType.VIDEO)
		{
			Debug.Log("VOD :: StartVideoPlayback :: go");
			this.StartVideoPlayback(str.url, time);
			return;
		}
		if (type != VODPlayer.VODStream.VODStreamType.IMAGE)
		{
			return;
		}
		Debug.Log("VOD :: StartImagePlayback :: go");
		this.StartImagePlayback(str.url, str.duration, time);
	}

	private void StartImagePlayback(string url, int duration, double time = 0.0)
	{
		VODPlayer.<StartImagePlayback>d__40 <StartImagePlayback>d__;
		<StartImagePlayback>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<StartImagePlayback>d__.<>4__this = this;
		<StartImagePlayback>d__.url = url;
		<StartImagePlayback>d__.duration = duration;
		<StartImagePlayback>d__.time = time;
		<StartImagePlayback>d__.<>1__state = -1;
		<StartImagePlayback>d__.<>t__builder.Start<VODPlayer.<StartImagePlayback>d__40>(ref <StartImagePlayback>d__);
	}

	private void StartVideoPlayback(string url, double time = 0.0)
	{
		VODPlayer.<StartVideoPlayback>d__41 <StartVideoPlayback>d__;
		<StartVideoPlayback>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<StartVideoPlayback>d__.<>4__this = this;
		<StartVideoPlayback>d__.url = url;
		<StartVideoPlayback>d__.time = time;
		<StartVideoPlayback>d__.<>1__state = -1;
		<StartVideoPlayback>d__.<>t__builder.Start<VODPlayer.<StartVideoPlayback>d__41>(ref <StartVideoPlayback>d__);
	}

	private void onTD(string s)
	{
		if (s.IsNullOrEmpty())
		{
			this.Crash("No schedule data");
			return;
		}
		try
		{
			this.schedule = JsonConvert.DeserializeObject<VODPlayer.VODStreamSchedule>(s);
			for (int i = 0; i < this.schedule.hourly.Length; i++)
			{
				this.schedule.hourly[i].ValidateDate();
			}
		}
		catch (Exception)
		{
			this.Crash("Malformed schedule data");
			return;
		}
		if (this.schedule.hourly.Length == 0)
		{
			this.Crash("Nothing scheduled in title data");
			return;
		}
		this.waitOnServerTime();
	}

	private void Crash(string msg)
	{
		VODPlayer.state = VODPlayer.State.CRASHED;
		if (VODPlayer.OnCrash != null)
		{
			VODPlayer.OnCrash();
		}
		Debug.LogError("VOD :: CRASHED :: " + msg);
		for (int i = 0; i < this.targets.Count; i++)
		{
			this.targets[i].gameObject.SetActive(false);
		}
	}

	private void onTDError(PlayFabError error)
	{
		this.Crash(error.ErrorMessage);
	}

	private const string PlayerPrefKey_Cache = "_VODCache_";

	public static Action OnCrash;

	public static VODPlayer.State state;

	private VideoPlayer player;

	private AudioSource audioSource;

	private VODPlayer.VODNextStream nextStream;

	[SerializeField]
	private VODPlayer.VODStreamSchedule schedule;

	[SerializeField]
	private string titleDataKey;

	[SerializeField]
	private Material standbyMaterial;

	[SerializeField]
	private Material playBackMaterial;

	[SerializeField]
	private Material disconnectedMaterial;

	[SerializeField]
	private Material busyMaterial;

	[SerializeField]
	private Material imageMaterial;

	private List<VODTarget> targets = new List<VODTarget>();

	private int lastCheck;

	private List<string> cache = new List<string>();

	private Coroutine _cr_cacheVOD;

	private bool playerBusy;

	private float imageClearTime;

	public enum State
	{
		INITIALIZING,
		IDLE,
		RUNNING,
		CRASHED
	}

	[Serializable]
	public class VODNextStream : IComparable<VODPlayer.VODNextStream>
	{
		public VODNextStream(string name, DateTime startTime, string url)
		{
			this.Title = name;
			this.StartTime = startTime;
			this.Url = url;
		}

		int IComparable<VODPlayer.VODNextStream>.CompareTo(VODPlayer.VODNextStream other)
		{
			return (int)(this.StartTime - other.StartTime).TotalSeconds;
		}

		public string Title;

		public DateTime StartTime;

		public string Url;
	}

	[Serializable]
	public struct VODStreamSchedule
	{
		public VODPlayer.VODHourlyStream[] hourly;
	}

	[Serializable]
	public struct VODStream
	{
		public string name;

		public string url;

		public VODPlayer.VODStream.VODStreamType type;

		public int duration;

		public enum VODStreamType
		{
			VIDEO,
			IMAGE
		}
	}

	[Serializable]
	public struct VODHourlyStream : IComparable<VODPlayer.VODHourlyStream>
	{
		public int CompareTo(VODPlayer.VODHourlyStream other)
		{
			return this.minute - other.minute;
		}

		public void ValidateDate()
		{
			try
			{
				this.startDT = DateTime.Parse(this.startDateTime);
			}
			catch
			{
				this.startDT = DateTime.Parse("1/1/0001");
			}
			try
			{
				this.endDT = DateTime.Parse(this.endDateTime);
			}
			catch
			{
				this.endDT = DateTime.Parse("1/1/3001");
			}
			this.startDateTime = this.startDT.ToString();
			this.endDateTime = this.endDT.ToString();
		}

		internal bool IsDateInRange(DateTime serverTime)
		{
			return serverTime >= this.startDT && serverTime <= this.endDT;
		}

		internal DateTime ClampedDateTime(DateTime dateTime)
		{
			if (dateTime < this.startDT)
			{
				return this.startDT;
			}
			if (dateTime > this.endDT)
			{
				return this.endDT;
			}
			return dateTime;
		}

		public VODPlayer.VODStream stream;

		[Range(0f, 59f)]
		public int minute;

		public string startDateTime;

		private DateTime startDT;

		public string endDateTime;

		private DateTime endDT;
	}
}
