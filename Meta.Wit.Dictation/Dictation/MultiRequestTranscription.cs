using System;
using System.Text;
using Meta.WitAi.Events;
using UnityEngine;
using UnityEngine.Events;

namespace Meta.WitAi.Dictation
{
	public class MultiRequestTranscription : MonoBehaviour
	{
		private void Awake()
		{
			if (!this.witDictation)
			{
				this.witDictation = Object.FindAnyObjectByType<DictationService>();
			}
			this._text = new StringBuilder();
			this._separator = new StringBuilder();
			for (int i = 0; i < this.linesBetweenActivations; i++)
			{
				this._separator.AppendLine();
			}
			if (!string.IsNullOrEmpty(this.activationSeparator))
			{
				this._separator.Append(this.activationSeparator);
			}
		}

		private void OnEnable()
		{
			this.witDictation.DictationEvents.OnFullTranscription.AddListener(new UnityAction<string>(this.OnFullTranscription));
			this.witDictation.DictationEvents.OnPartialTranscription.AddListener(new UnityAction<string>(this.OnPartialTranscription));
			this.witDictation.DictationEvents.OnAborting.AddListener(new UnityAction(this.OnCancelled));
		}

		private void OnDisable()
		{
			this._activeText = string.Empty;
			this.witDictation.DictationEvents.OnFullTranscription.RemoveListener(new UnityAction<string>(this.OnFullTranscription));
			this.witDictation.DictationEvents.OnPartialTranscription.RemoveListener(new UnityAction<string>(this.OnPartialTranscription));
			this.witDictation.DictationEvents.OnAborting.RemoveListener(new UnityAction(this.OnCancelled));
		}

		private void OnCancelled()
		{
			this._activeText = string.Empty;
			this.OnTranscriptionUpdated();
		}

		private void OnFullTranscription(string text)
		{
			this._activeText = string.Empty;
			if (this._text.Length > 0)
			{
				this._text.Append(this._separator);
			}
			this._text.Append(text);
			this.OnTranscriptionUpdated();
		}

		private void OnPartialTranscription(string text)
		{
			this._activeText = text;
			this.OnTranscriptionUpdated();
		}

		public void Clear()
		{
			this._text.Clear();
			this.onTranscriptionUpdated.Invoke(string.Empty);
		}

		private void OnTranscriptionUpdated()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this._text);
			if (!string.IsNullOrEmpty(this._activeText))
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(this._separator);
				}
				if (!string.IsNullOrEmpty(this._activeText))
				{
					stringBuilder.Append(this._activeText);
				}
			}
			this.onTranscriptionUpdated.Invoke(stringBuilder.ToString());
		}

		[SerializeField]
		private DictationService witDictation;

		[SerializeField]
		private int linesBetweenActivations = 2;

		[Multiline]
		[SerializeField]
		private string activationSeparator = string.Empty;

		[Header("Events")]
		[SerializeField]
		private WitTranscriptionEvent onTranscriptionUpdated = new WitTranscriptionEvent();

		private StringBuilder _text;

		private string _activeText;

		private bool _newSection;

		private StringBuilder _separator;
	}
}
