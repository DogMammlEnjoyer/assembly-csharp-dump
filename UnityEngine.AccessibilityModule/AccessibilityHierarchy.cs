using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Accessibility
{
	public class AccessibilityHierarchy
	{
		public IReadOnlyList<AccessibilityNode> rootNodes
		{
			get
			{
				return this.m_RootNodes;
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action<AccessibilityHierarchy> m_Changed;

		internal event Action<AccessibilityHierarchy> changed
		{
			[VisibleToOtherModules(new string[]
			{
				"UnityEditor.AccessibilityModule"
			})]
			add
			{
				this.m_Changed += value;
			}
			[VisibleToOtherModules(new string[]
			{
				"UnityEditor.AccessibilityModule"
			})]
			remove
			{
				this.m_Changed -= value;
			}
		}

		public AccessibilityHierarchy()
		{
			this.m_FirstLowestCommonAncestorChain = new Stack<AccessibilityNode>();
			this.m_SecondLowestCommonAncestorChain = new Stack<AccessibilityNode>();
			this.m_Nodes = new Dictionary<int, AccessibilityNode>();
			this.m_RootNodes = new List<AccessibilityNode>();
		}

		internal void NotifyHierarchyChanged()
		{
			Action<AccessibilityHierarchy> changed = this.m_Changed;
			if (changed != null)
			{
				changed(this);
			}
		}

		public void Clear()
		{
			for (int i = this.m_RootNodes.Count - 1; i >= 0; i--)
			{
				this.RemoveNode(this.m_RootNodes[i], true);
			}
		}

		public bool TryGetNode(int id, out AccessibilityNode node)
		{
			return this.m_Nodes.TryGetValue(id, out node);
		}

		public AccessibilityNode AddNode(string label = null, AccessibilityNode parent = null)
		{
			return this.InsertNode(-1, label, parent);
		}

		public AccessibilityNode InsertNode(int childIndex, string label = null, AccessibilityNode parent = null)
		{
			bool flag = parent != null;
			if (flag)
			{
				this.ValidateNodeInHierarchy(parent);
			}
			AccessibilityNode accessibilityNode = this.GenerateNewNode();
			this.m_Nodes[accessibilityNode.id] = accessibilityNode;
			bool flag2 = label != null;
			if (flag2)
			{
				accessibilityNode.label = label;
			}
			AccessibilityNode node = accessibilityNode;
			IList<AccessibilityNode> previousParentChildren = null;
			IList<AccessibilityNode> newParentChildren;
			if (parent != null)
			{
				newParentChildren = parent.childList;
			}
			else
			{
				IList<AccessibilityNode> rootNodes = this.m_RootNodes;
				newParentChildren = rootNodes;
			}
			this.SetParent(node, parent, previousParentChildren, newParentChildren, childIndex);
			this.NotifyHierarchyChanged();
			return accessibilityNode;
		}

		public bool MoveNode(AccessibilityNode node, AccessibilityNode newParent, int newChildIndex = -1)
		{
			this.ValidateNodeInHierarchy(node);
			bool flag = node == newParent;
			if (flag)
			{
				throw new ArgumentException(string.Format("Attempting to move the node {0} under itself.", node));
			}
			bool flag2 = node.parent == newParent;
			bool result;
			if (flag2)
			{
				IList<AccessibilityNode> list;
				if (newParent != null)
				{
					list = newParent.childList;
				}
				else
				{
					IList<AccessibilityNode> rootNodes = this.m_RootNodes;
					list = rootNodes;
				}
				IList<AccessibilityNode> list2 = list;
				bool flag3 = newChildIndex == list2.IndexOf(node);
				if (flag3)
				{
					result = false;
				}
				else
				{
					this.CheckForLoopsAndSetParent(node, newParent, newChildIndex);
					result = true;
				}
			}
			else
			{
				bool flag4 = newParent == null;
				if (flag4)
				{
					bool flag5 = node.parent == null;
					if (flag5)
					{
						result = false;
					}
					else
					{
						this.CheckForLoopsAndSetParent(node, null, newChildIndex);
						result = true;
					}
				}
				else
				{
					this.ValidateNodeInHierarchy(newParent);
					this.CheckForLoopsAndSetParent(node, newParent, newChildIndex);
					this.NotifyHierarchyChanged();
					result = true;
				}
			}
			return result;
		}

		public void RemoveNode(AccessibilityNode node, bool removeChildren = true)
		{
			this.ValidateNodeInHierarchy(node);
			if (removeChildren)
			{
				this.<RemoveNode>g__removeFromNodes|20_0(node);
			}
			else
			{
				this.m_Nodes.Remove(node.id);
			}
			bool flag = this.m_RootNodes.Contains(node);
			if (flag)
			{
				this.m_RootNodes.Remove(node);
				bool flag2 = !removeChildren;
				if (flag2)
				{
					this.m_RootNodes.AddRange(node.childList);
				}
			}
			node.Destroy(removeChildren);
			this.NotifyHierarchyChanged();
		}

		public bool ContainsNode(AccessibilityNode node)
		{
			return node != null && this.m_Nodes.ContainsKey(node.id) && this.m_Nodes[node.id] == node;
		}

		private void CheckForLoopsAndSetParent(AccessibilityNode node, AccessibilityNode parent, int newChildIndex = -1)
		{
			bool flag = parent == null;
			if (flag)
			{
				AccessibilityNode parent2 = null;
				AccessibilityNode parent3 = node.parent;
				this.SetParent(node, parent2, ((parent3 != null) ? parent3.childList : null) ?? this.m_RootNodes, this.m_RootNodes, newChildIndex);
			}
			else
			{
				bool flag2 = node.parent == parent;
				if (flag2)
				{
					this.SetParent(node, parent, parent.childList, parent.childList, newChildIndex);
				}
				else
				{
					bool flag3 = node.parent == null && parent.parent == null;
					if (flag3)
					{
						this.SetParent(node, parent, this.m_RootNodes, parent.childList, newChildIndex);
					}
					else
					{
						for (AccessibilityNode parent4 = parent.parent; parent4 != null; parent4 = parent4.parent)
						{
							bool flag4 = parent4 == node;
							if (flag4)
							{
								throw new ArgumentException(string.Format("Trying to set the node {0} to have parent {1}, but this would create a loop.", node, parent));
							}
						}
						AccessibilityNode parent5 = node.parent;
						this.SetParent(node, parent, ((parent5 != null) ? parent5.childList : null) ?? this.m_RootNodes, parent.childList, newChildIndex);
					}
				}
			}
		}

		private void SetParent(AccessibilityNode node, AccessibilityNode parent, IList<AccessibilityNode> previousParentChildren, IList<AccessibilityNode> newParentChildren, int newChildIndex = -1)
		{
			if (previousParentChildren != null)
			{
				previousParentChildren.Remove(node);
			}
			node.SetParent(parent, newChildIndex);
			bool flag = newChildIndex < 0 || newChildIndex > newParentChildren.Count;
			if (flag)
			{
				newParentChildren.Add(node);
			}
			else
			{
				newParentChildren.Insert(newChildIndex, node);
			}
		}

		internal void AllocateNative()
		{
			foreach (AccessibilityNode accessibilityNode in this.m_RootNodes)
			{
				accessibilityNode.AllocateNative();
			}
		}

		internal void FreeNative()
		{
			foreach (AccessibilityNode accessibilityNode in this.m_RootNodes)
			{
				accessibilityNode.FreeNative(true);
			}
		}

		public void RefreshNodeFrames()
		{
			foreach (AccessibilityNode accessibilityNode in this.m_Nodes.Values)
			{
				accessibilityNode.CalculateFrame();
			}
			AssistiveSupport.OnHierarchyNodeFramesRefreshed(this);
		}

		public bool TryGetNodeAt(float horizontalPosition, float verticalPosition, out AccessibilityNode node)
		{
			Vector2 pos = new Vector2(horizontalPosition, verticalPosition);
			node = AccessibilityHierarchy.<TryGetNodeAt>g__FindNodeContainingPoint|27_0(this.m_RootNodes, pos);
			return node != null;
		}

		public AccessibilityNode GetLowestCommonAncestor(AccessibilityNode firstNode, AccessibilityNode secondNode)
		{
			bool flag = firstNode == null || secondNode == null;
			AccessibilityNode result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = firstNode.parent == null && secondNode.parent == null;
				if (flag2)
				{
					result = null;
				}
				else
				{
					bool flag3 = !this.ContainsNode(firstNode) || !this.ContainsNode(secondNode);
					if (flag3)
					{
						result = null;
					}
					else
					{
						this.m_FirstLowestCommonAncestorChain.Clear();
						this.m_SecondLowestCommonAncestorChain.Clear();
						this.<GetLowestCommonAncestor>g__buildNodeIdStack|28_0(firstNode, ref this.m_FirstLowestCommonAncestorChain);
						this.<GetLowestCommonAncestor>g__buildNodeIdStack|28_0(secondNode, ref this.m_SecondLowestCommonAncestorChain);
						AccessibilityNode accessibilityNode = null;
						for (int i = Mathf.Min(this.m_FirstLowestCommonAncestorChain.Count, this.m_SecondLowestCommonAncestorChain.Count); i > 0; i--)
						{
							AccessibilityNode accessibilityNode2 = this.m_FirstLowestCommonAncestorChain.Pop();
							AccessibilityNode accessibilityNode3 = this.m_SecondLowestCommonAncestorChain.Pop();
							bool flag4 = accessibilityNode2 != accessibilityNode3;
							if (flag4)
							{
								break;
							}
							accessibilityNode = accessibilityNode2;
						}
						result = accessibilityNode;
					}
				}
			}
			return result;
		}

		internal AccessibilityNode GenerateNewNode()
		{
			bool flag = AccessibilityHierarchy.m_NextUniqueNodeId >= int.MaxValue;
			if (flag)
			{
				throw new Exception(string.Format("Could not generate unique node for hierarchy. A hierarchy may only have up to {0} nodes.", int.MaxValue));
			}
			AccessibilityNode accessibilityNode = new AccessibilityNode(AccessibilityHierarchy.m_NextUniqueNodeId, this);
			AccessibilityHierarchy.m_NextUniqueNodeId = accessibilityNode.id + 1;
			return accessibilityNode;
		}

		private void ValidateNodeInHierarchy(AccessibilityNode node)
		{
			bool flag = node != null;
			if (!flag)
			{
				throw new ArgumentNullException("node");
			}
			bool flag2 = this.ContainsNode(node);
			if (flag2)
			{
				return;
			}
			throw new ArgumentException(string.Format("Trying to use an AccessibilityNode with ID {0} that is not part of this hierarchy.", node.id));
		}

		[CompilerGenerated]
		private void <RemoveNode>g__removeFromNodes|20_0(AccessibilityNode child)
		{
			this.m_Nodes.Remove(child.id);
			for (int i = 0; i < child.childList.Count; i++)
			{
				this.<RemoveNode>g__removeFromNodes|20_0(child.childList[i]);
			}
		}

		[CompilerGenerated]
		internal static AccessibilityNode <TryGetNodeAt>g__FindNodeContainingPoint|27_0(IList<AccessibilityNode> nodes, Vector2 pos)
		{
			int i = nodes.Count - 1;
			while (i >= 0)
			{
				AccessibilityNode accessibilityNode = nodes[i];
				AccessibilityNode accessibilityNode2 = AccessibilityHierarchy.<TryGetNodeAt>g__FindNodeContainingPoint|27_0(accessibilityNode.childList, pos);
				bool flag = accessibilityNode2 != null;
				AccessibilityNode result;
				if (flag)
				{
					result = accessibilityNode2;
				}
				else
				{
					bool flag2 = accessibilityNode.isActive && accessibilityNode.frame.Contains(pos);
					if (!flag2)
					{
						i--;
						continue;
					}
					result = accessibilityNode;
				}
				return result;
			}
			return null;
		}

		[CompilerGenerated]
		private void <GetLowestCommonAncestor>g__buildNodeIdStack|28_0(AccessibilityNode node, ref Stack<AccessibilityNode> nodeStack)
		{
			while (node != null)
			{
				nodeStack.Push(node);
				node = this.m_Nodes[node.id].parent;
			}
		}

		internal List<AccessibilityNode> m_RootNodes;

		private Stack<AccessibilityNode> m_FirstLowestCommonAncestorChain;

		private Stack<AccessibilityNode> m_SecondLowestCommonAncestorChain;

		private static int m_NextUniqueNodeId;

		private readonly IDictionary<int, AccessibilityNode> m_Nodes;
	}
}
