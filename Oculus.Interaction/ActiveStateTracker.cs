using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction
{
	[DefaultExecutionOrder(1)]
	public class ActiveStateTracker : MonoBehaviour
	{
		protected virtual void Awake()
		{
			this.ActiveState = (this._activeState as IActiveState);
		}

		protected virtual void Start()
		{
			if (this._includeChildrenAsDependents)
			{
				for (int i = 0; i < base.transform.childCount; i++)
				{
					this._gameObjects.Add(base.transform.GetChild(i).gameObject);
				}
			}
			this.SetDependentsActive(false);
		}

		protected virtual void Update()
		{
			bool active = this.ActiveState.Active;
			if (this._active == active)
			{
				return;
			}
			this._active = active;
			this.SetDependentsActive(active);
		}

		private void SetDependentsActive(bool active)
		{
			for (int i = 0; i < this._gameObjects.Count; i++)
			{
				this._gameObjects[i].SetActive(active);
			}
			for (int j = 0; j < this._monoBehaviours.Count; j++)
			{
				this._monoBehaviours[j].enabled = active;
			}
		}

		public void InjectAllActiveStateTracker(IActiveState activeState)
		{
			this.InjectActiveState(activeState);
		}

		public void InjectActiveState(IActiveState activeState)
		{
			this._activeState = (activeState as Object);
			this.ActiveState = activeState;
		}

		public void InjectOptionalIncludeChildrenAsDependents(bool includeChildrenAsDependents)
		{
			this._includeChildrenAsDependents = includeChildrenAsDependents;
		}

		public void InjectOptionalGameObjects(List<GameObject> gameObjects)
		{
			this._gameObjects = gameObjects;
		}

		public void InjectOptionalMonoBehaviours(List<MonoBehaviour> monoBehaviours)
		{
			this._monoBehaviours = monoBehaviours;
		}

		[Tooltip("The IActiveState to be tracked.")]
		[SerializeField]
		[Interface(typeof(IActiveState), new Type[]
		{

		})]
		private Object _activeState;

		private IActiveState ActiveState;

		[Header("Active state dependents")]
		[SerializeField]
		[Tooltip("If true, all children of this object will be included as dependents.")]
		private bool _includeChildrenAsDependents;

		[SerializeField]
		[Optional]
		[Tooltip("Sets the `active` field on whole GameObjects.")]
		private List<GameObject> _gameObjects;

		[SerializeField]
		[Optional]
		[Tooltip("Sets the `enabled` field on individual components.")]
		private List<MonoBehaviour> _monoBehaviours;

		private bool _active;
	}
}
