using System;
using UnityEngine;
using UnityEngine.UI;

namespace Pathfinding.Examples
{
	[RequireComponent(typeof(Animator))]
	[HelpURL("http://arongranberg.com/astar/documentation/stable/class_pathfinding_1_1_examples_1_1_hexagon_trigger.php")]
	public class HexagonTrigger : MonoBehaviour
	{
		private void Awake()
		{
			this.anim = base.GetComponent<Animator>();
			this.button.interactable = false;
		}

		private void OnTriggerEnter(Collider coll)
		{
			TurnBasedAI componentInParent = coll.GetComponentInParent<TurnBasedAI>();
			GraphNode node = AstarPath.active.GetNearest(base.transform.position).node;
			if (componentInParent != null && componentInParent.targetNode == node)
			{
				this.button.interactable = true;
				this.visible = true;
				this.anim.CrossFade("show", 0.1f);
			}
		}

		private void OnTriggerExit(Collider coll)
		{
			if (coll.GetComponentInParent<TurnBasedAI>() != null && this.visible)
			{
				this.button.interactable = false;
				this.anim.CrossFade("hide", 0.1f);
			}
		}

		public Button button;

		private Animator anim;

		private bool visible;
	}
}
