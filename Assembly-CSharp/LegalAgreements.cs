using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using PlayFab;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(1)]
public class LegalAgreements : MonoBehaviour
{
	public static LegalAgreements instance { get; private set; }

	protected virtual void Awake()
	{
		if (LegalAgreements.instance != null)
		{
			Debug.LogError("Trying to set [LegalAgreements] instance but it is not null", this);
			base.gameObject.SetActive(false);
			return;
		}
		LegalAgreements.instance = this;
		this.stickHeldDuration = 0f;
		this.scrollSpeed = this._minScrollSpeed;
		base.enabled = false;
	}

	private void Update()
	{
		if (!this.legalAgreementsStarted)
		{
			return;
		}
		float num = Time.deltaTime * this.scrollSpeed;
		if (ControllerBehaviour.Instance.IsUpStick || ControllerBehaviour.Instance.IsDownStick)
		{
			if (ControllerBehaviour.Instance.IsDownStick)
			{
				num *= -1f;
			}
			this.scrollBar.value = Mathf.Clamp(this.scrollBar.value + num, 0f, 1f);
			if (this.scrollBar.value > 0f && this.scrollBar.value < 1f)
			{
				HandRayController.Instance.PulseActiveHandray(this._stickVibrationStrength, this._stickVibrationDuration);
			}
			this.stickHeldDuration += Time.deltaTime;
			this.scrollTime = Mathf.Clamp01(this.stickHeldDuration / this._scrollInterpTime);
			this.scrollSpeed = Mathf.Lerp(this._minScrollSpeed, this._maxScrollSpeed, this._scrollInterpCurve.Evaluate(this.scrollTime));
			this.scrollSpeed *= Mathf.Abs(ControllerBehaviour.Instance.StickYValue);
		}
		else
		{
			this.stickHeldDuration = 0f;
			this.scrollSpeed = this._minScrollSpeed;
		}
		if (this._scrollToBottomText)
		{
			if ((double)this.scrollBar.value < 0.001)
			{
				this._scrollToBottomText.gameObject.SetActive(false);
				this._pressAndHoldToConfirmButton.gameObject.SetActive(true);
				return;
			}
			this._scrollToBottomText.text = LegalAgreements.SCROLL_TO_END_MESSAGE;
			this._scrollToBottomText.gameObject.SetActive(true);
			this._pressAndHoldToConfirmButton.gameObject.SetActive(false);
		}
	}

	public virtual Task StartLegalAgreements()
	{
		LegalAgreements.<StartLegalAgreements>d__24 <StartLegalAgreements>d__;
		<StartLegalAgreements>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<StartLegalAgreements>d__.<>4__this = this;
		<StartLegalAgreements>d__.<>1__state = -1;
		<StartLegalAgreements>d__.<>t__builder.Start<LegalAgreements.<StartLegalAgreements>d__24>(ref <StartLegalAgreements>d__);
		return <StartLegalAgreements>d__.<>t__builder.Task;
	}

	public void OnAccepted(int currentAge)
	{
		this._accepted = true;
	}

	protected Task WaitForAcknowledgement()
	{
		LegalAgreements.<WaitForAcknowledgement>d__27 <WaitForAcknowledgement>d__;
		<WaitForAcknowledgement>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<WaitForAcknowledgement>d__.<>4__this = this;
		<WaitForAcknowledgement>d__.<>1__state = -1;
		<WaitForAcknowledgement>d__.<>t__builder.Start<LegalAgreements.<WaitForAcknowledgement>d__27>(ref <WaitForAcknowledgement>d__);
		return <WaitForAcknowledgement>d__.<>t__builder.Task;
	}

	private Task<bool> UpdateText(LegalAgreementTextAsset asset, string version)
	{
		LegalAgreements.<UpdateText>d__28 <UpdateText>d__;
		<UpdateText>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<UpdateText>d__.<>4__this = this;
		<UpdateText>d__.asset = asset;
		<UpdateText>d__.version = version;
		<UpdateText>d__.<>1__state = -1;
		<UpdateText>d__.<>t__builder.Start<LegalAgreements.<UpdateText>d__28>(ref <UpdateText>d__);
		return <UpdateText>d__.<>t__builder.Task;
	}

