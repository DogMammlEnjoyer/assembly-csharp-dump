using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace Liv.Lck.ErrorHandling
{
	internal class MainThreadCaptureErrorDispatcher : ILckCaptureErrorDispatcher, IDisposable
	{
		[Preserve]
		public MainThreadCaptureErrorDispatcher(ILckEventBus eventBus)
		{
			this._eventBus = eventBus;
			this._eventBus.AddListener<LckEvents.EncoderStartedEvent>(new Action<LckEvents.EncoderStartedEvent>(this.OnEncoderStarted));
			this._eventBus.AddListener<LckEvents.EncoderStoppedEvent>(new Action<LckEvents.EncoderStoppedEvent>(this.OnEncoderStopped));
		}

		public void PushError(LckCaptureError error)
		{
			LckLog.LogWarning("Capture error occurred: " + error.Message, "PushError", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\ErrorHandling\\MainThreadCaptureErrorDispatcher.cs", 37);
			this._errorQueue.Enqueue(error);
		}

		private void OnEncoderStarted(LckEvents.EncoderStartedEvent encoderStartedEvent)
		{
			if (encoderStartedEvent.Result.Success)
			{
				this.StartMonitoringErrors();
			}
		}

		private void OnEncoderStopped(LckEvents.EncoderStoppedEvent encoderStoppedEvent)
		{
			this.StopMonitoringErrors();
		}

		private void StartMonitoringErrors()
		{
			this._isMonitoringErrors = true;
			LckMonoBehaviourMediator.StartCoroutine(MainThreadCaptureErrorDispatcher._updateCoroutineName, this.Update());
		}

		private void StopMonitoringErrors()
		{
			this._isMonitoringErrors = false;
			LckMonoBehaviourMediator.StopCoroutineByName(MainThreadCaptureErrorDispatcher._updateCoroutineName);
		}

		private IEnumerable<LckCaptureError> DrainErrors()
		{
			LckCaptureError lckCaptureError;
			while (this._errorQueue.TryDequeue(out lckCaptureError))
			{
				yield return lckCaptureError;
			}
			yield break;
		}

		private IEnumerator Update()
		{
			while (this._isMonitoringErrors)
			{
				foreach (LckCaptureError error in this.DrainErrors())
				{
					this._eventBus.Trigger<LckEvents.CaptureErrorEvent>(new LckEvents.CaptureErrorEvent(error));
				}
				yield return null;
			}
			yield break;
		}

		public void Dispose()
		{
			this._eventBus.RemoveListener<LckEvents.EncoderStartedEvent>(new Action<LckEvents.EncoderStartedEvent>(this.OnEncoderStarted));
			this._eventBus.RemoveListener<LckEvents.EncoderStoppedEvent>(new Action<LckEvents.EncoderStoppedEvent>(this.OnEncoderStopped));
			if (this._isMonitoringErrors)
			{
				LckMonoBehaviourMediator.StopCoroutineByName(MainThreadCaptureErrorDispatcher._updateCoroutineName);
			}
		}

		private static readonly string _updateCoroutineName = "MainThreadCaptureErrorDispatcher:Update";

		private readonly ILckEventBus _eventBus;

		private readonly ConcurrentQueue<LckCaptureError> _errorQueue = new ConcurrentQueue<LckCaptureError>();

		private bool _isMonitoringErrors;
	}
}
