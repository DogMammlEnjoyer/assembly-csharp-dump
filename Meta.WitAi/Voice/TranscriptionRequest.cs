using System;
using Meta.Voice.Logging;
using Meta.WitAi;
using UnityEngine.Events;

namespace Meta.Voice
{
	public abstract class TranscriptionRequest<TUnityEvent, TOptions, TEvents, TResults> : VoiceRequest<TUnityEvent, TOptions, TEvents, TResults> where TUnityEvent : UnityEventBase where TOptions : ITranscriptionRequestOptions where TEvents : TranscriptionRequestEvents<TUnityEvent> where TResults : ITranscriptionRequestResults
	{
		public VoiceAudioInputState AudioInputState { get; private set; }

		public bool IsAudioInputActivated
		{
			get
			{
				return this.AudioInputState == VoiceAudioInputState.Activating || this.AudioInputState == VoiceAudioInputState.On;
			}
		}

		public bool IsListening
		{
			get
			{
				return this.AudioInputState == VoiceAudioInputState.On;
			}
		}

		public bool CanActivateAudio
		{
			get
			{
				return string.IsNullOrEmpty(this.GetActivateAudioError());
			}
		}

		public bool CanDeactivateAudio
		{
			get
			{
				return this.IsAudioInputActivated;
			}
		}

		protected TranscriptionRequest(TOptions newOptions, TEvents newEvents) : base(newOptions, newEvents)
		{
		}

		protected virtual void SetAudioInputState(VoiceAudioInputState newAudioInputState)
		{
			if (this.AudioInputState == newAudioInputState)
			{
				return;
			}
			this.AudioInputState = newAudioInputState;
			TEvents tevents = base.Events;
			this.RaiseEvent((tevents != null) ? tevents.OnAudioInputStateChange : default(TUnityEvent));
			switch (this.AudioInputState)
			{
			case VoiceAudioInputState.Off:
				this.OnStopListening();
				return;
			case VoiceAudioInputState.Activating:
				base.WaitForHold(new Action(this.OnCanActivate));
				return;
			case VoiceAudioInputState.On:
				this.OnStartListening();
				return;
			case VoiceAudioInputState.Deactivating:
				this.OnAudioDeactivation();
				this.HandleAudioDeactivation();
				return;
			default:
				return;
			}
		}

		protected virtual void OnCanActivate()
		{
			if (this.AudioInputState != VoiceAudioInputState.Activating)
			{
				return;
			}
			this.OnAudioActivation();
			this.HandleAudioActivation();
		}

		protected override void Log(string log, VLoggerVerbosity logLevel = VLoggerVerbosity.Info)
		{
			ICoreLogger logger = this.Logger;
			CorrelationID correlationID = this.Logger.CorrelationID;
			string message = "{0}\nRequest Id: {1}\nRequest State: {2}\nAudio Input State: {3}\nTranscription: {4}";
			object[] array = new object[5];
			array[0] = log;
			int num = 1;
			TOptions options = base.Options;
			array[num] = ((options != null) ? options.RequestId : null);
			array[2] = base.State;
			array[3] = this.AudioInputState;
			int num2 = 4;
			TResults results = base.Results;
			array[num2] = ((results != null) ? results.Transcription : null);
			logger.Log(correlationID, logLevel, message, array);
		}

		public string Transcription
		{
			get
			{
				TResults results = base.Results;
				if (results == null)
				{
					return null;
				}
				return results.Transcription;
			}
		}

		public string[] FinalTranscriptions
		{
			get
			{
				TResults results = base.Results;
				if (results == null)
				{
					return null;
				}
				return results.FinalTranscriptions;
			}
		}

		protected virtual void ApplyTranscription(string transcription, bool full)
		{
			TResults results = base.Results;
			results.SetTranscription(transcription, full);
			if (!full)
			{
				this.OnPartialTranscription();
				return;
			}
			this.OnFullTranscription();
		}

