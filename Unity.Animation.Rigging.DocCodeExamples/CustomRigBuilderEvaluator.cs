using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Playables;

namespace DocCodeExamples
{
	[RequireComponent(typeof(RigBuilder))]
	public class CustomRigBuilderEvaluator : MonoBehaviour
	{
		private void OnEnable()
		{
			this.m_RigBuilder = base.GetComponent<RigBuilder>();
			this.m_RigBuilder.enabled = false;
			if (this.m_RigBuilder.Build())
			{
				this.m_RigBuilder.graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
			}
		}

		private void LateUpdate()
		{
			this.m_RigBuilder.Evaluate(Time.deltaTime);
		}

		private RigBuilder m_RigBuilder;
	}
}
