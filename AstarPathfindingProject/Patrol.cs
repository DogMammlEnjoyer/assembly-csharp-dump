using System;
using UnityEngine;

namespace Pathfinding
{
	[UniqueComponent(tag = "ai.destination")]
	[HelpURL("http://arongranberg.com/astar/documentation/stable/class_pathfinding_1_1_patrol.php")]
	public class Patrol : VersionedMonoBehaviour
	{
		protected override void Awake()
		{
			base.Awake();
			this.agent = base.GetComponent<IAstarAI>();
		}

		private void Update()
		{
			if (this.targets.Length == 0)
			{
				return;
			}
			bool flag = false;
			if (this.agent.reachedEndOfPath && !this.agent.pathPending && float.IsPositiveInfinity(this.switchTime))
			{
				this.switchTime = Time.time + this.delay;
			}
			if (Time.time >= this.switchTime)
			{
				this.index++;
				flag = true;
				this.switchTime = float.PositiveInfinity;
			}
			this.index %= this.targets.Length;
			this.agent.destination = this.targets[this.index].position;
			if (flag)
			{
				this.agent.SearchPath();
			}
		}

		public Transform[] targets;

		public float delay;

		private int index;

		private IAstarAI agent;

		private float switchTime = float.PositiveInfinity;
	}
}
