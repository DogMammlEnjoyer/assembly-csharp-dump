using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GorillaGameModes;
using UnityEngine;

[Serializable]
public class SITechTreePage
{
	public EAssetReleaseTier EdReleaseTier
	{
		get
		{
			return this.m_edReleaseTier;
		}
		set
		{
			this.m_edReleaseTier = value;
		}
	}

	public bool IsValid
	{
		get
		{
			EAssetReleaseTier edReleaseTier = this.m_edReleaseTier;
			if (edReleaseTier != EAssetReleaseTier.Disabled && edReleaseTier <= EAssetReleaseTier.PublicRC)
			{
				SITechTreeNode[] array = this.treeNodes;
				return array != null && array.Length != 0;
			}
			return false;
		}
	}

	public bool IsAllowed
	{
		get
		{
			return (this.excludedGameModes & (ESuperGameModes)GameMode.CurrentGameModeFlag) == (ESuperGameModes)0;
		}
	}

	public List<GraphNode<SITechTreeNode>> Roots { get; private set; }

	public List<GraphNode<SITechTreeNode>> AllNodes { get; private set; }

	public List<SITechTreeNode> DispensableGadgets { get; private set; }

	public void ClearGraph()
	{
		this.Roots = null;
		this.AllNodes = null;
	}

	public void BuildGraph()
	{
		SITechTreePage.<>c__DisplayClass27_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		this.Roots = new List<GraphNode<SITechTreeNode>>();
		this.AllNodes = new List<GraphNode<SITechTreeNode>>();
		this.DispensableGadgets = new List<SITechTreeNode>();
		if (!this.IsValid)
		{
			return;
		}
		CS$<>8__locals1.nodeLookup = new Dictionary<SIUpgradeType, GraphNode<SITechTreeNode>>();
		foreach (SITechTreeNode sitechTreeNode in this.treeNodes)
		{
			if (sitechTreeNode.IsValid && (sitechTreeNode.parentUpgrades == null || sitechTreeNode.parentUpgrades.Length == 0))
			{
				this.Roots.Add(this.<BuildGraph>g__PopulateGraph|27_0(sitechTreeNode, this.excludedGameModes, ref CS$<>8__locals1));
			}
		}
		foreach (GraphNode<SITechTreeNode> graphNode in this.AllNodes)
		{
			if (graphNode.Value.IsDispensableGadget)
			{
				this.DispensableGadgets.Add(graphNode.Value);
			}
		}
	}

	public void PrintGraph()
	{
		foreach (GraphNode<SITechTreeNode> graphNode in this.Roots)
		{
			foreach (GraphNode<SITechTreeNode> graphNode2 in graphNode.TraversePreOrderDistinct(null))
			{
				Debug.Log(string.Concat(new string[]
				{
					"[SI] Graph node: ",
					graphNode2.Value.nickName,
					" [",
					SITechTreePage.<PrintGraph>g__NodeListText|28_2(graphNode2.Parents),
					"]"
				}));
			}
		}
	}

	[CompilerGenerated]
	private GraphNode<SITechTreeNode> <BuildGraph>g__PopulateGraph|27_0(SITechTreeNode node, ESuperGameModes parentExcludedGameModes, ref SITechTreePage.<>c__DisplayClass27_0 A_3)
	{
		node.excludedGameModes |= parentExcludedGameModes;
		GraphNode<SITechTreeNode> graphNode;
		if (!A_3.nodeLookup.TryGetValue(node.upgradeType, out graphNode))
		{
			graphNode = new GraphNode<SITechTreeNode>(node);
			A_3.nodeLookup.Add(node.upgradeType, graphNode);
			this.AllNodes.Add(graphNode);
		}
		SIUpgradeType upgradeType = node.upgradeType;
		foreach (SITechTreeNode sitechTreeNode in this.treeNodes)
		{
			if (sitechTreeNode.IsValid && sitechTreeNode.parentUpgrades != null)
			{
				SIUpgradeType[] parentUpgrades = sitechTreeNode.parentUpgrades;
				for (int j = 0; j < parentUpgrades.Length; j++)
				{
					if (parentUpgrades[j] == upgradeType)
					{
						GraphNode<SITechTreeNode> graphNode2 = this.<BuildGraph>g__PopulateGraph|27_0(sitechTreeNode, node.excludedGameModes, ref A_3);
						if (!graphNode.Children.Contains(graphNode2))
						{
							graphNode.AddChild(graphNode2);
						}
					}
				}
			}
		}
		return graphNode;
	}

	[CompilerGenerated]
	internal static string[] <PrintGraph>g__GetChildText|28_0(GraphNode<SITechTreeNode> root)
	{
		return (from n in root.TraversePreOrder()
		select SITechTreePage.<PrintGraph>g__NodeText|28_1(n)).ToArray<string>();
	}

	[CompilerGenerated]
	internal static string <PrintGraph>g__NodeText|28_1(GraphNode<SITechTreeNode> graphNode)
	{
		return string.Concat(new string[]
		{
			SITechTreePage.<PrintGraph>g__NodeListText|28_2(graphNode.Parents),
			" >> ",
			graphNode.Value.nickName,
			" << ",
			SITechTreePage.<PrintGraph>g__NodeListText|28_2(graphNode.Children)
		});
	}

	[CompilerGenerated]
	internal static string <PrintGraph>g__NodeListText|28_2(List<GraphNode<SITechTreeNode>> nodes)
	{
		return string.Join("|", from n in nodes
		select n.Value.nickName);
	}

	[SerializeField]
	private EAssetReleaseTier m_edReleaseTier = (EAssetReleaseTier)(-1);

	public string nickName;

	public SITechTreePageId pageId;

	public Sprite icon;

	public ESuperGameModes excludedGameModes;

	[SerializeField]
	private SITechTreeNode[] treeNodes;

	public float costMultiplier = 1f;
}
