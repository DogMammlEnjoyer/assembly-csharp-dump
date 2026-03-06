using System;
using Pathfinding.Util;
using UnityEngine;

namespace Pathfinding
{
	[HelpURL("http://arongranberg.com/astar/documentation/stable/class_pathfinding_1_1_local_space_graph.php")]
	public class LocalSpaceGraph : VersionedMonoBehaviour
	{
		public GraphTransform transformation { get; private set; }

		private void Start()
		{
			this.originalMatrix = base.transform.worldToLocalMatrix;
			base.transform.hasChanged = true;
			this.Refresh();
		}

		public void Refresh()
		{
			if (base.transform.hasChanged)
			{
				this.transformation = new GraphTransform(base.transform.localToWorldMatrix * this.originalMatrix);
				base.transform.hasChanged = false;
			}
		}

		private Matrix4x4 originalMatrix;
	}
}
