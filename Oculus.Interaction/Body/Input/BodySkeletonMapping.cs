using System;
using System.Collections.Generic;
using System.Linq;
using Oculus.Interaction.Collections;

namespace Oculus.Interaction.Body.Input
{
	public abstract class BodySkeletonMapping<TSourceJointId> : ISkeletonMapping where TSourceJointId : Enum
	{
		public IEnumerableHashSet<BodyJointId> Joints
		{
			get
			{
				return this._joints;
			}
		}

		public bool TryGetParentJointId(BodyJointId jointId, out BodyJointId parentJointId)
		{
			return this._jointToParent.TryGetValue(jointId, out parentJointId);
		}

		public bool TryGetSourceJointId(BodyJointId jointId, out TSourceJointId sourceJointId)
		{
			return this._reverseMap.TryGetValue(jointId, out sourceJointId);
		}

		public bool TryGetBodyJointId(TSourceJointId jointId, out BodyJointId bodyJointId)
		{
			return this._forwardMap.TryGetValue(jointId, out bodyJointId);
		}

		protected TSourceJointId GetSourceJointFromBodyJoint(BodyJointId jointId)
		{
			return this._reverseMap[jointId];
		}

		protected BodyJointId GetBodyJointFromSourceJoint(TSourceJointId sourceJointId)
		{
			return this._forwardMap[sourceJointId];
		}

		protected BodySkeletonMapping(TSourceJointId root, IReadOnlyDictionary<BodyJointId, BodySkeletonMapping<TSourceJointId>.JointInfo> jointMapping)
		{
			this._tree = new BodySkeletonMapping<TSourceJointId>.SkeletonTree(root, jointMapping);
			this._joints = new EnumerableHashSet<BodyJointId>(from n in this._tree.Nodes
			select n.BodyJointId);
			this._forwardMap = this._tree.Nodes.ToDictionary((BodySkeletonMapping<TSourceJointId>.SkeletonTree.Node n) => n.SourceJointId, (BodySkeletonMapping<TSourceJointId>.SkeletonTree.Node n) => n.BodyJointId);
			this._reverseMap = this._tree.Nodes.ToDictionary((BodySkeletonMapping<TSourceJointId>.SkeletonTree.Node n) => n.BodyJointId, (BodySkeletonMapping<TSourceJointId>.SkeletonTree.Node n) => n.SourceJointId);
			this._jointToParent = this._tree.Nodes.ToDictionary((BodySkeletonMapping<TSourceJointId>.SkeletonTree.Node n) => n.BodyJointId, (BodySkeletonMapping<TSourceJointId>.SkeletonTree.Node n) => n.Parent.BodyJointId);
		}

		private readonly BodySkeletonMapping<TSourceJointId>.SkeletonTree _tree;

		private readonly IEnumerableHashSet<BodyJointId> _joints;

		private readonly IReadOnlyDictionary<TSourceJointId, BodyJointId> _forwardMap;

		private readonly IReadOnlyDictionary<BodyJointId, TSourceJointId> _reverseMap;

		private readonly IReadOnlyDictionary<BodyJointId, BodyJointId> _jointToParent;

		private class SkeletonTree
		{
			public SkeletonTree(TSourceJointId root, IReadOnlyDictionary<BodyJointId, BodySkeletonMapping<TSourceJointId>.JointInfo> mapping)
			{
				Dictionary<TSourceJointId, BodySkeletonMapping<TSourceJointId>.SkeletonTree.Node> dictionary = new Dictionary<TSourceJointId, BodySkeletonMapping<TSourceJointId>.SkeletonTree.Node>();
				foreach (KeyValuePair<BodyJointId, BodySkeletonMapping<TSourceJointId>.JointInfo> keyValuePair in mapping)
				{
					BodyJointId key = keyValuePair.Key;
					BodySkeletonMapping<TSourceJointId>.JointInfo value = keyValuePair.Value;
					dictionary[value.SourceJointId] = new BodySkeletonMapping<TSourceJointId>.SkeletonTree.Node(value.SourceJointId, key);
				}
				foreach (BodySkeletonMapping<TSourceJointId>.JointInfo jointInfo in mapping.Values)
				{
					BodySkeletonMapping<TSourceJointId>.SkeletonTree.Node node = dictionary[jointInfo.SourceJointId];
					node.Parent = dictionary[jointInfo.ParentJointId];
					node.Parent.Children.Add(node);
				}
				this.Nodes = new List<BodySkeletonMapping<TSourceJointId>.SkeletonTree.Node>(dictionary.Values);
				this.Root = dictionary[root];
			}

			public readonly BodySkeletonMapping<TSourceJointId>.SkeletonTree.Node Root;

			public readonly IReadOnlyList<BodySkeletonMapping<TSourceJointId>.SkeletonTree.Node> Nodes;

			public class Node
			{
				public Node(TSourceJointId sourceJointId, BodyJointId bodyJointId)
				{
					this.SourceJointId = sourceJointId;
					this.BodyJointId = bodyJointId;
				}

				public readonly TSourceJointId SourceJointId;

				public readonly BodyJointId BodyJointId;

				public BodySkeletonMapping<TSourceJointId>.SkeletonTree.Node Parent;

				public List<BodySkeletonMapping<TSourceJointId>.SkeletonTree.Node> Children = new List<BodySkeletonMapping<TSourceJointId>.SkeletonTree.Node>();
			}
		}

		protected readonly struct JointInfo
		{
			public JointInfo(TSourceJointId sourceJointId, TSourceJointId parentJointId)
			{
				this.SourceJointId = sourceJointId;
				this.ParentJointId = parentJointId;
			}

			public readonly TSourceJointId SourceJointId;

			public readonly TSourceJointId ParentJointId;
		}
	}
}
