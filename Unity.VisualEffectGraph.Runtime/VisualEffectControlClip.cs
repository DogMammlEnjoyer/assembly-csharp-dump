using System;
using System.Collections.Generic;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.VFX.Utility;

namespace UnityEngine.VFX
{
	[Serializable]
	internal class VisualEffectControlClip : PlayableAsset, ITimelineClipAsset
	{
		public ClipCaps clipCaps
		{
			get
			{
				return ClipCaps.None;
			}
		}

		public double clipStart { get; set; }

		public double clipEnd { get; set; }

		public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
		{
			ScriptPlayable<VisualEffectControlPlayableBehaviour> playable = ScriptPlayable<VisualEffectControlPlayableBehaviour>.Create(graph, 0);
			VisualEffectControlPlayableBehaviour behaviour = playable.GetBehaviour();
			behaviour.clipStart = this.clipStart;
			behaviour.clipEnd = this.clipEnd;
			behaviour.scrubbing = this.scrubbing;
			behaviour.startSeed = this.startSeed;
			if (this.scrubbing)
			{
				behaviour.reinitEnter = true;
				behaviour.reinitExit = true;
			}
			else
			{
				switch (this.reinit)
				{
				case VisualEffectControlClip.ReinitMode.None:
					behaviour.reinitEnter = false;
					behaviour.reinitExit = false;
					break;
				case VisualEffectControlClip.ReinitMode.OnExitClip:
					behaviour.reinitEnter = false;
					behaviour.reinitExit = true;
					break;
				case VisualEffectControlClip.ReinitMode.OnEnterClip:
					behaviour.reinitEnter = true;
					behaviour.reinitExit = false;
					break;
				case VisualEffectControlClip.ReinitMode.OnEnterOrExitClip:
					behaviour.reinitEnter = true;
					behaviour.reinitExit = true;
					break;
				}
			}
			if (this.clipEvents == null)
			{
				this.clipEvents = new List<VisualEffectControlClip.ClipEvent>();
			}
			if (this.singleEvents == null)
			{
				this.singleEvents = new List<VisualEffectPlayableSerializedEvent>();
			}
			behaviour.clipEventsCount = (uint)this.clipEvents.Count;
			List<VisualEffectPlayableSerializedEvent> list = new List<VisualEffectPlayableSerializedEvent>();
			foreach (VisualEffectControlClip.ClipEvent clipEvent in this.clipEvents)
			{
				list.Add(clipEvent.enter);
				list.Add(clipEvent.exit);
			}
			foreach (VisualEffectPlayableSerializedEvent item in this.singleEvents)
			{
				list.Add(item);
			}
			behaviour.events = list.ToArray();
			if (!this.prewarm.enable || !behaviour.reinitEnter || this.prewarm.eventName == null || string.IsNullOrEmpty((string)this.prewarm.eventName))
			{
				behaviour.prewarmStepCount = 0U;
				behaviour.prewarmDeltaTime = 0f;
				behaviour.prewarmEvent = null;
			}
			else
			{
				behaviour.prewarmStepCount = this.prewarm.stepCount;
				behaviour.prewarmDeltaTime = this.prewarm.deltaTime;
				behaviour.prewarmEvent = this.prewarm.eventName;
			}
			return playable;
		}

		[NotKeyable]
		public bool scrubbing = true;

		[NotKeyable]
		public uint startSeed;

		[NotKeyable]
		public VisualEffectControlClip.ReinitMode reinit = VisualEffectControlClip.ReinitMode.OnEnterOrExitClip;

		[NotKeyable]
		public VisualEffectControlClip.PrewarmClipSettings prewarm = new VisualEffectControlClip.PrewarmClipSettings
		{
			enable = false,
			stepCount = 20U,
			deltaTime = 0.05f,
			eventName = "OnPlay"
		};

		[NotKeyable]
		public List<VisualEffectControlClip.ClipEvent> clipEvents = new List<VisualEffectControlClip.ClipEvent>
		{
			new VisualEffectControlClip.ClipEvent
			{
				editorColor = VisualEffectControlClip.ClipEvent.defaultEditorColor,
				enter = new VisualEffectPlayableSerializedEventNoColor
				{
					name = "OnPlay",
					time = 0.0,
					timeSpace = PlayableTimeSpace.AfterClipStart,
					eventAttributes = new EventAttributes
					{
						content = Array.Empty<EventAttribute>()
					}
				},
				exit = new VisualEffectPlayableSerializedEventNoColor
				{
					name = "OnStop",
					time = 0.0,
					timeSpace = PlayableTimeSpace.BeforeClipEnd,
					eventAttributes = new EventAttributes
					{
						content = Array.Empty<EventAttribute>()
					}
				}
			}
		};

		[NotKeyable]
		public List<VisualEffectPlayableSerializedEvent> singleEvents = new List<VisualEffectPlayableSerializedEvent>();

		public enum ReinitMode
		{
			None,
			OnExitClip,
			OnEnterClip,
			OnEnterOrExitClip
		}

		[Serializable]
		public struct PrewarmClipSettings
		{
			public bool enable;

			public uint stepCount;

			public float deltaTime;

			public ExposedProperty eventName;
		}

		[Serializable]
		public struct ClipEvent
		{
			public static Color defaultEditorColor = new Color32(123, 158, 5, byte.MaxValue);

			public Color editorColor;

			public VisualEffectPlayableSerializedEventNoColor enter;

			public VisualEffectPlayableSerializedEventNoColor exit;
		}
	}
}
