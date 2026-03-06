using System;
using System.Collections.Generic;
using Pathfinding.Serialization;
using Pathfinding.Util;
using UnityEngine;

namespace Pathfinding
{
	public abstract class NavGraph : IGraphInternals
	{
		internal bool exists
		{
			get
			{
				return this.active != null;
			}
		}

		public virtual int CountNodes()
		{
			int count = 0;
			this.GetNodes(delegate(GraphNode node)
			{
				int count = count;
				count++;
			});
			return count;
		}

		public void GetNodes(Func<GraphNode, bool> action)
		{
			bool cont = true;
			this.GetNodes(delegate(GraphNode node)
			{
				if (cont)
				{
					cont &= action(node);
				}
			});
		}

		public abstract void GetNodes(Action<GraphNode> action);

		[Obsolete("Use the transform field (only available on some graph types) instead", true)]
		public void SetMatrix(Matrix4x4 m)
		{
			this.matrix = m;
			this.inverseMatrix = m.inverse;
		}

		[Obsolete("Use RelocateNodes(Matrix4x4) instead. To keep the same behavior you can call RelocateNodes(newMatrix * oldMatrix.inverse).")]
		public void RelocateNodes(Matrix4x4 oldMatrix, Matrix4x4 newMatrix)
		{
			this.RelocateNodes(newMatrix * oldMatrix.inverse);
		}

		protected void AssertSafeToUpdateGraph()
		{
			if (!this.active.IsAnyWorkItemInProgress && !this.active.isScanning)
			{
				throw new Exception("Trying to update graphs when it is not safe to do so. Graph updates must be done inside a work item or when a graph is being scanned. See AstarPath.AddWorkItem");
			}
		}

		public virtual void RelocateNodes(Matrix4x4 deltaMatrix)
		{
			this.GetNodes(delegate(GraphNode node)
			{
				node.position = (Int3)deltaMatrix.MultiplyPoint((Vector3)node.position);
			});
		}

		public NNInfoInternal GetNearest(Vector3 position)
		{
			return this.GetNearest(position, NNConstraint.None);
		}

		public NNInfoInternal GetNearest(Vector3 position, NNConstraint constraint)
		{
			return this.GetNearest(position, constraint, null);
		}

		public virtual NNInfoInternal GetNearest(Vector3 position, NNConstraint constraint, GraphNode hint)
		{
			float maxDistSqr = (constraint == null || constraint.constrainDistance) ? AstarPath.active.maxNearestNodeDistanceSqr : float.PositiveInfinity;
			float minDist = float.PositiveInfinity;
			GraphNode minNode = null;
			float minConstDist = float.PositiveInfinity;
			GraphNode minConstNode = null;
			this.GetNodes(delegate(GraphNode node)
			{
				float sqrMagnitude = (position - (Vector3)node.position).sqrMagnitude;
				if (sqrMagnitude < minDist)
				{
					minDist = sqrMagnitude;
					minNode = node;
				}
				if (sqrMagnitude < minConstDist && sqrMagnitude < maxDistSqr && (constraint == null || constraint.Suitable(node)))
				{
					minConstDist = sqrMagnitude;
					minConstNode = node;
				}
			});
			NNInfoInternal result = new NNInfoInternal(minNode);
			result.constrainedNode = minConstNode;
			if (minConstNode != null)
			{
				result.constClampedPosition = (Vector3)minConstNode.position;
			}
			else if (minNode != null)
			{
				result.constrainedNode = minNode;
				result.constClampedPosition = (Vector3)minNode.position;
			}
			return result;
		}

		public virtual NNInfoInternal GetNearestForce(Vector3 position, NNConstraint constraint)
		{
			return this.GetNearest(position, constraint);
		}

		protected virtual void OnDestroy()
		{
			this.DestroyAllNodes();
		}

		protected virtual void DestroyAllNodes()
		{
			this.GetNodes(delegate(GraphNode node)
			{
				node.Destroy();
			});
		}

