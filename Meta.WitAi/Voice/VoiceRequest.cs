using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Lib.Wit.Runtime.Utilities.Logging;
using Meta.Voice.Logging;
using Meta.Voice.TelemetryUtilities;
using Meta.WitAi;
using Meta.WitAi.Data;
using UnityEngine.Events;

namespace Meta.Voice
{
	[LogCategory(LogCategory.Requests)]
	public abstract class VoiceRequest<TUnityEvent, TOptions, TEvents, TResults> : ILogSource where TUnityEvent : UnityEventBase where TOptions : IVoiceRequestOptions where TEvents : VoiceRequestEvents<TUnityEvent> where TResults : IVoiceRequestResults
	{
		public virtual IVLogger Logger { get; } = LoggerRegistry.Instance.GetLogger(LogCategory.Requests, null);

		public VoiceRequestState State { get; private set; } = (VoiceRequestState)(-1);

		public bool IsActive
		{
			get
			{
				return this.State == (VoiceRequestState)(-1) || this.State == VoiceRequestState.Initialized || this.State == VoiceRequestState.Transmitting;
			}
		}

		public TaskCompletionSource<bool> Completion { get; private set; } = new TaskCompletionSource<bool>();

		public Task HoldTask { get; set; }

		public float DownloadProgress { get; private set; }

		public float UploadProgress { get; private set; }

		public TOptions Options { get; }

		public TEvents Events { get; }

		public TResults Results { get; }

		public bool CanSend
		{
			get
			{
				return string.IsNullOrEmpty(this.GetSendError());
			}
		}

		public VoiceRequest(TOptions newOptions, TEvents newEvents)
		{
			this.Options = ((newOptions != null) ? newOptions : Activator.CreateInstance<TOptions>());
			this.Events = Activator.CreateInstance<TEvents>();
			if (newEvents != null)
			{
				this.AddEventListeners(newEvents);
			}
			this.Results = this.GetNewResults();
			this.SetState(VoiceRequestState.Initialized);
		}

		protected virtual TResults GetNewResults()
		{
			return Activator.CreateInstance<TResults>();
		}

		public void AddEventListeners(TEvents newEvents)
		{
			this.SetEventListeners(newEvents, true);
		}

		public void RemoveEventListeners(TEvents newEvents)
		{
			this.SetEventListeners(newEvents, false);
		}

		protected abstract void SetEventListeners(TEvents newEvents, bool addListeners);

		protected abstract void RaiseEvent(TUnityEvent requestEvent);

		protected virtual void OnInit()
		{
			TEvents tevents = this.Events;
			this.RaiseEvent((tevents != null) ? tevents.OnInit : default(TUnityEvent));
			this.SetUploadProgress(0f);
			this.SetDownloadProgress(0f);
		}

		protected virtual void SetState(VoiceRequestState newState)
		{
			if (this.State == newState)
			{
				return;
			}
			this.State = newState;
			this.OnStateChange();
			bool flag = false;
			switch (this.State)
			{
			case VoiceRequestState.Initialized:
				try
				{
					this.OnInit();
					goto IL_FC;
				}
				catch (Exception e)
				{
					this.LogE("OnInit Exception Caught", e);
					goto IL_FC;
				}
				break;
			case VoiceRequestState.Transmitting:
				break;
			case VoiceRequestState.Canceled:
				try
				{
					this.HandleCancel();
				}
				catch (Exception e2)
				{
					this.LogE("HandleCancel Exception Caught", e2);
				}
				try
				{
					this.OnCancel();
				}
				catch (Exception e3)
				{
					this.LogE("OnCancel Exception Caught", e3);
				}
				flag = true;
				goto IL_FC;
			case VoiceRequestState.Failed:
				try
				{
					this.OnFailed();
				}
				catch (Exception e4)
				{
					this.LogE("OnFailed Exception Caught", e4);
				}
				flag = true;
				goto IL_FC;
			case VoiceRequestState.Successful:
				try
				{
					this.OnSuccess();
				}
				catch (Exception e5)
				{
					this.LogE("OnSuccess Exception Caught", e5);
				}
				flag = true;
				goto IL_FC;
			default:
				goto IL_FC;
			}
			try
			{
				this.OnSend();
			}
			catch (Exception e6)
			{
				this.LogE("OnSend Exception Caught", e6);
			}
			this.WaitForHold(new Action(this.HoldSend));
			IL_FC:
			if (flag)
			{
				try
				{
					this.OnComplete();
				}
				catch (Exception e7)
				{
					this.LogE("OnComplete Exception Caught", e7);
				}
			}
		}

