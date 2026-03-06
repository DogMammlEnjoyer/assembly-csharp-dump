using System;
using System.Linq;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace UnityEngine.VFX
{
	[TrackColor(0.5990566f, 0.9038978f, 1f)]
	[TrackClipType(typeof(VisualEffectControlClip))]
	[TrackBindingType(typeof(VisualEffect))]
	internal class VisualEffectControlTrack : TrackAsset
	{
		public bool IsUpToDate()
		{
			return this.m_VFXVersion == 1;
		}

		protected override void OnBeforeTrackSerialize()
		{
			base.OnBeforeTrackSerialize();
			if (base.GetClips().All((TimelineClip x) => x.asset is VisualEffectControlClip))
			{
				this.m_VFXVersion = 1;
			}
		}

		public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
		{
			foreach (TimelineClip timelineClip in base.GetClips())
			{
				VisualEffectControlClip visualEffectControlClip = timelineClip.asset as VisualEffectControlClip;
				if (visualEffectControlClip != null)
				{
					visualEffectControlClip.clipStart = timelineClip.start;
					visualEffectControlClip.clipEnd = timelineClip.end;
				}
			}
			ScriptPlayable<VisualEffectControlTrackMixerBehaviour> playable = ScriptPlayable<VisualEffectControlTrackMixerBehaviour>.Create(graph, inputCount);
			VisualEffectControlTrackMixerBehaviour behaviour = playable.GetBehaviour();
			bool reinitWithBinding = this.reinit == VisualEffectControlTrack.ReinitMode.OnBindingEnable || this.reinit == VisualEffectControlTrack.ReinitMode.OnBindingEnableOrDisable;
			bool reinitWithUnbinding = this.reinit == VisualEffectControlTrack.ReinitMode.OnBindingDisable || this.reinit == VisualEffectControlTrack.ReinitMode.OnBindingEnableOrDisable;
			behaviour.Init(this, reinitWithBinding, reinitWithUnbinding);
			return playable;
		}

		public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
		{
			if (director.GetGenericBinding(this) is VisualEffect)
			{
				base.GatherProperties(director, driver);
			}
		}

		private const int kCurrentVersion = 1;

		[SerializeField]
		[HideInInspector]
		private int m_VFXVersion;

		[SerializeField]
		[NotKeyable]
		public VisualEffectControlTrack.ReinitMode reinit = VisualEffectControlTrack.ReinitMode.OnBindingEnableOrDisable;

		public enum ReinitMode
		{
			None,
			OnBindingEnable,
			OnBindingDisable,
			OnBindingEnableOrDisable
		}
	}
}
