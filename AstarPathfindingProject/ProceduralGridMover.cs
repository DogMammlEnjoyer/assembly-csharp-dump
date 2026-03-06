using System;
using System.Collections;
using UnityEngine;

namespace Pathfinding
{
	[HelpURL("http://arongranberg.com/astar/documentation/stable/class_pathfinding_1_1_procedural_grid_mover.php")]
	public class ProceduralGridMover : VersionedMonoBehaviour
	{
		public bool updatingGraph { get; private set; }

		private void Start()
		{
			if (AstarPath.active == null)
			{
				throw new Exception("There is no AstarPath object in the scene");
			}
			if (this.graph == null)
			{
				if (this.graphIndex < 0)
				{
					throw new Exception("Graph index should not be negative");
				}
				if (this.graphIndex >= AstarPath.active.data.graphs.Length)
				{
					throw new Exception(string.Concat(new string[]
					{
						"The ProceduralGridMover was configured to use graph index ",
						this.graphIndex.ToString(),
						", but only ",
						AstarPath.active.data.graphs.Length.ToString(),
						" graphs exist"
					}));
				}
				this.graph = (AstarPath.active.data.graphs[this.graphIndex] as GridGraph);
				if (this.graph == null)
				{
					throw new Exception("The ProceduralGridMover was configured to use graph index " + this.graphIndex.ToString() + " but that graph either does not exist or is not a GridGraph or LayerGridGraph");
				}
			}
			this.UpdateGraph();
		}

		private void Update()
		{
			if (this.graph == null)
			{
				return;
			}
			Vector3 a = this.PointToGraphSpace(this.graph.center);
			Vector3 b = this.PointToGraphSpace(this.target.position);
			if (VectorMath.SqrDistanceXZ(a, b) > this.updateDistance * this.updateDistance)
			{
				this.UpdateGraph();
			}
		}

		private Vector3 PointToGraphSpace(Vector3 p)
		{
			return this.graph.transform.InverseTransform(p);
		}

		public void UpdateGraph()
		{
			if (this.updatingGraph)
			{
				return;
			}
			this.updatingGraph = true;
			IEnumerator ie = this.UpdateGraphCoroutine();
			AstarPath.active.AddWorkItem(new AstarWorkItem(delegate(IWorkItemContext context, bool force)
			{
				if (force)
				{
					while (ie.MoveNext())
					{
					}
				}
				bool flag;
				try
				{
					flag = !ie.MoveNext();
				}
				catch (Exception exception)
				{
					Debug.LogException(exception, this);
					flag = true;
				}
				if (flag)
				{
					this.updatingGraph = false;
				}
				return flag;
			}));
		}

