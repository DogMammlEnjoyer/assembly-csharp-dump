using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Playables;
using UnityEngine.VFX.Utility;

namespace UnityEngine.VFX
{
	internal class VisualEffectControlTrackController
	{
		private void OnEnterChunk(int currentChunk)
		{
			VisualEffectControlTrackController.Chunk chunk = this.m_Chunks[currentChunk];
			if (chunk.reinitEnter)
			{
				this.m_Target.resetSeedOnPlay = false;
				this.m_Target.startSeed = chunk.startSeed;
				this.m_Target.Reinit(false);
				if (chunk.prewarmCount != 0U)
				{
					this.m_Target.SendEvent(chunk.prewarmEvent);
					this.m_Target.Simulate(chunk.prewarmDeltaTime, chunk.prewarmCount);
				}
			}
		}

		private void OnLeaveChunk(int previousChunkIndex, bool leavingGoingBeforeClip)
		{
			VisualEffectControlTrackController.Chunk chunk = this.m_Chunks[previousChunkIndex];
			if (chunk.reinitExit)
			{
				this.m_Target.Reinit(false);
			}
			else
			{
				this.ProcessNoScrubbingEvents(chunk, this.m_LastPlayableTime, leavingGoingBeforeClip ? double.NegativeInfinity : double.PositiveInfinity);
			}
			this.RestoreVFXState(chunk.scrubbing, chunk.reinitEnter);
		}

		private bool IsTimeInChunk(double time, int index)
		{
			VisualEffectControlTrackController.Chunk chunk = this.m_Chunks[index];
			return chunk.begin <= time && time < chunk.end;
		}

		public void Update(double playableTime, float deltaTime)
		{
			double num = playableTime + VisualEffectControlTrackController.kEpsilonEvent;
			bool flag = (double)deltaTime == 0.0;
			int num2 = int.MinValue;
			if (this.m_LastChunk != num2 && this.IsTimeInChunk(playableTime, this.m_LastChunk))
			{
				num2 = this.m_LastChunk;
			}
			if (num2 == -2147483648)
			{
				uint num3 = (uint)((this.m_LastChunk != int.MinValue) ? this.m_LastEvent : 0);
				uint num4 = num3;
				while ((ulong)num4 < (ulong)num3 + (ulong)((long)this.m_Chunks.Length))
				{
					int num5 = (int)((ulong)num4 % (ulong)((long)this.m_Chunks.Length));
					if (this.IsTimeInChunk(playableTime, num5))
					{
						num2 = num5;
						break;
					}
					num4 += 1U;
				}
			}
			bool flag2 = false;
			if (this.m_LastChunk != num2)
			{
				if (this.m_LastChunk != -2147483648)
				{
					bool leavingGoingBeforeClip = playableTime < this.m_Chunks[this.m_LastChunk].begin;
					this.OnLeaveChunk(this.m_LastChunk, leavingGoingBeforeClip);
				}
				if (num2 != -2147483648)
				{
					this.OnEnterChunk(num2);
					flag2 = true;
				}
				this.m_LastChunk = num2;
				this.m_LastEvent = int.MinValue;
			}
			if (num2 != -2147483648)
			{
				VisualEffectControlTrackController.Chunk chunk = this.m_Chunks[num2];
				if (chunk.scrubbing)
				{
					this.m_Target.pause = flag;
					double num6 = chunk.begin + (double)this.m_Target.time;
					if (!flag2)
					{
						num6 -= chunk.prewarmOffset;
					}
					if (playableTime >= this.m_LastPlayableTime)
					{
						if (Math.Abs(this.m_LastPlayableTime - num6) < (double)VFXManager.maxDeltaTime)
						{
							num6 = this.m_LastPlayableTime;
						}
					}
					else
					{
						num6 = chunk.begin;
						this.m_LastEvent = int.MinValue;
						this.OnEnterChunk(this.m_LastChunk);
					}
					double num7;
					if (flag)
					{
						num7 = playableTime;
					}
					else
					{
						num7 = playableTime - (double)VFXManager.fixedTimeStep;
					}
					if (this.m_LastPlayableTime < num6)
					{
						List<int> eventListIndexCache = this.m_EventListIndexCache;
						VisualEffectControlTrackController.GetEventsIndex(chunk, this.m_LastPlayableTime, num6, this.m_LastEvent, eventListIndexCache);
						foreach (int eventIndex in eventListIndexCache)
						{
							this.ProcessEvent(eventIndex, chunk);
						}
					}
					if (num6 < num7)
					{
						List<int> eventListIndexCache2 = this.m_EventListIndexCache;
						VisualEffectControlTrackController.GetEventsIndex(chunk, num6, num7, this.m_LastEvent, eventListIndexCache2);
						int count = eventListIndexCache2.Count;
						int num8 = 0;
						float maxScrubTime = VFXManager.maxScrubTime;
						float num9 = VFXManager.maxDeltaTime;
						if (num7 - num6 > (double)maxScrubTime)
						{
							num9 = (float)((num7 - num6) * (double)VFXManager.maxDeltaTime / (double)maxScrubTime);
						}
						while (num6 < num7)
						{
							int num10 = int.MinValue;
							uint num11;
							if (num8 < count)
							{
								num10 = eventListIndexCache2[num8++];
								num11 = (uint)((chunk.events[num10].time - num6) / (double)num9);
							}
							else
							{
								num11 = (uint)((num7 - num6) / (double)num9);
								if (num11 == 0U)
								{
									break;
								}
							}
							if (num11 != 0U)
							{
								this.m_Target.Simulate(num9, num11);
								num6 += (double)(num9 * num11);
							}
							this.ProcessEvent(num10, chunk);
						}
					}
					if (num6 >= num)
					{
						goto IL_35E;
					}
					List<int> eventListIndexCache3 = this.m_EventListIndexCache;
					VisualEffectControlTrackController.GetEventsIndex(chunk, num6, num, this.m_LastEvent, eventListIndexCache3);
					using (List<int>.Enumerator enumerator = eventListIndexCache3.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							int eventIndex2 = enumerator.Current;
							this.ProcessEvent(eventIndex2, chunk);
						}
						goto IL_35E;
					}
				}
				this.m_Target.pause = false;
				this.ProcessNoScrubbingEvents(chunk, this.m_LastPlayableTime, num);
			}
			IL_35E:
			this.m_LastPlayableTime = playableTime;
		}