		[Obsolete("Use AstarPath.Scan instead")]
		public void ScanGraph()
		{
			this.Scan();
		}

		public void Scan()
		{
			this.active.Scan(this);
		}

		protected abstract IEnumerable<Progress> ScanInternal();

		protected virtual void SerializeExtraInfo(GraphSerializationContext ctx)
		{
		}

		protected virtual void DeserializeExtraInfo(GraphSerializationContext ctx)
		{
		}

		protected virtual void PostDeserialization(GraphSerializationContext ctx)
		{
		}

		protected virtual void DeserializeSettingsCompatibility(GraphSerializationContext ctx)
		{
			this.guid = new Guid(ctx.reader.ReadBytes(16));
			this.initialPenalty = ctx.reader.ReadUInt32();
			this.open = ctx.reader.ReadBoolean();
			this.name = ctx.reader.ReadString();
			this.drawGizmos = ctx.reader.ReadBoolean();
			this.infoScreenOpen = ctx.reader.ReadBoolean();
		}

		public virtual void OnDrawGizmos(RetainedGizmos gizmos, bool drawNodes)
		{
			if (!drawNodes)
			{
				return;
			}
			RetainedGizmos.Hasher hasher = new RetainedGizmos.Hasher(this.active);
			this.GetNodes(delegate(GraphNode node)
			{
				hasher.HashNode(node);
			});
			if (!gizmos.Draw(hasher))
			{
				using (GraphGizmoHelper gizmoHelper = gizmos.GetGizmoHelper(this.active, hasher))
				{
					this.GetNodes(new Action<GraphNode>(gizmoHelper.DrawConnections));
				}
			}
			if (this.active.showUnwalkableNodes)
			{
				this.DrawUnwalkableNodes(this.active.unwalkableNodeDebugSize);
			}
		}

		protected void DrawUnwalkableNodes(float size)
		{
			Gizmos.color = AstarColor.UnwalkableNode;
			this.GetNodes(delegate(GraphNode node)
			{
				if (!node.Walkable)
				{
					Gizmos.DrawCube((Vector3)node.position, Vector3.one * size);
				}
			});
		}

		string IGraphInternals.SerializedEditorSettings
		{
			get
			{
				return this.serializedEditorSettings;
			}
			set
			{
				this.serializedEditorSettings = value;
			}
		}

		void IGraphInternals.OnDestroy()
		{
			this.OnDestroy();
		}

		void IGraphInternals.DestroyAllNodes()
		{
			this.DestroyAllNodes();
		}

		IEnumerable<Progress> IGraphInternals.ScanInternal()
		{
			return this.ScanInternal();
		}

		void IGraphInternals.SerializeExtraInfo(GraphSerializationContext ctx)
		{
			this.SerializeExtraInfo(ctx);
		}

		void IGraphInternals.DeserializeExtraInfo(GraphSerializationContext ctx)
		{
			this.DeserializeExtraInfo(ctx);
		}

		void IGraphInternals.PostDeserialization(GraphSerializationContext ctx)
		{
			this.PostDeserialization(ctx);
		}

		void IGraphInternals.DeserializeSettingsCompatibility(GraphSerializationContext ctx)
		{
			this.DeserializeSettingsCompatibility(ctx);
		}

		public AstarPath active;

		[JsonMember]
		public Guid guid;

		[JsonMember]
		public uint initialPenalty;

		[JsonMember]
		public bool open;

		public uint graphIndex;

		[JsonMember]
		public string name;

		[JsonMember]
		public bool drawGizmos = true;

		[JsonMember]
		public bool infoScreenOpen;

		[JsonMember]
		private string serializedEditorSettings;

		[Obsolete("Use the transform field (only available on some graph types) instead", true)]
		public Matrix4x4 matrix = Matrix4x4.identity;

		[Obsolete("Use the transform field (only available on some graph types) instead", true)]
		public Matrix4x4 inverseMatrix = Matrix4x4.identity;
	}
}
