using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class WarningScreens : MonoBehaviour
{
	private void Awake()
	{
		if (WarningScreens._activeReference == null)
		{
			WarningScreens._activeReference = this;
			return;
		}
		Debug.LogError("[WARNINGS] WarningScreens already exists. Destroying this instance.");
		Object.Destroy(this);
	}

	private Task<WarningButtonResult> StartWarningScreenInternal(CancellationToken cancellationToken)
	{
		WarningScreens.<StartWarningScreenInternal>d__14 <StartWarningScreenInternal>d__;
		<StartWarningScreenInternal>d__.<>t__builder = AsyncTaskMethodBuilder<WarningButtonResult>.Create();
		<StartWarningScreenInternal>d__.<>4__this = this;
		<StartWarningScreenInternal>d__.cancellationToken = cancellationToken;
		<StartWarningScreenInternal>d__.<>1__state = -1;
		<StartWarningScreenInternal>d__.<>t__builder.Start<WarningScreens.<StartWarningScreenInternal>d__14>(ref <StartWarningScreenInternal>d__);
		return <StartWarningScreenInternal>d__.<>t__builder.Task;
	}

	private Task<WarningButtonResult> StartOptInFollowUpScreenInternal(CancellationToken cancellationToken)
	{
		WarningScreens.<StartOptInFollowUpScreenInternal>d__15 <StartOptInFollowUpScreenInternal>d__;
		<StartOptInFollowUpScreenInternal>d__.<>t__builder = AsyncTaskMethodBuilder<WarningButtonResult>.Create();
		<StartOptInFollowUpScreenInternal>d__.<>4__this = this;
		<StartOptInFollowUpScreenInternal>d__.cancellationToken = cancellationToken;
		<StartOptInFollowUpScreenInternal>d__.<>1__state = -1;
		<StartOptInFollowUpScreenInternal>d__.<>t__builder.Start<WarningScreens.<StartOptInFollowUpScreenInternal>d__15>(ref <StartOptInFollowUpScreenInternal>d__);
		return <StartOptInFollowUpScreenInternal>d__.<>t__builder.Task;
	}

	public static Task<WarningButtonResult> StartWarningScreen(CancellationToken cancellationToken)
	{
		WarningScreens.<StartWarningScreen>d__16 <StartWarningScreen>d__;
		<StartWarningScreen>d__.<>t__builder = AsyncTaskMethodBuilder<WarningButtonResult>.Create();
		<StartWarningScreen>d__.cancellationToken = cancellationToken;
		<StartWarningScreen>d__.<>1__state = -1;
		<StartWarningScreen>d__.<>t__builder.Start<WarningScreens.<StartWarningScreen>d__16>(ref <StartWarningScreen>d__);
		return <StartWarningScreen>d__.<>t__builder.Task;
	}

	public static Task<WarningButtonResult> StartOptInFollowUpScreen(CancellationToken cancellationToken)
	{
		WarningScreens.<StartOptInFollowUpScreen>d__17 <StartOptInFollowUpScreen>d__;
		<StartOptInFollowUpScreen>d__.<>t__builder = AsyncTaskMethodBuilder<WarningButtonResult>.Create();
		<StartOptInFollowUpScreen>d__.cancellationToken = cancellationToken;
		<StartOptInFollowUpScreen>d__.<>1__state = -1;
		<StartOptInFollowUpScreen>d__.<>t__builder.Start<WarningScreens.<StartOptInFollowUpScreen>d__17>(ref <StartOptInFollowUpScreen>d__);
		return <StartOptInFollowUpScreen>d__.<>t__builder.Task;
	}

	private static Task WaitForResponse(CancellationToken cancellationToken)
	{
		WarningScreens.<WaitForResponse>d__18 <WaitForResponse>d__;
		<WaitForResponse>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<WaitForResponse>d__.cancellationToken = cancellationToken;
		<WaitForResponse>d__.<>1__state = -1;
		<WaitForResponse>d__.<>t__builder.Start<WarningScreens.<WaitForResponse>d__18>(ref <WaitForResponse>d__);
		return <WaitForResponse>d__.<>t__builder.Task;
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

	public static void OnLeftButtonClicked()
	{
		WarningScreens._result = WarningScreens._leftButtonResult;
		WarningScreens._closedMessageBox = true;
		WarningScreens activeReference = WarningScreens._activeReference;
		if (activeReference == null)
		{
			return;
		}
		Action onLeftButtonPressedAction = activeReference._onLeftButtonPressedAction;
		if (onLeftButtonPressedAction == null)
		{
			return;
		}
		onLeftButtonPressedAction();
	}

	public static void OnRightButtonClicked()
	{
		WarningScreens._result = WarningScreens._rightButtonResult;
		WarningScreens._closedMessageBox = true;
		WarningScreens activeReference = WarningScreens._activeReference;
		if (activeReference == null)
		{
			return;
		}
		Action onRightButtonPressedAction = activeReference._onRightButtonPressedAction;
		if (onRightButtonPressedAction == null)
		{
			return;
		}
		onRightButtonPressedAction();
	}

	private static WarningScreens _activeReference;

	[SerializeField]
	private MessageBox _messageBox;

	[SerializeField]
	private GameObject _imageContainerAfter;

	[SerializeField]
	private GameObject _imageContainerBefore;

	[SerializeField]
	private TMP_Text _withImageTextBefore;

	[SerializeField]
	private TMP_Text _withImageTextAfter;

	[SerializeField]
	private TMP_Text _noImageText;

	private Action _onLeftButtonPressedAction;

	private Action _onRightButtonPressedAction;

	private static WarningButtonResult _result;

	private static WarningButtonResult _leftButtonResult;

	private static WarningButtonResult _rightButtonResult;

	private static bool _closedMessageBox;
}