	public Task<bool> UpdateTextFromPlayFabTitleData(string key, string version, TMP_Text target)
	{
		LegalAgreements.<UpdateTextFromPlayFabTitleData>d__33 <UpdateTextFromPlayFabTitleData>d__;
		<UpdateTextFromPlayFabTitleData>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<UpdateTextFromPlayFabTitleData>d__.<>4__this = this;
		<UpdateTextFromPlayFabTitleData>d__.key = key;
		<UpdateTextFromPlayFabTitleData>d__.version = version;
		<UpdateTextFromPlayFabTitleData>d__.target = target;
		<UpdateTextFromPlayFabTitleData>d__.<>1__state = -1;
		<UpdateTextFromPlayFabTitleData>d__.<>t__builder.Start<LegalAgreements.<UpdateTextFromPlayFabTitleData>d__33>(ref <UpdateTextFromPlayFabTitleData>d__);
		return <UpdateTextFromPlayFabTitleData>d__.<>t__builder.Task;
	}

	private void OnPlayFabError(PlayFabError error)
	{
		this.state = -1;
	}

	private void OnTitleDataReceived(string obj)
	{
		this.cachedText = obj;
		this.state = 1;
	}

	private Task<string> GetTitleDataAsync(string key)
	{
		LegalAgreements.<GetTitleDataAsync>d__36 <GetTitleDataAsync>d__;
		<GetTitleDataAsync>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
		<GetTitleDataAsync>d__.key = key;
		<GetTitleDataAsync>d__.<>1__state = -1;
		<GetTitleDataAsync>d__.<>t__builder.Start<LegalAgreements.<GetTitleDataAsync>d__36>(ref <GetTitleDataAsync>d__);
		return <GetTitleDataAsync>d__.<>t__builder.Task;
	}

	private Task<Dictionary<string, string>> GetAcceptedAgreements(LegalAgreementTextAsset[] agreements)
	{
		LegalAgreements.<GetAcceptedAgreements>d__37 <GetAcceptedAgreements>d__;
		<GetAcceptedAgreements>d__.<>t__builder = AsyncTaskMethodBuilder<Dictionary<string, string>>.Create();
		<GetAcceptedAgreements>d__.agreements = agreements;
		<GetAcceptedAgreements>d__.<>1__state = -1;
		<GetAcceptedAgreements>d__.<>t__builder.Start<LegalAgreements.<GetAcceptedAgreements>d__37>(ref <GetAcceptedAgreements>d__);
		return <GetAcceptedAgreements>d__.<>t__builder.Task;
	}

	private Task SubmitAcceptedAgreements(Dictionary<string, string> agreements)
	{
		LegalAgreements.<SubmitAcceptedAgreements>d__38 <SubmitAcceptedAgreements>d__;
		<SubmitAcceptedAgreements>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<SubmitAcceptedAgreements>d__.agreements = agreements;
		<SubmitAcceptedAgreements>d__.<>1__state = -1;
		<SubmitAcceptedAgreements>d__.<>t__builder.Start<LegalAgreements.<SubmitAcceptedAgreements>d__38>(ref <SubmitAcceptedAgreements>d__);
		return <SubmitAcceptedAgreements>d__.<>t__builder.Task;
	}

	public void OnDisable()
	{
		KIDAudioManager instance = KIDAudioManager.Instance;
		if (instance == null)
		{
			return;
		}
		instance.PlaySoundWithDelay(KIDAudioManager.KIDSoundType.PageTransition);
	}

	private static string SCROLL_TO_END_MESSAGE = "<b>Scroll to the bottom</b> to continue.";

	[Header("Scroll Behavior")]
	[SerializeField]
	protected float _minScrollSpeed = 0.02f;

	[SerializeField]
	private float _maxScrollSpeed = 3f;

	[SerializeField]
	private float _scrollInterpTime = 3f;

	[SerializeField]
	private AnimationCurve _scrollInterpCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[SerializeField]
	protected Transform uiParent;

	[SerializeField]
	protected TMP_Text tmpBody;

	[SerializeField]
	protected TMP_Text tmpTitle;

	[SerializeField]
	protected Scrollbar scrollBar;

	[SerializeField]
	private LegalAgreementTextAsset[] legalAgreementScreens;

	[SerializeField]
	protected KIDUIButton _pressAndHoldToConfirmButton;

	[SerializeField]
	private TMP_Text _scrollToBottomText;

	[SerializeField]
	private float _stickVibrationStrength = 0.1f;

	[SerializeField]
	private float _stickVibrationDuration = 0.05f;

	protected float stickHeldDuration;

	protected float scrollSpeed;

	private float scrollTime;

	protected bool legalAgreementsStarted;

	protected bool _accepted;

	private string cachedText;

	private int state;

	private bool optIn;

	private bool optional;
}
