using System;
using UnityEngine;

namespace Pathfinding.Examples
{
	[HelpURL("http://arongranberg.com/astar/documentation/stable/class_pathfinding_1_1_examples_1_1_example_mover.php")]
	public class ExampleMover : MonoBehaviour
	{
		private void Awake()
		{
			this.agent = base.GetComponent<RVOExampleAgent>();
		}

		private void Start()
		{
			this.agent.SetTarget(this.target.position);
		}

		private void LateUpdate()
		{
			if (Input.GetKeyDown(KeyCode.Mouse0))
			{
				this.agent.SetTarget(this.target.position);
			}
		}

		private RVOExampleAgent agent;

		public Transform target;
	}
}
