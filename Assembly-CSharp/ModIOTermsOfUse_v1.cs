using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class ModIOTermsOfUse_v1 : MonoBehaviour
{
	private void OnEnable()
	{
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction += this.PostUpdate;
		}
	}

	private void OnDisable()
	{
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction -= this.PostUpdate;
		}
	}

	private void PostUpdate()
	{
		if (ControllerBehaviour.Instance.IsLeftStick)
		{
			this.TurnPage(-1);
		}
		if (ControllerBehaviour.Instance.IsRightStick)
		{
			this.TurnPage(1);
		}
		if (this.waitingForAcknowledge)
		{
			this.acceptButtonDown = ControllerBehaviour.Instance.ButtonDown;
		}
	}

	private void Start()
	{
		ModIOTermsOfUse_v1.<Start>d__19 <Start>d__;
		<Start>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<Start>d__.<>4__this = this;
		<Start>d__.<>1__state = -1;
		<Start>d__.<>t__builder.Start<ModIOTermsOfUse_v1.<Start>d__19>(ref <Start>d__);
	}

	private Task<bool> UpdateTextFromTerms()
	{
		ModIOTermsOfUse_v1.<UpdateTextFromTerms>d__20 <UpdateTextFromTerms>d__;
		<UpdateTextFromTerms>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<UpdateTextFromTerms>d__.<>4__this = this;
		<UpdateTextFromTerms>d__.<>1__state = -1;
		<UpdateTextFromTerms>d__.<>t__builder.Start<ModIOTermsOfUse_v1.<UpdateTextFromTerms>d__20>(ref <UpdateTextFromTerms>d__);
		return <UpdateTextFromTerms>d__.<>t__builder.Task;
	}

	public Task<bool> UpdateTextWithFullTerms()
	{
		ModIOTermsOfUse_v1.<UpdateTextWithFullTerms>d__21 <UpdateTextWithFullTerms>d__;
		<UpdateTextWithFullTerms>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<UpdateTextWithFullTerms>d__.<>1__state = -1;
		<UpdateTextWithFullTerms>d__.<>t__builder.Start<ModIOTermsOfUse_v1.<UpdateTextWithFullTerms>d__21>(ref <UpdateTextWithFullTerms>d__);
		return <UpdateTextWithFullTerms>d__.<>t__builder.Task;
	}

	private string GetStringForListItemIdx_LowerAlpha(int idx)
	{
		switch (idx)
		{
		case 0:
			return "  a. <indent=5%>";
		case 1:
			return "  b. <indent=5%>";
		case 2:
			return "  c. <indent=5%>";
		case 3:
			return "  d. <indent=5%>";
		case 4:
			return "  e. <indent=5%>";
		case 5:
			return "  f. <indent=5%>";
		case 6:
			return "  g. <indent=5%>";
		case 7:
			return "  h. <indent=5%>";
		case 8:
			return "  i. <indent=5%>";
		case 9:
			return "  j. <indent=5%>";
		case 10:
			return "  k. <indent=5%>";
		case 11:
			return "  l. <indent=5%>";
		case 12:
			return "  m. <indent=5%>";
		case 13:
			return "  n. <indent=5%>";
		case 14:
			return "  o. <indent=5%>";
		case 15:
			return "  p. <indent=5%>";
		case 16:
			return "  q. <indent=5%>";
		case 17:
			return "  r. <indent=5%>";
		case 18:
			return "  s. <indent=5%>";
		case 19:
			return "  t. <indent=5%>";
		case 20:
			return "  u. <indent=5%>";
		case 21:
			return "  v. <indent=5%>";
		case 22:
			return "  w. <indent=5%>";
		case 23:
			return "  x. <indent=5%>";
		case 24:
			return "  y. <indent=5%>";
		case 25:
			return "  z. <indent=5%>";
		default:
			return "";
		}
	}

	private Task WaitForAcknowledgement()
	{
		ModIOTermsOfUse_v1.<WaitForAcknowledgement>d__23 <WaitForAcknowledgement>d__;
		<WaitForAcknowledgement>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<WaitForAcknowledgement>d__.<>4__this = this;
		<WaitForAcknowledgement>d__.<>1__state = -1;
		<WaitForAcknowledgement>d__.<>t__builder.Start<ModIOTermsOfUse_v1.<WaitForAcknowledgement>d__23>(ref <WaitForAcknowledgement>d__);
		return <WaitForAcknowledgement>d__.<>t__builder.Task;
	}

	public void TurnPage(int i)
	{
		this.tmpBody.pageToDisplay = Mathf.Clamp(this.tmpBody.pageToDisplay + i, 1, this.tmpBody.textInfo.pageCount);
		this.tmpPage.text = string.Format("page {0} of {1}", this.tmpBody.pageToDisplay, this.tmpBody.textInfo.pageCount);
		this.nextButton.SetActive(this.tmpBody.pageToDisplay < this.tmpBody.textInfo.pageCount);
		this.prevButton.SetActive(this.tmpBody.pageToDisplay > 1);
		this.ActivateAcceptButtonGroup();
	}

	private void ActivateAcceptButtonGroup()
	{
		bool active = this.tmpBody.pageToDisplay == this.tmpBody.textInfo.pageCount;
		this.yesNoButtons.SetActive(active);
		this.waitingForAcknowledge = active;
	}

	public void Acknowledge(bool didAccept)
	{
		this.accepted = didAccept;
	}

	[SerializeField]
	private Transform uiParent;

	[SerializeField]
	private string title;

	[SerializeField]
	private TMP_Text tmpBody;

	[SerializeField]
	private TMP_Text tmpTitle;

	[SerializeField]
	private TMP_Text tmpPage;

	[SerializeField]
	public GameObject yesNoButtons;

	[SerializeField]
	public GameObject nextButton;

	[SerializeField]
	public GameObject prevButton;

	private bool hasTermsOfUse;

	private Action<bool> termsAcknowledgedCallback;

	private string cachedTermsText;

	private bool waitingForAcknowledge;

	private bool accepted;

	private bool acceptButtonDown;

	[SerializeField]
	private float holdTime = 5f;

	[SerializeField]
	private LineRenderer progressBar;
}
