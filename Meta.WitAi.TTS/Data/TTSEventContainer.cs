using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Meta.WitAi.Json;

namespace Meta.WitAi.TTS.Data
{
	[Serializable]
	public class TTSEventContainer
	{
		public IEnumerable<ITTSEvent> Events
		{
			get
			{
				return this._events;
			}
		}

		public IEnumerable<TTSWordEvent> WordEvents
		{
			get
			{
				return this.GetEvents<TTSWordEvent>(null);
			}
		}

		public IEnumerable<TTSVisemeEvent> VisemeEvents
		{
			get
			{
				return this.GetEvents<TTSVisemeEvent>(null);
			}
		}

		public event Action<WitResponseNode> OnEventJsonAdded;

		public event Action<ITTSEvent> OnEventAdded;

		public IEnumerable<TEvent> GetEvents<TEvent>(string eventTypeKey = null) where TEvent : ITTSEvent
		{
			return from e in this._events
			where e is TEvent && (string.IsNullOrEmpty(eventTypeKey) || eventTypeKey.Equals(e.EventType))
			select (TEvent)((object)e);
		}

		public void AddEvents(IEnumerable<WitResponseNode> events)
		{
			if (events == null)
			{
				return;
			}
			foreach (WitResponseNode eventNode in events)
			{
				this.AddEvent(eventNode);
			}
		}

		public bool AddEvent(WitResponseNode eventNode)
		{
			ITTSEvent ittsevent = this.DecodeEvent(eventNode);
			if (ittsevent == null)
			{
				return false;
			}
			this._events.Enqueue(ittsevent);
			Action<WitResponseNode> onEventJsonAdded = this.OnEventJsonAdded;
			if (onEventJsonAdded != null)
			{
				onEventJsonAdded(eventNode);
			}
			Action<ITTSEvent> onEventAdded = this.OnEventAdded;
			if (onEventAdded != null)
			{
				onEventAdded(ittsevent);
			}
			return true;
		}

		private ITTSEvent DecodeEvent(WitResponseNode eventNode)
		{
			ITTSEvent result;
			try
			{
				string value = eventNode["type"].Value;
				if (!(value == "WORD"))
				{
					if (!(value == "VISEME"))
					{
						if (!(value == "PHONE"))
						{
							if (!(value == "EMOTE"))
							{
								if (!(value == "ACTION"))
								{
									result = JsonConvert.DeserializeObject<TTSStringEvent>(eventNode, null, false);
								}
								else
								{
									result = JsonConvert.DeserializeObject<TTSActionEvent>(eventNode, null, false);
								}
							}
							else
							{
								result = JsonConvert.DeserializeObject<TTSEmoteEvent>(eventNode, null, false);
							}
						}
						else
						{
							result = JsonConvert.DeserializeObject<TTSPhonemeEvent>(eventNode, null, false);
						}
					}
					else
					{
						result = JsonConvert.DeserializeObject<TTSVisemeEvent>(eventNode, null, false);
					}
				}
				else
				{
					result = JsonConvert.DeserializeObject<TTSWordEvent>(eventNode, null, false);
				}
			}
			catch (Exception)
			{
				result = null;
			}
			return result;
		}

		public void GetClosestEvents<TEvent>(int sample, ref int previousEventIndex, ref TEvent previousEvent, ref TEvent nextEvent) where TEvent : ITTSEvent
		{
			if (previousEvent == null || sample < previousEvent.SampleOffset)
			{
				previousEventIndex = 0;
			}
			nextEvent = default(TEvent);
			int num = 0;
			foreach (ITTSEvent ittsevent in this._events)
			{
				if (num >= previousEventIndex && ittsevent is TEvent)
				{
					TEvent tevent = (TEvent)((object)ittsevent);
					if (sample < tevent.SampleOffset)
					{
						nextEvent = tevent;
						break;
					}
					previousEventIndex = num;
					previousEvent = tevent;
				}
				num++;
			}
		}

		private ConcurrentQueue<ITTSEvent> _events = new ConcurrentQueue<ITTSEvent>();

		internal const string EVENT_TYPE_KEY = "type";

		internal const string EVENT_WORD_TYPE_KEY = "WORD";

		internal const string EVENT_VISEME_TYPE_KEY = "VISEME";

		internal const string EVENT_PHONEME_TYPE_KEY = "PHONE";

		internal const string EVENT_EMOTE_TYPE_KEY = "EMOTE";

		internal const string EVENT_ACTION_TYPE_KEY = "ACTION";
	}
}
