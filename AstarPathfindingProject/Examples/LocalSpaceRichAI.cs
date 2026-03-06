using System;
using UnityEngine;

namespace Pathfinding.Examples
{
	[HelpURL("http://arongranberg.com/astar/documentation/stable/class_pathfinding_1_1_examples_1_1_local_space_rich_a_i.php")]
	public class LocalSpaceRichAI : RichAI
	{
		private void RefreshTransform()
		{
			this.graph.Refresh();
			this.richPath.transform = this.graph.transformation;
			this.movementPlane = this.graph.transformation;
		}

		protected override void Start()
		{
			this.RefreshTransform();
			base.Start();
		}

		protected override void CalculatePathRequestEndpoints(out Vector3 start, out Vector3 end)
		{
			this.RefreshTransform();
			base.CalculatePathRequestEndpoints(out start, out end);
			start = this.graph.transformation.InverseTransform(start);
			end = this.graph.transformation.InverseTransform(end);
		}

		protected override void Update()
		{
			this.RefreshTransform();
			base.Update();
		}

		public LocalSpaceGraph graph;
	}
}
