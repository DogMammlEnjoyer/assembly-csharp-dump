using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Oculus.Interaction.DebugTree
{
	public abstract class DebugTreeUI<TLeaf> : MonoBehaviour where TLeaf : class
	{
		protected abstract TLeaf Value { get; }

		protected abstract INodeUI<TLeaf> NodePrefab { get; }

		protected virtual void Start()
		{
			if (this._buildTreeOnStart)
			{
				base.StartCoroutine(this.BuildTree());
			}
		}

		public IEnumerator BuildTree()
		{
			this._tree = this.CreateTree(this.Value);
			Task task = this._tree.RebuildAsync();
			yield return new WaitUntil(() => task.IsCompleted);
			this._nodeToUI.Clear();
			this.ClearContentArea();
			this.SetTitleText();
			this.BuildTreeRecursive(this._contentArea, this._tree.GetRootNode(), true);
			yield break;
		}

		private void BuildTreeRecursive(RectTransform parent, ITreeNode<TLeaf> node, bool isRoot)
		{
			INodeUI<TLeaf> nodeUI = Object.Instantiate(this.NodePrefab as Object, parent) as INodeUI<TLeaf>;
			bool flag = this._nodeToUI.ContainsKey(node);
			nodeUI.Bind(node, isRoot, flag);
			if (!flag)
			{
				this._nodeToUI.Add(node, nodeUI);
				foreach (ITreeNode<TLeaf> node2 in node.Children)
				{
					this.BuildTreeRecursive(nodeUI.ChildArea, node2, false);
				}
			}
		}

		private void ClearContentArea()
		{
			for (int i = 0; i < this._contentArea.childCount; i++)
			{
				Transform child = this._contentArea.GetChild(i);
				INodeUI<TLeaf> nodeUI;
				if (child != null && child.TryGetComponent<INodeUI<TLeaf>>(out nodeUI))
				{
					Object.Destroy(child.gameObject);
				}
			}
		}

		private void SetTitleText()
		{
			if (this._title != null)
			{
				this._title.text = this.TitleForValue(this.Value);
			}
		}

		protected abstract DebugTree<TLeaf> CreateTree(TLeaf value);

		protected abstract string TitleForValue(TLeaf value);

		[Tooltip("Node prefabs will be instantiated inside of this content area.")]
		[SerializeField]
		private RectTransform _contentArea;

		[Tooltip("This title text will display the GameObject name of the IActiveState.")]
		[SerializeField]
		[Optional]
		private TMP_Text _title;

		[Tooltip("If true, the tree UI will be built on Start.")]
		[SerializeField]
		private bool _buildTreeOnStart;

		private DebugTree<TLeaf> _tree;

		private Dictionary<ITreeNode<TLeaf>, INodeUI<TLeaf>> _nodeToUI = new Dictionary<ITreeNode<TLeaf>, INodeUI<TLeaf>>();
	}
}
