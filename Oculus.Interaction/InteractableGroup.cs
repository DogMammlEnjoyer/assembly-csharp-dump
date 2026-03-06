using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Oculus.Interaction
{
	public class InteractableGroup : MonoBehaviour
	{
		public object Data { get; protected set; }

		protected virtual void Awake()
		{
			this.Interactables = this._interactables.ConvertAll<IInteractable>((Object mono) => mono as IInteractable);
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this._limits = new List<InteractableGroup.InteractableLimits>();
			foreach (IInteractable interactable in this.Interactables)
			{
				this._limits.Add(new InteractableGroup.InteractableLimits
				{
					MaxInteractors = interactable.MaxInteractors,
					MaxSelectingInteractors = interactable.MaxSelectingInteractors
				});
			}
			if (this.Data == null)
			{
				this._data = this;
				this.Data = this._data;
			}
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				foreach (IInteractable interactable in this.Interactables)
				{
					interactable.WhenInteractorViewAdded += this.HandleInteractorViewAdded;
					interactable.WhenInteractorViewRemoved += this.HandleInteractorViewRemoved;
					interactable.WhenSelectingInteractorViewAdded += this.HandleSelectingInteractorViewAdded;
					interactable.WhenSelectingInteractorViewRemoved += this.HandleSelectingInteractorViewRemoved;
				}
				this.UpdateInteractorCount();
				this.UpdateSelectingInteractorCount();
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				foreach (IInteractable interactable in this.Interactables)
				{
					interactable.WhenInteractorViewAdded -= this.HandleInteractorViewAdded;
					interactable.WhenInteractorViewRemoved -= this.HandleInteractorViewRemoved;
					interactable.WhenSelectingInteractorViewAdded -= this.HandleSelectingInteractorViewAdded;
					interactable.WhenSelectingInteractorViewRemoved -= this.HandleSelectingInteractorViewRemoved;
				}
				this.UpdateInteractorCount();
				this.UpdateSelectingInteractorCount();
			}
		}

		private void UpdateInteractorCount()
		{
			this._interactors = 0;
			foreach (IInteractable interactable in this.Interactables)
			{
				this._interactors += interactable.InteractorViews.Count<IInteractorView>();
			}
			this.UpdateMaxInteractors();
		}

		private void UpdateSelectingInteractorCount()
		{
			this._selectInteractors = 0;
			foreach (IInteractable interactable in this.Interactables)
			{
				this._selectInteractors += interactable.SelectingInteractorViews.Count<IInteractorView>();
			}
			this.UpdateMaxSelecting();
		}

		private void HandleInteractorViewAdded(IInteractorView interactorView)
		{
			this.UpdateInteractorCount();
		}

		private void HandleInteractorViewRemoved(IInteractorView interactorView)
		{
			this.UpdateInteractorCount();
		}

		private void HandleSelectingInteractorViewAdded(IInteractorView interactorView)
		{
			this.UpdateInteractorCount();
		}

		private void HandleSelectingInteractorViewRemoved(IInteractorView interactorView)
		{
			this.UpdateInteractorCount();
		}

		private void UpdateMaxInteractors()
		{
			if (this._maxInteractors == -1)
			{
				return;
			}
			int num = Mathf.Max(0, this._maxInteractors - this._interactors);
			for (int i = 0; i < this.Interactables.Count; i++)
			{
				this.Interactables[i].MaxInteractors = ((this._limits[i].MaxInteractors == -1) ? num : Mathf.Max(0, this._limits[i].MaxInteractors - this._interactors)) + this.Interactables[i].InteractorViews.Count<IInteractorView>();
			}
		}

		private void UpdateMaxSelecting()
		{
			if (this._maxSelectingInteractors == -1)
			{
				return;
			}
			int num = Mathf.Max(0, this._maxSelectingInteractors - this._selectInteractors);
			for (int i = 0; i < this.Interactables.Count; i++)
			{
				this.Interactables[i].MaxSelectingInteractors = ((this._limits[i].MaxSelectingInteractors == -1) ? num : Mathf.Max(0, this._limits[i].MaxSelectingInteractors - this._selectInteractors)) + this.Interactables[i].SelectingInteractorViews.Count<IInteractorView>();
			}
		}

		public void InjectAllInteractableGroup(List<IInteractable> interactables)
		{
			this.InjectInteractables(interactables);
		}

		public void InjectInteractables(List<IInteractable> interactables)
		{
			this.Interactables = interactables;
			this._interactables = interactables.ConvertAll<Object>((IInteractable interactable) => interactable as Object);
		}

		public void InjectOptionalData(object data)
		{
			this._data = (data as Object);
			this.Data = data;
		}

		[SerializeField]
		[Interface(typeof(IInteractable), new Type[]
		{

		})]
		private List<Object> _interactables;

		private List<IInteractable> Interactables;

		private List<InteractableGroup.InteractableLimits> _limits;

		[SerializeField]
		private int _maxInteractors;

		[SerializeField]
		private int _maxSelectingInteractors;

		private int _interactors;

		private int _selectInteractors;

		[SerializeField]
		[Optional]
		private Object _data;

		protected bool _started;

		private struct InteractableLimits
		{
			public int MaxInteractors;

			public int MaxSelectingInteractors;
		}
	}
}
