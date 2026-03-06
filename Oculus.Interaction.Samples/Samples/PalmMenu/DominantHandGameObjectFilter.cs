using System;
using System.Collections.Generic;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.Samples.PalmMenu
{
	public class DominantHandGameObjectFilter : MonoBehaviour, IGameObjectFilter
	{
		private IHand LeftHand { get; set; }

		protected virtual void Start()
		{
			foreach (GameObject item in this._leftHandedGameObjects)
			{
				this._leftHandedGameObjectSet.Add(item);
			}
			foreach (GameObject item2 in this._rightHandedGameObjects)
			{
				this._rightHandedGameObjectSet.Add(item2);
			}
			this.LeftHand = (this._leftHand as IHand);
		}

		public bool Filter(GameObject go)
		{
			if (this.LeftHand.IsDominantHand)
			{
				return this._leftHandedGameObjectSet.Contains(go);
			}
			return this._rightHandedGameObjectSet.Contains(go);
		}

		[SerializeField]
		[Interface(typeof(IHand), new Type[]
		{

		})]
		private Object _leftHand;

		[SerializeField]
		private GameObject[] _leftHandedGameObjects;

		[SerializeField]
		private GameObject[] _rightHandedGameObjects;

		private readonly HashSet<GameObject> _leftHandedGameObjectSet = new HashSet<GameObject>();

		private readonly HashSet<GameObject> _rightHandedGameObjectSet = new HashSet<GameObject>();
	}
}
