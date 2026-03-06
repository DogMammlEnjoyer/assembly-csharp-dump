using System;
using Meta.Voice.Logging;
using Meta.WitAi.Attributes;
using Meta.WitAi.TTS.Data;
using Meta.WitAi.TTS.Interfaces;
using UnityEngine;

namespace Meta.WitAi.TTS.Integrations
{
	public abstract class TTSEventAnimator<TEvent, TData> : MonoBehaviour where TEvent : TTSEvent<TData>
	{
		public IVLogger Logger { get; } = LoggerRegistry.Instance.GetLogger(LogCategory.TextToSpeech, null);

		public ITTSEventPlayer Player
		{
			get
			{
				return this._player as ITTSEventPlayer;
			}
			set
			{
				Object @object = value as Object;
				if (@object != null)
				{
					this._player = @object;
					return;
				}
				if (value != null)
				{
					this.Logger.Error("Invalid ITTSEventPlayer type: {0}", new object[]
					{
						value.GetType().Name
					});
				}
				this._player = null;
			}
		}

		public TTSEventContainer EventContainer { get; private set; }

		protected virtual void Awake()
		{
			this._minEvent = Activator.CreateInstance<TEvent>();
			this._maxEvent = Activator.CreateInstance<TEvent>();
		}

		protected virtual void OnEnable()
		{
			if (this.Player == null)
			{
				this._player = base.gameObject.GetComponentInChildren(typeof(ITTSEventPlayer));
				if (this.Player == null)
				{
					VLog.E("No ITTSEventPlayer found for " + base.GetType().Name + " on " + base.name, null);
				}
			}
			this.RefreshSample(true);
		}

		protected virtual void Update()
		{
			this.RefreshSample(false);
		}

		protected virtual void RefreshSample(bool force)
		{
			int num = (this.Player == null) ? 0 : this.Player.ElapsedSamples;
			if (!force && num == this._sample)
			{
				return;
			}
			this._sample = num;
			ITTSEventPlayer player = this.Player;
			TTSEventContainer ttseventContainer = (player != null) ? player.CurrentEvents : null;
			if (num < 0 || ((ttseventContainer != null) ? ttseventContainer.Events : null) == null)
			{
				if (!this.sendMinEvent)
				{
					return;
				}
				this.LerpEvent(this._minEvent, this._minEvent, 0f);
				return;
			}
			else
			{
				ttseventContainer.GetClosestEvents<TEvent>(this._sample, ref this._prevEventIndex, ref this._prevEvent, ref this._nextEvent);
				TEvent tevent;
				if ((tevent = this._prevEvent) == null)
				{
					tevent = this._minEvent;
				}
				TEvent tevent2 = tevent;
				TEvent tevent3;
				if ((tevent3 = this._nextEvent) == null)
				{
					tevent3 = this._maxEvent;
				}
				TEvent tevent4 = tevent3;
				if (this.Player != null)
				{
					this._maxEvent.offset = this.Player.TotalSamples;
				}
				float sampleEventProgress = this.GetSampleEventProgress(num, tevent2.SampleOffset, tevent4.SampleOffset);
				if (tevent4 == this._minEvent && !this.sendMinEvent)
				{
					return;
				}
				if (tevent4 == this._maxEvent && !this.sendMaxEvent)
				{
					return;
				}
				this.LerpEvent(tevent2, tevent4, sampleEventProgress);
				return;
			}
		}

		private float GetSampleEventProgress(int sample, int previousEventSample, int nextEventSample)
		{
			float num = 0f;
			if (previousEventSample != nextEventSample)
			{
				num = Mathf.Clamp01((float)(sample - previousEventSample) / (float)(nextEventSample - previousEventSample));
			}
			if (this.easeIgnored)
			{
				num = ((num >= 1f) ? 1f : 0f);
			}
			else
			{
				num = this.easeCurve.Evaluate(num);
			}
			return num;
		}

		protected abstract void LerpEvent(TEvent fromEvent, TEvent toEvent, float percentage);

		[SerializeField]
		[ObjectType(typeof(ITTSEventPlayer), new Type[]
		{

		})]
		private Object _player;

		public bool easeIgnored;

		public AnimationCurve easeCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		public bool sendMinEvent = true;

		public bool sendMaxEvent = true;

		private int _sample = -1;

		private int _prevEventIndex;

		private TEvent _prevEvent;

		private TEvent _nextEvent;

		private TEvent _minEvent;

		private TEvent _maxEvent;
	}
}
