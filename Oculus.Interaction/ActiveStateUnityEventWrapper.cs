using System;
using UnityEngine;
using UnityEngine.Events;

namespace Oculus.Interaction
{
	public class ActiveStateUnityEventWrapper : MonoBehaviour
	{
		public UnityEvent WhenActivated
		{
			get
			{
				return this._whenActivated;
			}
		}

		public UnityEvent WhenDeactivated
		{
			get
			{
				return this._whenDeactivated;
			}
		}

		protected virtual void Awake()
		{
			this.ActiveState = (this._activeState as IActiveState);
		}

		protected virtual void Start()
		{
			this._savedState = false;
		}

		protected virtual void Update()
		{
			if (this._emitOnFirstUpdate && !this._emittedOnFirstUpdate)
			{
				this.InvokeEvent();
				this._emittedOnFirstUpdate = true;
			}
			bool active = this.ActiveState.Active;
			if (this._savedState != active)
			{
				this._savedState = active;
				this.InvokeEvent();
			}
		}

		private void InvokeEvent()
		{
			if (this._savedState)
			{
				this._whenActivated.Invoke();
				return;
			}
			this._whenDeactivated.Invoke();
		}

		public void InjectAllActiveStateUnityEventWrapper(IActiveState activeState)
		{
			this.InjectActiveState(activeState);
		}

		public void InjectActiveState(IActiveState activeState)
		{
			this._activeState = (activeState as Object);
			this.ActiveState = activeState;
		}

		public void InjectOptionalEmitOnFirstUpdate(bool emitOnFirstUpdate)
		{
			this._emitOnFirstUpdate = emitOnFirstUpdate;
		}

		public void InjectOptionalWhenActivated(UnityEvent whenActivated)
		{
			this._whenActivated = whenActivated;
		}

		public void InjectOptionalWhenDeactivated(UnityEvent whenDeactivated)
		{
			this._whenDeactivated = whenDeactivated;
		}

		[Tooltip("Events will fire based on the state of this IActiveState.")]
		[SerializeField]
		[Interface(typeof(IActiveState), new Type[]
		{

		})]
		private Object _activeState;

		private IActiveState ActiveState;

		[Tooltip("This event will be fired when the provided IActiveState becomes active.")]
		[SerializeField]
		private UnityEvent _whenActivated;

		[Tooltip("This event will be fired when the provided IActiveState becomes inactive.")]
		[SerializeField]
		private UnityEvent _whenDeactivated;

		[SerializeField]
		[Tooltip("If true, the corresponding event will be fired at the beginning of Update.")]
		private bool _emitOnFirstUpdate = true;

		private bool _emittedOnFirstUpdate;

		private bool _savedState;
	}
}
