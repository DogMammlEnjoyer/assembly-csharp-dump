using System;
using System.Runtime.CompilerServices;
using GorillaNetworking;
using PlayFab;
using TMPro;
using UnityEngine;

public class Clock3x3 : ObservableBehavior
{
	protected override void ObservableSliceUpdate()
	{
		DateTime serverTime = GorillaComputer.instance.GetServerTime();
		DateTime now = DateTime.Now;
		if (serverTime.Year < 2000)
		{
			return;
		}
		this.display.text = string.Format(this.formatString, new object[]
		{
			this.headings[0],
			this.headings[1],
			serverTime.ToString("hh:mm:sstt"),
			DateTime.Now.ToString("hh:mm:sstt"),
			this.HexColor(this.color.Evaluate(((float)serverTime.Hour + (float)serverTime.Minute / 60f) / 24f)),
			this.HexColor(this.color.Evaluate(((float)now.Hour + (float)now.Minute / 60f) / 24f))
		});
	}

	public string HexColor(Color color)
	{
		return "#" + Mathf.FloorToInt(Mathf.Clamp01(color.r) * 255f).ToString("X2") + Mathf.FloorToInt(Mathf.Clamp01(color.g) * 255f).ToString("X2") + Mathf.FloorToInt(Mathf.Clamp01(color.b) * 255f).ToString("X2");
	}

	protected override void OnBecameObservable()
	{
		this.display.gameObject.SetActive(true);
		this.Initialize();
	}

	private void Initialize()
	{
		Clock3x3.<Initialize>d__9 <Initialize>d__;
		<Initialize>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<Initialize>d__.<>4__this = this;
		<Initialize>d__.<>1__state = -1;
		<Initialize>d__.<>t__builder.Start<Clock3x3.<Initialize>d__9>(ref <Initialize>d__);
	}

	private void onTD(string s)
	{
		this.headings = s.Split(";", StringSplitOptions.None);
	}

	private void onTDError(PlayFabError error)
	{
		Debug.LogError(string.Format("Clock3x3 :: onTDError :: {0} :: {1}", this.titleDataKey, error));
	}

	protected override void OnLostObservable()
	{
		this.display.gameObject.SetActive(false);
	}

	[SerializeField]
	private string titleDataKey;

	[SerializeField]
	private TMP_Text display;

	[SerializeField]
	private Gradient color;

	private string formatString;

	private bool initialized;

	[SerializeField]
	private string[] headings;
}
