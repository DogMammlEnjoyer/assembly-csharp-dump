using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Oculus.Interaction
{
	public class SecondaryInteractorFilter : MonoBehaviour, IGameObjectFilter
	{
		public IInteractable PrimaryInteractable { get; private set; }

		public IInteractable SecondaryInteractable { get; private set; }

		protected virtual void Awake()
		{
			this.PrimaryInteractable = (this._primaryInteractable as IInteractable);
			this.SecondaryInteractable = (this._secondaryInteractable as IInteractable);
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				if (this._selectRequired)
				{
					this.PrimaryInteractable.WhenSelectingInteractorViewAdded += this.HandleInteractorAdded;
					this.PrimaryInteractable.WhenSelectingInteractorViewRemoved += this.HandleInteractorRemoved;
					return;
				}
				this.PrimaryInteractable.WhenInteractorViewAdded += this.HandleInteractorAdded;
				this.PrimaryInteractable.WhenInteractorViewRemoved += this.HandleInteractorRemoved;
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				if (this._selectRequired)
				{
					this.PrimaryInteractable.WhenSelectingInteractorViewAdded -= this.HandleInteractorAdded;
					this.PrimaryInteractable.WhenSelectingInteractorViewRemoved -= this.HandleInteractorRemoved;
					return;
				}
				this.PrimaryInteractable.WhenInteractorViewAdded -= this.HandleInteractorAdded;
				this.PrimaryInteractable.WhenInteractorViewRemoved -= this.HandleInteractorRemoved;
			}
		}

		public bool Filter(GameObject gameObject)
		{
			if (this._primaryToSecondaryMap == null)
			{
				return false;
			}
			SecondaryInteractorConnection secondaryInteractorConnection;
			if (!gameObject.TryGetComponent<SecondaryInteractorConnection>(out secondaryInteractorConnection))
			{
				return false;
			}
			int identifier = secondaryInteractorConnection.PrimaryInteractor.Identifier;
			if (!this._primaryToSecondaryMap.ContainsKey(identifier))
			{
				return false;
			}
			List<int> list = this._primaryToSecondaryMap[identifier];
			if (!list.Contains(secondaryInteractorConnection.SecondaryInteractor.Identifier))
			{
				list.Add(secondaryInteractorConnection.SecondaryInteractor.Identifier);
			}
			return true;
		}

		private void HandleInteractorAdded(IInteractorView interactor)
		{
			if (this._primaryToSecondaryMap == null)
			{
				this._primaryToSecondaryMap = CollectionPool<Dictionary<int, List<int>>, KeyValuePair<int, List<int>>>.Get();
			}
			this._primaryToSecondaryMap.Add(interactor.Identifier, CollectionPool<List<int>, int>.Get());
		}

		private void HandleInteractorRemoved(IInteractorView primaryInteractor)
		{
			foreach (int id in this._primaryToSecondaryMap[primaryInteractor.Identifier])
			{
				this.SecondaryInteractable.RemoveInteractorByIdentifier(id);
			}
			CollectionPool<List<int>, int>.Release(this._primaryToSecondaryMap[primaryInteractor.Identifier]);
			this._primaryToSecondaryMap.Remove(primaryInteractor.Identifier);
			if (this._primaryToSecondaryMap.Count == 0)
			{
				CollectionPool<Dictionary<int, List<int>>, KeyValuePair<int, List<int>>>.Release(this._primaryToSecondaryMap);
				this._primaryToSecondaryMap = null;
			}
		}

		public void InjectAllSecondaryInteractorFilter(IInteractable primaryInteractable, IInteractable secondaryInteractable, bool selectRequired = false)
		{
			this.InjectPrimaryInteractable(primaryInteractable);
			this.InjectSecondaryInteractable(secondaryInteractable);
			this.InjectSelectRequired(selectRequired);
		}

		public void InjectPrimaryInteractable(IInteractable interactableView)
		{
			this.PrimaryInteractable = interactableView;
			this._primaryInteractable = (interactableView as Object);
		}

		public void InjectSecondaryInteractable(IInteractable interactable)
		{
			this.SecondaryInteractable = interactable;
			this._secondaryInteractable = (interactable as Object);
		}

		public void InjectSelectRequired(bool selectRequired)
		{
			this._selectRequired = selectRequired;
		}

		[SerializeField]
		[Interface(typeof(IInteractable), new Type[]
		{

		})]
		private Object _primaryInteractable;

		[SerializeField]
		[Interface(typeof(IInteractable), new Type[]
		{

		})]
		private Object _secondaryInteractable;

		[SerializeField]
		private bool _selectRequired;

		private Dictionary<int, List<int>> _primaryToSecondaryMap;

		protected bool _started;
	}
}
