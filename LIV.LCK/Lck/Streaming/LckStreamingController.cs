using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Liv.Lck.Core;
using Liv.Lck.Core.Cosmetics;
using Liv.Lck.DependencyInjection;
using Liv.Lck.Tablet;
using UnityEngine;
using UnityEngine.Events;

namespace Liv.Lck.Streaming
{
	public class LckStreamingController : MonoBehaviour
	{
		[InjectLck]
		public ILckCore LckCore { get; private set; }

		[InjectLck]
		public ILckCosmeticsCoordinator LckCosmeticsCoordinator { get; private set; }

		public bool IsConfiguredCorrectly { get; private set; }

		public LckStreamingBaseState CurrentState { get; private set; }

		public LckStreamingGetCurrentState GetCurrentState { get; private set; } = new LckStreamingGetCurrentState();

		public LckStreamingShowCodeState ShowCodeState { get; private set; } = new LckStreamingShowCodeState();

		public LckStreamingCheckSubscribedState CheckSubscribedState { get; private set; } = new LckStreamingCheckSubscribedState();

		public LckStreamingWaitingForConfigureState WaitingForConfigureState { get; private set; } = new LckStreamingWaitingForConfigureState();

		public LckStreamingConfiguredCorrectlyState ConfiguredCorrectlyState { get; private set; } = new LckStreamingConfiguredCorrectlyState();

		public LckInternalErrorState InternalErrorState { get; private set; } = new LckInternalErrorState();

		public LckMissingTrackingIdState MissingTrackingIdState { get; private set; } = new LckMissingTrackingIdState();

		public LckInvalidArgumentState InvalidArgumentState { get; private set; } = new LckInvalidArgumentState();

		public CancellationTokenSource CancellationTokenSource { get; private set; } = new CancellationTokenSource();

		public void CheckCurrentState()
		{
			this.SwitchState(this.GetCurrentState);
		}

		public void StopCheckingStates()
		{
			this.CancellationTokenSource.Cancel();
			this.CancellationTokenSource.Dispose();
			this.CancellationTokenSource = new CancellationTokenSource();
		}

		public void SwitchState(LckStreamingBaseState state)
		{
			if (this.CurrentState == state)
			{
				this.Log("[LCK Streaming Controller] tried switching to the same state!");
				return;
			}
			this.CancellationTokenSource.Cancel();
			this.CancellationTokenSource.Dispose();
			this.CancellationTokenSource = new CancellationTokenSource();
			this.Log((this.CurrentState != null) ? string.Concat(new string[]
			{
				"[LCK Streaming Controller] changing states from: <color=#42f542>",
				this.CurrentState.GetType().Name,
				"</color> to: <color=#42f542>",
				state.GetType().Name,
				"</color>"
			}) : ("[LCK Streaming Controller] changing states from: <color=#42f542>null</color> to: <color=#42f542>" + state.GetType().Name + "</color>"));
			this.CurrentState = state;
			this.IsConfiguredCorrectly = (this.CurrentState is LckStreamingConfiguredCorrectlyState);
			this.CurrentState.EnterState(this);
		}

		public void StartStreaming()
		{
			if (!this.IsConfiguredCorrectly)
			{
				this._onStreamButtonError.Invoke();
				return;
			}
			this.StartStreamIfNoLivHubChanges();
		}

		private Task StartStreamIfNoLivHubChanges()
		{
			LckStreamingController.<StartStreamIfNoLivHubChanges>d__63 <StartStreamIfNoLivHubChanges>d__;
			<StartStreamIfNoLivHubChanges>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<StartStreamIfNoLivHubChanges>d__.<>4__this = this;
			<StartStreamIfNoLivHubChanges>d__.<>1__state = -1;
			<StartStreamIfNoLivHubChanges>d__.<>t__builder.Start<LckStreamingController.<StartStreamIfNoLivHubChanges>d__63>(ref <StartStreamIfNoLivHubChanges>d__);
			return <StartStreamIfNoLivHubChanges>d__.<>t__builder.Task;
		}

		public void StopStreaming()
		{
			if (this._lckService == null)
			{
				LckLog.LogWarning("LCK Could not get Service");
				return;
			}
			if (this._lckService.IsStreaming().Result)
			{
				this._lckService.StopStreaming();
			}
		}

		public void GoToErrorState()
		{
			this.SwitchState(this.InternalErrorState);
		}

		public void LogError(string error)
		{
			if (this._showDebugLogs)
			{
				Debug.LogError("[LCK Streaming Controller] " + error);
			}
		}

		public void Log(string message)
		{
			if (this._showDebugLogs)
			{
				Debug.Log("[LCK Streaming Controller] " + message);
			}
		}

		public void ShowNotification(NotificationType type)
		{
			this._notificationController.ShowNotification(type);
		}

		public void HideNotifications()
		{
			this._notificationController.HideNotifications();
		}

		public void SetNotificationStreamCode(string code)
		{
			this._notificationController.SetNotificationStreamCode(code);
		}

		private void OnValidate()
		{
			if (this._topButtonsController && !this._topButtonsController.activeSelf)
			{
				this._topButtonsController.SetActive(true);
			}
			if (this._livHubButton && !this._livHubButton.activeSelf)
			{
				this._livHubButton.SetActive(true);
			}
		}

		private void OnDestroy()
		{
			this.CancellationTokenSource.Cancel();
			this.CancellationTokenSource.Dispose();
			if (this._lckService != null && this._lckService.IsStreaming().Result)
			{
				this._lckService.StopStreaming();
			}
		}

		[Tooltip("Enable this to see detailed logs from this controller in the Unity console. Recommended for development.")]
		[SerializeField]
		private bool _showDebugLogs;

		[InjectLck]
		private ILckService _lckService;

		[Tooltip("A reference to the controller responsible for displaying UI notifications (e.g., 'Enter this code:', 'Please subscribe'). Assign this in the Inspector.")]
		[SerializeField]
		private LckNotificationController _notificationController;

		[Tooltip("This event is invoked when the user presses the stream button but the setup is not yet complete. Use this to trigger visual feedback, like a button shake or an error icon.")]
		[SerializeField]
		private UnityEvent _onStreamButtonError;

		[Tooltip("This event is invoked when the user presses the stream button and the setup is complete, just before streaming starts. Use this to trigger positive feedback, like a button color change.")]
		[SerializeField]
		private UnityEvent _onStreamButtonPressWithCorrectConfig;

		[Header("Game Objects disabled when streaming package removed")]
		[SerializeField]
		private GameObject _topButtonsController;

		[SerializeField]
		private GameObject _livHubButton;
	}
}
