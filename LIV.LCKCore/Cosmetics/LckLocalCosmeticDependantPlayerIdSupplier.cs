using System;
using UnityEngine;

namespace Liv.Lck.Cosmetics
{
	public class LckLocalCosmeticDependantPlayerIdSupplier : MonoBehaviour, ILckCosmeticDependantPlayerIdSupplier
	{
		public event PlayerIdUpdatedEvent PlayerIdUpdated;

		private void Start()
		{
			PlayerIdUpdatedEvent playerIdUpdated = this.PlayerIdUpdated;
			if (playerIdUpdated == null)
			{
				return;
			}
			playerIdUpdated();
		}

		public virtual string GetPlayerId()
		{
			return this._playerId;
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
		private string _playerId;
	}
}
