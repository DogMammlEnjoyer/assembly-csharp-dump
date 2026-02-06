using System;
using GorillaGameModes;
using UnityEngine;

[Serializable]
public class SITechTreeNode
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
			return edReleaseTier != EAssetReleaseTier.Disabled && edReleaseTier <= EAssetReleaseTier.PublicRC && (this.excludedGameModes & (ESuperGameModes)GameMode.CurrentGameModeFlag) == (ESuperGameModes)0;
		}
	}

	public bool IsAllowed
	{
		get
		{
			return (this.excludedGameModes & (ESuperGameModes)GameMode.CurrentGameModeFlag) == (ESuperGameModes)0;
		}
	}

	public bool IsDispensableGadget
	{
		get
		{
			return this.IsValid && this.unlockedGadgetPrefab && this.IsAllowed;
		}
	}

	[SerializeField]
	private EAssetReleaseTier m_edReleaseTier = (EAssetReleaseTier)(-1);

	public SIUpgradeType upgradeType;

	public string nickName;

	public string description;

	public ESuperGameModes excludedGameModes;

	public SIUpgradeType[] parentUpgrades;

	public GameEntity unlockedGadgetPrefab;

	public SIResource.ResourceCost[] nodeCost;

	public bool costOverride;
}
