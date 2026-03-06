using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Pathfinding.Serialization;
using Pathfinding.Util;
using UnityEngine;

namespace Pathfinding
{
	[JsonOptIn]
	[Preserve]
	public class GridGraph : NavGraph, IUpdatableGraph, ITransformedGraph, IRaycastableGraph
	{
		protected override void OnDestroy()
		{
			base.OnDestroy();
			this.RemoveGridGraphFromStatic();
		}

		protected override void DestroyAllNodes()
		{
			this.GetNodes(delegate(GraphNode node)
			{
				(node as GridNodeBase).ClearCustomConnections(true);
				node.ClearConnections(false);
				node.Destroy();
			});
		}

		private void RemoveGridGraphFromStatic()
		{
			GridNode.ClearGridGraph(this.active.data.GetGraphIndex(this), this);
		}

		public virtual bool uniformWidthDepthGrid
		{
			get
			{
				return true;
			}
		}

		public virtual int LayerCount
		{
			get
			{
				return 1;
			}
		}

		public override int CountNodes()
		{
			if (this.nodes == null)
			{
				return 0;
			}
			return this.nodes.Length;
		}

		public override void GetNodes(Action<GraphNode> action)
		{
			if (this.nodes == null)
			{
				return;
			}
			for (int i = 0; i < this.nodes.Length; i++)
			{
				action(this.nodes[i]);
			}
		}

		protected bool useRaycastNormal
		{
			get
			{
				return Math.Abs(90f - this.maxSlope) > float.Epsilon;
			}
		}

		public Vector2 size { get; protected set; }

		public GraphTransform transform { get; private set; }

		public bool is2D
		{
			get
			{
				return Quaternion.Euler(this.rotation) * Vector3.up == -Vector3.forward;
			}
			set
			{
				if (value != this.is2D)
				{
					this.rotation = (value ? new Vector3(this.rotation.y - 90f, 270f, 90f) : new Vector3(0f, this.rotation.x + 90f, 0f));
				}
			}
		}

		public GridGraph()
		{
			this.unclampedSize = new Vector2(10f, 10f);
			this.nodeSize = 1f;
			this.collision = new GraphCollision();
			this.transform = new GraphTransform(Matrix4x4.identity);
		}

		public override void RelocateNodes(Matrix4x4 deltaMatrix)
		{
			throw new Exception("This method cannot be used for Grid Graphs. Please use the other overload of RelocateNodes instead");
		}

		public void RelocateNodes(Vector3 center, Quaternion rotation, float nodeSize, float aspectRatio = 1f, float isometricAngle = 0f)
		{
			GraphTransform previousTransform = this.transform;
			this.center = center;
			this.rotation = rotation.eulerAngles;
			this.aspectRatio = aspectRatio;
			this.isometricAngle = isometricAngle;
			this.SetDimensions(this.width, this.depth, nodeSize);
			this.GetNodes(delegate(GraphNode node)
			{
				GridNodeBase gridNodeBase = node as GridNodeBase;
				float y = previousTransform.InverseTransform((Vector3)node.position).y;
				node.position = this.GraphPointToWorld(gridNodeBase.XCoordinateInGrid, gridNodeBase.ZCoordinateInGrid, y);
			});
		}

		public Int3 GraphPointToWorld(int x, int z, float height)
		{
			return (Int3)this.transform.Transform(new Vector3((float)x + 0.5f, height, (float)z + 0.5f));
		}

		public static float ConvertHexagonSizeToNodeSize(InspectorGridHexagonNodeSize mode, float value)
		{
			if (mode == InspectorGridHexagonNodeSize.Diameter)
			{
				value *= 1.5f / (float)Math.Sqrt(2.0);
			}
			else if (mode == InspectorGridHexagonNodeSize.Width)
			{
				value *= (float)Math.Sqrt(1.5);
			}
			return value;
		}

		public static float ConvertNodeSizeToHexagonSize(InspectorGridHexagonNodeSize mode, float value)
		{
			if (mode == InspectorGridHexagonNodeSize.Diameter)
			{
				value *= (float)Math.Sqrt(2.0) / 1.5f;
			}
			else if (mode == InspectorGridHexagonNodeSize.Width)
			{
				value *= (float)Math.Sqrt(0.6666666865348816);
			}
			return value;
		}

		public int Width
		{
			get
			{
				return this.width;
			}
			set
			{
				this.width = value;
			}
		}

		public int Depth
		{
			get
			{
				return this.depth;
			}
			set
			{
				this.depth = value;
			}
		}

		public uint GetConnectionCost(int dir)
		{
			return this.neighbourCosts[dir];
		}

		[Obsolete("Use GridNode.HasConnectionInDirection instead")]
		public GridNode GetNodeConnection(GridNode node, int dir)
		{
			if (!node.HasConnectionInDirection(dir))
			{
				return null;
			}
			if (!node.EdgeNode)
			{
				return this.nodes[node.NodeInGridIndex + this.neighbourOffsets[dir]] as GridNode;
			}
			int nodeInGridIndex = node.NodeInGridIndex;
			int num = nodeInGridIndex / this.Width;
			int x = nodeInGridIndex - num * this.Width;
			return this.GetNodeConnection(nodeInGridIndex, x, num, dir);
		}

		[Obsolete("Use GridNode.HasConnectionInDirection instead")]
		public bool HasNodeConnection(GridNode node, int dir)
		{
			if (!node.HasConnectionInDirection(dir))
			{
				return false;
			}
			if (!node.EdgeNode)
			{
				return true;
			}
			int nodeInGridIndex = node.NodeInGridIndex;
			int num = nodeInGridIndex / this.Width;
			int x = nodeInGridIndex - num * this.Width;
			return this.HasNodeConnection(nodeInGridIndex, x, num, dir);
		}

		[Obsolete("Use GridNode.SetConnectionInternal instead")]
		public void SetNodeConnection(GridNode node, int dir, bool value)
		{
			int nodeInGridIndex = node.NodeInGridIndex;
			int num = nodeInGridIndex / this.Width;
			int x = nodeInGridIndex - num * this.Width;
			this.SetNodeConnection(nodeInGridIndex, x, num, dir, value);
		}

		[Obsolete("Use GridNode.HasConnectionInDirection instead")]
		private GridNode GetNodeConnection(int index, int x, int z, int dir)
		{
			if (!this.nodes[index].HasConnectionInDirection(dir))
			{
				return null;
			}
			int num = x + this.neighbourXOffsets[dir];
			if (num < 0 || num >= this.Width)
			{
				return null;
			}
			int num2 = z + this.neighbourZOffsets[dir];
			if (num2 < 0 || num2 >= this.Depth)
			{
				return null;
			}
			int num3 = index + this.neighbourOffsets[dir];
			return this.nodes[num3] as GridNode;
		}

		[Obsolete("Use GridNode.SetConnectionInternal instead")]
		public void SetNodeConnection(int index, int x, int z, int dir, bool value)
		{
			(this.nodes[index] as GridNode).SetConnectionInternal(dir, value);
		}

		[Obsolete("Use GridNode.HasConnectionInDirection instead")]
		public bool HasNodeConnection(int index, int x, int z, int dir)
		{
			if (!this.nodes[index].HasConnectionInDirection(dir))
			{
				return false;
			}
			int num = x + this.neighbourXOffsets[dir];
			if (num < 0 || num >= this.Width)
			{
				return false;
			}
			int num2 = z + this.neighbourZOffsets[dir];
			return num2 >= 0 && num2 < this.Depth;
		}

		public void SetGridShape(InspectorGridMode shape)
		{
			switch (shape)
			{
			case InspectorGridMode.Grid:
				this.isometricAngle = 0f;
				this.aspectRatio = 1f;
				this.uniformEdgeCosts = false;
				if (this.neighbours == NumNeighbours.Six)
				{
					this.neighbours = NumNeighbours.Eight;
				}
				break;
			case InspectorGridMode.IsometricGrid:
				this.uniformEdgeCosts = false;
				if (this.neighbours == NumNeighbours.Six)
				{
					this.neighbours = NumNeighbours.Eight;
				}
				this.isometricAngle = GridGraph.StandardIsometricAngle;
				break;
			case InspectorGridMode.Hexagonal:
				this.isometricAngle = GridGraph.StandardIsometricAngle;
				this.aspectRatio = 1f;
				this.uniformEdgeCosts = true;
				this.neighbours = NumNeighbours.Six;
				break;
			}
			this.inspectorGridMode = shape;
		}

		public void SetDimensions(int width, int depth, float nodeSize)
		{
			this.unclampedSize = new Vector2((float)width, (float)depth) * nodeSize;
			this.nodeSize = nodeSize;
			this.UpdateTransform();
		}

		[Obsolete("Use SetDimensions instead")]
		public void UpdateSizeFromWidthDepth()
		{
			this.SetDimensions(this.width, this.depth, this.nodeSize);
		}

		[Obsolete("This method has been renamed to UpdateTransform")]
		public void GenerateMatrix()
		{
			this.UpdateTransform();
		}

		public void UpdateTransform()
		{
			this.CalculateDimensions(out this.width, out this.depth, out this.nodeSize);
			this.transform = this.CalculateTransform();
		}

