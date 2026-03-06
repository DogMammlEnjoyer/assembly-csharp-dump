using System;
using System.Collections.Concurrent;
using System.Linq;
using Meta.Voice.Audio;
using Meta.WitAi.TTS.Data;
using Meta.WitAi.TTS.Interfaces;
using UnityEngine;

namespace Meta.WitAi.TTS.Integrations
{
	public class BaseTTSRuntimeCache : MonoBehaviour, ITTSRuntimeCacheHandler
	{
		public event TTSClipCallback OnClipAdded;

		public event TTSClipCallback OnClipRemoved;

		public virtual TTSClipData[] GetClips()
		{
			return this._clips.Values.ToArray<TTSClipData>();
		}

		protected virtual void OnDestroy()
		{
			this._clips.Clear();
		}

		public virtual TTSClipData GetClip(string clipId)
		{
			TTSClipData result;
			this._clips.TryGetValue(clipId, out result);
			return result;
		}

		public virtual bool AddClip(TTSClipData clipData)
		{
			if (clipData == null || string.IsNullOrEmpty(clipData.clipID))
			{
				return false;
			}
			TTSClipData ttsclipData;
			if (this._clips.TryGetValue(clipData.clipID, out ttsclipData) && ttsclipData != null && ttsclipData.Equals(clipData))
			{
				return true;
			}
			this._clips[clipData.clipID] = clipData;
			this.SetupClip(clipData);
			return true;
		}

		protected virtual void SetupClip(TTSClipData clipData)
		{
			TTSClipCallback onClipAdded = this.OnClipAdded;
			if (onClipAdded == null)
			{
				return;
			}
			onClipAdded(clipData);
		}

		public virtual void RemoveClip(string clipID)
		{
			TTSClipData ttsclipData;
			if (this._clips.TryGetValue(clipID, out ttsclipData) && string.IsNullOrEmpty(ttsclipData.textToSpeak))
			{
				return;
			}
			if (!this._clips.TryRemove(clipID, out ttsclipData))
			{
				return;
			}
			this.BreakdownClip(ttsclipData);
		}

		protected virtual void BreakdownClip(TTSClipData clipData)
		{
			IAudioClipStream clipStream = clipData.clipStream;
			if (clipStream != null)
			{
				clipStream.Unload();
			}
			clipData.clipStream = null;
			TTSClipCallback onClipRemoved = this.OnClipRemoved;
			if (onClipRemoved == null)
			{
				return;
			}
			onClipRemoved(clipData);
		}

		protected ConcurrentDictionary<string, TTSClipData> _clips = new ConcurrentDictionary<string, TTSClipData>();
	}
}
