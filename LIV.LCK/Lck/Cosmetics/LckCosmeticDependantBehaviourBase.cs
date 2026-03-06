using System;
using System.Collections.Generic;
using Liv.Lck.DependencyInjection;
using UnityEngine;

namespace Liv.Lck.Cosmetics
{
	public abstract class LckCosmeticDependantBehaviourBase : MonoBehaviour, ILckCosmeticDependant
	{
		public abstract string PlayerId { get; set; }

		public string GetCosmeticType()
		{
			if (this._cosmeticType == null)
			{
				LckLog.LogWarning("LCK: CosmeticType is not assigned on this dependant!");
				return string.Empty;
			}
			return this._cosmeticType.TypeValue;
		}

		public abstract void OnCosmeticLoaded(List<Object> assets);

		public virtual void Awake()
		{
			this._lckCosmeticDependantPlayerIdSupplier = this._playerIdSupplier.GetComponent<ILckCosmeticDependantPlayerIdSupplier>();
			if (this._lckCosmeticDependantPlayerIdSupplier == null)
			{
				LckLog.LogError("LCK: LckCosmeticDependantBehaviour has no _lckCosmeticDependantPlayerIdSupplier set. Cosmetic dependants will fail to load for: " + base.name);
				return;
			}
			this._lckCosmeticDependantPlayerIdSupplier.PlayerIdUpdated += delegate()
			{
				this.OnCosmeticReset();
				this.PlayerId = this._lckCosmeticDependantPlayerIdSupplier.GetPlayerId();
				ILckCosmeticsManager cosmeticsManager = this._cosmeticsManager;
				if (cosmeticsManager == null)
				{
					return;
				}
				cosmeticsManager.RegisterDependant(this);
			};
		}

		public abstract void OnCosmeticReset();

		public virtual void OnDestroy()
		{
			ILckCosmeticsManager cosmeticsManager = this._cosmeticsManager;
			if (cosmeticsManager == null)
			{
				return;
			}
			cosmeticsManager.UnregisterDependant(this);
		}

		[InjectLck]
		private ILckCosmeticsManager _cosmeticsManager;

		[SerializeField]
		[Tooltip("Assign the CosmeticType of this asset, provided as a LckCosmeticType SO.")]
		private LckCosmeticType _cosmeticType;

		[Tooltip("The player ID supplier implementing ILckCosmeticDependantPlayerIdSupplier.")]
		[SerializeField]
		private GameObject _playerIdSupplier;

		private ILckCosmeticDependantPlayerIdSupplier _lckCosmeticDependantPlayerIdSupplier;
	}
}
