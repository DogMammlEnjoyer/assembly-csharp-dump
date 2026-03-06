using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Unity.Cinemachine
{
	public sealed class CinemachineShot : PlayableAsset, IPropertyPreview
	{
		public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
		{
			ScriptPlayable<CinemachineShotPlayable> playable = ScriptPlayable<CinemachineShotPlayable>.Create(graph, 0);
			playable.GetBehaviour().VirtualCamera = this.VirtualCamera.Resolve(graph.GetResolver());
			return playable;
		}

		public void GatherProperties(PlayableDirector director, IPropertyCollector driver)
		{
			driver.AddFromName<Transform>("m_LocalPosition.x");
			driver.AddFromName<Transform>("m_LocalPosition.y");
			driver.AddFromName<Transform>("m_LocalPosition.z");
			driver.AddFromName<Transform>("m_LocalRotation.x");
			driver.AddFromName<Transform>("m_LocalRotation.y");
			driver.AddFromName<Transform>("m_LocalRotation.z");
			driver.AddFromName<Transform>("m_LocalRotation.w");
			driver.AddFromName<Camera>("field of view");
			driver.AddFromName<Camera>("near clip plane");
			driver.AddFromName<Camera>("far clip plane");
		}

		[Tooltip("The name to display on the track.  If empty, the CmCamera's name will be used.")]
		public string DisplayName;

		[Tooltip("The Cinemachine camera to use for this shot")]
		public ExposedReference<CinemachineVirtualCameraBase> VirtualCamera;
	}
}
