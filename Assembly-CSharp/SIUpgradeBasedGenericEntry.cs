using System;
using UnityEngine;

[Serializable]
internal struct SIUpgradeBasedGenericEntry<T>
{
	public bool IsActive(SIUpgradeSet withUpgrades)
	{
		bool flag = true;
		if (this.activeRequirements.Length != 0)
		{
			flag = false;
			foreach (SIUpgradeType upgrade in this.activeRequirements)
			{
				if (withUpgrades.Contains(upgrade))
				{
					flag = true;
					break;
				}
			}
		}
		if (flag)
		{
			foreach (SIUpgradeType upgrade2 in this.inactiveRequirements)
			{
				if (withUpgrades.Contains(upgrade2))
				{
					flag = false;
					break;
				}
			}
		}
		return flag;
	}

	public T value;

	[Tooltip("For the objects to become activated, you must match AT LEAST ONE appearRequirement (if there are any), and not match any disappearRequirements.")]
	public SIUpgradeType[] activeRequirements;

	[Tooltip("For the objects to become deactivated, you must match AT LEAST ONE disappearRequirement (if there are any).")]
	public SIUpgradeType[] inactiveRequirements;
}
