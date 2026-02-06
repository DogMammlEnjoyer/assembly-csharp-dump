using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

public class KIDUI_AgeDiscrepancyScreen : MonoBehaviour
{
	private void Awake()
	{
		this.CheckLocalizationReferences();
	}

	public Task ShowAgeDiscrepancyScreenWithAwait(string description)
	{
		KIDUI_AgeDiscrepancyScreen.<ShowAgeDiscrepancyScreenWithAwait>d__8 <ShowAgeDiscrepancyScreenWithAwait>d__;
		<ShowAgeDiscrepancyScreenWithAwait>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<ShowAgeDiscrepancyScreenWithAwait>d__.<>4__this = this;
		<ShowAgeDiscrepancyScreenWithAwait>d__.description = description;
		<ShowAgeDiscrepancyScreenWithAwait>d__.<>1__state = -1;
		<ShowAgeDiscrepancyScreenWithAwait>d__.<>t__builder.Start<KIDUI_AgeDiscrepancyScreen.<ShowAgeDiscrepancyScreenWithAwait>d__8>(ref <ShowAgeDiscrepancyScreenWithAwait>d__);
		return <ShowAgeDiscrepancyScreenWithAwait>d__.<>t__builder.Task;
	}

	public Task ShowAgeDiscrepancyScreenWithAwait(int userAge, int accAge, int lowestAge)
	{
		KIDUI_AgeDiscrepancyScreen.<ShowAgeDiscrepancyScreenWithAwait>d__9 <ShowAgeDiscrepancyScreenWithAwait>d__;
		<ShowAgeDiscrepancyScreenWithAwait>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<ShowAgeDiscrepancyScreenWithAwait>d__.<>4__this = this;
		<ShowAgeDiscrepancyScreenWithAwait>d__.userAge = userAge;
		<ShowAgeDiscrepancyScreenWithAwait>d__.accAge = accAge;
		<ShowAgeDiscrepancyScreenWithAwait>d__.lowestAge = lowestAge;
		<ShowAgeDiscrepancyScreenWithAwait>d__.<>1__state = -1;
		<ShowAgeDiscrepancyScreenWithAwait>d__.<>t__builder.Start<KIDUI_AgeDiscrepancyScreen.<ShowAgeDiscrepancyScreenWithAwait>d__9>(ref <ShowAgeDiscrepancyScreenWithAwait>d__);
		return <ShowAgeDiscrepancyScreenWithAwait>d__.<>t__builder.Task;
	}

	private Task WaitForCompletion()
	{
		KIDUI_AgeDiscrepancyScreen.<WaitForCompletion>d__10 <WaitForCompletion>d__;
		<WaitForCompletion>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<WaitForCompletion>d__.<>4__this = this;
		<WaitForCompletion>d__.<>1__state = -1;
		<WaitForCompletion>d__.<>t__builder.Start<KIDUI_AgeDiscrepancyScreen.<WaitForCompletion>d__10>(ref <WaitForCompletion>d__);
		return <WaitForCompletion>d__.<>t__builder.Task;
	}

	public void OnHoldComplete()
	{
		this._hasCompleted = true;
	}

	public void OnQuitPressed()
	{
		Application.Quit();
	}

	private void CheckLocalizationReferences()
	{
		if (this._bodyLocStr != null && this._userAgeVar != null && this._accountAgeVar != null && this._lowestAgeVar != null)
		{
			return;
		}
		if (this._bodyTextLoc == null)
		{
			Debug.LogError("[LOCALIZATION::KIDUI_AGE_DISCREPANCY_SCREEN] [_bodyTextLoc] is not set, unable to localize smart string");
			return;
		}
		this._bodyLocStr = this._bodyTextLoc.StringReference;
		this._userAgeVar = (this._bodyLocStr["user-age"] as IntVariable);
		this._accountAgeVar = (this._bodyLocStr["account-age"] as IntVariable);
		this._lowestAgeVar = (this._bodyLocStr["lowest-age"] as IntVariable);
	}

	[SerializeField]
	private TMP_Text _descriptionText;

	[Header("Localization")]
	[SerializeField]
	private LocalizedText _bodyTextLoc;

	private bool _hasCompleted;

	private LocalizedString _bodyLocStr;

	private IntVariable _userAgeVar;

	private IntVariable _accountAgeVar;

	private IntVariable _lowestAgeVar;
}
