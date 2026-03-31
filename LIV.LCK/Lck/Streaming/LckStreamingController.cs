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

		public LckStreamingWaitingForConfigureState WaitingForConfigureState { get; private set; } = new LckStreamingWaitingForConfigureState();

		public LckStreamingConfiguredCorrectlyState ConfiguredCorrectlyState { get; private set; } = new LckStreamingConfiguredCorrectlyState();

		public LckInternalErrorState InternalErrorState { get; private set; } = new LckInternalErrorState();

		public LckMissingTrackingIdState MissingTrackingIdState { get; private set; } = new LckMissingTrackingIdState();

		public LckInvalidArgumentState InvalidArgumentState { get; private set; } = new LckInvalidArgumentState();

		public LckRateLimiterBackoffState RateLimiterBackoffState { get; private set; } = new LckRateLimiterBackoffState();

		public LckServiceUnavailableState ServiceUnavailableState { get; private set; } = new LckServiceUnavailableState();

		public CancellationTokenSource CancellationTokenSource { get; private set; } = new CancellationTokenSource();

		private void Start()
		{
			if (this._lckService != null)
			{
				this._lckService.OnStreamingStarted += this.OnStreamingStarted;
			}
			LckMonoBehaviourMediator.OnApplicationLifecycleEvent += this.OnApplicationLifecycle;
		}

		private void OnApplicationLifecycle(LckMonoBehaviourMediator.ApplicationLifecycleEventType eventType)
		{
			switch (eventType)
			{
			case LckMonoBehaviourMediator.ApplicationLifecycleEventType.Pause:
				this.OnSystemPaused();
				return;
			case LckMonoBehaviourMediator.ApplicationLifecycleEventType.Resume:
				this.OnSystemResumed();
				return;
			case LckMonoBehaviourMediator.ApplicationLifecycleEventType.HMDIdle:
				this.OnHMDIdle();
				return;
			case LckMonoBehaviourMediator.ApplicationLifecycleEventType.HMDActive:
				this.OnHMDActive();
				return;
			default:
				return;
			}
		}

		private void OnSystemPaused()
		{
			this.Log("[LCK Streaming Controller] System paused - stopping any active polling");
			this.StopCheckingStates();
			if (this._lckService != null && this._lckService.IsStreaming().Result)
			{
				this.Log("[LCK Streaming Controller] Stopping streaming due to system pause");
				this._lckService.StopStreaming();
				this._notificationController.ShowNotification(NotificationType.InternalError);
			}
		}

		private void OnSystemResumed()
		{
			this.Log("[LCK Streaming Controller] System resumed");
			if (this._lckService != null)
			{
				bool result = this._lckService.IsStreaming().Result;
				this.Log(string.Format("[LCK Streaming Controller] Post-resume streaming state: {0}", result));
			}
			if (this.IsConfiguredCorrectly)
			{
				this.CheckCurrentState();
			}
		}

		private void OnHMDIdle()
		{
			this.Log("[LCK Streaming Controller] HMD idle detected - stopping polling but keeping stream active");
			this.StopCheckingStates();
		}

		private void OnHMDActive()
		{
			this.Log("[LCK Streaming Controller] HMD active again");
			if (this.IsConfiguredCorrectly)
			{
				this.CheckCurrentState();
			}
		}

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
				this.Log(string.Concat(new string[]
				{
					"[LCK Streaming Controller] tried switching to the same state! Current State: <color=#FF0000>",
					this.CurrentState.GetType().Name,
					"</color> to: <color=#FF0000>",
					state.GetType().Name,
					"</color>"
				}));
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
			LckStreamingController.<StartStreamIfNoLivHubChanges>d__74 <StartStreamIfNoLivHubChanges>d__;
			<StartStreamIfNoLivHubChanges>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<StartStreamIfNoLivHubChanges>d__.<>4__this = this;
			<StartStreamIfNoLivHubChanges>d__.<>1__state = -1;
			<StartStreamIfNoLivHubChanges>d__.<>t__builder.Start<LckStreamingController.<StartStreamIfNoLivHubChanges>d__74>(ref <StartStreamIfNoLivHubChanges>d__);
			return <StartStreamIfNoLivHubChanges>d__.<>t__builder.Task;
		}

		public void StopStreaming()
		{
			if (this._lckService == null)
			{
				LckLog.LogWarning("LCK Could not get Service", "StopStreaming", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\Streaming\\LckStreamingController.cs", 290);
				return;
			}
			if (this._lckService.IsStreaming().Result)
			{
				this._lckService.StopStreaming();
			}
		}

		private void OnStreamingStarted(LckResult result)
		{
			if (!result.Success)
			{
				this._notificationController.ShowNotification(NotificationType.UnknownStreamingError);
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

		public void ToggleCameraPage()
		{
			this._topButtonsController.ToggleCameraPage(true);
			this._topButtonsController.SetCameraPageVisualsManually();
		}

		private void OnValidate()
		{
			if (this._topButtonsControllerGameObject && !this._topButtonsControllerGameObject.activeSelf)
			{
				this._topButtonsControllerGameObject.SetActive(true);
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
			LckMonoBehaviourMediator.OnApplicationLifecycleEvent -= this.OnApplicationLifecycle;
			if (this._lckService != null)
			{
				if (this._lckService.IsStreaming().Result)
				{
					this._lckService.StopStreaming();
				}
				this._lckService.OnStreamingStarted -= this.OnStreamingStarted;
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

		[Tooltip("Reference to the Top Buttons Controller which handles switching to Camera or Stream modes on the tablet UI")]
		[SerializeField]
		private LckTopButtonsController _topButtonsController;

		[Tooltip("This event is invoked when the user presses the stream button but the setup is not yet complete. Use this to trigger visual feedback, like a button shake or an error icon.")]
		[SerializeField]
		private UnityEvent _onStreamButtonError;

		[Tooltip("This event is invoked when the user presses the stream button and the setup is complete, just before streaming starts. Use this to trigger positive feedback, like a button color change.")]
		[SerializeField]
		private UnityEvent _onStreamButtonPressWithCorrectConfig;

		[Header("Game Objects disabled when streaming package removed")]
		[SerializeField]
		private GameObject _topButtonsControllerGameObject;

		[SerializeField]
		private GameObject _livHubButton;
	}
}
