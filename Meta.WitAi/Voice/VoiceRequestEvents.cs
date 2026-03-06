using System;
using UnityEngine;
using UnityEngine.Events;

namespace Meta.Voice
{
	[Serializable]
	public class VoiceRequestEvents<TUnityEvent> where TUnityEvent : UnityEventBase
	{
		public TUnityEvent OnStateChange
		{
			get
			{
				return this._onStateChange;
			}
		}

		public TUnityEvent OnInit
		{
			get
			{
				return this._onInit;
			}
		}

		public TUnityEvent OnSend
		{
			get
			{
				return this._onSend;
			}
		}

		public TUnityEvent OnCancel
		{
			get
			{
				return this._onCancel;
			}
		}

		public TUnityEvent OnFailed
		{
			get
			{
				return this._onFailed;
			}
		}

		public TUnityEvent OnSuccess
		{
			get
			{
				return this._onSuccess;
			}
		}

		public TUnityEvent OnComplete
		{
			get
			{
				return this._onComplete;
			}
		}

		public TUnityEvent OnDownloadProgressChange
		{
			get
			{
				return this._onDownloadProgressChange;
			}
		}

		public TUnityEvent OnUploadProgressChange
		{
			get
			{
				return this._onUploadProgressChange;
			}
		}

		[Header("State Events")]
		[Tooltip("Called whenever a request state changes.")]
		[SerializeField]
		private TUnityEvent _onStateChange = Activator.CreateInstance<TUnityEvent>();

		[Tooltip("Called on initial request generation.")]
		[SerializeField]
		private TUnityEvent _onInit = Activator.CreateInstance<TUnityEvent>();

		[Tooltip("Called following the start of data transmission.")]
		[SerializeField]
		private TUnityEvent _onSend = Activator.CreateInstance<TUnityEvent>();

		[Tooltip("Called following the cancellation of a request.")]
		[SerializeField]
		private TUnityEvent _onCancel = Activator.CreateInstance<TUnityEvent>();

		[Tooltip("Called following an error response from a request.")]
		[SerializeField]
		private TUnityEvent _onFailed = Activator.CreateInstance<TUnityEvent>();

		[Tooltip("Called following a successful request & data parse with results provided.")]
		[SerializeField]
		private TUnityEvent _onSuccess = Activator.CreateInstance<TUnityEvent>();

		[Tooltip("Called following cancellation, failure or success to finalize request.")]
		[SerializeField]
		private TUnityEvent _onComplete = Activator.CreateInstance<TUnityEvent>();

		[Header("Progress Events")]
		[Tooltip("Called on download progress update.")]
		[SerializeField]
		private TUnityEvent _onDownloadProgressChange = Activator.CreateInstance<TUnityEvent>();

		[Tooltip("Called on upload progress update.")]
		[SerializeField]
		private TUnityEvent _onUploadProgressChange = Activator.CreateInstance<TUnityEvent>();
	}
}
