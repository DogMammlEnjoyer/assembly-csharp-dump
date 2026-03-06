using System;
using Meta.WitAi.Attributes;
using Meta.WitAi.TTS.Data;
using Meta.WitAi.TTS.Integrations;
using UnityEngine;
using UnityEngine.Serialization;

namespace Meta.WitAi.TTS.LipSync
{
	public class VisemeLipSyncAnimator : TTSEventAnimator<TTSVisemeEvent, Viseme>, IVisemeAnimatorProvider
	{
		public Viseme LastViseme { get; private set; }

		public VisemeChangedEvent OnVisemeStarted
		{
			get
			{
				return this._onVisemeStarted;
			}
		}

		public VisemeChangedEvent OnVisemeFinished
		{
			get
			{
				return this._onVisemeFinished;
			}
		}

		public VisemeLerpEvent OnVisemeLerp
		{
			get
			{
				return this._onVisemeLerp;
			}
		}

		[Obsolete("Use OnVisemeStarted, OnVisemeLerp or OnVisemeFinished instead.")]
		public VisemeChangedEvent OnVisemeChanged
		{
			get
			{
				return this.OnVisemeStarted;
			}
		}

		protected override void LerpEvent(TTSVisemeEvent fromEvent, TTSVisemeEvent toEvent, float percentage)
		{
			this.SetViseme((percentage >= 1f) ? toEvent.Data : fromEvent.Data);
			percentage = Mathf.Clamp01(percentage);
			VisemeLerpEvent onVisemeLerp = this.OnVisemeLerp;
			if (onVisemeLerp == null)
			{
				return;
			}
			onVisemeLerp.Invoke(fromEvent.Data, toEvent.Data, percentage);
		}

		private void SetViseme(Viseme newViseme)
		{
			if (this.LastViseme == newViseme)
			{
				return;
			}
			Viseme lastViseme = this.LastViseme;
			this.LastViseme = newViseme;
			VisemeChangedEvent onVisemeFinished = this.OnVisemeFinished;
			if (onVisemeFinished != null)
			{
				onVisemeFinished.Invoke(lastViseme);
			}
			VisemeChangedEvent onVisemeStarted = this.OnVisemeStarted;
			if (onVisemeStarted == null)
			{
				return;
			}
			onVisemeStarted.Invoke(this.LastViseme);
		}

		[Header("Viseme Events")]
		[TooltipBox("Fired when entering or passing a sample with this specified viseme")]
		[SerializeField]
		private VisemeChangedEvent _onVisemeStarted = new VisemeChangedEvent();

		[TooltipBox("Fired when entering or passing a new sample with a different specified viseme")]
		[SerializeField]
		private VisemeChangedEvent _onVisemeFinished = new VisemeChangedEvent();

		[TooltipBox("Fired once per frame with the previous viseme and next viseme as well as a percentage of the current frame in between each viseme.")]
		[SerializeField]
		[FormerlySerializedAs("onVisemeLerp")]
		private VisemeLerpEvent _onVisemeLerp = new VisemeLerpEvent();
	}
}
