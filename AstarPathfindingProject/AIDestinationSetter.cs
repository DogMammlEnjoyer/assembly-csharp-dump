using System;
using UnityEngine;

namespace Pathfinding
{
	[UniqueComponent(tag = "ai.destination")]
	[HelpURL("http://arongranberg.com/astar/documentation/stable/class_pathfinding_1_1_a_i_destination_setter.php")]
	public class AIDestinationSetter : VersionedMonoBehaviour
	{
		private void OnEnable()
		{
			this.ai = base.GetComponent<IAstarAI>();
			if (this.ai != null)
			{
				IAstarAI astarAI = this.ai;
				astarAI.onSearchPath = (Action)Delegate.Combine(astarAI.onSearchPath, new Action(this.Update));
			}
		}

		private void OnDisable()
		{
			if (this.ai != null)
			{
				IAstarAI astarAI = this.ai;
				astarAI.onSearchPath = (Action)Delegate.Remove(astarAI.onSearchPath, new Action(this.Update));
			}
		}

		private void Update()
		{
			if (this.target != null && this.ai != null)
			{
				this.ai.destination = this.target.position;
			}
		}

		public Transform target;

		private IAstarAI ai;
	}
}