		protected virtual void OnPartialTranscription()
		{
			ThreadUtility.CallOnMainThread(delegate()
			{
				TEvents tevents = base.Events;
				if (tevents != null)
				{
					TranscriptionRequestEvent onPartialTranscription = tevents.OnPartialTranscription;
					if (onPartialTranscription != null)
					{
						onPartialTranscription.Invoke(this.Transcription);
					}
				}
				TEvents tevents2 = base.Events;
				if (tevents2 == null)
				{
					return;
				}
				UserTranscriptionRequestEvent onUserPartialTranscription = tevents2.OnUserPartialTranscription;
				if (onUserPartialTranscription == null)
				{
					return;
				}
				TOptions options = base.Options;
				onUserPartialTranscription.Invoke(options.ClientUserId, this.Transcription);
			});
		}

		protected virtual void OnFullTranscription()
		{
			ThreadUtility.CallOnMainThread(delegate()
			{
				TEvents tevents = base.Events;
				if (tevents != null)
				{
					TranscriptionRequestEvent onFullTranscription = tevents.OnFullTranscription;
					if (onFullTranscription != null)
					{
						onFullTranscription.Invoke(this.Transcription);
					}
				}
				TEvents tevents2 = base.Events;
				if (tevents2 == null)
				{
					return;
				}
				UserTranscriptionRequestEvent onUserFullTranscription = tevents2.OnUserFullTranscription;
				if (onUserFullTranscription == null)
				{
					return;
				}
				TOptions options = base.Options;
				onUserFullTranscription.Invoke(options.ClientUserId, this.Transcription);
			});
		}

		protected abstract string GetActivateAudioError();

		public virtual void ActivateAudio()
		{
			if (this.IsAudioInputActivated)
			{
				base.LogW("Activate Audio Ignored\nReason: Already activated");
				return;
			}
			string activateAudioError = this.GetActivateAudioError();
			if (!string.IsNullOrEmpty(activateAudioError))
			{
				base.LogW("Activate Audio Failed\nReason: " + activateAudioError);
				this.HandleFailure(activateAudioError);
				return;
			}
			this.SetAudioInputState(VoiceAudioInputState.Activating);
		}

		protected virtual void OnAudioActivation()
		{
			this.Log("Activate Audio Begin", VLoggerVerbosity.Info);
			TEvents tevents = base.Events;
			this.RaiseEvent((tevents != null) ? tevents.OnAudioActivation : default(TUnityEvent));
		}

		protected abstract void HandleAudioActivation();

		protected virtual void OnStartListening()
		{
			this.Log("Activate Audio Complete", VLoggerVerbosity.Info);
			TEvents tevents = base.Events;
			this.RaiseEvent((tevents != null) ? tevents.OnStartListening : default(TUnityEvent));
		}

		public virtual void DeactivateAudio()
		{
			if (!this.IsAudioInputActivated)
			{
				base.LogW("Deactivate Audio Ignored\nReason: Not currently activated");
				return;
			}
			this.SetAudioInputState(VoiceAudioInputState.Deactivating);
		}

		protected virtual void OnAudioDeactivation()
		{
			this.Log("Deactivate Audio Begin", VLoggerVerbosity.Info);
			TEvents tevents = base.Events;
			this.RaiseEvent((tevents != null) ? tevents.OnAudioDeactivation : default(TUnityEvent));
		}

		protected abstract void HandleAudioDeactivation();

		protected virtual bool HasSentAudio()
		{
			return true;
		}

		protected virtual void OnStopListening()
		{
			this.Log("Deactivate Audio Complete", VLoggerVerbosity.Info);
			TEvents tevents = base.Events;
			this.RaiseEvent((tevents != null) ? tevents.OnStopListening : default(TUnityEvent));
			if (base.State == VoiceRequestState.Initialized)
			{
				this.Cancel("Request cancelled prior to transmission begin");
				return;
			}
			if (base.State == VoiceRequestState.Transmitting && !this.HasSentAudio())
			{
				this.Cancel("Request cancelled prior to audio transmission");
			}
		}

		public override void Send()
		{
			if (!this.IsAudioInputActivated && this.CanActivateAudio)
			{
				this.ActivateAudio();
			}
			base.Send();
		}

		public override void Cancel(string reason = "Request was cancelled.")
		{
			if (this.IsAudioInputActivated)
			{
				this.DeactivateAudio();
			}
			base.Cancel(reason);
		}
	}
}
