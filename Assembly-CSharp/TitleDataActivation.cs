using System;
using System.Runtime.CompilerServices;
using GorillaNetworking;
using Newtonsoft.Json;
using PlayFab;
using UnityEngine;

public class TitleDataActivation : MonoBehaviour, IGorillaSliceableSimple
{
	[RuntimeInitializeOnLoadMethod]
	private static void RuntimeInit()
	{
		TitleDataActivation.<RuntimeInit>d__1 <RuntimeInit>d__;
		<RuntimeInit>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<RuntimeInit>d__.<>1__state = -1;
		<RuntimeInit>d__.<>t__builder.Start<TitleDataActivation.<RuntimeInit>d__1>(ref <RuntimeInit>d__);
	}

	private static void onTDReferenceDate(string s)
	{
		if (!DateTime.TryParse(s, out TitleDataActivation.ReferenceDate))
		{
			Debug.LogError("TitleDataActivation :: onTDReferenceDate :: No Reference Date Set!!");
		}
	}

	private static void onTDReferenceDateError(PlayFabError error)
	{
		Debug.LogError("TitleDataActivation :: onTDReferenceDateError :: No Reference Date Set!!");
	}

	private void Initialize()
	{
		TitleDataActivation.<Initialize>d__15 <Initialize>d__;
		<Initialize>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<Initialize>d__.<>4__this = this;
		<Initialize>d__.<>1__state = -1;
		<Initialize>d__.<>t__builder.Start<TitleDataActivation.<Initialize>d__15>(ref <Initialize>d__);
	}

	private void onTD(string s)
	{
		TitleDataActivation.TitleDataActivationData titleDataActivationData = null;
		try
		{
			titleDataActivationData = JsonConvert.DeserializeObject<TitleDataActivation.TitleDataActivationData>(s);
		}
		catch (Exception ex)
		{
			Debug.LogError("TitleDataActivation :: onTD :: " + ex.Message);
			return;
		}
		for (int i = 0; i < titleDataActivationData.Data.Length; i++)
		{
			if (titleDataActivationData.Data[i].TitleDataObjectID == this.titleDataObjectID)
			{
				this.activationData = titleDataActivationData.Data[i];
				return;
			}
		}
	}

	private void onTDError(PlayFabError error)
	{
		Debug.LogError(string.Format("TitleDataActivation :: onTDError :: {0} :: {1}", this.titleDataKey, error));
	}