		private void ProcessNoScrubbingEvents(VisualEffectControlTrackController.Chunk chunk, double oldTime, double newTime)
		{
			if (newTime < oldTime)
			{
				List<int> eventListIndexCache = this.m_EventListIndexCache;
				VisualEffectControlTrackController.GetEventsIndex(chunk, newTime, oldTime, int.MinValue, eventListIndexCache);
				if (eventListIndexCache.Count > 0)
				{
					for (int i = eventListIndexCache.Count - 1; i >= 0; i--)
					{
						int num = eventListIndexCache[i];
						VisualEffectControlTrackController.Event @event = chunk.events[num];
						if (@event.clipType == VisualEffectControlTrackController.Event.ClipType.Enter)
						{
							this.ProcessEvent(chunk.clips[@event.clipIndex].exit, chunk);
						}
						else if (@event.clipType == VisualEffectControlTrackController.Event.ClipType.Exit)
						{
							this.ProcessEvent(chunk.clips[@event.clipIndex].enter, chunk);
						}
					}
					this.m_LastEvent = int.MinValue;
					return;
				}
			}
			else
			{
				List<int> eventListIndexCache2 = this.m_EventListIndexCache;
				VisualEffectControlTrackController.GetEventsIndex(chunk, oldTime, newTime, this.m_LastEvent, eventListIndexCache2);
				foreach (int eventIndex in eventListIndexCache2)
				{
					this.ProcessEvent(eventIndex, chunk);
				}
			}
		}

