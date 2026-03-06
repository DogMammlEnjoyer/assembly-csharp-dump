using System;
using System.Collections.Generic;
using Meta.Voice.Logging;
using Meta.WitAi.Attributes;
using Meta.WitAi.TTS.Data;
using Meta.WitAi.TTS.Interfaces;
using UnityEngine;

namespace Meta.WitAi.TTS.Integrations
{
	public abstract class TTSEventTrigger<TEvent, TData> : MonoBehaviour where TEvent : TTSEvent<TData>
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

		protected virtual void OnEnable()
		{
		}

		protected virtual void OnDisable()
		{
			this.ClearCurrentEvents();
		}

		private void ClearCurrentEvents()
		{
			if (this._currentEvents != null)
			{
				this._currentEvents.OnEventAdded -= this.OnEventAdded;
				this._currentEvents = null;
			}
		}

		private void OnEventAdded(ITTSEvent ev)
		{
			if (ev is TEvent)
			{
				this.queuedEvents.Enqueue(ev);
			}
		}

		protected virtual void Update()
		{
			if (this.Player == null || this.Player.CurrentEvents == null)
			{
				return;
			}
			if (this._currentEvents != this.Player.CurrentEvents)
			{
				this.ClearCurrentEvents();
				this._currentEvents = this.Player.CurrentEvents;
				if (this._currentEvents != null)
				{
					this._currentEvents.OnEventAdded += this.OnEventAdded;
					foreach (ITTSEvent ittsevent in this.Player.CurrentEvents.Events)
					{
						if (ittsevent is TEvent)
						{
							this.queuedEvents.Enqueue(ittsevent);
						}
					}
				}
			}
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
			while (this.queuedEvents.Count > 0 && this._sample > this.queuedEvents.Peek().SampleOffset)
			{
				this.OnEventTriggered((TEvent)((object)this.queuedEvents.Dequeue()));
			}
		}

		protected abstract void OnEventTriggered(TEvent queuedEvent);

		private int _sample = -1;

		private Queue<ITTSEvent> queuedEvents = new Queue<ITTSEvent>();

		[SerializeField]
		[ObjectType(typeof(ITTSEventPlayer), new Type[]
		{

		})]
		private Object _player;

		private TTSEventContainer _currentEvents;
	}
}
