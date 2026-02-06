using System;
using GorillaTagScripts;
using TMPro;
using UnityEngine;

[Obsolete("DEPRECATED! Use SubscriptionKiosk instead")]
public class SubscriptionStation : MonoBehaviour
{
	private void Awake()
	{
		this.formatString = this.screenText.text;
		this.screenText.text = string.Format(this.formatString, new object[]
		{
			"*",
			"*",
			"*",
			"*"
		});
	}

	private void UpdateScreen()
	{
		Debug.Log(":::SubscriptionStation::UpdateScreen");
		bool flag = SubscriptionManager.GetSubscriptionDetails(VRRig.LocalRig).tier > 0;
		int daysAccrued = SubscriptionManager.GetSubscriptionDetails(VRRig.LocalRig).daysAccrued;
		bool subsOnlyMatchmaking = SubscriptionManager.SubsOnlyMatchmaking;
		bool showGoldNameTag = VRRig.LocalRig.ShowGoldNameTag;
		if (flag)
		{
			this.screenText.text = string.Format(this.formatString, new object[]
			{
				"Y",
				subsOnlyMatchmaking ? "Y" : "N",
				showGoldNameTag ? "Y" : "N",
				daysAccrued
			});
			return;
		}
		this.screenText.text = string.Format(this.formatString, new object[]
		{
			"N",
			"*",
			"*",
			"*"
		});
	}

	public void ToggleSubscriptionStatus()
	{
		SubscriptionManager.ForceRecheck();
		this.UpdateScreen();
	}

	public void ToggleSubsOnly()
	{
		SubscriptionManager.SubsOnlyMatchmaking = !SubscriptionManager.SubsOnlyMatchmaking;
		this.UpdateScreen();
	}

	public void ToggleSubsDecoration()
	{
		this.UpdateScreen();
	}

	[SerializeField]
	private TMP_Text screenText;

	private string formatString;
}