		private void ProcessEvent(int eventIndex, VisualEffectControlTrackController.Chunk currentChunk)
		{
			if (eventIndex == -2147483648)
			{
				return;
			}
			this.m_LastEvent = eventIndex;
			VisualEffectControlTrackController.Event @event = currentChunk.events[eventIndex];
			this.m_Target.SendEvent(@event.nameId, @event.attribute);
		}

		private static void GetEventsIndex(VisualEffectControlTrackController.Chunk chunk, double minTime, double maxTime, int lastIndex, List<int> eventListIndex)
		{
			eventListIndex.Clear();
			for (int i = (lastIndex == int.MinValue) ? 0 : (lastIndex + 1); i < chunk.events.Length; i++)
			{
				VisualEffectControlTrackController.Event @event = chunk.events[i];
				if (@event.time >= maxTime)
				{
					break;
				}
				if (minTime <= @event.time)
				{
					eventListIndex.Add(i);
				}
			}
		}

		private static VFXEventAttribute ComputeAttribute(VisualEffect vfx, EventAttributes attributes)
		{
			if (attributes.content == null)
			{
				return null;
			}
			VFXEventAttribute vfxeventAttribute = vfx.CreateVFXEventAttribute();
			bool flag = false;
			foreach (EventAttribute eventAttribute in attributes.content)
			{
				if (eventAttribute != null && eventAttribute.ApplyToVFX(vfxeventAttribute))
				{
					flag = true;
				}
			}
			if (!flag)
			{
				return null;
			}
			return vfxeventAttribute;
		}

		private static IEnumerable<VisualEffectControlTrackController.Event> ComputeRuntimeEvent(VisualEffectControlPlayableBehaviour behavior, VisualEffect vfx)
		{
			IEnumerable<VisualEffectPlayableSerializedEvent> eventNormalizedSpace = VFXTimeSpaceHelper.GetEventNormalizedSpace(PlayableTimeSpace.Absolute, behavior);
			foreach (VisualEffectPlayableSerializedEvent visualEffectPlayableSerializedEvent in eventNormalizedSpace)
			{
				double time = Math.Max(behavior.clipStart, Math.Min(behavior.clipEnd, visualEffectPlayableSerializedEvent.time));
				yield return new VisualEffectControlTrackController.Event
				{
					attribute = VisualEffectControlTrackController.ComputeAttribute(vfx, visualEffectPlayableSerializedEvent.eventAttributes),
					nameId = visualEffectPlayableSerializedEvent.name,
					time = time,
					clipIndex = -1,
					clipType = VisualEffectControlTrackController.Event.ClipType.None
				};
			}
			IEnumerator<VisualEffectPlayableSerializedEvent> enumerator = null;
			yield break;
			yield break;
		}

		public void RestoreVFXState(bool restorePause = true, bool restoreSeedState = true)
		{
			if (this.m_Target == null)
			{
				return;
			}
			if (restorePause)
			{
				this.m_Target.pause = false;
			}
			if (restoreSeedState)
			{
				this.m_Target.startSeed = this.m_BackupStartSeed;
				this.m_Target.resetSeedOnPlay = this.m_BackupReseedOnPlay;
			}
		}

