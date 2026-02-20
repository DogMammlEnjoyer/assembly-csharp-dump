using System;
using Liv.Lck.Cosmetics;
using UnityEngine;

public class GtLckNetworkCosmeticDependantPlayerIdSupplier : MonoBehaviour, ILckCosmeticDependantPlayerIdSupplier
{
	public event PlayerIdUpdatedEvent PlayerIdUpdated;

	public string GetPlayerId()
	{
		return this.vrrig.OwningNetPlayer.UserId;
	}

	public void UpdatePlayerId()
	{
		PlayerIdUpdatedEvent playerIdUpdated = this.PlayerIdUpdated;
		if (playerIdUpdated == null)
		{
			return;
		}
		playerIdUpdated();
	}

	[SerializeField]
	private VRRig vrrig;
}