	private void OnEnable()
	{
		this.Initialize();
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	private void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	void IGorillaSliceableSimple.SliceUpdate()
	{
		if (this.activationData == null)
		{
			return;
		}
		DateTime serverTime = GorillaComputer.instance.GetServerTime();
		if (serverTime.Year < 2000)
		{
			return;
		}
		bool flag = false;
		float num = 0f;
		int num2 = 0;
		while (this.activationData.AbsoluteDateTimeWindow != null && num2 < this.activationData.AbsoluteDateTimeWindow.Length && !flag)
		{
			this.activationData.AbsoluteDateTimeWindow[num2].IsInWindow(serverTime, out flag, out num);
			num2++;
		}
		int num3 = 0;
		while (this.activationData.RelativeDateTimeWindow != null && num3 < this.activationData.RelativeDateTimeWindow.Length && !flag)
		{
			this.activationData.RelativeDateTimeWindow[num3].IsInWindow(serverTime, out flag, out num);
			num3++;
		}
		if (flag != this.onOffState)
		{
			this.SetState(flag, num);
			this.onOffState = flag;
		}
	}

	private void SetState(bool onOff, float delayedActivation)
	{
		for (int i = 0; i < this.gameObjects.Length; i++)
		{
			this.gameObjects[i].SetActive(onOff);
			if (onOff && delayedActivation > 0f)
			{
				Animator[] componentsInChildren = this.gameObjects[i].GetComponentsInChildren<Animator>();
				for (int j = 0; j < componentsInChildren.Length; j++)
				{
					int fullPathHash = componentsInChildren[j].GetCurrentAnimatorStateInfo(0).fullPathHash;
					componentsInChildren[j].PlayInFixedTime(fullPathHash, 0, delayedActivation);
				}
			}
		}
	}

	public float GetDelayedActivationTime()
	{
		DateTime serverTime = GorillaComputer.instance.GetServerTime();
		if (serverTime.Year < 2000)
		{
			return 0f;
		}
		bool flag = false;
		float b = 0f;
		int num = 0;
		while (this.activationData.AbsoluteDateTimeWindow != null && num < this.activationData.AbsoluteDateTimeWindow.Length && !flag)
		{
			this.activationData.AbsoluteDateTimeWindow[num].IsInWindow(serverTime, out flag, out b);
			num++;
		}
		int num2 = 0;
		while (this.activationData.RelativeDateTimeWindow != null && num2 < this.activationData.RelativeDateTimeWindow.Length && !flag)
		{
			this.activationData.RelativeDateTimeWindow[num2].IsInWindow(serverTime, out flag, out b);
			num2++;
		}
		return Mathf.Max(0f, b);
	}

	public void PlayAnimatorAtScheduledTime(Animator animator)
	{
		float delayedActivationTime = this.GetDelayedActivationTime();
		int fullPathHash = animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
		animator.PlayInFixedTime(fullPathHash, 0, this.GetDelayedActivationTime());
		AudioSource[] componentsInChildren = animator.GetComponentsInChildren<AudioSource>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].playOnAwake && componentsInChildren[i].clip != null && componentsInChildren[i].clip.length > delayedActivationTime)
			{
				componentsInChildren[i].time = delayedActivationTime;
			}
		}
	}

	public static DateTime ReferenceDate = DateTime.MinValue;

	[SerializeField]
	private string titleDataKey;

	[SerializeField]
	private string titleDataObjectID;

	private TitleDataActivation.TitleDataObjectActivationData activationData;

	private GameObject[] gameObjects;

	private bool initialized;

	private bool onOffState;

	[Serializable]
	public class TitleDataActivationData
	{
		public TitleDataActivation.TitleDataObjectActivationData[] Data
		{
			get
			{
				return this.data;
			}
			set
			{
				this.data = value;
			}
		}

		[SerializeField]
		private TitleDataActivation.TitleDataObjectActivationData[] data;

		private bool validated;
	}

	[Serializable]
	public class TitleDataObjectActivationData
	{
		public string TitleDataObjectID
		{
			get
			{
				return this.titleDataObjectID;
			}
			set
			{
				this.titleDataObjectID = value;
			}
		}

		public TitleDataActivation.AbsoluteDateTimeWindow[] AbsoluteDateTimeWindow
		{
			get
			{
				return this.absoluteDateTimeWindow;
			}
			set
			{
				this.absoluteDateTimeWindow = value;
			}
		}

		public TitleDataActivation.RelativeDateTimeWindow[] RelativeDateTimeWindow
		{
			get
			{
				return this.relativeDateTimeWindow;
			}
			set
			{
				this.relativeDateTimeWindow = value;
			}
		}

		[SerializeField]
		private string titleDataObjectID;

		[SerializeField]
		private TitleDataActivation.AbsoluteDateTimeWindow[] absoluteDateTimeWindow;

		[SerializeField]
		private TitleDataActivation.RelativeDateTimeWindow[] relativeDateTimeWindow;

		private bool validated;
	}

	[Serializable]
	public class AbsoluteDateTimeWindow
	{
		public string StartDateTime
		{
			get
			{
				return this.startDateTime;
			}
			set
			{
				if (DateTime.TryParse(value, out this.dtStart))
				{
					this.startDateTime = this.dtStart.ToString();
				}
			}
		}

		public string EndDateTime
		{
			get
			{
				return this.endDateTime;
			}
			set
			{
				if (DateTime.TryParse(value, out this.dtEnd))
				{
					this.endDateTime = this.dtEnd.ToString();
				}
			}
		}

		public void IsInWindow(DateTime d, out bool inRange, out float delay)
		{
			inRange = (d >= this.dtStart && d <= this.dtEnd);
			delay = (float)(d - this.dtStart).TotalSeconds;
		}

		protected DateTime dtStart;

		protected DateTime dtEnd;

		[SerializeField]
		private string startDateTime;

		[SerializeField]
		private string endDateTime;
	}

	[Serializable]
	public class RelativeDateTimeWindow
	{
		public TitleDataActivation.RelativeDateTime StartDateTime
		{
			get
			{
				return this.startDateTime;
			}
			set
			{
				this.startDateTime = value;
				this.dtStart = TitleDataActivation.ReferenceDate.AddDays((double)this.startDateTime.DaysPast).AddHours((double)this.startDateTime.Hour).AddMinutes((double)this.startDateTime.Minute);
			}
		}

		public TitleDataActivation.RelativeDateTime EndDateTime
		{
			get
			{
				return this.endDateTime;
			}
			set
			{
				this.endDateTime = value;
				this.dtEnd = TitleDataActivation.ReferenceDate.AddDays((double)this.endDateTime.DaysPast).AddHours((double)this.endDateTime.Hour).AddMinutes((double)this.endDateTime.Minute);
			}
		}

		public void IsInWindow(DateTime d, out bool inRange, out float delay)
		{
			inRange = (d >= this.dtStart && d <= this.dtEnd);
			delay = (float)(d - this.dtStart).TotalSeconds;
		}

		protected DateTime dtStart;

		protected DateTime dtEnd;

		[SerializeField]
		private TitleDataActivation.RelativeDateTime startDateTime;

		[SerializeField]
		private TitleDataActivation.RelativeDateTime endDateTime;
	}

	[Serializable]
	public struct RelativeDateTime
	{
		public int DaysPast;

		public int Hour;

		public int Minute;
	}
}