		public void Init(Playable playable, VisualEffect vfx, VisualEffectControlTrack parentTrack)
		{
			this.m_Target = vfx;
			this.m_BackupStartSeed = this.m_Target.startSeed;
			this.m_BackupReseedOnPlay = this.m_Target.resetSeedOnPlay;
			Stack<ValueTuple<VisualEffectControlTrackController.Chunk, List<VisualEffectControlTrackController.Event>, List<VisualEffectControlTrackController.Clip>>> stack = new Stack<ValueTuple<VisualEffectControlTrackController.Chunk, List<VisualEffectControlTrackController.Event>, List<VisualEffectControlTrackController.Clip>>>();
			int inputCount = playable.GetInputCount<Playable>();
			List<VisualEffectControlPlayableBehaviour> list = new List<VisualEffectControlPlayableBehaviour>();
			for (int i = 0; i < inputCount; i++)
			{
				Playable input = playable.GetInput(i);
				if (!(input.GetPlayableType() != typeof(VisualEffectControlPlayableBehaviour)))
				{
					VisualEffectControlPlayableBehaviour behaviour = ((ScriptPlayable<T>)input).GetBehaviour();
					if (behaviour != null)
					{
						list.Add(behaviour);
					}
				}
			}
			list.Sort(new VisualEffectControlTrackController.VisualEffectControlPlayableBehaviourComparer());
			foreach (VisualEffectControlPlayableBehaviour visualEffectControlPlayableBehaviour in list)
			{
				if (stack.Count == 0 || visualEffectControlPlayableBehaviour.clipStart > stack.Peek().Item1.end || visualEffectControlPlayableBehaviour.scrubbing != stack.Peek().Item1.scrubbing || (!visualEffectControlPlayableBehaviour.scrubbing && (visualEffectControlPlayableBehaviour.reinitEnter || stack.Peek().Item1.reinitExit)) || visualEffectControlPlayableBehaviour.startSeed != stack.Peek().Item1.startSeed || visualEffectControlPlayableBehaviour.prewarmStepCount != 0U)
				{
					Stack<ValueTuple<VisualEffectControlTrackController.Chunk, List<VisualEffectControlTrackController.Event>, List<VisualEffectControlTrackController.Clip>>> stack2 = stack;
					ValueTuple<VisualEffectControlTrackController.Chunk, List<VisualEffectControlTrackController.Event>, List<VisualEffectControlTrackController.Clip>> item = default(ValueTuple<VisualEffectControlTrackController.Chunk, List<VisualEffectControlTrackController.Event>, List<VisualEffectControlTrackController.Clip>>);
					VisualEffectControlTrackController.Chunk item2 = default(VisualEffectControlTrackController.Chunk);
					item2.begin = visualEffectControlPlayableBehaviour.clipStart;
					item2.scrubbing = visualEffectControlPlayableBehaviour.scrubbing;
					item2.startSeed = visualEffectControlPlayableBehaviour.startSeed;
					item2.reinitEnter = visualEffectControlPlayableBehaviour.reinitEnter;
					item2.reinitExit = visualEffectControlPlayableBehaviour.reinitExit;
					item2.prewarmCount = visualEffectControlPlayableBehaviour.prewarmStepCount;
					item2.prewarmDeltaTime = visualEffectControlPlayableBehaviour.prewarmDeltaTime;
					ExposedProperty prewarmEvent = visualEffectControlPlayableBehaviour.prewarmEvent;
					item2.prewarmEvent = ((prewarmEvent != null) ? prewarmEvent : 0);
					item2.prewarmOffset = visualEffectControlPlayableBehaviour.prewarmStepCount * (double)visualEffectControlPlayableBehaviour.prewarmDeltaTime;
					item.Item1 = item2;
					item.Item2 = new List<VisualEffectControlTrackController.Event>();
					item.Item3 = new List<VisualEffectControlTrackController.Clip>();
					stack2.Push(item);
				}
				ValueTuple<VisualEffectControlTrackController.Chunk, List<VisualEffectControlTrackController.Event>, List<VisualEffectControlTrackController.Clip>> valueTuple = stack.Pop();
				valueTuple.Item1.end = visualEffectControlPlayableBehaviour.clipEnd;
				List<VisualEffectControlTrackController.Event> list2 = new List<VisualEffectControlTrackController.Event>(VisualEffectControlTrackController.ComputeRuntimeEvent(visualEffectControlPlayableBehaviour, vfx));
				if (!valueTuple.Item1.scrubbing)
				{
					List<ValueTuple<VisualEffectControlTrackController.Event, int>> list3 = new List<ValueTuple<VisualEffectControlTrackController.Event, int>>();
					for (int j = 0; j < list2.Count; j++)
					{
						list3.Add(new ValueTuple<VisualEffectControlTrackController.Event, int>(list2[j], j));
					}
					list3.Sort(([TupleElementNames(new string[]
					{
						"evt",
						"sourceIndex"
					})] ValueTuple<VisualEffectControlTrackController.Event, int> x, [TupleElementNames(new string[]
					{
						"evt",
						"sourceIndex"
					})] ValueTuple<VisualEffectControlTrackController.Event, int> y) => x.Item1.time.CompareTo(y.Item1.time));
					VisualEffectControlTrackController.Clip[] array = new VisualEffectControlTrackController.Clip[visualEffectControlPlayableBehaviour.clipEventsCount];
					List<VisualEffectControlTrackController.Event> list4 = new List<VisualEffectControlTrackController.Event>();
					for (int k = 0; k < list3.Count; k++)
					{
						VisualEffectControlTrackController.Event item3 = list3[k].Item1;
						int item4 = list3[k].Item2;
						if ((long)item4 < (long)((ulong)(visualEffectControlPlayableBehaviour.clipEventsCount * 2U)))
						{
							int num = valueTuple.Item2.Count + k;
							int num2 = item4 / 2;
							item3.clipIndex = num2 + valueTuple.Item3.Count;
							if (item4 % 2 == 0)
							{
								item3.clipType = VisualEffectControlTrackController.Event.ClipType.Enter;
								array[num2].enter = num;
							}
							else
							{
								item3.clipType = VisualEffectControlTrackController.Event.ClipType.Exit;
								array[num2].exit = num;
							}
							list4.Add(item3);
						}
						else
						{
							list4.Add(item3);
						}
					}
					valueTuple.Item3.AddRange(array);
					valueTuple.Item2.AddRange(list4);
				}
				else
				{
					list2.Sort((VisualEffectControlTrackController.Event x, VisualEffectControlTrackController.Event y) => x.time.CompareTo(y.time));
					valueTuple.Item2.AddRange(list2);
				}
				stack.Push(valueTuple);
			}
			this.m_Chunks = new VisualEffectControlTrackController.Chunk[stack.Count];
			for (int l = 0; l < this.m_Chunks.Length; l++)
			{
				ValueTuple<VisualEffectControlTrackController.Chunk, List<VisualEffectControlTrackController.Event>, List<VisualEffectControlTrackController.Clip>> valueTuple2 = stack.Pop();
				this.m_Chunks[l] = valueTuple2.Item1;
				this.m_Chunks[l].clips = valueTuple2.Item3.ToArray();
				this.m_Chunks[l].events = valueTuple2.Item2.ToArray();
			}
		}

