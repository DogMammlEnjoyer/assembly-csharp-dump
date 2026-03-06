using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
internal class VisualEffectActivationClip : PlayableAsset, ITimelineClipAsset
{
	public ClipCaps clipCaps
	{
		get
		{
			return ClipCaps.None;
		}
	}

	public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
	{
		ScriptPlayable<VisualEffectActivationBehaviour> playable = ScriptPlayable<VisualEffectActivationBehaviour>.Create(graph, this.activationBehavior, 0);
		playable.GetBehaviour();
		return playable;
	}

	public VisualEffectActivationBehaviour activationBehavior = new VisualEffectActivationBehaviour();
}
