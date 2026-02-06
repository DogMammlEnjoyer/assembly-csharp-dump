using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

public class KIDUI_SendUpgradeEmailScreen : MonoBehaviour
{
	public Task SendUpgradeEmail(List<string> requestedPermissions)
	{
		KIDUI_SendUpgradeEmailScreen.<SendUpgradeEmail>d__4 <SendUpgradeEmail>d__;
		<SendUpgradeEmail>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<SendUpgradeEmail>d__.<>4__this = this;
		<SendUpgradeEmail>d__.requestedPermissions = requestedPermissions;
		<SendUpgradeEmail>d__.<>1__state = -1;
		<SendUpgradeEmail>d__.<>t__builder.Start<KIDUI_SendUpgradeEmailScreen.<SendUpgradeEmail>d__4>(ref <SendUpgradeEmail>d__);
		return <SendUpgradeEmail>d__.<>t__builder.Task;
	}

	public void OnCancel()
	{
		base.gameObject.SetActive(false);
		this._mainScreen.ShowMainScreen(EMainScreenStatus.None);
	}

	private void OnSuccess()
	{
		base.gameObject.SetActive(false);
		this._successScreen.Show(null);
	}

	private void OnFailure(string errorMessage)
	{
		base.gameObject.SetActive(false);
		this._errorScreen.Show(errorMessage);
	}

	[SerializeField]
	private KIDUI_AnimatedEllipsis _animatedEllipsis;

	[SerializeField]
	private KIDUI_MessageScreen _successScreen;

	[SerializeField]
	private KIDUI_MessageScreen _errorScreen;

	[SerializeField]
	private KIDUI_MainScreen _mainScreen;
}
