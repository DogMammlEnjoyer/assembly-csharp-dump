using System;
using Meta.WitAi.Events;
using UnityEngine;
using UnityEngine.Events;

namespace Meta.WitAi.Interfaces
{
	public abstract class CustomTranscriptionProvider : MonoBehaviour, ITranscriptionProvider
	{
		public string LastTranscription { get; }

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

		public UnityEvent OnStoppedListening
		{
			get
			{
				return this.onStoppedListening;
			}
		}

		public UnityEvent OnStartListening
		{
			get
			{
				return this.onStartListening;
			}
		}

		public WitMicLevelChangedEvent OnMicLevelChanged
		{
			get
			{
				return this.onMicLevelChanged;
			}
		}

		public bool OverrideMicLevel
		{
			get
			{
				return this.overrideMicLevel;
			}
		}

		public abstract void Activate();

		public abstract void Deactivate();

		[SerializeField]
		private bool overrideMicLevel;

		private WitTranscriptionEvent onPartialTranscription = new WitTranscriptionEvent();

		private WitTranscriptionEvent onFullTranscription = new WitTranscriptionEvent();

		private UnityEvent onStoppedListening = new UnityEvent();

		private UnityEvent onStartListening = new UnityEvent();

		private WitMicLevelChangedEvent onMicLevelChanged = new WitMicLevelChangedEvent();
	}
}