		protected virtual void OnStateChange()
		{
			TEvents tevents = this.Events;
			this.RaiseEvent((tevents != null) ? tevents.OnStateChange : default(TUnityEvent));
		}

		protected void WaitForHold(Action onReady)
		{
			VoiceRequest<TUnityEvent, TOptions, TEvents, TResults>.<>c__DisplayClass46_0 CS$<>8__locals1 = new VoiceRequest<TUnityEvent, TOptions, TEvents, TResults>.<>c__DisplayClass46_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.onReady = onReady;
			ThreadUtility.BackgroundAsync(this.Logger, delegate()
			{
				VoiceRequest<TUnityEvent, TOptions, TEvents, TResults>.<>c__DisplayClass46_0.<<WaitForHold>b__0>d <<WaitForHold>b__0>d;
				<<WaitForHold>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<WaitForHold>b__0>d.<>4__this = CS$<>8__locals1;
				<<WaitForHold>b__0>d.<>1__state = -1;
				<<WaitForHold>b__0>d.<>t__builder.Start<VoiceRequest<TUnityEvent, TOptions, TEvents, TResults>.<>c__DisplayClass46_0.<<WaitForHold>b__0>d>(ref <<WaitForHold>b__0>d);
				return <<WaitForHold>b__0>d.<>t__builder.Task;
			}).WrapErrors();
		}

		protected virtual void HoldSend()
		{
			if (this.State != VoiceRequestState.Transmitting || this.OnSimulateResponse())
			{
				return;
			}
			this.HandleSend();
		}

		protected void SetDownloadProgress(float newProgress)
		{
			if (this.DownloadProgress.Equals(newProgress))
			{
				return;
			}
			this.DownloadProgress = newProgress;
			TEvents tevents = this.Events;
			this.RaiseEvent((tevents != null) ? tevents.OnDownloadProgressChange : default(TUnityEvent));
		}

		protected void SetUploadProgress(float newProgress)
		{
			if (this.UploadProgress.Equals(newProgress))
			{
				return;
			}
			this.UploadProgress = newProgress;
			TEvents tevents = this.Events;
			this.RaiseEvent((tevents != null) ? tevents.OnUploadProgressChange : default(TUnityEvent));
		}

		protected virtual void Log(string log, VLoggerVerbosity logLevel = VLoggerVerbosity.Info)
		{
			ICoreLogger logger = this.Logger;
			CorrelationID correlationID = this.Logger.CorrelationID;
			string message = "{0}\nRequest Id: {1}\nRequest State: {2}";
			object[] array = new object[3];
			array[0] = log;
			int num = 1;
			TOptions options = this.Options;
			array[num] = ((options != null) ? options.RequestId : null);
			array[2] = this.State;
			logger.Log(correlationID, logLevel, message, array);
		}

		protected void LogW(string log)
		{
			this.Log(log, VLoggerVerbosity.Warning);
		}

		protected void LogE(string log, Exception e)
		{
			this.Log(string.Format("{0}\n\n{1}", log, e), VLoggerVerbosity.Error);
		}

		protected virtual string GetSendError()
		{
			if (this.State != VoiceRequestState.Initialized)
			{
				return string.Format("Cannot send request in '{0}' state.", this.State);
			}
			TOptions options = this.Options;
			if (string.IsNullOrEmpty((options != null) ? options.RequestId : null))
			{
				return "Cannot send request without a request id.";
			}
			return string.Empty;
		}

