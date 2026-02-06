using System;
using System.Runtime.CompilerServices;
using GorillaNetworking;
using PlayFab;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshPro))]
public class SimpleCountdown : ObservableBehavior
{
	private void Start()
	{
		SimpleCountdown.<Start>d__8 <Start>d__;
		<Start>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<Start>d__.<>4__this = this;
		<Start>d__.<>1__state = -1;
		<Start>d__.<>t__builder.Start<SimpleCountdown.<Start>d__8>(ref <Start>d__);
	}

	private void onTD(string s)
	{
		this.date = s;
		this.ParseDateTime();
	}

	private void onTDError(PlayFabError error)
	{
		Debug.Log(string.Concat(new string[]
		{
			"SimpleCountdown component on ",
			base.name,
			" failed to get '",
			this.titleDataKey,
			"' from title data. Using Fallback: '",
			this.date,
			"'"
		}));
		this.ParseDateTime();
	}

	private void ParseDateTime()
	{
		if (!DateTime.TryParse(this.date, out this.dt))
		{
			Debug.Log(string.Concat(new string[]
			{
				"SimpleCountdown component on ",
				base.name,
				" has an unparsable date string: '",
				this.date,
				"'"
			}));
			Object.Destroy(base.gameObject);
		}
	}

	protected override void ObservableSliceUpdate()
	{
		if (GorillaComputer.instance == null)
		{
			return;
		}
		DateTime dateTime = this.dt;
		TimeSpan timeSpan = this.dt - GorillaComputer.instance.GetServerTime();
		if (timeSpan.TotalHours <= (double)this.hourRange.x || timeSpan.TotalHours >= (double)this.hourRange.y)
		{
			timeSpan = timeSpan.Multiply(0.0);
		}
		switch (this.displayFormat)
		{
		case SimpleCountdown.DisplayFormat.DD_HH_MM_SS:
			this.tmp.text = string.Format("{0:00}:{1:00}:{2:00}:{3:00}", new object[]
			{
				timeSpan.Days,
				timeSpan.Hours,
				timeSpan.Minutes,
				timeSpan.Seconds
			});
			return;
		case SimpleCountdown.DisplayFormat.HH_MM_SS:
			this.tmp.text = string.Format("{0:00}:{1:00}:{2:00}", Math.Floor(timeSpan.TotalHours), timeSpan.Minutes, timeSpan.Seconds);
			return;
		case SimpleCountdown.DisplayFormat.DD_HH_MM:
			this.tmp.text = string.Format("{0:00}:{1:00}:{2:00}", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes);
			return;
		case SimpleCountdown.DisplayFormat.HH_MM:
			this.tmp.text = string.Format("{0:00}:{1:00}", Math.Floor(timeSpan.TotalHours), timeSpan.Minutes);
			return;
		default:
			return;
		}
	}

	protected override void OnBecameObservable()
	{
	}

	protected override void OnLostObservable()
	{
	}

	[SerializeField]
	private SimpleCountdown.DisplayFormat displayFormat;

	[SerializeField]
	private bool useTitleData = true;

	[SerializeField]
	private string titleDataKey;

	[SerializeField]
	private string date;

	[SerializeField]
	private Vector2 hourRange = new Vector2(float.MinValue, float.MaxValue);

	private DateTime dt;

	private TextMeshPro tmp;

	private enum DisplayFormat
	{
		DD_HH_MM_SS,
		HH_MM_SS,
		DD_HH_MM,
		HH_MM
	}
}
