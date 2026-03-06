using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Unity.Cinemachine
{
	[TrackClipType(typeof(CinemachineShot))]
	[TrackBindingType(typeof(CinemachineBrain), TrackBindingFlags.None)]
	[TrackColor(0.53f, 0f, 0.08f)]
	[Serializable]
	public class CinemachineTrack : TrackAsset
	{
		public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
		{
			ScriptPlayable<CinemachinePlayableMixer> playable = ScriptPlayable<CinemachinePlayableMixer>.Create(graph, 0);
			playable.SetInputCount(inputCount);
			playable.GetBehaviour().Priority = this.TrackPriority;
			return playable;
		}

		[Tooltip("The priority controls the precedence that this track takes over other CinemachineTracks.  Tracks with higher priority will override tracks with lower priority.  If two simultaneous tracks have the same priority, then the more-recently instanced track will take precedence.  Track priority is unrelated to Cinemachine Camera priority.")]
		public int TrackPriority;
	}
}
