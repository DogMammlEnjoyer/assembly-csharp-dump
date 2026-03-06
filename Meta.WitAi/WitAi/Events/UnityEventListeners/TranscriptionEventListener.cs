using System;
using Meta.WitAi.Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace Meta.WitAi.Events.UnityEventListeners
{
	public class TranscriptionEventListener : MonoBehaviour, ITranscriptionEvent
	{
		public WitTranscriptionEvent OnPartialTranscription
		{
			get
			{
				return this.onPartialTranscription;
			}
		}

		public WitTranscriptionEvent OnFullTranscription
		{
			get
			{
				return this.onFullTranscription;
			}
		}

		private ITranscriptionEvent TranscriptionEvents
		{
			get
			{
				if (this._events == null)
				{
					ITranscriptionEventProvider component = base.GetComponent<ITranscriptionEventProvider>();
					if (component != null)
					{
						this._events = component.TranscriptionEvents;
					}
				}
				return this._events;
			}
		}

		private void OnEnable()
		{
			ITranscriptionEvent transcriptionEvents = this.TranscriptionEvents;
			if (transcriptionEvents != null)
			{
				transcriptionEvents.OnPartialTranscription.AddListener(new UnityAction<string>(this.onPartialTranscription.Invoke));
				transcriptionEvents.OnFullTranscription.AddListener(new UnityAction<string>(this.onFullTranscription.Invoke));
			}
		}

		private void OnDisable()
		{
			ITranscriptionEvent transcriptionEvents = this.TranscriptionEvents;
			if (transcriptionEvents != null)
			{
				transcriptionEvents.OnPartialTranscription.RemoveListener(new UnityAction<string>(this.onPartialTranscription.Invoke));
				transcriptionEvents.OnFullTranscription.RemoveListener(new UnityAction<string>(this.onFullTranscription.Invoke));
			}
		}

		[SerializeField]
		private WitTranscriptionEvent onPartialTranscription = new WitTranscriptionEvent();

		[SerializeField]
		private WitTranscriptionEvent onFullTranscription = new WitTranscriptionEvent();

		private ITranscriptionEvent _events;
	}
}
