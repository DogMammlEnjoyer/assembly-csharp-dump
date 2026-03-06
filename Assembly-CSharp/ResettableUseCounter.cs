using System;

public struct ResettableUseCounter
{
	public ResettableUseCounter(int maxRegularUses, int maxSuperchargeUses, Action<bool> onReadyChanged = null)
	{
		this.maxRegularUses = maxRegularUses;
		this.maxSuperchargeUses = maxSuperchargeUses;
		this.usesRemaining = maxRegularUses;
		this.onReadyChanged = onReadyChanged;
	}

	public bool IsReady
	{
		get
		{
			return this.usesRemaining > 0;
		}
	}

	public bool TryUse()
	{
		if (!this.IsReady)
		{
			return false;
		}
		SuperInfectionManager activeSuperInfectionManager = SuperInfectionManager.activeSuperInfectionManager;
		bool flag = activeSuperInfectionManager != null && activeSuperInfectionManager.IsSupercharged;
		if (this.usesRemaining > this.maxRegularUses && !flag)
		{
			this.usesRemaining = this.maxRegularUses;
		}
		this.usesRemaining--;
		if (!this.IsReady)
		{
			Action<bool> action = this.onReadyChanged;
			if (action != null)
			{
				action(false);
			}
		}
		return true;
	}

	public void Reset()
	{
		bool isReady = this.IsReady;
		this.usesRemaining = this.maxSuperchargeUses;
		if (!isReady)
		{
			Action<bool> action = this.onReadyChanged;
			if (action == null)
			{
				return;
			}
			action(true);
		}
	}

	private int usesRemaining;

	private int maxRegularUses;

	private int maxSuperchargeUses;

	private Action<bool> onReadyChanged;
}
