using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Liv.Lck.Cosmetics
{
	public class LckGameObjectSwapCosmetic : LckCosmeticDependantBehaviourBase
	{
		public override string PlayerId
		{
			get
			{
				return this._playerId;
			}
			set
			{
				this._playerId = value;
			}
		}

		public override void Awake()
		{
			base.Awake();
		}

		public override void OnCosmeticReset()
		{
			if (this._instantiatedCosmetic != null)
			{
				Object.Destroy(this._instantiatedCosmetic);
			}
		}

		public override void OnCosmeticLoaded(List<Object> assets)
		{
			if (this._targetGameObject == null)
			{
				LckLog.LogError("LCK: Target GameObject is not assigned on LckGameObjectSwapCosmetic. Cannot apply skin.", "OnCosmeticLoaded", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\Components\\LckGameObjectSwapCosmetic.cs", 47);
				return;
			}
			GameObject gameObject = assets.FirstOrDefault<Object>() as GameObject;
			if (gameObject == null)
			{
				LckLog.LogWarning("LCK: Expected a GameObject from the cosmetic bundle, but found none or it was invalid.", "OnCosmeticLoaded", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\Components\\LckGameObjectSwapCosmetic.cs", 55);
				return;
			}
			if (this._instantiatedCosmetic != null)
			{
				Object.Destroy(this._instantiatedCosmetic);
			}
			this._instantiatedCosmetic = Object.Instantiate<GameObject>(gameObject, this._targetGameObject.transform.parent);
			this._instantiatedCosmetic.transform.localPosition = this._targetGameObject.transform.localPosition;
			this._instantiatedCosmetic.transform.localRotation = this._targetGameObject.transform.localRotation;
			this._instantiatedCosmetic.transform.localScale = this._targetGameObject.transform.localScale;
			this.SetLayerRecursively(this._instantiatedCosmetic, this._targetGameObject.layer);
			this._targetGameObject.SetActive(false);
			Action<GameObject> onCosmeticSpawned = this.OnCosmeticSpawned;
			if (onCosmeticSpawned != null)
			{
				onCosmeticSpawned(this._instantiatedCosmetic);
			}
			LckLog.Log(string.Concat(new string[]
			{
				"LCK: Applied cosmetic skin '",
				gameObject.name,
				"' to replace '",
				this._targetGameObject.name,
				"'."
			}), "OnCosmeticLoaded", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\Components\\LckGameObjectSwapCosmetic.cs", 75);
		}

		private void SetLayerRecursively(GameObject obj, int layer)
		{
			obj.layer = layer;
			Transform[] componentsInChildren = obj.GetComponentsInChildren<Transform>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].gameObject.layer = layer;
			}
		}

		public override void OnDestroy()
		{
			base.OnDestroy();
			if (this._instantiatedCosmetic != null)
			{
				Object.Destroy(this._instantiatedCosmetic);
			}
		}

		[Header("Skin Configuration")]
		[SerializeField]
		[Tooltip("The target GameObject to be replaced by the cosmetic skin.")]
		private GameObject _targetGameObject;

		[FormerlySerializedAs("_overridePlayerId")]
		[SerializeField]
		private string _playerId;

		private GameObject _instantiatedCosmetic;

		public Action<GameObject> OnCosmeticSpawned;
	}
}
