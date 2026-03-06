using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public class ActiveStateGate : MonoBehaviour, IActiveState
	{
		private ISelector OpenSelector { get; set; }

		private ISelector CloseSelector { get; set; }

		public bool Active { get; private set; }

		protected virtual void Awake()
		{
			this.OpenSelector = (this._openSelector as ISelector);
			this.CloseSelector = (this._closeSelector as ISelector);
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this.EndStart(ref this._started);
		}

		private void OnEnable()
		{
			if (this._started)
			{
				this.OpenSelector.WhenSelected += this.HandleOpenSelected;
				this.CloseSelector.WhenSelected += this.HandleCloseSelected;
			}
		}

		private void OnDisable()
		{
			if (this._started)
			{
				this.Active = false;
				this.OpenSelector.WhenSelected -= this.HandleOpenSelected;
				this.CloseSelector.WhenSelected -= this.HandleCloseSelected;
			}
		}

		private void HandleOpenSelected()
		{
			this.Active = true;
		}

		private void HandleCloseSelected()
		{
			this.Active = false;
		}

		public void InjectAllActiveStateGate(ISelector openSelector, ISelector closeSelector)
		{
			this.InjectOpenState(openSelector);
			this.InjectCloseState(closeSelector);
		}

		public void InjectOpenState(ISelector openSelector)
		{
			this._openSelector = (openSelector as Object);
			this.OpenSelector = openSelector;
		}

		public void InjectCloseState(ISelector closeSelector)
		{
			this._closeSelector = (closeSelector as Object);
			this.CloseSelector = closeSelector;
		}

		[SerializeField]
		[Interface(typeof(ISelector), new Type[]
		{

		})]
		private Object _openSelector;

		[SerializeField]
		[Interface(typeof(ISelector), new Type[]
		{

		})]
		private Object _closeSelector;

		protected bool _started;
	}
}