		private IEnumerator UpdateGraphCoroutine()
		{
			Vector3 vector = this.PointToGraphSpace(this.target.position) - this.PointToGraphSpace(this.graph.center);
			vector.x = Mathf.Round(vector.x);
			vector.z = Mathf.Round(vector.z);
			vector.y = 0f;
			if (vector == Vector3.zero)
			{
				yield break;
			}
			Int2 offset = new Int2(-Mathf.RoundToInt(vector.x), -Mathf.RoundToInt(vector.z));
			this.graph.center += this.graph.transform.TransformVector(vector);
			this.graph.UpdateTransform();
			int width = this.graph.width;
			int depth = this.graph.depth;
			int layers = this.graph.LayerCount;
			LayerGridGraph layerGridGraph = this.graph as LayerGridGraph;
			GridNodeBase[] nodes;
			if (layerGridGraph != null)
			{
				nodes = layerGridGraph.nodes;
			}
			else
			{
				nodes = this.graph.nodes;
			}
			if (this.buffer == null || this.buffer.Length != width * depth)
			{
				this.buffer = new GridNodeBase[width * depth];
			}
			if (Mathf.Abs(offset.x) <= width && Mathf.Abs(offset.y) <= depth)
			{
				IntRect recalculateRect = new IntRect(0, 0, offset.x, offset.y);
				if (recalculateRect.xmin > recalculateRect.xmax)
				{
					int xmax = recalculateRect.xmax;
					recalculateRect.xmax = width + recalculateRect.xmin;
					recalculateRect.xmin = width + xmax;
				}
				if (recalculateRect.ymin > recalculateRect.ymax)
				{
					int ymax = recalculateRect.ymax;
					recalculateRect.ymax = depth + recalculateRect.ymin;
					recalculateRect.ymin = depth + ymax;
				}
				IntRect connectionRect = recalculateRect.Expand(1);
				connectionRect = IntRect.Intersection(connectionRect, new IntRect(0, 0, width, depth));
				int num7;
				for (int i = 0; i < layers; i = num7 + 1)
				{
					int layerOffset = i * width * depth;
					for (int j = 0; j < depth; j++)
					{
						int num = j * width;
						int num2 = (j + offset.y + depth) % depth * width;
						for (int k = 0; k < width; k++)
						{
							this.buffer[num2 + (k + offset.x + width) % width] = nodes[layerOffset + num + k];
						}
					}
					yield return null;
					for (int l = 0; l < depth; l++)
					{
						int num3 = l * width;
						for (int m = 0; m < width; m++)
						{
							int num4 = num3 + m;
							GridNodeBase gridNodeBase = this.buffer[num4];
							if (gridNodeBase != null)
							{
								gridNodeBase.NodeInGridIndex = num4;
							}
							nodes[layerOffset + num4] = gridNodeBase;
						}
						int num5;
						int num6;
						if (l >= recalculateRect.ymin && l < recalculateRect.ymax)
						{
							num5 = 0;
							num6 = depth;
						}
						else
						{
							num5 = recalculateRect.xmin;
							num6 = recalculateRect.xmax;
						}
						for (int n = num5; n < num6; n++)
						{
							GridNodeBase gridNodeBase2 = this.buffer[num3 + n];
							if (gridNodeBase2 != null)
							{
								gridNodeBase2.ClearConnections(false);
							}
						}
					}
					yield return null;
					num7 = i;
				}
				int yieldEvery = 1000;
				int num8 = Mathf.Max(Mathf.Abs(offset.x), Mathf.Abs(offset.y)) * Mathf.Max(width, depth);
				yieldEvery = Mathf.Max(yieldEvery, num8 / 10);
				int counter = 0;
				for (int i = 0; i < depth; i = num7 + 1)
				{
					int num9;
					int num10;
					if (i >= recalculateRect.ymin && i < recalculateRect.ymax)
					{
						num9 = 0;
						num10 = width;
					}
					else
					{
						num9 = recalculateRect.xmin;
						num10 = recalculateRect.xmax;
					}
					for (int num11 = num9; num11 < num10; num11++)
					{
						this.graph.RecalculateCell(num11, i, false, false);
					}
					counter += num10 - num9;
					if (counter > yieldEvery)
					{
						counter = 0;
						yield return null;
					}
					num7 = i;
				}
				for (int i = 0; i < depth; i = num7 + 1)
				{
					int num12;
					int num13;
					if (i >= connectionRect.ymin && i < connectionRect.ymax)
					{
						num12 = 0;
						num13 = width;
					}
					else
					{
						num12 = connectionRect.xmin;
						num13 = connectionRect.xmax;
					}
					for (int num14 = num12; num14 < num13; num14++)
					{
						this.graph.CalculateConnections(num14, i);
					}
					counter += num13 - num12;
					if (counter > yieldEvery)
					{
						counter = 0;
						yield return null;
					}
					num7 = i;
				}
				yield return null;
				for (int num15 = 0; num15 < depth; num15++)
				{
					for (int num16 = 0; num16 < width; num16++)
					{
						if (num16 == 0 || num15 == 0 || num16 == width - 1 || num15 == depth - 1)
						{
							this.graph.CalculateConnections(num16, num15);
						}
					}
				}
			}
			else
			{
				int counter = Mathf.Max(depth * width / 20, 1000);
				int yieldEvery = 0;
				int num7;
				for (int i = 0; i < depth; i = num7 + 1)
				{
					for (int num17 = 0; num17 < width; num17++)
					{
						this.graph.RecalculateCell(num17, i, true, true);
					}
					yieldEvery += width;
					if (yieldEvery > counter)
					{
						yieldEvery = 0;
						yield return null;
					}
					num7 = i;
				}
				for (int i = 0; i < depth; i = num7 + 1)
				{
					for (int num18 = 0; num18 < width; num18++)
					{
						this.graph.CalculateConnections(num18, i);
					}
					yieldEvery += width;
					if (yieldEvery > counter)
					{
						yieldEvery = 0;
						yield return null;
					}
					num7 = i;
				}
			}
			yield break;
		}

		public float updateDistance = 10f;

		public Transform target;

		private GridNodeBase[] buffer;

		public GridGraph graph;

		[HideInInspector]
		public int graphIndex;
	}
}
