using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UnityEngine.UIElements.Experimental
{
	internal class EventDebugger
	{
		public IPanel panel { get; set; }

		public bool isReplaying { get; private set; }

		public float playbackSpeed { get; set; } = 1f;

		public bool isPlaybackPaused { get; set; }

		public void UpdateModificationCount()
		{
			bool flag = this.panel == null;
			if (!flag)
			{
				long num;
				bool flag2 = !this.m_ModificationCount.TryGetValue(this.panel, out num);
				if (flag2)
				{
					num = 0L;
				}
				num += 1L;
				this.m_ModificationCount[this.panel] = num;
			}
		}

		public void BeginProcessEvent(EventBase evt, IEventHandler mouseCapture)
		{
			this.AddBeginProcessEvent(evt, mouseCapture);
			this.UpdateModificationCount();
		}

		public void EndProcessEvent(EventBase evt, long duration, IEventHandler mouseCapture)
		{
			this.AddEndProcessEvent(evt, duration, mouseCapture);
			this.UpdateModificationCount();
		}

		public void LogCall(int cbHashCode, string cbName, EventBase evt, bool propagationHasStopped, bool immediatePropagationHasStopped, long duration, IEventHandler mouseCapture)
		{
			this.AddCallObject(cbHashCode, cbName, evt, propagationHasStopped, immediatePropagationHasStopped, duration, mouseCapture);
			this.UpdateModificationCount();
		}

		public void LogIMGUICall(EventBase evt, long duration, IEventHandler mouseCapture)
		{
			this.AddIMGUICall(evt, duration, mouseCapture);
			this.UpdateModificationCount();
		}

		public void LogExecuteDefaultAction(EventBase evt, PropagationPhase phase, long duration, IEventHandler mouseCapture)
		{
			this.AddExecuteDefaultAction(evt, phase, duration, mouseCapture);
			this.UpdateModificationCount();
		}

		public static void LogPropagationPaths(EventBase evt, PropagationPaths paths)
		{
		}

		private void LogPropagationPathsInternal(EventBase evt, PropagationPaths paths)
		{
			this.AddPropagationPaths(evt, paths);
			this.UpdateModificationCount();
		}

		public List<EventDebuggerCallTrace> GetCalls(IPanel panel, EventDebuggerEventRecord evt = null)
		{
			List<EventDebuggerCallTrace> list;
			bool flag = !this.m_EventCalledObjects.TryGetValue(panel, out list);
			List<EventDebuggerCallTrace> result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = evt != null && list != null;
				if (flag2)
				{
					List<EventDebuggerCallTrace> list2 = new List<EventDebuggerCallTrace>();
					foreach (EventDebuggerCallTrace eventDebuggerCallTrace in list)
					{
						bool flag3 = eventDebuggerCallTrace.eventBase.eventId == evt.eventId;
						if (flag3)
						{
							list2.Add(eventDebuggerCallTrace);
						}
					}
					list = list2;
				}
				result = list;
			}
			return result;
		}

		public List<EventDebuggerDefaultActionTrace> GetDefaultActions(IPanel panel, EventDebuggerEventRecord evt = null)
		{
			List<EventDebuggerDefaultActionTrace> list;
			bool flag = !this.m_EventDefaultActionObjects.TryGetValue(panel, out list);
			List<EventDebuggerDefaultActionTrace> result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = evt != null && list != null;
				if (flag2)
				{
					List<EventDebuggerDefaultActionTrace> list2 = new List<EventDebuggerDefaultActionTrace>();
					foreach (EventDebuggerDefaultActionTrace eventDebuggerDefaultActionTrace in list)
					{
						bool flag3 = eventDebuggerDefaultActionTrace.eventBase.eventId == evt.eventId;
						if (flag3)
						{
							list2.Add(eventDebuggerDefaultActionTrace);
						}
					}
					list = list2;
				}
				result = list;
			}
			return result;
		}

		public List<EventDebuggerPathTrace> GetPropagationPaths(IPanel panel, EventDebuggerEventRecord evt = null)
		{
			List<EventDebuggerPathTrace> list;
			bool flag = !this.m_EventPathObjects.TryGetValue(panel, out list);
			List<EventDebuggerPathTrace> result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = evt != null && list != null;
				if (flag2)
				{
					List<EventDebuggerPathTrace> list2 = new List<EventDebuggerPathTrace>();
					foreach (EventDebuggerPathTrace eventDebuggerPathTrace in list)
					{
						bool flag3 = eventDebuggerPathTrace.eventBase.eventId == evt.eventId;
						if (flag3)
						{
							list2.Add(eventDebuggerPathTrace);
						}
					}
					list = list2;
				}
				result = list;
			}
			return result;
		}

		public List<EventDebuggerTrace> GetBeginEndProcessedEvents(IPanel panel, EventDebuggerEventRecord evt = null)
		{
			List<EventDebuggerTrace> list;
			bool flag = !this.m_EventProcessedEvents.TryGetValue(panel, out list);
			List<EventDebuggerTrace> result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = evt != null && list != null;
				if (flag2)
				{
					List<EventDebuggerTrace> list2 = new List<EventDebuggerTrace>();
					foreach (EventDebuggerTrace eventDebuggerTrace in list)
					{
						bool flag3 = eventDebuggerTrace.eventBase.eventId == evt.eventId;
						if (flag3)
						{
							list2.Add(eventDebuggerTrace);
						}
					}
					list = list2;
				}
				result = list;
			}
			return result;
		}

		public long GetModificationCount(IPanel panel)
		{
			bool flag = panel == null;
			long result;
			if (flag)
			{
				result = -1L;
			}
			else
			{
				long num;
				bool flag2 = !this.m_ModificationCount.TryGetValue(panel, out num);
				if (flag2)
				{
					num = -1L;
				}
				result = num;
			}
			return result;
		}

		public void ClearLogs()
		{
			this.UpdateModificationCount();
			bool flag = this.panel == null;
			if (flag)
			{
				this.m_EventCalledObjects.Clear();
				this.m_EventDefaultActionObjects.Clear();
				this.m_EventPathObjects.Clear();
				this.m_EventProcessedEvents.Clear();
				this.m_StackOfProcessedEvent.Clear();
				this.m_EventTypeProcessedCount.Clear();
			}
			else
			{
				this.m_EventCalledObjects.Remove(this.panel);
				this.m_EventDefaultActionObjects.Remove(this.panel);
				this.m_EventPathObjects.Remove(this.panel);
				this.m_EventProcessedEvents.Remove(this.panel);
				this.m_StackOfProcessedEvent.Remove(this.panel);
				Dictionary<long, int> dictionary;
				bool flag2 = this.m_EventTypeProcessedCount.TryGetValue(this.panel, out dictionary);
				if (flag2)
				{
					dictionary.Clear();
				}
			}
		}

		public void SaveReplaySessionFromSelection(string path, List<EventDebuggerEventRecord> eventList)
		{
			bool flag = string.IsNullOrEmpty(path);
			if (!flag)
			{
				EventDebuggerRecordList obj = new EventDebuggerRecordList
				{
					eventList = eventList
				};
				string contents = JsonUtility.ToJson(obj);
				File.WriteAllText(path, contents);
				Debug.Log("Saved under: " + path);
			}
		}

		public EventDebuggerRecordList LoadReplaySession(string path)
		{
			bool flag = string.IsNullOrEmpty(path);
			EventDebuggerRecordList result;
			if (flag)
			{
				result = null;
			}
			else
			{
				string json = File.ReadAllText(path);
				result = JsonUtility.FromJson<EventDebuggerRecordList>(json);
			}
			return result;
		}

		public IEnumerator ReplayEvents(IEnumerable<EventDebuggerEventRecord> eventBases, Action<int, int> refreshList)
		{
			bool flag = eventBases == null;
			if (flag)
			{
				yield break;
			}
			this.isReplaying = true;
			IEnumerator doReplay = this.DoReplayEvents(eventBases, refreshList);
			while (doReplay.MoveNext())
			{
				yield return null;
			}
			yield break;
		}

		public void StopPlayback()
		{
			this.isReplaying = false;
			this.isPlaybackPaused = false;
		}

		private IEnumerator DoReplayEvents(IEnumerable<EventDebuggerEventRecord> eventBases, Action<int, int> refreshList)
		{
			EventDebugger.<>c__DisplayClass34_0 CS$<>8__locals1 = new EventDebugger.<>c__DisplayClass34_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.sortedEvents = (from e in eventBases
			orderby e.timestamp
			select e).ToList<EventDebuggerEventRecord>();
			int sortedEventsCount = CS$<>8__locals1.sortedEvents.Count;
			int i = 0;
			while (i < sortedEventsCount)
			{
				bool flag = !this.isReplaying;
				if (flag)
				{
					break;
				}
				EventDebuggerEventRecord eventBase = CS$<>8__locals1.sortedEvents[i];
				Event newEvent = new Event
				{
					button = eventBase.button,
					clickCount = eventBase.clickCount,
					modifiers = eventBase.modifiers,
					mousePosition = eventBase.mousePosition
				};
				bool flag2 = eventBase.eventTypeId == EventBase<PointerMoveEvent>.TypeId();
				if (flag2)
				{
					newEvent.type = EventType.MouseMove;
					CS$<>8__locals1.<DoReplayEvents>g__SendEvent|2(UIElementsUtility.CreateEvent(newEvent, EventType.MouseMove));
					goto IL_6BA;
				}
				bool flag3 = eventBase.eventTypeId == EventBase<PointerDownEvent>.TypeId();
				if (flag3)
				{
					newEvent.type = EventType.MouseDown;
					CS$<>8__locals1.<DoReplayEvents>g__SendEvent|2(UIElementsUtility.CreateEvent(newEvent, EventType.MouseDown));
					goto IL_6BA;
				}
				bool flag4 = eventBase.eventTypeId == EventBase<PointerUpEvent>.TypeId();
				if (flag4)
				{
					newEvent.type = EventType.MouseUp;
					CS$<>8__locals1.<DoReplayEvents>g__SendEvent|2(UIElementsUtility.CreateEvent(newEvent, EventType.MouseUp));
					goto IL_6BA;
				}
				bool flag5 = eventBase.eventTypeId == EventBase<ContextClickEvent>.TypeId();
				if (flag5)
				{
					newEvent.type = EventType.ContextClick;
					CS$<>8__locals1.<DoReplayEvents>g__SendEvent|2(UIElementsUtility.CreateEvent(newEvent, EventType.ContextClick));
					goto IL_6BA;
				}
				bool flag6 = eventBase.eventTypeId == EventBase<MouseEnterWindowEvent>.TypeId();
				if (flag6)
				{
					newEvent.type = EventType.MouseEnterWindow;
					CS$<>8__locals1.<DoReplayEvents>g__SendEvent|2(UIElementsUtility.CreateEvent(newEvent, EventType.MouseEnterWindow));
					goto IL_6BA;
				}
				bool flag7 = eventBase.eventTypeId == EventBase<MouseLeaveWindowEvent>.TypeId();
				if (flag7)
				{
					newEvent.type = EventType.MouseLeaveWindow;
					CS$<>8__locals1.<DoReplayEvents>g__SendEvent|2(UIElementsUtility.CreateEvent(newEvent, EventType.MouseLeaveWindow));
					goto IL_6BA;
				}
				bool flag8 = eventBase.eventTypeId == EventBase<WheelEvent>.TypeId();
				if (flag8)
				{
					newEvent.type = EventType.ScrollWheel;
					newEvent.delta = eventBase.delta;
					CS$<>8__locals1.<DoReplayEvents>g__SendEvent|2(UIElementsUtility.CreateEvent(newEvent, EventType.ScrollWheel));
					goto IL_6BA;
				}
				bool flag9 = eventBase.eventTypeId == EventBase<KeyDownEvent>.TypeId();
				if (flag9)
				{
					newEvent.type = EventType.KeyDown;
					newEvent.character = eventBase.character;
					newEvent.keyCode = eventBase.keyCode;
					CS$<>8__locals1.<DoReplayEvents>g__SendEvent|2(UIElementsUtility.CreateEvent(newEvent, EventType.KeyDown));
					goto IL_6BA;
				}
				bool flag10 = eventBase.eventTypeId == EventBase<KeyUpEvent>.TypeId();
				if (flag10)
				{
					newEvent.type = EventType.KeyUp;
					newEvent.character = eventBase.character;
					newEvent.keyCode = eventBase.keyCode;
					CS$<>8__locals1.<DoReplayEvents>g__SendEvent|2(UIElementsUtility.CreateEvent(newEvent, EventType.KeyUp));
					goto IL_6BA;
				}
				bool flag11 = eventBase.eventTypeId == EventBase<NavigationMoveEvent>.TypeId();
				if (flag11)
				{
					CS$<>8__locals1.<DoReplayEvents>g__SendEvent|2(NavigationMoveEvent.GetPooled(eventBase.navigationDirection, eventBase.deviceType, eventBase.modifiers));
					goto IL_6BA;
				}
				bool flag12 = eventBase.eventTypeId == EventBase<NavigationSubmitEvent>.TypeId();
				if (flag12)
				{
					CS$<>8__locals1.<DoReplayEvents>g__SendEvent|2(NavigationEventBase<NavigationSubmitEvent>.GetPooled(eventBase.deviceType, eventBase.modifiers));
					goto IL_6BA;
				}
				bool flag13 = eventBase.eventTypeId == EventBase<NavigationCancelEvent>.TypeId();
				if (flag13)
				{
					CS$<>8__locals1.<DoReplayEvents>g__SendEvent|2(NavigationEventBase<NavigationCancelEvent>.GetPooled(eventBase.deviceType, eventBase.modifiers));
					goto IL_6BA;
				}
				bool flag14 = eventBase.eventTypeId == EventBase<ValidateCommandEvent>.TypeId();
				if (flag14)
				{
					newEvent.type = EventType.ValidateCommand;
					newEvent.commandName = eventBase.commandName;
					CS$<>8__locals1.<DoReplayEvents>g__SendEvent|2(UIElementsUtility.CreateEvent(newEvent, EventType.ValidateCommand));
					goto IL_6BA;
				}
				bool flag15 = eventBase.eventTypeId == EventBase<ExecuteCommandEvent>.TypeId();
				if (flag15)
				{
					newEvent.type = EventType.ExecuteCommand;
					newEvent.commandName = eventBase.commandName;
					CS$<>8__locals1.<DoReplayEvents>g__SendEvent|2(UIElementsUtility.CreateEvent(newEvent, EventType.ExecuteCommand));
					goto IL_6BA;
				}
				bool flag16 = eventBase.eventTypeId == EventBase<IMGUIEvent>.TypeId();
				if (flag16)
				{
					string str = "Skipped IMGUI event (";
					string eventBaseName = eventBase.eventBaseName;
					string str2 = "): ";
					EventDebuggerEventRecord eventDebuggerEventRecord = eventBase;
					Debug.Log(str + eventBaseName + str2 + ((eventDebuggerEventRecord != null) ? eventDebuggerEventRecord.ToString() : null));
					IEnumerator awaitSkipped = CS$<>8__locals1.<DoReplayEvents>g__AwaitForNextEvent|1(i);
					while (awaitSkipped.MoveNext())
					{
						yield return null;
					}
				}
				else
				{
					string str3 = "Skipped event (";
					string eventBaseName2 = eventBase.eventBaseName;
					string str4 = "): ";
					EventDebuggerEventRecord eventDebuggerEventRecord2 = eventBase;
					Debug.Log(str3 + eventBaseName2 + str4 + ((eventDebuggerEventRecord2 != null) ? eventDebuggerEventRecord2.ToString() : null));
					IEnumerator awaitSkipped2 = CS$<>8__locals1.<DoReplayEvents>g__AwaitForNextEvent|1(i);
					while (awaitSkipped2.MoveNext())
					{
						yield return null;
					}
				}
				IL_764:
				int num = i;
				i = num + 1;
				continue;
				IL_6BA:
				if (refreshList != null)
				{
					refreshList(i, sortedEventsCount);
				}
				Debug.Log(string.Format("Replayed event {0} ({1}): {2}", eventBase.eventId.ToString(), eventBase.eventBaseName, newEvent));
				IEnumerator await = CS$<>8__locals1.<DoReplayEvents>g__AwaitForNextEvent|1(i);
				while (await.MoveNext())
				{
					yield return null;
				}
				eventBase = null;
				newEvent = null;
				await = null;
				goto IL_764;
			}
			this.isReplaying = false;
			yield break;
		}

		public Dictionary<string, EventDebugger.HistogramRecord> ComputeHistogram(List<EventDebuggerEventRecord> eventBases)
		{
			List<EventDebuggerTrace> list;
			bool flag = this.panel == null || !this.m_EventProcessedEvents.TryGetValue(this.panel, out list);
			Dictionary<string, EventDebugger.HistogramRecord> result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = list == null;
				if (flag2)
				{
					result = null;
				}
				else
				{
					Dictionary<string, EventDebugger.HistogramRecord> dictionary = new Dictionary<string, EventDebugger.HistogramRecord>();
					foreach (EventDebuggerTrace eventDebuggerTrace in list)
					{
						bool flag3 = eventBases == null || eventBases.Count == 0 || eventBases.Contains(eventDebuggerTrace.eventBase);
						if (flag3)
						{
							string eventBaseName = eventDebuggerTrace.eventBase.eventBaseName;
							long num = eventDebuggerTrace.duration;
							long num2 = 1L;
							EventDebugger.HistogramRecord histogramRecord;
							bool flag4 = dictionary.TryGetValue(eventBaseName, out histogramRecord);
							if (flag4)
							{
								num += histogramRecord.duration;
								num2 += histogramRecord.count;
							}
							dictionary[eventBaseName] = new EventDebugger.HistogramRecord
							{
								count = num2,
								duration = num
							};
						}
					}
					result = dictionary;
				}
			}
			return result;
		}

		public Dictionary<long, int> eventTypeProcessedCount
		{
			get
			{
				Dictionary<long, int> dictionary;
				return this.m_EventTypeProcessedCount.TryGetValue(this.panel, out dictionary) ? dictionary : null;
			}
		}

		public bool suspended { get; set; }

		public EventDebugger()
		{
			this.m_EventCalledObjects = new Dictionary<IPanel, List<EventDebuggerCallTrace>>();
			this.m_EventDefaultActionObjects = new Dictionary<IPanel, List<EventDebuggerDefaultActionTrace>>();
			this.m_EventPathObjects = new Dictionary<IPanel, List<EventDebuggerPathTrace>>();
			this.m_StackOfProcessedEvent = new Dictionary<IPanel, Stack<EventDebuggerTrace>>();
			this.m_EventProcessedEvents = new Dictionary<IPanel, List<EventDebuggerTrace>>();
			this.m_EventTypeProcessedCount = new Dictionary<IPanel, Dictionary<long, int>>();
			this.m_ModificationCount = new Dictionary<IPanel, long>();
			this.m_Log = true;
		}

		private void AddCallObject(int cbHashCode, string cbName, EventBase evt, bool propagationHasStopped, bool immediatePropagationHasStopped, long duration, IEventHandler mouseCapture)
		{
			bool suspended = this.suspended;
			if (!suspended)
			{
				bool log = this.m_Log;
				if (log)
				{
					EventDebuggerCallTrace item = new EventDebuggerCallTrace(this.panel, evt, cbHashCode, cbName, propagationHasStopped, immediatePropagationHasStopped, duration, mouseCapture);
					List<EventDebuggerCallTrace> list;
					bool flag = !this.m_EventCalledObjects.TryGetValue(this.panel, out list);
					if (flag)
					{
						list = new List<EventDebuggerCallTrace>();
						this.m_EventCalledObjects.Add(this.panel, list);
					}
					list.Add(item);
				}
			}
		}

		private void AddExecuteDefaultAction(EventBase evt, PropagationPhase phase, long duration, IEventHandler mouseCapture)
		{
			bool suspended = this.suspended;
			if (!suspended)
			{
				bool log = this.m_Log;
				if (log)
				{
					EventDebuggerDefaultActionTrace item = new EventDebuggerDefaultActionTrace(this.panel, evt, phase, duration, mouseCapture);
					List<EventDebuggerDefaultActionTrace> list;
					bool flag = !this.m_EventDefaultActionObjects.TryGetValue(this.panel, out list);
					if (flag)
					{
						list = new List<EventDebuggerDefaultActionTrace>();
						this.m_EventDefaultActionObjects.Add(this.panel, list);
					}
					list.Add(item);
				}
			}
		}

		private void AddPropagationPaths(EventBase evt, PropagationPaths paths)
		{
			bool suspended = this.suspended;
			if (!suspended)
			{
				bool log = this.m_Log;
				if (log)
				{
					EventDebuggerPathTrace item = new EventDebuggerPathTrace(this.panel, evt, new PropagationPaths(paths));
					List<EventDebuggerPathTrace> list;
					bool flag = !this.m_EventPathObjects.TryGetValue(this.panel, out list);
					if (flag)
					{
						list = new List<EventDebuggerPathTrace>();
						this.m_EventPathObjects.Add(this.panel, list);
					}
					list.Add(item);
				}
			}
		}

		private void AddIMGUICall(EventBase evt, long duration, IEventHandler mouseCapture)
		{
			bool suspended = this.suspended;
			if (!suspended)
			{
				bool log = this.m_Log;
				if (log)
				{
					EventDebuggerCallTrace item = new EventDebuggerCallTrace(this.panel, evt, 0, "OnGUI", false, false, duration, mouseCapture);
					List<EventDebuggerCallTrace> list;
					bool flag = !this.m_EventCalledObjects.TryGetValue(this.panel, out list);
					if (flag)
					{
						list = new List<EventDebuggerCallTrace>();
						this.m_EventCalledObjects.Add(this.panel, list);
					}
					list.Add(item);
				}
			}
		}

		private void AddBeginProcessEvent(EventBase evt, IEventHandler mouseCapture)
		{
			bool suspended = this.suspended;
			if (!suspended)
			{
				EventDebuggerTrace eventDebuggerTrace = new EventDebuggerTrace(this.panel, evt, -1L, mouseCapture);
				Stack<EventDebuggerTrace> stack;
				bool flag = !this.m_StackOfProcessedEvent.TryGetValue(this.panel, out stack);
				if (flag)
				{
					stack = new Stack<EventDebuggerTrace>();
					this.m_StackOfProcessedEvent.Add(this.panel, stack);
				}
				List<EventDebuggerTrace> list;
				bool flag2 = !this.m_EventProcessedEvents.TryGetValue(this.panel, out list);
				if (flag2)
				{
					list = new List<EventDebuggerTrace>();
					this.m_EventProcessedEvents.Add(this.panel, list);
				}
				list.Add(eventDebuggerTrace);
				stack.Push(eventDebuggerTrace);
				Dictionary<long, int> dictionary;
				bool flag3 = !this.m_EventTypeProcessedCount.TryGetValue(this.panel, out dictionary);
				if (!flag3)
				{
					int num;
					bool flag4 = !dictionary.TryGetValue(eventDebuggerTrace.eventBase.eventTypeId, out num);
					if (flag4)
					{
						num = 0;
					}
					dictionary[eventDebuggerTrace.eventBase.eventTypeId] = num + 1;
				}
			}
		}

		private void AddEndProcessEvent(EventBase evt, long duration, IEventHandler mouseCapture)
		{
			bool suspended = this.suspended;
			if (!suspended)
			{
				bool flag = false;
				Stack<EventDebuggerTrace> stack;
				bool flag2 = this.m_StackOfProcessedEvent.TryGetValue(this.panel, out stack);
				if (flag2)
				{
					bool flag3 = stack.Count > 0;
					if (flag3)
					{
						EventDebuggerTrace eventDebuggerTrace = stack.Peek();
						bool flag4 = eventDebuggerTrace.eventBase.eventId == evt.eventId;
						if (flag4)
						{
							stack.Pop();
							eventDebuggerTrace.duration = duration;
							bool flag5 = eventDebuggerTrace.eventBase.target == null;
							if (flag5)
							{
								eventDebuggerTrace.eventBase.target = evt.target;
							}
							flag = true;
						}
					}
				}
				bool flag6 = !flag;
				if (flag6)
				{
					EventDebuggerTrace eventDebuggerTrace2 = new EventDebuggerTrace(this.panel, evt, duration, mouseCapture);
					List<EventDebuggerTrace> list;
					bool flag7 = !this.m_EventProcessedEvents.TryGetValue(this.panel, out list);
					if (flag7)
					{
						list = new List<EventDebuggerTrace>();
						this.m_EventProcessedEvents.Add(this.panel, list);
					}
					list.Add(eventDebuggerTrace2);
					Dictionary<long, int> dictionary;
					bool flag8 = !this.m_EventTypeProcessedCount.TryGetValue(this.panel, out dictionary);
					if (!flag8)
					{
						int num;
						bool flag9 = !dictionary.TryGetValue(eventDebuggerTrace2.eventBase.eventTypeId, out num);
						if (flag9)
						{
							num = 0;
						}
						dictionary[eventDebuggerTrace2.eventBase.eventTypeId] = num + 1;
					}
				}
			}
		}

		public static string GetObjectDisplayName(object obj, bool withHashCode = true)
		{
			bool flag = obj == null;
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				Type type = obj.GetType();
				string text = EventDebugger.GetTypeDisplayName(type);
				bool flag2 = obj is VisualElement;
				if (flag2)
				{
					VisualElement visualElement = obj as VisualElement;
					bool flag3 = !string.IsNullOrEmpty(visualElement.name);
					if (flag3)
					{
						text = text + "#" + visualElement.name;
					}
				}
				if (withHashCode)
				{
					text = text + " (" + obj.GetHashCode().ToString("x8") + ")";
				}
				result = text;
			}
			return result;
		}

		public static string GetTypeDisplayName(Type type)
		{
			return type.IsGenericType ? (type.Name.TrimEnd(new char[]
			{
				'`',
				'1'
			}) + "<" + type.GetGenericArguments()[0].Name + ">") : type.Name;
		}

		private Dictionary<IPanel, List<EventDebuggerCallTrace>> m_EventCalledObjects;

		private Dictionary<IPanel, List<EventDebuggerDefaultActionTrace>> m_EventDefaultActionObjects;

		private Dictionary<IPanel, List<EventDebuggerPathTrace>> m_EventPathObjects;

		private Dictionary<IPanel, List<EventDebuggerTrace>> m_EventProcessedEvents;

		private Dictionary<IPanel, Stack<EventDebuggerTrace>> m_StackOfProcessedEvent;

		private Dictionary<IPanel, Dictionary<long, int>> m_EventTypeProcessedCount;

		private readonly Dictionary<IPanel, long> m_ModificationCount;

		private readonly bool m_Log;

		internal struct HistogramRecord
		{
			public long count;

			public long duration;
		}
	}
}
