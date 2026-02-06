using System;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class KIDAgeAppeal : MonoBehaviour
{
	public void ShowAgeAppealScreen()
	{
		this._ageSlider = base.GetComponentInChildren<AgeSliderWithProgressBar>(true);
		this._ageSlider.ControllerActive = true;
		base.gameObject.SetActive(true);
		this._inputsContainer.SetActive(true);
		this._monkeLoader.SetActive(false);
	}

	public void OnNewAgeConfirmed()
	{
		KIDAgeAppeal.<OnNewAgeConfirmed>d__6 <OnNewAgeConfirmed>d__;
		<OnNewAgeConfirmed>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<OnNewAgeConfirmed>d__.<>4__this = this;
		<OnNewAgeConfirmed>d__.<>1__state = -1;
		<OnNewAgeConfirmed>d__.<>t__builder.Start<KIDAgeAppeal.<OnNewAgeConfirmed>d__6>(ref <OnNewAgeConfirmed>d__);
	}

	[SerializeField]
	private TMP_Text _ageText;

	[SerializeField]
	private KIDUI_AgeAppealEmailScreen _ageAppealEmailScreen;

	[SerializeField]
	private GameObject _inputsContainer;

	[SerializeField]
	private GameObject _monkeLoader;

	private AgeSliderWithProgressBar _ageSlider;
}
