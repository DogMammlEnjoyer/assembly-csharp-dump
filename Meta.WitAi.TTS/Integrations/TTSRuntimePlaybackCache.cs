using System;
using System.Collections.Concurrent;
using Meta.WitAi.TTS.Data;
using UnityEngine;

namespace Meta.WitAi.TTS.Integrations
{
	public class TTSRuntimePlaybackCache : BaseTTSRuntimeCache
	{
		protected override void SetupClip(TTSClipData clipData)
		{
			this._requests[clipData.clipID] = 0;
			clipData.onRequestBegin = (Action<TTSClipData>)Delegate.Combine(clipData.onRequestBegin, new Action<TTSClipData>(this.OnRequestBegin));
			clipData.onRequestComplete = (Action<TTSClipData>)Delegate.Combine(clipData.onRequestComplete, new Action<TTSClipData>(this.OnRequestComplete));
			base.SetupClip(clipData);
		}

		private void OnRequestBegin(TTSClipData clipData)
		{
			string clipID = clipData.clipID;
			int num;
			if (!this._requests.TryGetValue(clipID, out num))
			{
				num = 0;
			}
			this._requests[clipID] = num + 1;
		}

		private void OnRequestComplete(TTSClipData clipData)
		{
			string clipID = clipData.clipID;
			int num;
			if (!this._requests.TryGetValue(clipID, out num))
			{
				return;
			}
			num = Mathf.Max(0, num - 1);
			this._requests[clipID] = num;
			if (num == 0)
			{
				this.RemoveClip(clipData.clipID);
			}
		}

		protected override void BreakdownClip(TTSClipData clipData)
		{
			clipData.onRequestBegin = (Action<TTSClipData>)Delegate.Remove(clipData.onRequestBegin, new Action<TTSClipData>(this.OnRequestBegin));
			clipData.onRequestComplete = (Action<TTSClipData>)Delegate.Remove(clipData.onRequestComplete, new Action<TTSClipData>(this.OnRequestComplete));
			int num;
			this._requests.TryRemove(clipData.clipID, out num);
			base.BreakdownClip(clipData);
		}

		private ConcurrentDictionary<string, int> _requests = new ConcurrentDictionary<string, int>();
	}
}
