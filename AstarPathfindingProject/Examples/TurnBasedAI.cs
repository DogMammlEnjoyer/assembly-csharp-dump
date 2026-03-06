using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding.Examples
{
	[HelpURL("http://arongranberg.com/astar/documentation/stable/class_pathfinding_1_1_examples_1_1_turn_based_a_i.php")]
	public class TurnBasedAI : VersionedMonoBehaviour
	{
		private void Start()
		{
			this.blocker.BlockAtCurrentPosition();
		}

		protected override void Awake()
		{
			base.Awake();
			this.traversalProvider = new BlockManager.TraversalProvider(this.blockManager, BlockManager.BlockMode.AllExceptSelector, new List<SingleNodeBlocker>
			{
				this.blocker
			});
		}

		public int movementPoints = 2;

		public BlockManager blockManager;

		public SingleNodeBlocker blocker;

		public GraphNode targetNode;

		public BlockManager.TraversalProvider traversalProvider;
	}
}