		public virtual void Send()
		{
			if (this.State != VoiceRequestState.Initialized)
			{
				this.LogW("Request Send Ignored\nReason: Invalid state");
				return;
			}
			string sendError = this.GetSendError();
			if (!string.IsNullOrEmpty(sendError))
			{
				this.HandleFailure(sendError);
				return;
			}
			this.SetState(VoiceRequestState.Transmitting);
		}

		protected virtual void OnSend()
		{
			this.Log("Request Transmitting", VLoggerVerbosity.Info);
			TEvents tevents = this.Events;
			this.RaiseEvent((tevents != null) ? tevents.OnSend : default(TUnityEvent));
		}

		protected abstract void HandleSend();

		protected virtual bool OnSimulateResponse()
		{
			return false;
		}

		protected virtual void HandleFailure(string error)
		{
			this.HandleFailure(-1, error);
		}

		protected virtual void HandleFailure(int errorStatusCode, string errorMessage)
		{
			if (!this.IsActive)
			{
				this.LogW("Request Failure Ignored\nReason: Request is already complete");
				return;
			}
			if (string.Equals("Cancelled", errorMessage))
			{
				this.Cancel("Request was cancelled.");
				return;
			}
			if (this.ShouldIgnoreError(errorStatusCode, errorMessage))
			{
				this.HandleSuccess();
				return;
			}
			TResults results = this.Results;
			results.SetError(errorStatusCode, errorMessage);
			this.SetState(VoiceRequestState.Failed);
		}

		protected virtual bool ShouldIgnoreError(int errorStatusCode, string errorMessage)
		{
			return string.IsNullOrEmpty(errorMessage);
		}

		protected virtual void OnFailed()
		{
			TEvents tevents = this.Events;
			this.RaiseEvent((tevents != null) ? tevents.OnFailed : default(TUnityEvent));
		}

		protected virtual void HandleSuccess()
		{
			if (!this.IsActive)
			{
				this.LogW("Request Success Ignored\nReason: Request is already complete");
				return;
			}
			this.SetState(VoiceRequestState.Successful);
		}

		protected virtual void OnSuccess()
		{
			this.Log(string.Format("Request Success\nResults: {0}", this.Results != null), VLoggerVerbosity.Info);
			TEvents tevents = this.Events;
			this.RaiseEvent((tevents != null) ? tevents.OnSuccess : default(TUnityEvent));
		}

		public virtual void Cancel(string reason = "Request was cancelled.")
		{
			if (!this.IsActive)
			{
				this.LogW("Request Cancel Ignored\nReason: Request is already complete");
				return;
			}
			TResults results = this.Results;
			results.SetCancel(reason);
			this.SetState(VoiceRequestState.Canceled);
		}

		protected abstract void HandleCancel();

		protected virtual void OnCancel()
		{
			TEvents tevents = this.Events;
			this.RaiseEvent((tevents != null) ? tevents.OnCancel : default(TUnityEvent));
		}

		protected virtual void OnComplete()
		{
			this.Completion.SetResult(this.State != VoiceRequestState.Failed);
			TEvents tevents = this.Events;
			this.RaiseEvent((tevents != null) ? tevents.OnComplete : default(TUnityEvent));
			switch (this.State)
			{
			case VoiceRequestState.Canceled:
			{
				RuntimeTelemetry instance = RuntimeTelemetry.Instance;
				TOptions options = this.Options;
				instance.LogEventTermination((OperationID)options.OperationId, TerminationReason.Canceled, "");
				return;
			}
			case VoiceRequestState.Failed:
			{
				RuntimeTelemetry instance2 = RuntimeTelemetry.Instance;
				TOptions options = this.Options;
				instance2.LogEventTermination((OperationID)options.OperationId, TerminationReason.Failed, "");
				return;
			}
			case VoiceRequestState.Successful:
				break;
			default:
			{
				RuntimeTelemetry instance3 = RuntimeTelemetry.Instance;
				TOptions options = this.Options;
				instance3.LogEventTermination((OperationID)options.OperationId, TerminationReason.Undetermined, "");
				break;
			}
			}
		}

		protected void MainThreadCallback(Action action)
		{
			ThreadUtility.CallOnMainThread(action);
		}

		public static SimulatedResponse simulatedResponse;
	}
}
