using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XNode;

[CreateAssetMenu(fileName = "TechTreePage", menuName = "SuperInfection/TechTree Page")]
public class TechTreeGadgetGraph : NodeGraph
{
	public GadgetNode[] GadgetNodes
	{
		get
		{
			return (from n in this.nodes
			select n as GadgetNode).ToArray<GadgetNode>();
		}
	}

	public bool IsValid
	{
		get
		{
			EAssetReleaseTier eassetReleaseTier = this.releaseTier;
			if (eassetReleaseTier != EAssetReleaseTier.Disabled && eassetReleaseTier <= EAssetReleaseTier.PublicRC)
			{
				List<Node> nodes = this.nodes;
				return nodes != null && nodes.Count > 0;
			}
			return false;
		}
	}

	public string nickName;

	public SITechTreePageId pageId;

	public Sprite icon;

	public float costMultiplier = 1f;

	public ESuperGameModes excludedGameModes;

	public EAssetReleaseTier releaseTier;

	private const float XLayoutStep = 300f;

	private const float YLayoutStep = 250f;
}
