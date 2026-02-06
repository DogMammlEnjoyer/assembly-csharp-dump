using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XNode;

public class GadgetNode : TechTreeNodeBase
{
	private static bool InEditor
	{
		get
		{
			return NodeInspectorBridge.InNodeEditor;
		}
	}

	public bool IsValid
	{
		get
		{
			EAssetReleaseTier eassetReleaseTier = this.releaseTier;
			return eassetReleaseTier != EAssetReleaseTier.Disabled && eassetReleaseTier <= EAssetReleaseTier.PublicRC;
		}
	}

	public bool IsDispensableGadget
	{
		get
		{
			return this.unlockedGadgetPrefab;
		}
	}

	private bool ShowGadgetPrefab
	{
		get
		{
			return !GadgetNode.InEditor || this.IsDispensableGadget;
		}
	}

	private bool ShowExcludedGameModes
	{
		get
		{
			return !GadgetNode.InEditor || this.excludedGameModes > (ESuperGameModes)0;
		}
	}

	private bool ShowReleaseTier
	{
		get
		{
			return !GadgetNode.InEditor || this.releaseTier != EAssetReleaseTier.PublicRC;
		}
	}

	public void ConfigureFrom(SITechTreeNode sourceNode)
	{
		this.releaseTier = sourceNode.EdReleaseTier;
		this.upgradeType = sourceNode.upgradeType;
		this.nickName = sourceNode.nickName;
		this.description = sourceNode.description;
		this.unlockedGadgetPrefab = sourceNode.unlockedGadgetPrefab;
		this.excludedGameModes = sourceNode.excludedGameModes;
		this.nodeCost = sourceNode.nodeCost.ToArray<SIResource.ResourceCost>();
		this.costOverride = sourceNode.costOverride;
		base.name = this.nickName;
	}

	public void AssignParentUpgrades(SIUpgradeType[] prerequisites)
	{
		NodePort port = base.GetPort("input");
		port.ClearConnections();
		for (int i = 0; i < prerequisites.Length; i++)
		{
			SIUpgradeType id = prerequisites[i];
			GadgetNode gadgetNode = this.graph.nodes.FirstOrDefault(delegate(Node n)
			{
				GadgetNode gadgetNode2 = n as GadgetNode;
				return gadgetNode2 != null && gadgetNode2.upgradeType == id;
			}) as GadgetNode;
			if (gadgetNode != null)
			{
				NodePort port2 = gadgetNode.GetPort("output");
				port.Connect(port2);
			}
		}
	}

	public List<SIUpgradeType> GetParentUpgradeTypes()
	{
		List<SIUpgradeType> list = new List<SIUpgradeType>();
		foreach (Node node in from n in base.GetPort("input").GetConnections()
		select n.node)
		{
			GadgetNode gadgetNode = node as GadgetNode;
			if (gadgetNode != null)
			{
				list.Add(gadgetNode.upgradeType);
			}
		}
		return list;
	}

	public SITechTreeNode GenerateTechTreeNode()
	{
		return new SITechTreeNode
		{
			upgradeType = this.upgradeType,
			nickName = this.nickName,
			description = this.description,
			unlockedGadgetPrefab = this.unlockedGadgetPrefab,
			nodeCost = this.nodeCost.ToArray<SIResource.ResourceCost>(),
			excludedGameModes = this.excludedGameModes,
			EdReleaseTier = this.releaseTier,
			parentUpgrades = this.GetParentUpgradeTypes().ToArray()
		};
	}

	public int GetDepth()
	{
		int num = 0;
		NodePort inputPort = base.GetInputPort("input");
		IEnumerable<NodePort> enumerable = (inputPort != null) ? inputPort.GetConnections() : null;
		foreach (NodePort nodePort in (enumerable ?? Enumerable.Empty<NodePort>()))
		{
			GadgetNode gadgetNode = nodePort.node as GadgetNode;
			if (gadgetNode != null)
			{
				num = Mathf.Max(num, gadgetNode.GetDepth() + 1);
			}
		}
		return num;
	}

	public int GetTreeDepth()
	{
		int num = this.GetDepth();
		foreach (NodePort nodePort in base.GetOutputPort("output").GetConnections())
		{
			GadgetNode gadgetNode = nodePort.node as GadgetNode;
			if (gadgetNode != null)
			{
				num = Mathf.Max(num, gadgetNode.GetTreeDepth());
			}
		}
		return num;
	}

	public List<GadgetNode> GetTreeNodes()
	{
		List<GadgetNode> list = new List<GadgetNode>();
		this.GetTreeNodes(list);
		return list;
	}

	public void GetTreeNodes(List<GadgetNode> nodes)
	{
		nodes.Add(this);
		foreach (NodePort nodePort in base.GetOutputPort("output").GetConnections())
		{
			GadgetNode gadgetNode = nodePort.node as GadgetNode;
			if (gadgetNode != null)
			{
				gadgetNode.GetTreeNodes(nodes);
			}
		}
	}

	public List<GadgetNode> GetParentNodes()
	{
		List<GadgetNode> list = new List<GadgetNode>();
		foreach (NodePort nodePort in base.GetPort("input").GetConnections())
		{
			GadgetNode gadgetNode = nodePort.node as GadgetNode;
			if (gadgetNode != null)
			{
				list.Add(gadgetNode);
			}
		}
		return list;
	}

	public List<GadgetNode> GetChildNodes()
	{
		List<GadgetNode> list = new List<GadgetNode>();
		foreach (NodePort nodePort in base.GetOutputPort("output").GetConnections())
		{
			GadgetNode gadgetNode = nodePort.node as GadgetNode;
			if (gadgetNode != null)
			{
				list.Add(gadgetNode);
			}
		}
		return list;
	}

	public int GetTreeWidth()
	{
		List<GadgetNode> childNodes = this.GetChildNodes();
		if (childNodes.Count == 0)
		{
			return 1;
		}
		int num = 0;
		foreach (GadgetNode gadgetNode in childNodes)
		{
			num += gadgetNode.GetTreeWidth();
		}
		return num;
	}

	public bool CostEquals(SIResource.ResourceCost[] cost)
	{
		if (cost.Length != this.nodeCost.Length)
		{
			return false;
		}
		for (int i = 0; i < cost.Length; i++)
		{
			if (!cost[i].Equals(this.nodeCost[i]))
			{
				return false;
			}
		}
		return true;
	}

	[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
	public TechTreeNodeBase.Empty input;

	[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
	public TechTreeNodeBase.Empty output;

	public SIUpgradeType upgradeType;

	public string nickName;

	[TextArea]
	public string description;

	public SIResource.ResourceCost[] nodeCost;

	public bool costOverride;

	[Header("Prefab")]
	public GameEntity unlockedGadgetPrefab;

	public ESuperGameModes excludedGameModes;

	public EAssetReleaseTier releaseTier = (EAssetReleaseTier)(-1);
}
