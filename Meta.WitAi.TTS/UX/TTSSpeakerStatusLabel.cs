using System;
using System.Collections;
using System.Text;
using Meta.WitAi.TTS.Data;
using Meta.WitAi.TTS.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Meta.WitAi.TTS.UX
{
	public class TTSSpeakerStatusLabel : TTSSpeakerObserver
	{
		protected override void OnEnable()
		{
			base.OnEnable();
			this.RefreshLabel();
			this._refreshUpdater = base.StartCoroutine(this.RefreshUpdater());
		}

		protected override void OnDisable()
		{
			if (this._refreshUpdater != null)
			{
				base.StopCoroutine(this._refreshUpdater);
				this._refreshUpdater = null;
			}
		}

		protected override void OnLoadBegin(TTSSpeaker speaker, TTSClipData clipData)
		{
			this._needsRefresh = true;
		}

		protected override void OnLoadAbort(TTSSpeaker speaker, TTSClipData clipData)
		{
			this._needsRefresh = true;
		}

		protected override void OnLoadFailed(TTSSpeaker speaker, TTSClipData clipData, string error)
		{
			this._needsRefresh = true;
		}

		protected override void OnLoadSuccess(TTSSpeaker speaker, TTSClipData clipData)
		{
			this._needsRefresh = true;
		}

		protected override void OnPlaybackReady(TTSSpeaker speaker, TTSClipData clipData)
		{
			this._needsRefresh = true;
		}

		protected override void OnPlaybackStart(TTSSpeaker speaker, TTSClipData clipData)
		{
			this._needsRefresh = true;
		}

		protected override void OnPlaybackCancelled(TTSSpeaker speaker, TTSClipData clipData, string reason)
		{
			this._needsRefresh = true;
		}

		protected override void OnPlaybackComplete(TTSSpeaker speaker, TTSClipData clipData)
		{
			this._needsRefresh = true;
		}

		private IEnumerator RefreshUpdater()
		{
			for (;;)
			{
				if (this._needsRefresh)
				{
					this.RefreshLabel();
				}
				yield return null;
			}
			yield break;
		}

		private void RefreshLabel()
		{
			this._needsRefresh = false;
			StringBuilder stringBuilder = new StringBuilder();
			if (base.Speaker.IsSpeaking)
			{
				this.AppendClipText(stringBuilder, base.Speaker.SpeakingClip, "Speaking");
			}
			int num = 1;
			foreach (TTSClipData clipData in base.Speaker.QueuedClips)
			{
				this.AppendClipText(stringBuilder, clipData, string.Format("Queue[{0}]", num));
				num++;
			}
			this._label.text = stringBuilder.ToString();
			this._label.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, this._label.preferredHeight);
		}

		private void AppendClipText(StringBuilder status, TTSClipData clipData, string clipKey)
		{
			status.AppendLine(clipKey);
			status.AppendLine("\tText: '" + clipData.textToSpeak + "'");
			status.AppendLine("\tVoice: " + ((clipData.voiceSettings == null) ? "" : clipData.voiceSettings.SettingsId));
			status.AppendLine("\tType: " + clipData.extension);
			status.AppendLine(string.Format("\tStatus: {0}", clipData.loadState));
			if (clipData.loadState == TTSClipLoadState.Loaded)
			{
				status.AppendLine(string.Format("\tLoad Time: {0:0.000} seconds", clipData.readyDuration));
			}
			status.Append("\n");
		}

		[SerializeField]
		private Text _label;

		private bool _needsRefresh = true;

		private Coroutine _refreshUpdater;
	}
}
