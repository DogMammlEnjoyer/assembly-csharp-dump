using System;
using Meta.WitAi.TTS.Data;
using Meta.WitAi.TTS.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace Meta.WitAi.TTS.UX
{
	public class TTSSpeakerObserver : MonoBehaviour
	{
		public TTSSpeaker Speaker
		{
			get
			{
				return this._speaker;
			}
		}

		protected virtual void Awake()
		{
			if (this._speaker == null)
			{
				this._speaker = base.gameObject.GetComponentInChildren<TTSSpeaker>();
			}
		}

		protected virtual void OnEnable()
		{
			if (this._speaker == null)
			{
				return;
			}
			this._speaker.Events.OnPlaybackQueueBegin.AddListener(new UnityAction(this.OnPlaybackQueueBegin));
			this._speaker.Events.OnPlaybackQueueComplete.AddListener(new UnityAction(this.OnPlaybackQueueComplete));
			this._speaker.Events.OnLoadBegin.AddListener(new UnityAction<TTSSpeaker, TTSClipData>(this.OnLoadBegin));
			this._speaker.Events.OnLoadAbort.AddListener(new UnityAction<TTSSpeaker, TTSClipData>(this.OnLoadAbort));
			this._speaker.Events.OnLoadFailed.AddListener(new UnityAction<TTSSpeaker, TTSClipData, string>(this.OnLoadFailed));
			this._speaker.Events.OnLoadSuccess.AddListener(new UnityAction<TTSSpeaker, TTSClipData>(this.OnLoadSuccess));
			this._speaker.Events.OnPlaybackReady.AddListener(new UnityAction<TTSSpeaker, TTSClipData>(this.OnPlaybackReady));
			this._speaker.Events.OnPlaybackStart.AddListener(new UnityAction<TTSSpeaker, TTSClipData>(this.OnPlaybackStart));
			this._speaker.Events.OnPlaybackCancelled.AddListener(new UnityAction<TTSSpeaker, TTSClipData, string>(this.OnPlaybackCancelled));
			this._speaker.Events.OnPlaybackComplete.AddListener(new UnityAction<TTSSpeaker, TTSClipData>(this.OnPlaybackComplete));
		}

		protected virtual void OnDisable()
		{
			if (this._speaker == null)
			{
				return;
			}
			this._speaker.Events.OnPlaybackQueueBegin.RemoveListener(new UnityAction(this.OnPlaybackQueueBegin));
			this._speaker.Events.OnPlaybackQueueComplete.RemoveListener(new UnityAction(this.OnPlaybackQueueComplete));
			this._speaker.Events.OnLoadBegin.RemoveListener(new UnityAction<TTSSpeaker, TTSClipData>(this.OnLoadBegin));
			this._speaker.Events.OnLoadAbort.RemoveListener(new UnityAction<TTSSpeaker, TTSClipData>(this.OnLoadAbort));
			this._speaker.Events.OnLoadFailed.RemoveListener(new UnityAction<TTSSpeaker, TTSClipData, string>(this.OnLoadFailed));
			this._speaker.Events.OnLoadSuccess.RemoveListener(new UnityAction<TTSSpeaker, TTSClipData>(this.OnLoadSuccess));
			this._speaker.Events.OnPlaybackReady.RemoveListener(new UnityAction<TTSSpeaker, TTSClipData>(this.OnPlaybackReady));
			this._speaker.Events.OnPlaybackStart.RemoveListener(new UnityAction<TTSSpeaker, TTSClipData>(this.OnPlaybackStart));
			this._speaker.Events.OnPlaybackCancelled.RemoveListener(new UnityAction<TTSSpeaker, TTSClipData, string>(this.OnPlaybackCancelled));
			this._speaker.Events.OnPlaybackComplete.RemoveListener(new UnityAction<TTSSpeaker, TTSClipData>(this.OnPlaybackComplete));
		}

		protected virtual void OnPlaybackQueueBegin()
		{
		}

		protected virtual void OnPlaybackQueueComplete()
		{
		}

		protected virtual void OnLoadBegin(TTSSpeaker speaker, TTSClipData clipData)
		{
		}

		protected virtual void OnLoadAbort(TTSSpeaker speaker, TTSClipData clipData)
		{
		}

		protected virtual void OnLoadFailed(TTSSpeaker speaker, TTSClipData clipData, string error)
		{
		}

		protected virtual void OnLoadSuccess(TTSSpeaker speaker, TTSClipData clipData)
		{
		}

		protected virtual void OnPlaybackReady(TTSSpeaker speaker, TTSClipData clipData)
		{
		}

		protected virtual void OnPlaybackStart(TTSSpeaker speaker, TTSClipData clipData)
		{
		}

		protected virtual void OnPlaybackCancelled(TTSSpeaker speaker, TTSClipData clipData, string reason)
		{
		}

		protected virtual void OnPlaybackComplete(TTSSpeaker speaker, TTSClipData clipData)
		{
		}

		[Header("Speaker Settings")]
		[SerializeField]
		[Tooltip("TTSSpeaker being observed, if left empty it will grab the speaker from the GameObject")]
		private TTSSpeaker _speaker;
	}
}
