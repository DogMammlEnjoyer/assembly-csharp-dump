using System;
using System.Collections.Generic;

namespace UnityEngine.VFX
{
	internal static class VFXTimeSpaceHelper
	{
		public static IEnumerable<VisualEffectPlayableSerializedEvent> GetEventNormalizedSpace(PlayableTimeSpace space, VisualEffectControlPlayableBehaviour source)
		{
			return VFXTimeSpaceHelper.GetEventNormalizedSpace(space, source.events, source.clipStart, source.clipEnd);
		}

		private static IEnumerable<VisualEffectPlayableSerializedEvent> CollectClipEvents(VisualEffectControlClip source)
		{
			if (source.clipEvents != null)
			{
				foreach (VisualEffectControlClip.ClipEvent clipEvent in source.clipEvents)
				{
					VisualEffectPlayableSerializedEvent visualEffectPlayableSerializedEvent = clipEvent.enter;
					VisualEffectPlayableSerializedEvent eventExit = clipEvent.exit;
					visualEffectPlayableSerializedEvent.editorColor = (eventExit.editorColor = clipEvent.editorColor);
					yield return visualEffectPlayableSerializedEvent;
					yield return eventExit;
					eventExit = default(VisualEffectPlayableSerializedEvent);
				}
				List<VisualEffectControlClip.ClipEvent>.Enumerator enumerator = default(List<VisualEffectControlClip.ClipEvent>.Enumerator);
			}
			yield break;
			yield break;
		}

		public static IEnumerable<VisualEffectPlayableSerializedEvent> GetEventNormalizedSpace(PlayableTimeSpace space, VisualEffectControlClip source, bool clipEvents)
		{
			IEnumerable<VisualEffectPlayableSerializedEvent> events;
			if (clipEvents)
			{
				events = VFXTimeSpaceHelper.CollectClipEvents(source);
			}
			else
			{
				events = source.singleEvents;
			}
			return VFXTimeSpaceHelper.GetEventNormalizedSpace(space, events, source.clipStart, source.clipEnd);
		}

		private static IEnumerable<VisualEffectPlayableSerializedEvent> GetEventNormalizedSpace(PlayableTimeSpace space, IEnumerable<VisualEffectPlayableSerializedEvent> events, double clipStart, double clipEnd)
		{
			foreach (VisualEffectPlayableSerializedEvent visualEffectPlayableSerializedEvent in events)
			{
				VisualEffectPlayableSerializedEvent visualEffectPlayableSerializedEvent2 = visualEffectPlayableSerializedEvent;
				visualEffectPlayableSerializedEvent2.timeSpace = space;
				visualEffectPlayableSerializedEvent2.time = VFXTimeSpaceHelper.GetTimeInSpace(visualEffectPlayableSerializedEvent.timeSpace, visualEffectPlayableSerializedEvent.time, space, clipStart, clipEnd);
				yield return visualEffectPlayableSerializedEvent2;
			}
			IEnumerator<VisualEffectPlayableSerializedEvent> enumerator = null;
			yield break;
			yield break;
		}

		public static double GetTimeInSpace(PlayableTimeSpace srcSpace, double srcTime, PlayableTimeSpace dstSpace, double clipStart, double clipEnd)
		{
			if (srcSpace == dstSpace)
			{
				return srcTime;
			}
			if (dstSpace == PlayableTimeSpace.AfterClipStart)
			{
				switch (srcSpace)
				{
				case PlayableTimeSpace.BeforeClipEnd:
					return clipEnd - srcTime - clipStart;
				case PlayableTimeSpace.Percentage:
					return (clipEnd - clipStart) * (srcTime / 100.0);
				case PlayableTimeSpace.Absolute:
					return srcTime - clipStart;
				}
			}
			else if (dstSpace == PlayableTimeSpace.BeforeClipEnd)
			{
				switch (srcSpace)
				{
				case PlayableTimeSpace.AfterClipStart:
					return clipEnd - srcTime - clipStart;
				case PlayableTimeSpace.Percentage:
					return clipEnd - clipStart - (clipEnd - clipStart) * (srcTime / 100.0);
				case PlayableTimeSpace.Absolute:
					return clipEnd - srcTime;
				}
			}
			else if (dstSpace == PlayableTimeSpace.Percentage)
			{
				switch (srcSpace)
				{
				case PlayableTimeSpace.AfterClipStart:
					return 100.0 * srcTime / (clipEnd - clipStart);
				case PlayableTimeSpace.BeforeClipEnd:
					return 100.0 * (clipEnd - srcTime - clipStart) / (clipEnd - clipStart);
				case PlayableTimeSpace.Absolute:
					return 100.0 * (srcTime - clipStart) / (clipEnd - clipStart);
				}
			}
			else if (dstSpace == PlayableTimeSpace.Absolute)
			{
				switch (srcSpace)
				{
				case PlayableTimeSpace.AfterClipStart:
					return clipStart + srcTime;
				case PlayableTimeSpace.BeforeClipEnd:
					return clipEnd - srcTime;
				case PlayableTimeSpace.Percentage:
					return clipStart + (clipEnd - clipStart) * (srcTime / 100.0);
				}
			}
			throw new NotImplementedException(srcSpace.ToString() + " to " + dstSpace.ToString());
		}
	}
}