		public void Release()
		{
			this.RestoreVFXState(true, true);
		}

		private const int kErrorIndex = -2147483648;

		private int m_LastChunk = int.MinValue;

		private int m_LastEvent = int.MinValue;

		private double m_LastPlayableTime = double.MinValue;

		private List<int> m_EventListIndexCache = new List<int>();

		private VisualEffect m_Target;

		private bool m_BackupReseedOnPlay;

		private uint m_BackupStartSeed;

		private VisualEffectControlTrackController.Chunk[] m_Chunks;

		private static readonly double kEpsilonEvent = 1E-12;

		private struct Event
		{
			public int nameId;

			public VFXEventAttribute attribute;

			public double time;

			public int clipIndex;

			public VisualEffectControlTrackController.Event.ClipType clipType;

			public enum ClipType
			{
				None,
				Enter,
				Exit
			}
		}

		private struct Clip
		{
			public int enter;

			public int exit;
		}

		private struct Chunk
		{
			public bool scrubbing;

			public bool reinitEnter;

			public bool reinitExit;

			public uint startSeed;

			public double begin;

			public double end;

			public uint prewarmCount;

			public float prewarmDeltaTime;

			public double prewarmOffset;

			public int prewarmEvent;

			public VisualEffectControlTrackController.Event[] events;

			public VisualEffectControlTrackController.Clip[] clips;
		}

		private class VisualEffectControlPlayableBehaviourComparer : IComparer<VisualEffectControlPlayableBehaviour>
		{
			public int Compare(VisualEffectControlPlayableBehaviour x, VisualEffectControlPlayableBehaviour y)
			{
				return x.clipStart.CompareTo(y.clipStart);
			}
		}
	}
}
