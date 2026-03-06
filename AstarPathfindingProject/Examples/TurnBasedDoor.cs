using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding.Examples
{
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(SingleNodeBlocker))]
	[HelpURL("http://arongranberg.com/astar/documentation/stable/class_pathfinding_1_1_examples_1_1_turn_based_door.php")]
	public class TurnBasedDoor : MonoBehaviour
	{
		private void Awake()
		{
			this.animator = base.GetComponent<Animator>();
			this.blocker = base.GetComponent<SingleNodeBlocker>();
		}

		private void Start()
		{
			this.blocker.BlockAtCurrentPosition();
			this.animator.CrossFade("close", 0.2f);
		}

		public void Close()
		{
			base.StartCoroutine(this.WaitAndClose());
		}

		private IEnumerator WaitAndClose()
		{
			List<SingleNodeBlocker> selector = new List<SingleNodeBlocker>
			{
				this.blocker
			};
			GraphNode node = AstarPath.active.GetNearest(base.transform.position).node;
			if (this.blocker.manager.NodeContainsAnyExcept(node, selector))
			{
				this.animator.CrossFade("blocked", 0.2f);
			}
			while (this.blocker.manager.NodeContainsAnyExcept(node, selector))
			{
				yield return null;
			}
			this.open = false;
			this.animator.CrossFade("close", 0.2f);
			this.blocker.BlockAtCurrentPosition();
			yield break;
		}

		public void Open()
		{
			base.StopAllCoroutines();
			this.animator.CrossFade("open", 0.2f);
			this.open = true;
			this.blocker.Unblock();
		}

		public void Toggle()
		{
			if (this.open)
			{
				this.Close();
				return;
			}
			this.Open();
		}

		private Animator animator;

		private SingleNodeBlocker blocker;

		private bool open;
	}
}
