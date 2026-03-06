using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction
{
	public class HoverInteractorsGate : MonoBehaviour
	{
		protected virtual void Awake()
		{
			this.InteractorsA = this._interactorsA.FindAll((Object i) => i != null).ConvertAll<IInteractor>((Object i) => i as IInteractor);
			this.InteractorsB = this._interactorsB.FindAll((Object i) => i != null).ConvertAll<IInteractor>((Object i) => i as IInteractor);
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
				foreach (IInteractor interactor in this.InteractorsA)
				{
					interactor.WhenStateChanged += this.HandleInteractorAStateChanged;
				}
				foreach (IInteractor interactor2 in this.InteractorsB)
				{
					interactor2.WhenStateChanged += this.HandleInteractorBStateChanged;
				}
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				foreach (IInteractor interactor in this.InteractorsA)
				{
					interactor.WhenStateChanged -= this.HandleInteractorAStateChanged;
				}
				foreach (IInteractor interactor2 in this.InteractorsB)
				{
					interactor2.WhenStateChanged -= this.HandleInteractorBStateChanged;
				}
				this._hoveringInteractorsACount = 0;
				this._hoveringInteractorsBCount = 0;
			}
		}

		private void HandleInteractorAStateChanged(InteractorStateChangeArgs stateChange)
		{
			this.ProcessInteractorsStateChange(stateChange, ref this._hoveringInteractorsACount, this.InteractorsB);
		}

		private void HandleInteractorBStateChanged(InteractorStateChangeArgs stateChange)
		{
			this.ProcessInteractorsStateChange(stateChange, ref this._hoveringInteractorsBCount, this.InteractorsA);
		}

		private void ProcessInteractorsStateChange(InteractorStateChangeArgs stateChange, ref int hoveringCounter, List<IInteractor> oppositeInteractors)
		{
			if (stateChange.PreviousState == InteractorState.Normal && stateChange.NewState == InteractorState.Hover)
			{
				int num = hoveringCounter;
				hoveringCounter = num + 1;
				if (num == 0)
				{
					this.EnableAll(oppositeInteractors, false);
				}
			}
			if (stateChange.PreviousState == InteractorState.Hover && stateChange.NewState == InteractorState.Normal)
			{
				int num = hoveringCounter - 1;
				hoveringCounter = num;
				if (num == 0)
				{
					this.EnableAll(oppositeInteractors, true);
				}
			}
		}

		private void EnableAll(List<IInteractor> interactors, bool enable)
		{
			foreach (IInteractor interactor in interactors)
			{
				Behaviour behaviour = interactor as Behaviour;
				if (behaviour != null)
				{
					behaviour.enabled = enable;
				}
			}
		}

		public void InjectAllHoverInteractorsGate(List<IInteractor> interactorsA, List<IInteractor> interactorsB)
		{
			this.InjectInteractorsA(interactorsA);
			this.InjectInteractorsB(interactorsB);
		}

		public void InjectInteractorsA(List<IInteractor> interactors)
		{
			this.InteractorsA = interactors;
			this._interactorsA = interactors.ConvertAll<Object>((IInteractor i) => i as Object);
		}

		public void InjectInteractorsB(List<IInteractor> interactors)
		{
			this.InteractorsB = interactors;
			this._interactorsB = interactors.ConvertAll<Object>((IInteractor i) => i as Object);
		}

		[SerializeField]
		[Interface(typeof(IInteractor), new Type[]
		{

		})]
		private List<Object> _interactorsA;

		private List<IInteractor> InteractorsA;

		[SerializeField]
		[Interface(typeof(IInteractor), new Type[]
		{

		})]
		private List<Object> _interactorsB;

		private List<IInteractor> InteractorsB;

		private int _hoveringInteractorsACount;

		private int _hoveringInteractorsBCount;

		protected bool _started;
	}
}
