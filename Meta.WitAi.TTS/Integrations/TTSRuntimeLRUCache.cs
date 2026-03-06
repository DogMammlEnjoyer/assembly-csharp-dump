using System;
using System.Collections.Generic;
using Meta.WitAi.TTS.Data;
using UnityEngine;
using UnityEngine.Serialization;

namespace Meta.WitAi.TTS.Integrations
{
	public class TTSRuntimeLRUCache : BaseTTSRuntimeCache
	{
		public override TTSClipData[] GetClips()
		{
			TTSClipData[] array = new TTSClipData[this._clipOrder.Count];
			for (int i = 0; i < array.Length; i++)
			{
				TTSClipData ttsclipData;
				this._clips.TryGetValue(this._clipOrder[i], out ttsclipData);
				array[i] = ttsclipData;
			}
			return array;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			this._clipOrder.Clear();
		}

		public bool RefreshClipLRU(string clipId)
		{
			int num = this._clipOrder.IndexOf(clipId);
			if (num != -1)
			{
				this._clipOrder.RemoveAt(num);
				this._clipOrder.Add(clipId);
				return true;
			}
			return false;
		}

		public override TTSClipData GetClip(string clipId)
		{
			this.RefreshClipLRU(clipId);
			return base.GetClip(clipId);
		}

		protected override void SetupClip(TTSClipData clipData)
		{
			this._clipOrder.Add(clipData.clipID);
			base.SetupClip(clipData);
		}

		public override bool AddClip(TTSClipData clipData)
		{
			if (!base.AddClip(clipData))
			{
				return false;
			}
			this.RefreshClipLRU(clipData.clipID);
			while (this.IsCacheFull() && this._clipOrder.Count > 0)
			{
				string clipID = this._clipOrder[0];
				this._clipOrder.RemoveAt(0);
				this.RemoveClip(clipID);
			}
			return this._clipOrder.Count > 0;
		}

		protected override void BreakdownClip(TTSClipData clipData)
		{
			int num = this._clipOrder.IndexOf(clipData.clipID);
			if (num != -1)
			{
				this._clipOrder.RemoveAt(num);
			}
			base.BreakdownClip(clipData);
		}

		private bool IsCacheFull()
		{
			return (this.ClipLimit && this._clipOrder.Count > this.ClipCapacity) || (this.RamLimit && this.GetCacheDiskSize() > this.RamCapacity);
		}

		public int GetCacheDiskSize()
		{
			long num = 0L;
			foreach (string key in this._clips.Keys)
			{
				if (this._clips[key].clipStream != null)
				{
					num += TTSRuntimeLRUCache.GetClipBytes(this._clips[key].clipStream.Channels, this._clips[key].clipStream.TotalSamples);
				}
			}
			return (int)(num / 1024L) + 1;
		}

		public static long GetClipBytes(AudioClip clip)
		{
			if (clip != null)
			{
				return TTSRuntimeLRUCache.GetClipBytes(clip.channels, clip.samples);
			}
			return 0L;
		}

		public static long GetClipBytes(int channels, int samples)
		{
			return (long)(channels * samples * 2);
		}

		[Header("Runtime Cache Settings")]
		[Tooltip("Whether or not to unload clip data after the clip capacity is hit")]
		[FormerlySerializedAs("_clipLimit")]
		public bool ClipLimit;

		[Tooltip("The maximum clips allowed in the runtime cache")]
		[FormerlySerializedAs("_clipCapacity")]
		[Min(1f)]
		public int ClipCapacity = 5;

		[Tooltip("Whether or not to unload clip data after the ram capacity is hit")]
		[FormerlySerializedAs("_ramLimit")]
		public bool RamLimit = true;

		[Tooltip("The maximum amount of RAM allowed in the runtime cache in KBs.  For example, 24k samples per second * 2bits per sample * 10 minutes (600 seconds) = 3600KBs")]
		[FormerlySerializedAs("_ramCapacity")]
		[Min(1f)]
		public int RamCapacity = 3600;

		private List<string> _clipOrder = new List<string>();
	}
}
