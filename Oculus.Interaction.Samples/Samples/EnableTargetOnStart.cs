using System;
using UnityEngine;

namespace Oculus.Interaction.Samples
{
	public class EnableTargetOnStart : MonoBehaviour
	{
		private void Start()
		{
			if (this._components != null)
			{
				MonoBehaviour[] components = this._components;
				for (int i = 0; i < components.Length; i++)
				{
					components[i].enabled = true;
				}
			}
			if (this._gameObjects != null)
			{
				GameObject[] gameObjects = this._gameObjects;
				for (int i = 0; i < gameObjects.Length; i++)
				{
					gameObjects[i].SetActive(true);
				}
			}
		}

		public MonoBehaviour[] _components;

		public GameObject[] _gameObjects;
	}
}
