using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Playables;

namespace DocCodeExamples
{
	[RequireComponent(typeof(RigBuilder))]
	public class CustomPlayableGraphEvaluator : MonoBehaviour
	{
		private void OnEnable()
		{
			this.m_RigBuilder = base.GetComponent<RigBuilder>();
			this.m_PlayableGraph = PlayableGraph.Create();
			this.m_PlayableGraph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
			this.m_RigBuilder.Build(this.m_PlayableGraph);
		}

		private void OnDisable()
		{
			if (this.m_PlayableGraph.IsValid())
			{
				this.m_PlayableGraph.Destroy();
			}
		}

		private void LateUpdate()
		{
			this.m_RigBuilder.SyncLayers();
			this.m_PlayableGraph.Evaluate(Time.deltaTime);
		}

		private RigBuilder m_RigBuilder;

		private PlayableGraph m_PlayableGraph;
	}
}