		public GraphTransform CalculateTransform()
		{
			int num;
			int num2;
			float num3;
			this.CalculateDimensions(out num, out num2, out num3);
			Matrix4x4 rhs = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 45f, 0f), Vector3.one);
			rhs = Matrix4x4.Scale(new Vector3(Mathf.Cos(0.017453292f * this.isometricAngle), 1f, 1f)) * rhs;
			rhs = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, -45f, 0f), Vector3.one) * rhs;
			return new GraphTransform(Matrix4x4.TRS((Matrix4x4.TRS(this.center, Quaternion.Euler(this.rotation), new Vector3(this.aspectRatio, 1f, 1f)) * rhs).MultiplyPoint3x4(-new Vector3((float)num * num3, 0f, (float)num2 * num3) * 0.5f), Quaternion.Euler(this.rotation), new Vector3(num3 * this.aspectRatio, 1f, num3)) * rhs);
		}

		private void CalculateDimensions(out int width, out int depth, out float nodeSize)
		{
			Vector2 vector = this.unclampedSize;
			vector.x *= Mathf.Sign(vector.x);
			vector.y *= Mathf.Sign(vector.y);
			nodeSize = Mathf.Max(this.nodeSize, vector.x / 1024f);
			nodeSize = Mathf.Max(this.nodeSize, vector.y / 1024f);
			vector.x = ((vector.x < nodeSize) ? nodeSize : vector.x);
			vector.y = ((vector.y < nodeSize) ? nodeSize : vector.y);
			this.size = vector;
			width = Mathf.FloorToInt(this.size.x / nodeSize);
			depth = Mathf.FloorToInt(this.size.y / nodeSize);
			if (Mathf.Approximately(this.size.x / nodeSize, (float)Mathf.CeilToInt(this.size.x / nodeSize)))
			{
				width = Mathf.CeilToInt(this.size.x / nodeSize);
			}
			if (Mathf.Approximately(this.size.y / nodeSize, (float)Mathf.CeilToInt(this.size.y / nodeSize)))
			{
				depth = Mathf.CeilToInt(this.size.y / nodeSize);
			}
		}

		public override NNInfoInternal GetNearest(Vector3 position, NNConstraint constraint, GraphNode hint)
		{
			if (this.nodes == null || this.depth * this.width != this.nodes.Length)
			{
				return default(NNInfoInternal);
			}
			position = this.transform.InverseTransform(position);
			float x = position.x;
			float z = position.z;
			int num = Mathf.Clamp((int)x, 0, this.width - 1);
			int num2 = Mathf.Clamp((int)z, 0, this.depth - 1);
			NNInfoInternal result = new NNInfoInternal(this.nodes[num2 * this.width + num]);
			float y = this.transform.InverseTransform((Vector3)this.nodes[num2 * this.width + num].position).y;
			result.clampedPosition = this.transform.Transform(new Vector3(Mathf.Clamp(x, (float)num, (float)num + 1f), y, Mathf.Clamp(z, (float)num2, (float)num2 + 1f)));
			return result;
		}

		protected virtual GridNodeBase GetNearestFromGraphSpace(Vector3 positionGraphSpace)
		{
			if (this.nodes == null || this.depth * this.width != this.nodes.Length)
			{
				return null;
			}
			int x = (int)positionGraphSpace.x;
			float z = positionGraphSpace.z;
			int num = Mathf.Clamp(x, 0, this.width - 1);
			int num2 = Mathf.Clamp((int)z, 0, this.depth - 1);
			return this.nodes[num2 * this.width + num];
		}

		public override NNInfoInternal GetNearestForce(Vector3 position, NNConstraint constraint)
		{
			if (this.nodes == null || this.depth * this.width * this.LayerCount != this.nodes.Length)
			{
				return default(NNInfoInternal);
			}
			Vector3 b = position;
			position = this.transform.InverseTransform(position);
			float x = position.x;
			float z = position.z;
			int num = Mathf.Clamp((int)x, 0, this.width - 1);
			int num2 = Mathf.Clamp((int)z, 0, this.depth - 1);
			GridNodeBase gridNodeBase = null;
			bool flag = constraint != null && constraint.distanceXZ;
			float num3 = (constraint == null || constraint.constrainDistance) ? AstarPath.active.maxNearestNodeDistance : float.PositiveInfinity;
			float num4 = num3 * num3;
			int layerCount = this.LayerCount;
			int num5 = this.width * this.depth;
			for (int i = 0; i < layerCount; i++)
			{
				GridNodeBase gridNodeBase2 = this.nodes[num2 * this.width + num + num5 * i];
				if (gridNodeBase2 != null && (constraint == null || constraint.Suitable(gridNodeBase2)))
				{
					float num6 = flag ? (this.nodeSize * this.nodeSize * (((float)num + 0.5f - x) * ((float)num + 0.5f - x) + ((float)num2 + 0.5f - z) * ((float)num2 + 0.5f - z))) : ((Vector3)gridNodeBase2.position - b).sqrMagnitude;
					if (num6 <= num4)
					{
						num4 = num6;
						gridNodeBase = gridNodeBase2;
					}
				}
			}
			int num7 = 1;
			while (this.nodeSize * this.nodeSize * (float)num7 * (float)num7 <= num4 * 2f)
			{
				bool flag2 = false;
				int num8 = num + num7;
				int num9 = num2;
				int num10 = -1;
				int num11 = 1;
				for (int j = 0; j < 4; j++)
				{
					for (int k = 0; k < num7; k++)
					{
						if (num8 >= 0 && num9 >= 0 && num8 < this.width && num9 < this.depth)
						{
							flag2 = true;
							int num12 = num8 + num9 * this.width;
							for (int l = 0; l < layerCount; l++)
							{
								GridNodeBase gridNodeBase3 = this.nodes[num12 + num5 * l];
								if (gridNodeBase3 != null && (constraint == null || constraint.Suitable(gridNodeBase3)))
								{
									float num13 = flag ? (this.nodeSize * this.nodeSize * (((float)num8 + 0.5f - x) * ((float)num8 + 0.5f - x) + ((float)num9 + 0.5f - z) * ((float)num9 + 0.5f - z))) : ((Vector3)gridNodeBase3.position - b).sqrMagnitude;
									if (num13 <= num4)
									{
										num4 = num13;
										gridNodeBase = gridNodeBase3;
									}
								}
							}
						}
						num8 += num10;
						num9 += num11;
					}
					int num14 = -num11;
					int num15 = num10;
					num10 = num14;
					num11 = num15;
				}
				if (!flag2)
				{
					break;
				}
				num7++;
			}
			NNInfoInternal result = new NNInfoInternal(null);
			if (gridNodeBase != null)
			{
				int xcoordinateInGrid = gridNodeBase.XCoordinateInGrid;
				int zcoordinateInGrid = gridNodeBase.ZCoordinateInGrid;
				result.node = gridNodeBase;
				result.clampedPosition = this.transform.Transform(new Vector3(Mathf.Clamp(x, (float)xcoordinateInGrid, (float)xcoordinateInGrid + 1f), this.transform.InverseTransform((Vector3)gridNodeBase.position).y, Mathf.Clamp(z, (float)zcoordinateInGrid, (float)zcoordinateInGrid + 1f)));
			}
			return result;
		}

		public virtual void SetUpOffsetsAndCosts()
		{
			this.neighbourOffsets[0] = -this.width;
			this.neighbourOffsets[1] = 1;
			this.neighbourOffsets[2] = this.width;
			this.neighbourOffsets[3] = -1;
			this.neighbourOffsets[4] = -this.width + 1;
			this.neighbourOffsets[5] = this.width + 1;
			this.neighbourOffsets[6] = this.width - 1;
			this.neighbourOffsets[7] = -this.width - 1;
			uint num = (uint)Mathf.RoundToInt(this.nodeSize * 1000f);
			uint num2 = this.uniformEdgeCosts ? num : ((uint)Mathf.RoundToInt(this.nodeSize * Mathf.Sqrt(2f) * 1000f));
			this.neighbourCosts[0] = num;
			this.neighbourCosts[1] = num;
			this.neighbourCosts[2] = num;
			this.neighbourCosts[3] = num;
			this.neighbourCosts[4] = num2;
			this.neighbourCosts[5] = num2;
			this.neighbourCosts[6] = num2;
			this.neighbourCosts[7] = num2;
			this.neighbourXOffsets[0] = 0;
			this.neighbourXOffsets[1] = 1;
			this.neighbourXOffsets[2] = 0;
			this.neighbourXOffsets[3] = -1;
			this.neighbourXOffsets[4] = 1;
			this.neighbourXOffsets[5] = 1;
			this.neighbourXOffsets[6] = -1;
			this.neighbourXOffsets[7] = -1;
			this.neighbourZOffsets[0] = -1;
			this.neighbourZOffsets[1] = 0;
			this.neighbourZOffsets[2] = 1;
			this.neighbourZOffsets[3] = 0;
			this.neighbourZOffsets[4] = -1;
			this.neighbourZOffsets[5] = 1;
			this.neighbourZOffsets[6] = 1;
			this.neighbourZOffsets[7] = -1;
		}

		protected override IEnumerable<Progress> ScanInternal()
		{
			if (this.nodeSize <= 0f)
			{
				yield break;
			}
			this.UpdateTransform();
			if (this.width > 1024 || this.depth > 1024)
			{
				Debug.LogError("One of the grid's sides is longer than 1024 nodes");
				yield break;
			}
			if (this.useJumpPointSearch)
			{
				Debug.LogError("Trying to use Jump Point Search, but support for it is not enabled. Please enable it in the inspector (Grid Graph settings).");
			}
			this.SetUpOffsetsAndCosts();
			GridNode.SetGridGraph((int)this.graphIndex, this);
			yield return new Progress(0.05f, "Creating nodes");
			GridNodeBase[] array = new GridNode[this.width * this.depth];
			this.nodes = array;
			for (int i = 0; i < this.depth; i++)
			{
				for (int j = 0; j < this.width; j++)
				{
					int num = i * this.width + j;
					GridNodeBase gridNodeBase = this.nodes[num] = new GridNode(this.active);
					gridNodeBase.GraphIndex = this.graphIndex;
					gridNodeBase.NodeInGridIndex = num;
				}
			}
			if (this.collision == null)
			{
				this.collision = new GraphCollision();
			}
			this.collision.Initialize(this.transform, this.nodeSize);
			this.textureData.Initialize();
			int progressCounter = 0;
			int num2;
			for (int z = 0; z < this.depth; z = num2 + 1)
			{
				if (progressCounter >= 1000)
				{
					progressCounter = 0;
					yield return new Progress(Mathf.Lerp(0.1f, 0.7f, (float)z / (float)this.depth), "Calculating positions");
				}
				progressCounter += this.width;
				for (int k = 0; k < this.width; k++)
				{
					this.RecalculateCell(k, z, true, true);
					this.textureData.Apply(this.nodes[z * this.width + k] as GridNode, k, z);
				}
				num2 = z;
			}
			progressCounter = 0;
			for (int z = 0; z < this.depth; z = num2 + 1)
			{
				if (progressCounter >= 1000)
				{
					progressCounter = 0;
					yield return new Progress(Mathf.Lerp(0.7f, 0.9f, (float)z / (float)this.depth), "Calculating connections");
				}
				progressCounter += this.width;
				for (int l = 0; l < this.width; l++)
				{
					this.CalculateConnections(l, z);
				}
				num2 = z;
			}
			yield return new Progress(0.95f, "Calculating erosion");
			this.ErodeWalkableArea();
			yield break;
		}

		[Obsolete("Use RecalculateCell instead which works both for grid graphs and layered grid graphs")]
		public virtual void UpdateNodePositionCollision(GridNode node, int x, int z, bool resetPenalty = true)
		{
			this.RecalculateCell(x, z, resetPenalty, false);
		}

		public virtual void RecalculateCell(int x, int z, bool resetPenalties = true, bool resetTags = true)
		{
			GridNodeBase gridNodeBase = this.nodes[z * this.width + x];
			gridNodeBase.position = this.GraphPointToWorld(x, z, 0f);
			RaycastHit raycastHit;
			bool flag;
			Vector3 ob = this.collision.CheckHeight((Vector3)gridNodeBase.position, out raycastHit, out flag);
			gridNodeBase.position = (Int3)ob;
			if (resetPenalties)
			{
				gridNodeBase.Penalty = this.initialPenalty;
				if (this.penaltyPosition)
				{
					gridNodeBase.Penalty += (uint)Mathf.RoundToInt(((float)gridNodeBase.position.y - this.penaltyPositionOffset) * this.penaltyPositionFactor);
				}
			}
			if (resetTags)
			{
				gridNodeBase.Tag = 0U;
			}
			if (flag && this.useRaycastNormal && this.collision.heightCheck && raycastHit.normal != Vector3.zero)
			{
				float num = Vector3.Dot(raycastHit.normal.normalized, this.collision.up);
				if (this.penaltyAngle && resetPenalties)
				{
					gridNodeBase.Penalty += (uint)Mathf.RoundToInt((1f - Mathf.Pow(num, this.penaltyAnglePower)) * this.penaltyAngleFactor);
				}
				float num2 = Mathf.Cos(this.maxSlope * 0.017453292f);
				if (num < num2)
				{
					flag = false;
				}
			}
			gridNodeBase.Walkable = (flag && this.collision.Check((Vector3)gridNodeBase.position));
			gridNodeBase.WalkableErosion = gridNodeBase.Walkable;
		}

		protected virtual bool ErosionAnyFalseConnections(GraphNode baseNode)
		{
			GridNode gridNode = baseNode as GridNode;
			if (this.neighbours == NumNeighbours.Six)
			{
				for (int i = 0; i < 6; i++)
				{
					if (!gridNode.HasConnectionInDirection(GridGraph.hexagonNeighbourIndices[i]))
					{
						return true;
					}
				}
			}
			else
			{
				for (int j = 0; j < 4; j++)
				{
					if (!gridNode.HasConnectionInDirection(j))
					{
						return true;
					}
				}
			}
			return false;
		}

		private void ErodeNode(GraphNode node)
		{
			if (node.Walkable && this.ErosionAnyFalseConnections(node))
			{
				node.Walkable = false;
			}
		}

		private void ErodeNodeWithTagsInit(GraphNode node)
		{
			if (node.Walkable && this.ErosionAnyFalseConnections(node))
			{
				node.Tag = (uint)this.erosionFirstTag;
				return;
			}
			node.Tag = 0U;
		}

		private void ErodeNodeWithTags(GraphNode node, int iteration)
		{
			GridNodeBase gridNodeBase = node as GridNodeBase;
			if (gridNodeBase.Walkable && (ulong)gridNodeBase.Tag >= (ulong)((long)this.erosionFirstTag) && (ulong)gridNodeBase.Tag < (ulong)((long)(this.erosionFirstTag + iteration)))
			{
				if (this.neighbours == NumNeighbours.Six)
				{
					for (int i = 0; i < 6; i++)
					{
						GridNodeBase neighbourAlongDirection = gridNodeBase.GetNeighbourAlongDirection(GridGraph.hexagonNeighbourIndices[i]);
						if (neighbourAlongDirection != null)
						{
							uint tag = neighbourAlongDirection.Tag;
							if ((ulong)tag > (ulong)((long)(this.erosionFirstTag + iteration)) || (ulong)tag < (ulong)((long)this.erosionFirstTag))
							{
								neighbourAlongDirection.Tag = (uint)(this.erosionFirstTag + iteration);
							}
						}
					}
					return;
				}
				for (int j = 0; j < 4; j++)
				{
					GridNodeBase neighbourAlongDirection2 = gridNodeBase.GetNeighbourAlongDirection(j);
					if (neighbourAlongDirection2 != null)
					{
						uint tag2 = neighbourAlongDirection2.Tag;
						if ((ulong)tag2 > (ulong)((long)(this.erosionFirstTag + iteration)) || (ulong)tag2 < (ulong)((long)this.erosionFirstTag))
						{
							neighbourAlongDirection2.Tag = (uint)(this.erosionFirstTag + iteration);
						}
					}
				}
			}
		}

		public virtual void ErodeWalkableArea()
		{
			this.ErodeWalkableArea(0, 0, this.Width, this.Depth);
		}

		public void ErodeWalkableArea(int xmin, int zmin, int xmax, int zmax)
		{
			if (this.erosionUseTags)
			{
				if (this.erodeIterations + this.erosionFirstTag > 31)
				{
					Debug.LogError(string.Concat(new string[]
					{
						"Too few tags available for ",
						this.erodeIterations.ToString(),
						" erode iterations and starting with tag ",
						this.erosionFirstTag.ToString(),
						" (erodeIterations+erosionFirstTag > 31)"
					}), this.active);
					return;
				}
				if (this.erosionFirstTag <= 0)
				{
					Debug.LogError("First erosion tag must be greater or equal to 1", this.active);
					return;
				}
			}
			if (this.erodeIterations == 0)
			{
				return;
			}
			IntRect rect = new IntRect(xmin, zmin, xmax - 1, zmax - 1);
			List<GraphNode> nodesInRegion = this.GetNodesInRegion(rect);
			int count = nodesInRegion.Count;
			for (int i = 0; i < this.erodeIterations; i++)
			{
				if (this.erosionUseTags)
				{
					if (i == 0)
					{
						for (int j = 0; j < count; j++)
						{
							this.ErodeNodeWithTagsInit(nodesInRegion[j]);
						}
					}
					else
					{
						for (int k = 0; k < count; k++)
						{
							this.ErodeNodeWithTags(nodesInRegion[k], i);
						}
					}
				}
				else
				{
					for (int l = 0; l < count; l++)
					{
						this.ErodeNode(nodesInRegion[l]);
					}
					for (int m = 0; m < count; m++)
					{
						this.CalculateConnections(nodesInRegion[m] as GridNodeBase);
					}
				}
			}
			ListPool<GraphNode>.Release(ref nodesInRegion);
		}

		public virtual bool IsValidConnection(GridNodeBase node1, GridNodeBase node2)
		{
			if (!node1.Walkable || !node2.Walkable)
			{
				return false;
			}
			if (this.maxClimb <= 0f || this.collision.use2D)
			{
				return true;
			}
			if (this.transform.onlyTranslational)
			{
				return (float)Math.Abs(node1.position.y - node2.position.y) <= this.maxClimb * 1000f;
			}
			Vector3 vector = (Vector3)node1.position;
			Vector3 rhs = (Vector3)node2.position;
			Vector3 lhs = this.transform.WorldUpAtGraphPosition(vector);
			return Math.Abs(Vector3.Dot(lhs, vector) - Vector3.Dot(lhs, rhs)) <= this.maxClimb;
		}

		public void CalculateConnectionsForCellAndNeighbours(int x, int z)
		{
			this.CalculateConnections(x, z);
			for (int i = 0; i < 8; i++)
			{
				int num = x + this.neighbourXOffsets[i];
				int num2 = z + this.neighbourZOffsets[i];
				if (num >= 0 & num2 >= 0 & num < this.width & num2 < this.depth)
				{
					this.CalculateConnections(num, num2);
				}
			}
		}

		[Obsolete("Use the instance function instead")]
		public static void CalculateConnections(GridNode node)
		{
			(AstarData.GetGraph(node) as GridGraph).CalculateConnections(node);
		}

		public virtual void CalculateConnections(GridNodeBase node)
		{
			int nodeInGridIndex = node.NodeInGridIndex;
			int x = nodeInGridIndex % this.width;
			int z = nodeInGridIndex / this.width;
			this.CalculateConnections(x, z);
		}

		[Obsolete("Use CalculateConnections(x,z) or CalculateConnections(node) instead")]
		public virtual void CalculateConnections(int x, int z, GridNode node)
		{
			this.CalculateConnections(x, z);
		}

		public virtual void CalculateConnections(int x, int z)
		{
			GridNode gridNode = this.nodes[z * this.width + x] as GridNode;
			if (!gridNode.Walkable)
			{
				gridNode.ResetConnectionsInternal();
				return;
			}
			int nodeInGridIndex = gridNode.NodeInGridIndex;
			if (this.neighbours == NumNeighbours.Four || this.neighbours == NumNeighbours.Eight)
			{
				int num = 0;
				for (int i = 0; i < 4; i++)
				{
					int num2 = x + this.neighbourXOffsets[i];
					int num3 = z + this.neighbourZOffsets[i];
					if (num2 >= 0 & num3 >= 0 & num2 < this.width & num3 < this.depth)
					{
						GridNodeBase node = this.nodes[nodeInGridIndex + this.neighbourOffsets[i]];
						if (this.IsValidConnection(gridNode, node))
						{
							num |= 1 << i;
						}
					}
				}
				int num4 = 0;
				if (this.neighbours == NumNeighbours.Eight)
				{
					if (this.cutCorners)
					{
						for (int j = 0; j < 4; j++)
						{
							if (((num >> j | num >> j + 1 | num >> j + 1 - 4) & 1) != 0)
							{
								int num5 = j + 4;
								int num6 = x + this.neighbourXOffsets[num5];
								int num7 = z + this.neighbourZOffsets[num5];
								if (num6 >= 0 & num7 >= 0 & num6 < this.width & num7 < this.depth)
								{
									GridNodeBase node2 = this.nodes[nodeInGridIndex + this.neighbourOffsets[num5]];
									if (this.IsValidConnection(gridNode, node2))
									{
										num4 |= 1 << num5;
									}
								}
							}
						}
					}
					else
					{
						for (int k = 0; k < 4; k++)
						{
							if ((num >> k & 1) != 0 && ((num >> k + 1 | num >> k + 1 - 4) & 1) != 0)
							{
								GridNodeBase node3 = this.nodes[nodeInGridIndex + this.neighbourOffsets[k + 4]];
								if (this.IsValidConnection(gridNode, node3))
								{
									num4 |= 1 << k + 4;
								}
							}
						}
					}
				}
				gridNode.SetAllConnectionInternal(num | num4);
				return;
			}
			gridNode.ResetConnectionsInternal();
			for (int l = 0; l < GridGraph.hexagonNeighbourIndices.Length; l++)
			{
				int num8 = GridGraph.hexagonNeighbourIndices[l];
				int num9 = x + this.neighbourXOffsets[num8];
				int num10 = z + this.neighbourZOffsets[num8];
				if (num9 >= 0 & num10 >= 0 & num9 < this.width & num10 < this.depth)
				{
					GridNodeBase node4 = this.nodes[nodeInGridIndex + this.neighbourOffsets[num8]];
					gridNode.SetConnectionInternal(num8, this.IsValidConnection(gridNode, node4));
				}
			}
		}

		public override void OnDrawGizmos(RetainedGizmos gizmos, bool drawNodes)
		{
			using (GraphGizmoHelper singleFrameGizmoHelper = gizmos.GetSingleFrameGizmoHelper(this.active))
			{
				int num;
				int num2;
				float num3;
				this.CalculateDimensions(out num, out num2, out num3);
				Bounds bounds = default(Bounds);
				bounds.SetMinMax(Vector3.zero, new Vector3((float)num, 0f, (float)num2));
				GraphTransform graphTransform = this.CalculateTransform();
				singleFrameGizmoHelper.builder.DrawWireCube(graphTransform, bounds, Color.white);
				int num4 = (this.nodes != null) ? this.nodes.Length : -1;
				if (this is LayerGridGraph)
				{
					num4 = (((this as LayerGridGraph).nodes != null) ? (this as LayerGridGraph).nodes.Length : -1);
				}
				if (drawNodes && this.width * this.depth * this.LayerCount != num4)
				{
					Color color = new Color(1f, 1f, 1f, 0.2f);
					for (int i = 0; i < num2; i++)
					{
						singleFrameGizmoHelper.builder.DrawLine(graphTransform.Transform(new Vector3(0f, 0f, (float)i)), graphTransform.Transform(new Vector3((float)num, 0f, (float)i)), color);
					}
					for (int j = 0; j < num; j++)
					{
						singleFrameGizmoHelper.builder.DrawLine(graphTransform.Transform(new Vector3((float)j, 0f, 0f)), graphTransform.Transform(new Vector3((float)j, 0f, (float)num2)), color);
					}
				}
			}
			if (!drawNodes)
			{
				return;
			}
			GridNodeBase[] array = ArrayPool<GridNodeBase>.Claim(1024 * this.LayerCount);
			for (int k = this.width / 32; k >= 0; k--)
			{
				for (int l = this.depth / 32; l >= 0; l--)
				{
					int nodesInRegion = this.GetNodesInRegion(new IntRect(k * 32, l * 32, (k + 1) * 32 - 1, (l + 1) * 32 - 1), array);
					RetainedGizmos.Hasher hasher = new RetainedGizmos.Hasher(this.active);
					hasher.AddHash(this.showMeshOutline ? 1 : 0);
					hasher.AddHash(this.showMeshSurface ? 1 : 0);
					hasher.AddHash(this.showNodeConnections ? 1 : 0);
					for (int m = 0; m < nodesInRegion; m++)
					{
						hasher.HashNode(array[m]);
					}
					if (!gizmos.Draw(hasher))
					{
						using (GraphGizmoHelper gizmoHelper = gizmos.GetGizmoHelper(this.active, hasher))
						{
							if (this.showNodeConnections)
							{
								for (int n = 0; n < nodesInRegion; n++)
								{
									if (array[n].Walkable)
									{
										gizmoHelper.DrawConnections(array[n]);
									}
								}
							}
							if (this.showMeshSurface || this.showMeshOutline)
							{
								this.CreateNavmeshSurfaceVisualization(array, nodesInRegion, gizmoHelper);
							}
						}
					}
				}
			}
			ArrayPool<GridNodeBase>.Release(ref array, false);
			if (this.active.showUnwalkableNodes)
			{
				base.DrawUnwalkableNodes(this.nodeSize * 0.3f);
			}
		}

		private void CreateNavmeshSurfaceVisualization(GridNodeBase[] nodes, int nodeCount, GraphGizmoHelper helper)
		{
			int num = 0;
			for (int i = 0; i < nodeCount; i++)
			{
				if (nodes[i].Walkable)
				{
					num++;
				}
			}
			int[] array;
			if (this.neighbours != NumNeighbours.Six)
			{
				RuntimeHelpers.InitializeArray(array = new int[4], fieldof(<PrivateImplementationDetails>.BAED642339816AFFB3FE8719792D0E4CE82F12DB72B7373D244EAA65445800FE).FieldHandle);
			}
			else
			{
				array = GridGraph.hexagonNeighbourIndices;
			}
			int[] array2 = array;
			float num2 = (this.neighbours == NumNeighbours.Six) ? 0.333333f : 0.5f;
			int num3 = array2.Length - 2;
			int num4 = 3 * num3;
			Vector3[] array3 = ArrayPool<Vector3>.Claim(num * num4);
			Color[] array4 = ArrayPool<Color>.Claim(num * num4);
			int num5 = 0;
			for (int j = 0; j < nodeCount; j++)
			{
				GridNodeBase gridNodeBase = nodes[j];
				if (gridNodeBase.Walkable)
				{
					Color color = helper.NodeColor(gridNodeBase);
					if (color.a > 0.001f)
					{
						for (int k = 0; k < array2.Length; k++)
						{
							int num6 = array2[k];
							int num7 = array2[(k + 1) % array2.Length];
							GridNodeBase gridNodeBase2 = null;
							GridNodeBase neighbourAlongDirection = gridNodeBase.GetNeighbourAlongDirection(num6);
							if (neighbourAlongDirection != null && this.neighbours != NumNeighbours.Six)
							{
								gridNodeBase2 = neighbourAlongDirection.GetNeighbourAlongDirection(num7);
							}
							GridNodeBase neighbourAlongDirection2 = gridNodeBase.GetNeighbourAlongDirection(num7);
							if (neighbourAlongDirection2 != null && gridNodeBase2 == null && this.neighbours != NumNeighbours.Six)
							{
								gridNodeBase2 = neighbourAlongDirection2.GetNeighbourAlongDirection(num6);
							}
							Vector3 vector = new Vector3((float)gridNodeBase.XCoordinateInGrid + 0.5f, 0f, (float)gridNodeBase.ZCoordinateInGrid + 0.5f);
							vector.x += (float)(this.neighbourXOffsets[num6] + this.neighbourXOffsets[num7]) * num2;
							vector.z += (float)(this.neighbourZOffsets[num6] + this.neighbourZOffsets[num7]) * num2;
							vector.y += this.transform.InverseTransform((Vector3)gridNodeBase.position).y;
							if (neighbourAlongDirection != null)
							{
								vector.y += this.transform.InverseTransform((Vector3)neighbourAlongDirection.position).y;
							}
							if (neighbourAlongDirection2 != null)
							{
								vector.y += this.transform.InverseTransform((Vector3)neighbourAlongDirection2.position).y;
							}
							if (gridNodeBase2 != null)
							{
								vector.y += this.transform.InverseTransform((Vector3)gridNodeBase2.position).y;
							}
							vector.y /= 1f + ((neighbourAlongDirection != null) ? 1f : 0f) + ((neighbourAlongDirection2 != null) ? 1f : 0f) + ((gridNodeBase2 != null) ? 1f : 0f);
							vector = this.transform.Transform(vector);
							array3[num5 + k] = vector;
						}
						if (this.neighbours == NumNeighbours.Six)
						{
							array3[num5 + 6] = array3[num5];
							array3[num5 + 7] = array3[num5 + 2];
							array3[num5 + 8] = array3[num5 + 3];
							array3[num5 + 9] = array3[num5];
							array3[num5 + 10] = array3[num5 + 3];
							array3[num5 + 11] = array3[num5 + 5];
						}
						else
						{
							array3[num5 + 4] = array3[num5];
							array3[num5 + 5] = array3[num5 + 2];
						}
						for (int l = 0; l < num4; l++)
						{
							array4[num5 + l] = color;
						}
						for (int m = 0; m < array2.Length; m++)
						{
							GridNodeBase neighbourAlongDirection3 = gridNodeBase.GetNeighbourAlongDirection(array2[(m + 1) % array2.Length]);
							if (neighbourAlongDirection3 == null || (this.showMeshOutline && gridNodeBase.NodeInGridIndex < neighbourAlongDirection3.NodeInGridIndex))
							{
								helper.builder.DrawLine(array3[num5 + m], array3[num5 + (m + 1) % array2.Length], (neighbourAlongDirection3 == null) ? Color.black : color);
							}
						}
						num5 += num4;
					}
				}
			}
			if (this.showMeshSurface)
			{
				helper.DrawTriangles(array3, array4, num5 * num3 / num4);
			}
			ArrayPool<Vector3>.Release(ref array3, false);
			ArrayPool<Color>.Release(ref array4, false);
		}

		public IntRect GetRectFromBounds(Bounds bounds)
		{
			bounds = this.transform.InverseTransform(bounds);
			Vector3 min = bounds.min;
			Vector3 max = bounds.max;
			int xmin = Mathf.RoundToInt(min.x - 0.5f);
			int xmax = Mathf.RoundToInt(max.x - 0.5f);
			int ymin = Mathf.RoundToInt(min.z - 0.5f);
			int ymax = Mathf.RoundToInt(max.z - 0.5f);
			IntRect a = new IntRect(xmin, ymin, xmax, ymax);
			IntRect b = new IntRect(0, 0, this.width - 1, this.depth - 1);
			return IntRect.Intersection(a, b);
		}

		[Obsolete("This method has been renamed to GetNodesInRegion", true)]
		public List<GraphNode> GetNodesInArea(Bounds bounds)
		{
			return this.GetNodesInRegion(bounds);
		}

		[Obsolete("This method has been renamed to GetNodesInRegion", true)]
		public List<GraphNode> GetNodesInArea(GraphUpdateShape shape)
		{
			return this.GetNodesInRegion(shape);
		}

		[Obsolete("This method has been renamed to GetNodesInRegion", true)]
		public List<GraphNode> GetNodesInArea(Bounds bounds, GraphUpdateShape shape)
		{
			return this.GetNodesInRegion(bounds, shape);
		}

		public List<GraphNode> GetNodesInRegion(Bounds bounds)
		{
			return this.GetNodesInRegion(bounds, null);
		}

		public List<GraphNode> GetNodesInRegion(GraphUpdateShape shape)
		{
			return this.GetNodesInRegion(shape.GetBounds(), shape);
		}

		protected virtual List<GraphNode> GetNodesInRegion(Bounds bounds, GraphUpdateShape shape)
		{
			IntRect rectFromBounds = this.GetRectFromBounds(bounds);
			if (this.nodes == null || !rectFromBounds.IsValid() || this.nodes.Length != this.width * this.depth)
			{
				return ListPool<GraphNode>.Claim();
			}
			List<GraphNode> list = ListPool<GraphNode>.Claim(rectFromBounds.Width * rectFromBounds.Height);
			for (int i = rectFromBounds.xmin; i <= rectFromBounds.xmax; i++)
			{
				for (int j = rectFromBounds.ymin; j <= rectFromBounds.ymax; j++)
				{
					int num = j * this.width + i;
					GraphNode graphNode = this.nodes[num];
					if (bounds.Contains((Vector3)graphNode.position) && (shape == null || shape.Contains((Vector3)graphNode.position)))
					{
						list.Add(graphNode);
					}
				}
			}
			return list;
		}

		public virtual List<GraphNode> GetNodesInRegion(IntRect rect)
		{
			IntRect b = new IntRect(0, 0, this.width - 1, this.depth - 1);
			rect = IntRect.Intersection(rect, b);
			if (this.nodes == null || !rect.IsValid() || this.nodes.Length != this.width * this.depth)
			{
				return ListPool<GraphNode>.Claim(0);
			}
			List<GraphNode> list = ListPool<GraphNode>.Claim(rect.Width * rect.Height);
			for (int i = rect.ymin; i <= rect.ymax; i++)
			{
				int num = i * this.Width;
				for (int j = rect.xmin; j <= rect.xmax; j++)
				{
					list.Add(this.nodes[num + j]);
				}
			}
			return list;
		}

		public virtual int GetNodesInRegion(IntRect rect, GridNodeBase[] buffer)
		{
			IntRect b = new IntRect(0, 0, this.width - 1, this.depth - 1);
			rect = IntRect.Intersection(rect, b);
			if (this.nodes == null || !rect.IsValid() || this.nodes.Length != this.width * this.depth)
			{
				return 0;
			}
			if (buffer.Length < rect.Width * rect.Height)
			{
				throw new ArgumentException("Buffer is too small");
			}
			int num = 0;
			int i = rect.ymin;
			while (i <= rect.ymax)
			{
				Array.Copy(this.nodes, i * this.Width + rect.xmin, buffer, num, rect.Width);
				i++;
				num += rect.Width;
			}
			return num;
		}

		public virtual GridNodeBase GetNode(int x, int z)
		{
			if (x < 0 || z < 0 || x >= this.width || z >= this.depth)
			{
				return null;
			}
			return this.nodes[x + z * this.width];
		}

		GraphUpdateThreading IUpdatableGraph.CanUpdateAsync(GraphUpdateObject o)
		{
			return GraphUpdateThreading.UnityThread;
		}

		void IUpdatableGraph.UpdateAreaInit(GraphUpdateObject o)
		{
		}

		void IUpdatableGraph.UpdateAreaPost(GraphUpdateObject o)
		{
		}

		protected void CalculateAffectedRegions(GraphUpdateObject o, out IntRect originalRect, out IntRect affectRect, out IntRect physicsRect, out bool willChangeWalkability, out int erosion)
		{
			Bounds bounds = this.transform.InverseTransform(o.bounds);
			Vector3 vector = bounds.min;
			Vector3 vector2 = bounds.max;
			int xmin = Mathf.RoundToInt(vector.x - 0.5f);
			int xmax = Mathf.RoundToInt(vector2.x - 0.5f);
			int ymin = Mathf.RoundToInt(vector.z - 0.5f);
			int ymax = Mathf.RoundToInt(vector2.z - 0.5f);
			originalRect = new IntRect(xmin, ymin, xmax, ymax);
			affectRect = originalRect;
			physicsRect = originalRect;
			erosion = (o.updateErosion ? this.erodeIterations : 0);
			willChangeWalkability = (o.updatePhysics || o.modifyWalkability);
			if (o.updatePhysics && !o.modifyWalkability && this.collision.collisionCheck)
			{
				Vector3 a = new Vector3(this.collision.diameter, 0f, this.collision.diameter) * 0.5f;
				vector -= a * 1.02f;
				vector2 += a * 1.02f;
				physicsRect = new IntRect(Mathf.RoundToInt(vector.x - 0.5f), Mathf.RoundToInt(vector.z - 0.5f), Mathf.RoundToInt(vector2.x - 0.5f), Mathf.RoundToInt(vector2.z - 0.5f));
				affectRect = IntRect.Union(physicsRect, affectRect);
			}
			if (willChangeWalkability || erosion > 0)
			{
				affectRect = affectRect.Expand(erosion + 1);
			}
		}

		void IUpdatableGraph.UpdateArea(GraphUpdateObject o)
		{
			if (this.nodes == null || this.nodes.Length != this.width * this.depth)
			{
				Debug.LogWarning("The Grid Graph is not scanned, cannot update area");
				return;
			}
			IntRect a;
			IntRect a2;
			IntRect intRect;
			bool flag;
			int num;
			this.CalculateAffectedRegions(o, out a, out a2, out intRect, out flag, out num);
			IntRect b = new IntRect(0, 0, this.width - 1, this.depth - 1);
			IntRect intRect2 = IntRect.Intersection(a2, b);
			for (int i = intRect2.ymin; i <= intRect2.ymax; i++)
			{
				for (int j = intRect2.xmin; j <= intRect2.xmax; j++)
				{
					o.WillUpdateNode(this.nodes[i * this.width + j]);
				}
			}
			if (o.updatePhysics && !o.modifyWalkability)
			{
				this.collision.Initialize(this.transform, this.nodeSize);
				intRect2 = IntRect.Intersection(intRect, b);
				for (int k = intRect2.ymin; k <= intRect2.ymax; k++)
				{
					for (int l = intRect2.xmin; l <= intRect2.xmax; l++)
					{
						this.RecalculateCell(l, k, o.resetPenaltyOnPhysics, false);
					}
				}
			}
			intRect2 = IntRect.Intersection(a, b);
			for (int m = intRect2.ymin; m <= intRect2.ymax; m++)
			{
				for (int n = intRect2.xmin; n <= intRect2.xmax; n++)
				{
					int num2 = m * this.width + n;
					GridNodeBase gridNodeBase = this.nodes[num2];
					if (o.bounds.Contains((Vector3)gridNodeBase.position))
					{
						if (flag)
						{
							gridNodeBase.Walkable = gridNodeBase.WalkableErosion;
							o.Apply(gridNodeBase);
							gridNodeBase.WalkableErosion = gridNodeBase.Walkable;
						}
						else
						{
							o.Apply(gridNodeBase);
						}
					}
				}
			}
			if (flag && num == 0)
			{
				intRect2 = IntRect.Intersection(a2, b);
				for (int num3 = intRect2.xmin; num3 <= intRect2.xmax; num3++)
				{
					for (int num4 = intRect2.ymin; num4 <= intRect2.ymax; num4++)
					{
						this.CalculateConnections(num3, num4);
					}
				}
				return;
			}
			if (flag && num > 0)
			{
				IntRect a3 = IntRect.Union(a, intRect).Expand(num);
				IntRect intRect3 = a3.Expand(num);
				a3 = IntRect.Intersection(a3, b);
				intRect3 = IntRect.Intersection(intRect3, b);
				for (int num5 = intRect3.xmin; num5 <= intRect3.xmax; num5++)
				{
					for (int num6 = intRect3.ymin; num6 <= intRect3.ymax; num6++)
					{
						int num7 = num6 * this.width + num5;
						GridNodeBase gridNodeBase2 = this.nodes[num7];
						bool walkable = gridNodeBase2.Walkable;
						gridNodeBase2.Walkable = gridNodeBase2.WalkableErosion;
						if (!a3.Contains(num5, num6))
						{
							gridNodeBase2.TmpWalkable = walkable;
						}
					}
				}
				for (int num8 = intRect3.xmin; num8 <= intRect3.xmax; num8++)
				{
					for (int num9 = intRect3.ymin; num9 <= intRect3.ymax; num9++)
					{
						this.CalculateConnections(num8, num9);
					}
				}
				this.ErodeWalkableArea(intRect3.xmin, intRect3.ymin, intRect3.xmax + 1, intRect3.ymax + 1);
				for (int num10 = intRect3.xmin; num10 <= intRect3.xmax; num10++)
				{
					for (int num11 = intRect3.ymin; num11 <= intRect3.ymax; num11++)
					{
						if (!a3.Contains(num10, num11))
						{
							int num12 = num11 * this.width + num10;
							GridNodeBase gridNodeBase3 = this.nodes[num12];
							gridNodeBase3.Walkable = gridNodeBase3.TmpWalkable;
						}
					}
				}
				for (int num13 = intRect3.xmin; num13 <= intRect3.xmax; num13++)
				{
					for (int num14 = intRect3.ymin; num14 <= intRect3.ymax; num14++)
					{
						this.CalculateConnections(num13, num14);
					}
				}
			}
		}

		public bool Linecast(Vector3 from, Vector3 to)
		{
			GraphHitInfo graphHitInfo;
			return this.Linecast(from, to, out graphHitInfo, null, null);
		}

		[Obsolete("The hint parameter is deprecated")]
		public bool Linecast(Vector3 from, Vector3 to, GraphNode hint)
		{
			GraphHitInfo graphHitInfo;
			return this.Linecast(from, to, hint, out graphHitInfo);
		}

		[Obsolete("The hint parameter is deprecated")]
		public bool Linecast(Vector3 from, Vector3 to, GraphNode hint, out GraphHitInfo hit)
		{
			return this.Linecast(from, to, hint, out hit, null);
		}

		protected static long CrossMagnitude(Int2 a, Int2 b)
		{
			return (long)a.x * (long)b.y - (long)b.x * (long)a.y;
		}

		protected bool ClipLineSegmentToBounds(Vector3 a, Vector3 b, out Vector3 outA, out Vector3 outB)
		{
			if (a.x < 0f || a.z < 0f || a.x > (float)this.width || a.z > (float)this.depth || b.x < 0f || b.z < 0f || b.x > (float)this.width || b.z > (float)this.depth)
			{
				Vector3 vector = new Vector3(0f, 0f, 0f);
				Vector3 vector2 = new Vector3(0f, 0f, (float)this.depth);
				Vector3 vector3 = new Vector3((float)this.width, 0f, (float)this.depth);
				Vector3 vector4 = new Vector3((float)this.width, 0f, 0f);
				int num = 0;
				bool flag;
				Vector3 vector5 = VectorMath.SegmentIntersectionPointXZ(a, b, vector, vector2, out flag);
				if (flag)
				{
					num++;
					if (!VectorMath.RightOrColinearXZ(vector, vector2, a))
					{
						a = vector5;
					}
					else
					{
						b = vector5;
					}
				}
				vector5 = VectorMath.SegmentIntersectionPointXZ(a, b, vector2, vector3, out flag);
				if (flag)
				{
					num++;
					if (!VectorMath.RightOrColinearXZ(vector2, vector3, a))
					{
						a = vector5;
					}
					else
					{
						b = vector5;
					}
				}
				vector5 = VectorMath.SegmentIntersectionPointXZ(a, b, vector3, vector4, out flag);
				if (flag)
				{
					num++;
					if (!VectorMath.RightOrColinearXZ(vector3, vector4, a))
					{
						a = vector5;
					}
					else
					{
						b = vector5;
					}
				}
				vector5 = VectorMath.SegmentIntersectionPointXZ(a, b, vector4, vector, out flag);
				if (flag)
				{
					num++;
					if (!VectorMath.RightOrColinearXZ(vector4, vector, a))
					{
						a = vector5;
					}
					else
					{
						b = vector5;
					}
				}
				if (num == 0)
				{
					outA = Vector3.zero;
					outB = Vector3.zero;
					return false;
				}
			}
			outA = a;
			outB = b;
			return true;
		}

		[Obsolete("The hint parameter is deprecated")]
		public bool Linecast(Vector3 from, Vector3 to, GraphNode hint, out GraphHitInfo hit, List<GraphNode> trace)
		{
			return this.Linecast(from, to, out hit, trace, null);
		}

		public bool Linecast(Vector3 from, Vector3 to, out GraphHitInfo hit, List<GraphNode> trace = null, Func<GraphNode, bool> filter = null)
		{
			hit = default(GraphHitInfo);
			GridHitInfo gridHitInfo;
			bool flag = this.Linecast(from, to, out gridHitInfo, trace, filter);
			hit.origin = from;
			hit.node = gridHitInfo.node;
			if (!flag)
			{
				hit = default(GraphHitInfo);
				return flag;
			}
			int direction = gridHitInfo.direction;
			if (direction == -1 || gridHitInfo.node == null)
			{
				hit.point = ((gridHitInfo.node != null) ? ((Vector3)gridHitInfo.node.position) : from);
				hit.tangentOrigin = Vector3.zero;
				hit.tangent = Vector3.zero;
				return flag;
			}
			Vector3 vector = this.transform.InverseTransform(from);
			Vector3 vector2 = this.transform.InverseTransform(to);
			Vector2 start = new Vector2(vector.x - 0.5f, vector.z - 0.5f);
			Vector2 end = new Vector2(vector2.x - 0.5f, vector2.z - 0.5f);
			Vector2 a = new Vector2((float)this.neighbourXOffsets[direction], (float)this.neighbourZOffsets[direction]);
			Vector2 b = new Vector2((float)this.neighbourXOffsets[direction - 1 + 4 & 3], (float)this.neighbourZOffsets[direction - 1 + 4 & 3]);
			Vector2 vector3 = new Vector2((float)this.neighbourXOffsets[direction + 1 & 3], (float)this.neighbourZOffsets[direction + 1 & 3]);
			Vector2 vector4 = new Vector2((float)gridHitInfo.node.XCoordinateInGrid, (float)gridHitInfo.node.ZCoordinateInGrid) + (a + b) * 0.5f;
			Vector2 vector5 = VectorMath.LineIntersectionPoint(vector4, vector4 + vector3, start, end);
			Vector3 vector6 = this.transform.InverseTransform((Vector3)gridHitInfo.node.position);
			Vector3 point = new Vector3(vector5.x + 0.5f, vector6.y, vector5.y + 0.5f);
			Vector3 point2 = new Vector3(vector4.x + 0.5f, vector6.y, vector4.y + 0.5f);
			hit.point = this.transform.Transform(point);
			hit.tangentOrigin = this.transform.Transform(point2);
			hit.tangent = this.transform.TransformVector(new Vector3(vector3.x, 0f, vector3.y));
			return flag;
		}

		[Obsolete("Use Linecast instead")]
		public bool SnappedLinecast(Vector3 from, Vector3 to, GraphNode hint, out GraphHitInfo hit)
		{
			return this.Linecast((Vector3)base.GetNearest(from, null).node.position, (Vector3)base.GetNearest(to, null).node.position, hint, out hit);
		}

		public bool Linecast(GridNodeBase fromNode, GridNodeBase toNode, Func<GraphNode, bool> filter = null)
		{
			GridHitInfo gridHitInfo;
			return this.Linecast(fromNode, new Vector2(0.5f, 0.5f), toNode, new Vector2(0.5f, 0.5f), out gridHitInfo, null, filter, false);
		}

		public bool Linecast(Vector3 from, Vector3 to, out GridHitInfo hit, List<GraphNode> trace = null, Func<GraphNode, bool> filter = null)
		{
			Vector3 a = this.transform.InverseTransform(from);
			Vector3 vector = this.transform.InverseTransform(to);
			Vector3 vector2;
			Vector3 vector3;
			if (!this.ClipLineSegmentToBounds(a, vector, out vector2, out vector3))
			{
				hit = new GridHitInfo
				{
					node = null,
					direction = -1
				};
				return false;
			}
			if ((a - vector2).sqrMagnitude > 1.0000001E-06f)
			{
				hit = new GridHitInfo
				{
					node = null,
					direction = -1
				};
				return true;
			}
			bool continuePastEnd = (vector - vector3).sqrMagnitude > 1.0000001E-06f;
			GridNodeBase nearestFromGraphSpace = this.GetNearestFromGraphSpace(vector2);
			GridNodeBase nearestFromGraphSpace2 = this.GetNearestFromGraphSpace(vector3);
			if (nearestFromGraphSpace == null || nearestFromGraphSpace2 == null)
			{
				hit = new GridHitInfo
				{
					node = null,
					direction = -1
				};
				return false;
			}
			return this.Linecast(nearestFromGraphSpace, new Vector2(vector2.x - (float)nearestFromGraphSpace.XCoordinateInGrid, vector2.z - (float)nearestFromGraphSpace.ZCoordinateInGrid), nearestFromGraphSpace2, new Vector2(vector3.x - (float)nearestFromGraphSpace2.XCoordinateInGrid, vector3.z - (float)nearestFromGraphSpace2.ZCoordinateInGrid), out hit, trace, filter, continuePastEnd);
		}

		public bool Linecast(GridNodeBase fromNode, Vector2 normalizedFromPoint, GridNodeBase toNode, Vector2 normalizedToPoint, out GridHitInfo hit, List<GraphNode> trace = null, Func<GraphNode, bool> filter = null, bool continuePastEnd = false)
		{
			Int2 fixedNormalizedFromPoint = new Int2((int)Mathf.Round(normalizedFromPoint.x * 1024f), (int)Mathf.Round(normalizedFromPoint.y * 1024f));
			Int2 fixedNormalizedToPoint = new Int2((int)Mathf.Round(normalizedToPoint.x * 1024f), (int)Mathf.Round(normalizedToPoint.y * 1024f));
			return this.Linecast(fromNode, fixedNormalizedFromPoint, toNode, fixedNormalizedToPoint, out hit, trace, filter, continuePastEnd);
		}

		public bool Linecast(GridNodeBase fromNode, Int2 fixedNormalizedFromPoint, GridNodeBase toNode, Int2 fixedNormalizedToPoint, out GridHitInfo hit, List<GraphNode> trace = null, Func<GraphNode, bool> filter = null, bool continuePastEnd = false)
		{
			if (fixedNormalizedFromPoint.x < 0 || fixedNormalizedFromPoint.x > 1024)
			{
				throw new ArgumentOutOfRangeException("normalizedFromPoint must be between 0 and 1");
			}
			if (fixedNormalizedToPoint.x < 0 || fixedNormalizedToPoint.x > 1024)
			{
				throw new ArgumentOutOfRangeException("normalizedToPoint must be between 0 and 1");
			}
			if (fromNode == null)
			{
				throw new ArgumentNullException("fromNode");
			}
			if (toNode == null)
			{
				throw new ArgumentNullException("toNode");
			}
			if ((filter != null && !filter(fromNode)) || !fromNode.Walkable)
			{
				hit = new GridHitInfo
				{
					node = fromNode,
					direction = -1
				};
				return true;
			}
			if (fromNode == toNode)
			{
				hit = new GridHitInfo
				{
					node = fromNode,
					direction = -1
				};
				return false;
			}
			Int2 @int = new Int2(fromNode.XCoordinateInGrid, fromNode.ZCoordinateInGrid);
			Int2 int2 = new Int2(toNode.XCoordinateInGrid, toNode.ZCoordinateInGrid);
			Int2 int3 = new Int2(@int.x * 1024, @int.y * 1024) + fixedNormalizedFromPoint;
			Int2 int4 = new Int2(int2.x * 1024, int2.y * 1024) + fixedNormalizedToPoint;
			Int2 int5 = int4 - int3;
			int i = Math.Abs(@int.x - int2.x) + Math.Abs(@int.y - int2.y);
			if (continuePastEnd)
			{
				i = int.MaxValue;
			}
			if (int3 == int4)
			{
				i = 0;
			}
			int num = 0;
			Int2 int6 = int5;
			if (int6.x == 0)
			{
				int6.x = Math.Sign(512 - fixedNormalizedToPoint.x);
			}
			if (int6.y == 0)
			{
				int6.y = Math.Sign(512 - fixedNormalizedToPoint.y);
			}
			if (int6.x <= 0 && int6.y > 0)
			{
				num = 1;
			}
			else if (int6.x < 0 && int6.y <= 0)
			{
				num = 2;
			}
			else if (int6.x >= 0 && int6.y < 0)
			{
				num = 3;
			}
			int num2 = num + 1 & 3;
			int num3 = num + 2 & 3;
			long num4 = GridGraph.CrossMagnitude(int5, new Int2(this.neighbourXOffsets[num3] + this.neighbourXOffsets[num2], this.neighbourZOffsets[num3] + this.neighbourZOffsets[num2]));
			Int2 b = new Int2(512, 512) - fixedNormalizedFromPoint;
			long num5 = GridGraph.CrossMagnitude(int5, b) * 2L / 1024L;
			long num6 = (long)(-int5.y * 2);
			long num7 = (long)(int5.x * 2);
			int num8 = num3;
			int num9 = num2;
			Int2 a = new Int2(int2.x * 1024, int2.y * 1024) + new Int2(512, 512);
			if (GridGraph.CrossMagnitude(int5, a - int3) < 0L)
			{
				num8 = num2;
				num9 = num3;
			}
			GridNodeBase gridNodeBase = null;
			GridNodeBase gridNodeBase2 = null;
			while (i > 0)
			{
				if (trace != null)
				{
					trace.Add(fromNode);
				}
				long num10 = num5 + num4;
				int num11;
				GridNodeBase gridNodeBase3;
				if (num10 == 0L)
				{
					num11 = num8;
					gridNodeBase3 = fromNode.GetNeighbourAlongDirection(num11);
					if ((filter != null && gridNodeBase3 != null && !filter(gridNodeBase3)) || gridNodeBase3 == gridNodeBase)
					{
						gridNodeBase3 = null;
					}
					if (gridNodeBase3 == null)
					{
						num11 = num9;
						gridNodeBase3 = fromNode.GetNeighbourAlongDirection(num11);
						if ((filter != null && gridNodeBase3 != null && !filter(gridNodeBase3)) || gridNodeBase3 == gridNodeBase)
						{
							gridNodeBase3 = null;
						}
					}
				}
				else
				{
					num11 = ((num10 < 0L) ? num3 : num2);
					gridNodeBase3 = fromNode.GetNeighbourAlongDirection(num11);
					if ((filter != null && gridNodeBase3 != null && !filter(gridNodeBase3)) || gridNodeBase3 == gridNodeBase)
					{
						gridNodeBase3 = null;
					}
				}
				if (gridNodeBase3 == null)
				{
					int j = -1;
					while (j <= 1)
					{
						int num12 = num11 + j + 4 & 3;
						if (num5 + num6 / 2L * (long)(this.neighbourXOffsets[num11] + this.neighbourXOffsets[num12]) + num7 / 2L * (long)(this.neighbourZOffsets[num11] + this.neighbourZOffsets[num12]) == 0L)
						{
							gridNodeBase3 = fromNode.GetNeighbourAlongDirection(num12);
							if ((filter != null && gridNodeBase3 != null && !filter(gridNodeBase3)) || gridNodeBase3 == gridNodeBase || gridNodeBase3 == gridNodeBase2)
							{
								gridNodeBase3 = null;
							}
							if (gridNodeBase3 != null)
							{
								i = 1 + Math.Abs(gridNodeBase3.XCoordinateInGrid - int2.x) + Math.Abs(gridNodeBase3.ZCoordinateInGrid - int2.y);
								num11 = num12;
								gridNodeBase = fromNode;
								gridNodeBase2 = gridNodeBase3;
								break;
							}
							break;
						}
						else
						{
							j += 2;
						}
					}
					if (gridNodeBase3 == null)
					{
						hit = new GridHitInfo
						{
							node = fromNode,
							direction = num11
						};
						return true;
					}
				}
				num5 += num6 * (long)this.neighbourXOffsets[num11] + num7 * (long)this.neighbourZOffsets[num11];
				fromNode = gridNodeBase3;
				i--;
			}
			hit = new GridHitInfo
			{
				node = fromNode,
				direction = -1
			};
			if (fromNode != toNode)
			{
				Int2 int7 = int4 - (new Int2(fromNode.XCoordinateInGrid * 1024, fromNode.ZCoordinateInGrid * 1024) + new Int2(512, 512));
				if (Mathf.Abs(int7.x) == 512 && Mathf.Abs(int7.y) == 512)
				{
					Int2 int8 = new Int2(int7.x * 2 / 1024, int7.y * 2 / 1024);
					int num13 = -1;
					for (int k = 0; k < 4; k++)
					{
						if (this.neighbourXOffsets[k] + this.neighbourXOffsets[k + 1 & 3] == int8.x && this.neighbourZOffsets[k] + this.neighbourZOffsets[k + 1 & 3] == int8.y)
						{
							num13 = k;
							break;
						}
					}
					int num14 = (trace != null) ? trace.Count : 0;
					int num15 = num13;
					GridNodeBase gridNodeBase4 = fromNode;
					int num16 = 0;
					while (num16 < 3 && gridNodeBase4 != toNode)
					{
						if (trace != null)
						{
							trace.Add(gridNodeBase4);
						}
						gridNodeBase4 = gridNodeBase4.GetNeighbourAlongDirection(num15);
						if (gridNodeBase4 == null || (filter != null && !filter(gridNodeBase4)))
						{
							gridNodeBase4 = null;
							break;
						}
						num15 = (num15 + 1 & 3);
						num16++;
					}
					if (gridNodeBase4 != toNode)
					{
						if (trace != null)
						{
							trace.RemoveRange(num14, trace.Count - num14);
						}
						gridNodeBase4 = fromNode;
						num15 = (num13 + 1 & 3);
						int num17 = 0;
						while (num17 < 3 && gridNodeBase4 != toNode)
						{
							if (trace != null)
							{
								trace.Add(gridNodeBase4);
							}
							gridNodeBase4 = gridNodeBase4.GetNeighbourAlongDirection(num15);
							if (gridNodeBase4 == null || (filter != null && !filter(gridNodeBase4)))
							{
								gridNodeBase4 = null;
								break;
							}
							num15 = (num15 - 1 + 4 & 3);
							num17++;
						}
						if (gridNodeBase4 != toNode && trace != null)
						{
							trace.RemoveRange(num14, trace.Count - num14);
						}
					}
					fromNode = gridNodeBase4;
				}
			}
			if (trace != null)
			{
				trace.Add(fromNode);
			}
			return fromNode != toNode;
		}

		public bool CheckConnection(GridNode node, int dir)
		{
			if (this.neighbours == NumNeighbours.Eight || this.neighbours == NumNeighbours.Six || dir < 4)
			{
				return node.HasConnectionInDirection(dir);
			}
			int num = dir - 4 - 1 & 3;
			int num2 = dir - 4 + 1 & 3;
			if (!node.HasConnectionInDirection(num) || !node.HasConnectionInDirection(num2))
			{
				return false;
			}
			GridNodeBase gridNodeBase = this.nodes[node.NodeInGridIndex + this.neighbourOffsets[num]];
			GridNodeBase gridNodeBase2 = this.nodes[node.NodeInGridIndex + this.neighbourOffsets[num2]];
			return gridNodeBase.Walkable && gridNodeBase2.Walkable && gridNodeBase2.HasConnectionInDirection(num) && gridNodeBase.HasConnectionInDirection(num2);
		}

		protected override void SerializeExtraInfo(GraphSerializationContext ctx)
		{
			if (this.nodes == null)
			{
				ctx.writer.Write(-1);
				return;
			}
			ctx.writer.Write(this.nodes.Length);
			for (int i = 0; i < this.nodes.Length; i++)
			{
				this.nodes[i].SerializeNode(ctx);
			}
		}

		protected override void DeserializeExtraInfo(GraphSerializationContext ctx)
		{
			int num = ctx.reader.ReadInt32();
			if (num == -1)
			{
				this.nodes = null;
				return;
			}
			GridNodeBase[] array = new GridNode[num];
			this.nodes = array;
			for (int i = 0; i < this.nodes.Length; i++)
			{
				this.nodes[i] = new GridNode(this.active);
				this.nodes[i].DeserializeNode(ctx);
			}
		}

		protected override void DeserializeSettingsCompatibility(GraphSerializationContext ctx)
		{
			base.DeserializeSettingsCompatibility(ctx);
			this.aspectRatio = ctx.reader.ReadSingle();
			this.rotation = ctx.DeserializeVector3();
			this.center = ctx.DeserializeVector3();
			this.unclampedSize = ctx.DeserializeVector3();
			this.nodeSize = ctx.reader.ReadSingle();
			this.collision.DeserializeSettingsCompatibility(ctx);
			this.maxClimb = ctx.reader.ReadSingle();
			ctx.reader.ReadInt32();
			this.maxSlope = ctx.reader.ReadSingle();
			this.erodeIterations = ctx.reader.ReadInt32();
			this.erosionUseTags = ctx.reader.ReadBoolean();
			this.erosionFirstTag = ctx.reader.ReadInt32();
			ctx.reader.ReadBoolean();
			this.neighbours = (NumNeighbours)ctx.reader.ReadInt32();
			this.cutCorners = ctx.reader.ReadBoolean();
			this.penaltyPosition = ctx.reader.ReadBoolean();
			this.penaltyPositionFactor = ctx.reader.ReadSingle();
			this.penaltyAngle = ctx.reader.ReadBoolean();
			this.penaltyAngleFactor = ctx.reader.ReadSingle();
			this.penaltyAnglePower = ctx.reader.ReadSingle();
			this.isometricAngle = ctx.reader.ReadSingle();
			this.uniformEdgeCosts = ctx.reader.ReadBoolean();
			this.useJumpPointSearch = ctx.reader.ReadBoolean();
		}

		protected override void PostDeserialization(GraphSerializationContext ctx)
		{
			this.UpdateTransform();
			this.SetUpOffsetsAndCosts();
			GridNode.SetGridGraph((int)this.graphIndex, this);
			if (this.nodes == null || this.nodes.Length == 0)
			{
				return;
			}
			if (this.width * this.depth != this.nodes.Length)
			{
				Debug.LogError("Node data did not match with bounds data. Probably a change to the bounds/width/depth data was made after scanning the graph just prior to saving it. Nodes will be discarded");
				GridNodeBase[] array = new GridNode[0];
				this.nodes = array;
				return;
			}
			for (int i = 0; i < this.depth; i++)
			{
				for (int j = 0; j < this.width; j++)
				{
					GridNodeBase gridNodeBase = this.nodes[i * this.width + j];
					if (gridNodeBase == null)
					{
						Debug.LogError("Deserialization Error : Couldn't cast the node to the appropriate type - GridGenerator");
						return;
					}
					gridNodeBase.NodeInGridIndex = i * this.width + j;
				}
			}
		}

		[JsonMember]
		public InspectorGridMode inspectorGridMode;

		[JsonMember]
		public InspectorGridHexagonNodeSize inspectorHexagonSizeMode;

		public int width;

		public int depth;

		[JsonMember]
		public float aspectRatio = 1f;

		[JsonMember]
		public float isometricAngle;

		public static readonly float StandardIsometricAngle = 90f - Mathf.Atan(1f / Mathf.Sqrt(2f)) * 57.29578f;

		public static readonly float StandardDimetricAngle = Mathf.Acos(0.5f) * 57.29578f;

		[JsonMember]
		public bool uniformEdgeCosts;

		[JsonMember]
		public Vector3 rotation;

		[JsonMember]
		public Vector3 center;

		[JsonMember]
		public Vector2 unclampedSize;

		[JsonMember]
		public float nodeSize = 1f;

		[JsonMember]
		public GraphCollision collision;

		[JsonMember]
		public float maxClimb = 0.4f;

		[JsonMember]
		public float maxSlope = 90f;

		[JsonMember]
		public int erodeIterations;

		[JsonMember]
		public bool erosionUseTags;

		[JsonMember]
		public int erosionFirstTag = 1;

		[JsonMember]
		public NumNeighbours neighbours = NumNeighbours.Eight;

		[JsonMember]
		public bool cutCorners = true;

		[JsonMember]
		public float penaltyPositionOffset;

		[JsonMember]
		public bool penaltyPosition;

		[JsonMember]
		public float penaltyPositionFactor = 1f;

		[JsonMember]
		public bool penaltyAngle;

		[JsonMember]
		public float penaltyAngleFactor = 100f;

		[JsonMember]
		public float penaltyAnglePower = 1f;

		[JsonMember]
		public bool useJumpPointSearch;

		[JsonMember]
		public bool showMeshOutline = true;

		[JsonMember]
		public bool showNodeConnections;

		[JsonMember]
		public bool showMeshSurface = true;

		[JsonMember]
		public GridGraph.TextureData textureData = new GridGraph.TextureData();

		[NonSerialized]
		public readonly int[] neighbourOffsets = new int[8];

		[NonSerialized]
		public readonly uint[] neighbourCosts = new uint[8];

		[NonSerialized]
		public readonly int[] neighbourXOffsets = new int[8];

		[NonSerialized]
		public readonly int[] neighbourZOffsets = new int[8];

		internal static readonly int[] hexagonNeighbourIndices = new int[]
		{
			0,
			1,
			5,
			2,
			3,
			7
		};

		public const int getNearestForceOverlap = 2;

		public GridNodeBase[] nodes;

		private const int FixedPrecisionScale = 1024;

		public class TextureData
		{
			public void Initialize()
			{
				if (this.enabled && this.source != null)
				{
					for (int i = 0; i < this.channels.Length; i++)
					{
						if (this.channels[i] != GridGraph.TextureData.ChannelUse.None)
						{
							try
							{
								this.data = this.source.GetPixels32();
								break;
							}
							catch (UnityException ex)
							{
								Debug.LogWarning(ex.ToString());
								this.data = null;
								break;
							}
						}
					}
				}
			}

			public void Apply(GridNodeBase node, int x, int z)
			{
				if (this.enabled && this.data != null && x < this.source.width && z < this.source.height)
				{
					Color32 color = this.data[z * this.source.width + x];
					if (this.channels[0] != GridGraph.TextureData.ChannelUse.None)
					{
						this.ApplyChannel(node, x, z, (int)color.r, this.channels[0], this.factors[0]);
					}
					if (this.channels[1] != GridGraph.TextureData.ChannelUse.None)
					{
						this.ApplyChannel(node, x, z, (int)color.g, this.channels[1], this.factors[1]);
					}
					if (this.channels[2] != GridGraph.TextureData.ChannelUse.None)
					{
						this.ApplyChannel(node, x, z, (int)color.b, this.channels[2], this.factors[2]);
					}
					node.WalkableErosion = node.Walkable;
				}
			}

			private void ApplyChannel(GridNodeBase node, int x, int z, int value, GridGraph.TextureData.ChannelUse channelUse, float factor)
			{
				switch (channelUse)
				{
				case GridGraph.TextureData.ChannelUse.Penalty:
					node.Penalty += (uint)Mathf.RoundToInt((float)value * factor);
					return;
				case GridGraph.TextureData.ChannelUse.Position:
					node.position = GridNode.GetGridGraph(node.GraphIndex).GraphPointToWorld(x, z, (float)value);
					return;
				case GridGraph.TextureData.ChannelUse.WalkablePenalty:
					if (value == 0)
					{
						node.Walkable = false;
						return;
					}
					node.Penalty += (uint)Mathf.RoundToInt((float)(value - 1) * factor);
					return;
				default:
					return;
				}
			}

			public bool enabled;

			public Texture2D source;

			public float[] factors = new float[3];

			public GridGraph.TextureData.ChannelUse[] channels = new GridGraph.TextureData.ChannelUse[3];

			private Color32[] data;

			public enum ChannelUse
			{
				None,
				Penalty,
				Position,
				WalkablePenalty
			}
		}
	}
}
