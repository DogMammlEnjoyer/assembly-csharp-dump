using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public class GameObjectActiveState : MonoBehaviour, IActiveState
	{
		public bool SourceActiveSelf
		{
			get
			{
				return this._sourceActiveSelf;
			}
			set
			{
				this._sourceActiveSelf = value;
			}
		}

		protected virtual void Start()
		{
		}

		public bool Active
		{
			get
			{
				if (!this._sourceActiveSelf)
				{
					return this._sourceGameObject.activeInHierarchy;
				}
				return this._sourceGameObject.activeSelf;
			}
		}

		public void InjectAllGameObjectActiveState(GameObject sourceGameObject)
		{
			this.InjectSourceGameObject(sourceGameObject);
		}

		public void InjectSourceGameObject(GameObject sourceGameObject)
		{
			this._sourceGameObject = sourceGameObject;
		}

		[SerializeField]
		private GameObject _sourceGameObject;

		[SerializeField]
		private bool _sourceActiveSelf;
	}
}
