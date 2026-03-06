using System;
using UnityEngine;

namespace Pathfinding.Examples
{
	[HelpURL("http://arongranberg.com/astar/documentation/stable/class_pathfinding_1_1_examples_1_1_mine_bot_animation.php")]
	public class MineBotAnimation : VersionedMonoBehaviour
	{
		protected override void Awake()
		{
			base.Awake();
			this.ai = base.GetComponent<IAstarAI>();
			this.tr = base.GetComponent<Transform>();
		}

		private void OnTargetReached()
		{
			if (this.endOfPathEffect != null && Vector3.Distance(this.tr.position, this.lastTarget) > 1f)
			{
				Object.Instantiate<GameObject>(this.endOfPathEffect, this.tr.position, this.tr.rotation);
				this.lastTarget = this.tr.position;
			}
		}

		protected void Update()
		{
			if (this.ai.reachedEndOfPath)
			{
				if (!this.isAtDestination)
				{
					this.OnTargetReached();
				}
				this.isAtDestination = true;
			}
			else
			{
				this.isAtDestination = false;
			}
			Vector3 vector = this.tr.InverseTransformDirection(this.ai.velocity);
			vector.y = 0f;
			this.anim.SetFloat("NormalizedSpeed", vector.magnitude / this.anim.transform.lossyScale.x);
		}

		public Animator anim;

		public GameObject endOfPathEffect;

		private bool isAtDestination;

		private IAstarAI ai;

		private Transform tr;

		protected Vector3 lastTarget;
	}
}
